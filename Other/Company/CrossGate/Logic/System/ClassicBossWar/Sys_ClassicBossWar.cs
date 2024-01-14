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
    public class Sys_ClassicBossWar : SystemModuleBase<Sys_ClassicBossWar>
    {
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            OnUpdateClassicBossData,     //更新经典boss数据
            PlayerConfirmNtf,            //经典boss战准备投票通知
            StartVote,                   //开始投票
            DoVote,                      //执行投票
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 经典boss战数据 </summary>
        public CmdClassicBossDataNtf cmdClassicBossDataNtf = new CmdClassicBossDataNtf();
        /// <summary> 投票ID </summary>
        public ulong VoteID { get; set; }
        /// <summary> 投票状态 </summary>
        private Dictionary<ulong, int> voteState = new Dictionary<ulong, int>();
        /// <summary> 战斗结果 </summary>
        public CmdClassicBossResultNtf cmdClassicBossResultNtf;
        /// <summary> 战斗结束 </summary>
        private bool IsBattleOver = false;
        /// <summary> 当前投票数据 </summary>
        public ClassicBossCliVoteData curBossCliVoteData;

        /// <summary> 挑战的bossId </summary>
        public uint ChallengeBossID = 0u;
        #endregion
        #region 系统函数
        public override void Init()
        {
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public override void OnLogin()
        {
        }
        public override void OnLogout()
        {
        }
        public override void OnSyncFinished()
        {
        }
        public void OnUpdate()
        {
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
                /// <summary> 经典boss战 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdClassicBoss.DataNtf, OnClassicBossDataNtf, CmdClassicBossDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdClassicBoss.ResultNtf, OnClassicBossResultNtf, CmdClassicBossResultNtf.Parser);
                Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.ClassicBoss, OnNotify_Vote);
                Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnBattleOver, OnBattleOver, true);
            }
            else
            {
                /// <summary> 经典boss战 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdClassicBoss.DataNtf, OnClassicBossDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdClassicBoss.ResultNtf, OnClassicBossResultNtf);
                Sys_Vote.Instance.RemoveVoteLisitener((ushort)VoteType.ClassicBoss, OnNotify_Vote);
                Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnBattleOver, OnBattleOver, false);
            }
        }
        #endregion
        #region 数据处理
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// npc前发起投票准备进战斗
        /// </summary>
        /// <param name="classicBossId"></param>
        /// <param name="uNpcId"></param>
        public void ClassicBossStartVoteReq(uint classicBossId, ulong uNpcId)
        {
            CmdClassicBossStartVoteReq req = new CmdClassicBossStartVoteReq();
            req.ClassicBossId = classicBossId;
            req.UNpcId = uNpcId;
            NetClient.Instance.SendMessage((ushort)CmdClassicBoss.StartVoteReq, req);
        }
        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        /// <param name="reuslt">投票结果 1 同意 ，2 反对</param>
        public void OnDoVoteReq(bool agree)
        {
            Sys_Vote.Instance.Send_DoVoteReq(VoteID, (uint)(agree ? 1 : 2));
        }
        /// <summary>
        /// 取消投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        public void OnDoVoteCancel()
        {
            Sys_Vote.Instance.Send_CancleVoteReq(VoteID);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 上线数据通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnClassicBossDataNtf(NetMsg msg)
        {
            CmdClassicBossDataNtf ntf = NetMsgUtil.Deserialize<CmdClassicBossDataNtf>(CmdClassicBossDataNtf.Parser, msg);
            cmdClassicBossDataNtf = ntf;
            ChallengeBossID = ntf.Data.LastBossId;
            Sys_ClassicBossWar.Instance.eventEmitter.Trigger(EEvents.OnUpdateClassicBossData);
        }
        /// <summary>
        /// 战斗结束通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnClassicBossResultNtf(NetMsg msg)
        {
            CmdClassicBossResultNtf ntf = NetMsgUtil.Deserialize<CmdClassicBossResultNtf>(CmdClassicBossResultNtf.Parser, msg);
            cmdClassicBossDataNtf.Data.RewardLimit = ntf.RewardLimit;
            if (ntf.FirstExplore)
            {
                if (!cmdClassicBossDataNtf.Data.ExploredBossIds.Contains(ntf.ClassicBossId))
                {
                    cmdClassicBossDataNtf.Data.ExploredBossIds.Add(ntf.ClassicBossId);
                }
            }
            cmdClassicBossResultNtf = ntf;
            ChallengeBossID = ntf.ClassicBossId;
            OpenClassicBossWarResultView();
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event27);
            Sys_ActivityShortcut.Instance.eventEmitter.Trigger(Sys_ActivityShortcut.EEvents.OnRefreshActivity);
        }
        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="value"></param>
        private void OnBattleOver(bool value)
        {
            IsBattleOver = value;
            OpenClassicBossWarResultView();
        }
        /// <summary>
        /// 投票通知
        /// </summary>
        /// <param name="eVote"></param>
        /// <param name="message"></param>
        void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {
            if (eVote == Sys_Vote.EVote.Start)
                OnStartVoteNtf(message as CmdRoleStartVoteNtf);

            if (eVote == Sys_Vote.EVote.DoVote)
                OnDoVoteNtf(message as CmdRoleDoVoteNtf);

            if (eVote == Sys_Vote.EVote.End)
                OnVoteEndNtf(message as CmdRoleVoteEndNtf);

            if (eVote == Sys_Vote.EVote.Update)
                OnUpdateVoteNtf(message as CmdRoleVoteUpdateNtf);
        }
        /// <summary>
        /// 发起boss战准备投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnStartVoteNtf(CmdRoleStartVoteNtf info)
        {
            NetMsgUtil.TryDeserialize<ClassicBossCliVoteData>(ClassicBossCliVoteData.Parser, info.ClientData.ToByteArray(), out ClassicBossCliVoteData data);
            VoteID = info.VoteId;
            curBossCliVoteData = data;
            UIManager.CloseUI(EUIID.UI_ClassicBossWarEnterConfirm);
            UIManager.OpenUI(EUIID.UI_ClassicBossWarEnterConfirm);
            eventEmitter.Trigger(EEvents.StartVote);
        }
        /// <summary>
        /// 队员投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnDoVoteNtf(CmdRoleDoVoteNtf info)
        {
            if (voteState.ContainsKey(info.RoleId))
            {
                voteState[info.RoleId] = info.Op;
            }
            else
            {
                voteState.Add(info.RoleId, info.Op);
            }
            eventEmitter.Trigger<ulong>(EEvents.DoVote, info.RoleId);
        }
        /// <summary>
        /// 投票信息更新， 一般是新加了投票人
        /// </summary>
        /// <param name="info"></param>
        private void OnUpdateVoteNtf(CmdRoleVoteUpdateNtf info)
        {
            NetMsgUtil.TryDeserialize(ClassicBossCliVoteData.Parser, info.ClientData.ToByteArray(), out ClassicBossCliVoteData data);
            VoteID = info.VoteId;
            curBossCliVoteData = data;
            eventEmitter.Trigger(EEvents.DoVote);
        }
        /// <summary>
        /// 队员投票结果
        /// </summary>
        /// <param name="msg"></param>
        void OnVoteEndNtf(CmdRoleVoteEndNtf info)
        {
            if (info.ResultType == 2)
            {
                StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                switch ((VoteFailReason)info.FailReason)
                {

                    case VoteFailReason.Disagree:
                        {
                            for (int i = 0; i < info.DisagreeIds.Count; ++i)
                            {
                                if (info.DisagreeIds[i] == Sys_Role.Instance.RoleId)
                                {
                                    stringBuilder.Append(Sys_Role.Instance.sRoleName + " ");
                                }
                                else
                                {
                                    TeamMem roleInfo = Sys_Team.Instance.getTeamMem(info.DisagreeIds[i]);
                                    if (roleInfo != null)
                                    {
                                        stringBuilder.Append(roleInfo.Name.ToStringUtf8() + " ");
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105003, stringBuilder.ToString()));
                        }
                        break;
                    case VoteFailReason.ManualCancel:
                        {
                            if (info.CancelVoterId == Sys_Role.Instance.RoleId)
                            {
                                stringBuilder.Append(Sys_Role.Instance.sRoleName);
                            }
                            else
                            {
                                TeamMem roleInfo = Sys_Team.Instance.getTeamMem(info.CancelVoterId);
                                if (roleInfo != null)
                                {
                                    stringBuilder.Append(roleInfo.Name.ToStringUtf8());
                                }
                            }

                            if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105003, stringBuilder.ToString()));
                        }
                        break;
                    case VoteFailReason.SystemCancel:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105005));
                        }
                        break;
                    default:
                        break;
                }
                StringBuilderPool.ReleaseTemporary(stringBuilder);
            }

            voteState.Clear();
            VoteID = 0;
            UIManager.CloseUI(EUIID.UI_ClassicBossWarEnterConfirm);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 前往经典boss战
        /// </summary>
        /// <param name="classicBossId"></param>
        /// <param name="uNpcId"></param>
        public void GotoClassicBossWar(uint classicBossId, ulong uNpcId)
        {
            if (Sys_Team.Instance.HaveTeam)
            {
                // var teamMem = Sys_Team.Instance.teamMemsOfUnActive;
                // if (teamMem.Count > 0)
                // {
                //     StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                //     for (int i = 0; i < teamMem.Count; i++)
                //     {
                //         stringBuilder.Append(teamMem[i].Name.ToStringUtf8() + " ");
                //     }
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105006, stringBuilder.ToString()));
                //     StringBuilderPool.ReleaseTemporary(stringBuilder);
                //     return;
                // }
            }

            Sys_ClassicBossWar.Instance.ClassicBossStartVoteReq(classicBossId, uNpcId);
        }

        /// <summary>
        /// 获取成员投票状态
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public VoterOpType GetVoteOp_ClassicBossWar(ulong roleId)
        {
            if (voteState.ContainsKey(roleId))
                return (VoterOpType)voteState[roleId];
            else
                return VoterOpType.None;
        }
        /// <summary>
        /// 获得活动次数
        /// </summary>
        /// <returns></returns>
        public int GetDailyTimes()
        {
            int times = 0;
            CheckClassicBossWarExpireTime();
            if (null != cmdClassicBossDataNtf && null != cmdClassicBossDataNtf.Data && null != cmdClassicBossDataNtf.Data.RewardLimit)
            {
                times = (int)cmdClassicBossDataNtf.Data.RewardLimit.UsedTimes;
            }
            else
            {
                times = 0;
            }
            return times;
        }
        /// <summary>
        /// 活动活动最大次数
        /// </summary>
        /// <returns></returns>
        public int GetDailyTotalTimes()
        {
            int MaxTimes = 0;
            CSVDailyActivity.Data cSVDailyActivityData = CSVDailyActivity.Instance.GetConfData(10);
            if (null != cSVDailyActivityData)
                MaxTimes = (int)cSVDailyActivityData.limite;
            return MaxTimes;
        }
        /// <summary>
        /// 打开经典Boss战结果界面
        /// </summary>
        public void OpenClassicBossWarResultView()
        {
            if (null != cmdClassicBossResultNtf && IsBattleOver)
            {
                UIManager.OpenUI(EUIID.UI_ClassicBossWarResult, false, cmdClassicBossResultNtf);
                cmdClassicBossResultNtf = null;
            }
        }
        /// <summary>
        /// 检测经典boss战过期
        /// </summary>
        public void CheckClassicBossWarExpireTime()
        {
            if (Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf.Data.RewardLimit.ExpireTime < Sys_Time.Instance.GetServerTime())
            {
                Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf.Data.RewardLimit.UsedTimes = 0;
            }
        }
        #endregion
    }
}