using Lib.Core;
using Logic.Core;
using UnityEngine;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class UI_Society : UIBase, UI_Society_Layout.IListener
    {
        ulong contactsCurrentSelectRoleID;
        List<Sys_Society.ChatData> mChatDatasContacts = new List<Sys_Society.ChatData>();
        Dictionary<string, Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>> contacts_RoleInfoItems = new Dictionary<string, Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>>();
        public static Dictionary<string, bool> friendGroupSelectFlags = new Dictionary<string, bool>();

        public void OnClickContactsToggle()
        {
            layout.recentToggle_Light.SetActive(false);
            layout.recentToggle_Dark.SetActive(true);

            layout.groupToggle_Light.SetActive(false);
            layout.groupToggle_Dark.SetActive(true);

            layout.contactsToggle_Light.SetActive(true);
            layout.contactsToggle_Dark.SetActive(false);

            layout.mailToggle_Light.SetActive(false);
            layout.mailToggle_Dark.SetActive(true);

            layout.recentRoot.SetActive(false);
            layout.groupRoot.SetActive(false);
            layout.contactsRoot.SetActive(true);
            layout.mailRoot.SetActive(false);
            layout.buttonRoot.SetActive(true);

            ResetFriendGroupSelectFlags();

            UpdateContactsList();

            contactsCurrentSelectRoleID = 0;

            layout.recentRightRoot.SetActive(false);
            layout.contactsRightRoot.SetActive(false);
            layout.groupRightRoot.SetActive(false);
            layout.mailRightRoot.SetActive(false);
            layout.rightMOLIRoot.SetActive(false);
            layout.rightNoneRoot.SetActive(true);
            TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
            layout.inputRoot.SetActive(false);
        }

        void UpdateContactsRole(Sys_Society.RoleInfo roleInfo)
        {
            foreach (var dict in contacts_RoleInfoItems.Values)
            {
                if (dict.ContainsKey(roleInfo.roleID))
                    dict[roleInfo.roleID].Update(roleInfo);
            }
            
        }

        void UpdateContactsList()
        {
            foreach (var dict in contacts_RoleInfoItems.Values)
            {
                foreach (var item in dict.Values)
                {
                    item.Dispose();
                }
            }
            contacts_RoleInfoItems.Clear();
            layout.contactsContentRoot.DestoryAllChildren();

            GameObject moliInfoItemGo = GameObject.Instantiate(layout.contacts_moliInfoPrefab);
            moliInfoItemGo.SetActive(true);
            UI_Society_Layout.Contacts_MOLIInfoItem moliInfoItem = new UI_Society_Layout.Contacts_MOLIInfoItem(moliInfoItemGo);
            moliInfoItem.chooseContactsRoleInfoItem = (id) =>
            {
                contactsCurrentSelectRoleID = id;

                layout.contactsRightRoot.SetActive(false);
                layout.rightMOLIRoot.SetActive(false);
                layout.rightNoneRoot.SetActive(true);
                TextHelper.SetText(layout.rightNoneText, LanguageHelper.GetTextContent(2004005));
                layout.inputRoot.SetActive(false);
            };
            moliInfoItemGo.transform.SetParent(layout.contactsContentRoot.transform, false);

            foreach (Sys_Society.FriendGroupInfo friendGroupInfo in Sys_Society.Instance.socialFriendGroupsInfo.friendGroupInfosDic.Values)
            {
                GameObject customFriendGroupGo = GameObject.Instantiate(layout.contacts_groupPrefab);
                customFriendGroupGo.SetActive(true);
                UI_Society_Layout.Contacts_CustomFriendGroup customFriendGroup = new UI_Society_Layout.Contacts_CustomFriendGroup(customFriendGroupGo);
                customFriendGroup.OnClickFriendGroupItem = (name) =>
                {
                    if (friendGroupSelectFlags[name])
                    {
                        friendGroupSelectFlags[name] = false;
                    }
                    else
                    {
                        friendGroupSelectFlags[name] = true;
                    }
                    UpdateContactsList();
                };
                customFriendGroup.Update(friendGroupInfo);
                customFriendGroupGo.transform.SetParent(layout.contactsContentRoot.transform, false);

                if (friendGroupSelectFlags[friendGroupInfo.name])
                {
                    foreach (Sys_Society.RoleInfo roleInfo in friendGroupInfo.GetAllSortedRoleInfos())
                    {
                        GameObject roleInfoItemGo = GameObject.Instantiate(layout.contacts_roleInfoPrefab);
                        roleInfoItemGo.SetActive(true);
                        UI_Society_Layout.Contacts_RoleInfoItem roleInfoItem = new UI_Society_Layout.Contacts_RoleInfoItem(roleInfoItemGo);
                        roleInfoItem.chooseContactsRoleInfoItem = (id) =>
                        {
                            contactsCurrentSelectRoleID = id;

                            layout.contactsRightRoot.SetActive(true);
                            layout.rightMOLIRoot.SetActive(false);
                            layout.rightNoneRoot.SetActive(false);
                            layout.inputRoot.SetActive(true);

                            if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                                Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                            UpdateContactsFriendInfo();
                            UpdateContactsChatList(id);
                            Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, roleInfo.roleID);
                        };
                        roleInfoItem.Update(roleInfo);
                        roleInfoItemGo.transform.SetParent(layout.contactsContentRoot.transform, false);
                        if (!contacts_RoleInfoItems.ContainsKey(friendGroupInfo.name))
                        {
                            contacts_RoleInfoItems[friendGroupInfo.name] = new Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>();
                        }
                        contacts_RoleInfoItems[friendGroupInfo.name].Add(roleInfo.roleID, roleInfoItem);
                    }
                }
            }

            GameObject myFriendGroupGo = GameObject.Instantiate(layout.contacts_groupPrefab);
            myFriendGroupGo.SetActive(true);
            UI_Society_Layout.Contacts_MyFriendGroup myFriendGroup = new UI_Society_Layout.Contacts_MyFriendGroup(myFriendGroupGo);
            myFriendGroup.OnClickFriendGroupItem = (name) =>
            {
                if (friendGroupSelectFlags[name])
                {
                    friendGroupSelectFlags[name] = false;
                }
                else
                {
                    friendGroupSelectFlags[name] = true;
                }
                UpdateContactsList();
            };
            myFriendGroupGo.transform.SetParent(layout.contactsContentRoot.transform, false);

            if (friendGroupSelectFlags[Sys_Society.Instance.MYFRIEND])
            {
                foreach (Sys_Society.RoleInfo roleInfo in Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos())
                {
                    GameObject roleInfoItemGo = GameObject.Instantiate(layout.contacts_roleInfoPrefab);
                    roleInfoItemGo.SetActive(true);
                    UI_Society_Layout.Contacts_RoleInfoItem roleInfoItem = new UI_Society_Layout.Contacts_RoleInfoItem(roleInfoItemGo);
                    roleInfoItem.chooseContactsRoleInfoItem = (id) =>
                    {
                        contactsCurrentSelectRoleID = id;

                        layout.contactsRightRoot.SetActive(true);
                        layout.rightMOLIRoot.SetActive(false);
                        layout.rightNoneRoot.SetActive(false);
                        layout.inputRoot.SetActive(true);

                        if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                            Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                        UpdateContactsFriendInfo();
                        UpdateContactsChatList(id);
                        Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, roleInfo.roleID);
                    };
                    roleInfoItem.Update(roleInfo);
                    roleInfoItemGo.transform.SetParent(layout.contactsContentRoot.transform, false);
                    if (!contacts_RoleInfoItems.ContainsKey(Sys_Society.Instance.MYFRIEND))
                    {
                        contacts_RoleInfoItems[Sys_Society.Instance.MYFRIEND] = new Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>();
                    }
                    contacts_RoleInfoItems[Sys_Society.Instance.MYFRIEND].Add(roleInfo.roleID, roleInfoItem);
                }
            }

            GameObject recentTeamMemberGroupGo = GameObject.Instantiate(layout.contacts_groupPrefab);
            recentTeamMemberGroupGo.SetActive(true);
            UI_Society_Layout.Contacts_RecentTeamMemberGroup recentTeamMemberGroup = new UI_Society_Layout.Contacts_RecentTeamMemberGroup(recentTeamMemberGroupGo);
            recentTeamMemberGroup.OnClickFriendGroupItem = (name) =>
            {
                if (friendGroupSelectFlags[name])
                {
                    friendGroupSelectFlags[name] = false;
                }
                else
                {
                    friendGroupSelectFlags[name] = true;
                }
                UpdateContactsList();
            };
            recentTeamMemberGroupGo.transform.SetParent(layout.contactsContentRoot.transform, false);

            if (friendGroupSelectFlags[Sys_Society.Instance.TEAMPLAYER])
            {
                foreach (Sys_Society.RoleInfo roleInfo in Sys_Society.Instance.socialTeamMembersInfo.GetAllSortedTeamMemberInfos())
                {
                    GameObject roleInfoItemGo = GameObject.Instantiate(layout.contacts_roleInfoPrefab);
                    roleInfoItemGo.SetActive(true);
                    UI_Society_Layout.Contacts_RoleInfoItem roleInfoItem = new UI_Society_Layout.Contacts_RoleInfoItem(roleInfoItemGo);
                    roleInfoItem.chooseContactsRoleInfoItem = (id) =>
                    {
                        contactsCurrentSelectRoleID = id;

                        layout.contactsRightRoot.SetActive(true);
                        layout.rightMOLIRoot.SetActive(false);
                        layout.rightNoneRoot.SetActive(false);
                        layout.inputRoot.SetActive(true);

                        if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                            Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                        UpdateContactsFriendInfo();
                        UpdateContactsChatList(id);
                        Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, roleInfo.roleID);
                    };
                    roleInfoItem.Update(roleInfo);
                    roleInfoItemGo.transform.SetParent(layout.contactsContentRoot.transform, false);
                    if (!contacts_RoleInfoItems.ContainsKey(Sys_Society.Instance.TEAMPLAYER))
                    {
                        contacts_RoleInfoItems[Sys_Society.Instance.TEAMPLAYER] = new Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>();
                    }
                    contacts_RoleInfoItems[Sys_Society.Instance.TEAMPLAYER].Add(roleInfo.roleID, roleInfoItem);
                }
            }

            GameObject blackListGroupGo = GameObject.Instantiate(layout.contacts_groupPrefab);
            blackListGroupGo.SetActive(true);
            UI_Society_Layout.Contacts_BlackListGroup blackListGroup = new UI_Society_Layout.Contacts_BlackListGroup(blackListGroupGo);
            blackListGroup.OnClickFriendGroupItem = (name) =>
            {
                if (friendGroupSelectFlags[name])
                {
                    friendGroupSelectFlags[name] = false;
                }
                else
                {
                    friendGroupSelectFlags[name] = true;
                }
                UpdateContactsList();
            };
            blackListGroupGo.transform.SetParent(layout.contactsContentRoot.transform, false);

            if (friendGroupSelectFlags[Sys_Society.Instance.BLACK])
            {
                foreach (Sys_Society.RoleInfo roleInfo in Sys_Society.Instance.socialBlacksInfo.GetAllSortedBlacksInfos())
                {
                    GameObject roleInfoItemGo = GameObject.Instantiate(layout.contacts_roleInfoPrefab);
                    roleInfoItemGo.SetActive(true);
                    UI_Society_Layout.Contacts_RoleInfoItem roleInfoItem = new UI_Society_Layout.Contacts_RoleInfoItem(roleInfoItemGo);

                    roleInfoItem.chooseContactsRoleInfoItem = (id) =>
                    {
                        contactsCurrentSelectRoleID = id;

                        layout.contactsRightRoot.SetActive(true);
                        layout.rightMOLIRoot.SetActive(false);
                        layout.rightNoneRoot.SetActive(false);
                        layout.inputRoot.SetActive(true);

                        if (!Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(id))
                            Sys_Society.Instance.DeserializeChatsInfoFromJsonFile(string.Format(Sys_Society.Instance.chatsPath, Sys_Role.Instance.RoleId.ToString(), id.ToString()));

                        UpdateContactsFriendInfo();
                        UpdateContactsChatList(id);
                        Sys_Society.Instance.eventEmitter.Trigger<ulong>(Sys_Society.EEvents.OnReadRoleChat, roleInfo.roleID);
                    };
                    roleInfoItem.Update(roleInfo);
                    roleInfoItemGo.transform.SetParent(layout.contactsContentRoot.transform, false);
                    if (!contacts_RoleInfoItems.ContainsKey(Sys_Society.Instance.BLACK))
                    {
                        contacts_RoleInfoItems[Sys_Society.Instance.BLACK] = new Dictionary<ulong, UI_Society_Layout.Contacts_RoleInfoItem>();
                    }
                    contacts_RoleInfoItems[Sys_Society.Instance.BLACK].Add(roleInfo.roleID, roleInfoItem);
                }
            }

            if (Sys_Society.Instance.socialFriendGroupsInfo.friendGroupInfosDic.Count < Constants.CUSTOMFRIENDGROUPCOUNTMAX)
            {
                GameObject createGroupGo = GameObject.Instantiate(layout.contacts_createGroupPrefab);
                createGroupGo.SetActive(true);
                UI_Society_Layout.Contacts_CreateGroup createGroup = new UI_Society_Layout.Contacts_CreateGroup(createGroupGo);
                createGroupGo.transform.SetParent(layout.contactsContentRoot.transform, false);
            }
        }

        void UpdateContactsFriendInfo()
        {
            if (Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(contactsCurrentSelectRoleID))
            {
                layout.contactsFriendTopRoot.SetActive(true);
                layout.contactsNotFriendTopRoot.SetActive(false);
                layout.contactsBlackTopRoot.SetActive(false);

                CSVFriendIntimacy.Data cSVFriendIntimacyData = CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[contactsCurrentSelectRoleID].friendValue);
                if (cSVFriendIntimacyData != null)
                {
                    TextHelper.SetText(layout.contactsFriendLevelText, cSVFriendIntimacyData.IntimacyLvlLan);
                }
            }
            else
            {
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(recentCurrentSelectRoleID))
                {
                    layout.contactsFriendTopRoot.SetActive(false);
                    layout.contactsNotFriendTopRoot.SetActive(false);
                    layout.contactsBlackTopRoot.SetActive(true);
                }
                else
                {
                    layout.contactsFriendTopRoot.SetActive(false);
                    layout.contactsNotFriendTopRoot.SetActive(true);
                    layout.contactsBlackTopRoot.SetActive(false);
                }
            }

            UpdateContactsGetGiftRootVisable();
        }

        void UpdateContactsGetGiftRootVisable()
        {
            if (Sys_Society.Instance.socialRolesInfo.rolesDic[contactsCurrentSelectRoleID].hasGift)
            {
                layout.contactsGetGiftRoot.SetActive(true);
            }
            else
            {
                layout.contactsGetGiftRoot.SetActive(false);
            }
        }

        void OnCreateCellContactsCell(InfinityGridCell cell)
        {
            UI_Society_Layout.ChatItem chatItem = new UI_Society_Layout.ChatItem();
            chatItem.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(chatItem);
        }

        void OnCellChangeContactsCell(InfinityGridCell cell, int index)
        {
            if (mChatDatasContacts != null)
            {
                UI_Society_Layout.ChatItem chatItem = cell.mUserData as UI_Society_Layout.ChatItem;
                chatItem.SetData(mChatDatasContacts[index]);
            }
        }

        void UpdateContactsChatList(ulong roleId)
        {
            layout.contactsChatContentRoot.Clear();

            if (Sys_Society.Instance.roleChatDataInfo.roleChatDatas.ContainsKey(roleId))
            {
                mChatDatasContacts = Sys_Society.Instance.roleChatDataInfo.roleChatDatas[roleId].chatsInfo;
            }
            else
            {
                mChatDatasContacts = null;
            }

            if (mChatDatasContacts != null)
            {
                for (int index = 0, len = mChatDatasContacts.Count; index < len; index++)
                {
                    layout.contactsChatContentRoot.Add(CalculateSizeRoleChat(mChatDatasContacts[index]));
                }
                layout.contactsChatContentRoot.Update();
                layout.contactsChatContentRoot.ScrollView.verticalNormalizedPosition = 0;
            }
        }

        void ContactsProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<string>(Sys_Society.EEvents.OnCreateOrSettingFriendGroupSuccess, OnCreateOrSettingFriendGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<string>(Sys_Society.EEvents.OnDelFriendGroupSuccess, OnDelFriendGroupSuccess, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong>(Sys_Society.EEvents.OnClearSingleChatLog, OnClearSingleChatLog, toRegister);
        }

        void OnCreateOrSettingFriendGroupSuccess(string name)
        {
            friendGroupSelectFlags[name] = false;
            if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void OnDelFriendGroupSuccess(string name)
        {
            if (friendGroupSelectFlags.ContainsKey(name))
            {
                friendGroupSelectFlags.Remove(name);
            }
            if (layout.contactsRoot.activeSelf)
            {
                UpdateContactsList();
            }
        }

        void ResetFriendGroupSelectFlags()
        {
            friendGroupSelectFlags.Clear();
            friendGroupSelectFlags.Add(Sys_Society.Instance.MYFRIEND, false);
            friendGroupSelectFlags.Add(Sys_Society.Instance.TEAMPLAYER, false);
            friendGroupSelectFlags.Add(Sys_Society.Instance.BLACK, false);

            foreach (Sys_Society.FriendGroupInfo friendGroupInfo in Sys_Society.Instance.socialFriendGroupsInfo.friendGroupInfosDic.Values)
            {
                friendGroupSelectFlags.Add(friendGroupInfo.name, false);
            }
        }

        public void OnClickContactsDeleteButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13031);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ClearSingleChatLog(contactsCurrentSelectRoleID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickContactsNoFriendDeleteButton()
        {
            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13031);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ClearSingleChatLog(contactsCurrentSelectRoleID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickContactsSendGiftButton()
        {
            UIManager.OpenUI(EUIID.UI_SendGift, false, new List<ulong>() { 0, contactsCurrentSelectRoleID, 0 });
        }

        public void OnClickContactsGetGiftButton()
        {
            if (contactsCurrentSelectRoleID != 0)
            {
                Sys_Society.Instance.GetGift(contactsCurrentSelectRoleID);
            }
        }

        public void OnClickContactsFriendTipButton()
        {
            layout.viewTipsRoot.SetActive(true);
        }
    }
}