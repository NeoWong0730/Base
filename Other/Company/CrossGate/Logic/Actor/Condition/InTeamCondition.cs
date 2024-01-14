using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件: 组队条件///
    /// </summary>
    public class InTeamCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.InTeam;
            }
        }

        uint param;

        public override void DeserializeObject(List<int> data)
        {
            param = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (param ==  0)
            {
                return !Sys_Team.Instance.HaveTeam;
            }
            else
            {
                return Sys_Team.Instance.HaveTeam;
            }
        }

        protected override void OnDispose()
        {
            param = 0u;
        }
    }
}
