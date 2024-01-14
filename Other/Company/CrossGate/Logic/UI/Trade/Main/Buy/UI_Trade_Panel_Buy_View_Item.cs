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
    public class UI_Trade_Panel_Buy_View_Item
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
                if (m_IsPublicity)
                {
                    //公示期最后两分钟,可以参与购买
                    if (m_Brief.OnsaleTime > Sys_Time.Instance.GetServerTime())
                    {
                        uint leftTime = m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime();
                        if (leftTime < Sys_Trade.Instance.PublicityRemainTime())
                        {
                            UIManager.OpenUI(EUIID.UI_Trade_Publicity_Buy, false, m_Brief);
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104404));
                    }
                }
                else
                {
                    bool isBargin = Sys_Trade.Instance.IsBargin(m_Brief);
                    if (isBargin)
                    {
                        Sys_Trade.DetailSourceType srcType = Sys_Trade.DetailSourceType.None;
                        if (CheckEquipOrPet())
                            srcType = Sys_Trade.DetailSourceType.EquipOrPet;

                        Sys_Trade.Instance.OnDetailInfoReq(m_Brief.BCross, m_Brief.GoodsUid, srcType);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_Buy, false, m_Brief);
                    }
                }
            }

            private void OnClickWatch()
            {
                Sys_Trade.Instance.OnWatchItemReq(m_Brief, !m_IsWatch, Sys_Trade.Instance.CurBuyServerType == Sys_Trade.ServerType.Cross);
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
                    m_TradeItem.TextTime.text = LanguageHelper.TimeToString(m_Brief.OnsaleTime - Sys_Time.Instance.GetServerTime(), LanguageHelper.TimeFormat.Type_2);
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
                        //m_TradeItem.ImgTip.gameObject.SetActive(true);
                        m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011260);
                        //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993413);
                    }
                }
                else
                {
                    m_TradeItem.BtnWatch.gameObject.SetActive(false);
                    m_TradeItem.BtnHeart.gameObject.SetActive(false);
                }
                
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
                return (Sys_Trade.Instance.CurBuyCategory == 101u
                          || Sys_Trade.Instance.CurBuyCategory == 102u
                          || Sys_Trade.Instance.CurBuyCategory == 201u
                          || Sys_Trade.Instance.CurBuyCategory == 202u
                          || Sys_Trade.Instance.CurBuyCategory == 123u
                          || Sys_Trade.Instance.CurBuyCategory == 130u);
            }
        }

        private Transform transform;

        //private InfinityGridLayoutGroup gridGroup;
        //private int visualGridCount;
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
            ClearDatas();
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

        public void ClearDatas()
        {
            _infinityGrid.Clear();
        }

        public void UpdateInfo()
        {
            m_ListBriefs.Clear();
            m_ListBriefs.AddRange(Sys_Trade.Instance.GetGoodList(false));
            _infinityGrid.CellCount = m_ListBriefs.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}


