using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:队伍人数大于等于///
    /// </summary>
    public class TeamMemberCountCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TeamMemberCount;
            }
        }

        uint count;

        public override void DeserializeObject(List<int> data)
        {
            count = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Team.Instance.HaveTeam && Sys_Team.Instance.getTeamMems().Count >= count)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            count = 0;
        }
    }
}
