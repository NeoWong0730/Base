using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Trade_Panel_Buy_View_Top
    {
        private class SubClass
        {
            private Transform transform;

            private Dropdown m_DropDown;

            private int m_lastIndex;
            private int m_Index;
            private List<uint> m_ListIds = new List<uint>();

            private Action m_Action;
            
            public uint SubId {
                get {
                    if (m_Index >= 0)
                        return m_ListIds[m_Index];
                    else
                        return 0u;
                }
            }

            public void Init(Transform trans)
            {
                transform = trans;

                m_DropDown = transform.Find("Dropdown").GetComponent<Dropdown>();
                m_DropDown.onValueChanged.AddListener((int value)=> OnClickDrop(value));
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClickDrop(int value)
            {
                if (m_lastIndex != value)
                {
                    m_lastIndex = m_Index = value;
                    m_Action?.Invoke();
                }
            }

            public void Register(Action action)
            {
                m_Action = action;
            }

            public void SetData(CSVCommodityCategory.Data category, uint subClass)
            {
                if (category.subclass == null)
                {
                    m_Index = -1;
                    Hide();
                    return;
                }

                m_ListIds = category.subclass;
                m_Index = m_ListIds.IndexOf(subClass);
                m_lastIndex = m_Index = m_Index < 0 ? 0 : m_Index;

                m_DropDown.ClearOptions();
                List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                if (category.subclass_name != null)
                {
                    for (int i = 0; i < category.subclass_name.Count; ++i)
                    {
                        Dropdown.OptionData op = new Dropdown.OptionData();
                        op.text = LanguageHelper.GetTextContent(category.subclass_name[i]);
                        options.Add(op);
                    }
                }

                m_DropDown.AddOptions(options);
                m_DropDown.value = m_Index;
                Show();
            }
        }

        private class Bargin 
        {
            private Transform transform;

            private Dropdown m_DropDown;

            private int m_lastIndex;
            private int m_Index;
            private Action m_Action;


            public int BarginIndex {
                get{
                    return m_Index; //0全部，1非议价，2议价
                }
            }

            public void Init(Transform trans)
            {
                transform = trans;

                m_DropDown = transform.Find("Dropdown").GetComponent<Dropdown>();
                m_DropDown.onValueChanged.AddListener((int value) => OnClickDrop(value));
            }

            private void OnClickDrop(int value)
            {
                if (m_lastIndex != value)
                {
                    m_lastIndex = m_Index = value;
                    m_Action?.Invoke();
                }
            }

            public void Register(Action action)
            {
                m_Action = action;
            }

            public void SetData(CSVCommodityCategory.Data category, int index)
            {
                m_DropDown.ClearOptions();
                if (category.bargain)
                {
                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

                    Dropdown.OptionData opAll = new Dropdown.OptionData();
                    opAll.text = LanguageHelper.GetTextContent(2011190);
                    options.Add(opAll);

                    Dropdown.OptionData opNone = new Dropdown.OptionData();
                    opNone.text = LanguageHelper.GetTextContent(2011093);
                    options.Add(opNone);

                    Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = LanguageHelper.GetTextContent(2011090);
                    options.Add(op);

                    m_DropDown.AddOptions(options);
                    m_DropDown.interactable = true;

                    m_lastIndex = m_Index = index;
                    m_DropDown.value = m_Index;
                }
                else
                {
                    List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
                    Dropdown.OptionData opNone = new Dropdown.OptionData();
                    opNone.text = LanguageHelper.GetTextContent(2011093);
                    options.Add(opNone);
                    m_DropDown.AddOptions(options);
                    m_DropDown.interactable = false;

                    m_lastIndex = m_Index = 1;
                    //m_DropDown.value = m_Index;
                }
            }
        }

        private class PriceSort
        {
            private Transform transform;

            private bool m_IsUpRank;
            private CP_Toggle m_Toggle;
            private Action m_Action;
            private Transform m_transBlack;

            private bool m_Freeze;

            private bool bActive;
            private Lib.Core.Timer cdTimer;

            public bool IsDownRank
            {
                get {
                    return !m_IsUpRank;
                }
            }

            public void Init(Transform trans)
            {
                transform = trans;
      
                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.toggleType = CP_Toggle.ToggleType.Single;
                m_Toggle.onValueChanged.AddListener(OnValueChange);
                m_transBlack = transform.Find("Background/Black");
            }

            private void OnValueChange(bool isOn)
            {
                m_IsUpRank = isOn;
                if (!m_Freeze)
                    m_Action?.Invoke();
                
                if (bActive)
                    StartCD();
            }

            public void Register(Action action)
            {
                m_Action = action;
            }

            public void SetData(CSVCommodityCategory.Data category)
            {
                m_Freeze = true;
                Lib.Core.Timer.Register(0.5f, () => { m_Freeze = false; });

                m_IsUpRank = true;
                m_Toggle.SetSelected(m_IsUpRank, true);
                //m_Toggle.isOn = m_IsUpRank;

                bActive = category.turn_page;
                m_Toggle.enabled = bActive;
                m_transBlack.gameObject.SetActive(!bActive);

                ImageHelper.SetImageGray(transform, !bActive, true);

                if (bActive)
                    StartCD();
            }

            private void StartCD()
            {
                cdTimer?.Cancel();
                cdTimer = null;
                m_Toggle.enabled = false;
                cdTimer = Lib.Core.Timer.Register(0.5f, () =>
                {
                    m_Toggle.enabled = true;
                }, null);
            }
        }

        private Transform transform;

        private Button m_BtnBack;

        private SubClass m_SubClass;
        private Bargin m_Bargin;
        private PriceSort m_PriceSort;


        public uint SubClassId
        {
            get
            {
                return m_SubClass.SubId;
            }
        }

        public int BarginIndex
        {
            get {
                return m_Bargin.BarginIndex;
            }
        }

        public bool IsDownRank
        {
            get {
                return m_PriceSort.IsDownRank;
            }
        }

        private IListner m_Listner;

        public void Init(Transform trans)
        {
            transform = trans;

            m_BtnBack = transform.Find("Button_Back").GetComponent<Button>();
            m_BtnBack.onClick.AddListener(OnClickBack);

            m_SubClass = new SubClass();
            m_SubClass.Init(transform.Find("Level"));
            m_SubClass.Register(OnChangeSubClass);

            m_Bargin = new Bargin();
            m_Bargin.Init(transform.Find("Bargain"));
            m_Bargin.Register(OnChangeBargin);

            m_PriceSort = new PriceSort();
            m_PriceSort.Init(transform.Find("Toggle"));
            m_PriceSort.Register(OnChangePriceSort);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnClickBack()
        {
            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != Packet.TradeShowType.Publicity)
                Sys_Trade.Instance.SearchParam.Reset();

            Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnViewBuyBackToSubCategory);
        }

        private void OnChangeSubClass()
        {
            m_Listner?.OnChangeSubClass();
        }

        private void OnChangeBargin()
        {
            m_Listner?.OnChangeBargin();
        }

        private void OnChangePriceSort()
        {
            m_Listner?.OnChangePriceSort();
        }

        public void Register(IListner listner)
        {
            m_Listner = listner;
        }

        public void UpdateInfo()
        {
            CSVCommodityCategory.Data data = CSVCommodityCategory.Instance.GetConfData(Sys_Trade.Instance.CurBuySubCategory);
            
            uint subClass = 0;
            int barginIndex = 0;

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != Packet.TradeShowType.Publicity)
            {
                subClass = Sys_Trade.Instance.SearchParam.SubClass;
                if (Sys_Trade.Instance.SearchParam.showType == Packet.TradeShowType.OnSaleAndDiscuss)
                    barginIndex = 0;
                else if (Sys_Trade.Instance.SearchParam.showType == Packet.TradeShowType.OnSale)
                    barginIndex = 1;
                else if (Sys_Trade.Instance.SearchParam.showType == Packet.TradeShowType.Discuss)
                    barginIndex = 2;
            }
            else
            {
                //需要设置subclass
                Sys_Trade.Instance.CalDefaultSubClass(data, ref subClass);
            }

            m_SubClass.SetData(data, subClass);
            m_Bargin.SetData(data, barginIndex);
            m_PriceSort.SetData(data);
        }

        public interface IListner
        {
            void OnChangeSubClass();
            void OnChangeBargin();
            void OnChangePriceSort();
        }
    }
}


