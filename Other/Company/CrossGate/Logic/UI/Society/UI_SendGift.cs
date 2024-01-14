using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class UI_SendGift_Layout
    {
        public class RoleInfoItem
        {
            GameObject root;

            public Image roleIcon;
            public Image roleIconFrame;
            public Text roleName;
            public Text roleLv;
            public Text value;
            public Toggle toggle;
            public GameObject selectImage;

            Sys_Society.RoleInfo roleInfo;
            public Action<ulong> action;

            public void BindGameObject(GameObject go)
            {
                root = go;

                roleIcon = root.FindChildByName("Head").GetComponent<Image>();
                roleIconFrame = root.FindChildByName("Image_Before_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                roleLv = root.FindChildByName("Text_Lv").GetComponent<Text>();
                value = root.FindChildByName("Text").GetComponent<Text>();
                toggle = root.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                selectImage = root.FindChildByName("Image_Select");
            }

            public void UpdateItem(Sys_Society.RoleInfo _roleInfo, Action<ulong> _action)
            {
                roleInfo = _roleInfo;
                action = _action;

                ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(roleInfo.heroID, roleInfo.iconId));
                ImageHelper.SetIcon(roleIconFrame, CSVHeadframe.Instance.GetConfData(roleInfo.iconFrameId).HeadframeIcon, true);
                TextHelper.SetText(roleName, roleInfo.roleName);
                //HZCTODO
                TextHelper.SetText(roleLv, LanguageHelper.GetTextContent(2029408, roleInfo.level.ToString()));
                TextHelper.SetText(value, roleInfo.friendValue.ToString());
            }

            void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(roleInfo.roleID);
                }
            }
        }

        public GameObject root;
        public Button closeButton;

        public GameObject leftRoot;
        public CP_Toggle friendToggle;
        public CP_Toggle familyToggle;
        public InfinityGrid roleInfinityGrid;

        public GameObject middleRoot;
        public CP_Toggle giftToggle;
        public CP_Toggle commonToggle;
        public CP_Toggle unCommonToggle;
        public InfinityGrid itemInfinityGrid;
        public GameObject itemSendNumRoot;
        public GameObject itemSendToday;
        public GameObject itemSendWeek;
        public Text itemSendNumText;
        public GameObject commonTipRoot;
        public GameObject unCommonTipRoot;

        public GameObject rightRoot;      
        public GameObject selectItemGo;
        public Text itemName;
        public Text itemDesc;
        public Button subButton;
        public Button addButton;
        public Button maxButton;
        public Button sendButton;
        public InputField inputField;
        public PropItem selectItem;
        public GameObject addValueRoot;
        public Text addValueText;
        public GameObject emptyRoot;
        public GameObject normalRoot;

        public GameObject allCostRoot;
        public Image allCostImage;
        public Text allCostNum;

        public void Init(GameObject obj)
        {
            root = obj;
            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();

            leftRoot = root.FindChildByName("View_Left");
            friendToggle = leftRoot.FindChildByName("Toggle0").GetComponent<CP_Toggle>();
            familyToggle = leftRoot.FindChildByName("Toggle1").GetComponent<CP_Toggle>();
            roleInfinityGrid = leftRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();

            middleRoot = root.FindChildByName("View_Center");
            giftToggle = middleRoot.FindChildByName("Toggle0").GetComponent<CP_Toggle>();
            commonToggle = middleRoot.FindChildByName("Toggle1").GetComponent<CP_Toggle>();
            unCommonToggle = middleRoot.FindChildByName("Toggle2").GetComponent<CP_Toggle>();
            itemInfinityGrid = middleRoot.FindChildByName("Scroll View").GetComponent<InfinityGrid>();
            itemSendNumRoot = middleRoot.FindChildByName("Text_Num");
            itemSendToday = itemSendNumRoot.FindChildByName("Text");
            itemSendWeek = itemSendNumRoot.FindChildByName("Text2");
            itemSendNumText = itemSendNumRoot.GetComponent<Text>();
            commonTipRoot = middleRoot.FindChildByName("Text_Limit_wupin");
            unCommonTipRoot = middleRoot.FindChildByName("Text_Limit");

            rightRoot = root.FindChildByName("View_Right");
            selectItemGo = rightRoot.FindChildByName("PropItem");
            itemName = selectItemGo.FindChildByName("Text_Name").GetComponent<Text>();
            itemDesc = rightRoot.FindChildByName("Text_Detail").GetComponent<Text>();
            subButton = rightRoot.FindChildByName("Button_Sub").GetComponent<Button>();
            addButton = rightRoot.FindChildByName("Button_Add").GetComponent<Button>();
            maxButton = rightRoot.FindChildByName("Button_Max").GetComponent<Button>();
            sendButton = rightRoot.FindChildByName("Btn_01").GetComponent<Button>();
            inputField = rightRoot.FindChildByName("InputField").GetComponent<InputField>();
            addValueRoot = rightRoot.FindChildByName("Add");
            addValueText = addValueRoot.FindChildByName("Text_Num").GetComponent<Text>();
            emptyRoot = rightRoot.FindChildByName("Empty");
            normalRoot = rightRoot.FindChildByName("Normal");

            allCostRoot = rightRoot.FindChildByName("Add");
            allCostImage = allCostRoot.FindChildByName("Image_Icon").GetComponent<Image>();
            allCostNum = allCostRoot.FindChildByName("Cost").GetComponent<Text>();

            selectItem = new PropItem();
            selectItem.BindGameObject(selectItemGo);

        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);

            friendToggle.onValueChanged.AddListener(listener.OnFriendToggleValueChange);
            familyToggle.onValueChanged.AddListener(listener.OnFamilyToggleValueChange);

            giftToggle.onValueChanged.AddListener(listener.OnGiftToggleValueChange);
            commonToggle.onValueChanged.AddListener(listener.OnCommonToggleValueChange);
            unCommonToggle.onValueChanged.AddListener(listener.OnUnCommonToggleValueChange);

            subButton.onClick.AddListener(listener.OnClickSubButton);
            addButton.onClick.AddListener(listener.OnClickAddButton);
            maxButton.onClick.AddListener(listener.OnClickMaxButton);
            sendButton.onClick.AddListener(listener.OnClickSendButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnFriendToggleValueChange(bool isOn);

            void OnFamilyToggleValueChange(bool isOn);

            void OnGiftToggleValueChange(bool isOn);

            void OnCommonToggleValueChange(bool isOn);

            void OnUnCommonToggleValueChange(bool isOn);

            void OnClickSubButton();

            void OnClickAddButton();

            void OnClickMaxButton();

            void OnClickSendButton();
        }
    }

    public class UI_SendGift : UIBase, UI_SendGift_Layout.IListener
    {
        private UI_SendGift_Layout layout = new UI_SendGift_Layout();
        UI_CurrencyTitle UI_CurrencyTitle;

        public static ulong curSelectRoleID;
        public static uint curSelectItemID;
        public static uint curSelectTab;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);

            layout.roleInfinityGrid.onCreateCell += RoleInfinityGridCreateCell;
            layout.roleInfinityGrid.onCellChange += RoleInfinityGridCellChange;

            layout.itemInfinityGrid.onCreateCell += ItemInfinityGridCreateCell;
            layout.itemInfinityGrid.onCellChange += ItemInfinityGridCellChange;

            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            transform.Find("Animator/UI_Property").gameObject.SetActive(true);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<ulong, uint>(Sys_Society.EEvents.OnIntimacyChange, OnIntimacyChange, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<ulong, uint, uint>(Sys_Society.EEvents.OnSendGiftSuccess, OnSendGiftSuccess, toRegister);
        }


        protected override void OnOpen(object arg)
        {
            List<ulong> args = arg as List<ulong>;
            if (args != null && args.Count > 0)
            {
                curSelectTab = (uint)args[0];
                if (args.Count > 1)
                {
                    curSelectRoleID = args[1];
                    if (args.Count > 2)
                    {
                        curSelectItemID = (uint)args[2];
                    }
                }
            }
        }

        protected override void OnShow()
        {
            UI_CurrencyTitle.InitUi();

            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
            if (cSVItemData != null)
            {
                if (cSVItemData.PresentType == 1)
                {
                    layout.giftToggle.SetSelected(true, false);
                    layout.giftToggle.onValueChanged.Invoke(true);                
                }
                else if (cSVItemData.PresentType == 2)
                {
                    layout.commonToggle.SetSelected(true, false);
                    layout.commonToggle.onValueChanged.Invoke(true);
                }
                else if (cSVItemData.PresentType == 3)
                {
                    layout.unCommonToggle.SetSelected(true, false);
                    layout.unCommonToggle.onValueChanged.Invoke(true);
                }
                layout.addValueRoot.SetActive(true);
            }
            else
            {
                layout.giftToggle.SetSelected(true, false);
                layout.giftToggle.onValueChanged.Invoke(true);

                layout.itemName.text = string.Empty;
                layout.itemDesc.text = string.Empty;
                layout.addValueRoot.SetActive(false);
            }

            if (curSelectTab == 0)
            {
                layout.friendToggle.SetSelected(true, false);
                layout.friendToggle.onValueChanged.Invoke(true);
            }
            else if (curSelectTab == 1)
            {
                layout.familyToggle.SetSelected(true, false);
                layout.familyToggle.onValueChanged.Invoke(true);  
            }   
        }

        protected override void OnDestroy()
        {
            UI_CurrencyTitle.Dispose();
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_SendGift);
            curSelectTab = 0;
            curSelectRoleID = 0;
            curSelectItemID = 0;
        }

        #region Left

        public void OnFriendToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.roleInfinityGrid.CellCount = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos().Count;
                layout.roleInfinityGrid.ForceRefreshActiveCell();

                var list = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos();
                for (int index = 0, len = list.Count; index < len; index++)
                {
                    if (list[index].roleID == curSelectRoleID)
                    {
                        layout.roleInfinityGrid.MoveToIndex(index);
                    }
                }
            }
        }

        public void OnFamilyToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.roleInfinityGrid.CellCount = 0;
                layout.roleInfinityGrid.ForceRefreshActiveCell();
            }
        }

        void RoleInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_SendGift_Layout.RoleInfoItem itemCell = new UI_SendGift_Layout.RoleInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void RoleInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_SendGift_Layout.RoleInfoItem mCell = cell.mUserData as UI_SendGift_Layout.RoleInfoItem;
            List<Sys_Society.RoleInfo> roleInfos = Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos();
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item, (roleID) =>
                {
                    curSelectRoleID = roleID;
                    if (curSelectItemID != 0)
                    {
                        CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
                        if (cSVItemData.PresentType == 2)
                        {
                            layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].commonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited2}";
                        }
                        else if (cSVItemData.PresentType == 3)
                        {
                            layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].unCommonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited3}";
                        }

                        
                    }
                    else
                    {
                        if (layout.giftToggle.IsOn)
                        {

                        }
                        else if (layout.commonToggle.IsOn)
                        {
                            layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].commonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited2}";
                        }
                        else if (layout.unCommonToggle.IsOn)
                        {
                            layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].unCommonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited3}";
                        }
                    }

                    if (layout.giftToggle.IsOn)
                    {
                        layout.itemSendNumRoot.SetActive(false);
                        layout.commonTipRoot.SetActive(false);
                        layout.unCommonTipRoot.SetActive(false);
                    }
                    else if (layout.commonToggle.IsOn)
                    {
                        if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue >= uint.Parse(CSVParam.Instance.GetConfData(1351).str_value))
                        {
                            layout.itemSendNumRoot.SetActive(true);
                            layout.itemSendToday.SetActive(true);
                            layout.itemSendWeek.SetActive(false);
                            layout.commonTipRoot.SetActive(false);
                            layout.unCommonTipRoot.SetActive(false);
                        }
                        else
                        {
                            layout.itemSendNumRoot.SetActive(false);
                            layout.commonTipRoot.SetActive(true);
                            layout.unCommonTipRoot.SetActive(false);
                        }
                    }
                    else if (layout.unCommonToggle.IsOn)
                    {
                        if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue >= uint.Parse(CSVParam.Instance.GetConfData(1352).str_value))
                        {
                            layout.itemSendNumRoot.SetActive(true);
                            layout.itemSendToday.SetActive(false);
                            layout.itemSendWeek.SetActive(true);
                            layout.commonTipRoot.SetActive(false);
                            layout.unCommonTipRoot.SetActive(false);
                        }
                        else
                        {
                            layout.itemSendNumRoot.SetActive(false);
                            layout.commonTipRoot.SetActive(false);
                            layout.unCommonTipRoot.SetActive(true);
                        }
                    }
                });

                if (item.roleID == curSelectRoleID)
                {
                    mCell.toggle.isOn = true;
                    mCell.toggle.onValueChanged.Invoke(true);
                }
            }
        }

        #endregion

        #region Middle

        public void OnGiftToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.itemInfinityGrid.CellCount = CSVItem.Instance.GiftDatas.Count;
                layout.itemInfinityGrid.ForceRefreshActiveCell();

                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
                if (cSVItemData != null && cSVItemData.PresentType == 1)
                {
                    int index = CSVItem.Instance.GetGiftIndexByID(curSelectItemID);
                    if (index != -1)
                    {
                        layout.itemInfinityGrid.MoveToIndex(index);
                    }
                }
                else
                {
                    curSelectItemID = 0;
                    ResetSelectItem();
                }

                layout.itemSendNumRoot.SetActive(false);
                layout.commonTipRoot.SetActive(false);
                layout.unCommonTipRoot.SetActive(false);
            }
        }

        public void OnCommonToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.itemInfinityGrid.CellCount = CSVItem.Instance.CommonItemDatas.Count;
                layout.itemInfinityGrid.ForceRefreshActiveCell();

                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
                if (cSVItemData != null && cSVItemData.PresentType == 2)
                {
                    int index = CSVItem.Instance.GetCommonIndexByID(curSelectItemID);
                    if (index != -1)
                    {
                        layout.itemInfinityGrid.MoveToIndex(index);
                    }
                }
                else
                {
                    curSelectItemID = 0;
                    ResetSelectItem();
                }

                if (curSelectRoleID != 0)
                {
                    if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue >= uint.Parse(CSVParam.Instance.GetConfData(1351).str_value))
                    {
                        layout.itemSendNumRoot.SetActive(true);
                        layout.itemSendToday.SetActive(true);
                        layout.itemSendWeek.SetActive(false);
                        layout.commonTipRoot.SetActive(false);
                        layout.unCommonTipRoot.SetActive(false);
                        layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].commonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited2}";
                    }
                    else
                    {
                        layout.itemSendNumRoot.SetActive(false);
                        layout.commonTipRoot.SetActive(true);
                        layout.unCommonTipRoot.SetActive(false);
                    }
                }
                else
                {
                    layout.itemSendNumRoot.SetActive(false);
                    layout.commonTipRoot.SetActive(false);
                    layout.unCommonTipRoot.SetActive(false);
                }

            }
        }

        public void OnUnCommonToggleValueChange(bool isOn)
        {
            if (isOn)
            {
                layout.itemInfinityGrid.CellCount = CSVItem.Instance.UnCommonItemDatas.Count;
                layout.itemInfinityGrid.ForceRefreshActiveCell();

                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
                if (cSVItemData != null && cSVItemData.PresentType == 3)
                {
                    int index = CSVItem.Instance.GetUnCommonIndexByID(curSelectItemID);
                    if (index != -1)
                    {
                        layout.itemInfinityGrid.MoveToIndex(index);
                    }
                }
                else
                {
                    curSelectItemID = 0;
                    ResetSelectItem();
                }

                if (curSelectRoleID != 0)
                {
                    if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue >= uint.Parse(CSVParam.Instance.GetConfData(1352).str_value))
                    {
                        layout.itemSendNumRoot.SetActive(true);
                        layout.itemSendToday.SetActive(false);
                        layout.itemSendWeek.SetActive(true);
                        layout.commonTipRoot.SetActive(false);
                        layout.unCommonTipRoot.SetActive(false);
                        layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].unCommonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited3}";
                    }
                    else
                    {
                        layout.itemSendNumRoot.SetActive(false);
                        layout.commonTipRoot.SetActive(false);
                        layout.unCommonTipRoot.SetActive(true);
                    }
                }
                else
                {
                    layout.itemSendNumRoot.SetActive(false);
                    layout.commonTipRoot.SetActive(false);
                    layout.unCommonTipRoot.SetActive(false);
                }
            }
        }

        public class PropItem_SendGift : PropItem
        {
            public GameObject costRoot;
            public Image costItemIcon;
            public Text costItemNum;

            public void BindGameObject_SendGiftPlus(GameObject go)
            {
                costRoot = go.FindChildByName("Cost");
                costItemIcon = costRoot.FindChildByName("Icon").GetComponent<Image>();
                costItemNum = costRoot.GetComponent<Text>();
            }

            public void ShowCost(PropIconLoader.ShowItemData itemData, CSVItem.Data cSVItemData)
            {
                costRoot.SetActive(cSVItemData.PresentType == 3);
                if (cSVItemData.PresentType == 3)
                {
                    txtNumber.gameObject.SetActive(false);
                    txtNumberZero.gameObject.SetActive(false);
                    btnNone.gameObject.SetActive(false);

                    CSVFriendSendItem.Data csVFriendSendItemData = CSVFriendSendItem.Instance.GetConfData(cSVItemData.id);
                    if (csVFriendSendItemData != null)
                    {
                        ImageHelper.SetIcon(costItemIcon, CSVItem.Instance.GetConfData(csVFriendSendItemData.itemPrice_type).icon_id);
                        TextHelper.SetText(costItemNum, $"x{csVFriendSendItemData.itemPrice.ToString()}");
                    }
                }
                else
                {
                    txtNumber.gameObject.SetActive(!(itemData.bagGridCount == 0));
                    txtNumberZero.gameObject.SetActive(itemData.bagGridCount == 0);
                }              
            }
        }

        void ItemInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            PropItem_SendGift itemCell = new PropItem_SendGift();
            itemCell.BindGameObject(go);
            itemCell.BindGameObject_SendGiftPlus(go);
            cell.BindUserData(itemCell);
        }

        void ItemInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            PropItem_SendGift mCell = cell.mUserData as PropItem_SendGift;
            if (layout.giftToggle.IsOn)
            {
                List<CSVItem.Data>  items = CSVItem.Instance.GiftDatas;
                items.Sort((a, b) =>
                {
                    if (Sys_Bag.Instance.GetItemCount(a.id) > 0)
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                });

                if (index < items.Count)
                {
                    var item = items[index];
                    SetMCell(item, mCell);
                }
            }
            else if (layout.commonToggle.IsOn)
            {
                List<CSVItem.Data>  items = CSVItem.Instance.CommonItemDatas;
                items.Sort((a, b) =>
                {
                    if (Sys_Bag.Instance.GetItemCount(a.id) > 0)
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                });
                if (index < items.Count)
                {
                    var item = items[index];
                    SetMCell(item, mCell);
                }
            }
            else if (layout.unCommonToggle.IsOn)
            {
                List<CSVItem.Data>  items = CSVItem.Instance.UnCommonItemDatas;
                items.Sort((a, b) =>
                {
                    if (Sys_Bag.Instance.GetItemCount(a.id) > 0)
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (Sys_Bag.Instance.GetItemCount(b.id) > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            if (a.id > b.id)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                });
                if (index < items.Count)
                {
                    var item = items[index];
                    SetMCell(item, mCell);
                }
            }
        }

        void SetMCell(CSVItem.Data item, PropItem mCell)
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(item.id, Sys_Bag.Instance.GetItemCount(item.id), true, false, false, false, false, true, true, true, (PropItem propItem) =>
            {
                if (item.PresentType == 3)
                {
                    UpdateSelectItem(propItem);
                }
                else
                {
                    if (propItem.ItemData.bagGridCount > 0)
                    {
                        UpdateSelectItem(propItem);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_SendGift, propItem.ItemData));
                    }
                }
            }, false, false);
            itemData.bCopyBagGridCount = true;
            itemData.bagGridCount = (int)Sys_Bag.Instance.GetItemCount(item.id);
            mCell.SetData(itemData, EUIID.UI_SendGift);         
            mCell.BtnBoneShow(itemData.bagGridCount == 0);
            ((PropItem_SendGift)mCell).ShowCost(itemData, item);
            if (item.id == curSelectItemID)
            {
                UpdateSelectItem(mCell);
            }
        }

        #endregion

        #region Right

        void ResetSelectItem()
        {
            for (int index = 0, len = layout.itemInfinityGrid.gameObject.FindChildByName("Content").transform.childCount; index < len; index++)
            {
                layout.itemInfinityGrid.gameObject.FindChildByName("Content").transform.GetChild(index).gameObject.FindChildByName("Image_Select").SetActive(false);
            }
            layout.selectItem.SetEmpty();
            TextHelper.SetText(layout.itemName, string.Empty);
            TextHelper.SetText(layout.itemDesc, string.Empty);
            layout.inputField.text = string.Empty;
            layout.addValueRoot.SetActive(false);

            layout.emptyRoot.SetActive(true);
            layout.normalRoot.SetActive(false);
        }

        void UpdateSelectItem(PropItem propItem)
        {
            curSelectItemID = propItem.ItemData.id;
            for (int index = 0, len = layout.itemInfinityGrid.gameObject.FindChildByName("Content").transform.childCount; index < len; index++)
            {
                layout.itemInfinityGrid.gameObject.FindChildByName("Content").transform.GetChild(index).gameObject.FindChildByName("Image_Select").SetActive(false);
            }
            propItem.SetSelected(true);           
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(propItem.ItemData.id, 0, true, false, false, false, false, false, false, false, null, false, true);
            layout.selectItem.SetData(itemData, EUIID.UI_SendGift);

            var csvItemData = CSVItem.Instance.GetConfData(itemData.id);

            TextHelper.SetText(layout.itemName, csvItemData.name_id);
            TextHelper.SetText(layout.itemDesc, csvItemData.describe_id);
            layout.inputField.text = "1";

            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
            if (cSVItemData.PresentIntimacy > 0)
            {
                layout.addValueRoot.SetActive(true);
                TextHelper.SetText(layout.addValueText, cSVItemData.PresentIntimacy.ToString());
            }
            else
            {
                layout.addValueRoot.SetActive(false);
            }

            layout.emptyRoot.SetActive(false);
            layout.normalRoot.SetActive(true);

            if (csvItemData.PresentType == 3)
            {
                layout.allCostRoot.SetActive(true);
                CSVFriendSendItem.Data cSVFriendSendItemData = CSVFriendSendItem.Instance.GetConfData(csvItemData.id);
                ImageHelper.SetIcon(layout.allCostImage, CSVItem.Instance.GetConfData(cSVFriendSendItemData.itemPrice_type).icon_id);

                if (Sys_Bag.Instance.GetItemCount(cSVFriendSendItemData.itemPrice_type) >= cSVFriendSendItemData.itemPrice)
                {
                    layout.inputField.text = "1";
                    TextHelper.SetText(layout.allCostNum, $"{cSVFriendSendItemData.itemPrice}");
                }
                else
                {
                    layout.inputField.text = "0";
                    TextHelper.SetText(layout.allCostNum, "0");
                }
            }
            else
            {
                layout.allCostRoot.SetActive(false);
            }
        }

        public void OnClickSubButton()
        {
            if (curSelectItemID != 0)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);

                int num = System.Convert.ToInt32(layout.inputField.text);
                if (num > 0)
                    num--;
                layout.inputField.text = num.ToString();

                if (cSVItemData.PresentType == 3)
                {
                    TextHelper.SetText(layout.addValueText, (cSVItemData.PresentIntimacy * num).ToString());
                    TextHelper.SetText(layout.allCostNum, $"{CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice * System.Convert.ToInt32(layout.inputField.text)}");
                }
            }
        }

        public void OnClickAddButton()
        {
            if (curSelectItemID != 0)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);

                int num = System.Convert.ToInt32(layout.inputField.text);
                long numMax;
                if (cSVItemData.PresentType != 3)
                {
                    numMax = Sys_Bag.Instance.GetItemCount(curSelectItemID);
                }
                else
                {
                    numMax = Sys_Bag.Instance.GetItemCount(CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice_type) / CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice;
                }
                if (num < numMax)
                {
                    num++;
                }
                else
                {
                    if (cSVItemData.PresentType == 3)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13021));
                    }
                }
                layout.inputField.text = num.ToString();

                if (cSVItemData.PresentType == 3)
                {
                    TextHelper.SetText(layout.addValueText, (cSVItemData.PresentIntimacy * num).ToString());
                    TextHelper.SetText(layout.allCostNum, $"{CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice * System.Convert.ToInt32(layout.inputField.text)}");
                }
            }
        }

        public void OnClickMaxButton()
        {
            if (curSelectItemID != 0)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
                long num;
                if (cSVItemData.PresentType != 3)
                {
                    num = Sys_Bag.Instance.GetItemCount(curSelectItemID);
                }
                else
                {
                    num = Sys_Bag.Instance.GetItemCount(CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice_type) / CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice;
                }

                if (cSVItemData.PresentType == 3 && num == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13021));
                }

                layout.inputField.text = num.ToString();

                if (cSVItemData.PresentType == 3)
                {
                    TextHelper.SetText(layout.addValueText, (cSVItemData.PresentIntimacy * num).ToString());
                    TextHelper.SetText(layout.allCostNum, $"{CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice * System.Convert.ToInt32(layout.inputField.text)}");
                }
            }
        }

        public void OnClickSendButton()
        {
            if (curSelectRoleID == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13027));
                return;
            }

            if (curSelectItemID == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13006));
                return;
            }

            if (string.IsNullOrEmpty(layout.inputField.text))
                return;

            uint count = System.Convert.ToUInt32(layout.inputField.text);

            if (count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13026));
                return;
            }

            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(curSelectItemID);
            if (cSVItemData.PresentType == 2)
            {
                if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue < uint.Parse(CSVParam.Instance.GetConfData(1351).str_value))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13024));
                    return;
                }

                if ((Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].commonSendNum + count) > CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited2)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13005));
                    return;
                }
            
            }
            else if (cSVItemData.PresentType == 3)
            {
                if (Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue < uint.Parse(CSVParam.Instance.GetConfData(1352).str_value))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13025));
                    return;
                }

                if ((Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].unCommonSendNum + count) > CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited3)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13005));
                    return;
                }

                if (Sys_Bag.Instance.GetItemCount(CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice_type) < CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice * count)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13021));
                }
            }

            if (cSVItemData.PresentType == 3)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13022, count.ToString(), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(curSelectItemID).name_id), Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].roleName, (CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice * count).ToString(), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(CSVFriendSendItem.Instance.GetConfData(curSelectItemID).itemPrice_type).name_id));
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Society.Instance.SendGift(curSelectRoleID, curSelectItemID, count);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Society.Instance.SendGift(curSelectRoleID, curSelectItemID, count);
            }           
        }

        #endregion

        void OnSendGiftSuccess(ulong roleID, uint itemID, uint count)
        {
            layout.itemInfinityGrid.ForceRefreshActiveCell();

            if (layout.commonToggle.IsOn)
            {
                layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].commonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited2}";
            }
            else if (layout.unCommonToggle.IsOn)
            {
                layout.itemSendNumText.text = $"{Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].unCommonSendNum} / {CSVFriendIntimacy.Instance.GetDataByIntimacyValue(Sys_Society.Instance.socialRolesInfo.rolesDic[curSelectRoleID].friendValue).PresentLimited3}";
            }
        }

        void OnIntimacyChange(ulong roleID, uint value)
        {
            layout.roleInfinityGrid.ForceRefreshActiveCell();
        }
    }
}
