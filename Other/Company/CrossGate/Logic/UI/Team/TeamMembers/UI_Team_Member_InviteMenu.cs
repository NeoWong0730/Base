using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Lib.Core;

public partial class UI_Team_Member : UIBase,UI_Team_Member_Layout.IListener
{
    private bool m_WaitFamilyInfo = false;
    private void OnClickInviteMenu(int index)
    {
        HideInviteMenu();

        switch (index)
        {
            case 0:
                OnBtnInviteFriend();
                break;
            case 1:
                OnBtnInviteGroup();
                break;
            case 2:
                OnBtnInviteWeixin();
                break;
            case 3:
                OnBtnInviteQQ();
                break;
        }
    }

    private void ShowInviteMenu()
    {
        m_Layout.ActiveInviteMenu(true);
    }

    private void HideInviteMenu()
    {
        m_Layout.ActiveInviteMenu(false);
    }

    private void OnBtnInviteFriend()
    {
        UIManager.OpenUI(EUIID.UI_Team_Invite, false, new UI_Team_Invite.Parmas());
    }

    private Timer m_Timer;
    private void OnBtnInviteGroup()
    {
        Sys_Family.Instance.SendGuildGetMemberInfoReq();


        if (m_WaitFamilyInfo == false)
        {
            if (m_Timer != null && m_Timer.isDone == false)
                m_Timer.Cancel();

            m_Timer = Timer.Register(10, () =>
            {
                m_WaitFamilyInfo = false;
            });
        }

        m_WaitFamilyInfo = true;
    }

    private void OnBtnInviteWeixin()
    {
        UIManager.OpenUI(EUIID.UI_Team_Invite, false, new UI_Team_Invite.Parmas() { Type = 2});
    }

    private void OnBtnInviteQQ()
    {

    }

    private void OnFamilyInfo()
    {

        if (m_WaitFamilyInfo)
        {
            m_WaitFamilyInfo = false;
            UIManager.OpenUI(EUIID.UI_Team_Invite, false, new UI_Team_Invite.Parmas() { Type = 1 });
        }
       
    }
}
