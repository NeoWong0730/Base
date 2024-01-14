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
        public class Group_CreateGroupItem
        {
            GameObject root;

            Button button;

            public Group_CreateGroupItem(GameObject gameObject)
            {
                root = gameObject;

                button = root.GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);
            }

            void OnClickButton()
            {
                UIManager.OpenUI(EUIID.UI_Society_GroupCreate, false, Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos());
            }
        }

        public class Group_GroupInfoItem
        {
            GameObject root;

            public GameObject selectImage;

            public GameObject icon1Root;
            public GameObject icon2Root;
            public GameObject icon3Root;
            public GameObject icon4Root;

            public Image icon1_1;
            public Image icon2_1;
            public Image icon2_2;
            public Image icon3_1;
            public Image icon3_2;
            public Image icon3_3;
            public Image icon4_1;
            public Image icon4_2;
            public Image icon4_3;
            public Image icon4_4;

            public Text groupName;
            public Text groupCount;
            public Toggle toggle;
            public Button button;

            GameObject redPoint;
            Image rootImage;

            Sys_Society.GroupInfo groupInfo;

            public Action<uint> chooseGroupInfoItem;

            public Group_GroupInfoItem(GameObject gameObject)
            {
                root = gameObject;

                selectImage = root.FindChildByName("Image_Select");

                icon1Root = root.FindChildByName("View_Head01");
                icon1_1 = icon1Root.FindChildByName("Image_Icon1").GetComponent<Image>();

                icon2Root = root.FindChildByName("View_Head02");
                icon2_1 = icon2Root.FindChildByName("Image_Icon1").GetComponent<Image>();
                icon2_2 = icon2Root.FindChildByName("Image_Icon2").GetComponent<Image>();

                icon3Root = root.FindChildByName("View_Head03");
                icon3_1 = icon3Root.FindChildByName("Image_Icon1").GetComponent<Image>();
                icon3_2 = icon3Root.FindChildByName("Image_Icon2").GetComponent<Image>();
                icon3_3 = icon3Root.FindChildByName("Image_Icon3").GetComponent<Image>();

                icon4Root = root.FindChildByName("View_Head04");
                icon4_1 = icon4Root.FindChildByName("Image_Icon1").GetComponent<Image>();
                icon4_2 = icon4Root.FindChildByName("Image_Icon2").GetComponent<Image>();
                icon4_3 = icon4Root.FindChildByName("Image_Icon3").GetComponent<Image>();
                icon4_4 = icon4Root.FindChildByName("Image_Icon4").GetComponent<Image>();

                groupName = root.FindChildByName("Text_Name").GetComponent<Text>();
                groupCount = root.FindChildByName("Text_Num").GetComponent<Text>();
                toggle = root.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                button = root.FindChildByName("Btn_Arrow").GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);

                redPoint = root.FindChildByName("RedPointImage");
                rootImage = root.FindChildByName("Root").GetComponent<Image>();

                RedPointElement.eventEmitter.Handle<object[]>(RedPointElement.EEvents.OnReadGroupChat, OnReadGroupChat, true);
                Sys_Society.Instance.eventEmitter.Handle<uint, Sys_Society.ChatData>(Sys_Society.EEvents.OnGetGroupChat, OnGetGroupChat, true);
            }

            public void Update(Sys_Society.GroupInfo _groupInfo)
            {
                groupInfo = _groupInfo;

                if (_groupInfo.heroIDs.Count == 1)
                {
                    icon1Root.SetActive(true);
                    icon2Root.SetActive(false);
                    icon3Root.SetActive(false);
                    icon4Root.SetActive(false);

                    ImageHelper.SetIcon(icon1_1, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[0].infoID).headid);
                }
                else if (_groupInfo.heroIDs.Count == 2)
                {
                    icon1Root.SetActive(false);
                    icon2Root.SetActive(true);
                    icon3Root.SetActive(false);
                    icon4Root.SetActive(false);

                    ImageHelper.SetIcon(icon2_1, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[0].infoID).headid);
                    ImageHelper.SetIcon(icon2_2, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[1].infoID).headid);
                }
                else if (_groupInfo.heroIDs.Count == 3)
                {
                    icon1Root.SetActive(false);
                    icon2Root.SetActive(false);
                    icon3Root.SetActive(true);
                    icon4Root.SetActive(false);

                    ImageHelper.SetIcon(icon3_1, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[0].infoID).headid);
                    ImageHelper.SetIcon(icon3_2, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[1].infoID).headid);
                    ImageHelper.SetIcon(icon3_3, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[2].infoID).headid);
                }
                else if (_groupInfo.heroIDs.Count >= 4)
                {
                    icon1Root.SetActive(false);
                    icon2Root.SetActive(false);
                    icon3Root.SetActive(false);
                    icon4Root.SetActive(true);

                    ImageHelper.SetIcon(icon4_1, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[0].infoID).headid);
                    ImageHelper.SetIcon(icon4_2, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[1].infoID).headid);
                    ImageHelper.SetIcon(icon4_3, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[2].infoID).headid);
                    ImageHelper.SetIcon(icon4_4, CSVCharacter.Instance.GetConfData((uint)_groupInfo.heroIDs[3].infoID).headid);
                }

                TextHelper.SetText(groupName, groupInfo.name);
                TextHelper.SetText(groupCount, string.Format("{0}/{1}", groupInfo.count, Sys_Society.Instance.GroupMaxCount));

                redPoint.SetActive(!Sys_Society.Instance.IsGroupChatAllRead(groupInfo.groupID));
            }

            void OnReadGroupChat(object[] objs)
            {
                if (redPoint != null)
                {
                    redPoint.SetActive(!Sys_Society.Instance.IsGroupChatAllRead(groupInfo.groupID));
                }
            }
            
            void OnGetGroupChat(uint groupID, Sys_Society.ChatData chatData)
            {
                if (groupInfo.groupID == groupID)
                {
                    if (redPoint != null)
                    {
                        redPoint.SetActive(!Sys_Society.Instance.IsGroupChatAllRead(groupInfo.groupID));
                    }
                }
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    chooseGroupInfoItem?.Invoke(groupInfo.groupID);
                }
                selectImage.SetActive(isOn);
            }

            void OnClickButton()
            {
                if (groupInfo.leader == Sys_Role.Instance.RoleId)
                {
                    UIManager.OpenUI(EUIID.UI_Society_GroupMasterOperation, false, groupInfo);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Society_GroupMemberOperation, false, groupInfo);
                }
            }
        }

        public GameObject groupRoot;
        public GameObject groupContentRoot;

        public GameObject group_groupInfoPrefab;
        public GameObject group_createGroupPrefab;

        public GameObject groupRightRoot;
        public GameObject groupRightContentRoot;
        public Scrollbar groupRightContentScrollbar;

        public GameObject groupUnReadRoot;
        public Button groupJumpUnReadButton;
        public Text groupJumpUnReadText;

        public Button groupSettingButton;
        public Text notice;

        public void GroupInit()
        {
            #region Left

            groupRoot = leftRoot.FindChildByName("GroupRoot");
            groupContentRoot = groupRoot.FindChildByName("Content");

            group_groupInfoPrefab = groupRoot.FindChildByName("Group_GroupInfoPrefab");
            group_createGroupPrefab = groupRoot.FindChildByName("Group_CreateGroupPrefab");

            #endregion

            #region Right

            groupRightRoot = rightRoot.FindChildByName("GroupRoot");

            groupRightContentRoot = groupRightRoot.FindChildByName("Content");
            groupRightContentScrollbar = groupRightRoot.FindChildByName("Scrollbar Vertical").GetComponent<Scrollbar>();

            groupUnReadRoot = groupRightRoot.FindChildByName("UnReadRoot");
            groupJumpUnReadButton = groupUnReadRoot.FindChildByName("UnReadJumpBtn").GetComponent<Button>();
            groupJumpUnReadText = groupJumpUnReadButton.gameObject.FindChildByName("Text").GetComponent<Text>();

            groupSettingButton = groupRightRoot.FindChildByName("Btn_Setting").GetComponent<Button>();
            notice = groupRightRoot.FindChildByName("Text_Annonce").GetComponent<Text>();

            #endregion
        }

        void GroupRegisterEvents(IListener listener)
        {
            groupSettingButton.onClick.AddListener(listener.OnClickGroupSettingButton);
        }
    }
}
