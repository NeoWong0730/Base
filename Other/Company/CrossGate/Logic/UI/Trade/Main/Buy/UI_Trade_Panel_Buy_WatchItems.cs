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
    public class UI_Trade_Panel_Buy_WatchItems : UI_Trade_Panel_Buy_WatchItems.View_Bottom.IListner
    {
        public class View_Bottom
        {
            private Transform transform;

            private Button m_BtnRight;
            private Button m_BtnLeft;
            //private Text m_TextPage;
            private UI_Common_Num m_Num;

            private IListner m_Listner;

            public void Init(Transform trans)
            {
                transform = trans;

                m_BtnLeft = transform.GetComponent<Button>();
                m_BtnRight = transform.Find("Button_Right").GetComponent<Button>();
                //m_TextPage = transform.Find("Text").GetComponent<Text>();
                m_Num = new UI_Common_Num();
                m_Num.Init(transform.Find("Image_input"));
                m_Num.RegEnd(OnNumInput);

                m_BtnLeft.onClick.AddListener(OnClickLeft);
                m_BtnRight.onClick.AddListener(OnClickRight);
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClickLeft()
            {
                m_Listner?.OnPageLeft();
            }

            private void OnClickRight()
            {
                m_Listner?.OnPageRight();
            }

            private void OnNumInput(uint num)
            {
                m_Listner?.OnChangePage(num);
            }

            public void Register(IListner listner)
            {
                m_Listner = listner;
            }

            public void SetPage(string page)
            {
                //m_TextPage.text = page;
                m_Num.Dsiplay(page);
            }

            public interface IListner
            {
                void OnPageLeft();
                void OnPageRight();
                void OnChangePage(uint num);
            }
        }

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

            private void OnClickBrief()
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
                        Sys_Trade.Instance.OnDetailInfoReq(m_Brief.BCross, m_Brief.GoodsUid, Sys_Trade.DetailSourceType.None);
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
                m_GoodData = CSVCommodity.Instance.GetConfData(m_Brief.InfoId);

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
                m_TradeItem.propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_SellDetail_Sale, showItem));
                m_TradeItem.TextName.text = LanguageHelper.GetTextContent(item.cSVItemData.name_id);

                //bool isAssign = Sys_Trade.Instance.IsAssign(m_Brief);
                bool isBargin = Sys_Trade.Instance.IsBargin(m_Brief);
                m_IsPublicity = Sys_Trade.Instance.IsPublicity(m_Brief);

                //ImageTip
                m_TradeItem.ImgTip.enabled = false;
                m_TradeItem.TextTip.text = string.Empty;
                m_TradeItem.TextTip.text = LanguageHelper.GetTextContent(2011260);
                //m_TradeItem.ImgTip.enabled = true;
                //ImageHelper.SetIcon(m_TradeItem.ImgTip, 993413);

                //价格
                m_TradeItem.TextPriceInfo.text = "";
                if (isBargin)   //议价
                {
                    m_TradeItem.TextPriceInfo.text = LanguageHelper.GetTextContent(2011090);
                }
                else if (m_GoodData.bulk_sale > 1) //批量
                {
                    m_TradeItem.TextPriceInfo.text = LanguageHelper.GetTextContent(2011010);
                }
                else
                {
                    m_TradeItem.TextPrice.text = Sys_Trade.Instance.RealPrice(m_Brief).ToString();
                }
                //m_TradeItem.TextPrice.text = m_Brief.Price.ToString();

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

                    //if (m_IsWatch)
                    //{
                    //    m_TradeItem.ImgTip.enabled = true;
                    //    ImageHelper.SetIcon(m_TradeItem.ImgTip, 993413);
                    //}
                }
                else
                {
                    m_TradeItem.BtnWatch.gameObject.SetActive(false);
                    m_TradeItem.BtnHeart.gameObject.SetActive(false);
                }

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
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private Lib.Core.CoroutineHandler handler;
        private Dictionary<GameObject, Item_Cell> dicCells = new Dictionary<GameObject, Item_Cell>();

        private View_Bottom _ViewBottom;
        private Transform _transEmpty;

        private List<TradeBrief> _TotalBriefs;
        private List<TradeBrief> _CurBriefs = new List<TradeBrief>();

        private int _CurPage;
        private int _MaxPage;
        private int _NumCount = 8;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.Find("GameObject/Rect").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            //gridGroup = transform.Find("GameObject/Rect/Rectlist").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 12;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform tran = gridGroup.transform.GetChild(i);
            //    Item_Cell cell = new Item_Cell();
            //    cell.Init(tran);
            //    dicCells.Add(tran.gameObject, cell);
            //}

            _ViewBottom = new View_Bottom();
            _ViewBottom.Init(transform.Find("GameObject/Button_Left"));
            _ViewBottom.Register(this);

            _transEmpty = transform.Find("Image_BG");
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            ProcessEvents(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);

            ProcessEvents(false);
        }

        public void ProcessEvents(bool register)
        {
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnViewBuyServerType, OnSelectServer, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnWatchListNtf, OnWatchListNtf, register);
            Sys_Trade.Instance.eventEmitter.Handle(Sys_Trade.EEvents.OnBuyNtf, OnBuyNtf, register);
            Sys_Trade.Instance.eventEmitter.Handle<TradeBrief>(Sys_Trade.EEvents.OnWatchItemNtf, OnWatchItemNtf, register);
        }

        public void OnDestroy()
        {
            ProcessEvents(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Item_Cell entry = new Item_Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Item_Cell entry = cell.mUserData as Item_Cell;
            entry.UpdateInfo(_CurBriefs[index]);
        }

        private void OnWatchItemNtf(TradeBrief brief)
        {
            for (int i = 0; i < _CurBriefs.Count; ++i)
            {
                if (_CurBriefs[i].GoodsUid == brief.GoodsUid)
                    _CurBriefs[i] = brief;
            }
            _infinityGrid.ForceRefreshActiveCell();
        }

        public void OnReqWatchList()
        {
            //请求关注列表
            Sys_Trade.Instance.OnWatchListReq();
            //UpdateInfo();
        }

        private void OnSelectServer()
        {
            OnReqWatchList();
        }

        private void OnBuyNtf()
        {
            OnReqWatchList();
        }

        private void OnWatchListNtf()
        {
            UpdateInfo();
        }

        private void GenItems()
        {
            _CurBriefs.Clear();

            int start = (_CurPage - 1) * _NumCount;
            int end = start + _NumCount;
            for (int i = start; i < end; ++i)
            {
                if (i < _TotalBriefs.Count)
                    _CurBriefs.Add(_TotalBriefs[i]);
            }

            _infinityGrid.CellCount = _CurBriefs.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            _ViewBottom.SetPage(string.Format("{0}/{1}", _CurPage, _MaxPage));
        }

        private void UpdateInfo()
        {
            _TotalBriefs = Sys_Trade.Instance.GetWatchItems(Sys_Trade.Instance.CurBuyServerType == Sys_Trade.ServerType.Cross, Sys_Trade.PageType.Buy);
            _CurPage = 1;
            int count = _TotalBriefs.Count / _NumCount;
            if (_TotalBriefs.Count % _NumCount != 0)
                count++;
            count = count == 0 ? 1 : count;
            _MaxPage = count;
            
            if (_TotalBriefs.Count > 1)
            {
                _TotalBriefs.Sort((d1, d2) =>
                {
                    return d1.Price.CompareTo(d2.Price);
                }); 
            }

            _transEmpty.gameObject.SetActive(_TotalBriefs.Count == 0);

            GenItems();
        }

        public void OnPageLeft()
        {
            int tempPage = _CurPage;
            if (tempPage <= 1)
                tempPage = _MaxPage;
            else
                tempPage--;

            if (tempPage != _CurPage)
            {
                _CurPage = tempPage;
                GenItems();
            }
        }

        public void OnPageRight()
        {
            int tempPage = _CurPage;
            if (tempPage >= _MaxPage)
                tempPage = 1;
            else
                tempPage++;

            if (tempPage != _CurPage)
            {
                _CurPage = tempPage;
                GenItems();
            }
        }

        public void OnChangePage(uint num)
        {
            int tempPage = num <= 1 ? 1 : (int)num;
            tempPage = tempPage >= _MaxPage ? _MaxPage : tempPage;
            if (tempPage != _CurPage)
            {
                _CurPage = tempPage;
                GenItems();
            }
            else
            {
                _ViewBottom.SetPage(string.Format("{0}/{1}", _CurPage, _MaxPage));
            }
        }
    }
}


