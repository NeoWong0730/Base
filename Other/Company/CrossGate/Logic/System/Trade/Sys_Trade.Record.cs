
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Packet;
using Logic.Core;
using Lib.Core;
using Net;


namespace Logic
{
    public partial class Sys_Trade : SystemModuleBase<Sys_Trade>
    {
        private uint RecordVer;// 记录版本号
        private CmdTradeGetRecordRes _recordRes;

        public class ApplealResult
        {
            public int code;
            public object data;
            public string msg;
        }

        private string recordVersionPath = "TradeRecord";
        private void LoadRecordData()
        {
            RecordVer = 0;
            _recordRes = null;
            _recordRes = FileStore.ReadProto<CmdTradeGetRecordRes>(CmdTradeGetRecordRes.Parser, recordVersionPath);
            if (_recordRes != null)
            {
                RecordVer = _recordRes.RecordVer;
            }
            //Debug.LogError("LoadRecordData ver = " + _recordRes.RecordVer);
        }

        private void SaveRecordData()
        {
            FileStore.WriteProto(recordVersionPath, _recordRes);
        }

        private void OnRecordUpdateNtf(NetMsg msg)
        {
            CmdTradeRecordUpdateNtf res = NetMsgUtil.Deserialize<CmdTradeRecordUpdateNtf>(CmdTradeRecordUpdateNtf.Parser, msg);
            //Debug.LogError("CmdTradeRecordUpdateNtf ver=" + res.RecordVer);
            //Debug.LogError("LoadRecordData ver = " + _recordRes.RecordVer);
            if (_recordRes != null)
            {
                if (res.RecordVer - _recordRes.RecordVer > 1)
                {
                    OnTradeRecordReq();
                    return;
                }
                
                _recordRes.RecordVer = res.RecordVer;
                foreach (var data in res.UpdateList)
                {
                    if (data.SaleRecord != null)
                    {
                        if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Add)
                        {
                            _recordRes.SaleRecord.Add(data.SaleRecord);
                        }
                        else if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Mod)
                        {
                            for (int i = 0; i < _recordRes.SaleRecord.Count; ++i)
                            {
                                if (_recordRes.SaleRecord[i].RecordUid == data.SaleRecord.RecordUid)
                                {
                                    _recordRes.SaleRecord[i] = data.SaleRecord;
                                    eventEmitter.Trigger(EEvents.OnRerordUpdateNtf, data.SaleRecord);
                                    break;
                                }
                            }
                        }
                        else if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Remove)
                        {
                            for (int i = _recordRes.SaleRecord.Count - 1; i >= 0; --i)
                            {
                                if (_recordRes.SaleRecord[i].DealId == data.SaleRecord.DealId)
                                {
                                    _recordRes.SaleRecord.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                    else if (data.BuyRecord != null)
                    {
                        if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Add)
                        {
                            _recordRes.BuyRecord.Add(data.BuyRecord);
                        }
                        else if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Mod)
                        {
                            for (int i = 0; i < _recordRes.BuyRecord.Count; ++i)
                            {
                                if (_recordRes.BuyRecord[i].RecordUid == data.BuyRecord.RecordUid)
                                {
                                    _recordRes.BuyRecord[i] = data.BuyRecord;
                                    //Debug.LogError("OnRecordBuyNtf");
                                    eventEmitter.Trigger(EEvents.OnRecordBuyNtf, data.BuyRecord);
                                    break;
                                }
                            }
                        }
                        else if (data.Type == CmdTradeRecordUpdateNtf.Types.Type.Remove)
                        {
                            for (int i = _recordRes.BuyRecord.Count - 1; i >= 0; --i)
                            {
                                if (_recordRes.BuyRecord[i].RecordUid == data.BuyRecord.RecordUid)
                                {
                                    _recordRes.BuyRecord.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                }

                SaveRecordData();
            }
            else
            {
                OnTradeRecordReq();
            }
        }

        public void CheckAdvanceBuyUsedTimes()
        {
            if (m_advanceOfferData != null)
            {
                if (!Sys_Time.IsServerSameDay5(m_advanceOfferData.LastAdvanceOfferTime, Sys_Time.Instance.GetServerTime()))
                    m_advanceOfferData.UsedAdvanceOfferTimes = 0;
            }
        }

        public void OnRecordTakeOutReq(uint recordUId)
        {
            CmdTradeTakeOutGoodsReq req = new CmdTradeTakeOutGoodsReq();
            req.RecordUid = recordUId;
            NetClient.Instance.SendMessage((ushort)CmdTrade.TakeOutGoodsReq, req);
        }

        private void OnRecordTakeOutRes(NetMsg msg)
        {
            CmdTradeTakeOutGoodsRes res = NetMsgUtil.Deserialize<CmdTradeTakeOutGoodsRes>(CmdTradeTakeOutGoodsRes.Parser, msg);
        }

        public void OnSaleReviewReq()
        {
            CmdTradeSaleReviewReq req = new CmdTradeSaleReviewReq();
            //req.BCross = cross;
            req.SellerId = Sys_Role.Instance.RoleId;
            req.DealId = _dealId;

            NetClient.Instance.SendMessage((ushort)CmdTrade.SaleReviewReq, req);
        }

        private void OnSaleReviewRes(NetMsg msg)
        {
            CmdTradeSaleReviewRes res = NetMsgUtil.Deserialize<CmdTradeSaleReviewRes>(CmdTradeSaleReviewRes.Parser, msg);
            TradeSaleRecord saleRecord = null;
            foreach (var data in _recordRes.SaleRecord)
            {
                if (data.DealId == res.DealId)
                {
                    data.MarkStatus = res.ReviewStatus;
                    saleRecord = data;
                    break;
                }
                if (saleRecord != null)
                    eventEmitter.Trigger(EEvents.OnRerordUpdateNtf, saleRecord);
            }
        }

        private ulong _dealId;
        public void OnAppeal(ulong dealId, string reason, uint checkTime)
        {
            _dealId = dealId;

            GetAppealWebRequest(dealId, reason, GetAppealEndTime(checkTime));
        }

        private void OnSDKAppealStatus(string result)
        {
            ApplealResult appealRes = LitJson.JsonMapper.ToObject<ApplealResult>(result);
            if (appealRes != null)
            {
                if (appealRes.code == 0) //申诉成功
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011254));
                    OnSaleReviewReq();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(appealRes.msg);
                }
            }
            //DebugUtil.LogErrorFormat("OnSDKAppealStatus 解析失败");
        }


        //出售非珍品
        public List<TradeSaleRecord> GetSaleRecordNormal()
        {
            List<TradeSaleRecord> list = new List<TradeSaleRecord>();
            if (_recordRes != null)
            {
                foreach(var data in _recordRes.SaleRecord)
                {
                    if (!data.Treasure)
                        list.Add(data);
                }
            }

            return list;
        }

        //出售珍品
        public List<TradeSaleRecord> GetSaleRecordTreasure()
        {
            List<TradeSaleRecord> list = new List<TradeSaleRecord>();
            if (_recordRes != null)
            {
                foreach(var data in _recordRes.SaleRecord)
                {
                    if (data.Treasure)
                        list.Add(data);
                }
            }

            return list;
        }

        //购买非珍品
        public List<TradeBuyRecord> GetBuyRecordNormal()
        {
            List<TradeBuyRecord> list = new List<TradeBuyRecord>();
            if (_recordRes != null)
            {
                foreach(var data in _recordRes.BuyRecord)
                {
                    if (!data.Treasure)
                        list.Add(data);
                }
            }

            return list;
        }

        //购买珍品
        public List<TradeBuyRecord> GetBuyRecordTreasure()
        {
            List<TradeBuyRecord> list = new List<TradeBuyRecord>();
            if (_recordRes != null)
            {
                foreach(var data in _recordRes.BuyRecord)
                {
                    if (data.Treasure)
                        list.Add(data);
                }
            }

            return list;
        }

        /// <summary>
        /// 计算到账时间
        /// </summary>
        /// <param name="recordTime"></param>
        /// <returns></returns>
        public uint CalAccountTime(uint recordTime)
        {
            uint leftTime = recordTime + _timeCheckGood * 3600;
            uint curTime = Sys_Time.Instance.GetServerTime();
            if (leftTime > curTime)
                leftTime -= curTime;
            else
                leftTime = 0u;

            return leftTime;
        }


        public bool IsShowFreezeAppealBtn(TradeSaleRecord record)
        {
            uint minTime = record.CheckTime + record.TotalDay * 86400;
            uint maxTime = record.CheckTime + _timeAppeal * 86400;
            uint time = System.Math.Min(minTime, maxTime);

            return Sys_Time.Instance.GetServerTime() < time;
        }

        public bool IsShowPunishAppealBtn(TradeSaleRecord record)
        {
            uint time = record.CheckTime + _timeAppeal * 86400;

            return Sys_Time.Instance.GetServerTime() < time;
        }

        /// <summary>
        /// 审核截止时间
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        public long GetAppealEndTime(uint checkTime)
        {
            return ((long)checkTime + (_timeAppeal + _timeApppealSecond) * 86400) * 1000; //毫秒
        }


        private class AppealData
        {
            public string app_id;
            public string order_id;
            public string appeal_reason;
            public string appeal_verify_end_time;
        }

        static UnityWebRequest unityWebRequest;

        public void GetAppealWebRequest(ulong dealId, string content, long endTime) //申诉
        {
            if (null == unityWebRequest)
            {
                AppealData data = new AppealData();
                data.app_id = SDKManager.GetAppid();
                data.order_id = dealId.ToString();
                data.appeal_reason = content;
                data.appeal_verify_end_time = endTime.ToString();
                //DebugUtil.LogErrorFormat("AppealWebRequest:{0} - {1}", url, data);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(LitJson.JsonMapper.ToJson(data));
                unityWebRequest = UnityWebRequest.Put(_appealUrl, bytes);
                unityWebRequest.timeout = 2;
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");
                unityWebRequest.SendWebRequest().completed += AsyncOperationAppealCompleted;
            }
        }

        private void AsyncOperationAppealCompleted(AsyncOperation asyncOperation)
        {
            if (unityWebRequest.isDone)
            {
                using (MemoryStream ms = new MemoryStream(unityWebRequest.downloadHandler.data))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        string content = sr.ReadToEnd();
                        //DebugUtil.LogErrorFormat("申诉接口获取内容：\n {0}", content);
                        OnSDKAppealStatus(content);
                    }
                }
            }
            unityWebRequest.Dispose();
            unityWebRequest = null;
        }

        /// <summary>
        /// 判断商品是否被自己预购
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public bool IsAdvaceBuy(ulong uId)
        {
            bool IsBuy = false;
            if (_recordRes != null)
            {
                for (int i = 0; i < _recordRes.BuyRecord.Count; ++i)
                {
                    if (uId == _recordRes.BuyRecord[i].GoodsUid)
                    {
                        IsBuy = _recordRes.BuyRecord[i].ReceiveState == 4;
                        break;
                    }
                }
            }

            return IsBuy;
        }
    }
}
