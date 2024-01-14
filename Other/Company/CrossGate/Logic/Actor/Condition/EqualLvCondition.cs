using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:等于等级///
    /// </summary>
    public class EqualLvCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.EqualLv;
            }
        }

        uint level;

        public override void DeserializeObject(List<int> data)
        {
            level = (uint)data[0];
        }

        public override bool IsValid()
        {
            return Sys_Role.Instance.Role.Level == level;
        }

        protected override void OnDispose()
        {
            level = 0;
        }
    }
}
