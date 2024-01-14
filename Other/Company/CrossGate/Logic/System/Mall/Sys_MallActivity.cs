using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;

namespace Logic
{
    public class Sys_MallActivity : SystemModuleBase<Sys_MallActivity>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents : int
        {
            OnFillShopData,
            OnRefreshShopData,
        }
        
        private CmdActivityShopDataRes m_ShopData;
        private List<ShopGoods> m_ListGoods;
        private uint m_ActivityId;
        public uint NextTurnTime;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityShopDataReq, (ushort)CmdActivityRuler.CmdActivityShopDataRes, OnShopDataRes, CmdActivityShopDataRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityShopBuyReq, (ushort)CmdActivityRuler.CmdActivityShopBuyRes, OnBuyRes, CmdActivityShopBuyRes.Parser);
        }

        public void OnShopDataReq(uint activiyId)
        {
            CmdActivityShopDataReq req = new CmdActivityShopDataReq();
            req.ActivityId = activiyId;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityShopDataReq, req);
        }

        private void OnShopDataRes(NetMsg msg)
        {
            m_ShopData = NetMsgUtil.Deserialize<CmdActivityShopDataRes>(CmdActivityShopDataRes.Parser, msg);
            m_ActivityId = m_ShopData.ActivityId;
            if (m_ListGoods == null)
                m_ListGoods = new List<ShopGoods>();
            m_ListGoods.Clear();
            m_ListGoods.AddRange(m_ShopData.GoodsList);

            NextTurnTime = m_ShopData.RefreshTime;
            
            eventEmitter.Trigger(EEvents.OnFillShopData);
        }

        public void OnBuyReq(uint goodsId, uint count)
        {
            CmdActivityShopBuyReq req = new CmdActivityShopBuyReq();
            req.GoodsId = goodsId;
            req.BuyCount = count;
            NetClient.Instance.SendMessage((ushort) CmdActivityRuler.CmdActivityShopBuyReq, req);
        }

        private void OnBuyRes(NetMsg msg)
        {
            CmdActivityShopBuyRes res = NetMsgUtil.Deserialize<CmdActivityShopBuyRes>(CmdActivityShopBuyRes.Parser, msg);
            if (res.Goods != null)
            {
                for (int i = 0; i < res.Goods.Count; ++i)
                {
                    for (int j = 0; j < m_ListGoods.Count; ++j)
                    {
                        if (m_ListGoods[j].GoodsId == res.Goods[i].GoodsId)
                        {
                            m_ListGoods[j] = res.Goods[i];
                            break;
                        }
                    }

                }
            }
            
            eventEmitter.Trigger(EEvents.OnRefreshShopData);
         
        }

        public List<ShopGoods> GetActivityShopItems()
        {
            return m_ListGoods;
        }
        
        
        public int CalActivityAllServerLeftNum(uint shopItemId)
        {
            int leftNum = 0;
            CSVActivityShopGoods.Data shopData = CSVActivityShopGoods.Instance.GetConfData(shopItemId);
            leftNum = (int)shopData.server_limit;

            for (int i = 0; i < m_ListGoods.Count; ++i)
            {
                if (m_ListGoods[i].GoodsId == shopItemId)
                {
                    leftNum -= (int)m_ListGoods[i].GlobalCount;
                    break;
                }
            }

            return leftNum;
        }
        
        public int CalActivitySelfLeftNum(uint shopItemId)
        {
            int leftNum = 0;
            CSVActivityShopGoods.Data shopData = CSVActivityShopGoods.Instance.GetConfData(shopItemId);
            leftNum = (int)shopData.personal_limit;
            for (int i = 0; i < m_ListGoods.Count; ++i)
            {
                if (m_ListGoods[i].GoodsId == shopItemId)
                {
                    leftNum -= (int)m_ListGoods[i].SelfCount;
                    break;
                }
            }

            return leftNum;
        }

        public int CalCanBuyMaxCount(uint shopItemId, bool isPrice = false)
        {
            CSVActivityShopGoods.Data shopData = CSVActivityShopGoods.Instance.GetConfData(shopItemId);
            int maxBuyNum = (int)shopData.perPurchase_limit_count; //默认数字
            if (shopData.server_limit != 0 || shopData.personal_limit != 0)
            {
                //如果同时存在限购
                if (shopData.server_limit != 0 && shopData.personal_limit != 0)
                {
                    int serverNum = CalActivityAllServerLeftNum(shopItemId);
                    int selfNum = CalActivitySelfLeftNum(shopItemId);
                    maxBuyNum = UnityEngine.Mathf.Min(serverNum, selfNum);
                }
                else
                {
                    if (shopData.server_limit != 0)
                        maxBuyNum = CalActivityAllServerLeftNum(shopItemId);
                    else
                        maxBuyNum = CalActivitySelfLeftNum(shopItemId);
                }
            }

            if (isPrice)
            {
                long priceCount = Sys_Bag.Instance.GetItemCount(shopData.price_type) / shopData.price_now;
                maxBuyNum = UnityEngine.Mathf.Min(maxBuyNum, (int)priceCount);
            }
            
            return maxBuyNum;
        }

        public bool IsActivitySellOut(uint shopItemId)
        {
            CSVActivityShopGoods.Data shopData = CSVActivityShopGoods.Instance.GetConfData(shopItemId);
            if (shopData.server_limit != 0)
            {
                return CalActivityAllServerLeftNum(shopItemId) == 0;
            }
            else
            {
                return false;
            }
        }

        public ShopGoods GetNewData(uint shopItemId)
        {
            for (int i = 0; i < m_ListGoods.Count; ++i)
            {
                if (m_ListGoods[i].GoodsId == shopItemId)
                    return m_ListGoods[i];
            }

            return null;
        }
    }
}