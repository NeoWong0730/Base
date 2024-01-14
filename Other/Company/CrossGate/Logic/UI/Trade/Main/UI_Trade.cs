using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Trade : UIBase
    {
        private Button m_BtnClose;
        private Button m_BtnTradeRecord;
        private UI_CurrencyTitle m_Currency;

        private UI_Trade_Page m_Page;

        private UI_Trade_Panel_Buy m_PanelBuy;
        private UI_Trade_Panel_Sell m_PanelSell;
        private UI_Trade_Panel_Publicity m_PanelPublicity;

        private Sys_Trade.TelData _TelData;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            _TelData = null;
            if (arg != null)
            {
                _TelData = (Sys_Trade.TelData)arg;
            }
            //请求是否有审核信息
            Sys_Trade.Instance.OnCheckInfoReq();
            // //请求关注列表
            // Sys_Trade.Instance.OnWatchListReq();
        }

        protected override void OnLoaded()
        {
            m_Currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            m_BtnClose = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();

            m_BtnTradeRecord = transform.Find("Animator/Button_person").GetComponent<Button>();

            m_Page = new UI_Trade_Page();
            m_Page.Init(transform.Find("Animator/View_Right"));

            m_PanelBuy = new UI_Trade_Panel_Buy();
            m_PanelBuy.Init(transform.Find("Animator/Page0"));

            m_PanelSell = new UI_Trade_Panel_Sell();
            m_PanelSell.Init(transform.Find("Animator/Page1"));

            m_PanelPublicity = new UI_Trade_Panel_Publicity();
            m_PanelPublicity.Init(transform.Find("Animator/Page2"));

            m_BtnClose.onClick.AddListener(() => { this.CloseSelf(); });
            m_BtnTradeRecord.onClick.AddListener(OnClickTradeRecord);
        }

        protected override void OnShow()
        {
            m_Currency?.InitUi();
            Sys_Trade.Instance.CheckAdvanceBuyUsedTimes();
            OnCheckTel();
        }

        protected override void OnHide()
        {
            m_PanelBuy.Hide();
            m_PanelSell.Hide();
            Sys_Trade.Instance.ClearData();
        }

        protected override void OnDestroy()
        {
            m_Currency?.Dispose();
            m_PanelBuy?.OnDestroy();
            m_PanelPublicity?.OnDestroy();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Trade.Instance.eventEmitter.Handle<Sys_Trade.PageType>(Sys_Trade.EEvents.OnSelectPage, OnSelecPageType, toRegister);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnSearchNtf, OnSearchNtf, toRegister);
        }

        private void OnClickTradeRecord()
        {
            UIManager.OpenUI(EUIID.UI_Trade_Record);
        }

        private void OnCheckTel()
        {
            if (_TelData != null)
            {
                if (_TelData.telType == 0u) //查找
                {
                    CSVCommodity.Data data = CSVCommodity.Instance.GetConfData(_TelData.itemInfoId);
                    if (data != null)
                    {
                        uint categoryId = _TelData.bCross ? data.cross_category : data.category;
                        CSVCommodityCategory.Data cateData = CSVCommodityCategory.Instance.GetConfData(categoryId);

                        if (cateData != null)
                        {
                            Sys_Trade.SearchData searchParam = Sys_Trade.Instance.SearchParam;
                            searchParam.Reset();
                            searchParam.isSearch = true;
                            searchParam.showType = _TelData.tradeShowType;
                            searchParam.searchType = Packet.TradeSearchType.Category;
                            searchParam.goodsUId = _TelData.goodsUId;
                            searchParam.infoId = _TelData.itemInfoId;
                            searchParam.Category = cateData.list;
                            searchParam.SubCategory = categoryId;
                            searchParam.SubClass = data.subclass;

                            Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSearchNtf);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011229));
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("找不到 商品id = {0}", _TelData.itemInfoId);
                    }
                }
                else if (_TelData.telType == 1u) //上架
                {
                    m_Page.OnSelectPage(Sys_Trade.PageType.Sell);
                    if (_TelData.itemData != null)
                    {
                        Sys_Trade.TradeItemInfo itemInfo = new Sys_Trade.TradeItemInfo() { uID = _TelData.itemData.Uuid, infoID = _TelData.itemData.Id };
                        Sys_Trade.Instance.OnSaleItemCheck(itemInfo);
                    }
                }
                else if (_TelData.telType == 2u)//跳转到某个分类
                {
                    Sys_Trade.SearchData searchParam = Sys_Trade.Instance.SearchParam;
                    searchParam.Reset();
                    searchParam.isSearch = true;
                    //searchParam.showType = _TelData.tradeShowType;
                    //searchParam.searchType = Packet.TradeSearchType.Category;
                    //searchParam.infoId = _TelData.itemInfoId;
                    searchParam.Category = _TelData.itemInfoId;
                    //searchParam.SubCategory = categoryId;
                    //searchParam.SubClass = data.subclass;

                    Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSearchNtf);
                }
            }
            else
            {
                Sys_Trade.PageType pageType = Sys_Trade.Instance.CurPageType;
                if (pageType == Sys_Trade.PageType.None)
                    pageType = Sys_Trade.PageType.Buy;

                m_Page.OnSelectPage(pageType);
            }
        }

        private void OnSelecPageType(Sys_Trade.PageType pageType)
        {
            m_PanelBuy.Hide();
            m_PanelSell.Hide();
            m_PanelPublicity.Hide();

            switch (pageType)
            {
                case Sys_Trade.PageType.Buy:
                    m_PanelBuy.Show();

                    ////if none op
                    //if (!Sys_Trade.Instance.IsOpBuy)
                    //{
                    //    m_PanelBuy.UpdateInfo(Sys_Trade.ServerType.Local);
                    //    Sys_Trade.Instance.IsOpBuy = true;
                    //}

                    if (Sys_Trade.Instance.SearchParam.isSearch)
                    {
                        Sys_Trade.ServerType serverType = Sys_Trade.ServerType.Local;
                        if (Sys_Trade.Instance.SearchParam.bCross)
                            serverType = Sys_Trade.ServerType.Cross;
                        m_PanelBuy.UpdateInfo(serverType);
                    }
                    else if (!Sys_Trade.Instance.IsOpBuy)
                    {
                        m_PanelBuy.UpdateInfo(Sys_Trade.ServerType.Local);
                        Sys_Trade.Instance.IsOpBuy = true;
                    }

                    break;
                case Sys_Trade.PageType.Sell:
                    m_PanelSell.Show();
                    break;
                case Sys_Trade.PageType.Publicity:
                    m_PanelPublicity.Show();

                    ////if none op
                    //if (!Sys_Trade.Instance.IsOpPublicity)
                    //{
                    //    m_PanelPublicity.UpdateInfo(Sys_Trade.ServerType.Local);
                    //    Sys_Trade.Instance.IsOpPublicity = true;
                    //}

                    if (Sys_Trade.Instance.SearchParam.isSearch)
                    {
                        Sys_Trade.ServerType serverType = Sys_Trade.ServerType.Local;
                        if (Sys_Trade.Instance.SearchParam.bCross)
                            serverType = Sys_Trade.ServerType.Cross;
                        m_PanelPublicity.UpdateInfo(serverType);
                    }
                    else if (!Sys_Trade.Instance.IsOpPublicity)
                    {
                        m_PanelPublicity.UpdateInfo(Sys_Trade.ServerType.Local);
                        Sys_Trade.Instance.IsOpPublicity = true;
                    }


                    break;
                default:
                    break;
            }
        }

        private void OnSearchNtf()
        {
            if (Sys_Trade.Instance.SearchParam.isSearch)
            {
                Sys_Trade.PageType curPageType = Sys_Trade.PageType.Buy;
                if (Sys_Trade.Instance.SearchParam.showType == Packet.TradeShowType.Publicity)
                {
                    curPageType = Sys_Trade.PageType.Publicity;
                }

                if (curPageType == Sys_Trade.PageType.Buy)
                {
                    m_Page.OnSelectPage(curPageType);
                    Sys_Trade.Instance.IsOpBuy = true;
                }
                else if (curPageType == Sys_Trade.PageType.Publicity)
                {
                    m_Page.OnSelectPage(curPageType);
                    Sys_Trade.Instance.IsOpPublicity = true;
                }
            }
        }
    }
}


