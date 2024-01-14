using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_WarriorGroup_Transfer_Layout
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

            public Button transferButton;

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

                transferButton = root.FindChildByName("Btn_01_Small").GetComponent<Button>();
                transferButton.onClick.AddListener(OnClickTransferButton);
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

            /// <summary>
            /// 点击了按钮///
            /// </summary>
            void OnClickTransferButton()
            {
                if (roleInfo == null)
                    return;

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13541, roleInfo.RoleName);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_WarriorGroup.Instance.ReqResetLeader(roleInfo.RoleID);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public Transform transform;

        public Button closeButton;
        public GameObject emptyRoot;

        public InfinityGrid roleInfinityGrid;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            emptyRoot = transform.gameObject.FindChildByName("Empty");

            roleInfinityGrid = transform.gameObject.FindChildByName("Scroll View").GetComponent<InfinityGrid>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();
        }
    }

    public class UI_WarriorGroup_Transfer : UIBase, UI_WarriorGroup_Transfer_Layout.IListener
    {
        UI_WarriorGroup_Transfer_Layout layout = new UI_WarriorGroup_Transfer_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.roleInfinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            layout.roleInfinityGrid.onCellChange += RoleInfinityGridCellChange;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RefrehedLeader, OnRefrehedLeader, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.QuitSuccessed, OnQuitSuccessed, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.AddedNewMember, OnAddedNewMember, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.RevomedMember, OnRevomedMember, toRegister);
        }

        void OnRefrehedLeader()
        {
            CloseSelf();
        }

        void OnQuitSuccessed()
        {
            CloseSelf();
        }

        void OnAddedNewMember()
        {
            RefreshRoles();
        }

        void OnRevomedMember()
        {
            RefreshRoles();
        }

        protected override void OnShow()
        {
            RefreshRoles();
        }

        void RefreshRoles()
        {
            int count = Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count - 1;
            layout.emptyRoot.SetActive(count == 0);
            if (count > 0)
            {
                layout.roleInfinityGrid.CellCount = count;
                layout.roleInfinityGrid.ForceRefreshActiveCell();
            }
        }

        void RoleInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Transfer_Layout.RoleItem itemCell = new UI_WarriorGroup_Transfer_Layout.RoleItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RoleInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Transfer_Layout.RoleItem mCell = cell.mUserData as UI_WarriorGroup_Transfer_Layout.RoleItem;
            List<Sys_WarriorGroup.WarriorInfo> roleInfos = new List<Sys_WarriorGroup.WarriorInfo>();
            foreach (var warrior in Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Values)
            {
                if (warrior.RoleID != Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID)
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
            UIManager.CloseUI(EUIID.UI_WarriorGroup_Transfer);
        }
    }
}
