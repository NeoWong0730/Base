using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 勇者团系统_勇士信息///
    /// </summary>
    public partial class Sys_WarriorGroup : SystemModuleBase<Sys_WarriorGroup>
    {
        /// <summary>
        /// 勇士信息///
        /// </summary>
        public class WarriorInfo
        {
            /// <summary>
            /// 所属勇士团名///
            /// </summary>
            public string GroupName
            {
                get;
                set;
            }

            /// <summary>
            /// 是否团长///
            /// </summary>
            public bool IsLeader
            {
                get;
                set;
            }

            /// <summary>
            /// 角色ID///
            /// </summary>
            public ulong RoleID
            {
                get;
                set;
            }

            /// <summary>
            /// 角色名///
            /// </summary>
            public string RoleName
            {
                get;
                set;
            }

            /// <summary>
            /// 角色头像ID///
            /// </summary>
            public uint IconID
            {
                get;
                set;
            }

            /// <summary>
            /// 角色头像框ID///
            /// </summary>
            public uint FrameID
            {
                get;
                set;
            }

            /// <summary>
            /// 角色表ID///
            /// </summary>
            public uint HeroID
            {
                get;
                set;
            }

            /// <summary>
            /// 角色武器ID///
            /// </summary>
            public uint WeaponID
            {
                get;
                set;
            }

            public uint DressID
            {
                get;
                set;
            }

            /// <summary>
            /// 角色染色数据///
            /// </summary>
            public Dictionary<uint, List<dressData>> DressData
            {
                get;
                set;
            }

            /// <summary>
            /// 角色等级///
            /// </summary>
            public uint Level
            {
                get;
                set;
            }

            /// <summary>
            /// 角色职业ID///
            /// </summary>
            public uint Occ
            {
                get;
                set;
            }

            /// <summary>
            /// 职业等级///
            /// </summary>
            public uint OccRank
            {
                get;
                set;
            }

            /// <summary>
            /// 称号///
            /// </summary>
            public uint TitleID
            {
                get;
                set;
            }

            /// <summary>
            /// 加入时间///
            /// </summary>
            public uint JoinTime
            {
                get;
                set;
            }

            /// <summary>
            /// 上次在线时间///
            /// </summary>
            public uint LastOffline
            {
                get;
                set;
            }
        }
    }
}