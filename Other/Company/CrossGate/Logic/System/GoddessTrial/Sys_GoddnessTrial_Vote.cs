using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// 进副本投票
    /// </summary>
    public partial class Sys_GoddnessTrial
    {
        private void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {

           // DebugUtil.LogError("OnNotify_Vote :  " + eVote);

            switch (eVote)
            {
                case Sys_Vote.EVote.Start:
                    OnNotify_StartVote(message as CmdRoleStartVoteNtf);
                    break;
                case Sys_Vote.EVote.DoVote:
                    OnNotify_DoVote(message as CmdRoleDoVoteNtf);
                    break;
                case Sys_Vote.EVote.End:
                    OnNotify_EndVote(message as CmdRoleVoteEndNtf);
                    break;
                case Sys_Vote.EVote.Update:
                    OnVateUpdate(message as CmdRoleVoteUpdateNtf);
                    break;
            }
        }

        private void OnNotify_StartVote(CmdRoleStartVoteNtf msg)
        {
            if (VoteID != 0)
                return;

            VoteID = msg.VoteId;

            NetMsgUtil.TryDeserialize<GoddessTrialTeamMemData>(GoddessTrialTeamMemData.Parser, msg.ClientData.ToByteArray(), out GoddessTrialTeamMemData data);

            SelectStageID = data.StageId;
            SelectInstance = data.InstanceId;

            CSVGoddessTopic.Data topicData;
            if (GetTopicDataByInstanceID(SelectInstance, out topicData) >= 0)
            {
                SelectID = topicData.id;
                SelectDifficlyID = topicData.topicDifficulty;
            }

            TeamMemsRecord = data;//投票，队伍记录数据刷新

            eventEmitter.Trigger(EEvents.StartVote);

            if (Sys_Team.Instance.isCaptain() == false)
                UIManager.OpenUI(EUIID.UI_Goddess_Enter);
        }

        private void OnNotify_DoVote(CmdRoleDoVoteNtf msg)
        {
            if (VoteID != msg.VoteId)
                return;

            eventEmitter.Trigger<ulong, int>(EEvents.DoVate, msg.RoleId, msg.Op);
        }

        private void OnNotify_EndVote(CmdRoleVoteEndNtf msg)
        {
            if (msg.VoteId != VoteID)
                return;

            if (msg.ResultType == 2) //投票未通过
            {
                string strNames = "";

                switch ((VoteFailReason)msg.FailReason)
                {

                    case VoteFailReason.Disagree:

                        for (int i = 0; i < msg.DisagreeIds.Count; ++i)
                        {
                            if (msg.DisagreeIds[i] == Sys_Role.Instance.RoleId)
                            {
                                strNames += Sys_Role.Instance.sRoleName + " ";
                            }
                            else
                            {
                                TeamMem roleInfo = Sys_Team.Instance.getTeamMem(msg.DisagreeIds[i]);
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

                        if (msg.CancelVoterId == Sys_Role.Instance.RoleId)
                        {
                            strNames += Sys_Role.Instance.sRoleName;
                        }
                        else
                        {
                            TeamMem roleInfo = Sys_Team.Instance.getTeamMem(msg.CancelVoterId);
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

            VoteID = 0;

            if (msg.ResultType == 1)
            {
                mWaitSwitchStage = 1;
            }

            eventEmitter.Trigger(EEvents.EndVate);

            UIManager.CloseUI(EUIID.UI_Goddess_Enter);

        }

        private void OnVateUpdate(CmdRoleVoteUpdateNtf msg)
        {
            if (msg.VoteId != VoteID)
                return;

            NetMsgUtil.TryDeserialize<GoddessTrialTeamMemData>(GoddessTrialTeamMemData.Parser, msg.ClientData.ToByteArray(), out GoddessTrialTeamMemData data);

            VoteID = msg.VoteId;

            TeamMemsRecord = data;

            eventEmitter.Trigger(EEvents.VoteUpdate);
        }


        /// <summary>
        /// 成员投票请求
        /// </summary>
        /// <param name="b"></param>
        public void OnSendPlayerConfirmres(bool b)
        {
            if (VoteID == 0)
                return;

            Sys_Vote.Instance.Send_DoVoteReq(VoteID, (uint)(b ? 1 : 2));

        }

        public void OnSendExitConfirmres()
        {
            if (VoteID == 0)
                return;

            Sys_Vote.Instance.Send_CancleVoteReq(VoteID);
        }
    }
}
