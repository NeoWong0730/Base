using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:职业相等///
    /// </summary>
    public class EqualCareerCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.EqualCareer;
            }
        }

        uint careerID;

        public override void DeserializeObject(List<int> data)
        {
            careerID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Role.Instance.Role.Career == careerID)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            careerID = 0;
        }
    }
}
