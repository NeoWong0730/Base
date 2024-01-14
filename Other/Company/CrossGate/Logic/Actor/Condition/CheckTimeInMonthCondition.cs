using System;
using System.Collections.Generic;
using Framework;

namespace Logic
{
    public class CheckTimeInMonthCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckTimeMonth;
            }
        }

        uint month;
        uint startTime;
        uint endTime;

        public override void DeserializeObject(List<int> data)
        {
            month = (uint)data[0];
            startTime = (uint)data[1];
            endTime = (uint)data[2];
        }

        public override bool IsValid()
        {
            DateTime curTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            bool isValid = false;
            if (curTime.Month == month)
            {
                uint elapseSeconds = ((uint)curTime.Day - 1) * 24 * 3600;
                elapseSeconds += (uint)curTime.Hour * 3600;
                elapseSeconds += (uint)curTime.Minute * 60;
                elapseSeconds += (uint)curTime.Second;

                isValid = elapseSeconds >= startTime && elapseSeconds <= endTime;
            }
            return isValid;
        }

        protected override void OnDispose()
        {
            month = 0;
            startTime = 0;
            endTime = 0;
        }
    }
}
