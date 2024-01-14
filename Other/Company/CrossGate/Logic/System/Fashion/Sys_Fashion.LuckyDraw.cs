using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf.Collections;
using Table;
using System;
using Framework;
using System.Json;

namespace Logic
{
    public partial class Sys_Fashion : SystemModuleBase<Sys_Fashion>, ISystemModuleUpdate
    {
        private uint m_ActiveId; //活动编号 如果活动未开启 是 0

        public uint activeId
        {
            get { return m_ActiveId; }
            set
            {
                if (m_ActiveId != value)
                {
                    m_ActiveId = value;

                    if (m_ActiveId != 0)
                    {
                        AutoBuyDrawReq(autoBuyDraw ? 1 : 0);
                    }

                    eventEmitter.Trigger(EEvents.OnRefreshLuckyDrawActiveState);
                }
            }
        }

        public CSVFashionActivity.Data cSVFashionActivityData
        {
            get { return CSVFashionActivity.Instance.GetConfData(m_ActiveId); }
        }

        public uint startTime;
        public uint endTime;

        public uint lastFreeDrawTime; //上一次免费抽奖的时间戳
        public bool autoBuyDraw; //是否自动购买
        public List<Item> dropItems = new List<Item>(); //当次抽奖获得的道具
        public Item dropCurrency;
        public uint lastDrawTimes; //上一次抽奖的次数(1/10)
        public bool drawFromRes;

        private bool b_FreeDraw;

        public bool freeDraw
        {
            get { return b_FreeDraw; }
            set
            {
                if (b_FreeDraw != value)
                {
                    b_FreeDraw = value;
                    eventEmitter.Trigger(EEvents.OnRefreshFreeDrawState);
                }
            }
        }

        private readonly string m_StoreFilePath = "FashionLuckyDraw";

        public FashionFreeRedInfo fashionFreeRedInfo = new FashionFreeRedInfo();

        public class FashionFreeRedInfo
        {
            public bool played;

            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }

        public void LoadedMemory()
        {
            JsonObject json = FileStore.ReadJson(m_StoreFilePath);
            if (json != null)
            {
                fashionFreeRedInfo.DeserializeObject(json);
            }
        }

        public void SaveMemory()
        {
            FileStore.WriteJson(m_StoreFilePath, fashionFreeRedInfo);
        }

        //data 抽奖次数
        public void FashionDrawReq(uint drawTimes)
        {
            lastDrawTimes = drawTimes;
            CmdFashionDrawReq cmdFashionDrawReq = new CmdFashionDrawReq();
            cmdFashionDrawReq.DrawTimes = drawTimes;
            NetClient.Instance.SendMessage((ushort) CmdFashion.DrawReq, cmdFashionDrawReq);
        }

        private void OnDrawRes(NetMsg netMsg)
        {
            CmdFashionDrawRes cmdFashionDrawRes = NetMsgUtil.Deserialize<CmdFashionDrawRes>(CmdFashionDrawRes.Parser, netMsg);
            lastFreeDrawTime = cmdFashionDrawRes.FreeDrawTime;

            dropItems.Clear();
            for (int i = 0; i < cmdFashionDrawRes.ItemList.Count - 1; i++) 
            {
                Item item = cmdFashionDrawRes.ItemList[i];
                if (item.Count >= 1) 
                {
                    dropItems.Add(item);
                }
            }

            dropCurrency = cmdFashionDrawRes.ItemList[cmdFashionDrawRes.ItemList.Count - 1];
            
            eventEmitter.Trigger(EEvents.OnDrawLucky);
            if (!UIManager.IsOpen(EUIID.UI_LuckyDraw_Result))
            {
                UIManager.OpenUI(EUIID.UI_LuckyDraw_Result);
            }
            else
            {
                eventEmitter.Trigger(EEvents.OnRefreshDrawLuckyResult);
            }
        }

        //魔币兑换抽奖道具(抽奖次数)
        public void FashionExchangeDrawItemReq(uint drawTimes)
        {
            CmdFashionExchangeDrawItemReq req = new CmdFashionExchangeDrawItemReq();
            req.DrawTimes = drawTimes;
            NetClient.Instance.SendMessage((ushort) CmdFashion.ExchangeDrawItemReq, req);
        }

        private void FashionExchangeDrawItemRes(NetMsg netMsg)
        {
            CmdFashionExchangeDrawItemRes res = NetMsgUtil.Deserialize<CmdFashionExchangeDrawItemRes>(CmdFashionExchangeDrawItemRes.Parser, netMsg);
            eventEmitter.Trigger(EEvents.OnExchangeDraw);
        }

        //0 取消  1 设置
        public void AutoBuyDrawReq(int data)
        {
            CmdFashionAutoBuyDrawItemReq cmdFashionAutoBuyDrawCoinReq = new CmdFashionAutoBuyDrawItemReq();
            cmdFashionAutoBuyDrawCoinReq.Type = (uint) data;

            NetClient.Instance.SendMessage((ushort) CmdFashion.AutoBuyDrawItemReq, cmdFashionAutoBuyDrawCoinReq);
        }

        private void OnAutoBuyDrawCoinRes(NetMsg netMsg)
        {
            CmdFashionAutoBuyDrawItemRes res = NetMsgUtil.Deserialize<CmdFashionAutoBuyDrawItemRes>(CmdFashionAutoBuyDrawItemRes.Parser, netMsg);
            autoBuyDraw = res.Type == 1;
        }

        private void DrawActiveInfoNtf(NetMsg netMsg)
        {
            CmdFashionDrawActiveInfoNtf res = NetMsgUtil.Deserialize<CmdFashionDrawActiveInfoNtf>(CmdFashionDrawActiveInfoNtf.Parser, netMsg);
            activeId = res.ActiveId;
            startTime = res.StartTime;
            endTime = res.EndTime;
        }


        public void StartLuckyDrawFromRes()
        {
            drawFromRes = true;
            //eventEmitter.Trigger<uint>(EEvents.OnStartLuckyDrawFromRes, lastDrawTimes);
        }
    }
}