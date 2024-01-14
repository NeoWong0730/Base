using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// Boss资格挑战赛的阶段(1资格赛 2boss战)
    /// </summary>
    public class CheckBossTowerStateCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.CheckBossTowerState;
            }
        }

        uint state;

        public override void DeserializeObject(List<int> data)
        {
            state = (uint)data[0];
        }

        public override bool IsValid()
        {
            bool isCurState = false;
            if (state == 1)
                isCurState = Sys_ActivityBossTower.Instance.curBossTowerState == Packet.BossTowerState.Challenge || Sys_ActivityBossTower.Instance.curBossTowerState == Packet.BossTowerState.ChallengeOver;
            else if (state == 2)
                isCurState = Sys_ActivityBossTower.Instance.curBossTowerState == Packet.BossTowerState.Boss || Sys_ActivityBossTower.Instance.curBossTowerState == Packet.BossTowerState.BossOver;
            return isCurState;
        }

        protected override void OnDispose()
        {
            state = 0;
        }
    }
}