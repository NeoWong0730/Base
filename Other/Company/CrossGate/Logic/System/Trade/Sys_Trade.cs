using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {
        private TradeAdvanceOfferData m_advanceOfferData; //预购信息
        private List<TradeBrief> m_SaleGoods = new List<TradeBrief>();          //自己的摊位上架信息
        private List<TradeBrief> m_WatchGoods = new List<TradeBrief>();         //关注的商品信息
        
        private CmdTradeListRes _goodListRes;

        public uint BoothGridCout
        {
            get
            {
                return _boothGridCount + Sys_OperationalActivity.Instance.GetCardTradeGridCount();
            }
        }

        private ulong reqSaleId;

        private bool searchCD = false;
        private Timer searchTimer;
        
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.DataNtf, OnTradeDataNtf, CmdTradeDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.MaxPosNtf, OnTradeMaxPosNtf, CmdTradeMaxPosNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.OnSaleReq, (ushort)CmdTrade.OnSaleRes, OnSaleRes, CmdTradeOnSaleRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.ReSaleReq, (ushort)CmdTrade.ReSaleRes, OnReSaleRes, CmdTradeReSaleRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.BuyReq, (ushort)CmdTrade.BuyRes, OnBuyRes, CmdTradeBuyRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.WatchItemReq, (ushort)CmdTrade.WatchItemRes, OnWatchItemRes, CmdTradeWatchItemRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.OnSaleUpdateNtf, OnSaleUpdateNtf, CmdTradeOnSaleUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.ComparePriceReq, (ushort)CmdTrade.ComparePriceRes, OnComparePriceRes, CmdTradeComparePriceRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.OffSaleReq, (ushort)CmdTrade.OffSaleRes, OnOffSaleRes, CmdTradeOffSaleRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.ListReq, (ushort)CmdTrade.ListRes, OnTradeListRes, CmdTradeListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.SaleCountReq, (ushort)CmdTrade.SaleCountRes, OnSaleCountRes, CmdTradeSaleCountRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.WatchListReq, (ushort)CmdTrade.WatchListRes, OnWatchListRes, CmdTradeWatchListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.GetRecordReq, (ushort)CmdTrade.GetRecordRes, OnTradeRecordRes, CmdTradeGetRecordRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.GetSaleCoinReq, (ushort)CmdTrade.GetSaleCoinRes, OnGetSaleCoinRes, CmdTradeGetSaleCoinRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.DetailInfoReq, (ushort)CmdTrade.DetailInfoRes, OnDetailInfoRes, CmdTradeDetailInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.OfferPriceReq, (ushort)CmdTrade.OfferPriceRes, OnOfferPriceRes, CmdTradeOfferPriceRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.OfferPriceResultNtf, OnOfferPriceResultNtf, CmdTradeOfferPriceResultNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.CheckInfoReq, (ushort)CmdTrade.CheckInfoRes, OnCheckInfoRes, CmdTradeCheckInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.SaleReviewReq, (ushort)CmdTrade.SaleReviewRes, OnSaleReviewRes, CmdTradeSaleReviewRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.RecordUpdateNtf, OnRecordUpdateNtf, CmdTradeRecordUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.AdvanceOfferReq, (ushort)CmdTrade.AdvanceOfferRes, OnAdvanceOffPriceRes, CmdTradeAdvanceOfferRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrade.AdvanceOfferTimesNtf, OnAdvanceOffOfferTimesNtf, CmdTradeAdvanceOfferTimesNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTrade.CancelAdvanceOfferReq, (ushort)CmdTrade.CancelAdvanceOfferRes, OnCancelAdvanceOffPriceRes, CmdTradeCancelAdvanceOfferRes.Parser);

            ParseCateData();
        }

        public override void OnLogin()
        {
            base.OnLogin();

            searchCD = false;
            searchTimer?.Cancel();
            searchTimer = null;
            
            LoadSearchHistory ();
            LoadSaleSuccessInfo();
            LoadTradePrama();
        }

        public override void OnLogout()
        {
            searchCD = false;
            searchTimer?.Cancel();
            searchTimer = null;

            base.OnLogout();
        }

        #region NetMsg
        private void OnTradeDataNtf(NetMsg msg)
        {
            CmdTradeDataNtf ntf = NetMsgUtil.Deserialize<CmdTradeDataNtf>(CmdTradeDataNtf.Parser, msg);

            _appealUrl = ntf.ReviewUrl.ToStringUtf8();
            m_advanceOfferData = ntf.AdvanceOfferData;

            LoadRecordData();
            if (ntf.RecordVer != RecordVer || RecordVer == 0) //版本号为0，也请求一下，这样res就不会为null
            {
                OnTradeRecordReq();
            }

            m_SaleGoods.Clear();
            m_WatchGoods.Clear();

            m_SaleGoods.AddRange(ntf.SaleGoods);

            ResetParamData();
        }

        private void ResetParamData()
        {
            CurPageType = PageType.None;

            CurBuyServerType = ServerType.Local;
            CurBuyCategory = 0u;
            CurBuySubCategory = 0u;

            CurSellServerType = ServerType.Local;
            CurSellCategory = SellCategory.None;
            CurSellDetailType = SellDetailType.None;
            CurSellOpType = SellOpType.None;

            CurPublicityServerType = ServerType.Local;
            CurPublicityCategory = 0u;
            CurPublicitySubCategory = 0u;

            CurSearchPageType = SearchPageType.None;
            SearchParam?.Reset();
            
            ClearSearchEquipData();
            ClearSearchPetData();
            ClearSearchCoreData();
            ClearSearchOraData();
        }

        private void OnTradeMaxPosNtf(NetMsg msg)
        {
            //CmdTradeMaxPosNtf ntf = NetMsgUtil.Deserialize<CmdTradeMaxPosNtf>(CmdTradeMaxPosNtf.Parser, msg);

            ////m_maxPosCount = ntf.MaxPos;
        }

        /// <summary>
        /// 请求上架
        /// </summary>
        /// <param name="cross"></param>
        /// <param name="infoId"></param>
        /// <param name="uId"></param>
        /// <param name="salePrice"></param>
        /// <param name="count"></param>
        /// <param name="discuss"></param>
        /// <param name="targetId"></param>
        /// <param name="targetPrice"></param>
        /// <param name="targetLast"></param>
        public void OnSaleReq(bool cross, uint infoId, ulong uId, uint salePrice, uint count, bool discuss = false, ulong targetId = 0, uint targetPrice = 0, uint targetLast = 0)
        {
            CmdTradeOnSaleReq req = new CmdTradeOnSaleReq();
            req.BCross = cross;
            req.InfoId = infoId;
            req.Uid = uId;
            req.SalePrice = salePrice;
            req.Count = count;
            req.Discuss = discuss;
            req.TargetId = targetId;
            req.TargetPrice = targetPrice;
            req.TargetLast = targetLast;

            reqSaleId = uId;
            NetClient.Instance.SendMessage((ushort)CmdTrade.OnSaleReq, req);
        }

        void OnSaleRes(NetMsg msg)
        {
            CmdTradeOnSaleRes res = NetMsgUtil.Deserialize<CmdTradeOnSaleRes>(CmdTradeOnSaleRes.Parser, msg);

            if (UIManager.IsOpen(EUIID.UI_Trade_SellDetail_Pricing))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_Pricing);

            if (UIManager.IsOpen(EUIID.UI_Trade_Sure_Tip))
                UIManager.CloseUI(EUIID.UI_Trade_Sure_Tip);
            
            eventEmitter.Trigger(EEvents.OnSaleSuccessNtf);
            eventEmitter.Trigger<ulong>(EEvents.OnSaleSuccessNtf, reqSaleId);
            reqSaleId = 0;
            CheckSaleSuccessTip(res.Goods);
        }

        /// <summary>
        /// 重新上架
        /// </summary>
        /// <param name="cross"></param>
        /// <param name="uId"></param>
        /// <param name="salePrice"></param>
        public void OnReSaleReq(bool cross, ulong uId, uint salePrice)
        {
            CmdTradeReSaleReq req = new CmdTradeReSaleReq();
            req.BCross = cross;
            req.GoodsUid = uId;
            req.Price = salePrice;
            reqSaleId = uId;
            NetClient.Instance.SendMessage((ushort)CmdTrade.ReSaleReq, req);
        }

        void OnReSaleRes(NetMsg msg)
        {
            //CmdTradeOnSaleRes res = NetMsgUtil.Deserialize<CmdTradeOnSaleRes>(CmdTradeOnSaleRes.Parser, msg);

            if (UIManager.IsOpen(EUIID.UI_Trade_SellDetail_OutOfDate))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_OutOfDate);

            if (UIManager.IsOpen(EUIID.UI_Trade_Sure_Tip))
                UIManager.CloseUI(EUIID.UI_Trade_Sure_Tip);

            eventEmitter.Trigger(EEvents.OnSaleSuccessNtf);
            eventEmitter.Trigger<ulong>(EEvents.OnSaleSuccessNtf, reqSaleId);
            reqSaleId = 0;
        }

        /// <summary>
        /// 购买商品
        /// </summary>
        public void OnBuyReq(bool cross, uint infoId, ulong uId, uint price, uint count)
        {
            CmdTradeBuyReq req = new CmdTradeBuyReq();
            req.BCross = cross;
            req.InfoId = infoId;
            req.GoodsUid = uId;
            req.Price = price;
            req.Count = count;

            NetClient.Instance.SendMessage((ushort)CmdTrade.BuyReq, req);
        }

        void OnBuyRes(NetMsg msg)
        {
            CmdTradeBuyRes res = NetMsgUtil.Deserialize<CmdTradeBuyRes>(CmdTradeBuyRes.Parser, msg);

            UIManager.CloseUI(EUIID.UI_Trade_Buy);
            eventEmitter.Trigger(EEvents.OnBuyNtf);

            if (res.Ret != 0) //购买失败
            {
                uint languageID = 100000u + (uint)res.Ret;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetErrorCodeContent(languageID));
                return;
            }

            //魔力宝典
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event31);

            if (res.BuyCheap)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011182, res.RealCoin.ToString()));
            }
        }

        public void OnWatchItemReq(TradeBrief brief, bool watch, bool bCross)
        {
            if (watch)
            {
                if (!CheckWatchItem(brief))
                {
                    return;
                }
            }

            CmdTradeWatchItemReq req = new CmdTradeWatchItemReq();
            req.BCross = bCross;
            req.GoodsUid = brief.GoodsUid;
            req.Watch = watch;

            NetClient.Instance.SendMessage((ushort)CmdTrade.WatchItemReq, req);
        }

        void OnWatchItemRes(NetMsg msg)
        {
            CmdTradeWatchItemRes res = NetMsgUtil.Deserialize<CmdTradeWatchItemRes>(CmdTradeWatchItemRes.Parser, msg);

            TradeBrief brief = null;
            if (res.Watch)
            {
                m_WatchGoods.Add(res.Goods);
                brief = res.Goods;
            }
            else
            {
                for (int i = 0; i < m_WatchGoods.Count; ++i)
                {
                    if (m_WatchGoods[i].GoodsUid == res.GoodsUid)
                    {
                        brief = m_WatchGoods[i];
                        break;
                    }
                }

                if (brief != null)
                {
                    brief.WatchTimes--;
                    m_WatchGoods.Remove(brief);
                }
            }

            if (brief != null)
            {
                CheckWatchItemTip(brief, res.Watch);
                eventEmitter.Trigger(EEvents.OnWatchItemNtf, brief);
            }
        }

        void OnSaleUpdateNtf(NetMsg msg)
        {
            CmdTradeOnSaleUpdateNtf res = NetMsgUtil.Deserialize<CmdTradeOnSaleUpdateNtf>(CmdTradeOnSaleUpdateNtf.Parser, msg);
            if (res.Op == CmdTradeOnSaleUpdateNtf.Types.Op.Add)
            {
                m_SaleGoods.Add(res.Goods);
            }
            else if (res.Op == CmdTradeOnSaleUpdateNtf.Types.Op.Remove)
            {
                bool isRemove = false;
                int index = 0;
                for (int i = 0; i < m_SaleGoods.Count; ++i)
                {
                    if (m_SaleGoods[i].GoodsUid == res.Goods.GoodsUid)
                    {
                        isRemove = true;
                        index = i;
                        break;
                    }
                }

                if (isRemove)
                    m_SaleGoods.RemoveAt(index);
            }
            else if (res.Op == CmdTradeOnSaleUpdateNtf.Types.Op.Update)
            {
                for (int i = 0; i < m_SaleGoods.Count; ++i) 
                {
                    if (m_SaleGoods[i].GoodsUid == res.Goods.GoodsUid)
                    {
                        m_SaleGoods[i] = res.Goods;
                        break;
                    }
                }
            }

            eventEmitter.Trigger(EEvents.OnSaleBoothUpdateNtf);
        }

        /// <summary>
        /// 请求商品比价
        /// </summary>
        /// <param name="bCross"></param>
        /// <param name="infoId"></param>
        public void OnComparePriceReq(bool bCross, uint infoId)
        {
            CmdTradeComparePriceReq req = new CmdTradeComparePriceReq();
            req.BCross = bCross;
            req.InfoId = infoId;

            NetClient.Instance.SendMessage((ushort)CmdTrade.ComparePriceReq, req);
        }

        /// <summary>
        /// 商品比价信息返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnComparePriceRes(NetMsg msg)
        {
            CmdTradeComparePriceRes res = NetMsgUtil.Deserialize<CmdTradeComparePriceRes>(CmdTradeComparePriceRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnComparePriceNtf, res);
            //DebugUtil.LogErrorFormat(res.ToString());
        }

        /// <summary>
        /// 商品下架
        /// </summary>
        /// <param name="uId"></param>
        /// <param name="bCross"></param>
        public void OnOffSaleReq(ulong uId, bool bCross = false)
        {
            CmdTradeOffSaleReq req = new CmdTradeOffSaleReq();
            req.BCross = bCross;
            req.GoodsUid = uId;

            NetClient.Instance.SendMessage((ushort)CmdTrade.OffSaleReq, req);
        }

        void OnOffSaleRes(NetMsg msg)
        {
            eventEmitter.Trigger(EEvents.OnOffSaleSuccessNtf);
            //DebugUtil.LogErrorFormat("OnOffSaleRes");

            if (UIManager.IsOpen(EUIID.UI_Trade_SellDetail_Publicity))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_Publicity);

            if (UIManager.IsOpen(EUIID.UI_Trade_SellDetail_Sale))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_Sale);

            if(UIManager.IsOpen(EUIID.UI_Trade_SellDetail_OutOfDate))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_OutOfDate);

            if (UIManager.IsOpen(EUIID.UI_Trade_SellDetail_Bargin))
                UIManager.CloseUI(EUIID.UI_Trade_SellDetail_Bargin);
        }

        /// <summary>
        /// 请求商品分类列表
        /// </summary>
        /// <param name="bCross"></param>
        /// <param name="showType"></param>
        /// <param name="downRank"></param>
        /// <param name="page"></param>
        /// <param name="subCategory"></param>
        /// <param name="subClass"></param>
        public void OnTradeListReq(bool bCross, uint showType, bool downRank, uint page, TradeSearchType searchType, uint infoId, uint subCategory, uint subClass, 
            TradeSearchEquipParam equipParam = null, TradeSearchPetParam petParam = null, TradeSearchPetEquipParam petEquipParam = null, TradeSearchOrnamentParam oraParam = null, ulong goodsUId = 0)
        {
            CmdTradeListReq req = new CmdTradeListReq();
            req.BCross = bCross;
            req.ShowType = showType;
            req.DownRank = downRank;
            req.Page = page;
            req.SearchType = (uint)searchType;
            req.InfoId = infoId;
            req.Category = subCategory;
            req.Subclass = subClass;
            req.EquipParam = equipParam;
            req.PetParam = petParam;
            req.PetEquipParam = petEquipParam;
            req.OrnamentParam = oraParam;
            req.ShareUid = goodsUId;

            NetClient.Instance.SendMessage((ushort)CmdTrade.ListReq, req);
        }

        void OnTradeListRes(NetMsg msg)
        {
            _goodListRes = NetMsgUtil.Deserialize<CmdTradeListRes>(CmdTradeListRes.Parser, msg);
            shareGoodsId = _goodListRes.ShareUid;
            eventEmitter.Trigger(EEvents.OnTradeListNtf);
        }

        private ulong shareGoodsId = 0;
        public bool IsRecommend(ulong goodsId)
        {
            return shareGoodsId == goodsId;
        }

        /// <summary>
        /// //查询某列表具体在售数量
        /// </summary>
        /// <param name="cross"></param>
        /// <param name="listId"></param>
        public void OnSaleCountReq(bool bCross, uint categoryId, bool publicity = false)
        {
            CmdTradeSaleCountReq req = new CmdTradeSaleCountReq();
            req.BCross = bCross;
            req.ListId = categoryId;
            req.Publicity = publicity;

            NetClient.Instance.SendMessage((ushort)CmdTrade.SaleCountReq, req);
        }

        private void OnSaleCountRes(NetMsg msg)
        {
            //DebugUtil.LogErrorFormat("OnSaleCountRes");

            CmdTradeSaleCountRes res = NetMsgUtil.Deserialize<CmdTradeSaleCountRes>(CmdTradeSaleCountRes.Parser, msg);
            this.SaleCountList.Clear();
            this.SaleCountList.AddRange(res.Infos);
            
            if (res.Publicity)
                eventEmitter.Trigger(EEvents.OnViewPublicityListSaleCountNtf);
            else
                eventEmitter.Trigger(EEvents.OnViewBuyListSaleCountNtf);
        }
        
        private  List<CmdTradeSaleCountRes.Types.Info> SaleCountList = new List<CmdTradeSaleCountRes.Types.Info>();
        public List<CmdTradeSaleCountRes.Types.Info> GetSaleCountList()
        {
            return this.SaleCountList;
        }

        public void OnWatchListReq()
        {
            CmdTradeWatchListReq req = new CmdTradeWatchListReq();
            NetClient.Instance.SendMessage((ushort)CmdTrade.WatchListReq, req);
        }

        private void OnWatchListRes(NetMsg msg)
        {
            CmdTradeWatchListRes res = NetMsgUtil.Deserialize<CmdTradeWatchListRes>(CmdTradeWatchListRes.Parser, msg);
            m_WatchGoods.Clear();
            m_WatchGoods.AddRange(res.WatchGoods);
            
            eventEmitter.Trigger(EEvents.OnWatchListNtf);
        }

        public void OnTradeRecordReq()
        {
            CmdTradeGetRecordReq req = new CmdTradeGetRecordReq();
            NetClient.Instance.SendMessage((ushort)CmdTrade.GetRecordReq, req);
        }

        /// <summary>
        /// 交易记录response
        /// </summary>
        /// <param name="msg"></param>
        private void OnTradeRecordRes(NetMsg msg)
        {
            _recordRes = NetMsgUtil.Deserialize<CmdTradeGetRecordRes>(CmdTradeGetRecordRes.Parser, msg);
            SaveRecordData();
        }

        /// <summary>
        /// 出售记录，取回冻结资金
        /// </summary>
        /// <param name="dealId"></param>
        public void OnGetSaleCoinReq(ulong dealId)
        {
            CmdTradeGetSaleCoinReq req = new CmdTradeGetSaleCoinReq();
            req.DealId = dealId;
            NetClient.Instance.SendMessage((ushort)CmdTrade.GetSaleCoinReq, req);
        }

        /// <summary>
        /// 取回冻结资金
        /// </summary>
        /// <param name="msg"></param>
        private void OnGetSaleCoinRes(NetMsg msg)
        {
            //CmdTradeGetSaleCoinRes res = NetMsgUtil.Deserialize<CmdTradeGetSaleCoinRes>(CmdTradeGetSaleCoinRes.Parser, msg);

            //TradeSaleRecord saleRecord = null;
            //foreach (var data in _recordRes.SaleRecord)
            //{
            //    if (data.DealId == res.DealId)
            //    {
            //        data.CheckStatus = res.CheckStatus;
            //        data.ReceiveStep = res.ReceiveStep;
            //        saleRecord = data;
            //        break;
            //    }
            //}

            //if (saleRecord != null)
            //    eventEmitter.Trigger(EEvents.OnGetSaleCoin, saleRecord);
        }


        /// <summary>
        /// 请求详细信息的来源类型
        /// </summary>
        public enum DetailSourceType
        {
            None,
            EquipOrPet,
        }

        /// <summary>
        /// 请求商品详细信息
        /// </summary>
        /// <param name="bCross"></param>
        /// <param name="uId"></param>
        //bool isShowTips = false;
        private bool _detailCross = false;
        private DetailSourceType _detailSrcType;
        public void OnDetailInfoReq(bool bCross, ulong uId, DetailSourceType srcType)
        {
            _detailCross = bCross;
            _detailSrcType = srcType;
            //isShowTips = showTip;

            CmdTradeDetailInfoReq req = new CmdTradeDetailInfoReq();
            req.BCross = bCross;
            req.GoodsUid = uId;

            NetClient.Instance.SendMessage((ushort)CmdTrade.DetailInfoReq, req);
        }

        private void OnDetailInfoRes(NetMsg msg)
        {
            CmdTradeDetailInfoRes res = NetMsgUtil.Deserialize<CmdTradeDetailInfoRes>(CmdTradeDetailInfoRes.Parser, msg);

            TradeMsgBoxParam param = new TradeMsgBoxParam();
            param.bCross = _detailCross;
            param.srcType = _detailSrcType;
            param.tradeItem = res.Goods;

            if (_detailSrcType == DetailSourceType.EquipOrPet)
            {
                param.idlist = new List<ulong>();
                foreach (var data in _goodListRes.Goods)
                    param.idlist.Add(data.GoodsUid);
            }

            //DebugUtil.LogError(_detailSrcType.ToString());

            if (res.Goods.GoodsType == (uint)TradeGoodsType.Pet)
            {
                if (UIManager.IsOpen(EUIID.UI_Tips_Pet))
                {
                    Sys_Trade.Instance.eventEmitter.Trigger(EEvents.OnTipsInfo, param);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Tips_Pet, false, param);
                }
            }
            else
            {
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(res.Goods.InfoId);
                if (itemData.type_id == (uint)EItemType.Equipment)
                {
                    if (UIManager.IsOpen(EUIID.UI_Trade_Box_Equip))
                    {
                        Sys_Trade.Instance.eventEmitter.Trigger(EEvents.OnTipsInfo, param);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_Box_Equip, false, param);
                    }
                }
                else if (itemData.type_id == (uint)EItemType.Ornament)
                {
                    if (UIManager.IsOpen(EUIID.UI_Trade_Box_Ornament))
                    {
                        Sys_Trade.Instance.eventEmitter.Trigger(EEvents.OnTipsInfo, param);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_Box_Ornament, false, param);
                    }
                }
                else if (itemData.type_id == (uint) EItemType.PetEquipment)
                {
                    if (UIManager.IsOpen(EUIID.UI_Trade_Box_PetEquip))
                    {
                        Sys_Trade.Instance.eventEmitter.Trigger(EEvents.OnTipsInfo, param);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_Trade_Box_PetEquip, false, param);
                    }
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Trade_Box_Com, false, param);
                }
            }
        }

        /// <summary>
        /// 公示期竞价
        /// </summary>
        public void OnOfferPriceReq(bool cross, uint infoId, ulong uId, uint price, bool offer)
        {
            CmdTradeOfferPriceReq req = new CmdTradeOfferPriceReq();
            req.BCross = cross;
            req.InfoId = infoId;
            req.GoodsUid = uId;
            req.Price = price;
            req.Offer = offer;

            NetClient.Instance.SendMessage((ushort)CmdTrade.OfferPriceReq, req);
        }

        private void OnOfferPriceRes(NetMsg msg)
        {
            CmdTradeOfferPriceRes res = NetMsgUtil.Deserialize<CmdTradeOfferPriceRes>(CmdTradeOfferPriceRes.Parser, msg);
            if (res.Ret != 0)
            {
                UIManager.CloseUI(EUIID.UI_Trade_Publicity_Buy);
                uint languageID = 100000u + (uint)res.Ret;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetErrorCodeContent(languageID));
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011222));
                eventEmitter.Trigger(EEvents.OnOfferPriceNtf);
            }
        }

        private void OnOfferPriceResultNtf(NetMsg msg)
        {
            CmdTradeOfferPriceResultNtf res = NetMsgUtil.Deserialize<CmdTradeOfferPriceResultNtf>(CmdTradeOfferPriceResultNtf.Parser, msg);
            if (!res.Success)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011184));
            //else
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent());
            eventEmitter.Trigger(EEvents.OnOfferPriceNtf);
        }

        public void OnAdvanceOffPriceReq(ulong uId, uint price)
        {
            CmdTradeAdvanceOfferReq req = new CmdTradeAdvanceOfferReq();
            req.GoodsUid = uId;
            req.Price = price;
            NetClient.Instance.SendMessage((ushort)CmdTrade.AdvanceOfferReq, req);
        }

        private void OnAdvanceOffPriceRes(NetMsg msg)
        {
            CmdTradeAdvanceOfferRes res = NetMsgUtil.Deserialize<CmdTradeAdvanceOfferRes>(CmdTradeAdvanceOfferRes.Parser, msg);
            if (res.Ret != 0)
            {
                uint errorCode = (uint)res.Ret;
                //TODO : 错误表文本最好和游戏文本分开
                uint languageID = 100000u + errorCode;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetErrorCodeContent(languageID));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011274));
                eventEmitter.Trigger(EEvents.OnAdvancePriceNtf);
            }
        }

        public void OnCancelAdvanceOffPriceReq(uint recordId)
        {
            CmdTradeCancelAdvanceOfferReq req = new CmdTradeCancelAdvanceOfferReq();
            req.RecordUid = recordId;
            NetClient.Instance.SendMessage((ushort)CmdTrade.CancelAdvanceOfferReq, req);
        }

        private void OnCancelAdvanceOffPriceRes(NetMsg msg)
        {
            CmdTradeCancelAdvanceOfferRes res = NetMsgUtil.Deserialize<CmdTradeCancelAdvanceOfferRes>(CmdTradeCancelAdvanceOfferRes.Parser, msg);
        }

        private void OnAdvanceOffOfferTimesNtf(NetMsg msg)
        {
            CmdTradeAdvanceOfferTimesNtf res = NetMsgUtil.Deserialize<CmdTradeAdvanceOfferTimesNtf>(CmdTradeAdvanceOfferTimesNtf.Parser, msg);
            m_advanceOfferData = res.AdvanceOfferData;
        }

        public void OnCheckInfoReq()
        {
            CmdTradeCheckInfoReq req = new CmdTradeCheckInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdTrade.CheckInfoReq, req);
        }

        public bool IsCheckInfo = false;
        private uint limitEndTime = 0u;
        private void OnCheckInfoRes(NetMsg msg)
        {
            CmdTradeCheckInfoRes res = NetMsgUtil.Deserialize<CmdTradeCheckInfoRes>(CmdTradeCheckInfoRes.Parser, msg);
            IsCheckInfo = res.HasCheckGoods;
            limitEndTime = res.TradeLimitEndTime;
        }


        /// <summary>
        /// 是否限制交易
        /// </summary>
        /// <returns></returns>
        public bool IsTradeLimit()
        {
            if (limitEndTime > Sys_Time.Instance.GetServerTime())
                return true;
            return false;
        }

        public string GetLimitTime()
        {
            System.Text.StringBuilder sb = StringBuilderPool.GetTemporary();
            //string strTime = string.Empty;

            System.DateTime dateTime = Sys_Time.ConvertToLocalTime(limitEndTime);
            sb.Append(dateTime.Year.ToString()).Append("-");
            sb.Append(dateTime.Month.ToString("d2")).Append("-");
            sb.Append(dateTime.Day.ToString("d2")).Append(" ");
            sb.Append(dateTime.Hour.ToString("d2")).Append(":");
            sb.Append(dateTime.Minute.ToString("d2"));

            return StringBuilderPool.ReleaseTemporaryAndToString(sb);
        }

        public bool IsCrossServer()
        {
            if (CurPageType == PageType.Buy)
            {
                return CurBuyServerType == ServerType.Cross;
            }
            else if (CurPageType == PageType.Publicity)
            {
                return CurPublicityServerType == ServerType.Cross;
            }

            return false;
        }

        #endregion
    }
}
