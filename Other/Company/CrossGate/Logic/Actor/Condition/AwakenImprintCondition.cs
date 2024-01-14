using System.Collections.Generic;

namespace Logic
{
    //觉醒印记开启
    public class AwakenImprintCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.AwakenImprint;
            }
        }
        uint level;
        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_TravellerAwakening.Instance.CheckAwakenCondition())
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            level = 0;
        }
    }
}