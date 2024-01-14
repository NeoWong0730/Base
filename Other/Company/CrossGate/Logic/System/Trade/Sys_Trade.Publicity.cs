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
        public bool IsOpPublicity = false;

        /// <summary>
        /// 公示界面的服务器类型
        /// </summary>
        public ServerType CurPublicityServerType { get; set; } = ServerType.Local;

        /// <summary>
        /// 公示界面大的分类
        /// </summary>
        public uint CurPublicityCategory = 0u;

        /// <summary>
        /// 公示界面子分类
        /// </summary>
        public uint CurPublicitySubCategory = 0u;

        /// <summary>
        /// 设置公示服务器类型
        /// </summary>
        /// <param name="serverType"></param>
        public void SetPublicityServerType(ServerType serverType)
        {
            CurPublicityServerType = serverType;
            eventEmitter.Trigger(EEvents.OnViewPublicityServerType);
        }

        /// <summary>
        /// 获得公示商品大的分类
        /// </summary>
        /// <returns></returns>
        public List<uint> GetPublicityCategoryList()
        {
            uint tradeId = (uint)CurPublicityServerType + 1;

            List<uint> list = new List<uint>();
            foreach (var data in CSVCommodityList.Instance.GetAll())
            {
                if (data.trade_ID == tradeId)
                {
                    if (data.show)
                        list.Add(data.id);
                }
            }

            return list;
        }

        /// <summary>
        /// 设置公示商品大分类
        /// </summary>
        /// <param name="category"></param>
        public void SetPublicityCatergory(uint category)
        {
            CurPublicityCategory = category;
            eventEmitter.Trigger(EEvents.OnViewPublicityCategory, CurPublicityCategory);

            if (Sys_Trade.Instance.SearchParam.isSearch && Sys_Trade.Instance.SearchParam.showType == TradeShowType.Publicity)
            {
                if (CurPublicityCategory != Sys_Trade.Instance.SearchParam.Category)
                    Sys_Trade.Instance.SearchParam.Reset();
                else
                    Sys_Trade.Instance.SearchParam.OnClick();
            }
        }

        /// <summary>
        /// 设置公示商品子分类
        /// </summary>
        /// <param name="subCategory"></param>
        public void SetPublicitySubCategory(uint subCategory)
        {
            CurPublicitySubCategory = subCategory;
            eventEmitter.Trigger(EEvents.OnViewPublicitySubCategory, CurPublicitySubCategory);
        }
    }
}
