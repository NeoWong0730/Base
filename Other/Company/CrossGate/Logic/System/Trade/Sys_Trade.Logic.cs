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

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSelectPage,

            //购买View
            OnViewBuyServerType,
            OnViewBuyCategory,
            OnViewBuySubCategory,
            OnViewBuyBackToSubCategory,
            OnViewBuyListSaleCountNtf,

            //公示View
            OnViewPublicityServerType,
            OnViewPublicityCategory,
            OnViewPublicitySubCategory,
            OnViewPublicityBackToSubCategory,
            OnViewPublicityListSaleCountNtf,

            //出售View
            OnSellCategory,
            OnSellOptionType,
            OnSellDetailType,
            OnSellServerType,

            OnComparePriceNtf,
            OnAssingBuyerNtf,
            OnSaleSuccessNtf,
            OnOffSaleSuccessNtf,
            OnSaleBoothUpdateNtf,

            OnTradeListNtf,
            OnSearchTradeListNtf,

            OnWatchItemNtf,
            OnWatchListNtf,

            OnBuyNtf,
            OnOfferPriceNtf,
            OnAdvancePriceNtf,

            //交易记录
            OnRecordType, //交易记录类型
            OnRecordTreasureType,
            OnRecordNtf, //交易记录数据
            OnRerordUpdateNtf, //交易记录,取回冻结资金
            OnRecordBuyNtf,

            //搜索
            OnSearchPageType,
            OnSelectEquip, //装备搜索, 选中装备
            OnSelectEquipSpecialAttr, //装备搜索，选择特殊属性
            OnSelectEquipBasicAttr, //装备搜索，选择基础属性
            OnSelectEquipAdditionAttr, //装备搜索，选择附加属性，绿字属性

            OnSelectPet, //宠物搜索, 选中宠物
            OnSelectPetQuality, //宠物搜索, 选中宠物资质
            OnSelectPetSkills, //宠物搜索, 选中宠物改造技能
            OnSelectNewPetSkills, //宠物搜索，选中宠物技能

            //元核搜索
            OnSelectCore,
            OnSelectCoreAttr,
            OnSelectCoreSuit,
            OnSelectCoreDress,
            OnSelectCoreEffect,
            
            //饰品搜索
            OnSelectOrnament,
            OnSelectOraExtraAttr,
            
            OnSearchNtf,    //开始搜索

            OnTipsInfo,
        }

        /// <summary>
        /// 操作页签类型
        /// </summary>
        public enum PageType
        {
            None = 0,       //标识交易行没打开过
            Buy = 1,        //购买
            Sell = 2,       //出售
            Publicity = 3,  //公示
        }

        public PageType CurPageType { get; set; } = PageType.None;

        /// <summary>
        /// 服务器类型
        /// </summary>
        public enum ServerType
        {
            Local = 0,  //本服
            Cross = 1,    //跨服
        }

        public void ClearData()
        {
            IsOpBuy = false;
            IsOpPublicity = false;
        }


        /// <summary>
        /// 设置页签类型
        /// </summary>
        /// <param name="pageType"></param>
        public void SetPageType(PageType pageType)
        {
            CurPageType = pageType;
            eventEmitter.Trigger(EEvents.OnSelectPage, CurPageType);
        }

        #region 关注操作
        /// <summary>
        /// 判断商品是否被关注
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public bool IsWatch(ulong uId)
        {
            if (m_WatchGoods != null)
            {
                for (int i = 0; i < m_WatchGoods.Count; ++i)
                {
                    if (uId == m_WatchGoods[i].GoodsUid)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 关注爱心度
        /// </summary>
        /// <param name="watchNum"></param>
        /// <returns></returns>
        public float CalWatchHeartPercent(uint watchNum)
        {
            return watchNum / 10.0f;
        }
        #endregion

        /// <summary>
        /// 获得关注的商品
        /// </summary>
        /// <param name="cross"></param>
        /// <returns></returns>
        public List<TradeBrief> GetWatchItems(bool bCross, PageType pagetType)
        {
            bool isPublicity = pagetType == PageType.Publicity;

            List<TradeBrief> list = new List<TradeBrief>(m_WatchGoods.Count);
            for (int i = 0; i < m_WatchGoods.Count; ++i)
            {
                if (m_WatchGoods[i].BCross == bCross && isPublicity == IsPublicity(m_WatchGoods[i]))
                    list.Add(m_WatchGoods[i]);
            }

            return list;
        }



        #region 状态判断
        /// <summary>
        /// 是否指定
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool IsAssign(TradeBrief brief)
        {
            return brief.TargetId != 0L;
        }

        /// <summary>
        /// 商品实际价格
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public uint RealPrice(TradeBrief brief)
        {
            return brief.TargetId == Sys_Role.Instance.RoleId && IsAssignTime(brief) ? brief.TargetPrice : brief.Price;
        }

        /// <summary>
        /// 指定时间结束
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool IsAssignTime(TradeBrief brief)
        {
            return brief.TargetTime > Sys_Time.Instance.GetServerTime();
        }

        /// <summary>
        /// 是否议价
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool IsBargin(TradeBrief brief)
        {
            return brief.Price == 0u;
        }

        /// <summary>
        /// 是否在公示期
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool IsPublicity(TradeBrief brief)
        {
            if (brief.Price == 0u) //议价
                return false;

            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(brief.InfoId);
            if (goodData != null)
            {
                if (!goodData.publicity)
                    return false;
                else
                    return brief.OnsaleTime > Sys_Time.Instance.GetServerTime();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 是否过期
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool IsOutOfDate(TradeBrief brief)
        {
            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(brief.InfoId);
            if (goodData != null)
            {
                uint saleTime = Sys_Trade.Instance.SaleTime(goodData.treasure);
                saleTime *= 86400u;
                uint endTime = saleTime + brief.OnsaleTime;
                return Sys_Time.Instance.GetServerTime() >= endTime;
            }
            else
            {
                return false;
            }
        }
        #endregion


        /// <summary>
        /// 获得商品列表
        /// </summary>
        /// <returns></returns>
        public CmdTradeListRes GetGoodListRes()
        {
            return _goodListRes;
        }

        private List<TradeBrief> goodList = new List<TradeBrief>(12);

        public void GetGoodPage(ref uint page, ref uint maxPage)
        {
            page = _goodListRes.Page;
            maxPage = _goodListRes.MaxPage;
        }
        
        public List<TradeBrief> GetGoodList(bool publicity)
        {
            goodList.Clear();
            for (int i = 0; i < _goodListRes.Goods.Count; ++i)
            {
                if (IsPublicity(_goodListRes.Goods[i]) == publicity)
                    goodList.Add(_goodListRes.Goods[i]);
            }

            return goodList;
        }

        public bool IsEmptyGood()
        {
            return goodList.Count == 0;
        }

        /// <summary>
        /// 检测是否可以关注商品
        /// </summary>
        /// <param name="brief"></param>
        /// <returns></returns>
        public bool CheckWatchItem(TradeBrief brief)
        {
            if (m_WatchGoods.Count >= GetWatchMaxNum())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011170));
                return false;
            }
            else
            {
                TradeBrief selfBrief = m_SaleGoods.Find(good => good.GoodsUid == brief.GoodsUid);

                if (selfBrief != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011171));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 检测关注完商品提示
        /// </summary>
        /// <param name="brief"></param>
        /// <param name="isWatch"></param>
        public void CheckWatchItemTip(TradeBrief brief, bool isWatch)
        {
            if (isWatch)
            {
                int leftCount = GetWatchMaxNum() - m_WatchGoods.Count;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011172, leftCount.ToString(), brief.WatchTimes.ToString()));
            }
            else
            {
                int leftCount = GetWatchMaxNum() - m_WatchGoods.Count;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011202, leftCount.ToString(), brief.WatchTimes.ToString()));
            }
        }

        /// <summary>
        /// 计算默认选择subclass
        /// </summary>
        /// <param name="data"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public bool CalDefaultSubClass(CSVCommodityCategory.Data data, ref uint subClassId)
        {
            if (data.current_subclass)
            {
                uint subId = Sys_Role.Instance.Role.Level / 10 + 1;
                if (data.subclass != null)
                {
                    if (data.subclass.Contains(subId))
                    {
                        subClassId = subId;
                        return true;
                    }
                    else
                    {
                        if (subId < data.subclass[0])
                        {
                            subClassId = data.subclass[0];
                            return true;
                        }
                        else if (subId > data.subclass[data.subclass.Count - 1])
                        {
                            subClassId = data.subclass[data.subclass.Count - 1];
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #region 上架参数
        public class SaleConfirmParam
        {
            public TradeBrief Brief;
            public uint TargetLeast;
            public bool BReSale = false;
        }
        #endregion

        public uint AdvanceBuyTimes()
        {
            if (m_advanceOfferData != null)
                return m_advanceOfferData.MaxAdvanceOfferTimes - m_advanceOfferData.UsedAdvanceOfferTimes;
            else
                return 0;
        }
    }
}
