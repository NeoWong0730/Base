using System.Collections.Generic;
using Packet;
using UnityEngine;

namespace Logic
{
    public class HeroBaseComponent : Logic.Core.Component
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public uint HeroID
        {
            get;
            set;
        }

        /// <summary>
        /// 角色Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        public uint TitleId
        {
            get;
            set;
        }

        public string FamilyName
        {
            get;
            set;
        }

        public uint Pos
        {
            get;
            set;
        }

        public string bGroupName
        {
            get;
            set;
        }

        public uint bGPos
        {
            get;
            set;
        }

        /// <summary>
        /// 等级
        /// </summary>
        public uint Level { get; set; }
        /// <summary>
        /// 时装信息
        /// </summary>
        public Dictionary<uint, List<dressData>> fashData;

        public ulong TeamID
        {
            get;
            set;
        }

        public bool IsCaptain
        {
            get;
            set;
        }

        public uint TeamMemNum
        {
            get;
            set;
        }

        public uint HeadId
        {
            get;
            set;
        }

        public uint HeadFrameId
        {
            get;
            set;
        }

        public Vector3 Scale
        {
            get;
            set;
        }

        public uint TeamLogeId
        {
            get;
            set;
        }

        // 是否在战斗
        public bool bInFight
        {
            get;
            set;
        }
        public bool bIsReturn
        {
            get;
            set;
        }
    }
}
