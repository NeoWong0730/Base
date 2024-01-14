using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;

public partial class UI_Team_Member : UIBase,UI_Team_Member_Layout.IListener
{
    public void EditCommand()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "EditFightCommand");

        m_Layout.HideCommandTransMenu();

       UIManager.OpenUI(EUIID.UI_Team_WarCommand);
    }

    public void DelegationCommand()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "DelegationCommand");

        m_Layout.HideCommandTransMenu();


        if (Sys_Team.Instance.TeamMemsCount == 1)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10662));
            return;
        }
        ShowMembersDelegation();
    }

}
