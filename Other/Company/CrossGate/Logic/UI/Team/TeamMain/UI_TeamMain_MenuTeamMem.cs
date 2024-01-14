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
    public class UI_TeamMain_MenuTeamMem : UIComponent
    {
        UI_TeamMain_MenuTeamMem_Layout m_Layout = new UI_TeamMain_MenuTeamMem_Layout();

        private ulong mSelectRoleID = 0;        

        private Action mHideAction;
        protected override void Loaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.ClickMenuAction = OnClickCommand;

            m_Layout.onClickCloseAction = OnClickClose;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public override void Show()
        {

            m_Layout.Show();

        }
        public override void Hide()
        {
            m_Layout.Hide();

            mSelectRoleID = 0;

            mHideAction?.Invoke();

            mHideAction = null;
        }


        private void OnClickClose()
        {
            Hide();

            mHideAction?.Invoke();

        }
        //enum Command
        //{
        //    ShortLeave = 0,//暂离
        //    Leave,//离开
        //    Captain,//升为队长
        //    ShutDown,//请离队伍
        //    AddFriend,//加为好友
        //    CallBack,//召回
        //    Leader,//申请带队
        //    ComeBack,//回归队伍
        //}

        //0 暂离,1 离开,2 升为队长,3 请离队伍,4 加为好友,5 召回,6 申请带队,7 回归队伍

        uint[] CommandArray = { 2002092 , 2002094 , 2002095, 2002096, 2002097 , 2002098, 2002091 , 2002047 };
        public void Show(Vector3[] points,ulong roleID,Action endAc)
        {
            SetSelectRoleID(roleID);

           // if (m_Command.Count > 0)
            {
                m_Layout.SetMenu(m_Command);

                m_Layout.Show(points);
  
            }

            mHideAction = endAc;
        }

        private List<uint> m_Command = new List<uint>(5);
        private void SetSelectRoleID(ulong roleID)
        {
            mSelectRoleID = roleID;

            m_Command.Clear();

            bool isCaptain = Sys_Team.Instance.isCaptain(roleID);

            ulong ownID = Sys_Role.Instance.Role.RoleId;

            bool isPlayerCaptain = Sys_Team.Instance.isCaptain();

            TeamMem teamMem = Sys_Team.Instance.getTeamMem(roleID);

            bool bleave = teamMem.IsLeave();

            //玩家为队长 点击自己
            if (isPlayerCaptain && ownID == roleID)
            {
                m_Command.Add(CommandArray[0]);
                m_Command.Add(CommandArray[1]);
                return;
            }

            //玩家为队长 点击其他玩家
            if (isPlayerCaptain)
            {
                if (bleave == false)
                    m_Command.Add(CommandArray[2]);

                m_Command.Add(CommandArray[3]);

                //加好友
                bool isFriend0 = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(roleID);

                if (ownID != roleID && !isFriend0)
                    m_Command.Add(CommandArray[4]);

                if (bleave)
                    m_Command.Add(CommandArray[5]);

                return;
            }

            // 玩家为队员

            if (ownID == roleID)
            {
                if (!bleave)
                    m_Command.Add(CommandArray[6]);

                m_Command.Add(bleave ? CommandArray[7] : CommandArray[0]);

                m_Command.Add(CommandArray[1]);

                return;
            }


            bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(roleID);

            if (ownID != roleID && !isFriend)
                m_Command.Add(CommandArray[4]);

          

        }

        private void OnClickCommand(int index)
        {
            m_Layout.Hide();

            mHideAction?.Invoke();

            mHideAction = null;

            uint command = m_Command[index];

            switch(command)
            {
                case 2002092:
                    OnLeave();
                    break;
                case 2002094:
                    OnExitTeam();
                    break;
                case 2002095:
                    OnToBeCaptain();
                    break;
                case 2002096:
                    OnOutTeam();
                    break;
                case 2002097:
                    OnAddFriend();
                    break;
                case 2002098:
                    OnCallBack();
                    break;
                case 2002091:
                    OnLeadTeam();
                    break;
                case 2002047:
                    OnGoBack();
                    break;
            }
        }

        //暂离
        private void OnLeave()
        {
            //if (mSelectRoleID != Sys_Role.Instance.RoleId)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002243));
            //    return;
            //}
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - Leave");

            Sys_Team.Instance.ApplyLeave(Sys_Role.Instance.RoleId);
        }

        //离开
        private void OnExitTeam()
        {
            //if (mSelectRoleID != Sys_Role.Instance.RoleId)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002243));
            //    return;
            //}

            UIManager.HitButton(EUIID.UI_MainInterface, "Team - ExitTeam");

            Sys_Team.Instance.ApplyExitTeam();
        }

        //升为队长
        private void OnToBeCaptain()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - ToBeCaptain");

            if (Sys_Team.Instance.isCaptain() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002217));
                return;
            }

            Sys_Team.Instance.ApplyToCaptaion(mSelectRoleID);
        }

        //请离队伍
        private void OnOutTeam()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - OutTeam");

            if (Sys_Team.Instance.isCaptain() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002217));
                return;
            }
            Sys_Team.Instance.KickMemTeam(mSelectRoleID);
        }

        //加为好友
        private void OnAddFriend()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - AddFriend");

            bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(mSelectRoleID);

            if (isFriend)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2004200));
                return;
            }

            Sys_Society.Instance.ReqAddFriend(mSelectRoleID);
        }

        //召回
        private void OnCallBack()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - CallBack");

            if (Sys_Team.Instance.isCaptain() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002217));
                return;
            }

            Sys_Team.Instance.ApplyCallBack(mSelectRoleID);
        }

        //申请带队
        private void OnLeadTeam()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - LeadTeam");
            //if ((Sys_Role.Instance.RoleId) != mSelectRoleID)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002243));
            //    return;
            //}


            Sys_Team.Instance.ApplyLeader(Sys_Role.Instance.RoleId);
        }

        //回归队伍
        private void OnGoBack()
        {
            UIManager.HitButton(EUIID.UI_MainInterface, "Team - GoBack");
            //if ((Sys_Role.Instance.RoleId) != mSelectRoleID)
            //{
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2002243));
            //    return;
            //}

            Sys_Team.Instance.ApplyComeBack(Sys_Role.Instance.RoleId);
        }
    }
}
