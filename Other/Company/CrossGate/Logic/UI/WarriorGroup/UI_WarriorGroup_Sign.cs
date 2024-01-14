using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_WarriorGroup_Sign_Layout
    {
        public class RoleItem
        {
            Sys_WarriorGroup.WarriorInfo roleInfo;

            GameObject root;

            public Image roleIcon;
            public Image roleFrame;
            public Text roleName;
            public Text roleLv;
            public Image occIcon;
            public Text occText;
            public Text value;

            public Button contactButton;

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

                contactButton = root.FindChildByName("Button").GetComponent<Button>();
                contactButton.onClick.AddListener(OnClickContactButton);
            }

            public void UpdateItem(Sys_WarriorGroup.WarriorInfo _roleInfo)
            {
                roleInfo = _roleInfo;

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

            void OnClickContactButton()
            {
                if (roleInfo == null)
                    return;

                Sys_Society.Instance.OpenPrivateChat(new Sys_Society.RoleInfo()
                {
                    roleID = roleInfo.RoleID,
                    roleName = roleInfo.RoleName,
                    level = roleInfo.Level,
                    heroID = roleInfo.HeroID,
                    occ = roleInfo.Occ,
                    iconId = roleInfo.IconID,
                    iconFrameId = roleInfo.FrameID,
                });
            }
        }

        public Transform transform;

        public Button closeButton;

        public InfinityGrid roleInfinityGrid;
        public Text GroupName;
        public Text Desc;
        public Button AcceptButton;
        public Animator sign;


        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            roleInfinityGrid = transform.gameObject.FindChildByName("Scroll View").GetComponent<InfinityGrid>();
            GroupName = transform.gameObject.FindChildByName("Text_TeamName").GetComponent<Text>();
            Desc = transform.gameObject.FindChildByName("Text_Invite").GetComponent<Text>();
            AcceptButton = transform.gameObject.FindChildByName("Btn_01").GetComponent<Button>();
            sign = transform.gameObject.FindChildByName("Sign").GetComponent<Animator>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            AcceptButton.onClick.AddListener(listener.OnClickAcceptButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickAcceptButton();
        }
    }

    public class UI_WarriorGroup_Sign : UIBase, UI_WarriorGroup_Sign_Layout.IListener
    {
        UI_WarriorGroup_Sign_Layout layout = new UI_WarriorGroup_Sign_Layout();

        Sys_WarriorGroup.InvitedInfo invitedInfo;
        Timer signTimer;
        protected override void OnOpen(object arg)
        {
            invitedInfo = arg as Sys_WarriorGroup.InvitedInfo;
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.roleInfinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            layout.roleInfinityGrid.onCellChange += RoleInfinityGridCellChange;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.JoinedSuccess, OnJoinedSuccess, toRegister);
        }

        void OnJoinedSuccess()
        {
            layout.AcceptButton.gameObject.SetActive(false);
            layout.sign.gameObject.SetActive(true);
            layout.sign.Play("Open");
            signTimer?.Cancel();
            signTimer = Timer.Register(2.17f, () =>
            {
                layout.sign.gameObject.SetActive(false);
                CloseSelf();
                UIManager.OpenUI(EUIID.UI_WarriorGroup);
            }, null, false, true);
            
        }

        protected override void OnShow()
        {
            int count = invitedInfo.WarriorGroup.warriorInfos.Count;
            if (count > 0)
            {
                layout.roleInfinityGrid.CellCount = count;
                layout.roleInfinityGrid.ForceRefreshActiveCell();
            }
            RefreshInfo();
        }

        protected override void OnHide()
        {
            signTimer?.Cancel();
            signTimer = null;
        }

        void RefreshInfo()
        {
            TextHelper.SetText(layout.GroupName, invitedInfo.WarriorGroup.GroupName);
            TextHelper.SetText(layout.Desc, LanguageHelper.GetTextContent(13547, invitedInfo.RoleName, invitedInfo.WarriorGroup.GroupName));
        }

        void RoleInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Sign_Layout.RoleItem itemCell = new UI_WarriorGroup_Sign_Layout.RoleItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RoleInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Sign_Layout.RoleItem mCell = cell.mUserData as UI_WarriorGroup_Sign_Layout.RoleItem;
            List<Sys_WarriorGroup.WarriorInfo> roleInfos = new List<Sys_WarriorGroup.WarriorInfo>();
            foreach (var warrior in invitedInfo.WarriorGroup.warriorInfos.Values)
            {
                roleInfos.Add(warrior);
            }
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item);
            }
        }

        /// <summary>
        /// 点击了关闭按钮///
        /// </summary>
        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_WarriorGroup_Sign);
        }

        /// <summary>
        /// 点击了同意按钮///
        /// </summary>
        public void OnClickAcceptButton()
        {
            Sys_WarriorGroup.Instance.ReqAgreeInvite(invitedInfo.RoleID, invitedInfo.WarriorGroup.GroupUID);
        }
    }
}
