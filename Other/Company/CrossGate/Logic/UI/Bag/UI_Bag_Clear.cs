using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using System;
using Google.Protobuf.Collections;

namespace Logic
{
    public class UI_Bag_Clear : UIBase
    {
        private Button m_AllSale;
        private InfinityGrid m_InfinityGridSale;
        private Dictionary<GameObject, Grid> m_SaleGrids = new Dictionary<GameObject, Grid>();
        private List<ulong> uuIDs_Sale = new List<ulong>();
        private List<bool> m_SelectStateSale = new List<bool>();
        private bool b_Saled;

        private InfinityGrid m_InfinityGridDel;
        private Button m_AllDel;
        private Dictionary<GameObject, Grid> m_DelGrids = new Dictionary<GameObject, Grid>();
        private List<ulong> uuIDs_Del = new List<ulong>();
        private List<bool> m_SelectStateDel = new List<bool>();
        private bool b_Deled;

        private Button m_CloseButton;
        private Button m_AllSaleButton;
        private Button m_AllDelButton;
        private GameObject m_AllSaleObj_Recemond;
        private GameObject m_AllDelObj_Recemond;
        private Text m_AllSaleText_Recemond;
        private Text m_AllDelText_Recemond;

        private GameObject m_AllSaleObj_Get;
        private GameObject m_AllDelObj_Get;
        private Text m_AllSaleText_Get;
        private Text m_AllDelText_Get;

        protected override void OnInit()
        {
            ProcessData();
        }

        private void ProcessData()
        {
            uuIDs_Sale.Clear();
            uuIDs_Del.Clear();
            m_SelectStateSale.Clear();
            m_SelectStateDel.Clear();
            for (int i = 0; i < Sys_Bag.Instance.itemsCanSale.Count; i++)
            {
                ItemData itemData = Sys_Bag.Instance.itemsCanSale[i];
                uuIDs_Sale.Add(itemData.Uuid);

                if (itemData.cSVItemData.type_id == (uint)EItemType.Equipment)
                {
                    if (itemData.Quality == (uint)EQualityType.Orange || itemData.Quality == (uint)EQualityType.Purple)
                    {
                        m_SelectStateSale.Add(false);
                    }
                    else
                    {
                        m_SelectStateSale.Add(true);
                    }
                }
                else
                {
                    m_SelectStateSale.Add(true);
                }
            }

            for (int i = 0; i < Sys_Bag.Instance.itemsCanDel.Count; i++)
            {
                ItemData itemData = Sys_Bag.Instance.itemsCanDel[i];
                uuIDs_Del.Add(Sys_Bag.Instance.itemsCanDel[i].Uuid);
                if (itemData.cSVItemData.Recommend_Undo == 1)
                {
                    m_SelectStateDel.Add(true);
                }
                else if (itemData.cSVItemData.Recommend_Undo == 2)
                {
                    m_SelectStateDel.Add(false);
                }
                else
                {
                    m_SelectStateDel.Add(true);
                }
            }

            b_Saled = false;
            b_Deled = false;
        }

        protected override void OnLoaded()
        {
            m_InfinityGridSale = transform.Find("Animator/Scroll_View/Viewport/View01/Scroll_View").GetComponent<InfinityGrid>();
            m_InfinityGridDel = transform.Find("Animator/Scroll_View/Viewport/View02/Scroll_View").GetComponent<InfinityGrid>();
            m_AllSale = transform.Find("Animator/Scroll_View/Viewport/View01/Btn_01").GetComponent<Button>();
            m_AllDel = transform.Find("Animator/Scroll_View/Viewport/View01/Btn_01").GetComponent<Button>();

            m_InfinityGridSale.onCreateCell = OnCreateSaleCell;
            m_InfinityGridSale.onCellChange = OnCellSaleChange;

            m_InfinityGridDel.onCreateCell = OnCreateDelCell;
            m_InfinityGridDel.onCellChange = OnCellDelChange;

            m_CloseButton = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);

            m_AllSaleButton = transform.Find("Animator/Scroll_View/Viewport/View01/Btn_01").GetComponent<Button>();
            m_AllDelButton = transform.Find("Animator/Scroll_View/Viewport/View02/Btn_01").GetComponent<Button>();

            m_AllSaleObj_Recemond = transform.Find("Animator/Scroll_View/Viewport/View01/Image_Title02").gameObject;
            m_AllDelObj_Recemond = transform.Find("Animator/Scroll_View/Viewport/View02/Image_Title02").gameObject;
            m_AllSaleText_Recemond = transform.Find("Animator/Scroll_View/Viewport/View01/Image_Title02/Text").GetComponent<Text>();
            m_AllDelText_Recemond = transform.Find("Animator/Scroll_View/Viewport/View02/Image_Title02/Text").GetComponent<Text>();

            m_AllSaleObj_Get = transform.Find("Animator/Scroll_View/Viewport/View01/Image_Title01").gameObject;
            m_AllDelObj_Get = transform.Find("Animator/Scroll_View/Viewport/View02/Image_Title01").gameObject;
            m_AllSaleText_Get = transform.Find("Animator/Scroll_View/Viewport/View01/Image_Title01/Text").GetComponent<Text>();
            m_AllDelText_Get = transform.Find("Animator/Scroll_View/Viewport/View02/Image_Title01/Text").GetComponent<Text>();

            m_AllSaleButton.onClick.AddListener(OnAllSaleButtonClicked);
            m_AllDelButton.onClick.AddListener(OnAllDelButtonClicked);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<ulong>(Sys_Bag.EEvents.OnAllSale, OnAllSale, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnAllDel, OnAllDel, toRegister);
            Sys_LivingSkill.Instance.eventEmitter.Handle<ulong>(Sys_LivingSkill.EEvents.OnEquip, OnEquip, toRegister);
        }

        protected override void OnShow()
        {
            int count = Sys_Bag.Instance.itemsCanSale.Count;
            m_AllSaleButton.gameObject.SetActive(count > 0);
            m_InfinityGridSale.CellCount = count;
            m_InfinityGridSale.ForceRefreshActiveCell();

            int _count = Sys_Bag.Instance.itemsCanDel.Count;
            m_AllDelButton.gameObject.SetActive(_count > 0);
            m_InfinityGridDel.CellCount = _count;
            m_InfinityGridDel.ForceRefreshActiveCell();

            m_AllSaleObj_Get.SetActive(false);
            m_AllDelObj_Get.SetActive(false);

            m_AllSaleObj_Recemond.SetActive(true);
            m_AllDelObj_Recemond.SetActive(true);

            TextHelper.SetText(m_AllSaleText_Recemond, 2025003);
            TextHelper.SetText(m_AllDelText_Recemond, 2025005);
        }

        private void OnCreateSaleCell(InfinityGridCell cell)
        {
            Grid grid = new Grid();
            grid.BindGameObject(cell.mRootTransform.gameObject);
            grid.BindEvent(SelectChanged);
            cell.BindUserData(grid);
            m_SaleGrids.Add(cell.mRootTransform.gameObject, grid);
        }

        private void OnCellSaleChange(InfinityGridCell cell, int index)
        {
            Grid grid = cell.mUserData as Grid;
            if (m_SelectStateSale.Count == 0)
            {
                grid.SetData(Sys_Bag.Instance.itemsCanSale[index], index, false, 1);
            }
            else
            {
                grid.SetData(Sys_Bag.Instance.itemsCanSale[index], index, m_SelectStateSale[index], 1);
            }
            if (b_Saled)
            {
                grid.HideToggle();
            }
        }

        private void OnCreateDelCell(InfinityGridCell cell)
        {
            Grid grid = new Grid();
            grid.BindGameObject(cell.mRootTransform.gameObject);
            grid.BindEvent(SelectChanged);
            cell.BindUserData(grid);
            m_DelGrids.Add(cell.mRootTransform.gameObject, grid);
        }

        private void OnCellDelChange(InfinityGridCell cell, int index)
        {
            Grid grid = cell.mUserData as Grid;
            if (m_SelectStateDel.Count == 0)
            {
                grid.SetData(Sys_Bag.Instance.itemsCanDel[index], index, false, 2);
            }
            else
            {
                grid.SetData(Sys_Bag.Instance.itemsCanDel[index], index, m_SelectStateDel[index], 2);
            }
            if (b_Deled)
            {
                grid.HideToggle();
            }
        }

        private void OnEquip(ulong uuid)
        {
            for (int i = Sys_Bag.Instance.itemsCanSale.Count - 1; i >= 0; --i)
            {
                if (Sys_Bag.Instance.itemsCanSale[i].Uuid == uuid)
                {
                    Sys_Bag.Instance.itemsCanSale.RemoveAt(i);
                }
            }
            for (int i = Sys_Bag.Instance.itemsCanDel.Count - 1; i >= 0; --i)
            {
                if (Sys_Bag.Instance.itemsCanDel[i].Uuid == uuid)
                {
                    Sys_Bag.Instance.itemsCanDel.RemoveAt(i);
                }
            }
            ProcessData();

            int count = Sys_Bag.Instance.itemsCanSale.Count;
            m_AllSaleButton.gameObject.SetActive(count > 0);
            m_InfinityGridSale.CellCount = count;
            m_InfinityGridSale.ForceRefreshActiveCell();

            int _count = Sys_Bag.Instance.itemsCanDel.Count;
            m_AllDelButton.gameObject.SetActive(_count > 0);
            m_InfinityGridDel.CellCount = _count;
            m_InfinityGridDel.ForceRefreshActiveCell();
        }

        private void OnAllSale(ulong sliverCount)
        {
            b_Saled = true;
            m_SelectStateSale.Clear();
            Sys_Bag.Instance.itemsCanSale.Clear();

            ItemData itemData = new ItemData(1, 0, 3, (uint)sliverCount, 0, false, false, null, null, 0);
            Sys_Bag.Instance.itemsCanSale.Add(itemData);
            m_InfinityGridSale.CellCount = 1;
            m_InfinityGridSale.ForceRefreshActiveCell();
        }

        private void OnAllDel()
        {
            b_Deled = true;
            m_SelectStateDel.Clear();
            Sys_Bag.Instance.itemsCanDel.Clear();

            int count = Sys_Bag.Instance.itemIds.Count;
            for (int i = 0; i < count; i++)
            {
                ItemData itemData = new ItemData(1, 0, Sys_Bag.Instance.itemIds[i], Sys_Bag.Instance.itemNums[i], 0, false, false, null, null, 0);
                Sys_Bag.Instance.itemsCanDel.Add(itemData);
            }
            m_InfinityGridDel.CellCount = count;
            m_InfinityGridDel.ForceRefreshActiveCell();
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Bag_Clear);
        }


        private void OnAllSaleButtonClicked()
        {
            for (int i = uuIDs_Sale.Count - 1; i >= 0; i--)
            {
                if (!m_SelectStateSale[i])
                {
                    uuIDs_Sale.RemoveAt(i);
                }
            }

            ItemData lockItem = null;
            for (int i = 0; i < uuIDs_Sale.Count; ++i)
            {
                ItemData item = Sys_Bag.Instance.GetItemDataByUuid(uuIDs_Sale[i]);
                if (item != null)
                {
                    if (item.IsLocked)
                    {
                        lockItem = item;
                        break;
                    }
                }
            }

            if (lockItem != null)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(lockItem.cSVItemData.name_id), LanguageHelper.GetTextContent(lockItem.cSVItemData.name_id));
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Bag.Instance.OnItemLockReq(lockItem.Uuid, false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Bag.Instance.BatchSellReq(uuIDs_Sale);
                m_AllSaleButton.gameObject.SetActive(false);
    
                m_AllSaleObj_Recemond.SetActive(false);
                m_AllSaleObj_Get.SetActive(true);
                TextHelper.SetText(m_AllSaleText_Get, 2025004);
            }
        }


        private void OnAllDelButtonClicked()
        {
            for (int i = uuIDs_Del.Count - 1; i >= 0; i--)
            {
                if (!m_SelectStateDel[i])
                {
                    uuIDs_Del.RemoveAt(i);
                }
            }
            
            ItemData lockItem = null;
            for (int i = 0; i < uuIDs_Del.Count; ++i)
            {
                ItemData item = Sys_Bag.Instance.GetItemDataByUuid(uuIDs_Del[i]);
                if (item != null)
                {
                    if (item.IsLocked)
                    {
                        lockItem = item;
                        break;
                    }
                }
            }

            if (lockItem != null)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(lockItem.cSVItemData.name_id), LanguageHelper.GetTextContent(lockItem.cSVItemData.name_id));
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Bag.Instance.OnItemLockReq(lockItem.Uuid, false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Bag.Instance.BatchDeComposeReq(uuIDs_Del);
                m_AllDelButton.gameObject.SetActive(false);
    
                m_AllDelObj_Recemond.SetActive(false);
                m_AllDelObj_Get.SetActive(true);
                TextHelper.SetText(m_AllDelText_Get, 2025006);
            }
        }

        private void SelectChanged(Grid grid)
        {
            if (grid.type == 1)//出售
            {
                if (grid.toggle.isOn)
                {
                    //uuIDs_Sale.AddOnce(grid.itemData.Uuid);
                    if (!b_Saled)
                        m_SelectStateSale[grid.dataIndex] = true;
                }
                else
                {
                    //uuIDs_Sale.TryRemove(grid.itemData.Uuid);
                    if (!b_Saled)
                        m_SelectStateSale[grid.dataIndex] = false;
                }
                bool enable = false;
                for (int i = 0; i < m_SelectStateSale.Count; i++)
                {
                    if (m_SelectStateSale[i])
                    {
                        enable = true;
                        break;
                    }
                }
                ButtonHelper.Enable(m_AllSaleButton, enable);
            }
            else if (grid.type == 2)//分解
            {
                if (grid.toggle.isOn)
                {
                    //uuIDs_Del.AddOnce(grid.itemData.Uuid);
                    if (!b_Deled)
                        m_SelectStateDel[grid.dataIndex] = true;
                }
                else
                {
                    //uuIDs_Del.TryRemove(grid.itemData.Uuid);
                    if (!b_Deled)
                        m_SelectStateDel[grid.dataIndex] = false;
                }
                bool enable = false;
                for (int i = 0; i < m_SelectStateDel.Count; i++)
                {
                    if (m_SelectStateDel[i])
                    {
                        enable = true;
                        break;
                    }
                }
                ButtonHelper.Enable(m_AllDelButton, enable);
            }
        }

        public class Grid
        {
            private GameObject m_Go;
            public ItemData itemData;
            public int type;        //1 一键出售 2 一键分解
            public int dataIndex;
            public Toggle toggle;
            private Action<Grid> m_SelectChanged;
            private Text m_Name;
            private bool selectState;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_Name = m_Go.transform.Find("Text_Name").GetComponent<Text>();
                toggle = m_Go.transform.Find("Toggle").GetComponent<Toggle>();
                toggle.isOn = true;
                toggle.onValueChanged.AddListener(OnValueChanged);
            }

            public void BindEvent(Action<Grid> selectChanged)
            {
                m_SelectChanged = selectChanged;
            }

            public void SetData(ItemData item, int dataIndex, bool selectState, int type)
            {
                this.itemData = item;
                this.dataIndex = dataIndex;
                this.type = type;
                this.selectState = selectState;
                Refresh();
            }

            private void Refresh()
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(m_Go);
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                    (_id: itemData.Id,
                    _count: itemData.Count,
                    _bUseQuailty: true,
                    _bBind: false,
                    _bNew: false,
                    _bUnLock: false,
                    _bSelected: false,
                    _bShowCount: true,
                    _bShowBagCount: false,
                    _bUseClick: true,
                    _onClick: OnClick,
                    _bshowBtnNo: false,
                    _bUseTips: false);
                showItem.SetQuality(itemData.Quality);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));

                toggle.isOn = selectState;
                TextHelper.SetText(m_Name, itemData.cSVItemData.name_id);
            }

            public void HideToggle()
            {
                toggle.gameObject.SetActive(false);
            }

            private void OnClick(PropItem propItem)
            {
                uint typeId = itemData.cSVItemData.type_id;
                //装备道具,单独处理
                if (typeId == (uint)EItemType.Equipment)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = itemData;
                    tipData.isShowOpBtn = false;
                    tipData.isCompare = false;
                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
                else if (typeId == (uint)EItemType.Crystal)
                {
                    CrystalTipsData crystalTipsData = new CrystalTipsData();
                    crystalTipsData.itemData = itemData;
                    crystalTipsData.bShowOp = false;
                    crystalTipsData.bShowCompare = false;
                    crystalTipsData.bShowDrop = false;
                    crystalTipsData.bShowSale = false;
                    UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
                }
                else if (typeId == (uint)EItemType.Ornament)
                {
                    OrnamentTipsData tipData = new OrnamentTipsData();
                    tipData.equip = itemData;
                    tipData.isShowOpBtn = false;
                    tipData.isShowSourceBtn = false;
                    tipData.sourceUiId = EUIID.UI_Bag_Clear;
                    UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                }
                else
                {
                    PropMessageParam propParam = new PropMessageParam();
                    propParam.itemData = itemData;
                    propParam.needShowInfo = false;
                    propParam.needShowMarket = true;
                    propParam.showOpButton = false;
                    propParam.sourceUiId = EUIID.UI_Bag_Clear;
                    UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                }
            }

            private void OnValueChanged(bool flag)
            {
                m_SelectChanged?.Invoke(this);
            }
        }
    }
}

