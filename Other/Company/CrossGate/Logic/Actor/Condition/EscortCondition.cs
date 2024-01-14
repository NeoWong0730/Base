using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:NPC是否护送中///
    /// </summary>
    public class EscortCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.Escort;
            }
        }

        uint npcInfoID;

        public override void DeserializeObject(List<int> data)
        {
            npcInfoID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Escort.Instance.IsNpcEscorting(npcInfoID))
                return true;
            return false;
        }

        protected override void OnDispose()
        {
            npcInfoID = 0u;
        }
    }
}
