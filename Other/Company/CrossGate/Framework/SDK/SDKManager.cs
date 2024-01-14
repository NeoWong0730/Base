using AceSdk;
using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GME;
using UnityEngine;
using UnityEngine.Networking;

#if !USE_OLDSDK
using com.kwai.game;
using com.kwai.game.features;
using Framework;
using System.Reflection;
#endif

/// <summary>
/// 实现 MTP 接口 
/// </summary>
public class MyTssInfoReceiver : tss.TssInfoReceiver
{
#if UNITY_IOS
    Dictionary<string, string> plainDict = new Dictionary<string, string>();
    int indexRecord;
#endif
    enum TP2GameStatus
    {
        TSS_INFO_TYPE_DETECT_RESULT_ANO = 1,// 二选一功能  id=1|name=外挂包名|...
        TSS_INFO_TYPE_DETECT_RESULT_APPFORBID = 2,//id=2 应用列表权限检测  只要收到该信息，可确认应用列表权限被禁止或设别上只安装
        TSS_INFO_TYPE_DETECT_RESULT_GP4 = 3,//id=3|reason=检测到内存访问原因|root=手机是否root
        TSS_INFO_TYPE_DETECT_RESULT_EMULATOR = 8,//判断是否是模拟器
        TSS_INFO_TYPE_DETECT_COMMON_INFO = 10,//id=10|jailbreak=|...jailbreak=1表示手机越狱，jailbreak=0表示手机未越狱
    }

    public void onReceive(int tssInfoType, string info)
    {
#if UNITY_ANDROID || UNITY_IOS

        string plain = Tp2Sdk.Tp2DecTssInfo(info);

        DebugUtil.LogFormat(ELogType.eSdk, string.Format("C# Info:{0} {1}", tssInfoType, plain));
        // 此函数不能被阻塞
        if (tssInfoType == tss.TssInfoPublisher.TSS_INFO_TYPE_DETECT_RESULT)
        {
            //DebugUtil.LogFormat(ELogType.eSdk, string.Format("C# Info:{0} {1}", "检测结果", plain));

            // 处理检测结果 如果不关心，可以忽略
            if (plain.Equals("-1"))
                return;

#if UNITY_IOS
            //键值对
            plainDict.Clear();
            if (plain.Contains("id=10"))
            {
                string[] plainStrArr = plain.Split('|');
                for (int i = 0; i < plainStrArr.Length; i++)
                {
                    if (plainStrArr[i].Contains("="))
                    {
                        indexRecord = plainStrArr[i].IndexOf("=");
                        plainDict.Add(plainStrArr[i].Substring(0, indexRecord), plainStrArr[i].Substring(indexRecord + 1));
                    }
                }

                if (plainDict.TryGetValue("id", out string plainId))
                {
                    TP2GameStatus tempEnumStatus = (TP2GameStatus)Enum.Parse(typeof(Tp2GameStatus), plainId);
                    if (tempEnumStatus == TP2GameStatus.TSS_INFO_TYPE_DETECT_COMMON_INFO)
                    {
                        if (plainDict.ContainsKey("jailbreak") || plainDict.ContainsKey("jb_record"))
                        {
                            if (plainDict["jailbreak"] == "1" || plainDict["jb_record"] == "1")
                            {
                                SDKManager.sdk.bIsJailbreak = "1";
                                SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKJailBreakIOS);
                                DebugUtil.LogErrorFormat(string.Format("手机越狱:{0} 信息:{1}", "true", plain));
                            }
                        }
                    }

                    //bool mtpStart = SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.MTPReport);
                    //if (mtpStart)
                    //{
                    //    SDKMonoBehaviour.QueueOnMainThread((param) =>
                    //    {
                    //        HitPointManager.HitPoint("ace_broadcast", param);
                    //    }, this.plainDict);
                    //}
                }
            }
#endif
        }
        else if (tssInfoType == tss.TssInfoPublisher.TSS_INFO_TYPE_HEARTBEAT)
        {
            // 处理心跳，如果不关心，可以忽略
            if (plain.Equals("-1"))
                return;
        }

#endif

    }
}



public static class SDKManager
{
    /// <summary> SDK基类 </summary>
    public static SDKBase sdk;
    /// <summary> bInit：代表同意隐私政策回调之后(android,ios), 在pc上指sdk初始化</summary>
    public static bool bInit;
    public static bool bPaiLianTu;
    public static bool bSwitchAccount;
    //public static bool bBindIphoneActivityIsOpen;
    public static bool bIsSimulator;
    //public static bool bIsQRCode;
    public static int iAccountType;
    private static Transform msdkRoot;

    //private static Dictionary<EThirdSdkType, bool> eThirdSdkTypeDict = new Dictionary<EThirdSdkType, bool>();
    private static MyTssInfoReceiver mTssInfoReceiver;
    private static Action BuglyInitCallBack;
    private static string sDefaultHitPoint = "NULL";

#if !USE_OLDSDK
        public static KwaiGatewayZoneInfo _zoneinfo;
#endif

    public static bool bwaitSDKGetGameZoneComplete;
    public enum SDKReportState
    {
        CREATE = 0,
        LOGIN,
        LEVEL,
        EXIT
    }

    public enum ExitGameState
    {
        None = 0, //默认 由调用者自己支配 
        LogoutBackToLogin, //sdk 强制登出 返回登录界面
        ExitApp //退出app
    }

    public enum EThirdSdkType
    {
        GME,//腾讯语音
        MTP,//加固反外挂
        RNC,//实名认证
        AddQQGroup,//加入qq群聊
        QuestionSurvey,//问卷调查
        CDKey,//兑换码
        ReportPoint,//关闭新手路径追踪打点上报 
        MTPReport,//mtp广播打点上报
        PreWarmAll,//预热活动
        PersonalShop,//个性化的商城
        ConditionGiftPack,//条件触发礼包机制
        BindIphone,//绑定手机送礼包活动
        CloseLeBianPlayLoadInMobile,//默认关闭乐变在wifi情况下的边玩边下的控制
        ScoreInApp,//应用内评分
        fps,
        pkserver,
    }


    public enum ESDKLoginStatus : int
    {
        OnSDKLoginFail,
        OnSDKLoginSucced,
        OnSDKLogout,
        OnSDKExitGame,//sdk登出回调,返回登录界面
        onSDKExitReportExtraData,
        onSDKReportErrorToChannel,
        OnSDKPayFailure,
        OnSDKPayAutoRepairOrder,
        OnSDKAppealStatus, //交易申诉状态通知
        OnSDKBindIphoneStatus,
        OnSDKHintMsg,
        OnSDKJailBreakIOS,
    }

    public enum ESDKLogLevel
    {
        VERSION,
        DEBUG,
        INFO,
        WARN,
        ERROR,
        ASSERT
    }

    public readonly static EventEmitter<ESDKLoginStatus> eventEmitter = new EventEmitter<ESDKLoginStatus>();


#if UNITY_ANDROID
    public readonly static  string officialChannel = "ks";
#elif UNITY_IOS
    public readonly static string officialChannel = "appstore";
#else
    public readonly static  string officialChannel = "ks";
#endif

    //public static Dictionary<string, int> channelFlag = new Dictionary<string, int>()
    //{
    //    { "ks",1}
    //};
    //public static Dictionary<string, int> channelFlag = new Dictionary<string, int>()
    //{// int 拆分：0 没有 1有登出
    //    { "ks", 1},{"4399",1},{"bili",1},{"uc",1},{"huawei",1},{"ysdk",1},{"meizu",1},{"baidu",1},{"lizhi",1},{ "mumu",1},
    //    { "samsung",1},{ "360",1},{ "htc",1},{ "seven",1},{ "shark",1},{ "coolpad",1},{ "dangle",1},{ "yiwan",1},
    //    { "guopan",1},{ "douyu",1},{ "huya",1},{ "imgo",1},{ "wanka",1},{ "kuaikan",1}
    //};



    public static void Init(Action buglyInitCallBack)
    {
        msdkRoot = new GameObject("SdkMgr").transform;
        if (msdkRoot != null)
        {
            GameObject.DontDestroyOnLoad(msdkRoot);

#if UNITY_ANDROID || UNITY_IPHONE
            msdkRoot.gameObject.AddComponent<SDKMonoBehaviour>();
            BuglyInitCallBack = buglyInitCallBack;
#endif

#if UNITY_EDITOR && SKIP_SDK_Login
            msdkRoot.gameObject.AddComponent<SkipSDKLogin>();
#endif
        }

#if UNITY_EDITOR && !USE_PCSDK
        sdk = new SDKBase();
#elif UNITY_ANDROID
        sdk = new SDKForAndroid();
#elif UNITY_IOS
        sdk = new SDKForIOS();
#elif UNITY_STANDALONE_WIN && USE_PCSDK
        sdk = new SDKForPC();
#else
        sdk = new SDKBase();
#endif
        OnEnter();
    }

    public static void OnEnter()
    {
        SDKInit();

#if UNITY_ANDROID || UNITY_IPHONE
        //if (GetHaveSDK())
        //    SDKManager.SDKBuglyInit();
#endif
    }

    public static void OnExit()
    {
        //退出游戏
        UninitGMESDK();

        UninitMTPSDK();
    }


#if !USE_OLDSDK
    private static void InitSDKDelegate()
    {
        AllinSDK.Account.ForceLogout = OnForceLogout;
        AllinSDK.Account.OnDidAddictInfoUpdate = OnDidAddictInfoUpdate;

        //AllinSDK.Account.OnCertificationCompletion = OnCertificationCompletion;
        //AllinSDK.Account.OnCertQueryResult = OnRealNameQueryResult;
        AllinSDK.Account.OnCertSuccess = OnRealNameSuccess;
        AllinSDK.Account.OnCertError = OnCertError;
        //AllinSDK.Account.OnRegisterCustomCertUIListener = OnRegisterCustomCertUIListener;

        // 支付成功回调
        AllinSDK.Pay.OnPaySuccess = OnPaySuccess;
        // 支付失败回调
        AllinSDK.Pay.OnPayFailed = OnPayFailed;


        //todo 缺少切换账号成功的回调


#if UNITY_IOS
        AllinSDK.Pay.OnRequestPaymentDetail = OnRequestPaymentDetail;
        AllinSDK.Pay.OnSyncIncompleteOrdersProgress = SyncIncompleteOrdersOnProgress;
        AllinSDK.Pay.OnSyncIncompleteOrdersCompletion = SyncIncompleteOrdersOnCompletion;
#endif
    }


    /// <summary>
    /// 需要回传预下单信息给支付系统
    /// </summary>
    /// <param name="resultModel">商品信息</param>
    /// <param name="taskId">回传给预下单系统时需要</param>
    public static void OnRequestPaymentDetail(PayResultModel resultModel, string taskId)
    {
        DebugUtil.LogFormat(ELogType.eNone, string.Format("OnRequestPaymentDetail productId:{0},money:{1},currencyType:{2},extension:{3}",
            resultModel.ProductId, resultModel.Money, resultModel.CurrencyType, resultModel.Extension));

        DebugUtil.LogFormat(ELogType.eNone, "---------->taskId:" + taskId);

        //PayController.responsePaymentDetail(resultModel, taskId);

        //回传预下单信息,从根据resultModel的ProducId从server端获取具体参数构造PayModel返回给SDK

        SDKManager.eventEmitter.Trigger<PayResultModel, string>(SDKManager.ESDKLoginStatus.OnSDKPayAutoRepairOrder, resultModel, taskId);

        //PayModel model = new PayModel();
        //AllinSDK.Pay.ResponsePaymentDetail(taskId, model);
    }

    /// <summary>
    /// 自动补单完成
    /// </summary>
    /// <param name="error"></param>
    public static void SyncIncompleteOrdersOnCompletion(Error error)
    {
        //LogView.AddLog($"SyncIncompleteOrdersOnCompletion 补单完成 code:{error.code}, msg:{error.message}");
    }


    /// <summary>
    /// 自动补单进度回调
    /// </summary>
    /// <param name="error"></param>
    /// <param name="payModel"></param>
    public static void SyncIncompleteOrdersOnProgress(Error error, PayResultModel payModel)
    {
        string payModelInfo = MiniJSON.jsonEncode(payModel.toHashTable());
        //LogView.AddLog($"SyncIncompleteOrdersOnCompletion 补单正在进行 code:{error.code}, payModel:{payModelInfo}");
    }




    /// <summary>
    /// 支付成功回调
    /// </summary>
    /// <param name="resultModel"></param>
    private static void OnPaySuccess(PayResultModel resultModel)
    {
        DebugUtil.LogFormat(ELogType.eNone, string.Format("OnPaySuccess productId:{0},productName:{1},money:{2},currencyType:{3},extension:{4}",
            resultModel.ProductId, resultModel.ProductName, resultModel.Money, resultModel.CurrencyType, resultModel.Extension));

        DebugUtil.LogFormat(ELogType.eNone, "支付成功");
        SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "sdk支付请求完成");
    }

    /// <summary>
    /// 支付失败回调
    /// </summary>
    /// <param name="error"></param>
    private static void OnPayFailed(Error error)
    {
        DebugUtil.LogError(string.Format("OnPayFailed errorCode:{0},errorMsg:{1}", error.code, error.message));

        string strMsg = string.Format("{0}|{1}", error.code, error.message);
        SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, string.Format("sdk支付请求失败:{0}", strMsg));
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKPayFailure, strMsg);
    }


    private static void OnRealNameSuccess()
    {
        SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, "实名认证成功");
        DebugUtil.LogFormat(ELogType.eNone, "OnRealNameSuccess!");
    }

    private static void OnCertError(Error error)
    {
        DebugUtil.LogError(string.Format("OnCertError code:{0},message:{1}!", error.code, error.message));

        SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, string.Format("实名认证失败({0})", error.code));

        //如果实名失败会通过OnCertError回调回来，需要游戏主动退出 暂不管会不会走ForceLogout
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);
        SDKManager.SDKLogout();
    }

    private static void OnForceLogout()
    {
        DebugUtil.LogFormat(ELogType.eNone, "receive OnForceLogout callback");

        //退出游戏 Todo后面优化
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);
        SDKManager.SDKLogout();
    }

    private static void OnDidAddictInfoUpdate(AddictInfo addictInfo)
    {
        DebugUtil.LogFormat(ELogType.eNone,
            $"OnDidAddictInfoUpdate status:{addictInfo.Status} remindStatus:{addictInfo.RemindStatus} remindWay:{addictInfo.RemindWay} message:{addictInfo.Message} interval:{addictInfo.Interval} gameTimeLeft:{addictInfo.GameTimeLeft}");
    }


    /// <summary>
    /// 隐私合规同意回调
    /// </summary>
    private static void OnPolicyAgreeCallback()
    {
        Debug.Log("========================>隐私合规同意");

        BuglyInitCallBack?.Invoke();
        Debug.LogError("Bugly 用于测试");

        SDKManager.bInit = true;
        sdk.sMobileOperators = AllinSDK.DeviceInfo.GetMobileOperators();
        sdk.sDeviceid = AllinSDK.Launcher.GetDeviceId();
        Debug.Log($"MobileOperator: {sdk.sMobileOperators},Deviceid:{sdk.sDeviceid}");

        SDKLeBianPrivacyChecked();

        //通知权限检查
        if (!AllinSDK.Push.IsNotificationEnable())
        {
            UI_Box.Create(UseUIBOxType.PushNotify);
            DebugUtil.LogFormat(ELogType.eNone, string.Format($"PushAgent.IsNotificationEnable not enable"));
        }
    }
#endif







#region SDK功能
    /// <summary>
    /// 判定是否有SDK
    /// </summary>
    private static void SDKISHaveSDK()
    {
        Debug.Log("SDKISHaveSDK");
        sdk.IsHaveSDK();
    }


    /// <summary>
    /// SDK初始化
    /// </summary>
    private static void SDKInit()
    {
        //判断是否有SDK
        SDKISHaveSDK();

        DebugUtil.LogFormat(ELogType.eNone, "SDKInit");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

#if UNITY_ANDROID
        //1.设置拒绝SDK申请的权限后一定时间（建议48小时）不再重复弹出
        AllinSDK.Permission.SetPermissionLimitTime(48 * 60 * 60 * 1000);
#endif

        //2.1 设置隐私合规监听回调
        AllinSDK.Permission.OnPolicyAgreeCallback = OnPolicyAgreeCallback;
        //2.2注册隐私合规监听
        AllinSDK.Permission.SetPolicyProxyListener();


        //3.调用初始化
        var initConfig = new InitConfig();
        initConfig.GameName = "CrossGate";
        initConfig.IsAllInApkUpgradeCustomUI = false;
        AllinSDK.Init(initConfig, true, data =>
        {
            // TODO 初始化之后需要做的操作
            DebugUtil.LogFormat(ELogType.eNone, "========================>初始化成功:" + data.ToString());

            sdk.sdkInit = true;
            sdk.sAppid = data.AppId;
            sdk.sChannel = data.Channel;
            sdk.sMarketChannel = data.MarketChannel;
            DebugUtil.LogFormat(ELogType.eNone, string.Format("sAppid：{0}，sChannel：{1}，sMarketChannel：{2}",
                data.AppId, data.Channel, sdk.sMarketChannel));

            HitPointManager.HitPoint("game_sdk_launch");
        }, error =>
        {
            sdk.sdkInit = false;
            UI_Box.Create(UseUIBOxType.InitFailure);
            DebugUtil.LogError("Init fail:" + error.ToString());
        });

        InitSDKDelegate();

#else
        sdk.Init();
#endif
    }


    /// <summary>
    /// SDK登录
    /// </summary>
    public static void SDKLogin()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKLogin");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

#if !USE_SDK_QRCODE

        if (sdk.IsQRCODE)
        {
            Debug.Log("KuaiShou QRCODE Login");
            AllinSDK.Account.Login(LoginTypeOption.QRCODE, delegate (AccountInfo accountInfo)
            {
                DebugUtil.LogFormat(ELogType.eNone, "SDKLogin callback accountInfo:" + accountInfo.ToString());

                SDKManager.sdk.sdkUid = accountInfo.GameUserId;
                SDKManager.sdk.token = accountInfo.GameUserToken;

                SDKManager.MTPUserLogin(0, SDKManager.sdk.sdkUid, "");

                //请求Web服务器列表
                SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginSucced);
                HitPointManager.HitPoint("game_sdk_login");
                SDKGetGameUser();
            },
            delegate (Error error)
            {
                string errorMsg = string.Format("{0}|{1}", error.code, error.message);
                SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginFail, errorMsg);
                DebugUtil.LogError(string.Format("SDKLogin error callback:{0}({1})", error.message, error.code));
            });
        }
        else
        {
            Debug.Log("KuaiShou Common Login");
            AllinSDK.Account.Login(LoginTypeOption.KS | LoginTypeOption.QQ | LoginTypeOption.WEIXIN | LoginTypeOption.VISITOR | LoginTypeOption.PHONE | LoginTypeOption.APPLE,
             delegate (AccountInfo accountInfo)
             {
                 DebugUtil.LogFormat(ELogType.eNone, "SDKLogin callback accountInfo:" + accountInfo.ToString());

                 SDKManager.sdk.sdkUid = accountInfo.GameUserId;
                 SDKManager.sdk.token = accountInfo.GameUserToken;

                 SDKManager.MTPUserLogin(0, SDKManager.sdk.sdkUid, "");

                    //请求Web服务器列表
                    SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginSucced);
                 HitPointManager.HitPoint("game_sdk_login");

                 SDKGetGameUser();
             },
             delegate (Error error)
             {
                 string errorMsg = string.Format("{0}|{1}", error.code, error.message);
                 SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginFail, errorMsg);
                 DebugUtil.LogError(string.Format("SDKLogin error callback:{0}({1})", error.message, error.code));
             });
        }

#endif

#else
        HitPointManager.HitPoint("game_sdk_login");
        if (sdk == null) return;
        sdk.Login();
#endif

    }


    /// <summary>
    /// SDK登出
    /// </summary>
    public static void SDKLogout()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKLogout");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

        AllinSDK.Account.Logout(delegate
        {
            DebugUtil.LogFormat(ELogType.eNone, "SDKLogout success");

            SDKManager.sdk.sdkUid = null;
            SDKManager.sdk.token = null;
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLogout, "NULL");
        },
        delegate (Error error)
        {
            DebugUtil.LogError(string.Format("SDKLogout error callback:{0}", error));

            SDKManager.sdk.sdkUid = null;
            SDKManager.sdk.token = null;
            SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLogout, string.Format("账号登出失败:({0})", error.code));
        });
#else
      if (sdk == null) return;
        sdk.Logout();
#endif


#if UNITY_STANDALONE_WIN
        ACESDKLoginOff();
#endif
    }

#if !USE_OLDSDK
    /// <summary>
    /// SDK付费
    /// </summary>
    public static void SDKPay(PayModel payModel)
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKPay");
        if (!sdk.IsHaveSdk)
        {
            SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, "支付需要带sdk的包进行测试");
            return;
        }

#if !USE_OLDSDK
        AllinSDK.Pay.Pay(payModel);
#else
        if (sdk == null) return;
        sdk.Pay(payjson);
#endif
    }
#endif


    /// <summary>
    /// IOS设置支付代理，在角色登陆后调用 --C#版sdk不在使用
    /// </summary>
    public static void SDKPay_AutoRepairRecoder_RegistDelete()
    {
#if !USE_OLDSDK

#else
        if (sdk == null) return;
        sdk.PayAutoRepairRecoder_RegistDelete();
#endif
    }

#if !USE_OLDSDK
    /// <summary>
    /// SDK 自动补单信息回掉
    /// </summary>
    /// <param name="payjson"></param>
    public static void SDKPay_RepairOrder(PayModel payModel, string taskId)//string payjson)
    {
#if !USE_OLDSDK

        AllinSDK.Pay.ResponsePaymentDetail(taskId, payModel);
#else
        if (sdk == null) return;
        sdk.PayRepairOrder(payjson);
#endif
    }
#endif


    /// <summary>
    /// SDK退出游戏
    /// </summary>
    public static void SDKExitGame()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKExitGame");
        if (!sdk.IsHaveSdk)
            return;

        sdk.ExitGame();
    }


    /// <summary>
    /// SDK用户中心
    /// </summary>
    public static void SDKOpenUserCenter()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKOpenUserCenter");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK
        AllinSDK.Account.ShowAccountCenter();
#else
        sdk.UserCenter();
#endif
    }


    /// <summary>
    /// 隐私政策
    /// </summary>
    public static void SDKPrivacyPolicy()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKPrivacyPolicy");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK
        AllinSDK.Permission.ShowPrivacyProtocol(Protocol.PRIVACY);
#else
        sdk.PrivacyPolicy();
#endif
    }


    /// <summary>
    /// 用户协议
    /// </summary>
    public static void SDKUserAgreement()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKUserAgreement");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK
        AllinSDK.Permission.ShowPrivacyProtocol(Protocol.POLICY);
#else
        sdk.UserAgreement();
#endif
    }


    /// <summary>
    /// 调查问卷
    /// </summary>
    /// <param name="urlStr"></param>
    public static void SDKOpenH5Questionnaire(string urlStr)
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKOpenH5Questionnaire");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK
        AllInWebViewConfig config = new AllInWebViewConfig();
        // 设置在左上角，屏幕占比宽和高都是50%
        var webViewRect = new WebViewRect();
        webViewRect.LeftScale = 0;
        webViewRect.TopScale = 0;
        webViewRect.WidthScale = 100;
        webViewRect.HeightScale = 100;
        config.Rect = webViewRect;
        config.WindowStyle = WebWindowStyle.FullScreen; // 设置全屏
        config.Orientation = WebViewOrientation.Auto;
        config.HideToolBar = false;
        config.ShareValue = (int)WebViewShare.None;
        //(WebViewShare.Kwai | WebViewShare.Wechat | WebViewShare.QQ | WebViewShare.QZone | WebViewShare.WeiBo | WebViewShare.WXFriend);
        config.HideProgressBar = false;
        config.HideTitle = false;
        AllinSDK.WebView.InitWithConfig(config);
        AllinSDK.WebView.OpenUrl(urlStr);
#else
        sdk.OpenH5Questionnaire(urlStr);
#endif
    }


    /// <summary>
    /// 扫描二维码
    /// </summary>
    public static void SDKScanQRCode()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKScanQRCode");
        if (!sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

        AllinSDK.QRScan.ScanQRCode(delegate (int result, string message)
        {
            string log = string.Format("扫描结果: {0}, msg:{1}", Convert.ToString(result), message);
            DebugUtil.LogFormat(ELogType.eNone, log);

        }, delegate (Error error)
        {
            DebugUtil.LogFormat(ELogType.eNone, "扫描失败:" + error.message);
        });
#else
        sdk.SDKScanQRCode();
#endif
    }


    /// <summary>
    /// 一键加入官方群
    /// </summary>
    /// <returns>result:-1 未安装QQ应用 1快跳能跳 0快跳不能跳</returns>
    public static int SDKjoinQQGroup()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKjoinQQGroup");
        if (!sdk.IsHaveSdk)
            return 0;

#if !USE_OLDSDK

        int result = 0;
        if (!sdk.QQIsInstalled())
        {
            return -1;
        }

        String jumpKey = "qq1";
        AllinSDK.QuickJump.CheckQuickJumpEnable(jumpKey, delegate (bool quickJumpEnable)
        {
            if (quickJumpEnable)
            {
                result = 1;
                AllinSDK.QuickJump.QuickJump(jumpKey, delegate (int code, string msg)
                {
                    DebugUtil.LogFormat(ELogType.eNone, $"qq1 QuickJump code = {code} msg = {msg}");

                }, delegate (Error error)
                {
                    DebugUtil.LogError(string.Format("qq1 QuickJump error callback:{0}", error.ToString()));
                });


                DebugUtil.LogFormat(ELogType.eNone, "qq1 QuickJump enable");
            }
            else
            {
                result = 0;
                DebugUtil.LogError("qq1 QuickJump error disenable");
            }
        });

        return result;
#else
        if (sdk == null) return -1;
        int result = -1;
        result = sdk.JoinQQGroup();
        DebugUtil.LogFormat(ELogType.eSdk, string.Format("join QQ Group :{0}", result));
        return result;
#endif
    }


    /// <summary>
    /// 进入战斗后，需要设置此变量，防止防沉迷弹框的弹出
    /// </summary>
    /// <param name="isFighting"></param>
    public static void SDK_SetGameFightStatus(bool isFighting)
    {
#if !USE_OLDSDK


#else
        sdk.SetGameFightStatus(isFighting);
        if (!isFighting)
            SDKUpdateAntiAddictStatus();
#endif


    }
    /// <summary>
    /// 同步 防沉迷的状态
    /// </summary>
    public static void SDKUpdateAntiAddictStatus()
    {
#if !USE_OLDSDK


#else
     sdk.SyncAntiAddictStatus();
#endif
    }


    /// <summary>
    /// 索要打点基础信息
    /// </summary>
    public static string SDKGetHitPointBaseInfo()
    {
        //todo yd
        string baseData = string.Empty;
#if !USE_OLDSDK
        baseData = "123";
        return baseData;
#else
        if (GetHaveSDK())
        {
            //baseData = sdk.GetHitPointBaseData();
           // DebugUtil.LogFormat(ELogType.eNone, string.Format("SDKGetHitPointBaseInfo:{0}", baseData));
        }
        return baseData;
#endif
    }


    /// <summary>
    /// 上报打点信息
    /// </summary>
    /// <param name="actionName">上报事件名称</param>
    /// <param name="jsonContent">上报事件内容</param>
    public static void HitPoint(string actionName, object obj)
    {
        //DebugUtil.LogFormat(ELogType.eNone, "HitPoint");
        if (!sdk.IsHaveSdk) return;
#if !USE_OLDSDK
        Hashtable hashtable = Object2Hashtable(obj);
        AllinSDK.Report.Report(actionName, hashtable);
#else
        string strContent = sDefaultHitPoint;
        if (obj != null)
            strContent = LitJson.JsonMapper.ToJson(obj);
        sdk.ReportHitPoint(actionName, strContent);
#endif

        //if (actionName.Equals("guide"))
        //Debug.LogError(string.Format("上报打点信息:{0}, {1}",actionName, strContent));
    }
    /// <summary>
    /// 上报打点信息(PC端)
    /// </summary>
    /// <param name="actionName">上报事件名称</param>
    /// <param name="jsonContent">上报事件内容</param>
    public static void HitPointPC(string actionName, object obj)
    {
#if UNITY_STANDALONE_WIN && USE_PCSDK
        if (sdk == null) return;
        string strContent = "";
        if (obj != null)
            strContent = LitJson.JsonMapper.ToJson(obj);
        sdk.ReportHitPoint(actionName, strContent);
#endif
    }


    /// <summary>
    /// 通过sdk打印重要的信息（日志留存时间、及最大数值如无要求无需设置。默认为36M存储空间、7天有效时长）
    /// </summary>
    public static void SDKPrintLog(SDKManager.ESDKLogLevel logLevel, string msg)
    {
        if (!SDKManager.sdk.IsHaveSdk)
            return;
#if !USE_OLDSDK

        //日志等级 0:Debug 1:Info 2:Warn 3:Error
        int _level;
        if (logLevel == ESDKLogLevel.ERROR)
            _level = 3;
        else if (logLevel == ESDKLogLevel.INFO)
            _level = 1;
        else
            _level = 0;

        AllinSDK.Launcher.Log(_level, msg);

#else

#if UNITY_STANDALONE_WIN && USE_PCSDK
        PCSDKPriintLog(logLevel, msg);
#else
        sdk.SDKPrintLog(logLevel, msg);
#endif

#endif


    }
    private static void PCSDKPriintLog(ESDKLogLevel logLevel, string msg)
    {
        string logType = "";
        switch (logLevel)
        {
            case ESDKLogLevel.INFO:
            case ESDKLogLevel.DEBUG:
                logType = "Debug";
                break;
            case ESDKLogLevel.WARN:
                logType = "Warn";
                break;
            case ESDKLogLevel.ERROR:
                logType = "Error";
                break;
            default:
                break;
        }
        if (!string.IsNullOrEmpty(logType))
            sdk.SDKPrintLog(logType, msg);
        else
            sdk.SDKPrintLog(logLevel.ToString(), msg);
    }

#if !USE_OLDSDK
    /// <summary>
    /// 上报玩家角色信息  (ios不在上报)
    /// </summary>
    /// <param name="reportJson"></param>
    public static void SDKUpdateRoleData(RoleData roleData)
    {
#if !USE_OLDSDK
        if (SDKManager.sdk.IsHaveSdk)
            AllinSDK.Account.UpdateRoleData(roleData);
#else

#if UNITY_ANDROID
            if (GetHaveSDK())
                sdk.SDKReportExtraData(reportJson);
#endif

#endif
    }

    /// <summary>
    /// 设置公共参数  公共参数仅需要设置一次
    /// </summary>
    public static void SDKSetCommonReportParam(CommonReport commonReport)
    {
        if (SDKManager.sdk.IsHaveSdk)
            AllinSDK.Report.SetCommonReportParam(commonReport);
    }
#endif


    /// <summary>
    /// 客服
    /// </summary>
    public static void SDKCustomService()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKCustomService");
        if (!SDKManager.sdk.IsHaveSdk)
            return;
#if !USE_OLDSDK
        if (AllinSDK.CustomService.IsSupportCustomServicePage())
        {
            AllinSDK.CustomService.ShowCustomServicePage();
            DebugUtil.LogFormat(ELogType.eNone, "CustomService.OnClickCustomService");
        }
        else
        {
            DebugUtil.LogError("IsSupportCustomServicePage == false");
        }
#else
        sdk.SDKCustomService();
#endif

    }


    /// <summary>
    /// 程序日志以及SDK日志上报给客服。一般渠道包使用该功能，日志可以从业受后台获取
    /// </summary>
    /// <param name="flagName">标记信息</param>
    public static void SDKReportErrorToChannel(string flagName)
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKReportErrorToChannel");
        if (!SDKManager.sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

        AllinSDK.Launcher.UploadLog(flagName, delegate (int i, string s)
        {
            DebugUtil.LogFormat(ELogType.eNone, "LauncherAgent.updatelog tag: {0} callback code:{1} msg:{2}", flagName, i, s);
            SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.onSDKReportErrorToChannel, 0);

        }, delegate (Error error)
        {
            DebugUtil.LogError(string.Format("AccountAgent.updatelog error callback:{0}", error));
            SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.onSDKReportErrorToChannel, -1);
        });

#else

#if UNITY_ANDROID || UNITY_IPHONE
         sdk.SDKReportErrorToChannel(flagName);
#elif UNITY_STANDALONE_WIN && USE_PCSDK
         sdk.SDKReportErrorToChannel();
#endif

#endif
    }


    /// <summary>
    /// 拍连图
    /// </summary>
    public static void SDKPaiLianTu()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKPaiLianTu");
        if (!SDKManager.sdk.IsHaveSdk)
            return;
#if !USE_OLDSDK

        if (bPaiLianTu)
        {
            bPaiLianTu = false;
            String jumpKey = "pic1";
#if UNITY_IPHONE
            jumpKey = "pic2";
#endif
            AllinSDK.QuickJump.CheckQuickJumpEnable(jumpKey, delegate (bool quickJumpEnable)
            {
                if (quickJumpEnable)
                {
                    AllinSDK.QuickJump.QuickJump(jumpKey, delegate (int code, string msg)
                    {
                        DebugUtil.LogFormat(ELogType.eNone, $"jumpKey = {jumpKey} QuickJump code = {code} msg = {msg}");

                    }, delegate (Error error)
                    {
                        DebugUtil.LogError($"jumpKey = {jumpKey}, QuickJump error callback:{error}");
                    });

                    DebugUtil.LogFormat(ELogType.eNone, $"jumpKey = {jumpKey} QuickJump enable");
                }
                else
                {
                    DebugUtil.LogError($"jumpKey = {jumpKey} QuickJump error disenable");
                }
            });
        }

#else

            if (GetHaveSDK() && bPaiLianTu)
        {
            bPaiLianTu = false;
            sdk.SDKPaiLianTu();
        }
#endif
    }


    /// <summary>
    /// 推送系统 跳转通知设置页面
    /// </summary>
    public static void SDKPushOpenSystemSetting()
    {
#if !USE_OLDSDK
        AllinSDK.Push.OpenNotification();
#else
        sdk.SDKPushOpenSystemSetting();
#endif
    }


    /// <summary>
    /// 设置绑定手机号
    /// </summary>
    public static void SDKSetPhoneBind()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKSetPhoneBind");
        if (!SDKManager.sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK

        // 绑定快手账号
        AllinSDK.Account.BindAccount(LoginTypeOption.PHONE, delegate (string type)
        {
            DebugUtil.LogFormat(ELogType.eNone, "SDKSetPhoneBind BindAccount callback loginType = " + type);

            //更新绑定状态
            SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, 2);
        },
        delegate (Error error)
        {
            if (error.code == -100)
            {
                DebugUtil.LogError("领取礼包失败，不支持的礼包活动");
                SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, string.Format("领取礼包失败，不支持的礼包活动{0}", error.code));
            }
            else if (error.code == -101)
            {
                DebugUtil.LogError("领取礼包失败，未开启礼包活动");
                SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, string.Format("领取礼包失败，未开启礼包活动{0}",error.code));
            }
            else
            {
                DebugUtil.LogError("AccountAgent.BindAccount error callback: " + error.ToString());
                SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, string.Format("绑定账号失败（{0})", error.code));
            }

            Debug.Log("AccountAgent.BindAccount error callback: " + error.ToString());
        }
        );


#else
        sdk.SDKSetPhoneBind();
#endif

    }


    /// <summary>
    /// 温馨提示
    /// </summary>
    public static void SDKSDKWarmTips()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKSDKWarmTips");
        if (!SDKManager.sdk.IsHaveSdk)
            return;

#if !USE_OLDSDK
        AllinSDK.Permission.ShowPrivacyProtocol(Protocol.ALLIN);
#else
        if (GetHaveSDK())
            sdk.SDKWarmTips();
#endif
    }


    /// <summary>
    /// 用于判断 在某些渠道 登出按钮显示或隐藏 
    /// </summary>
    /// <param name="funcName"></param>
    /// <returns></returns>
    public static bool SDKApiAvailable(string funcName)
    {
        bool IsEnable = true;
#if !USE_OLDSDK
        //todo yd
        return IsEnable;
#else
        if (GetHaveSDK())
            IsEnable = sdk.SDKApiAvailable(funcName);
        return IsEnable;
#endif
    }


    /// <summary>
    ///SDK 接口判断是不是模拟器（只在android平台有效）
    /// </summary>
    public static bool SDKISEmulator()
    {
        // 快手提供的 系统的真机还是模拟器 接口，判断不出来 一些魔改的模拟器
        //AllinSDK.DeviceInfo.IsEmulator();
        bool isEmulator = false;
#if UNITY_ANDROID || UNITY_IOS
        if (SDKManager.sdk.IsHaveSdk)
        {
            //MTP服务到期或者关闭MTP功能，以及MTP没有登录，都会导致判断模拟器错误
            if (IsOpenGetExtJsonParam(EThirdSdkType.MTP.ToString(), out string paramsValue))
            {
                isEmulator = MTPIsSumulator();

                if (!isEmulator)
                    isEmulator = AllinSDK.DeviceInfo.IsEmulator();
            }
            else
                isEmulator = AllinSDK.DeviceInfo.IsEmulator();

            DebugUtil.LogFormat(ELogType.eNone, "SDKISEmulator isEmulator:" + isEmulator.ToString());
        }
#endif
        return isEmulator;

    }



    /// <summary>
    /// 预热活动
    /// </summary>
    /// <returns>预热快跳能否能跳</returns>
    public static bool SDKPreWarmActivity()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKPreWarmActivity");
        if (!SDKManager.sdk.IsHaveSdk)
            return false;

        bool isOpen = false;
#if !USE_OLDSDK

        String jumpKey = AllinSDK.Launcher.GetChannel().Equals("ks") ? "PreWarmG" : "PreWarmQ";
#if UNITY_IOS
        jumpKey = "PreWarmI";
#endif

        AllinSDK.QuickJump.CheckQuickJumpEnable(jumpKey, delegate (bool quickJumpEnable)
        {
            if (quickJumpEnable)
            {
                AllinSDK.QuickJump.QuickJump(jumpKey, delegate (int code, string msg)
                {
                    DebugUtil.LogFormat(ELogType.eNone, $"jumpKey={jumpKey} QuickJump code = {code} msg = {msg}");

                }, delegate (Error error)
                {
                    DebugUtil.LogError($"jumpKey={jumpKey} QuickJump error callback:{error}");
                });

                isOpen = true;
                DebugUtil.LogFormat(ELogType.eNone, $"jumpKey={jumpKey} QuickJump enable");
            }
            else
            {
                isOpen = false;
                DebugUtil.LogError($"jumpKey={jumpKey} QuickJump error disenable");
            }
        });
#else
        if (GetHaveSDK())
        {
            isOpen = sdk.SDKPreWarmActivity();
            DebugUtil.LogFormat(ELogType.eNone, "预热活动是否开启" + isOpen);
        }
#endif
        return isOpen;
    }


    /// <summary>
    /// iOS评分
    /// </summary>
    /// <returns>iOS评分功能是否开启/returns>
    public static bool SDKGetSCoreInAppStore()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKGetSCoreInAppStore");
        if (!SDKManager.sdk.IsHaveSdk)
            return false;

        bool isOpen = false;
#if !USE_OLDSDK

        if (IsOpenGetExtJsonParam(EThirdSdkType.ScoreInApp.ToString(), out string paramsValue))
        {
            String jumpKey = "score1";
            AllinSDK.QuickJump.CheckQuickJumpEnable(jumpKey, delegate (bool quickJumpEnable)
            {
                if (quickJumpEnable)
                {

                    AllinSDK.QuickJump.QuickJump(jumpKey, delegate (int code, string msg)
                    {
                        DebugUtil.LogFormat(ELogType.eNone, $"score1 QuickJump code = {code} msg = {msg}");

                    }, delegate (Error error)
                    {
                        DebugUtil.LogError(string.Format("score1 QuickJump error callback:{0}", error.ToString()));
                    });


                    DebugUtil.LogFormat(ELogType.eNone, "score1 QuickJump enable");
                }
                else
                {

                    DebugUtil.LogError("score1 QuickJump error disenable");
                }
            });
        }
#else
        if (GetHaveSDK() && SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.ScoreInApp))
        {
            isOpen = sdk.SDKGetSCoreInAppStore();
            //DebugUtil.LogFormat(ELogType.eNone, "iOS评分能否快跳" + isJump);
        }
#endif
        return isOpen;
    }


    /// <summary>
    /// 获取手机剩余磁盘
    /// </summary>
    /// <returns>返回多少兆</returns>
    public static int SDKGetLeftMemorySize()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKGetLeftMemorySize");

#if !USE_OLDSDK
        long leftsize = AllinSDK.DeviceInfo.GetFreeDiskSpace();
        if (leftsize > 0)
            return (int)(leftsize / 1024);
        return 0;
#else
        return sdk.GetLeftMemorySize();
#endif
    }


    /// <summary>
    /// 获取手机屏幕亮度
    /// </summary>
    /// <returns></returns>
    public static float GetBrightnessNative()
    {
        float value = sdk.GetBrightnessNative();
        Debug.Log("GetBrightnessNative:" + value);
        return value;
    }


    /// <summary>
    /// 设置手机屏幕亮度
    /// </summary>
    /// <returns></returns>
    public static void SetBrightnessNative(float value)
    {
        Debug.Log("SetBrightnessNative:" + value);
        sdk.SetBrightnessNative(value);
    }


    /// <summary>
    /// 获取手机网络连接的状态 (android接口:在部分android手机unity提供的接口不准确，是有android底层api)
    /// </summary>
    public static bool SDKInternetStatus()
    {
        bool netStatus = sdk.SDKInternetStatus();
        return netStatus;
    }



    /// <summary>
    /// ios获取手机的电量信息
    /// </summary>
    /// <returns></returns>
    public static int GetBatteryLevel()
    {
        return sdk.GetBatteryLevel();
    }

    /// <summary>
    /// 获取设备类型更细（用于打点上报）
    /// Android手机 = 1，Android Pad =2,iPhone手机 = 3，iPad =4，Windows PC= 5，Android手机内H5页面 = 6，Iphone手机内H5页面 =7，PC Web= 10
    /// </summary>
    /// <returns></returns>
    public static string SDKGetDeviceType()
    {
        return sdk.SDKGetDeviceType();
    }



    /// <summary>
    /// 获取设备类型 0=移动端, 1=模拟器, 2=PC
    /// </summary>
    /// <param name="IsSimulator"></param>
    /// <returns></returns>
    public static int GetDeviceType()
    {
#if !USE_OLDSDK
        // 设备类型(0=移动端, 1=模拟器, 2=PC) IOS目前只有移动端，后面如果需要区分模拟器和PC在搞吧
        int deviceType = 0;
        if (string.Equals(AssetPath.sPlatformName, "StandaloneWindows", StringComparison.Ordinal))
        {
            deviceType = 2;
        }
        else
        {
            //deviceType = (AllinSDK.DeviceInfo.GetDeviceType() == com.kwai.game.features.DeviceType.PHONE) ? 0 : 1;
            deviceType = SDKISEmulator() ? 1 : 0;
        }

        return deviceType;
#else
       // 设备类型(0=移动端, 1=模拟器, 2=PC) IOS目前只有移动端，后面如果需要区分模拟器和PC在搞吧
        int deviceType = 0;
        if (SDKISEmulator())
        {
            deviceType = 1;
        }
        else
        {
            //if(Application.platform == RuntimePlatform.WindowsPlayer)
            if (string.Equals(AssetPath.sPlatformName, "StandaloneWindows", StringComparison.Ordinal))
                deviceType = 2;
            else
                deviceType = 0;
        }
        return deviceType;
#endif
    }


    /// <summary>
    /// 设置返回键可以退出变量（android 接口）
    /// </summary>
    public static void SDKSetCanExitVariable()
    {
#if UNITY_ANDROID

        if (SDKManager.sdk.IsHaveSdk)
            sdk.SDKSetCanExitVariable();

#endif
    }


    /// <summary>
    /// 触发乐变隐私同意行为
    /// </summary>
    public static void SDKLeBianPrivacyChecked()
    {
#if UNITY_ANDROID
        if (SDKManager.sdk.IsHaveSdk)
            sdk.SDKLeBianPrivacyChecked();
#endif
    }


    /// <summary>
    /// 判断是不是乐变小包 （android 接口）
    /// </summary>
    /// <returns></returns>
    public static bool SDKLeBianSmallApk()
    {
        bool lebianSmallApk = false;

#if UNITY_ANDROID
        if (SDKManager.sdk.IsHaveSdk)
            lebianSmallApk = sdk.SDKLeBianSmallApk();
#endif
        return lebianSmallApk;
    }

    /// <summary>
    /// 开启乐变边玩边下 （android 接口）
    /// </summary>
    public static void SDKLeBianPlayLoadOpen()
    {
        if (SDKManager.sdk.IsHaveSdk)
            sdk.SDKLeBianPlayLoadOpen();
    }

    /// <summary>
    /// 乐变一次性下载所有的资源  （android 接口）
    /// </summary>
    public static void SDKLeBianDownloadFullRes()
    {
        if (SDKManager.sdk.IsHaveSdk)
            sdk.SDKLeBianDownloadFullRes();
    }

    /// <summary>
    /// 乐变获取需要下载的资源  （android 接口）
    /// </summary>
    /// <returns></returns>
    public static ulong SDKLeBianNeedDownLoadTotalSize(out string tempSize)
    {
        ulong totalSize = 0;
        ulong.TryParse(sdk.SDKLeBianNeedDownLoadTotalSize(), out totalSize);
        tempSize = Framework.HotFixStateManager.CountSize(totalSize);
        return totalSize;
    }


    //public static void SDKGetWangYiToken()
    //{
    //    if (SDKManager.sdk.IsHaveSdk)
    //        sdk.SDKGetWangYiToken();
    //}
#endregion



#region 获取SDK参数
#if USE_OLDSDK
    /// <summary>
    /// 判定是否有SDK
    /// </summary>
    /// <returns></returns>
    public static bool GetHaveSDK()
    {
        bool havesdk = false;
        return sdk.IsHaveSdk;
    }
#endif


    /// <summary>
    /// 标识登录用的渠道，如小米、华为、快手，初始化完成后可以获取。
    /// </summary>
    /// <returns></returns>
    public static string GetChannel()
    {
#if UNITY_EDITOR && SKIP_SDK_Login
        return SkipSDKLogin.Instance.channel;
#else
        return sdk.sChannel;//AllinSDK.Launcher.GetChannel();
#endif
    }


    /// <summary>
    /// 媒体投放的标识，比如channel都是快手渠道，分别投放在快手游戏中心、taptap，需要两个不同媒体投放标识的包，初始化完成后可以获取。
    /// </summary>
    /// <returns></returns>
    public static string GetPublishAppMarket()
    {
#if UNITY_EDITOR && SKIP_SDK_Login
        return SkipSDKLogin.Instance.channel;
#else
        return sdk.sMarketChannel;//AllinSDK.Launcher.GetMarketChannel();
#endif
    }


    /// <summary>
    /// 获取手机运营商
    /// </summary>
    /// <returns></returns>
    public static string GetMobileOperators()
    {
        return sdk.sMobileOperators;//AllinSDK.DeviceInfo.GetMobileOperators();
    }


    /// <summary>
    /// 唯一ID (前端称userid，后端称gameid或accountid)
    /// </summary>
    /// <returns></returns>
    public static string GetUID()
    {
#if UNITY_EDITOR && SKIP_SDK_Login
        return SkipSDKLogin.Instance.Account;
#else
        return sdk.sdkUid;
#endif
    }

    /// <summary>
    /// 令牌
    /// </summary>
    /// <returns></returns>
    public static string GetToken()
    {
        return sdk.token;
    }

    /// <summary>
    /// 获取设备id
    /// </summary>
    /// <returns></returns>
    public static string GetDeviceId()
    {
        return sdk.sDeviceid;//AllinSDK.Launcher.GetDeviceId();
    }


    /// <summary>
    /// 游戏大区ID
    /// </summary>
    /// <returns></returns>
    public static int GetZoneid()
    {
        return VersionHelper.ZoneId;
    }


    /// <summary>
    /// 获取游戏到AppID
    /// </summary>
    /// <returns></returns>
    public static string GetAppid()
    {
        return sdk.sAppid;
    }


    /// <summary>
    /// 获取当前app version name
    /// </summary>
    /// <returns></returns>
    public static string GetAppVersion()
    {
        string appVersion = "";
#if UNITY_EDITOR

#if SKIP_SDK_Login
        appVersion = SkipSDKLogin.Instance.appVersion;
#else
        appVersion = VersionHelper.AppBuildVersion.ToString();
#endif

#else
        appVersion = sdk.IsHaveSdk ? sdk.SDKGetAppVersion() : VersionHelper.AppBuildVersion.ToString();
#endif
        Debug.Log("appVersion:" + appVersion);
        return appVersion;
    }


    public static bool GetiOSPhaseCanPlay()
    {
        return sdk.SDKiOSPhaseCanPlay();
    }

    /// <summary>
    /// 获取三方SDK的设置
    /// </summary>k
    public static void GetThirdSdkSetting(Dictionary<string, string> ext_json)
    {
        //eThirdSdkTypeDict.Clear();
        //eThirdSdkTypeDict.Add(EThirdSdkType.GME, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.MTP, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.RNC, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.AddQQGroup, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.QuestionSurvey, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.CDKey, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.ReportPoint, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.MTPReport, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.PreWarmAll, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.PersonalShop, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.ConditionGiftPack, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.BindIphone, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.CloseLeBianPlayLoadInMobile, true);
        //eThirdSdkTypeDict.Add(EThirdSdkType.ScoreInApp, true);

        //if (ext_json != null && ext_json.Count > 0)
        //{
        //    foreach (var item in ext_json)
        //    {
        //        if (System.Enum.TryParse<EThirdSdkType>(item.Key, out EThirdSdkType eThirdSdkType))
        //        {
        //            if (item.Value == "1")
        //                eThirdSdkTypeDict[eThirdSdkType] = false;
        //        }
        //    }
        //}
    }


    /// <summary>
    /// 根据三方SDK类型 获取开关状态
    /// </summary>
    /// <param name="thirdSdkType"></param>
    /// <returns></returns>
    public static bool GetThirdSdkStatus(EThirdSdkType thirdSdkType)
    {
        bool status = true;
        //if (eThirdSdkTypeDict.ContainsKey(thirdSdkType))
        //    status = eThirdSdkTypeDict[thirdSdkType];
        return status;
    }


    /// <summary>
    /// 根据扩展参数：获取功能开启(关闭需要配置value=1，默认不配置或者配置0代表开启)，或者只是根据key获取Value
    /// </summary>
    /// <param name="paramKey"></param>
    /// <param name="paramValue"></param>
    /// <returns>功能开启关闭</returns>
    public static bool IsOpenGetExtJsonParam(string paramKey, out string paramValue)
    {
        bool isOpen = true;
        paramValue = null;
        if (VersionHelper.ExtJson != null && VersionHelper.ExtJson.ContainsKey(paramKey))
        {
            paramValue = VersionHelper.ExtJson[paramKey];
            if (paramValue.Equals("1"))
                isOpen = false;
        }
        return isOpen;
    }

    /// <summary>
    /// 切换账号成功后退回到选服界面
    /// </summary>
    /// <returns></returns>
    public static bool GetEnableSwitchAccount()
    {
        DebugUtil.LogFormat(ELogType.eNone, "GetEnableSwitchAccount");
        if (SDKManager.sdk.IsHaveSdk && bSwitchAccount)
            return true;
        else
            return false;
    }


    /// <summary>
    /// 判断一键加群功能 是否开启
    /// </summary>
    /// <returns></returns>
    public static bool AddQQGroupIsOpen()
    {
        return GetChannel().Equals(officialChannel) && IsOpenGetExtJsonParam(EThirdSdkType.AddQQGroup.ToString(), out string paramsValue);
    }




#endregion

#region GME
    private static bool bGMEInit = false;
    public static int InitGMESDK(ulong roleID)
    {
        if (!IsOpenGetExtJsonParam(EThirdSdkType.GME.ToString(), out string paramsValue))
            return -1;

        DebugUtil.Log(ELogType.eChat, "InitGMESDK");

        string path = Application.persistentDataPath + "/TempFilesVoice";
        Lib.AssetLoader.AssetPath.CreateDirectory(path);


        ITMGContext.GetInstance().SetLogPath(Application.persistentDataPath);
        //todo sdk 支持的是有符号的 INT64
        int ret = ITMGContext.GetInstance().Init("1400525929", roleID.ToString());
        if (ret == QAVError.OK)
        {
            bGMEInit = true;
        }
        else
        {
            bGMEInit = false;
            DebugUtil.LogErrorFormat("SDK GME 初始化失败 {0}", ret);
        }
        return ret;
    }
    public static void UninitGMESDK()
    {
        string path = Application.persistentDataPath + "/TempFilesVoice";
        Lib.AssetLoader.AssetPath.DeleteDirectory(path);

        ITMGContext.GetInstance().Uninit();
    }

#endregion

#region MPT
    public static void InitMTPSDK()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (sdk.IsHaveSdk && GetThirdSdkStatus(EThirdSdkType.MTP))
        {
            //在游戏启动的第一时间调用 MTP
            mTssInfoReceiver = new MyTssInfoReceiver();
            Tp2Sdk.Tp2RegistTssInfoReceiver(mTssInfoReceiver);
            Tp2Sdk.Tp2SdkInitEx(20133, "4bba0f46eacca4bee7132b681a57c13c");
        }
#endif
    }

    public static void MTPUserLogin(int world_id, string open_id, string role_id)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (sdk.IsHaveSdk && GetThirdSdkStatus(EThirdSdkType.MTP))
        {
            // 第一次调用在获取用户信息并登录游戏时,如[微信登录].[QQ登录]或[开始游戏]按钮事件触发
            // 在每次获取用户授权信息之后都要调用Tp2UserLogin,如断线重连
            //world_id = 用户游戏角色的大区信息
            //open_id = 与登录平台相关的帐号id             
            //role_id = 区分用户创建的不同角色的标识
            int account_type = (int)Tp2Entry.ENTRY_ID_OTHERS;        /*与运营平台相关的帐号类型*/
            Tp2Sdk.Tp2UserLogin(account_type, world_id, open_id, role_id);
        }
#endif
    }

    public static void UninitMTPSDK()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (sdk.IsHaveSdk && GetThirdSdkStatus(EThirdSdkType.MTP))
        {
            mTssInfoReceiver = null;
        }
#endif
    }


    public static bool MTPIsSumulator()
    {
        //只有 MTPUserLogin 登录成功后，模拟器的判断才有效
        bool isSumulator = false;
        string result = Tp2Sdk.Ioctl(Tp2Sdk.TssSDKCmd_IsEmulator, "files_dir=/data/data/package_name/files|wait=1");
        if (!String.IsNullOrEmpty(result) && result.Contains("|"))
        {
            string[] strArr = result.Split('|');
            if (strArr[0].Contains("="))
            {
                String[] str0Arr = strArr[0].Split('=');
                if (str0Arr[0].Equals("retval") && str0Arr[1].Equals("1"))
                    isSumulator = true;
            }
        }

        DebugUtil.LogFormat(ELogType.eNone,string.Format("MTPIsSumulator:{0},{1}", result, isSumulator.ToString()));

        return isSumulator;
    }

#endregion

#region ACE
    private static AceClient mAceClient = default;
    private static bool isACELogin = false;

    private static bool IsACEInit()
    {
        if (mAceClient != null)
            return true;
        return false;
    }
    private static void ACESDKInit()
    {
        //ace登陆的账号类型申请的是手机号类型，所以必须要要有pcsdk
        if (!SDKManager.sdk.IsHaveSdk)
            return;

        ClientInitInfo init_info = default;
        init_info.first_process_pid = System.Diagnostics.Process.GetCurrentProcess().Id;
        init_info.current_process_role_id = -1;
        init_info.base_dat_path = null;
        var result = AceClient.init(ref init_info, null, out mAceClient);
        if (result != AntiCheatExpertResult.ACE_OK)
        {
            DebugUtil.LogFormat(ELogType.eNone, "ACESDKInit  failed:{0}", result);
        }
    }
    public static void ThirdACESdkLogin(string role_id, int serverId)
    {
        if (!IsACEInit()) return;
        ACESDKLogin(GetUID(), role_id, serverId);
    }
    private static void ACESDKLogin(string open_id, string role_id, int serverId)
    {
        if (!IsACEInit()) return;

        AceAccountInfo acc_info = default;
        acc_info.account_id_.account_ = open_id;
        acc_info.account_id_.account_type_ = (ushort)AceAccountType.ACEACCOUNT_TYPE_PHONE_OPENID;
        acc_info.plat_id_ = (ushort)AceAccountPlatId.ACEPLAT_ID_PC_CLIENT;
        acc_info.world_id_ = (uint)VersionHelper.ZoneId;
        acc_info.game_id_ = 6010016;//游戏在接入时由业务安全部主动提供给开发商，为固定值
        acc_info.channel_id_ = (uint)serverId;

        ulong roleid;
        if (ulong.TryParse(role_id, out roleid))
            acc_info.role_id_ = roleid;

        var result = mAceClient.log_on(ref acc_info);
        if (result != AntiCheatExpertResult.ACE_OK)
        {
            DebugUtil.LogFormat(ELogType.eNone, "ACESDKLogin  failed:{0}", result);
            return;
        }
        if (result == AntiCheatExpertResult.ACE_OK)
        {
            isACELogin = true;
        }
    }

    public static void ACESDKTick()
    {
        if (!IsACEInit()) return;
        mAceClient.tick();
    }

    public static void ACESDKLoginOff()
    {
        if (!IsACEInit()) return;
        if (isACELogin)
        {
            isACELogin = false;
            mAceClient.log_off();
        }
    }
    public static void ACESDKExitProcess()
    {
        if (!IsACEInit()) return;
        ACESDKLoginOff();
        mAceClient.exit_process();
    }
#endregion

    public static void Update()
    {
        if (bGMEInit)
        {
            ITMGContext.GetInstance().Poll();
        }
        ACESDKTick();
    }
    public static void Pause()
    {
        if (bGMEInit)
        {
            ITMGContext.GetInstance().Pause();
        }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (GetThirdSdkStatus(SDKManager.EThirdSdkType.MTP))
        {
            Tp2Sdk.Tp2SetGamestatus(Tp2GameStatus.BACKEND);
        }
#endif

    }
    public static void Resume()
    {
        if (bGMEInit)
        {
            ITMGContext.GetInstance().Resume();
        }

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (GetThirdSdkStatus(SDKManager.EThirdSdkType.MTP))
        {
            Tp2Sdk.Tp2SetGamestatus(Tp2GameStatus.FRONTEND);
        }
#endif

    }




#if !USE_OLDSDK
    public static void SDKGetGameZone()
    {
        DebugUtil.LogFormat(ELogType.eNone, "SDKGetGameZone");

        AllinSDK.ServerList.GetGameZone(
            delegate (int code, long zoneId, string zongName, KwaiGatewayZoneInfo zoneInfo)
            {
                bwaitSDKGetGameZoneComplete = true;
                if (zoneInfo != null)
                {
                    Debug.Log(string.Format("SDKGetGameZone zoneinfo:{0}", zoneInfo));
                    _zoneinfo = zoneInfo;
                }
                else
                {
                    DebugUtil.LogError("SDKGetGameZone zoneinfo: is null");
                    AppManager.eAppError = EAppError.RemoteVersionInfoError;
                }

            },
            delegate (Error error)
            {
                DebugUtil.LogError(string.Format("SDKGetGameZone error:{0}", error));

                bwaitSDKGetGameZoneComplete = true;
                AppManager.eAppError = EAppError.RemoteVersionNetError;

            });
    }

    public static void SDKGetGameUser()
    {
        List<string> bindChannels;

        AllinSDK.Account.GetGameUser(delegate (KwaiGameUser userInfo)
        {
            bindChannels = userInfo.BindChannel;
            if (bindChannels.Contains("passport"))//已绑
                SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, 2);
            else
                SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, 1);

            DebugUtil.LogFormat(ELogType.eNone, string.Format("UserName：{0}, BindChannel：{1}", userInfo.UserName, userInfo.BindChannel.Count));

        }, delegate (Error error)
        {
            DebugUtil.LogError(string.Format("AccountAgent.GetGameUser error callback: {0}", error));
        });
    }

    public static void SDKCheckGiftActivityStatus()
    {
        //礼物领取，服务器告知，这边不在判断
        AllinSDK.Account.CheckGiftActivityStatus(KwaiGameGiftActivityType.BIND_PHONE.ToString(),
           delegate (GiftStatusResponse response)
           {
               DebugUtil.LogFormat(ELogType.eNone, "AccountAgent.CheckGiftActivityStatus success");
               if (response.Result)
               { //已领取
                 //setGiftButtonStatus(false);
                   DebugUtil.LogFormat(ELogType.eNone, "AccountAgent.CheckGiftActivityStatus 已领取");
               }
               else
               {
                   // setGiftButtonStatus(true);
               }

           },
           delegate (Error error)
           {
               //LogView.AddLog("AccountAgent.CheckGiftActivityStatus error callback: " + error.ToString());
               //setGiftButtonStatus(false);
           });
    }

    public static void SDKGetProductDetail()
    {
        //目前这个版本用不到
        DebugUtil.LogFormat(ELogType.eNone, "PayAgent.QueryProductDetails");
        var queryProductConfig = new QueryProductConfig();
        queryProductConfig.IsSubscribe = false;
        queryProductConfig.ExtendJSON = "";
        List<string> productIds = new List<string>() { @"TestGameItem2", @"TestGameItem3", @"test1234", @"test1234_1" };
        queryProductConfig.ProductIds = productIds;
        AllinSDK.Pay.QueryProductDetails(queryProductConfig,
            delegate (List<ProductDetail> detailInfos)
            {
                // LogView.AddLog("PayAgent.QueryProductDetails callback detailInfos:" + detailInfos.ToString());
                DebugUtil.LogFormat(ELogType.eNone, "PayAgent.QueryProductDetails callback");
                foreach (var productDetail in detailInfos)
                {
                    DebugUtil.LogFormat(ELogType.eNone, "PayAgent.QueryProductDetails callback item detailInfos:" + productDetail.ToString());
                }
            },
            delegate (Error error)
            {
                DebugUtil.LogFormat(ELogType.eNone, "PayAgent.QueryProductDetails callback error: " + error.ToString());
            });
    }


    /// <summary>
    /// Android 接口，点击返回键掉sdk的退出弹框
    /// </summary>
    public static void SDKExitApp()
    {
        AllinSDK.Account.ExitApp(delegate (int option)
        {
            if ((ExistOption)option == ExistOption.CONFIRM)
            {
                //退出前上报打点
                SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.onSDKExitReportExtraData, SDKManager.SDKReportState.EXIT);


                DebugUtil.LogFormat(ELogType.eNone, "user confirm exist app");
                //需要区分平台  
                SDKManager.SDKExitGame();
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "user cancel exist app");
            }
        });
    }



    /// <summary>
    /// C# 实体对象Object转HashTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Hashtable Object2Hashtable(object obj)
    {
        Hashtable hashtable;
        if (obj != null)
        {
            string strContent = LitJson.JsonMapper.ToJson(obj);
            hashtable = Newtonsoft.Json.JsonConvert.DeserializeObject<Hashtable>(strContent);
        }
        else
        {
            hashtable = new Hashtable();
        }

        return hashtable;
    }



#endif


}
