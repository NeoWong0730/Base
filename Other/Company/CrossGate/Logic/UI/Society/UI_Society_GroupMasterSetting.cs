using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Society_GroupMasterSetting_Layout
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

            public Button delButton;

            Sys_Society.RoleInfo roleInfo;

            public RoleInfoItem(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                ownerImage = root.FindChildByName("Image_Owner").GetComponent<Image>();
                delButton = root.FindChildByName("DelBtn").GetComponent<Button>();
                delButton.onClick.AddListener(OnClickButton);
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

            void OnClickButton()
            {
                if (groupInfo.leader == roleInfo.roleID)
                {
                    Sys_Hint.Instance.PushContent_Normal("不能踢群主");
                    return;
                }

                PromptBoxParameter.Instance.Clear();
                //TODO
                PromptBoxParameter.Instance.content = "确定要踢掉么";
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Society.Instance.ReqKickMember(groupInfo.groupID, roleInfo.roleID);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public class InviteItem : RoleInfoItemBase
        {
            public Button button;

            public InviteItem(GameObject gameObject, Sys_Society.GroupInfo _groupInfo)
            {
                root = gameObject;

                button = root.FindChildByName("Image_BG").GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);

                groupInfo = _groupInfo;
            }

            void OnClickButton()
            {
                UIManager.OpenUI(EUIID.UI_Society_InviteFriends, false, groupInfo);
            }
        }

        public GameObject root;

        public Button closeButton;

        public Text groupTitle;
        public InputField groupNameInput;
        public Button changeNameButton;

        public InputField groupNoticeInput;
        public Button changeNoticeButton;

        public GameObject roleContent;
        public GameObject roleItemPrefab;
        public GameObject invitePrefab;

        public Toggle remindToggle;
        public Button dissmissButton;
        public Button clearChatButton;

        public GameObject changeNameView;
        public InputField changeNameViewInput;
        public Button changeNameSureButton;
        public Button changeNameCancelButton;

        public GameObject changeDesView;
        public InputField changeDesViewInput;
        public Button changeDesSureButton;
        public Button changeDesCancelButton;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();
            groupTitle = root.FindChildByName("Text_GroupName").GetComponent<Text>();
            groupNameInput = root.FindChildByName("InputField_GroupName").GetComponent<InputField>();
            changeNameButton = groupNameInput.gameObject.FindChildByName("Button").GetComponent<Button>();
            groupNoticeInput = root.FindChildByName("InputField_Describe").GetComponent<InputField>();
            changeNoticeButton = groupNoticeInput.gameObject.FindChildByName("Button").GetComponent<Button>();

            roleContent = root.FindChildByName("Content");
            roleItemPrefab = root.FindChildByName("RoleInfoPrefab");
            invitePrefab = root.FindChildByName("InvitePrefab");

            remindToggle = root.FindChildByName("Toggle_Remind").GetComponent<Toggle>();
            dissmissButton = root.FindChildByName("Btn_Dismiss").GetComponent<Button>();
            clearChatButton = root.FindChildByName("Btn_Clear").GetComponent<Button>();

            changeNameView = root.FindChildByName("View_Change");
            changeNameView.SetActive(false);
            changeNameViewInput = changeNameView.FindChildByName("InputField").GetComponent<InputField>();
            changeNameSureButton = changeNameView.FindChildByName("Button_OK").GetComponent<Button>();
            changeNameCancelButton = changeNameView.FindChildByName("Button_Cancel").GetComponent<Button>();

            changeDesView = root.FindChildByName("View_ChangeDes");
            changeDesView.SetActive(false);
            changeDesViewInput = changeDesView.FindChildByName("InputField_Describe").GetComponent<InputField>();
            changeDesSureButton = changeDesView.FindChildByName("Button_Modify").GetComponent<Button>();
            changeDesCancelButton = changeDesView.FindChildByName("Btn_Close").GetComponent<Button>();

        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            changeNameButton.onClick.AddListener(listener.OnClickChangeNameButton);
            changeNoticeButton.onClick.AddListener(listener.OnClickChangeNoticeButton);
            remindToggle.onValueChanged.AddListener(listener.OnClickRemindButton);
            dissmissButton.onClick.AddListener(listener.OnClickDismissButton);
            clearChatButton.onClick.AddListener(listener.OnClickClearChatButton);
            groupNameInput.onValueChanged.AddListener(listener.OnGroupNameInputValueChanged);
            groupNoticeInput.onValueChanged.AddListener(listener.OnGroupNoticeInputValueChanged);
            changeNameViewInput.onValueChanged.AddListener(listener.OnChangeNameViewInputValueChanged);
            changeNameSureButton.onClick.AddListener(listener.OnClickChangeNameSureButton);
            changeNameCancelButton.onClick.AddListener(listener.onClickChangeNameCancelButton);
            changeDesViewInput.onValueChanged.AddListener(listener.OnChangeDesViewInputValueChanged);
            changeDesSureButton.onClick.AddListener(listener.OnClickChangeDesSureButton);
            changeDesCancelButton.onClick.AddListener(listener.onClickChangeDesCancelButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickChangeNameButton();

            void OnClickChangeNoticeButton();

            void OnClickRemindButton(bool isOn);

            void OnClickDismissButton();

            void OnClickClearChatButton();

            void OnGroupNameInputValueChanged(string str);

            void OnGroupNoticeInputValueChanged(string str);

            void OnChangeNameViewInputValueChanged(string str);

            void OnClickChangeNameSureButton();

            void onClickChangeNameCancelButton();

            void OnChangeDesViewInputValueChanged(string str);

            void OnClickChangeDesSureButton();

            void onClickChangeDesCancelButton();
        }
    }

    public class UI_Society_GroupMasterSetting : UIBase, UI_Society_GroupMasterSetting_Layout.IListener
    {
        private UI_Society_GroupMasterSetting_Layout layout = new UI_Society_GroupMasterSetting_Layout();

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
        }

        void UpdateAll(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            UpdateGroupName(groupInfo);
            UpdateGroupNotice(groupInfo);
            UpdateRoleList(groupInfo);
            UpdateOther(_groupInfo);
        }

        void UpdateRoleList(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            layout.roleContent.DestoryAllChildren();

            GameObject inviteItemGo = GameObject.Instantiate(layout.invitePrefab);
            inviteItemGo.SetActive(true);
            UI_Society_GroupMasterSetting_Layout.InviteItem inviteItem = new UI_Society_GroupMasterSetting_Layout.InviteItem(inviteItemGo, groupInfo);
            inviteItemGo.transform.SetParent(layout.roleContent.transform, false);

            foreach (var roleInfo in groupInfo.GetAllGroupRoleInfos().Values)
            {
                GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                roleItemGo.SetActive(true);
                UI_Society_GroupMasterSetting_Layout.RoleInfoItem roleInfoItem = new UI_Society_GroupMasterSetting_Layout.RoleInfoItem(roleItemGo);
                roleInfoItem.Update(groupInfo, roleInfo);
                roleItemGo.transform.SetParent(layout.roleContent.transform, false);
            }
        }

        void UpdateGroupName(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            TextHelper.SetText(layout.groupTitle, groupInfo.name);
            layout.groupNameInput.text = groupInfo.name;
        }

        void UpdateGroupNotice(Sys_Society.GroupInfo _groupInfo)
        {
            groupInfo = _groupInfo;

            layout.groupNoticeInput.text = groupInfo.notice;
        }

        void UpdateOther(Sys_Society.GroupInfo _groupInfo)
        {
            layout.remindToggle.isOn = _groupInfo.remind;
            layout.remindToggle.onValueChanged.Invoke(layout.remindToggle.isOn);
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_GroupMasterSetting);
        }

        public void OnClickChangeNameButton()
        {
            layout.changeNameView.SetActive(true);         
        }

        public void OnClickChangeNameSureButton()
        {
            if (layout.changeNameViewInput.text.Length <= 1)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11652));
                return;
            }

            Sys_Society.Instance.ReqGroupChangeName(groupInfo.groupID, layout.changeNameViewInput.text);
        }

        public void onClickChangeNameCancelButton()
        {
            layout.changeNameViewInput.text = string.Empty;
            layout.changeNameView.SetActive(false);
        }

        public void OnClickChangeNoticeButton()
        {
            layout.changeDesView.SetActive(true);
        }

        public void OnClickChangeDesSureButton()
        {
            string str = Sys_WordInput.Instance.LimitLengthAndFilter(layout.changeDesViewInput.text);
            Sys_Society.Instance.ReqSetGroupNotice(groupInfo.groupID, str);
        }

        public void onClickChangeDesCancelButton()
        {
            layout.changeDesViewInput.text = string.Empty;
            layout.changeDesView.SetActive(false);
        }

        public void OnClickRemindButton(bool isOn)
        {
            Sys_Society.Instance.socialGroupsInfo.groupsDic[groupInfo.groupID].remind = isOn;
            Sys_Society.Instance.SerializeGroupsInfoToJsonFile();            
        }

        public void OnClickDismissButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2004051, groupInfo.name);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ReqDestroyGroup(groupInfo.groupID);
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

        public void OnGroupNameInputValueChanged(string Namestr)
        {
            layout.groupNameInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(Namestr);
        }

        public void OnGroupNoticeInputValueChanged(string noticeStr)
        {
            layout.groupNoticeInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(noticeStr);
        }

        public void OnChangeNameViewInputValueChanged(string Namestr)
        {
            layout.changeNameViewInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(Namestr);
        }

        public void OnChangeDesViewInputValueChanged(string Namestr)
        {
            layout.changeDesViewInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(Namestr);
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
    }
}
