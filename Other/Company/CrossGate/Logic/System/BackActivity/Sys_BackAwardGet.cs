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

namespace Logic
{
    public class Sys_BackAwardGet : SystemModuleBase<Sys_BackAwardGet>
    {
        public uint BackGroupType { get; private set; } = 1;//所在分组

        public Dictionary<uint, BackAward> BackAwardDictionary = new Dictionary<uint, BackAward>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private Timer m_Timer;

        public uint NowBackAwardType=1;
        public enum EEvents
        {
            OnBackAwardDataUpdate,
        }

        #region 系统函数
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdBackAwardGet.GetDataReq, (ushort)CmdBackAwardGet.GetDataRes, OnBackAwardDataRes, CmdBackAwardGetGetDataRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdBackAwardGet.GetAwardReq, (ushort)CmdBackAwardGet.GetAwardRes, OnBackAwardGetRes, CmdBackAwardGetGetAwardRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBackAwardGet.RedTipsNtf, OnBackAwardGetRedTipsNtf, CmdBackAwardGetRedTipsNtf.Parser);

            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackAwardGet.GetDataRes, OnBackAwardDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackAwardGet.GetAwardRes, OnBackAwardGetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackAwardGet.RedTipsNtf, OnBackAwardGetRedTipsNtf);
            }
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        public override void Init()
        {
            ProcessEvents(true);
        }
        public override void OnLogin()
        {
            base.OnLogin();
        }
        public override void OnLogout()
        {
            base.OnLogout();
            NowBackAwardType = 1;
            m_Timer?.Cancel();
            BackAwardDictionary.Clear();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        #endregion
        #region net
        /// <summary>
        /// 请求奖励找回数据
        /// </summary>
        public void OnBackAwardDataReq(uint activityType)
        {//1-每日 2-限时
            CmdBackAwardGetGetDataReq req = new CmdBackAwardGetGetDataReq();
            req.ActivityType = activityType;
            ReqTickTimer();
            NetClient.Instance.SendMessage((ushort)CmdBackAwardGet.GetDataReq, req);
        }

        /// <summary>
        /// 奖励找回数据返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnBackAwardDataRes(NetMsg msg)
        {
            CmdBackAwardGetGetDataRes res = NetMsgUtil.Deserialize<CmdBackAwardGetGetDataRes>(CmdBackAwardGetGetDataRes.Parser, msg);
            if (BackAwardDictionary.ContainsKey(res.ActivityType))
            {
                BackAwardDictionary[res.ActivityType].UpdateThisBackAward(res.StartTime, res.EndTime, res.AwardGet);
                eventEmitter.Trigger(EEvents.OnBackAwardDataUpdate);
                Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
            }
        }
        /// <summary>
        /// 请求领取找回奖励
        /// </summary>
        public void OnBackAwardGetReq(uint _activityType, uint _awardType, uint _index)
        {//1-活动类型 BackAwardGetActivityType;2-奖励类型 BackAwardGetAwardType;3-奖励所处的index
            CmdBackAwardGetGetAwardReq req = new CmdBackAwardGetGetAwardReq();
            req.ActivityType = _activityType;
            req.AwardType = _awardType;
            req.Index = _index;
            NetClient.Instance.SendMessage((ushort)CmdBackAwardGet.GetAwardReq, req);
        }
        /// <summary>
        /// 领取找回奖励返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnBackAwardGetRes(NetMsg msg)
        {
            CmdBackAwardGetGetAwardRes res = NetMsgUtil.Deserialize<CmdBackAwardGetGetAwardRes>(CmdBackAwardGetGetAwardRes.Parser, msg);
            if (BackAwardDictionary.ContainsKey(res.ActivityType))
            {
                BackAwardDictionary[res.ActivityType].UpdateSingleAwardState(res.AwardType, (int)res.Index);
            }
            eventEmitter.Trigger(EEvents.OnBackAwardDataUpdate);
            Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
            
        }
        /// <summary>
        /// 领取红点主动(只在登录发)
        /// </summary>
        /// <param name="msg"></param>
        private void OnBackAwardGetRedTipsNtf(NetMsg msg)
        {
            CmdBackAwardGetRedTipsNtf ntf = NetMsgUtil.Deserialize<CmdBackAwardGetRedTipsNtf>(CmdBackAwardGetRedTipsNtf.Parser, msg);
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            ReqTickTimer();
        }
        #endregion
        #region DataInit
        public void BackAwardDataInit()
        {
            BackAwardDictionary.Clear();
            BackGroupType = Sys_BackActivity.Instance.ActivityGroup;
            for (uint i = 1; i < (uint)BackAwardType.End; i++)
            {
                BackAward awardCeil = new BackAward((BackAwardType)i);
                awardCeil.InitBackAwardData();
                BackAwardDictionary.Add(i, awardCeil);
            }
            OnBackAwardDataReq(1);
            OnBackAwardDataReq(2);
        }

        #endregion
        #region Fuction
        /// <summary>
        /// 跨零点刷新请求
        /// </summary>
        private void ReqTickTimer()
        {
            if (!Sys_BackActivity.Instance.CheckBackActivityIsOpen())
            {
                return;
            }
            DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            DateTime _nextDayZero = _nowTime.AddDays(1).Date;
            double _duration = _nextDayZero.Subtract(_nowTime).TotalSeconds;
            m_Timer?.Cancel();
            m_Timer = Timer.Register((float)_duration, () =>
            {
                OnBackAwardDataReq(NowBackAwardType);
            }, null, true);
        }
        /// <summary>
        /// 限时找回按钮是否显示
        /// </summary>
        /// <returns></returns>
        public bool CheckLimitedTimeOpen()
        {
            var _param = CSVReturnParam.Instance.GetConfData(10);
            if (int.Parse(_param.str_value)==0)
            {
                return false;
            }
            return true;
        }
        public bool BackAwardGetFunctionOpen()
        {
            return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(219);
        }
        public bool BackAwardRedPoint()
        {
            foreach (var item in BackAwardDictionary)
            {
                if (item.Value.ThisTypeRedPoint())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        public class BackAwardCeil
        {
            public uint TableId { get; private set; }//奖励找回表id
            private uint _freeState;
            private uint _payState;

            public BackAwardCeil(uint tableId)
            {
                TableId = tableId;
            }

            public uint FreeGetState
            {//免费奖励领取状态 1-已领取 0-未领取
                get
                {
                    return _freeState;
                }
                set
                {
                    if (_freeState != value)
                    {
                        _freeState = value;
                    }
                }
            }
            public uint PayGetState
            {//付费奖励领取状态 1-已领取 0-未购买(点击领取按钮后直接购买并领取,无中间状态)
                
                get
                {
                    return _payState;
                }
                set
                {
                    if (_payState != value)
                    {
                        _payState = value;
                    }

                }
            }


        }
        public class BackAward
        {
            public BackAwardType backActivityType
            {
                get;
                private set;
            }

            public uint BackStartTime { get; private set; }
            public uint BackEndTime { get; private set; }
            public uint lastTime { get; private set; } = 0;

            public List<BackAwardCeil> BackAwardList = new List<BackAwardCeil>();
            public BackAward(BackAwardType backActivityType)
            {
                this.backActivityType = backActivityType;
            }
            public void InitBackAwardData()
            {
                BackAwardList.Clear();
                var backData = CSVReturnRecover.Instance.GetAll();
                for (int i = 0; i < backData.Count; i++)
                {
                    var singleData = backData[i];
                    if (singleData.Activity_Group == Instance.BackGroupType && singleData.type == (uint)backActivityType)
                    {
                        BackAwardCeil ceil = new BackAwardCeil(singleData.id);
                        ceil.FreeGetState = 0;
                        ceil.PayGetState = 0;
                        BackAwardList.Add(ceil);
                    }
                }
            }
            public void UpdateThisBackAward(uint _start, uint _end, AwardGetData _data)
            {
                BackStartTime = _start;
                BackEndTime = _end;
                uint _nowTime = Sys_Time.Instance.GetServerTime();
                lastTime = (_nowTime > BackEndTime)?0:(BackEndTime - _nowTime) / 86400+1;
                for (int i = 0; i <BackAwardList.Count; i++)
                {
                    BackAwardList[i].FreeGetState = StateCheck(i, _data.FreeGet);
                    BackAwardList[i].PayGetState = StateCheck(i, _data.PayGet);
                }
            }
            private uint StateCheck(int index, uint binaryNum)
            {//0-可领取 1-已领取
                return (binaryNum >> index) & 1;
            }
            public void UpdateSingleAwardState(uint _type, int _index)
            {
                uint _state = 1;
                switch ((EBackAwardGetCharge)_type)
                {
                    case EBackAwardGetCharge.Free://免费
                        BackAwardList[_index].FreeGetState = _state;
                        break;
                    case EBackAwardGetCharge.Pay://付费
                        BackAwardList[_index].PayGetState = _state;
                        break;
                    default: break;
                }
            }
            public bool ThisTypeRedPoint()
            {
                uint _nowTime = Sys_Time.Instance.GetServerTime();
                if (_nowTime>BackEndTime)
                {
                    return false;
                }
                if (backActivityType==BackAwardType.LimitedTime&&!Instance.CheckLimitedTimeOpen())
                {
                    return false;
                }
                for (int i=0;i< BackAwardList.Count;i++)
                {
                    if (BackAwardList[i].FreeGetState==0)
                    {//有免费可领取时显示红点
                        return true;
                    }
                }
                return false;
            }
        }
        public enum BackAwardType
        {
            None = 0,
            Normal = 1,//每日
            LimitedTime = 2,//限时
            End = 3,
        }
        public enum EBackAwardGetCharge
        {
            None = 0,
            Free = 1,
            Pay = 2,
        }
    }
}
