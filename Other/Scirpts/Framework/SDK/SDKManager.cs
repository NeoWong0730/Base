
using Lib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
    public static class SDKManager
    {
        /// <summary> SDK基类 </summary>
        public static SDKBase sdk;
        /// <summary> bInit：代表同意隐私政策回调之后(android,ios), 在pc上指sdk初始化</summary>
        public static bool bInit;
        public static bool bPaiLianTu;
        public static bool bSwitchAccount;
        public static bool bIsSimulator;
        public static bool bIsQRCode;
        public static int iAccountType;
        private static Transform msdkRoot;

        private static Dictionary<EThirdSdkType, bool> eThirdSdkTypeDict = new Dictionary<EThirdSdkType, bool>();

        private static Action BuglyInitCallBack;
        private static string sDefaultHitPoint = "NULL";
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
        }


        public enum ESDKLoginStatus : int
        {
            OnSDKLoginCancel,
            OnSDKLoginSucced,
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

        public readonly static string officialChannel = "ks";

        public static void Init(Action buglyInitCallBack)
        {
            msdkRoot = new GameObject("SdkManager").transform;
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
            //判断是否有SDK
            SDK_ISHaveSDK();
            OnEnter();
        }

        public static void OnEnter()
        {
#if UNITY_ANDROID || UNITY_IPHONE
        if (GetHaveSDK())
            SDKManager.SDKBuglyInit();
#endif
        }

        public static void OnExit()
        {

        }

        #region SDK功能
        /// <summary>
        /// 判定是否有SDK
        /// </summary>
        private static void SDK_ISHaveSDK()
        {
            if (sdk == null) return;
            sdk.IsHaveSDK();
        }
        /// <summary>
        /// SDK初始化
        /// </summary>
        public static void SDKInit()
        {
            if (sdk == null) return;
            sdk.Init();
        }
        /// <summary>
        /// SDK登录
        /// </summary>
        public static void SDKLogin()
        {
            HitPointManager.HitPoint("game_sdk_login");

            if (sdk == null) return;
            sdk.Login();
        }
        /// <summary>
        /// SDK登出
        /// </summary>
        public static void SDKLogout()
        {
            if (sdk == null) return;
            sdk.Logout();
        }
        /// <summary>
        /// SDK付费
        /// </summary>
        public static void SDKPay(string payjson)
        {
            if (sdk == null) return;
            sdk.Pay(payjson);
        }

        public static void SDKPay_AutoRepairRecoder_RegistDelete()
        {
            if (sdk == null) return;
            sdk.PayAutoRepairRecoder_RegistDelete();
        }
        /// <summary>
        /// SDK 自动补单信息回掉
        /// </summary>
        /// <param name="payjson"></param>
        public static void SDKPay_RepairOrder(string payjson)
        {
            if (sdk == null) return;
            sdk.PayRepairOrder(payjson);
        }
        /// <summary>
        /// SDK发送游戏数据
        /// </summary>
        public static void SDKSendGameData(string rolejson)
        {
            if (sdk == null) return;
            sdk.SendGameData(rolejson);
        }
        /// <summary>
        /// SDK退出游戏
        /// </summary>
        public static void SDKExitGame()
        {
            if (sdk == null) return;
            sdk.ExitGame();
        }
        /// <summary>
        /// SDK用户中心
        /// </summary>
        public static void SDKOpenUserCenter()
        {
            if (sdk == null) return;
            sdk.UserCenter();
        }
        /// <summary>
        /// SDK浮窗
        /// </summary>
        /// <param name="isShow"></param>
        public static void SDKShowWindow(bool isShow)
        {
            if (sdk == null) return;
            sdk.ShowWindow(isShow);
        }
        /// <summary>
        /// 隐私政策
        /// </summary>
        public static void SDKPrivacyPolicy()
        {
            if (sdk == null) return;
            sdk.PrivacyPolicy();
        }
        /// <summary>
        /// 用户协议
        /// </summary>
        public static void SDKUserAgreement()
        {
            if (sdk == null) return;
            sdk.UserAgreement();
        }
        /// <summary>
        /// 进入战斗后，需要设置此变量，防止防沉迷弹框的弹出
        /// </summary>
        /// <param name="isFighting"></param>
        public static void SDK_SetGameFightStatus(bool isFighting)
        {
            if (sdk == null) return;
            sdk.SetGameFightStatus(isFighting);
            if (!isFighting)
                SDKUpdateAntiAddictStatus();
        }
        /// <summary>
        /// 同步 防沉迷的状态
        /// </summary>
        public static void SDKUpdateAntiAddictStatus()
        {
            if (sdk == null) return;
            sdk.SyncAntiAddictStatus();
        }
        /// <summary>
        /// 索要打点基础信息
        /// </summary>
        public static string SDKGetHitPointBaseInfo()
        {
            if (sdk == null) return null;
            string baseData = string.Empty;
            if (GetHaveSDK())
            {
                baseData = sdk.GetHitPointBaseData();
                DebugUtil.LogFormat(ELogType.eNone, string.Format("SDKGetHitPointBaseInfo:{0}", baseData));
            }
            return baseData;
        }
        /// <summary>
        /// 调查问卷
        /// </summary>
        /// <param name="urlStr"></param>
        public static void SDKOpenH5Questionnaire(string urlStr)
        {
            if (sdk == null) return;
            sdk.OpenH5Questionnaire(urlStr);
        }
        /// <summary>
        /// 上报打点信息
        /// </summary>
        /// <param name="actionName">上报事件名称</param>
        /// <param name="jsonContent">上报事件内容</param>
        public static void HitPoint(string actionName, object obj)
        {
            if (sdk == null || !GetHaveSDK()) return;

            string strContent = sDefaultHitPoint;
            if (obj != null)
                strContent = LitJson.JsonMapper.ToJson(obj);
            sdk.ReportHitPoint(actionName, strContent);

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
        /// 一键加入官方群
        /// </summary>
        public static int SDKjoinQQGroup()
        {
            if (sdk == null) return -1;
            int result = -1;
            result = sdk.JoinQQGroup();
            DebugUtil.LogFormat(ELogType.eNone, string.Format("join QQ Group :{0}", result));
            return result;
        }
        /// <summary>
        /// 打开实名认证界面
        /// </summary>
        public static void SDKCallRealNameUI()
        {
            if (sdk == null) return;
            sdk.CallRealNameUI();
        }
        /// <summary>
        /// 获取手机剩余内存
        /// </summary>
        /// <returns></returns>
        public static int SDKGetLeftMemorySize()
        {
            if (sdk == null) return -1;
            return sdk.GetLeftMemorySize();
        }

        /// <summary>
        /// ios获取手机屏幕亮度
        /// </summary>
        /// <returns></returns>
        public static float GetBrightnessNative()
        {
            float value = sdk.GetBrightnessNative();
            Debug.Log("GetBrightnessNative:" + value);
            return value;
        }

        /// <summary>
        /// ios设置手机屏幕亮度
        /// </summary>
        /// <returns></returns>
        public static void SetBrightnessNative(float value)
        {
            Debug.Log("SetBrightnessNative:" + value);
            sdk.SetBrightnessNative(value);
        }

        /// <summary>
        /// 通过sdk打印重要的信息
        /// </summary>
        public static void SDKPrintLog(SDKManager.ESDKLogLevel logLevel, string msg)
        {
            if (!GetHaveSDK())
                return;

#if UNITY_STANDALONE_WIN && USE_PCSDK
        PCSDKPriintLog(logLevel, msg);
#else
            sdk.SDKPrintLog(logLevel, msg);
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
        /// <summary>
        /// 上报玩家角色信息  (ios不在上报)
        /// </summary>
        /// <param name="reportJson"></param>
        public static void SDKReportExtraData(string reportJson)
        {
#if UNITY_ANDROID
        if (GetHaveSDK())
            sdk.SDKReportExtraData(reportJson);
#endif
        }

        public static void SDKCommonConfigHitPoint(string dataJson)
        {
#if UNITY_ANDROID || UNITY_IOS
        if (GetHaveSDK())
        {
            sdk.SDKCommonConfigHitPoint(dataJson);
        }
#endif
        }


        /// <summary>
        /// 扫码二位码
        /// </summary>
        public static void SDKScanQRCode()
        {
            sdk.SDKScanQRCode();
        }
        /// <summary>
        /// 客服
        /// </summary>
        public static void SDKCustomService()
        {
            sdk.SDKCustomService();
        }
        /// <summary>
        /// 社区
        /// </summary>
        public static void SDKCommunityService()
        {
            sdk.SDKCommunityService();
        }
        /// <summary>
        /// 获取手机网络连接的状态
        /// </summary>
        public static bool SDKInternetStatus()
        {
            bool netStatus = sdk.SDKInternetStatus();
            return netStatus;
        }

        /// <summary>
        /// 渠道上报错误信息
        /// </summary>
        /// <param name="flagName">标记信息</param>
        public static void SDKReportErrorToChannel(string flagName)
        {
#if UNITY_ANDROID || UNITY_IPHONE
        if (GetHaveSDK())
            sdk.SDKReportErrorToChannel(flagName);
#elif UNITY_STANDALONE_WIN && USE_PCSDK
        sdk.SDKReportErrorToChannel();
#endif
        }
        /// <summary>
        /// 拍连图
        /// </summary>
        public static void SDKPaiLianTu()
        {
            if (GetHaveSDK() && bPaiLianTu)
            {
                bPaiLianTu = false;
                sdk.SDKPaiLianTu();
            }
        }

        public static void SDKBuglyInit()
        {
            DebugUtil.LogFormat(ELogType.eNone, "SDKBuglyInit: " + (BuglyInitCallBack != null).ToString());
            BuglyInitCallBack?.Invoke();
        }
        /// <summary>
        /// 推送系统 只加了小米的推送
        /// </summary>
        public static void SDKPushOpenSystemSetting()
        {
            sdk.SDKPushOpenSystemSetting();
        }

        /// <summary>
        /// 获取设备类型 0=移动端, 1=模拟器, 2=PC
        /// </summary>
        /// <param name="IsSimulator"></param>
        /// <returns></returns>
        public static int GetDeviceType()
        {
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
        }

        /// <summary>
        /// 设置绑定手机号
        /// </summary>
        public static void SDKSetPhoneBind()
        {
            if (GetHaveSDK())
                sdk.SDKSetPhoneBind();
        }

        /// <summary>
        /// 接入快游盾（以后可替代MTP）
        /// </summary>
        /// <param name="roleId"></param>
        public static void SDKRunGameShiled(string roleId)
        {
            if (GetHaveSDK())
                sdk.SDKRunGameShiled(roleId);
        }

        /// <summary>
        /// 温馨提示
        /// </summary>
        public static void SDKSDKWarmTips()
        {
            if (GetHaveSDK())
                sdk.SDKWarmTips();
        }

        /// <summary>
        /// 用于判断 在某些渠道 登出按钮显示或隐藏 
        /// </summary>
        /// <param name="funcName"></param>
        /// <returns></returns>
        public static bool SDKApiAvailable(string funcName)
        {
            bool IsEnable = true;
            if (GetHaveSDK())
                IsEnable = sdk.SDKApiAvailable(funcName);
            return IsEnable;
        }

        /// <summary>
        /// 判断是不是乐变小包
        /// </summary>
        /// <returns></returns>
        public static bool SDKLeBianSmallApk()
        {
            bool lebianSmallApk = false;
#if UNITY_ANDROID
        if (GetHaveSDK())
            lebianSmallApk = sdk.SDKLeBianSmallApk();
#endif
            return lebianSmallApk;
        }

        /// <summary>
        /// 开启乐变边玩边下
        /// </summary>
        public static void SDKLeBianPlayLoadOpen()
        {
            if (GetHaveSDK())
                sdk.SDKLeBianPlayLoadOpen();
        }

        /// <summary>
        /// 乐变一次性下载所有的资源
        /// </summary>
        public static void SDKLeBianDownloadFullRes()
        {
            if (GetHaveSDK())
                sdk.SDKLeBianDownloadFullRes();
        }

        /// <summary>
        /// 乐变获取需要下载的资源
        /// </summary>
        /// <returns></returns>
        public static ulong SDKLeBianNeedDownLoadTotalSize(out string tempSize)
        {
            ulong totalSize;
            ulong.TryParse(sdk.SDKLeBianNeedDownLoadTotalSize(), out totalSize);
            tempSize = Framework.HotFixStateManager.CountSize(totalSize);
            return totalSize;
        }

        /// <summary>
        ///SDK 接口判断是不是模拟器 
        /// </summary>
        public static bool SDKISEmulator()
        {
            bool isEmulator = false;
            if (GetHaveSDK())
            {
                //MTP服务到期或者关闭MTP功能，会导致判断模拟器错误,这时备用方案使用jar包再次判断
                //DebugUtil.LogFormat(ELogType.eNone, "isEmulator：" + isEmulator);
                if (GetThirdSdkStatus(EThirdSdkType.MTP))
                {
                    //isEmulator = MTPIsSumulator();
                    if (!isEmulator)
                        isEmulator = sdk.SDKISEmulator();
                }
                else
                    sdk.SDKISEmulator();
            }
            return isEmulator;
        }

        /// <summary>
        /// 退回热更前，杀掉进程，重启app
        /// </summary>
        public static void SDKAppKillAndResart()
        {
            if (GetHaveSDK())
                sdk.SDKAppKillAndResart();
        }

        /// <summary>
        /// 预热活动
        /// </summary>
        /// <returns>预热快跳能否能跳</returns>
        public static bool SDKPreWarmActivity()
        {
            bool isOpen = false;
            if (GetHaveSDK())
            {
                isOpen = sdk.SDKPreWarmActivity();
                DebugUtil.LogFormat(ELogType.eNone, "预热活动是否开启" + isOpen);
            }
            return isOpen;
        }


        /// <summary>
        /// iOS评分
        /// </summary>
        /// <returns>iOS评分能否能跳</returns>
        public static bool SDKGetSCoreInAppStore()
        {
            bool isJump = false;
            if (GetHaveSDK() && SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.ScoreInApp))
            {
                isJump = sdk.SDKGetSCoreInAppStore();
                DebugUtil.LogFormat(ELogType.eNone, "iOS评分能否快跳" + isJump);
            }
            return isJump;
        }

        public static void SDKSetCanExitVariable()
        {
#if UNITY_ANDROID
        if (GetHaveSDK())
            sdk.SDKSetCanExitVariable();
#endif
        }

        public static void SDKGetWangYiToken()
        {
            if (GetHaveSDK())
                sdk.SDKGetWangYiToken();
        }
        #endregion

        #region 获取SDK参数
        /// <summary>
        /// 判定是否有SDK
        /// </summary>
        /// <returns></returns>
        public static bool GetHaveSDK()
        {
            bool havesdk = false;
            if (sdk != null)
            {
                havesdk = sdk.GetIsHaveSdk();
            }
            return havesdk;
        }


        /// <summary>
        /// 标识登录用的渠道，如小米、华为、快手，初始化完成后可以获取。
        /// </summary>
        /// <returns></returns>
        public static string GetChannel()
        {
            if (sdk != null)
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            return SkipSDKLogin.Instance.channel;
#else
                return sdk.Channel;
#endif
            }
            return string.Empty;
        }


        /// <summary>
        /// 媒体投放的标识，比如channel都是快手渠道，分别投放在快手游戏中心、taptap，需要两个不同媒体投放标识的包，初始化完成后可以获取。
        /// </summary>
        /// <returns></returns>
        public static string GetPublishAppMarket()
        {
            if (sdk != null)
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            return SkipSDKLogin.Instance.channel;
#else
                return sdk.PublishAppMarket;
#endif
            }
            return null;
        }



        /// <summary>
        /// 唯一ID (前端称userid，后端称gameid或accountid)
        /// </summary>
        /// <returns></returns>
        public static string GetUID()
        {
            if (sdk != null)
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            return SkipSDKLogin.Instance.Account;
#else
                return sdk.Uid;
#endif
            }
            return string.Empty;
        }

        /// <summary>
        /// 令牌
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            if (sdk != null)
            {
                return sdk.Token;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取设备id
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceId()
        {
            if (sdk != null)
            {
                return sdk.Deviceid;
            }
            return string.Empty;
        }


        /// <summary>
        /// 游戏大区ID
        /// </summary>
        /// <returns></returns>
        public static string GetGameid()
        {
            if (sdk != null)
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            return SkipSDKLogin.Instance.Account;
#else
                return sdk.Uid;
#endif
            }
            return string.Empty;
        }

        /// <summary>
        /// 是否已登入SDK
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            if (sdk != null)
            {
                return sdk.CheckIsLogin();
            }
            return false;
        }

        /// <summary>
        /// 获取游戏到AppID
        /// </summary>
        /// <returns></returns>
        public static string GetAppid()
        {
            string Appid = "ks695524668676586328";
            if (sdk != null)
            {
                return sdk.Appid;
            }
            return Appid;
        }

        /// <summary>
        /// 获取游戏内部资源号
        /// </summary>
        /// <returns></returns>
        public static string GetGameVersion()
        {
            string GameVersion = "1.0.0";
            if (sdk != null)
            {
#if UNITY_EDITOR && SKIP_SDK_Login
            return SkipSDKLogin.Instance.appVersion;
#else
                if (GetHaveSDK())
                    return sdk.GameVersion;
                else
                    return HitPointManager.GetSDKHitPointBaseData().app_version;
#endif
            }


            return GameVersion;
        }

        /// <summary>
        /// 支付内容转为字符串
        /// </summary>
        /// <param name="orderId"> 订单 id </param>
        /// <param name="productId"> 商品 id </param>
        /// <param name="productName"> 商品名称 </param>
        /// <param name="productDesc"> 商品描述 </param>
        /// <param name="price"> 商品价格 </param>
        /// <param name="coinNum"> 当前账户虚拟币余额数 </param>
        /// <param name="buyNum"> 购买虚拟币数 </param>
        /// <param name="currency"> 虚拟币名称 </param>
        /// <param name="rate"> 单位人民币兑换比例(所有商品比例必须相同) </param>
        /// <param name="extension"> 扩展字段 </param>
        /// <returns></returns>
        public static string PayJsonToString(string orderId, string productId, string productName, string productDesc, string price, string coinNum, string buyNum, string currency, string rate, string extension)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"orderId\":\"{0}\"", string.IsNullOrEmpty(orderId) ? "" : orderId);
            sb.Append(",");
            sb.AppendFormat("\"productId\":\"{0}\"", string.IsNullOrEmpty(productId) ? "" : productId);
            sb.Append(",");
            sb.AppendFormat("\"productName\":\"{0}\"", string.IsNullOrEmpty(productName) ? "" : productName);
            sb.Append(",");
            sb.AppendFormat("\"productDesc\":\"{0}\"", string.IsNullOrEmpty(productDesc) ? "" : productDesc);
            sb.Append(",");
            sb.AppendFormat("\"price\":\"{0}\"", string.IsNullOrEmpty(price) ? "" : price);
            sb.Append(",");
            sb.AppendFormat("\"coinNum\":\"{0}\"", string.IsNullOrEmpty(coinNum) ? "" : coinNum);
            sb.Append(",");
            sb.AppendFormat("\"buyNum\":\"{0}\"", string.IsNullOrEmpty(buyNum) ? "" : buyNum);
            sb.Append(",");
            sb.AppendFormat("\"currency\":\"{0}\"", string.IsNullOrEmpty(currency) ? "" : currency);
            sb.Append(",");
            sb.AppendFormat("\"rate\":\"{0}\"", string.IsNullOrEmpty(rate) ? "" : rate);
            sb.Append(",");
            sb.AppendFormat("\"extension\":\"{0}\"", string.IsNullOrEmpty(extension) ? "" : extension);
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 角色信息转为字符串
        /// </summary>
        /// <param name="sendType"> 数据上报类型(0:角色信息变更 1:登陆 2:创角) </param>
        /// <param name="roleId"> 游戏角色 id </param>
        /// <param name="roleName"> 游戏角色名 </param>
        /// <param name="roleLevel"> 游戏等级 </param>
        /// <param name="vipLevel"> vip 等级 </param>
        /// <param name="clientId"> 服务器 id </param>
        /// <param name="clientName"> 服务器名称 </param>
        /// <param name="laborunion"> 游戏公会 </param>
        /// <param name="extension"> 扩展字段 </param>
        /// <returns></returns>
        public static string RoleInfoJsonToString(string sendType, string roleId, string roleName, string roleLevel, string vipLevel, string clientId, string clientName, string laborunion, string extension)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"sendType\":\"{0}\"", string.IsNullOrEmpty(sendType) ? "null" : sendType);
            sb.Append(",");
            sb.AppendFormat("\"roleId\":\"{0}\"", string.IsNullOrEmpty(roleId) ? "null" : roleId);
            sb.Append(",");
            sb.AppendFormat("\"roleName\":\"{0}\"", string.IsNullOrEmpty(roleName) ? "null" : roleName);
            sb.Append(",");
            sb.AppendFormat("\"roleLevel\":\"{0}\"", string.IsNullOrEmpty(roleLevel) ? "null" : roleLevel);
            sb.Append(",");
            sb.AppendFormat("\"vipLevel\":\"{0}\"", string.IsNullOrEmpty(vipLevel) ? "null" : vipLevel);
            sb.Append(",");
            sb.AppendFormat("\"clientId\":\"{0}\"", string.IsNullOrEmpty(clientId) ? "null" : clientId);
            sb.Append(",");
            sb.AppendFormat("\"clientName\":\"{0}\"", string.IsNullOrEmpty(clientName) ? "null" : clientName);
            sb.Append(",");
            sb.AppendFormat("\"laborunion\":\"{0}\"", string.IsNullOrEmpty(laborunion) ? "null" : laborunion);
            sb.Append(",");
            sb.AppendFormat("\"extension\":\"{0}\"", string.IsNullOrEmpty(extension) ? "null" : extension);
            sb.Append("}");
            return sb.ToString();
        }
        /// <summary>
        /// 获取设备等级
        /// </summary>
        /// <returns></returns>
        public static int GetDeviceLevel()
        {
            return sdk.GetDeviceLevel();
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
        /// 获取三方SDK的设置
        /// </summary>
        public static void GetThirdSdkSetting(Dictionary<string, string> ext_json)
        {
            if (sdk == null) return;
            eThirdSdkTypeDict.Clear();
            eThirdSdkTypeDict.Add(EThirdSdkType.GME, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.MTP, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.RNC, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.AddQQGroup, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.QuestionSurvey, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.CDKey, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.ReportPoint, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.MTPReport, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.PreWarmAll, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.PersonalShop, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.ConditionGiftPack, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.BindIphone, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.CloseLeBianPlayLoadInMobile, true);
            eThirdSdkTypeDict.Add(EThirdSdkType.ScoreInApp, true);


            if (ext_json != null && ext_json.Count > 0)
            {
                foreach (var item in ext_json)
                {
                    if (System.Enum.TryParse<EThirdSdkType>(item.Key, out EThirdSdkType eThirdSdkType))
                    {
                        if (item.Value == "1")
                            eThirdSdkTypeDict[eThirdSdkType] = false;
                    }
                }
            }

            #region 老方式 读取配置表
            //Stream stream = AssetMananger.Instance.LoadStream("Config/ThirdSdkSetting.txt");
            //StreamReader sr = new StreamReader(stream);
            //string line;
            //while ((line = sr.ReadLine()) != null)
            //{
            //    if (string.IsNullOrEmpty(line))
            //        continue;
            //    string[] strs = line.Split('|');
            //    if (System.Enum.TryParse<EThirdSdkType>(strs[0], out EThirdSdkType eThirdSdkType))
            //    {
            //        eThirdSdkTypeDict.Add(eThirdSdkType, strs[1].Equals("1"));
            //        DebugUtil.LogFormat(ELogType.eSdk, string.Format("{0}:{1}", eThirdSdkType, strs[1]));
            //    }
            //}
            //sr.Close();
            //sr.Dispose();
            //stream.Close();
            //stream.Dispose();
            #endregion
        }

        /// <summary>
        /// 根据三方SDK类型 获取开关状态
        /// </summary>
        /// <param name="thirdSdkType"></param>
        /// <returns></returns>
        public static bool GetThirdSdkStatus(EThirdSdkType thirdSdkType)
        {
            bool status = true;
            if (eThirdSdkTypeDict.ContainsKey(thirdSdkType))
                status = eThirdSdkTypeDict[thirdSdkType];
            return status;
        }

        /// <summary>
        /// 获取实名认证的状态
        /// </summary>
        /// <returns></returns>
        public static Action onRealNameStatusChange;
        public static Action onRealNameStatusFail;
        public static bool GetRealNameStatus()
        {
            //那就只针对快手渠道需要先实名认证才能聊天，三方渠道不管实名不实名都可以聊天
            if (GetHaveSDK() && GetThirdSdkStatus(EThirdSdkType.RNC) && GetChannel().Equals(officialChannel))
            {
                return sdk.RealNameStatus;
            }
            return true;
        }
        public static void SetRealNameStatus(bool v)
        {
            if (sdk.RealNameStatus != v)
            {
                sdk.RealNameStatus = v;
                onRealNameStatusChange?.Invoke();
            }
        }
        static UnityWebRequest unityWebRequest;
        public static void GetRealNameWebRequest()
        {
            if (null == unityWebRequest)
            {
                // 可以使用以下链接测试
                //string url = string.Format("{0}?app_id={1}&game_id={2}&game_token={3}", VersionHelper.UserInfoUrl, "ks695524668676586328", "120592009db2d8e97607fb38a26ffceaae", 
                //"ChJvcGVucGxhdGZvcm0udG9rZW4SMB2AmZo6GAMhxNvv_TJeC5YZPEbeKqphHtXKGWTccQe-IdEl7OzZVjmxMTkt_ZRWkxoSL6Sa8XKMQxi4mINpGJvYLFJkIiB9yKjeVtdj1wt8QoOjPBtiB7FYZDOOtpxssVAmJAaEWigFMAE");
                string url = string.Format("{0}?app_id={1}&game_id={2}&game_token={3}", VersionHelper.UserInfoUrl, GetAppid(), GetUID(), GetToken());
                DebugUtil.LogFormat(ELogType.eNone, "realNameWeb:{0}", url);
                unityWebRequest = UnityWebRequest.Get(url);
                unityWebRequest.timeout = 2;
                unityWebRequest.SendWebRequest().completed += AsyncOperation_completed;
            }
        }
        private static void AsyncOperation_completed(AsyncOperation asyncOperation)
        {
            bool iscertificated = false;

            if (unityWebRequest.isDone)
            {
                using (MemoryStream ms = new MemoryStream(unityWebRequest.downloadHandler.data))
                {
                    using (StreamReader sr = new StreamReader(ms))
                    {
                        string content = sr.ReadToEnd();
                        DebugUtil.LogFormat(ELogType.eNone, "接口获取内容：\n {0}", content);
                        if (!string.IsNullOrEmpty(content))
                        {
                            UserInfoRealName versionInfo = LitJson.JsonMapper.ToObject<UserInfoRealName>(content);
                            iscertificated = versionInfo.certificated;
                        }
                    }
                }
            }
            unityWebRequest.Dispose();
            unityWebRequest = null;

            if (!iscertificated)
            {
                onRealNameStatusFail?.Invoke();
                SDKCallRealNameUI();
            }
            else
            {
                SetRealNameStatus(true);
            }
        }

        public static bool GetEnableSwitchAccount()
        {
            if (GetHaveSDK() && bSwitchAccount)
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
            return GetChannel().Equals(officialChannel) && GetThirdSdkStatus(EThirdSdkType.AddQQGroup);
        }

        #endregion


        public static void Update()
        {

        }
        public static void Pause()
        {


        }
        public static void Resume()
        {


        }

    }
}