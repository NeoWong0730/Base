using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:时间是否相符(白天晚上)///
    /// </summary>
    public class TimeCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.Time;
            }
        }

        uint TimeType;

        public override void DeserializeObject(List<int> data)
        {
            TimeType = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (TimeType == 1)
            {
                if (Sys_Weather.Instance.isDay)
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (Sys_Weather.Instance.isDay)
                {
                    return false;
                }
                return true;
            }
        }

        protected override void OnDispose()
        {
            TimeType = 0;
        }
    }
}
