

using System.Collections.Generic;

namespace Logic
{
    public class CheckBlessStateCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckBlessState;
            }
        }

        uint state;

        public override void DeserializeObject(List<int> data)
        {
            state = (uint)data[0];
        }

        public override bool IsValid()
        {
        
            int result = Sys_Blessing.Instance.GetState(state);

            return result == 1;
        }

        protected override void OnDispose()
        {
            state = 0;
        }

    }
}
