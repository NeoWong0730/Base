using Lib.Core;
using Logic.Core;

namespace Logic
{
    public partial class UI_WarriorGroup : UIBase, UI_WarriorGroup_Layout.IListener
    {
        UI_WarriorGroup_Layout layout = new UI_WarriorGroup_Layout();

        Timer quitColdTimer;
        Timer createMeetTimer;
        Timer fastInviteTimer;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.actionInfinityGrid.onCreateCell += ActionInfinityGridCreateCell;
            layout.actionInfinityGrid.onCellChange += ActionInfinityGridCellChange;

            layout.currentMeetingGrid.onCreateCell += CurrentMeetingGridCreateCell;
            layout.currentMeetingGrid.onCellChange += CurrentMeetingGridCellChange;

            layout.historyMeetingGrid.onCreateCell += HistoryMeetingGridCreateCell;
            layout.historyMeetingGrid.onCellChange += HistoryMeetingGridCellChange;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.AddedNewActions, OnAddedNewActions, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefreshQuitTime, OnRefreshQuitTime, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefreshCreateMeetTime, OnRefreshCreateMeetTime, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefreshFastInviteTime, OnRefreshFastInviteTime, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.QuitSuccessed, OnQuitSuccessed, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedLeader, OnRefrehedLeader, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle<Sys_WarriorGroup.MeetingInfoBase>(Sys_WarriorGroup.EEvents.AddNewDoingMeeting, OnAddNewDoingMeeting, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.DelDoingMeeting, OnDelDoingMeeting, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.AddedNewMember, OnAddedNewMember, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RevomedMember, OnRevomedMember, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedDeclaration, OnRefrehedDeclaration, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedName, OnRefrehedName, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.AddNewHistoryMeeting, OnAddNewHistoryMeeting, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.DelHistoryMeeting, OnDelHistoryMeeting, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.ReadHistoryMeetingInfo, OnReadHistoryMeetingInfo, toRegister);
        }

        #region events

        void OnAddedNewActions()
        {
            if (layout.infoRoot.activeSelf)
            {
                RefreshActions();
            }
        }

        void OnRefreshQuitTime()
        {
            if (layout.memberRoot.activeSelf)
            {
                layout.leaveButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.QuitBeforeThinkingTime == 0);
                layout.leaveCancelButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.QuitBeforeThinkingTime != 0);
            }
        }

        void OnRefreshCreateMeetTime()
        {
            if (layout.meetingRoot.activeSelf)
            {
                RefreshMeeting();
            }
        }

        void OnRefreshFastInviteTime()
        {
            if (layout.memberRoot.activeSelf)
            {
                RefreshTeamButton();
            }
        }

        void OnQuitSuccessed()
        {
            CloseSelf();
        }

        void OnRefrehedLeader()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13559, Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos[Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID].RoleName));

            if (layout.infoRoot.activeSelf)
            {
                RefreshInfo();
            }
            else if (layout.memberRoot.activeSelf)
            {
                layout.transferButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId);
                layout.inviteButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId);
            }
        }

        void OnAddNewDoingMeeting(Sys_WarriorGroup.MeetingInfoBase meetingInfoBase)
        {
            if (layout.meetingRoot.activeSelf && layout.currentMeetingRoot.activeSelf)
            {
                layout.currentMeetingGrid.ForceRefreshActiveCell();
            }
        }

        void OnDelDoingMeeting()
        {
            if (layout.meetingRoot.activeSelf && layout.currentMeetingRoot.activeSelf)
            {
                layout.currentMeetingGrid.ForceRefreshActiveCell();
                RefreshMeetingRedPoint();
            }
            RefreshToggleRedPoint();
        }

        void OnAddNewHistoryMeeting()
        {
            if (layout.meetingRoot.activeSelf && layout.historyMeetingRoot.activeSelf)
            {
                int count = Sys_WarriorGroup.Instance.MyWarriorGroup.historyMeetingInfos.Count;
                if (count > 0)
                {
                    layout.historyMeetingGrid.CellCount = count;
                    layout.historyMeetingGrid.ForceRefreshActiveCell();
                }              
            }
            RefreshMeetingRedPoint();
            RefreshToggleRedPoint();
        }

        void OnDelHistoryMeeting()
        {
            if (layout.meetingRoot.activeSelf && layout.historyMeetingRoot.activeSelf)
            {
                int count = Sys_WarriorGroup.Instance.MyWarriorGroup.historyMeetingInfos.Count;
                if (count > 0)
                {
                    layout.historyMeetingGrid.CellCount = count;
                    layout.historyMeetingGrid.ForceRefreshActiveCell();
                }
                RefreshMeetingRedPoint();
            }
            RefreshToggleRedPoint();
        }

        void OnReadHistoryMeetingInfo()
        {
            RefreshMeetingRedPoint();
            RefreshToggleRedPoint();
        }

        void OnAddedNewMember()
        {
            if (layout.infoRoot.activeSelf)
            {
                RefreshInfo();
            }
            else if (layout.memberRoot.activeSelf)
            {
                RefreshMemberActors(currentPageIndex);
            }
        }

        void OnRevomedMember()
        {
            if (layout.infoRoot.activeSelf)
            {
                RefreshInfo();
            }
            else if (layout.memberRoot.activeSelf)
            {
                RefreshMemberActors(currentPageIndex);
            }
        }

        void OnRefrehedDeclaration()
        {
            if (layout.infoRoot.activeSelf)
            {
                RefreshInfo();
            }
        }

        void OnRefrehedName()
        {
            if (layout.infoRoot.activeSelf)
            {
                RefreshInfo();
            }
            RefreshMemberActors(currentPageIndex);
        }

        #endregion

        protected override void OnShow()
        {
            layout.LoadShowScene();
            RefreshToggleRedPoint();
            layout.infoToggle.SetSelected(true, false);
            layout.infoToggle.onValueChanged.Invoke(true);
        }

        protected override void OnHide()
        {
            layout.UnLoadShowScene();

            quitColdTimer?.Cancel();
            quitColdTimer = null;

            createMeetTimer?.Cancel();
            createMeetTimer = null;

            fastInviteTimer?.Cancel();
            fastInviteTimer = null;
        }

        protected override void OnDestroy()
        {
            if (layout != null && layout.members != null)
            {
                for (int index = 0, len = layout.members.Count; index < len; index++)
                {
                    layout.members[index].Dispose();
                }
            }
        }

        void RefreshToggleRedPoint()
        {
            layout.meetingRedPoint.SetActive(Sys_WarriorGroup.Instance.HavingUnVoteDoingMeetingOrHavingUnReadHistoryMeeting());
        }

        /// <summary>
        /// 点击了关闭按钮///
        /// </summary>
        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_WarriorGroup);
        }

        public void OnInfoToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                RefreshInfo();
                RefreshActions();
            }
        }

        public void OnMemberToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                RefreshMember();
                RefreshMemberActors(0);
            }
        }

        public void OnMeetingToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                RefreshMeeting();
                layout.currentMeetingToggle.isOn = true;
                layout.currentMeetingToggle.onValueChanged.Invoke(true);
            }
        }
    }
}
