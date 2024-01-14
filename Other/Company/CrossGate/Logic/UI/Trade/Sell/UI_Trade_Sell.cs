using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Trade_Sell : UIBase, UI_Trade_Sell_Option_Assign.IListner
    {
        private Button m_BtnClose;

        private UI_Trade_Sell_Top m_SellTop;
        private UI_Trade_Sell_Server m_SellServer;
        private UI_Trade_Sell_Option_Type m_SellOpType;

        private UI_Trade_Sell_Option_Common m_Common;
        private UI_Trade_Sell_Option_Assign m_Assign;

        private UI_Trade_Sell_Buyer m_Buyer;

        private Button m_BtnSale;
        private Button m_BtnAssignInfo;

        private Sys_Trade.SellOpType m_SellType = Sys_Trade.SellOpType.None;

        private Sys_Trade.TradeItemInfo m_itemInfo;
        private ItemData _saleItem;
        private CSVCommodity.Data m_GoodData;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            //_saleItem = null;
            //m_GoodData = null;

            if (arg != null)
                m_itemInfo = (Sys_Trade.TradeItemInfo)arg;
        }

        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/View_TipsBg02_Middle/Btn_Close").GetComponent<Button>();
            m_BtnClose.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_Trade_Sell); });

            m_SellServer = new UI_Trade_Sell_Server();
            m_SellServer.Init(transform.Find("Animator/View_TipsBg02_Middle/Toggles"));

            m_SellOpType = new UI_Trade_Sell_Option_Type();
            m_SellOpType.Init(transform.Find("Animator/Image_Right/Toggles"));

            m_Common = new UI_Trade_Sell_Option_Common();
            m_Common.Init(transform.Find("Animator/Image_Right/State0"));

            m_Assign = new UI_Trade_Sell_Option_Assign();
            m_Assign.Init(transform.Find("Animator/Image_Right/State1"));
            m_Assign.Register(this);

            m_Buyer = new UI_Trade_Sell_Buyer();
            m_Buyer.Init(transform.Find("View_Buyer"));
            m_Buyer.Hide();

            m_SellTop = new UI_Trade_Sell_Top();
            m_SellTop.Init(transform.Find("Animator/Image_Right/Top"));

            m_BtnSale = transform.Find("Animator/Image_Right/Btn_01").GetComponent<Button>();
            m_BtnSale.onClick.AddListener(OnClickSale);

            m_BtnAssignInfo = transform.Find("Animator/Image_Right/Button_Tip").GetComponent<Button>();
            m_BtnAssignInfo.onClick.AddListener(OnClickAssignInfo);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {
            m_Common?.Hide();
            m_Assign?.Hide();
        }

        //protected override void OnDestroy()
        //{
        //    //m_Currency?.Dispose();
        //}

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.SellOpType>(Sys_Trade.EEvents.OnSellOptionType, OnSellOption, toRegister);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Society.RoleInfo>(Sys_Trade.EEvents.OnAssingBuyerNtf, OnAssignBuyer, toRegister);
        }

        private void OnAssignBuyer(Sys_Society.RoleInfo roleInfo)
        {
            m_Assign.SetRoleInfo(roleInfo);
            m_Buyer.Hide();
        }

        public void OnClickBuyner()
        {
            m_Buyer.Show();
        }

        private void OnClickSale()
        {
            if (m_SellType == Sys_Trade.SellOpType.Common)
            {
                bool bCross = Sys_Trade.Instance.CurSellServerType == Sys_Trade.ServerType.Cross;
                uint infoId = m_GoodData.id;
                bool isBargin = m_Common.IsBargin();
                uint salePrice = isBargin ? 0u : m_Common.Price;
                uint count = 1u;

                if (!isBargin && salePrice == 0u)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011157));
                    return;
                }

                uint minPrice = Sys_Trade.Instance.GetFreePricingMin(bCross);
                if (!isBargin && salePrice < minPrice)
                {
                    uint templanId = bCross ? 2011285u : 2011156;
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(templanId, minPrice.ToString()));
                    return;
                }

                if (!isBargin)
                {
                    //摊位费不足，需要弹出货币兑换
                    uint boothPrice = Sys_Trade.Instance.CalBoothPrice(count * salePrice);
                    boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, bCross);
                    if (boothPrice > Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.SilverCoin))
                    {
                        ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                        exchangeCoinParm.ExchangeType = (uint)ECurrencyType.SilverCoin;
                        exchangeCoinParm.needCount = 0;
                        UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                        //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, (uint)ECurrencyType.SilverCoin);
                        return;
                    }
                }

                //需要弹二次确认框
                Sys_Trade.SaleConfirmParam param = new Sys_Trade.SaleConfirmParam();
                param.Brief = new Packet.TradeBrief();
                param.Brief.BCross = bCross;
                param.Brief.InfoId = infoId;
                param.Brief.Count = count;
                param.Brief.GoodsUid = _saleItem.Uuid;
                param.Brief.Price = salePrice;
                param.Brief.Color = _saleItem.Quality;

                UIManager.CloseUI(EUIID.UI_Trade_Sell);
                UIManager.OpenUI(EUIID.UI_Trade_Sure_Tip, false, param);
            }
            else if (m_SellType == Sys_Trade.SellOpType.Assign)
            {
                bool bCross = Sys_Trade.Instance.CurSellServerType == Sys_Trade.ServerType.Cross;
                uint infoId = m_GoodData.id;
                uint salePrice = m_Assign.HotPrice;
                uint count = 1u;
                ulong roleId = m_Assign.AssignRoleId;
                uint targetPrice = m_Assign.SalePrice;
                uint time = m_Assign.TradeTime;

                if (targetPrice < Sys_Trade.Instance.GetAssignPriceMin())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011158, Sys_Trade.Instance.GetAssignPriceMin().ToString()));
                    return;
                }

                if (roleId != 0L)
                {

                    //摊位费不足，需要弹出货币兑换
                    uint boothPrice = Sys_Trade.Instance.CalBoothPrice(count * targetPrice);
                    boothPrice = Sys_Trade.Instance.GetBoothPrice(boothPrice, bCross);
                    if (boothPrice > Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.SilverCoin))
                    {
                        ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                        exchangeCoinParm.ExchangeType = (uint)ECurrencyType.SilverCoin;
                        exchangeCoinParm.needCount = 0;
                        UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                        //UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, (uint)ECurrencyType.SilverCoin);
                        return;
                    }

                    //需要弹二次确认框
                    Sys_Trade.SaleConfirmParam param = new Sys_Trade.SaleConfirmParam();
                    param.Brief = new Packet.TradeBrief();
                    param.Brief.BCross = bCross;
                    param.Brief.InfoId = infoId;
                    param.Brief.Count = count;
                    param.Brief.GoodsUid = _saleItem.Uuid;
                    param.Brief.Price = salePrice;
                    param.Brief.TargetId = roleId;
                    param.Brief.TargetPrice = targetPrice;
                    param.TargetLeast = time;
                    param.Brief.Color = _saleItem.Quality;

                    UIManager.CloseUI(EUIID.UI_Trade_Sell);
                    UIManager.OpenUI(EUIID.UI_Trade_Sure_Tip, false, param);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011206));
                }                
            }
        }

        private void OnClickAssignInfo()
        {
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(2011133, Sys_Trade.Instance.GetAssignTradeRate().ToString());

            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void UpdateInfo()
        {
            _saleItem = Sys_Trade.Instance.GetItemDataByTradeItemInfo(m_itemInfo);
            if (_saleItem != null)
                m_GoodData = CSVCommodity.Instance.GetConfData(_saleItem.Id);

            if (null == m_GoodData)
            {
                Debug.LogErrorFormat("CSVCommodity.Data is Null");
                return;
            }

            m_Buyer.Hide();

            m_SellTop.UpdateInfo(_saleItem);

            m_SellServer.Hide();
            if (m_GoodData.cross_server) //可跨服
            {
                m_SellServer.Show();
                m_SellServer.OnSelectServer(Sys_Trade.ServerType.Local);
            }
            else
            {
                Sys_Trade.Instance.SetSellServerType(Sys_Trade.ServerType.Local);
            }

            m_SellOpType.Hide();
            m_Common.Hide();
            m_Assign.Hide();
            if (m_GoodData.assignation) //可指定
            {
                m_SellOpType.Show();
                m_SellOpType.OnSelect(Sys_Trade.SellOpType.Common);
            }
            else
            {
                m_Common.Show();
                m_Common.UpdateInfo(m_GoodData);
                m_SellType = Sys_Trade.SellOpType.Common;
            }

            m_BtnAssignInfo.gameObject.SetActive(m_SellType == Sys_Trade.SellOpType.Assign);
        }

        private void OnSellOption(Sys_Trade.SellOpType opType)
        {
            m_Common?.Hide();
            m_Assign?.Hide();

            m_SellType = opType;
            m_BtnAssignInfo.gameObject.SetActive(m_SellType == Sys_Trade.SellOpType.Assign);

            switch (m_SellType)
            {
                case Sys_Trade.SellOpType.Common:
                    m_Common?.Show();
                    m_Common?.UpdateInfo(m_GoodData);
                    break;
                case Sys_Trade.SellOpType.Assign:
                    m_Assign?.Show();
                    m_Assign?.UpdateInfo(m_GoodData);
                    break;
            }
        }
    }
}


