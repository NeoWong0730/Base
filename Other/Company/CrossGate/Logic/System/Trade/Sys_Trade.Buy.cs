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
        public bool IsOpBuy = false;

        /// <summary>
        /// 购买界面的服务器类型
        /// </summary>
        public ServerType CurBuyServerType { get; set; } = ServerType.Local;

        /// <summary>
        /// 购买界面大的分类
        /// </summary>
        public uint CurBuyCategory = 0u;

        /// <summary>
        /// 购买界面子分类
        /// </summary>
        public uint CurBuySubCategory = 0u;

        /// <summary>
        /// 设置购买服务器类型
        /// </summary>
        /// <param name="serverType"></param>
        public void SetBuyServerType(ServerType serverType)
        {
            CurBuyServerType = serverType;
            eventEmitter.Trigger(EEvents.OnViewBuyServerType);
        }

        /// <summary>
        /// 获得购买界面商品大的分类
        /// </summary>
        /// <returns></returns>
        public List<uint> GetBuyCategoryList()
        {
            uint tradeId = (uint)CurBuyServerType + 1;
            List<uint> list = new List<uint>();
            foreach (var data in CSVCommodityList.Instance.GetAll())
            {
                if (data.trade_ID == tradeId)
                {
                    list.Add(data.id);
                }
            }

            return list;
        }

        /// <summary>
        /// 设置购买商品大分类
        /// </summary>
        /// <param name="category"></param>
        public void SetBuyCatergory(uint category)
        {
            CurBuyCategory = category;
            eventEmitter.Trigger(EEvents.OnViewBuyCategory, CurBuyCategory);

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType != TradeShowType.Publicity)
            {
                if (CurBuyCategory != Sys_Trade.Instance.SearchParam.Category)
                    Sys_Trade.Instance.SearchParam.Reset();
                else
                    Sys_Trade.Instance.SearchParam.OnClick();
            }
        }

        /// <summary>
        /// 设置购买商品子分类
        /// </summary>
        /// <param name="subCategory"></param>
        public void SetBuySubCategory(uint subCategory)
        {
            CurBuySubCategory = subCategory;
            eventEmitter.Trigger(EEvents.OnViewBuySubCategory, CurBuySubCategory);
        }
    }
}
