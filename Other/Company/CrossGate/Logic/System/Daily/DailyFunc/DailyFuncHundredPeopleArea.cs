using System;
using System.Collections.Generic;


namespace Logic
{
    /// <summary>
    /// 百人道场
    /// </summary>
    public class DailyFuncHundredPeopleArea : DailyFunc
    {

        bool hadReward = false;
        public override void Init()
        {
            base.Init();

            Sys_HundredPeopleArea.Instance.eventEmitter.Handle(Sys_HundredPeopleArea.EEvents.OnGotDailyAward, OnGotDailyAward, true);
            Sys_HundredPeopleArea.Instance.eventEmitter.Handle(Sys_HundredPeopleArea.EEvents.DailyRewardTime, OnGotDailyAward, true);

            hadReward = HaveReward();
        }
        public override bool HaveReward()
        {
            if (isUnLock() == false  ||( DailyType == EDailyType.Limite && IsInOpenDay() == false))
                return false;


            bool result = false;

            if (Sys_HundredPeopleArea.Instance.HasGotAward() == false)
            {
               result =  Sys_HundredPeopleArea.Instance.towerInsData != null && Sys_HundredPeopleArea.Instance.towerInsData.RewardStageId > 0;
            }
           
            return result;
        }

        private void OnGotDailyAward()
        {
            bool isreward = HaveReward();

            if (isreward != hadReward)
            {
                hadReward = isreward;

                Sys_Daily.Instance.eventEmitter.Trigger<uint>(Sys_Daily.EEvents.DailyReward, Data.id);
            }
        }

        public override bool HadUsedTimes()
        {

            Sys_HundredPeopleArea.Instance.GetPassedStage(out int curstage, out int curlevel);
            Sys_HundredPeopleArea.Instance.GetMaxStage(out int maxstage, out int maxlevel);

            if (curstage >= maxstage && curlevel >= maxlevel)

                return base.HadUsedTimes();

            return false;
        }
    }
}
