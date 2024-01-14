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
    public class UI_Trade_Panel_Publicity_View_Item
    {
        private class Item_Cell 
        {
            private Transform transform;

            private UI_TradeItem m_TradeItem;

            private TradeBrief m_Brief;
            private CSVCommodity.Data m_GoodData;
            private bool m_IsWatch = false;
            private bool m_IsPublicity = false;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TradeItem = new UI_TradeItem();
                m_TradeItem.Init(transform);
                m_TradeItem.BtnBg.onClick.AddListener(OnClickBrief);
                m_TradeItem.BtnWatch.onClick.AddListener(OnClickWatch);
                m_TradeItem.BtnHeart.onClick.AddListener(OnClickHeart);

                m_TradeItem.TextState.gameObject.SetActive(false);
            }

            public void OnClickBrief()
            {
                UpdateInfo(m_Brief);

                if (m_IsPublicity)
                {
                    //公示期最后两分钟,可以参与购买
                    if (m_Brief.OnsaleTime > Sys_Time.Instance.GetServerTime())
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_Publicity_Buy, false, m_Brief);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104404));
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104404));
                    //公示期结束, 出价通知(是为了刷新UI)
                    Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnOfferPriceNtf);
                }
            }

            private void OnClickWatch()
            {
                Sys_Trade.Instance.OnWatchItemReq(m_Brief, !m_IsWatch, Sys_Trade.Instance.CurPublicityServerType == Sys_Trade.ServerType.Cross);
            }

            private void OnClickHeart()
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011205, m_Brief.WatchTimes.ToString()));
            }

            public void UpdateInfo(TradeBrief brief)
            {
                m_Brief = brief;
                if (null == m_Brief)
                {
                    DebugUtil.LogErrorFormat("BoothCell brief is null !!!!!!");
                    return;
                }

                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);
                if (null == m_GoodData)
                {
                    DebugUtil.LogErrorFormat("CSVCommodity not fundId {0} !!!!!!", m_Brief.InfoId);
                    return;
                }

                ItemData item = new ItemData(99, m_Brief.GoodsUid, m_Brief.InfoId, m_Brief.Count, 0, false, false, null, null, 0, null);
                item.SetQuality(m_Brief.Color);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, true, false);
                showItem.SetTradeEnd(true);
                showItem.SetCross(m_Brief.BCross);
                showItem.SetLevel(m_Brief.Petlv);
                m_TradeItem.propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_SellDetail_Sale, showItem));
                m_TradeItem.TextName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

                bool isAssign = Sys_Trade.Instance.IsAssign(m_Brief);
                if(isAssign)
                    isAssign &= Sys_Trade.Instance.IsAssignTime(m_Brief);

                bool isBargin = Sys_Trade.Instance.IsBargin(m_Brief);
                m_IsPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);

                //ImageTip
                m_TradeItem.ImgTip.gameObject.SetActive(false);
                m_TradeItem.TextTip.text = string.Empty;
                if (isAssign) //指定
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011258);
                    //m_TradeItem.ImgTip.gameObject.SetActive(true);
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993410);
                }
                else if (isBargin) //议价
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011257);
                    //m_TradeItem.ImgTip.gameObject.SetActive(true);
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993411);
                }
                else if (m_IsPublicity) //公示
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011259);
                    //m_TradeItem.ImgTip.gameObject.SetActive(true);
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993412);
                }

                //价格
                m_TradeItem.TextPriceInfo.text = "";

                if (isBargin)   //议价
                {
                    m_TradeItem.TextPrice.text = LanguageHelper.GetTextContent(2011090);
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
                m_TradeItem.TextTime.text = "";
                if (m_IsPublicity)
                {
                    m_TradeItem.TextTime.gameObject.SetActive(true);
                    m_TradeItem.TextTime.text = LanguageHelper.TimeToString(m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime(), LanguageHelper.TimeFormat.Type_4);
                }
                //else
                //{
                //    uint saleTime = Sys_Trade.Instance.SaleTime(m_GoodData.treasure == 1);
                //    saleTime *= 86400u;
                //    uint endTime = saleTime + m_Brief.OnsaleTime;
                //    m_TradeItem.TextTime.text = LanguageHelper.TimeToString(m_Brief.OnsaleTime - Framework.TimeManager.GetServerTime(), LanguageHelper.TimeFormat.Type_2);
                //}

                //关注
                if (m_GoodData.attention)
                {
                    m_TradeItem.BtnWatch.gameObject.SetActive(true);

                    m_IsWatch = Sys_Trade.Instance.IsWatch(m_Brief.GoodsUid);
                    m_TradeItem.ImgWatch.enabled = m_IsWatch;

                    m_TradeItem.BtnHeart.gameObject.SetActive(true);
                    m_TradeItem.ImgHeart.fillAmount = Sys_Trade.Instance.CalWatchHeartPercent(m_Brief.WatchTimes);

                    if (m_IsWatch)
                    {
                        m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011260);
                        //m_TradeItem.ImgTip.gameObject.SetActive(true);
                        //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993413);
                    }
                }
                else
                {
                    m_TradeItem.BtnWatch.gameObject.SetActive(false);
                    m_TradeItem.BtnHeart.gameObject.SetActive(false);
                }
                
                //是否已预购
                bool isAdvanceBuy = Sys_Trade.Instance.IsAdvaceBuy(m_Brief.GoodsUid);
                if (isAdvanceBuy)
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011288);
                
                //推荐
                bool isRecommend = Sys_Trade.Instance.IsRecommend(m_Brief.GoodsUid);
                if (isRecommend)
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011298);

                m_TradeItem.goDomestication_0.SetActive(false);
                m_TradeItem.goDomestication_1.SetActive(false);
                m_TradeItem.ShowFx(false);
                //宠物驯化判断
                if (m_GoodData.type == 2u)
                {
                    CSVPetNew.Data petNew = CSVPetNew.Instance.GetConfData(m_Brief.InfoId);
                    if (petNew != null && petNew.mount)
                    {
                        m_TradeItem.goDomestication_0.SetActive(!m_Brief.Domestication);
                        m_TradeItem.goDomestication_1.SetActive(m_Brief.Domestication);
                    }
                    m_TradeItem.ShowFx(m_Brief.LostGrade == 0u);
                }

                if (CheckEquipOrPet())
                    m_TradeItem.SetDetailType(Sys_Trade.DetailSourceType.EquipOrPet);
            }

            /// <summary>
            /// 检测是否高级装备或者高级宠物
            /// </summary>
            /// <returns></returns>
            private bool CheckEquipOrPet()
            {
                return (Sys_Trade.Instance.CurPublicityCategory == 101u
                            || Sys_Trade.Instance.CurPublicityCategory == 102u
                            || Sys_Trade.Instance.CurPublicityCategory == 201u
                            || Sys_Trade.Instance.CurPublicityCategory == 202u
                            || Sys_Trade.Instance.CurPublicityCategory == 123u
                            || Sys_Trade.Instance.CurPublicityCategory == 130u);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<TradeBrief> m_ListBriefs = new List<TradeBrief>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            ProcessEvents(false);
            ProcessEvents(true);
        }

        public void Hide()
        {
            _infinityGrid.Clear();
            transform.gameObject.SetActive(false);
            ProcessEvents(false);
        }

        public void OnDestroy()
        {
            ProcessEvents(false);
        }

        public void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle<TradeBrief>(Sys_Trade.EEvents.OnWatchItemNtf, OnWatchItemNtf, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnAdvancePriceNtf, OnAdvancePriceNtf, register);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Item_Cell entry = new Item_Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Item_Cell entry = cell.mUserData as Item_Cell;
            if (index < m_ListBriefs.Count)
                entry.UpdateInfo(m_ListBriefs[index]);
        }

        private void OnWatchItemNtf(TradeBrief brief)
        {
            for (int i = 0; i < m_ListBriefs.Count; ++i)
            {
                if (m_ListBriefs[i].GoodsUid == brief.GoodsUid)
                    m_ListBriefs[i] = brief;
            }
            _infinityGrid.ForceRefreshActiveCell();
        }

        private void OnAdvancePriceNtf()
        {
            _infinityGrid.ForceRefreshActiveCell();
        }

        public void UpdateInfo()
        {
            m_ListBriefs.Clear();
            m_ListBriefs.AddRange(Sys_Trade.Instance.GetGoodList(true));

            _infinityGrid.CellCount = m_ListBriefs.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}


