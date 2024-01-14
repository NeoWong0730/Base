using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public class UI_Society_FriendGroupSetting_Layout
    {
        public class RoleInfoItem
        {
            public GameObject root;

            public Image roleIcon;
            public Image roleIconFrame;
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
                roleIconFrame = root.FindChildByName("Image_Frame").GetComponent<Image>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                friendValue = root.FindChildByName("Text_Friendly").GetComponent<Text>();
                toggle = root.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }

            public void Update(Sys_Society.RoleInfo _roleInfo, bool isSelect)
            {
                roleInfo = _roleInfo;

                //TODO OCCICON

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                roleIconFrame.SetNativeSize();
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.occ).icon);
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                //TextHelper.SetText(friendValue, roleInfo.friendValue);

                toggle.onValueChanged.Invoke(isSelect);
                toggle.isOn = isSelect;
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    UI_Society_FriendGroupSetting.selectRoleInfos[roleInfo.roleID] = roleInfo;
                }
                else
                {                                
                    if (UI_Society_FriendGroupSetting.selectRoleInfos.ContainsKey(roleInfo.roleID))
                    {
                        UI_Society_FriendGroupSetting.selectRoleInfos.Remove(roleInfo.roleID);
                    }
                }
                toggleChange?.Invoke();
            }
        }

        public GameObject root;

        public Button closeButton;

        public InputField friendgroupNameInput;
        public CheckTextLengthScript checkTextLengthScript;
        public Text selectNum;
        public Button sureButton;

        public GameObject roleContent;
        public GameObject roleItemPrefab;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();

            friendgroupNameInput = root.FindChildByName("FriendGroupNameInputField").GetComponent<InputField>();
            checkTextLengthScript = friendgroupNameInput.GetComponent<CheckTextLengthScript>();
            selectNum = root.FindChildByName("SelectNum").GetComponent<Text>();
            sureButton = root.FindChildByName("SureBtn").GetComponent<Button>();

            roleContent = root.FindChildByName("Content");
            roleItemPrefab = root.FindChildByName("FriendItemPrefab");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            sureButton.onClick.AddListener(listener.OnClickSureButton);
            friendgroupNameInput.onValueChanged.AddListener(listener.OnFriendgroupNameInputValueChanged);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickSureButton();

            void OnFriendgroupNameInputValueChanged(string str);
        }
    }

    public class UI_Society_FriendGroupSetting : UIBase, UI_Society_FriendGroupSetting_Layout.IListener
    {
        private UI_Society_FriendGroupSetting_Layout layout = new UI_Society_FriendGroupSetting_Layout();

        private Sys_Society.FriendGroupInfo friendGroupInfo;

        public static Dictionary<ulong, Sys_Society.RoleInfo> selectRoleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<string>(Sys_Society.EEvents.OnCreateOrSettingFriendGroupSuccess, OnCreateOrSettingFriendGroupSuccess, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            friendGroupInfo = arg as Sys_Society.FriendGroupInfo;
        }

        protected override void OnShow()
        {
            layout.friendgroupNameInput.text = friendGroupInfo.name;
            UpdateRoleList(friendGroupInfo);
        }

        void ResetSelectRoleInfos()
        {
            TextHelper.SetText(layout.selectNum, "0");
            selectRoleInfos.Clear();
        }

        void UpdateRoleList(Sys_Society.FriendGroupInfo _friendGroupInfo)
        {
            friendGroupInfo = _friendGroupInfo;
            ResetSelectRoleInfos();
            layout.roleContent.DestoryAllChildren();


            foreach (var roleInfo in Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                roleItemGo.SetActive(true);
                UI_Society_FriendGroupSetting_Layout.RoleInfoItem roleInfoItem = new UI_Society_FriendGroupSetting_Layout.RoleInfoItem(roleItemGo);
                roleInfoItem.toggleChange = () =>
                {
                    TextHelper.SetText(layout.selectNum, selectRoleInfos.Count.ToString());
                };
                roleInfoItem.Update(roleInfo, _friendGroupInfo.roleIdsDic.ContainsKey(roleInfo.roleID));

                roleItemGo.transform.SetParent(layout.roleContent.transform, false);
            }
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_FriendGroupSetting);
        }

        public void OnClickSureButton()
        {
            if (layout.checkTextLengthScript.Check().IsSplit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13018));
                return;
            }

            if (Sys_WordInput.Instance.HasLimitWord(layout.friendgroupNameInput.text))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13037));
                return;
            }

            List<ulong> roleIDs = new List<ulong>();
            foreach (var roleInfo in selectRoleInfos.Values)
            {
                roleIDs.Add(roleInfo.roleID);
            }

            Sys_Society.Instance.ReqFriendGroup(layout.friendgroupNameInput.text, friendGroupInfo.name, roleIDs);
        }

        public void OnFriendgroupNameInputValueChanged(string str)
        {
            layout.friendgroupNameInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(str);
        }

        /// <summary>
        /// 创建或编辑好友分组成功回调///
        /// </summary>
        /// <param name="_friendGroupInfo">创建或编辑的好友分组</param>
        void OnCreateOrSettingFriendGroupSuccess(string name)
        {
            CloseSelf();
        }
    }
}
