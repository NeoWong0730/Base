using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class Sys_pub : SystemModuleBase<Sys_pub>
    {
        public uint CurrentLottoTimes { get; set; }

        public uint CurrentRideLottoTimes { get; set; }
        public uint TotalLottoTimes { get; private set; }

        public uint TotalRideLottoTimes { get; private set; }

        public bool showBuyPromptBox = true;

        public bool showRideBuyPromptBox = true;

        public uint SinglePrice { get; private set; }

        public uint RideSinglePrice { get; private set; }

        public uint CostItemID { get; set; }

        public uint RideCostItemID { get; set; }

        public uint ExtraID { get; private set; }

        public uint RideExtraID { get; private set; }

        public bool isAutoCompletementToggleCheck = true;

        public bool isRideAutoCompletementToggleCheck = true;

        public bool canDoLotto = true;

        public float lastLottoTime = 0;

        public const float lottoInternalTime = 3.0f;

        /// <summary> 是否显示过登录红点 </summary>
        private bool isShowLoginRedPoint = false;
        public enum EEvents
        {
            Result,//抽奖结果
            RideResult //骑宠抽奖结果
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init()
        {

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdDropReward.CmdLuckyDrawDrawRes, Notify_Lotto, CmdLuckyDrawDrawRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdDropReward.CmdLuckyDrawDrawInfoNtf, OnCmdLuckyDrawDrawInfoNtf, CmdLuckyDrawDrawInfoNtf.Parser);
            TotalLottoTimes = uint.Parse(CSVParam.Instance.GetConfData(1274).str_value);
            SinglePrice = uint.Parse(CSVParam.Instance.GetConfData(1268).str_value);
            RideSinglePrice = uint.Parse(CSVParam.Instance.GetConfData(1350).str_value);
            ReadConfig();
        }
        public override void OnLogout()
        {
            isShowLoginRedPoint = false;
        }
        private void ReadConfig()
        {
            var data = CSVParam.Instance.GetConfData(711);

            var rideData = CSVParam.Instance.GetConfData(1333);

            CostItemID = uint.Parse(data.str_value);

            RideCostItemID = uint.Parse(rideData.str_value);

            var extraStr = CSVParam.Instance.GetConfData(1053);

            var rideExtraStr = CSVParam.Instance.GetConfData(1336);

            ExtraID = uint.Parse(extraStr.str_value);

            RideExtraID = uint.Parse(rideExtraStr.str_value);
        }
        /// <summary>
        /// 申请抽奖
        /// </summary>
        /// <param name="mode"> 1 单抽 2 五连抽</param>
        public void Apply_Lotto(int mode, bool useMoney)
        {
            CmdLuckyDrawDrawReq info = new CmdLuckyDrawDrawReq() { Flag = (uint)mode, UseMoney = useMoney ? (uint)1 : (uint)0 };

            NetClient.Instance.SendMessage((ushort)CmdDropReward.CmdLuckyDrawDrawReq, info);

            canDoLotto = false;
            lastLottoTime = UnityEngine.Time.time;
        }

        public void Apply_RideLotto(int mode, bool useMoney)
        {
            CmdLuckyDrawDrawReq info = new CmdLuckyDrawDrawReq() { Flag = (uint)mode, UseMoney = useMoney ? (uint)1 : (uint)0 };

            NetClient.Instance.SendMessage((ushort)CmdDropReward.CmdLuckyDrawDrawReq, info);
        }

        /// <summary>
        /// 获奖通知
        /// </summary>
        private void Notify_Lotto(NetMsg msg)
        {
            CmdLuckyDrawDrawRes info = NetMsgUtil.Deserialize<CmdLuckyDrawDrawRes>(CmdLuckyDrawDrawRes.Parser, msg);
            if (info.Flag < 4)
            {
                eventEmitter.Trigger<CmdLuckyDrawDrawRes>(EEvents.Result, info);
                canDoLotto = true;
            }
            else
                eventEmitter.Trigger<CmdLuckyDrawDrawRes>(EEvents.RideResult, info);
        }

        private void OnCmdLuckyDrawDrawInfoNtf(NetMsg msg)
        {
            CmdLuckyDrawDrawInfoNtf info = NetMsgUtil.Deserialize<CmdLuckyDrawDrawInfoNtf>(CmdLuckyDrawDrawInfoNtf.Parser, msg);
            CurrentLottoTimes = info.Timesrst;
            CurrentRideLottoTimes = info.MountTimesrst;
            UI_Lotto uiLotto = UIManager.GetUI((int)EUIID.UI_Lotto) as UI_Lotto;
            if (uiLotto != null)
            {
                uiLotto.RefreshLottoType();
            }
        }

        public bool isItemEnuge(int needcount)
        {
            return GetItemCount() >= needcount;
        }

        public bool isRideItemEnuge(int needcount)
        {
            return GetRideItemCount() >= needcount;
        }

        public long GetItemCount()
        {
            return Sys_Bag.Instance.GetItemCount(CostItemID);
        }

        public long GetRideItemCount()
        {
            return Sys_Bag.Instance.GetItemCount(RideCostItemID);
        }

        public bool CheckLottoRedPoint()
        {
            return GetItemCount() > 0 && !isShowLoginRedPoint;
        }
        /// <summary>
        /// 设置礼券登录红点状态 (从红点进来的才算)
        /// </summary>
        public void SetLottoLoginRedPointState()
        {
            if(GetItemCount() > 0)
            {
                isShowLoginRedPoint = true;
            }
        }

        //public void SetRideLottoLoginRedPointState()
        //{
        //    if(GetRideItemCount() > 0)
        //    {
                
        //    }
        //}
    }
}
