using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Table;
namespace Logic
{
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        public enum EEvents
        {
            /// <summary>
            /// 组队信息更新
            /// </summary>
            NetMsg_InfoNtf,

            /// <summary>
            /// 申请入队返回
            /// </summary>
            NetMsg_ApplyRes,
            /// <summary>
            /// 给队长通知入队申请
            /// </summary>
            NetMsg_ApplyNtf,

            /// <summary>
            /// 收到邀请通知
            /// </summary>
            NetMsg_InviteNtf,

            /// <summary>
            /// 队长收到队员邀请他人入队伍
            /// </summary>
            NetMsg_CaptainInvite,

            NetMsg_MemState,

            NetMsg_MemLeaveNtf,

            NetMsg_ApplyListOpRes,

            MembersInfo,

            NetMsg_MemInfoUpdateNtf,

            TeamClear,//队伍解散
            BeCaptain,//成为队长
            BeMember,//成为队员
            CustomTargetInfo,//自定义目标信息
            TargetUpdate,//目标更改
            TargetDesc,//目标描述
            TargetMatching,//目标匹配状态
            TargetList,//目标匹配列表

            WarCommandNtf,//战斗指令
            WarCommandRes,//战斗指令修改

            MatchState,//匹配状态

            EntrustNtf,//委托指挥
            MemEnterNtf,//队员加入
            HaveTeam,//加入队伍

            EditQuickCommandRes,

            SearchNearRole,
            RefuseTeam,
            LeaderSkipBtnUpdateNtf,

        }

        /// <summary>
        /// 组队消息委托
        /// </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public CmdTeam CurcmdTeam { get; private set; }

        protected List<float> MsgWaitOpTimeconfig = new List<float>();

        private float m_TipsCDTime = 0;

        private float m_TipsDefatulWaitTime = 0;

        public float MemberDistance { get; private set; } = 1;

        public CmdTeamSearchNearRoleRes SearchNearRoles { get; private set; } = null;

        public int TeamMemberCountMax { get; private set; } = 5;


        private float MatchTeamTime = 0;
        private float MatchTeamTipsTime = 0;

        public string DefaultTalkString = string.Empty;
        public override void Init()
        {
           

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InfoNtf, Notify_Message_TeamInfo, CmdTeamInfoNtf.Parser);//组队信息

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ApplyRes, Notify_Message_EnterRes, CmdTeamApplyRes.Parser);//申请入队返回 给自由人的申请入队返回 

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ApplyNtf, Notify_Message_EnterNtf, CmdTeamApplyNtf.Parser);//申请入队通知 给队长的有人申请入队通知

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteNtf, Notify_Message_InviteNtf, CmdTeamInviteNtf.Parser);//被邀请人收到邀请入队通知

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MemInviteNtf, Notify_Message_MemInviteNtf, CmdTeamMemInviteNtf.Parser);//队长收到队员邀请他人入队

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MemStateNtf, Notify_Message_MemStateNtf, CmdTeamMemStateNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MemLeaveNtf, Notify_Message_MemLeaveNtf, CmdTeamMemLeaveNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MemEnterNtf, Notify_Message_MemEnterNtf, CmdTeamMemEnterNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ApplyListOpRes, Notify_Message_ApplyListOpRes, CmdTeamApplyListOpRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.BeKickedNtf, Notify_Message_BeKickedNtf, CmdTeamBeKickedNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.TransLeaderNtf, Notify_Message_TransLeaderNtf, CmdTeamTransLeaderNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MemInfoUpdateNtf, Notify_Message_MemInfoUpdateNtf, CmdTeamMemInfoUpdateNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteOpRes, Notify_Message_InviteOpRes, CmdTeamInviteOpRes.Parser);//邀请操作服务器反馈

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ApplyLeadingNtf, Notify_Message_ApplyLeadingNtf, CmdTeamApplyLeadingNtf.Parser);//申请带队通知
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ApplyLeadingResultNtf, Notify_Message_ApplyLeadingResultNtf, CmdTeamApplyLeadingResultNtf.Parser);//申请带队结果通知

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.CallBackNtf, Notify_Message_CallBackNtf, CmdTeamCallBackNtf.Parser);//召回通知

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.TargetChangeNtf, NotifyTargetChangeNtf, CmdTeamTargetChangeNtf.Parser);//召回通知



            //队伍目标
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.CustomInfoDataNtf, Notify_Message_CustomInfoDataNtf, CmdTeamCustomInfoDataNtf.Parser);//自定义
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EditTargetRes, Notify_Message_EditTargetRes, CmdTeamEditTargetRes.Parser);//更改组队目标请求返回
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.TargetUpdateNtf, Notify_Message_TargetUpdateNtf, CmdTeamTargetUpdateNtf.Parser);//队伍目标更新
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EditCustomInfoRes, Notify_Message_EditCustomInfoRes, CmdTeamEditCustomInfoRes.Parser);//修改自定义目标返回
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EditDescNtf, Notify_Message_EditDescRes, CmdTeamEditDescNtf.Parser);//修改队伍描述返回
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.MatchingNtf, Notify_Message_MatchingNtf, CmdTeamMatchingNtf.Parser);//匹配状态队伍广播
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.QueryMatchListRes, Notify_Message_QueryMatchListRes, CmdTeamQueryMatchListRes.Parser);//请求队伍列表返回


            //指令

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.CommandDataNtf, Notify_Message_CommandDataNtf, CmdTeamCommandDataNtf.Parser);//指令下发
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EditCommandRes, Notify_Message_EditCommandRes, CmdTeamEditCommandRes.Parser);//指令修改
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EditQuickCommandRes, Notify_Message_EditQuickCommandRes, CmdTeamEditQuickCommandRes.Parser);//指令修改

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.ChangeOrderNtf, Notify_Message_ChangeOrderNtf, CmdTeamChangeOrderNtf.Parser);//指令修改


            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.EntrustCommandNtf, Notify_Message_EntrustCommandNtf, CmdTeamEntrustCommandNtf.Parser);//委托指挥

            //Sys_Input.Instance.onKeyDown += onKeyDown;

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncStat, OnSyncStat, CmdTeamSyncStat.Parser); //同步队长流程状态

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SearchNearRoleRes, OnSearchNearRole, CmdTeamSearchNearRoleRes.Parser); //同步队长流程状态

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.LookForTeamRes, OnTalkBack, CmdTeamLookForTeamRes.Parser); //喊话反馈

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteSocialDataNtf, OnInviteSocialNtf, CmdTeamInviteSocialDataNtf.Parser); //喊话反馈

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteSocialRes, OnInviteSocialRes, CmdTeamInviteSocialRes.Parser); //喊话反馈

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteSocialConfirmRes, OnInviteSocialConfirmRes, CmdTeamInviteSocialConfirmRes.Parser); //


            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.LeaderSkipBtnUpdateNtf, OnLeaderSkipBtnUpdataNtf, CmdTeamLeaderSkipBtnUpdateNtf.Parser); //喊话反馈

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.InviteRes, OnInviteResNtf, CmdTeamInviteRes.Parser); //要求反馈

            Sys_Input.Instance.onTouchUp += OnTouchUp;
            //Sys_Input.Instance.onLeftJoystick += OnLeftJoystick;

            Sys_Fight.Instance.OnEnterFight += OnEnterFight;
            Sys_Fight.Instance.OnExitFight += OnExitFight;

            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnBattleSettlement, this.OnEndBattle, true);

            ReadParamConfig();
        }

        private void ReadParamConfig()
        {
            var timeParamData = CSVParam.Instance.GetConfData(500);
            if (timeParamData != null)
            {
                string[] valueA = timeParamData.str_value.Split('|');

                for (int i = 0; i < valueA.Length; i++)
                {
                    int value0 = 0;
                    if (int.TryParse(valueA[i], out value0))
                    {
                        MsgWaitOpTimeconfig.Add(value0 / 1000f);
                    }
                }
            }

            var tTipsCDTimeParamData = CSVParam.Instance.GetConfData(922);
            float.TryParse(tTipsCDTimeParamData.str_value, out m_TipsCDTime);

            var tTipsWaitTimeParamData = CSVParam.Instance.GetConfData(921);
            float.TryParse(tTipsWaitTimeParamData.str_value, out m_TipsDefatulWaitTime);

            ReadWarCommandConfig();


            var memberDistanceParamData = CSVParam.Instance.GetConfData(1004);
            float distance = 0;
            if (float.TryParse(memberDistanceParamData.str_value, out distance))
            {
                MemberDistance = distance / 100f;
            }

            MatchTeamTime = ReadParmaTime(1287, 1);
            MatchTeamTipsTime = ReadParmaTime(1288, 1);

            AdviceTipTime = (uint)ReadParmaTime(1297,1);
            EnterTeamTimeConfig = (uint)ReadParmaTime(1295, 1);
            AdviceCDTime = (uint)ReadParmaTime(1296, 1);
        }


        private float ReadParmaTime(uint id, float scale = 1000f)
        {
            var data = CSVParam.Instance.GetConfData(id);

            if (data == null)
                return 0;
            float value = 0;

            if (float.TryParse(data.str_value, out value))
            {
                return value/ scale;
            }

            return 0;
        }
        /// <summary>
        /// 同步队长流程状态///
        /// </summary>
        /// <param name="msg"></param>
        void OnSyncStat(NetMsg msg)
        {
            CmdTeamSyncStat info = NetMsgUtil.Deserialize<CmdTeamSyncStat>(CmdTeamSyncStat.Parser, msg);

            //GameMain.Procedure.TriggerEvent(this, (int)info.Stat);
        }

        
        public override void OnLogout()
        {
            base.OnLogout();

            ClearTeam();

            ClearPlayerCommandTag();

            mCDCon.Clear();


            StopNoTeamMatchTimer();
            //ClearCDTime();
        }

        public override void Dispose()
        {
           
        }
        public override void OnLogin()
        {
            base.OnLogin();
            AddCDTime();

            DefaultTalkString = LanguageHelper.GetTextContent(12111);
        }


        public float GetMsgOpWaitTime(int index)
        {
            if (MsgWaitOpTimeconfig.Count <= index)
                return 60;

            return MsgWaitOpTimeconfig[index];
        }

        /// <summary>
        /// 组队功能是否开放
        /// </summary>
        /// <param name="bTips"></param>
        /// <returns></returns>
        public bool IsOpen(bool bTips = false)
        {
            return Sys_FunctionOpen.Instance.IsOpen(30301, bTips);
        }

        /// <summary>
        /// 快捷组队功能是否开放
        /// </summary>
        /// <param name="bTips"></param>
        /// <returns></returns>
        public bool IsFastOpen(bool bTips = false)
        {
            return Sys_FunctionOpen.Instance.IsOpen(30302, bTips);
        }


        public void OpenFastUI(uint focusTargetID,bool canOpenTeamUI = true)
        {
            if (HaveTeam == false)
            {
                UIManager.OpenUI(EUIID.UI_Team_Fast, false, focusTargetID);
            }          
            else if (canOpenTeamUI && isCaptain())
            {
                UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);


                UI_Team_Target_Parma parma = new UI_Team_Target_Parma();
                parma.FocusID = focusTargetID;
                UIManager.OpenUI(EUIID.UI_Team_Target, false, parma);
            }
                
        }

        public void OnEnterFight(CSVBattleType.Data data)
        {
            if (UIManager.IsOpen(EUIID.UI_Teaming))
            {
                UIManager.CloseUI(EUIID.UI_Teaming);
            }
        }

        public void OnExitFight()
        {
            if (HaveTeam && isCaptain() == false && isPlayerLeave() == false)
            {
                UIManager.OpenUI(EUIID.UI_Teaming);
            }

           
        }

        private void OnEndBattle(CmdBattleEndNtf ntf)
        {
            if (ntf.BattleResult == 1)
                DoAdvice(Sys_Fight.Instance.BattleTypeId);
        }
    }

    public static class TeamMemHelper
    {
        public static bool IsLeave(this TeamMem teamMem)
        {
            bool bleave = (teamMem.State & ((int)MemStatePart.TmpLeave)) != 0;

            return bleave;
        }

        public static bool IsOffLine(this TeamMem teamMem)
        {
            bool bOff = (teamMem.State & (1 << 1)) != 0;

            return bOff;
        }


        public static void SetState(this TeamMem teamMem, uint by)
        {
            teamMem.State = by;
        }

        public static bool IsRob(this TeamMem teamMem)
        {
            return ((teamMem.MemId >> 63) & 1) == 1;
        }

        public static bool HaveFamily(this TeamMem teamMem)
        {
            return teamMem.GuildId > 0;
        }

    }
}
