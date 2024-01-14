using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Search_Normal
    {
        private class HistoryGoods
        {
            private class CellGood
            {
                private Transform transform;

                private Button m_Btn;
                private Text m_TextName;
                //private Image m_ImgItemIcon;
                private PropItem propItem;
                private Image m_ImgPetIcon;

                private uint m_GoodId;

                private Action<uint> _action;

                public void Init(Transform trans)
                {
                    transform = trans;

                    m_Btn = transform.Find("Image_BG").GetComponent<Button>();
                    m_Btn.onClick.AddListener(OnClick);
                    m_TextName = transform.Find("Text_name").GetComponent<Text>();

                    propItem = new PropItem();
                    propItem.BindGameObject(transform.Find("PropItem").gameObject);
                    //m_ImgItemIcon = transform.Find("PropItem/Image_Icon").GetComponent<Image>();
                    m_ImgPetIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
                }

                public void Show()
                {
                    transform.gameObject.SetActive(true);
                }

                public void Hide()
                {
                    transform.gameObject.SetActive(false);
                }

                private void OnClick()
                {
                    _action?.Invoke(m_GoodId);
                }

                public void Register(Action<uint> action)
                {
                    _action = action;
                }

                public void UpdateInfo(uint goodId)
                {
                    m_GoodId = goodId;

                    propItem.SetActive(false);
                    //m_ImgItemIcon.transform.parent.gameObject.SetActive(false);
                    m_ImgPetIcon.transform.parent.gameObject.SetActive(false);

                    CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(m_GoodId);
                    if (goodData.type == 1
                        || goodData.type == 3
                        || goodData.type == 4
                        || goodData.type == 6)
                    {
                        CSVItem.Data item = CSVItem.Instance.GetConfData(m_GoodId);
                        if (item != null)
                        {
                            m_TextName.text = LanguageHelper.GetTextContent(item.name_id);
                            //m_ImgItemIcon.transform.parent.gameObject.SetActive(true);
                            //ImageHelper.SetIcon(m_ImgItemIcon, item.icon_id);
                            propItem.SetActive(true);
                            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(m_GoodId, 1, true, false, false, false, false, false, false, false);
                            propItem.SetData(itemData, EUIID.UI_Trade_Search);
                        }
                        else
                        {
                            Debug.LogErrorFormat("CSVItem 没有配置{0}", m_GoodId);
                        }
                    }
                    else if (goodData.type == 2)
                    {
                        CSVPetNew.Data pet = CSVPetNew.Instance.GetConfData(m_GoodId);
                        if (pet != null)
                        {
                            m_TextName.text = LanguageHelper.GetTextContent(pet.name);
                            m_ImgPetIcon.transform.parent.gameObject.SetActive(true);
                            ImageHelper.SetIcon(m_ImgPetIcon, pet.icon_id);
                        }
                        else
                        {
                            Debug.LogErrorFormat("CSVPet 没有配置{0}", m_GoodId);
                        }
                    }
                }
            }

            private Transform transform;

            private CellGood[] cellGoods = new CellGood[6];

            private Action<uint> _action;

            public void Init(Transform trans)
            {
                transform = trans;

                for (int i = 0; i < cellGoods.Length; ++i)
                {
                    cellGoods[i] = new CellGood();
                    cellGoods[i].Init(transform.Find(string.Format("Item{0}", i)));
                    cellGoods[i].Register(OnSelectGood);
                }
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);

                UpdateInfo();
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnSelectGood(uint goodId)
            {
                _action?.Invoke(goodId);
            }

            public void Register(Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo()
            {
                List<uint> ids = Sys_Trade.Instance.GetSearchHistory();
                for (int i = 0; i < cellGoods.Length; ++i)
                {
                    if (i < ids.Count)
                    {
                        cellGoods[i].Show();
                        cellGoods[i].UpdateInfo(ids[i]);
                    }
                    else
                    {
                        cellGoods[i].Hide();
                    }
                }
            }
        }

        private class StateGood
        {
            private class StateCell
            {
                private Transform transform;

                private uint m_State = 0; //0:上架，1：公示
                private CP_Toggle m_Toggle;

                private Action<uint> m_Action;
                public void Init(Transform trans)
                {
                    transform = trans;

                    m_Toggle = transform.GetComponent<CP_Toggle>();
                    m_Toggle.onValueChanged.AddListener(OnClick);
                }

                public void SetState(uint state)
                {
                    m_State = state;
                }

                private void OnClick(bool isOn)
                {
                    if (isOn)
                    {
                        m_Action?.Invoke((uint)m_State);
                    }
                }

                public void Register(Action<uint> action)
                {
                    m_Action = action;
                }

                public void OnSelect(bool isOn)
                {
                    m_Toggle.SetSelected(isOn, true);
                }
            }

            private Transform transform;

            public uint m_State = 0;
            private List<StateCell> listCells = new List<StateCell>();

            public void Init(Transform trans)
            {
                transform = trans;

                int count = transform.childCount;
                for (int i = 0; i < count; ++i)
                {
                    StateCell cell = new StateCell();

                    cell.Init(transform.GetChild(i));
                    cell.SetState((uint)i);
                    cell.Register(OnState);
                    cell.OnSelect(false);

                    listCells.Add(cell);
                }
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnState(uint state)
            {
                m_State = state;
            }

            public void UpdateState()
            {
                m_State = Sys_Trade.Instance.CurPageType == Sys_Trade.PageType.Buy ? (uint)0 : 1;
                int index = (int)m_State;
                listCells[index].OnSelect(true);
            }
        }

        private class MatchGoods
        {
            private class Cell
            {
                private Transform transform;

                private Button m_Btn;
                private Text m_TextName;

                private PropItem propItem;

                private GameObject m_PetGo;
                private Image m_ImgPetIcon;

                private uint m_GoodId;
                private System.Action<uint> m_Action;
                public void Init(Transform trans)
                {
                    transform = trans;

                    m_Btn = transform.Find("Image_BG").GetComponent<Button>();
                    m_Btn.onClick.AddListener(OnClick);

                    m_TextName = transform.Find("Text_name").GetComponent<Text>();

                    propItem = new PropItem();
                    propItem.BindGameObject(transform.Find("PropItem").gameObject);

                    m_PetGo = transform.Find("PetItem").gameObject;
                    m_ImgPetIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
                }

                private void OnClick()
                {
                    m_Action?.Invoke(m_GoodId);
                }

                public void Register(System.Action<uint> action)
                {
                    m_Action = action;
                }

                public void UpdateInfo(uint goodId)
                {
                    m_GoodId = goodId;

                    CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(goodId);
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(goodId);
                    m_TextName.text = LanguageHelper.GetTextContent(itemData.name_id);

                    propItem.SetActive(data.type != 2);
                    m_PetGo.SetActive(data.type == 2);
                    if (data.type == 2)
                    {
                        ImageHelper.SetIcon(m_ImgPetIcon, itemData.icon_id);
                    }
                    else
                    {
                        propItem.SetActive(true);
                        PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(m_GoodId, 1, true, false, false, false, false, false, false, false);
                        propItem.SetData(showitem, EUIID.UI_Trade_Search);
                    }
                }
            }

            private Transform transform;

            //private InfinityGridLayoutGroup gridGroup;
            //private int visualGridCount;
            private InfinityGrid _infinityGrid;
            private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();

            private List<uint> m_ListIds = new List<uint>();
            private Action<uint> m_Action;

            public void Init(Transform trans)
            {
                transform = trans;

                Button btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
                btnClose.onClick.AddListener(() => { Hide(); });

                _infinityGrid = transform.Find("Tips_Rect").GetComponent<InfinityGrid>();
                _infinityGrid.onCreateCell += OnCreateCell;
                _infinityGrid.onCellChange += OnChangeCell;
            }

            private void OnCreateCell(InfinityGridCell gridCell)
            {
                Cell entry = new Cell();

                entry.Init(gridCell.mRootTransform);
                gridCell.BindUserData(entry);
                entry.Register(OnSelectGood);

                dicCells.Add(gridCell.mRootTransform.gameObject, entry);
            }

            private void OnChangeCell(InfinityGridCell gridCell, int index)
            {
                Cell entry = gridCell.mUserData as Cell;
                entry.UpdateInfo(m_ListIds[index]);
            }


            private void OnSelectGood(uint goodId)
            {
                m_Action?.Invoke(goodId);
            }

            public void Register(Action<uint> action)
            {
                m_Action = action;
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            public void UpdateInfo(List<uint> listIds)
            {
                m_ListIds = listIds;
                _infinityGrid.CellCount = m_ListIds.Count;
                _infinityGrid.MoveToIndex(0);
                _infinityGrid.ForceRefreshActiveCell();
            }
        }

        private Transform transform;

        private InputField m_InputGood;
        private Button m_BtnSearch;
        private HistoryGoods m_HistoryGoods;
        private StateGood m_StateGood;
        private MatchGoods m_MathGoods;

        private uint _SelectGoodId = 0;

        public void Init(Transform trans)
        {
            transform = trans;

            m_InputGood = transform.Find("InputField").GetComponent<InputField>();
            //m_InputGood.contentType = TouchScreenKeyboardType.snt;
            m_InputGood.onEndEdit.AddListener(OnInputGood);

            m_BtnSearch = transform.Find("Button_Search").GetComponent<Button>();
            m_BtnSearch.onClick.AddListener(OnClickSearch);

            m_StateGood = new StateGood();
            m_StateGood.Init(transform.Find("Text_Bottom/Toggle"));

            m_HistoryGoods = new HistoryGoods();
            m_HistoryGoods.Init(transform.Find("Rectlist"));
            m_HistoryGoods.Register(OnSelectGoodId);

            m_MathGoods = new MatchGoods();
            m_MathGoods.Init(transform.Find("Image_Tip"));
            m_MathGoods.Register(OnSelectGoodId);

            InitInfo();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            m_HistoryGoods.Show();
            m_StateGood?.UpdateState();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void Destroy()
        {

        }

        private void OnInputGood(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                _SelectGoodId = 0u;
                return;
            }

            List<uint> tradeIds = new List<uint>();

            //List<uint> listIds = new List<uint>();
            //listIds.AddRange(CSVCommodity.Instance.GetDictData().Keys);

            var datas = CSVCommodity.Instance.GetAll();
            bool isFound = false;
            for (int i = 0, len = datas.Count; i < len; ++i)
            {
                uint itemID = datas[i].id;
                CSVItem.Data item = CSVItem.Instance.GetConfData(itemID);
                if (item != null)
                {
                    string name = LanguageHelper.GetTextContent(item.name_id);
                    if (name.IndexOf(str) >= 0)
                    {
                        tradeIds.Add(itemID);
                    }

                    //完全匹配
                    if (!isFound && name.Equals(str))
                    {
                        _SelectGoodId = itemID;
                        isFound = true;
                    }
                }
            }

            if (tradeIds.Count > 0)
            {
                m_MathGoods.Show();
                m_MathGoods.UpdateInfo(tradeIds);
            }
            else
            {
                m_MathGoods.Hide();
            }

            if (!isFound)
            {
                _SelectGoodId = 0u;
            }
        }

        private void OnClickSearch()
        {
            CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(_SelectGoodId);
            if (data != null)
            {
                TradeShowType showType = TradeShowType.OnSaleAndDiscuss;

                //公示
                if ((TradeShowType)m_StateGood.m_State == TradeShowType.Publicity)
                {
                    if (!data.publicity)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011204));
                        return;
                    }

                    showType = TradeShowType.Publicity;
                }

                ////议价
                //if ((TradeShowType)m_StateGood.m_State == TradeShowType.Discuss)
                //{
                //    if (!data.bargain)
                //    {
                //        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011203));
                //        return;
                //    }
                //}

                uint categoryId = Sys_Trade.Instance.SearchParam.bCross ? data.cross_category : data.category;
                uint subClass = Sys_Trade.Instance.SearchParam.bCross ? data.cross_subclass : data.subclass;

                CSVCommodityCategory.Data cateData = CSVCommodityCategory.Instance.GetConfData(categoryId);

                if (cateData != null)
                {
                    Sys_Trade.SearchData searchParam = Sys_Trade.Instance.SearchParam;
                    searchParam.Reset();
                    searchParam.isSearch = true;
                    searchParam.showType = showType;
                    searchParam.searchType = TradeSearchType.InfoId;
                    searchParam.infoId = _SelectGoodId;
                    searchParam.Category = cateData.list;
                    searchParam.SubCategory = categoryId;
                    searchParam.SubClass = subClass;

                    Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSearchNtf);

                    UIManager.CloseUI(EUIID.UI_Trade_Search, false, false);
                    Sys_Trade.Instance.PushHistory(_SelectGoodId);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011173));
            }
        }

        private void OnSelectGoodId(uint goodId)
        {
            m_MathGoods.Hide();
            _SelectGoodId = goodId;
            CSVItem.Data data = CSVItem.Instance.GetConfData(goodId);
            m_InputGood.text = LanguageHelper.GetTextContent(data.name_id);
        }

        private void InitInfo()
        {
            m_MathGoods.Hide();
            //m_HistoryGoods.Show();
        }
    }
}


