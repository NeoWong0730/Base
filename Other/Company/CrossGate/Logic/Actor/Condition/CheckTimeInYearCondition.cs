using System.Collections.Generic;
using UnityEngine;
using Framework;


namespace Logic
{
    public class CheckTimeInYearCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckTimeInYear;
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
            uint curSvrTime = Sys_Time.Instance.GetServerTime();
            uint start = TimeManager.ConvertFromZeroTimeZone(startTime);
            uint end = TimeManager.ConvertFromZeroTimeZone(endTime);
            bool isValid = curSvrTime >= start && curSvrTime <= end;
            return isValid;
        }

        protected override void OnDispose()
        {
            startTime = 0;
            endTime = 0;
        }
    }
}
