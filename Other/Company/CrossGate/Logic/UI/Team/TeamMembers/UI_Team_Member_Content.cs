using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;

public partial class UI_Team_Member : UIBase,UI_Team_Member_Layout.IListener
{
    private int ContentIndex { get; set; }
    private void OnShowContent()
    {
        int index = 0;
        switch(m_eType)
        {
            case EType.Team:
                index = 0;
                break;
            case EType.Apply:
                index = 1;
                break;
        }
        OnClickContent(true, index);
        m_Layout.SetFocusContent(index);

       // m_Layout.SetApplyRedState(Sys_Team.Instance.ApplyRolesCount > 0);

    }

    private void OnHideContent()
    {
        ContentIndex = -1;

        m_Layout.CloseContent();
    }

    private void OnClickContent(bool state, int index)
    {
        if (state && index == ContentIndex)
            return;

        if(index == 0)
        {
            ToTeam(state);
        }

        //if (index == 1)
        //{
        //    ToApply(state);
        //}
        if (state)
            ContentIndex = index;
    }

    private void ToTeam(bool b)
    {
        if (!b)
            m_Layout.HideMembersInfo();
        else
        {
            m_Layout.ShowMembersInfo();
            RefreshMembersInfo();
        }
    }

    //private void ToApply(bool b)
    //{
    //    if (!b)
    //        m_Layout.HideApplyInfo();
    //    else
    //    {
    //        m_Layout.ShowApplyInfo();
    //        RefreshApply();
    //    }
    //}
}
