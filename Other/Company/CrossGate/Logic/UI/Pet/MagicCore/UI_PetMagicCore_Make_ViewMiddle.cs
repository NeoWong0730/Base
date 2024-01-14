using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;
using Packet;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_PetMagicCore_Make_ViewMiddle : UIComponent
    {
        private uint equipId;
        private uint selectRemakeItenId;
        private Image imgIcon;
        private Text txtName;
        private Text txtItemName1;
        private Text txtItemName2;
        private Text txtItemName3;
        private Text txtItemLv2;
        private Button previewBtn;
        private Button makeBtn;
        private UI_PetMagicCore_Make_Item selectItem;
        private UI_PetMagicCore_Make_Item costItem1;
        private UI_PetMagicCore_Make_Item costItem2;
        private GameObject selectTipsGo;
        private GameObject effectGo;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            InitPreview();
            effectGo?.SetActive(false);
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnSelectItem, OnSelectRemakeItemCallBack, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<ulong>(Sys_Pet.EEvents.OnPetEquipMakeSuccess, OnMakeSuccess, toRegister);
        }
        public override void OnDestroy()
        {
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, OnRefeshMainBagData, false);
            base.OnDestroy();
        }
        #endregion

        #region func
        
        public void UpdateView(uint type, uint id)
        {
            if (equipId != id)
            {
                InitPreview();
            }
            equipId = id;
            RefreshForgeView();
        }

        private void RefreshForgeView()
        {
            CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(equipId);
            if (petEquip != null)
            {
                CSVItem.Data item = CSVItem.Instance.GetConfData(equipId);
                imgIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(imgIcon, item.icon_id);
                txtName.text = LanguageHelper.GetTextContent(item.name_id);
                txtItemLv2.text = LanguageHelper.GetTextContent(1018007, petEquip.equipment_level.ToString());

                UpdateRemakeItemView();
                UpdateForgeBaseView(petEquip.forge_base);
            }
        }
        
        private void UpdateRemakeItemView()
        {
            uint key = 0;
            uint value = 0;
            if(selectRemakeItenId == 0)
            {
                CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(equipId);
                selectItem.SetPetEquipId(equipId);
                SetRemakeItemCeil(key, value);
                txtItemName1.text = "";
                selectTipsGo.gameObject.SetActive(true);
            }
            else
            {
                CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(equipId);
                if(null != petEquip && null != petEquip.forge_special)
                {
                    for (int i = 0; i < petEquip.forge_special.Count; i++)
                    {
                        if(petEquip.forge_special[i][0] == selectRemakeItenId)
                        {
                            value = petEquip.forge_special[i][1];
                        }
                    }
                    ItemIdCount item = new ItemIdCount(selectRemakeItenId, value);
                    selectItem.SetPetEquipId(equipId);
                    if (item.Enough)
                    {
                        SetRemakeItemCeil(selectRemakeItenId, value);
                    }
                    else
                    {
                        selectRemakeItenId = 0;
                        SetRemakeItemCeil(selectRemakeItenId, 0);
                    }
                }
                selectTipsGo.gameObject.SetActive(false);
            }
        }

        private void SetRemakeItemCeil(uint key, uint value)
        {
            selectItem.UpdateView(key, value);
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(key);
            CSVPetEquip.Data petEquipData = CSVPetEquip.Instance.GetConfData(key);
            string name = "";
            if (itemData != null)
            {
                name = LanguageHelper.GetTextContent(itemData.name_id);
            }
            txtItemName1.text = name;
        }

        /// <summary>
        /// 设置基础制作消耗道具
        /// </summary>
        /// <param name="costItems"></param>
        private void UpdateForgeBaseView(List<List<uint>> costItems)
        {
            uint key = 0;
            uint value = 0;
            uint key1 = 0;
            uint value1 = 0;
            string name = "";
            string name1 = "";

            if (costItems != null)
            {
                for (int i = 0; i < costItems.Count; i++)
                {
                    key = costItems[0][0];
                    value = costItems[0][1];
                    CSVItem.Data item = CSVItem.Instance.GetConfData(key);
                    if (null != item)
                        name = LanguageHelper.GetTextContent(item.name_id);
                    key1 = costItems[1][0];
                    value1 = costItems[1][1];
                    CSVItem.Data item1 = CSVItem.Instance.GetConfData(key1);
                    if(null != item1)
                        name1 = LanguageHelper.GetTextContent(item1.name_id);
                }
                
            }
            costItem1.UpdateView(key1, value1);
            costItem2.UpdateView(key, value);
            txtItemName3.text = name;
            txtItemName2.text = name1;

        }
        private void OnSelectRemakeItemCallBack(uint selectItem)
        {
            selectRemakeItenId = selectItem;
            UpdateRemakeItemView();
        }

        private void OnRefeshMainBagData(int boxId)
        {
            RefreshForgeView();
        }

        #endregion

        #region event
        private void OnPreviewBtnClikced()
        {
            if(equipId > 0)
            {
                UIManager.OpenUI(EUIID.UI_PetMagicCore_MakePreview, false, equipId);
            }
        }

        private void OnMakeBtnClicked()
        {
            CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(equipId);
            if (petEquip != null)
            {
                if (petEquip.forge_base != null)
                {
                    for (int i = 0; i < petEquip.forge_base.Count; i++)
                    {
                        uint id = petEquip.forge_base[i][0];
                        uint value  = petEquip.forge_base[i][1];
                        ItemIdCount itemIdCount = new ItemIdCount(id, value);
                        if(!itemIdCount.Enough)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000904, LanguageHelper.GetTextContent(itemIdCount.CSV.name_id)));
                            return;
                        }
                    }
                }

                if (Sys_Bag.Instance.BoxFull(Packet.BoxIDEnum.BoxIdNormal))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000900));
                    return;
                }
                else if (selectRemakeItenId == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000905));
                    return;
                }
                effectGo?.SetActive(false);
                effectGo?.SetActive(true);
                Sys_Pet.Instance.ItemBuildPetEquipReq(petEquip.id, selectRemakeItenId);
            }
        }
        #endregion

        public class UI_PetMagicCore_Make_Item : UIComponent
        {
            public bool isLeft;
            private uint petEquipId;
            private CSVItem.Data itemData;
            private GameObject goPlus;
            private Text txtNum;
            private Button btnItem;
            private Image imgItemIcon;
            private Image imgItemQuality;

            protected override void Loaded()
            {
                goPlus = transform.Find("Image_Add").gameObject;
                txtNum = transform.Find("Text_Number").GetComponent<Text>();
                txtNum.gameObject.SetActive(true);
                txtNum.text = "";
                btnItem = transform.Find("Btn_Item").GetComponent<Button>();
                btnItem.onClick.AddListener(OnItemClick);
                imgItemIcon = transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                imgItemQuality = transform.Find("Btn_Item/Image_Quality").GetComponent<Image>();
            }

            public void UpdateView(uint key, uint value)
            {
                itemData = CSVItem.Instance.GetConfData(key);
                if (itemData != null)
                {
                    imgItemIcon.gameObject.SetActive(true);
                    imgItemQuality.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgItemIcon, itemData.icon_id);
                    uint quality = itemData.quality;
                    ImageHelper.GetQualityColor_Frame(imgItemQuality, (int)quality);
                    goPlus.SetActive(false);
                    long hasNum = Sys_Bag.Instance.GetItemCount(itemData.id);
                    uint worldStyleId = hasNum >= value ? (uint)83 : 84;
                    TextHelper.SetText(txtNum, hasNum + "/" + value, CSVWordStyle.Instance.GetConfData(worldStyleId));
                }
                else
                {
                    imgItemIcon.gameObject.SetActive(false);
                    imgItemQuality.gameObject.SetActive(false);
                    goPlus.SetActive(isLeft);
                    txtNum.text = "";
                }
            }
            public void SetPetEquipId(uint _id)
            {
                petEquipId = _id;
            }
            private void OnItemClick()
            {
                if (petEquipId > 0)
                {
                    UIManager.OpenUI(EUIID.UI_PetSelectMagicMakeItem, false, new Tuple<uint, uint, uint>(petEquipId, 1018205, (uint)EItemType.PetRemakeItem));
                }
                else
                {
                    PropIconLoader.ShowItemData messageData = new PropIconLoader.ShowItemData(itemData.id, 0, true, false, false, false, false, true, false, true, null, false, true);
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_PetMagicCore, messageData));
                }
            }
        }
    }

    public partial class UI_PetMagicCore_Make_ViewMiddle : UIComponent
    {
        private Button m_MakeListButton;
        private List<ulong> m_MakeLists = new List<ulong>();
        private List<ulong> m_MakeListWillRemove = new List<ulong>();
        private GameObject m_ViewList;
        private Image m_ViewListClose;
        private InfinityGrid m_InfinityGrid;
        private Dictionary<GameObject, EquipGrid> m_EquipGrids = new Dictionary<GameObject, EquipGrid>();
        private bool b_ViewListOpen = false;

        private void Parse()
        {
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
            txtName = transform.Find("Text_Name").GetComponent<Text>();
            txtItemName1 = transform.Find("Text_Name_3").GetComponent<Text>();
            txtItemName2 = transform.Find("Text_Name_2").GetComponent<Text>();
            txtItemName3 = transform.Find("Text_Name_1").GetComponent<Text>();
            txtItemLv2 = transform.Find("Text_Level").GetComponent<Text>();
            previewBtn = transform.Find("Btn_Preview").GetComponent<Button>();
            previewBtn.onClick.AddListener(OnPreviewBtnClikced);
            makeBtn = transform.Find("Btn_Make").GetComponent<Button>();
            makeBtn.onClick.AddListener(OnMakeBtnClicked);
            selectItem = AddComponent<UI_PetMagicCore_Make_Item>(transform.Find("PropItem_3"));
            selectItem.isLeft = true;
            costItem1 = AddComponent<UI_PetMagicCore_Make_Item>(transform.Find("PropItem_2"));
            costItem2 = AddComponent<UI_PetMagicCore_Make_Item>(transform.Find("PropItem_1"));
            selectTipsGo = transform.Find("Text_Tips_3").gameObject;
            effectGo = transform.Find("bg/Fx_ui_PetMagicCore3_01").gameObject;

            m_MakeListButton = transform.Find("Btn_Check").GetComponent<Button>();
            m_MakeListButton.onClick.AddListener(OnMakeListButtonClicked);

            m_ViewList = transform.Find("View_CheckPopup").gameObject;
            m_InfinityGrid = transform.Find("View_CheckPopup/Scroll View").GetComponent<InfinityGrid>();
            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;
            m_ViewListClose = transform.Find("View_CheckPopup/Image_Close").GetComponent<Image>();
            Lib.Core.EventTrigger eventTrigger = Lib.Core.EventTrigger.Get(m_ViewListClose);
            eventTrigger.AddEventListener(EventTriggerType.PointerClick, OnViewListClose);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, OnRefeshMainBagData, true);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            EquipGrid entry = new EquipGrid();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddEventListener(OnGridSelected, ActiveRedPoint);
            cell.BindUserData(entry);
            m_EquipGrids.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            EquipGrid entry = cell.mUserData as EquipGrid;
            entry.SetData(m_MakeLists[index]);
        }

        private void OnGridSelected(ulong uuid)
        {
            m_MakeListWillRemove.AddOnce<ulong>(uuid);
        }

        private bool ActiveRedPoint(ulong uuid)
        {
            return !m_MakeListWillRemove.Contains(uuid);
        }

        private void OnMakeListButtonClicked()
        {
            ClearBuff();
            if (m_MakeLists.Count == 1)
            {
                ulong equidId = m_MakeLists[0];
                ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(equidId);
                if (itemData != null)
                {
                    PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                    petEquipTipsData.openUI = EUIID.UI_PetMagicCore;
                    petEquipTipsData.petEquip = itemData;
                    petEquipTipsData.isCompare = false;
                    petEquipTipsData.isShowOpBtn = false;
                    UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                    m_MakeLists.Clear();
                    m_MakeListWillRemove.Clear();
                    m_MakeListButton.gameObject.SetActive(false);
                }
            }
            else
            {
                m_ViewList.SetActive(true);
                b_ViewListOpen = true;
                ForceRefreshViewList();
            }
        }

        private void OnViewListClose(BaseEventData baseEventData)
        {
            ClearBuff();
            m_ViewList.SetActive(false);
            b_ViewListOpen = false;
        }

        private void OnNtfDecomposeItem(ulong uuid)
        {
            if (m_MakeLists.Remove(uuid))
            {
                m_MakeListWillRemove.Remove(uuid);
                if (m_MakeLists.Count == 0)
                {
                    m_MakeListButton.gameObject.SetActive(false);
                }
                ForceRefreshViewList();
            }
        }

        private void ClearBuff()
        {
            if (m_MakeLists.Count > m_MakeListWillRemove.Count)
            {
                for (int i = m_MakeListWillRemove.Count - 1; i >= 0; --i)
                {
                    m_MakeLists.Remove(m_MakeListWillRemove[i]);
                    m_MakeListWillRemove.RemoveAt(i);
                }
            }
            else
            {
                for (int i = m_MakeListWillRemove.Count - 1; i >= 1; --i)
                {
                    m_MakeLists.Remove(m_MakeListWillRemove[i]);
                    m_MakeListWillRemove.RemoveAt(i);
                }
            }
        }

        private void ForceRefreshViewList()
        {
            if (!b_ViewListOpen)
            {
                return;
            }
            m_InfinityGrid.CellCount = m_MakeLists.Count;
            m_InfinityGrid.ForceRefreshActiveCell();
        }

        private void OnMakeSuccess(ulong equipId)
        {
            m_MakeLists.Add(equipId);
            if (!m_MakeListButton.gameObject.activeSelf)
            {
                m_MakeListButton.gameObject.SetActive(true);
            }
        }

        private void InitPreview()
        {
            b_ViewListOpen = false;
            m_MakeListButton.gameObject.SetActive(false);
            m_ViewList.gameObject.SetActive(false);
            m_MakeLists.Clear();
            m_MakeListWillRemove.Clear();
        }


        public class EquipGrid
        {
            private GameObject m_Go;
            private GameObject m_RedPoint;
            private Button m_ClickButton;
            private Image m_Icon;
            private Image m_Quality;
            private Text m_ItemName;
            private ItemData m_ItemData;
            private Action<ulong> m_OnClicked;
            private Func<ulong, bool> m_ActiveRedPoint;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_RedPoint = m_Go.transform.Find("Image_Red").gameObject;
                m_Icon = m_Go.transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
                m_ItemName = m_Go.transform.Find("PropItem/Text_Name").GetComponent<Text>();
                m_Quality = m_Go.transform.Find("PropItem/Btn_Item/Image_BG").GetComponent<Image>();
                m_ClickButton = m_Go.transform.Find("PropItem/Btn_Item").GetComponent<Button>();

                m_ClickButton.onClick.AddListener(OnClicked);
            }
            public void AddEventListener(Action<ulong> action, Func<ulong, bool> activeRed)
            {
                m_OnClicked = action;
                m_ActiveRedPoint = activeRed;
            }

            public void OnClicked()
            {
                PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                petEquipTipsData.openUI = EUIID.UI_PetMagicCore;
                petEquipTipsData.petEquip = m_ItemData;
                petEquipTipsData.isCompare = false;
                petEquipTipsData.isShowOpBtn = false;
                UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);

                m_OnClicked?.Invoke(m_ItemData.Uuid);
                m_RedPoint.SetActive(m_ActiveRedPoint(m_ItemData.Uuid));
            }


            public void SetData(ulong equipId)
            {
                m_ItemData = Sys_Bag.Instance.GetItemDataByUuid(equipId);

                ImageHelper.SetIcon(m_Icon, m_ItemData.cSVItemData.icon_id);
                ImageHelper.GetQualityColor_Frame(m_Quality, (int)m_ItemData.Quality);
                TextHelper.SetText(m_ItemName, m_ItemData.cSVItemData.name_id);
                m_RedPoint.SetActive(m_ActiveRedPoint(equipId));
            }
        }
    }
}
