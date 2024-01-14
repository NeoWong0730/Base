using System.Text.RegularExpressions;

public class SDKBase
{
    //登录渠道标识 标识登录用的渠道，如小米、华为、快手，初始化完成后可以获取。
    public string Channel = "test";

    //投放渠道标识 媒体投放的标识，比如channel都是快手渠道，分别投放在快手游戏中心、taptap，需要两个不同媒体投放标识的包，初始化完成后可以获取。
    public string PublishAppMarket = "testPublish";

    public string Uid = "testUid";
    public string Token = "testToken";
    public string Gameid = "testGameid";
    public string Deviceid = "testdevice";
    public string Appid = "ks695524668676586328";
    public string GameVersion = "1.0.0";
    public string HitPointBaseData;
    public string WangYiToken = "";

    public bool IsHaveSdk = false;
    public bool RealNameStatus = false;

#if UNITY_IOS
    public string bIsJailbreak = "0";//默认越狱为false = 0 (只有ios使用)
#endif

    public SDKBase() { }

    public virtual void IsHaveSDK() { }
    public virtual void Init() { }

    public virtual void Login() { }

    public virtual void Logout() { }

    public virtual void Pay(string payjson) { }

    public virtual void PayAutoRepairRecoder_RegistDelete() { }

    public virtual void PayRepairOrder(string payjson) { }

    public virtual void SendGameData(string rolejson) { }

    public virtual void ExitGame() { }

    public virtual void UserCenter() { }

    public virtual void ShowWindow(bool isShow) { }

    public virtual void UserAgreement() { }

    public virtual void PrivacyPolicy() { }

    public virtual void ReportHitPoint(string actionName, string recordJson) { }

    public virtual void SetGameFightStatus(bool isFighting) { }

    public virtual void SyncAntiAddictStatus() { }

    public virtual string GetHitPointBaseData() { return string.Empty; }

    public virtual void OpenH5Questionnaire(string urlStr) { }

    public virtual int GetDeviceLevel() { return 2; }

    public virtual int GetBatteryLevel() { return 100; }

    public virtual int JoinQQGroup() { return -1; }

    public virtual void CallRealNameUI() { }

    public virtual int GetLeftMemorySize() { return -1; }

    public virtual void SDKPrintLog(SDKManager.ESDKLogLevel logLevel, string msg) { }
    public virtual void SDKPrintLog(string logLevel, string msg) { }

    public virtual void SDKReportExtraData(string reportJson) { }
    public virtual void SDKCommonConfigHitPoint(string sValue) { }

    public virtual void SDKScanQRCode() { }

    public virtual void SDKCustomService() { }

    public virtual void SDKCommunityService() { }
    public virtual bool SDKInternetStatus() { return false; }

    public virtual void SDKReportErrorToChannel(string flagName) { }
    public virtual void SDKReportErrorToChannel() { }
    public virtual void SDKPaiLianTu() { }

    public virtual float GetBrightnessNative() { return -1; }
    public virtual void SetBrightnessNative(float value) { }

    public virtual void SDKPushOpenSystemSetting() { }

    public virtual void SDKSetPhoneBind() { }

    public virtual void SDKRunGameShiled(string roleId) { }

    public virtual void SDKWarmTips() { }
    public virtual bool SDKApiAvailable(string funcName) { return true; }

    public virtual bool SDKLeBianSmallApk() { return false; }
    public virtual void SDKLeBianPlayLoadOpen() { }
    public virtual void SDKLeBianDownloadFullRes() { }
    public virtual string SDKLeBianNeedDownLoadTotalSize() { return "0"; }

    public virtual bool SDKISEmulator() { return false; }

    public virtual void SDKAppKillAndResart() { }

    public virtual bool SDKPreWarmActivity() { return false; }

    public virtual void SDKSetCanExitVariable() { }

    public virtual bool SDKGetSCoreInAppStore() { return false; }

    public virtual void SDKGetWangYiToken() { }


    public void SetChannel(string channel)
    {
        if (string.IsNullOrEmpty(channel)) return;
        Channel = channel;
    }

    public void SetPublishAppMarket(string publish_app_market)
    {
        if (string.IsNullOrEmpty(publish_app_market)) return;
        PublishAppMarket = publish_app_market;
    }

    public void SetDeviceId(string deviceid)
    {
        if (string.IsNullOrEmpty(deviceid)) return;
        Deviceid = deviceid;
    }

    public void SetAppVersion(string appversion)
    {
        if (string.IsNullOrEmpty(appversion)) return;
        GameVersion = appversion;
    }

    public bool CheckIsInitSDK()
    {
        return !string.IsNullOrEmpty(Channel);
    }

    public bool CheckIsLogin()
    {
        return !string.IsNullOrEmpty(Uid);
    }

    public bool IsNumber(string value)
    {
        string pattern = @"^\d*$";
        return Regex.IsMatch(value, pattern);
    }

    public string ReturnHitPointBaseData()
    {
        return HitPointBaseData;
    }

    public bool GetIsHaveSdk()
    {
        return IsHaveSdk;
    }
}