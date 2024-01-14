using System.Collections.Generic;

namespace Logic
{
    public class TownTaskAvaiableCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TownTaskAvaiable;
            }
        }

        public override void DeserializeObject(List<int> data)
        {
        }

        public override bool IsValid()
        {
            if (Sys_NPCFavorability.Instance.AnyNpcReachedMax)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
        }
    }
}
