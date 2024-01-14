using Packet;
using Logic.Core;
using System.Collections.Generic;
using Net;
using Lib.Core;
using Framework;
using Table;

namespace Logic
{
    public partial class Sys_Sign : SystemModuleBase<Sys_Sign>
    {
        public uint buyCount;
        public uint awardCount;
        public uint buyTime;
        public uint signTime;
        public uint refreshTime;
        public bool isInitData;

        private Timer timer;
        public List<uint> awardPoolList = new List<uint>();
        public List<uint> awardTakeList = new List<uint>();
        public List<DailySignRecord> dailySignRecordList = new List<DailySignRecord>();
        public List<DailySignRecord> tempDailySignRecordList = new List<DailySignRecord>();

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            UpdateDailySignAwardCount,   //更新每日签到抽奖次数更新
            UpdateDailySignAwardPool,   //更新每日签到奖池更新
            UpdateTakeDailySignAward,   //更新每日签到奖品获取
            UpdateDailySignRecord,   //更新每日签到广播
            CompletePlayingCircle,     //完成播放转圈
            UpdateDailySignNotice,     //更新每日签到跑马灯
        }
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSign.DailySignDataNtf, OnDailySignDataNtf, CmdSignDailySignDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSign.DailySignRecordNtf, OnDailySignRecordNtf, CmdSignDailySignRecordNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSign.DailySignInfoReq, (ushort)CmdSign.DailySignInfoRes, OnDailySignInfoRes, CmdSignDailySignInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSign.DailySignInReq, (ushort)CmdSign.DailySignInRes, OnDailySignInRes, CmdSignDailySignInRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSign.DailySignDrawReq, (ushort)CmdSign.DailySignDrawRes, OnDailySignDrawRes, CmdSignDailySignDrawRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSign.DailySignBuyReq, (ushort)CmdSign.DailySignBuyRes, OnDailySignBuyRes, CmdSignDailySignBuyRes.Parser);
        }

        private void OnDailySignDataNtf(NetMsg msg)
        {
            CmdSignDailySignDataNtf ntf = NetMsgUtil.Deserialize<CmdSignDailySignDataNtf>(CmdSignDailySignDataNtf.Parser, msg);
            awardCount = ntf.AwardCoun;
            signTime = ntf.SignTime;
        }

        private void OnDailySignRecordNtf(NetMsg msg)
        {
            CmdSignDailySignRecordNtf ntf = NetMsgUtil.Deserialize<CmdSignDailySignRecordNtf>(CmdSignDailySignRecordNtf.Parser, msg);
            if (CSVSign.Instance.GetConfData(ntf.Record.InfoId).Log)
            {
                tempDailySignRecordList.Clear();
                for (int i = 0; i < dailySignRecordList.Count; i++)
                {
                    tempDailySignRecordList.Add(dailySignRecordList[i]);
                }
                dailySignRecordList.Clear();
                dailySignRecordList.Add(ntf.Record);
                dailySignRecordList.AddRange(tempDailySignRecordList);
                eventEmitter.Trigger(EEvents.UpdateDailySignRecord);
            }
            if (CSVSign.Instance.GetConfData(ntf.Record.InfoId).Notice)
            {
                uint rewardItemId = CSVSign.Instance.GetConfData(ntf.Record.InfoId).Reward;
                uint count = CSVSign.Instance.GetConfData(ntf.Record.InfoId).Count;
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(rewardItemId);
                string itemName = LanguageHelper.GetLanguageColorWordsFormat(CSVLanguage.Instance.GetConfData(itemData.name_id).words, TextHelper.GetQuailtyLangId(itemData.quality));
                string content = LanguageHelper.GetErrorCodeContent(2021101, ntf.Record.RoleName, itemName, count.ToString());
                ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(2021101).pos, content, content, 2021101,null,true);
            }
        }

        public void SignDailySignInfoReq()
        {
            if (!isInitData)
            {
                CmdSignDailySignInfoReq req = new CmdSignDailySignInfoReq();
                NetClient.Instance.SendMessage((ushort)CmdSign.DailySignInfoReq, req);
            }
        }

        private void OnDailySignInfoRes(NetMsg msg)
        {
            CmdSignDailySignInfoRes res = NetMsgUtil.Deserialize<CmdSignDailySignInfoRes>(CmdSignDailySignInfoRes.Parser, msg);
            buyCount = res.BuyCount;
            buyTime = res.BuyCountTime;
            awardCount = res.AwardCount;
            refreshTime = res.RefreshTime;
            CheckSignRefresh();

            awardPoolList.Clear();
            dailySignRecordList.Clear();
            awardTakeList.Clear();
            uint bigAwardId = 0;
            int bigAwardIdIndex = 0;
            for (int i = 0; i < res.AwardPool.Count; ++i)
            {
                if (CSVSign.Instance.TryGetValue(res.AwardPool[i], out CSVSign.Data data) && data.Pond==1)
                {
                    bigAwardId = res.AwardPool[i];
                    bigAwardIdIndex = i;
                }
                awardPoolList.Add(res.AwardPool[i]);
            }
            uint tempId = awardPoolList[11];
            awardPoolList[11] = bigAwardId;
            awardPoolList[bigAwardIdIndex] = tempId;
            for (int i = 0; i < res.AwardTake.Count; ++i)
            {
                awardTakeList.Add(res.AwardTake[i]);
            }
            for (int i = res.Record.Count - 1; i >= 0; i--)
            {
                if (CSVSign.Instance.TryGetValue(res.Record[i].InfoId,out CSVSign.Data data)&& data.Log)
                {
                    dailySignRecordList.Add(res.Record[i]);
                }     
            }
            eventEmitter.Trigger(EEvents.UpdateDailySignAwardPool);
            eventEmitter.Trigger(EEvents.UpdateDailySignAwardCount);
            isInitData = true;
        }

        public void SignDailySignInReq()
        {
            CmdSignDailySignInReq req = new CmdSignDailySignInReq();
            NetClient.Instance.SendMessage((ushort)CmdSign.DailySignInReq, req);
        }

        private void OnDailySignInRes(NetMsg msg)
        {
            CmdSignDailySignInRes res = NetMsgUtil.Deserialize<CmdSignDailySignInRes>(CmdSignDailySignInRes.Parser, msg);
            awardCount = res.AwardCount;
            eventEmitter.Trigger(EEvents.UpdateDailySignAwardCount);
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5226));   
        }

        public void SignDailySignBuyReq(uint count)
        {
            CmdSignDailySignBuyReq req = new CmdSignDailySignBuyReq();
            req.Num = count;
            NetClient.Instance.SendMessage((ushort)CmdSign.DailySignBuyReq, req);
        }

        private void OnDailySignBuyRes(NetMsg msg)
        {
            CmdSignDailySignBuyRes res = NetMsgUtil.Deserialize<CmdSignDailySignBuyRes>(CmdSignDailySignBuyRes.Parser, msg);
            awardCount = res.AwardCount;
            buyCount = res.BuyCount;
            buyTime = res.BuyCountTime;
            eventEmitter.Trigger(EEvents.UpdateDailySignAwardCount);
        }

        public void SignDailySignDrawReq()
        {
            CmdSignDailySignDrawReq req = new CmdSignDailySignDrawReq();
            NetClient.Instance.SendMessage((ushort)CmdSign.DailySignDrawReq, req);
        }

        private void OnDailySignDrawRes(NetMsg msg)
        {
            CmdSignDailySignDrawRes res = NetMsgUtil.Deserialize<CmdSignDailySignDrawRes>(CmdSignDailySignDrawRes.Parser, msg);
            awardCount = res.AwardCount;
            awardTakeList.Add(res.InfoId);
            eventEmitter.Trigger<uint>(EEvents.UpdateTakeDailySignAward, res.InfoId);
            eventEmitter.Trigger(EEvents.UpdateDailySignAwardCount);
        }

        public override void OnLogout()
        {
            buyCount = 0;
            awardCount = 0;
            buyTime = 0;
            refreshTime = 0;
            signTime = 0;
            isInitData = false;
            timer?.Cancel();
            awardPoolList.Clear();
            awardTakeList.Clear();
        }

        public bool CheckDailySignRedPoint()
        {
            return CheckDailySignIsOpen() && awardCount > 0;
        }

        private void CheckSignRefresh()
        {
            if (Sys_Sign.Instance.refreshTime > Sys_Time.Instance.GetServerTime())
            {
                float time = Sys_Sign.Instance.refreshTime - Sys_Time.Instance.GetServerTime();
                timer?.Cancel();
                timer = Timer.Register(time+2, OnComplete, null,false,true);
            }
            else
            {
                Sys_Sign.Instance.isInitData = false;
                Sys_Sign.Instance.SignDailySignInfoReq();
            }
        }

        private void OnComplete()
        {
            if (UIManager.IsOpen(EUIID.UI_OperationalActivity))
            {
                Sys_Sign.Instance.isInitData = false;
                Sys_Sign.Instance.SignDailySignInfoReq();
            }
        }

        public void CheckUpdateOnOpenUI()
        {
            if (Sys_Sign.Instance.refreshTime <= Sys_Time.Instance.GetServerTime() || signTime==0)
            {
                Sys_Sign.Instance.isInitData = false;
                Sys_Sign.Instance.SignDailySignInfoReq();
            }
        }

        public bool CheckDailySignIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50908) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(105);
        }
    }
}
