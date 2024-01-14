using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public class Sys_Mall : SystemModuleBase<Sys_Mall>
    {
        //private List<uint> listInofIds = new List<uint>();

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public MallPrama skip2MallFromItemSource;

        public enum EEvents : int
        {
            OnFillShopData,
            OnRefreshShopData,
            OnSelectShopItem,
            OnBuyScuccess,
            OnBuyFail,
            OnTelCharge,
            OnRefreshRedDot,
        }

        private uint mShopItemId = 0;
        public uint SelectShopItemId {
            get {
                return mShopItemId;
            }
            set {
                mShopItemId = value;
            }
        }

        private long MaxBuyCount = 99999;

        private CmdShopItemRecordRes shopRecord;
        private uint mShopId;

        private bool isRecordChange = false;

        public uint SelectShopId = 0;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdShop.ItemRecordReq, (ushort)CmdShop.ItemRecordRes, OnItemRecordRes, CmdShopItemRecordRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdShop.BuyReq, (ushort)CmdShop.BuyRes, OnBuyRes, CmdShopBuyRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdShop.ItemRecordUpdateNtf, OnRecordUpdateNtf, CmdShopItemRecordUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdShop.RedDotItemsNtf, OnRedDotItemsNtf, CmdShopRedDotItemsNtf.Parser);
        }

        public void OnItemRecordReq(uint shopId, ulong npcUid = 0)
        {
            CmdShopItemRecordReq req = new CmdShopItemRecordReq();
            req.ShopId = shopId;
            req.NpcUid = npcUid;
            NetClient.Instance.SendMessage((ushort)CmdShop.ItemRecordReq, req);
        }

        private void OnItemRecordRes(NetMsg msg)
        {
            shopRecord = NetMsgUtil.Deserialize<CmdShopItemRecordRes>(CmdShopItemRecordRes.Parser, msg);
            mShopId = shopRecord.ShopId;

            if (isRecordChange)
            {
                eventEmitter.Trigger(EEvents.OnRefreshShopData);
            }
            else
            {
                mShopItemId = 0;
                eventEmitter.Trigger<uint>(EEvents.OnFillShopData, mShopId);
            }

            isRecordChange = false;
        }

        public void OnBuyReq(uint shopItemId, uint count, ulong npcUid = 0, ulong friendId = 0)
        {
            CmdShopBuyReq req = new CmdShopBuyReq();
            req.ShopItemId = shopItemId;
            req.BuyCount = count;
            req.BuyPrice = GetItemPrice(shopItemId);
            req.NpcUid = npcUid;
            req.FriendId = friendId;
            NetClient.Instance.SendMessage((ushort)CmdShop.BuyReq, req);
        }
    
        private void OnBuyRes(NetMsg msg)
        {
            CmdShopBuyRes ntf = NetMsgUtil.Deserialize<CmdShopBuyRes>(CmdShopBuyRes.Parser, msg);
            if (ntf.Ret == 0)
            {
                //购买成功
                eventEmitter.Trigger(EEvents.OnBuyScuccess);
            }
            else
            {
                eventEmitter.Trigger(EEvents.OnBuyFail);
                if (ntf.Ret == (int)ErrorShop.PriceHasBeenChanged)
                {
                    isRecordChange = true;
                    //重新请求刷新
                    OnItemRecordReq(mShopId);
                }
            }
        }

        private void OnRecordUpdateNtf(NetMsg msg)
        {
            CmdShopItemRecordUpdateNtf ntf = NetMsgUtil.Deserialize<CmdShopItemRecordUpdateNtf>(CmdShopItemRecordUpdateNtf.Parser, msg);

            bool haveRecord = false;
            for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
            {
                if (shopRecord.ShopItemList[i].ShopItemId == ntf.ShopItemId)
                {
                    haveRecord = true;
                    shopRecord.ShopItemList[i].SelfCount = ntf.SelfCount;
                    shopRecord.ShopItemList[i].GlobalCount = ntf.GlobalCount;
                    break;
                }
            }

            if (!haveRecord)
            {
                ShopItem item = new ShopItem();
                item.ShopItemId = ntf.ShopItemId;
                item.SelfCount = ntf.SelfCount;
                item.GlobalCount = ntf.GlobalCount;

                shopRecord.ShopItemList.Add(item);
            }

            eventEmitter.Trigger(EEvents.OnRefreshShopData);
        }

        private List<CmdShopRedDotItemsNtf.Types.Item> listRedItems = new List<CmdShopRedDotItemsNtf.Types.Item>();
        private void OnRedDotItemsNtf(NetMsg msg)
        {
            CmdShopRedDotItemsNtf ntf = NetMsgUtil.Deserialize<CmdShopRedDotItemsNtf>(CmdShopRedDotItemsNtf.Parser, msg);
            listRedItems.Clear();
            listRedItems.AddRange(ntf.ShopItems);
        }

        public bool IsMallRed(uint mallId)
        {
            bool isRed = false;

            CSVMall.Data mallData = CSVMall.Instance.GetConfData(mallId);
            if (mallData != null)
            {
                for (int i = 0; i < mallData.shop_id.Count; ++i)
                {
                    if (IsShopRed(mallData.shop_id[i]))
                    {
                        isRed = true;
                        break;
                    }
                }
            }

            return isRed;
        }

        public bool IsShopRed(uint shopId)
        {
            bool isRed = false;
            for (int i = 0; i < listRedItems.Count; ++i)
            {
                if (listRedItems[i].ShopId == shopId)
                {
                    isRed = true;
                    break;
                }
            }

            return isRed;
        }

        public bool IsShopItemRed(uint shopItemId)
        {
            bool isRed = false;
            for (int i = 0; i < listRedItems.Count; ++i)
            {
                if (listRedItems[i].ShopItemId == shopItemId)
                {
                    isRed = true;
                    break;
                }
            }

            return isRed;
        }

        public void RemoveShopItemRed(uint shopItemId)
        {
            bool isRemove = false;
            for (int i = listRedItems.Count - 1 ; i >= 0; --i)
            {
                if (listRedItems[i].ShopItemId == shopItemId)
                {
                    listRedItems.RemoveAt(i);
                    isRemove = true;
                    break;
                }
            }

            if (isRemove)
                eventEmitter.Trigger(EEvents.OnRefreshRedDot);
        }

        #region Logic
        public uint GetItemPrice(uint shopItemId)
        {
            for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
            {
                if (shopRecord.ShopItemList[i].ShopItemId == shopItemId)
                {
                    return shopRecord.ShopItemList[i].Price;
                }
            }

            return 0;
        }

        // ignoreCountZero: 是否忽略可购次数为0的商品
        public List<ShopItem> GetShopItems(uint shopId)
        {
            List<ShopItem> list = new List<ShopItem>();
            if (shopRecord != null) 
            {
                if (shopRecord.ShopId == shopId) 
                {
                    for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
                    {
                        CSVShopItem.Data shopItemData = CSVShopItem.Instance.GetConfData(shopRecord.ShopItemList[i].ShopItemId);
                        if (shopItemData == null)
                        {
                            DebugUtil.LogErrorFormat("商品id {0} 未配置", shopRecord.ShopItemList[i].ShopItemId.ToString());
                            continue;
                        }

                        if (shopItemData.need_func_id == 0u)
                        {
                            list.Add(shopRecord.ShopItemList[i]);
                        }
                        else if (Sys_FunctionOpen.Instance.IsOpen(shopItemData.need_func_id, false))
                        {
                            list.Add(shopRecord.ShopItemList[i]);
                        }
                    }
                }

                int Compare(ShopItem item1, ShopItem item2) {
                    CSVShopItem.Data shoItemData1 = CSVShopItem.Instance.GetConfData(item1.ShopItemId);
                    CSVShopItem.Data shoItemData2 = CSVShopItem.Instance.GetConfData(item2.ShopItemId);
                    return (int)shoItemData1.produce_id - (int)shoItemData2.produce_id;
                }

                list.Sort(Compare);
            }

            return list;
        }

        public static int GetCanBuyCount(ShopItem shopItem) {
            if (shopItem.SelfNum != 0 && shopItem.GlobalNum == 0) {
                return (int)(shopItem.SelfNum - (long)shopItem.SelfCount);
            }
            else if (shopItem.GlobalNum != 0 && shopItem.SelfNum == 0) {
                return (int)(shopItem.GlobalNum - (long)shopItem.GlobalCount);
            }
            return 0;
        }

        public void OnSelectShopItem(uint shopItemId)
        {
            mShopItemId = shopItemId;
            eventEmitter.Trigger(EEvents.OnSelectShopItem);
        }

        public int CalAllServerLeftNum(uint shopItemId)
        {
            int leftNum = 0;
            CSVShopItem.Data shopData = CSVShopItem.Instance.GetConfData(shopItemId);
            leftNum = (int)shopData.server_limit;

            for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
            {
                if (shopRecord.ShopItemList[i].ShopItemId == shopItemId)
                {
                    leftNum -= (int)shopRecord.ShopItemList[i].GlobalCount;
                    break;
                }
            }

            return leftNum;
        }

        public bool IsFixCard(CSVShopItem.Data shopItemData)
        {
            if (shopItemData.need_card == 0)
            {
                return true;
            }
            
            return Sys_OperationalActivity.Instance.CheckSpecialCardIsActive(shopItemData.need_card);
        }
        
        public bool IsFixLevel(CSVShopItem.Data shopItemData)
        {
            return Sys_Role.Instance.Role.Level >= shopItemData.level_require;
        }

        public bool IsFixLikeAbility(CSVShopItem.Data shopItemData)
        {
            bool isFix = true;
            if (shopItemData.likability != null && shopItemData.likability.Count != 0)
            {
                for (int i = 0; i < shopItemData.likability.Count; ++i)
                {
                    uint npcId = shopItemData.likability[i][0];
                    uint ability = shopItemData.likability[i][1];

                    if (ability > Sys_NPCFavorability.Instance.GetNpcFavorability(npcId))
                    {
                        isFix = false;
                        break;
                    }
                }
            }

            return isFix;
        }

        public bool IsLikeAbilityFixed(List<uint> data)
        {
            uint npcId = data[0];
            uint ability = data[1];

            return ability <= Sys_NPCFavorability.Instance.GetNpcFavorability(npcId);
        }

        public bool IsNeedTask(CSVShopItem.Data shopItemData)
        {
            if (shopItemData.need_task != null && shopItemData.need_task.Count > 0)
            {
                for (int i = 0; i < shopItemData.need_task.Count; ++i)
                {
                    bool isSubmit = Sys_Task.Instance.IsSubmited(shopItemData.need_task[i]);

                    if (!isSubmit)
                        return false;
                }
            }

            return true;
        }

        public bool IsFixRank(CSVShopItem.Data shopItemData)
        {
            if (Sys_LadderPvp.Instance.LevelID == 0 || shopItemData.need_rank == 0)
                return true;
            return shopItemData.need_rank <= Sys_LadderPvp.Instance.LevelID;
        }

        public bool isTransformGoodLocked(CSVShopItem.Data shopItemData)
        {
            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(shopItemData.need_race_lv);
            if (csvRaceDepartmentResearchData == null)
            {
                return false;
            }
            else
            {
                if (Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftRaceSubNodes.ContainsKey(csvRaceDepartmentResearchData.type))
                {
                    uint curId = Sys_Transfiguration.Instance.GetCurUseShapeShiftData().shapeShiftRaceSubNodes[csvRaceDepartmentResearchData.type].Subnodeid;
                    CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchCurData = CSVRaceDepartmentResearch.Instance.GetConfData(curId);
                    if (csvRaceDepartmentResearchCurData.rank >= csvRaceDepartmentResearchData.rank &&csvRaceDepartmentResearchCurData.level >= csvRaceDepartmentResearchData.level)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (csvRaceDepartmentResearchData.rank == 0 && csvRaceDepartmentResearchData.level == 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }

        public int CalSelfLeftNum(uint shopItemId)
        {
            int leftNum = 0;
            CSVShopItem.Data shopData = CSVShopItem.Instance.GetConfData(shopItemId);
            leftNum = (int)shopData.personal_limit;
            for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
            {
                if (shopRecord.ShopItemList[i].ShopItemId == shopItemId)
                {
                    leftNum -= (int)shopRecord.ShopItemList[i].SelfCount;
                    break;
                }
            }

            return leftNum;
        }

        public int CalCanBuyMaxCount(uint shopItemId)
        {
            uint price = GetItemPrice(shopItemId);
            CSVShopItem.Data shopData = CSVShopItem.Instance.GetConfData(shopItemId);
            long priceCount = Sys_Bag.Instance.GetItemCount(shopData.price_type) / (long)price;
            long leftNum = MaxBuyCount;

            long personalNum = MaxBuyCount;
            long serverNum = MaxBuyCount;

            if (shopData.limit_type != 0)
            {
                if (shopData.personal_limit != 0)
                {
                    personalNum = CalSelfLeftNum(shopItemId);
                }

                if (shopData.server_limit != 0)
                {
                    serverNum = CalAllServerLeftNum(shopItemId);
                }

                leftNum = Math.Min(personalNum, serverNum);
            }

            if (priceCount < leftNum)
            {
                leftNum = priceCount == 0 ? 1 : priceCount;
            }

            //默认显示一个
            if (leftNum < 1)
                leftNum = 1;

            //配置表限制了最大购买数 
            if (shopData.perPurchase_limit_count != 0)
            {
                leftNum = leftNum > shopData.perPurchase_limit_count ? (int)shopData.perPurchase_limit_count : leftNum;
            }

            return (int)leftNum;
        }

        public bool IsSellOut(uint shopItemId)
        {
            CSVShopItem.Data shopData = CSVShopItem.Instance.GetConfData(shopItemId);
            if (shopData.limit_type == 1 || shopData.limit_type == 2 || shopData.limit_type == 5)
            {
                return CalSelfLeftNum(shopItemId) == 0;
            }
            else if (shopData.limit_type == 3 || shopData.limit_type == 4)
            {
                return CalAllServerLeftNum(shopItemId) == 0;
            }
            else
            {
                return false;
            }
        }

        public ShopItem GetNewData(uint shopItemId)
        {
            for (int i = 0; i < shopRecord.ShopItemList.Count; ++i)
            {
                if (shopRecord.ShopItemList[i].ShopItemId == shopItemId)
                    return shopRecord.ShopItemList[i];
            }

            return null;
        }

        public void ClearData()
        {
            mShopItemId = 0;
            skip2MallFromItemSource = null;
        }
        #endregion
    }
}