using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:季节是否相符///
    /// </summary>
    public class SeasonCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.Season;
            }
        }

        uint seasonType;

        public override void DeserializeObject(List<int> data)
        {
            seasonType = (uint)data[0];
        }

        public override bool IsValid()
        {
            if ((uint)Sys_Weather.Instance.GetESeasonStage() + 1 == seasonType)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            seasonType = 0;
        }
    }
}
