using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class Sys_Compete : SystemModuleBase<Sys_Compete>
    {
        public ulong inviteId = 0L;
        public ulong beInvitedId = 0L;
        public CompeteState curState = CompeteState.CompeteNormal;

        public uint oppositeNum = 0;

        public enum EEvents
        {
            //OnCollectStarted,
            //OnCollectEnded,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdCompete.InviteReq, (ushort)CmdCompete.InviteRes, OnInviteRes, CmdCompeteInviteRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCompete.InviteNtf, OnInviteNtf, CmdCompeteInviteNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCompete.InviteStateNtf, OnInviteStateNtf, CmdCompeteInviteStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCompete.InviteOpReq, (ushort)CmdCompete.InviteOpRes, OnInviteOpRes, CmdCompeteInviteOpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCompete.CancelInviteReq, (ushort)CmdCompete.CancelInviteRes, OnCancelInviteRes, CmdCompeteCancelInviteRes.Parser);
        }

        public void OnInviteReq(ulong beInvitedRoleId)
        {
            //检测地图是否可以pk
            CSVMapInfo.Data mapInfo = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            if (mapInfo != null && mapInfo.can_pvp == 1)
            {
                bool competeForbid = OptionManager.Instance.GetBoolean(OptionManager.EOptionID.RefusalToCompete); //OptionManager.Instance.mCompeteForbid.Get();
                if (competeForbid)
                {
                    //TODO: tip
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4003010));
                    return;
                }

                inviteId = Sys_Role.Instance.RoleId;
                beInvitedId = beInvitedRoleId;

                CmdCompeteInviteReq req = new CmdCompeteInviteReq();
                req.BeInviteRoleId = beInvitedRoleId;

                NetClient.Instance.SendMessage((ushort)CmdCompete.InviteReq, req);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4003005));
            }
        }

        private void OnInviteRes(NetMsg msg)
        {
            CmdCompeteInviteRes res = NetMsgUtil.Deserialize<CmdCompeteInviteRes>(CmdCompeteInviteRes.Parser, msg);
            oppositeNum = res.MemNum;
            UIManager.OpenUI(EUIID.UI_Compete);
            //UnityEngine.Debug.LogErrorFormat("OnInviteRes");
        }

        private void OnInviteNtf(NetMsg msg)
        {
            CmdCompeteInviteNtf ntf = NetMsgUtil.Deserialize<CmdCompeteInviteNtf>(CmdCompeteInviteNtf.Parser, msg);
            inviteId = ntf.InviteId;
            beInvitedId = ntf.BeInviteId;
            oppositeNum = ntf.MemNum;
            ProcessState(ntf.State);
        }

        private void OnInviteStateNtf(NetMsg msg)
        {
            CmdCompeteInviteStateNtf ntf = NetMsgUtil.Deserialize<CmdCompeteInviteStateNtf>(CmdCompeteInviteStateNtf.Parser, msg);
            ProcessState(ntf.State);
        }

        private void ProcessState(uint state)
        {
            curState = (CompeteState)state;

            //DebugUtil.LogErrorFormat("当前切磋状态:", curState.ToString());

            switch (curState)
            {
                case CompeteState.CompeteInviting:
                    OnInvtingCompeteCheckSettings();
                    break;
                case CompeteState.CompeteInviteAgree:
                    UIManager.CloseUI(EUIID.UI_Compete);
                    break;
                case CompeteState.CompeteInviteRefuse:
                    UIManager.CloseUI(EUIID.UI_Compete);
                    if (IsInviteCamp())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10690));
                    }
                    break;
                case CompeteState.CompeteCancel:
                    UIManager.CloseUI(EUIID.UI_Compete);
                    if (!IsInviteCamp())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10949));
                    }
                    break;
                //case CompeteState.CompeteSetInviteRefuse:
                //    //TODO: 对方禁止切磋提示
                //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4003011));
                    //break;
                default:
                    break;
            }
        }

        private void OnInvtingCompeteCheckSettings()
        {
            bool competeForbid = OptionManager.Instance.GetBoolean(OptionManager.EOptionID.RefusalToCompete); //OptionManager.Instance.mCompeteForbid.Get();
            if (!competeForbid)
            {
                UIManager.OpenUI(EUIID.UI_Compete);
            }
            //else
            //{
            //    CmdCompeteInviteOpReq req = new CmdCompeteInviteOpReq();
            //    req.State = (uint)CompeteState.CompeteSetInviteRefuse;
            //    NetClient.Instance.SendMessage((ushort)CmdCompete.InviteOpReq, req);
            //}
        }

        public void OnInviteOpReq(CompeteState state)
        {
            CmdCompeteInviteOpReq req = new CmdCompeteInviteOpReq();
            req.State = (uint)state;
            NetClient.Instance.SendMessage((ushort)CmdCompete.InviteOpReq, req);
        }

        private void OnInviteOpRes(NetMsg msg)
        {
            CmdCompeteInviteRes res = NetMsgUtil.Deserialize<CmdCompeteInviteRes>(CmdCompeteInviteRes.Parser, msg);
            //state = (InviteState)res.State;
        }

        public void OnCancelInviteReq()
        {
            UIManager.CloseUI(EUIID.UI_Compete);

            CmdCompeteCancelInviteReq req = new CmdCompeteCancelInviteReq();
            NetClient.Instance.SendMessage((ushort)CmdCompete.CancelInviteReq, req);
        }

        private void OnCancelInviteRes(NetMsg msg)
        {
            CmdCompeteInviteRes res = NetMsgUtil.Deserialize<CmdCompeteInviteRes>(CmdCompeteInviteRes.Parser, msg);
        }

        public void OnTimeOutReq()
        {
            CmdCompeteTimeOutCancelReq req = new CmdCompeteTimeOutCancelReq();
            NetClient.Instance.SendMessage((ushort)CmdCompete.TimeOutCancelReq, req);
        }

        /// <summary>
        /// 是否是邀请方阵营
        /// </summary>
        /// <returns></returns>
        public bool IsInviteCamp()
        {
            if (Sys_Team.Instance.isTeamMem(Sys_Role.Instance.Role.RoleId))
            {
                return  Sys_Team.Instance.isCaptain(inviteId);
            }
            else
            {
                return inviteId == Sys_Role.Instance.Role.RoleId;
            }
        }

        /// <summary>
        /// 是否是发起邀请者
        /// </summary>
        /// <returns></returns>
        public bool IsInviteRole()
        {
            if (Sys_Team.Instance.isTeamMem(Sys_Role.Instance.Role.RoleId))
            {
                return Sys_Team.Instance.isCaptain(Sys_Role.Instance.Role.RoleId);
            }
            else
            {
                return inviteId == Sys_Role.Instance.Role.RoleId;
            }
        }

        /// <summary>
        /// 是否是被邀请者
        /// </summary>
        /// <returns></returns>
        public bool IsBeInvitedRole()
        {
            return beInvitedId == Sys_Role.Instance.Role.RoleId;
        }

        /// <summary>
        /// 获得邀请方队员数量
        /// </summary>
        /// <returns></returns>
        public uint GetInviteNum()
        {
            if (IsInviteCamp())
            {
                if (Sys_Team.Instance.isTeamMem(Sys_Role.Instance.Role.RoleId))
                {
                    return (uint)Sys_Team.Instance.TeamMemsCount;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return oppositeNum;
            }
        }

        /// <summary>
        /// 获得被邀请方队员数量
        /// </summary>
        /// <returns></returns>
        public uint GetBeInviteNum()
        {
            if (IsInviteCamp())
            {
                return oppositeNum;
            }
            else
            {
                if (Sys_Team.Instance.isTeamMem(Sys_Role.Instance.Role.RoleId))
                {
                    return  (uint)Sys_Team.Instance.TeamMemsCount;
                }
                else
                {
                    return 0;
                }
            }
        }
    }
}