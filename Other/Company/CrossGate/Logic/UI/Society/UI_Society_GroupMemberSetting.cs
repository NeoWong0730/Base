using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Society_GroupMemberSetting_Layout
    {
        public abstract class RoleInfoItemBase
        {
            public GameObject root;

            protected Sys_Society.GroupInfo groupInfo;
        }
        public class RoleInfoItem : RoleInfoItemBase
        {
            public Image roleIcon;
            public Image occIcon;
            public Text roleName;
            public Text roleLv;
            public Image ownerImage;
            public GameObject delBtn;

            Sys_Society.RoleInfo roleInfo;

            public RoleInfoItem(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                ownerImage = root.FindChildByName("Image_Owner").GetComponent<Image>();
                delBtn = root.FindChildByName("DelBtn");
                delBtn.SetActive(false);
            }

            public void Update(Sys_Society.GroupInfo _groupInfo, Sys_Society.RoleInfo _roleInfo)
            {
                if (_groupInfo == null || _roleInfo == null)
                {
                    DebugUtil.LogError("_groupInfo is null or _roleInfo is null");
                    return;
                }

                groupInfo = _groupInfo;
                roleInfo = _roleInfo;

                ImageHelper.SetIcon(roleIcon, CSVCharacter.Instance.GetConfData(roleInfo.heroID).headid);
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.occ).icon);
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                ownerImage.gameObject.SetActive(roleInfo.roleID == groupInfo.leader);
            }
        }

        //public class InviteItem : RoleInfoItemBase
        //{
        //    public Button button;

        //    public InviteItem(GameObject gameObject, Sys_Society.GroupInfo _groupInfo)
        //    {
        //        root = gameObject;

        //        button = root.FindChildByName("Image_BG").GetComponent<Button>();
        //        button.onClick.AddListener(OnClickButton);

        //        groupInfo = _groupInfo;
        //    }

        //    void OnClickButton()
        //    {
        //        UIManager.OpenUI(EUIID.UI_Society_InviteFriends, false, groupInfo);
        //    }
        //}

        public GameObject root;

        public Button closeButton;
        public Text groupTitle;

        public Text groupNameText;
        public Text groupNoticeText;

        public GameObject roleContent;
        public GameObject roleItemPrefab;
        public GameObject invitePrefab;

        public Toggle remindToggle;
        public Button quitButton;
        public Button clearChatButton;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();
            groupTitle = root.FindChildByName("Text_GroupName").GetComponent<Text>();

            groupNameText = root.FindChildByName("GroupNameText").GetComponent<Text>();
            groupNoticeText = root.FindChildByName("DescribeText").GetComponent<Text>();

            roleContent = root.FindChildByName("Content");
            roleItemPrefab = root.FindChildByName("RoleInfoPrefab");
            invitePrefab = root.FindChildByName("InvitePrefab");

            remindToggle = root.FindChildByName("Toggle_Remind").GetComponent<Toggle>();
            quitButton = root.FindChildByName("Btn_Quit").GetComponent<Button>();
            clearChatButton = root.FindChildByName("Btn_Clear").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            remindToggle.onValueChanged.AddListener(listener.OnClickRemindButton);
            quitButton.onClick.AddListener(listener.OnClickQuitButton);
            clearChatButton.onClick.AddListener(listener.OnClickClearChatButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickRemindButton(bool isOn);

            void OnClickQuitButton();

            void OnClickClearChatButton();
        }
    }

    public class UI_Society_GroupMemberSetting : UIBase, UI_Society_GroupMemberSetting_Layout.IListener
    {
        private UI_Society_GroupMemberSetting_Layout layout = new UI_Society_GroupMemberSetting_Layout();

        private Sys_Society.GroupInfo groupInfo;

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
            UpdateAll(groupInfo);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnDismissGroup, OnDismissGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnChangeGroupName, OnChangeGroupName, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnChangeGroupNotice, OnChangeGroupNotice, toRegister);

            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherAddGroupSuccess, OnOtherAddGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherQuitGroupSuccess, OnOtherQuitGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherKickFromGroup, OnOtherKickFromGroup, toRegister);

            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfQuitGroupSuccess, OnSelfQuitGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfKickFromGroup, OnSelfKickFromGroup, toRegister);
        }

        void UpdateAll(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            UpdateGroupName(groupInfo);
            UpdateGroupNotice(groupInfo);
            UpdateRoleList(groupInfo);
            UpdateOther(groupInfo);
        }

        void UpdateRoleList(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            layout.roleContent.DestoryAllChildren();

            //GameObject inviteItemGo = GameObject.Instantiate(layout.invitePrefab);
            //inviteItemGo.SetActive(true);
            //UI_Society_GroupMemberSetting_Layout.InviteItem inviteItem = new UI_Society_GroupMemberSetting_Layout.InviteItem(inviteItemGo, groupInfo);
            //inviteItemGo.transform.SetParent(layout.roleContent.transform, false);

            foreach (var roleInfo in groupInfo.GetAllGroupRoleInfos().Values)
            {
                GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                roleItemGo.SetActive(true);
                UI_Society_GroupMemberSetting_Layout.RoleInfoItem roleInfoItem = new UI_Society_GroupMemberSetting_Layout.RoleInfoItem(roleItemGo);
                roleInfoItem.Update(groupInfo, roleInfo);
                roleItemGo.transform.SetParent(layout.roleContent.transform, false);
            }
        }

        void UpdateGroupName(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            TextHelper.SetText(layout.groupTitle, groupInfo.name);
            TextHelper.SetText(layout.groupNameText, groupInfo.name);
        }

        void UpdateGroupNotice(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            TextHelper.SetText(layout.groupNoticeText, groupInfo.name);
        }

        void UpdateOther(Sys_Society.GroupInfo _groupInfo)
        {
            layout.remindToggle.isOn = _groupInfo.remind;
            layout.remindToggle.onValueChanged.Invoke(layout.remindToggle.isOn);
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_GroupMemberSetting);
        }

        public void OnClickRemindButton(bool isOn)
        {
            Sys_Society.Instance.socialGroupsInfo.groupsDic[groupInfo.groupID].remind = isOn;
            Sys_Society.Instance.SerializeGroupsInfoToJsonFile();
        }

        public void OnClickQuitButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2004054, groupInfo.name);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ReqQuitGroup(groupInfo.groupID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickClearChatButton()
        {
            //TODO
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13031);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ClearGroupChatLog(groupInfo.groupID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
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

        void OnChangeGroupName(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                UpdateGroupName(_groupInfo);
            }
        }

        void OnChangeGroupNotice(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                UpdateGroupNotice(_groupInfo);
            }
        }

        void OnOtherAddGroupSuccess(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                UpdateRoleList(_groupInfo);
            }
        }

        void OnOtherQuitGroupSuccess(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                UpdateRoleList(_groupInfo);
            }
        }

        void OnOtherKickFromGroup(Sys_Society.GroupInfo _groupInfo)
        {
            if (groupInfo.groupID == _groupInfo.groupID)
            {
                UpdateRoleList(_groupInfo);
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
    }
}
