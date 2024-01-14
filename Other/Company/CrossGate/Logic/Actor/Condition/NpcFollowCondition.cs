using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:NPC是否在跟随中///
    /// </summary>
    public class NpcFollowCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.NpcFollow;
            }
        }

        uint npcInfoID;

        public override void DeserializeObject(List<int> data)
        {
            npcInfoID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_NpcFollow.Instance.IsNpcFollowing(npcInfoID))
                return true;
            return false;
        }

        protected override void OnDispose()
        {
            npcInfoID = 0u;
        }
    }
}