using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public enum ESafeType
    {
        ItemSafe,
        PetSefe,
    }

    public class UI_SafeBox : UIBase, UI_Safe_Right_Table.IListener
    {
        private UI_Safe_Right_Table rightTabs;
        private UI_ItemSafeBox_Component mUI_ItemSafeBox_Component;
        private UI_PetSafeBox_Component mUI_PetSafeBox_Component;
        private UI_CurrencyTitle UI_CurrencyTitle;
        private Dictionary<ESafeType, UIComponent> mDictTablesComponent = new Dictionary<ESafeType, UIComponent>();
        private Button closeBtn;

        private ESafeType tabType;

        public class UIMainBag_Copy_Component : UIComponent
        {
            private Dictionary<GameObject, CeilGrid> ceilGrids = new Dictionary<GameObject, CeilGrid>();
            private List<CeilGrid> uuidGrids = new List<CeilGrid>();
            private int visualableGridCount; //可以看到的总格子数量

            public int VisualableGridCount
            {
                get { return visualableGridCount; }
                set
                {
                    if (visualableGridCount != value)
                    {
                        visualableGridCount = value;
                        //infinity.SetAmount(visualableGridCount);
                    }
                }
            }

            List<ItemData> itemDatas = new List<ItemData>(); //数据部分

            Dictionary<uint, ItemData> positions = new Dictionary<uint, ItemData>(); //记录当前页签实际数据部分所有的位置信息

            //Dictionary<uint, CeilGrid> positionGridMap = new Dictionary<uint, CeilGrid>();
            private int curBoxId = 0; //1:物品   2:任务  3:材料  4:图鉴

            public int CurBoxId
            {
                get { return curBoxId; }
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
                        OnRefreshUIData(curBoxId);
                        //infinity.SetAmount(visualableGridCount);
                        infinity.CellCount = visualableGridCount;
                        infinity.ForceRefreshActiveCell();
                        infinity.MoveToIndex(0);
                    }
                }
            }

            private bool b_ReachMaxCapcity = false; //当前页签是否满了
            private int m_Column = 5; //一共多少列

            private InfinityGrid infinity;
            private Transform parent;
            private List<Toggle> toggles = new List<Toggle>();
            private Button clearUpBtn;
            private int curSelectDataIndex = -1;
            public Timer timer;


            public void OnRefreshUIData(int _curBagTab)
            {
                if (_curBagTab != CurBoxId)
                    return;
                itemDatas = Sys_Bag.Instance.BagItems[_curBagTab];
                positions.Clear();
                foreach (var item in itemDatas)
                    positions.Add(item.Position, item);
                UpdateBoxLevel();
            }

            public void OnUnLockBox(int boxid)
            {
                if (boxid != CurBoxId)
                    return;
                UpdateBoxLevel();
                //infinity._SetAmount(visualableGridCount);
                infinity.CellCount = visualableGridCount;
                infinity.ForceRefreshActiveCell();
                foreach (var item in ceilGrids)
                {
                    CeilGrid ceilGrid = item.Value;
                    int index = ceilGrid.gridIndex;
                    if (positions.ContainsKey((uint) index))
                    {
                        ceilGrid.SetData(positions[(uint) index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                    }
                    else
                    {
                        if (index < visualableGridCount - m_Column)
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                        }
                    }

                    if (index >= visualableGridCount - m_Column && index < visualableGridCount)
                    {
                        if (b_ReachMaxCapcity)
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                        }
                        else
                        {
                            ceilGrid.SetData(null, index, CeilGrid.EGridState.Unlock, CeilGrid.ESource.e_SafeBox);
                        }
                    }
                }
            }

            public void OnRefreshChangeData(int changeType, int curBoxid)
            {
                infinity.ForceRefreshActiveCell();
                /*
                 if (changeType == 0)
                 {
                     //保证页签拷贝的数据是当前页签的数据
                     if (curBoxid != CurBoxId)
                         return;
                     //清空了当前页签数据
                     if (Sys_Bag.Instance.changeItems.Count == 0)
                     {
                         for (int j = 0; j < uuidGrids.Count; j++)
                         {
                             uuidGrids[j].SetData(null, (int)uuidGrids[j].gridIndex, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
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
                                 uuidGrids[i].SetData(filterPositions[(uint)uuidGrids[i].gridIndex], uuidGrids[i].gridIndex,
                                     CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                             }
                             else
                             {
                                 uuidGrids[i].SetData(null, uuidGrids[i].gridIndex, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                             }
                         }
                     }
                 }
                 else if (changeType == 1)
                 {
                     List<ItemData> filters = new List<ItemData>();

                     for (int i = 0; i < Sys_Bag.Instance.changeItems.Count; i++)
                     {
                         if (Sys_Bag.Instance.changeItems[i].BoxId == CurBoxId)
                         {
                             filters.Add(Sys_Bag.Instance.changeItems[i]);
                         }
                     }

                     for (int i = 0; i < filters.Count; i++)
                     {
                         for (int j = 0; j < uuidGrids.Count; j++)
                         {
                             if (uuidGrids[j].gridIndex == filters[i].Position)
                             {
                                 uuidGrids[j].SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                             }
                         }
                     }

                     //for (int i = 0; i < filters.Count; i++)
                     //{
                     //    if (positionGridMap.ContainsKey(filters[i].Position))
                     //    {
                     //        CeilGrid ceilGrid = positionGridMap[filters[i].Position];
                     //        ceilGrid.SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                     //    }
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
                     //                focusceil.SetData(filters[i], (int)filters[i].Position, filters[i].Count > 0 ? CeilGrid.EGridState.Normal : CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                     //            }
                     //        }
                     //    }
                     //}

                 }
                 */
            }


            private void UpdateBoxLevel()
            {
                List<List<uint>> str = CSVBoxType.Instance.GetConfData((uint) (CurBoxId)).uarray2_value;
                int index = (int) Sys_Bag.Instance.Grids[(uint) CurBoxId];
                if (index == str.Count - 1)
                {
                    VisualableGridCount = (int) str[index][0];
                    b_ReachMaxCapcity = true;
                }
                else
                {
                    VisualableGridCount = (int) str[index][0] + m_Column;
                    b_ReachMaxCapcity = false;
                }
            }

            protected override void Loaded()
            {
                toggles.Add(transform.Find("List_Menu/TabList/ListItem").GetComponent<Toggle>());
                toggles.Add(transform.Find("List_Menu/TabList/ListItem (1)").GetComponent<Toggle>());
                toggles.Add(transform.Find("List_Menu/TabList/ListItem (2)").GetComponent<Toggle>());
                foreach (var item in toggles)
                {
                    item.onValueChanged.AddListener(val => { OnToggleLabelChanged(toggles.IndexOf(item), val); });
                }

                clearUpBtn = transform.Find("Button_Clear_Up").GetComponent<Button>();
                clearUpBtn.onClick.AddListener(
                    () =>
                    {
                        Sys_Bag.Instance.TidyBagPackReq((uint) CurBoxId);
                        //infinity.SetAmount(visualableGridCount);
                        //infinity.CellCount = visualableGridCount;
                        infinity.ForceRefreshActiveCell();
                        infinity.MoveToIndex(0);
                        ButtonHelper.Enable(clearUpBtn, false);
                        timer?.Cancel();
                        timer = Timer.Register(2, () => { ButtonHelper.Enable(clearUpBtn, true); });
                        OnClearAllSelect();
                    }
                );

                parent = transform.Find("Scroll_View_Bag/TabList").transform;
                infinity = parent.gameObject.GetNeedComponent<InfinityGrid>();
                infinity.onCreateCell = OnCreateGrid;
                infinity.onCellChange = OnCellChange;

                curBoxId = 1;
                Sys_Bag.Instance.curBoxId = curBoxId;
            }

            public override void Show()
            {
                gameObject.SetActive(true);
                ButtonHelper.Enable(clearUpBtn, true);
                //CurBoxId = 1;
                OnRefreshUIData(curBoxId);
                //infinity.SetAmount(visualableGridCount);
                infinity.CellCount = visualableGridCount;
                infinity.ForceRefreshActiveCell();
                toggles[CurBoxId - 1].isOn = true;
            }

            private void OnCreateGrid(InfinityGridCell cell)
            {
                CeilGrid entry = new CeilGrid();
                entry.BindGameObject(cell.mRootTransform.gameObject);
                entry.AddClickListener(OnGridSelected, OnGridLongPressed);
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

                if (positions.ContainsKey((uint) index))
                {
                    taskDataGrid.SetData(positions[(uint) index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                }
                else
                {
                    if (index < visualableGridCount - m_Column)
                    {
                        taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                    }
                }

                if (index >= visualableGridCount - m_Column && index < visualableGridCount)
                {
                    if (b_ReachMaxCapcity)
                    {
                        if (positions.ContainsKey((uint) index))
                        {
                            taskDataGrid.SetData(positions[(uint) index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                        }
                        else
                        {
                            taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                        }
                    }
                    else
                    {
                        taskDataGrid.SetData(null, index, CeilGrid.EGridState.Unlock, CeilGrid.ESource.e_SafeBox);
                    }
                }

                //positionGridMap[(uint)index] = taskDataGrid;
            }

            public override void Hide()
            {
                gameObject.SetActive(false);
            }

            private void OnToggleLabelChanged(int index, bool active)
            {
                if (active)
                {
                    if (index == 0)
                    {
                        CurBoxId = 1;
                    }
                    else if (index == 1)
                    {
                        CurBoxId = 3;
                    }
                    else if (index == 2)
                    {
                        CurBoxId = 4;
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
                            if (item.mItemData.cSVItemData.bank_use == 0)
                            {
                                string content = CSVLanguage.Instance.GetConfData(1000964).words;
                                Sys_Hint.Instance.PushContent_Normal(content);
                            }
                            else
                            {
                                Sys_Bag.Instance.TransformItemReq(bagCeilGrid.mItemData.Uuid, 5, bagCeilGrid.mItemData.Position);
                            }
                        }
                        else if (bagCeilGrid.eGridState == CeilGrid.EGridState.Unlock)
                        {
                            //解锁格子
                            PromptBoxParameter.Instance.Clear();
                            string currency = LanguageHelper.GetLanguageColorWordsFormat(1000907);
                            string content = CSVLanguage.Instance.GetConfData(1000909).words;
                            string count = LanguageHelper.GetLanguageColorWordsFormat(Sys_Bag.Instance.GetCurMainBagBoxLevelUpCost().ToString(), 1000907);
                            PromptBoxParameter.Instance.content = string.Format(content, count, currency);
                            PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Bag.Instance.UnLockGridReq((uint) bagCeilGrid.boxId, Sys_Bag.Instance.Grids[(uint) bagCeilGrid.boxId] + 1); });
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

            private void OnGridLongPressed(CeilGrid bagCeilGrid)
            {
                if (bagCeilGrid.eGridState == CeilGrid.EGridState.Empty || bagCeilGrid.eGridState == CeilGrid.EGridState.Unlock)
                    return;
                Sys_Bag.Instance.ProcessCeilGridClick(bagCeilGrid, false);
            }
        }

        public class UISafeBox_Component : UIComponent
        {
            private int curSafeBoxTabId = 0;

            public int CurSafeBoxTabId
            {
                get { return curSafeBoxTabId; }
                set
                {
                    if (curSafeBoxTabId != value)
                    {
                        curSafeBoxTabId = value;
                        Sys_Bag.Instance.curSafeBoxTabId = curSafeBoxTabId;
                        OnRefreshUIData(curSafeBoxTabId);
                        infinity_Box.CellCount = VisualableGridCount_Box;
                        infinity_Box.ForceRefreshActiveCell();
                        infinity_Box.MoveToIndex(0);
                    }
                }
            }

            private Transform m_LabParent;

            private CP_ToggleRegistry m_CpToggleRegistry_Lab;

            private int SafeboxTabUnLuckLevel
            {
                get { return (int) Sys_Bag.Instance.Grids[5]; }
            }

            private Dictionary<GameObject, CeilGrid> ceilGrids = new Dictionary<GameObject, CeilGrid>();

            private List<CeilGrid> uuidGrids = new List<CeilGrid>();

            private InfinityGrid infinity_Box;

            private Transform parent_Box;

            private List<ItemData> itemDatas = new List<ItemData>(); //当前银行页签过滤的数据

            Dictionary<uint, ItemData> positions = new Dictionary<uint, ItemData>(); //记录当前页签实际数据部分所有的位置信息

            private uint startPosition;

            private uint endPosition;

            private int curSelectDataIndex = -1;

            private int visualableGridCount_Box; //可以看到的总格子数量

            public int VisualableGridCount_Box
            {
                get { return visualableGridCount_Box; }
                set
                {
                    if (visualableGridCount_Box != value)
                    {
                        visualableGridCount_Box = value;
                        //infinity_Box.SetAmount(visualableGridCount_Box);
                        infinity_Box.CellCount = VisualableGridCount_Box;
                        infinity_Box.ForceRefreshActiveCell();
                        infinity_Box.MoveToIndex(0);
                    }
                }
            }

            private Button tidySafeBox;
            public Timer timer;

            protected override void Loaded()
            {
                m_LabParent = transform.Find("List_Menu/TabList");
                m_CpToggleRegistry_Lab = transform.Find("List_Menu/TabList").GetComponent<CP_ToggleRegistry>();
                m_CpToggleRegistry_Lab.onToggleChange += OnTabChanged;

                parent_Box = transform.Find("Scroll_View_Bag/TabList");
                infinity_Box = parent_Box.gameObject.GetNeedComponent<InfinityGrid>();
                infinity_Box.onCreateCell = OnCreateGrid;
                infinity_Box.onCellChange = OnCellChange;

                tidySafeBox = transform.Find("Button_Clear_Up").GetComponent<Button>();
                tidySafeBox.onClick.AddListener(() =>
                {
                    Sys_Bag.Instance.TidyBagPackReq(5);
                    //infinity_Box.SetAmount(VisualableGridCount_Box);
                    infinity_Box.CellCount = VisualableGridCount_Box;
                    infinity_Box.ForceRefreshActiveCell();
                    infinity_Box.MoveToIndex(0);
                    ButtonHelper.Enable(tidySafeBox, false);
                    timer?.Cancel();
                    timer = Timer.Register(2, () => { ButtonHelper.Enable(tidySafeBox, true); });
                    OnClearAllSelect();
                });

                //for (int i = 0; i < parent_Box.childCount; i++)
                //{
                //    GameObject go = parent_Box.GetChild(i).gameObject;
                //    CeilGrid bagCeilGrid = new CeilGrid();
                //    bagCeilGrid.BindGameObject(go);
                //    bagCeilGrid.AddClickListener(OnGridSelected, OnGridLongPressed);
                //    ceilGrids.Add(go, bagCeilGrid);
                //    uuidGrids.Add(bagCeilGrid);
                //}
            }

            private void OnCreateGrid(InfinityGridCell cell)
            {
                CeilGrid entry = new CeilGrid();
                entry.BindGameObject(cell.mRootTransform.gameObject);
                entry.AddClickListener(OnGridSelected, OnGridLongPressed);
                cell.BindUserData(entry);
                ceilGrids.Add(cell.mRootTransform.gameObject, entry);
                uuidGrids.Add(entry);
            }

            private void OnCellChange(InfinityGridCell cell, int index)
            {
                if (index >= endPosition - startPosition + 1)
                    return;
                CeilGrid taskDataGrid = ceilGrids[cell.mRootTransform.gameObject];
                if (curSelectDataIndex != index)
                {
                    taskDataGrid.Release();
                }

                if (positions.ContainsKey((uint) index + startPosition))
                {
                    taskDataGrid.SetData(positions[(uint) index + startPosition], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                }
                else
                {
                    taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                }
            }


            public override void Show()
            {
                gameObject.SetActive(true);
                ButtonHelper.Enable(tidySafeBox, true);
                Sys_Bag.Instance.curSafeBoxTabId = 0;
                CurSafeBoxTabId = Sys_Bag.Instance.curSafeBoxTabId;

                BuildLab();

                OnRefreshUIData(curSafeBoxTabId);
                infinity_Box.CellCount = VisualableGridCount_Box;
                infinity_Box.ForceRefreshActiveCell();
                infinity_Box.MoveToIndex(0);
            }

            private void BuildLab()
            {
                int needCount = Sys_Bag.Instance.GetSafeBoxTabCount();

                FrameworkTool.CreateChildList(m_LabParent, needCount);

                for (int i = 0; i < needCount; i++)
                {
                    CP_Toggle ct = m_LabParent.GetChild(i).GetComponent<CP_Toggle>();
                    ct.id = i;
                }

                m_CpToggleRegistry_Lab.SwitchTo(curSafeBoxTabId);

                UpdateSafeBoxTabUI();
            }

            public override void Hide()
            {
                gameObject.SetActive(false);
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

            private void OnGridSelected(CeilGrid bagCeilGrid)
            {
                curSelectDataIndex = bagCeilGrid.gridIndex;
                foreach (var item in uuidGrids)
                {
                    if (item.gridIndex == curSelectDataIndex)
                    {
                        item.Select();
                        if (item.eGridState == CeilGrid.EGridState.Normal)
                        {
                            Sys_Bag.Instance.TransformItemReq(item.mItemData.Uuid, item.mItemData.cSVItemData.box_id, item.mItemData.Position);
                        }
                    }
                    else
                    {
                        item.Release();
                    }
                }
            }


            private void OnGridLongPressed(CeilGrid bagCeilGrid)
            {
                if (bagCeilGrid.eGridState == CeilGrid.EGridState.Empty)
                    return;
                Sys_Bag.Instance.ProcessCeilGridClick(bagCeilGrid, false);
            }


            private void OnTabChanged(int curTab, int oldTab)
            {
                if (curTab == oldTab)
                {
                    return;
                }

                CurSafeBoxTabId = curTab;

                if (curSafeBoxTabId > SafeboxTabUnLuckLevel)
                {
                    //解锁格子
                    PromptBoxParameter.Instance.Clear();
                    string currency = LanguageHelper.GetLanguageColorWordsFormat(1000907);
                    string content = CSVLanguage.Instance.GetConfData(1000908).words;
                    string count = LanguageHelper.GetLanguageColorWordsFormat(Sys_Bag.Instance.GetCurSafeBoxLevelUpCost().ToString(), 1000907);
                    PromptBoxParameter.Instance.content = string.Format(content, count, currency);
                    PromptBoxParameter.Instance.SetConfirm(true, () => { Sys_Bag.Instance.UnLockGridReq(5, Sys_Bag.Instance.Grids[5] + 1); });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }

            private void UpdateChildrenCallback_Box(int index, Transform trans)
            {
                if (index >= endPosition - startPosition + 1)
                    return;
                CeilGrid taskDataGrid = ceilGrids[trans.gameObject];
                if (curSelectDataIndex != index)
                {
                    taskDataGrid.Release();
                }

                if (positions.ContainsKey((uint) index + startPosition))
                {
                    taskDataGrid.SetData(positions[(uint) index + startPosition], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                }
                else
                {
                    taskDataGrid.SetData(null, index, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                }
            }

            public void OnRefreshUIData(int safeboxTab)
            {
                List<ItemData> datas = Sys_Bag.Instance.BagItems[5];
                List<List<uint>> val = CSVBoxType.Instance.GetConfData(5).uarray2_value;
                if (safeboxTab == 0)
                {
                    startPosition = 0;
                    endPosition = val[0][0] - 1;
                }
                else
                {
                    startPosition = val[safeboxTab - 1][0];
                    endPosition = val[safeboxTab][0] - 1;
                }

                positions.Clear();
                itemDatas.Clear();
                for (int i = 0; i < datas.Count; i++)
                {
                    if (datas[i].Position >= startPosition && datas[i].Position <= endPosition)
                    {
                        itemDatas.Add(datas[i]);
                        positions.Add(datas[i].Position, datas[i]);
                    }
                }

                VisualableGridCount_Box = (int) endPosition - (int) startPosition + 1;
            }

            public void OnRefreshChangeData(int changeType, int curBoxid)
            {
                //页签拷贝数据
                if (changeType == 0)
                {
                    //保证页签拷贝的数据是当前页签的数据
                    if (curBoxid != 5)
                        return;
                    //清空了当前页签数据
                    if (Sys_Bag.Instance.changeItems.Count == 0)
                    {
                        for (int j = 0; j < uuidGrids.Count; j++)
                        {
                            uuidGrids[j].SetData(null, (int) uuidGrids[j].gridIndex, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                        }
                    }
                    else
                    {
                        List<uint> filterPositions = new List<uint>();
                        for (int i = 0; i < Sys_Bag.Instance.changeItems.Count; i++)
                        {
                            if (Sys_Bag.Instance.changeItems[i].Position >= startPosition && Sys_Bag.Instance.changeItems[i].Position <= endPosition)
                            {
                                filterPositions.Add(Sys_Bag.Instance.changeItems[i].Position);
                            }
                        }

                        for (int i = 0; i < uuidGrids.Count; i++)
                        {
                            if (filterPositions.Contains((uint) uuidGrids[i].gridIndex + startPosition))
                            {
                                uuidGrids[i].SetData(Sys_Bag.Instance.changeItems[(int) (i + startPosition)], uuidGrids[i].gridIndex, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                            }
                            else
                            {
                                uuidGrids[i].SetData(null, uuidGrids[i].gridIndex, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                            }
                        }
                    }
                }
                else if (changeType == 1)
                {
                    List<ItemData> filters = new List<ItemData>();

                    for (int i = 0; i < Sys_Bag.Instance.changeItems.Count; i++)
                    {
                        if (Sys_Bag.Instance.changeItems[i].BoxId == 5)
                        {
                            filters.Add(Sys_Bag.Instance.changeItems[i]);
                        }
                    }

                    for (int i = 0; i < filters.Count; i++)
                    {
                        for (int j = 0; j < uuidGrids.Count; j++)
                        {
                            if (uuidGrids[j].gridIndex + startPosition == filters[i].Position)
                            {
                                if (filters[i].Count > 0)
                                {
                                    uuidGrids[j].SetData(filters[i], uuidGrids[j].gridIndex, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_SafeBox);
                                }
                                else
                                {
                                    uuidGrids[j].SetData(filters[i], uuidGrids[j].gridIndex, CeilGrid.EGridState.Empty, CeilGrid.ESource.e_SafeBox);
                                }
                            }
                        }
                    }
                }
            }


            public void UpdateSafeBoxTabUI()
            {
                for (int i = 0; i < m_LabParent.childCount; i++)
                {
                    if (i <= SafeboxTabUnLuckLevel)
                    {
                        //表示该页签已经解锁，
                        m_LabParent.GetChild(i).Find("Image_LockState").gameObject.SetActive(false);
                    }
                    else
                    {
                        //表示该页签还未解锁，
                        m_LabParent.GetChild(i).Find("Image_LockState").gameObject.SetActive(true);
                    }
                }
            }
        }

        public class UI_ItemSafeBox_Component : UIComponent
        {
            public UIMainBag_Copy_Component mUIMainBag_Copy_Component;
            public UISafeBox_Component mUISafeBox_Component;

            protected override void Loaded()
            {
                mUIMainBag_Copy_Component = AddComponent<UIMainBag_Copy_Component>(transform.Find("View_Right_List"));
                mUISafeBox_Component = AddComponent<UISafeBox_Component>(transform.Find("View_Middle"));
            }

            public override void Show()
            {
                mUIMainBag_Copy_Component.Show();
                mUISafeBox_Component.Show();
            }

            public override void OnDestroy()
            {
                mUIMainBag_Copy_Component.OnDestroy();
            }

            public override void Hide()
            {
                mUIMainBag_Copy_Component.timer?.Cancel();
                mUISafeBox_Component.timer?.Cancel();
                mUISafeBox_Component.Hide();
                mUIMainBag_Copy_Component.Hide();
            }
        }

        protected override void OnLoaded()
        {
            rightTabs = AddComponent<UI_Safe_Right_Table>(transform.Find("Animator/View_Left_Tabs"));
            rightTabs.Register(this);
            mUI_ItemSafeBox_Component = AddComponent<UI_ItemSafeBox_Component>(transform.Find("Animator"));
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            mUI_PetSafeBox_Component = AddComponent<UI_PetSafeBox_Component>(transform.Find("Animator/View_Pet"));

            mDictTablesComponent.Add(ESafeType.ItemSafe, mUI_ItemSafeBox_Component);
            mDictTablesComponent.Add(ESafeType.PetSefe, mUI_PetSafeBox_Component);

            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(
                () =>
                {
                    Sys_Bag.Instance.ClearNewIconReq(Sys_Bag.Instance.curBoxId);
                    UIManager.CloseUI(EUIID.UI_SafeBox);
                }
            );
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, mUI_ItemSafeBox_Component.mUIMainBag_Copy_Component.OnRefreshChangeData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnUnLuckBox, mUI_ItemSafeBox_Component.mUIMainBag_Copy_Component.OnUnLockBox, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, mUI_ItemSafeBox_Component.mUIMainBag_Copy_Component.OnRefreshUIData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnClearCopyBagSelect, mUI_ItemSafeBox_Component.mUIMainBag_Copy_Component.OnClearAllSelect, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnClearSafeBoxSelect, mUI_ItemSafeBox_Component.mUISafeBox_Component.OnClearAllSelect, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, mUI_ItemSafeBox_Component.mUISafeBox_Component.OnRefreshChangeData, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle(Sys_Bag.EEvents.OnUpdateSafeBoxTab, mUI_ItemSafeBox_Component.mUISafeBox_Component.UpdateSafeBoxTabUI, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshSafeBoxBagData, mUI_ItemSafeBox_Component.mUISafeBox_Component.OnRefreshUIData, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnGetPetBankData, mUI_PetSafeBox_Component.LeftViewRefresh, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnStorageChang, mUI_PetSafeBox_Component.OnStorageChange, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnPetBankUnlock, mUI_PetSafeBox_Component.mUI_PetSafeBox_Left.ButtonStateCheck, toRegister);
        }

        protected override void OnShow()
        {
            rightTabs.Show();
            UI_CurrencyTitle.InitUi();
            mUI_ItemSafeBox_Component.Show();

            Sys_Bag.Instance.SetCurrentInteractiveNPC();
        }

        protected override void OnDestroy()
        {
            mUI_ItemSafeBox_Component.OnDestroy();
            UI_CurrencyTitle.Dispose();
        }

        protected override void OnHide()
        {
            foreach (var data in mDictTablesComponent)
            {
                data.Value.Hide();
            }

            rightTabs.Hide();
        }

        public void OnClickTabType(ESafeType _type)
        {
            tabType = _type;

            foreach (var dict in mDictTablesComponent)
            {
                if (dict.Key == _type)
                {
                    dict.Value.Show();
                }
                else
                {
                    dict.Value.Hide();
                }
            }
        }
    }
}