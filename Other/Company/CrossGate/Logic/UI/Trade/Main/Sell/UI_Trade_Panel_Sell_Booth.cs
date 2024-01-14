using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Panel_Sell_Booth
    {
        private class BoothCell
        {
            private Transform transform;

            private Transform transEmpty;
            private UI_TradeItem m_TradeItem;

            private int m_Index;
            private TradeBrief m_Brief;
            private CSVCommodity.Data m_GoodData;

            private bool m_IsAssign;
            private bool m_IsBargin;
            private bool m_IsPublicity;
            private bool m_IsOutOfDate;

            public void Init(Transform trans)
            {
                transform = trans;

                transEmpty = transform.Find("Empty");

                m_TradeItem = new UI_TradeItem();
                m_TradeItem.Init(transform.Find("TradeItem"));
                m_TradeItem.BtnBg.onClick.AddListener(OnClick);
                m_TradeItem.BtnTip.onClick.AddListener(OnClickAssignInfo);
            }

            private void OnClick()
            {
                //点击查看的时候，刷新显示（主要是时间）
                UpdateInfo(m_Index);

                if (m_IsPublicity)
                {
                    UIManager.OpenUI(EUIID.UI_Trade_SellDetail_Publicity, false, m_Brief);
                }
                else
                {
                    if (m_IsBargin)
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_SellDetail_Bargin, false, m_Brief);
                    }
                    else
                    {
                        if (m_IsOutOfDate)
                        {
                            UIManager.OpenUI(EUIID.UI_Trade_SellDetail_OutOfDate, false, m_Brief);
                        }
                        else
                        {
                            UIManager.OpenUI(EUIID.UI_Trade_SellDetail_Sale, false, m_Brief);
                        }
                    }
                }
            }

            private void OnClickAssignInfo()
            {
                UIManager.OpenUI(EUIID.UI_Trade_Assign_Info, false, m_Brief);
            }

            public void UpdateInfo(int index)
            {
                m_Index = index;
                m_Brief = Sys_Trade.Instance.GetSaleBoothBrief(index);
                if (null == m_Brief)
                {
                    transEmpty.gameObject.SetActive(true);
                    m_TradeItem.transform.gameObject.SetActive(false);
                    //DebugUtil.LogErrorFormat("BoothCell brief is null !!!!!!");
                    return;
                }

                transEmpty.gameObject.SetActive(false);
                m_TradeItem.transform.gameObject.SetActive(true);
                m_TradeItem.ImgCrossTip.gameObject.SetActive(m_Brief.BCross);

                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
                if (null == m_GoodData)
                {
                    DebugUtil.LogErrorFormat("CSVCommodity not fundId {0} !!!!!!", m_Brief.InfoId);
                    return;
                }

                bool isShowCount = m_GoodData.bulk_sale > 1;

                ItemData item = new ItemData(99, m_Brief.GoodsUid, m_Brief.InfoId, m_Brief.Count, 0, false, false, null, null, 0, null);
                item.SetQuality(m_Brief.Color);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, isShowCount, false);
                showItem.SetTradeEnd(true);
                showItem.SetCross(m_Brief.BCross);
                showItem.SetLevel(m_Brief.Petlv);
                m_TradeItem.propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_SellDetail_Sale, showItem));
                m_TradeItem.TextName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

                m_IsAssign = Sys_Trade.Instance.IsAssign(m_Brief);
                if (m_IsAssign)
                    m_IsAssign &= Sys_Trade.Instance.IsAssignTime(m_Brief);

                m_IsBargin = Sys_Trade.Instance.IsBargin(m_Brief);
                m_IsPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);
                m_IsOutOfDate = Sys_Trade.Instance.IsOutOfDate(m_Brief);

                m_TradeItem.BtnTip.gameObject.SetActive(m_IsAssign && !m_IsOutOfDate);

                //ImageTip
                m_TradeItem.ImgTip.enabled = false;
                m_TradeItem.TextTip.text = string.Empty;
                if (m_IsAssign) //指定
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011258);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993410);
                }
                else if (m_IsBargin) //议价
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011257);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993411);
                }
                else if (m_IsPublicity) //公示
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011259);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993412);
                }

                //价格
                m_TradeItem.TextPriceInfo.text = "";
                if (m_IsAssign) //指定
                {
                    m_TradeItem.TextPrice.text = Sys_Trade.Instance.RealPrice(m_Brief).ToString();
                }
                else if (m_IsBargin)   //议价
                {
                    m_TradeItem.TextPrice.text = LanguageHelper.GetTextContent(2011090);
                    //m_TextPriceInfo.text = LanguageHelper.GetTextContent(2011090);
                }
                else if (m_GoodData.bulk_sale > 1) //批量
                {
                    m_TradeItem.TextPriceInfo.text = LanguageHelper.GetTextContent(2011010);
                    m_TradeItem.TextPrice.text = m_Brief.Price.ToString();
                }
                else
                {
                    m_TradeItem.TextPrice.text = Sys_Trade.Instance.RealPrice(m_Brief).ToString();
                }
                
                //剩余时间
                if (m_IsPublicity)
                {
                    m_TradeItem.TextState.text = "";
                    m_TradeItem.TextTime.text = "";
                }
                else
                {
                    if (m_IsOutOfDate)
                    {
                        m_TradeItem.TextState.text = "";
                        m_TradeItem.TextTime.text = LanguageHelper.GetTextContent(2011103);
                    }
                    else
                    {
                        if (m_IsBargin)
                            m_TradeItem.TextState.text = LanguageHelper.GetTextContent(2011043);
                        else
                            m_TradeItem.TextState.text = LanguageHelper.GetTextContent(2011101);

                        uint saleTime = Sys_Trade.Instance.SaleTime(m_GoodData.treasure);
                        saleTime *= 86400u;
                        uint endTime = saleTime + m_Brief.OnsaleTime;
                        m_TradeItem.TextTime.text = LanguageHelper.TimeToString(endTime - Framework.TimeManager.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                    }
                }

                m_TradeItem.goDomestication_0.SetActive(false);
                m_TradeItem.goDomestication_1.SetActive(false);
                //宠物驯化判断
                if (m_GoodData.type == 2u)
                {
                    CSVPetNew.Data petNew = CSVPetNew.Instance.GetConfData(m_Brief.InfoId);
                    if (petNew != null && petNew.mount)
                    {
                        m_TradeItem.goDomestication_0.SetActive(!m_Brief.Domestication);
                        m_TradeItem.goDomestication_1.SetActive(m_Brief.Domestication);
                    }
                }
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private Text _textTitle;

        private Transform _transFreeze;
        private Text _textFreeze;
        private Button _btnFreeze;

        private Transform _transBottom;
        private Text _textBottom;
        private Button _btnBottomDetail;
        
        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("Rect").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            _textTitle = transform.Find("Text_Title").GetComponent<Text>();

            _transFreeze = transform.Find("Image_Freeze");
            _transFreeze.gameObject.SetActive(false);
            _textFreeze = transform.Find("Image_Freeze/Text").GetComponent<Text>();
            _btnFreeze = transform.Find("Image_Freeze/Text_Button").GetComponent<Button>();
            _btnFreeze.onClick.AddListener(OnClickFreeze);

            _transBottom = transform.Find("Image_Bottom");
            _transBottom.gameObject.SetActive(false);
            _textBottom = transform.Find("Image_Bottom/Text").GetComponent<Text>();
            _btnBottomDetail = transform.Find("Image_Bottom/Text_Detail").GetComponent<Button>();
            _btnBottomDetail.onClick.AddListener(OnClickDetail);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            BoothCell entry = new BoothCell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            BoothCell entry = cell.mUserData as BoothCell;
            entry.UpdateInfo(index);
        }

        private void OnClickDetail()
        {
            UIManager.OpenUI(EUIID.UI_Trade_Record);
            //SDKManager.SDKOpenUserCenter();
        }

        private void OnClickFreeze()
        {
            SDKManager.SDKOpenUserCenter();
        }

        public void UpdateInfo()
        {
            int totalCount = Sys_Trade.Instance.GetSaleBoothTotalCount();
            _infinityGrid.CellCount = totalCount;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            int count = Sys_Trade.Instance.GetSaleBoothCount();
            _textTitle.text = LanguageHelper.GetTextContent(2011100, count.ToString(), Sys_Trade.Instance.BoothGridCout.ToString());

            _transBottom.gameObject.SetActive(Sys_Trade.Instance.IsCheckInfo);

            bool isLimit = Sys_Trade.Instance.IsTradeLimit();
            _transFreeze.gameObject.SetActive(isLimit);
            if (isLimit)
                _textFreeze.text = LanguageHelper.GetTextContent(2011131, Sys_Trade.Instance.GetLimitTime());
        }
    }
}


