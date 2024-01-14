using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using Packet;
using System;

namespace Logic
{
    public class UI_TeamMain : UIComponent,UI_TeamMain_Layout.IListener
    {
        UI_TeamMain_Layout m_Layout = new UI_TeamMain_Layout();

        UI_TeamMain_AutoFind m_AutoFind;
        UI_TeamMain_Mems m_Mems;
        UI_TeamMain_MenuTeamMem m_MenuTeamMem;
        UI_TeamMain_WithoutTeam m_WithoutTeam;
        private bool Active { get; set; }

       // private bool bReEvent = false;

        enum EState
        {
            None,
            NoTeam,
            WithTeam,
            AutoFind,
            TeamMember

        }

        private EState m_eState = EState.None;

        private bool isEventEnable = false;
        protected override void Loaded()
        {
            m_Layout.Loaded(gameObject.transform);

            m_AutoFind = AddComponent<UI_TeamMain_AutoFind>(gameObject.transform);

            m_Mems = AddComponent<UI_TeamMain_Mems>(gameObject.transform);

            m_MenuTeamMem = AddComponent<UI_TeamMain_MenuTeamMem>(gameObject.transform);

            m_WithoutTeam = AddComponent<UI_TeamMain_WithoutTeam>(gameObject.transform);

            m_WithoutTeam.RegisterEvents(this, true);
            m_Mems.RegisterEvents(this, true);
            m_AutoFind.RegisterEvents(this, true);

            Hide();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            if (isEventEnable == toRegister)
                return;
            
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnOpenFunc, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_InfoNtf, OnMemberInfo, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberState, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnMemberLeave, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<int, uint>(Sys_Team.EEvents.NetMsg_MemInfoUpdateNtf, OnMemberInfoUpdate, toRegister);

            Sys_Team.Instance.eventEmitter.Handle<bool>(Sys_Team.EEvents.MatchState, OnAutoApplyState, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TeamClear, OnTeamClear, toRegister);

            isEventEnable = toRegister;
        }

        private void OnOpenFunc(Sys_FunctionOpen.FunctionOpenData data)
        {
            if (data.id == 30301)
            {
                Active = true;
                ShowNoTeam();
                m_eState = EState.NoTeam;
            }
        }

        public override void Show()
        {
            ProcessEventsForEnable(true);

            if (Sys_FunctionOpen.Instance.IsOpen(30301, false) == false)
            {
                var level = DailyDataHelper.OpenLevel(30301);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, level.ToString()));

                return;
            }

            OnTeamRefresh();

            //base.Show();
        }

        public void DoRefresh()
        {
            ProcessEventsForEnable(true);

            if (Sys_FunctionOpen.Instance.IsOpen(30301, false) == false)
                return;

            HideAllUI();

            Active = true;
       
            ShowTeam();
        }


        private void OnTeamRefresh()
        {

            HideAllUI();

            if (Sys_Instance.Instance.NeedTeam() == false)
            {
                return;
            }
            ShowTeam();
            Active = true;
        }
        public void ToAgain()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(30301, false) == false)
            {
                var level = DailyDataHelper.OpenLevel(30301);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, level.ToString()));
                return;
            }

            if (Sys_Instance.Instance.NeedTeam() == false)
            {

                if (Active == false)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10713));
                }
 
                return;
            }

            UIManager.OpenUI(EUIID.UI_Team_Member,false,UI_Team_Member.EType.Team);

            HideAllUI();
        }
        public override void Hide()
        {
            ProcessEventsForEnable(false);

            HideAllUI();

            m_eState = EState.None;

            Active = false;  
        }

        public void HideWith()
        {
            ProcessEventsForEnable(false);

            HideAllUI();
        }

        private void HideAllUI()
        {
            m_AutoFind.Hide();
            m_Mems.Hide();
            m_MenuTeamMem.Hide();
            m_WithoutTeam.Hide();
        }
        #region operator ui

        /// <summary>
        /// 根据当前队伍的状态显示UI
        /// </summary>
        private void ShowTeam()
        {
            if (Sys_Team.Instance.isMatching && Sys_Team.Instance.HaveTeam == false)
            {
                ShowAutoFind();
                m_eState = EState.AutoFind;
                return;
            }
            if (Sys_Team.Instance.TeamMemsCount == 0)
            {
                if (m_eState != EState.TeamMember && Active == false)
                {
                    UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
                    m_eState = EState.TeamMember;
                }

                else
                {
                    ShowNoTeam();
                    m_eState = EState.NoTeam;
                }
                  
                return;
            }

            ShowWithTeam();
            m_eState = EState.WithTeam;
        }

        /// <summary>
        /// 无队伍信息
        /// </summary>
        private void ShowNoTeam()
        {
            m_WithoutTeam.Show();
        }

        /// <summary>
        /// 有队伍信息
        /// </summary>
        private void ShowWithTeam()
        {

            m_Mems.Show();

        }

        /// <summary>
        /// 自动匹配
        /// </summary>
        private void ShowAutoFind()
        {
            m_AutoFind.Show();
        }

        #endregion

        #region event
        public void Join()
        {
            // m_WithoutTeam.Hide();

            if (Sys_FunctionOpen.Instance.IsOpen(30302, true) == false)
                return;

            UIManager.OpenUI(EUIID.UI_Team_Fast);

            UIManager.HitButton(EUIID.UI_MainInterface, "Team - " + "Join");

        }

        public void Create()
        {
            ulong roleid = Sys_Role.Instance.Role.RoleId;

            Sys_Team.Instance.ApplyCreateTeam(roleid);

            UIManager.HitButton(EUIID.UI_MainInterface, "Team - " + "Create");

        }

        public void ExitAutoFind()
        {
            Sys_Team.Instance.ApplyMatching(1, 0);

            UIManager.HitButton(EUIID.UI_MainInterface, "Team - " + "ExitAutoFind");
        }

        public void OnClickMemItem(Vector3[] points, ulong roleID ,Action hideAc)
        {
            m_MenuTeamMem.Show(points,roleID, hideAc);

            UIManager.HitButton(EUIID.UI_MainInterface, "Team - " + "MemItem : " + roleID);
        }
        #endregion

        private void OnMemberInfo()
        {

            var laststate = m_eState;

            HideAllUI();

            ShowTeam();

            if(Sys_Team.Instance.TeamMemsCount == 1 && laststate == EState.NoTeam)
                UIManager.OpenUI(EUIID.UI_Team_Member, false, UI_Team_Member.EType.Team);
        }

        private void OnMemberState(ulong roleID)
        {
            int teamMem = Sys_Team.Instance.MemIndex(roleID);

            m_Mems.UpdateInfo(teamMem);
        }

        private void OnMemberLeave(ulong roleID)
        {
            HideAllUI();

            ShowTeam();
        }

        private void OnMemberInfoUpdate(int  teamMem,uint type)
        {
            m_Mems.UpdateInfo(teamMem);
        }

        private void OnAutoApplyState(bool state)
        {
           
            HideAllUI();

            ShowTeam();
        }

        private void OnTeamClear()
        {
            HideAllUI();

            ShowTeam();
        }
    }
}
