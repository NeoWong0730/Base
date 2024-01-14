using Framework;
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
    /// <summary> 活动(玩法)系统 </summary>
    public partial class Sys_Activity : SystemModuleBase<Sys_Activity>, ISystemModuleUpdate, IDailyTimes
    {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            OnUpdateAreaProtectionEvent, //区域防范事件更新
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #endregion
        #region 系统函数
        public override void Init()
        {
            //按照120帧跑120/120 1000ms
            SetIntervalFrame(120);
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
            SetData_AreaProtection();
        }
        public override void OnLogout()
        {
            ClearData_AreaProtection();
            ClearRingTaskData();
        }
        public override void OnSyncFinished()
        {
        }
        public void OnUpdate()
        {
            Update_AreaProtection();
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterExitFightEffect, OnExitEffect, true);
                /// <summary> 区域防范事件 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.RingTaskNtf, OnRingTaskNtf, CmdTaskRingTaskNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.RingTaskUpdateNtf, OnRingTaskUpdateNtf, CmdTaskRingTaskUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.RingTaskChangeNtf, OnRingTaskChangeNtf, CmdTaskRingTaskChangeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.RingTaskFinishNtf, OnRingTaskFinishNtf, CmdTaskRingTaskFinishNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.DoBattleFailNtf, OnDoBattleFailNtf, CmdTaskDoBattleFailNtf.Parser);
            }
            else
            {
                ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnAfterExitFightEffect, OnExitEffect, false);
                /// <summary> 区域防范事件 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTask.RingTaskNtf, OnRingTaskNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTask.RingTaskUpdateNtf, OnRingTaskUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTask.RingTaskChangeNtf, OnRingTaskChangeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTask.RingTaskFinishNtf, OnRingTaskFinishNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTask.DoBattleFailNtf, OnDoBattleFailNtf);
            }
        }
        #endregion
        #region 功能
        /// <summary>
        /// 获得活动次数
        /// </summary>
        /// <param name="dailyID"></param>
        /// <returns></returns>
        public int GetDailyTimes(uint dailyID)
        {
            int times = -1;

            switch (dailyID)
            {
                case 50:
                    {
                        if (null != ringTaskData && null != ringTaskData.rewardLimit)
                        {
                            CheckAreaProtectionExpireTime();
                            times = (int)ringTaskData.rewardLimit.UsedTimes;
                        }
                        else
                        {
                            times = 0;
                        }
                    }
                    break;
            }
            return times;
        }
        #endregion
    }

    /// <summary> 活动系统-区域防范事件 </summary>
    public partial class Sys_Activity : SystemModuleBase<Sys_Activity>
    {
        #region 数据定义
        /// <summary> 区域防范事件 </summary>
        public class AreaProtectionEvent
        {
            /// <summary> 事件ID </summary>
            public uint eventId;
            /// <summary> 事件配置 </summary>
            public CSVAreaProtection.Data cSVAreaProtectionData;
            /// <summary> 限定时间  x开始秒数,y结束秒数 </summary>
            public List<Vector2> limitTime = new List<Vector2>();
            /// <summary> 下次开放时间剩余秒数 </summary>
            public int nextOpenTime = int.MaxValue;
            /// <summary>
            /// 是否显示(开启时间内显示)
            /// </summary>
            private bool _isShow;
            public bool isShow
            {
                get { return _isShow; }
                set
                {
                    if (_isShow != value)
                    {
                        _isShow = value;
                        Sys_Activity.Instance.eventEmitter.Trigger<AreaProtectionEvent>(EEvents.OnUpdateAreaProtectionEvent, this);
                    }
                }
            }
            /// <summary>
            /// 是否开启功能(满足等级)
            /// </summary>
            /// <returns></returns>
            public bool isOpen
            {
                get { return cSVAreaProtectionData.eventLv <= Sys_Team.Instance.GetTeamMemMinLv(); }
            }
            /// <summary>
            /// 构建函数
            /// </summary>
            /// <param name="eventId"></param>
            public AreaProtectionEvent(uint eventId)
            {
                CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventId);

                if (null == cSVAreaProtectionData || null == cSVAreaProtectionData.openTime || 0 == cSVAreaProtectionData.openTime.Count)
                    return;

                this.eventId = cSVAreaProtectionData.id;
                this.cSVAreaProtectionData = cSVAreaProtectionData;

                foreach (var item in cSVAreaProtectionData.openTime)
                {
                    limitTime.Add(new Vector2(item[0] * 60, item[1] * 60 + 59));
                }
            }
            /// <summary>
            /// 对比是否在时间内
            /// </summary>
            /// <param name="serverTime"></param>
            /// <returns></returns>
            public void UpdateOpenState(int totalSeconds)
            {
                this.nextOpenTime = int.MaxValue;
                int nextTime = 0;
                bool isShow = false;
                foreach (var time in limitTime)
                {
                    int start = (int)time.x;
                    int end = (int)time.y;
                    //下次执行时间
                    if (start < totalSeconds)
                        nextTime = 3600 - (totalSeconds - start);
                    else
                        nextTime = start - totalSeconds;
                    //取最小值
                    if (this.nextOpenTime > nextTime)
                        this.nextOpenTime = nextTime;
                    //判断是否打开状态
                    if (start <= totalSeconds && totalSeconds <= end)
                    {
                        isShow = true;
                        break;
                    }
                }
                this.isShow = isShow;
            }
        }
        /// <summary> 地域防范数据 </summary>
        public class RingTaskData
        {
            public uint ringId;
            public ResLimit rewardLimit;
        }
        /// <summary> 地域防范数据 </summary>
        public RingTaskData ringTaskData = new RingTaskData();
        /// <summary> 地域防范任务完成数据延迟处理 </summary>
        public CmdTaskRingTaskFinishNtf cmdTaskRingTaskFinishNtf;
        /// <summary> 事件时间列表 </summary>
        public List<AreaProtectionEvent> list_EventTimes = new List<AreaProtectionEvent>();
        /// <summary> 下次事件出现时间(秒) </summary>
        public int nextEventTime
        {
            get
            {
                int time = int.MaxValue;
                foreach (var item in list_EventTimes)
                {
                    if (time > item.nextOpenTime)
                        time = item.nextOpenTime;
                }
                return time;
            }
        }
        #endregion
        #region 数据处理
        /// <summary>
        /// 设置数据
        /// </summary>
        private void SetData_AreaProtection()
        {
            var dict = CSVAreaProtection.Instance.GetAll();
            foreach (var item in dict)
            {
                list_EventTimes.Add(new AreaProtectionEvent(item.id));
            }
        }
        /// <summary>
        /// 清理数据
        /// </summary>
        private void ClearData_AreaProtection()
        {
            list_EventTimes.Clear();
        }
        /// <summary>
        /// 更新
        /// </summary>
        private void Update_AreaProtection()
        {
            uint time = Sys_Time.Instance.GetServerTime();
            int totalSeconds = (int)(time % 3600);
            for (int i = 0; i < list_EventTimes.Count; i++)
            {
                var item = list_EventTimes[i];
                item.UpdateOpenState(totalSeconds);
            }
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 接取地域防范事件请求
        /// </summary>
        public void AcceptRingTaskReq()
        {
            CmdTaskAcceptRingTaskReq req = new CmdTaskAcceptRingTaskReq();
            NetClient.Instance.SendMessage((ushort)CmdTask.AcceptRingTaskReq, req);
        }
        /// <summary>
        /// 放弃地域防范事件请求
        /// </summary>
        public void GiveUpRingTaskReq()
        {
            CmdTaskGiveUpRingTaskReq req = new CmdTaskGiveUpRingTaskReq();
            NetClient.Instance.SendMessage((ushort)CmdTask.GiveUpRingTaskReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 地域防范事件登录通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnRingTaskNtf(NetMsg msg)
        {
            CmdTaskRingTaskNtf res = NetMsgUtil.Deserialize<CmdTaskRingTaskNtf>(CmdTaskRingTaskNtf.Parser, msg);
            ringTaskData.ringId = res.RingId;
            ringTaskData.rewardLimit = res.RewardLimit;
        }
        /// <summary>
        /// 地域防范任务数据更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnRingTaskUpdateNtf(NetMsg msg)
        {
            CmdTaskRingTaskUpdateNtf res = NetMsgUtil.Deserialize<CmdTaskRingTaskUpdateNtf>(CmdTaskRingTaskUpdateNtf.Parser, msg);
            ringTaskData.rewardLimit = res.RewardLimit;
        }
        /// <summary>
        /// 事件接取or放弃
        /// </summary>
        /// <param name="msg"></param>
        private void OnRingTaskChangeNtf(NetMsg msg)
        {
            CmdTaskRingTaskChangeNtf res = NetMsgUtil.Deserialize<CmdTaskRingTaskChangeNtf>(CmdTaskRingTaskChangeNtf.Parser, msg);
            ringTaskData.ringId = res.RingId;
            if (Sys_Team.Instance.isCaptain())
            {
                if (UIManager.IsOpen(EUIID.UI_AreaProtection))
                {
                    UIManager.CloseUI(EUIID.UI_AreaProtection);
                }
                if (res.RingId != 0)
                {
                    SetDialogue(res.RingId);
                }
            }
            else if (Sys_Team.Instance.isTeamMem(Sys_Role.Instance.RoleId))
            {
                if (res.RingId != 0)
                {
                    UIManager.OpenUI(EUIID.UI_AreaProtectionTips, false, new UI_AreaProtectionTips.EventData(res.RingId, true));
                }
            }
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event32);
        }
        /// <summary>
        /// 广播队伍地域防范事件完成
        /// </summary>
        /// <param name="msg"></param>
        private void OnRingTaskFinishNtf(NetMsg msg)
        {
            CmdTaskRingTaskFinishNtf res = NetMsgUtil.Deserialize<CmdTaskRingTaskFinishNtf>(CmdTaskRingTaskFinishNtf.Parser, msg);
            ringTaskData.ringId = 0;
            cmdTaskRingTaskFinishNtf = res;
            if (!Sys_Fight.Instance.IsFight())
            {
                OnExitEffect();
            }
            Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.FinishDF, 0);
        }
        /// <summary>
        /// 通知队长执行任务失败(包括组队副本，地域防范任务等)
        /// </summary>
        /// <param name="msg"></param>
        private void OnDoBattleFailNtf(NetMsg msg)
        {
            CmdTaskDoBattleFailNtf res = NetMsgUtil.Deserialize<CmdTaskDoBattleFailNtf>(CmdTaskDoBattleFailNtf.Parser, msg);
            if (Sys_Team.Instance.isCaptain())
            {
                if (null != res.LvlErr)//人员等级不足
                {
                    OnDealFailTipLvErr(res);
                }
                else if (null != res.NumErr) //人数不足
                {
                    OnDealFailTipNumErr(res);
                }
                else //功能未解锁
                {
                    OnDealFailTipFuncOpenErr(res);
                }
            }
        }
        /// <summary>
        /// 人员等级不足
        /// </summary>
        /// <param name="ntf"></param>
        private void OnDealFailTipLvErr(CmdTaskDoBattleFailNtf ntf)
        {
            switch (ntf.TaskCategory)
            {
                case 9://地域防范
                    {
                        StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                        for (int i = 0, count = ntf.LvlErr.Mems.Count; i < count; i++)
                        {
                            var name = ntf.LvlErr.Mems[i];

                            if (i == 0)
                            {
                                stringBuilder.Append(name.ToStringUtf8());
                            }
                            else
                            {
                                stringBuilder.Append("、");
                                stringBuilder.Append(name.ToStringUtf8());
                            }
                        }
                        CSVAreaProtectionParameters.Data cSVAreaProtectionParametersData = CSVAreaProtectionParameters.Instance.GetConfData(1);
                        float restartCountdown = (float)cSVAreaProtectionParametersData.restartCountdown;

                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(3830000006, stringBuilder.ToString());
                        PromptBoxParameter.Instance.SetCountdown(restartCountdown, PromptBoxParameter.ECountdown.Confirm);
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            MoveToNpcAndTaskReq(50);
                        }, 3830000008);
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        StringBuilderPool.ReleaseTemporary(stringBuilder);
                    }
                    break;
                default:
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(103904));
                    break;
            }
        }
        /// <summary>
        /// //人数不足
        /// </summary>
        /// <param name="ntf"></param>
        private void OnDealFailTipNumErr(CmdTaskDoBattleFailNtf ntf)
        {
            switch (ntf.TaskCategory)
            {
                case 9://地域防范
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3830000009));

                        if (Sys_Team.Instance.IsFastOpen(true))
                            Sys_Team.Instance.OpenFastUI((uint)60);
                    }
                    break;
                case 14://恐怖旅团
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006070));
                        UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
                    }
                    break;
                default:
                    {
                        UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
                    }
                    break;
            }
        }
        /// <summary>
        /// 功能未解锁
        /// </summary>
        /// <param name="ntf"></param>
        private void OnDealFailTipFuncOpenErr(CmdTaskDoBattleFailNtf ntf)
        {
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            for (int i = 0; i < ntf.FuncOpenErr.Mems.Count; i++)
            {
                var mems = ntf.FuncOpenErr.Mems[i];
                if (i != 0) stringBuilder.Append("、");
                stringBuilder.Append(mems.ToStringUtf8());
            }
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3830000010, stringBuilder.ToString()));
            StringBuilderPool.ReleaseTemporary(stringBuilder);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 清理数据
        /// </summary>
        public void ClearRingTaskData()
        {
            ringTaskData.ringId = 0;
            ringTaskData.rewardLimit = new ResLimit();
            cmdTaskRingTaskFinishNtf = null;
        }
        /// <summary>
        /// 设置对话
        /// </summary>
        /// <param name="EventID"></param>
        public void SetDialogue(uint EventID)
        {
            CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(EventID);
            if (null == cSVAreaProtectionData)
                return;

            uint DialogueID = cSVAreaProtectionData.dialogue_id;
            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(DialogueID);
            if (null == cSVDialogueData)
                return;

            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);

            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {
                UIManager.CloseUI(EUIID.UI_Dialogue);
                UIManager.OpenUI(EUIID.UI_AreaProtectionTips, false, new UI_AreaProtectionTips.EventData(EventID, true, () =>
                {
                    if (cSVAreaProtectionData.task_id_array.Count > 0)
                    {
                        uint taskId = cSVAreaProtectionData.task_id_array[0];
                        Sys_Task.Instance.TryDoTask(taskId, true, false, true);
                    }
                }));
            }, cSVDialogueData);
            Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
        }
        /// <summary>
        /// 移动向npc请求任务
        /// </summary>
        /// <param name="EventID"></param>
        public void MoveToNpcAndTaskReq(uint EventID)
        {
            var data = CSVDailyActivity.Instance.GetConfData(EventID);
            ActionCtrl.Instance.MoveToTargetNPC(data.Npcid,
                () =>
                {
                    if (ringTaskData.ringId != 0)
                        GiveUpRingTaskReq();
                    AcceptRingTaskReq();
                });
        }
        /// <summary>
        /// 退出战斗
        /// </summary>
        public void OnExitEffect()
        {
            if (null == cmdTaskRingTaskFinishNtf) return;

            Action action = () =>
            {
                if (Sys_Team.Instance.isCaptain())
                {

                    CSVAreaProtectionParameters.Data cSVAreaProtectionParametersData = CSVAreaProtectionParameters.Instance.GetConfData(1);
                    bool haveTimes = GetDailyTimes(50u) < cSVAreaProtectionParametersData.rewardTimes;
                    uint lanID = haveTimes ? 3830000005 : 3830000011;
                    int countdown = (int)cSVAreaProtectionParametersData.countdown;

                    MsgBoxParam param = new MsgBoxParam();
                    param.strContent = LanguageHelper.GetTextContent(lanID);
                    param.isToggle = true;
                    param.toggleState = Sys_Task.Instance.GetSkipState(50 * 100000u + 0);
                    param.strToggleTip = LanguageHelper.GetTextContent(3830000013);
                    param.actionToggle = (isOn) =>
                    {
                        Sys_Task.Instance.OnChageSkipReq(50 * 100000u + 0, isOn);
                    };
                    param.actionBtn = (isOk)=>
                    {
                        Sys_ActivityShortcut.Instance.isHaveEventTask = isOk;
                        if (isOk)
                            MoveToNpcAndTaskReq(50);
                    };
                    param.countTime = haveTimes ? countdown : 0;

                    UIManager.OpenUI(EUIID.UI_MessageBox_Tip, false, param);
                }
            };

            UIManager.OpenUI(EUIID.UI_AreaProtectionTips, false, new UI_AreaProtectionTips.EventData(cmdTaskRingTaskFinishNtf.RingId, false, action));
            cmdTaskRingTaskFinishNtf = null;
        }
        /// <summary>
        /// 得到区域防范事件
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public AreaProtectionEvent GetAreaProtectionEvent(uint eventId)
        {
            return list_EventTimes.Find(x => x.eventId == eventId);
        }
        /// <summary>
        /// 检测地域防范过期
        /// </summary>
        public void CheckAreaProtectionExpireTime()
        {
            if(Sys_Activity.Instance.ringTaskData.rewardLimit.ExpireTime < Sys_Time.Instance.GetServerTime())
            {
                Sys_Activity.Instance.ringTaskData.rewardLimit.UsedTimes = 0;
            }
        }
        #endregion
    }
}

