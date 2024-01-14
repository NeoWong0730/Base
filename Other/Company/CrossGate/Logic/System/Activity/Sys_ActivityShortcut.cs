using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_ActivityShortcut : SystemModuleBase<Sys_ActivityShortcut>
    {
        #region 系统函数
        public override void Init()
        {
            isLogin = false;
            ProcessEvents(true);
        }
        public override void OnLogin()
        {
            InitData();
        }
        public override void OnLogout()
        {
            curDailyActivityData = null;
            lastFinishedActivityData = null;
            isCanShow = false;
            finishedList.Clear();
        }
        public override void Dispose()
        {
            isLogin = false;
            isCanShow = false;
            ProcessEvents(false);
            ClearData();
        }
        #endregion
        #region 数据定义
        public enum EEvents
        {
            OnRefreshActivity,            //刷新活动
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        bool isLogin;
        List<DailyActivityData> dailyActivityDataList = new List<DailyActivityData>();
        List<uint> finishedList = new List<uint>();
        DailyActivityData lastFinishedActivityData;
        public DailyActivityData curDailyActivityData { get; set; }
        bool isCanShow;
        bool isOnInstance;
        public bool isHaveEventTask;
        #endregion
        #region 事件
        private void ProcessEvents(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceEnter, OnInstanceEnter, toRegister);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceExit, OnInstanceExit, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, OnLoadMapOk, toRegister);
            Sys_Instance.Instance.eventEmitter.Handle(Sys_Instance.EEvents.InstanceDataUpdate, OnDailyValueChange, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, EndExit, toRegister);
            eventEmitter.Handle(EEvents.OnRefreshActivity, OnDailyValueChange, toRegister);
        }
        private void OnInstanceEnter()
        {
            isOnInstance = true;
        }
        private void OnInstanceExit()
        {
            isCanShow = true;
            isOnInstance = false;
        }
        private void OnLoadMapOk()
        {
            if (isCanShow)
            {
                isCanShow = false;
                CheckDailyActivity();
                ShowTip();
            }
        }
        private void OnDailyValueChange()
        {
            if (dailyActivityDataList != null && dailyActivityDataList.Count > 0)
            {
                for (int i = 0; i < dailyActivityDataList.Count; i++)
                {
                    bool isFinished = CheckActivity(dailyActivityDataList[i].activityData);
                    bool isHave = CheckFinishedList(dailyActivityDataList[i].tid);
                    if (isHave)
                    {
                        if (!isFinished)
                        {
                            lastFinishedActivityData = null;
                            finishedList.Remove(dailyActivityDataList[i].tid);
                        }
                    }
                    else
                    {
                        if (isFinished)
                            finishedList.Add(dailyActivityDataList[i].tid);
                    }
                }
            }
        }
        private void EndExit(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_ClassicBossWarResult || eId == EUIID.UI_MessageBox_Tip || eId == EUIID.UI_RewardsShow)
            {
                CheckDailyActivity();
                if ((eId == EUIID.UI_MessageBox_Tip && isHaveEventTask) || isOnInstance)
                    return;
                ShowTip();
            }
        }
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            curDailyActivityData = null;
            lastFinishedActivityData = null;
            isCanShow = false;
            finishedList.Clear();
            if (!isLogin)
            {
                isLogin = true;
                var dailyActivitys = CSVDailyActivity.Instance.GetAll();
                for (int i = 0; i < dailyActivitys.Count; i++)
                {
                    if (dailyActivitys[i].DragonRecommend != 0)
                    {
                        DailyActivityData data = new DailyActivityData()
                        {
                            tid = dailyActivitys[i].id,
                            recommendId = dailyActivitys[i].DragonRecommend,
                            activityData = dailyActivitys[i],
                        };
                        dailyActivityDataList.Add(data);
                    }
                }
                dailyActivityDataList.Sort((a, b) => {
                    return (int)(a.recommendId - b.recommendId);
                });
            }
        }
        private void ClearData()
        {
            curDailyActivityData = null;
            lastFinishedActivityData = null;
            dailyActivityDataList.Clear();
            finishedList.Clear();
        }
        #region function
        private bool CheckActivity(CSVDailyActivity.Data data)
        {
            bool isFinished = false;
            if (Sys_FunctionOpen.Instance.IsOpen(data.FunctionOpenid) && Sys_Daily.Instance.isTodayDaily(data.id))
            {
                if (data.limite > 0 && Sys_Daily.Instance.getDailyCurTimes(data.id) >= data.limite)
                {
                    isFinished = true;
                }
            }
            return isFinished;
        }
        private bool CheckFinishedList(uint id)
        {
            bool isHave = false;
            if (finishedList != null && finishedList.Count > 0)
            {
                for (int i = 0; i < finishedList.Count; i++)
                {
                    if (id == finishedList[i])
                        isHave = true;
                }
            }
            return isHave;
        }
        public void CheckDailyActivity()
        {
            curDailyActivityData = null;
            int startIndex = -1;
            if (dailyActivityDataList != null && dailyActivityDataList.Count > 0)
            {
                if (lastFinishedActivityData == null)
                {
                    OnDailyValueChange();
                    for (int i = 0; i < dailyActivityDataList.Count; i++)
                    {
                        uint id = GetCurFinishedActivity();
                        if (id == dailyActivityDataList[i].tid)
                        {
                            startIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    startIndex = dailyActivityDataList.IndexOf(lastFinishedActivityData);
                }
                if (startIndex != -1)
                {
                    CheckDailyCurTimes(startIndex >= dailyActivityDataList.Count - 1 ? 0 : startIndex);
                    if (curDailyActivityData == null)
                    {
                        startIndex = 0;
                        CheckDailyCurTimes(startIndex);
                    }
                }
            }
        }
        private void CheckDailyCurTimes(int startIndex)
        {
            for (int i = startIndex; i < dailyActivityDataList.Count; i++)
            {
                if (Sys_FunctionOpen.Instance.IsOpen(dailyActivityDataList[i].activityData.FunctionOpenid) && Sys_Daily.Instance.isTodayDaily(dailyActivityDataList[i].tid))
                {
                    if (dailyActivityDataList[i].activityData.limite > 0 && Sys_Daily.Instance.getDailyCurTimes(dailyActivityDataList[i].tid) < dailyActivityDataList[i].activityData.limite)
                    {
                        curDailyActivityData = dailyActivityDataList[i];
                        break;
                    }
                }
            }
        }
        private uint GetCurFinishedActivity()
        {
            uint id = 0;
            if (finishedList != null && finishedList.Count > 0)
            {
                id = finishedList[finishedList.Count - 1];
            }
            return id;
        }
        private void ShowTip()
        {
            if (curDailyActivityData != null)
            {
                if (lastFinishedActivityData != null && lastFinishedActivityData == curDailyActivityData)
                    return;
                UIManager.OpenUI(EUIID.UI_ActivityShortcut);
                lastFinishedActivityData = curDailyActivityData;
            }
        }
        #endregion

        public class DailyActivityData
        {
            public uint tid;
            public uint recommendId;
            public CSVDailyActivity.Data activityData;
        }
    }
}