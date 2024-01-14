using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class UI_WarriorGroup_CreateMeeting_Layout
    {
        public class RecruitRoleItem
        {
            Sys_Society.RoleInfo roleInfo;
            Action<ulong> onSelect;

            GameObject root;

            public Image roleIcon;
            public Image roleFrame;
            public Text roleName;
            public Text roleLv;
            public Image occIcon;
            public Text occText;
            public Text value;
            public CP_Toggle toggle;
            
            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Head").GetComponent<Image>();
                roleFrame = root.FindChildByName("Image_Before_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                occText = root.FindChildByName("Text_Profession").GetComponent<Text>();
                value = root.FindChildByName("FriendValue").GetComponent<Text>();
                toggle = root.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }

            public void UpdateItem(Sys_Society.RoleInfo _roleInfo, Action<ulong> _onSelect)
            {
                roleInfo = _roleInfo;
                onSelect = _onSelect;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);
                //HZCTODO
                TextHelper.SetText(roleLv, $"等级{roleInfo.level}级");
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.occ).icon);
                TextHelper.SetText(occText, CSVCareer.Instance.GetConfData(roleInfo.occ).name);
                TextHelper.SetText(value, roleInfo.friendValue.ToString());
            }

            void OnToggleValueChanged(bool isOn)
            {
                if (isOn)
                {
                    onSelect(roleInfo.roleID);
                }
            }
        }

        public class FireRoleItem
        {
            Sys_WarriorGroup.WarriorInfo roleInfo;
            Action<ulong> onSelect;

            GameObject root;

            public Image roleIcon;
            public Image roleFrame;
            public Text roleName;
            public Text roleLv;
            public Image occIcon;
            public Text occText;
            public Text value;
            public CP_Toggle toggle;

            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                roleIcon = root.FindChildByName("Head").GetComponent<Image>();
                roleFrame = root.FindChildByName("Image_Before_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                occIcon = root.FindChildByName("Image_Profession").GetComponent<Image>();
                occText = root.FindChildByName("Text_Profession").GetComponent<Text>();
                value = root.FindChildByName("FriendValue").GetComponent<Text>();
                toggle = root.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnToggleValueChanged);
            }

            public void UpdateItem(Sys_WarriorGroup.WarriorInfo _roleInfo, Action<ulong> _onSelect)
            {
                roleInfo = _roleInfo;
                onSelect = _onSelect;


                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.HeroID, roleInfo.IconID));
                ImageHelper.SetIcon(roleFrame, CSVHeadframe.Instance.GetConfData(roleInfo.FrameID).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.RoleName);
                //HZCTODO
                TextHelper.SetText(roleLv, $"等级{roleInfo.Level}级");
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.Occ).icon);
                TextHelper.SetText(occText, CSVCareer.Instance.GetConfData(roleInfo.Occ).name);
                if (Sys_Society.Instance.socialFriendsInfo.friendsIdsDic.ContainsKey(roleInfo.RoleID))
                    TextHelper.SetText(value, Sys_Society.Instance.socialRolesInfo.rolesDic[roleInfo.RoleID].friendValue.ToString());
                else
                    TextHelper.SetText(value, "0");
            }

            void OnToggleValueChanged(bool isOn)
            {
                if (isOn)
                {
                    onSelect(roleInfo.RoleID);
                }
            }
        }

        public Transform transform;

        public Button closeButton;
        public Button createButton;

        public GameObject suggestSelfRoot;
        public GameObject recruitRoot;
        public GameObject fireRoot;
        public GameObject changeNameRoot;
        public GameObject changeDeclarationRoot;

        public GameObject suggestSelfEmptyRoot;
        public GameObject suggestSelfInfoRoot;
        public InputField suggestSelfInputField;
        public Text suggestSelfTextCount;

        public GameObject recruitEmptyRoot;
        public GameObject recruitInfoRoot;
        public InfinityGrid recruitRoleGrid;

        public GameObject fireEmptyRoot;
        public GameObject fireInfoRoot;
        public InfinityGrid fireRoleGrid;

        public Text changeNameOldName;
        public InputField changeNameInputField;

        public InputField changeDeclarationInputField;
        public Text changeDeclarationTextCount;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            createButton = transform.gameObject.FindChildByName("Btn_01").GetComponent<Button>();
            suggestSelfRoot = transform.gameObject.FindChildByName("Type1");
            recruitRoot = transform.gameObject.FindChildByName("Type0");
            fireRoot = transform.gameObject.FindChildByName("Type3");
            changeNameRoot = transform.gameObject.FindChildByName("Type2");
            changeDeclarationRoot = transform.gameObject.FindChildByName("Type4");

            suggestSelfEmptyRoot = suggestSelfRoot.FindChildByName("Empty");
            suggestSelfInfoRoot = suggestSelfRoot.FindChildByName("Info");
            suggestSelfInputField = suggestSelfInfoRoot.FindChildByName("InputField").GetComponent<InputField>();
            suggestSelfTextCount = suggestSelfInfoRoot.FindChildByName("Text_Tips").GetComponent<Text>();

            recruitEmptyRoot = recruitRoot.FindChildByName("Empty");
            recruitInfoRoot = recruitRoot.FindChildByName("Info");
            recruitRoleGrid = recruitInfoRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();

            fireEmptyRoot = fireRoot.FindChildByName("Empty");
            fireInfoRoot = fireRoot.FindChildByName("Info");
            fireRoleGrid = fireInfoRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();

            changeNameOldName = changeNameRoot.FindChildByName("Text_Name").GetComponent<Text>();
            changeNameInputField = changeNameRoot.FindChildByName("InputField").GetComponent<InputField>();

            changeDeclarationInputField = changeDeclarationRoot.FindChildByName("InputField").GetComponent<InputField>();
            changeDeclarationTextCount = changeDeclarationRoot.FindChildByName("Text_Tips").GetComponent<Text>();
        }

        public void Refresh(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.SuggestSelf)
            {
                suggestSelfRoot.SetActive(true);
                recruitRoot.SetActive(false);
                fireRoot.SetActive(false);
                changeNameRoot.SetActive(false);
                changeDeclarationRoot.SetActive(false);
                RefreshSuggestSelf(meetingInfo);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.Recruit)
            {
                suggestSelfRoot.SetActive(false);
                recruitRoot.SetActive(true);
                fireRoot.SetActive(false);
                changeNameRoot.SetActive(false);
                changeDeclarationRoot.SetActive(false);
                RefreshRecruit(meetingInfo);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.Fire)
            {
                suggestSelfRoot.SetActive(false);
                recruitRoot.SetActive(false);
                fireRoot.SetActive(true);
                changeNameRoot.SetActive(false);
                changeDeclarationRoot.SetActive(false);
                RefreshFire(meetingInfo);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.ChangeName)
            {
                suggestSelfRoot.SetActive(false);
                recruitRoot.SetActive(false);
                fireRoot.SetActive(false);
                changeNameRoot.SetActive(true);
                changeDeclarationRoot.SetActive(false);
                RefreshChangeName(meetingInfo);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.ChangeDeclaration)
            {
                suggestSelfRoot.SetActive(false);
                recruitRoot.SetActive(false);
                fireRoot.SetActive(false);
                changeNameRoot.SetActive(false);
                changeDeclarationRoot.SetActive(true);
                RefreshChangeDeclaration(meetingInfo);
            }
        }

        void RefreshSuggestSelf(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            Sys_WarriorGroup.Meeting_SuggestSelf meeting_SuggestSelf = meetingInfo as Sys_WarriorGroup.Meeting_SuggestSelf;
            if (meeting_SuggestSelf != null)
            {
                suggestSelfEmptyRoot.SetActive(!meeting_SuggestSelf.IsValid());
                suggestSelfInfoRoot.SetActive(meeting_SuggestSelf.IsValid());
                createButton.gameObject.SetActive(meeting_SuggestSelf.IsValid());
            }
        }

        void RefreshRecruit(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            Sys_WarriorGroup.Meeting_Recruit meeting_Recruit = meetingInfo as Sys_WarriorGroup.Meeting_Recruit;
            if (meeting_Recruit != null)
            {
                recruitEmptyRoot.SetActive(!meeting_Recruit.IsValid());
                recruitInfoRoot.SetActive(meeting_Recruit.IsValid());
                createButton.gameObject.SetActive(meeting_Recruit.IsValid());

                int count = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitlerForWarriorGroup(uint.Parse(CSVParam.Instance.GetConfData(1378).str_value), uint.Parse(CSVParam.Instance.GetConfData(1368).str_value)).Count;
                if (count > 0)
                {
                    recruitRoleGrid.CellCount = count;
                    recruitRoleGrid.ForceRefreshActiveCell();
                }
                else
                {
                    recruitEmptyRoot.SetActive(true);
                    recruitInfoRoot.SetActive(false);
                    createButton.gameObject.SetActive(false);
                }
            }
        }

        public void RefreshFire(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            Sys_WarriorGroup.Meeting_Fire meeting_Fire = meetingInfo as Sys_WarriorGroup.Meeting_Fire;
            if (meeting_Fire != null)
            {
                fireEmptyRoot.SetActive(!meeting_Fire.IsValid());
                fireInfoRoot.SetActive(meeting_Fire.IsValid());
                createButton.gameObject.SetActive(meeting_Fire.IsValid());


                int count = Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count - 1;
                if (count > 0)
                {
                    fireRoleGrid.CellCount = count;
                    fireRoleGrid.ForceRefreshActiveCell();
                }
            }
        }

        void RefreshChangeName(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            Sys_WarriorGroup.Meeting_ChangeName meeting_ChangeName = meetingInfo as Sys_WarriorGroup.Meeting_ChangeName;
            if (meeting_ChangeName != null)
            {
                TextHelper.SetText(changeNameOldName, Sys_WarriorGroup.Instance.MyWarriorGroup.GroupName);
            }
        }

        void RefreshChangeDeclaration(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            Sys_WarriorGroup.Meeting_ChangeDeclaration meeting_ChangeDeclaration = meetingInfo as Sys_WarriorGroup.Meeting_ChangeDeclaration;
            if (meeting_ChangeDeclaration != null)
            {

            }
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            createButton.onClick.AddListener(listener.OnClickCreateButton);
            changeDeclarationInputField.onValueChanged.AddListener(listener.OnChangeDeclarationInputFieldValueChanged);
            suggestSelfInputField.onValueChanged.AddListener(listener.OnSuggestSelfInputFieldValueChanged);
            changeDeclarationInputField.onValidateInput = OnValidateInput_ChangeDeclaration;
            suggestSelfInputField.onValidateInput = OnValidateInput_SuggestSelf;
        }

        char OnValidateInput_ChangeDeclaration(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > int.Parse(CSVParam.Instance.GetConfData(1375).str_value))
            {
                return '\0';
            }
            return addedChar;
        }

        char OnValidateInput_SuggestSelf(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > int.Parse(CSVParam.Instance.GetConfData(1375).str_value))
            {
                return '\0';
            }
            return addedChar;
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickCreateButton();

            void OnChangeDeclarationInputFieldValueChanged(string value);

            void OnSuggestSelfInputFieldValueChanged(string value);
        }
    }

    public class UI_WarriorGroup_CreateMeeting : UIBase, UI_WarriorGroup_CreateMeeting_Layout.IListener
    {
        UI_WarriorGroup_CreateMeeting_Layout layout = new UI_WarriorGroup_CreateMeeting_Layout();

        Sys_WarriorGroup.MeetingInfoBase meetingInfo;

        ulong currentSelectRecruitID;
        ulong currentSelectFireID;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.recruitRoleGrid.onCreateCell += RecruitRoleGridCreateCell;
            layout.recruitRoleGrid.onCellChange += RecruitRoleGridCellChange;

            layout.fireRoleGrid.onCreateCell += FireRoleGridCreateCell;
            layout.fireRoleGrid.onCellChange += FireRoleGridCellChange;
        }
        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.QuitSuccessed, OnQuitSuccessed, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.AddedNewMember, OnAddedNewMember, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RevomedMember, OnRevomedMember, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle<Sys_WarriorGroup.MeetingInfoBase>(Sys_WarriorGroup.EEvents.AddNewDoingMeeting, OnAddNewDoingMeeting, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            meetingInfo = arg as Sys_WarriorGroup.MeetingInfoBase;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
            currentSelectRecruitID = 0;
            currentSelectFireID = 0;
        }

        void OnQuitSuccessed()
        {
            CloseSelf();
        }

        void OnAddedNewMember()
        {
            if (layout.fireRoot.activeSelf && layout.fireInfoRoot.activeSelf)
            {
                layout.RefreshFire(meetingInfo);
            }
        }

        void OnRevomedMember()
        {
            if (layout.fireRoot.activeSelf && layout.fireInfoRoot.activeSelf)
            {
                layout.RefreshFire(meetingInfo);
            }
        }

        void OnAddNewDoingMeeting(Sys_WarriorGroup.MeetingInfoBase meetingInfoBase)
        {
            if (meetingInfoBase.CreateRoleId == Sys_Role.Instance.RoleId && meetingInfoBase.InfoID == meetingInfo.InfoID)
            {
                CloseSelf();
            }
        }

        void FireRoleGridCreateCell(InfinityGridCell cell)
        {         
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_CreateMeeting_Layout.FireRoleItem itemCell = new UI_WarriorGroup_CreateMeeting_Layout.FireRoleItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void FireRoleGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_CreateMeeting_Layout.FireRoleItem mCell = cell.mUserData as UI_WarriorGroup_CreateMeeting_Layout.FireRoleItem;
            List<Sys_WarriorGroup.WarriorInfo> roleInfos = new List<Sys_WarriorGroup.WarriorInfo>();
            foreach (var warrior in Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Values)
            {
                if (warrior.RoleID == Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID)
                {
                }
                else
                {
                    roleInfos.Add(warrior);
                }
            }
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item, (ulong roleID) => 
                {
                    currentSelectFireID = roleID;
                });
            }
        }

        void RecruitRoleGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_CreateMeeting_Layout.RecruitRoleItem itemCell = new UI_WarriorGroup_CreateMeeting_Layout.RecruitRoleItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RecruitRoleGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_CreateMeeting_Layout.RecruitRoleItem mCell = cell.mUserData as UI_WarriorGroup_CreateMeeting_Layout.RecruitRoleItem;
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitlerForWarriorGroup(uint.Parse(CSVParam.Instance.GetConfData(1378).str_value), uint.Parse(CSVParam.Instance.GetConfData(1368).str_value));
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item, (ulong roleID) =>
                {
                    currentSelectRecruitID = roleID;
                });
            }
        }

        void UpdateInfo()
        {
            if (meetingInfo == null)
                return;

            layout.Refresh(meetingInfo);
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_WarriorGroup_CreateMeeting);
        }

        public void OnChangeDeclarationInputFieldValueChanged(string value)
        {
            int count = TextHelper.GetCharNum(value);
            int max = int.Parse(CSVParam.Instance.GetConfData(1375).str_value);          
            TextHelper.SetText(layout.changeDeclarationTextCount, $"{count}/{max}");
        }

        public void OnSuggestSelfInputFieldValueChanged(string value)
        {
            int count = TextHelper.GetCharNum(value);
            int max = int.Parse(CSVParam.Instance.GetConfData(1375).str_value);
            TextHelper.SetText(layout.suggestSelfTextCount, $"{count}/{max}");
        }

        public void OnClickCreateButton()
        {
            if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.SuggestSelf)
            {
                Sys_WarriorGroup.Instance.ReqCreateMeeting_SuggestSelf(meetingInfo.InfoID, layout.suggestSelfInputField.text);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.Recruit)
            {
                if (currentSelectRecruitID == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13580));
                    return;
                }
                Sys_WarriorGroup.Instance.ReqCreateMeeting_Recruit(meetingInfo.InfoID, currentSelectRecruitID);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.Fire)
            {
                if (currentSelectFireID == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13580));
                    return;
                }
                if (Sys_Role.Instance.RoleId == currentSelectFireID)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13578));
                    return;
                }
                Sys_WarriorGroup.Instance.ReqCreateMeeting_Fire(meetingInfo.InfoID, currentSelectFireID);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.ChangeName)
            {
                if (string.IsNullOrEmpty(layout.changeNameInputField.text))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13557));
                    return;
                }

                if (TextHelper.GetCharNum(layout.changeNameInputField.text) < UI_CreateWarriorGroup_Layout.minLimit_Name || TextHelper.GetCharNum(layout.changeNameInputField.text) > UI_CreateWarriorGroup_Layout.maxLimit_Name)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13544));
                    return;
                }

                if (Sys_RoleName.Instance.HasBadNames(layout.changeNameInputField.text) || Sys_WordInput.Instance.HasLimitWord(layout.changeNameInputField.text))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10023));
                    return;
                }

                Sys_WarriorGroup.Instance.ReqCreateMeeting_ChangeName(meetingInfo.InfoID, layout.changeNameInputField.text);
            }
            else if (meetingInfo.InfoID == (uint)Sys_WarriorGroup.MeetingInfoBase.MeetingType.ChangeDeclaration)
            {
                Sys_WarriorGroup.Instance.ReqCreateMeeting_ChangeDeclaration(meetingInfo.InfoID, layout.changeDeclarationInputField.text);
            }
        }
    }
}
