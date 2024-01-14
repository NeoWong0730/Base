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
    public class Sys_Activity_2048 : SystemModuleBase<Sys_Activity_2048>
    {
        /// <summary> 当前活动ID </summary>
        public uint curActivityId { get; private set; }
        /// <summary>
        /// 奖励状态
        /// </summary>
        public uint rewardState;
        /// <summary>
        /// 最好成绩
        /// </summary>
        public uint bestTime;

        public uint curStartTime { get; private set; } = 0;
        public uint curEndTime { get; private set; } = 0;

        private Timer timer;
        public enum EEvents : int
        {
            /// <summary>
            /// 刷新活动2048数据
            /// </summary>
            OnActivity2048DataUpdate,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #region 系统函数
        public override void Init()
        {
            RegisterEvents(true);
        }

        public override void Dispose()
        {
            RegisterEvents(false);
            timer?.Cancel();
        }

        public override void OnLogin()
        {
            InitActivityData();
        }
        public override void OnLogout()
        {
            //清空一下缓存
            curActivityId = 0;
            curStartTime = 0;
            curEndTime = 0;
            rewardState = 0;
            bestTime = 0;
            timer?.Cancel();
        }
        #endregion

        #region 服务器消息
        /// <summary> 请求活动2048数据 </summary>
        public void ReqActivity2048Data()
        {
            CmdActivityMagic2048DataReq req = new CmdActivityMagic2048DataReq();
            req.ActivityId = curActivityId;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityMagic2048DataReq, req);
        }

        public void OnActivity2048DataRes(NetMsg msg)
        {
            CmdActivityMagic2048DataRes ntf = NetMsgUtil.Deserialize<CmdActivityMagic2048DataRes>(CmdActivityMagic2048DataRes.Parser, msg);
            bestTime = ntf.Time;
            rewardState = ntf.Award;
            eventEmitter.Trigger(EEvents.OnActivity2048DataUpdate);
        }

        /// <summary> 请求提交活动2048成绩 </summary>
        public void ReqActivity2048ReportTime(uint time)
        {
            CmdActivityMagic2048ReportTimeReq req = new CmdActivityMagic2048ReportTimeReq();
            req.ActivityId = curActivityId;
            req.Time = time;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityMagic2048ReportTimeReq, req);
        }
        public void OnActivity2048UpdateTimeNtf(NetMsg msg)
        {
            CmdActivityMagic2048UpdateTimeNtf ntf = NetMsgUtil.Deserialize<CmdActivityMagic2048UpdateTimeNtf>(CmdActivityMagic2048UpdateTimeNtf.Parser, msg);
            bestTime = ntf.Time;
            eventEmitter.Trigger(EEvents.OnActivity2048DataUpdate);
        }

        /// <summary> 请求活动2048领奖 </summary>
        public void ReqActivity2048GetAward(uint index)
        {
            CmdActivityMagic2048GetAwardReq req = new CmdActivityMagic2048GetAwardReq();
            req.ActivityId = curActivityId;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityMagic2048GetAwardReq, req);
        }

        public void OnActivity2048UpdateAwardStatusNtf(NetMsg msg)
        {
            CmdActivityMagic2048UpdateAwardStatusNtf ntf = NetMsgUtil.Deserialize<CmdActivityMagic2048UpdateAwardStatusNtf>(CmdActivityMagic2048UpdateAwardStatusNtf.Parser, msg);
            rewardState = ntf.Award;
            eventEmitter.Trigger(EEvents.OnActivity2048DataUpdate);
        }
        #endregion

        #region func
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);

            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityMagic2048DataReq, (ushort)CmdActivityRuler.CmdActivityMagic2048DataRes, OnActivity2048DataRes, CmdActivityMagic2048DataRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.CmdActivityMagic2048UpdateTimeNtf, OnActivity2048UpdateTimeNtf, CmdActivityMagic2048UpdateTimeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityRuler.CmdActivityMagic2048UpdateAwardStatusNtf, OnActivity2048UpdateAwardStatusNtf, CmdActivityMagic2048UpdateAwardStatusNtf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityMagic2048DataRes, OnActivity2048DataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityMagic2048UpdateTimeNtf, OnActivity2048UpdateTimeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityMagic2048UpdateAwardStatusNtf, OnActivity2048UpdateAwardStatusNtf);
            }
        }
        /// <summary>
        /// 判断活动2048是否开启
        /// </summary>
        public bool CheckActivity2048IsOpen()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            bool isActivityTime = nowTime >= curStartTime && nowTime <= curEndTime;
            bool isProtect = curStartTime < Sys_Role.Instance.openServiceGameTime + Sys_ActivityOperationRuler.Instance.protectTerm * 86400;
            return isActivityTime && Sys_FunctionOpen.Instance.IsOpen(50902) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(220) && !isProtect;
        }
        /// <summary>
        /// 初始化活动数据,获取当期活动id，启动更新倒计时
        /// </summary>
        private void InitActivityData()
        {
            timer?.Cancel();
            var activityDatas = CSVOperationalActivityRuler.Instance.GetAll();
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint nextTime = 0;
            for (int i = 0; i < activityDatas.Count; i++)
            {
                var activityData = activityDatas[i];
                if (activityData.Product_Type == 17 && activityData.Activity_Switch == 1)
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
                        if (CheckActivity2048IsOpen())
                        {
                            ReqActivity2048Data();
                        }
                        eventEmitter.Trigger(EEvents.OnActivity2048DataUpdate);
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
        /// <summary>
        /// 检测总红点
        /// </summary>
        public bool CheckAllRedPoint()
        {
            if (bestTime <= 0)
            {
                return false;
            }
            var data = CSVSynthesis.Instance.GetConfData(curActivityId);
            if (data != null)
            {
                var listTime = data.Reward_Time;
                for (int i = 0; i < listTime.Count; i++)
                {
                    var curTime = listTime[i];
                    if(bestTime <= curTime && !CheckRewardIsGet(i))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 根据下标，检测奖励是否已经领取
        /// </summary>
        public bool CheckRewardIsGet(int index)
        {
            return (rewardState >> index & 1) == 1;
        }

        /// <summary>
        /// 获取活动2048按钮倒计时
        /// </summary>
        public uint GetActivity2048BtnCD()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var endTime = curEndTime;
            if (endTime > nowTime)
            {
                return endTime - nowTime;
            }
            return 0;
        }
        /// <summary>
        /// 获取活动2048排名百分比
        /// </summary>
        public uint GetActivity2048RankPercent(uint useTime)
        {
            var csvData = CSVSynthesis.Instance.GetConfData(curActivityId);
            if (csvData != null)
            {
                uint maxNum = 0;
                uint minNum = 0;
                var ListTime = csvData.Time_Section;
                for (int i = 0; i < ListTime.Count; i++)
                {
                    if(useTime < ListTime[i])
                    {
                        if(i == 0)
                        {
                            return csvData.Rank_Section[0];
                        }
                        minNum = csvData.Rank_Section[i];
                        maxNum = csvData.Rank_Section[i - 1];
                        break;
                    }
                }
                if(minNum == 0)
                {
                    return csvData.Rank_Section[csvData.Rank_Section.Count - 1];
                }
                else
                {
                    System.Random r = new System.Random();
                    uint value = (uint)r.Next((int)minNum, (int)maxNum + 1);
                    return value;
                }
            }
            return 0;
        }
        /// <summary>
        /// 检测奖励是否可以领取
        /// </summary>
        public bool CheckRewardCanGet(uint targetTime)
        {
            return bestTime > 0 && bestTime < targetTime;
        }
        /// <summary>
        /// 获取奖励时间文本
        /// </summary>
        public string GetRewardTimeText(uint targetTime)
        {
            string timeString;
            uint second = targetTime % 60;
            if (second > 0)
            {
                timeString = LanguageHelper.GetTextContent(11739, (targetTime / 60).ToString()) + LanguageHelper.GetTextContent(2007268, (targetTime % 60).ToString());
            }
            else
            {
                timeString = LanguageHelper.GetTextContent(10156, (targetTime / 60).ToString());
            }
            return LanguageHelper.GetTextContent(2026202, timeString);
        }
        #endregion

        #region event
        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            InitActivityData();
        }
        #endregion
    }
}
