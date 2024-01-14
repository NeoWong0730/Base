#if UNITY_STANDALONE_WIN && USE_PCSDK
using KwaiSDK;
using KwaiSDK.DataModel;
using KwaiSDK.Listener;
using Lib.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

public class SDKForPC : SDKBase
{
    KwaiSDKInitListener InitListener = new KwaiSDKInitListener();
    KwaiSDKUserListener UserListener = new KwaiSDKUserListener();
    KwaiSDKPayListener PayListener = new KwaiSDKPayListener();
    KwaiSDKUploadLogListener UploadLogListener = new KwaiSDKUploadLogListener();

    PayModel payModel = new PayModel();

    private KwaiSDKInitListener SDKInitListener()
    {
        InitListener.onError = (int code, string msg) =>
        {
            UI_Box.Create(UseUIBOxType.InitFailure);
        };
        InitListener.onSuccess=(string channelId) =>
        {
            SDKManager.bInit = true;
            SDKManager.sdk.sChannel = channelId;
            HitPointManager.HitPoint("game_sdk_launch");
        };
        return InitListener;
    }

    private KwaiSDKUserListener SDKUserListener()
    {
        UserListener.onSuccess = (AccountModel accountModel) =>{
            DebugUtil.LogFormat(ELogType.eNone, "登陆成功");
            SDKManager.sdk.sdkUid = accountModel.sdkUserId;
            SDKManager.sdk.token = accountModel.sdkToken;
            DebugUtil.LogFormat(ELogType.eNone, "SDKManager.sdk.Uid-------{0}", SDKManager.sdk.sdkUid);
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginSucced);
        };
        UserListener.onLoginCancel = () =>{
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginCancel, "");
        };
        UserListener.onLogout = () =>{
            SDKManager.sdk.sdkUid = string.Empty;
            SDKManager.sdk.token = string.Empty;
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);
        };
        UserListener.onError = (int code, string msg) =>
        {
            DebugUtil.LogFormat(ELogType.eNone, "SDKUserError错误码信息 code:{0}  msg:{1}", code, msg);
        };
        return UserListener;
    }

    private KwaiSDKPayListener SDKPayListener()
    {
        PayListener.finish = (PayResultModel pare) => {
            DebugUtil.LogFormat(ELogType.eNone, "PayListener.finish: {0}|{1}", pare.productName, pare.productID);
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "sdk支付请求完成");
        };
        PayListener.onError = (code, msg) => {
            DebugUtil.LogFormat(ELogType.eNone, "SDKPay错误码信息 code:{0}  msg:{1}", code, msg);
            string result = string.Format("{0}|{1}", code, msg);
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, string.Format("sdk支付请求失败:{0}", result));
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKPayFailure, result);
        };
        return PayListener;
    }

    private KwaiSDKUploadLogListener SDKUploadLogListener()
    {
        UploadLogListener.onResult = (int code, string msg) => {
            DebugUtil.LogFormat(ELogType.eNone, "SDKUploadLog错误码信息 code:{0}  msg:{1}", code, msg);
            SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.onSDKReportErrorToChannel, code);
        };
        return UploadLogListener;
    }

    public override void IsHaveSDK()
    {
        IsHaveSdk = true;
    }

    public override void Init()
    {
        KwaiGameSDK.init(true, SDKInitListener(), "魔力宝贝：旅人");
    }

    public override void Login()
    {
        KwaiGameSDK.login(SDKUserListener());
    }

    public override void Logout()
    {
        KwaiGameSDK.logout();
    }
    public override void Pay(string payjson)
    {
        if (!KwaiGameSDK.isInit()) return;
        var pay = JsonConvert.DeserializeObject<Dictionary<string, string>>(payjson);
        PayModel payModel = new PayModel();
        int ProductNum, price;
        int.TryParse(pay["productNum"], out ProductNum);
        int.TryParse(pay["price"], out price);
        payModel.setProductNum(ProductNum);
        payModel.setPrice(price);
        payModel.setProductId(pay["productId"]);
        payModel.setProductName(pay["productName"]);
        payModel.setProductDesc(pay["productDesc"]);
        payModel.setRoleId(pay["roleId"]);
        payModel.setRoleLevel(pay["roleLevel"]);
        payModel.setRoleName(pay["roleName"]);
        payModel.setServerId(pay["serverId"]);
        payModel.setServerName(pay["serverName"]);
        payModel.setMerchantName(pay["merchantName"]);
        payModel.setVip(pay["vip"]);
        payModel.setPayNotifyUrl(pay["payNotifyUrl"]);
        payModel.setAppID(pay["appId"]);
        payModel.setCoinName(pay["coinName"]);
        payModel.setChannel(pay["channel"]);
        payModel.setUserIp(pay["userIp"]);
        payModel.setOrderId(pay["orderId"]);
        payModel.setExtension(pay["extension"]);
        payModel.setSign(pay["sign"]);
        DebugUtil.LogFormat(ELogType.eNone, "pay:extension{0}", payModel.getExtension());
        KwaiGameSDK.pay(payModel, SDKPayListener());
    }

    public override void ReportHitPoint(string actionName, string recordJson)
    {
        if (!SDKManager.bInit)
            return;
        Dictionary<string, string> record = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(recordJson))
            record = JsonConvert.DeserializeObject<Dictionary<string, string>>(recordJson);
        DebugUtil.LogFormat(ELogType.eNone, "ReportHitPoint*****{0}:{1}", actionName, recordJson);
        KwaiGameSDK.report(actionName, record);
    }

    public override void SDKPrintLog(string logLevel, string msg)
    {
        KwaiGameSDK.printlnLog(logLevel, msg);
    }

    public override void SDKReportErrorToChannel()
    {
        KwaiGameSDK.uploadLog(SDKUploadLogListener());
    }

    //public override string GetHitPointBaseData()
    //{
    //    Appid = KwaiGameSDK.getAppId();
    //    SetChannel(KwaiGameSDK.getChannel());
    //    SetDeviceId(KwaiGameSDK.getDeviceId());
    //    SetAppVersion(KwaiGameSDK.getGameVersion());
    //    SetPublishAppMarket(KwaiGameSDK.getDistributionChannel());
    //    //packagechannel  运营商，app版本
    //    string pointdata = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", Appid, Deviceid, Channel, PublishAppMarket, "5", "PC", GameVersion, "0");
    //    DebugUtil.LogFormat(ELogType.eNone,"HitPointBaseData:{0}" , pointdata);
    //    HitPointBaseData = pointdata;
    //    return HitPointBaseData;
    //}
    public override void UserAgreement()
    {
        if (!KwaiGameSDK.isInit()) return;
    }

    public override void PrivacyPolicy()
    {
        if (!KwaiGameSDK.isInit()) return;
    }
    public override void OpenH5Questionnaire(string urlStr)
    {
        if (!KwaiGameSDK.isInit()) return;
    }
}
#endif
