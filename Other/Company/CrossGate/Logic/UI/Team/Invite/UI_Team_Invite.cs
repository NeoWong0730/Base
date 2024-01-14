using System;
using System.Collections.Generic;

using Logic.Core;
using UnityEngine;

namespace Logic
{
    public partial class UI_Team_Invite : UIBase, UI_Team_Invite_Layout.IListener
    {
        UI_Team_Invite_Layout m_Layout = new UI_Team_Invite_Layout();

        Dictionary<ulong, RoleInfo> m_Infos = new Dictionary<ulong, RoleInfo>();

        List<RoleInfo> m_InfosList = new List<RoleInfo>();

        List<RoleInfo> m_ShowInfo = new List<RoleInfo>();

        Parmas m_Parmas;

        string m_SearchKey = string.Empty;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMember, OnUpdateFamily, toRegister);

            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.SearchNearRole, OnSearchNearRole, toRegister);
        }
        protected override void OnOpen(object arg)
        {
            var parmas = arg as Parmas;

            if (parmas == null)
                return;

            m_Parmas = parmas;

        }

        protected override void OnClose()
        {
            m_Parmas = null;

            m_SearchKey = string.Empty;

            m_ShowInfo.Clear();

            m_Infos.Clear();

            m_InfosList.Clear();
        }

        protected override void OnShow()
        {
            m_Layout.SetFocusTogFriend();
        }

        private void GetFriendList()
        {

            m_Infos.Clear();
            m_InfosList.Clear();

            foreach (var friend in Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                if (friend.isOnLine)
                {
                    var item = new RoleInfo();

                    item.RoleID = friend.roleID;
                    item.Name = friend.roleName;
                    item.Level = friend.level;
                    item.Occ = friend.occ;
                    item.Icon = friend.heroID;
                    //     item.Rank = friend.;

                    m_Infos.Add(item.RoleID, item);
                    m_InfosList.Add(item);
                }
            }

        }

        private void GetFamilyList()
        {
            var mems = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetMemberInfoAck.Member;

            int count = mems.Count;

            m_Infos.Clear();
            m_InfosList.Clear();

            for (int i = 0; i < count; i++)
            {
                if (mems[i].RoleId != Sys_Role.Instance.RoleId && mems[i].LastOffline == 0)
                {
                    var item = new RoleInfo();
                    item.RoleID = mems[i].RoleId;
                    item.Name = mems[i].Name.ToStringUtf8();
                    item.Level = mems[i].Lvl;
                    item.Occ = mems[i].Occ;
                    item.Icon = mems[i].HeroId;
                    item.Rank = 0;// mems[i].CareerRank;
                    item.Head = mems[i].Photo;
                    item.HeadFrame = mems[i].PhotoFrame;
                    m_Infos.Add(item.RoleID, item);
                    m_InfosList.Add(item);
                }

            }
        }

        private void Refresh()
        {
            RefreshSearch();

            int count = m_ShowInfo.Count;

            m_Layout.SetItemCount(count);

            for (int i = 0; i < count; i++)
            {
                m_Layout.Set(i, m_ShowInfo[i].RoleID, m_ShowInfo[i].Icon, m_ShowInfo[i].Level, m_ShowInfo[i].Name, m_ShowInfo[i].Occ, m_ShowInfo[i].Rank, m_ShowInfo[i].Head, m_ShowInfo[i].HeadFrame);
            }

        }


        private void RefreshSearch()
        {
            m_ShowInfo.Clear();

            if (string.IsNullOrEmpty(m_SearchKey))
            {
                m_ShowInfo.AddRange(m_InfosList);
                return;
            }

            ulong searchID = 0;
            if (ulong.TryParse(m_SearchKey, out searchID))
            {
                if (m_Infos.ContainsKey(searchID))
                {
                    m_ShowInfo.Add(m_Infos[searchID]);
                }
                return;
            }

            var resh = m_InfosList.FindAll(o =>
            {
                return o.Name.Contains(m_SearchKey);
            });

            m_ShowInfo.AddRange(resh);
        }


        private void OnSearchNearRole()
        {
            var roleList = Sys_Team.Instance.SearchNearRoles.RoleList;

            int count = roleList.Count;

            m_Layout.SetItemCount(count);

            m_InfosList.Clear();
            m_Infos.Clear();
            for (int i = 0; i < count; i++)
            {
                var item = new RoleInfo();
                item.RoleID = roleList[i].RoleId;
                item.Name = roleList[i].Name.ToStringUtf8();
                item.Level = roleList[i].Level;
                item.Occ = roleList[i].Career;
                item.Icon = roleList[i].HeroId;

                item.HeadFrame = roleList[i].HeadId;
                m_InfosList.Add(item);
                m_Infos.Add(item.RoleID, item);
            }

            Refresh();
        }

        private void OnUpdateFamily()
        {

            GetFamilyList();

            Refresh();
        }

        public class Parmas
        {
            public uint Type { get; set; }
        }

        class RoleInfo
        {
            public ulong RoleID { get; set; }

            public string Name { get; set; }

            public uint Level { get; set; }

            public uint Occ { get; set; }

            public uint Rank { get; set; }

            public uint Icon { get; set; } = 0;

            public uint Head { get; set; }

            public uint HeadFrame { get; set; }

            public bool isOffline { get; set; } = false;
        }

        public void OnClickClose()
        {
            UIManager.HitButton(EUIID.UI_Team_Invite, "Close");

            CloseSelf();
        }

        public void OnClickInvite(ulong id)
        {
            UIManager.HitButton(EUIID.UI_Team_Invite, "Invite");

            Sys_Team.Instance.InvitedOther(id);
        }

        public void OnClickSreach()
        {
            UIManager.HitButton(EUIID.UI_Team_Invite, "Sreach");

            Refresh();

            if (string.IsNullOrEmpty(m_SearchKey))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10702));
                return;
            }

            if (m_ShowInfo.Count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10703));
            }
        }

        public void OnInputEnd(string value)
        {
            UIManager.HitButton(EUIID.UI_Team_Invite, "InputEnd");

            if (Sys_WordInput.Instance.HasLimitWord(value))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));

                m_Layout.SetInputField(string.Empty);

                m_SearchKey = string.Empty;

                return;
            }
            m_SearchKey = value;
        }


        public void OnTogFriend(bool state)
        {
            if (state)
            {
                UIManager.HitButton(EUIID.UI_Team_Invite, "TogFriend");
                m_Layout.SetItemCount(0);
                GetFriendList();
                Refresh();
            }
                
        }
        public void OnTogFamliy(bool state)
        {
            if (state)
            {
                UIManager.HitButton(EUIID.UI_Team_Invite, "TogFamliy");
                m_Layout.SetItemCount(0);
                Sys_Family.Instance.SendGuildGetMemberInfoReq();
            }
                
        }
        public void OnTogNear(bool state)
        {
            if (state)
            {
                UIManager.HitButton(EUIID.UI_Team_Invite, "TogNear");
                m_Layout.SetItemCount(0);
                Sys_Team.Instance.ApplySearchNearRole();
            }
               
        }
    }
}
