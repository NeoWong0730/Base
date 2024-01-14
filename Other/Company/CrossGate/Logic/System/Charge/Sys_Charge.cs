using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using Google.Protobuf.Collections;
using LitJson;
using System;
using Lib.Core;

#if !USE_OLDSDK
using com.kwai.game.features;
#endif


namespace Logic
{
    //public class PayModel
    //{
    //    public string channel;
    //    public string userIp;
    //    public string appId;
    //    public string productId;
    //    public string productName;
    //    public string productDesc;
    //    public int productNum;
    //    public string price;
    //    public string serverId;
    //    public string serverName;
    //    public string roleId;
    //    public string roleName;
    //    public string roleLevel;
    //    public string vip;
    //    public string orderId;
    //    public string payNotifyUrl;
    //    public string extension;
    //    public string merchantName;
    //    public string coinName;
    //    public string sign;
    //    public string taskid;
    //}

    public partial class Sys_Charge : SystemModuleBase<Sys_Charge>, ISystemModuleApplicationPause
    {

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents : int
        {
            OnChargedNotify,
        }

        private List<uint> _firstChargeIds;
        
        private bool isChargeCD = false;
        private Timer timerCD;

        public override void Init()
        {
            base.Init();

            _firstChargeIds = new List<uint>(16);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.DataNtf, OnDataNtf, CmdChargeDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.ChargeReq, (ushort)CmdCharge.ChargeRes, OnChargeRes, CmdChargeChargeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCharge.ChargeNtf, OnChargeNtf, CmdChargeChargeNtf.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdCharge.UseCashCouponReq, (ushort)CmdCharge.UseCashCouponRes, OnUseCashCouponRes, CmdChargeUseCashCouponRes.Parser);

            SDKManager.eventEmitter.Handle<string>(SDKManager.ESDKLoginStatus.OnSDKPayFailure, OnSDKPayFailure, true);

#if !USE_OLDSDK
            SDKManager.eventEmitter.Handle<PayResultModel, string>(SDKManager.ESDKLoginStatus.OnSDKPayAutoRepairOrder, OnSDKPayAutoRepairOrder, true);
#endif

        }
        public override void OnLogin()
        {
            base.OnLogin();
        }

        public override void OnLogout()
        {
            timerCD?.Cancel();
            timerCD = null;
            
            base.OnLogout();
        }

        //public override void Dispose()
        //{
        //    base.Dispose();
        //}

#region NetMsg

        private void OnDataNtf(NetMsg msg)
        {
            _firstChargeIds.Clear();

            CmdChargeDataNtf ntf = NetMsgUtil.Deserialize<CmdChargeDataNtf>(CmdChargeDataNtf.Parser, msg);
            _firstChargeIds.AddRange(ntf.FirstChargedIds);
            Sys_OperationalActivity.Instance.ParseChargeDataNtf(ntf);
        }
        /// <summary>
        /// 充值请求
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="repairOrder"></param>
        /// <param name="taskid"></param>
        /// <param name="limitGiftID">条件礼包ID</param>
        /// <param name="presentTargetUid">赠送的好友的UID</param>
        public void OnChargeReq(uint productId, uint limitGiftID = 0,ulong presentFriendUid = 0, EUIID uuid=0)
        {
            if (isChargeCD)
                return;

            isChargeCD = true;
            timerCD?.Cancel();
            timerCD = null;
            timerCD = Timer.Register(1f, () =>
            {
                isChargeCD = false;
            });
            
            if (SDKManager.sdk.IsQRCODE)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12282));
                return;
            }
            //特权卡不支持代金卷充值
            if (uuid != EUIID.UI_SpecialCardPresent)
            {
                ulong itemId = Sys_OperationalActivity.Instance.CheckIsHaveCashCoupon(productId);
                if (itemId != 0)
                {
                    OnChargeUseCashCouponReq(productId, itemId, limitGiftID);
                    return;
                }
            }

//#if UNITY_IOS
//            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "发起预下单pay请求");
//            CSVChargeList.Data chargeData =  CSVChargeList.Instance.GetConfData(productId);
//            PayModel model = new PayModel();
//            model.ProductId = chargeData.Ks_Apple;
//            model.Price = (int)chargeData.RechargeCurrency;
//            model.Extension = string.Format("{0},{1}", limitGiftID, presentFriendUid);
//            model.CoinName = "CNY";
//            string str = JsonMapper.ToJson(model);
//            DebugUtil.LogFormat(ELogType.eNone, string.Format("c#发起预下单请求信息= {0}", str));
            
//            SDKManager.SDKPay(model);
//#else

            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "发起充值请求");
            CmdChargeChargeReq req = new CmdChargeChargeReq();
            req.ChargeId = productId;

            if (limitGiftID > 0)
            {
                req.LimitGiftId = limitGiftID;
            }
            if(presentFriendUid > 0)
            {
                req.FriendId = presentFriendUid;
            }
            ChargeProductIdType idType = ChargeProductIdType.Normal;
            req.ProductIdType = (uint)idType;
#if UNITY_IOS
            req.ChargeChannel = FrameworkTool.ConvertToGoogleByteString("ks");
            req.ProductIdType = (uint)ChargeProductIdType.Apple;
#else
            req.ChargeChannel = FrameworkTool.ConvertToGoogleByteString(SDKManager.GetChannel());
#endif
            NetClient.Instance.SendMessage((ushort)CmdCharge.ChargeReq, req);
//#endif
        }

        private void OnChargeRepairReq(uint productId, uint repairOrder = 0, string taskid = "", uint limitGiftID = 0, ulong presentFriendUid = 0)
        {
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "向服务器请求补单信息");
            CmdChargeChargeReq req = new CmdChargeChargeReq();
            req.ChargeId = productId;
            req.ChargeChannel = FrameworkTool.ConvertToGoogleByteString("ks");
            req.ClientParam = repairOrder;
            req.ClientParam2 = FrameworkTool.ConvertToGoogleByteString(taskid);
            req.ProductIdType = (uint)ChargeProductIdType.Apple;

            if (limitGiftID > 0)
            {
                req.LimitGiftId = limitGiftID;
            }
            if (presentFriendUid > 0)
            {
                req.FriendId = presentFriendUid;
            }

            NetClient.Instance.SendMessage((ushort)CmdCharge.ChargeReq, req);
        }

        private void OnChargeRes(NetMsg msg)
        {
            UIManager.OpenUI(EUIID.UI_Skip);
            
            CmdChargeChargeRes res = NetMsgUtil.Deserialize<CmdChargeChargeRes>(CmdChargeChargeRes.Parser, msg);
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "服务器返回充值签名信息");

            //TODO : 请求sdk支付
            //CSVChargeList.Data data = CSVChargeList.Instance.GetConfData(res.ChargeId);
#if !USE_OLDSDK
            PayModel model = new PayModel();
            model.Channel = res.ChargeChannel.ToStringUtf8();
            model.UserIp = res.UserIp.ToStringUtf8();
            model.AppID = res.AppId.ToStringUtf8();
            model.ProductId = res.ProductId.ToStringUtf8();
            model.ProductName = res.ProductName.ToStringUtf8();
            model.ProductDesc = res.ProductDesc.ToStringUtf8();
            model.Price = (int)res.Money;
            model.ServerId = res.ServerId.ToString();
            model.ServerName = res.ServerName.ToStringUtf8();
            model.RoleId = res.RoleId.ToString();
            model.RoleName = res.RoleName.ToStringUtf8();
            model.RoleLevel = res.Level.ToString();
            model.Vip = res.Vip.ToString();
            model.OrderId = res.ThirdPartyTradeNo.ToStringUtf8();
            model.PayNotifyUrl = res.NotifyUrl.ToStringUtf8();
            model.Extension = res.Extension.ToStringUtf8();
            model.MerchantName = "";
            model.CoinName = "CNY";
            model.Sign = res.Sign.ToStringUtf8();
            model.ProductNum = (int)res.ProductNum;
            model.IsSubscribe = false;


            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "发起sdk支付请求");
            string str = JsonMapper.ToJson(model);
            DebugUtil.LogFormat(ELogType.eNone, string.Format("c# 支付信息= {0}", str));

            //ClientParam =1,需要走自动补单逻辑
            if (res.ClientParam == 1)
            {
                SDKManager.SDKPay_RepairOrder(model, res.ClientParam2.ToStringUtf8());
            }
            else
            {
                SDKManager.SDKPay(model);
            }
#endif


        }

        private void OnChargeNtf(NetMsg msg)
        {
            UIManager.CloseUI(EUIID.UI_Skip);

            CmdChargeChargeNtf ntf = NetMsgUtil.Deserialize<CmdChargeChargeNtf>(CmdChargeChargeNtf.Parser, msg);
            //支付成功
            if (ntf.FirstCharged)
            {
                _firstChargeIds.Add(ntf.ChargeId);
            }

            eventEmitter.Trigger(EEvents.OnChargedNotify, ntf.ChargeId);
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "支付成功到账");
            //UIManager.CloseUI(EUIID.UI_BlockClickTime);
        }
        /// <summary>
        /// 代金券充值
        /// </summary>
        /// <param name="chargeId"></param>
        /// <param name="itemId"></param>
        public void OnChargeUseCashCouponReq(uint chargeId,ulong itemId,uint limitGiftID)
        {
            List<ulong> uidList = Sys_Bag.Instance.GetUuidsByItemId((uint)itemId);
            string content=LanguageHelper.GetTextContent(591000650, CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData((uint)itemId).name_id).words);
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = content;
            PromptBoxParameter.Instance.SetConfirm(true, () => {
                CmdChargeUseCashCouponReq req = new CmdChargeUseCashCouponReq();
                req.ChargeId = chargeId;
                req.ItemUid = uidList[0];
                if (limitGiftID > 0)
                    req.LimitGiftId = limitGiftID;
                NetClient.Instance.SendMessage((ushort)CmdCharge.UseCashCouponReq, req);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            if (UIManager.IsVisibleAndOpen(EUIID.UI_PromptBox))
                UIManager.SendMsg(EUIID.UI_PromptBox, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 代金券充值返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnUseCashCouponRes(NetMsg msg)
        {
            CmdChargeUseCashCouponRes ntf = NetMsgUtil.Deserialize<CmdChargeUseCashCouponRes>(CmdChargeUseCashCouponRes.Parser, msg);
            eventEmitter.Trigger(EEvents.OnChargedNotify, ntf.ChargeId);
        }
#endregion

#region SDK支付回调
        private void OnSDKPayFailure(string result)
        {
            //支付失败 部分由于网络原因需要做二次验证
            //支付失败的errorcode中，3002代表支付失败，3003代表支付取消

            //501 未满8周岁的未成年人不能在游戏内充值
            //502 8 - 16岁的未成年人单次充值金额不能超过50元
            //503 8 - 16岁的未成年人每月累计的充值金额不能超过200元
            //504 16 - 18岁的未成年人单次充值金额不能超过100元
            //505 16 - 18岁的未成年人每月累计的充值金额不能超过400元
            //506 匿名登陆不允许充值  匿名登陆不允许充值提示页面可以提示用户进行账号绑定。
            //507 未成年人单次充值金额不能超过100元
            //508 未成年人每月累计的充值金额不能超过400元
            //UIManager.CloseUI(EUIID.UI_BlockClickTime);
            UIManager.CloseUI(EUIID.UI_Skip);
#if UNITY_IOS
            string msg = LanguageHelper.GetTextContent(12417);//"支付失败";
            if (result.Contains("|"))
            {
                int.TryParse(result.Split('|')[0], out int errorCode);
                msg = string.Format("{0}:{1}", msg, result.Split('|')[1]);
                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, result);
                DebugUtil.LogErrorFormat(string.Format("OnSDKPayFailure: {0} {1}", errorCode, msg));
                if (errorCode >= 501
                    && errorCode <= 508)
                {
                    return;
                }

                if (errorCode == -100012)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11720));
                }
                else
                {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                    PromptBoxParameter.Instance.content = string.Format("{0}({1})", msg, errorCode);
                    PromptBoxParameter.Instance.SetConfirm(true, null);
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(string.Format("{0}:{1}", msg, result));
            }
#else
            if (result.Contains("|"))
            {
                int.TryParse(result.Split('|')[0], out int errorCode);
                string msg = result.Split('|')[1];
                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, result);

                if (errorCode == 3002)
                {
                    DebugUtil.LogErrorFormat(string.Format("OnSDKPayFailure: {0} {1}", errorCode, msg));

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(12416);//"支付失败，请检查网络，或稍后再试！";
                    PromptBoxParameter.Instance.SetConfirm(true, null);
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                }
                else if (errorCode == 3003)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11720));
                    //PromptBoxParameter.Instance.Clear();
                    //PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                    //PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11720);
                    //PromptBoxParameter.Instance.SetConfirm(true, null);
                    //PromptBoxParameter.Instance.SetCancel(true, null);
                    //UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(result);
                }

            }
#endif

        }
#if !USE_OLDSDK
        private void OnSDKPayAutoRepairOrder(PayResultModel payResult, string taskId)
        {
            //真正的下单（向服务器请求该订单信息）taskId：标志sdk生成的订单唯一性（只做转发，对我们没什么用）

            foreach (var data in CSVChargeList.Instance.GetAll())
            {
                if (data.Ks_Apple == payResult.ProductId)
                {
                    if (data.GoodsType == 4 || data.GoodsType == 5)//4:条件礼包 5:特权卡赠送 客户端组装回传SDK
                    {
                        DebugUtil.LogError(string.Format("OnSDKPayAutoRepairOrder:extension null productid={0} GoodsType={1} taskid ={2}", payResult.ProductId, data.GoodsType, taskId));
                        SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, string.Format("SDK要求补单，而补单Extension为空,productid={0} GoodsType={1} taskid ={2}", payResult.ProductId, data.GoodsType, taskId));

                        if (string.IsNullOrEmpty(payResult.Extension))
                        {
                            ResponseByClientStructPayModel(data, payResult, taskId);
                        }
                        else
                        {
                            string[] extensionArr = payResult.Extension.Split(',');
                            uint limitGiftID = 0;
                            uint.TryParse(extensionArr[0], out limitGiftID);
                            ulong presentFriendUid = 0;
                            ulong.TryParse(extensionArr[1], out presentFriendUid);
                            OnChargeRepairReq(data.id, 1, taskId, limitGiftID, presentFriendUid);
                        }
                    }
                    else
                    {
                        OnChargeRepairReq(data.id, 1, taskId, 0, 0);
                    }


                    //if ("Empty".Equals(tempArr[2]))
                    //{
                    //    DebugUtil.LogError(string.Format("OnSDKPayAutoRepairOrder:extension null productid={0} GoodsType={1}", tempArr[0], data.GoodsType));
                    //    SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO,string.Format("SDK要求补单，而补单Extension为空，不出所料此单已丢,productid={0} GoodsType={1}", tempArr[0], data.GoodsType));

                    //    if (data.GoodsType == 4 || data.GoodsType == 5)//4:条件礼包 5:特权卡赠送 客户端组装回传SDK
                    //    {
                    //        ResponseByClientStructPayModel(data, tempArr[1]);
                    //    }
                    //    else
                    //    {
                    //        OnChargeRepairReq(data.id, 1, tempArr[1], 0, 0);
                    //    }
                    //}
                    //else
                    //{
                    //    string[] extensionArr = tempArr[2].Split(','); //result.Split(',');
                    //    uint limitGiftID = 0;
                    //    uint.TryParse(extensionArr[0],out limitGiftID);
                    //    ulong presentFriendUid = 0;
                    //    ulong.TryParse(extensionArr[1], out presentFriendUid);

                    //    OnChargeRepairReq(data.id, 1, tempArr[1], limitGiftID, presentFriendUid);
                    //}
                    //break;
                }
            }
        }

        private void ResponseByClientStructPayModel(CSVChargeList.Data data, PayResultModel resultModel, String taskid)
        {
            //构造客户端已知的参数，回传给sdk
            PayModel model = new PayModel();
            model.Channel = "ks";//SDKManager.GetChannel();
            model.ProductId = data.Ks_Apple;
            model.Price = (int)data.RechargeCurrency;
            model.ServerId = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerId.ToString();
            model.ServerName = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;
            model.RoleId = Sys_Role.Instance.Role.RoleId.ToString();
            model.RoleName = Sys_Role.Instance.Role.Name.ToString();
            model.RoleLevel = Sys_Role.Instance.Role.Level.ToString();
            model.CoinName = "CNY";
            model.ProductName = resultModel.ProductName;
            model.ProductDesc = data.Describe;
            model.Extension = resultModel.Extension;
            model.IsSubscribe = false;


            //这些字段没有，无法组装
            //model.UserIp = res.UserIp.ToStringUtf8();
            //model.AppID = 
            //model.ProductNum = ;
            //model.Vip = res.Vip.ToString();
            //model.OrderId = res.ThirdPartyTradeNo.ToStringUtf8();
            //model.PayNotifyUrl = res.NotifyUrl.ToStringUtf8();
            //model.MerchantName = "";
            //model.Sign = res.Sign.ToStringUtf8();


            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "尽量构造客户端已知的参数，回传给SDK");
            string str = JsonMapper.ToJson(model);
            DebugUtil.LogFormat(ELogType.eNone, string.Format("c# 支付信息= {0}", str));

            SDKManager.SDKPay_RepairOrder(model, taskid);
        }
#endif




#endregion

        public List<uint> GetChargeList(uint type)
        {
            List<uint> list = new List<uint>();
            foreach (var data in CSVChargeList.Instance.GetAll())
            {
                if (data.GoodsType == type)
                {
#if UNITY_IOS
                      if (data.PlatformId == 0u || data.PlatformId == 2)
                        list.Add(data.id);
#elif UNITY_ANDROID
                    if (data.PlatformId == 0u || data.PlatformId == 1)
                        list.Add(data.id);
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN
                    if (data.PlatformId == 0u || data.PlatformId == 3)
                        list.Add(data.id);
#endif
                }
            }

            return list;
        }

        public bool IsCharged(uint chargeId)
        {
            return _firstChargeIds != null && _firstChargeIds.IndexOf(chargeId) >= 0;
        }

        public void OnApplicationPause(bool pause)
        {
#if UNITY_ANDROID
            if (!pause)
                UIManager.CloseUI(EUIID.UI_Skip);
#endif
        }
    }
}

