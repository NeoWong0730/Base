using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_PedigreedDraw : SystemModuleBase<Sys_PedigreedDraw>, ISystemModuleUpdate
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnUpdateTaskAward,//更新任务奖励状态
        }

        public CSVOperationalActivityRuler.Data curCSVActivityRulerData;
        public CSVPedigreedDraw.Data cSVPedigreedDrawData;
        private Dictionary<uint, List<CSVPedigreedDraw.Data>> m_ClentPedigreeData = new Dictionary<uint, List<CSVPedigreedDraw.Data>>();//key: 运营活动id  同一个id对应不同天的活动数据

        private bool b_IsActivity;
        public bool isActivity
        {
            get
            {
                return b_IsActivity;
            }
            set
            {
                if (b_IsActivity != value)
                {
                    b_IsActivity = value;
                }
            }
        }

        public uint drawStopTimeStamp;  //当日奖券领取截止时间
        public uint luckyTimeStamp;     //当日幸运奖展示时间
        public uint finnalTimeStamp;    //当日特等奖展示时间
        private uint m_FiveTimeStamp;

        public bool reqedLuckyTime;
        public bool reachedFinnalTime;

        public uint startTime;
        public uint endTime;

        public RepeatedField<PetLotteryTaskInfo> petLotteryTaskInfos = new RepeatedField<PetLotteryTaskInfo>();
        public RepeatedField<PetLotteryCodeState> petLotteryCodeStates = new RepeatedField<PetLotteryCodeState>();
        public FinalAwardInfo finalAwardInfo;

        public bool ignorePlayAnim = false;

        private bool m_InValidGetTime;

        public bool inValidGetTime
        {
            get { return m_InValidGetTime; }
            set
            {
                if (m_InValidGetTime != value)
                {
                    m_InValidGetTime = value;
                }
            }
        }

        public override void Init()
        {
            ParseClientData();
            ProcessEvents(true);
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        private void ParseClientData()
        {
            for (int i = 0; i < CSVPedigreedDraw.Instance.GetAll().Count; i++)
            {
                CSVPedigreedDraw.Data cSVPedigreedDrawData = CSVPedigreedDraw.Instance.GetByIndex(i);
                if (!m_ClentPedigreeData.TryGetValue(cSVPedigreedDrawData.Activity_ID, out List<CSVPedigreedDraw.Data> datas))
                {
                    datas = new List<CSVPedigreedDraw.Data>();
                    m_ClentPedigreeData.Add(cSVPedigreedDrawData.Activity_ID, datas);
                }
                datas.Add(cSVPedigreedDrawData);
            }

            drawStopTimeStamp = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(2).str_value);
            luckyTimeStamp = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(3).str_value) + 1;
            finnalTimeStamp = uint.Parse(CSVPedigreedParameter.Instance.GetConfData(4).str_value);
            m_FiveTimeStamp = 5 * 3600;
        }

        private void ProcessEvents(bool register)
        {
            if (register)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetLottery.InfoReq, (ushort)CmdPetLottery.InfoNtf, PetLotteryInfoNtf, CmdPetLotteryInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetLottery.GetTaskAwardReq, (ushort)CmdPetLottery.GetTaskAwardRes, PetLotteryGetTaskAwardRes, CmdPetLotteryGetTaskAwardRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPetLottery.FinalAwardNtf, PetLotteryFinalAwardNtf, CmdPetLotteryFinalAwardNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetLottery.CodeStateReq, (ushort)CmdPetLottery.CodeStateRes, CodeStateRes, CmdPetLotteryCodeStateRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPetLottery.TaskUpdateNtf, PetLotteryTaskUpdateNtf, CmdPetLotteryTaskUpdateNtf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetLottery.InfoNtf, PetLotteryInfoNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetLottery.GetTaskAwardRes, PetLotteryGetTaskAwardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetLottery.FinalAwardNtf, PetLotteryFinalAwardNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetLottery.CodeStateRes, CodeStateRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetLottery.TaskUpdateNtf, PetLotteryTaskUpdateNtf);
            }
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, CheckActivityTime, register);
        }

        public override void OnLogin()
        {
            isActivity = false;
            curCSVActivityRulerData = null;
            cSVPedigreedDrawData = null;
        }

        private void CheckActivityTime()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.PedigreedDraw);
            if (activityInfo != null)
            {
                isActivity = true;
                curCSVActivityRulerData = activityInfo.csvData;
                if (m_ClentPedigreeData.TryGetValue(activityInfo.infoId, out List<CSVPedigreedDraw.Data> datas))
                {
                    for (int i = 0; i < datas.Count; i++)
                    {
                        if (datas[i].Date == activityInfo.currDay)
                        {
                            cSVPedigreedDrawData = datas[i];
                            break;
                        }
                    }
                }
                if (cSVPedigreedDrawData != null)
                {
                    PetLotteryInfoReq(cSVPedigreedDrawData.Activity_ID);
                }
                if (curCSVActivityRulerData != null)
                {
                    if (curCSVActivityRulerData.Activity_Type == 3)//开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day - 1) * 86400;
                    }
                    else if (curCSVActivityRulerData.Activity_Type == 2)//限时
                    {
                        startTime = curCSVActivityRulerData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endTime = startTime + (curCSVActivityRulerData.Duration_Day - 1) * 86400;
                    }
                }
                DebugUtil.LogFormat(ELogType.eOperationActivity, "CheckActivityTime : openServerTime:{0}, curday:{1}",
                    TimeManager.GetDateTime(Sys_Role.Instance.openServiceGameTime).ToString(), cSVPedigreedDrawData.Date.ToString());
            }
            else
            {
                isActivity = false;
                curCSVActivityRulerData = null;
                cSVPedigreedDrawData = null;
            }
        }


        //活动日期间上线、跨天会请求 ;客户端 9点请求
        private void PetLotteryInfoReq(uint activityId)
        {
            CmdPetLotteryInfoReq req = new CmdPetLotteryInfoReq();
            req.ActId = activityId;
            NetClient.Instance.SendMessage((ushort)CmdPetLottery.InfoReq, req);
        }

        private void PetLotteryInfoNtf(NetMsg netMsg)
        {
            CmdPetLotteryInfoNtf res = NetMsgUtil.Deserialize<CmdPetLotteryInfoNtf>(CmdPetLotteryInfoNtf.Parser, netMsg);

            DebugUtil.LogFormat(ELogType.eOperationActivity, "InfoNtf: dayChange:{0}  taskInfo:{1}  codeState:{2} openserverTime:{3}", res.IsDayChange,
                res.Tasks == null ? 0 : res.Tasks.Count, res.Codes == null ? 0 : res.Codes.Count, TimeManager.GetDateTime(Sys_Role.Instance.openServiceGameTime).ToString());

            if (res.IsDayChange)
            {
                reqedLuckyTime = false;
                reachedFinnalTime = false;
            }

            petLotteryTaskInfos = res.Tasks;
            petLotteryCodeStates = res.Codes;

            RepeatedField<uint> lotteryCodes = new RepeatedField<uint>();

            if (petLotteryCodeStates.Count > 0)
            {
                for (int i = 0; i < petLotteryCodeStates.Count; i++)
                {
                    lotteryCodes.Add(petLotteryCodeStates[i].LotteryCode);
                }
            }
            CodeStateReq(lotteryCodes);
        }

        private void CodeStateReq(RepeatedField<uint> lotteryCodes)
        {
            if (cSVPedigreedDrawData == null)
            {
                Debug.LogErrorFormat("cSVPedigreedDrawData=null");
                return;
            }
            CmdPetLotteryCodeStateReq req = new CmdPetLotteryCodeStateReq();
            req.ActId = cSVPedigreedDrawData.Activity_ID;
            req.LotteryCodes.AddRange(lotteryCodes);
            NetClient.Instance.SendMessage((ushort)CmdPetLottery.CodeStateReq, req);
        }

        private void CodeStateRes(NetMsg netMsg)
        {
            CmdPetLotteryCodeStateRes ntf = NetMsgUtil.Deserialize<CmdPetLotteryCodeStateRes>(CmdPetLotteryCodeStateRes.Parser, netMsg);

            DebugUtil.LogFormat(ELogType.eOperationActivity, "CodeStateRes: CodeState:{0}  FinalAwardInfo:{1}",
                ntf.CodeState == null ? 0 : ntf.CodeState.Count, ntf.FinalAwardInfo != null ? ntf.FinalAwardInfo.RoleName.ToStringUtf8() : "null");

            for (int i = 0; i < ntf.CodeState.Count; i++)
            {
                PetLotteryCodeState codeState = ntf.CodeState[i];

                for (int j = 0; j < petLotteryCodeStates.Count; j++)
                {
                    if (petLotteryCodeStates[j].LotteryCode == codeState.LotteryCode)
                    {
                        petLotteryCodeStates[j].State = codeState.State;
                    }
                }
            }

            finalAwardInfo = ntf.FinalAwardInfo;
            eventEmitter.Trigger(EEvents.OnUpdateTaskAward);
        }

        /// <summary>
        /// 领取奖券
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="taskId"></param>
        public void PetLotteryGetTaskAwardReq(uint activityId, uint taskId)
        {
            CmdPetLotteryGetTaskAwardReq req = new CmdPetLotteryGetTaskAwardReq();
            req.ActId = activityId;
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort)CmdPetLottery.GetTaskAwardReq, req);
        }

        private void PetLotteryGetTaskAwardRes(NetMsg netMsg)
        {
            CmdPetLotteryGetTaskAwardRes res = NetMsgUtil.Deserialize<CmdPetLotteryGetTaskAwardRes>(CmdPetLotteryGetTaskAwardRes.Parser, netMsg);
            petLotteryCodeStates = res.Codes;

            DebugUtil.LogFormat(ELogType.eOperationActivity, "GetTaskAwardRes: CodeState:{0}", res.Codes == null ? 0 : res.Codes.Count);
            eventEmitter.Trigger(EEvents.OnUpdateTaskAward);
        }

        /// <summary>
        /// 9点10分 摇奖动画推送
        /// </summary>
        /// <param name="netMsg"></param>
        private void PetLotteryFinalAwardNtf(NetMsg netMsg)
        {
            CmdPetLotteryFinalAwardNtf ntf = NetMsgUtil.Deserialize<CmdPetLotteryFinalAwardNtf>(CmdPetLotteryFinalAwardNtf.Parser, netMsg);
            if (ignorePlayAnim)
            {
                return;
            }
            finalAwardInfo = ntf.FinalAwardInfo;

            DebugUtil.LogFormat(ELogType.eOperationActivity, "FinalAwardNtf：{0}", finalAwardInfo != null ? ntf.LotteryCode.ToString() : "null");

            if (UIManager.IsVisibleAndOpen(EUIID.UI_Menu))
            {
                UIManager.OpenUI(EUIID.UI_LuckyPet, false, ntf.LotteryCode);
            }
        }

        private void PetLotteryTaskUpdateNtf(NetMsg netMsg)
        {
            CmdPetLotteryTaskUpdateNtf ntf = NetMsgUtil.Deserialize<CmdPetLotteryTaskUpdateNtf>(CmdPetLotteryTaskUpdateNtf.Parser, netMsg);

            DebugUtil.LogFormat(ELogType.eOperationActivity, "PetLotteryTaskUpdateNtf,{0}", ntf.Infos == null ? 0 : ntf.Infos.TaskId);

            bool find = false;

            for (int i = 0; i < petLotteryTaskInfos.Count; i++)
            {
                if (petLotteryTaskInfos[i].TaskId == ntf.Infos.TaskId)
                {
                    petLotteryTaskInfos[i].Value = ntf.Infos.Value;
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                PetLotteryTaskInfo taskInfo = new PetLotteryTaskInfo();
                taskInfo.TaskId = ntf.Infos.TaskId;
                taskInfo.Value = ntf.Infos.Value;
                petLotteryTaskInfos.Add(taskInfo);
            }

            eventEmitter.Trigger(EEvents.OnUpdateTaskAward);
        }

        public void OnUpdate()
        {
            if (!isActivity || cSVPedigreedDrawData == null)
            {
                return;
            }

            inValidGetTime = InValidGetTime();

            if (!reqedLuckyTime)
            {
                uint curDayTimeStamp = Sys_Time.Instance.GetServerTime() - (uint)Sys_Time.Instance.GetDayZeroTimestamp();
                if (curDayTimeStamp >= luckyTimeStamp)
                {
                    PetLotteryInfoReq(cSVPedigreedDrawData.Activity_ID);
                    DebugUtil.LogFormat(ELogType.eOperationActivity, "reqedLucky:{0}", TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime()).ToString());
                    reqedLuckyTime = true;
                }
            }
            if (!reachedFinnalTime)
            {
                uint curDayTimeStamp = Sys_Time.Instance.GetServerTime() - (uint)Sys_Time.Instance.GetDayZeroTimestamp();
                if (curDayTimeStamp >= finnalTimeStamp)
                {
                    eventEmitter.Trigger(EEvents.OnUpdateTaskAward);
                    DebugUtil.LogFormat(ELogType.eOperationActivity, "reachedFinnalTime: {0}", TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime()).ToString());
                    reachedFinnalTime = true;
                }
            }
        }

        public PetLotteryTaskInfo GetTaskInfo(uint taskId)
        {
            for (int i = 0; i < petLotteryTaskInfos.Count; i++)
            {
                if (taskId == petLotteryTaskInfos[i].TaskId)
                {
                    return petLotteryTaskInfos[i];
                }
            }
            return null;
        }

        public PetLotteryCodeState GetCodeState(uint taskId)
        {
            for (int i = 0; i < petLotteryCodeStates.Count; i++)
            {
                if (taskId == petLotteryCodeStates[i].TaskId)
                {
                    return petLotteryCodeStates[i];
                }
            }
            return null;
        }

        public bool HasRewardUnGet()
        {
            bool flag = false;
            for (int i = 0; i < petLotteryTaskInfos.Count; i++)
            {
                uint taskId = petLotteryTaskInfos[i].TaskId;
                CSVPedigreedTaskGroup.Data cSVPedigreedTaskGroupData = CSVPedigreedTaskGroup.Instance.GetConfData(taskId);
                uint reachValue = cSVPedigreedTaskGroupData.ReachTypeAchievement[cSVPedigreedTaskGroupData.ReachTypeAchievement.Count - 1];
                if (petLotteryTaskInfos[i].Value == reachValue && GetCodeState(taskId) == null)
                {
                    flag = true;
                }
            }
            inValidGetTime = InValidGetTime();
            flag &= inValidGetTime;
            return flag;
        }

        public bool CheckPedigreedIsOpen()
        {
            return isActivity && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(118);
        }

        public bool InValidGetTime()
        {
            uint curTime = Sys_Time.Instance.GetServerTime();
            uint curZeroTime = (uint)Sys_Time.Instance.GetDayZeroTimestamp();

            uint extra = curTime - curZeroTime;

            if (extra < m_FiveTimeStamp || extra > drawStopTimeStamp)
            {
                return false;
            }
            return true;
        }

        public string HandleLotteryCode(uint lotteryCode)
        {
            string value = lotteryCode.ToString();

            if (value.Length < 9)
            {
                int require = 9 - value.Length;
                StringBuilder sb = StringBuilderPool.GetTemporary();
                for (int i = 0; i < require; i++)
                {
                    sb.Append("0");
                }
                sb.Append(value);
                value = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }

            return value;
        }
    }
}

