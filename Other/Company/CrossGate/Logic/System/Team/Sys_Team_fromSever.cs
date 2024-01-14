using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
namespace Logic
{
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        #region fromServer

        enum ERoleStateTeamInfo
        {
            None,
            NewTeamMember,
            NewTeamCaptain,
            MemberToCaptain,
            CaptainToMember,
            RemoveTeam,

        }

        //private ERoleStateTeamInfo GetRoleTeamState(CmdTeamInfoNtf newInfo, CmdTeamInfoNtf oldInfo)
        //{

        //}
        /// <summary>
        /// 组队信息
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_TeamInfo(NetMsg msg)
        {
            CmdTeamInfoNtf info = NetMsgUtil.Deserialize<CmdTeamInfoNtf>(CmdTeamInfoNtf.Parser, msg);


            if (CheckTeamClear(info))
                return;

            bool  isLastHaveTeam = HaveTeam;
            bool  isLastCaptain = isCaptain();
            bool isLastLeave = isPlayerLeave();

            var LastTeamInfo = TeamInfo;

            TeamInfo = info;

            bool isNewTeam = (!isLastHaveTeam && HaveTeam);

            if (isNewTeam)
                ToHaveNewTeam();

            if ((isNewTeam || isLastCaptain)&& isCaptain() == false) // 加入队伍，成为队员  由队长变成队员，对断线重连导致数据发生变化的处理
                ToBeTeamMember(isLastCaptain);


            CheckPlayerState(LastTeamInfo);

            if (isCaptain())
                ToBeCaptain(isLastCaptain);
            
            if (!isLastHaveTeam)
                CreateTeamHud();

            UpdateFollowManager();

            eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);
        }

        private bool CheckTeamClear(CmdTeamInfoNtf info)
        {
            bool isLastHaveTeam = HaveTeam;
            bool isLastCaptain = isCaptain();

            if (info.TeamInfo.TeamId == 0)
            {
                ClearFollow();

                ClearTeam();

                eventEmitter.Trigger(EEvents.TeamClear);

                return true;
            }

            return false;
        }

        private void ToHaveNewTeam()
        {
            StopNoTeamMatchTimer();
            eventEmitter.Trigger(EEvents.HaveTeam);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event30);
        }

        /// <summary>
        /// 成为队员
        /// </summary>
        /// <param name="isLastCaptain"></param>
        private void ToBeTeamMember(bool isLastCaptain)
        {
            clearMsg((ushort)CmdTeam.InviteNtf);

            activeFristMsg();

            eventEmitter.Trigger(EEvents.BeMember);

        }

        /// <summary>
        /// 成为队长
        /// </summary>
        private void ToBeCaptain()
        {
            
        }

        /// <summary>
        /// 成为无队伍
        /// </summary>
        private void ToWithOutTeam()
        {
            
        }
        private void CheckPlayerState(CmdTeamInfoNtf LastTeamInfo)
        {
            var roldId = Sys_Role.Instance.RoleId;

            bool isLastEmpty = TeamInfoHelper.IsEmpty(LastTeamInfo);

            bool isLastPlayerLeave = TeamInfoHelper.IsLeave(LastTeamInfo, roldId);

            bool isNowPlayerLeave = isPlayerLeave();

            //玩家为队员时，转成暂离
            if (isNowPlayerLeave)
            {
                DoNotFollow(roldId);
            }

            //玩家为队员时，由暂离转成未暂离状态
            if (isLastPlayerLeave && !isNowPlayerLeave)
            {
                DoFollow(roldId); 
            }

        }
        private void ToBeCaptain(bool isLastCaptain)
        {
           // FollowCaptain.DoTeamUpLoadCap();

            if (!isLastCaptain)
                eventEmitter.Trigger(EEvents.BeCaptain);
        }

        private void CreateTeamHud()
        {
            int count = TeamMemsCount;

            for (int i = 0; i < count; i++)
            {
                var teamMem = getTeamMem(i);

                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName,
                    new ActorHUDTitleNameUpdateEvt() { id = teamMem.MemId, eFightOutActorType = EFightOutActorType.Teammate });
            }
        }

        protected void Notify_Message_ChangeOrderNtf(NetMsg msg)
        {
            CmdTeamChangeOrderNtf info = NetMsgUtil.Deserialize<CmdTeamChangeOrderNtf>(CmdTeamChangeOrderNtf.Parser, msg);

            var oldmems = TeamInfo.TeamInfo.Mems;

            TeamInfo.TeamInfo.Mems.Clear();

            TeamInfo.TeamInfo.Mems.AddRange(info.Mems);

            UpdateFollowManager();

            eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);

        }
        /// <summary>
        /// 自由人申请入队请求，服务器返回
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_EnterRes(NetMsg msg)
        {
            CmdTeamApplyRes info = NetMsgUtil.Deserialize<CmdTeamApplyRes>(CmdTeamApplyRes.Parser, msg);
        }


        private float m_LastTipsTime = 0;
        /// <summary>
        /// 服务器通知，给队长申请入队通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_EnterNtf(NetMsg msg)
        {
            CmdTeamApplyNtf info = NetMsgUtil.Deserialize<CmdTeamApplyNtf>(CmdTeamApplyNtf.Parser, msg);

            TeamInfo.TeamInfo.Applys.Add(info.Applyrole);


            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.RefusalToTeamApplyTips) == false && /*Sys_Instance.Instance.IsInInstance == false&&*/
                UIManager.IsOpen(EUIID.UI_Team_Tips) == false /*&& Sys_Fight.Instance.IsFight() == false*/ &&

                Time.time - m_LastTipsTime >= m_TipsCDTime)
            {
                m_LastTipsTime = Time.time;

                var parmas = new UI_Team_Tips_Param();
                parmas.time = m_TipsDefatulWaitTime;
                parmas.message = LanguageHelper.GetTextContent(10948, info.Applyrole.Info.Name.ToStringUtf8());
                parmas.OpReslutAc = OnTipsCall;
                UIManager.OpenUI(EUIID.UI_Team_Tips, false, parmas);

            }

            eventEmitter.Trigger(EEvents.NetMsg_ApplyListOpRes);

            // NetMsgNotify_GetApply(info);
        }

        private void OnTipsCall(int result)
        {
            m_LastTipsTime = Time.time;
        }
        /// <summary>
        /// 服务器通知，被邀请人收到邀请通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_InviteNtf(NetMsg msg)
        {
            CmdTeamInviteNtf info = NetMsgUtil.Deserialize<CmdTeamInviteNtf>(CmdTeamInviteNtf.Parser, msg);

            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.RefusalToTeam))
            {
                InvitedAnswer(2,info);
                return;
            }


            //NetMsgNotify_Invite(info);

            var emsgtype = info.IsTutor ? EMessageBagType.Tutor : EMessageBagType.Team;

            Sys_MessageBag.Instance.SendMessageInfo(emsgtype, new Sys_MessageBag.MessageContent() {

                mType = emsgtype,
                invitorId = info.InvitorId,
                invitiorName = info.InvitorName,
                countDownTime = GetMsgOpWaitTime(0),
                tMess = new Sys_MessageBag.TeamMessage() {

                    targetId = info.TargetId,
                    teamId = info.TeamId,
                    lowLv = info.LowLv,
                    highLv = info.HighLv,
                    isTutor = info.IsTutor
                }

            });
        }



        /// <summary>
        /// 服务器通知，队长收到队员邀请他人入队
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_MemInviteNtf(NetMsg msg)
        {
            CmdTeamMemInviteNtf info = NetMsgUtil.Deserialize<CmdTeamMemInviteNtf>(CmdTeamMemInviteNtf.Parser, msg);

            NetMsgNotify_InviteApply(info);
        }
        /// <summary>
        /// 玩家状态通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_MemStateNtf(NetMsg msg)
        {
            CmdTeamMemStateNtf info = NetMsgUtil.Deserialize<CmdTeamMemStateNtf>(CmdTeamMemStateNtf.Parser, msg);

            TeamMem teamMem = getTeamMem(info.RoleId);

            if (teamMem == null || teamMem.State == info.State)
                return;

            bool lastLeave = teamMem.IsLeave();

            teamMem.SetState(info.State);

            eventEmitter.Trigger<ulong>(EEvents.NetMsg_MemState, info.RoleId);

            //if (teamMem.IsLeave())
            //{
            //    followManager.ExitTeam(teamMem.MemId);
            //}
            //else if (lastLeave && teamMem.IsLeave() == false)
            //{
            //    followManager.EnterTeam(teamMem.MemId);
            //}

            followManager.UpdateFormation();

        }

        /// <summary>
        /// 玩家离队通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_MemLeaveNtf(NetMsg msg)
        {
            CmdTeamMemLeaveNtf info = NetMsgUtil.Deserialize<CmdTeamMemLeaveNtf>(CmdTeamMemLeaveNtf.Parser, msg);

            ulong playerRoleId = Sys_Role.Instance.Role.RoleId;

            TeamMem teamMemLeave = getTeamMem(info.RoleId);

            if (playerRoleId == info.RoleId)
            {
                foreach (TeamMem teamMem in teamMems)
                {
                    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName,
                                                          new ActorHUDTitleNameUpdateEvt()
                                                          {
                                                              id = teamMem.MemId,
                                                              eFightOutActorType = teamMem.MemId == Sys_Role.Instance.RoleId ? EFightOutActorType.MainHero : EFightOutActorType.OtherHero
                                                          });
                }

                

                ClearTeam();

                UpdateFollowManager();

                eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);

                eventEmitter.Trigger(EEvents.TeamClear);
                return;
            }

            DoNotFollow(info.RoleId);

            ulong lastCapID = CaptainRoleId;

            bool b = removeTeamMem(info.RoleId);

            UpdateFollowManager();


            if (b)
            {
                eventEmitter.Trigger<ulong>(EEvents.NetMsg_MemLeaveNtf, info.RoleId);
            }
                

            Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName,
                                                                              new ActorHUDTitleNameUpdateEvt(){
                                                                              id = info.RoleId,
                                                                              eFightOutActorType = info.RoleId == playerRoleId ? EFightOutActorType.MainHero : EFightOutActorType.OtherHero
                                                                               });

        }

        /// <summary>
        /// 玩家入队通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_MemEnterNtf(NetMsg msg)
        {
            CmdTeamMemEnterNtf info = NetMsgUtil.Deserialize<CmdTeamMemEnterNtf>(CmdTeamMemEnterNtf.Parser, msg);

            bool result = AddTeamMem(info.Mem);

            if (result)
            {
                followManager.EnterTeam(info.Mem.MemId);

                eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);

                eventEmitter.Trigger<TeamMem>(EEvents.MemEnterNtf, info.Mem);

                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName,
                                                      new ActorHUDTitleNameUpdateEvt()
                                                      {
                                                          id = info.Mem.MemId,
                                                          eFightOutActorType =  EFightOutActorType.Teammate
                                                      });
            }

            uint targetid = teamTarget != null ? teamTarget.TargetId : 0;

            var teamdata = CSVTeam.Instance.GetConfData(targetid);

            if (isCaptain()&& teamdata != null && teamdata.is_show > 0 && isMatching && TeamMemsCount == 5 && HaveRob() == false)
            {
                OpenTeamTargetMatchFullTips();
            }

        }

        /// <summary>
        /// 玩家入队申请列表通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_ApplyListOpRes(NetMsg msg)
        {
            CmdTeamApplyListOpRes info = NetMsgUtil.Deserialize<CmdTeamApplyListOpRes>(CmdTeamApplyListOpRes.Parser, msg);

            TeamInfo.TeamInfo.Applys.Clear();

            TeamInfo.TeamInfo.Applys.AddRange(info.Applyroles);

            eventEmitter.Trigger(EEvents.NetMsg_ApplyListOpRes);
           
        }

        /// <summary>
        /// 被踢
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_BeKickedNtf(NetMsg msg)
        {
            CmdTeamBeKickedNtf info = NetMsgUtil.Deserialize<CmdTeamBeKickedNtf>(CmdTeamBeKickedNtf.Parser, msg);

            foreach (TeamMem teamMem in teamMems)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDTitleNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUDTitleName,
                                                      new ActorHUDTitleNameUpdateEvt()
                                                      {
                                                          id = teamMem.MemId,
                                                          eFightOutActorType = teamMem.MemId == Sys_Role.Instance.RoleId ? EFightOutActorType.MainHero : EFightOutActorType.OtherHero
                                                      });
            }

            ClearTeam();

            UpdateFollowManager();

            eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);
            eventEmitter.Trigger(EEvents.TeamClear);
            // Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002126));



        }
        /// <summary>
        /// 移交队长通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_TransLeaderNtf(NetMsg msg)
        {
            CmdTeamTransLeaderNtf info = NetMsgUtil.Deserialize<CmdTeamTransLeaderNtf>(CmdTeamTransLeaderNtf.Parser, msg);

            bool lastCaptain = isCaptain();


            ClearFollow();

            SetTeamMems(info.Mems);

            UpdateFollowManager();

            if (lastCaptain && isCaptain() == false)
            {
                TeamInfo.TeamInfo.Applys.Clear();

                eventEmitter.Trigger(EEvents.BeMember);

            }
            else if (!lastCaptain && isCaptain())
            {
                eventEmitter.Trigger(EEvents.BeCaptain);
            }
                

            eventEmitter.Trigger(EEvents.NetMsg_InfoNtf);

            string chatStr = string.Format("[@{0}]", Captain.Name.ToStringUtf8());

            Sys_Chat.Instance.PushMessage(ChatType.Team, null,
                LanguageHelper.GetTextContent(2002207, chatStr));

        }
        ///// <summary>
        ///// 服务器通知，队长收到队员邀请他人入队的反馈 同意或者拒绝
        ///// </summary>
        ///// <param name="msg"></param>
        //protected void Notify_Message_MemInviteOpReq(NetMsg msg)
        //{
        //    CmdTeamMemInviteOpReq info = NetMsgUtil.Deserialize<CmdTeamMemInviteOpReq>(CmdTeamMemInviteOpReq.Parser, msg);
        //}


        /// <summary>
        /// 队员信息更新通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_MemInfoUpdateNtf(NetMsg msg)
        {
            CmdTeamMemInfoUpdateNtf info = NetMsgUtil.Deserialize<CmdTeamMemInfoUpdateNtf>(CmdTeamMemInfoUpdateNtf.Parser, msg);

            int memIndex = SetTeamMem(info.Mem,info.UpdateType);

            if (memIndex >= 0)
                eventEmitter.Trigger<int,uint>(EEvents.NetMsg_MemInfoUpdateNtf, memIndex,info.UpdateType);

        }

        /// <summary>
        /// 玩家操作邀请后，系统反馈
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_InviteOpRes(NetMsg msg)
        {
            CmdTeamInviteOpRes info = NetMsgUtil.Deserialize<CmdTeamInviteOpRes>(CmdTeamInviteOpRes.Parser, msg);

            if (info.Op!=0)
            {
                eventEmitter.Trigger(EEvents.RefuseTeam);
            }
            
            
            clearMsg((ushort)CmdTeam.ApplyLeadingNtf);
        }

        /// <summary>
        /// 申请带队通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_ApplyLeadingNtf(NetMsg msg)
        {
            CmdTeamApplyLeadingNtf info = NetMsgUtil.Deserialize<CmdTeamApplyLeadingNtf>(CmdTeamApplyLeadingNtf.Parser, msg);

            NetMsgNotify_Leading(info);

        }

        /// <summary>
        /// 申请带队结果通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_ApplyLeadingResultNtf(NetMsg msg)
        {
            CmdTeamApplyLeadingResultNtf info = NetMsgUtil.Deserialize<CmdTeamApplyLeadingResultNtf>(CmdTeamApplyLeadingResultNtf.Parser, msg);

           

        }

        /// <summary>
        /// 收到召回通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_CallBackNtf(NetMsg msg)
        {
            CmdTeamCallBackNtf info = NetMsgUtil.Deserialize<CmdTeamCallBackNtf>(CmdTeamCallBackNtf.Parser, msg);

            NetMsgNotify_Leading(info);

        }

        protected void NotifyTargetChangeNtf(NetMsg msg)
        {
            CmdTeamTargetChangeNtf info = NetMsgUtil.Deserialize<CmdTeamTargetChangeNtf>(CmdTeamTargetChangeNtf.Parser, msg);

            teamTarget.TargetId = info.TargetId;
            teamTarget.HighLv = info.HighLv;
            teamTarget.LowLv = info.LowLv;


            var data = CSVTeam.Instance.GetConfData(info.TargetId);

            if (data != null)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12200, LanguageHelper.GetTextContent(data.subclass_name)));

        }

        /// <summary>
        /// 委托指挥通知
        /// </summary>
        /// <param name="msg"></param>
        protected void Notify_Message_EntrustCommandNtf(NetMsg msg)
        {
            CmdTeamEntrustCommandNtf info = NetMsgUtil.Deserialize<CmdTeamEntrustCommandNtf>(CmdTeamEntrustCommandNtf.Parser, msg);

            var lastEntrustmemId = TeamInfo.TeamInfo.EntrustMemId;

            TeamInfo.TeamInfo.EntrustMemId = info.EntrustMemId;

            TeamInfo.TeamInfo.CommandDatas.Clear();
            TeamInfo.TeamInfo.CommandDatas.AddRange(info.CommandDatas);

            UpdataTeamWarCommand();


            if (lastEntrustmemId == info.EntrustMemId)
                return;

            var member = getTeamMem(info.EntrustMemId);


            if (info.EntrustMemId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002254));
            }


            if (member != null)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002255, member.Name.ToStringUtf8()));

                string chatStr = string.Format("[@{0}]", member.Name.ToStringUtf8());

                Sys_Chat.Instance.PushMessage(ChatType.Team, null, LanguageHelper.GetTextContent(2002255, chatStr));

            }

            eventEmitter.Trigger(EEvents.EntrustNtf);

        }

        #endregion

        #region 队伍目标

        //自定义数据更新
        protected void Notify_Message_CustomInfoDataNtf(NetMsg msg)
        {
            CmdTeamCustomInfoDataNtf info = NetMsgUtil.Deserialize<CmdTeamCustomInfoDataNtf>(CmdTeamCustomInfoDataNtf.Parser, msg);

            mtargetCunstomInfo.Clear();
            for (int i = 0; i < info.CustomInfoList.Count; i++)
            {
                mtargetCunstomInfo.Add(info.CustomInfoList[i].TargetId, info.CustomInfoList[i]);
            }

           // eventEmitter.Trigger(EEvents.CustomTargetInfo);
        }

        //更改目标反馈
        protected void Notify_Message_EditTargetRes(NetMsg msg)
        {
            CmdTeamEditTargetRes info = NetMsgUtil.Deserialize<CmdTeamEditTargetRes>(CmdTeamEditTargetRes.Parser, msg);

          

            eventEmitter.Trigger(EEvents.TargetUpdate);
        }

        //目标刷新
        protected void Notify_Message_TargetUpdateNtf(NetMsg msg)
        {
            CmdTeamTargetUpdateNtf info = NetMsgUtil.Deserialize<CmdTeamTargetUpdateNtf>(CmdTeamTargetUpdateNtf.Parser, msg);

            teamTarget = info.Target;

            teamDesc = info.Target.Desc.ToStringUtf8();

            eventEmitter.Trigger(EEvents.TargetUpdate);

            var data = CSVTeam.Instance.GetConfData(info.Target.TargetId);

            if(data != null)
               Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12200, LanguageHelper.GetTextContent(data.subclass_name)));

            
               


        }

        //自定义信息返回
        protected void Notify_Message_EditCustomInfoRes(NetMsg msg)
        {
            CmdTeamEditCustomInfoRes info = NetMsgUtil.Deserialize<CmdTeamEditCustomInfoRes>(CmdTeamEditCustomInfoRes.Parser, msg);

            if (mtargetCunstomInfo.ContainsKey(info.TargetId))
            {
                mtargetCunstomInfo[info.TargetId] = new CmdTeamCustomInfoDataNtf.Types.CustomInfo() { TargetId = info.TargetId, CustomInfo_ = info.CustomInfo };
            }
            else
            {
                mtargetCunstomInfo.Add(info.TargetId,new CmdTeamCustomInfoDataNtf.Types.CustomInfo() { TargetId = info.TargetId, CustomInfo_ = info.CustomInfo });
            }

            eventEmitter.Trigger(EEvents.CustomTargetInfo,info.TargetId);
        }

        //队伍目标描述更新
        protected void Notify_Message_EditDescRes(NetMsg msg)
        {
            CmdTeamEditDescNtf info = NetMsgUtil.Deserialize<CmdTeamEditDescNtf>(CmdTeamEditDescNtf.Parser, msg);

            teamDesc = info.Desc.ToStringUtf8();

            eventEmitter.Trigger(EEvents.TargetDesc);
        }

        //队伍目标匹配状态
        protected void Notify_Message_MatchingNtf(NetMsg msg)
        {
            CmdTeamMatchingNtf info = NetMsgUtil.Deserialize<CmdTeamMatchingNtf>(CmdTeamMatchingNtf.Parser, msg);

            isMatching = info.Op == 0 ? true : false;

            var lastid = MatchingTarget;

            MatchingTarget = info.TargetId;

            if(HaveTeam == false && lastid != MatchingTarget)
                StartNoTeamMatch();

            eventEmitter.Trigger<uint,uint,bool>(EEvents.TargetMatching,lastid,MatchingTarget, isMatching);
        }

        public CmdTeamQueryMatchListRes MatchInfo { get; private set; } = new CmdTeamQueryMatchListRes();
        //队伍目标匹配列表
        protected void Notify_Message_QueryMatchListRes(NetMsg msg)
        {
            CmdTeamQueryMatchListRes info = NetMsgUtil.Deserialize<CmdTeamQueryMatchListRes>(CmdTeamQueryMatchListRes.Parser, msg);

            m_targetTeamInfos.Clear();

            m_targetTeamInfos.AddRange(info.List);

            m_targetTeamInfos.Sort((x, y) => {

                if (x.CreateTime > y.CreateTime)
                    return 1;

                if (x.CreateTime < y.CreateTime)
                    return -1;

                return 0;
            });

            MatchInfo = info;

            eventEmitter.Trigger(EEvents.TargetList);

        }


        private void OnSearchNearRole(NetMsg msg)
        {
            CmdTeamSearchNearRoleRes info = NetMsgUtil.Deserialize<CmdTeamSearchNearRoleRes>(CmdTeamSearchNearRoleRes.Parser, msg);

            SearchNearRoles = info;

            eventEmitter.Trigger(EEvents.SearchNearRole);
        }

        private void OnTalkBack(NetMsg msg)
        {
            CmdTeamLookForTeamRes info = NetMsgUtil.Deserialize<CmdTeamLookForTeamRes>(CmdTeamLookForTeamRes.Parser, msg);

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10991));
        }

        private void OnInviteSocialNtf(NetMsg msg)
        {
            CmdTeamInviteSocialDataNtf info = NetMsgUtil.Deserialize<CmdTeamInviteSocialDataNtf>(CmdTeamInviteSocialDataNtf.Parser, msg);

            TeamInfo.TeamInfo.InviteSocialData = info.Data;
        }

        private void OnInviteSocialRes(NetMsg msg)
        {
            CmdTeamInviteSocialRes info = NetMsgUtil.Deserialize<CmdTeamInviteSocialRes>(CmdTeamInviteSocialRes.Parser, msg);

            TeamInfo.TeamInfo.InviteSocialData = info.Data;

            OpenAdviceDialog(info.Type, info.MemIds);
        }

        private void OnInviteSocialConfirmRes(NetMsg msg)
        {
            CmdTeamInviteSocialConfirmRes info = NetMsgUtil.Deserialize<CmdTeamInviteSocialConfirmRes>(CmdTeamInviteSocialConfirmRes.Parser, msg);

            TeamInfo.TeamInfo.InviteSocialData = info.Data;
        }

        private void OnLeaderSkipBtnUpdataNtf(NetMsg msg)
        {
            CmdTeamLeaderSkipBtnUpdateNtf info = NetMsgUtil.Deserialize<CmdTeamLeaderSkipBtnUpdateNtf>(CmdTeamLeaderSkipBtnUpdateNtf.Parser, msg);

            if (info.All)
            {
                TeamInfo.TeamInfo.SkipTalkBtnList.Clear();
                TeamInfo.TeamInfo.SkipTalkBtnList.AddRange(info.SkipBtnList);
            }
            else
            {
                int count = info.SkipBtnList.Count;

                for (int i = 0; i < count; i++)
                {
                    var data = TeamInfo.TeamInfo.SkipTalkBtnList.Find(o => o.Key == info.SkipBtnList[i].Key);
                    if (data != null)
                    {
                        data.Skip = info.SkipBtnList[i].Skip;
                    }
                    else
                    {
                        TeamInfo.TeamInfo.SkipTalkBtnList.Add(info.SkipBtnList[i]);
                    }
                }
            }

            
           

            eventEmitter.Trigger(EEvents.LeaderSkipBtnUpdateNtf);
            

        }

        private void OnInviteResNtf(NetMsg msg)
        {
            // CmdTeamInviteRes info = NetMsgUtil.Deserialize<CmdTeamInviteRes>(CmdTeamInviteRes.Parser, msg);

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12466));
        }
        #endregion

        #region 服务器消息处理
        /// <summary>
        /// 得到入队申请
        /// </summary>
        private void NetMsgNotify_GetApply(CmdTeamApplyNtf info)
        {

            pushMsg(info, (ushort)CmdTeam.ApplyNtf, ApplyCaptainAnswer);
        }

        /// <summary>
        /// 得到邀请申请-队长
        /// </summary>
        private void NetMsgNotify_InviteApply(CmdTeamMemInviteNtf info)
        {
            if (m_MsgQueue.isContain((ushort)CmdTeam.MemInviteNtf))
                return;
            pushMsg(info, (ushort)CmdTeam.MemInviteNtf, MemberCaptainAnswer, GetMsgOpWaitTime(3));
        }

        /// <summary>
        /// 得到邀请
        /// </summary>
        private void NetMsgNotify_Invite(CmdTeamInviteNtf info)
        {
            //if (GameCenter.playState == GameCenter.EPlayState.Fight)
            //    return;

            //clearMsg(o => {

            //    var value = o.Message as CmdTeamInviteNtf;

            //    if (value == null)
            //        return false;

            //    if (value.TeamId == info.TeamId)
            //        return true;

            //    return false;
            //});


            pushMsg(info, (ushort)CmdTeam.InviteNtf, InvitedAnswer, GetMsgOpWaitTime(0));
        }


        /// <summary>
        /// 申请带队
        /// </summary>
        private void NetMsgNotify_Leading(CmdTeamApplyLeadingNtf info)
        {
            //if (GameCenter.playState == GameCenter.EPlayState.Fight)
            //    return;

            pushMsg(info, (ushort)CmdTeam.ApplyLeadingNtf, ApplyLeadingAnswer, GetMsgOpWaitTime(2));
        }


        /// <summary>
        /// 召回通知
        /// </summary>
        private void NetMsgNotify_Leading(CmdTeamCallBackNtf info)
        {
            //if (GameCenter.playState == GameCenter.EPlayState.Fight)
            //    return;

            if (m_MsgQueue.isContain((ushort)CmdTeam.CallBackNtf))
                return;

            pushMsg(info, (ushort)CmdTeam.CallBackNtf, CallBackAnswer,60,2);
        }




        #endregion
    }
}
