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
    public class Sys_BackSign : SystemModuleBase<Sys_BackSign>
    {
        public uint TotalLoginDay { get; private set; } = 0; //总登录天数
        public uint BackSignGroup { get; private set; }

        private uint AwardGeted;

        public List<uint> BackSignList = new List<uint>();//签到表 0-可领取 1-已领取 2-不可领取

        public List<uint> BackSignGetList = new List<uint>();//领取表—掉落表id

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        private bool IsSeverNtf=false;

        private Timer m_Timer;
        public enum EEvents
        {
            OnBackSignDataUpdate,
            OnBackSignRes,
        }

        #region 系统函数
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdBackSignIn.GetBackSignInDataReq, (ushort)CmdBackSignIn.GetBackSignInDataRes, OnGetBackSignInDataRes, CmdBackSignInGetBackSignInDataRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdBackSignIn.GetAwardReq, (ushort)CmdBackSignIn.GetAwardRes, OnBackSignSendRes, CmdBackSignInGetAwardRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBackSignIn.RedTipsNtf, OnBackSignRedTipsNtf, CmdBackAwardGetRedTipsNtf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackSignIn.GetBackSignInDataRes, OnGetBackSignInDataRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackSignIn.GetAwardRes, OnBackSignSendRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBackSignIn.RedTipsNtf, OnBackSignRedTipsNtf);
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
            BackSignList.Clear();
            BackSignGetList.Clear();
            m_Timer?.Cancel();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        #endregion
        #region DataInit
        private uint StateCheck(int index)
        {//0-可领取 1-已领取
            return (AwardGeted >> (index - 1)) & 1;
        }
        private void BackSignInit()
        {
            BackSignList.Clear();
            for (int i = 1; i <= BackSignGetList.Count; i++)
            {
                if (i <= TotalLoginDay)
                {
                    BackSignList.Add(StateCheck(i));
                }
                else
                {//登录天数不足
                    BackSignList.Add(2);
                }
            }
        }
        public void BackSignGetInit()
        {
            BackSignGetList.Clear();
            var _data = CSVReturnSign.Instance.GetAll();
            for (int i = 0; i < _data.Count; i++)
            {
                var _singleData = _data[i];
                if (_singleData.Activity_Group == BackSignGroup)
                {
                    BackSignGetList.Add(_singleData.itemId);
                }
            }
        }
        #endregion
        #region net
        /// <summary>
        /// 请求签到数据
        /// </summary>
        public void OnGetBackSignInDataReq()
        {
            CmdBackSignInGetBackSignInDataReq req = new CmdBackSignInGetBackSignInDataReq();
            NetClient.Instance.SendMessage((ushort)CmdBackSignIn.GetBackSignInDataReq, req);
            ReqTickTimer();

        }
        /// <summary>
        /// 签到数据返回
        ///awardGeted: bit位操作:第7天|第6天.....|第2天|第1天. bit位低位
        /// </summary>
        /// <param name="msg"></param>
        private void OnGetBackSignInDataRes(NetMsg msg)
        {
            CmdBackSignInGetBackSignInDataRes res = NetMsgUtil.Deserialize<CmdBackSignInGetBackSignInDataRes>(CmdBackSignInGetBackSignInDataRes.Parser, msg);
            TotalLoginDay = res.TotalLogin;
            BackSignGroup = res.Group;
            AwardGeted = res.AwardGeted;
            Debug.Log("totalday:"+ res.TotalLogin+ "AwardGeted"+ res.AwardGeted);
            BackSignGetInit();
            BackSignInit();
            eventEmitter.Trigger(EEvents.OnBackSignDataUpdate);
            Sys_BackActivity.Instance.eventEmitter.Trigger(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate);
        }
        /// <summary>
        /// 签到领取请求
        /// 第几天 0 1 2 3 .... 6
        /// </summary>
        public void OnBackSignSendReq(uint _index)
        {
            CmdBackSignInGetAwardReq req = new CmdBackSignInGetAwardReq();
            req.DaySeqIndex = _index;
            NetClient.Instance.SendMessage((ushort)CmdBackSignIn.GetAwardReq, req);
        }
        /// <summary>
        /// 签到领取返回
        /// 第几天 0 1 2 3 .... 6
        /// </summary>
        /// <param name="msg"></param>
        private void OnBackSignSendRes(NetMsg msg)
        {
            CmdBackSignInGetAwardRes res = NetMsgUtil.Deserialize<CmdBackSignInGetAwardRes>(CmdBackSignInGetAwardRes.Parser, msg);
            BackSignList[(int)res.DaySeqIndex] = 1;
            eventEmitter.Trigger<uint>(EEvents.OnBackSignRes, res.DaySeqIndex);
        }
        private void OnBackSignRedTipsNtf(NetMsg msg)
        {
            IsSeverNtf = true;
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            ReqTickTimer();
        }
        #endregion
        #region Function
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
            double _duration = _nextDayZero.Subtract(_nowTime).TotalSeconds + 3;
            m_Timer?.Cancel();
            m_Timer = Timer.Register((float)_duration, () =>
            {
                OnGetBackSignInDataReq();
            }, null, true);
        }
        public bool BackSignFunctionOpen()
        {
            return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(217);
        }
        public bool BackSignRedPoint()
        {
            if (IsSeverNtf)
            {
                IsSeverNtf = false;
                return true;
            }
            for (int i = 0; i < BackSignList.Count; i++)
            {
                if (BackSignList[i] == 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
