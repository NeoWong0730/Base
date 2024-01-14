using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_WarriorGroup_Invite_Layout
    {
        public class FriendItem
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

            public Button inviteButton;

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

                inviteButton = root.FindChildByName("Btn_01_Small").GetComponent<Button>();
                inviteButton.onClick.AddListener(OnClickInviteButton);
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
            void OnClickInviteButton()
            {
                if (roleInfo == null)
                    return;

                if (Sys_WarriorGroup.Instance.MyWarriorGroup.MemberCount < Sys_WarriorGroup.Instance.memberMaxCount)
                {
                    Sys_WarriorGroup.Instance.ReqInviteFriendIntoGroup(roleInfo.roleID);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13558));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13562));
                }
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

    public class UI_WarriorGroup_Invite : UIBase, UI_WarriorGroup_Invite_Layout.IListener
    {
        UI_WarriorGroup_Invite_Layout layout = new UI_WarriorGroup_Invite_Layout();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

            layout.roleInfinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            layout.roleInfinityGrid.onCellChange += RoleInfinityGridCellChange;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.QuitSuccessed, OnQuitSuccessed, toRegister);
        }

        protected override void OnShow()
        {
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitlerForWarriorGroup(uint.Parse(CSVParam.Instance.GetConfData(1378).str_value), uint.Parse(CSVParam.Instance.GetConfData(1368).str_value));
            layout.emptyRoot.SetActive(roleInfos.Count == 0);
            if (roleInfos.Count > 0)
            {
                layout.roleInfinityGrid.CellCount = roleInfos.Count;
                layout.roleInfinityGrid.ForceRefreshActiveCell();
            }
        }

        void OnQuitSuccessed()
        {
            CloseSelf();
        }

        void RoleInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Invite_Layout.FriendItem itemCell = new UI_WarriorGroup_Invite_Layout.FriendItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RoleInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Invite_Layout.FriendItem mCell = cell.mUserData as UI_WarriorGroup_Invite_Layout.FriendItem;
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfosByFitlerForWarriorGroup(uint.Parse(CSVParam.Instance.GetConfData(1378).str_value), uint.Parse(CSVParam.Instance.GetConfData(1368).str_value));
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
            UIManager.CloseUI(EUIID.UI_WarriorGroup_Invite);
        }
    }
}