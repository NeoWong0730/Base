using System.Collections.Generic;

namespace Logic
{
    public class GreaterOrEqualLvCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.GreaterOrEqualLv;
            }
        }

        uint level;

        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.Role.Level >= level)
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

    /// <summary>
    /// 条件:大于等级///
    /// </summary>
    public class GreaterThanLvCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.GreaterThanLv;
            }
        }

        uint level;

        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.Role.Level > level)
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
