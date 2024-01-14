using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件：有调查过有好感度的NPC///
    /// </summary>
    public class HasInquiryedFavorabilityNpcCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HasInquiryedFavorabilityNpc;
            }
        }

        public override bool IsValid()
        {
            if (Sys_NPCFavorability.Instance.AnyNpcUnlocked)
            {
                return true;
            }
            return false;
        }

        public override void DeserializeObject(List<int> data)
        {
            
        }

        protected override void OnDispose()
        {
            
        }
    }

    /// <summary>
    /// 条件：是否激活NPC///
    /// </summary>
    public class ActivedNPCCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.ActivedNPC;
            }
        }

        uint npcInfoID;

        public override void DeserializeObject(List<int> data)
        {
            npcInfoID = (uint)data[0];
        }

        protected override void OnDispose()
        {
            npcInfoID = 0;
        }

        public override bool IsValid()
        {
            if (Sys_Npc.Instance.IsActivatedNpc(npcInfoID))
            {
                return true;
            }
            return false;
        }
    }
}
