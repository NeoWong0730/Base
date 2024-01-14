using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using Logic;
using System;
using UnityEngine;
using Framework;
using UnityEngine.UI;
public class Sys_BackAssist : SystemModuleBase<Sys_BackAssist>
{
    #region 数据定义
    public bool IsBackAssistOpen { get; private set; }
    public uint TotalLovePoint { get; private set; }
    public uint TodayLovePoint
    {
        get
        {
            uint m_todayPoint = 0;
            foreach (var item in LovePointDictionary)
            {
                m_todayPoint += item.Value;
            }
            return m_todayPoint;
        }
    }
    public Dictionary<uint, uint> LovePointDictionary = new Dictionary<uint, uint>();//热心值表uid/value(每日明细)
    public Dictionary<uint, uint> LoveRewardDictionary = new Dictionary<uint, uint>();//热心值奖励表uid/value代表兑换次数（0未兑换）
    private Timer m_Timer;
    public enum EEvents : int
    {
        UpdateActivityReturnLovePointData,
        ActivityReturnLovePointAwardRes,
        ActivityReturnRedPoint,
    }
    public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
    #endregion
    #region 系统函数
    private void ProcessEvents(bool toRegister)
    {
        if (toRegister)
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdActivityReturn.LovePointDataReq, (ushort)CmdActivityReturn.LovePointDataRes, OnActivityReturnLovePointDataRes, CmdActivityReturnLovePointDataRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdActivityReturn.LovePointRewardReq, (ushort)CmdActivityReturn.LovePointRewardRes, OnActivityReturnLovePointRewardRes, CmdActivityReturnLovePointRewardRes.Parser);
        }
        else
        {
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityReturn.LovePointDataRes, OnActivityReturnLovePointDataRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityReturn.LovePointRewardRes, OnActivityReturnLovePointRewardRes);
        }
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, LovePointDataInit, toRegister);
        Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
    }
    public override void Init()
    {
        ProcessEvents(true);
    }
    public override void OnLogin()
    {
        base.OnLogin();
        IsBackAssistOpen = false;
    }
    public override void OnLogout()
    {
        base.OnLogout();
        m_Timer?.Cancel();
        LovePointDictionary.Clear();
        LoveRewardDictionary.Clear();
    }
    public override void Dispose()
    {
        ProcessEvents(false);
    }
    #endregion
    #region net
    /// <summary>
    /// 获取热心值数据
    /// </summary>
    public void OnActivityReturnLovePointDataReq()
    {
        CmdActivityReturnLovePointDataReq req = new CmdActivityReturnLovePointDataReq();
        NetClient.Instance.SendMessage((ushort)CmdActivityReturn.LovePointDataReq, req);
    }
    /// <summary>
    /// 热心值数据返回
    /// </summary>
    /// <param name="msg"></param>
    private void OnActivityReturnLovePointDataRes(NetMsg msg)
    {
        CmdActivityReturnLovePointDataRes res = NetMsgUtil.Deserialize<CmdActivityReturnLovePointDataRes>(CmdActivityReturnLovePointDataRes.Parser, msg);
        TotalLovePoint = res.LovePoint;
        for (int i = 0; i < res.DailyList.Count; i++)
        {
            LovePointDictionary[res.DailyList[i].Id] = res.DailyList[i].Value;
        }
        for (int i = 0; i < res.Rewards.Count; i++)
        {
            LoveRewardDictionary[res.Rewards[i].Id] = res.Rewards[i].Value;
        }
        ReqTickTimer();
        eventEmitter.Trigger(EEvents.UpdateActivityReturnLovePointData);
        eventEmitter.Trigger(EEvents.ActivityReturnRedPoint);
    }
    /// <summary>
    /// 兑换请求
    /// </summary>
    public void OnActivityReturnLovePointRewardReq(uint _index)
    {
        CmdActivityReturnLovePointRewardReq req = new CmdActivityReturnLovePointRewardReq();
        req.Index = _index;
        NetClient.Instance.SendMessage((ushort)CmdActivityReturn.LovePointRewardReq, req);
    }
    /// <summary>
    /// 兑换返回
    /// </summary>
    /// <param name="msg"></param>
    private void OnActivityReturnLovePointRewardRes(NetMsg msg)
    {
        CmdActivityReturnLovePointRewardRes res = NetMsgUtil.Deserialize<CmdActivityReturnLovePointRewardRes>(CmdActivityReturnLovePointRewardRes.Parser, msg);
        TotalLovePoint = res.LovePoint;
        LoveRewardDictionary[res.Index] += 1;
        eventEmitter.Trigger(EEvents.ActivityReturnLovePointAwardRes);
        eventEmitter.Trigger(EEvents.ActivityReturnRedPoint);
    }
    private void OnTimeNtf(uint oldTime, uint newTime)
    {
        ReqTickTimer();
    }
    #endregion
    #region Fuction

    private void ReqTickTimer()
    {
        DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
        DateTime m_nowDayFive= _nowTime.Date.AddHours(5);
        double _duration = 0;
        if (_nowTime< m_nowDayFive)
        {
            _duration = m_nowDayFive.Subtract(_nowTime).TotalSeconds+1;
        }
        else
        {
            DateTime m_nextDayFive = _nowTime.AddDays(1).Date.AddHours(5);
            _duration = m_nextDayFive.Subtract(_nowTime).TotalSeconds + 1;
        }
        m_Timer?.Cancel();
        m_Timer = Timer.Register((float)_duration, () =>
        {
            LovePointDictionaryInit();
        }, null, true);
    }
    public void LovePointDataInit()
    {
        if (IsBackAssistOpen) return;

        var param = CSVReturnParam.Instance.GetConfData(7);
        if (Sys_Role.Instance.Role.Level >= int.Parse(param.str_value))
        {
            IsBackAssistOpen = true;
            LovePointDictionaryInit();
        }
    }
    private void LovePointDictionaryInit()
    {
        var _date = CSVReturnArdent.Instance.GetAll();
        for (int i = 0; i < _date.Count; i++)
        {
            LovePointDictionary[_date[i].id] = 0;
        }
        var _rData = CSVReturnArdentReward.Instance.GetAll();
        for (int i = 0; i < _rData.Count; i++)
        {
            LoveRewardDictionary[_rData[i].id] = 0;
        }

        OnActivityReturnLovePointDataReq();
    }
    /// <summary>
    /// 回归援助是否开启
    /// </summary>
    /// <returns></returns>
    public bool CheckActivityReturnLovePointOpen()
    {
        return IsBackAssistOpen&& BackAssistFunctionOpen();
    }
    /// <summary>
    /// 回归援助红点
    /// </summary>
    /// <returns></returns>
    public bool ActivityReturnLovePointRedPoint()
    {
        if (!CheckActivityReturnLovePointOpen())
        {
            return false;
        }
        var _pData = CSVReturnParam.Instance.GetConfData(5);//热心值总上限
        if (TotalLovePoint>=uint.Parse(_pData.str_value))
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 回归援助开关
    /// </summary>
    /// <returns></returns>
    public bool BackAssistFunctionOpen()
    {
        return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(122);
    }
    #endregion
}
