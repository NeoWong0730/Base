using DG.Tweening;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class UI_Society : UIBase, UI_Society_Layout.IListener, IChatInputEvent
    {
        private UI_Society_Layout layout = new UI_Society_Layout();
        UI_Society_RedPoint redPoint;

        private int nCaretPosition = -1;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);

            layout.inputField.onEndEdit.AddListener(OnEndEdit);

            redPoint = gameObject.AddComponent<UI_Society_RedPoint>();
            if (redPoint != null)
            {
                redPoint.Init(this);
            }
            MailInit();

            layout.recentChatContentRoot.onCreateCell += OnCreateCellRecentCell;
            layout.recentChatContentRoot.onCellChange += OnCellChangeRecentCell;

            layout.contactsChatContentRoot.onCreateCell += OnCreateCellContactsCell;
            layout.contactsChatContentRoot.onCellChange += OnCellChangeContactsCell;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnReNameSuccess, OnReNameSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnGetGiftSuccess, OnGetGiftSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnReceiveGift, OnReceiveGift, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong, uint>(Sys_Society.EEvents.OnIntimacyChange, OnIntimacyChange, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.InputChange, OnInputChange, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnSendSingleChat, OnSendSingleChat, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.ChatData>(Sys_Society.EEvents.OnGetSingleChat, OnGetSingleChat, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnSendGroupChat, OnSendGroupChat, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint, Sys_Society.ChatData>(Sys_Society.EEvents.OnGetGroupChat, OnGetGroupChat, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnAddFriendSuccess, OnAddFriendSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnFriendOnLine, OnFriendOnLine, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnFriendOffLine, OnFriendOffLine, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherAddGroupSuccess, OnOtherAddGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnOtherKickFromGroup, OnOtherKickFromGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnAddTeamMember, OnAddTeamMember, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<Sys_Society.GroupInfo>(Sys_Society.EEvents.OnChangeGroupName, OnChangeGroupName, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle(Sys_Mail.EEvents.OnAddMail, OnAddMail, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle(Sys_Mail.EEvents.OnDeleteMail, OnDeleteMail, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle<ulong>(Sys_Mail.EEvents.OnGetAttach, OnGetAttach, toRegister);
            Sys_Mail.Instance.eventEmitter.Handle(Sys_Mail.EEvents.OnGetAll, ReadAllMail, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnDelFriendSuccess, OnDelFriendSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnAddBlack, OnAddBlack, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.OnRemoveBlack, OnRemoveBlack, toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.CloseEmoji, OnCloseEmoji, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnPlayVideoSuccess, OnPlayVideoSuccess, toRegister);


            GroupProcessEventsForEnable(toRegister);
            ContactsProcessEventsForEnable(toRegister);
        }

        void OnCloseEmoji()
        {
            layout.root.transform.Find("Animator").GetComponent<RectTransform>().DOLocalMoveY(0, 0.2f);
        }

        void OnPlayVideoSuccess()
        {
            UIManager.CloseUI(EUIID.UI_Society);
        }
        void OnAddBlack()
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentFriendInfo();
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnRemoveBlack()
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentFriendInfo();
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnDelFriendSuccess()
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentRoleList();
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnAddFriendSuccess(ulong roleID)
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentRoleList();
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnAddTeamMember()
        {
            if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnClearSingleChatLog(ulong roleID)
        {
            if (layout.recentRoot.activeSelf)
            {
                if (roleID == recentCurrentSelectRoleID)
                {
                    UpdateRecentRoleChatList(roleID);
                }
            }
            else if (layout.contactsRoot.activeSelf)
            {
                if (roleID == contactsCurrentSelectRoleID)
                {
                    UpdateContactsChatList(roleID);
                }
            }
        }

        void OnReNameSuccess(ulong roleID)
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == recentCurrentSelectRoleID)
                {
                    UpdateRecentRoleChatList(roleID);
                }
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == contactsCurrentSelectRoleID)
                {
                    UpdateContactsChatList(roleID);
                }
            }
        }

        void OnFriendOnLine(ulong roleID)
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == recentCurrentSelectRoleID)
                {
                    UpdateRecentRoleChatList(roleID);
                }
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == contactsCurrentSelectRoleID)
                {
                    UpdateContactsChatList(roleID);
                }
            }
        }

        void OnFriendOffLine(ulong roleID)
        {
            if (layout.recentRoot.activeSelf)
            {
                UpdateRecentRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == recentCurrentSelectRoleID)
                {
                    UpdateRecentRoleChatList(roleID);
                }
            }
            else if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsRole(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);
                if (roleID == contactsCurrentSelectRoleID)
                {
                    UpdateContactsChatList(roleID);
                }
            }
        }

        void OnOtherAddGroupSuccess(Sys_Society.GroupInfo groupInfo)
        {
            if (layout.groupRoot.activeSelf)
            {
                UpdateGroupList();
            }
        }

        void OnChangeGroupName(Sys_Society.GroupInfo groupInfo)
        {
            if (layout.groupRoot.activeSelf)
            {
                UpdateGroupList();
            }
        }

        void OnOtherKickFromGroup(Sys_Society.GroupInfo groupInfo)
        {
            if (layout.groupRoot.activeSelf)
            {
                UpdateGroupList();
            }
        }

        protected override void OnShow()
        {
            if (Sys_Mail.Instance.mailEnterType==1)
            {
                layout.mailToggle.onClick.Invoke();
                Sys_Mail.Instance.mailEnterType = 0;
            }
            else
            {
                Sys_Society.Instance.socialRolesInfo.UpdateRoleInfoChatTime(curOpenPrivateRoleID);
                layout.recentToggle.onClick.Invoke();
            }
            if (curOpenPrivateRoleID != 0 && recent_RoleInfoItems.ContainsKey(curOpenPrivateRoleID))
            {
                recent_RoleInfoItems[curOpenPrivateRoleID].toggle.isOn = true;
                recent_RoleInfoItems[curOpenPrivateRoleID].toggle.onValueChanged.Invoke(true);

                curOpenPrivateRoleID = 0;
            }
            RefreshMailRedPoint();
            TextHelper.SetText(layout.friendLimitText, CSVLanguage.Instance.GetConfData(2004008), Sys_Society.Instance.friendsMaxCount.ToString());
        }

        public ulong curOpenPrivateRoleID;

        protected override void OnOpen(object arg)
        {            
            if (arg != null)
                curOpenPrivateRoleID = (ulong)arg;
        }

        protected override void OnHide()
        {
            Sys_Society.Instance.SerializeAllRolesInfoToJsonFile();
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society);
            Sys_Mail.Instance.bMailShowing = false;
        }

        private void OnEndEdit(string arg0)
        {
            nCaretPosition = layout.inputField.caretPosition;
        }

        public void OnChatInputClose()
        {

        }

        public int GetCaretPosition()
        {
            return nCaretPosition;
        }

        public void SetCaretPosition(int caretPosition)
        {
            nCaretPosition = caretPosition;
        }

        public void OnInputFieldValueChanged(string arg)
        {
            Sys_Society.Instance.inputCache.SetContent(arg);
        }

        void OnInputChange()
        {
            layout.inputField.SetTextWithoutNotify(Sys_Society.Instance.inputCache.GetContent());
        }

        public void OnClickAddButton()
        {
            UIManager.OpenUI(EUIID.UI_Society_FriendSearch);
        }

        public void OnClickZoneButton()
        {

        }

        /// <summary>
        /// 点击表情///
        /// </summary>
        public void OnClickEmojiButton()
        {
            UIManager.OpenUI(EUIID.UI_SocietyChatInput, true, this);
            layout.root.transform.Find("Animator").GetComponent<RectTransform>().DOLocalMoveY(275f, 0.2f);
        }

        /// <summary>
        /// 发送消息///
        /// </summary>
        public void OnClickSendButton()
        {
            if (Sys_Role.Instance.Role.Level < 15)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, "15"));              
                return;
            }

            //if (!SDKManager.GetRealNameStatus())
            //{
            //    SDKManager.GetRealNameWebRequest();
            //    return;
            //}

            string sendStr = Sys_WordInput.Instance.LimitLengthAndFilter(layout.inputField.text);

            if (string.IsNullOrWhiteSpace(sendStr))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11845));
                return;
            }

            ///最近界面///
            if (layout.recentRightRoot.activeSelf)
            {
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(recentCurrentSelectRoleID))
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11664);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Society.Instance.ReqRemoveBlackList(recentCurrentSelectRoleID);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {
                    Sys_Society.Instance.ReqChatSingle(recentCurrentSelectRoleID, sendStr);
                }
            }
            ///联系人界面///
            else if (layout.contactsRightRoot.activeSelf)
            {
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(contactsCurrentSelectRoleID))
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11664);
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        Sys_Society.Instance.ReqRemoveBlackList(contactsCurrentSelectRoleID);
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {
                    Sys_Society.Instance.ReqChatSingle(contactsCurrentSelectRoleID, sendStr);
                }
            }
            ///在群组界面///
            else if (layout.groupRightRoot.activeSelf)
            {
                if (Sys_Society.Instance.socialGroupsInfo.groupsDic.ContainsKey(groupCurrentSelectGroupID))
                {
                    Sys_Society.Instance.ReqGroupChat(groupCurrentSelectGroupID, sendStr);
                }
            }

            Sys_Society.Instance.mInputCacheRecord.RecodeInputCache(Sys_Society.Instance.inputCache);
            Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.RecodeChange);

            Sys_Society.Instance.inputCache.Clear();
            layout.inputField.SetTextWithoutNotify(Sys_Society.Instance.inputCache.GetContent());
            //layout.inputField.text = string.Empty;
        }

        /// <summary>
        /// 发送私聊消息///
        /// </summary>
        void OnSendSingleChat()
        {
            ///在最近界面///
            if (layout.recentRightRoot.activeSelf)
            {
                UpdateRecentRoleChatList(recentCurrentSelectRoleID);
            }
            ///在联系人界面///
            else if (layout.contactsRightRoot.activeSelf)
            {
                UpdateContactsChatList(contactsCurrentSelectRoleID);
            }
        }

        /// <summary>
        /// 发送群组消息///
        /// </summary>
        void OnSendGroupChat()
        {
            //再群组界面
            if (layout.groupRightRoot.activeSelf)
            {
                UpdateGroupChatList(groupCurrentSelectGroupID);
            }
        }

        /// <summary>
        /// 接受群组消息///
        /// </summary>
        void OnGetGroupChat(uint groupID, Sys_Society.ChatData chatData)
        {
            //在群组界面
            if (layout.groupRightContentRoot.activeSelf)
            {
                ///是当前选中的GroupID，直接刷新界面///
                if (groupCurrentSelectGroupID == groupID)
                {
                    UpdateGroupChatList(groupID);
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// 接受到私聊信息///
        /// </summary>
        /// <param name="chatData"></param>
        void OnGetSingleChat(Sys_Society.ChatData chatData)
        {
            ///在最近界面///
            if (layout.recentRightRoot.activeSelf)
            {
                ///是当前选中的RoleID, 直接刷新界面///
                if (recentCurrentSelectRoleID == chatData.roleID)
                {
                    UpdateRecentRoleChatList(recentCurrentSelectRoleID);
                }
                else
                {
                    UpdateRecentRoleList();
                    if (recent_RoleInfoItems.ContainsKey(recentCurrentSelectRoleID))
                    {
                        recent_RoleInfoItems[recentCurrentSelectRoleID].toggle.isOn = true;
                        recent_RoleInfoItems[recentCurrentSelectRoleID].toggle.onValueChanged.Invoke(true);
                    }
                }
            }
            ///在联系人界面///
            else if (layout.contactsRightRoot.activeSelf)
            {
                if (contactsCurrentSelectRoleID == chatData.roleID)
                {
                    UpdateContactsChatList(contactsCurrentSelectRoleID);
                }
                else
                {
                    UpdateContactsList();
                }
            }
        }

        /// <summary>
        /// 亲密度变化///
        /// </summary>
        void OnIntimacyChange(ulong roleID, uint value)
        {
            ///在最近界面///
            if (layout.recentRightRoot.activeSelf)
            {
                if (recent_RoleInfoItems.ContainsKey(roleID))
                {
                    recent_RoleInfoItems[roleID].Update(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);

                    if (recentCurrentSelectRoleID == roleID)
                    {
                        CSVFriendIntimacy.Data cSVFriendIntimacyData = CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[recentCurrentSelectRoleID].friendValue);
                        if (cSVFriendIntimacyData != null)
                        {
                            TextHelper.SetText(layout.recentFriendLevelText, cSVFriendIntimacyData.IntimacyLvlLan);
                        }
                    }
                }
            }
            ///在联系人界面///
            else if (layout.contactsRightRoot.activeSelf)
            {
                foreach (var dict in contacts_RoleInfoItems.Values)
                {
                    if (dict.ContainsKey(roleID))
                    {
                        dict[roleID].Update(Sys_Society.Instance.socialRolesInfo.rolesDic[roleID]);

                        if (contactsCurrentSelectRoleID == roleID)
                        {
                            CSVFriendIntimacy.Data cSVFriendIntimacyData = CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[contactsCurrentSelectRoleID].friendValue);
                            if (cSVFriendIntimacyData != null)
                            {
                                TextHelper.SetText(layout.contactsFriendLevelText, cSVFriendIntimacyData.IntimacyLvlLan);

                            }
                        }
                    }
                }
            }
        }

        void OnReceiveGift(ulong roleID)
        {
            if (layout.recentRightRoot.activeSelf)
            {
                if (recentCurrentSelectRoleID == roleID)
                {
                    UpdateRecentGetGiftRootVisable();
                }
            }
            else if (layout.contactsRightRoot.activeSelf)
            {
                if (contactsCurrentSelectRoleID == roleID)
                {
                    UpdateContactsGetGiftRootVisable();
                }
            }
        }

        void OnGetGiftSuccess(ulong roleID)
        {
            if (layout.recentRightRoot.activeSelf)
            {
                if (recentCurrentSelectRoleID == roleID)
                {
                    UpdateRecentGetGiftRootVisable();
                }
            }
            else if (layout.contactsRightRoot.activeSelf)
            {
                if (contactsCurrentSelectRoleID == roleID)
                {
                    UpdateContactsGetGiftRootVisable();
                }
            }
        }

        public void OnClickViewTipsButton()
        {
            layout.viewTipsRoot.SetActive(false);
        }
    }
}
