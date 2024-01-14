using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;


namespace Logic
{
    public partial class Sys_GoddnessTrial
    {
        /// <summary>
        /// 队长发起投票反馈
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySelectStageStart(NetMsg msg)
        {
            CmdGoddessTrialSelectStageStartVoteRes info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageStartVoteRes>(CmdGoddessTrialSelectStageStartVoteRes.Parser, msg);


        }

        /// <summary>
        /// 关卡投票反馈
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySelectStageVote(NetMsg msg)
        {
            CmdGoddessTrialSelectStageVoteRes info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageVoteRes>(CmdGoddessTrialSelectStageVoteRes.Parser, msg);

            eventEmitter.Trigger(EEvents.SelectStageVote);


        }

        /// <summary>
        /// 跟随队长反馈
        /// </summary>
        private void NotifyFollowLeader(NetMsg msg)
        {
            CmdGoddessTrialVoteFollowLeaderRes info = NetMsgUtil.Deserialize<CmdGoddessTrialVoteFollowLeaderRes>(CmdGoddessTrialVoteFollowLeaderRes.Parser, msg);


            eventEmitter.Trigger(EEvents.SelectFollow);

        }

        /// <summary>
        /// 被通知开始关卡投票
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyStageStartVoteNtf(NetMsg msg)
        {
            CmdGoddessTrialSelectStageStartVoteNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageStartVoteNtf>(CmdGoddessTrialSelectStageStartVoteNtf.Parser, msg);

            if (Sys_Team.Instance.isCaptain() == false && UIManager.IsOpen(EUIID.UI_Dialogue))
            {
                UIManager.CloseUI(EUIID.UI_Dialogue);
            }


            if (UIManager.IsOpen(EUIID.UI_Goddess_Select))
            {
                eventEmitter.Trigger<uint, uint>(EEvents.SelectStartVote, info.InstanceId, info.StageId);
            }

            UIManager.OpenUI(EUIID.UI_Goddess_Select, false, new UI_Goddness_Select_Parma() { State = 1, InstanceID = info.InstanceId, LevelID = info.StageId });


           
        }

        /// <summary>
        /// 同步队长的选择，只有勾选同步了队长选择的队员才会收到同步消息
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySelestStageProcessNtf(NetMsg msg)
        {
            CmdGoddessTrialSelectStageVoteProcessNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageVoteProcessNtf>(CmdGoddessTrialSelectStageVoteProcessNtf.Parser, msg);

            eventEmitter.Trigger<uint>(EEvents.StageVoteProcess, info.StageIndex);
        }

        /// <summary>
        /// 关卡投票结束并同步投票结果
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySelectStageVoteEndNtf(NetMsg msg)
        {
            CmdGoddessTrialSelectStageVoteEndNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageVoteEndNtf>(CmdGoddessTrialSelectStageVoteEndNtf.Parser, msg);

            StateVoteResult = info;

            eventEmitter.Trigger(EEvents.StageVoteEnd);

            m_SelectVoteState = 0;
        }

        /// <summary>
        /// 关卡投票取消
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySelectStageVoteCancleNtf(NetMsg msg)
        {
            CmdGoddessTrialSelectStageCancelVoteNtf info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectStageCancelVoteNtf>(CmdGoddessTrialSelectStageCancelVoteNtf.Parser, msg);

            eventEmitter.Trigger(EEvents.StageVoteCancle);
        }

        /// <summary>
        /// 难度设置返回
        /// </summary>
        /// <param name="msg"></param>
        private void NotifySetDifficulty(NetMsg msg)
        {
            CmdGoddessTrialSelectTopicDifficultyRes info = NetMsgUtil.Deserialize<CmdGoddessTrialSelectTopicDifficultyRes>(CmdGoddessTrialSelectTopicDifficultyRes.Parser, msg);

            eventEmitter.Trigger(EEvents.SetDifficult);
        }
    }
}
