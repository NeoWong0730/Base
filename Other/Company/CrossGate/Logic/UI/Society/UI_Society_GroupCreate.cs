using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public class UI_Society_GroupCreate_Layout
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
                toggle.onValueChanged.AddListener(OnClickCheckToggle);
            }

            public void Update(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                //TODO OCCICON

                ImageHelper.SetIcon(roleIcon, CSVCharacter.Instance.GetConfData(roleInfo.heroID).headid);
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.occ).icon);
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                TextHelper.SetText(friendValue, roleInfo.friendValue.ToString());
            }

            void OnClickCheckToggle(bool isOn)
            {
                if (isOn)
                {
                    UI_Society_GroupCreate.selectRoleInfos[roleInfo.roleID] = roleInfo;
                }
                else
                {
                    if (UI_Society_GroupCreate.selectRoleInfos.ContainsKey(roleInfo.roleID))
                    {
                        UI_Society_GroupCreate.selectRoleInfos.Remove(roleInfo.roleID);
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
        public InputField groupNameInput;
        public Button sureButton;

        public GameObject roleContent;
        public GameObject roleItemPrefab;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();

            chooseRoleCount = root.FindChildByName("SelectNum").GetComponent<Text>();
            searchInput = root.FindChildByName("SearchInputField").GetComponent<InputField>();
            searchButton = root.FindChildByName("Btn_Find").GetComponent<Button>();
            groupNameInput = root.FindChildByName("GroupNameInputField").GetComponent<InputField>();
            sureButton = root.FindChildByName("SureBtn").GetComponent<Button>();

            roleContent = root.FindChildByName("Content");
            roleItemPrefab = root.FindChildByName("FriendItemPrefab");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            searchButton.onClick.AddListener(listener.OnClickSearchButton);
            sureButton.onClick.AddListener(listener.OnClickSureButton);
            groupNameInput.onValueChanged.AddListener(listener.OnGroupNameInputValueChanged);
            searchInput.onValueChanged.AddListener(listener.OnSearchInputValueChanged);
        }

       public interface IListener
        {
            void OnClickCloseButton();

            void OnClickSearchButton();

            void OnClickSureButton();

            void OnGroupNameInputValueChanged(string str);

            void OnSearchInputValueChanged(string str);
        }
    }

    public class UI_Society_GroupCreate : UIBase, UI_Society_GroupCreate_Layout.IListener
    {
        private UI_Society_GroupCreate_Layout layout = new UI_Society_GroupCreate_Layout();

        private Dictionary<ulong, Sys_Society.RoleInfo> roleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        public static Dictionary<ulong, Sys_Society.RoleInfo> selectRoleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        static Sys_Society.GroupInfo creatingGroupInfo;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEvents(bool toRegister)
        {          
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnCreateGroupSuccess, OnCreateGroupSuccess, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            roleInfos = arg as Dictionary<ulong, Sys_Society.RoleInfo>;            
        }

        protected override void OnShow()
        {
            UpdateRoleList(roleInfos);

            layout.searchInput.text = string.Empty;
            layout.groupNameInput.text = string.Empty;
        }

        void ResetSelectRoleInfos()
        {
            TextHelper.SetText(layout.chooseRoleCount, "0");
            selectRoleInfos.Clear();
        }

        void UpdateRoleList(Dictionary<ulong, Sys_Society.RoleInfo> _roleInfos)
        {
            roleInfos = _roleInfos;
            ResetSelectRoleInfos();
            layout.roleContent.DestoryAllChildren();

            foreach (var roleInfo in roleInfos.Values)
            {
                if (roleInfo.isOnLine && roleInfo.level >= 20)
                {
                    GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                    roleItemGo.SetActive(true);
                    UI_Society_GroupCreate_Layout.RoleInfoItem roleInfoItem = new UI_Society_GroupCreate_Layout.RoleInfoItem(roleItemGo);
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
            UIManager.CloseUI(EUIID.UI_Society_GroupCreate);
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

        public void OnClickSureButton()
        {
            if (layout.groupNameInput.text.Length <= 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11652));
                return;
            }

            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(670);
            if (cSVParamData != null)
            {
                if (Sys_Role.Instance.Role.Level < int.Parse(cSVParamData.str_value))
                {
                    Sys_Hint.Instance.PushContent_Normal($"{int.Parse(cSVParamData.str_value)}后才能创建群组");
                    return;
                }
            }
           
            List<ulong> roleIDs = new List<ulong>();
            foreach (var roleInfo in selectRoleInfos.Values)
            {
                roleIDs.Add(roleInfo.roleID);
            }
            Sys_Society.Instance.ReqCreateGroup(layout.groupNameInput.text, roleIDs);
        }

        public void OnGroupNameInputValueChanged(string Namestr)
        {
            layout.groupNameInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(Namestr);
        }

        public void OnSearchInputValueChanged(string str)
        {
            layout.searchInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(str);
        }

        /// <summary>
        /// 创建群组成功回调///
        /// </summary>
        void OnCreateGroupSuccess()
        {
            CloseSelf();
        }
    }
}
