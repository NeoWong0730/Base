using Lib.Core;
using Logic.Core;
using UnityEngine;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class UI_Society : UIBase, UI_Society_Layout.IListener
    {
        ulong recentCurrentSelectRoleID;
        List<Sys_Society.ChatData> mChatDatasRecent = new List<Sys_Society.ChatData>();
        Dictionary<ulong, UI_Society_Layout.Recent_RoleInfoItem> recent_RoleInfoItems = new Dictionary<ulong, UI_Society_Layout.Recent_RoleInfoItem>();
        UI_Society_Layout.Recent_MOLIInfeoItem moliInfiItem;

        public void OnClickRecentToggle()
        {
            layout.recentToggle_Light.SetActive(true);
            layout.recentToggle_Dark.SetActive(false);

            layout.groupToggle_Light.SetActive(false);
            layout.groupToggle_Dark.SetActive(true);

            layout.contactsToggle_Light.SetActive(false);
            layout.contactsToggle_Dark.SetActive(true);

            layout.mailToggle_Light.SetActive(false);
            layout.mailToggle_Dark.SetActive(true);

            layout.recentRoot.SetActive(true);
            layout.groupRoot.SetActive(false);
            layout.contactsRoot.SetActive(false);
            layout.mailRoot.SetActive(false);
            layout.buttonRoot.SetActive(true);

            UpdateRecentRoleList();

            recentCurrentSelectRoleID = 0;

            layout.recentRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.mailRightRoot.SetActive(false);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(true);
            TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
            layout.inputRoot.SetActive(false);

            moliInfiItem.toggle.isOn = true;
            moliInfiItem.toggle.onValueChanged.Invoke(true);
        }

        void UpdateRecentRole(Sys_Society.RoleInfo roleInfo)
        {
            if (recent_RoleInfoItems.ContainsKey(roleInfo.roleID))
                recent_RoleInfoItems[roleInfo.roleID].Update(roleInfo);
        }

        void UpdateRecentRoleList()
        {
            foreach (var recent_RoleInfoItem in recent_RoleInfoItems.Values)
            {
                recent_RoleInfoItem.Dispose();
            }
            recent_RoleInfoItems.Clear();
            layout.recentContentRoot.DestoryAllChildren();

            GameObject systemInfoItemGo = GameObject.Instantiate(layout.recent_systemInfoPrefab);
            systemInfoItemGo.SetActive(true);
            UI_Society_Layout.Recent_SystemInfoItem systemInfoItem = new UI_Society_Layout.Recent_SystemInfoItem(systemInfoItemGo);
            systemInfoItem.chooseRecentRoleInfoItem = (id) =>
            {
                recentCurrentSelectRoleID = id;

                layout.recentRightRoot.SetActive(true);
                layout.rightNoneRoot.SetActive(false);
                layout.rightMOLIRoot.SetActive(false);
                layout.inputRoot.SetActive(false);

                layout.recentGetGiftRoot.SetActive(false);
                layout.recentFriendTopRoot.SetActive(false);
                layout.recentNotFriendTopRoot.SetActive(false);
                layout.recentBlackTopRoot.SetActive(false);

                if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                    Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                UpdateRecentRoleChatList(id);
                Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, Sys_Society.socialSystemID);
            };
            systemInfoItem.Update();
            systemInfoItemGo.transform.SetParent(layout.recentContentRoot.transform, false);

            GameObject moliInfoItemGo = GameObject.Instantiate(layout.recent_moliInfoPrefab);
            moliInfoItemGo.SetActive(true);
            moliInfiItem = new UI_Society_Layout.Recent_MOLIInfeoItem(moliInfoItemGo);
            moliInfiItem.chooseRecentRoleInfoItem = (id) =>
            {
                recentCurrentSelectRoleID = id;

                layout.recentRightRoot.SetActive(false);

                //layout.rightMOLIRoot.SetActive(true);
                //layout.rightNoneRoot.SetActive(false);
                //foreach (uint key in CSVMoliSpirit.Instance.GetKeys())
                //{
                //    if (key != 1)
                //    {
                //        GameObject moliItem = GameObject.Instantiate<GameObject>(layout.rightMOLIButtonPrefab);
                //        UI_Society_Layout.MOLIButton mOLIButton = new UI_Society_Layout.MOLIButton(moliItem, key);
                //        mOLIButton.gameObject.transform.SetParent(layout.rightMOLIButtonRoot.transform, false);
                //    }
                //}

                layout.rightMOLIRoot.SetActive(false);
                layout.rightNoneRoot.SetActive(true);

                TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
                layout.inputRoot.SetActive(false);              
            };
            moliInfoItemGo.transform.SetParent(layout.recentContentRoot.transform, false);

            foreach (Sys_Society.RoleInfo roleInfo in Sys_Society.Instance.socialRecentlysInfo.GetAllSortedRecentlysInfos())
            {
                GameObject roleInfoItemGo = GameObject.Instantiate(layout.recent_roleInfoPrefab);
                roleInfoItemGo.SetActive(true);
                UI_Society_Layout.Recent_RoleInfoItem roleInfoItem = new UI_Society_Layout.Recent_RoleInfoItem(roleInfoItemGo);
                roleInfoItem.chooseRecentRoleInfoItem = (id) =>
                {
                    recentCurrentSelectRoleID = id;

                    layout.recentRightRoot.SetActive(true);
                    layout.rightNoneRoot.SetActive(false);
                    layout.rightMOLIRoot.SetActive(false);
                    layout.inputRoot.SetActive(true);

                    UpdateRecentFriendInfo();

                    if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                        Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                    UpdateRecentRoleChatList(id);
                    Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, roleInfo.roleID);
                };
                roleInfoItem.Update(roleInfo);
                roleInfoItemGo.transform.SetParent(layout.recentContentRoot.transform, false);
                recent_RoleInfoItems.Add(roleInfo.roleID, roleInfoItem);
            }
        }

        void OnCreateCellRecentCell(InfinityGridCell cell)
        {
            UI_Society_Layout.ChatItem chatItem = new UI_Society_Layout.ChatItem();
            chatItem.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(chatItem);
        }

        void OnCellChangeRecentCell(InfinityGridCell cell, int index)
        {
            if (mChatDatasRecent != null)
            {
                UI_Society_Layout.ChatItem chatItem = cell.mUserData as UI_Society_Layout.ChatItem;
                chatItem.SetData(mChatDatasRecent[index]);
            }
        }

        void UpdateRecentFriendInfo()
        {
            if (Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(recentCurrentSelectRoleID))
            {
                layout.recentFriendTopRoot.SetActive(true);
                layout.recentNotFriendTopRoot.SetActive(false);
                layout.recentBlackTopRoot.SetActive(false);

                CSVFriendIntimacy.Data cSVFriendIntimacyData = CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[recentCurrentSelectRoleID].friendValue);
                if (cSVFriendIntimacyData != null)
                {
                    TextHelper.SetText(layout.recentFriendLevelText, cSVFriendIntimacyData.IntimacyLvlLan);
                }
            }
            else
            {
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(recentCurrentSelectRoleID))
                {
                    layout.recentFriendTopRoot.SetActive(false);
                    layout.recentNotFriendTopRoot.SetActive(false);
                    layout.recentBlackTopRoot.SetActive(true);
                }
                else
                {
                    layout.recentFriendTopRoot.SetActive(false);
                    layout.recentNotFriendTopRoot.SetActive(true);
                    layout.recentBlackTopRoot.SetActive(false);
                }
            }

            UpdateRecentGetGiftRootVisable();
        }

        void UpdateRecentGetGiftRootVisable()
        {
            if (Sys_Society.Instance.socialRolesInfo.rolesDic.ContainsKey(recentCurrentSelectRoleID))
            {
                if (Sys_Society.Instance.socialRolesInfo.rolesDic[recentCurrentSelectRoleID].hasGift)
                {
                    layout.recentGetGiftRoot.SetActive(true);
                }
                else
                {
                    layout.recentGetGiftRoot.SetActive(false);
                }
            }
        }

        void UpdateRecentRoleChatList(ulong roleId)
        {
            layout.recentChatContentRoot.Clear();

            if (Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(roleId))
            {
                mChatDatasRecent = Sys_Society.Instance.roleChatDataInfo.roleChatDatas[roleId].chatsInfo;
            }
            else
            {
                mChatDatasRecent = null;
            }

            if (mChatDatasRecent != null)
            {
                for (int index = 0, len = mChatDatasRecent.Count; index < len; index++)
                {
                    layout.recentChatContentRoot.Add(CalculateSizeRoleChat(mChatDatasRecent[index]));
                }
                layout.recentChatContentRoot.Update();
                layout.recentChatContentRoot.ScrollView.verticalNormalizedPosition = 0 ;
            }
        }

        int CalculateSizeRoleChat(Sys_Society.ChatData chatData)
        {
            if (chatData.roleID == Sys_Society.socialSystemTipID)
            {
                return 50;
            }
            else
            {
                int h = UI_Society_Layout.ChatItem.gTop + UI_Society_Layout.ChatItem.contentTop;
                TextHelper.SetText(layout.roleChatText, chatData.content, CSVWordStyle.Instance.GetConfData(CSVChatWord.Instance.GetConfData(chatData.chatTextID).WordIcon));
                h += Mathf.Max((int)layout.roleChatText.preferredHeight, UI_Society_Layout.ChatItem.contentMinHeight);
                h += UI_Society_Layout.ChatItem.contentBottom;
                h += UI_Society_Layout.ChatItem.contentTop;
                return h;
            }
        }
      
        public void OnClickRecentFriendDeleteButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13031);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ClearSingleChatLog(recentCurrentSelectRoleID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickRecentNoFriendDeleteButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13031);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ClearSingleChatLog(recentCurrentSelectRoleID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickRecentSendGiftButton()
        {
            UIManager.OpenUI(EUIID.UI_SendGift, false, new List<ulong> { 0, recentCurrentSelectRoleID, 0 });
        }

        public void OnClickRecentFriendTipButton()
        {
            layout.viewTipsRoot.SetActive(true);
        }

        public void OnClickRecentGetGiftButton()
        {
            if (recentCurrentSelectRoleID != 0)
            {
                Sys_Society.Instance.GetGift(recentCurrentSelectRoleID);
            }
        }
    }
}
