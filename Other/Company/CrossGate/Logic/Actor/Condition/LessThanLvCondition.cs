using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:小于等级///
    /// </summary>
    public class LessThanLvCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.LessThanLv;
            }
        }

        uint level;

        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.Role.Level < level)
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