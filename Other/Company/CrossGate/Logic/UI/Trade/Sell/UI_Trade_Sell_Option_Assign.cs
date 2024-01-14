using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Sell_Option_Assign
    {
        private class AssignBuyner
        {
            private Transform transform;

            private Button m_Btn;
            private Text m_TextBuyner;
            private Button m_BtnBuyner;

            private System.Action _Action;
            public ulong m_AssignRoleId;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Btn = transform.Find("InputField").GetComponent<Button>();
                m_Btn.onClick.AddListener(OnClickBuyner);

                m_TextBuyner = transform.Find("InputField/Text").GetComponent<Text>();

                m_BtnBuyner = transform.Find("Button").GetComponent<Button>();
                m_BtnBuyner.onClick.AddListener(OnClickBuyner);
            }

            private void OnClickBuyner()
            {
                _Action?.Invoke();
            }

            public void Register(System.Action action)
            {
                _Action = action;
            }

            public void Reset()
            {
                m_AssignRoleId = 0L;
                m_TextBuyner.text = "选择购买人";
            }

            public void SetRoleInfo(Sys_Society.RoleInfo roleInfo)
            {
                m_AssignRoleId = roleInfo.roleID;
                m_TextBuyner.text = roleInfo.roleName;
            }
        }

        private class AssignTime
        {
            private Transform transform;

            private UI_Common_Num m_InputTime;

            private Button m_BtnSubTime;
            private Button m_BtnAddTime;

            public uint TradeTime;

            private uint _timeMin;
            private uint _timeMax;
            private uint m_PriceMin;

            public void Init(Transform trans)
            {
                transform = trans;

                m_InputTime = new UI_Common_Num();
                m_InputTime.Init(transform.Find("InputField"));
                m_InputTime.RegEnd(OnInputEndTime);

                m_BtnSubTime = transform.Find("Button_Sub").GetComponent<Button>();
                UI_LongPressButton SubButtonTime = m_BtnSubTime.gameObject.AddComponent<UI_LongPressButton>();
                //SubButtonTime.interval = 0.3f;
                //SubButtonPrice.bPressAcc = true;
                SubButtonTime.onRelease.AddListener(OnSubTime);
                //SubButtonPrice.OnPressAcc.AddListener(OnClickBtnPriceSub);

                m_BtnAddTime = transform.Find("Button_Add").GetComponent<Button>();
                UI_LongPressButton AddButtomTime = m_BtnAddTime.gameObject.AddComponent<UI_LongPressButton>();
                //AddButtomTime.interval = 0.3f;
                //AddButtonPrice.bPressAcc = true;
                AddButtomTime.onRelease.AddListener(OnAddTime);

                _timeMin = 1u;
                _timeMax = Sys_Trade.Instance.GetAssignTimeMax();
                m_PriceMin = Sys_Trade.Instance.GetAssignPriceMin();
            }

            private void OnSubTime()
            {
                if (TradeTime > _timeMin)
                {
                    TradeTime--;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011226));
                }

                m_InputTime.SetData(TradeTime);
            }

            public void OnAddTime()
            {
                if (TradeTime < _timeMax)
                {
                    TradeTime++;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011227));
                }

                m_InputTime.SetData(TradeTime);
            }

            private void OnInputEndTime(uint num)
            {
                TradeTime = num;
                if (TradeTime < _timeMin)
                {
                    TradeTime = _timeMin;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011226));
                }

                if (TradeTime > _timeMax)
                {
                    TradeTime = _timeMax;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011227));
                }

                m_InputTime.SetData(TradeTime);
            }

            public void Reset()
            {
                TradeTime = _timeMin;
                m_InputTime.SetData(TradeTime);
            }
        }

        private class AssignPrice
        {
            private Transform transform;

            private UI_Common_Num m_InputPrice;
            private Text m_TextPriceUnit;

            private System.Action _Action;
            public uint m_Price;
            private bool _isNumInput = false;
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
                if (m_Price < Sys_Trade.Instance.GetAssignPriceMin())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011158, Sys_Trade.Instance.GetAssignPriceMin().ToString()));
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
                m_InputPrice.SetData(m_Price);
                CalPriceUnit();
            }
        }

        private class AssignBoothPrice
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

        private class AssignHotPrice
        {
            private Transform transform;

            private Text m_TextHotPrice;
            public uint m_HotPrice;
            public void Init(Transform trans)
            {
                transform = trans;
                m_TextHotPrice = transform.Find("Label_Num").GetComponent<Text>();
            }

            public void Reset()
            {
                CalHotPrice(0u);
            }

            public void CalHotPrice(uint price)
            {
                m_HotPrice = Sys_Trade.Instance.GetHotPrice(price);
                //数量限制
                if (m_HotPrice > Sys_Trade.Instance.GetInputValueMax())
                {
                    m_HotPrice = Sys_Trade.Instance.GetInputValueMax();
                }
                m_TextHotPrice.text = m_HotPrice.ToString();
            }
        }

        private Transform transform;

        private AssignBuyner _assignBuyner;
        private AssignTime _assignTime;
        private AssignPrice _assignPrice;
        private AssignBoothPrice _assignBoothPrice;
        private AssignHotPrice _assignHotPrice;

        private IListner m_Listner;

        public ulong AssignRoleId { get { return _assignBuyner.m_AssignRoleId; } }
        public uint TradeTime { get { return _assignTime.TradeTime; } }
        public uint SalePrice { get { return _assignPrice.m_Price; } }
        public uint HotPrice { get { return _assignHotPrice.m_HotPrice; } }

        public void Init(Transform trans)
        {
            transform = trans;

            _assignBuyner = new AssignBuyner();
            _assignBuyner.Init(transform.Find("Image0"));
            _assignBuyner.Register(OnClickBuyner);

            _assignTime = new AssignTime();
            _assignTime.Init(transform.Find("Image1"));

            _assignPrice = new AssignPrice();
            _assignPrice.Init(transform.Find("Image2"));
            _assignPrice.Register(OnPriceChange);

            _assignBoothPrice = new AssignBoothPrice();
            _assignBoothPrice.Init(transform.Find("Image3"));

            _assignHotPrice = new AssignHotPrice();
            _assignHotPrice.Init(transform.Find("Image4"));
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

        private void OnClickBuyner()
        {
            m_Listner?.OnClickBuyner();
        }

        private void OnPriceChange()
        {
            _assignBoothPrice.CalBoothPrice(SalePrice);
            _assignHotPrice.CalHotPrice(SalePrice);
        }

        private void OnSelectSellServerType()
        {
            _assignBoothPrice.CalBoothPrice(SalePrice);
        }

        public void Register(IListner listner)
        {
            m_Listner = listner;
        }

        public void SetRoleInfo(Sys_Society.RoleInfo roleInfo)
        {
            _assignBuyner.SetRoleInfo(roleInfo);
        }

        public void UpdateInfo(CSVCommodity.Data goodData)
        {
            _assignBuyner.Reset();
            _assignTime.Reset();
            _assignPrice.Reset();
            _assignBoothPrice.Reset();
            _assignHotPrice.Reset();
        }

        public interface IListner
        {
            void OnClickBuyner();
        }
    }
}


