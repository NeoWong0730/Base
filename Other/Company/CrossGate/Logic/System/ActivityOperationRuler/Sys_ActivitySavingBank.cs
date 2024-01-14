using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 鼠王存钱活动
    /// </summary>
    public class Sys_ActivitySavingBank : SystemModuleBase<Sys_ActivitySavingBank>
    {
        #region 系统函数
        public override void Init()
        {
            ProcessEvents(true);
            InitData();
        }
        public override void OnLogin()
        {

        }
        public override void OnLogout()
        {
            ClearData();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
            ClearData();
            csvSavingBankDataDic.Clear();
        }
        #endregion
        #region 数据
        public class SavingsBankData
        {
            public uint itemId;
            public uint haveCount;   //拥有数量
            public ulong consumeCount;//消费的货币数量
            public uint saveCount;   //已存入数量
            public uint expenseRatio;//消费比例
            public uint rebateRatio;   //返还比例数量
            public uint upperLimit;  //存入上限

            public void SetHaveCount(bool isAdd)
            {
                haveCount = (uint)Math.Floor((double)consumeCount / expenseRatio);
                if (isAdd)
                {
                    if (haveCount + saveCount >= upperLimit)
                        haveCount = upperLimit - saveCount;
                }
                else
                {
                    if (haveCount + saveCount >= upperLimit)
                        haveCount = 0;
                }
            }
            public bool CheckIsUpperLimit()
            {
                return saveCount >= upperLimit;
            }
            public ulong GetRebateCount()
            {
                return (ulong)(saveCount * rebateRatio);
            }
        }
        public enum ESavingsBankState
        {
            None = 0,
            SaveTerm = 1,  //存钱期
            RebateTerm = 2,//返利期
        }

        Dictionary<uint, CSVSavingsBank.Data> csvSavingBankDataDic = new Dictionary<uint, CSVSavingsBank.Data>();
        public List<SavingsBankData> curSavingsBankDataList = new List<SavingsBankData>();
        CSVOperationalActivityRuler.Data curCSVActivityRulerData;
        CSVSavingsBank.Data curSavingBankData;
        Timer saveEndTimer;
        Timer endTimer;
        uint rebateDay;
        //开始时间
        public uint startTime { get; private set; }
        //存钱结束时间
        public uint saveEndTime { get; private set; }
        //返利结束时间
        public uint allEndTime { get; private set; }
        //是否已领取返利
        public bool isGetRebate { get; private set; }
        //当前活动状态
        public ESavingsBankState curSavingsBankState { get; private set; }

        public enum EEvents
        {
            OnRefreshSavingsBankState = 1, //刷新鼠王存钱活动状态
            OnRefreshSavingsBankData = 2,  //刷新存钱数据
            OnRefreshRedPoint = 3,         //刷新红点
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #endregion
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankDataReq, (ushort)CmdActivityRuler.CmdActivityPiggyBankDataRes, OnActivityPiggyBankDataRes, CmdActivityPiggyBankDataRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankSaveReq, (ushort)CmdActivityRuler.CmdActivityPiggyBankSaveRes, OnActivityPiggyBankSaveRes, CmdActivityPiggyBankSaveRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankRewardGetReq, (ushort)CmdActivityRuler.CmdActivityPiggyBankRewardGetRes, OnActivityPiggyBankRewardGetRes, CmdActivityPiggyBankRewardGetRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankDataRes, OnActivityPiggyBankDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankSaveRes, OnActivityPiggyBankSaveRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityPiggyBankRewardGetRes, OnActivityPiggyBankRewardGetRes);
            }
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, OnUpdateActivityOperationRuler, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
            //Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, RefreshActivityState, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, BeginEnter, toRegister);
        }

        private void BeginEnter(uint arg1, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_Menu)
                OnActivityPiggyBankDataReq();
        }

        //private void RefreshActivityState()
        //{
        //    eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
        //}
        private void OnUpdateActivityOperationRuler()
        {
            ActivityInfo info = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.SavingBank);
            if (info != null)
            {
                curCSVActivityRulerData = info.csvData;
                curSavingBankData = GetCSVSavingsBankData(info.infoId);
                startTime = TimeManager.ConvertFromZeroTimeZone(curCSVActivityRulerData.Begining_Date);
                saveEndTime = startTime + (curCSVActivityRulerData.Duration_Day - rebateDay) * 86400;
                allEndTime = startTime + curCSVActivityRulerData.Duration_Day * 86400;
                curSavingsBankState = CheckActivityState(); 
                SeAllTimer();
                SetData();
                OnActivityPiggyBankDataReq();
            }
        }
        private void OnCurrencyChanged(uint itemId, long value)
        {
            if (GetSavingsBankData(itemId) != null)
                OnActivityPiggyBankDataReq();
        }
        #region Res
        private void OnActivityPiggyBankDataRes(NetMsg msg)
        {
            CmdActivityPiggyBankDataRes ntf = NetMsgUtil.Deserialize<CmdActivityPiggyBankDataRes>(CmdActivityPiggyBankDataRes.Parser, msg);
            if (ntf != null)
            {
                if (ntf.PiggyBankData!=null)
                {
                    isGetRebate = ntf.PiggyBankData.IsGet;
                    if (ntf.PiggyBankData.CoinList.Count > 0)
                    {
                        for (int i = 0; i < ntf.PiggyBankData.CoinList.Count; i++)
                        {
                            SavingsBankData data= GetSavingsBankData(ntf.PiggyBankData.CoinList[i].CoinId);
                            if (data != null)
                            {
                                data.consumeCount = ntf.PiggyBankData.CoinList[i].ConsumeCount;
                                data.saveCount = ntf.PiggyBankData.CoinList[i].SaveCount;
                                data.SetHaveCount(true);
                            }
                        }
                    }
                }
                eventEmitter.Trigger(EEvents.OnRefreshSavingsBankData);
                eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
            }
        }
        private void OnActivityPiggyBankSaveRes(NetMsg msg)
        {
            CmdActivityPiggyBankSaveRes ntf = NetMsgUtil.Deserialize<CmdActivityPiggyBankSaveRes>(CmdActivityPiggyBankSaveRes.Parser, msg);
            if (ntf != null)
            {
                if (ntf.PiggyBankData != null)
                {
                    isGetRebate = ntf.PiggyBankData.IsGet;
                    if (ntf.PiggyBankData.CoinList.Count > 0)
                    {
                        for (int i = 0; i < ntf.PiggyBankData.CoinList.Count; i++)
                        {
                            SavingsBankData data = GetSavingsBankData(ntf.PiggyBankData.CoinList[i].CoinId);
                            if (data != null)
                            {
                                data.consumeCount = ntf.PiggyBankData.CoinList[i].ConsumeCount;
                                data.saveCount = ntf.PiggyBankData.CoinList[i].SaveCount;
                                data.SetHaveCount(false);
                            }
                        }
                    }
                }
                eventEmitter.Trigger(EEvents.OnRefreshSavingsBankData);
                eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
            }
        }
        private void OnActivityPiggyBankRewardGetRes(NetMsg msg)
        {
            CmdActivityPiggyBankRewardGetRes ntf = NetMsgUtil.Deserialize<CmdActivityPiggyBankRewardGetRes>(CmdActivityPiggyBankRewardGetRes.Parser, msg);
            if (ntf != null)
            {
                isGetRebate = true;
                if (ntf.Rewards.Count > 0)
                {
                    UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                    for (int i = 0; i < ntf.Rewards.Count; i++)
                    {
                        itemRewardParms.itemIds.Add(ntf.Rewards[i].Id);
                        itemRewardParms.itemCounts.Add(ntf.Rewards[i].Num);
                    }
                    UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                }
                eventEmitter.Trigger(EEvents.OnRefreshSavingsBankData);
                eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
            }
        }
        #endregion
        #region Req
        public void OnActivityPiggyBankDataReq()
        {
            CmdActivityPiggyBankDataReq req = new CmdActivityPiggyBankDataReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityPiggyBankDataReq, req);
        }
        public void OnActivityPiggyBankSaveReq()
        {
            if (CheckSaveCondition())
            {
                CmdActivityPiggyBankSaveReq req = new CmdActivityPiggyBankSaveReq();
                NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityPiggyBankSaveReq, req);
            }
        }
        public void OnActivityPiggyBankRewardGetRes()
        {
            if (CheckGetCondition())
            {
                CmdActivityPiggyBankRewardGetReq req = new CmdActivityPiggyBankRewardGetReq();
                NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityPiggyBankRewardGetReq, req);
            }
        }
        #endregion
        private void InitData()
        {
            rebateDay = uint.Parse(CSVParam.Instance.GetConfData(1410).str_value);
            csvSavingBankDataDic.Clear();
            var dataList = CSVSavingsBank.Instance.GetAll();
            for (int i = 0; i < dataList.Count; i++)
            {
                csvSavingBankDataDic[dataList[i].Activity_Id] = dataList[i];
            }
        }
        private void ClearData()
        {
            saveEndTimer?.Cancel();
            endTimer?.Cancel();
            curSavingsBankDataList.Clear();
            curCSVActivityRulerData = null;
            curSavingBankData = null;
        }
        private void SetData()
        {
            if (curSavingBankData != null)
            {
                curSavingsBankDataList.Clear();
                for (int i = 0; i < curSavingBankData.Consume.Count; i++)
                {
                    SavingsBankData data = new SavingsBankData
                    {
                        itemId = curSavingBankData.Consume[i][0],
                        haveCount = 0,
                        saveCount = 0,
                        expenseRatio = curSavingBankData.Consume[i][1],
                        rebateRatio = curSavingBankData.Rebate[i][1],
                        upperLimit = curSavingBankData.Upper[i]
                    };
                    curSavingsBankDataList.Add(data);
                }
                curSavingsBankDataList.Sort((a,b)=> {
                    return (int)(b.itemId - a.itemId);
                });
            }
        }
        private CSVSavingsBank.Data GetCSVSavingsBankData(uint id)
        {
            if (csvSavingBankDataDic.ContainsKey(id))
                return csvSavingBankDataDic[id];
            return null;
        }
        private void SeAllTimer()
        {
            //设置存钱结束计时器
            float duration = saveEndTime - TimeManager.GetServerTime();
            if (duration > 0)
            {
                saveEndTimer?.Cancel();
                saveEndTimer = Timer.Register(duration, () => {
                    saveEndTimer?.Cancel();
                    curSavingsBankState = CheckActivityState();
                    eventEmitter.Trigger(EEvents.OnRefreshSavingsBankState);
                    eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
                }, null, false, true);
            }

            //设置活动结束计时器
            float duration_1 = allEndTime - TimeManager.GetServerTime();
            if (duration_1 > 0)
            {
                endTimer?.Cancel();
                endTimer = Timer.Register(duration_1, () => {
                    endTimer?.Cancel();
                    curSavingsBankState = CheckActivityState();
                    curCSVActivityRulerData = null;
                    curSavingBankData = null;
                    eventEmitter.Trigger(EEvents.OnRefreshSavingsBankState);
                }, null, false, true);
            }
        }
        /// <summary>
        /// 活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool ActivityIsOpen()
        {
            bool isOpen = true;
            if (curCSVActivityRulerData == null || curCSVActivityRulerData.Activity_Switch == 0 || curSavingBankData==null)
                isOpen = false;
            return isOpen && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(213);
        }
        /// <summary>
        /// 检查红点
        /// </summary>
        /// <returns></returns>
        public bool CheckRedPoint()
        {
            bool isHave = false;
            if (!ActivityIsOpen())
                isHave = false;
            else
            {
                curSavingsBankState = CheckActivityState();
                switch (curSavingsBankState)
                {
                    case ESavingsBankState.None:
                        isHave = false;
                        break;
                    case ESavingsBankState.SaveTerm:
                        for (int i = 0; i < curSavingsBankDataList.Count; i++)
                        {
                            if (!curSavingsBankDataList[i].CheckIsUpperLimit())//未达到上限
                            {
                                if (curSavingsBankDataList[i].haveCount > 0)//有代币
                                {
                                    isHave = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case ESavingsBankState.RebateTerm:
                        uint count = GetTokenCount(1);
                        if (count > 0)
                            isHave = !isGetRebate;
                        break;
                    default:
                        break;
                }
            }
            return isHave;
        }
        /// <summary>
        /// 检查活动期间状态
        /// </summary>
        /// <returns></returns>
        public ESavingsBankState CheckActivityState()
        {
            ESavingsBankState state = ESavingsBankState.None;
            if (startTime <= TimeManager.GetServerTime() && allEndTime > TimeManager.GetServerTime())
            {
                //存钱期
                if (saveEndTime > TimeManager.GetServerTime())
                    state = ESavingsBankState.SaveTerm;
                //返利期
                else
                    state = ESavingsBankState.RebateTerm;
            }
            return state;
        }
        public SavingsBankData GetSavingsBankData(uint itemId)
        {
            return curSavingsBankDataList.Find(o => o.itemId == itemId);
        }
        /// <summary>
        /// 检查代币存入条件
        /// </summary>
        /// <returns></returns>
        private bool CheckSaveCondition()
        {
            bool isHaveToken = false;
            bool isAllUpperLimit = true;
            for (int i = 0; i < curSavingsBankDataList.Count; i++)
            {
                if (!curSavingsBankDataList[i].CheckIsUpperLimit())
                {
                    isAllUpperLimit = false;
                    if (curSavingsBankDataList[i].haveCount > 0)
                    {
                        isHaveToken = true;
                        break;
                    }
                }
            }
            if (isAllUpperLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2026013));
                return false;
            }
            if (!isHaveToken)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2026012));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查返利领取条件
        /// </summary>
        /// <returns></returns>
        private bool CheckGetCondition()
        {
            uint count = GetTokenCount(1);
            if (count <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2026011));
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取对应代币的数量
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="type">0(拥有的) 1(存入的) 2(最大上限)</param>
        /// <returns></returns>
        public uint GetTokenCount(uint type, uint itemId = 0)
        {
            uint value = 0;
            if (itemId == 0)
            {
                for (int i = 0; i < curSavingsBankDataList.Count; i++)
                {
                    if (type == 0)
                        value += curSavingsBankDataList[i].haveCount;
                    else if (type == 1)
                        value += curSavingsBankDataList[i].saveCount;
                    else
                        value += curSavingsBankDataList[i].upperLimit;
                }
            }
            else
            {
                SavingsBankData data = GetSavingsBankData(itemId);
                if (data.itemId != 0)
                    value = type == 0 ? data.haveCount : type == 1 ? data.saveCount : data.upperLimit;
            }
            return value;
        }
        public bool CheckIsSaveTerm()
        {
            return curSavingsBankState == ESavingsBankState.SaveTerm;
        }
        public int GetTokenSaveExtent()
        {
            int value;
            uint allSave = GetTokenCount(1);
            uint allLimit = GetTokenCount(2);
            double diff = Math.Round((double)allSave / allLimit,3);
            if (diff <= 0.25)
                value = 0;
            else if (diff <= 0.5)
                value = 1;
            else if (diff <= 0.75)
                value = 2;
            else if (diff >= 1)
                value = 4;
            else
                value = 3;
            return value;
        }
    }
}