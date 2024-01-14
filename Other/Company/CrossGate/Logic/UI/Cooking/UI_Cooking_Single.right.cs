using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_Cooking_Single : UIBase
    {
        private GameObject m_CullRight;
        private ScrollGridVertical m_ScrollGridVertical_Right;
        private Dictionary<GameObject, IngredientGrid> m_IngredientGrids = new Dictionary<GameObject, IngredientGrid>();
        private int m_CurSelectIngredientIndex = -1;                    //食材格子选择
        private CP_ToggleRegistry m_CP_ToggleRegistry_Ingredients;      //食材页签
        private int m_CurIngredientTab;                                 //当前食材页签
        private int curIngredientTab
        {
            get { return m_CurIngredientTab; }
            set
            {
                if (m_CurIngredientTab != value)
                {
                    m_CurIngredientTab = value;
                    OnRefreshIngredient();
                }
            }
        }

        private List<ItemData> itemDatas = new List<ItemData>();

        private void RegisterRight()
        {
            m_CullRight = transform.Find("Animator/Right/Image_Lock").gameObject;
            m_ScrollGridVertical_Right = transform.Find("Animator/Right/Scroll View").GetComponent<ScrollGridVertical>();
            m_CP_ToggleRegistry_Ingredients = transform.Find("Animator/Right/Toggle_Tab").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Ingredients = transform.Find("Animator/Right/Toggle_Tab").GetComponent<CP_ToggleRegistry>();
            m_ScrollGridVertical_Right.AddCellListener(OnCellUpdateCallback_Right);
            m_ScrollGridVertical_Right.AddCreateCellListener(OnCreateCellCallback_Right);
            m_CP_ToggleRegistry_Ingredients.onToggleChange = OnIngredientTabChange;
        }


        private void OnIngredientTabChange(int curToggle, int old)
        {
            curIngredientTab = curToggle;
        }

        private void OnRefreshIngredient()
        {
            if (m_CurIngredientTab == 0)
            {
                itemDatas = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Meat);
                itemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.SeaFood));
                itemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Fruit));
                itemDatas.AddRange(Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Ingredients));
            }
            else if (m_CurIngredientTab == 1)
            {
                itemDatas = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Meat);
            }
            else if (m_CurIngredientTab == 2)
            {
                itemDatas = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.SeaFood);
            }
            else if (m_CurIngredientTab == 3)
            {
                itemDatas = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Fruit);
            }
            else if (m_CurIngredientTab == 4)
            {
                itemDatas = Sys_Bag.Instance.GetItemDatasByItemType((uint)EItemType.Ingredients);
            }
            m_ScrollGridVertical_Right.SetCellCount(itemDatas.Count);
            RecoverItemData();
        }

        private void RecoverItemData()
        {
            foreach (var item in m_Step1)
            {
                foreach (var kv in m_IngredientGrids)
                {
                    if (item.Value != null && kv.Value.itemData != null)
                    {
                        if (item.Value.Uuid == kv.Value.itemData.Uuid)
                        {
                            kv.Value.Use();
                        }
                    }
                }
            }
            foreach (var item in m_Step2)
            {
                foreach (var kv in m_IngredientGrids)
                {
                    if (item.Value != null && kv.Value.itemData != null)
                    {
                        if (item.Value.Uuid == kv.Value.itemData.Uuid)
                        {
                            kv.Value.Use();
                        }
                    }
                }
            }
        }


        private void RefreshRightView()
        {
            m_CP_ToggleRegistry_Ingredients.SwitchTo(m_CurIngredientTab);
            OnRefreshIngredient();
            OnShowOrHideRightMask();
        }

        private void OnShowOrHideRightMask()
        {
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                //自由烹饪
                m_CullRight.SetActive(false);
            }
            else if (m_CurSelectCookingModel == 3)
            {
                //食谱烹饪
                m_CullRight.SetActive(true);
            }
        }

        private void OnCellUpdateCallback_Right(ScrollGridCell cell)
        {
            IngredientGrid gridWarp = m_IngredientGrids[cell.gameObject];
            gridWarp.SetData(itemDatas[cell.index], cell.index);
            if (m_CurSelectIngredientIndex == cell.index)
            {
                gridWarp.Select();
            }
            else
            {
                gridWarp.Release();
            }
        }

        private void OnCreateCellCallback_Right(ScrollGridCell cell)
        {
            IngredientGrid gridWarp = new IngredientGrid();
            gridWarp.BindGameObject(cell.gameObject);
            gridWarp.AddEvent(OnIngredientItemSelected);
            m_IngredientGrids[cell.gameObject] = gridWarp;
        }

        private void OnIngredientItemSelected(IngredientGrid gridWarp)
        {
            if (m_CurSelectIngredientIndex != gridWarp.dataIndex)
            {
                m_CurSelectIngredientIndex = gridWarp.dataIndex;
                foreach (var item in m_IngredientGrids)
                {
                    if (item.Value.dataIndex == m_CurSelectIngredientIndex)
                    {
                        item.Value.Select();
                    }
                    else
                    {
                        item.Value.Release();
                    }
                }
            }
            if (gridWarp.count > 0)
            {
                SetNormalMidData(gridWarp.itemData);
            }
        }

        private void OnReplaced(ItemData itemData)
        {
            foreach (var item in m_IngredientGrids)
            {
                if (item.Value.itemData.Uuid == itemData.Uuid)
                {
                    item.Value.Add();
                }
            }
        }

        public class IngredientGrid
        {
            public int dataIndex;
            public ItemData itemData;
            public GameObject gameObject;
            private GameObject m_Select;
            //private Button m_ClickButton;
            private Action<IngredientGrid> m_OnClicked;
            private Action<IngredientGrid> m_OnLongPressed;
            private PropItem m_PropItem;
            public uint count;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;

                m_Select = gameObject.transform.Find("Image_Select").gameObject;
                //m_ClickButton = gameObject.transform.Find("Btn_Item").GetComponent<Button>();
                //m_ClickButton.onClick.AddListener(OnClicked);

                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(gameObject);
                m_PropItem.OnEnableLongPress(true);
            }

            public void SetData(ItemData itemData, int dataIndex)
            {
                this.itemData = itemData;
                this.dataIndex = dataIndex;
                this.count = itemData.Count;
                Refresh();
            }

            public void Refresh()
            {
                gameObject.name = dataIndex.ToString();
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                  (_id: itemData.Id,
                  _count: count,
                  _bUseQuailty: true,
                  _bBind: false,
                  _bNew: false,
                  _bUnLock: false,
                  _bSelected: false,
                  _bShowCount: true,
                  _bShowBagCount: false,
                  _bUseClick: true,
                  _onClick: OnClicked);
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_Cooking_Single, showItem));
            }

            private void RefreshCount()
            {
                m_PropItem.RefreshCount(count);
            }


            public void AddEvent(Action<IngredientGrid> onClick = null)
            {
                m_OnClicked = onClick;
            }


            private void OnClicked(PropItem propItem)
            {
                m_OnClicked.Invoke(this);
                Use();
            }

            public void Use()
            {
                if (count > 0)
                {
                    count--;
                    RefreshCount();
                }
            }

            public void Add()
            {
                if (count < itemData.Count)
                {
                    count++;
                    RefreshCount();
                }
            }


            private void OnLongPressed()
            {
                m_OnLongPressed.Invoke(this);
            }

            public void Release()
            {
                m_Select.SetActive(false);
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
        }
    }
}


