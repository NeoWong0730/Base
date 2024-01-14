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
    public class UI_Trade_SellDetail_Publicity_List 
    {
        private class Cell
        {
            private Transform transform;

            private UI_TradeItem m_TradeItem;

            private TradeBrief m_Brief;
            private CSVCommodity.Data m_GoodData;

            public void Init(Transform trans)
            {
                transform = trans;

                m_TradeItem = new UI_TradeItem();
                m_TradeItem.Init(transform);
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

                bool isShowCount = m_GoodData.bulk_sale > 1;

                ItemData item = new ItemData(99, m_Brief.GoodsUid, m_Brief.InfoId, m_Brief.Count, 0, false, false, null, null, 0, null);
                item.SetQuality(m_Brief.Color);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, isShowCount, false);
                showItem.SetTradeEnd(true);
                showItem.SetCross(m_Brief.BCross);
                showItem.SetLevel(m_Brief.Petlv);
                m_TradeItem.propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_SellDetail_Publicity, showItem));
                m_TradeItem.TextName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);
                //m_TradeItem.ProItem.SetActive(false);
                //m_TradeItem.PetItem.SetActive(false);

                //if (m_GoodData.type == (uint)Sys_Trade.ETradeType.Item)
                //{
                //    m_TradeItem.ProItem.SetActive(true);

                //    bool isShowCount = m_GoodData.bulk_sale > 1;
                //    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(m_GoodData.id, m_Brief.Count, true, false, false, false, false, isShowCount, false, true);
                //    showItem.SetQuality(m_Brief.Color);
                //    showItem.SetMarketEnd(true);
                //    m_TradeItem.ProItem.SetData(new MessageBoxEvt(EUIID.UI_Trade, showItem));

                //    m_TradeItem.TextName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(m_Brief.InfoId).name_id);
                //}
                //else if (m_GoodData.type == (uint)Sys_Trade.ETradeType.Pet)
                //{
                //    m_TradeItem.PetItem.SetActive(true);

                //    CSVPet.Data petData = CSVPet.Instance.GetConfData(m_GoodData.id);
                //    m_TradeItem.PetItem.SetData(petData);
                //    m_TradeItem.PetItem.TextLevel.text = m_Brief.Petlv.ToString();

                //    m_TradeItem.TextName.text = LanguageHelper.GetTextContent(petData.name);
                //}

                bool isAssign = Sys_Trade.Instance.IsAssign(m_Brief);
                if (isAssign)
                    isAssign &= Sys_Trade.Instance.IsAssignTime(m_Brief);

                bool isBargin = Sys_Trade.Instance.IsBargin(m_Brief);
                bool isPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);
                bool isOutOfDate = Sys_Trade.Instance.IsOutOfDate(m_Brief);

                //ImageTip
                m_TradeItem.ImgTip.enabled = false;
                m_TradeItem.TextTip.text = string.Empty;
                if (isAssign) //指定
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011258);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993410);
                }
                else if (isBargin) //议价
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011257);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993411);
                }
                else if (isPublicity) //公示
                {
                    m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011259);
                    //m_TradeItem.ImgTip.enabled = true;
                    //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993412);
                }

                //价格
                m_TradeItem.TextPriceInfo.text = "";
                if (isAssign) //指定
                {
                    m_TradeItem.TextPrice.text = Sys_Trade.Instance.RealPrice(m_Brief).ToString();
                }
                else if (isBargin)   //议价
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

                if (m_GoodData.attention)
                {
                    m_TradeItem.TextWatchNum.gameObject.SetActive(true);
                    m_TradeItem.TextWatchNum.text = LanguageHelper.GetTextContent(2011081, m_Brief.WatchTimes.ToString());
                }
                else
                {
                    m_TradeItem.TextWatchNum.gameObject.SetActive(false);
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

        //private InfinityGridLayoutGroup gridGroup;
        //private int visualGridCount;
        //private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();
        private InfinityGrid _infinityGrid;

        private List<TradeBrief> m_ListBrief = new List<TradeBrief>();
        
        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            //gridGroup = transform.Find("Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 4;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform tran = gridGroup.transform.GetChild(i);
            //    Cell cell = new Cell();
            //    cell.Init(tran);
            //    dicCells.Add(tran.gameObject, cell);
            //}
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Cell entry = new Cell();
            entry.Init(cell.mRootTransform);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            entry.UpdateInfo(m_ListBrief[index]);
        }

        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= visualGridCount)
        //        return;

        //    if (dicCells.ContainsKey(trans.gameObject))
        //    {
        //        Cell cell = dicCells[trans.gameObject];
        //        cell.UpdateInfo(m_ListBrief[index]);
        //    }
        //}

        public void OnUpdateListInfos(List<TradeBrief> breifs)
        {
            m_ListBrief = breifs;

            _infinityGrid.CellCount = m_ListBrief.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
            //visualGridCount = m_ListBrief.Count;
            //gridGroup.SetAmount(visualGridCount);
        }
    }
}


