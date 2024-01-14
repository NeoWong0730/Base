using Framework;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;


namespace Logic
{
    /// <summary> 运营活动系统 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    { 
        #region 数据定义
        /// <summary> 事件枚举 </summary>
        public enum EEvents
        {
            UpdateOperatinalActivityShowOrHide,   //刷新活动显示隐藏

            UpdateSignSevenDayData, //更新七日签到数据
            ReceiveSignReward,      //领取签到奖励
            UpdateLevelGiftData,    //更新等级礼包数据
            UpdateGrowthFundData,   //更新成长基金数据
            UpdateTotalChargeData,  //更新累计充值数据
            UpdateSpecialCardData,  //更新特权卡(月卡)数据
            UpdateFirstChargeGiftData,  //更新首充礼包数据
            UpdateLotteryActivityData,  //更新大地鼠彩票页签活动数据
            UpdateSevenDaysTargetData,  //更新七日目标数据
            UpdateDailyGiftData,        //更新每日礼包数据
            UpdatePhoneBindStatus,
            UpdateRankActivityData,  //更新排行榜页签活动数据
            UpdateTimelimitGiftData,    //更新限时礼包活动数据
            UpdateExpRetrieveData,    //更新经验找回活动数据
            UpdateFightTreasureData,//更新夺宝活动数据
            UpdateRebateData,     //更新充值返利数据
            UpdateActivityRewardData,   //更新运营活动奖励数据
            UpdateChargeABData,   //更新充值选礼数据
            OnChargeABActivityEnd,   //充值选礼活动结束
            UpdateActivityTotalCharge, //更新累计充值活动
            UpdateActivityTotalConsume, //更新累计消费活动
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        /// <summary> 活动开关字典</summary>
        /// 
        private Dictionary<uint, Packet.ActivityInfo> dictActivitySwitch = new Dictionary<uint, Packet.ActivityInfo>();

        #endregion
        #region 系统函数
        public override void Init()
        {

            SetSignSevenDayData();
            LevelGiftInit();
            GrowthFundInit();
            TotalChargeInit();
            SpecialCardInit();
            FirstChargeGiftInit();
            SevenDaysTargetInit();
            //InitTestGiftOpen();
            ProcessEvents(true);
        }
        public override void Dispose()
        {
            ProcessEvents(false);
            ChargeABOnDispose();
        }
        public override void OnLogin()
        {
            SevenDaysTargetOnLogin();
            RebateOnLogin();
            ChargeABOnLogin();
            ActivityTotalChargeOnLogin();
            ActivityTotalConsumeOnLogin();
        }
        public override void OnLogout()
        {
            DestroyGrowthFundData();
            DestroyTotlaChargeGiftData();
            DestroySpecialCardData();
            DestroyFirstChargeGiftData();
            SevenDaysTargetOnLogout();
            DestoryExpRetrieveData();
            dictActivitySwitch.Clear();
            DestoryRebateData();
            DestoryFightTreasureDate();
            DestoryActivityRewardData();
            ChargeABOnLogout();
            ActivityTotalChargeOnLogout();
            ActivityTotalConsumeOnLogout();
        }
        public override void OnSyncFinished()
        {

        }
        #endregion
        #region 初始化
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void ProcessEvents(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnOnUpdateLevelNtf, toRegister);
            Sys_Charge.Instance.eventEmitter.Handle<uint>(Sys_Charge.EEvents.OnChargedNotify, OnChargedNotify, toRegister);
            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, OnUpdateActivityOperationRuler, toRegister);
            if (toRegister)
            {

                /// <summary> 七日登录 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSign.SevenDayDataNtf, OnSignSevenDayDataNtf, CmdSignSevenDayDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdSign.SevenDaySignReq, (ushort)CmdSign.SevenDaySignRes, OnSignSevenDaySignRes, CmdSignSevenDaySignRes.Parser);
                /// <summary> 等级礼包 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdLevelGift.DataNtf, OnLevelGiftUnit, CmdLevelGiftDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdLevelGift.GetGiftReq, (ushort)CmdLevelGift.GetGiftRes, OnLevelGiftGetGiftRes, CmdLevelGiftGetGiftRes.Parser);
                /// <summary> 成长基金 </summary>
                EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.GrowthGetReq, (ushort)CmdCharge.GrowthGetRes, OnChargeGrowthGetRes, CmdChargeGrowthGetRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.GrowthNtf, OnChargeGrowthNtf, CmdChargeGrowthNtf.Parser);
                /// <summary> 生涯累充 </summary>
                EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.CumulateGetReq, (ushort)CmdCharge.CumulateGetRes, OnChargeCumulateGetRes, CmdChargeCumulateGetRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.CumulateNtf, OnChargeCumulateNtf, CmdChargeCumulateNtf.Parser);
                /// <summary> 特权卡(月卡) </summary>
                EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.CardRewardGetReq, (ushort)CmdCharge.CardRewardGetRes, OnChargeCardRewardGetRes, CmdChargeCardRewardGetRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.CardDataNtf, OnChargeCardDataNtf, CmdChargeCardDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.GiftCardNtf, OnChargeGiftCardNtf, CmdChargeGiftCardNtf.Parser);
                /// <summary> 首充礼包 </summary>
                EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.FirstGiftGetReq, (ushort)CmdCharge.FirstGiftGetRes, OnChargeFirstGiftGetRes, CmdChargeFirstGiftGetRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.FirstGiftNtf, OnChargeFirstGiftNtf, CmdChargeFirstGiftNtf.Parser);
                /// <summary> 七日目标 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityTarget.TwoWeekData, OnSevenDaysTargetNtf, CmdActivityTargetTwoWeekData.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityTarget.TwoWeekDataUpdate, OnSevenDaysTargetDataUpdateNtf, CmdActivityTargetTwoWeekDataUpdate.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityTarget.TwoWeekTaskRewardReq, (ushort)CmdActivityTarget.TwoWeekTaskRewardRes, OnGetSevenDaysTargetTaskRewardRes, CmdActivityTargetTwoWeekTaskRewardRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityTarget.TwoWeekScoreRewardReq, (ushort)CmdActivityTarget.TwoWeekScoreRewardRes, OnGetSevenDaysTargetScoreRewardRes, CmdActivityTargetTwoWeekScoreRewardRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityTarget.ActivityStartReq, (ushort)CmdActivityTarget.ActivityStartRes, OnSevenDaysTargetStartRes, CmdActivityTargetActivityStartRes.Parser);
                //累充经验
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.ExpNtf, OnExpNtf, CmdChargeExpNtf.Parser);
                /// <summary> 每日礼包 </summary>
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdDailyGift.DataNtf, OnDailyGiftDataNtf, CmdDailyGiftDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdDailyGift.OperatorReq, (ushort)CmdDailyGift.OperatorAck, OnDailyGiftOperatorAck, CmdDailyGiftOperatorAck.Parser);

                EventDispatcher.Instance.AddEventListener((ushort)CmdRole.BindPhoneTakeAwardReq, (ushort)CmdRole.BindPhoneTakeAwardRes, this.OnBindPhoneTakeWardRes, CmdRoleBindPhoneTakeAwardRes.Parser);
                //限时礼包                
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityTarget.LimitGiftDataNtf, OnLimitGiftDataNtf, CmdActivityTargetLimitGiftDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityTarget.LimitGiftDataUpdate, OnLimitGiftDataUpdate, CmdActivityTargetLimitGiftDataUpdate.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityTarget.LimitGiftBuyReq, (ushort)CmdActivityTarget.LimitGiftBuyRes, OnLimitGiftBuyRes, CmdActivityTargetLimitGiftBuyRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityTarget.LimitGiftBuyNtf, OnLimitGiftBuyNtf, CmdActivityTargetLimitGiftBuyNtf.Parser);
                //经验找回
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.CompensationExpNty, OnCompensationExpNty, CmdAttrCompensationExpNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAttr.RemoveCompensationNty, OnRemoveCompensationNty, CmdAttrRemoveCompensationNty.Parser);
                //
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.FreeChatUpdateNtf, OnFreeChatUpdateNtf, CmdChargeFreeChatUpdateNtf.Parser);
                //活动开关
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivitySwitch.InfoListNtf, OnActivitySwitchInfoListNtf, CmdActivitySwitchInfoListNtf.Parser);

                //充值返利
                EventDispatcher.Instance.AddEventListener((ushort)CmdRole.ChargeRebateGetReq, (ushort)CmdRole.ChargeRebateNtf, OnChargeRebateNtf, CmdRoleChargeRebateNtf.Parser);

                //夺宝活动
                EventDispatcher.Instance.AddEventListener((ushort)CmdFightTreasure.DataReq, (ushort)CmdFightTreasure.DataRes, OnFightTreasureDataRes, CmdFightTreasureDataRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdFightTreasure.ApplyActivityReq, (ushort)CmdFightTreasure.ApplyActivityRes, OnFightTreasureApplyActivityRes, CmdFightTreasureApplyActivityRes.Parser);
                //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFightTreasure.ApplyTotalNumNtf, OnFightTreasureApplyTotalNumNtf, CmdFightTreasureApplyTotalNumNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFightTreasure.DataNtf, OnFightTreasureDataNtf, CmdFightTreasureDataNtf.Parser);

                //运营活动奖励
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.RewardNtf, OnActivityRewardNtf, CmdRoleRewardNtf.Parser);
                //累计充值消费
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityCumulateDataReq, (ushort)CmdActivityRuler.CmdActivityCumulateDataNtf, OnCmdActivityCumulateDataNtf, CmdActivityCumulateDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityRuler.CmdActivityCumulateRewardReq, (ushort)CmdActivityRuler.CmdActivityCumulateRewardRes, OnCmdActivityCumulateRewardRes, CmdActivityCumulateRewardRes.Parser);
            }
            else
            {
                /// <summary> 七日登录 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdSign.SevenDayDataNtf, OnSignSevenDayDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdSign.SevenDaySignRes, OnSignSevenDaySignRes);
                /// <summary> 等级礼包 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdLevelGift.DataNtf, OnLevelGiftUnit);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdLevelGift.GetGiftRes, OnLevelGiftGetGiftRes);
                /// <summary> 成长基金 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.GrowthGetRes, OnChargeGrowthGetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.GrowthNtf, OnChargeGrowthNtf);
                /// <summary> 生涯累充 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.CumulateGetRes, OnChargeCumulateGetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.CumulateNtf, OnChargeCumulateNtf);
                /// <summary> 特权卡(月卡) </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.CardRewardGetRes, OnChargeCardRewardGetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.CardDataNtf, OnChargeCardDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.GiftCardNtf, OnChargeGiftCardNtf);
                /// <summary> 首充礼包 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.FirstGiftGetRes, OnChargeFirstGiftGetRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdCharge.FirstGiftNtf, OnChargeFirstGiftNtf);
                /// <summary> 七日目标 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.TwoWeekData, OnSevenDaysTargetNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.TwoWeekDataUpdate, OnSevenDaysTargetDataUpdateNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.TwoWeekTaskRewardRes, OnGetSevenDaysTargetTaskRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.TwoWeekScoreRewardRes, OnGetSevenDaysTargetScoreRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.ActivityStartRes, OnSevenDaysTargetStartRes);
                /// <summary> 每日礼包 </summary>
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyGift.DataNtf, OnDailyGiftDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdDailyGift.OperatorAck, OnDailyGiftOperatorAck);

                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRole.BindPhoneTakeAwardRes, this.OnBindPhoneTakeWardRes);
                //限时礼包                
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.LimitGiftDataNtf, OnLimitGiftDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.LimitGiftDataUpdate, OnLimitGiftDataUpdate);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.LimitGiftBuyRes, OnLimitGiftBuyRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityTarget.LimitGiftBuyNtf, OnLimitGiftBuyNtf);

                //经验找回
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAttr.CompensationExpNty, OnCompensationExpNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdAttr.RemoveCompensationNty, OnRemoveCompensationNty);
                //活动开关
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivitySwitch.InfoListNtf, OnActivitySwitchInfoListNtf);
                //充值返利
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRole.ChargeRebateNtf, OnChargeRebateNtf);
                //运营活动奖励
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRole.RewardNtf, OnActivityRewardNtf);
                //累充累消
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityCumulateDataNtf, OnCmdActivityCumulateDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityRuler.CmdActivityCumulateRewardRes, OnCmdActivityCumulateRewardRes);
            }
            SDKManager.eventEmitter.Handle<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, OnSDKBindIphoneStatus, toRegister);
            RegisterChargeABEvents(toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
        }
        #endregion
        #region 服务器消息
        /// <summary>
        /// 活动开关列表数据广播
        /// </summary>
        private void OnActivitySwitchInfoListNtf(NetMsg msg)
        {
            CmdActivitySwitchInfoListNtf res = NetMsgUtil.Deserialize<CmdActivitySwitchInfoListNtf>(CmdActivitySwitchInfoListNtf.Parser, msg);
            for (int i = 0; i < res.InfoList.Count; i++)
            {
                var data = res.InfoList[i];
                dictActivitySwitch[data.Id] = data;
                DebugUtil.Log(ELogType.eActivitySwitch, "同步服务器开关 " + data.Id + " | " + data.Open);
            }
            OnFightTreasureActivitySwitch();
            eventEmitter.Trigger(EEvents.UpdateOperatinalActivityShowOrHide);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 福利按钮红点检测
        /// </summary>
        /// <returns></returns>
        public bool CheckOperationalActivityRedPoint()
        {
            return CheckSevenDaysSignShowRedPoint()
                || CheckLevelGiftShowRedPoint()
                || CheckGrowthFundShowRedPoint()
                || CheckTotlaChargeGiftRedPoint()
                || CheckSpecialCardRedPoint()
                || Sys_Sign.Instance.CheckDailySignRedPoint()
                || CheckLotteryActivityRedPoint()
                || CheckRankActivityRedPoint()
                || CheckDailyGiftRedPoint()
                || CheckBindPhoneRedPoint()
                || CheckExpRetrieveRedPoint()
                || (Sys_Qa.Instance.HasQa() && SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.QuestionSurvey.ToString(), out string paramsValue) && !Sys_Qa.Instance.hasShowRedPoint)
                || CheckRebateRedPoint()
                || CheckOneDollarRedPoint()
                || CheckHundredDollarRedPoint()
                || CheckActivityRewardRedPoint()
                || Sys_PedigreedDraw.Instance.HasRewardUnGet()
                || CheckChargeABRedPoint()
                || Sys_ActivitySubscribe.Instance.redPoint
                || Sys_SinglePay.Instance.CheckSinglePlayRedPoint()
                || Sys_SinglePay.Instance.CheckSinglePayHeFuRedPoint()
                || Sys_BackAssist.Instance.ActivityReturnLovePointRedPoint()
                || CheckActivityTotalChargeRedPoint()
                || CheckActivityTotalConsumeRedPoint();
        }
        /// <summary>
        /// 是否有需要显示的活动
        /// </summary>
        /// <returns></returns>
        public bool IsShowActivity()
        {
            var values = System.Enum.GetValues(typeof(EOperationalActivity));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EOperationalActivity type = (EOperationalActivity)values.GetValue(i);
                switch (type)
                {
                    case EOperationalActivity.SevenDaysSign:
                        {
                            if (Sys_FunctionOpen.Instance.IsOpen(50903) && IsShowSignSevenDay())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.LevelGift:
                        {
                            if (CheckLevelGiftIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.GrowthFund:
                        {
                            if (CheckGrowthFundIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.TotalCharge:
                        {
                            if (CheckTotalChargeIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.SpecialCard:
                        {
                            if (CheckSpecialCardIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.DailySign:
                        {
                            if (Sys_Sign.Instance.CheckDailySignIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.LotteryActivity:
                        {
                            if (CheckLotteryActivityIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.DailyGift:
                        {
                            if (CheckDailyGiftIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.PhoneBindGift:
                        {
                            if (BindPhoneIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.AddQQGroup:
                        {
                            if (IsShowQQGroup())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.ExpRetrieve:
                        {
                            if (IsShowExpRetrieve())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.Rebate:
                        {
                            if (CheckRebateisOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.Alipay:
                        {
                            if (CheckAlipayActivityIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.OneDollar:
                        {
                            if (CheckOneDollarIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.HundredDollar:
                        {
                            if (CheckHundredDollarIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.ChargeAB:
                        {
                            if (CheckChargeABIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.KingPet:
                        {
                            if (CheckKingPetActivityIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.SinglePay:
                        {
                            if (Sys_SinglePay.Instance.CheckSinglePayIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.BackAssist:
                        {
                            if (Sys_BackAssist.Instance.CheckActivityReturnLovePointOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.ActivityTotalCharge:
                        {
                            if (CheckActivityTotalChargeIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.ActivityTotalConsume:
                        {
                            if (CheckActivityTotalConsumeIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    case EOperationalActivity.SinglePayHeFu:
                        {
                            if (Sys_SinglePay.Instance.CheckSinglePayHefuIsOpen())
                            {
                                return true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return false;
        }
        /// <summary> 解析充值数据 (运营活动)</summary>
        public void ParseChargeDataNtf(CmdChargeDataNtf ntf)
        {
            RefreshTotalChargeGiftData(ntf.Cumulate);
            RefreshGrowthFund(ntf);
            RefreshAllSpecialCardData(ntf);
            RefreshFirstChargeGiftAllData(ntf);
            if (ntf != null)
                NewChargeActExp = ntf.ChargeActExp;
        }

        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            RefreshSevenDaysTargetOpenState();
            //关闭倒计时，在同步时间的时候需要刷新
            StartSevenDaysEndTimer();
            InitChargeABActivityData();
        }
        private void OnOnUpdateLevelNtf()
        {
            DailygiftBagTypeInit();
            eventEmitter.Trigger(EEvents.UpdateDailyGiftData);
        }
        private void OnChargedNotify(uint chargeId)
        {
            OnSpecialCardPresentSucceed(chargeId);
            eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
            eventEmitter.Trigger(EEvents.UpdateDailyGiftData);
        }
        private void OnUpdateActivityOperationRuler()
        {
            InitFightActivityDictionary();//夺宝活动
            InitTotalCharge();
            InitTotalConsume();
        }

        public bool BindPhoneTakeWard = false;
        //public bool BindPhoneSwitch = false;
        public bool IsClickBindPhone = false;

        private int bindIphoneState; //1 未绑定, 2已绑定
        private void OnSDKBindIphoneStatus(int code)
        {
            DebugUtil.LogError("BindPhoneStatus =" + code);
            bindIphoneState = code;
            eventEmitter.Trigger(EEvents.UpdatePhoneBindStatus);
        }

        public void OnBindPhoneTakeWardReq()
        {
            CmdRoleBindPhoneTakeAwardReq req = new CmdRoleBindPhoneTakeAwardReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.BindPhoneTakeAwardReq, req);
        }

        private void OnBindPhoneTakeWardRes(NetMsg msg)
        {
            CmdRoleBindPhoneTakeAwardRes res = NetMsgUtil.Deserialize<CmdRoleBindPhoneTakeAwardRes>(CmdRoleBindPhoneTakeAwardRes.Parser, msg);
            BindPhoneTakeWard = true;
            eventEmitter.Trigger(EEvents.UpdatePhoneBindStatus);
        }

        public bool BindPhoneIsOpen()
        {
            return SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.BindIphone.ToString(), out string paramsValue) && !BindPhoneTakeWard;
        }

        public bool BindPhoneBindingState()
        {
            return bindIphoneState == 2; //2 已绑定
        }

        public bool CheckBindPhoneRedPoint()
        {
            if (SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.BindIphone.ToString(), out string paramsValue) && !BindPhoneTakeWard)
            {
                if (BindPhoneBindingState())
                {
                    return true;
                }
                else
                {
                    return !IsClickBindPhone;
                }
            }
            return false;
        }

        public bool IsShowQQGroup()
        {
            return SDKManager.AddQQGroupIsOpen();
        }

        public bool CheckQQGroupRedPoint()
        {
            return false;
        }
        /// <summary>
        /// 检测活动开关是否开启 id为活动紧急开关配置表对应ID
        /// </summary>
        public bool CheckActivitySwitchIsOpen(uint id)
        {
            if (dictActivitySwitch.TryGetValue(id, out Packet.ActivityInfo switchData))
            {
                return switchData.Open;
            }
            //else
            //{
            //    DebugUtil.Log(ELogType.eNone, "未检测到id为 " + id + "的活动开关");
            //}
            //未找到开关时，功能默认开启
            return true;
        }
        /// <summary>
        /// 检测金宠抽奖福利菜单内界面是否开启
        /// </summary>
        public bool CheckKingPetActivityIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50917);
        }
        #endregion
    }
    /// <summary> 运营活动系统-七日签到 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        /// <summary>
        /// 七日签到状态
        /// </summary>
        public struct SevenDaySignState
        {
            /// <summary>
            /// 签到状态|-1表示未开启|0表示已结束|1表示正在进行中
            /// </summary>
            public int state;
            /// <summary>
            /// 下一次签到时间|-1表示无法获取下一次时间|0表示当前可签到|>0表示正常一下可签到剩余时间
            /// </summary>
            public int nextTime;
        }
        /// <summary> 七日登录数据通知 </summary>
        public CmdSignSevenDayDataNtf cmdSignSevenDayDataNtf { get; set; } = new CmdSignSevenDayDataNtf();
        /// <summary> 持续时间(秒) </summary>
        public uint DurationTime_SevenDaysSign = 0;
        /// <summary> 签到状态 </summary>
        private SevenDaySignState sevenDaySignState = new SevenDaySignState();
        /// <summary> 关闭签到时间 PS:服务器认为这个字段必要记录,本地临时记录 </summary>
        private uint closeSignTime = 0;
        #endregion
        #region 数据处理
        /// <summary>
        /// 设置持续时间
        /// </summary>
        public void SetSignSevenDayData()
        {
            uint.TryParse(CSVParam.Instance.GetConfData(903).str_value, out DurationTime_SevenDaysSign);
            DurationTime_SevenDaysSign = DurationTime_SevenDaysSign * 86400; //持续时间
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 请求七日登录签到
        /// </summary>
        /// <param name="awardId"></param>
        public void SendSignSevenDaySignReq(uint awardId)
        {
            CmdSignSevenDaySignReq req = new CmdSignSevenDaySignReq();
            req.AwardId = awardId;
            NetClient.Instance.SendMessage((ushort)CmdSign.SevenDaySignReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 七日登录数据通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnSignSevenDayDataNtf(NetMsg msg)
        {
            CmdSignSevenDayDataNtf ntf = NetMsgUtil.Deserialize<CmdSignSevenDayDataNtf>(CmdSignSevenDayDataNtf.Parser, msg);
            cmdSignSevenDayDataNtf = ntf;
            Sys_OperationalActivity.Instance.eventEmitter.Trigger(EEvents.UpdateSignSevenDayData);
        }
        /// <summary>
        /// 请求七日登录签到返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSignSevenDaySignRes(NetMsg msg)
        {
            CmdSignSevenDaySignRes res = NetMsgUtil.Deserialize<CmdSignSevenDaySignRes>(CmdSignSevenDaySignRes.Parser, msg);
            int awardTake = cmdSignSevenDayDataNtf.AwardTake;
            int awardId = (int)res.AwardId;
            int newAwardTake = awardTake + (1 << awardId);
            cmdSignSevenDayDataNtf.AwardTake = newAwardTake;
            if (IsCompleteSign())
            {
                uint nowTime = Sys_Time.Instance.GetServerTime();
                uint today = nowTime / 86400;
                closeSignTime = (today + 1) * 86400;
            }
            Sys_OperationalActivity.Instance.eventEmitter.Trigger<uint>(EEvents.ReceiveSignReward, res.AwardId);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 七日签到是否开启
        /// </summary>
        /// <returns></returns>
        public bool IsOpenSignSevenDay()
        {
            return CheckActivitySwitchIsOpen(100) && cmdSignSevenDayDataNtf.EndTime != 0;
        }
        /// <summary>
        /// 是否完成签到
        /// </summary>
        /// <returns></returns>
        public bool IsCompleteSign()
        {
            return cmdSignSevenDayDataNtf.AwardTake == Convert.ToInt32("11111110", 2);
        }
        /// <summary>
        /// 某天是否已签到
        /// </summary>
        /// <param name="awardId"></param>
        /// <returns></returns>
        public bool IsSign(int signID)
        {
            int awardTake = cmdSignSevenDayDataNtf.AwardTake;
            int mark = awardTake & (1 << signID);
            return Convert.ToBoolean(mark);
        }
        /// <summary>
        /// 得到下一个未签到编号
        /// </summary>
        /// <returns></returns>
        public uint GetNextSignID()
        {
            for (uint i = 1; i < 8; i++)
            {
                if (!IsSign((int)i))
                {
                    return i;
                }
            }
            return 0;
        }
        /// <summary>
        /// 得到七日签到状态
        /// </summary>
        /// <returns></returns>
        public SevenDaySignState GetSevenDaySignData()
        {
            if (!IsOpenSignSevenDay() ||//签到未开启
                 IsCompleteSign())      //签到已超次
            {
                sevenDaySignState.state = -1;
                sevenDaySignState.nextTime = -1;
                return sevenDaySignState;
            }

            uint nowTime = Sys_Time.Instance.GetServerTime();      //现在时间
            uint endTime = cmdSignSevenDayDataNtf.EndTime;         //结束时间
            uint startTime = endTime - DurationTime_SevenDaysSign; //开始时间

            if (nowTime < startTime) //未开始
            {
                sevenDaySignState.state = -1;
                sevenDaySignState.nextTime = -1;
            }
            else if (nowTime > endTime) //已结束
            {
                sevenDaySignState.state = 0;
                sevenDaySignState.nextTime = -1;
            }
            else //进行中
            {
                int awardTake = (int)cmdSignSevenDayDataNtf.AwardTake;
                uint count = cmdSignSevenDayDataNtf.LoginCount;

                for (int i = 1; i < count + 1; i++)
                {
                    if (!IsSign(i))
                    {
                        sevenDaySignState.state = 1;
                        sevenDaySignState.nextTime = 0;
                        return sevenDaySignState;
                    }
                }

                uint today = nowTime / 86400;
                sevenDaySignState.state = 1;
                sevenDaySignState.nextTime = (int)((today + 1) * 86400 - nowTime);
            }
            return sevenDaySignState;
        }
        /// <summary>
        /// 是否显示七日签到
        /// </summary>
        /// <returns></returns>
        public bool IsShowSignSevenDay()
        {
            if (!IsOpenSignSevenDay())
            {
                return false;
            }
            uint nowTime = Sys_Time.Instance.GetServerTime();      //现在时间
            uint endTime = cmdSignSevenDayDataNtf.EndTime;         //结束时间
            uint startTime = endTime - DurationTime_SevenDaysSign; //开始时间
            return startTime <= nowTime && nowTime <= endTime && !(IsCompleteSign() && nowTime > closeSignTime);
        }

        /// <summary>
        /// 检测七日登入红点
        /// </summary>
        /// <returns></returns>
        public bool CheckSevenDaysSignShowRedPoint()
        {
            bool isShow = false;
            if (IsShowSignSevenDay())
            {
                var signinRewardDatas = CSVSigninReward.Instance.GetAll();
                for (int i = 0, len = signinRewardDatas.Count; i < len; ++i)
                {
                    if (CanGetSevenDaySignReward(signinRewardDatas[i].id))
                    {
                        isShow = true;
                        break;
                    }
                }
                //Sys_OperationalActivity.SevenDaySignState sevenDaySignState = Sys_OperationalActivity.Instance.GetSevenDaySignData();
                //return sevenDaySignState.nextTime == 0;
            }
            return isShow;
        }

        private bool CanGetSevenDaySignReward(uint Id)
        {
            bool isUnSign = Id <= cmdSignSevenDayDataNtf.LoginCount;
            return isUnSign && !IsSign((int)Id);
        }
        #endregion
    }
    /// <summary> 运营活动系统-等级礼包 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        public List<uint> LstLevelGiftIds { get; private set; } = new List<uint>();
        public Dictionary<uint, LevelGiftUnit> DictGifts { get; private set; } = new Dictionary<uint, LevelGiftUnit>();
        #endregion
        #region 数据处理
        private void LevelGiftInit()
        {
            LstLevelGiftIds.AddRange(CSVLevelGift.Instance.GetKeys());
        }
        #endregion
        #region 服务器消息
        public void OnLevelGiftUnit(NetMsg msg)
        {
            CmdLevelGiftDataNtf ntf = NetMsgUtil.Deserialize<CmdLevelGiftDataNtf>(CmdLevelGiftDataNtf.Parser, msg);
            for (int i = 0; i < ntf.Gifts.Count; i++)
            {
                LevelGiftUnit data = ntf.Gifts[i];
                if (DictGifts.ContainsKey(data.GiftId))
                {
                    DictGifts[data.GiftId] = data;
                }
                else
                {
                    DictGifts.Add(data.GiftId, data);
                }
            }

            eventEmitter.Trigger(EEvents.UpdateLevelGiftData);
        }

        public void LevelGiftGetGiftReq(uint giftId)
        {
            CmdLevelGiftGetGiftReq req = new CmdLevelGiftGetGiftReq();
            req.GiftId = giftId;
            NetClient.Instance.SendMessage((ushort)CmdLevelGift.GetGiftReq, req);
        }
        public void OnLevelGiftGetGiftRes(NetMsg msg)
        {
            CmdLevelGiftGetGiftRes res = NetMsgUtil.Deserialize<CmdLevelGiftGetGiftRes>(CmdLevelGiftGetGiftRes.Parser, msg);
            LevelGiftUnit data = res.Gift;
            if (DictGifts.ContainsKey(data.GiftId))
            {
                DictGifts[data.GiftId] = data;
            }
            else
            {
                DictGifts.Add(data.GiftId, data);
            }
            eventEmitter.Trigger(EEvents.UpdateLevelGiftData);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 
        /// </summary>
        public bool CheckLevelGiftIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50904) && CheckActivitySwitchIsOpen(101);
        }
        public List<ItemData> GetLevelGiftItems(uint rewardId)
        {
            List<ItemData> items = new List<ItemData>();
            List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(rewardId);
            if (dropItems != null)
            {
                int len = dropItems.Count;
                for (int j = 0; j < len; j++)
                {
                    ItemData item = new ItemData(0, 0, dropItems[j].id, (uint)dropItems[j].count, 0, false, false, null, null, 0);
                    items.Add(item);
                }
            }
            return items;
        }
        public List<uint> GetLevelGiftIds()
        {
            List<uint> lstUnGet = new List<uint>();
            List<uint> lstIsGet = new List<uint>();
            for (int i = 0; i < LstLevelGiftIds.Count; i++)
            {
                uint giftId = LstLevelGiftIds[i];
                if (DictGifts.TryGetValue(giftId, out LevelGiftUnit data))
                {
                    if (data.IsGet)
                    {
                        lstIsGet.Add(giftId);
                    }
                    else
                    {
                        lstUnGet.Add(giftId);
                    }
                }
            }
            lstUnGet.AddRange(lstIsGet);
            return lstUnGet;
        }
        /// <summary>
        /// 检测等级礼包红点
        /// </summary>
        /// <returns></returns>
        public bool CheckLevelGiftShowRedPoint()
        {
            if (CheckLevelGiftIsOpen() && Sys_Role.Instance.Role.Career != (uint)ECareerType.None)
            {
                for (int i = 0; i < LstLevelGiftIds.Count; i++)
                {
                    uint giftId = LstLevelGiftIds[i];
                    CSVLevelGift.Data giftData = CSVLevelGift.Instance.GetConfData(giftId);
                    bool canGet = Sys_Role.Instance.Role.Level >= giftData.Level;
                    if (DictGifts.TryGetValue(giftId, out LevelGiftUnit giftState) && canGet && !giftState.IsGet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
    /// <summary> 运营活动系统-成长基金 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        private uint GrowthFundDataRefreshTimes = 0;
        public List<uint> GrowthFundIds { get; private set; } = new List<uint>();
        public Dictionary<uint, List<uint>> DictGrowthFund { get; private set; } = new Dictionary<uint, List<uint>>();

        #endregion
        #region 数据处理
        private void GrowthFundInit()
        {
            GrowthFundIds.AddRange(CSVGrowthFund.Instance.GetKeys());
        }
        #endregion
        #region 服务器消息
        /// <summary> 领取基金奖励 </summary>
        public void GetGrowFundReward(uint actId, uint index)
        {
            CmdChargeGrowthGetReq req = new CmdChargeGrowthGetReq();
            req.ActId = actId;
            req.RewardIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdCharge.GrowthGetReq, req);
        }
        public void OnChargeGrowthGetRes(NetMsg msg)
        {
            CmdChargeGrowthGetRes res = NetMsgUtil.Deserialize<CmdChargeGrowthGetRes>(CmdChargeGrowthGetRes.Parser, msg);
            if (DictGrowthFund.ContainsKey(res.ActId))
            {
                DictGrowthFund[res.ActId][(int)res.RewardIndex] = (uint)ChargeRewardStatus.Receivid;
            }
            eventEmitter.Trigger(EEvents.UpdateGrowthFundData);
        }
        public void OnChargeGrowthNtf(NetMsg msg)
        {
            CmdChargeGrowthNtf ntf = NetMsgUtil.Deserialize<CmdChargeGrowthNtf>(CmdChargeGrowthNtf.Parser, msg);
            RefreshGrowthFundSingelData(ntf);
            eventEmitter.Trigger(EEvents.UpdateGrowthFundData);
        }
        /// <summary> 成长基金-数据刷新请求 </summary>
        public void ReqGrowthRefreshData()
        {
            CmdChargeGrowthRefreshReq req = new CmdChargeGrowthRefreshReq();
            NetClient.Instance.SendMessage((ushort)CmdCharge.GrowthRefreshReq, req);
        }
        #endregion
        #region 提供功能

        /// <summary>
        /// 检测成长基金功能是否开启
        /// </summary>
        public bool CheckGrowthFundIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50905) && CheckActivitySwitchIsOpen(102);
        }
        /// <summary>检测成长基金红点</summary>
        public bool CheckGrowthFundShowRedPoint()
        {
            if (CheckGrowthFundIsOpen())
            {
                for (int i = 0; i < GrowthFundIds.Count; i++)
                {
                    if (CheckGrowthFundShowRedPointByActId(GrowthFundIds[i]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>检测单种成长基金红点</summary>
        public bool CheckGrowthFundShowRedPointByActId(uint ActId)
        {
            if (CheckGrowthFundFirstRedPoint(ActId))
            {
                return true;
            }
            CSVGrowthFund.Data fundData = CSVGrowthFund.Instance.GetConfData(ActId);
            if (fundData != null && DictGrowthFund.TryGetValue(ActId, out List<uint> indexList))
            {
                var lvList = fundData.level;
                for (int i = 0; i < lvList.Count; i++)
                {
                    if (lvList[i] <= Sys_Role.Instance.Role.Level && indexList[i] != (uint)ChargeRewardStatus.Receivid)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检测第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckGrowthFundFirstRedPoint(uint ActId)
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "GrowthFundFirstRedPoint" + ActId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetGrowthFundFirstRedPoint(uint ActId)
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "GrowthFundFirstRedPoint" + ActId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateGrowthFundData);
            }
        }

        /// <summary> 同步成长基金状态数据 </summary>
        public void RefreshGrowthFund(CmdChargeDataNtf ntf)
        {
            if (ntf.Growth != null)
            {
                for (int i = 0; i < ntf.Growth.Count; i++)
                {
                    var fundInfo = ntf.Growth[i];
                    RefreshGrowthFundSingelData(fundInfo);
                }
                eventEmitter.Trigger(EEvents.UpdateGrowthFundData);
            }
        }
        private void RefreshGrowthFundSingelData(CmdChargeGrowthNtf ntf)
        {
            if (!DictGrowthFund.ContainsKey(ntf.ActId))
            {
                DictGrowthFund[ntf.ActId] = new List<uint>();
            }
            DictGrowthFund[ntf.ActId].Clear();
            DictGrowthFund[ntf.ActId].AddRange(ntf.Status);
            CSVGrowthFund.Data fundData = CSVGrowthFund.Instance.GetConfData(ntf.ActId);
            if (DictGrowthFund[ntf.ActId].Count != fundData.reward_Id.Count)
            {
                if (GrowthFundDataRefreshTimes < 5)
                {
                    ReqGrowthRefreshData();
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, "CmdChargeGrowthRefreshReq 成长基金礼包同步，重复请求已达" + GrowthFundDataRefreshTimes + "次");
                }
                GrowthFundDataRefreshTimes++;
            }
            else
            {
                GrowthFundDataRefreshTimes = 0;
            }
            //string str = "";
            //for (int i = 0; i < ntf.Status.Count; i++)
            //{
            //    str += ntf.Status[i] + " | ";
            //}
            //Debug.Log("fund 同步 Id:" + ntf.ActId + " | " + str);
        }
        /// <summary> 检测单条成长基金是否已经领取（默认返回true） </summary>
        public bool CheckGrowthFundIsGet(uint actId, int index)
        {
            if (DictGrowthFund.TryGetValue(actId, out List<uint> indexList))
            {
                return indexList[index] == (uint)ChargeRewardStatus.Receivid;
            }
            return false;
        }
        /// <summary> 检测是否购买成长基金(没礼包状态数据就是没购买) </summary>
        public bool CheckGrowFundIsBuy(uint actId)
        {
            if (DictGrowthFund.ContainsKey(actId))
            {
                return true;
            }
            return false;
        }
        /// <summary> 清除成长基金数据 </summary>
        public void DestroyGrowthFundData()
        {
            GrowthFundDataRefreshTimes = 0;
            DictGrowthFund.Clear();
        }
        /// <summary>
        /// 获取排序后的成长基金下标(对应单个基金等级数组下标 以及 基金状态列表下标)
        /// </summary>
        /// <param name="fundId"></param>
        /// <returns></returns>
        public List<int> GetGrowthDataSortIndex(uint fundId)
        {
            List<int> unLock = new List<int>();
            List<int> unGet = new List<int>();
            List<int> isGet = new List<int>();
            CSVGrowthFund.Data fundData = CSVGrowthFund.Instance.GetConfData(fundId);
            var levels = fundData.level;
            if (DictGrowthFund.TryGetValue(fundId, out List<uint> indexList))
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    if (Sys_Role.Instance.Role.Level >= levels[i])
                    {
                        if (indexList[i] == (uint)ChargeRewardStatus.Receivid)
                        {
                            isGet.Add(i);
                        }
                        else
                        {
                            unGet.Add(i);
                        }
                    }
                    else
                    {
                        unLock.Add(i);
                    }
                }
                unGet.AddRange(unLock);
                unGet.AddRange(isGet);
            }
            else
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    unGet.Add(i);
                }
            }
            return unGet;
        }

        /// <summary>
        /// 成长基金埋点
        /// </summary>
        public void ReportGrowthFundClickEventHitPoint(string strValue)
        {
            //Debug.Log("GrowthFund" + strValue);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "GrowthFund_" + strValue);
        }
        #endregion
    }
    /// <summary> 运营活动系统-生涯累充 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        private uint TotleChargeDataRefreshTimes = 0;//刷新请求次数，避免数据错误造成死循环
        //public List<uint> LstTotalChargeIds { get; private set; } = new List<uint>();
        /// <summary> 累充额度 </summary>
        public uint TotalChargeValue { get; private set; }
        
        //新的累充额度，目前只用在家族红包
        public uint NewChargeActExp;
        /// <summary> 累充礼包状态 </summary>
        public List<uint> LstTotalGiftStatus { get; private set; } = new List<uint>();

        #endregion
        #region 数据处理
        private void TotalChargeInit()
        {
            //LstTotalChargeIds.AddRange(CSVCareerChange.Instance.GetDictData().Keys);
        }
        #endregion
        #region 服务器消息
        public void GetTotalChargeGiftReq(uint rewardIndex)
        {
            CmdChargeCumulateGetReq req = new CmdChargeCumulateGetReq();
            req.RewardIndex = rewardIndex;
            NetClient.Instance.SendMessage((ushort)CmdCharge.CumulateGetReq, req);
        }

        public void OnChargeCumulateGetRes(NetMsg msg)
        {
            CmdChargeCumulateGetRes res = NetMsgUtil.Deserialize<CmdChargeCumulateGetRes>(CmdChargeCumulateGetRes.Parser, msg);
            if (LstTotalGiftStatus.Count > res.RewardIndex)
            {
                LstTotalGiftStatus[(int)res.RewardIndex] = (uint)ChargeRewardStatus.Receivid;
            }
            eventEmitter.Trigger(EEvents.UpdateTotalChargeData);
        }
        public void OnChargeCumulateNtf(NetMsg msg)
        {
            CmdChargeCumulateNtf ntf = NetMsgUtil.Deserialize<CmdChargeCumulateNtf>(CmdChargeCumulateNtf.Parser, msg);
            RefreshTotalChargeGiftData(ntf);
        }
        /// <summary> 生涯累充-数据刷新请求 </summary>
        public void ReqCumulateRefreshData()
        {
            CmdChargeCumulateRefreshReq req = new CmdChargeCumulateRefreshReq();
            NetClient.Instance.SendMessage((ushort)CmdCharge.CumulateRefreshReq, req);
        }
        private void OnExpNtf(NetMsg msg)
        {
            CmdChargeExpNtf ntf = NetMsgUtil.Deserialize<CmdChargeExpNtf>(CmdChargeExpNtf.Parser, msg);
            NewChargeActExp = ntf.Exp;
        }
        #endregion
        #region 提供功能
        public bool CheckTotalChargeIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50906) && CheckActivitySwitchIsOpen(103);
        }

        /// <summary>获取排序后的基金id列表下标</summary>
        public List<int> GetTotalChargeIdIndex()
        {
            List<int> unLock = new List<int>();
            List<int> unGet = new List<int>();
            List<int> isGet = new List<int>();

            var LstTotalChargeDatas = CSVCareerChange.Instance.GetAll();
            for (int i = 0, len = LstTotalChargeDatas.Count; i < len; i++)
            {
                //CSVCareerChange.Data TotalChargeData = CSVCareerChange.Instance.GetConfData(LstTotalChargeIds[i]);
                CSVCareerChange.Data TotalChargeData = LstTotalChargeDatas[i];
                if (TotalChargeValue >= TotalChargeData.money_see)
                {
                    if (TotalChargeData.money <= TotalChargeValue)
                    {
                        if (LstTotalGiftStatus.Count > i && LstTotalGiftStatus[i] != (uint)ChargeRewardStatus.Receivid)
                        {
                            unGet.Add(i);
                        }
                        else
                        {
                            isGet.Add(i);
                        }
                    }
                    else
                    {
                        unLock.Add(i);
                    }
                }
            }
            unGet.AddRange(unLock);
            unGet.AddRange(isGet);
            return unGet;
        }
        /// <summary>刷新同步累充礼包数据</summary>
        public void RefreshTotalChargeGiftData(CmdChargeCumulateNtf ntf)
        {
            if (ntf != null)
            {
                TotalChargeValue = ntf.CumulateValue;
                LstTotalGiftStatus.Clear();
                LstTotalGiftStatus.AddRange(ntf.Status);
                if (LstTotalGiftStatus.Count != CSVCareerChange.Instance.Count)
                {
                    if (TotleChargeDataRefreshTimes < 5)
                    {
                        ReqCumulateRefreshData();
                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, "CmdChargeCumulateRefreshReq 累充礼包同步，重复请求已达" + TotleChargeDataRefreshTimes + "次");
                    }
                    TotleChargeDataRefreshTimes++;
                }
                else
                {
                    TotleChargeDataRefreshTimes = 0;
                }
            }
            eventEmitter.Trigger(EEvents.UpdateTotalChargeData);
        }
        public bool CheckTotalChargeGiftIsGet(uint fundId)
        {
            int index = (int)fundId - 1;
            if (LstTotalGiftStatus.Count > index)
            {
                return LstTotalGiftStatus[index] == (uint)ChargeRewardStatus.Receivid;
            }
            return false;
        }
        public bool CheckTotlaChargeGiftRedPoint()
        {
            if (CheckTotalChargeIsOpen())
            {
                var LstTotalChargeDatas = CSVCareerChange.Instance.GetAll();
                for (int i = 0, len = LstTotalChargeDatas.Count; i < len; i++)
                {
                    if (LstTotalGiftStatus.Count > i)
                    {
                        //var TotalChargeData = CSVCareerChange.Instance.GetConfData(LstTotalChargeIds[i]);
                        var TotalChargeData = LstTotalChargeDatas[i];
                        if (TotalChargeData.money <= TotalChargeValue && LstTotalGiftStatus[i] != (uint)ChargeRewardStatus.Receivid)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary> 清除生涯累充数据 </summary>
        public void DestroyTotlaChargeGiftData()
        {
            TotalChargeValue = 0;
            TotleChargeDataRefreshTimes = 0;
            LstTotalGiftStatus.Clear();
        }
        /// <summary>
        /// 生涯累充埋点
        /// </summary>
        public void ReportTotlaChargeClickEventHitPoint(string strValue)
        {
            //Debug.Log("TotlaCharge_" + strValue);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "TotlaCharge_" + strValue);
        }
        #endregion
    }
    /// <summary> 运营活动系统-特权卡(月卡) </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        public List<uint> ListSpecialCardIds { get; private set; } = new List<uint>();
        public Dictionary<uint, CmdChargeCardDataNtf> DictSpecialCards = new Dictionary<uint, CmdChargeCardDataNtf>();
        /// <summary> 上次赠送对象的名字 </summary>
        public string lastSpecialCardPresentTargetName = "";
        #endregion
        #region 数据处理
        private void SpecialCardInit()
        {
            //ListSpecialCardIds.AddRange(CSVMonthCard.Instance.GetDictData().Keys);
            ListSpecialCardIds.AddRange(CSVMonthCard.Instance.GetKeys());
        }
        #endregion
        #region 服务器消息
        public void GetSpecialCardReward(uint cardId)
        {
            CmdChargeCardRewardGetReq req = new CmdChargeCardRewardGetReq();
            req.CardId = cardId;
            NetClient.Instance.SendMessage((ushort)CmdCharge.CardRewardGetReq, req);
        }
        public void OnChargeCardRewardGetRes(NetMsg msg)
        {
            CmdChargeCardRewardGetRes res = NetMsgUtil.Deserialize<CmdChargeCardRewardGetRes>(CmdChargeCardRewardGetRes.Parser, msg);
            if (DictSpecialCards.TryGetValue(res.CardId, out CmdChargeCardDataNtf ntf))
            {
                ntf.IsGet = true;
            }
            eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
        }
        public void OnChargeCardDataNtf(NetMsg msg)
        {
            CmdChargeCardDataNtf ntf = NetMsgUtil.Deserialize<CmdChargeCardDataNtf>(CmdChargeCardDataNtf.Parser, msg);
            RefreshSpecialCardData(ntf);
            eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
        }
        /// <summary> 月卡数据同步请求 </summary>
        public void ReqRefreshSpecialCardInfo(uint cardId)
        {
            CmdChargeCardRefreshReq req = new CmdChargeCardRefreshReq();
            req.CardId = cardId;
            NetClient.Instance.SendMessage((ushort)CmdCharge.CardRefreshReq, req);
        }
        /// <summary> 同步特权卡数据 </summary>
        public void RefreshAllSpecialCardData(CmdChargeDataNtf ChargeNtf)
        {
            if (ChargeNtf.CardData != null)
            {
                for (int i = 0; i < ChargeNtf.CardData.Count; i++)
                {
                    var ntf = ChargeNtf.CardData[i];
                    RefreshSpecialCardData(ntf);
                }
                eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
            }
        }
        private void RefreshSpecialCardData(CmdChargeCardDataNtf ntf)
        {
            DebugUtil.Log(ELogType.eNone, "月卡数据同步 id:" + ntf.CardId + "| 领取状态:" + ntf.IsGet);
            if (DictSpecialCards.ContainsKey(ntf.CardId))
            {
                DictSpecialCards[ntf.CardId] = ntf;
            }
            else
            {
                DictSpecialCards.Add(ntf.CardId, ntf);
            }
        }
        private void OnChargeGiftCardNtf(NetMsg msg)
        {
            CmdChargeGiftCardNtf ntf = NetMsgUtil.Deserialize<CmdChargeGiftCardNtf>(CmdChargeGiftCardNtf.Parser, msg);
            DebugUtil.Log(ELogType.eNone, "赠送月卡同步" + ntf.CardId);
            if (DictSpecialCards.TryGetValue(2, out CmdChargeCardDataNtf cardInfo))
            {
                var cardId = ntf.CardId;
                if (cardId == 1 && cardInfo.GiftCard.Count >= 1)
                {
                    var param = cardInfo.GiftCard[0];
                    cardInfo.GiftCard[0] = param >= 1 ? param - 1 : 0;
                }
                else if (cardId == 2 && cardInfo.GiftCard.Count >= 2)
                {
                    var param = cardInfo.GiftCard[1];
                    cardInfo.GiftCard[1] = param >= 1 ? param - 1 : 0;
                }
            }
            eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
        }
        #endregion
        #region 提供功能
        public bool CheckSpecialCardIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50907) && CheckActivitySwitchIsOpen(104);
        }

        public bool CheckLotteryActivityIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50909) && CheckActivitySwitchIsOpen(106);
        }

        public bool CheckRideLotteryActivityIsOpen()
        {
            return CheckActivitySwitchIsOpen(207);
        }

        /// <summary>检测月卡红点</summary>
        public bool CheckSpecialCardRedPoint()
        {
            if (CheckSpecialCardIsOpen())
            {
                for (int i = 0; i < ListSpecialCardIds.Count; i++)
                {
                    var cardId = ListSpecialCardIds[i];
                    if (CheckSpepcialSingleRedPoint(cardId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary> 检测单种月卡红点 </summary>
        public bool CheckSpepcialSingleRedPoint(uint cardId)
        {
            if (CheckSpecialCardFirstRedPoint(cardId))
            {
                return true;
            }
            if (DictSpecialCards.TryGetValue(cardId, out CmdChargeCardDataNtf ntf))
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                if (ntf.Times > nowTime && !ntf.IsGet)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckSpecialCardFirstRedPoint(uint cardId)
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SpecialCardFirstRedPoint" + cardId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetSpecialCardFirstRedPoint(uint cardId)
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SpecialCardFirstRedPoint" + cardId.ToString();
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateSpecialCardData);
            }
        }
        /// <summary>检测月卡是否激活 id对应月卡表id</summary>
        public bool CheckSpecialCardIsActive(uint id)
        {
            if (DictSpecialCards.TryGetValue(id, out CmdChargeCardDataNtf ntf))
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                if (ntf.Times > nowTime)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 获取月卡状态数据 </summary>
        public CmdChargeCardDataNtf GetSpecialCardInfo(uint cardId)
        {
            if (DictSpecialCards.TryGetValue(cardId, out CmdChargeCardDataNtf ntf))
            {
                return ntf;
            }
            return null;
        }
        /// <summary> 检测月卡奖励是否可领取 </summary>
        public bool CheckSpepcialGiftCanGet(uint cardId)
        {

            if (DictSpecialCards.TryGetValue(cardId, out CmdChargeCardDataNtf ntf))
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                //再加一个月卡是否过期检测
                if (!ntf.IsGet && ntf.Times > nowTime)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获得特权卡，添加交易格子
        /// </summary>
        /// <returns></returns>
        public uint GetCardTradeGridCount()
        {
            uint count = 0;
            foreach (var data in DictSpecialCards)
            {
                if (data.Value.Times > Sys_Time.Instance.GetServerTime())
                {
                    CSVMonthCard.Data csv = CSVMonthCard.Instance.GetConfData(data.Value.CardId);
                    if (csv != null)
                        count += csv.Extra_Deal;
                }
            }

            return count;
        }

        /// <summary> 清除月卡数据 </summary>
        public void DestroySpecialCardData()
        {
            DictSpecialCards.Clear();
        }

        /// <summary> 获取月卡特权封印加成概率（万分比） </summary>
        public uint GetSpecialCardCaptureProbability()
        {
            uint pro = 0;
            for (int i = 0; i < ListSpecialCardIds.Count; i++)
            {
                uint cardId = ListSpecialCardIds[i];
                if (CheckSpecialCardIsActive(cardId))
                {
                    CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData.Pet_Extra_Seal_successrate > 0)
                    {
                        pro += cardData.Pet_Extra_Seal_successrate;
                    }
                }
            }
            return pro;
        }

        /// <summary> 检测月卡里，随身银行特权是否开启 </summary>
        public bool CheckSpecialCardWithBankIsUnlock()
        {
            for (int i = 0; i < ListSpecialCardIds.Count; i++)
            {
                uint cardId = ListSpecialCardIds[i];
                if (CheckSpecialCardIsActive(cardId))
                {
                    CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData.With_bank)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary> 检测月卡里，随身厨房特权是否开启 </summary>
        public bool CheckSpecialCardPocketKitchenIsUnlock(bool showMsg = false)
        {
            for (int i = 0; i < ListSpecialCardIds.Count; i++)
            {
                uint cardId = ListSpecialCardIds[i];
                if (CheckSpecialCardIsActive(cardId))
                {
                    CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData.pocket_kitchen)
                    {
                        return true;
                    }
                }
            }
            if (showMsg)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5930));
            }
            return false;
        }
        /// <summary>
        /// 月卡埋点
        /// </summary>
        public void ReportSpecialCardClickEventHitPoint(string strValue)
        {
            //Debug.Log("SpecialCard_" + strValue);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "SpecialCard_" + strValue);
        }
        /// <summary>
        /// 检测月卡是否显示首次购买
        /// </summary>
        public bool CheckSpecialCardFirstChargeShow(uint cardId)
        {
            var cardData = CSVMonthCard.Instance.GetConfData(cardId);
            if (cardData != null)
            {
                //当前卡的常规充值和首充充值都未购买，就判断为首充
                return !Sys_Charge.Instance.IsCharged(cardData.Change_Id) && cardData.First_Change_Id > 0 && !Sys_Charge.Instance.IsCharged(cardData.First_Change_Id);
            }
            return false;
        }
        /// <summary>
        /// 获取月卡送礼的好友列表
        /// </summary>
        public List<Sys_Society.RoleInfo> GetSpecialCardPresentFriendList(string keyWord)
        {
            List<Sys_Society.RoleInfo> friendList = new List<Sys_Society.RoleInfo>();
            foreach (var friend in Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().Values)
            {
                if (friend.isOnLine)
                {
                    if (!string.IsNullOrEmpty(keyWord) && keyWord.Length > 0)
                    {
                        //筛选name
                        if (friend.roleName.Contains(keyWord))
                        {
                            friendList.Add(friend);
                        }
                    }
                    else
                    {
                        friendList.Add(friend);
                    }
                }
            }
            return friendList;
        }
        /// <summary>
        /// 检测月卡赠送功能是否开启
        /// </summary>
        public bool CheckSpecialCardPresentIsOpen(bool showMsg = false)
        {
            for (int i = 0; i < ListSpecialCardIds.Count; i++)
            {
                uint cardId = ListSpecialCardIds[i];
                if (CheckSpecialCardIsActive(cardId))
                {
                    CSVMonthCard.Data cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData.give_Pirviege)
                    {
                        return true;
                    }
                }
            }
            if (showMsg)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12266));
            }
            return false;
        }
        /// <summary>
        /// 获取月卡剩余赠送次数
        /// </summary>
        public uint GetSpecialCardPresentNum(uint cardId)
        {
            if (CheckSpecialCardPresentIsOpen())
            {
                var cardInfo = Sys_OperationalActivity.Instance.GetSpecialCardInfo(2);
                if (cardId == 1 && cardInfo.GiftCard.Count >= 1)
                {
                    return cardInfo.GiftCard[0];
                }
                else if (cardId == 2 && cardInfo.GiftCard.Count >= 2)
                {
                    return cardInfo.GiftCard[1];
                }
            }
            return 0;
        }
        /// <summary>
        /// 月卡赠送成功逻辑
        /// </summary>
        private void OnSpecialCardPresentSucceed(uint chargeId)
        {
            if (chargeId == 33 || chargeId == 34)
            {
                string userName = lastSpecialCardPresentTargetName;
                string cardName = string.Empty;
                for (int i = 0; i < ListSpecialCardIds.Count; i++)
                {
                    var cardId = ListSpecialCardIds[i];
                    var cardData = CSVMonthCard.Instance.GetConfData(cardId);
                    if (cardData.Present_Change_Id == chargeId)
                    {
                        cardName = LanguageHelper.GetTextContent(cardData.Pirviege_Title);
                    }
                }
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12257, cardName, userName));
            }
        }
        #endregion
    }

    /// <summary> 运营活动系统-首充礼包 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        /// <summary>是否已经首充</summary>
        public bool IsFirstCharge { get; private set; }
        /// <summary> 职业jobID对应首充礼包表 </summary>
        public Dictionary<uint, CSVFirstCharge.Data> DictFirstChargeGift { get; private set; } = new Dictionary<uint, CSVFirstCharge.Data>();
        /// <summary> 首充礼包领取状态 </summary>
        public List<uint> ListFirstChargeGiftState { get; private set; } = new List<uint>();

        #endregion
        #region 数据处理
        private void FirstChargeGiftInit()
        {
            DictFirstChargeGift.Clear();
            //var keys = new List<uint>();
            //keys.AddRange(CSVFirstCharge.Instance.GetDictData().Keys);

            var datas = CSVFirstCharge.Instance.GetAll();
            for (int i = 0; i < datas.Count; i++)
            {
                //var key = keys[i];
                //CSVFirstCharge.Data data = CSVFirstCharge.Instance.GetConfData(key);

                CSVFirstCharge.Data data = datas[i];
                DictFirstChargeGift.Add(data.Job_Id, data);
            }
        }
        #endregion
        #region 服务器消息
        public void GetFirstChargeGiftReq(uint rewardIndex)
        {
            CmdChargeFirstGiftGetReq req = new CmdChargeFirstGiftGetReq();
            req.RewardIndex = rewardIndex;
            NetClient.Instance.SendMessage((ushort)CmdCharge.FirstGiftGetReq, req);
        }
        public void OnChargeFirstGiftGetRes(NetMsg msg)
        {
            CmdChargeFirstGiftGetRes res = NetMsgUtil.Deserialize<CmdChargeFirstGiftGetRes>(CmdChargeFirstGiftGetRes.Parser, msg);
            if (ListFirstChargeGiftState.Count > res.RewardIndex)
            {
                ListFirstChargeGiftState[(int)res.RewardIndex] = (uint)ChargeRewardStatus.Receivid;
            }
            var firstChargeData = GetFirstChargeGiftData();
            List<List<int>> rewards = new List<List<int>>();
            switch (res.RewardIndex)
            {
                case 0:
                    rewards = firstChargeData.Reward_Items_d1;
                    break;
                case 1:
                    rewards = firstChargeData.Reward_Items_d2;
                    break;
                case 2:
                    rewards = firstChargeData.Reward_Items_d3;
                    break;
            }
            UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
            for (int i = 0; i < rewards.Count; i++)
            {
                itemRewardParms.itemIds.Add((uint)rewards[i][0]);
                itemRewardParms.itemCounts.Add((uint)rewards[i][1]);
            }
            UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
            eventEmitter.Trigger(EEvents.UpdateFirstChargeGiftData);
        }
        public void OnChargeFirstGiftNtf(NetMsg msg)
        {
            CmdChargeFirstGiftNtf ntf = NetMsgUtil.Deserialize<CmdChargeFirstGiftNtf>(CmdChargeFirstGiftNtf.Parser, msg);
            RefreshFirstChargeGiftData(ntf);
            eventEmitter.Trigger(EEvents.UpdateFirstChargeGiftData);
        }
        #endregion
        #region 提供功能
        /// <summary>检测首充礼包按钮是否显示</summary>
        public bool CheckFirstChargeIsShow()
        {
            if (!(Sys_FunctionOpen.Instance.IsOpen(51301) && CheckActivitySwitchIsOpen(200)))
            {
                return false;
            }
            if (ListFirstChargeGiftState.Count <= 0)
            {
                return true;
            }
            for (int i = 0; i < ListFirstChargeGiftState.Count; i++)
            {
                if (ListFirstChargeGiftState[i] != (uint)ChargeRewardStatus.Receivid)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>检测首充礼包红点</summary>
        public bool CheckFirstChargeRedPoint()
        {
            if (CheckFirstChargeFirstRedPoint())
            {
                return true;
            }
            for (int i = 0; i < ListFirstChargeGiftState.Count; i++)
            {
                if (ListFirstChargeGiftState[i] == (uint)ChargeRewardStatus.CanReceive)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckFirstChargeFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "FirstChargeFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetFirstChargeFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "FirstChargeFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateFirstChargeGiftData);
            }
        }
        /// <summary> 返回自己职业id对应的首充表数据 </summary>
        public CSVFirstCharge.Data GetFirstChargeGiftData()
        {
            if (DictFirstChargeGift.TryGetValue(Sys_Role.Instance.Role.Career, out CSVFirstCharge.Data data))
            {
                return data;
            }
            return null;
        }
        /// <summary> 获取当前应该显示第几天 </summary>
        public int GetFirstChargeShowDay()
        {
            if (IsFirstCharge)
            {
                for (int i = 0; i < ListFirstChargeGiftState.Count; i++)
                {
                    if (ListFirstChargeGiftState[i] == (uint)ChargeRewardStatus.CanReceive)
                    {
                        return i + 1;
                    }
                    else if (ListFirstChargeGiftState[i] == (uint)ChargeRewardStatus.None)
                    {
                        if (i == 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
            }
            return 1;
        }
        public void RefreshFirstChargeGiftAllData(CmdChargeDataNtf ntf)
        {
            if (ntf.FirstGift != null)
            {
                RefreshFirstChargeGiftData(ntf.FirstGift);
            }
        }
        private void RefreshFirstChargeGiftData(CmdChargeFirstGiftNtf ntf)
        {
            IsFirstCharge = true;
            ListFirstChargeGiftState.Clear();
            ListFirstChargeGiftState.AddRange(ntf.Status);
        }
        /// <summary> 清除首充礼包数据 </summary>
        public void DestroyFirstChargeGiftData()
        {
            IsFirstCharge = false;
            ListFirstChargeGiftState.Clear();
        }
        /// <summary> 首充礼包-检测当前道具是否需要特殊弹窗 </summary>
        public bool CheckFirstChargeNeedSpecialPop(uint itemId)
        {
            var partnerParams = CSVParam.Instance.GetConfData(1029).str_value.Split('|');
            if (itemId == uint.Parse(partnerParams[1]))
            {
                return true;
            }
            var petParams = CSVParam.Instance.GetConfData(1030).str_value.Split('|');
            if (itemId == uint.Parse(petParams[1]))
            {
                return true;
            }
            return false;
        }
        /// <summary> 首充礼包-获取特殊弹窗对应的全局表参数 </summary>
        public CSVParam.Data GetFirstChargeSpecialPopParam(uint itemId)
        {
            CSVParam.Data partnerParam = CSVParam.Instance.GetConfData(1029);
            var partnerParams = partnerParam.str_value.Split('|');
            if (uint.Parse(partnerParams[1]) == itemId)
            {
                return partnerParam;
            }
            CSVParam.Data petParam = CSVParam.Instance.GetConfData(1030);
            var petParams = petParam.str_value.Split('|');
            if (uint.Parse(petParams[1]) == itemId)
            {
                return petParam;
            }
            return null;
        }
        /// <summary>
        /// 首充埋点
        /// </summary>
        public void ReportFirstChargeClickEventHitPoint(string strValue)
        {
            //Debug.Log("FirstCharge_" + strValue);
            UIManager.HitButton(EUIID.UI_OperationalActivity, "FirstCharge_" + strValue);
        }

        /// <summary>
        /// 开启首充(封装区分处理IOS安卓首充)
        /// </summary>
        public void OpenFirstCharge()
        {
            if (CheckAlipayActivityIsOpen())
            {
                //IOS
                UIManager.OpenUI(EUIID.UI_FirstCharge_AlyIOS);
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_FirstCharge);
            }
        }
        #endregion
    }

    /// <summary> 运营活动系统-大地鼠彩票页签 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        #endregion
        #region 数据处理
        #endregion
        #region 服务器消息
        #endregion
        #region 提供功能
        /// <summary>检测大地鼠彩票页签红点</summary>
        public bool CheckLotteryActivityRedPoint()
        {
            if (CheckLotteryActivityFirstRedPoint())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检测 大地鼠彩票页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckLotteryActivityFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "LotteryActivityFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 大地鼠彩票页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetLotteryActivityFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "LotteryActivityFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateLotteryActivityData);
            }
        }
        #endregion
    }

    /// <summary> 运营活动系统-七日目标(和七日签到是两个功能)</summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        /// <summary> 一页显示几个奖励 (用于分割数组) </summary>
        readonly uint pageRewardNum = 7;
        /// <summary> 七日目标活动标识 </summary>
        private uint SevenDaysTargetActivityKey = 0;
        /// <summary> 七日目标总积分 </summary>
        public uint SevenDaysTargetScore { get; private set; } = 0;
        /// <summary> 七日目标天数字典（数字代表第几天 活动id - 天数列表） </summary>
        public Dictionary<uint, List<uint>> TargetDaysDict { get; private set; } = new Dictionary<uint, List<uint>>();
        /// <summary> 七日目标任务列表（活动id - 开启天数 - 任务列表） </summary>
        private Dictionary<uint, Dictionary<uint, List<CSVActivityTarget.Data>>> dictSevenDaysTargets = new Dictionary<uint, Dictionary<uint, List<CSVActivityTarget.Data>>>();
        /// <summary> 七日目标任务状态 字典</summary>
        private Dictionary<uint, TargetTask> dictSevenDaysTargetTasks = new Dictionary<uint, TargetTask>();
        /// <summary> 七日目标分数奖励状态 字典</summary>
        private Dictionary<uint, ScoreReward> dictSevenDaysTargetScoreRewards = new Dictionary<uint, ScoreReward>();
        /// <summary> 七日目标奖励列表字典 (按页分组) </summary>
        private Dictionary<uint, List<List<CSVCumulativeReward.Data>>> dictTargetDaysReward = new Dictionary<uint, List<List<CSVCumulativeReward.Data>>>();
        /// <summary> 七日目标奖励档位所需积分列表 </summary>
        private Dictionary<uint, List<uint>> dictTargetDaysRewardScore = new Dictionary<uint, List<uint>>();
        /// <summary> 七日目标 是否同步过活动开启信息 </summary>
        private bool isSendActivityStart = false;
        /// <summary> 七日目标计时器 </summary>
        private Timer SevenDaysTargetTimer;
        /// <summary> 七日目标 服务器同步的活动开启天数  days * 86400 就是当天0点时间戳</summary>
        public uint SevenDaysTargetDays { get; private set; } = 0;

        /// <summary> 是否弹出过七日目标跳脸界面 </summary>
        private bool isShowSevenDaysTargetPopup = false;
        private bool isShowExpRetrieveFaceTip = false;

        /// <summary>
        /// 是否登录初始化 (防止活动数据未初始化前走到界面刷新逻辑)
        /// </summary>
        private bool isLoginInit = false;
        #endregion
        #region 数据处理
        private void SevenDaysTargetInit()
        {
            //目标表
            var activityTargetDatas = CSVActivityTarget.Instance.GetAll();
            for (int i = 0, len = activityTargetDatas.Count; i < len; i++)
            {
                CSVActivityTarget.Data data = activityTargetDatas[i];
                //天数列表
                if (!TargetDaysDict.TryGetValue(data.Activityid, out List<uint> dayList))
                {
                    dayList = new List<uint>();
                    TargetDaysDict[data.Activityid] = dayList;
                }
                if (!dayList.Contains(data.RankType))
                {
                    dayList.Add(data.RankType);
                }
                //目标任务列表
                if (!dictSevenDaysTargets.TryGetValue(data.Activityid, out Dictionary<uint, List<CSVActivityTarget.Data>> taskDic))
                {
                    taskDic = new Dictionary<uint, List<CSVActivityTarget.Data>>();
                    dictSevenDaysTargets[data.Activityid] = taskDic;
                }
                if (!taskDic.TryGetValue(data.RankType, out List<CSVActivityTarget.Data> taskList))
                {
                    taskList = new List<CSVActivityTarget.Data>();
                    taskDic[data.RankType] = taskList;
                }
                taskList.Add(data);
            }
            foreach (var data in TargetDaysDict)
            {
                data.Value.Sort();
            }
            //累计奖励表
            var cumulativeRewardDatas = CSVCumulativeReward.Instance.GetAll();
            for (int i = 0, len = cumulativeRewardDatas.Count; i < len; i++)
            {
                CSVCumulativeReward.Data rewardData = cumulativeRewardDatas[i];
                if (!dictTargetDaysReward.TryGetValue(rewardData.Activityid, out List<List<CSVCumulativeReward.Data>> rewardList))
                {
                    rewardList = new List<List<CSVCumulativeReward.Data>>();
                    dictTargetDaysReward[rewardData.Activityid] = rewardList;
                }
                if (rewardList.Count <= 0 || rewardList[rewardList.Count - 1].Count >= pageRewardNum)
                {
                    rewardList.Add(new List<CSVCumulativeReward.Data>());
                }
                rewardList[rewardList.Count - 1].Add(rewardData);
                if (!dictTargetDaysRewardScore.TryGetValue(rewardData.Activityid, out List<uint> scoreList))
                {
                    scoreList = new List<uint>();
                    dictTargetDaysRewardScore[rewardData.Activityid] = scoreList;
                }
                scoreList.Add(rewardData.Requiredpoints);
            }
        }
        private void SevenDaysTargetOnLogin()
        {
            //RefreshSevenDaysTargetOpenState();
        }
        private void SevenDaysTargetOnLogout()
        {
            //登出的时候清一下服务器下发的状态
            dictSevenDaysTargetTasks.Clear();
            dictSevenDaysTargetScoreRewards.Clear();

            SevenDaysTargetTimer?.Cancel();
            isSendActivityStart = false;
            isShowExpRetrieveFaceTip = false;
            SevenDaysTargetActivityKey = 0;
            SevenDaysTargetScore = 0;
            SevenDaysTargetDays = 0;
            isLoginInit = false;
        }
        #endregion
        #region 服务器消息
        /// <summary> 七日目标数据同步 </summary>
        private void OnSevenDaysTargetNtf(NetMsg msg)
        {
            CmdActivityTargetTwoWeekData ntf = NetMsgUtil.Deserialize<CmdActivityTargetTwoWeekData>(CmdActivityTargetTwoWeekData.Parser, msg);
            SevenDaysTargetScore = ntf.Score;
            SevenDaysTargetDays = ntf.Days;
            //Debug.Log("SevenDaysTarget 数据同步Days " + ntf.Days);
            StartSevenDaysEndTimer();
            var rewards = ntf.Rewards;
            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                dictSevenDaysTargetScoreRewards[reward.Index] = reward;
            }
            var tasks = ntf.TaskList;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                dictSevenDaysTargetTasks[task.TaskId] = task;
            }
            RefreshSevenDaysTargetOpenState();
            SyncSevenDaysTargetActivityStart();
            isLoginInit = true;
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }
        /// <summary> 七日目标单条目标数据同步 </summary>
        private void OnSevenDaysTargetDataUpdateNtf(NetMsg msg)
        {
            CmdActivityTargetTwoWeekDataUpdate ntf = NetMsgUtil.Deserialize<CmdActivityTargetTwoWeekDataUpdate>(CmdActivityTargetTwoWeekDataUpdate.Parser, msg);
            var task = ntf.Task;
            dictSevenDaysTargetTasks[task.TaskId] = task;
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }
        /// <summary> 七日目标单条目标奖励领取请求 </summary>
        public void GetSevenDaysTargetTaskRewardReq(uint taskId)
        {
            CmdActivityTargetTwoWeekTaskRewardReq req = new CmdActivityTargetTwoWeekTaskRewardReq();
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort)CmdActivityTarget.TwoWeekTaskRewardReq, req);
        }
        /// <summary> 七日目标单条目标奖励领取回调 </summary>
        private void OnGetSevenDaysTargetTaskRewardRes(NetMsg msg)
        {
            CmdActivityTargetTwoWeekTaskRewardRes res = NetMsgUtil.Deserialize<CmdActivityTargetTwoWeekTaskRewardRes>(CmdActivityTargetTwoWeekTaskRewardRes.Parser, msg);
            uint taskId = res.TaskId;
            //客户端通过后端返回的taskId 更新本地数据
            if (dictSevenDaysTargetTasks.TryGetValue(taskId, out TargetTask task))
            {
                task.Progress = -1;
            }
            CSVActivityTarget.Data targetData = CSVActivityTarget.Instance.GetConfData(taskId);
            SevenDaysTargetScore += targetData.Points;
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }
        /// <summary> 七日目标分数奖励领取请求 </summary>
        public void GetSevenDaysTargetScoreRewardReq(uint index)
        {
            CmdActivityTargetTwoWeekScoreRewardReq req = new CmdActivityTargetTwoWeekScoreRewardReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdActivityTarget.TwoWeekScoreRewardReq, req);
        }
        /// <summary> 七日目标分数奖励领取回调 </summary>
        private void OnGetSevenDaysTargetScoreRewardRes(NetMsg msg)
        {
            CmdActivityTargetTwoWeekScoreRewardRes res = NetMsgUtil.Deserialize<CmdActivityTargetTwoWeekScoreRewardRes>(CmdActivityTargetTwoWeekScoreRewardRes.Parser, msg);
            var rewardId = res.Index;
            //客户端通过后端返回的rewardId 更新本地数据
            if (dictSevenDaysTargetScoreRewards.TryGetValue(rewardId, out ScoreReward reward))
            {
                reward.IsGot = true;
            }
            else
            {
                ScoreReward mReward = new ScoreReward();
                mReward.Index = rewardId;
                mReward.IsGot = true;
                dictSevenDaysTargetScoreRewards[rewardId] = mReward;
            }
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }
        /// <summary> 七日目标活动开启请求 </summary>
        public void SevenDaysTargetStartReq()
        {
            CmdActivityTargetActivityStartReq req = new CmdActivityTargetActivityStartReq();
            req.ActId = SevenDaysTargetActivityKey;
            NetClient.Instance.SendMessage((ushort)CmdActivityTarget.ActivityStartReq, req);
        }
        /// <summary> 七日目标活动开启回调 </summary>
        private void OnSevenDaysTargetStartRes(NetMsg msg)
        {
            CmdActivityTargetActivityStartRes res = NetMsgUtil.Deserialize<CmdActivityTargetActivityStartRes>(CmdActivityTargetActivityStartRes.Parser, msg);
            SevenDaysTargetActivityKey = res.ActId;
            //Debug.Log("SevenDaysTarget 同步数据请求回调");
            SevenDaysTargetDays = res.Days;
            isSendActivityStart = true;
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }

        #endregion
        #region 提供功能
        /// <summary>
        /// 检测七日目标功能是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckSevenDaysTargetIsOpen()
        {
            if (CheckActivitySwitchIsOpen(202))
            {
                CSVActivityTime.Data data = CSVActivityTime.Instance.GetConfData(SevenDaysTargetActivityKey);
                if (data != null)
                {
                    bool isOpen = data.Conditionid <= 0 || Sys_FunctionOpen.Instance.IsOpen(data.Conditionid);
                    if (isOpen)
                    {
                        if (CheckSevenDaysTargetIsPopuped())
                        {
                            UIManager.OpenUI(EUIID.UI_SevenDaysTargetPopup);
                        }
                    }
                    return isOpen && isLoginInit;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测七日目标活动对应天数内容是否开启
        /// </summary>
        public bool CheckSevenDaysTargetDayTaskIsOpen(uint dayNum, bool showMsg = false)
        {
            CSVActivityTime.Data data = CSVActivityTime.Instance.GetConfData(SevenDaysTargetActivityKey);
            if (data != null)
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                //var startTimeDate = data.Time;
                var openTime = (SevenDaysTargetDays + dayNum - 1) * 86400;
                bool isOpen = nowTime >= openTime;
                if (showMsg && !isOpen)
                {
                    //var timeStr = LanguageHelper.TimeToString((uint)(openTime - nowTime), LanguageHelper.TimeFormat.Type_4);
                    //Debug.Log(timeStr + "后解锁");
                }
                return isOpen;
            }
            return false;
        }
        /// <summary>
        /// 获取七日目标活动结束倒计时
        /// </summary>
        public uint GetSevenDaysTargetCountDownTime()
        {
            CSVActivityTime.Data data = CSVActivityTime.Instance.GetConfData(SevenDaysTargetActivityKey);
            if (data != null)
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                //var startTime = data.Time;
                var endTime = (data.Day + SevenDaysTargetDays) * 86400;
                return endTime - nowTime;
            }
            return 0;
        }
        /// <summary>
        /// 检测 七日目标 总红点
        /// </summary>
        public bool CheckSevenDaysTargetAllRedPoint()
        {
            var dayList = GetSevenDaysTargetDayList();
            //目标红点
            for (int i = 0; i < dayList.Count; i++)
            {
                var dayNum = dayList[i];
                if (CheckSevenDaysTargetTaskRedPoint(dayNum))
                {
                    return true;
                }
            }
            //累计奖励红点
            var rewardLists = GetSevenDaysTargetRewardLists();
            for (int i = 0; i < rewardLists.Count; i++)
            {
                var list = rewardLists[i];
                for (int j = 0; j < list.Count; j++)
                {
                    var rewardData = list[j];
                    if (CheckSevenDaysTargetRewardCanGet(rewardData.id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检测 七日目标 单日红点
        /// </summary>
        public bool CheckSevenDaysTargetTaskRedPoint(uint dayNum)
        {
            var targetDict = GetSevenDaysTargetTaskDict();
            if (CheckSevenDaysTargetDayTaskIsOpen(dayNum) && targetDict.TryGetValue(dayNum, out List<CSVActivityTarget.Data> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    var isFinish = CheckTargetIsFinish(data.id);
                    var isGet = CheckTargetTaskRewardIsGet(data.id);
                    if (isFinish && !isGet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检测七日目标是否弹窗过
        /// </summary>
        /// <returns></returns>
        public bool CheckSevenDaysTargetIsPopuped()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SevenDaysTargetIsPopuped";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置七日目标是否弹窗过
        /// </summary>
        /// <returns></returns>
        public void SetSevenDaysTargetIsPopuped()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "SevenDaysTargetIsPopuped";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
            }
        }
        /// <summary>
        /// 检测七日目标单日奖励全部领完 
        /// </summary>
        public bool CheckSevenDaysTargetIsGetByDay(uint dayNum)
        {
            var targetDict = GetSevenDaysTargetTaskDict();
            if (CheckSevenDaysTargetDayTaskIsOpen(dayNum) && targetDict.TryGetValue(dayNum, out List<CSVActivityTarget.Data> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    var isGet = CheckTargetTaskRewardIsGet(data.id);
                    if (!isGet)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检测 七日目标 累计奖励(单个奖励)是否可领取/红点
        /// </summary>
        public bool CheckSevenDaysTargetRewardCanGet(uint rewardId)
        {
            CSVCumulativeReward.Data rewardData = CSVCumulativeReward.Instance.GetConfData(rewardId);
            //return SevenDaysTargetScore > rewardData.Requiredpoints;
            if (rewardData != null)
            {
                var myScore = SevenDaysTargetScore;
                if (dictSevenDaysTargetScoreRewards.TryGetValue(rewardId, out ScoreReward scoreReward))
                {
                    return !scoreReward.IsGot && myScore >= rewardData.Requiredpoints;
                }
                else
                {
                    return myScore >= rewardData.Requiredpoints;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测 七日目标 累计奖励(单个奖励)是否已领取
        /// </summary>
        public bool CheckSevenDaysTargetRewardIsGet(uint rewardId)
        {
            if (dictSevenDaysTargetScoreRewards.TryGetValue(rewardId, out ScoreReward scoreReward))
            {
                return scoreReward.IsGot;
            }
            return false;
        }
        /// <summary>
        /// 获取七日目标 当前活动的天数列表
        /// </summary>
        public List<uint> GetSevenDaysTargetDayList()
        {
            if (TargetDaysDict.TryGetValue(SevenDaysTargetActivityKey, out List<uint> dayList))
            {
                return dayList;
            }
            DebugUtil.Log(ELogType.eNone, "七日目标天数列表获取错误，未找到活动id为 " + SevenDaysTargetActivityKey.ToString() + " 的数据");
            return new List<uint>();
        }
        /// <summary>
        /// 获取七日目标 当前活动(天数-目标列表)的字典数据
        /// </summary>
        public Dictionary<uint, List<CSVActivityTarget.Data>> GetSevenDaysTargetTaskDict()
        {
            if (dictSevenDaysTargets.TryGetValue(SevenDaysTargetActivityKey, out Dictionary<uint, List<CSVActivityTarget.Data>> dict))
            {
                return dict;
            }
            DebugUtil.Log(ELogType.eNone, "七日目标任务字典获取错误，未找到活动id为 " + SevenDaysTargetActivityKey.ToString() + " 的数据");
            return null;
        }
        /// <summary>
        /// 获取七日目标 七日目标奖励列表(按页分组)
        /// </summary>
        public List<List<CSVCumulativeReward.Data>> GetSevenDaysTargetRewardLists()
        {
            if (dictTargetDaysReward.TryGetValue(SevenDaysTargetActivityKey, out List<List<CSVCumulativeReward.Data>> lists))
            {
                return lists;
            }
            DebugUtil.Log(ELogType.eNone, "七日目标奖励列表获取错误，未找到活动id为 " + SevenDaysTargetActivityKey.ToString() + " 的数据");
            return new List<List<CSVCumulativeReward.Data>>();
        }
        /// <summary>
        /// 获取对应天数的目标列表(已排序)
        /// </summary>
        public List<CSVActivityTarget.Data> GetSevenDaysTargetList(uint dayNum)
        {
            var targetDict = GetSevenDaysTargetTaskDict();
            if (targetDict != null && targetDict.TryGetValue(dayNum, out List<CSVActivityTarget.Data> list))
            {
                //排个序，可领取>未完成>已领取(内部按照表Priority字段排序)
                List<CSVActivityTarget.Data> canGetList = new List<CSVActivityTarget.Data>();
                List<CSVActivityTarget.Data> unFinishList = new List<CSVActivityTarget.Data>();
                List<CSVActivityTarget.Data> isGetList = new List<CSVActivityTarget.Data>();
                for (int i = 0; i < list.Count; i++)
                {
                    var data = list[i];
                    if (dictSevenDaysTargetTasks.TryGetValue(data.id, out TargetTask taskInfo))
                    {
                        bool isGet = taskInfo.Progress == -1;
                        uint maxNum = GetTargetTaskProgressMaxNum(data.id);
                        bool isFinish = taskInfo.Progress >= maxNum;
                        if (isGet)
                        {
                            isGetList.Add(data);
                        }
                        else
                        {
                            if (isFinish)
                            {
                                canGetList.Add(data);
                            }
                            else
                            {
                                unFinishList.Add(data);
                            }
                        }
                    }
                    else
                    {
                        unFinishList.Add(data);
                    }
                }
                isGetList = SortSevenDaysTargetListByPriority(isGetList);
                canGetList = SortSevenDaysTargetListByPriority(canGetList);
                unFinishList = SortSevenDaysTargetListByPriority(unFinishList);
                canGetList.AddRange(unFinishList);
                canGetList.AddRange(isGetList);
                return canGetList;
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "七日目标数据获取错误，未找到第 " + dayNum.ToString() + " 天的目标数据");
                return new List<CSVActivityTarget.Data>();
            }
        }
        /// <summary>
        /// 按照表Priority字段排序
        /// </summary>
        private List<CSVActivityTarget.Data> SortSevenDaysTargetListByPriority(List<CSVActivityTarget.Data> origList)
        {
            var count = origList.Count;
            for (int i = 0; i < count - 1; i++)
            {
                bool flag = false;
                for (int j = 0; j < count - i - 1; j++)
                {
                    if (origList[j].Priority > origList[j + 1].Priority)
                    {
                        var temp = origList[j];
                        origList[j] = origList[j + 1];
                        origList[j + 1] = temp;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            return origList;
        }
        /// <summary>
        /// 检测目标任务是否完成
        /// </summary>
        public bool CheckTargetIsFinish(uint targetId)
        {
            CSVActivityTarget.Data data = CSVActivityTarget.Instance.GetConfData(targetId);
            if (data != null)
            {
                if (dictSevenDaysTargetTasks.TryGetValue(data.id, out TargetTask taskInfo))
                {
                    if (taskInfo.Progress == -1)
                    {
                        //已经领取过，已完成
                        return true;
                    }
                }
                uint maxNum = GetTargetTaskProgressMaxNum(targetId);
                int curNum = GetTargetTaskProgressCurNum(targetId);
                return curNum >= maxNum;
            }
            return false;
        }
        /// <summary>
        /// 获取目标任务进度最大值
        /// </summary>
        public uint GetTargetTaskProgressMaxNum(uint targetId)
        {
            CSVActivityTarget.Data data = CSVActivityTarget.Instance.GetConfData(targetId);
            if (data != null && data.ReachTypeAchievement.Count > 0)
            {
                return data.ReachTypeAchievement[data.ReachTypeAchievement.Count - 1];
            }
            return 1;
        }
        /// <summary>
        /// 获取目标任务进度当前值
        /// </summary>
        public int GetTargetTaskProgressCurNum(uint targetId)
        {
            CSVActivityTarget.Data data = CSVActivityTarget.Instance.GetConfData(targetId);
            if (data != null)
            {
                if (dictSevenDaysTargetTasks.TryGetValue(targetId, out TargetTask taskInfo))
                {
                    var maxNum = (int)GetTargetTaskProgressMaxNum(targetId);
                    if (taskInfo.Progress == -1)
                    {
                        return maxNum;
                    }
                    else
                    {
                        var curNum = taskInfo.Progress;
                        return curNum < maxNum ? curNum : maxNum;
                    }
                }
                else
                {
                    if (data.TypeAchievement == 220)
                    {
                        //觉醒等级
                        return (int)Sys_TravellerAwakening.Instance.awakeLevel;
                    }
                    //先注释，不确定要不要
                    //if (data.TypeAchievement == 210)
                    //{
                    //    return (int)Sys_Role.Instance.Role.Level;
                    //}
                    //else if (data.TypeAchievement == 120)
                    //{
                    //    //主线任务
                    //    return TaskHelper.HasSubmited(data.ReachTypeAchievement[0]) ? 1 : 0;
                    //}
                }
            }
            return 0;
        }
        /// <summary>
        /// 检测目标任务奖励是否领取
        /// </summary>
        public bool CheckTargetTaskRewardIsGet(uint targetId)
        {
            CSVActivityTarget.Data data = CSVActivityTarget.Instance.GetConfData(targetId);
            if (data != null && dictSevenDaysTargetTasks.TryGetValue(data.id, out TargetTask taskInfo))
            {
                if (taskInfo.Progress == -1)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取七日目标当前页的累计奖励列表
        /// </summary>
        public List<CSVCumulativeReward.Data> GetSevenDaysTargetRewardList(int pageNum)
        {
            var lists = GetSevenDaysTargetRewardLists();
            if (lists.Count > pageNum)
            {
                return lists[pageNum];
            }
            //DebugUtil.Log(ELogType.eNone, "七日目标累计奖励数据获取错误，未找到第 " + pageNum.ToString() + " 页的累计奖励数据");
            return null;
        }
        /// <summary>
        /// 获取累计奖励积分进度条的宽度
        /// </summary>
        public float GetSevenDaysTargetScoreSliderWidth(int pageNum, uint cellWidth, uint maxWidth)
        {
            var myScore = SevenDaysTargetScore;
            float width = 0;
            var lists = GetSevenDaysTargetRewardLists();
            if (lists.Count > pageNum)
            {
                var rewardList = lists[pageNum];
                uint startIndex = 0;
                uint LastScore = 0;//上一阶段分数
                uint diffScore = 0;//当前阶段间的分数差
                uint LastDiffScore = 0;//上一阶段间的分数差
                for (int i = 0; i < rewardList.Count; i++)
                {
                    var rewardData = rewardList[i];
                    if (myScore >= rewardData.Requiredpoints)
                    {
                        startIndex++;
                        LastDiffScore = rewardData.Requiredpoints - LastScore;
                        LastScore = rewardData.Requiredpoints;
                    }
                    else
                    {
                        if (LastScore == 0)
                        {
                            if (pageNum == 0)
                            {
                                LastScore = 0;
                            }
                            else
                            {
                                var lastRewardList = lists[pageNum - 1];
                                LastScore = lastRewardList[lastRewardList.Count - 1].Requiredpoints;
                            }
                        }
                        diffScore = rewardData.Requiredpoints - LastScore;
                        break;
                    }
                }
                if (diffScore <= 0)
                {
                    diffScore = LastDiffScore / 2;
                }
                if (myScore <= 0 || myScore < LastScore)
                {
                    width = 0;
                }
                else
                {
                    if (startIndex <= 0)
                    {
                        //第一段只有一半
                        width = cellWidth / 2 * (myScore - LastScore) / diffScore;
                    }
                    else
                    {

                        width = startIndex * cellWidth - cellWidth / 2 + cellWidth * (myScore - LastScore) / diffScore;
                    }
                    return width <= 0 ? 0 : (width >= maxWidth ? maxWidth : width);
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取七日目标任务内容描述
        /// </summary>
        public string GetSevenDaysTargetTaskDesc(CSVActivityTarget.Data data)
        {
            var type = data.TypeAchievement;
            var paramList = data.ReachTypeAchievement;
            if (type == 220)
            {
                //达到xx觉醒等级
                var TravelData = CSVTravellerAwakening.Instance.GetConfData(paramList[0]);
                if (TravelData != null)
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(TravelData.NameId));
                }
            }
            else if (type == 230)
            {
                //当前职业到X阶 读取配置对应的职业名称
                var careerId = Sys_Role.Instance.Role.Career * 100 + paramList[0];
                CSVPromoteCareer.Data careerData = CSVPromoteCareer.Instance.GetConfData(careerId);
                if (careerData != null)
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(careerData.professionLan));
                }
            }
            else if (type == 432)
            {
                //一个参数，纯语言表
                return LanguageHelper.GetTextContent(data.Langid);
            }
            else if (paramList.Count == 1 || type == 273)
            {
                //一个空位，参数只有一个/读参数第一个位置
                return LanguageHelper.GetTextContent(data.Langid, paramList[0].ToString());
            }
            else if (type == 121 || type == 130 || type == 130)
            {
                //一个空位，读参数的第二个位置
                return LanguageHelper.GetTextContent(data.Langid, paramList[1].ToString());
            }
            else if (type == 273 || type == 281 || type == 340)
            {
                //两个空位 反序
                if (paramList.Count > 1)
                {
                    return LanguageHelper.GetTextContent(data.Langid, paramList[1].ToString(), paramList[0].ToString());
                }
            }
            else if (type == 561 || type == 571)
            {
                //两个空位 正序
                if (paramList.Count > 1)
                {
                    return LanguageHelper.GetTextContent(data.Langid, paramList[0].ToString(), paramList[1].ToString());
                }
            }
            else if (type == 260)
            {
                //三个空位
                if (paramList.Count > 2)
                {
                    return LanguageHelper.GetTextContent(data.Langid, paramList[2].ToString(), paramList[0].ToString(), paramList[1].ToString());
                }
            }
            else if (type == 270 || type == 733)
            {
                //第三个空位显示品质颜色
                if (paramList.Count > 2)
                {
                    List<uint> colorId = new List<uint> { 590001805, 590001806, 590001807, 590001808, 590001809 };
                    string strColor = LanguageHelper.GetTextContent(colorId[(int)paramList[1] - 1]);
                    return LanguageHelper.GetTextContent(data.Langid, paramList[2].ToString(), paramList[0].ToString(), strColor);
                }
            }
            else if (type == 100 || type == 101 || type == 102)
            {
                //消耗道具/获得道具 先只显示数量
                return LanguageHelper.GetTextContent(data.Langid, paramList[1].ToString());
                //CSVItem.Data item = CSVItem.Instance.GetConfData(paramList[0]);
                //if (item != null)
                //{
                //    return LanguageHelper.GetTextContent(data.Langid, paramList[1].ToString(), LanguageHelper.GetTextContent(item.name_id));
                //}
            }
            else if (type == 110)
            {
                //百人道场 副本关卡表
                CSVInstanceDaily.Data instanceData = CSVInstanceDaily.Instance.GetConfData(paramList[0]);
                if (instanceData != null)
                {
                    var num = (instanceData.LayerStage - 1) * 10 + instanceData.Layerlevel;
                    return LanguageHelper.GetTextContent(data.Langid, num.ToString());
                }
            }
            else if (type == 111)
            {
                //人物传记 副本关卡表
                CSVInstanceDaily.Data instanceData = CSVInstanceDaily.Instance.GetConfData(paramList[0]);
                if (instanceData != null)
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(instanceData.Name));
                }
            }
            else if (type == 115)
            {
                //经典头目表
                CSVClassicBoss.Data bossData = CSVClassicBoss.Instance.GetConfData(paramList[0]);
                if (bossData != null)
                {
                    CSVNpc.Data npc = CSVNpc.Instance.GetConfData(bossData.NPCID);
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetNpcTextContent(npc.name));
                }
            }
            else if (type == 120)
            {
                //任务
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(paramList[0]);
                if (taskData != null)
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(taskData.taskName));
                }
            }
            else if (type == 731)
            {
                //生活技能
                var skillId = paramList[0];
                var skillLv = paramList[1];
                if (skillId > 0)
                {
                    CSVLifeSkill.Data skillData = CSVLifeSkill.Instance.GetConfData(skillId);
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(skillData.name_id), skillLv.ToString());
                }
                else
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(590001800), skillLv.ToString());//任意
                }
            }
            else if (type == 240)
            {
                //声望达到x段x等级
                uint rank = paramList[0] / 100;
                uint lv = paramList[0] % 100;
                CSVFameRank.Data fameRankData = CSVFameRank.Instance.GetConfData(rank);
                if (fameRankData != null)
                {
                    return LanguageHelper.GetTextContent(data.Langid, LanguageHelper.GetTextContent(fameRankData.name), lv.ToString());
                }
            }
            else
            {
                return LanguageHelper.GetTextContent(data.Langid);
            }
            return "";
        }
        /// <summary>
        /// 七日目标任务前往跳转
        /// </summary>
        public void SevenDaysTargetTaskToGo(CSVActivityTarget.Data data)
        {
            if (data != null)
            {
                if (data.Functionid > 0 && !Sys_FunctionOpen.Instance.IsOpen(data.Functionid))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590001810));
                    return;
                }
                //Debug.Log("跳转目标：" + data.Tel_type.ToString() + "| 跳转参数" + data.Skip_Id.ToString());
                var type = data.Tel_type;
                List<uint> jumpPrama = data.Skip_Id;
                if (jumpPrama[0] == 311)//EUIID.UI_Family
                {
                    //家族跳转特殊处理
                    bool isInFamily = Sys_Family.Instance.familyData.isInFamily;
                    if (!isInFamily)
                    {
                        UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                        UIManager.OpenUI(EUIID.UI_ApplyFamily);
                        return;
                    }
                }
                //先关界面再跳转(有些界面返回主界面会被hide)
                switch (type)
                {
                    //1: 跳转界面 Id;  2:跳转界面Iid和子界面Id;  3: 日常活动 界面 4:只有提示，没有前往。5：前往npc
                    case 1:
                        {
                            //EUIID.UI_Society
                            if (jumpPrama[0] == 94)
                            {
                                //好友跳转特殊处理
                                UIManager.OpenUI((int)jumpPrama[0]);
                            }
                            else
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                UIManager.OpenUI((int)jumpPrama[0]);
                            }
                        }
                        break;
                    case 2:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama[0] == 58)//EUIID.UI_SkillUpgrade
                            {
                                UIManager.OpenUI((int)jumpPrama[0], false, new List<int> { (int)jumpPrama[1] });
                            }
                            else if (jumpPrama[0] == 125)//EUIID.UI_Pet_Message
                            {
                                PetPrama petPrama = new PetPrama
                                {
                                    page = (EPetMessageViewState)jumpPrama[1]
                                };
                                UIManager.OpenUI((int)jumpPrama[0], false, petPrama);
                            }
                            else if (jumpPrama[0] == 377)
                            {
                                MagicbookEvt evt = new MagicbookEvt();
                                evt.type = (EMagicBookViewType)jumpPrama[1];
                                if (jumpPrama.Count > 2)
                                {
                                    evt.subChapterId = jumpPrama[2];//子章节id
                                }
                                UIManager.OpenUI(EUIID.UI_MagicBook, false, evt);
                            }
                            else
                            {
                                UIManager.OpenUI((int)jumpPrama[0], false, jumpPrama[1]);
                            }
                        }
                        break;
                    case 3:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama != null && jumpPrama.Count > 0)
                            {
                                UIDailyActivitesParmas uiDaily = new UIDailyActivitesParmas();
                                uiDaily.IsSkipDetail = true;
                                uiDaily.SkipToID = jumpPrama[0];
                                UIManager.OpenUI(EUIID.UI_DailyActivites, false, uiDaily);
                            }
                            else
                            {
                                UIManager.OpenUI(EUIID.UI_DailyActivites);
                            }
                        }
                        break;
                    case 4:
                        {
                            //弹个提示，参数是语言表id
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(jumpPrama[0]));
                        }
                        break;
                    case 5:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(jumpPrama[0]);
                        }
                        break;
                    case 6://6 跳转多重界面(界面1|子id|界面2)
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama.Count >= 2)
                            {
                                UIManager.OpenUI((int)jumpPrama[0], false, jumpPrama[1]);
                                UIManager.OpenUI((int)jumpPrama[2]);
                            }
                        }
                        break;
                    case 7://7 商店类型跳转( 商城界面ID | 商城表ID | 商店ID)
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama.Count >= 2)
                            {
                                MallPrama mallPrama = new MallPrama();
                                mallPrama.mallId = jumpPrama[1];
                                mallPrama.shopId = jumpPrama[2];
                                UIManager.OpenUI((int)jumpPrama[0], false, mallPrama);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 刷新七日目标活动开启状态 (登陆、当前活动结束、下场活动开启、服务器同步时间 时调用)
        /// </summary>
        private void RefreshSevenDaysTargetOpenState()
        {
            bool isOpen = false;
            uint nextActivityTimestamp = 0;
            //解析活动时间表，算出当前活动标识
            var nowTime = Sys_Time.Instance.GetServerTime();
            var activityCountDatas = CSVActivityTime.Instance.GetAll();
            for (int i = 0, len = activityCountDatas.Count; i < len; i++)
            {
                CSVActivityTime.Data data = activityCountDatas[i];
                var startTime = data.Time;
                var endTime = startTime + data.Day * 86400;
                //Debug.Log("七日目标开启时间" + startTime + " | " + startTimeDate.ToString() + " | 当前时间戳" + nowTime);
                var isOver = SevenDaysTargetDays > 0 && nowTime >= (data.Day + SevenDaysTargetDays) * 86400;
                //DateTime startTimeData = TimeZone.CurrentTimeZone.ToLocalTime(Consts.START_TIME);
                //Debug.Log("刷新七日目标活动开启状态 SevenDaysTargetDays:" + SevenDaysTargetDays + "|nowTime " + startTimeData.AddSeconds(nowTime).ToString() + "|emdTime " + startTimeData.AddSeconds((data.Day + SevenDaysTargetDays) * 86400).ToString());
                if (nowTime >= startTime && !isOver)
                {
                    isOpen = true;
                    SevenDaysTargetActivityKey = data.id;
                    //启动活动关闭倒计时 改到服务器下发开启天数之后再启动
                    //StartSevenDaysTargetTimer(endTime);
                    //目前表里只有一个活动，当有多个活动时会有问题
                    break;
                }
                else if (nowTime < startTime)
                {
                    //记录下一场活动开启时间戳
                    if (nextActivityTimestamp <= 0)
                    {
                        nextActivityTimestamp = startTime;
                    }
                    else
                    {
                        nextActivityTimestamp = nextActivityTimestamp < startTime ? nextActivityTimestamp : startTime;
                    }
                }
            }
            if (!isOpen)
            {
                SevenDaysTargetActivityKey = 0;
                isSendActivityStart = false;
                //当前非活动时间 开启下一场活动开启的倒计时
                StartSevenDaysTargetTimer(nextActivityTimestamp);
            }
            eventEmitter.Trigger(EEvents.UpdateSevenDaysTargetData);
        }
        /// <summary> 启动七日目标活动更新倒计时 </summary>
        private void StartSevenDaysTargetTimer(uint targetTime)
        {
            if (targetTime > 0)
            {
                uint nowtime = Sys_Time.Instance.GetServerTime();
                uint cd = targetTime - nowtime;
                SevenDaysTargetTimer?.Cancel();
                SevenDaysTargetTimer = Timer.Register(cd, RefreshSevenDaysTargetOpenState, null, false, false);
            }
        }
        /// <summary> 启动七日目标活动关闭倒计时 封装一层</summary>
        private void StartSevenDaysEndTimer()
        {
            CSVActivityTime.Data data = CSVActivityTime.Instance.GetConfData(SevenDaysTargetActivityKey);
            if (data != null && SevenDaysTargetDays > 0)
            {
                var endTime = (data.Day + SevenDaysTargetDays) * 86400;
                StartSevenDaysTargetTimer(endTime);
            }
        }
        /// <summary> 获取七日目标界面开启时的选中天数页签 </summary>
        public uint GetSevenDaysTaretPageNum()
        {
            var dayList = GetSevenDaysTargetDayList();
            uint unFinishDay = 0;
            //目标红点
            for (int i = 0; i < dayList.Count; i++)
            {
                var dayNum = dayList[i];
                if (CheckSevenDaysTargetTaskRedPoint(dayNum))
                {
                    return dayNum;
                }
                if (unFinishDay <= 0 && CheckSevenDaysTargetDayTaskIsOpen(dayNum))
                {
                    var targetDict = GetSevenDaysTargetTaskDict();
                    if (targetDict != null && targetDict.TryGetValue(dayNum, out List<CSVActivityTarget.Data> list))
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            var data = list[j];
                            if (dictSevenDaysTargetTasks.TryGetValue(data.id, out TargetTask taskInfo))
                            {
                                uint maxNum = GetTargetTaskProgressMaxNum(data.id);
                                bool isFinish = taskInfo.Progress == -1 || taskInfo.Progress >= maxNum;
                                if (!isFinish)
                                {
                                    unFinishDay = dayNum;
                                    break;
                                }
                            }
                            else
                            {
                                unFinishDay = dayNum;
                                break;
                            }
                        }
                    }
                }
            }
            return unFinishDay > 0 ? unFinishDay : 1;
        }
        /// <summary> 获取七日目标界面开启时显示的奖励页数 </summary>
        public int GetSevenDaysTaretRewardPageNum()
        {
            if (dictTargetDaysReward.TryGetValue(SevenDaysTargetActivityKey, out List<List<CSVCumulativeReward.Data>> lists))
            {
                for (int i = 0; i < lists.Count; i++)
                {
                    var list = lists[i];
                    for (int j = 0; j < list.Count; j++)
                    {
                        var data = list[j];
                        if (dictSevenDaysTargetScoreRewards.TryGetValue(data.id, out ScoreReward reward))
                        {
                            if (!reward.IsGot)
                            {
                                return i;
                            }
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary> 同步七日登陆开启数据 </summary>
        private void SyncSevenDaysTargetActivityStart()
        {
            if (!isSendActivityStart && SevenDaysTargetDays <= 0)
            {
                //Debug.Log("SevenDaysTarget 同步数据请求");
                SevenDaysTargetStartReq();
            }
        }
        /// <summary> 获取七日目标大奖图片路径 </summary>
        public string GetSevenDaysTargetBigRewardAddr()
        {
            var rewardLists = GetSevenDaysTargetRewardLists();
            string cacheAddr = "";
            for (int i = 0; i < rewardLists.Count; i++)
            {
                var list = rewardLists[i];
                for (int j = 0; j < list.Count; j++)
                {
                    var rewardData = list[j];
                    string imgAddr = rewardData.Background;
                    if (imgAddr != null && imgAddr.Length > 0)
                    {
                        cacheAddr = imgAddr;
                        if (!CheckSevenDaysTargetRewardIsGet(rewardData.id))
                        {
                            return imgAddr;
                        }
                    }
                }
            }
            return cacheAddr;
        }
        /// <summary> 检测是否显示第一个大奖 </summary>
        public bool CheckSevenDaysTargetShowFirstBigReward()
        {
            var rewardLists = GetSevenDaysTargetRewardLists();
            if (rewardLists.Count > 0)
            {
                var list = rewardLists[0];
                return !CheckSevenDaysTargetRewardIsGet(list[list.Count - 1].id);
            }
            return true;
        }
        #endregion
    }

    /// <summary> 运营活动系统-每日礼包 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        public Dictionary<int, uint> giftGetLevelDic = new Dictionary<int, uint>();//礼包id-领取时等级
        public Dictionary<int, int> giftDic = new Dictionary<int, int>();//礼包id-领取状态
        public bool dayGift { get; set; }//每日免费奖励领取状态，true已领取
        public uint giftEndDay { get;private set; }//结束的天数 未购买为0
        
        public uint giftBagType { get; set; }
        public bool isSevenDayAllReceived { get; private set; }
        public bool isSevenDayBuy { get;private set; }
        public bool isMaxTpye { get; private set; }
        public bool isDailyGiftOpen { get; set; } = true;//每日礼包活动开关

        private CSVDaliyPacks.Data packetData;//该等级数据
        #endregion
        #region 数据处理
        private void InitGiftLevelDic(RepeatedField<uint> giftGetLevel)
        {
            giftGetLevelDic.Clear();
            for (int i = 0; i < giftGetLevel.Count; i++)
            {
                giftGetLevelDic[i] = giftGetLevel[i];
            }
        }
        public void DailygiftBagTypeInit()
        {
            uint roleLv = Sys_Role.Instance.Role.Level;
            giftBagType = CheckDailyGiftBagType(roleLv);
            packetData = CSVDaliyPacks.Instance.GetConfData(giftBagType);
            isMaxTpye = IsMaxGrade(giftBagType);
        }
        //检测是否到最大礼包类型
        private bool IsMaxGrade(uint _type)
        {
            if (GetNextGiftData(_type) == null)
            {
                return true;
            }
            return false;
        }
        //根据等级转换出礼包类型
        public uint CheckDailyGiftBagType(uint _roleLevel)
        {
            uint _Type = 1;
            bool isMax = false;
            packetData = CSVDaliyPacks.Instance.GetConfData(_Type);
            CSVDaliyPacks.Data nextData = GetNextGiftData(_Type);
            if (nextData == null)
            {
                return _Type;
            }
            while (_roleLevel >= nextData.need_level && !isMax)
            {
                packetData = nextData;
                _Type++;
                if (GetNextGiftData(_Type) != null)
                {
                    nextData = GetNextGiftData(_Type);
                }
                else
                {
                    isMax = true;
                    break;
                }
            }
            return _Type;

        }

        private CSVDaliyPacks.Data GetNextGiftData(uint gType)
        {
            if (CSVDaliyPacks.Instance.TryGetValue(gType + 1, out CSVDaliyPacks.Data cData) && cData != null)
            {
                return cData;
            }
            return null;

        }
        private void InitGiftDictionary(uint _giftState)
        {
            isSevenDayBuy = !(giftEndDay == 0);
            giftDic.Clear();
            for (int i = 1; i <= 3; i++)
            {
                giftDic.Add(i, StateCheck(_giftState,i));
            }
            isSevenDayAllReceived = IsSevenDayReceived();

        }
        private int StateCheck(uint giftState,int index)
        {//位运算(状态：0无，1购买未领，2购买已领)
            return (int)(giftState >> (index - 1) * 2) & 3;
        }
        private bool IsSevenDayReceived()
        {
            for (int i = 1; i <= 3; i++)
            {
                if (giftDic[i] != 2)
                {
                    return false;
                }
            }
            return true;
        }
        private uint EndDayConvert(uint endTimeStamp)
        {
            if (endTimeStamp == 0)
            {
                return 0;
            }

            DateTime startTime = TimeManager.GetDateTime(endTimeStamp - 604800);
            DateTime nowdata = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            TimeSpan sp = nowdata.Subtract(startTime);
            uint endDay = 7 - (uint)sp.Days;
            return endDay;

        }


        #endregion
        #region 服务器消息
        public void OnDailyGiftDataNtf(NetMsg msg)
        {//上线|在线跨天发|七天礼包
            CmdDailyGiftDataNtf ntf = NetMsgUtil.Deserialize<CmdDailyGiftDataNtf>(CmdDailyGiftDataNtf.Parser, msg);
            dayGift = ntf.DayGift;
            giftEndDay = EndDayConvert(ntf.GiftEndDay);
            InitGiftLevelDic(ntf.GiftGetLevel);//礼包领取时玩家等级数组，当玩家每次点开该页面时读取玩家当前等级，未领取的礼包内容需要根据玩家当前等级所在区间展示，已领取礼包则为领取时的等级。
            InitGiftDictionary(ntf.GiftState);//GiftState十进制数
            isDailyGiftOpen = CheckDailyGiftSwitch();
            eventEmitter.Trigger(EEvents.UpdateDailyGiftData);

        }
        public void OnDailyGiftOperatorReq(uint onType, uint index)
        {//onType(看协议DailyGiftOperatorType)1-购买一个,2-全部购买,3-领取奖励,4-免费礼包|index礼包类型1-3，0是七天礼包
            CmdDailyGiftOperatorReq req = new CmdDailyGiftOperatorReq();
            req.OpType = onType;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdDailyGift.OperatorReq, req);
        }
        private void OnDailyGiftOperatorAck(NetMsg msg)
        {//单个礼包|每日赠送礼包
            CmdDailyGiftOperatorAck ntf = NetMsgUtil.Deserialize<CmdDailyGiftOperatorAck>(CmdDailyGiftOperatorAck.Parser, msg);
            int nowState = 0;
            if (ntf.OpType == 4)
            {
                dayGift = true;
            }
            nowState = (ntf.OpType == 1) ? 1 : 2;
            if (ntf.Index != 0)
            {
                giftDic[(int)ntf.Index] = nowState;
                giftGetLevelDic[(int)ntf.Index] = Sys_Role.Instance.Role.Level;
            }
            isSevenDayAllReceived = IsSevenDayReceived();
            eventEmitter.Trigger(EEvents.UpdateDailyGiftData);

        }
        #endregion
        #region 提供功能
        public void OpenChargeBox()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(8203);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                MallPrama param = new MallPrama();
                param.mallId = 101u;
                param.isCharge = true;
                UIManager.OpenUI(EUIID.UI_Mall, false, param);
            }, 11830);
            PromptBoxParameter.Instance.SetCancel(true, () =>
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(599003016));//魔币不足
                UIManager.CloseUI(EUIID.UI_PromptBox, false, true);
            });
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 每日礼包开关提示
        /// </summary>
        public bool CheckDailyGiftSwitch()
        {
            return CheckActivitySwitchIsOpen(107);
        }
        public bool CheckDailyGiftIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(50910) && CheckDailyGiftSwitch();
        }
        /// <summary>
        /// 每日礼包红点
        /// </summary>
        /// <returns></returns>
        public bool CheckDailyGiftRedPoint()
        {
            if (!CheckDailyGiftIsOpen())
            {
                return false;
            }
            if (CheckDailyGiftFirstRedPoint())
            {
                return true;
            }
            if (!dayGift)
            {
                return true;
            }
            return CheckSingleGiftRedPoint(1) || CheckSingleGiftRedPoint(2) || CheckSingleGiftRedPoint(3) || CheckSevenDayRedPoint();

        }
        /// <summary>
        /// 检测 每日礼包页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckDailyGiftFirstRedPoint()
        {

            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "DailyGiftFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 每日礼包页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetDailyGiftFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "DailyGiftFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateDailyGiftData);
            }
        }
        public bool CheckSingleGiftRedPoint(int type)
        {
            if (giftDic[type] == 1)
            {
                return true;
            }
            return false;
        }
        public bool CheckSevenDayRedPoint()
        {
            if (giftEndDay != 0)
            {
                return !IsSevenDayReceived();
            }

            return false;
        }
        #endregion
    }

    /// <summary> 运营活动系统-排行榜活动 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        #endregion
        #region 数据处理
        #endregion
        #region 服务器消息
        #endregion
        #region 提供功能
        /// <summary>检测排行榜活动页签红点</summary>
        public bool CheckRankActivityRedPoint()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(50912) && CheckRankActivityFirstRedPoint())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 检测 排行榜活动页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckRankActivityFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "RankActivityFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 排行榜活动页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetRankActivityFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "RankActivityFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateRankActivityData);
            }
        }
        /// <summary>
        /// 跳转到排行榜活动网页
        /// </summary>
        public void JunpToRankActivityWebPage()
        {
            CSVParam.Data paramData = CSVParam.Instance.GetConfData(1289);
            string url = paramData.str_value;
            //string url = "https://molibb.kuaishou.com/gameranking";
            Application.OpenURL(url);
        }
        #endregion
    }

    /// <summary> 运营活动系统-限时礼包活动(水蓝鼠宝藏) </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        private Dictionary<uint, LimitGiftInfo> dictLimitGifts = new Dictionary<uint, LimitGiftInfo>();
        private List<uint> listLimitGiftIds = new List<uint>();

        #endregion
        #region 数据处理
        #endregion
        #region 服务器消息
        /// <summary> 限时礼包全部数据同步 </summary>
        private void OnLimitGiftDataNtf(NetMsg msg)
        {
            CmdActivityTargetLimitGiftDataNtf ntf = NetMsgUtil.Deserialize<CmdActivityTargetLimitGiftDataNtf>(CmdActivityTargetLimitGiftDataNtf.Parser, msg);
            var gifts = ntf.GiftInfos;
            dictLimitGifts.Clear();
            listLimitGiftIds.Clear();
            for (int i = 0; i < gifts.Count; i++)
            {
                var gift = gifts[i];
                dictLimitGifts[gift.Id] = gift;
                listLimitGiftIds.Add(gift.Id);
                //Debug.Log("TimelimitGift限时礼包全部数据同步 " + i + "|" + gift.Id + "|" + gift.Time);
            }
            eventEmitter.Trigger(EEvents.UpdateTimelimitGiftData);
        }
        /// <summary> 限时礼包单条数据同步 </summary>
        private void OnLimitGiftDataUpdate(NetMsg msg)
        {
            CmdActivityTargetLimitGiftDataUpdate ntf = NetMsgUtil.Deserialize<CmdActivityTargetLimitGiftDataUpdate>(CmdActivityTargetLimitGiftDataUpdate.Parser, msg);
            var gift = ntf.GiftInfo;
            if (!dictLimitGifts.TryGetValue(gift.Id, out LimitGiftInfo value))
            {
                listLimitGiftIds.Add(gift.Id);
                //Debug.Log("TimelimitGift限时礼包单条数据同步 |" + gift.Id + "|" + gift.Time);
            }
            dictLimitGifts[gift.Id] = gift;
            if (CheckTimelimitGiftIsValid(gift.Id))
            {
                UIManager.OpenUI(EUIID.UI_TimelimitGift, false, gift.Id);
                //埋点
                UIManager.HitPointShow(EUIID.UI_TimelimitGift, gift.Id.ToString());
            }
            eventEmitter.Trigger(EEvents.UpdateTimelimitGiftData);
        }
        /// <summary> 限时礼包购买请求 </summary>
        public void LimitGiftBuyReq(uint giftId)
        {
            CmdActivityTargetLimitGiftBuyReq req = new CmdActivityTargetLimitGiftBuyReq();
            req.Id = giftId;
            //Debug.Log("TimelimitGift限时礼包购买请求 " + giftId);
            NetClient.Instance.SendMessage((ushort)CmdActivityTarget.LimitGiftBuyReq, req);
        }
        /// <summary> 限时礼包购买回调 </summary>
        private void OnLimitGiftBuyRes(NetMsg msg)
        {
            CmdActivityTargetLimitGiftBuyRes res = NetMsgUtil.Deserialize<CmdActivityTargetLimitGiftBuyRes>(CmdActivityTargetLimitGiftBuyRes.Parser, msg);
            uint giftId = res.Id;
            if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
            {
                gift.Time = 0;
            }
            PopupTimelimitGiftRewardView(giftId);
            //Debug.Log("TimelimitGift限时礼包游戏代币购买成功广播");
            eventEmitter.Trigger(EEvents.UpdateTimelimitGiftData);
        }
        /// <summary> 限时礼包购买成功广播(充值成功后推送) </summary>
        private void OnLimitGiftBuyNtf(NetMsg msg)
        {
            CmdActivityTargetLimitGiftBuyNtf res = NetMsgUtil.Deserialize<CmdActivityTargetLimitGiftBuyNtf>(CmdActivityTargetLimitGiftBuyNtf.Parser, msg);
            uint giftId = res.Id;
            if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
            {
                gift.Time = 0;
            }
            PopupTimelimitGiftRewardView(giftId);
            //Debug.Log("TimelimitGift限时礼包RMB购买成功广播");
            eventEmitter.Trigger(EEvents.UpdateTimelimitGiftData);
        }
        #endregion
        #region 提供功能
        /// <summary> 检测限时礼包活动功能是否开启 </summary>
        public bool CheckTimelimitFunctionIsOpen()
        {
            return CheckActivitySwitchIsOpen(206);
        }
        /// <summary> 检测限时礼包主界面按钮是否显示 </summary>
        public bool CheckTimelimitMainBtnIsShow()
        {
            if (CheckTimelimitFunctionIsOpen() && CheckTimelimitHasValidGift())
            {
                return true;
            }
            return false;
        }
        /// <summary> 检测礼包是否有效(没过期) </summary>
        public bool CheckTimelimitGiftIsValid(uint giftId)
        {
            if (!CheckTimelimitFunctionIsOpen())
            {
                return false;
            }
            if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                CSVConditionalGift.Data giftData = CSVConditionalGift.Instance.GetConfData(giftId);
                if (nowTime < gift.Time && giftData != null && gift.Time - nowTime <= giftData.Second)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 检测是否存在没过期的礼包 </summary>
        private bool CheckTimelimitHasValidGift()
        {
            for (int i = 0; i < listLimitGiftIds.Count; i++)
            {
                var giftId = listLimitGiftIds[i];
                if (CheckTimelimitGiftIsValid(giftId))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>检测限时礼包总红点</summary>
        public bool CheckTimelimitGiftRedPoint()
        {

            return false;
        }

        /// <summary>
        /// 获取限时礼包倒计时
        /// </summary>
        public uint GetTimelimitGiftCountdown(uint id)
        {
            if (dictLimitGifts.TryGetValue(id, out LimitGiftInfo gift))
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                CSVConditionalGift.Data giftData = CSVConditionalGift.Instance.GetConfData(id);
                if (nowTime < gift.Time && giftData != null && gift.Time - nowTime <= giftData.Second)
                {
                    return gift.Time - nowTime;
                }
            }
            return 0;
        }
        /// <summary> 获取最快过期的礼包数据 (会返回null) </summary>
        public LimitGiftInfo GetTimelimitMinCDValidGift()
        {
            uint minValidTime = 0;
            LimitGiftInfo minCDGift = null;
            for (int i = 0; i < listLimitGiftIds.Count; i++)
            {
                var giftId = listLimitGiftIds[i];
                if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
                {
                    var nowTime = Sys_Time.Instance.GetServerTime();
                    if (nowTime < gift.Time && (minValidTime <= 0 || minValidTime > gift.Time))
                    {
                        minCDGift = gift;
                        minValidTime = gift.Time;
                    }
                }
            }
            return minCDGift;
        }
        /// <summary>
        /// 获取限时礼包数量
        /// </summary>
        /// <returns></returns>
        public uint GetTimelimitGiftsNum()
        {
            uint num = 0;
            for (int i = 0; i < listLimitGiftIds.Count; i++)
            {
                var giftId = listLimitGiftIds[i];
                if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
                {
                    var nowTime = Sys_Time.Instance.GetServerTime();
                    if (nowTime < gift.Time)
                    {
                        num++;
                    }
                }
            }
            return num;
        }
        /// <summary>
        /// 获取排序后的限时礼包列表 (按过期时间从早到晚排序)
        /// </summary>
        public List<LimitGiftInfo> GetTimelimitSortGifts()
        {
            List<LimitGiftInfo> gifts = new List<LimitGiftInfo>();
            for (int i = 0; i < listLimitGiftIds.Count; i++)
            {
                var giftId = listLimitGiftIds[i];
                if (dictLimitGifts.TryGetValue(giftId, out LimitGiftInfo gift))
                {
                    var nowTime = Sys_Time.Instance.GetServerTime();
                    if (nowTime < gift.Time)
                    {
                        if (gifts.Count > 0)
                        {
                            var flag = false;
                            for (int j = 0; j < gifts.Count; j++)
                            {
                                if (gift.Time < gifts[j].Time)
                                {
                                    gifts.Insert(j, gift);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                gifts.Add(gift);
                            }
                        }
                        else
                        {
                            gifts.Add(gift);
                        }
                    }
                }
            }

            return gifts;
        }
        /// <summary>
        /// 弹限时礼包奖励界面
        /// </summary>
        public void PopupTimelimitGiftRewardView(uint giftId)
        {
            if (CheckTimelimitHasValidGift())
            {
                UIManager.OpenUI(EUIID.UI_TimeLimitGift_Reward, false, giftId);
            }
            else
            {
                //等回到主界面再弹
                UIScheduler.Push(EUIID.UI_TimeLimitGift_Reward, giftId, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
            }
        }
        #endregion
    }

    /// <summary> 运营活动系统-经验找回 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        public Dictionary<uint, uint> expDic = new Dictionary<uint, uint>();//活动id/倍率或经验
        private List<uint> randomList = new List<uint>();
        public uint temporaryId;
        public uint temporaryValue;
        public int JumpIndex = 0;
        public bool expRetriveRedPoint = false;
        #endregion
        #region 数据处理
        #endregion
        #region 服务器消息
        private void OnCompensationExpNty(NetMsg msg)
        {//上线时发/跨天主动发
            CmdAttrCompensationExpNty ntf = NetMsgUtil.Deserialize<CmdAttrCompensationExpNty>(CmdAttrCompensationExpNty.Parser, msg);
            expDic.Clear();
            randomList.Clear();
            for (int i = 0; i < ntf.Total.Count; i++)
            {
                expDic[ntf.Total[i].Tid] = ntf.Total[i].Value;

            }
            isShowExpRetrieveFaceTip = CheckFaceTipsOpen();
            if (randomList.Count != 0)
            {
                RandomFaceTips();
            }
            expRetriveRedPoint = IsShowExpRetrieve();
            eventEmitter.Trigger(EEvents.UpdateExpRetrieveData);
        }
        public void OnGetCompensationExpReq(uint _id)
        {
            CmdAttrGetCompensationExpReq req = new CmdAttrGetCompensationExpReq();
            req.Tid = _id;
            NetClient.Instance.SendMessage((ushort)CmdAttr.GetCompensationExpReq, req);
        }
        private void OnRemoveCompensationNty(NetMsg msg)
        {
            CmdAttrRemoveCompensationNty ntf = NetMsgUtil.Deserialize<CmdAttrRemoveCompensationNty>(CmdAttrRemoveCompensationNty.Parser, msg);
            if (expDic.ContainsKey(ntf.Tid))
            {
                expDic.Remove(ntf.Tid);
            }
            eventEmitter.Trigger(EEvents.UpdateExpRetrieveData);

        }

        private void OnFreeChatUpdateNtf(NetMsg msg)
        {
            CmdChargeFreeChatUpdateNtf ntf = NetMsgUtil.Deserialize<CmdChargeFreeChatUpdateNtf>(CmdChargeFreeChatUpdateNtf.Parser, msg);
            //添加剩余次数提示
            Sys_Chat.Instance.PushMessage(ChatType.World, null, LanguageHelper.GetTextContent(12209, ntf.LeftNum.ToString()), Sys_Chat.EMessageProcess.IgnoreSimplify);
        }
        #endregion
        #region 提供功能
        private bool CheckFaceTipsOpen()
        {
            if (!IsShowExpRetrieve())
            {
                return false;
            }
            foreach (var item in expDic)
            {
                CSVexpRetrieve.Data eData = CSVexpRetrieve.Instance.GetConfData(item.Key);
                if (eData.Type == 2)
                {
                    float _value = item.Value / 10000.0f;
                    if (_value >= 2.6f)
                    {
                        randomList.Add(item.Key);
                        return true;
                    }

                }

            }
            return false;
        }

        private void RandomFaceTips()
        {
            int _index = new System.Random().Next(randomList.Count);
            temporaryId = randomList[_index];
            temporaryValue = expDic[temporaryId];
        }
        private void DestoryExpRetrieveData()
        {
            expDic.Clear();
            randomList.Clear();
            JumpIndex = 0;
            expRetriveRedPoint = false;
        }
        public void CheckShowFaceTip()
        {
            if (isShowExpRetrieveFaceTip)
            {
                isShowExpRetrieveFaceTip = false;
                UIManager.OpenUI(EUIID.UI_Face_ExpRetrieve);
            }
        }

        public bool CheckExpRetrieveRedPoint()
        {

            if (!IsShowExpRetrieve())
            {
                return false;
            }
            if (expRetriveRedPoint)
            {
                return true;
            }
            foreach (var item in expDic)
            {
                CSVexpRetrieve.Data eData = CSVexpRetrieve.Instance.GetConfData(item.Key);
                if (eData.Type == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsShowExpRetrieve()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(50902))
            {
                return false;
            }
            if (expDic.Count != 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 检测 经验找回页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckExpRetrieveFirstRedPoint()
        {

            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "ExpRetrieveFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 经验找回页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetExpRetrieveFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "ExpRetrieveFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateDailyGiftData);
            }
        }
        #endregion

    }
    /// <summary> 运营活动系统-夺宝活动 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义

        public class SingleFightType
        {
            public uint activityId;
            public List<SingleRoundData> roundsList = new List<SingleRoundData>();
            public uint nowDate = 0;//活动第几天
            public int InRound;//当前轮次，从0计数
            public uint startRecord = 0;
            public int recordRound = 0;
            public EActivityRulerType eType;
            public SingleFightType(uint _id, EActivityRulerType _type)
            {
                activityId = _id;
                eType = _type;
            }

            public void InitRoundsList()
            {
                roundsList.Clear();
                InRound = -1;
                recordRound = 0;
                if (Instance.FightActivityDic.ContainsKey(eType))
                {
                    nowDate = Instance.FightActivityDic[eType].dayCount;
                }
                int _index = 0;
                for (int i = 1; i <= CSVOneCoinLottey.Instance.Count; i++)
                {
                    CSVOneCoinLottey.Data _cData = CSVOneCoinLottey.Instance.GetConfData((uint)i);
                    if (_cData.Activity_Id == activityId)
                    {
                        for (int j = 0; j < _cData.Begin_Time.Count; j++)
                        {
                            SingleRoundData singleRound = new SingleRoundData(_index);
                            singleRound.InitRoundData(activityId, (uint)i, j, eType);
                            if (singleRound.singleState == 1)
                            {
                                InRound = _index;//记录当前进行中的轮次
                                recordRound = _index;//(无进行中则记录的是上一轮)
                            }
                            roundsList.Add(singleRound);
                            _index++;

                        }
                    }
                }

            }
            public void UpDateRoundList(RepeatedField<FightTreasureActivityRound> _act)
            {
                if (Instance.FightActivityDic.ContainsKey(eType))
                {
                    nowDate = Instance.FightActivityDic[eType].dayCount;
                }
                for (int i = 0; i < _act.Count; i++)
                {

                    UpdateRoundListSingle(_act[i]);
                    if (i == _act.Count - 1)
                    {
                        startRecord = _act[i].Round;
                    }
                }
                CheckNowRound();
            }
            public void UpdateRoundListSingle(FightTreasureActivityRound _singleround)
            {
                if (_singleround.Round == uint.MaxValue)
                {
                    return;
                }
                int _index = (int)_singleround.Round;
                if (_index > roundsList.Count - 1)
                {
                    DebugUtil.LogError("夺宝活动：" + activityId + "轮次不一致");
                    return;
                }
                roundsList[_index].UpdateSingleRoundData(_singleround);
                if (roundsList[_index].singleState == 1)
                {
                    InRound = _index;
                    recordRound = _index;
                }
            }

            public bool CheckFirstLogin()
            {//判断进行中轮次是否登录过,登陆过返回false
                if (Sys_Role.Instance.Role.Level == 1)
                {
                    return true;
                }
                if (InRound > 0)
                {
                    SingleRoundData sfd = roundsList[InRound];
                    DateTime lastLogin = TimeManager.GetDateTime(Instance.lastLoginTime);
                    if (lastLogin < sfd.endDate && lastLogin > sfd.startDate)
                    {
                        return false;
                    }
                }
                return true;
            }

            public void CheckNowRound()
            {
                InRound = -1;
                for (int i = 0; i < roundsList.Count; i++)
                {
                    roundsList[i].CheckSingleState();
                    if (roundsList[i].singleState == 1)
                    {
                        InRound = i;
                        recordRound = i;
                    }
                }
            }

            public int ReturnRealRound(DateTime dt)
            {
                int _index = 0;
                for (int i = 0; i < roundsList.Count; i++)
                {
                    if (dt < roundsList[i].startDate)
                    {
                        return 0;
                    }
                    if ((i + 1) >= roundsList.Count)
                    {
                        return roundsList.Count - 1;
                    }
                    if (dt <= roundsList[i + 1].startDate && dt >= roundsList[i].startDate)
                    {
                        return i;
                    }
                }

                return _index;
            }
        }
        public class SingleRoundData
        {
            public uint _aid;//活动id
            public uint applyNum;
            public List<string> roleNamesList = new List<string>();
            public uint singleState;//0—结束，1—进行中，2—未开始
            public uint _uid;//表id
            public int thisRoundIndex;//轮次index,0计数，显示时要加1
            public int thisdayIndex;//该行数据的index
            public DateTime startDate;
            public DateTime endDate;
            private EActivityRulerType _Type;
            private List<uint> startTime;//开始时间[0]小时[1]分钟
            private uint ThisRoundDate;
            private bool _isSwitch = false;
            private uint duraTime;

            public SingleRoundData(int _index)
            {
                thisRoundIndex = _index;
                applyNum = 0;
            }
            public void UpdateSingleRoundApplyCount(uint _num)
            {
                applyNum = _num;
            }

            public void UpdateSingleRoundData(FightTreasureActivityRound _info)//更新单轮数据—报名人数、获奖名单
            {
                applyNum = _info.ApplyNum;
                CheckSingleState();
                if (_info.RoleNames != null)
                {

                    InitRoleNamesList(_info.RoleNames);
                }
            }
            public void InitRoleNamesList(RepeatedField<ByteString> _names)
            {
                roleNamesList.Clear();
                if (_names != null)
                {
                    for (int i = 0; i < _names.Count; i++)
                    {
                        roleNamesList.Add(_names[i].ToStringUtf8());
                    }
                }

            }

            public void InitRoundData(uint aid, uint uid, int _thisDayIndex, EActivityRulerType type)//初始化该轮状态
            {
                _Type = type;
                _aid = aid;
                _uid = uid;

                CSVOneCoinLottey.Data _cData = CSVOneCoinLottey.Instance.GetConfData(_uid);
                startTime = _cData.Begin_Time[_thisDayIndex];
                ThisRoundDate = _cData.Date;
                duraTime = _cData.Duration_Time[_thisDayIndex];
                thisdayIndex = _thisDayIndex;

                SetTime();
                CheckSingleState();
            }

            private void SetTime()
            {
                uint _nowdate = 0;
                if (Instance.FightActivityDic.ContainsKey(_Type))
                {
                    _nowdate = Instance.FightActivityDic[_Type].dayCount;
                }
                else
                {
                    return;
                }

                CSVOperationalActivityRuler.Data oData = CSVOperationalActivityRuler.Instance.GetConfData(_aid);
                DateTime totalActivityDate = GetActivityStartDate(_nowdate, (int)oData.Begining_Date);
                DateTime realActivityDate = new DateTime(totalActivityDate.Year, totalActivityDate.Month, totalActivityDate.Day, 0, 0, 0);
                Instance.activityTimeDic[_aid] = realActivityDate;
                startDate = new DateTime(totalActivityDate.Year, totalActivityDate.Month, totalActivityDate.Day, (int)startTime[0], (int)startTime[1], 0);
                startDate = startDate.AddDays(ThisRoundDate - 1);

                endDate = startDate.AddMinutes(duraTime);
            }

            private DateTime GetActivityStartDate(double _nowDay, int _beginDay)
            {//计算活动开启日期
                DateTime _date = Sys_OperationalActivity.Instance.ServerDateTime();
                _date = _date.AddDays(Math.Abs(_nowDay - 1) * (-1));
                return _date;
            }
            public void CheckSingleState()
            {//_nowdate是活动第几天
                uint _nowdate = 0;
                if (Instance.FightActivityDic.ContainsKey(_Type))
                {
                    _nowdate = Instance.FightActivityDic[_Type].dayCount;
                }
                _isSwitch = Instance.CheckFightTreasureSwitch(_Type);
                if (_nowdate == 0 || !_isSwitch)
                {
                    singleState = 0;//活动结束
                    return;
                }
                DateTime nowDate = Sys_OperationalActivity.Instance.ServerDateTime();
                if (nowDate < startDate)
                {
                    singleState = 2;
                }
                else if (nowDate > endDate)
                {
                    singleState = 0;
                }
                else
                {

                    singleState = 1;
                }

            }

            public bool CheckRealRound(DateTime _dt)
            {
                if (_dt < endDate && _dt > startDate)
                {
                    return true;
                }

                return false;
            }
        }
        public struct FightActivity
        {

            public uint aId;//活动id
            public uint dayCount;//当前天数
            public FightActivity(uint _id, uint _day)
            {
                aId = _id;
                dayCount = _day;
            }
            public void UpdateDayCount(uint _day)
            {
                dayCount = _day;
            }
        }


        private uint lastLoginTime;
        public Dictionary<uint, SingleFightType> fightTreasureDic = new Dictionary<uint, SingleFightType>();//活动id，类型
        public Dictionary<EActivityRulerType, FightActivity> FightActivityDic = new Dictionary<EActivityRulerType, FightActivity>();//活动类型/活动id+天数
        public Dictionary<uint, bool> FightTreasureRedPointDic = new Dictionary<uint, bool>();
        public Vector3 ScrollViewVect;
        Dictionary<uint, DateTime> activityTimeDic = new Dictionary<uint, DateTime>();
        public Dictionary<uint, List<bool>> applyList = new Dictionary<uint, List<bool>>();//活动id,玩家申请信息
        public bool isCheckFightTreasureRedPointDic = false;
        #endregion
        #region 数据处理
        private bool PlayerIsApply(uint _type)
        {
            return (_type == 1) ? true : false;
        }
        private void InitFightTreasureDictionary()
        {
            foreach (var item in FightActivityDic)
            {
                var _id = item.Value.aId;
                SingleFightType sft = new SingleFightType(_id, item.Key);
                sft.InitRoundsList();
                fightTreasureDic[_id] = sft;
            }

        }
        public bool CheckFightActivityEnd(uint _id)
        {

            DateTime nowDate = Sys_OperationalActivity.Instance.ServerDateTime();
            if (activityTimeDic.ContainsKey(_id))
            {
                CSVOperationalActivityRuler.Data oData = CSVOperationalActivityRuler.Instance.GetConfData(_id);
                DateTime _d = activityTimeDic[_id].AddDays(oData.Duration_Day);
                DateTime _end = new DateTime(_d.Year, _d.Month, _d.Day, 0, 0, 0);
                return nowDate > _end;
            }
            return true;

        }
        private void UpdateFightTreasureDictionary(RepeatedField<FightTreasureActivity> _act)
        {
            for (int j = 0; j < _act.Count; j++)
            {
                if (fightTreasureDic.ContainsKey(_act[j].ActivityId))
                {
                    fightTreasureDic[_act[j].ActivityId].UpDateRoundList(_act[j].ActivityRounds);
                }
                else
                {
                    DebugUtil.Log(ELogType.eOperationActivity, "夺宝活动:" + _act[j].ActivityId + "未开启");
                }
            }
        }

        private void InitPlayerApplyDictionary(RepeatedField<FightTreasureApplyInfo> _applyInfo)
        {
            for (int i = 0; i < _applyInfo.Count; i++)
            {
                if (_applyInfo[i].Round == uint.MaxValue)
                {
                    return;
                }
                applyList[_applyInfo[i].ActivityId][(int)_applyInfo[i].Round] = PlayerIsApply(_applyInfo[i].Apply);
                DebugUtil.Log(ELogType.eOperationActivity, "（上线）夺宝活动：" + _applyInfo[i].ActivityId + "轮次" + _applyInfo[i].Round + "是否报名" + PlayerIsApply(_applyInfo[i].Apply));

            }


        }
        private void InitFightActivityDictionary()
        {//更新活动天数(上线、跨天发)
            FightActivityDic.Clear();
            for (int i = 2; i < 4; i++)
            {
                ActivityInfo aInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo((EActivityRulerType)i);
                if (aInfo != null)
                {
                    FightActivityDic[(EActivityRulerType)i] = new FightActivity(aInfo.infoId, aInfo.currDay);
                }
                else
                {
                    if (FightActivityDic.ContainsKey((EActivityRulerType)i))
                    {
                        FightActivityDic.Remove((EActivityRulerType)i);
                    }

                }
            }
            InitFightTreasureDictionary();//活动数据初始化
            if (applyList.Count==0)
            {
                InitApplyList();//报名表初始化
            }
            isCheckFightTreasureRedPointDic = false;
        }
        public void CheckFightActivityDictionary()
        {

            for (int i = 2; i < 4; i++)
            {
                ActivityInfo aInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo((EActivityRulerType)i);
                if (aInfo != null&& activityTimeDic.Count!=0)
                {
                    FightActivityDic[(EActivityRulerType)i] = new FightActivity(aInfo.infoId, aInfo.currDay);
                    CSVOperationalActivityRuler.Data oData = CSVOperationalActivityRuler.Instance.GetConfData(aInfo.infoId);
                    DateTime _d = activityTimeDic[aInfo.infoId].AddDays(oData.Duration_Day);
                    DateTime _end = new DateTime(_d.Year, _d.Month, _d.Day, 0, 0, 0);
                    DateTime _date = Sys_OperationalActivity.Instance.ServerDateTime();
                    if (_date >= _end)
                    {
                        RemoveFightActivityDic(i);
                    }
                }
                else
                {
                    RemoveFightActivityDic(i);
                }
            }
        }

        private void RemoveFightActivityDic(int _i)
        {
            if (FightActivityDic.ContainsKey((EActivityRulerType)_i))
            {
                FightActivityDic.Remove((EActivityRulerType)_i);
            }
        }
        private void InitFightRedPointDictionary()
        {
            FightTreasureRedPointDic.Clear();
            foreach (var item in FightActivityDic)
            {
                var _id = item.Value.aId;
                bool isred = true;
                if (applyList.ContainsKey(_id) && fightTreasureDic.ContainsKey(_id))
                {
                    fightTreasureDic[_id].CheckNowRound();
                    var _type = fightTreasureDic[_id].recordRound;
                    if (applyList[_id].Count != 0&& _type< applyList[_id].Count)
                    {
                        isred = !applyList[_id][(int)_type];
                    }
                }


                FightTreasureRedPointDic.Add(_id, isred);
            }
        }
        public void InitApplyList()
        {
            applyList.Clear();
            foreach (var item in FightActivityDic)
            {
                if (fightTreasureDic.ContainsKey(item.Value.aId))
                {
                    applyList[item.Value.aId] = BuildApplyList(item.Value.aId);
                }
            }
        }

        private List<bool> BuildApplyList(uint _id)
        {
            List<bool> _list = new List<bool>();
            for (int i = 0, len = fightTreasureDic[_id].roundsList.Count; i < len; i++)
            {
                _list.Add(false);
            }

            return _list;
        }
        #endregion
        #region 服务器消息
        /// <summary>
        /// 上线主动发
        /// </summary>
        /// <param name="msg"></param>
        private void OnFightTreasureDataNtf(NetMsg msg)
        {
            CmdFightTreasureDataNtf ntf = NetMsgUtil.Deserialize<CmdFightTreasureDataNtf>(CmdFightTreasureDataNtf.Parser, msg);
            lastLoginTime = ntf.LastLoginTime;
            UpdateFightTreasureDictionary(ntf.Activities);//活动信息接服务器
            InitApplyList();//报名表初始化
            InitPlayerApplyDictionary(ntf.SelfApplyInfo);//报名信息接服务器
            InitFightRedPointDictionary();//活动红点
        }

        // 主动通知该轮活动报名人数更新(废弃)
        //private void OnFightTreasureApplyTotalNumNtf(NetMsg msg)
        //{
        //    CmdFightTreasureApplyTotalNumNtf ntf = NetMsgUtil.Deserialize<CmdFightTreasureApplyTotalNumNtf>(CmdFightTreasureApplyTotalNumNtf.Parser, msg);
        //}
        /// <summary>
        /// 活动报名请求
        /// </summary>
        public void OnFightTreasureApplyActivityReq(uint _id, uint _round)
        {
            CmdFightTreasureApplyActivityReq req = new CmdFightTreasureApplyActivityReq();
            req.ActivityId = _id;
            req.Round = _round;
            NetClient.Instance.SendMessage((ushort)CmdFightTreasure.ApplyActivityReq, req);
        }
        /// <summary>
        /// 活动报名返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnFightTreasureApplyActivityRes(NetMsg msg)
        {
            CmdFightTreasureApplyActivityRes ntf = NetMsgUtil.Deserialize<CmdFightTreasureApplyActivityRes>(CmdFightTreasureApplyActivityRes.Parser, msg);
            uint round = ntf.Round;
            if (applyList.ContainsKey(ntf.ActivityId))
            {

                if (ntf.Round == uint.MaxValue)
                {
                    round = 0;
                }
                applyList[ntf.ActivityId][(int)round] = true;


            }

            if (fightTreasureDic.ContainsKey(ntf.ActivityId))
            {
                int _nowRound = fightTreasureDic[ntf.ActivityId].recordRound;
                if (_nowRound != round)
                {
                    DebugUtil.LogError("夺宝活动：" + ntf.ActivityId + "报名轮次不一致");
                }
                fightTreasureDic[ntf.ActivityId].roundsList[(int)round].UpdateSingleRoundApplyCount(ntf.ApplyNum);//更新报名人数
            }
            eventEmitter.Trigger<uint>(EEvents.UpdateFightTreasureData, round);

        }
        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="msg"></param>
        public void OnFightTreasureDataReq(uint _id)
        {
            CmdFightTreasureDataReq req = new CmdFightTreasureDataReq();
            req.ActivityId = _id;
            if (fightTreasureDic.TryGetValue(_id, out SingleFightType _sft))
            {
                if (_sft.recordRound == 0)
                {
                    DateTime nowtime = Sys_OperationalActivity.Instance.ServerDateTime();
                    _sft.recordRound = _sft.ReturnRealRound(nowtime);
                }
                for (uint i = _sft.startRecord; i <= _sft.recordRound; i++)
                {
                    req.BeforeRounds.Add(i);
                }

            }
            NetClient.Instance.SendMessage((ushort)CmdFightTreasure.DataReq, req);
        }
        /// <summary>
        /// 请求数据返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnFightTreasureDataRes(NetMsg msg)
        {//没有轮次时返回默认值
            CmdFightTreasureDataRes ntf = NetMsgUtil.Deserialize<CmdFightTreasureDataRes>(CmdFightTreasureDataRes.Parser, msg);
            if (fightTreasureDic.ContainsKey(ntf.ActivityId))
            {
                for (int i = 0, len = ntf.ActivityRound.Count; i < len; i++)
                {
                    if (ntf.ActivityRound[i] != null)
                    {
                        fightTreasureDic[ntf.ActivityId].UpdateRoundListSingle(ntf.ActivityRound[i]);
                    }
                }

            }
            else
            {
                DebugUtil.LogError("夺宝活动:" + ntf.ActivityId + "服务器下发不一致");
            }
            eventEmitter.Trigger<uint>(EEvents.UpdateFightTreasureData, 0);
        }
        #endregion
        #region 提供功能
        private void DestoryFightTreasureDate()
        {
            fightTreasureDic.Clear();
            FightActivityDic.Clear();
            FightTreasureRedPointDic.Clear();
            isCheckFightTreasureRedPointDic = false;
        }

        public bool CheckFightTreasureSwitch(EActivityRulerType _type)
        {
            if (_type == EActivityRulerType.OneDollarTreasure)
            {
                return CheckActivitySwitchIsOpen(115) && Sys_FunctionOpen.Instance.IsOpen(50914);

            }
            else
            {
                return CheckActivitySwitchIsOpen(116) && Sys_FunctionOpen.Instance.IsOpen(50915);
            }
        }
        private void OnFightTreasureActivitySwitch()
        {
            foreach (var item in FightActivityDic)
            {
                if (!CheckFightTreasureSwitch(item.Key))
                {
                    CheckApplyDic();
                }

            }
        }
        public void CheckApplyDic()
        {
            if (applyList.Count == 0)
            {
                return;
            }
            foreach (var item in FightActivityDic)
            {
                if (fightTreasureDic.TryGetValue(item.Value.aId, out SingleFightType _sft))
                {
                    DateTime nowtime = Sys_OperationalActivity.Instance.ServerDateTime();
                    int index = _sft.ReturnRealRound(nowtime);
                    applyList[item.Value.aId][index] = false;
                }
            }

        }
        public void CheckFightTreasureRedPointDic()
        {
            if (!isCheckFightTreasureRedPointDic)
            {
                InitFightRedPointDictionary();//活动红点
                isCheckFightTreasureRedPointDic = true;
            }
        }

        public DateTime ServerDateTime()
        {
            uint secondsWithTimeZone = Sys_Time.Instance.GetServerTime();
            DateTime _dateTime = Sys_Time.ConvertToDatetime(secondsWithTimeZone);
            return _dateTime;
        }
        /// <summary>
        /// 检测一元夺宝页签是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckOneDollarIsOpen()
        {
            CheckFightTreasureRedPointDic();
            if (!CheckFightTreasureSwitch(EActivityRulerType.OneDollarTreasure))
            {//紧急开关
                return false;
            }

            if (FightActivityDic.ContainsKey(EActivityRulerType.OneDollarTreasure))
            {
                uint _id = FightActivityDic[EActivityRulerType.OneDollarTreasure].aId;
                if (fightTreasureDic.ContainsKey(_id))
                {
                    CheckFightActivityDictionary();
                }
                else
                {
                    return false;
                }

                return FightActivityDic.ContainsKey(EActivityRulerType.OneDollarTreasure);
            }
            return false;
        }

        /// <summary>
        /// 检测百元夺宝页签是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckHundredDollarIsOpen()
        {
            CheckFightTreasureRedPointDic();
            if (!CheckFightTreasureSwitch(EActivityRulerType.HundredDollarTreasure))
            {//紧急开关
                return false;
            }
            if (FightActivityDic.ContainsKey(EActivityRulerType.HundredDollarTreasure))
            {
                uint _id = FightActivityDic[EActivityRulerType.HundredDollarTreasure].aId;
                if (fightTreasureDic.ContainsKey(_id))
                {
                    CheckFightActivityDictionary();
                }
                else
                {
                    return false;
                }
                return FightActivityDic.ContainsKey(EActivityRulerType.HundredDollarTreasure);
            }
            return false;
        }
        /// <summary>
        /// 一元夺宝红点
        /// </summary>
        /// <returns></returns>
        public bool CheckOneDollarRedPoint()
        {
            if (!CheckOneDollarIsOpen())
            {
                return false;
            }
            if (FightActivityDic.ContainsKey(EActivityRulerType.OneDollarTreasure))
            {
                uint _id = FightActivityDic[EActivityRulerType.OneDollarTreasure].aId;
                fightTreasureDic[_id].CheckNowRound();
                if ((fightTreasureDic[_id].InRound >= 0))
                {
                    if (FightTreasureRedPointDic.ContainsKey(_id))
                    {
                        return FightTreasureRedPointDic[_id];

                    }

                }
            }

            return false;
        }
        /// <summary>
        /// 百元夺宝红点
        /// </summary>
        /// <returns></returns>
        public bool CheckHundredDollarRedPoint()
        {
            if (!CheckHundredDollarIsOpen())
            {
                return false;
            }
            if (FightActivityDic.ContainsKey(EActivityRulerType.HundredDollarTreasure))
            {
                uint _id = FightActivityDic[EActivityRulerType.HundredDollarTreasure].aId;
                fightTreasureDic[_id].CheckNowRound();
                if ((fightTreasureDic[_id].InRound >= 0))
                {
                    if (FightTreasureRedPointDic.ContainsKey(_id))
                    {
                        return FightTreasureRedPointDic[_id];

                    }
                }
            }

            return false;
        }
        /// <summary>
        /// 检测 夺宝活动页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public bool CheckFightTreatureFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "FightTreatureFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置 夺宝活动页签 第一次开启功能的红点
        /// </summary>
        /// <returns></returns>
        public void SetFightTreatureFirstRedPoint()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "FightTreatureFirstRedPoint";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 1);
                eventEmitter.Trigger(EEvents.UpdateFightTreasureData);
            }
        }
        #endregion

    }

    /// <summary> 运营活动系统-充值返利 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        /// <summary>是否领取代金券 </summary>
        public bool isGetCashCoupon;
        /// <summary> 活动有效时间 </summary>
        public long aliveTime;
        /// <summary> 活动有效时间计时器 </summary>
        Timer aliveTimer;
        bool activityIsAlive;
        public Action actionBtnState;
        #endregion
        #region 数据处理
        private void RebateOnLogin()
        {
            activityIsAlive = false;
            isGetCashCoupon = false;
            aliveTime = long.Parse(CSVParam.Instance.GetConfData(1326).str_value);
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 领取公测充值
        /// </summary>
        public void ChargeRebateGetReq()
        {
            if (!CheckRebateisOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(599003018).words);
                return;
            }
            CmdRoleChargeRebateGetReq req = new CmdRoleChargeRebateGetReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.ChargeRebateGetReq, req);
        }
        #endregion
        #region 服务器接收消息
        private void OnChargeRebateNtf(NetMsg msg)
        {
            CmdRoleChargeRebateNtf ntf = NetMsgUtil.Deserialize<CmdRoleChargeRebateNtf>(CmdRoleChargeRebateNtf.Parser, msg);
            isGetCashCoupon = ntf.HasGet;
            SetAliveTimer();
            actionBtnState?.Invoke();
            eventEmitter.Trigger(EEvents.UpdateRebateData);
        }
        #endregion
        #region 提供功能
        private void SetAliveTimer()
        {
            int duration = (int)(TimeManager.ConvertFromZeroTimeZone(aliveTime) - TimeManager.GetServerTime());
            if (duration > 0)
            {
                activityIsAlive = true;
                aliveTimer?.Cancel();
                aliveTimer = Timer.Register(duration, () =>
                {
                    aliveTimer?.Cancel();
                    activityIsAlive = false;
                }, null, false, true);
            }
            else
                activityIsAlive = false;
        }
        /// <summary>
        /// 充值返利功能是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckRebateisOpen()
        {
            return activityIsAlive && CheckActivitySwitchIsOpen(113);
        }
        /// <summary>
        /// 充值返利是否有红点
        /// </summary>
        /// <returns></returns>
        public bool CheckRebateRedPoint()
        {
            if (CheckRebateisOpen() && !isGetCashCoupon)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 清理充值返利数据
        /// </summary>
        public void DestoryRebateData()
        {
            aliveTimer?.Cancel();
            actionBtnState = null;
            activityIsAlive = false;
            isGetCashCoupon = false;
        }
        /// <summary>
        /// 检测是否有可用的代金券，有返回对应的代金券道具id，无返回0
        /// </summary>
        /// <returns></returns>
        public ulong CheckIsHaveCashCoupon(uint chargeId)
        {
            CSVChargeList.Data chargeData = CSVChargeList.Instance.GetConfData(chargeId);
            if (chargeData != null)
            {
                uint itemId = chargeData.CashCoupon;
                long count = Sys_Bag.Instance.GetItemCount(itemId);
                if (count > 0)
                {
                    List<ulong> uids = Sys_Bag.Instance.GetUuidsByItemId(itemId);
                    if (uids.Count > 0)
                    {
                        ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(uids[0]);
                        if (itemData != null)
                        {
                            if (itemData.outTime != 0)
                            {
                                if (itemData.outTime > Sys_Time.Instance.GetServerTime())
                                    return itemId;
                            }
                            else
                                return itemId;
                        }
                    }
                }
            }
            else
                DebugUtil.LogError("CSVChargeList not found id:" + chargeId);
            return 0;
        }
        #endregion
    }

    /// <summary> 运营活动系统-支付宝活动 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        /// <summary>
        /// 支付宝活动GM开关
        /// </summary>
        public bool AlipayGMSwitch = false;
        #endregion
        #region 数据处理
        #endregion
        #region 服务器消息
        #endregion
        #region 提供功能
        /// <summary>检测支付宝活动是否开启</summary>
        public bool CheckAlipayActivityIsOpen()
        {
#if UNITY_IOS || UNITY_TVOS
            return Sys_FunctionOpen.Instance.IsOpen(52101) && CheckActivitySwitchIsOpen(114);
#elif UNITY_EDITOR
            return AlipayGMSwitch && Sys_FunctionOpen.Instance.IsOpen(52101) && CheckActivitySwitchIsOpen(114);
#else
            return false;
#endif
        }

        /// <summary>
        /// 跳转到支付宝页面
        /// </summary>
        public void JunpToAlipayActivityPage()
        {
            CSVParam.Data paramData = CSVParam.Instance.GetConfData(1359);
            string url = paramData.str_value;
            //string url = "https://render.alipay.com/p/c/194xnx1p9xa8/page_u9232b0747e1a4007beeeeef587da755c.html";
            Application.OpenURL(url);
        }
        #endregion
    }

    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {

        #region 数据定义
        public enum Enum_RewardState
        {
            None,
            Enum_CanReceive, //可领取
            Enum_Received, //已领取
        }

        public List<uint> ActivityRewardIdList = new List<uint>();
        public Dictionary<uint, Enum_RewardState> ActivityRewardStateDic = new Dictionary<uint, Enum_RewardState>();
        #endregion
        #region 数据处理
        /// <summary>
        /// 清理数据
        /// </summary>
        public void DestoryActivityRewardData()
        {
            ActivityRewardStateDic?.Clear();
            ActivityRewardIdList?.Clear();
        }
        #endregion
        #region 服务器消息
        private void OnActivityRewardNtf(NetMsg msg)
        {
            ActivityRewardIdList.Clear();
            ActivityRewardStateDic.Clear();
            CmdRoleRewardNtf ntf = NetMsgUtil.Deserialize<CmdRoleRewardNtf>(CmdRoleRewardNtf.Parser, msg);
            for (int i = 0; i < ntf.Awardids.Count; i++)
            {
                ActivityRewardIdList.Add(ntf.Awardids[i]);
                ActivityRewardStateDic.Add(ntf.Awardids[i], (Enum_RewardState)ntf.AwardState[i]);
            }
            eventEmitter.Trigger(EEvents.UpdateActivityRewardData);
        }

        public void ActivityReceiveReq(uint id)
        {
            //if(!CheckActivityRewardRedPoint())
            //{
            //    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(599003018).words);
            //    return;
            //}
            if (!CheckActivityRewardIsOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(599003018).words);
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(1012101);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                CmdRoleRewardTakeReq req = new CmdRoleRewardTakeReq();
                req.Id = id;
                NetClient.Instance.SendMessage((ushort)CmdRole.RewardTakeReq, req);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 运营活动奖励是否有红点
        /// </summary>
        public bool CheckActivityRewardRedPoint()
        {
            foreach (var item in ActivityRewardStateDic.Values)
            {
                if (item == Enum_RewardState.Enum_CanReceive)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 运营活动奖励功能是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckActivityRewardIsOpen()
        {
            if (ActivityRewardStateDic.Count > 0 && CheckActivityRewardRedPoint())
                return CheckActivitySwitchIsOpen(117);
            return false;
        }
        #endregion
    }
    /// <summary> 运营活动系统-累计充值 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        Dictionary<uint, List<ActivityTotalChargeCell>> accruePayDic = new Dictionary<uint, List<ActivityTotalChargeCell>>();
        CSVOperationalActivityRuler.Data curPayRulerData;
        //累计充值金额(货币类型，金额)
        public Dictionary<uint, uint> totalChargeNumDic = new Dictionary<uint, uint>();
        uint chargeStartTime;
        uint chargeEndTime;
        #endregion
        #region 数据处理
        private void InitTotalCharge()
        {
            ActivityInfo info = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.ActivityTotalCharge);
            if (info != null)
            {
                curPayRulerData = info.csvData;
                chargeStartTime = TimeManager.ConvertFromZeroTimeZone(curPayRulerData.Begining_Date);
                chargeEndTime = chargeStartTime + curPayRulerData.Duration_Day * 86400;
                CmdActivityChargeDataReq();
            }
        }
        private void ActivityTotalChargeOnLogin()
        {
            totalChargeNumDic[1] = 0;
            var payDatas = CSVAccruePay.Instance.GetAll();
            for (int i = 0; i < payDatas.Count; i++)
            {
                ActivityTotalChargeCell cell = new ActivityTotalChargeCell();
                cell.id = payDatas[i].id;
                cell.csvData = payDatas[i];
                cell.isGet = false;
                if (accruePayDic.ContainsKey(payDatas[i].Activity_Id))
                    accruePayDic[payDatas[i].Activity_Id].Add(cell);
                else
                    accruePayDic[payDatas[i].Activity_Id] = new List<ActivityTotalChargeCell>() { cell };
            }
            foreach (var item in accruePayDic.Values)
            {
                item.Sort((a,b)=> {
                    return (int)(a.csvData.Sort - b.csvData.Sort);
                });
            }
        }
        private void ActivityTotalChargeOnLogout()
        {
            accruePayDic.Clear();
            curPayRulerData = null;
            totalChargeNumDic.Clear();
        }
        #endregion
        #region nty
        private void OnCurrencyChanged(uint itemId, long value)
        {
            OnChargeCurrencyChanged(itemId);
            OnConsumeCurrencyChanged(itemId);
        }
        private void OnCmdActivityCumulateDataNtf(NetMsg msg)
        {
            CmdActivityCumulateDataNtf ntf = NetMsgUtil.Deserialize<CmdActivityCumulateDataNtf>(CmdActivityCumulateDataNtf.Parser, msg);
            if (ntf != null)
            {
                if (curPayRulerData != null && ntf.ActivityId == curPayRulerData.id)
                    ActivityChargeNtf(ntf);
                else if (curConsumeRulerData != null && ntf.ActivityId == curConsumeRulerData.id)
                    ActivityConsumeNtf(ntf);
            }
        }
        private void OnCmdActivityCumulateRewardRes(NetMsg msg)
        {
            CmdActivityCumulateRewardRes ntf = NetMsgUtil.Deserialize<CmdActivityCumulateRewardRes>(CmdActivityCumulateRewardRes.Parser, msg);
            if (ntf != null)
            {
                if (curPayRulerData != null && ntf.ActivityId == curPayRulerData.id)
                    ActivityChargeRewardRes(ntf.RewardIndex);
                else if (curConsumeRulerData != null && ntf.ActivityId == curConsumeRulerData.id)
                    ActivityConsumeRewardRes(ntf.RewardIndex);
            }
        }
        private void ActivityChargeNtf(CmdActivityCumulateDataNtf ntf)
        {
            if (ntf.Datas != null && ntf.Datas.Count > 0)
            {
                for (int i = 0; i < ntf.Datas.Count; i++)
                {
                    totalChargeNumDic[ntf.Datas[i].Id] = ntf.Datas[i].Value;
                }
            }
            if (ntf.Rewards != null && ntf.Rewards.Count > 0)
            {
                List<ActivityTotalChargeCell> cellList = GetCSVAccruePayDataList(false);
                for (int i = 0; i < ntf.Rewards.Count; i++)
                {
                    ActivityTotalChargeCell cell = cellList.Find(o => o.id == ntf.Rewards[i]);
                    if (cell != null)
                        cell.isGet = true;
                    else
                        DebugUtil.LogError("CSVAccruePay not found id："+ ntf.Rewards[i]);
                }
            }
            eventEmitter.Trigger(EEvents.UpdateActivityTotalCharge);
        }
        private void ActivityChargeRewardRes(uint tid)
        {
            List<ActivityTotalChargeCell> cellList = GetCSVAccruePayDataList(false);
            if (cellList != null && cellList.Count > 0)
            {
                ActivityTotalChargeCell cell = cellList.Find(o => o.id == tid);
                if (cell != null)
                {
                    cell.isGet = true;
                    List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(cell.csvData.reward);
                    UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                    for (int i = 0; i < dropItems.Count; i++)
                    {
                        itemRewardParms.itemIds.Add(dropItems[i].id);
                        itemRewardParms.itemCounts.Add((uint)dropItems[i].count);
                    }
                    UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                }
                else
                    DebugUtil.LogError("CSVAccruePay not found id：" + tid);
            }
            eventEmitter.Trigger(EEvents.UpdateActivityTotalCharge);
        }
        private void OnChargeCurrencyChanged(uint itemId)
        {
            if (totalChargeNumDic.ContainsKey(itemId))
                CmdActivityChargeDataReq();
        }
        #endregion
        #region req
        /// <summary>
        /// 请求累充数据
        /// </summary>
        public void CmdActivityChargeDataReq()
        {
            if (!CheckActivityTotalChargeIsOpen())
                return;
            CmdActivityCumulateDataReq req = new CmdActivityCumulateDataReq();
            req.ActivityId = curPayRulerData.id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityCumulateDataReq, req);
        }
        /// <summary>
        /// 请求领取累充奖励
        /// </summary>
        /// <param name="rewardId"></param>
        public void CmdActivityChargeRewardReq(uint tid)
        {
            CmdActivityCumulateRewardReq req = new CmdActivityCumulateRewardReq();
            req.ActivityId = curPayRulerData.id;
            req.RewardIndex = tid;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityCumulateRewardReq, req);
        }
        #endregion
        #region function
        public bool CheckActivityTotalChargeIsOpen()
        {
            bool isOpen = true;
            if (curPayRulerData == null)
                isOpen = false;
            else
            {
                if (curPayRulerData.Activity_Switch == 0 || GetChargeDiffTime() <= 0)
                    isOpen = false;
            }
            return isOpen && CheckActivitySwitchIsOpen(123);
        }
        public bool CheckActivityTotalChargeRedPoint()
        {
            List<ActivityTotalChargeCell> dataList = GetCSVAccruePayDataList(false);
            if (dataList != null && dataList.Count > 0)
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (!dataList[i].isGet && dataList[i].isCanShow)
                        return true;
                }
            }
            return false;
        }
        public int GetChargeDiffTime()
        {
            return (int)(chargeEndTime - TimeManager.GetServerTime());
        }
        public List<ActivityTotalChargeCell> GetCSVAccruePayDataList(bool isSort = true)
        {
            List<ActivityTotalChargeCell> cellList = new List<ActivityTotalChargeCell>();
            List<ActivityTotalChargeCell> dataList = new List<ActivityTotalChargeCell>();
            if (!CheckActivityTotalChargeIsOpen())
                return null;
            if (accruePayDic.ContainsKey(curPayRulerData.id))
                cellList = accruePayDic[curPayRulerData.id];
            else
                DebugUtil.LogError("CSVAccruePay not found id："+ curPayRulerData.id);
            dataList.Clear();
            if (isSort)
            {
                if (cellList != null && cellList.Count > 0)
                {
                    List<ActivityTotalChargeCell> dataList1 = new List<ActivityTotalChargeCell>();
                    List<ActivityTotalChargeCell> dataList2 = new List<ActivityTotalChargeCell>();
                    List<ActivityTotalChargeCell> dataList3 = new List<ActivityTotalChargeCell>();
                    for (int i = 0; i < cellList.Count; i++)
                    {
                        if (cellList[i].isCanShow)
                        {
                            if (!cellList[i].isGet)
                            {
                                if (!dataList1.Contains(cellList[i]))
                                    dataList1.Add(cellList[i]);
                            }
                            else
                            {
                                if (!dataList3.Contains(cellList[i]))
                                    dataList3.Add(cellList[i]);
                            }
                        }
                        else
                        {
                            if (!dataList2.Contains(cellList[i]))
                                dataList2.Add(cellList[i]);
                        }
                    }
                    dataList.AddRange(dataList1);
                    dataList.AddRange(dataList2);
                    dataList.AddRange(dataList3);
                    dataList1.Clear();
                    dataList2.Clear();
                    dataList3.Clear();
                    return dataList;
                }
            }
            return cellList;
        }
        /// <summary>
        /// 获取充值货币数量
        /// </summary>
        /// <returns></returns>
        public uint GetCurencyChargeNum(uint type = 1)
        {
            if (totalChargeNumDic.ContainsKey(type))
                return totalChargeNumDic[type];
            return 0;
        }
        #endregion
        #region class
        public class ActivityTotalChargeCell
        {
            public uint id;
            public CSVAccruePay.Data csvData;
            public bool isGet;
            public bool isCanShow
            {
                get { return Instance.totalChargeNumDic[1] >= csvData.Recharge; }
            }
        }
        #endregion
    }
    /// <summary> 运营活动系统-累计消费 </summary>
    public partial class Sys_OperationalActivity : SystemModuleBase<Sys_OperationalActivity>
    {
        #region 数据定义
        Dictionary<uint, List<ActivityTotalConsumeCell>> accrueConsumeDic = new Dictionary<uint, List<ActivityTotalConsumeCell>>();
        CSVOperationalActivityRuler.Data curConsumeRulerData;
        //累计消费金额(货币类型，金额)
        public Dictionary<uint, uint> totalConsumeNumDic = new Dictionary<uint, uint>();
        uint consumeStartTime;
        uint consumeEndTime;
        #endregion
        #region 数据处理
        private void InitTotalConsume()
        {
            ActivityInfo info = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.ActivityTotalConsume);
            if (info != null)
            {
                curConsumeRulerData = info.csvData;
                consumeStartTime = TimeManager.ConvertFromZeroTimeZone(curConsumeRulerData.Begining_Date);
                consumeEndTime = consumeStartTime + curConsumeRulerData.Duration_Day * 86400;
                CmdActivityConsumeDataReq();
            }
        }
        private void ActivityTotalConsumeOnLogin()
        {
            var consumeDatas = CSVAccrueConsume.Instance.GetAll();
            for (int i = 0; i < consumeDatas.Count; i++)
            {
                ActivityTotalConsumeCell cell = new ActivityTotalConsumeCell();
                cell.id = consumeDatas[i].id;
                cell.csvData = consumeDatas[i];
                cell.isGet = false;
                totalConsumeNumDic[consumeDatas[i].Consume_Condition[0]] = 0;
                if (accrueConsumeDic.ContainsKey(consumeDatas[i].Activity_Id))
                    accrueConsumeDic[consumeDatas[i].Activity_Id].Add(cell);
                else
                    accrueConsumeDic[consumeDatas[i].Activity_Id] = new List<ActivityTotalConsumeCell>() { cell };
            }
            foreach (var item in accrueConsumeDic.Values)
            {
                item.Sort((a, b) => {
                    return (int)(a.csvData.Sort - b.csvData.Sort);
                });
            }
        }
        private void ActivityTotalConsumeOnLogout()
        {
            accrueConsumeDic.Clear();
            curConsumeRulerData = null;
            totalConsumeNumDic.Clear();
        }
        #endregion
        #region nty
        private void ActivityConsumeNtf(CmdActivityCumulateDataNtf ntf)
        {
            if (ntf.Datas != null && ntf.Datas.Count > 0)
            {
                for (int i = 0; i < ntf.Datas.Count; i++)
                {
                    totalConsumeNumDic[ntf.Datas[i].Id] = ntf.Datas[i].Value;
                }
            }
            if (ntf.Rewards != null && ntf.Rewards.Count > 0)
            {
                List<ActivityTotalConsumeCell> cellList = GetCSVAccrueConsumeDataList(false);
                for (int i = 0; i < ntf.Rewards.Count; i++)
                {
                    ActivityTotalConsumeCell cell = cellList.Find(o => o.id == ntf.Rewards[i]);
                    if (cell != null)
                        cell.isGet = true;
                    else
                        DebugUtil.LogError("CSVAccrueConsume not found id："+ ntf.Rewards[i]);
                }
            }
            eventEmitter.Trigger(EEvents.UpdateActivityTotalConsume);
        }
        private void ActivityConsumeRewardRes(uint tid)
        {
            List<ActivityTotalConsumeCell> cellList = GetCSVAccrueConsumeDataList(false);
            if (cellList != null && cellList.Count > 0)
            {
                ActivityTotalConsumeCell cell = cellList.Find(o => o.id == tid);
                if (cell != null)
                {
                    cell.isGet = true;
                    List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(cell.csvData.reward);
                    UI_Rewards_Result.ItemRewardParms itemRewardParms = new UI_Rewards_Result.ItemRewardParms();
                    for (int i = 0; i < dropItems.Count; i++)
                    {
                        itemRewardParms.itemIds.Add(dropItems[i].id);
                        itemRewardParms.itemCounts.Add((uint)dropItems[i].count);
                    }
                    UIManager.OpenUI(EUIID.UI_Rewards_Result, false, itemRewardParms);
                }
                else
                    DebugUtil.LogError("CSVAccrueConsume not found id：" + tid);
            }
            eventEmitter.Trigger(EEvents.UpdateActivityTotalConsume);
        }
        private void OnConsumeCurrencyChanged(uint itemId)
        {
            if (totalConsumeNumDic.ContainsKey(itemId))
                CmdActivityConsumeDataReq();
        }
        #endregion
        #region req
        public void CmdActivityConsumeDataReq()
        {
            if (!CheckActivityTotalConsumeIsOpen())
                return;
            CmdActivityCumulateDataReq req = new CmdActivityCumulateDataReq();
            req.ActivityId = curConsumeRulerData.id;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityCumulateDataReq, req);
        }
        /// <summary>
        /// 请求领取累消奖励
        /// </summary>
        /// <param name="rewardId"></param>
        public void CmdActivityConsumeRewardReq(uint tid)
        {
            CmdActivityCumulateRewardReq req = new CmdActivityCumulateRewardReq();
            req.ActivityId = curConsumeRulerData.id;
            req.RewardIndex = tid;
            NetClient.Instance.SendMessage((ushort)CmdActivityRuler.CmdActivityCumulateRewardReq, req);
        }
        #endregion
        #region function
        public bool CheckActivityTotalConsumeIsOpen()
        {
            bool isOpen = true;
            if (curConsumeRulerData == null)
                isOpen = false;
            else
            {
                if(curConsumeRulerData.Activity_Switch == 0 || GetConsumeDiffTime() <= 0)
                    isOpen = false;
            }
            return isOpen && CheckActivitySwitchIsOpen(124);
        }
        public bool CheckActivityTotalConsumeRedPoint()
        {
            List<ActivityTotalConsumeCell> dataList = GetCSVAccrueConsumeDataList(false);
            if (dataList != null && dataList.Count > 0)
            {
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (!dataList[i].isGet && dataList[i].isCanShow)
                        return true;
                }
            }
            return false;
        }
        public int GetConsumeDiffTime()
        {
            return (int)(consumeEndTime - TimeManager.GetServerTime());
        }
        public List<ActivityTotalConsumeCell> GetCSVAccrueConsumeDataList(bool isSort = true)
        {
            List<ActivityTotalConsumeCell> cellList = new List<ActivityTotalConsumeCell>();
            List<ActivityTotalConsumeCell> dataList = new List<ActivityTotalConsumeCell>();
            if (!CheckActivityTotalConsumeIsOpen())
                return null;
            if (accrueConsumeDic.ContainsKey(curConsumeRulerData.id))
                cellList = accrueConsumeDic[curConsumeRulerData.id];
            else
                DebugUtil.LogError("CSVAccrueConsume not found id："+ curConsumeRulerData.id);
            dataList.Clear();
            if (isSort)
            {
                if (cellList != null && cellList.Count > 0)
                {
                    List<ActivityTotalConsumeCell> dataList1 = new List<ActivityTotalConsumeCell>();
                    List<ActivityTotalConsumeCell> dataList2 = new List<ActivityTotalConsumeCell>();
                    List<ActivityTotalConsumeCell> dataList3 = new List<ActivityTotalConsumeCell>();
                    for (int i = 0; i < cellList.Count; i++)
                    {
                        if (cellList[i].isCanShow)
                        {
                            if (!cellList[i].isGet)
                            {
                                if (!dataList1.Contains(cellList[i]))
                                    dataList1.Add(cellList[i]);
                            }
                            else
                            {
                                if (!dataList3.Contains(cellList[i]))
                                    dataList3.Add(cellList[i]);
                            }
                        }
                        else
                        {
                            if (!dataList2.Contains(cellList[i]))
                                dataList2.Add(cellList[i]);
                        }
                    }
                    dataList.AddRange(dataList1);
                    dataList.AddRange(dataList2);
                    dataList.AddRange(dataList3);
                    dataList1.Clear();
                    dataList2.Clear();
                    dataList3.Clear();
                    return dataList;
                }
            }
            return cellList;
        }
        /// <summary>
        /// 获取消费货币数量
        /// </summary>
        /// <returns></returns>
        public uint GetCurencyConsumeNum(uint type = 1)
        {
            if (totalConsumeNumDic.ContainsKey(type))
                return totalConsumeNumDic[type];
            return 0;
        }
        #endregion
        #region class
        public class ActivityTotalConsumeCell
        {
            public uint id;
            public CSVAccrueConsume.Data csvData;
            public bool isGet;
            public bool isCanShow
            {
                get
                {
                    if (Instance.totalConsumeNumDic.ContainsKey(csvData.Consume_Condition[0]))
                        return Instance.totalConsumeNumDic[csvData.Consume_Condition[0]] >= csvData.Consume_Condition[1];
                    return false;
                }
            }
        }
        #endregion
    }
}
