using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 勇者团系统_会议条件///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        /// <summary>
        /// 会议条件基类///
        /// </summary>
        public abstract class MeetingConditionBase
        {
            /// <summary>
            /// 会议条件类型///
            /// </summary>
            public enum MeetingConditionType
            {
                None = 0,
                LeaderOfflineTime = 101,  //团长离线时间（天）大于等于
                MembersCount = 201,       //团队成员数量大于等于
                AvailableCount = 202,     //团队剩余成员位置大于等于
                Recruit = 301,           //招募亲密度等级在线
            }

            public abstract bool IsValid();
        }

        /// <summary>
        /// 团长离线时间（天）大于等于///
        /// </summary>
        public class MeetingCondition_LeaderOfflineTime : MeetingConditionBase
        {
            uint offLineTime;
            uint limitTime;

            public MeetingCondition_LeaderOfflineTime(uint _offLineTime)
            {
                offLineTime = _offLineTime;
                limitTime = 24 * 3600 * CSVBraveTeamMeetingCondition.Instance.GetConfData((uint)MeetingConditionType.LeaderOfflineTime).value;
            }

            public override bool IsValid()
            {
                if (offLineTime == 0)
                    return false;

                if (Sys_Time.Instance.GetServerTime() - offLineTime > limitTime)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 团队成员数量大于等于///
        /// </summary>
        public class MeetingCondition_MembersCount : MeetingConditionBase
        {
            uint count;
            uint countNeed;

            public MeetingCondition_MembersCount(uint _count)
            {
                count = _count;
                countNeed = CSVBraveTeamMeetingCondition.Instance.GetConfData((uint)MeetingConditionType.MembersCount).value;
            }

            public override bool IsValid()
            {
                if (count >= countNeed)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 团队剩余成员位置大于等于///
        /// </summary>
        public class MeetingCondition_AvailableCount : MeetingConditionBase
        {
            uint count;
            uint countLimit;

            public MeetingCondition_AvailableCount(uint _count)
            {
                count = _count;
                countLimit = CSVBraveTeamMeetingCondition.Instance.GetConfData((uint)MeetingConditionType.AvailableCount).value;
            }

            public override bool IsValid()
            {
                if (count >= countLimit)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// 招募亲密度等级在线///
        /// </summary>
        public class MeetingCondition_Recruit : MeetingConditionBase
        {
            uint friendValue;
            uint level;

            public MeetingCondition_Recruit(uint level)
            {
                friendValue = CSVBraveTeamMeetingCondition.Instance.GetConfData((uint)MeetingConditionType.Recruit).value;
            }

            public override bool IsValid()
            {
                foreach (var role in Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.Values)
                {
                    if (Sys_Society.Instance.socialRolesInfo.rolesDic.ContainsKey(role.infoID))
                    {
                        if (Sys_Society.Instance.socialRolesInfo.rolesDic[role.infoID].friendValue >= friendValue &&
                            Sys_Society.Instance.socialRolesInfo.rolesDic[role.infoID].level >= level &&
                            Sys_Society.Instance.socialRolesInfo.rolesDic[role.infoID].isOnLine)
                            return true;
                    }
                }

                return false;
            }
        }       
    }
}
