using Table;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    /// 勇者团介绍UI布局///
    /// </summary>
    public class UI_DescWarriorGroup_Layout
    {
        /// <summary>
        /// 联系好友Item///
        /// </summary>
        public class ContactFriendItem
        {
            Sys_Society.RoleInfo roleInfo;

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

                contactButton = root.FindChildByName("ContactButton").GetComponent<Button>();
                contactButton.onClick.AddListener(OnClickContactButton);
            }

            public void UpdateItem(Sys_Society.RoleInfo _roleInfo)
            {
                roleInfo = _roleInfo;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);
                //HZCTODO
                TextHelper.SetText(roleLv, $"等级{roleInfo.level}级");
                ImageHelper.SetIcon(occIcon, CSVCareer.Instance.GetConfData(roleInfo.occ).icon);
                TextHelper.SetText(occText, CSVCareer.Instance.GetConfData(roleInfo.occ).name);
                TextHelper.SetText(value, roleInfo.friendValue.ToString());
            }

            /// <summary>
            /// 点击了联系按钮///
            /// </summary>
            void OnClickContactButton()
            {
                if (roleInfo == null)
                    return;

                Sys_Society.Instance.OpenPrivateChat(roleInfo);
            }
        }

        public Transform transform;

        public Button closeButton;
        public Button createButton;
        public GameObject emptyRoot;

        public InfinityGrid roleInfinityGrid;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();
            createButton = transform.gameObject.FindChildByName("Btn_01").GetComponent<Button>();
            emptyRoot = transform.gameObject.FindChildByName("Empty");

            roleInfinityGrid = transform.gameObject.FindChildByName("Scroll View").GetComponent<InfinityGrid>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            createButton.onClick.AddListener(listener.onClickCreateButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void onClickCreateButton();
        }
    }

    /// <summary>
    /// 勇者团介绍///
    /// </summary>
    public class UI_DescWarriorGroup : UIBase, UI_DescWarriorGroup_Layout.IListener
    {
        UI_DescWarriorGroup_Layout layout = new UI_DescWarriorGroup_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.roleInfinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            layout.roleInfinityGrid.onCellChange += RoleInfinityGridCellChange;
        }

        protected override void ProcessEvents(bool toRegister)
        {
        }

        protected override void OnShow()
        {
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitler(uint.Parse(CSVParam.Instance.GetConfData(1389).str_value));
            layout.emptyRoot.SetActive(roleInfos.Count == 0);
            if (roleInfos.Count > 0)
            {
                layout.roleInfinityGrid.CellCount = roleInfos.Count;
                layout.roleInfinityGrid.ForceRefreshActiveCell();
            }
        }

        void RoleInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_DescWarriorGroup_Layout.ContactFriendItem itemCell = new UI_DescWarriorGroup_Layout.ContactFriendItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RoleInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_DescWarriorGroup_Layout.ContactFriendItem mCell = cell.mUserData as UI_DescWarriorGroup_Layout.ContactFriendItem;
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitler(uint.Parse(CSVParam.Instance.GetConfData(1389).str_value));
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
            UIManager.CloseUI(EUIID.UI_DescWarriorGroup);
        }

        /// <summary>
        /// 点击了创建按钮///
        /// </summary>
        public void onClickCreateButton()
        {
            OnClickCloseButton();
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(uint.Parse(CSVParam.Instance.GetConfData(1390).str_value));
        }
    }
}
