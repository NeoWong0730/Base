using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Table;
using System;

namespace Logic
{
    public partial class UI_Society_Layout
    {
        #region Item

        public abstract class Rencent_RoleInfoItemBase
        {
            protected GameObject root;

            protected Image roleIcon;
            protected Image roleIconFrame;
            protected Text roleName;
            public Toggle toggle;

            public Action<ulong> chooseRecentRoleInfoItem;

            public Rencent_RoleInfoItemBase(GameObject gameObject)
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

        public class Recent_SystemInfoItem : Rencent_RoleInfoItemBase
        {
            GameObject redPoint;

            public Recent_SystemInfoItem(GameObject gameObject) : base(gameObject)
            {
                redPoint = root.FindChildByName("UI_RedTips_Small");
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
            }

            public void Update()
            {
                redPoint?.SetActive(!Sys_Society.Instance.IsRoleChatAllRead(Sys_Society.socialSystemID));
            }

            protected override void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    chooseRecentRoleInfoItem?.Invoke(Sys_Society.socialSystemID);
                }
            }

            void OnGetChat(object[] objs)
            {
                if ((ulong)objs[0] == Sys_Society.socialSystemID)
                {
                    redPoint?.SetActive(true);
                }
            }

            void OnReadRoleChat(object[] objs)
            {
                if ((ulong)objs[0] == Sys_Society.socialSystemID)
                {
                    redPoint?.SetActive(false);
                }
            }

            void OnGetGift(object[] objs)
            {
                if ((ulong)objs[0] == Sys_Society.socialSystemID)
                {
                    redPoint?.SetActive(true);
                }
            }

            void OnReadGift(object[] objs)
            {
                if ((ulong)objs[0] == Sys_Society.socialSystemID)
                {
                    redPoint?.SetActive(false);
                }
            }
        }

        public class Recent_MOLIInfeoItem : Rencent_RoleInfoItemBase
        {
            public Recent_MOLIInfeoItem(GameObject gameObject) : base(gameObject)
            {

            }

            public override void Dispose()
            {
            }

            protected override void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    chooseRecentRoleInfoItem?.Invoke(Sys_Society.socialMOLIID);
                }
            }
        }

        public class Recent_RoleInfoItem : Rencent_RoleInfoItemBase
        {
            Text roleLv;
            Text roleFamily;
            GameObject friendRoot;
            Text roleFriendValue;
            Button button;
            GameObject redPoint;
            Image rootImage;

            Sys_Society.RoleInfo roleInfo;

            public Recent_RoleInfoItem(GameObject gameObject) : base(gameObject)
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
            }

            public void Update(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);              
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));     //HZCTODO语言表
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
                        redPoint.SetActive(false);
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
                    chooseRecentRoleInfoItem?.Invoke(roleInfo.roleID);
                }
            }

            void OnClickButton()
            {
                List<Sys_Role_Info.InfoItem> infoItems = new List<Sys_Role_Info.InfoItem>();

                Sys_Role_Info.InfoItem infoItemSendMessage = new Sys_Role_Info.InfoItem();
                infoItemSendMessage.mName = LanguageHelper.GetTextContent(2002111);     //HZCTODO语言表
                infoItemSendMessage.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                {
                    toggle.isOn = true;
                    toggle.onValueChanged.Invoke(true);
                };
                infoItems.Add(infoItemSendMessage);

                if (!Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.roleID))
                {
                    Sys_Role_Info.InfoItem infoItemAddFriend = new Sys_Role_Info.InfoItem();
                    infoItemAddFriend.mName = LanguageHelper.GetTextContent(2029472);     //HZCTODO语言表
                    infoItemAddFriend.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqAddFriend(roleInfo.roleID);
                    };
                    infoItems.Add(infoItemAddFriend);
                }
                else
                {
                    Sys_Role_Info.InfoItem infoItemDelFriend = new Sys_Role_Info.InfoItem();
                    infoItemDelFriend.mName = LanguageHelper.GetTextContent(2029473);     //HZCTODO语言表
                    infoItemDelFriend.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                    {
                        Sys_Society.Instance.ReqDelFriend(roleInfo.roleID);
                    };
                    infoItems.Add(infoItemDelFriend);
                }

                Sys_Role_Info.InfoItem infoItemInvite = new Sys_Role_Info.InfoItem();
                infoItemInvite.mName = LanguageHelper.GetTextContent(11791);     //HZCTODO语言表
                infoItemInvite.mClickAc = (Sys_Role_Info.DoRoleInfo info) =>
                {
                    Sys_Team.Instance.InvitedOther(info.getRoleID());
                };
                infoItems.Add(infoItemInvite);

                //Sys_Role_Info.InfoItem infoItemBattle = new Sys_Role_Info.InfoItem();
                //infoItemBattle.mName = "申请切磋";     //HZCTODO语言表
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

                Sys_Role_Info.Instance.OpenRoleInfo(roleInfo.roleID,Sys_Role_Info.EType.None, infoItems);
            }
        }

        #endregion

        public GameObject recentRoot;
        public GameObject recentContentRoot;

        public GameObject recent_roleInfoPrefab;
        public GameObject recent_systemInfoPrefab;
        public GameObject recent_moliInfoPrefab;

        public GameObject recentRightRoot;
        public InfinityIrregularGrid recentChatContentRoot;

        public GameObject recentFriendTopRoot;
        public Button recentFriendDeleteButton;
        public GameObject recentNotFriendTopRoot;
        public GameObject recentBlackTopRoot;
        public Button recentNoFriendDeleteButton;

        public GameObject recentFriendInfoRoot;
        public Button recentFriendTipButton;
        public Text recentFriendLevelText;
        public Button recentSendGiftButton;

        public GameObject recentGetGiftRoot;
        public Button recentGetGiftButton;

        public void RecentInit()
        {
            #region Left

            recentRoot = leftRoot.FindChildByName("RecentRoot");
            recentContentRoot = recentRoot.FindChildByName("Content");

            recent_roleInfoPrefab = recentRoot.FindChildByName("Recent_RoleInfoPrefab");
            recent_systemInfoPrefab = recentRoot.FindChildByName("Recent_SystemInfoPrefab");
            recent_moliInfoPrefab = recentRoot.FindChildByName("Recent_MOLIInfoPrefab");

            #endregion

            #region Right

            recentRightRoot = rightRoot.FindChildByName("RecentRoot");

            recentFriendTopRoot = recentRightRoot.FindChildByName("View_Friend");
            recentFriendDeleteButton = recentFriendTopRoot.FindChildByName("Btn_Delete").GetComponent<Button>();
            recentNotFriendTopRoot = recentRightRoot.FindChildByName("View_NoFriend");
            recentBlackTopRoot = recentRightRoot.FindChildByName("View_Blacklist");
            recentNoFriendDeleteButton = recentNotFriendTopRoot.FindChildByName("Btn_Delete").GetComponent<Button>();

            recentChatContentRoot = recentRightRoot.FindChildByName("Scroll View").GetComponent<InfinityIrregularGrid>();

            recentFriendInfoRoot = recentRightRoot.FindChildByName("Title_Tips05");
            recentFriendLevelText = recentFriendInfoRoot.FindChildByName("Text_Title").GetComponent<Text>();
            recentFriendTipButton = recentFriendInfoRoot.FindChildByName("Button_Detail").GetComponent<Button>();
            recentSendGiftButton = recentFriendInfoRoot.FindChildByName("Button_Gift").GetComponent<Button>();

            recentGetGiftRoot = recentRightRoot.FindChildByName("View_Getgift");
            recentGetGiftButton = recentGetGiftRoot.FindChildByName("Button").GetComponent<Button>();

            #endregion           
        }

        void RecentRegisterEvents(IListener listener)
        {
            recentFriendDeleteButton.onClick.AddListener(listener.OnClickRecentFriendDeleteButton);
            recentNoFriendDeleteButton.onClick.AddListener(listener.OnClickRecentNoFriendDeleteButton);
            recentSendGiftButton.onClick.AddListener(listener.OnClickRecentSendGiftButton);
            recentFriendTipButton.onClick.AddListener(listener.OnClickRecentFriendTipButton);
            recentGetGiftButton.onClick.AddListener(listener.OnClickRecentGetGiftButton);
        }
    }
}
