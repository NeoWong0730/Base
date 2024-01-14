using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:拥有相应数量的物品///
    /// </summary>
    public class HaveItemCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HaveItem;
            }
        }

        uint itemID;
        uint count;

        public override void DeserializeObject(List<int> data)
        {
            itemID = (uint)data[0];
            count = (uint)data[1];
        }

        public override bool IsValid()
        {
            if (Sys_Bag.Instance.GetItemCount(itemID) >= count)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            itemID = 0;
            count = 0;
        }
    }
}
