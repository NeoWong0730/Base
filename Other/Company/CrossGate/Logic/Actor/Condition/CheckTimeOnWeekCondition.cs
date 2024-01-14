using System;
using System.Collections.Generic;
using Framework;

namespace Logic
{
    public class CheckTimeOnWeekCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckTimeOnWeek;
            }
        }
        uint startWeek;
        uint endWeek;
        uint startTime;
        uint endTime;
        public override void DeserializeObject(List<int> data)
        {
            startWeek = (uint)data[0];
            startTime = (uint)data[1];
            endTime = (uint)data[2];
            uint dayNum = endTime / 86400;
            endWeek = startWeek + dayNum;
            //endWeek = endTime / 86400 + 1;
        }

        public override bool IsValid()
        {
            DateTime curTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            bool isValid = false;
            uint curDayOfWeek = curTime.DayOfWeek != DayOfWeek.Sunday ? (uint)curTime.DayOfWeek : 7;
            if (startWeek <= curDayOfWeek && curDayOfWeek <= endWeek)
            {
                //uint elapseSeconds = (curDayOfWeek - 1) * 86400;
                uint elapseSeconds = (curDayOfWeek - startWeek) * 86400;
                elapseSeconds += (uint)curTime.Hour * 3600;
                elapseSeconds += (uint)curTime.Minute * 60;
                elapseSeconds += (uint)curTime.Second;

                isValid = elapseSeconds >= startTime && elapseSeconds <= endTime;
            }
            return isValid;
        }

        protected override void OnDispose()
        {
            startWeek = 0;
            endWeek = 0;
            startTime = 0;
            endTime = 0;
        }
    }
}