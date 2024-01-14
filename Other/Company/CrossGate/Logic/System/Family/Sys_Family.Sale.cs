using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族系统-拍卖 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        //private List<CSVFamilyAuctionAct.Data>  _actData;
        public IReadOnlyList<CSVFamilyAuctionAct.Data>  actDatas
        {
            get
            {
                //if (null == _actData)
                //{
                //    _actData = new List<CSVFamilyAuctionAct.Data>();
                //    for (int i = 0; i < CSVFamilyAuctionAct.Instance.Count; i++)
                //    {
                //        _actData.Add(CSVFamilyAuctionAct.Instance[i]);
                //    }
                //}
                //return _actData;
                return CSVFamilyAuctionAct.Instance.GetAll();
            }

            //private set { }
        }

        public bool needShowAuctionRedPoint = true;
        /// <summary>
        /// 请求拍卖列表
        /// </summary>
        public void GuildAuctionListReq()
        {
            CmdGuildAuctionListReq req = new CmdGuildAuctionListReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.ListReq, req);
        }

        /// <summary>
        /// 活动列表数据 响应-通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionListAck(NetMsg msg)
        {
            CmdGuildAuctionListAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionListAck>(CmdGuildAuctionListAck.Parser, msg);
            familyData.familyAuctionInfo.ClearAuctionData();
            for (int i = 0; i < ack.Auctions.Count; i++)
            {
                familyData.familyAuctionInfo.SetActiveDicData(ack.Auctions[i]);
            }
            DebugUtil.Log(ELogType.eFamilyAuction, $"Now Action Count: {ack.Auctions.Count}");
            if(ack.Auctions.Count > 0)
            {
                DebugUtil.Log(ELogType.eFamilyAuction, $"Now AuctionItem Count :{ack.Auctions[0].Items.Count}");
            }

            familyData.familyAuctionInfo.SetMyActiveAuctionDicData(ack.MyAuctions);
            eventEmitter.Trigger(EEvents.OnAuctionMyInfoAckEnd);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyActiveRedPoint, null);
            eventEmitter.Trigger(EEvents.OnAuctionAckEnd);
        }

        /// <summary>
        /// 请求修改是否可见列表
        /// </summary>
        /// <param name="state"> 是否可见标识 </param>
        public void GuildAuctionWatchReq(bool state)
        {
            CmdGuildAuctionWatchReq req = new CmdGuildAuctionWatchReq();
            req.Watch = state;
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.WatchReq, req);
        }

        /// <summary>
        /// 服务器同步-可见与否
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionWatchAck(NetMsg msg)
        {
            CmdGuildAuctionWatchAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionWatchAck>(CmdGuildAuctionWatchAck.Parser, msg);
            familyData.familyAuctionInfo.IsWatch = ack.Watch;
        }

        /// <summary>
        /// 请求我的拍卖
        /// </summary>
        /// <param name="state"> 是否可见标识 </param>
        public void GuildAuctionMyInfoReq()
        {
            CmdGuildAuctionMyInfoReq req = new CmdGuildAuctionMyInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.MyInfoReq, req);
        }

        /// <summary>
        /// 我的拍卖 数据响应
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionMyInfoAck(NetMsg msg)
        {
            CmdGuildAuctionMyInfoAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionMyInfoAck>(CmdGuildAuctionMyInfoAck.Parser, msg);
            familyData.familyAuctionInfo.SetMyActiveAuctionDicData(ack.MyAuctions);
            eventEmitter.Trigger(EEvents.OnAuctionMyInfoAckEnd);
        }

        /// <summary>
        /// 玩家Watch后服务器会推送变动信息
        /// 新增拍卖、拍卖结束、物品价格变动、物品被一口价买走
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionListUpdateNtf(NetMsg msg)
        {
            CmdGuildAuctionListUpdateNtf ntf = NetMsgUtil.Deserialize<CmdGuildAuctionListUpdateNtf>(CmdGuildAuctionListUpdateNtf.Parser, msg);
            familyData.familyAuctionInfo.UpdateAuctionDataByServerNtf(ntf);
        }

        /// <summary>
        /// 请求拍卖记录
        /// </summary>
        public void GuildAuctionRecordReq()
        {
            CmdGuildAuctionRecordReq req = new CmdGuildAuctionRecordReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.RecordReq, req);
        }

        /// <summary>
        /// 拍卖记录 响应
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionRecordAck(NetMsg msg)
        {
            CmdGuildAuctionRecordAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionRecordAck>(CmdGuildAuctionRecordAck.Parser, msg);
            if (null != ack)
            {
                if (null == familyData.familyAuctionInfo.auctionRecord)
                {
                    familyData.familyAuctionInfo.auctionRecord = new List<GuildAuctionRecord>(ack.Reocords.Count);
                }
                familyData.familyAuctionInfo.auctionRecord.Clear();
                familyData.familyAuctionInfo.auctionRecord.AddRange(ack.Reocords);
            }
            eventEmitter.Trigger(EEvents.OnAuctionReocordAckEnd);
        }

        /// <summary>
        /// 请求拍卖竞价
        /// </summary>
        /// <param name="activeId">活动id</param>
        /// <param name="id">道具唯一id</param>
        /// <param name="infoId">表格id</param>
        /// <param name="price">竞拍价格， 需要是起拍加价的整数倍</param>
        public void GuildAuctionBidReq(uint activeId, uint id, uint infoId, uint price)
        {
            CmdGuildAuctionBidReq req = new CmdGuildAuctionBidReq();
            req.ActiveId = activeId;
            req.Id = id;
            req.InfoId = infoId;
            req.Price = price;
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.BidReq, req);
        }

        /// <summary>
        /// 竞拍返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionBidAck(NetMsg msg)
        {
            CmdGuildAuctionBidAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionBidAck>(CmdGuildAuctionBidAck.Parser, msg);
            if (ack.Ret == 0)
            {
                familyData.familyAuctionInfo.ChangeAuctionItem(ack.ActiveId, ack.Id, ack.Price);
                familyData.familyAuctionInfo.ChangeMyAuctionInfo(ack.ActiveId, new CmdGuildAuctionListUpdateNtf.Types.ItemUpdate { Id = ack.Id, Price = ack.Price }, false);

                //Sys_Hint.Instance.PushContent_Normal("出价成功"); //出价成功
                eventEmitter.Trigger(EEvents.OnAuctionItemChange, ack.ActiveId);
            }
            else if (ack.Ret == (uint)ErrorGuildAuction.PriceChangeBidFailed) // 说明价格发生变动，可以再请求一次List
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11915)); //该道具已被其他玩家购买，请重新选择拍卖道具
                GuildAuctionListReq();
                //GuildAuctionMyInfoReq();
                eventEmitter.Trigger(EEvents.OnAuctionItemStateError, ack.ActiveId);
            }
        }

        /// <summary>
        /// 一口价请求
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <param name="infoId"></param>
        public void GuildAuctionOnePriceReq(uint activeId, uint id, uint infoId)
        {
            CmdGuildAuctionOnePriceReq req = new CmdGuildAuctionOnePriceReq();
            req.ActiveId = activeId;
            req.Id = id;
            req.InfoId = infoId;
            NetClient.Instance.SendMessage((ushort)CmdGuildAuction.OnePriceReq, req);
        }

        /// <summary>
        /// 一口价返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnGuildAuctionOnePriceAck(NetMsg msg)
        {
            CmdGuildAuctionOnePriceAck ack = NetMsgUtil.Deserialize<CmdGuildAuctionOnePriceAck>(CmdGuildAuctionOnePriceAck.Parser, msg);
            if (ack.Ret == 0)
            {
                familyData.familyAuctionInfo.RemoveOnePriceItem(ack.ActiveId, ack.Id, ack.InfoId);
                familyData.familyAuctionInfo.ChangeMyAuctionInfo(ack.ActiveId, new CmdGuildAuctionListUpdateNtf.Types.ItemUpdate { Id = ack.Id, Price = 0 }, true);
                eventEmitter.Trigger(EEvents.OnAuctionItemChange, ack.ActiveId);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11917)); //该道具已被其他玩家购买，请重新选择拍卖道具
                GuildAuctionListReq();
                //GuildAuctionMyInfoReq();
                eventEmitter.Trigger(EEvents.OnAuctionItemStateError, ack.ActiveId);
            }
        }

        private void OnGuildAuctionNewNtf(NetMsg msg)
        {
            CmdGuildAuctionNewNtf ntf = NetMsgUtil.Deserialize<CmdGuildAuctionNewNtf>(CmdGuildAuctionNewNtf.Parser, msg);
            if (null != Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info)
            {
                for (int i = 0; i < ntf.Briefs.Count; i++)
                {
                    bool has = false;
                    for (int j = 0; j < familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Count; j++)
                    {
                        if (familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs[j].ActiveId == ntf.Briefs[i].ActiveId)
                        {
                            has = true;
                            familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs[j].EndTime = ntf.Briefs[i].EndTime;
                        }
                    }
                    if (!has)
                    {
                        familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.AuctionBriefs.Add(ntf.Briefs[i]);
                    }
                }
            }
            
            needShowAuctionRedPoint = true;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyActiveRedPoint, null);
            eventEmitter.Trigger(EEvents.OnAuctionAckEnd);
            Sys_Chat.Instance.PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(11910), Sys_Chat.EMessageProcess.None);
        }

        private void OnGuildAuctionMyInfoUpdateNtf(NetMsg msg)
        {
            CmdGuildAuctionMyInfoUpdateNtf ntf = NetMsgUtil.Deserialize<CmdGuildAuctionMyInfoUpdateNtf>(CmdGuildAuctionMyInfoUpdateNtf.Parser, msg);
            Sys_Family.Instance.familyData.familyAuctionInfo.UpdateMyAuctionDataByServerNtf(ntf);
        }

    }
}

/*message CmdGuildAuctionMyInfoUpdateNtf
{
    message MyInfoUpdate
    {
        uint32 activeId = 1;
        uint32 id = 2;
        //下列删除时不填充
        uint32 price = 3; //当前价格
        bool owned = 4; //是否归属自己
    }

    GuildAuctionMyInfo newMyInfo = 7; //我的拍卖变化
    MyInfoUpdate updateMyInfo = 8;
    repeated MyInfoUpdate delMyInfo = 9;
}*/