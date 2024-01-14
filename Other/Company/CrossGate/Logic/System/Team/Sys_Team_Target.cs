using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Table;

namespace Logic
{

    #region // 队伍目标
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        // List<string> targetCustomNames = new List<string>();

        Dictionary<uint, CmdTeamCustomInfoDataNtf.Types.CustomInfo> mtargetCunstomInfo = new Dictionary<uint, CmdTeamCustomInfoDataNtf.Types.CustomInfo>();

        public TeamTarget teamTarget
        {
            get
            {
                if (m_teamInfo == null || m_teamInfo.TeamInfo == null)
                    return null;

                return m_teamInfo.TeamInfo.Target;
            }

            set
            {
                if (m_teamInfo == null || m_teamInfo.TeamInfo == null)
                    return;

                m_teamInfo.TeamInfo.Target = value;

               
            }

        }

        public uint teamTargetID
        {
            get
            {
                if (teamTarget == null)
                    return 1;

                return teamTarget.TargetId;
            }
        }
        string m_teamDesc = string.Empty;
        public string teamDesc { get { return m_teamDesc; } set { m_teamDesc = value; } }

        //匹配状态
        bool m_isMatching = false;
        public bool isMatching { get { return m_isMatching; } set {
                if (m_isMatching == value)
                    return;
                m_isMatching = value; eventEmitter.Trigger<bool>(EEvents.MatchState, m_isMatching); } }


        public uint MatchingTarget { get; private set; }

        List<TargetTeamInfo> m_targetTeamInfos = new List<TargetTeamInfo>();

        public IList<TargetTeamInfo> MatchList { get { return m_targetTeamInfos; } }


        public string getTargetDes()
        {
            if (string.IsNullOrEmpty(teamDesc) == false)
                return teamDesc;

            if (teamTarget == null)
                return string.Empty;

            var data = CSVTeam.Instance.GetConfData(teamTarget.TargetId);

            if (data == null)
                return string.Empty;

            return LanguageHelper.GetTextContent(data.play_name);
        }
       
        //自动匹配队员
        public bool isAutoApply
        {
            get
            {
                if (teamTarget == null)
                    return false;

                return teamTarget.AutoApply;
            }

        }

        public bool isTargetAllowRobot
        {
            get {

                if (teamTarget == null)
                    return false;
                return teamTarget.AllowRobot;
            }
        }
        private void ClearTeamTarget()
        {

      
           m_targetTeamInfos.Clear();

            

            mtargetCunstomInfo.Clear();
        }

        //是否是目标
        public bool isTeamTarget(uint id)
        {
            if (teamTarget == null)
                return false;

            return teamTarget.TargetId == id;
        }

        //获取自定义目标名称
        public bool getCustomTargetName(uint targetID, out string strvalue)
        {
            strvalue = string.Empty;

            CmdTeamCustomInfoDataNtf.Types.CustomInfo value;

            bool result = mtargetCunstomInfo.TryGetValue(targetID, out value);

            if (result)
            {
                strvalue = value.CustomInfo_.ToStringUtf8();
            }
            return result;
        }
    }

    #endregion


    #region // 战斗指令
    partial class Sys_Team : SystemModuleBase<Sys_Team>
    {

        Dictionary<ulong, WarCommandTaget> TeamCommandTagetDic = new Dictionary<ulong, WarCommandTaget>();

       
        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="value0">0 队长，1 被委托队员</param>
        /// <param name="value1">指令小标</param>
        /// <param name="value2">0 敌方，1 友方</param>
        /// <returns></returns>
        public string getTeamTagString(uint value0, uint value1,uint value2)
        {
            if (TeamMemsCount == 0)
            {
               return getPlayerTagString(value1, value2);
            }
            ulong roleid = value0 == 0 ? CaptainRoleId : EntrustMemId;

            var tag = getTeamWarCommand(roleid);

            if (tag == null)
                return string.Empty;

            var list = value2 == 0 ? tag.EnemyCommands : tag.OwnCommands;

            if (list.Count <= (int)value1)
                return string.Empty;

            return list[(int)value1];
        }

        private void ClearTeamWarCommand()
        {
            foreach (var kvp in TeamCommandTagetDic)
            {
                kvp.Value.ClearWarCommand();
            }

            TeamCommandTagetDic.Clear();
        }


        private void UpdataTeamWarCommand()
        {
            ClearTeamWarCommand();

            SetTeamWarCommand(TeamInfo.TeamInfo.CommandDatas[0],CaptainRoleId);

            if(TeamInfo.TeamInfo.EntrustMemId != 0)
                SetTeamWarCommand(TeamInfo.TeamInfo.CommandDatas[1], TeamInfo.TeamInfo.EntrustMemId);
        }
        private WarCommandTaget getTeamWarCommand(ulong roleID)
        {
            WarCommandTaget taget;

           bool result = TeamCommandTagetDic.TryGetValue(roleID, out taget);

            if (result == false)
                return null;

            return taget;
        }

        private void AddTeamWarCommand(ulong roleID, WarCommandTaget taget)
        {
            TeamCommandTagetDic.Add(roleID, taget);
        }
        private void SetTeamWarCommand(TeamCommandData data, ulong roleID)
        {
            WarCommandTaget taget = getTeamWarCommand(roleID);

            if (taget == null)
            {
                taget = new WarCommandTaget();

                AddTeamWarCommand(roleID, taget);
            }

            taget.SetWarCommand(data);
        }


        public void SetTeamWarEnmyCommand(IList<TeamCommand> data, ulong roleID)
        {

            WarCommandTaget taget = getTeamWarCommand(roleID);

            if (taget == null)
                return;

            taget.SetWarEnmyCommand(data);
        }

        public void SetTeamWarOwnCommand(IList<TeamCommand> data, ulong roleID)
        {
            WarCommandTaget taget = getTeamWarCommand(roleID);

            if (taget == null)
                return;

            taget.SetWarOwnCommand(data);
        }


        public bool CanTakeWarCommandSign()
        {
            if (HaveTeam == false)
                return true;

            if (isPlayerLeave())
                return true;

            if (isCaptain())
                return true;

            if (isHadEntrust())
                return true;

            return false;
        }


    }

    #endregion

}
