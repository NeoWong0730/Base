using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    public class SinglePayRewardItem
    {
        public uint InfoId;
        /// <summary>
        /// 活动id
        /// </summary>
        public uint ActivityId;
        /// <summary>
        /// 可领取次数
        /// </summary>
        public uint RewardCount;
        /// <summary>
        /// 已领取次数
        /// </summary>
        public uint TakeCount;
    }
    public class Sys_SinglePay : SystemModuleBase<Sys_SinglePay>
    {
        /// <summary>
        /// 单笔充值的任务id列表(对应当期活动的表id)
        /// </summary>
        public List<uint> listTaskId = new List<uint>();
        /// <summary>
        /// 合服单笔充值的任务id列表(对应当期活动的表id)
        /// </summary>
        public List<uint> listHeFuTaskId = new List<uint>();

        /// <summary>
        /// 奖励id列表(对应字典的key)
        /// </summary>
        private List<uint> listRewardIds = new List<uint>();
        private Dictionary<uint, SinglePayRewardItem> dicRewards = new Dictionary<uint, SinglePayRewardItem>();

        /// <summary>
        /// 奖励id列表(对应字典的key)
        /// </summary>
        private List<uint> listHeFuRewardIds = new List<uint>();
        private Dictionary<uint, SinglePayRewardItem> dicHeFuRewards = new Dictionary<uint, SinglePayRewardItem>();

        public enum EEvents : int
        {
            /// <summary>
            /// 刷新单笔充值数据
            /// </summary>
            OnSinglePayDataUpdate,
            /// <summary>
            /// 单笔充值活动结束
            /// </summary>
            OnSinglePayEnd,
            /// <summary>
            /// 刷新单笔充值合服数据
            /// </summary>
            OnSinglePayHeFuDataUpdate,
            /// <summary>
            /// 单笔充值合服活动结束
            /// </summary>
            OnSinglePayHeFEnd,
        }
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary> 当前活动ID </summary>
        public uint curActivityId { get; private set; } = 0;
        public uint curStartTime { get; private set; } = 0;
        public uint curEndTime { get; private set; } = 0;

        private Timer timer;

        /// <summary> 当前合服活动ID </summary>
        public uint curHeFuActivityId { get; private set; } = 0;
        public uint curHeFuStartTime { get; private set; } = 0;
        public uint curHeFuEndTime { get; private set; } = 0;

        private Timer timerHeFu;

        #region 系统函数
        public override void Init()
        {
            RegisterEvents(true);
        }

        public override void Dispose()
        {
            timer?.Cancel();
            timerHeFu?.Cancel();
            RegisterEvents(false);
        }

        public override void OnLogin()
        {
            InitActivityData();
            InitHefuActivityData();
        }
        public override void OnLogout()
        {
            timer?.Cancel();
            timerHeFu?.Cancel();
            listRewardIds.Clear();
            dicRewards.Clear();
            listHeFuRewardIds.Clear();
            dicHeFuRewards.Clear();
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.CmdActivitySinglePayUpdateNtf, OnSinglePayUpdateNtf, CmdActivitySinglePayUpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivitySinglePayDataReq, (ushort)CmdActivityRuler.CmdActivitySinglePayDataRes, OnSinglePayDataRes, CmdActivitySinglePayDataRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.CmdActivitySinglePay2UpdateNtf, OnSinglePayHeFuUpdateNtf, CmdActivitySinglePay2UpdateNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivitySinglePay2DataReq, (ushort)CmdActivityRuler.CmdActivitySinglePay2DataRes, OnSinglePayHeFuDataRes, CmdActivitySinglePay2DataRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySinglePayUpdateNtf, OnSinglePayUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySinglePayDataRes, OnSinglePayDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySinglePay2UpdateNtf, OnSinglePayHeFuUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivitySinglePay2DataRes, OnSinglePayHeFuDataRes);
            }
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, OnUpdateActivityOperationRuler, toRegister);
        }
        #endregion

        #region 服务器消息 原版
        /// <summary> 请求数据 </summary>
        public void ReqSinglePayData()
        {
            CmdActivitySinglePayDataReq req = new CmdActivitySinglePayDataReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySinglePayDataReq, req);
        }
        public void OnSinglePayDataRes(NetMsg msg)
        {
            CmdActivitySinglePayDataRes res = NetMsgUtil.Deserialize<CmdActivitySinglePayDataRes>(CmdActivitySinglePayDataRes.Parser, msg);
            listRewardIds.Clear();
            dicRewards.Clear();
            for (int i = 0; i < res.RewardList.Count; i++)
            {
                var reward = res.RewardList[i];
                listRewardIds.Add(reward.InfoId);
                var csvData = CSVSinglePay.Instance.GetConfData(reward.InfoId);
                SinglePayRewardItem rewardData = new SinglePayRewardItem()
                {
                    InfoId = reward.InfoId,
                    ActivityId = csvData.Activity_Id,
                    RewardCount = reward.RewardCount,
                    TakeCount = reward.TakeCount
                };
                //Debug.Log("OnSinglePayDataRes " + reward.InfoId + " | " + reward.RewardCount +" | " + reward.TakeCount);
                dicRewards.Add(reward.InfoId, rewardData);
            }
            eventEmitter.Trigger(EEvents.OnSinglePayDataUpdate);
        }
        /// <summary> 请求领奖 </summary>
        public void ReqSinglePayReward(uint id)
        {
            CmdActivitySinglePayRewardReq req = new CmdActivitySinglePayRewardReq();
            req.InfoId = id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySinglePayRewardReq, req);
        }
        public void OnSinglePayUpdateNtf(NetMsg msg)
        {
            CmdActivitySinglePayUpdateNtf ntf = NetMsgUtil.Deserialize<CmdActivitySinglePayUpdateNtf>(CmdActivitySinglePayUpdateNtf.Parser, msg);
            if (dicRewards.TryGetValue(ntf.InfoId, out SinglePayRewardItem value))
            {
                if (ntf.TakeCount > value.TakeCount)
                {
                    //领奖弹窗
                    var data = CSVSinglePay.Instance.GetConfData(ntf.InfoId);
                    if (data != null)
                    {
                        var listItem = CSVDrop.Instance.GetDropItem(data.reward);
                        UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                        for (int i = 0; i < listItem.Count; i++)
                        {
                            itemRewardParms.itemIds.Add(listItem[i].id);
                            itemRewardParms.itemCounts.Add((uint)listItem[i].count);
                        }
                        UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                    }
                }
                value.RewardCount = ntf.RewardCount;
                value.TakeCount = ntf.TakeCount;
            }
            else
            {
                listRewardIds.Add(ntf.InfoId);
                var csvData = CSVSinglePay.Instance.GetConfData(ntf.InfoId);
                SinglePayRewardItem rewardData = new SinglePayRewardItem()
                {
                    InfoId = ntf.InfoId,
                    ActivityId = csvData.Activity_Id,
                    RewardCount = ntf.RewardCount,
                    TakeCount = ntf.TakeCount
                };
                dicRewards.Add(ntf.InfoId, rewardData);
            }
            eventEmitter.Trigger(EEvents.OnSinglePayDataUpdate);
        }
        #endregion
        #region 服务器消息 合服
        /// <summary> 请求数据 </summary>
        public void ReqSinglePayHeFuData()
        {
            CmdActivitySinglePay2DataReq req = new CmdActivitySinglePay2DataReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySinglePay2DataReq, req);
        }
        public void OnSinglePayHeFuDataRes(NetMsg msg)
        {
            CmdActivitySinglePay2DataRes res = NetMsgUtil.Deserialize<CmdActivitySinglePay2DataRes>(CmdActivitySinglePay2DataRes.Parser, msg);
            listHeFuRewardIds.Clear();
            dicHeFuRewards.Clear();
            for (int i = 0; i < res.RewardList.Count; i++)
            {
                var reward = res.RewardList[i];
                listHeFuRewardIds.Add(reward.InfoId);
                var csvData = CSVSinglePay.Instance.GetConfData(reward.InfoId);
                SinglePayRewardItem rewardData = new SinglePayRewardItem()
                {
                    InfoId = reward.InfoId,
                    ActivityId = csvData.Activity_Id,
                    RewardCount = reward.RewardCount,
                    TakeCount = reward.TakeCount
                };
                //Debug.Log("OnSinglePayHeFuDataRes " + reward.InfoId + " | " + reward.RewardCount + " | " + reward.TakeCount);
                dicHeFuRewards.Add(reward.InfoId, rewardData);
            }
            eventEmitter.Trigger(EEvents.OnSinglePayHeFuDataUpdate);
        }
        /// <summary> 请求领奖 </summary>
        public void ReqSinglePayHeFuReward(uint id)
        {
            CmdActivitySinglePay2RewardReq req = new CmdActivitySinglePay2RewardReq();
            req.InfoId = id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivitySinglePay2RewardReq, req);
        }
        public void OnSinglePayHeFuUpdateNtf(NetMsg msg)
        {
            CmdActivitySinglePay2UpdateNtf ntf = NetMsgUtil.Deserialize<CmdActivitySinglePay2UpdateNtf>(CmdActivitySinglePay2UpdateNtf.Parser, msg);
            if (dicHeFuRewards.TryGetValue(ntf.InfoId, out SinglePayRewardItem value))
            {
                if (ntf.TakeCount > value.TakeCount)
                {
                    //领奖弹窗
                    var data = CSVSinglePay.Instance.GetConfData(ntf.InfoId);
                    if (data != null)
                    {
                        var listItem = CSVDrop.Instance.GetDropItem(data.reward);
                        UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                        for (int i = 0; i < listItem.Count; i++)
                        {
                            itemRewardParms.itemIds.Add(listItem[i].id);
                            itemRewardParms.itemCounts.Add((uint)listItem[i].count);
                        }
                        UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                    }
                }
                value.RewardCount = ntf.RewardCount;
                value.TakeCount = ntf.TakeCount;
            }
            else
            {
                listHeFuRewardIds.Add(ntf.InfoId);
                var csvData = CSVSinglePay.Instance.GetConfData(ntf.InfoId);
                SinglePayRewardItem rewardData = new SinglePayRewardItem()
                {
                    InfoId = ntf.InfoId,
                    ActivityId = csvData.Activity_Id,
                    RewardCount = ntf.RewardCount,
                    TakeCount = ntf.TakeCount
                };
                dicHeFuRewards.Add(ntf.InfoId, rewardData);
            }
            eventEmitter.Trigger(EEvents.OnSinglePayHeFuDataUpdate);
        }
        #endregion

        #region func
        public bool CheckSinglePayIsOpen()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.SingleCharge);
            if (activityInfo != null)
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                bool isActivityTime = nowTime >= curStartTime && nowTime <= curEndTime;
                return isActivityTime && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(120);
            }
            return false;
        }
        public bool CheckSinglePayHefuIsOpen()
        {
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.SingleChargeHeFu);
            if (activityInfo != null)
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                bool isActivityTime = nowTime >= curHeFuStartTime && nowTime <= curHeFuEndTime;
                return isActivityTime && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(120);
            }
            return false;
        }
        public bool CheckSinglePlayRedPoint()
        {
            if (CheckSinglePayIsOpen())
            {
                for (int i = 0; i < listRewardIds.Count; i++)
                {
                    var curRewardId = listRewardIds[i];
                    if (dicRewards.TryGetValue(curRewardId, out SinglePayRewardItem value) && value.ActivityId == curActivityId && value.RewardCount > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckSinglePayHeFuRedPoint()
        {
            if (CheckSinglePayHefuIsOpen())
            {
                for (int i = 0; i < listHeFuRewardIds.Count; i++)
                {
                    var curRewardId = listHeFuRewardIds[i];
                    if (dicHeFuRewards.TryGetValue(curRewardId, out SinglePayRewardItem value) && value.ActivityId == curHeFuActivityId && value.RewardCount > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void InitActivityData()
        {
            timer?.Cancel();
            var activityDatas = CSVOperationalActivityRuler.Instance.GetAll();
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint nextTime = 0;
            for (int i = 0; i < activityDatas.Count; i++)
            {
                var activityData = activityDatas[i];
                if (activityData.Product_Type == 15 && activityData.Activity_Switch == 1)
                {
                    uint startTime = 0;
                    uint endTime = 0;
                    if (activityData.Activity_Type == 3)//开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    else if (activityData.Activity_Type == 2)//限时
                    {
                        startTime = activityData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    if (nowTime >= startTime && nowTime <= endTime)
                    {
                        curActivityId = activityData.id;
                        curStartTime = startTime;
                        curEndTime = endTime;
                        //解析表
                        ParseCsvData();
                        //请求数据
                        if (CheckSinglePayIsOpen())
                        {
                            ReqSinglePayData();
                        }
                        return;
                    }
                    if (nowTime < startTime)
                    {
                        if (nextTime <= 0)
                        {
                            nextTime = startTime;
                        }
                        else
                        {
                            //取最快要开始的活动时间
                            nextTime = startTime < nextTime ? startTime : nextTime;
                        }
                    }
                }
            }
            //当前不在活动时间段内，启动最近一次的倒计时
            if (nextTime > 0)
            {
                uint cd = nextTime - nowTime;
                timer = Timer.Register(cd, InitActivityData, null, false, false);
            }
        }
        private void InitHefuActivityData()
        {
            timerHeFu?.Cancel();
            var activityDatas = CSVOperationalActivityRuler.Instance.GetAll();
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint nextTime = 0;
            for (int i = 0; i < activityDatas.Count; i++)
            {
                var activityData = activityDatas[i];
                if (activityData.Product_Type == 105 && activityData.Activity_Switch == 1)
                {
                    uint startTime = 0;
                    uint endTime = 0;
                    if (activityData.Activity_Type == 3)//开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    else if (activityData.Activity_Type == 2)//限时
                    {
                        startTime = activityData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    if (nowTime >= startTime && nowTime <= endTime)
                    {
                        curHeFuActivityId = activityData.id;
                        curHeFuStartTime = startTime;
                        curHeFuEndTime = endTime;
                        //解析表
                        ParseCsvData();
                        //请求数据
                        if (CheckSinglePayHefuIsOpen())
                        {
                            ReqSinglePayHeFuData();
                        }
                        return;
                    }
                    if (nowTime < startTime)
                    {
                        if (nextTime <= 0)
                        {
                            nextTime = startTime;
                        }
                        else
                        {
                            //取最快要开始的活动时间
                            nextTime = startTime < nextTime ? startTime : nextTime;
                        }
                    }
                }
            }
            //当前不在活动时间段内，启动最近一次的倒计时
            if (nextTime > 0)
            {
                uint cd = nextTime - nowTime;
                timerHeFu = Timer.Register(cd, InitHefuActivityData, null, false, false);
            }
        }
        /// <summary>
        /// 解析表
        /// </summary>
        private void ParseCsvData()
        {
            listTaskId.Clear();
            listHeFuTaskId.Clear();
            List<uint> keyList = new List<uint>();
            List<CSVSinglePay.Data> dataList = new List<CSVSinglePay.Data>();
            List<CSVSinglePay.Data> dataHeFuList = new List<CSVSinglePay.Data>();
            keyList.AddRange(CSVSinglePay.Instance.GetKeys());
            for (int i = 0; i < keyList.Count; i++)
            {
                CSVSinglePay.Data data = CSVSinglePay.Instance.GetConfData(keyList[i]);
                if (data.Activity_Id == curActivityId)
                {
                    dataList.Add(data);
                }
                else if (data.Activity_Id == curHeFuActivityId)
                {
                    dataHeFuList.Add(data);
                }
            }
            //排序
            for (int i = 0; i < dataList.Count - 1; i++)
            {
                bool flag = false;
                for (int j = 0; j < dataList.Count - i - 1; j++)
                {
                    if (dataList[j].Sort > dataList[j + 1].Sort)
                    {
                        var temp = dataList[j];
                        dataList[j] = dataList[j + 1];
                        dataList[j + 1] = temp;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            for (int i = 0; i < dataList.Count; i++)
            {
                listTaskId.Add(dataList[i].id);
            }
            //合服数据排序
            for (int i = 0; i < dataHeFuList.Count - 1; i++)
            {
                bool flag = false;
                for (int j = 0; j < dataHeFuList.Count - i - 1; j++)
                {
                    if (dataHeFuList[j].Sort > dataHeFuList[j + 1].Sort)
                    {
                        var temp = dataHeFuList[j];
                        dataHeFuList[j] = dataHeFuList[j + 1];
                        dataHeFuList[j + 1] = temp;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            for (int i = 0; i < dataHeFuList.Count; i++)
            {
                listHeFuTaskId.Add(dataHeFuList[i].id);
            }
        }
        /// <summary>
        /// 获取剩余次数 最大次数 - 已领取次数
        /// </summary>
        public uint GetResidueCount(uint id)
        {
            var data = CSVSinglePay.Instance.GetConfData(id);
            if (data != null)
            {
                if (dicRewards.TryGetValue(id, out SinglePayRewardItem value))
                {
                    return data.Num - value.TakeCount;
                }
                return data.Num;
            }
            return 0;
        }
        /// <summary>
        /// 获取合服剩余次数 最大次数 - 已领取次数
        /// </summary>
        public uint GetHeFuResidueCount(uint id)
        {
            var data = CSVSinglePay.Instance.GetConfData(id);
            if (data != null)
            {
                if (dicHeFuRewards.TryGetValue(id, out SinglePayRewardItem value))
                {
                    return data.Num - value.TakeCount;
                }
                return data.Num;
            }
            return 0;
        }
        /// <summary>
        /// 检测当前id是否可领取
        /// </summary>
        public bool CheckCanGet(uint id)
        {
            if (dicRewards.TryGetValue(id, out SinglePayRewardItem value))
            {
                return value.RewardCount > 0;
            }
            return false;
        }
        /// <summary>
        /// 检测当前id是否可领取 合服活动
        /// </summary>
        public bool CheckHeFuCanGet(uint id)
        {
            if (dicHeFuRewards.TryGetValue(id, out SinglePayRewardItem value))
            {
                return value.RewardCount > 0;
            }
            return false;
        }
        #endregion

        #region event
        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            InitActivityData();
            InitHefuActivityData();
        }
        private void OnUpdateActivityOperationRuler()
        {
            InitActivityData();
            InitHefuActivityData();
        }
        #endregion

    }
}
