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
    public enum EActivityRulerType
    {
        RedEnvelopeRain = 1,    //天降红包
        OneDollarTreasure = 2,  //一元夺宝
        HundredDollarTreasure = 3,//百元夺宝
        PedigreedDraw = 4,    //金宠抽奖
        DiscountMall = 7,     //折扣商店   
        ItemExChange = 8,       //道具兑换
        ActivityQuest = 9,         //活动任务
        LimitedActivityPanel = 10,//限时活动面板
        LimitedActivitySign = 11,//限时活动签到
        TokenMall = 12,        //代币商店 
        SavingBank=13,           //鼠王存钱
        ChargeAB = 14,           //充值选礼
        SingleCharge = 15,           //单笔充值
        SuperMall = 18,         //超值商城
        ActivityTotalCharge=20, //累计充值活动
        ActivityTotalConsume=21, //累计消费活动
        HeFuMall = 101,  //合服商城
        MergeServerSign=102,//合服签到
        HeFuActivityQuest = 103,         //合服活动任务
        HeFuItemExChange = 104,         //合服道具兑换
        SingleChargeHeFu = 105,           //单笔充值合服
    }
    public class Sys_ActivityOperationRuler : SystemModuleBase<Sys_ActivityOperationRuler>
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
            activityDic.Clear();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        #endregion
        #region 数据
        Dictionary<EActivityRulerType, ActivityInfo> activityDic = new Dictionary<EActivityRulerType, ActivityInfo>();
        /// <summary> 开服保护天数 </summary>
        public uint protectTerm;
        public enum EEvents
        {
            OnRefreshActivityInfo = 1,//刷新活动数据
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #endregion
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.ActivityListNtf, ActivityListNtf, CmdActivityRulerActivityListNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityDataReq, (ushort)CmdActivityRuler.CmdActivityDataNtf, OnActivityDataNtf, CmdActivityDataNtf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.ActivityListNtf, ActivityListNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityDataNtf, OnActivityDataNtf);
            }
        }

        private void ActivityListNtf(NetMsg msg)
        {
            CmdActivityRulerActivityListNtf ntf = NetMsgUtil.Deserialize<CmdActivityRulerActivityListNtf>(CmdActivityRulerActivityListNtf.Parser, msg);
            activityDic.Clear();
            if (ntf.ActivityInfo != null && ntf.ActivityInfo.Count > 0)
            {
                for (int i = 0; i < ntf.ActivityInfo.Count; i++)
                {
                    CSVOperationalActivityRuler.Data csvData = CSVOperationalActivityRuler.Instance.GetConfData(ntf.ActivityInfo[i].InfoId);
                    if (csvData != null)
                    {
                        ActivityInfo info = new ActivityInfo();
                        info.infoId = ntf.ActivityInfo[i].InfoId;
                        info.currDay = ntf.ActivityInfo[i].CurrDay;
                        info.csvData = csvData;
                        activityDic[(EActivityRulerType)csvData.Product_Type] = info;
                    }
                    else
                    {
                        DebugUtil.LogError("CSVOperationalActivityRuler not found id：" + ntf.ActivityInfo[i].InfoId);
                    }
                }
                eventEmitter.Trigger(EEvents.OnRefreshActivityInfo);
            }
        }
        private void InitData()
        {
            protectTerm = uint.Parse(CSVParam.Instance.GetConfData(1302).str_value);
        }

        public int GetOpenServiceDay()
        {
            DateTime curServerDateTime = TimeManager.START_TIME.AddSeconds(TimeManager.GetServerTime());
            DateTime curServerStartDateTime = TimeManager.START_TIME.AddSeconds(Sys_Role.Instance.openServiceGameTime);
            DateTime time1 = new DateTime(curServerDateTime.Year, curServerDateTime.Month, curServerDateTime.Day, 0, 0, 0);
            DateTime time2 = new DateTime(curServerStartDateTime.Year, curServerStartDateTime.Month, curServerStartDateTime.Day, 0, 0, 0);
            TimeSpan span = time1.Subtract(time2);
            return span.Days + 1;
        }
        CheckActivityData checkData;
        /// <summary>
        /// 根据活动类型获取当天是否是活动日(有预告的活动本地会算出最近一天的活动日)
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        public CheckActivityData ChechActivityDay(EActivityRulerType activityType)
        {
            if (checkData == null)
                checkData = new CheckActivityData();

            checkData.curCSVActivityRulerData = null;
            DateTime dateTime = TimeManager.START_TIME.AddSeconds(TimeManager.GetServerTime());
            uint curYear = (uint)dateTime.Year;
            uint curMonth = (uint)dateTime.Month;

            int openServiceDay = GetOpenServiceDay();

            ActivityInfo activityInfo = GetActivityInfo(activityType);
            //服务器推送的活动数据
            if (activityInfo != null)
            {
                checkData.isActivityDay = true;
                checkData.curCSVActivityRulerData = activityInfo.csvData;
            }
            else
            {
                //如果确定当前类型活动不需要提前预告，可不进行以下操作，直接根据服务器推送数据来 活动类型在EActivityRulerType里自行添加
                //if (activityType == xxxxxx)
                //    return checkData;
                checkData.isActivityDay = false;
                int startDay = -1;
                int minStartDay = 0;
                int minStartDayIndex = -1;

                var operationalActivityRulers = CSVOperationalActivityRuler.Instance.GetAll();
                for (int i = 0, len = operationalActivityRulers.Count; i < len; i++)
                {
                    CSVOperationalActivityRuler.Data data = operationalActivityRulers[i];
                    if (data.Product_Type == (uint)activityType)
                    {
                        if (protectTerm >= openServiceDay)
                        {
                            if (data.Activity_Type == 3)
                            {
                                if (openServiceDay <= data.Begining_Date)
                                {
                                    int diffDay = (int)(openServiceDay - data.Begining_Date);
                                    startDay = dateTime.Subtract(TimeSpan.FromDays(diffDay)).Day;
                                }
                            }
                        }
                        else
                        {
                            if (data.Activity_Type == 1)
                            {
                                startDay = (int)data.Begining_Date;
                            }
                            else if (data.Activity_Type == 2)
                            {
                                DateTime time = TimeManager.START_TIME.AddSeconds(TimeManager.ConvertFromZeroTimeZone(data.Begining_Date));
                                if (curYear == time.Year && curMonth == time.Month)
                                    startDay = time.Day;
                            }
                            else if (data.Activity_Type == 4)
                            {
                                if (curMonth % 2 == 1)
                                    startDay = (int)data.Begining_Date;
                            }
                            else if (data.Activity_Type == 5)
                            {
                                if (curMonth % 2 == 0)
                                    startDay = (int)data.Begining_Date;
                            }
                        }

                        if (startDay != -1)
                        {
                            if (minStartDay == 0)
                            {
                                minStartDay = startDay;
                                minStartDayIndex = i;
                            }
                            else
                            {
                                if (minStartDay > startDay)
                                {
                                    minStartDay = startDay;
                                    minStartDayIndex = i;
                                }
                            }
                        }
                    }
                }
                if (minStartDayIndex == -1)
                {
                    checkData.curCSVActivityRulerData = null;
                }
                else
                {
                    checkData.curCSVActivityRulerData = operationalActivityRulers[minStartDayIndex];
                    //没有提前预告，按服务器推送走
                    if (checkData.curCSVActivityRulerData.Open_Bottom == 0 && checkData.curCSVActivityRulerData.RollingLoop_Time == 0)
                        checkData.curCSVActivityRulerData = null;
                }
            }
            return checkData;
        }
        public ActivityInfo GetActivityInfo(EActivityRulerType type)
        {
            if (activityDic.ContainsKey(type))
                return activityDic[type];
            return null;
        }

        public ActivityInfo GetOpenActivity(List<uint> ids)
        {
            foreach (var data in activityDic)
            {
                if (ids.IndexOf(data.Value.infoId) >= 0)
                    return data.Value;
            }

            return null;
        }

        private void OnActivityDataNtf(NetMsg msg)
        {
            CmdActivityDataNtf ntf = NetMsgUtil.Deserialize<CmdActivityDataNtf>(CmdActivityDataNtf.Parser, msg);
            Sys_ActivityTopic.Instance.signRefreshTime = ntf.RefreshTime;
            Sys_ActivityTopic.Instance.isDataReq = false;
            Sys_ActivityTopic.Instance.InitServerSign(0,ntf.SignData);
            Sys_ActivityTopic.Instance.InitServerSign(1,ntf.SignData2);
            Sys_ActivityTopic.Instance.eventEmitter.Trigger(Sys_ActivityTopic.EEvents.OnCommonActivityUpdate);
            Sys_ActivityQuest.Instance.UpdateQuestData(ntf.MissAward);
            Sys_ActivityQuest.Instance.UpdateQuestHeFuData(ntf.MissAward2);
            Sys_ItemExChange.Instance.UpdateExData(ntf.ExData);
            Sys_ItemExChange.Instance.UpdateExHeFuData(ntf.ExData2);
            
        }

        /// <summary>
        /// 检测是否在开服保护期内
        /// </summary>
        public bool CheckIsInProtectTerm()
        {
            return protectTerm >= GetOpenServiceDay();
        }
    }
    public class CheckActivityData
    {
        public CSVOperationalActivityRuler.Data curCSVActivityRulerData;//活动规则数据
        public bool isActivityDay;
    }

    public class ActivityInfo
    {
        public uint infoId;//活动id
        public uint currDay;//当前处于活动的第几天
        public CSVOperationalActivityRuler.Data csvData;//活动规则数据
    }
}