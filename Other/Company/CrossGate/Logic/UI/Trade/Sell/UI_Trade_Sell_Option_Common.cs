using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using System;

namespace Logic
{
    public class UI_Trade_Sell_Option_Common 
    {
        private enum OptionType
        {
            None,
            Sale,   //出售
            Bargin, //议价
        }

        private class OptionCell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;
            private OptionType m_Type = OptionType.None;
            private Action<OptionType> m_Action;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnToggle);
            }

            private void OnToggle(bool isOn)
            {
                if (isOn)
                {
                    m_Action?.Invoke(m_Type);
                }
            }

            public void SetType(int typeIndex)
            {
                m_Type = (OptionType)(typeIndex + 1);
            }

            public void Register(Action<OptionType> action)
            {
                m_Action = action;
            }

            public void OnSelect(bool isSelect)
            {
                m_Toggle.SetSelected(isSelect, true);
            }
        }

        private class SalePrice
        {
            private Transform transform;

            private UI_Common_Num m_InputPrice;
            private Text m_TextPriceUnit;

            private System.Action _Action;
            public uint m_Price;
            public void Init(Transform trans)
            {
                transform = trans;

                m_InputPrice = new UI_Common_Num();
                m_InputPrice.Init(transform.Find("InputField"));
                m_InputPrice.RegEnd(OnInputEndPrice);

                m_TextPriceUnit = transform.Find("Text").GetComponent<Text>();
            }


            private void OnInputEndPrice(uint num)
            {
                m_Price = num;
                CalPriceUnit();
                _Action?.Invoke();

                //价格有最小提示
                bool bCross = Sys_Trade.Instance.CurSellServerType == Sys_Trade.ServerType.Cross;
                uint minPrice = Sys_Trade.Instance.GetFreePricingMin(bCross);
                if (m_Price < minPrice)
                {
                    uint templanId = bCross ? 2011285u : 2011156;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(templanId, minPrice.ToString()));
                }
            }

            private void CalPriceUnit()
            {
                m_TextPriceUnit.text = Sys_Trade.Instance.CalUnit(m_Price);
            }

            public void Register(System.Action action)
            {
                _Action = action;
            }

            public void Reset()
            {
                m_Price = 0;
                m_InputPrice.Reset();
                m_InputPrice.SetDefault(LanguageHelper.GetTextContent(2011108));

                CalPriceUnit();
            }
        }

        private class SaleBoothPrice
        {
            private Transform transform;

            private Text m_TextBoothPrice;
            private Button m_BtnBoothInfo;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TextBoothPrice = transform.Find("Label_Num").GetComponent<Text>();
                m_BtnBoothInfo = transform.Find("Button").GetComponent<Button>();
                m_BtnBoothInfo.onClick.AddListener(OnClickBoothInfo);
            }

            private void OnClickBoothInfo()
            {
                UIManager.OpenUI(EUIID.UI_Rule, false, Sys_Trade.Instance.GetBoothPriceTip());
            }

            public void Reset()
            {
                CalBoothPrice(0u);
            }

            public void CalBoothPrice(uint price)
            {
                uint boothPrice = Sys_Trade.Instance.CalBoothPrice(price);
                boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, Sys_Trade.Instance.CurSellServerType == Sys_Trade.ServerType.Cross);
                m_TextBoothPrice.text = boothPrice.ToString();
            }
        }

        private Transform transform;

        private Transform m_OptionParent;
        private List<OptionCell> listCells = new List<OptionCell>();

        private Transform m_BarginTip;
        private Transform m_CommonPrice;

        private SalePrice _SalePrice;
        private SaleBoothPrice _SaleBoothPrice;

        private OptionType m_OpType = OptionType.None;

        public uint Price { get { return _SalePrice.m_Price; } }

        public void Init(Transform trans)
        {
            transform = trans;

            m_OptionParent = transform.Find("Toggle");

            int count = m_OptionParent.childCount;
            for (int i = 0; i < count; ++i)
            {
                OptionCell cell = new OptionCell();
                cell.Init(m_OptionParent.GetChild(i));

                cell.SetType(i);
                cell.Register(OnSelectOption);
                listCells.Add(cell);
            }

            m_BarginTip = transform.Find("Text");

            m_CommonPrice = transform.Find("Image0");

            _SalePrice = new SalePrice();
            _SalePrice.Init(transform.Find("Image0"));
            _SalePrice.Register(OnPriceChange);

            _SaleBoothPrice = new SaleBoothPrice();
            _SaleBoothPrice.Init(transform.Find("Image1"));
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSellServerType, OnSelectSellServerType, true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);

            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSellServerType, OnSelectSellServerType, false);
        }

        private void OnPriceChange()
        {
            _SaleBoothPrice.CalBoothPrice(Price);
        }

        private void OnSelectOption(OptionType type)
        {
            m_OpType = type;
            UpdateType(m_OpType);
        }

        private void OnSelectSellServerType()
        {
            _SaleBoothPrice.CalBoothPrice(Price);
        }

        private void UpdateType(OptionType type)
        {
            m_CommonPrice.gameObject.SetActive(type == OptionType.Sale);
            m_BarginTip.gameObject.SetActive(type == OptionType.Bargin);

            if (type == OptionType.Bargin)
                _SaleBoothPrice.CalBoothPrice(0);
            else
                _SaleBoothPrice.CalBoothPrice(Price);
        }

        public bool IsBargin()
        {
            return m_OpType == OptionType.Bargin;
        }

        public void UpdateInfo(CSVCommodity.Data goodData)
        {
            if (goodData.bargain) //可议价
            {
                m_OptionParent.gameObject.SetActive(true);
                listCells[0].OnSelect(true);
            }
            else
            {
                m_OptionParent.gameObject.SetActive(false);
                m_OpType = OptionType.Sale;
                UpdateType(m_OpType);
            }

            _SalePrice.Reset();
            _SaleBoothPrice.Reset();
        }
    }
}


