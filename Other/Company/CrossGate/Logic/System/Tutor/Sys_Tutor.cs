using Lib.Core;
using Logic.Core;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_Tutor : SystemModuleBase<Sys_Tutor>
    {
        private Timer coolTimer;
        private bool timeIsOver;
        #region 系统函数
        public override void Init()
        {
            Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, true);
            timeIsOver = true;
        }
        public override void OnLogin()
        {

        }
        public override void OnLogout()
        {

        }
        public override void Dispose()
        {
            base.Dispose();
            coolTimer?.Cancel();
            timeIsOver = true;
        }
        #endregion
        /// <summary>
        /// 进入战斗
        /// </summary>
        /// <param name="obj"></param>
        private void OnEnterBattle(CSVBattleType.Data cSVBattleTypeTb)
        {
            if (Sys_Team.Instance.HaveTeam)
            {
                //本场战斗是否有援助值
                if (cSVBattleTypeTb.AidBasePoint != null && cSVBattleTypeTb.AidBasePoint.Count > 0)
                {
                    if (cSVBattleTypeTb.AidBasePoint[1] > 0)
                    {
                        if (Sys_Team.Instance.isCaptain())//自己是否为队长
                        {
                            if (IsCanShowTutorGratitude())
                            {
                                if (timeIsOver)
                                {
                                    timeIsOver = false;
                                    UIManager.OpenUI(EUIID.UI_TutorGratitude);
                                    TimerCooling();
                                }
                            }
                        }
                        List<TeamMem> tutorRoleList = GetRoleOfTotor();
                        if (tutorRoleList.Count > 0)
                        {
                            System.Text.StringBuilder str = new System.Text.StringBuilder();
                            for (int i = 0; i < tutorRoleList.Count; i++)
                            {
                                str.Append(tutorRoleList[i].Name.ToStringUtf8());
                                if (i < tutorRoleList.Count - 1)
                                {
                                    str.Append("、");
                                }
                            }
                            Sys_Chat.Instance.PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(8115, str.ToString()));
                        }
                    }
                }
            }
        }
        private void OnEndBattle(CmdBattleEndNtf obj)
        {
            if (UIManager.IsVisibleAndOpen(EUIID.UI_TutorGratitude))
            {
                UIManager.CloseUI(EUIID.UI_TutorGratitude);
            }
        }
        /// <summary>
        /// 检查自己是弹出答谢弹窗
        /// </summary>
        private bool IsCanShowTutorGratitude()
        {
            bool isCan = true;
            List<TeamMem> tutorRoleList = GetRoleOfTotor();
            //队伍中有导师
            if (tutorRoleList.Count > 0)
            {
                for (int i = 0; i < tutorRoleList.Count; i++)
                {
                    //自己为导师
                    if (tutorRoleList[i].MemId == Sys_Role.Instance.RoleId)
                    {
                        isCan = false;
                        break;
                    }
                }
            }
            else//队伍中没有导师
            {
                isCan = false;
            }
            return isCan;
        }
        /// <summary>
        /// 获取队伍中的所有导师
        /// </summary>
        /// <returns></returns>
        public List<TeamMem> GetRoleOfTotor()
        {
            List<TeamMem> tutorRoleList = new List<TeamMem>();
            for (int i = 0; i < Sys_Team.Instance.teamMems.Count; i++)
            {
                TeamMem teamMem = Sys_Team.Instance.teamMems[i];
                if (!teamMem.IsRob() && teamMem.Level - Sys_Team.Instance.GetTeamMemMinLv() >= uint.Parse(CSVParam.Instance.GetConfData(1275).str_value))
                {
                    tutorRoleList.Add(Sys_Team.Instance.teamMems[i]);
                }
            }
            return tutorRoleList;
        }
        private void TimerCooling()
        {
            coolTimer = Timer.Register(float.Parse(CSVParam.Instance.GetConfData(1093).str_value) , () => {
                timeIsOver = true;
                coolTimer?.Cancel();
            }, null, false, true);
        }
    }
}