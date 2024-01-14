using Logic.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{

    public class DailyFuncWorldBoss : DailyFunc
    {
        public override void Init()
        {
            base.Init();

            Sys_WorldBoss.Instance.eventEmitter.Handle(Sys_WorldBoss.EEvents.OnRewardGot, OnReward, true);


         
        }

        public override uint GetTimes()
        {
            uint result = 0;
            bool isWorldBossActivity = Sys_WorldBoss.Instance.TryGetActivityIdByDailyId(DailyID, out var actitityId);
            if (isWorldBossActivity)
            {
                if (Sys_WorldBoss.Instance.usedCount.TryGetValue(actitityId, out BossPlayMode playmode))
                {
                    if (playmode.csv.rewardLimit != 0)
                    {
                        result = playmode.weeklyUsedCount;
                    }
                    else
                    {
                        result = playmode.dailyUsedCount;
                    }
                }
            }

            return result;
        }

        public override bool OnJoin()
        {
            if (Sys_WorldBoss.Instance.TryGetActivityIdByDailyId(Data.id, out uint activityId))
            {
                UIManager.OpenUI(EUIID.UI_WorldBossCampChallenge, false, activityId);

                return base.OnJoin();
            }
            return false;
        }

        public override bool HaveReward()
        {
            if (isUnLock() == false || IsInOpenDay() == false)
                return false;

            return Sys_WorldBoss.Instance.HasRewardUngot;
        }

        private void OnReward()
        {
            Sys_Daily.Instance.eventEmitter.Trigger<uint>(Sys_Daily.EEvents.DailyReward, Data.id);
        }
    }
}
