using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using Table;
using System.Json;

namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {
        /// <summary>
        /// 出售类型分类
        /// </summary>
        public enum SellCategory
        {
            None = 0,
            Item = 1,
            Pet = 2,
        }
        public SellCategory CurSellCategory { get; set; } = SellCategory.None;

        /// <summary>
        /// 商品出售选择的服务器
        /// </summary>
        public ServerType CurSellServerType { get; set; } = ServerType.Local;

        /// <summary>
        /// 出售操作类型(普通或指定)
        /// </summary>
        public enum SellOpType
        {
            None = 0,
            Common = 1, //普通
            Assign = 2, //指定
        }

        public SellOpType CurSellOpType { get; set; } = SellOpType.None;

        /// <summary>
        /// 出售详情分类
        /// </summary>
        public enum SellDetailType
        {
            None = 0,
            Selling = 1,    //在售
            Publicity = 2,  //公示
        }
        public SellDetailType CurSellDetailType { get; set; } = SellDetailType.None;

        /// <summary>
        /// 设置出售商品分类类型
        /// </summary>
        /// <param name="sellCategory"></param>
        public void SetSellCategory(SellCategory sellCategory)
        {
            CurSellCategory = sellCategory;
            eventEmitter.Trigger(EEvents.OnSellCategory, CurSellCategory);
        }


        public class TradeItemInfo //特殊情况，需要uid和infoid
        {
            public ulong uID;
            public uint infoID;
        }

        /// <summary>
        /// 设置出售商品的服务器类型
        /// </summary>
        /// <param name="serverType"></param>
        public void SetSellServerType(ServerType serverType)
        {
            CurSellServerType = serverType;

            eventEmitter.Trigger(EEvents.OnSellServerType);
        }

        /// <summary>
        /// 设置出售商品的操作类型
        /// </summary>
        /// <param name="opType"></param>
        public void SetSellOptionType(SellOpType opType)
        {
            CurSellOpType = opType;
            eventEmitter.Trigger(EEvents.OnSellOptionType, CurSellOpType);
        }

        /// <summary>
        /// 设置出售详情的类型
        /// </summary>
        /// <param name="type"></param>
        public void SetSellDeitalType(SellDetailType type)
        {
            CurSellDetailType = type;
            eventEmitter.Trigger(EEvents.OnSellDetailType, CurSellDetailType);
        }

        /// <summary>
        /// 获得出售道具列表
        /// </summary>
        /// <returns></returns>
        public List<TradeItemInfo> GetSellItemList()
        {
            List<TradeItemInfo> result = new List<TradeItemInfo>();

            //普通物品
            List<ItemData> tempList;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdNormal, out tempList))
            {
                for (int i = 0; i < tempList.Count; ++i)
                {
                    //非邦定,且不是永久禁售
                    if (!tempList[i].bBind && tempList[i].MarketendTime >= 0)
                    {
                        if (tempList[i].cSVItemData.on_sale) //可上架
                        {
                            //元核要特殊判断
                            if (tempList[i].cSVItemData.type_id == (int) EItemType.PetEquipment)
                            {
                                if (!Sys_Pet.Instance.IsEquip(tempList[i].Uuid)) //未装备宠物
                                    result.Add(new TradeItemInfo() {uID = tempList[i].Uuid, infoID = tempList[i].Id });
                            }
                            else
                            {
                                result.Add(new TradeItemInfo() {uID = tempList[i].Uuid, infoID = tempList[i].Id });
                            }
                        }
                    }
                }
            }

            //材料物品
            tempList = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdMaterial, out tempList))
            {
                for (int i = 0; i < tempList.Count; ++i)
                {
                    //非邦定,且不是永久禁售
                    if (!tempList[i].bBind && tempList[i].MarketendTime >= 0)
                    {
                        if (tempList[i].cSVItemData.on_sale)
                            result.Add(new TradeItemInfo() { uID = tempList[i].Uuid, infoID = tempList[i].Id });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 获得出售宠物列表
        /// </summary>
        public List<TradeItemInfo> GetSellPetList()
        {
            List<TradeItemInfo> tempList = new List<TradeItemInfo>();

            List<ClientPet> petList = Sys_Pet.Instance.petsList;
            for (int i = 0; i < petList.Count; ++i)
            {
                ClientPet pet = petList[i];

                if (pet.petUnit.SimpleInfo.Bind) //绑定
                    continue;
                if (pet.petUnit.SimpleInfo.LockPeriod == -1) //永久禁售
                    continue;

                tempList.Add(new TradeItemInfo() { uID = pet.petUnit.Uid, infoID = pet.petUnit.SimpleInfo.PetId });
            }

            return tempList;
        }

        /// <summary>
        /// 获取组合的道具数据
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <returns></returns>
        public ItemData GetItemDataByTradeItemInfo(TradeItemInfo itemInfo)
        {
            ItemData itemData = null;
            var data = CSVItem.Instance.GetConfData(itemInfo.infoID);
            if (data != null)
            {
                if (data.type_id == (uint)EItemType.Pet) //宠物
                {
                    List<ClientPet> petList = Sys_Pet.Instance.petsList;
                    for (int i = 0; i < petList.Count; ++i)
                    {
                        ClientPet pet = petList[i];
                        if (pet.petUnit.Uid == itemInfo.uID)
                        {
                            itemData = new ItemData(99, pet.petUnit.Uid, pet.petUnit.SimpleInfo.PetId, 1, 0, false, false, null, null, (int)pet.petUnit.SimpleInfo.LockPeriod, pet.petUnit);
                            break;
                        }
                    }
                }
                else
                {
                    itemData = Sys_Bag.Instance.GetItemDataByUuid(itemInfo.uID);
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("{0} 商品表里没有", itemInfo.infoID.ToString());
            }

            return itemData;
        }

        /// <summary>
        /// 获得出售摊位数量
        /// </summary>
        /// <returns></returns>
        public int GetSaleBoothCount()
        {
            return m_SaleGoods.Count;
        }

        /// <summary>
        /// 获得出售摊位商品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TradeBrief GetSaleBoothBrief(int index)
        {
            if (index < m_SaleGoods.Count)
            {
                return m_SaleGoods[index];
            }
            return null;
        }

        public int GetSaleBoothTotalCount()
        {
            int gridCount = (int)BoothGridCout;
            if (m_SaleGoods != null)
            {
                return m_SaleGoods.Count > gridCount ? m_SaleGoods.Count : gridCount;
            }
            else
            {
                return gridCount;
            }
        }

        /// <summary>
        /// 检测道具出售，需要弹哪个UI
        /// </summary>
        /// <param name="item"></param>
        public void OnSaleItemCheck(TradeItemInfo itemInfo)
        {
            ItemData item = GetItemDataByTradeItemInfo(itemInfo);
            item.marketendTimer.UpdateReMainMarkendTime();
            if (item.marketendTimer.remainTime > 0)
            {
                uint left = item.marketendTimer.remainTime % 86400;
                uint day = item.marketendTimer.remainTime / 86400;
                if (left != 0)
                    day = day < 1 ? 1 : day + 1;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, day.ToString()));
                //uint day = item.marketendTimer.remainTime / 86400;
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, LanguageHelper.TimeToString(item.marketendTimer.remainTime, LanguageHelper.TimeFormat.Type_4)));
                return;
            }

            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(item.Id);
            if (goodData != null)
            {
                if (!CheckGoodSaleLevel(goodData))
                    return;

                if (item.cSVItemData.type_id == (int)EItemType.Equipment)
                {
                    //安全锁
                    if (Sys_Equip.Instance.IsSecureLock(item))
                        return;
                }
                else if (item.cSVItemData.type_id == (int)EItemType.Pet)
                {
                    //安全锁
                    if (Sys_Pet.Instance.IsPetBeEffectWithSecureLock(item.Pet))
                        return;
                }
                else if (item.cSVItemData.type_id == (int) EItemType.PetEquipment)
                {
                    //安全锁
                    if (Sys_Pet.Instance.IsPetEquipmentSecureLock(item))
                        return;
                }
                
                //世界等级
                if (goodData.world_level <= Sys_Role.Instance.GetWorldLv())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011907, goodData.world_level.ToString()));
                    return;
                }

                bool isCommonSale = !goodData.cross_server && !goodData.bargain && !goodData.assignation;
                if (isCommonSale)
                {
                    UIManager.OpenUI(EUIID.UI_Trade_SellDetail_Pricing, false, itemInfo);
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_Trade_Sell, false, itemInfo);
                }
            }
            else
            {
                DebugUtil.LogErrorFormat("not found CSVCommodity itemId = {0}", item.Id);
            }
        }

        /// <summary>
        /// 检测商品,上架等级
        /// </summary>
        /// <param name="goodData"></param>
        /// <returns></returns>
        public bool CheckGoodSaleLevel(CSVCommodity.Data goodData)
        {
            uint level = GoodSaleLevel(goodData.pricing_type);
            bool isSale = Sys_Role.Instance.Role.Level >= level;

            if (!isSale)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011188, level.ToString()));

            return isSale;
        }

        /// <summary>
        /// 检测商品,购买等级
        /// </summary>
        /// <param name="goodData"></param>
        /// <returns></returns>
        public bool CheckGoodBuyLevel(CSVCommodity.Data goodData)
        {
            uint level = GoodBuyLevel(goodData.pricing_type);
            bool isBuy = Sys_Role.Instance.Role.Level >= level;

            if (!isBuy)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011189, level.ToString()));

            return isBuy;
        }

        #region 上架提示
        private class SaleSuccessInfo
        {
            public bool isTip = true;
            public uint tipTime = 0u;
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);

                //if (jo.ContainsKey("isTip"))
                //{
                //    isTip = (bool)jo["idList"];
                //}
            }

            public void Clear()
            {
                isTip = true;
                tipTime = 0u;
            }
        }


        private string SaleSuccessInfoPath = "TradeSaleSuccessInfo";
        private SaleSuccessInfo saleSuccessInfo = new SaleSuccessInfo();
        private uint sevenDayTime = 3600 * 24 * 7;
        public void CheckSaleSuccessTip(TradeBrief brief)
        {
            bool isNeedTip = true;
            if (!saleSuccessInfo.isTip)
            {
                if (saleSuccessInfo.tipTime + sevenDayTime >= Sys_Time.Instance.GetServerTime())
                {
                    isNeedTip = false;
                }
            }

            if (isNeedTip)
            {
                UIManager.OpenUI(EUIID.UI_Trade_SaleSuccess_Tip, false, brief);
            }
        }

        public void LoadSaleSuccessInfo()
        {
            saleSuccessInfo.Clear();

            JsonObject json = FileStore.ReadJson(SaleSuccessInfoPath);
            if (json != null)
            {
                saleSuccessInfo.DeserializeObject(json);
            }
        }

        public void SaveSuccessInfo()
        {
            saleSuccessInfo.isTip = false;
            saleSuccessInfo.tipTime = Sys_Time.Instance.GetServerTime();
            FileStore.WriteJson(SaleSuccessInfoPath, saleSuccessInfo);
        }
        #endregion
    }
}
