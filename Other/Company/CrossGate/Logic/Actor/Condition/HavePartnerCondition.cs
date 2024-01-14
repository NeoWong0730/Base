using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:拥有伙伴///
    /// </summary>
    public class HavePartnerCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HavePartner;
            }
        }

        uint partnerID;

        public override void DeserializeObject(List<int> data)
        {
            partnerID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Partner.Instance.IsUnLock(partnerID))
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            partnerID = 0;
        }
    }
}
