using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;

namespace Logic
{
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        private CmdTeamInfoNtf m_teamInfo = new CmdTeamInfoNtf() { TeamInfo = new OneTeam() };

        // private List<TeamMem> m_teamMems = new List<TeamMem>(5);

        // include captain
        public int TeamMemsCount
        {
            get
            {
                //这样写能节省一半的调度时间 teamMems还是有消耗 最好能缓存
                RepeatedField<TeamMem> tmpTeamMems = teamMems;
                if (tmpTeamMems == null)
                    return 0;
                return tmpTeamMems.Count;
            }
        }

        // 不包含 暂离/离队 的人员个数
        public int TeamMemsCountOfActive {
            get {
                int count = teamMems.Count;
                for (int i = 0, length = count; i < length; ++i) {
                    if (teamMems[i].IsLeave() || teamMems[i].IsOffLine()) {
                        --count;
                    }
                }
                return count;
            }
        }

        public bool HaveTeam { get { return TeamMemsCount > 0; } }
        public RepeatedField<TeamMem> teamMems { get { return TeamInfo.TeamInfo.Mems; } }
        public RepeatedField<TeamMem> teamMemsOfActive {
            get {
                RepeatedField<TeamMem> ls = new RepeatedField<TeamMem>();
                for (int i = 0, length = TeamInfo.TeamInfo.Mems.Count; i < length; ++i) {
                    var tmp = TeamInfo.TeamInfo.Mems[i];
                    if (!tmp.IsLeave() && !tmp.IsOffLine()) {
                        ls.Add(tmp);
                    }
                };
                return ls;
            }
        }
        public RepeatedField<TeamMem> teamMemsOfUnActive
        {
            get
            {
                RepeatedField<TeamMem> ls = new RepeatedField<TeamMem>();
                for (int i = 0, length = TeamInfo.TeamInfo.Mems.Count; i < length; ++i)
                {
                    var tmp = TeamInfo.TeamInfo.Mems[i];
                    if (tmp.IsLeave() || tmp.IsOffLine())
                    {
                        ls.Add(tmp);
                    }
                };
                return ls;
            }
        }


        public ulong teamID { get { return TeamInfo.TeamInfo.TeamId; } }


        public bool CanBuildTeam()
        {
            if (CSVParam.Instance == null)
                return false;

            var data = CSVParam.Instance.GetConfData(519);

            if (data == null)
                return false;

            int level = int.Parse(data.str_value);

            return Sys_Role.Instance.Role.Level >= level;
        }

        // 组队模式下，能否手动操作其他一些独立系统，比如任务
        public bool canManualOperate
        {
            get
            {
                return (HaveTeam && (isCaptain() || isPlayerLeave())) || (!HaveTeam);
            }
        }

        /// <summary>
        /// 设置队伍信息
        /// </summary>
        public CmdTeamInfoNtf TeamInfo
        {
            get { return m_teamInfo; }
            set
            {
                m_teamInfo = value; /*SetTeamMems(m_teamInfo.TeamInfo.Mems); */
                UpdataTeamWarCommand();
            }
        }


        /// <summary>
        /// 返回所有队员
        /// </summary>
        /// <returns></returns>
        public List<TeamMem> getTeamMems()
        {
            List<TeamMem> teams = new List<TeamMem>();
            foreach (TeamMem team in teamMems)
            {
                teams.Add(team);
            }
            return teams;
        }

        public TeamMem getTeamMem(int index)
        {
            if (index < 0 || index >= TeamMemsCount)
                return null;

            return teamMems[index];
        }

        public int MemIndex(ulong roleID)
        {
            for (int i = 0; i < TeamMemsCount; i++)
            {
                if (TeamInfo.TeamInfo.Mems[i].MemId == roleID)
                    return i;
            }

            return -1;
        }
        public TeamMem getTeamMem(ulong roleID)
        {

            for (int i = 0; i < TeamMemsCount; i++)
            {
                if (TeamInfo.TeamInfo.Mems[i].MemId == roleID)
                    return TeamInfo.TeamInfo.Mems[i];
            }
            return null;
        }

        /// <summary>
        /// 队员是否暂离
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool isLeave(ulong roleID)
        {
            TeamMem mem = getTeamMem(roleID);

            if (mem == null)
                return false;

            return mem.IsLeave();
        }

        /// <summary>
        /// 当前玩家 是否暂离
        /// </summary>
        /// <returns></returns>
        public bool isPlayerLeave()
        {
            return isLeave(Sys_Role.Instance.RoleId);
        }
        public bool removeTeamMem(ulong roleID)
        {

            TeamMem teamMem = getTeamMem(roleID);
            if (teamMem == null)
                return false;

            return teamMems.Remove(teamMem);

        }


        public bool AddTeamMem(TeamMem teamMem)
        {

            TeamMem teamMem0 = getTeamMem(teamMem.MemId);
            if (teamMem0 != null)
                return false;

            teamMems.Add(teamMem);

            return true;
        }
        /// <summary>
        /// 返回是否是队长
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public bool isCaptain(ulong roleId)
        {
            if (TeamMemsCount == 0)
                return false;

            return teamMems[0].MemId == roleId;
        }

        public bool isCaptain()
        {
            if (TeamMemsCount == 0)
                return false;

            return teamMems[0].MemId == Sys_Role.Instance.RoleId;
        }
        /// <summary>
        /// 得到队伍最小等级
        /// </summary>
        /// <returns></returns>
        public uint GetTeamMemMinLv()
        {
            if (TeamMemsCount == 0)
                return Sys_Role.Instance.Role.Level;

            uint lv = uint.MaxValue;

            for (int i = 0; i < teamMems.Count; i++)
            {
                if (lv > teamMems[i].Level)
                    lv = teamMems[i].Level;
            }
            return lv;
        }
        /// <summary>
        /// 玩家是否被委托指挥
        /// </summary>
        /// <returns></returns>
        public bool isHadEntrust()
        {
            ulong id = Sys_Role.Instance.RoleId;

            return TeamInfo.TeamInfo.EntrustMemId == id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isHadEntrust(ulong roleid)
        {
            ulong id = Sys_Role.Instance.RoleId;

            return TeamInfo.TeamInfo.EntrustMemId == roleid;
        }
        public bool NeedFollowCaptain(ulong roleId)
        {
            if (isCaptain(roleId))
                return false;

            TeamMem teamMem = getTeamMem(roleId);

            if (teamMem == null)
                return false;

            if (teamMem.IsLeave())
                return false;

            if (Captain.IsLeave())
                return false;

            return true;
        }
        /// <summary>
        /// 队长
        /// </summary>
        public TeamMem Captain
        {
            get
            {
                if (TeamMemsCount == 0)
                    return null;

                return teamMems[0];
            }
        }

        public ulong CaptainRoleId
        {
            get
            {
                if (Captain == null)
                    return 0;

                return Captain.MemId;
            }
        }

        public ulong EntrustMemId
        {
            get
            {

                return TeamInfo.TeamInfo.EntrustMemId;
            }
        }

        public bool isTeamMem(ulong roleId)
        {
            foreach (TeamMem team in teamMems)
            {
                if (team.MemId == roleId)
                    return true;
            }
            return false;
        }

        private void ClearTeam()
        {
            ClearFollow();

            teamMems.Clear();

            isMatching = false;

            TeamInfo.TeamInfo.TeamId = 0;

            TeamInfo.TeamInfo.Applys.Clear();
            eventEmitter.Trigger(EEvents.NetMsg_ApplyListOpRes);
            // TeamInfo.TeamInfo.Target.TargetId = 0;

            ClearTeamTarget();

            ClearTeamWarCommand();

        }
        public int ApplyRolesCount { get { return TeamInfo.TeamInfo.Applys.Count; } }

        public RepeatedField<ApplyRole> ApplyRoles { get { return TeamInfo.TeamInfo.Applys; } }
        public IEnumerable<ApplyRole> getApplyRoles()
        {
            foreach (ApplyRole applyRole in ApplyRoles)
            {
                yield return applyRole;
            }
        }

        public ApplyRole getAtApplyRole(int index)
        {
            if (ApplyRolesCount <= index)
                return null;

            return ApplyRoles[index];
        }

        /// <summary>
        /// 设置队伍成员。清空原有的队伍列表
        /// </summary>
        private void SetTeamMems(RepeatedField<TeamMem> mems)
        {
            teamMems.Clear();

            teamMems.AddRange(mems);
        }

        private int SetTeamMem(TeamMem teamMem, uint type)
        {
            int memIndex = MemIndex(teamMem.MemId);

            if (memIndex < 0)
                return memIndex;

            var mem = getTeamMem(memIndex);

            if (type == 0)
                teamMems[memIndex] = teamMem;
            else if (type == 6)
                mem.Career = teamMem.Career;
            else if (type == 10)
                mem.Level = teamMem.Level;
            else if (type == 11)
            {
                mem.FashionList.Clear();
                mem.FashionList.AddRange(teamMem.FashionList);
            }
            else if (type == 12)
            {
                mem.Title = teamMem.Title;
            }
            else if (type == 13)
            {
                mem.HpPer = teamMem.HpPer;
                mem.MpPer = teamMem.MpPer;
            }
            else if (type == 14)
            {
                mem.CareerRank = teamMem.CareerRank;
            }
            else if (type == 15)
            {
                mem.Photo = teamMem.Photo;
                mem.PhotoFrame = teamMem.PhotoFrame;
                mem.TeamLogo = teamMem.TeamLogo;
            }
            else if (type == 19)
            {
                mem.GuildName = teamMem.GuildName;
                mem.GuildPos = teamMem.GuildPos;
            }
            else if (type == 21)
            {
                mem.WeaponItemID = teamMem.WeaponItemID;
            }
            else if (type == (uint)TeamMemInfoUpdateType.MemInfoUpdateName)
            {
                mem.Name = teamMem.Name;
            }
            else if (type == (uint)TeamMemInfoUpdateType.MemInfoUpdateShapeShiftCardId)
            {
                mem.ShapeShiftCardId = teamMem.ShapeShiftCardId;
            }
            return memIndex;
        }

        public bool isFollowed(ulong roleID)
        {
            TeamMem teamMem = getTeamMem(roleID);
            if (teamMem == null)
                return false;

            if (teamMem.IsLeave())
                return false;

            if (CaptainRoleId == roleID)
                return false;

            return true;
        }

        public bool isMainHeroFollowed()
        {
            return isFollowed(Sys_Role.Instance.RoleId);
        }

        readonly int MaxCount = 5;
        public bool isFull()
        {
            if (TeamMemsCount < MaxCount)
                return false;

            return true;
        }
        public int GetPlayerTeamState()
        {
            if (HaveTeam == false)
                return 0;

            var isLeave = isPlayerLeave();

            var iscaptain = isCaptain();

            if (iscaptain && isLeave == false)
                return 1;

            if (!iscaptain && isLeave)
                return 2;

            if (!iscaptain && !isLeave)
                return 3;

            return -1;

        }

        private bool HaveRob()
        {
            int count = teamMems.Count;

            for (int i = 0; i < count; i++)
            {
                if (teamMems[i].IsRob())
                    return true;
            }

            return false;
        }

        public bool HaveLeaveOrOfflineMem()
        {
            int count = teamMems.Count;

            for (int i = 0; i < count; i++)
            {
                if (teamMems[i].IsLeave() || teamMems[i].IsOffLine())
                    return true;
            }

            return false;
        }
        #region 时装

        public IList<MapRoleFashionInfo> getFashion(int index)
        {
            if (index >= TeamMemsCount)
                return null;

            var mem = getTeamMem(index);

            return mem.FashionList;
        }

        public RepeatedField<MapRoleFashionInfo> getFashion(ulong roleID)
        {
            var mem = getTeamMem(roleID);

            if (mem == null)
                return null;

            return mem.FashionList;
        }
        #endregion


        private void OnTouchUp(Vector2 pos)
        {
            if (HaveTeam == false || Sys_Fight.Instance.IsFight())
                return;

            if (isCaptain() == false && Sys_Team.Instance.isMainHeroFollowed() && isPlayerLeave() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11890));
            }
        }

        //private void OnLeftJoystick(Vector2 dir, float value)
        //{
        //    if (Sys_Input.Instance.CurLeftJoystickData.state != Framework.JoystickState.Start)
        //        return;

        //    if (HaveTeam == false)
        //        return;

        //    if (isCaptain() == false && GameCenter.mPlayerController != null && GameCenter.mPlayerController.bFollowOther && isPlayerLeave() == false)
        //    {
        //        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11890));
        //    }
        //}

    }


    static class TeamInfoHelper
    {
        public static TeamMem GetTeamMem(CmdTeamInfoNtf info, ulong roleId)
        {
            if (info == null || info.TeamInfo == null || info.TeamInfo.Mems.Count == 0)
                return null;

            int count = info.TeamInfo.Mems.Count;

            for (int i = 0; i < count; i++)
            {
                if (info.TeamInfo.Mems[i].MemId == roleId)
                {
                    return info.TeamInfo.Mems[i];
                }
            }

            return null;
        }
        public static bool IsLeave(CmdTeamInfoNtf info, ulong roleId)
        {
            if (info == null || info.TeamInfo == null || info.TeamInfo.Mems.Count == 0)
                return false;

            var teamMem = GetTeamMem(info, roleId);

            if (teamMem == null)
                return false;

            return teamMem.IsLeave();
        }


        public static bool IsCapton(CmdTeamInfoNtf info, ulong roleId)
        {
            if (info == null || info.TeamInfo == null || info.TeamInfo.Mems.Count == 0)
                return false;

            var teamMem = info.TeamInfo.Mems[0];

            if (teamMem == null)
                return false;

            return teamMem.MemId == roleId;
        }

        public static bool IsEmpty(CmdTeamInfoNtf info)
        {
            if (info == null || info.TeamInfo == null || info.TeamInfo.TeamId == 0|| info.TeamInfo.Mems.Count == 0)
                return true;


            return false;
        }
    }
        
}
