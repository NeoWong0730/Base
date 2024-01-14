using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;

namespace Logic
{
    public class UI_Bag : UIBase
    {
        private UI_Bag_Equipment bagEquipment;
        private UI_MainBag mUI_MainBagComponent;
        private UI_CurrencyTitle UI_CurrencyTitle;
        private UI_CrystalBag m_UI_CrystalBag;
        private UI_OrnamentBag m_UI_OrnamentBag;
        public UI_Transform m_UI_Transform;
        public UI_Bag_RightTabs m_Tabs;
        private Button closeBtn;

        private Dictionary<EBagViewType, UIComponent> dictOpPanel;
        private int defaultType;
        private int curEBagViewType;

        public class UI_MainBag : UIComponent
        {
            private Dictionary<GameObject, CeilGrid> ceilGrids = new Dictionary<GameObject, CeilGrid>();
            private List<CeilGrid> uuidGrids = new List<CeilGrid>();
            private int visualableGridCount;                                //可以看到的总格子数量
            public int VisualableGridCount
            {
                get
                {
                    return visualableGridCount;
                }
                set
                {
                    if (visualableGridCount != value)
                    {
                        visualableGridCount = value;
                    }
                }
            }
            List<ItemData> itemDatas = new List<ItemData>();                //数据部分
            Dictionary<uint, ItemData> positions = new Dictionary<uint, ItemData>();  //key:position (带真实数据的格子)

            private int curBoxId = 0;                                      //1:物品   2:任务  3:材料  4:图鉴
            public int CurBoxId
            {
                get
                {
                    return curBoxId;
                }
                set
                {
                    if (curBoxId != value)
                    {
                        Sys_Bag.Instance.ClearNewIconReq(Sys_Bag.Instance.curBoxId);
                        curBoxId = value;
                        curSelectDataIndex = -1;
                        for (int i = 0; i < uuidGrids.Count; i++)
                        {
                            uuidGrids[i].SetBoxId(curBoxId);
                        }
                        Sys_Bag.Instance.curBoxId = curBoxId;
                        OnRefreshCurBoxData(curBoxId);
                        //infinity.SetAmount(visualableGridCount);
                        infinity.CellCount = visualableGridCount;
                        infinity.ForceRefreshActiveCell();
                        infinity.GetCells();
                        infinity.MoveToIndex(0);
                    }
                }
            }
            private bool b_ReachMaxCapcity = false;                      //当前页签是否达到格子最大等级(已无法再解锁格子)
            private int m_Column = 5;                                   //一共多少列

            private InfinityGrid infinity;
            private Transform parent;
            private List<Toggle> toggles = new List<Toggle>();
            private List<GameObject> m_LableFull = new List<GameObject>();

            private Button clearUpBtn;
            private Button m_ClearBagButton;
            private Button gotoSafeBox;
            private Text gotoSafeBoxText;
            private int curSelectDataIndex = -1;
            public Timer timer;
            public Timer clearBagButtonTimer;

            protected override void Loaded()
            {
                toggles.Add(transform.Find("List_Menu/TabList/ListItem").GetComponent<Toggle>());
                toggles.Add(transform.Find("List_Menu/TabList/ListItem (1)").GetComponent<Toggle>());
                toggles.Add(transform.Find("List_Menu/TabList/ListItem (2)").GetComponent<Toggle>());
                toggles.Add(transform.Find("List_Menu/TabList/ListItem (3)").GetComponent<Toggle>());
                m_LableFull.Add(transform.Find("List_Menu/TabList/ListItem/Image_Full").gameObject);
                m_LableFull.Add(transform.Find("List_Menu/TabList/ListItem (1)/Image_Full").gameObject);
                m_LableFull.Add(transform.Find("List_Menu/TabList/ListItem (2)/Image_Full").gameObject);
                m_LableFull.Add(transform.Find("List_Menu/TabList/ListItem (3)/Image_Full").gameObject);
                foreach (var item in toggles)
                {
                    item.onValueChanged.AddListener(val =>
                    {
                        OnToggleLabelChanged(toggles.IndexOf(item), val);
                    });
                }

                clearUpBtn = transform.Find("Button_Clear_Up").GetComponent<Button>();
                clearUpBtn.onClick.AddListener(() =>
                {
                    Sys_Bag.Instance.TidyBagPackReq((uint)curBoxId);
                    //infinity.SetAmount(visualableGridCount);
                    infinity.ForceRefreshActiveCell();
                    infinity.MoveToIndex(0);
                    ButtonHelper.Enable(clearUpBtn, false);
                    timer?.Cancel();
                    timer = Timer.Register(2, () =>
                    {
                        ButtonHelper.Enable(clearUpBtn, true);
                    });
                    OnClearAllSelect();
                }
                );

                m_ClearBagButton = transform.Find("Button_Clear").GetComponent<Button>();
                m_ClearBagButton.onClick.AddListener(OnClearBagButtonClicked);

                gotoSafeBox = transform.Find("Button_Go").GetComponent<Button>();
                gotoSafeBoxText = transform.Find("Button_Go/Text_01").GetComponent<Text>();
                gotoSafeBox.onClick.AddListener(OnButtonGoClicked);

                parent = transform.Find("Scroll_View_Bag/TabList").transform;
                infinity = parent.gameObject.GetNeedComponent<InfinityGrid>();
                infinity.onCreateCell = OnCreateGrid;
                infinity.onCellChange = OnCellChange;

                curBoxId = 1;
                Sys_Bag.Instance.curBoxId = curBoxId;  
            }

            private void OnClearBagButtonClicked()
            {
                Sys_Bag.Instance.ClearBagReq();
                ButtonHelper.Enable(m_ClearBagButton, false);
                clearBagButtonTimer?.Cancel();
                clearBagButtonTimer = Timer.Register(2, () =>
                {
                    ButtonHelper.Enable(m_ClearBagButton, true);
                });
            }

            public override void Show()
            {
                base.Show();
                //CurBoxId = 1;
                OnRefreshCurBoxData(curBoxId);
                //infinity.SetAmount(visualableGridCount);
                infinity.CellCount = visualableGridCount;
                infinity.ForceRefreshActiveCell();

                toggles[curBoxId - 1].isOn = true;
                ButtonHelper.Enable(clearUpBtn, true);
                ButtonHelper.Enable(m_ClearBagButton, true);
                OnRefreshButtonGo();
                OnUpdateFullState();
            }

            public override void Hide()
            {
                timer?.Cancel();
                clearBagButtonTimer?.Cancel();
                gameObject.SetActive(false);
            }
            private void OnCreateGrid(InfinityGridCell cell)
            {
                CeilGrid entry = new CeilGrid();
                entry.BindGameObject(cell.mRootTransform.gameObject);
                entry.AddClickListener(OnGridSelected);
                cell.BindUserData(entry);
                ceilGrids.Add(cell.mRootTransform.gameObject, entry);
                uuidGrids.Add(entry);
                for (int i = 0; i < uuidGrids.Count; i++)
                {
                    uuidGrids[i].SetBoxId(curBoxId);
                }
            }

            private void OnCellChange(InfinityGridCell cell, int index)
            {
                if (index >= visualableGridCount)
                    return;
                CeilGrid taskDataGrid = ceilGrids[cell.mRootTransform.gameObject];
                if (curSelectDataIndex != index)
                {
                    taskDataGrid.Release();
                }
                if (positions.ContainsKey((uint)index))
                {
                    taskDataGrid.SetData(positions[(uint)index], index, CeilGrid.EGridState.Normal);
                }
                else
                {
                    if (index < visualableGridCount - m_Column)
                    {
                        taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty);
                    }
                }
                if (index >= visualableGridCount - m_Column && index < visualableGridCount)
                {
                    if (b_ReachMaxCapcity)
                    {
                        if (positions.ContainsKey((uint)index))
                        {
                            taskDataGrid.SetData(positions[(uint)index], index, CeilGrid.EGridState.Normal);
                        }
                        else
                        {
                            taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty);
                        }
                    }
                    else
                    {
                        taskDataGrid.SetData(null, index, CeilGrid.EGridState.Unlock);
                    }
                }
            }

            private void OnUpdateFullState()
            {
                for (int i = 0; i < m_LableFull.Count; i++)
                {
                    int key = i + 1;
                    bool active = Sys_Bag.Instance.fullState[key];
                    m_LableFull[i].SetActive(active);
                }
            }

            private void OnRefreshButtonGo()
            {
                if (Sys_OperationalActivity.Instance.CheckSpecialCardWithBankIsUnlock())
                {
                    TextHelper.SetText(gotoSafeBoxText, 11899);//随身
                }
                else
                {
                    TextHelper.SetText(gotoSafeBoxText, 2007400);//前往
                }
            }

            private void OnButtonGoClicked()
            {
                if (Sys_OperationalActivity.Instance.CheckSpecialCardWithBankIsUnlock())
                {
                    UIManager.OpenUI(EUIID.UI_SafeBox);
                }
                else
                {
                    uint npcId = uint.Parse(CSVParam.Instance.GetConfData(243).str_value);
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);
                    UIManager.CloseUI(EUIID.UI_Bag, false, false);
                    Sys_Bag.Instance.useItemReq = false;
                }
            }

            public void OnRefreshCurBoxData(int curBagTab)
            {
                if (curBagTab != curBoxId)
                {
                    return;
                }
                itemDatas = Sys_Bag.Instance.BagItems[curBagTab];
                positions.Clear();
                for (int i = 0; i < itemDatas.Count; i++)
                {
                    positions.Add(itemDatas[i].Position, itemDatas[i]);
                }
                UpdateBoxLevel();
            }

            public void OnUnLockBox(int boxid)
            {
                if (boxid != curBoxId)
                    return;
                UpdateBoxLevel();
                //infinity._SetAmount(visualableGridCount);
                infinity.CellCount = visualableGridCount;
                infinity.ForceRefreshActiveCell();
                foreach (var item in ceilGrids)
                {
                    CeilGrid ceilGrid = item.Value;
                    int index = ceilGrid.gridIndex;
                    if (positions.ContainsKey((uint)index))
                    {
                        ceilGrid.SetData(positions[(uint)index], index, CeilGrid.EGridState.Normal);
                    }
                    else
                    {
                        if (index < visualableGridCount - m_Column)
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Empty);
                        }
                    }
                    if (index >= visualableGridCount - m_Column && index < visualableGridCount)
                    {
                        if (b_ReachMaxCapcity)
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Empty);
                        }
                        else
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Unlock);
                        }
                    }
                }

            }

            private List<ItemData> filters = new List<ItemData>();

            public void OnRefreshChangeData(int changeType, int curBoxid)
            {
                UIManager.CloseUI(EUIID.UI_Decompose);
                UIManager.CloseUI(EUIID.UI_Prop_Message);
                UIManager.CloseUI(EUIID.UI_PromptBox);
                infinity.ForceRefreshActiveCell();
                /*
                //页签拷贝数据
                if (changeType == 0)
                {
                    //保证页签拷贝的数据是当前页签的数据
                    if (curBoxid != curBoxId)
                        return;
                    //清空了当前页签数据
                    if (Sys_Bag.Instance.changeItems.Count == 0)
                    {
                        for (int j = 0; j < uuidGrids.Count; j++)
                        {
                            uuidGrids[j].SetData(null, (int)uuidGrids[j].gridIndex, CeilGrid.EGridState.Empty);
                        }
                    }
                    else
                    {
                        Dictionary<uint, ItemData> filterPositions = new Dictionary<uint, ItemData>();
                        for (int i = 0; i < Sys_Bag.Instance.changeItems.Count; i++)
                        {
                            filterPositions.Add(Sys_Bag.Instance.changeItems[i].Position, Sys_Bag.Instance.changeItems[i]);
                        }
                        for (int i = 0; i < uuidGrids.Count; i++)
                        {
                            if (filterPositions.ContainsKey((uint)uuidGrids[i].gridIndex))
                            {
                                uuidGrids[i].SetData(filterPositions[(uint)uuidGrids[i].gridIndex], uuidGrids[i].gridIndex, CeilGrid.EGridState.Normal);
                            }
                            else
                            {
                                uuidGrids[i].SetData(null, uuidGrids[i].gridIndex, CeilGrid.EGridState.Empty);
                            }
                        }
                    }
                }
                else if (changeType == 1)
                {
                    filters.Clear();
                    //for (int i = 0; i < Sys_Bag.Instance.changeItems.Count; i++)
                    //{
                    //    if (Sys_Bag.Instance.changeItems[i].BoxId == curBoxId)
                    //    {
                    //        filters.Add(Sys_Bag.Instance.changeItems[i]);
                    //    }
                    //}
                    infinity.ForceRefreshActiveCell();
                    //for (int i = 0; i < filters.Count; i++)
                    //{
                    //    CeilGrid ceilGrid = positionGridMap[filters[i].Position];
                    //    ceilGrid.SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty);
                    //}


                    //var fristElement = parent.GetChild(0);

                    //if (fristElement != null && ceilGrids.TryGetValue(fristElement.gameObject, out CeilGrid ceil))
                    //{
                    //    for (int i = 0; i < filters.Count; i++)
                    //    {
                    //        int minPostion = ceil.gridIndex;
                    //        int maxPostion = ceil.gridIndex + uuidGrids.Count - 1;

                    //        if (filters[i].Position >= minPostion && filters[i].Position <= maxPostion)
                    //        {
                    //            int index = (int)filters[i].Position - minPostion;

                    //            var focusElement = parent.GetChild(index);

                    //            if (focusElement != null && ceilGrids.TryGetValue(focusElement.gameObject, out CeilGrid focusceil))
                    //            {
                    //                focusceil.SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty);
                    //            }


                    //        }
                    //        //for (int j = 0; j < uuidGrids.Count; j++)
                    //        //{
                    //        //    if (uuidGrids[j].gridIndex == filters[i].Position)
                    //        //    {
                    //        //        uuidGrids[j].SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty);
                    //        //        break;
                    //        //    }
                    //        //}
                    //    }
                    //}

                }
                */
            }

            private void UpdateBoxLevel()
            {
                List<List<uint>> str = CSVBoxType.Instance.GetConfData((uint)(curBoxId)).uarray2_value;
                int boxLevel = (int)Sys_Bag.Instance.Grids[(uint)curBoxId];
                if (boxLevel == str.Count - 1)
                {
                    VisualableGridCount = (int)str[boxLevel][0];
                    b_ReachMaxCapcity = true;
                }
                else
                {
                    VisualableGridCount = (int)str[boxLevel][0] + m_Column;
                    b_ReachMaxCapcity = false;
                }
            }
            private void OnToggleLabelChanged(int index, bool active)
            {
                Sys_Bag.Instance.useItemReq = false;
                if (active)
                {
                    CurBoxId = index + 1;
                }
            }

            private void OnGridSelected(CeilGrid bagCeilGrid)
            {
                curSelectDataIndex = bagCeilGrid.gridIndex;

                foreach (var item in uuidGrids)
                {
                    if (item.gridIndex == curSelectDataIndex)
                    {
                        item.Select();

                        if (bagCeilGrid.eGridState == CeilGrid.EGridState.Normal)
                        {
                            Sys_Bag.Instance.ProcessCeilGridClick(bagCeilGrid);
                        }
                        else if (bagCeilGrid.eGridState == CeilGrid.EGridState.Unlock)
                        {
                            //解锁格子
                            PromptBoxParameter.Instance.Clear();

                            string currency = LanguageHelper.GetLanguageColorWordsFormat(1000907);

                            string content = CSVLanguage.Instance.GetConfData(1000909).words;

                            string count = LanguageHelper.GetLanguageColorWordsFormat(Sys_Bag.Instance.GetCurMainBagBoxLevelUpCost().ToString(), 1000907);

                            PromptBoxParameter.Instance.content = string.Format(content, count, currency);

                            PromptBoxParameter.Instance.SetConfirm(true, () =>
                            {
                                Sys_Bag.Instance.UnLockGridReq((uint)bagCeilGrid.boxId, Sys_Bag.Instance.Grids[(uint)bagCeilGrid.boxId] + 1);
                            });

                            PromptBoxParameter.Instance.SetCancel(true, null);

                            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        }
                    }
                    else
                    {
                        item.Release();
                    }
                }
            }

            public void OnClearAllSelect()
            {
                for (int i = 0; i < uuidGrids.Count; i++)
                {
                    if (uuidGrids[i].Selected)
                    {
                        uuidGrids[i].Release();
                    }
                }
            }

            public void OnUpdateBagFullState(int lable, bool full)
            {
                m_LableFull[lable - 1].gameObject.SetActive(full);
            }
        }

        public class UI_CrystalBag : UIComponent
        {
            private Image m_CrysyalIcon;
            private Image m_CrysyalQuality;
            private ItemData m_Crystal;
            private Button m_ClickButton;
            private GameObject m_NoneCrystal;
            private GameObject m_HasCrystal;
            private Button m_CrystalPerformButton;

            protected override void Loaded()
            {
                m_NoneCrystal = transform.Find("Crystal/Image_Empty").gameObject;
                m_HasCrystal = transform.Find("Crystal/Image_Attr").gameObject;
                m_CrystalPerformButton = transform.Find("Crystal/Image").GetComponent<Button>();
                m_CrysyalIcon = transform.Find("View_Equip/PropItemCrystal/Btn_Item/Image_Icon").GetComponent<Image>();
                m_CrysyalQuality = transform.Find("View_Equip/PropItemCrystal/Btn_Item/Image_Quality").GetComponent<Image>();
                m_ClickButton = transform.Find("View_Equip/PropItemCrystal/Btn_Item").GetComponent<Button>();
                m_ClickButton.onClick.AddListener(OnCrysyalClicked);
                m_CrystalPerformButton.onClick.AddListener(OnCrystalPerformButtonClicked);
            }

            public override void Show()
            {
                gameObject.SetActive(true);
                UpdateInfo();
                UpdateAttr();
            }

            public override void Hide()
            {
                gameObject.SetActive(false);
            }

            public void UpdateInfo()
            {
                m_Crystal = Sys_ElementalCrystal.Instance.GetEquipedCrystal();
                if (null == m_Crystal)
                {
                    m_CrysyalIcon.gameObject.SetActive(false);
                    m_CrysyalQuality.gameObject.SetActive(false);
                }
                else
                {
                    m_CrysyalIcon.gameObject.SetActive(true);
                    m_CrysyalQuality.gameObject.SetActive(true);
                    ImageHelper.SetIcon(m_CrysyalIcon, m_Crystal.cSVItemData.icon_id);
                    ImageHelper.GetQualityColor_Frame(m_CrysyalQuality, (int)m_Crystal.cSVItemData.quality);
                }
            }

            public void UpdateAttr()
            {
                int needCount = 0;
                List<uint> items = new List<uint>();
                foreach (var item in Sys_Attr.Instance.pkAttrs)
                {
                    CSVAttr.Data data = CSVAttr.Instance.GetConfData(item.Key);
                    if (data.attr_type == 3 && item.Value != 0)
                    {
                        items.Add((uint)item.Key);
                        needCount++;
                    }
                }
                if (needCount == 0)
                {
                    m_NoneCrystal.SetActive(true);
                    m_HasCrystal.SetActive(false);
                }
                else
                {
                    m_NoneCrystal.SetActive(false);
                    m_HasCrystal.SetActive(true);
                    FrameworkTool.CreateChildList(m_HasCrystal.transform, needCount);
                    for (int i = 0; i < needCount; i++)
                    {
                        GameObject gameObject = m_HasCrystal.transform.GetChild(i).gameObject;
                        Image icon = gameObject.transform.Find("Image_Attr").GetComponent<Image>();
                        Text num = gameObject.transform.Find("Image_Attr/Text").GetComponent<Text>();
                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(items[i]);
                        ImageHelper.SetIcon(icon, data.attr_icon);
                        TextHelper.SetText(num, Sys_Attr.Instance.pkAttrs[items[i]].ToString());
                    }
                }
            }

            private void OnCrysyalClicked()
            {
                if (null == m_Crystal)
                {
                    return;
                }
                CrystalTipsData crystalTipsData = new CrystalTipsData();
                crystalTipsData.itemData = m_Crystal;
                crystalTipsData.bShowOp = true;
                crystalTipsData.bShowCompare = false;
                crystalTipsData.bShowDrop = false;
                crystalTipsData.bShowSale = false;
                UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
            }

            private void OnCrystalPerformButtonClicked()
            {
                UIManager.OpenUI(EUIID.UI_ElementalCrystal);
            }
        }

        private void OnChageEquiped()
        {
            bagEquipment.UpdateInfo();
        }

        private void OnChangeCrystal()
        {
            m_UI_CrystalBag.UpdateInfo();
        }

        private void OnUpdateAttr()
        {
            m_UI_CrystalBag.UpdateAttr();
        }

        private void OnChangeOrnament()
        {
            m_UI_OrnamentBag.UpdateInfo();
        }

        private void OnSelectBagViewType(EBagViewType _type)
        {
            foreach (var data in dictOpPanel)
            {
                if (data.Key == _type)
                {
                    data.Value.Show();
                    if (data.Key == EBagViewType.ViewBag)
                    {
                        bagEquipment.Show();
                        m_UI_CrystalBag.Show();
                        m_UI_OrnamentBag.Show();
                    }
                }
                else
                {
                    data.Value.Hide();
                    if (data.Key == EBagViewType.ViewBag)
                    {
                        bagEquipment.Hide();
                        m_UI_CrystalBag.Hide();
                        m_UI_OrnamentBag.Hide();
                    }
                }
            }
            curEBagViewType = (int)_type;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, mUI_MainBagComponent.OnRefreshCurBoxData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnUnLuckBox, mUI_MainBagComponent.OnUnLockBox, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, mUI_MainBagComponent.OnRefreshChangeData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnClearMainBagSelect, mUI_MainBagComponent.OnClearAllSelect, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, bool>(Sys_Bag.EEvents.OnUpdateBagFullState, mUI_MainBagComponent.OnUpdateBagFullState, toRegister);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnChageEquiped, OnChageEquiped, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnChangeCrystal, OnChangeCrystal, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnChangeOrnament, OnChangeOrnament, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<EBagViewType>(Sys_Bag.EEvents.OnSelectBagViewType, OnSelectBagViewType, toRegister);
            m_UI_Transform.ProcessEvent(toRegister);
        }

        protected override void OnShow()
        {
            UI_CurrencyTitle.InitUi();
            if (defaultType==0 && curEBagViewType != 0)
            {
                defaultType = curEBagViewType;
            }
            m_Tabs.OnDefaultSelect(defaultType & 0xffff);
            if (defaultType > 2)
                SetMainBagTabIndex((defaultType >> 16) & 0xffff);
        }

        protected override void OnClose()
        {

        }

        protected override void OnHide()
        {
            foreach (var data in dictOpPanel)
            {
                data.Value.Hide();
                if (data.Key == EBagViewType.ViewBag)
                {
                    bagEquipment.Hide();
                    m_UI_CrystalBag.Hide();
                    m_UI_OrnamentBag.Hide();
                }
            }
            defaultType = 0;
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                defaultType = 1;
            }
            else
            {
                defaultType = Convert.ToInt32(arg);         
            }
        }

        protected override void OnLoaded()
        {
            AssetDependencies dependence = transform.GetComponent<AssetDependencies>();

            bagEquipment = AddComponent<UI_Bag_Equipment>(transform.Find("Animator/View_Middle"));
            bagEquipment.SceneObj = dependence.mCustomDependencies[0];

            m_UI_CrystalBag = AddComponent<UI_CrystalBag>(transform.Find("Animator/View_Middle"));
            m_UI_OrnamentBag = AddComponent<UI_OrnamentBag>(transform.Find("Animator/View_Middle"));

            mUI_MainBagComponent = AddComponent<UI_MainBag>(transform.Find("Animator/View_Right_List"));
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(
                () =>
                {
                    Sys_Bag.Instance.ClearNewIconReq(Sys_Bag.Instance.curBoxId);
                    UIManager.CloseUI(EUIID.UI_Bag, false, false);
                    Sys_Bag.Instance.useItemReq = false;
                }
            );
            m_UI_Transform= AddComponent<UI_Transform>(transform.Find("Animator/View_Transfiguration"));
            m_UI_Transform.assetDependencies = transform.GetComponent<AssetDependencies>();

            m_Tabs = new UI_Bag_RightTabs();
            m_Tabs.Init(this.transform.Find("Animator/View_Left_Tabs"));

            dictOpPanel = new Dictionary<EBagViewType, UIComponent>();

            dictOpPanel.Add(EBagViewType.ViewBag, mUI_MainBagComponent);
            dictOpPanel.Add(EBagViewType.ViewTransform, m_UI_Transform);    
        }

        public void SetMainBagTabIndex(int index)
        {
            UI_MainBag mainBag = dictOpPanel[EBagViewType.ViewBag] as UI_MainBag;
            if (mainBag != null)
            {
                mainBag.CurBoxId = index;
            }
            OnSelectBagViewType(EBagViewType.ViewBag);
        }

        private void OnBagValueChanged(bool isOn)
        {
            mUI_MainBagComponent.gameObject.SetActive(isOn);
            bagEquipment.gameObject.SetActive(isOn);
            m_UI_Transform.gameObject.SetActive(!isOn);
            if (isOn)
            {
                mUI_MainBagComponent.Show();
                bagEquipment.UpdateInfo();
                m_UI_CrystalBag.UpdateInfo();
                m_UI_CrystalBag.UpdateAttr();
                m_UI_OrnamentBag.UpdateInfo();
    
            }
            else
            {
                mUI_MainBagComponent.timer?.Cancel();
                mUI_MainBagComponent.clearBagButtonTimer?.Cancel();
                bagEquipment.Hide();
            }
        }

        private void OnTransValueChanged(bool isOn)
        {
            mUI_MainBagComponent.gameObject.SetActive(!isOn);
            bagEquipment.gameObject.SetActive(!isOn);
            m_UI_Transform.gameObject.SetActive(isOn);
            if (isOn)
            {
                m_UI_Transform.Show();
            }
            else
            {
                m_UI_Transform.Hide();
            }
        }

        protected override void OnDestroy()
        {
            bagEquipment.OnDestroy();
            UI_CurrencyTitle.Dispose();
            if (dictOpPanel != null)
            {
                foreach (var item in dictOpPanel)
                {
                    UIComponent uIComponent = item.Value;
                    if (item.Key == EBagViewType.ViewBag)
                    {
                        bagEquipment.OnDestroy();
                        m_UI_CrystalBag.OnDestroy();
                        m_UI_OrnamentBag.OnDestroy();
                    }
                    item.Value.OnDestroy();
                }
            }
            m_Tabs.OnDestroy();
            curEBagViewType = 0;
        }
    }
}
