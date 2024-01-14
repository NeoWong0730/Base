using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public partial class UI_Team_Player : UIBase, UI_Team_Player_Layout.IListener
    {

        UI_Team_Player_Layout m_Layout = new UI_Team_Player_Layout();


        private List<Sys_Role_Info.InfoItem> mCommands = new List<Sys_Role_Info.InfoItem>();

        private Sys_Role_Info.InfoParmas mCurInfoParmas = new Sys_Role_Info.InfoParmas();


        private int m_selectPlayer = -1;

        private List<Hero> m_Heroes = new List<Hero>(0);

        CmdSocialGetBriefInfoAck m_curRoleFromServer;

        private Sys_Role_Info.DoRoleInfo m_doRoleInfo;

        private Sys_Role_Info.LocalRole m_localRole = new Sys_Role_Info.LocalRole();

        private Sys_Role_Info.fromServer m_fromServerRole = new Sys_Role_Info.fromServer();

        //private bool isList = false;
        protected override void OnLoaded()
        {
            m_Layout.Loaded(gameObject.transform);

            HidePlayerInfo();
            HideRoleList();

            m_Layout.ProcessEvents(this);
        }
       
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            if (toRegister)
            {
                m_Layout.ClickPlayerItem.AddListener(OnClickPlayer);
                m_Layout.ClickCommand.AddListener(OnClickPlayerCommand);
            }
            else
            {
                m_Layout.ClickPlayerItem.RemoveListener(OnClickPlayer);
                m_Layout.ClickCommand.RemoveListener(OnClickPlayerCommand);
            }

            Sys_Role_Info.Instance.eventEmitter.Handle<Sys_Role_Info.InfoParmas>(Sys_Role_Info.EEvents.NetMsg_RoleInfo_Open, OnMsgRoleInfo, toRegister);
            Sys_Role_Info.Instance.eventEmitter.Handle<Sys_Role_Info.InfoParmas>(Sys_Role_Info.EEvents.OnClickSelect, OnMsgClickSelect, toRegister);
            Sys_Role_Info.Instance.eventEmitter.Handle<CmdSocialGetBriefInfoAck>(Sys_Role_Info.EEvents.NetMsg_RoleInfo, OnMsgRoleInfoRefresh, toRegister);

            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnAddFriendSuccess, OnAddFriendSuccess, toRegister);
        }

        private void OnClickPlayer(int index)
        {
            m_localRole.hero = m_Heroes[index];

            Sys_Role_Info.Instance.OpenRoleInfo(m_localRole.getRoleID(), Sys_Role_Info.EType.FromAvatar);
        }

        public void ClosePlayerInfo()
        {
            if (m_Heroes.Count > 1 && m_Layout.isPlayerInfoActive())
            {
                HidePlayerInfo();
                return;
            }

            Sys_Role_Info.Instance.CloseRoleInfo();
        }

        public void OnClickClose()
        {
            ClosePlayerInfo();
        }

        private void HidePlayerInfo()
        {

            m_Layout.SetActivePlayeInfo(false);

        }

        private void OnClickPlayerCommand(int index)
        {
            if (index < mCommands.Count)
            {
                var command = mCommands[index];
                command.mClickAc?.Invoke(m_doRoleInfo);
                if (command.mOnClickClose)
                {
                    Sys_Role_Info.Instance.CloseRoleInfo();
                }
            }
            else
            {
                Sys_Role_Info.Instance.CloseRoleInfo();
            }
        }


        protected override void OnShow()
        {            
            m_Heroes.Clear();

            m_selectPlayer = -1;

            DoUI();
        }

        protected override void OnOpen(object arg)
        {
            mCurInfoParmas.Copy(arg as Sys_Role_Info.InfoParmas);
        }


        private void HideRoleList()
        {
            m_Layout.SetPlayerListActive(false);
        }

        protected override void OnHide()
        {
            HidePlayerInfo();

            HideRoleList();            

            m_fromServerRole.hero = null;

            m_localRole.hero = null;

            m_doRoleInfo = null;

            CloseSelf();
        }
        private void getPlayerCommand()
        {
            mCommands.Clear();

            getCommand(mCurInfoParmas, m_doRoleInfo);


            if (mCurInfoParmas.mCustom == null)
                return;

            int count = mCurInfoParmas.mCustom.Count;

            for (int i = 0; i < count; i++)
            {
                mCommands.Add(mCurInfoParmas.mCustom[i]);
            }
        }

        private void RefreshInfo()
        {
            if (isVisibleAndOpen == false || m_doRoleInfo == null)
                return;

            getPlayerCommand();

            m_Layout.SetPlayerNameText(m_doRoleInfo.getRoleName());

            m_Layout.SetPlayerLabor(m_doRoleInfo.getGuildName());

            m_Layout.SetPlayerProfession(m_doRoleInfo.getRoleOccule(), m_doRoleInfo.getRoleOcculeRank());

            m_Layout.SetPlayerLevel(m_doRoleInfo.getLevel());

            m_Layout.SetPlayerId(m_doRoleInfo.getRoleID());

            m_Layout.SetPlayerIcon(m_doRoleInfo.getHeroID(), m_doRoleInfo.getHeadId(), m_doRoleInfo.getHeadFrameId());


            int count = mCommands.Count;

            m_Layout.SetCommandSize(count);

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetPlayerCommandText(i, mCommands[i].mName);
                m_Layout.SetPlayerCommandID(i, i);

                m_Layout.SetPlayerCommandInteractable(i, mCommands[i].mClickAc != null);
            }

            var roleid = m_doRoleInfo.getRoleID();
            bool isFriend = Sys_Society.Instance.IsFriend(roleid);

            if (isFriend)
            {
                var friendvalue = Sys_Society.Instance.GetFriendValueByID(roleid);
                var frienddata = Sys_Society.Instance.GetCSVFriendIntimacyDataByID(roleid);              
                string friendstring = frienddata == null ? string.Empty : LanguageHelper.GetTextContent(frienddata.IntimacyLvlLan);
                m_Layout.SetRelation(friendvalue, friendstring);
            }

            m_Layout.SetRelationActive(isFriend);

        }



        private void DoUI()
        {
            if (mCurInfoParmas.eType != Sys_Role_Info.EType.FromAvatar)
                m_Heroes.Clear();


            if (mCurInfoParmas.eType == Sys_Role_Info.EType.Avatar)
            {
                m_Heroes.AddRange(mCurInfoParmas.mHeroes);
            }

            if (mCurInfoParmas.eType == Sys_Role_Info.EType.Avatar && m_Heroes.Count > 1)
            {
                HidePlayerInfo();

                ShowRoleList();

               // isList = true;

                return;
            }

            m_fromServerRole.hero = mCurInfoParmas.mInfo;

            m_doRoleInfo = m_fromServerRole;

            ShowPlayerInfo();

            if (mCurInfoParmas.eType != Sys_Role_Info.EType.FromAvatar)
                HideRoleList();

            HideFamilyInfo();
        }

        private void ShowPlayerInfo()
        {
            m_Layout.SetActivePlayeInfo(true);

            RefreshInfo();
        }

        private void ShowRoleList()
        {
            m_Layout.SetPlayerListActive(true);

            m_Layout.SetPlayerSize(m_Heroes.Count);

            for (int i = 0; i < m_Heroes.Count; i++)
            {
                Hero hero = m_Heroes[i];

                m_Layout.SetPlayerItemName(i, hero.heroBaseComponent.Name);

                m_Layout.SetPlayerItemIcon(i, CharacterHelper.getHeadID(hero.heroBaseComponent.HeroID, hero.heroBaseComponent.HeadId), CharacterHelper.getHeadFrameID(hero.heroBaseComponent.HeadFrameId));

                m_Layout.SetPlayerItemLevel(i, hero.heroBaseComponent.Level);
            }
        }

        private void ShowFamilyInfo()
        {
            m_Layout.SetFamilyInfoActive(true);
            RefreshFamilyInfo();
        }
        private void HideFamilyInfo()
        {
            m_Layout.SetFamilyInfoActive(false);
        }
        private void RefreshFamilyInfo()
        {
            if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
            uint Position = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.MyPosition;
            Sys_Family.FamilyData.EFamilyStatus myStatus = (Sys_Family.FamilyData.EFamilyStatus)(Position % 10000);

            var member = Sys_Family.Instance.familyData.CheckMember(mCurInfoParmas.mInfo.RoleId);
            Sys_Family.FamilyData.EFamilyStatus targetStatus = (Sys_Family.FamilyData.EFamilyStatus)(member.Position % 10000);

            List<GuildDetailInfo.Types.BranchInfo> branchInfos = new List<GuildDetailInfo.Types.BranchInfo>();
            branchInfos.AddRange(Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.BranchMemberInfo);

            List<uint> FamilyStatuses = new List<uint>()
            {
                (uint)Sys_Family.FamilyData.EFamilyStatus.EViceLeader,
                (uint)Sys_Family.FamilyData.EFamilyStatus.EElders,
                (uint)Sys_Family.FamilyData.EFamilyStatus.EMember,
            };

            foreach (var info in branchInfos)
            {
                uint status = info.Id * 10000 + (uint)Sys_Family.FamilyData.EFamilyStatus.EBranchLeader;
                FamilyStatuses.Add(status);
            }

            var list_Toggles = m_Layout.GetToggles(FamilyStatuses.Count);
            for (int i = 0, count = list_Toggles.Count; i < count; i++)
            {
                if (i < FamilyStatuses.Count)
                {
                    uint BranchId = FamilyStatuses[i] / 10000;
                    Sys_Family.FamilyData.EFamilyStatus status = (Sys_Family.FamilyData.EFamilyStatus)(FamilyStatuses[i] % 10000);

                    Table.CSVFamilyPostAuthority.Data cSVFamilyPostAuthorityData = Table.CSVFamilyPostAuthority.Instance.GetConfData((uint)status);
                    if (null == cSVFamilyPostAuthorityData && status >= myStatus)
                    {
                        m_Layout.SetToggleActive(i, false);
                    }
                    else
                    {
                        if (BranchId > 0)
                        {
                            GuildDetailInfo.Types.BranchInfo BranchInfo = branchInfos.Find(x => x.Id == BranchId);
                            int number = Sys_Family.Instance.familyData.CheckMemberInfos(BranchId, new List<Sys_Family.FamilyData.EFamilyStatus>() { status }).Count;
                            m_Layout.SetToggle(i, (FamilyStatuses[i]).ToString(), BranchInfo.Name.ToStringUtf8() + LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName),
                            string.Format("{0}/{1}", number, Sys_Family.Instance.familyData.GetPostNum(status)));
                        }
                        else
                        {
                            int number = Sys_Family.Instance.familyData.CheckMemberInfos(0, new List<Sys_Family.FamilyData.EFamilyStatus>() { status }).Count;
                            m_Layout.SetToggle(i, (FamilyStatuses[i]).ToString(), LanguageHelper.GetTextContent(cSVFamilyPostAuthorityData.PostName),
                            string.Format("{0}/{1}", number, Sys_Family.Instance.familyData.GetPostNum(status)));
                        }

                        if(targetStatus == status && FamilyStatuses[i] == member.Position)
                        {
                            m_Layout.getAtToggleItem(i).isOn = true;
                        }
                        else if (targetStatus != Sys_Family.FamilyData.EFamilyStatus.EBranchLeader && targetStatus == status)//目标成员职位等于当前toggle 对应职位
                        {
                            m_Layout.getAtToggleItem(i).isOn = true;
                        }
                        else if (targetStatus == Sys_Family.FamilyData.EFamilyStatus.EBrachMember &&
                            status == Sys_Family.FamilyData.EFamilyStatus.EMember)//目标成员是家族分会成员且当前toggle是成员
                        {
                            m_Layout.getAtToggleItem(i).isOn = true;
                        }
                    }
                }
                else
                {
                    m_Layout.SetToggleActive(i, false);
                }
            }
        }
        public void OnClickSetStatus()
        {
            OnClickClose();
            uint StatusId = m_Layout.GetSelectStatus();
            Sys_Family.Instance.SendGuildChangePositionReq(mCurInfoParmas.mInfo.RoleId, StatusId);
        }
    }

    public partial class UI_Team_Player : UIBase, UI_Team_Player_Layout.IListener
    {
        private void OnMsgRoleInfo(Sys_Role_Info.InfoParmas infoParmas)
        {
            mCurInfoParmas.Copy(infoParmas);

            DoUI();

        }

        private void OnMsgClickSelect(Sys_Role_Info.InfoParmas infoParmas)
        {
            if (infoParmas.mHeroes == null || infoParmas.mHeroes.Count == 0)
            {
                Sys_Role_Info.Instance.CloseRoleInfo();
                return;
            }

            mCurInfoParmas.Copy(infoParmas);

            DoUI();
        }

        private void OnMsgRoleInfoRefresh(CmdSocialGetBriefInfoAck info)
        {
            m_fromServerRole.hero = info;

            m_doRoleInfo = m_fromServerRole;

            RefreshInfo();
        }


        private void OnAddFriendSuccess(ulong roleID)
        {
            if (m_doRoleInfo == null || roleID != m_doRoleInfo.getRoleID())
                return;

            RefreshInfo();
        }
    }
}