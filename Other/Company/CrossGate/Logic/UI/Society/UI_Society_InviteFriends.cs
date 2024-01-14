using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public class UI_Society_InviteFriends_Layout
    {
        public class RoleInfoItem
        {
            public GameObject root;

            public Image roleIcon;
            public Image occIcon;
            public Text roleName;
            public Text roleLv;
            public Text friendValue;
            public Toggle toggle;

            Sys_Society.RoleInfo roleInfo;

            public Action toggleChange;

            public RoleInfoItem(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                friendValue = root.FindChildByName("Text_Friendly").GetComponent<Text>();
                toggle = root.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }

            public void Update(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                //TODO OCCICON

                ImageHelper.SetIcon(roleIcon, CSVCharacter.Instance.GetConfData(roleInfo.heroID).headid);
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                TextHelper.SetText(friendValue, roleInfo.friendValue.ToString());
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    UI_Society_InviteFriends.selectRoleInfos[roleInfo.roleID] = roleInfo;
                }
                else
                {
                    if (UI_Society_InviteFriends.selectRoleInfos.ContainsKey(roleInfo.roleID))
                    {
                        UI_Society_InviteFriends.selectRoleInfos.Remove(roleInfo.roleID);
                    }
                }
                toggleChange?.Invoke();
            }
        }

        public GameObject root;

        public Button closeButton;

        public Text chooseRoleCount;
        public InputField searchInput;
        public Button searchButton;
        public Button inviteButton;

        public GameObject roleContent;
        public GameObject roleItemPrefab;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();

            chooseRoleCount = root.FindChildByName("SelectNum").GetComponent<Text>();
            searchInput = root.FindChildByName("SearchInputField").GetComponent<InputField>();
            searchButton = root.FindChildByName("Btn_Find").GetComponent<Button>();
            inviteButton = root.FindChildByName("InviteButton").GetComponent<Button>();
            
            roleContent = root.FindChildByName("Content");
            roleItemPrefab = root.FindChildByName("FriendItemPrefab");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            searchButton.onClick.AddListener(listener.OnClickSearchButton);
            inviteButton.onClick.AddListener(listener.OnClickInviteButton);
            searchInput.onValueChanged.AddListener(listener.OnSearchInputValueChanged);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickSearchButton();

            void OnClickInviteButton();

            void OnSearchInputValueChanged(string str);
        }
    }

    public class UI_Society_InviteFriends : UIBase, UI_Society_InviteFriends_Layout.IListener
    {
        private UI_Society_InviteFriends_Layout layout = new UI_Society_InviteFriends_Layout();

        private Sys_Society.GroupInfo groupInfo;

        private Dictionary<ulong, Sys_Society.RoleInfo> roleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        public static Dictionary<ulong, Sys_Society.RoleInfo> selectRoleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            groupInfo = arg as Sys_Society.GroupInfo;
        }

        protected override void OnShow()
        {            
            roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos();
            UpdateRoleList(roleInfos);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnDismissGroup, OnDismissGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfQuitGroupSuccess, OnSelfQuitGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfKickFromGroup, OnSelfKickFromGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherAddGroupSuccess, OnOtherAddGroupSuccess, toRegister);
        }

        void ResetSelectRoleInfos()
        {
            TextHelper.SetText(layout.chooseRoleCount, "0");
            selectRoleInfos.Clear();
        }

        void UpdateRoleList(Dictionary<ulong, Sys_Society.RoleInfo> roleInfos)
        {
            ResetSelectRoleInfos();
            layout.roleContent.DestoryAllChildren();

            foreach (Sys_Society.RoleInfo roleInfo in roleInfos.Values)
            {
                if (roleInfo.isOnLine && roleInfo.level >= 20 && !groupInfo.roleIDsDic.ContainsKey(roleInfo.roleID))
                {
                    GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                    roleItemGo.SetActive(true);
                    UI_Society_InviteFriends_Layout.RoleInfoItem roleInfoItem = new UI_Society_InviteFriends_Layout.RoleInfoItem(roleItemGo);
                    roleInfoItem.toggleChange = () =>
                    {
                        TextHelper.SetText(layout.chooseRoleCount, selectRoleInfos.Count.ToString());
                    };
                    roleInfoItem.Update(roleInfo);

                    roleItemGo.transform.SetParent(layout.roleContent.transform, false);
                }
            }
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_InviteFriends);
        }

        public void OnClickSearchButton()
        {
            Dictionary<ulong, Sys_Society.RoleInfo> roleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();
            foreach (Sys_Society.RoleInfo roleInfo in Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                if (roleInfo.roleName.Contains(layout.searchInput.text))
                {
                    roleInfos[roleInfo.roleID] = roleInfo;
                }

                if (roleInfo.roleID.ToString() == layout.searchInput.text)
                {
                    roleInfos[roleInfo.roleID] = roleInfo;
                }
            }
            UpdateRoleList(roleInfos);
        }

        public void OnClickInviteButton()
        {
            List<ulong> roleIDs = new List<ulong>();
            foreach (var info in selectRoleInfos.Values)
            {
                roleIDs.Add(info.roleID);
            }
            Sys_Society.Instance.ReqGroupAddMember(roleIDs, groupInfo.groupID);
        }

        public void OnSearchInputValueChanged(string str)
        {
            layout.searchInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(str);
        }

        void OnDismissGroup(uint groupID)
        {
            if (groupInfo.groupID == groupID)
            {
                CloseSelf();
                //TODO
                Sys_Hint.Instance.PushContent_Normal("该群已被解散");
            }
        }

        void OnSelfQuitGroupSuccess(uint groupID)
        {
            if (groupInfo.groupID == groupID)
            {
                CloseSelf();

                //TODO
                Sys_Hint.Instance.PushContent_Normal("退群成功");
            }
        }

        void OnSelfKickFromGroup(uint groupID)
        {
            if (groupInfo.groupID == groupID)
            {
                CloseSelf();

                //TODO
                Sys_Hint.Instance.PushContent_Normal("你被踢出群");
            }
        }

        void OnOtherAddGroupSuccess(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos();
                UpdateRoleList(roleInfos);
            }
        }
    }
}
