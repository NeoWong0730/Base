using Packet;
using System;
using System.Collections.Generic;

using Table;

namespace Logic
{

    /// <summary>
    /// 荣耀竞技场 
    /// </summary>
    public class DailyFuncSinglepvp : DailyFunc
    {
        bool hasReward = false;

        uint fristOpenTime = 0;
        public override void Init()
        {
            base.Init();

            Sys_Pvp.Instance.eventEmitter.Handle(Sys_Pvp.EEvents.DanLvUpAward, OnHaveReward, true);
            Sys_Pvp.Instance.eventEmitter.Handle<ArenaDanInfo>(Sys_Pvp.EEvents.GetDanLvUpAward, OnGetDanLvUpAward, true);
            Sys_Pvp.Instance.eventEmitter.Handle<List<ArenaDanInfo>>(Sys_Pvp.EEvents.GetAllDanLvUpAward, OnGetAllDanLvUpAward, true);

            hasReward = HaveReward();

            var fristParma= CSVParam.Instance.GetConfData(974u);
            if (fristParma != null)
            {
                uint.TryParse(fristParma.str_value, out fristOpenTime);
            }
        }

        public override bool InitTimeWork()
        {
            bool result = base.InitTimeWork();

            if (result)
            {
                Sys_Pvp.Instance.Apply_RiseInRank();
            }

            return result;
        }
        public override bool HaveReward()
        {
            if (isUnLock() == false || IsInOpenDay() == false)
                return false;

            if (Sys_Pvp.Instance.DanLvUpAwardList == null || Sys_Pvp.Instance.DanLvUpAwardList.Count == 0)
                return false;

            bool result = false;
            int count = Sys_Pvp.Instance.DanLvUpAwardList.Count;

            for (int i = 0; i < count; i++)
            {
                var value = Sys_Pvp.Instance.DanLvUpAwardList[i];

                if (value.CanGet && value.IsGet == false)
                {
                    result = true;
                    break;
                }
            }


            return result;
        }


        private void OnHaveReward()
        {
            bool newReward = HaveReward();

            if (newReward != hasReward)
            {
                hasReward = newReward;

                Sys_Daily.Instance.eventEmitter.Trigger<uint>(Sys_Daily.EEvents.DailyReward,Data.id);
            }
           
        }

        private void OnGetDanLvUpAward(ArenaDanInfo value)
        {
            OnHaveReward();
        }

        private void OnGetAllDanLvUpAward(List<ArenaDanInfo> value)
        {
            OnHaveReward();
        }
        public override bool IsInOpenDay()
        {
            if (IsFristOpenTimePass() == false)
            {
               return false;
            }

            return base.IsInOpenDay();
        }
        public override bool IsInOpenTime()
        {
            if (IsFristOpenTimePass() == false)
            {
                return false;
            }

            return base.IsInOpenTime();
        }

        private bool IsFristOpenTimePass()
        {
            if (fristOpenTime > 0)
            {
                var nowtime = Sys_Time.Instance.GetServerTime();

                if (nowtime < fristOpenTime)
                    return false;
            }

            return true;
        }
    }
}
