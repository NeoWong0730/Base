using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;

namespace Logic
{
    public class Sys_TerrorSeries : SystemModuleBase<Sys_TerrorSeries>
    {
        #region 数据段

        private Dictionary<uint, TerrorInsData> dictInsData = new Dictionary<uint, TerrorInsData>();

        public ulong VoteId { get; set; }
        private Dictionary<ulong, int> voteState = new Dictionary<ulong, int>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
           OnSelectLine,
           OnUpdateTaskData,
           OnNtfWeekTaskMemInfo,
           OnNtfWeekTaskVote,
           OnUpdateVoteNtf,
        }

        #endregion

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTerrorSeries.DailyTaskUpdateNtf, OnDailyTaskUpdateNtf, CmdTerrorSeriesDailyTaskUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTerrorSeries.QueryMemItemReq, (ushort)CmdTerrorSeries.QueryMemItemRes, OnWeekTasRes, CmdTerrorSeriesQueryMemItemRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTerrorSeries.DailyTaskCompleteNtf, OnTaskCompleteNtf, CmdTerrorSeriesDailyTaskCompleteNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTerrorSeries.UpdateInsAwardTimesNtf, OnUpdateTimesNtf, CmdTerrorSeriesUpdateInsAwardTimesNtf.Parser);

            Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.TerrorSeries, OnNotify_Vote);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnTaskReceived, true);
        }

        public override void OnLogin()
        {
            base.OnLogin();
        }

        public override void OnLogout()
        {
            base.OnLogout();
        }

        private void OnDailyTaskUpdateNtf(NetMsg msg)
        {
            CmdTerrorSeriesDailyTaskUpdateNtf ntf = NetMsgUtil.Deserialize<CmdTerrorSeriesDailyTaskUpdateNtf>(CmdTerrorSeriesDailyTaskUpdateNtf.Parser, msg);

            foreach (var data in dictInsData)
            {
                if (data.Value.PlayType == ntf.PlayType)
                {
                    data.Value.TerrorDailyTask = ntf.DailyTask;
                }

                if (ntf.Unlock != null)
                {
                    foreach (var entry in data.Value.Entries)
                    {
                        if (entry.InstanceId == ntf.Unlock.InstanceId)
                        {
                            entry.InsUnlock = ntf.Unlock.Unlock_;
                            if (!entry.Achieves.Contains(ntf.Unlock.AchieveIdx))
                            {
                                entry.Achieves.Add(ntf.Unlock.AchieveIdx);
                            }
                            break;
                        }
                    }
                }
            }

            eventEmitter.Trigger(EEvents.OnUpdateTaskData);
        }

        public void OnDailyTaskAccept(uint instanceId, uint line)
        {
            CmdTerrorSeriesAcceptDailyTaskReq req = new CmdTerrorSeriesAcceptDailyTaskReq();
            req.InstanceId = instanceId;
            req.Line = line;

            NetClient.Instance.SendMessage((ushort)CmdTerrorSeries.AcceptDailyTaskReq, req);
        }

        public void OnDailyTaskGiveUpReq(uint instanceId)
        {
            CmdTerrorSeriesGiveUpDailyTaskReq req = new CmdTerrorSeriesGiveUpDailyTaskReq();
            req.InstanceId = instanceId;

            NetClient.Instance.SendMessage((ushort)CmdTerrorSeries.GiveUpDailyTaskReq, req);
        }

        public void OnWeekTaskReq(uint instanceId)
        {
            CmdTerrorSeriesQueryMemItemReq req = new CmdTerrorSeriesQueryMemItemReq();
            req.InstanceId = instanceId;

            NetClient.Instance.SendMessage((ushort)CmdTerrorSeries.QueryMemItemReq, req);
        }

        private void OnWeekTasRes(NetMsg msg)
        {
            CmdTerrorSeriesQueryMemItemRes res = NetMsgUtil.Deserialize<CmdTerrorSeriesQueryMemItemRes>(CmdTerrorSeriesQueryMemItemRes.Parser, msg);

            //mTerrorInsData.TerrorDailyTask = ntf.DailyTask;

            eventEmitter.Trigger(EEvents.OnNtfWeekTaskMemInfo, res.Data);
        }

        private void OnTaskCompleteNtf(NetMsg msg)
        {
            CmdTerrorSeriesDailyTaskCompleteNtf ntf = NetMsgUtil.Deserialize<CmdTerrorSeriesDailyTaskCompleteNtf>(CmdTerrorSeriesDailyTaskCompleteNtf.Parser, msg);

            List<Table.ItemIdCount> items = new List<Table.ItemIdCount>();
            foreach (var item in ntf.Items)
            {
                Table.ItemIdCount itemData = new Table.ItemIdCount();
                itemData.id = item.InfoId;
                itemData.count = item.Count;
                items.Add(itemData);
            }

            if (items.Count > 0)
            {
                Logic.Core.UIScheduler.Push(EUIID.UI_RewardsShow, items, null, false, Logic.Core.UIScheduler.popTypes[Logic.Core.EUIPopType.WhenMaininterfaceRealOpenning]);
            }

            //魔力宝典
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event41);
        }

        private void OnTaskReceived(int taskCategory, uint taskId, TaskEntry taskEntry)
        {
            if (taskId == 6600001)
            {
                Sys_Task.Instance.TryDoTask(taskId, true, false, true);
            }
        }

        public void OnUpdateInsData(uint playType, TerrorInsData data)
        {
            TerrorInsData tempData;
            if (dictInsData.TryGetValue(playType, out tempData))
            {
                dictInsData[playType] = data;
            }
            else
            {
                dictInsData.Add(playType, data);
            }
        }

        #region 投票通知

        private void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {
            if (eVote == Sys_Vote.EVote.Start)
                OnStartVoteNtf(message as CmdRoleStartVoteNtf);

            if (eVote == Sys_Vote.EVote.DoVote)
                OnDoVoteNtf(message as CmdRoleDoVoteNtf);

            if (eVote == Sys_Vote.EVote.End)
                OnVoteEndNtf(message as CmdRoleVoteEndNtf);

            if (eVote == Sys_Vote.EVote.Update)
                OnVoteUpdatNtf(message as CmdRoleVoteUpdateNtf);
        }

        private void OnStartVoteNtf(CmdRoleStartVoteNtf ntf)
        {
            //TerrorSeriesMemItems res = NetMsgUtil.Deserialize<TerrorSeriesMemItems>(TerrorSeriesMemItems.Parser, ntf.ClientData.ToByteArray());
            NetMsgUtil.TryDeserialize<TerrorSeriesMemItems>(TerrorSeriesMemItems.Parser, ntf.ClientData.ToByteArray(), out TerrorSeriesMemItems res);
            VoteId = ntf.VoteId;

            UIManager.CloseUI(EUIID.UI_Dialogue);
            UIManager.CloseUI(EUIID.UI_TerroristWeek);
            UIManager.OpenUI(EUIID.UI_TerroristEnter, false, res);
        }

        private void OnDoVoteNtf(CmdRoleDoVoteNtf ntf)
        {
            if (voteState.ContainsKey(ntf.RoleId))
            {
                voteState[ntf.RoleId] = ntf.Op;
            }
            else
            {
                voteState.Add(ntf.RoleId, ntf.Op);
            }

            eventEmitter.Trigger(EEvents.OnNtfWeekTaskVote, ntf.RoleId);
        }

        private void OnVoteUpdatNtf(CmdRoleVoteUpdateNtf ntf)
        {
            //NetMsgUtil.TryDeserialize<BioInsVoteData>(BioInsVoteData.Parser, ntf.ClientData.ToByteArray(), out BioInsVoteData data);
            if (ntf.VoteId == VoteId)
            {
                NetMsgUtil.TryDeserialize<TerrorSeriesMemItems>(TerrorSeriesMemItems.Parser, ntf.ClientData.ToByteArray(), out TerrorSeriesMemItems res);
                eventEmitter.Trigger(EEvents.OnUpdateVoteNtf, res);
            }
        }

        private void OnVoteEndNtf(CmdRoleVoteEndNtf ntf)
        {
            //CmdRoleVoteEndNtf ntf = NetMsgUtil.Deserialize<CmdRoleVoteEndNtf>(CmdRoleVoteEndNtf.Parser, msg);
            voteState.Clear();

            if (ntf.ResultType == 2) //投票未通过
            {
                string strNames = "";

                switch ((VoteFailReason)ntf.FailReason)
                {

                    case VoteFailReason.Disagree:

                        for (int i = 0; i < ntf.DisagreeIds.Count; ++i)
                        {
                            if (ntf.DisagreeIds[i] == Sys_Role.Instance.RoleId)
                            {
                                strNames += Sys_Role.Instance.sRoleName + " ";
                            }
                            else
                            {
                                TeamMem roleInfo = Sys_Team.Instance.getTeamMem(ntf.DisagreeIds[i]);
                                if (roleInfo != null)
                                {
                                    strNames += roleInfo.Name.ToStringUtf8() + " ";
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(strNames))
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006057, strNames));

                        break;
                    case VoteFailReason.ManualCancel:
                        
                        if (ntf.CancelVoterId == Sys_Role.Instance.RoleId)
                        {
                            strNames += Sys_Role.Instance.sRoleName;
                        }
                        else
                        {
                            TeamMem roleInfo = Sys_Team.Instance.getTeamMem(ntf.CancelVoterId);
                            if (roleInfo != null)
                            {
                                strNames += roleInfo.Name.ToStringUtf8();
                            }
                        }

                        if (!string.IsNullOrEmpty(strNames))
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1006057, strNames));

                        break;
                    case VoteFailReason.SystemCancel:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009655));
                        break;
                    default:
                        break;
                }
            }


            UIManager.CloseUI(EUIID.UI_TerroristEnter);
        }

        private void OnUpdateTimesNtf(NetMsg msg)
        {
            CmdTerrorSeriesUpdateInsAwardTimesNtf ntf = NetMsgUtil.Deserialize<CmdTerrorSeriesUpdateInsAwardTimesNtf>(CmdTerrorSeriesUpdateInsAwardTimesNtf.Parser, msg);
            foreach (var data in dictInsData)
            {
                foreach (var entry in data.Value.Entries)
                {
                    if(entry.InstanceId == ntf.InstanceId)
                    {
                        entry.AwardStageTimes = ntf.AwardStageTimes;
                        entry.LastAwardStageTime = ntf.LastAwardStageTime;
                        break;
                    }
                }
            }
        }

        public void OnDoVoteReq(bool agree)
        {
            Sys_Vote.Instance.Send_DoVoteReq(VoteId, (uint)(agree ? 1 : 2));
        }

        public void OnDoVoteCancel()
        {
            Sys_Vote.Instance.Send_CancleVoteReq(VoteId);
        }

        public VoterOpType GetVoteOp(ulong roleId)
        {
            if (voteState.ContainsKey(roleId))
                return (VoterOpType)voteState[roleId];
            else
                return VoterOpType.None;
        }

        /// <summary>
        /// 获得周常次数
        /// </summary>
        /// <returns></returns>
        public uint GetWeekTime(uint instanceId)
        {
            foreach (var data in dictInsData)
            {
                foreach (var entry in data.Value.Entries)
                {
                    if(entry.InstanceId == instanceId)
                    {
                        return entry.AwardStageTimes;
                    }
                }
            }
            
            return 0u;
        }
        #endregion

        #region 日常
        /// <summary>
        /// 当前副本是否有选择线路
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public bool IsDailyTaskLineSelected(uint instanceId)
        {
            bool isTrue = false;
            foreach (var data in dictInsData)
            {
                if (data.Value.TerrorDailyTask != null)
                {
                    if (data.Value.TerrorDailyTask.InstanceId == instanceId)
                    {
                        isTrue = true;
                        break;
                    }
                }
            }

            return isTrue;
        }

        /// <summary>
        /// 日常的任务数据
        /// </summary>
        /// <returns></returns>
        public TerrorDailyTask GetDailyTaskData(uint playType)
        {
            TerrorInsData data;
            if (dictInsData.TryGetValue(playType, out data))
            {
                return data.TerrorDailyTask;
            }
            return null;
        }

        /// <summary>
        /// 当前线路是否在进行
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IsDailyTaskLineOnGoing(uint instanceId, uint line)
        {
            if (IsDailyTaskLineSelected(instanceId))
            {
                foreach (var data in dictInsData)
                {
                    if (data.Value.TerrorDailyTask != null)
                    {
                        if (data.Value.TerrorDailyTask.InstanceId == instanceId)
                        {
                            return data.Value.TerrorDailyTask.Line == line;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 当前关卡是否在进行中
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <param name="stateIndex"></param>
        /// <returns></returns>
        public bool IsDailyTaskStageOnGoing(uint instanceId, uint line, uint stateIndex)
        {
            if (IsDailyTaskLineSelected(instanceId))
            {
                foreach (var data in dictInsData)
                {
                    if (data.Value.TerrorDailyTask != null)
                    {
                        if (data.Value.TerrorDailyTask.InstanceId == instanceId && data.Value.TerrorDailyTask.Line == line)
                        {
                            return data.Value.TerrorDailyTask.Index == stateIndex;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 当前关卡是否完成
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="stateIndex"></param>
        /// <returns></returns>
        public bool IsDailyTaskStageComplete(uint instanceId, uint stateIndex)
        {
            foreach (var data in dictInsData)
            {
                foreach (var entry in data.Value.Entries)
                {
                    if (entry.InstanceId == instanceId)
                    {
                        if (entry.Achieves.Contains(stateIndex))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断日常副本是否完成
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public bool IsDailyTaskComplete(uint instanceId)
        {
            foreach (var data in dictInsData)
            {
                foreach (var entry in data.Value.Entries)
                {
                    if (entry.InstanceId == instanceId)
                    {
                        return entry.Achieves.Count == 5;
                    }
                }
            }
            
            return false;
        }

        #endregion
    }
}
