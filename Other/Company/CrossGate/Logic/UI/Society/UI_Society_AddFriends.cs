using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Society_AddFriends_Layout
    {
        public class SearchRoleInfoItem : RoleInfoItem
        {
            public SearchRoleInfoItem(GameObject gameObject) : base(gameObject)
            {

            }

            public override void Update(Sys_Society.RoleInfo _roleInfo)
            {
                base.Update(_roleInfo);

                reason.gameObject.SetActive(false);
            }
        }

        public class RoleInfoItem
        {
            public GameObject root;

            public Image roleIcon;
            public Image roleIconFrame;
            public Text reason;
            public Text roleName;
            public Text roleLv;

            public Button button;

            Sys_Society.RoleInfo roleInfo;

            public RoleInfoItem(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                roleIconFrame = root.FindChildByName("Image_Frame").GetComponent<Image>();
                reason = root.FindChildByName("ReasonText").GetComponent<Text>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Level").GetComponent<Text>();

                button = root.FindChildByName("Image_BG_Button").GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);
            }

            public virtual void Update(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                //TODO: REASON;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                roleIconFrame.SetNativeSize();
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
            }

            void OnClickButton()
            {
                ///是自己///
                if (roleInfo.roleID == Sys_Role.Instance.RoleId)
                {
                    //TODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13036));
                    return;
                }

                ///已经是好友了///
                if (Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID))
                {
                    //TODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13035));
                    return;
                }

                ///在我的黑名单中///
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(roleInfo.roleID))
                {
                    //TODO
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13034));
                    return;
                }

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2002093).words;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Society.Instance.ReqAddFriend(roleInfo.roleID);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public GameObject root;

        public Button closeButton;
        public Button reRollButton;
        public Button searchButton;
        public InputField searchInput;

        public GameObject roleContent;
        public GameObject roleItemPrefab;

        public GameObject noTip;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();
            reRollButton = root.FindChildByName("Button_Change").GetComponent<Button>();
            searchButton = root.FindChildByName("Button_Search").GetComponent<Button>();
            searchInput = root.FindChildByName("InputField_Describe").GetComponent<InputField>();

            roleContent = root.FindChildByName("Viewport");
            roleItemPrefab = root.FindChildByName("RoleItem");

            noTip = root.FindChildByName("View_Tips");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            reRollButton.onClick.AddListener(listener.OnClickReRollButton);
            searchButton.onClick.AddListener(listener.OnClickSearchButton);
            searchInput.onValueChanged.AddListener(listener.OnSearchInputValueChanged);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickReRollButton();

            void OnClickSearchButton();

            void OnSearchInputValueChanged(string str);
        }
    }

    public class UI_Society_AddFriends : UIBase, UI_Society_AddFriends_Layout.IListener
    {
        private UI_Society_AddFriends_Layout layout = new UI_Society_AddFriends_Layout();

        Dictionary<ulong, Sys_Society.RoleInfo> curRecommendRoleInfos = new Dictionary<ulong, Sys_Society.RoleInfo>();

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            layout.roleContent.DestoryAllChildren();
            layout.searchInput.text = string.Empty;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Society.EEvents.OnGetBriefInfoSuccess, OnGetBriefInfoSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnAddFriendSuccess, OnAddFriendSuccess, toRegister);
        }

        void UpdateRoleListByRecommend(Dictionary<ulong, Sys_Society.RoleInfo> roleInfos)
        {
            curRecommendRoleInfos = roleInfos;

            layout.roleContent.DestoryAllChildren();

            foreach (var roleInfo in roleInfos.Values)
            {
                GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                roleItemGo.SetActive(true);
                UI_Society_AddFriends_Layout.RoleInfoItem roleInfoItem = new UI_Society_AddFriends_Layout.RoleInfoItem(roleItemGo);
                roleInfoItem.Update(roleInfo);

                roleItemGo.transform.SetParent(layout.roleContent.transform, false);
            }
        }

        void UpdateRoleListBySearch(List<Sys_Society.RoleInfo> roleInfos)
        {
            layout.roleContent.DestoryAllChildren();

            if (roleInfos != null)
            {
                foreach (var roleInfo in roleInfos)
                {
                    GameObject roleItemGo = GameObject.Instantiate(layout.roleItemPrefab);
                    roleItemGo.SetActive(true);
                    UI_Society_AddFriends_Layout.SearchRoleInfoItem roleInfoItem = new UI_Society_AddFriends_Layout.SearchRoleInfoItem(roleItemGo);
                    roleInfoItem.Update(roleInfo);

                    roleItemGo.transform.SetParent(layout.roleContent.transform, false);
                }

                layout.noTip.SetActive(roleInfos.Count <= 0);
            }
            else
            {
                layout.noTip.SetActive(true);
            }
        }

        public void OnClickCloseButton()
        {
            //UIManager.CloseUI(EUIID.UI_Society_AddFriends);
        }

        public void OnClickReRollButton()
        {
            //TODO
        }

        public void OnClickSearchButton()
        {
            if (string.IsNullOrWhiteSpace(layout.searchInput.text))
            {
                //TODO
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13033));
                return;
            }
            Sys_Society.Instance.ReqGetBriefInfo(layout.searchInput.text);
        }

        public void OnSearchInputValueChanged(string str)
        {
            layout.searchInput.text = Sys_WordInput.Instance.LimitLengthAndFilter(str);
        }

        /// <summary>
        /// 获取玩家详细信息回调(搜索)///
        /// </summary>
        /// <param name="roleInfo">玩家详细信息</param>
        void OnGetBriefInfoSuccess(Sys_Society.RoleInfo roleInfo)
        {
            List<Sys_Society.RoleInfo> roleInfos = new List<Sys_Society.RoleInfo>();
            roleInfos.Add(roleInfo);

            UpdateRoleListBySearch(roleInfos);
        }

        /// <summary>
        /// 添加好友成功回调///
        /// </summary>
        /// <param name="roleInfo">添加的好友</param>
        void OnAddFriendSuccess(ulong roleID)
        {
            if (curRecommendRoleInfos.ContainsKey(roleID))
            {
                curRecommendRoleInfos.Remove(roleID);
            }

            UpdateRoleListByRecommend(curRecommendRoleInfos);
        }
    }
}