using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Lib.Core;

public partial class UI_Team_Member : UIBase, UI_Team_Member_Layout.IListener
{
    UI_Team_Member_Layout m_Layout = new UI_Team_Member_Layout();

    Timer m_UpdateShowScenePosTimer;
    public enum EType
    {
        None,
        Team,
        Apply
    }

    private EType m_eType = EType.None;
    protected override void OnLoaded()
    {
        m_Layout.Loaded(gameObject.transform);

        m_Layout.OnClickInviteMenu = OnClickInviteMenu;

        m_Layout.OnClickItemMenu.AddListener(OnClickItemMenu);   

        m_Layout.OnClickInfo.AddListener(OnClickMemberInfo);

        m_Layout.RegisterEvents(this);
  
       

    }

    protected override void OnOpen(object arg)
    {
        if (arg != null && arg is EType)
        {
            m_eType = (EType)arg;
        }
        else if (arg != null)
        {
            uint value = (uint)arg;
            if (value < 3)
                m_eType = (EType)value;
        }

    }
    protected override void OnShow()
    {
        m_Layout.LoadShowScene();        

        if (m_eType == EType.Team)
            m_Layout.SetApplyRedState(Sys_Team.Instance.ApplyRolesCount > 0);

        m_Layout.ShowMembersInfo();

        RefreshMembersInfo();

        m_UpdateShowScenePosTimer?.Cancel();
        m_UpdateShowScenePosTimer = Timer.Register(0.2f, m_Layout.UpdataShowScenePosPosition);
    }

    protected override void ProcessEventsForEnable(bool toRegister)
    {
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_InfoNtf, OnMemberInfo, toRegister);

        Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnMemberLeave, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.NetMsg_ApplyListOpRes, OnApplyListOpResMsg, toRegister);

        Sys_Team.Instance.eventEmitter.Handle<int, uint>(Sys_Team.EEvents.NetMsg_MemInfoUpdateNtf, OnMemberInfoUpdate, toRegister);

        Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberState, toRegister);

        Sys_Team.Instance.eventEmitter.Handle<uint, uint, bool>(Sys_Team.EEvents.TargetMatching, OnMatching, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TargetUpdate, OnTargetUpdate, toRegister);

        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.EntrustNtf, RefreshEntrustState, toRegister);

        Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMember, OnFamilyInfo, toRegister);

        Sys_Team.Instance.eventEmitter.Handle<uint>(Sys_Team.EEvents.CustomTargetInfo,OnTargetCustom, toRegister);
    }

    protected override void OnHide()
    {        
        m_MembersInfoState = MemberState.None;
 
        m_Layout.HideMembersInfo();

        m_WaitFamilyInfo = false;

        m_Layout.UnLoadShowScene();

        m_UpdateShowScenePosTimer?.Cancel();
    }


    protected override void OnClose()
    {        
        m_MembersInfoState = MemberState.None;

        m_Layout.HideMembersInfo();

  

    }
    public void Close()
    {
        //Hide(EUIState.Hide,false);
        UIManager.HitButton(EUIID.UI_Team_Member, "Close");
        UIManager.CloseUI(EUIID.UI_Team_Member);
    }

    private void OnMemberInfo()
    {
        m_Layout.SetApplyRedState(Sys_Team.Instance.ApplyRolesCount > 0);

        if (m_eType != EType.Team)
            return;

        RefreshMembersInfo();
    }

    private void OnMemberLeave(ulong role)
    {
        if (m_eType != EType.Team)
            return;

        RefreshMembersInfo();
    }

    private void OnMemberInfoUpdate(int teamMem, uint type)
    {
        if (type != 11)
            RefreshMemberInfo(teamMem);

        if (type == 21 || type == 11 || type == 0)
            RefreshMemberShowActor(teamMem);

        if (type == 12 || type == 19)
            RefreshMemberTitle(teamMem);

        if (type == 15)
            RefreshMemberLogoAndPhoto(teamMem);
    }

    private void OnMemberState(ulong role)
    {
        UpdateMemberInfo(role);
    }

    private void OnMatching(uint lastid, uint id, bool ismatching)
    {
        m_Layout.SetMatch(Sys_Team.Instance.isMatching);

    }

    private void OnTargetUpdate()
    {
        RefreshTargetInfo();
    }

    private void OnTargetCustom(uint targetid)
    {
        if (targetid != Sys_Team.Instance.teamTargetID)
        {
            return;
        }

        RefreshTargetInfo();
    }

    private void OnApplyListOpResMsg()
    {
        m_Layout.SetApplyRedState(Sys_Team.Instance.ApplyRolesCount > 0);
    }
}
