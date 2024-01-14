using System.Collections.Generic;

namespace Logic
{
    public class WorldLevelCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.WorldLevel;
            }
        }

        uint worldLevel;

        public override void DeserializeObject(List<int> data)
        {
            worldLevel = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.GetWorldLv() >= worldLevel)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            worldLevel = 0;
        }
    }
}
