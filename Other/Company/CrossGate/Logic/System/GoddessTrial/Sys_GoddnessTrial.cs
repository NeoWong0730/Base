using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public partial class Sys_GoddnessTrial:SystemModuleBase<Sys_GoddnessTrial>
    {
        public readonly uint PLAYTYPE = 90;
        public enum EEvents
        {
            FristCrossInfo,//首通信息
            StartVote,
            DoVate,
            EndVate,
            VoteUpdate,
            TeamMemsRecord,//队伍信息
            RefrshTopicLevel,//主题刷新
            RefrshData,//副本数据
            SelectStartVote,//发起选择关卡投票
            SelectStageVote,//关卡投票
            SelectFollow,//跟随选择
            StageVoteProcess,//同步队长投票
            StageVoteEnd,//关卡投票结束
            StageVoteCancle,//关卡投票取消
            SetDifficult,//设置主题难度反馈
            TopicRank,
            TopicDifficult,//主题难度
            TopicProperty,//本周特性
            TopicUnlock,
            GetTopicEndAward,
            

        }

        public  EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public override void Init()
        {
            InitNet();

            InitConfig();
        }

        public override void OnLogin()
        {
            base.OnLogin();
        }

        public override void OnLogout()
        {
            base.OnLogout();

            isInInstance = false;

            m_IsWaitShowMainUI = false;

            UpdateInsAwardNtf = null;
            //mWaitSwitchStage = 0;
        }


        public uint GetDailyTimes()
        {
            uint result = 0;

            if (GTData == null || GTData.Awards == null)
                return result;


            int count = GTData.Awards.Count;

            for (int i = 0; i < count; i++)
            {
                if (GTData.Awards[i] == 1)
                    result += 1u;
            }

            if (UpdateInsAwardNtf != null)
            {
                if (UpdateInsAwardNtf.AwardCount > result)
                {
                    result = UpdateInsAwardNtf.AwardCount;
                }
            }
            return result;
        }
    }
}
