using Framework;
using Google.Protobuf;
using Google.Protobuf.Collections;
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
    /// <summary> 运营活动系统-充值选礼 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        public uint curChargeABActivityId { get; private set; } = 0;
        public uint curChargeABStartTime { get; private set; } = 0;
        public uint curChargeABEndTime { get; private set; } = 0;
        /// <summary>
        /// 当前活动内的累充条目id列表
        /// </summary>
        public List<uint> listIds { get; private set; } = new List<uint>();
        /// <summary>
        /// 奖励状态 id, state
        /// </summary>
        private Dictionary<uint, uint> dictChargeABRewardState = new Dictionary<uint, uint>();

        private uint curChargeABValue = 0;

        private Timer chargeABTimer;

        #endregion

        #region 系统函数
        public void RegisterChargeABEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.CmdActivityChargeCumulateDataNtf, OnChargeABDataNtf, CmdActivityChargeCumulateDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityChargeCumulateRewardReq, (ushort)CmdActivityRuler.CmdActivityChargeCumulateRewardRes, OnChargeABGetRewardRes, CmdActivityChargeCumulateRewardRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityChargeCumulateDataNtf, OnChargeABDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityChargeCumulateRewardRes, OnChargeABGetRewardRes);
            }
        }
        public void ChargeABOnLogin()
        {
            InitChargeABActivityData();
        }
        public void ChargeABOnLogout()
        {
            chargeABTimer?.Cancel();
        }
        public void ChargeABOnDispose()
        {
            chargeABTimer?.Cancel();
        }
        #endregion

        #region 服务器消息
        /// <summary>
        /// 请求充值选礼信息
        /// </summary>
        public void ReqChargeABData()
        {
            CmdActivityChargeCumulateDataReq req = new CmdActivityChargeCumulateDataReq();
            req.ActivityId = curChargeABActivityId;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityChargeCumulateDataReq, req);
        }
        /// <summary>
        /// 充值选礼数据回调
        /// </summary>
        public void OnChargeABDataNtf(NetMsg msg)
        {
            CmdActivityChargeCumulateDataNtf ntf = NetMsgUtil.Deserialize<CmdActivityChargeCumulateDataNtf>(CmdActivityChargeCumulateDataNtf.Parser, msg);
            curChargeABValue = ntf.CumulateValue;
            var rewardList = ntf.Rewards;
            for (int i = 0; i < listIds.Count; i++)
            {
                var curId = listIds[i];
                CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(curId);
                if (data.Sort < rewardList.Count)
                {
                    var curReward = rewardList[(int)data.Sort];
                    if (dictChargeABRewardState.ContainsKey(data.Sort))
                    {
                        dictChargeABRewardState[data.Sort] = curReward;
                    }
                }
            }
            //string str = "";
            //for (int i = 0; i < rewardList.Count; i++)
            //{
            //    str += rewardList[i].ToString() + " | ";
            //}
            //Debug.Log("充值选里数据同步" + curChargeABValue + "| " + str);
            eventEmitter.Trigger(EEvents.UpdateChargeABData);
        }
        /// <summary>
        /// 充值选礼领奖请求
        /// </summary>
        public void ReqChargeABGetReward(uint giftId,uint index)
        {
            CmdActivityChargeCumulateRewardReq req = new CmdActivityChargeCumulateRewardReq();
            req.ActivityId = curChargeABActivityId;
            req.Id = giftId;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityChargeCumulateRewardReq, req);
        }
        /// <summary>
        /// 充值选礼领奖请求回调
        /// </summary>
        public void OnChargeABGetRewardRes(NetMsg msg)
        {
            CmdActivityChargeCumulateRewardRes res = NetMsgUtil.Deserialize<CmdActivityChargeCumulateRewardRes>(CmdActivityChargeCumulateRewardRes.Parser, msg);
            var curId = res.Id;
            if (dictChargeABRewardState.ContainsKey(curId))
            {
                dictChargeABRewardState[curId] = res.Index;
            }
            //Debug.Log("充值选礼领奖请求回调" + res.Id + " | " + res.Index);
            eventEmitter.Trigger(EEvents.UpdateChargeABData);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 检测充值选礼是否开启
        /// </summary>
        public bool CheckChargeABIsOpen()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            bool isActivityTime = nowTime >= curChargeABStartTime && nowTime <= curChargeABEndTime;
            bool isProtect = curChargeABStartTime < Sys_Role.Instance.openServiceGameTime + Sys_ActivityOperationRuler.Instance.protectTerm * 86400;
            //Debug.Log("curChargeABStartTime " + curChargeABStartTime + " openServiceGameTime " + Sys_Role.Instance.openServiceGameTime + " protectTerm " + Sys_ActivityOperationRuler.Instance.protectTerm);
            return isActivityTime && CheckActivitySwitchIsOpen(119) && !isProtect;
        }
        /// <summary>
        /// 充值选礼红点
        /// </summary>
        public bool CheckChargeABRedPoint()
        {
            if (CheckChargeABIsOpen())
            {
                for (int i = 0; i < listIds.Count; i++)
                {
                    var curId = listIds[i];
                    CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(curId);
                    if (data != null && curChargeABValue >= data.Recharge && dictChargeABRewardState.TryGetValue(data.Sort, out uint value) && value == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 初始化充值选礼活动数据 在登录时调用
        /// </summary>
        private void InitChargeABActivityData()
        {
            var activityDatas = CSVOperationalActivityRuler.Instance.GetAll();
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint nextTime = 0;
            for (int i = 0; i < activityDatas.Count; i++)
            {
                var activityData = activityDatas[i];
                if (activityData.Product_Type == (uint)EActivityRulerType.ChargeAB && activityData.Activity_Switch == 1)
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
                    //Debug.Log("充值选礼初始化 " + startTime + "|" + nowTime + "|" + endTime);
                    if (nowTime >= startTime && nowTime <= endTime)
                    {
                        curChargeABActivityId = activityData.id;
                        curChargeABStartTime = startTime;
                        curChargeABEndTime = endTime;
                        //遍历充值选礼活动表
                        List<uint> keyList = new List<uint>();
                        keyList.AddRange(CSVRechargeSelection.Instance.GetKeys());
                        listIds.Clear();
                        dictChargeABRewardState.Clear();
                        List<CSVRechargeSelection.Data> dataList = new List<CSVRechargeSelection.Data>();
                        for (int j = 0; j < keyList.Count; j++)
                        {
                            CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(keyList[j]);
                            if(data.Activity_Id == curChargeABActivityId)
                            {
                                dataList.Add(data);
                                listIds.Add(data.id);
                                dictChargeABRewardState.Add(data.Sort, 0);
                            }
                        }
                        //排序
                        for (int j = 0; j < dataList.Count; j++)
                        {
                            var curData = dataList[j];
                            if (curData.Sort - 1 < listIds.Count)
                            {
                                listIds[(int)curData.Sort - 1] = curData.id;
                            }
                        }
                        //解析完表格再请求活动数据
                        ReqChargeABData();
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
                chargeABTimer = Timer.Register(cd, InitChargeABActivityData, null, false, false);
            }
        }
        /// <summary>
        /// 获取充值选礼包活动到期时间戳
        /// </summary>
        public uint GetChargeABActivityOverTimestamp()
        {
            return curChargeABEndTime;
        }
        /// <summary>
        /// 获取当前充值选礼累计充值额度
        /// </summary>
        public uint GetChargeABTotalChargeNum()
        {
            return curChargeABValue;
        }
        /// <summary>
        /// 获取充值选礼打开时的页签下标
        /// </summary>
        public int GetChargeABOpenIndex()
        {
            for (int i = 0; i < listIds.Count; i++)
            {
                var curId = listIds[i];
                CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(curId);
                if (data!=null &&  dictChargeABRewardState.TryGetValue(data.Sort, out uint value) && value == 0)
                {
                    //策划说不需要满足条件，只要未领取就行
                    //CSVRechargeSelection.Data data = CSVRechargeSelection.Instance.GetConfData(curId);
                    //if (data != null && curChargeABValue >= data.Recharge)
                    //{
                    return i / 2;
                    //}
                }
            }
            return 0;
        }
        /// <summary>
        /// 检测充值AB奖励领取状态
        /// </summary>
        public uint CheckChargeABRewardGetState(uint id)
        {
            if (dictChargeABRewardState.TryGetValue(id, out uint value))
            {
                return value;
            }
            return 0u;
        }
        #endregion
    }
}
