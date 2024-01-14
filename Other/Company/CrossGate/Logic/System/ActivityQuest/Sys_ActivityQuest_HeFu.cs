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
    //合服相关逻辑写在这边
    public partial class Sys_ActivityQuest  : SystemModuleBase<Sys_ActivityQuest>, ISystemModuleUpdate
    {
        public CSVOperationalActivityRuler.Data curCSVActivityRulerDataHeFu;

        private List<uint> m_TodayHeFuIds = new List<uint>();

        public List<QuestData> questHeFuDatas = new List<QuestData>();

        public uint startHeFuTime;

        public uint endHeFuTime;

        private bool b_IsHeFuActivity;

        public bool isHeFuActivity
        {
            get { return b_IsHeFuActivity; }
            set
            {
                if (b_IsHeFuActivity != value)
                {
                    b_IsHeFuActivity = value;
                    if (!b_IsHeFuActivity)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange_HeFu);
                    }
                }
            }
        }

        private uint m_ActivityHeFuId;

        public uint activityHeFuId
        {
            get { return m_ActivityHeFuId; }
            set
            {
                if (m_ActivityHeFuId != value)
                {
                    if (m_ActivityHeFuId != 0)
                    {
                        UIManager.CloseUI(EUIID.UI_Activity_Exchange_HeFu);
                    }
                    m_ActivityHeFuId = value;
                }
            }
        }

        #region 服务器消息
        //领取奖励
        public void ActivityHeFuMissonAwardTakeReq(uint id)
        {
            CmdActivityMissonAwardTake2Req req = new CmdActivityMissonAwardTake2Req();
            req.MissonId = id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityMissonAwardTake2Req, req);
        }

        private void ActivityHeFuMissonAwardTakeNtf(NetMsg netMsg)
        {
            CmdActivityMissonAwardTake2Ntf ntf = NetMsgUtil.Deserialize<CmdActivityMissonAwardTake2Ntf>(CmdActivityMissonAwardTake2Ntf.Parser, netMsg);

            QuestData questData = TryGetHeFuQuestByMissionId(ntf.MissonId);

            if (questData != null)
            {
                questData.state = ntf.State;
                questData.completeNum = ntf.CompleteNums;
                DebugUtil.LogFormat(ELogType.eOperationActivity, "ActivityHeFuMissonAwardTakeNtf == > missonId:{0},completeNm:{1}, state:{2}", questData.id, questData.completeNum, questData.state);
            }
            else
            {
                DebugUtil.LogErrorFormat("HeFu questId not found{0}", ntf.MissonId);
            }

            eventEmitter.Trigger(EEvents.e_RefreshTaskEntry);
        }
        #endregion

        private void CheckHeFuActivityTime()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.HeFuActivityQuest);
            if (activityInfo != null)
            {
                isHeFuActivity = true;
                curCSVActivityRulerDataHeFu = activityInfo.csvData;
                activityHeFuId = curCSVActivityRulerDataHeFu.id;

                questHeFuDatas.Clear();

                if (m_ClientDatas.TryGetValue(curCSVActivityRulerDataHeFu.id, out List<CSVActivityQuestGroup.Data> datas))
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (datas[i].QuestType == 1 || datas[i].QuestType == 2)
                        {
                            QuestData questData = new QuestData(datas[i].id, 0, 0);

                            questHeFuDatas.Add(questData);
                        }
                    }
                }

                if (curCSVActivityRulerDataHeFu != null)
                {
                    if (curCSVActivityRulerDataHeFu.Activity_Type == 3) //开服
                    {
                        startHeFuTime = Sys_Role.Instance.openServiceGameTime;
                        endHeFuTime = startHeFuTime + (curCSVActivityRulerDataHeFu.Duration_Day) * 86400;
                    }
                    else if (curCSVActivityRulerDataHeFu.Activity_Type == 2) //限时
                    {
                        startHeFuTime = curCSVActivityRulerDataHeFu.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endHeFuTime = startHeFuTime + (curCSVActivityRulerDataHeFu.Duration_Day) * 86400;
                    }

                    DebugUtil.LogFormat(ELogType.eOperationActivity, "合服活动任务: activityId: {0},curDay:{1}", curCSVActivityRulerDataHeFu.id, activityInfo.currDay);
                }
            }
            else
            {
                isHeFuActivity = false;
                curCSVActivityRulerDataHeFu = null;
                activityHeFuId = 0;
            }
        }

        public QuestData TryGetHeFuQuestByMissionId(uint missionId)
        {
            for (int i = 0; i < questHeFuDatas.Count; i++)
            {
                if (questHeFuDatas[i].id == missionId)
                {
                    return questHeFuDatas[i];
                }
            }

            return null;
        }

        public void UpdateQuestHeFuData(ACMissionAward aCMissionAward)
        {
            if (aCMissionAward == null || !b_IsHeFuActivity)
            {
                return;
            }

            HandleTodayHeFuData(aCMissionAward);

            for (int i = 0; i < aCMissionAward.MissionIds.Count; i++)
            {
                QuestData questData = TryGetHeFuQuestByMissionId(aCMissionAward.MissionIds[i]);

                if (questData != null)
                {
                    questData.completeNum = aCMissionAward.CompleteNums[i];
                    questData.state = aCMissionAward.State[i];
                }
            }

            m_TodayHeFuIds.Clear();

            for (int i = 0; i < aCMissionAward.TodayMissons.Count; i++)
            {
                m_TodayHeFuIds.Add(aCMissionAward.TodayMissons[i]);
            }

            eventEmitter.Trigger(EEvents.e_RefreshTaskEntry);
        }

        private void HandleTodayHeFuData(ACMissionAward aCMissionAward)
        {
            //移除旧的当日数据
            for (int i = 0; i < m_TodayHeFuIds.Count; i++)
            {
                QuestData todayQd = TryGetHeFuQuestByMissionId(m_TodayHeFuIds[i]);

                if (todayQd != null)
                {
                    questHeFuDatas.Remove(todayQd);
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

                    questHeFuDatas.Add(questData);
                }
                else
                {
                    DebugUtil.LogErrorFormat("HeFu quest id not found{0}", questId);
                }
            }
            HeFuSort();
        }
        public void HeFuSort()
        {
            questHeFuDatas.Sort((x, y) => x.order.CompareTo(y.order));
        }

        public bool hasHeFuRed()
        {
            for (int i = 0; i < questHeFuDatas.Count; i++)
            {
                if (questHeFuDatas[i].canGet)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
