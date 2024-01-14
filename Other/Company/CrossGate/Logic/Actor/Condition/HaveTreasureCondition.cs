using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:拥有宝藏///
    /// </summary>
    public class HaveTreasureCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HaveTask;
            }
        }

        public override void DeserializeObject(List<int> data)
        {
            //taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Treasure.Instance.IsHaveTreasure())
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            //taskID = 0;
        }
    }
}
