using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Net;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_Activity_Nien : SystemModuleBase<Sys_Activity_Nien>
    {
       public enum EEvents : int
        {
            OnExchangeNtf,
            OnDailyResetNtf,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public uint NextResetTime;
        public uint LeftExchangeTimes;
        public uint FightStateTimes;
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMonsterNian.DataNty, OnDataNtf, CmdMonsterNianDataNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMonsterNian.ExchangeReq, (ushort)CmdMonsterNian.ExchangeNty, OnExchangeNtf, CmdMonsterNianExchangeNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMonsterNian.DailyResetNty, OnDailyResetNtf, CmdMonsterNianDailyResetNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMonsterNian.TotalDamageNty, OnTotalDamageNtf, CmdMonsterNianTotalDamageNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMonsterNian.DamageRewardReq, (ushort)CmdMonsterNian.DamageRewardNty, OnDamageRewardNtf, CmdMonsterNianDamageRewardNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMonsterNian.FightStageNty, OnFightStageNtf, CmdMonsterNianFightStageNty.Parser);
        }
        public override void OnLogin()
        {

        }
        public override void OnLogout()
        {

        }
        public override void Dispose()
        {

        }

        private void OnDataNtf(NetMsg msg)
        {
            CmdMonsterNianDataNty ntf = NetMsgUtil.Deserialize<CmdMonsterNianDataNty>(CmdMonsterNianDataNty.Parser, msg);
            NextResetTime = ntf.Data.Daily.Nextresettime;
            LeftExchangeTimes = ntf.Data.Daily.Leftexchangetimes;
            FightStateTimes = ntf.Data.Daily.Fightstage;
        }

        public void OnChallengeReq()
        {
            CmdMonsterNianChallengeReq req = new CmdMonsterNianChallengeReq();
            NetClient.Instance.SendMessage((ushort)CmdMonsterNian.ChallengeReq, req);
        }

        /// <summary>
        /// 兑换
        /// </summary>
        public void OnExchangeReq()
        {
            CmdMonsterNianExchangeReq req = new CmdMonsterNianExchangeReq();
            NetClient.Instance.SendMessage((ushort)CmdMonsterNian.ExchangeReq, req);
        }

        private void OnExchangeNtf(NetMsg msg)
        {
            CmdMonsterNianExchangeNty ntf = NetMsgUtil.Deserialize<CmdMonsterNianExchangeNty>(CmdMonsterNianExchangeNty.Parser, msg);
            LeftExchangeTimes = ntf.Leftexchangetimes;
            eventEmitter.Trigger(EEvents.OnExchangeNtf);
        }

        /// <summary>
        /// 通知每日刷新
        /// </summary>
        /// <param name="msg"></param>
        private void OnDailyResetNtf(NetMsg msg)
        {
            CmdMonsterNianDailyResetNty ntf =
                NetMsgUtil.Deserialize<CmdMonsterNianDailyResetNty>(CmdMonsterNianDailyResetNty.Parser, msg);
            
            NextResetTime = ntf.Daily.Nextresettime;
            LeftExchangeTimes = ntf.Daily.Leftexchangetimes;
            FightStateTimes = ntf.Daily.Fightstage;
            
            eventEmitter.Trigger(EEvents.OnDailyResetNtf);
        }

        /// <summary>
        /// 通知累积伤害
        /// </summary>
        /// <param name="msg"></param>
        private void OnTotalDamageNtf(NetMsg msg)
        {
            CmdMonsterNianTotalDamageNty ntf = NetMsgUtil.Deserialize<CmdMonsterNianTotalDamageNty>(CmdMonsterNianTotalDamageNty.Parser, msg);
        }

        /// <summary>
        /// 请求领取累积伤害奖励
        /// </summary>
        public void OnDamageRewardReq()
        {
            CmdMonsterNianDamageRewardReq req = new CmdMonsterNianDamageRewardReq();
            NetClient.Instance.SendMessage((ushort)CmdMonsterNian.DamageRewardReq, req);
        }
        
        /// <summary>
        /// 通知领取累积伤害奖励
        /// </summary>
        /// <param name="msg"></param>
        private void OnDamageRewardNtf(NetMsg msg)
        {
            CmdMonsterNianDamageRewardNty ntf = NetMsgUtil.Deserialize<CmdMonsterNianDamageRewardNty>(CmdMonsterNianDamageRewardNty.Parser, msg);
        }

        /// <summary>
        /// 通知战斗场次
        /// </summary>
        /// <param name="msg"></param>
        private void OnFightStageNtf(NetMsg msg)
        {
            CmdMonsterNianFightStageNty ntf = NetMsgUtil.Deserialize<CmdMonsterNianFightStageNty>(CmdMonsterNianFightStageNty.Parser, msg);
        }

        /// <summary>
        /// 购买爆竹
        /// </summary>
        public void OnPurchaseReq()
        {
            CmdMonsterNianPurchaseReq req = new CmdMonsterNianPurchaseReq();
            NetClient.Instance.SendMessage((ushort)CmdMonsterNian.PurchaseReq, req);
        }
    }
}