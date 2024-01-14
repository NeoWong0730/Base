using System;
using System.Collections.Generic;
using Framework;

namespace Logic
{
    public class CheckTimeOnDayCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckTimeOnDay;
            }
        }

        uint startTime;
        uint endTime;

        public override void DeserializeObject(List<int> data)
        {
            startTime = (uint)data[0];
            endTime = (uint)data[1];
        }

        public override bool IsValid()
        {
            DateTime curTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            bool isValid = false;
            uint elapseSeconds = 0;
            elapseSeconds += (uint)curTime.Hour * 3600;
            elapseSeconds += (uint)curTime.Minute * 60;
            elapseSeconds += (uint)curTime.Second;

            isValid = elapseSeconds >= startTime && elapseSeconds <= endTime;
            return isValid;
        }

        protected override void OnDispose()
        {
            startTime = 0;
            endTime = 0;
        }
    }
}