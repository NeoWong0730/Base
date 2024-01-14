using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Table;

namespace Logic
{
    public partial class UI_Society_Layout
    {
        public abstract class Contacts_RoleInfoItemBase
        {
            protected GameObject root;

            protected Image roleIcon;
            protected Image roleIconFrame;
            protected Text roleName;
            protected Toggle toggle;

            public Action<ulong> chooseContactsRoleInfoItem;

            public Contacts_RoleInfoItemBase(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                roleIconFrame = root.FindChildByName("Image_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                toggle = root.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
            }

            protected abstract void OnClickToggle(bool isOn);

            public abstract void Dispose();
        }

        public class Contacts_MOLIInfoItem : Contacts_RoleInfoItemBase
        {
            public Contacts_MOLIInfoItem(GameObject gameObject) : base(gameObject)
            {

            }

            protected override void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    chooseContactsRoleInfoItem?.Invoke(Sys_Society.socialMOLIID);
                }
            }

            public override void Dispose()
            {
                
            }
        }

        public class Contacts_RoleInfoItem : Contacts_RoleInfoItemBase
        {
            Text roleLv;
            Text roleFamily;
            GameObject friendRoot;
            Text roleFriendValue;
            Button button;
            GameObject redPoint;
            Image rootImage;

            Sys_Society.RoleInfo roleInfo;

            public Contacts_RoleInfoItem(GameObject gameObject) : base(gameObject)
            {
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                roleFamily = root.FindChildByName("Text_Family").GetComponent<Text>();
                friendRoot = root.FindChildByName("Image_Friend");
                roleFriendValue = friendRoot.FindChildByName("Text").GetComponent<Text>();
                button = root.FindChildByName("Btn_Arrow").GetComponentInChildren<Button>();
                button.onClick.AddListener(OnClickButton);
                redPoint = root.FindChildByName("RedPointImage");
                rootImage = root.FindChildByName("Root").GetComponent<Image>();
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnGetChat, OnGetChat, true);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnReadRoleChat, OnReadRoleChat, true);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnGetGift, OnGetGift, true);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnReadGift, OnReadGift, true);
            }

            public override void Dispose()
            {
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnGetChat, OnGetChat, false);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnReadRoleChat, OnReadRoleChat, false);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnGetGift, OnGetGift, false);
                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnReadGift, OnReadGift, false);

                redPoint = null;
            }

            public void Update(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                if (!string.IsNullOrEmpty(roleInfo.guildName))
                {
                    TextHelper.SetText(roleFamily, roleInfo.guildName);
                }
                else
                {
                    TextHelper.SetText(roleFamily, 11976);
                }
                TextHelper.SetText(roleFriendValue, roleInfo.friendValue.ToString());
                ImageHelper.SetImageGray(rootImage, !roleInfo.isOnLine, true);
               
                if (redPoint != null)
                    redPoint.SetActive(Sys_Society.Instance.IsRoleRedPointShow(roleInfo.roleID));
            }

            void OnGetChat(object[] objs)
            {
                if ((ulong)objs[0] == roleInfo.roleID)
                {
                    if (redPoint != null)
                        redPoint.SetActive(true);
                }
            }

            void OnReadRoleChat(object[] objs)
            {
                if ((ulong)objs[0] == roleInfo.roleID)
                {
                    if (redPoint != null)
                        redPoint.SetActive(false);
                }
            }

            void OnGetGift(object[] objs)
            {
                if ((ulong)objs[0] == roleInfo.roleID)
                {
                    if (redPoint != null)
                        redPoint.SetActive(true);
                }
            }   

            void OnReadGift(object[] objs)
            {
                if ((ulong)objs[0] == roleInfo.roleID)
                {
                    if (redPoint != null)
                        redPoint.SetActive(false);
                } 
            }

            protected override void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    chooseContactsRoleInfoItem?.Invoke(roleInfo.roleID);
                }
            }

            void OnClickButton()
            {
                List<Sys_Role_Info.InfoItem> infoItems = new List<Sys_Role_Info.InfoItem>();

                Sys_Role_Info.InfoItem infoItemSendMessage = new Sys_Role_Info.InfoItem();
                infoItemSendMessage.mName = LanguageHelper.GetTextContent(2002111);
                infoItemSendMessage.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                {
                    toggle.isOn = true;
                    toggle.onValueChanged.Invoke(true);
                };
                infoItems.Add(infoItemSendMessage);

                if (!Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID))
                {
                    Sys_Role_Info.InfoItem infoItemAddFriend = new Sys_Role_Info.InfoItem();
                    infoItemAddFriend.mName = LanguageHelper.GetTextContent(2029472);
                    infoItemAddFriend.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqAddFriend(roleInfo.roleID);
                    };
                    infoItems.Add(infoItemAddFriend);
                }
                else
                {
                    Sys_Role_Info.InfoItem infoItemDelFriend = new Sys_Role_Info.InfoItem();
                    infoItemDelFriend.mName = LanguageHelper.GetTextContent(2029473);
                    infoItemDelFriend.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqDelFriend(roleInfo.roleID);
                    };
                    infoItems.Add(infoItemDelFriend);
                }

                Sys_Role_Info.InfoItem infoItemInvite = new Sys_Role_Info.InfoItem();
                infoItemInvite.mName = LanguageHelper.GetTextContent(11791);
                infoItemInvite.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                {
                    Sys_Team.Instance.InvitedOther(info.getRoleID());
                };
                infoItems.Add(infoItemInvite);

                //Sys_Role_Info.InfoItem infoItemBattle = new Sys_Role_Info.InfoItem();
                //infoItemBattle.mName = "申请切磋";
                //infoItemBattle.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                //{
                //    Sys_Compete.Instance.OnInviteReq(info.getRoleID());
                //};
                //infoItems.Add(infoItemBattle);
              
               Sys_Role_Info.InfoItem infoItemBlack = new Sys_Role_Info.InfoItem();
                if (Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.ContainsKey(roleInfo.roleID))
                {
                    infoItemBlack.mName = LanguageHelper.GetTextContent(11662);
                    infoItemBlack.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqRemoveBlackList(info.getRoleID());
                    };
                }
                else
                {
                    infoItemBlack.mName = LanguageHelper.GetTextContent(11661);
                    infoItemBlack.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqAddBlackList(info.getRoleID());
                    };
                }
                infoItems.Add(infoItemBlack);

                Sys_Role_Info.InfoItem infoItemMoreInfo = new Sys_Role_Info.InfoItem();
                infoItemMoreInfo.mName = LanguageHelper.GetTextContent(2029474);    //HZCTODO语言表
                infoItemMoreInfo.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                {
                    ulong roleID = info.getRoleID();
                    Sys_Society.RoleInfo roleInfo;
                    if (Sys_Society.Instance.socialRolesInfo.rolesDic.TryGetValue(roleID, out roleInfo))
                    {
                        Sys_Attr.Instance.GetMoreInfo(roleID, CSVFriendIntimacy.Instance.GetDataByIntimacyValue(roleInfo.friendValue).id, roleInfo.isOnLine);
                    }
                };
                infoItems.Add(infoItemMoreInfo);

                Sys_Role_Info.Instance.OpenRoleInfo(roleInfo.roleID, Sys_Role_Info.EType.None, infoItems);
            }
        }

        public abstract class Contacts_FriendGroupBase
        {
            protected GameObject root;

            public GameObject arrow;
            public GameObject arrowChoose;
            protected Text name;
            protected Text num;
            protected Button detailButton;
            protected Button button;

            public Action<string> OnClickFriendGroupItem;

            public Contacts_FriendGroupBase(GameObject gameObject)
            {
                root = gameObject;

                arrow = root.FindChildByName("Image_Arrow");
                arrowChoose = root.FindChildByName("Image_Arrow_Choose");
                name = root.FindChildByName("Text_Name").GetComponent<Text>();
                num = root.FindChildByName("Text_Num").GetComponent<Text>();
                detailButton = root.FindChildByName("Btn_Details").GetComponent<Button>();
                detailButton.onClick.AddListener(OnClickDetailButton);
                button = root.GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);                
            }

            protected virtual void OnClickDetailButton()
            {

            }

            protected virtual void OnClickButton()
            {
                OnClickFriendGroupItem?.Invoke(name.text);
            }
        }

        public class Contacts_MyFriendGroup : Contacts_FriendGroupBase
        {
            public Contacts_MyFriendGroup(GameObject gameObject) : base(gameObject)
            {
                detailButton.gameObject.SetActive(false);
                TextHelper.SetText(name, LanguageHelper.GetTextContent(13028));
                TextHelper.SetText(num, $"({Sys_Society.Instance.GetOnLineFriendsCount()}/{Sys_Society.Instance.GetFriendsCount()})");

                arrow.SetActive(!UI_Society.friendGroupSelectFlags[Sys_Society.Instance.MYFRIEND]);
                arrowChoose.SetActive(UI_Society.friendGroupSelectFlags[Sys_Society.Instance.MYFRIEND]);
            }

            protected override void OnClickButton()
            {
                OnClickFriendGroupItem?.Invoke(Sys_Society.Instance.MYFRIEND);
            }
        }

        public class Contacts_RecentTeamMemberGroup : Contacts_FriendGroupBase
        {
            public Contacts_RecentTeamMemberGroup(GameObject gameObject) : base(gameObject)
            {
                detailButton.gameObject.SetActive(false);
                TextHelper.SetText(name, LanguageHelper.GetTextContent(13029));
                TextHelper.SetText(num, $"({Sys_Society.Instance.GetOnLineTeamMembersCount()}/{Sys_Society.Instance.GetTeamMembersCount()})");

                arrow.SetActive(!UI_Society.friendGroupSelectFlags[Sys_Society.Instance.TEAMPLAYER]);
                arrowChoose.SetActive(UI_Society.friendGroupSelectFlags[Sys_Society.Instance.TEAMPLAYER]);
            }

            protected override void OnClickButton()
            {
                OnClickFriendGroupItem?.Invoke(Sys_Society.Instance.TEAMPLAYER);
            }
        }

        public class Contacts_BlackListGroup : Contacts_FriendGroupBase
        {
            public Contacts_BlackListGroup(GameObject gameObject) : base(gameObject)
            {
                detailButton.gameObject.SetActive(false);
                TextHelper.SetText(name, LanguageHelper.GetTextContent(13030));
                TextHelper.SetText(num, $"({Sys_Society.Instance.socialBlacksInfo.blacksIdsDic.Count}/{Sys_Society.Instance.blacksMaxCount})");

                arrow.SetActive(!UI_Society.friendGroupSelectFlags[Sys_Society.Instance.BLACK]);
                arrowChoose.SetActive(UI_Society.friendGroupSelectFlags[Sys_Society.Instance.BLACK]);
            }

            protected override void OnClickButton()
            {
                OnClickFriendGroupItem?.Invoke(Sys_Society.Instance.BLACK);
            }
        }

        public class Contacts_CustomFriendGroup : Contacts_FriendGroupBase
        {
            Sys_Society.FriendGroupInfo friendGroupInfo;

            public Contacts_CustomFriendGroup(GameObject gameObject) : base(gameObject)
            {
                detailButton.gameObject.SetActive(true);
            }

            public void Update(Sys_Society.FriendGroupInfo _friendGroupInfo)
            {
                friendGroupInfo = _friendGroupInfo;

                TextHelper.SetText(name, friendGroupInfo.name);
                TextHelper.SetText(num, $"({friendGroupInfo.GetOnLineCount()}/{friendGroupInfo.roleIdsDic.Count})");

                arrow.SetActive(!UI_Society.friendGroupSelectFlags[name.text]);
                arrowChoose.SetActive(UI_Society.friendGroupSelectFlags[name.text]);
            }

            protected override void OnClickDetailButton()
            {
                UIManager.OpenUI(EUIID.UI_Society_FriendGroupOperation, false, friendGroupInfo);
            }
        }

        public class Contacts_CreateGroup
        {
            GameObject root;

            Button button;

            public Contacts_CreateGroup(GameObject gameObject)
            {
                root = gameObject;

                button = root.GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);
            }

            void OnClickButton()
            {
                UIManager.OpenUI(EUIID.UI_Society_FriendGroupCreate, false, Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos());
            }
        }

        public GameObject contactsRoot;
        public GameObject contactsContentRoot;

        public GameObject contacts_moliInfoPrefab;
        public GameObject contacts_roleInfoPrefab;
        public GameObject contacts_groupPrefab;
        public GameObject contacts_createGroupPrefab;

        public GameObject contactsRightRoot;
        public InfinityIrregularGrid contactsChatContentRoot;

        public GameObject contactsFriendTopRoot;
        public Button contactsDeleteButton;
        public GameObject contactsNotFriendTopRoot;
        public GameObject contactsBlackTopRoot;
        public Button contactsNoFriendDeleteButton;

        public GameObject contactsFriendInfoRoot;
        public Button contactsFriendTipButton;
        public Text contactsFriendLevelText;
        public Button contactsSendGiftButton;

        public GameObject contactsGetGiftRoot;
        public Button contactsGetGiftButton;

        public void ContactsInit()
        {
            #region Left

            contactsRoot = leftRoot.FindChildByName("ContactsRoot");
            contactsContentRoot = contactsRoot.FindChildByName("Content");

            contacts_moliInfoPrefab = contactsRoot.FindChildByName("Contacts_MOLIInfoPrefab");
            contacts_roleInfoPrefab = contactsRoot.FindChildByName("Contacts_RoleInfoPrefab");
            contacts_groupPrefab = contactsRoot.FindChildByName("Contacts_GroupPrefab");
            contacts_createGroupPrefab = contactsRoot.FindChildByName("Contacts_CreateGroupPrefab");

            #endregion

            #region Right

            contactsRightRoot = rightRoot.FindChildByName("ContactsRoot");

            contactsFriendTopRoot = contactsRightRoot.FindChildByName("View_Friend");
            contactsDeleteButton = contactsFriendTopRoot.FindChildByName("Btn_Delete").GetComponent<Button>();
            contactsFriendInfoRoot = contactsFriendTopRoot.FindChildByName("Title_Tips05");
            contactsFriendTipButton = contactsFriendInfoRoot.FindChildByName("Button_Detail").GetComponent<Button>();
            contactsFriendLevelText = contactsFriendInfoRoot.FindChildByName("Text_Title").GetComponent<Text>();
            contactsChatContentRoot = contactsRightRoot.FindChildByName("Scroll View").GetComponent<InfinityIrregularGrid>();

            contactsSendGiftButton = contactsRightRoot.FindChildByName("Button_Gift").GetComponent<Button>();

            contactsNotFriendTopRoot = contactsRightRoot.FindChildByName("View_NoFriend");
            contactsNoFriendDeleteButton = contactsNotFriendTopRoot.FindChildByName("Btn_Delete").GetComponent<Button>();

            contactsBlackTopRoot = contactsRightRoot.FindChildByName("View_Blacklist");

            contactsGetGiftRoot = contactsRightRoot.FindChildByName("View_Getgift");
            contactsGetGiftButton = contactsGetGiftRoot.FindChildByName("Button").GetComponent<Button>();

            #endregion
        }

        void ContactsRegisterEvents(IListener listener)
        {
            contactsDeleteButton.onClick.AddListener(listener.OnClickContactsDeleteButton);
            contactsNoFriendDeleteButton.onClick.AddListener(listener.OnClickContactsNoFriendDeleteButton);
            contactsSendGiftButton.onClick.AddListener(listener.OnClickContactsSendGiftButton);
            contactsGetGiftButton.onClick.AddListener(listener.OnClickContactsGetGiftButton);
            contactsFriendTipButton.onClick.AddListener(listener.OnClickContactsFriendTipButton);
        }
    }
}
