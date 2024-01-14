using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class Sys_ActivityQuest : SystemModuleBase<Sys_ActivityQuest>, ISystemModuleUpdate
    {
        public CSVOperationalActivityRuler.Data curCSVActivityRulerData;

        public List<QuestData> questDatas = new List<QuestData>();

        private List<uint> m_TodayIds = new List<uint>();

        private Dictionary<uint, List<CSVActivityQuestGroup.Data>> m_ClientDatas = new Dictionary<uint, List<CSVActivityQuestGroup.Data>>();

        public uint startTime;

        public uint endTime;

        private bool b_IsActivity;

        public bool isActivity
        {
            get { return b_IsActivity; }
            set
            {
                if (b_IsActivity != value)
                {
                    b_IsActivity = value;
                    if (!b_IsActivity)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                    }
                }
            }
        }

        private uint m_ActivityId;

        public uint activityId
        {
            get { return m_ActivityId; }
            set
            {
                if (m_ActivityId != value)
                {
                    if (m_ActivityId != 0)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                    }

                    m_ActivityId = value;
                }
            }
        }

        public ACMissionAward aCMissionAward;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            e_RefreshTaskEntry, //更新任务进度
            e_UpdateRedState,
            e_AcrossDay_Qt, //活动任务跨天
        }

        private void ParseClientData()
        {
            for (int i = 0; i < CSVActivityQuestGroup.Instance.GetAll().Count; i++)
            {
                CSVActivityQuestGroup.Data data = CSVActivityQuestGroup.Instance.GetByIndex(i);
                if (!m_ClientDatas.TryGetValue(data.Activity_Id, out List<CSVActivityQuestGroup.Data> datas))
                {
                    datas = new List<CSVActivityQuestGroup.Data>();
                    m_ClientDatas.Add(data.Activity_Id, datas);
                }

                datas.Add(data);
            }
        }


        public override void Init()
        {
            ProcessEvents(true);
            ParseClientData();
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool register)
        {
            if (register)
            {
                EventDispatcher.Instance.AddEventListener((ushort) CmdActivityRuler.CmdActivityMissonAwardTakeReq, (ushort) CmdActivityRuler.CmdActivityMissonAwardTakeNtf, ActivityMissonAwardTakeNtf, CmdActivityMissonAwardTakeNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort) CmdActivityRuler.CmdActivityMissonAwardTake2Req, (ushort) CmdActivityRuler.CmdActivityMissonAwardTake2Ntf, ActivityHeFuMissonAwardTakeNtf, CmdActivityMissonAwardTake2Ntf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort) CmdActivityRuler.CmdActivityMissonAwardTakeNtf, ActivityMissonAwardTakeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort) CmdActivityRuler.CmdActivityMissonAwardTake2Ntf, ActivityHeFuMissonAwardTakeNtf);
            }

            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, CheckActivityTime, register);
        }

        private void CheckActivityTime()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.ActivityQuest);
            if (activityInfo != null)
            {
                isActivity = true;
                curCSVActivityRulerData = activityInfo.csvData;
                activityId = curCSVActivityRulerData.id;

                questDatas.Clear();

                if (m_ClientDatas.TryGetValue(curCSVActivityRulerData.id, out List<CSVActivityQuestGroup.Data> datas))
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (datas[i].QuestType == 1 || datas[i].QuestType == 2)
                        {
                            QuestData questData = new QuestData(datas[i].id, 0, 0);

                            questDatas.Add(questData);
                        }
                    }
                }

                if (curCSVActivityRulerData != null)
                {
                    if (curCSVActivityRulerData.Activity_Type == 3) //开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day) * 86400;
                    }
                    else if (curCSVActivityRulerData.Activity_Type == 2) //限时
                    {
                        startTime = curCSVActivityRulerData.Begining_Date + (uint) TimeManager.TimeZoneOffset;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day) * 86400;
                    }

                    DebugUtil.LogFormat(ELogType.eOperationActivity, "活动任务: activityId: {0},curDay:{1}", curCSVActivityRulerData.id, activityInfo.currDay);
                }
            }
            else
            {
                isActivity = false;
                curCSVActivityRulerData = null;
                activityId = 0;
            }
            //合服
            CheckHeFuActivityTime();
        }


        //领取奖励
        public void ActivityMissonAwardTakeReq(uint id)
        {
            CmdActivityMissonAwardTakeReq req = new CmdActivityMissonAwardTakeReq();
            req.MissonId = id;
            NetClient.Instance.SendMessage((ushort) CmdActivityRuler.CmdActivityMissonAwardTakeReq, req);
        }

        private void ActivityMissonAwardTakeNtf(NetMsg netMsg)
        {
            CmdActivityMissonAwardTakeNtf ntf = NetMsgUtil.Deserialize<CmdActivityMissonAwardTakeNtf>(CmdActivityMissonAwardTakeNtf.Parser, netMsg);

            QuestData questData = TryGetQuestByMissionId(ntf.MissonId);

            if (questData != null)
            {
                questData.state = ntf.State;
                questData.completeNum = ntf.CompleteNums;
                DebugUtil.LogFormat(ELogType.eOperationActivity, "ActivityMissonAwardTakeNtf == > missonId:{0},completeNm:{1}, state:{2}", questData.id, questData.completeNum, questData.state);
            }
            else
            {
                DebugUtil.LogErrorFormat("questId not found{0}", ntf.MissonId);
            }

            eventEmitter.Trigger(EEvents.e_RefreshTaskEntry);
        }

        public void UpdateQuestData(ACMissionAward aCMissionAward)
        {
            if (aCMissionAward == null || !b_IsActivity)
            {
                return;
            }

            HandleTodayData(aCMissionAward);

            for (int i = 0; i < aCMissionAward.MissionIds.Count; i++)
            {
                QuestData questData = TryGetQuestByMissionId(aCMissionAward.MissionIds[i]);

                if (questData != null)
                {
                    questData.completeNum = aCMissionAward.CompleteNums[i];
                    questData.state = aCMissionAward.State[i];
                }
            }

            m_TodayIds.Clear();

            for (int i = 0; i < aCMissionAward.TodayMissons.Count; i++)
            {
                m_TodayIds.Add(aCMissionAward.TodayMissons[i]);
            }

            eventEmitter.Trigger(EEvents.e_RefreshTaskEntry);
        }

        private void HandleTodayData(ACMissionAward aCMissionAward)
        {
            //移除旧的当日数据
            for (int i = 0; i < m_TodayIds.Count; i++)
            {
                QuestData todayQd = TryGetQuestByMissionId(m_TodayIds[i]);

                if (todayQd != null)
                {
                    questDatas.Remove(todayQd);
                }
            }

            //添加新的当日数据
            for (int i = 0; i < aCMissionAward.TodayMissons.Count; i++)
            {
                uint questId = aCMissionAward.TodayMissons[i];

                CSVActivityQuestGroup.Data data = CSVActivityQuestGroup.Instance.GetConfData(questId);

                if (data != null)
                {
                    QuestData questData = new QuestData(aCMissionAward.TodayMissons[i], 0, 0);

                    questDatas.Add(questData);
                }
                else
                {
                    DebugUtil.LogErrorFormat("quest id not found{0}", questId);
                }
            }

            Sort();
        }

        public QuestData TryGetQuestByMissionId(uint missionId)
        {
            for (int i = 0; i < questDatas.Count; i++)
            {
                if (questDatas[i].id == missionId)
                {
                    return questDatas[i];
                }
            }

            return null;
        }


        public void Sort()
        {
            questDatas.Sort((x, y) => x.order.CompareTo(y.order));
        }

        private int QuestSort(QuestData q1, QuestData q2)
        {
            if (q1.csv.Order > q2.csv.Order)
            {
                return 1;
            }
            else if (q1.csv.Order < q2.csv.Order)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private int _Sort(QuestData q1, QuestData q2)
        {
            uint qt1 = q1.csv.QuestType;
            uint qt2 = q2.csv.QuestType;

            if (qt1 == qt2)
                return 0;
            else
            {
                if ((qt1 == 3 && qt2 == 1) || qt1 == 3 && qt2 == 2)
                    return -1;
                else if ((qt1 == 2 && qt2 == 1) || (qt1 == 2 && qt2 == 3))
                    return 1;
                else if ((qt1 == 1))
                {
                    if (qt2 == 3)
                        return 1;
                    if (qt2 == 2)
                        return -1;
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public bool hasRed()
        {
            for (int i = 0; i < Sys_ActivityQuest.Instance.questDatas.Count; i++)
            {
                if (Sys_ActivityQuest.Instance.questDatas[i].canGet)
                {
                    return true;
                }
            }

            return false;
        }

        public void OnUpdate()
        {
            if (isActivity)
            {
                if (Sys_Time.Instance.GetServerTime() > endTime)
                {
                    isActivity = false;
                }
            }
            if (isHeFuActivity)
            {
                if (Sys_Time.Instance.GetServerTime() > endHeFuTime)
                {
                    isHeFuActivity = false;
                }
            }
        }

        public class QuestData
        {
            public uint id;

            public uint completeNum;

            private uint m_State;

            public uint state // // 状态 0 没完成 1 已经完成还没领取 2 已经领取  > 2 表示可以完成多次还没领得次数+3
            {
                get { return m_State; }
                set
                {
                    if (m_State != value)
                    {
                        m_State = value;
                        UpdateRedState();
                    }
                }
            }

            public CSVActivityQuestGroup.Data csv;

            public int order;

            private bool b_CanGet;

            public bool canGet
            {
                get { return b_CanGet; }
                set
                {
                    if (b_CanGet != value)
                    {
                        b_CanGet = value;
                        Sys_ActivityQuest.Instance.eventEmitter.Trigger(EEvents.e_UpdateRedState);
                    }
                }
            }

            public void UpdateRedState()
            {
                if (state == 0)
                {
                    canGet = false;
                }
                else if (state == 1)
                {
                    canGet = true;
                }
                else if (state == 2)
                {
                    canGet = false;
                }
                else
                {
                    uint remain = state - 3;

                    canGet = remain > 0;
                }
            }

            public QuestData(uint id, uint completeNum, uint state)
            {
                this.id = id;
                this.completeNum = completeNum;
                this.state = state;

                csv = CSVActivityQuestGroup.Instance.GetConfData(id);

                this.order = (int) csv.Order;
            }
        }
    }
}