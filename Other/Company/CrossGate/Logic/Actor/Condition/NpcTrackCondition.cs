using System.Collections.Generic;

namespace Logic
{
    public class NpcTrackCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.NpcTrack;
            }
        }

        uint npcInfoID;

        public override void DeserializeObject(List<int> data)
        {
            npcInfoID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Track.Instance.IsNpcTracking(npcInfoID))
                return true;
            return false;
        }

        protected override void OnDispose()
        {
            npcInfoID = 0u;
        }
    }
}
