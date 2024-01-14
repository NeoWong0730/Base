using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 勇者团系统_被邀请信息///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        /// <summary>
        /// 被邀请信息///
        /// </summary>
        public class InvitedInfo
        {
            /// <summary>
            /// 邀请者Id///
            /// </summary>
            public ulong RoleID
            {
                get;
                set;
            }

            /// <summary>
            /// 邀请者昵称///
            /// </summary>
            public string RoleName
            {
                get
                {
                    if (WarriorGroup == null)
                        return string.Empty;

                    if (WarriorGroup.warriorInfos == null || WarriorGroup.warriorInfos.Count == 0)
                        return string.Empty;

                    if (WarriorGroup.warriorInfos.ContainsKey(RoleID))
                        return WarriorGroup.warriorInfos[RoleID].RoleName;
                    else
                        return string.Empty;
                }
                
            }

            public WarriorGroup WarriorGroup
            {
                get;
                set;
            }

            /// <summary>
            /// 邀请到期时间戳///
            /// </summary>
            public uint Time
            {
                get;
                set;
            }
        }
    }
}
