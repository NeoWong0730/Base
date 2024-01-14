using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;

public partial class UI_Team_Member : UIBase, UI_Team_Member_Layout.IListener
{

    Vector3[] infoMenuCorners = new Vector3[4];

    private int mMenuMemberIndex = -1;
    /// <summary>
    ///显示菜单并设置位置
    /// </summary>
    /// <param name="vectors">世界坐标</param>
    /// <param name="left"> 0 代表左边，1 代表 右边</param>
    private void ShowItemMenu(Vector3[] vectors,int left =0,int index=-1)
    {
        m_Layout.getInfoMenuWorldCorners(infoMenuCorners);

        int itemPointIndex = left  == 0 ? 1 : 2;
        int menuPointIndex = left == 0 ? 2 : 1;

        Vector3 disoffset = vectors[itemPointIndex] - infoMenuCorners[menuPointIndex];

        m_Layout.SetInfoMenuPosition(disoffset);

        mMenuMemberIndex = index;
        
        ShowItemMenu();

        bool iscaptain = Sys_Team.Instance.isCaptain();

        if (iscaptain)
            ShowCpataionMode(mMenuMemberIndex-1);
        else
            ShowMemberMode(mMenuMemberIndex-1);


        var teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex-1);

        if (teamMem == null)
            return;

        bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(teamMem.MemId);

        m_Layout.SetCommandState(UI_Team_Member_Layout.ECommandType.AddFriend, !isFriend);
    }


    private void ShowCpataionMode(int memindex)
    {
        m_Layout.SetModeCpataion();

        var teamMem = Sys_Team.Instance.getTeamMem(memindex);

        if (teamMem == null)
            return;

        bool result = Sys_Team.Instance.isHadEntrust(teamMem.MemId);

        m_Layout.SetCommand(UI_Team_Member_Layout.ECommandType.Delegate, (uint)(result ? 2002257 : 2002114));


        bool isleave = teamMem.IsLeave();

        m_Layout.SetCommandState(UI_Team_Member_Layout.ECommandType.CallBlack, isleave);

        m_Layout.SetCommandState(UI_Team_Member_Layout.ECommandType.SetPositionOrder, Sys_Team.Instance.TeamMemsCount >= 2);

    }

    private void ShowMemberMode(int memindex)
    {
        m_Layout.SetModeMember();
    }
    private void ShowItemMenu()
    {
        m_Layout.ActiveItemMenu(true);
    }
    private void HideItemMenu()
    {
        m_Layout.ActiveItemMenu(false);

        LostFocusItem();
    }
    private void OnClickItemMenu(int index)
    {
        switch (index)
        {
            case 0:
                SetMessage();
                break;
            case 1:
                SetTeamPostiton();
                break;
            case 2:
                AddFriend();
                break;
            case 3:
                delCommond();
                break;
            case 4:
                toBeCaptain();
                break;
            case 5:
                OutTeam();
                break;
            case 6:
                OnClickCallBlack();
                break;
        }

        HideItemMenu();
    }

    //发送消息
    private void SetMessage()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "SendMesssage");

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex - 1);

        if (teamMem == null || teamMem.MemId == Sys_Role.Instance.RoleId)
            return;


        Sys_Society.Instance.OpenPrivateChat(new Sys_Society.RoleInfo()
        {
            roleID = teamMem.MemId,
            roleName = teamMem.Name.ToStringUtf8(),
            level = teamMem.Level,
            heroID = teamMem.HeroId,
            occ = teamMem.Career,
            isOnLine = !teamMem.IsOffLine(),
        });
          //  UIManager.OpenUI(EUIID.UI_Society, false,teamMem.MemId);
    }

    //调整站位
    private void SetTeamPostiton()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "SetPosition");

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex - 1);

        if (teamMem == null || Sys_Team.Instance.TeamMemsCount < 2)
            return;

        ShowMemberChangeOrder(teamMem.MemId);
    }

    //加为好友
    private void AddFriend()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "AddFriend");

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex - 1);

        if (teamMem == null || teamMem.MemId == Sys_Role.Instance.RoleId)
            return;


        bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(teamMem.MemId);

        if (isFriend)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2004200));
            return;
        }

        Sys_Society.Instance.ReqAddFriend(teamMem.MemId);
    }

    //委托指挥
    private void delCommond()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "delCommond");

        if (Sys_Team.Instance.isCaptain() == false)
            return;

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex - 1);

        if (teamMem == null)
            return;

        bool isHadEntrust = Sys_Team.Instance.isHadEntrust(teamMem.MemId);

        if (isHadEntrust)
        {
            Sys_Team.Instance.CancleToCommand();
            return;
        }

        Sys_Team.Instance.GiveToCommand(teamMem.MemId);
    }

    //升为队长
    private void toBeCaptain()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "toBeCaptain");

        if (Sys_Team.Instance.isCaptain() == false)
            return;

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex-1);

        if (teamMem == null)
        {
            return;
        }
         

        Sys_Team.Instance.ApplyToCaptaion(teamMem.MemId);
    }

    //请离队伍
    private void OutTeam()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "KickMemTeam");

        if (mMenuMemberIndex < 0)
            return;

        if (Sys_Team.Instance.isCaptain() == false)
            return;

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex -1);

        Sys_Team.Instance.KickMemTeam(teamMem.MemId);
    }

    private void OnClickCallBlack()
    {
        UIManager.HitButton(EUIID.UI_Team_Member, "CallBlack");

        if (mMenuMemberIndex < 0)
            return;

        if (Sys_Team.Instance.isCaptain() == false)
            return;

        TeamMem teamMem = Sys_Team.Instance.getTeamMem(mMenuMemberIndex - 1);

        Sys_Team.Instance.ApplyCallBack(teamMem.MemId);
    }
}
