using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:天气是否相符///
    /// </summary>
    public class WeatherCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.Weather;
            }
        }

        uint weatherType;

        public override void DeserializeObject(List<int> data)
        {
            weatherType = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Weather.Instance.curWeather == weatherType)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            weatherType = 0;
        }
    }
}
