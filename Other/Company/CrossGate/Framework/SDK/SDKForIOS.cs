#if UNITY_IPHONE
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class SDKForIOS : SDKBase
{
    [DllImport("__Internal")]
    // 给iOS传string参数,无返回值,返回值通过iOS的UnitySendMessage方法返回给Unity
    private static extern void CallSDKFunc(string funcName, string args);

    [DllImport("__Internal")]
    // 给iOS传2个string参数,无返回值,返回值通过iOS的UnitySendMessage方法返回给Unity
    private static extern void CallSDKFunc2Args(string funcName, string args1, string args2);


    [DllImport("__Internal")]
    // 给iOS传string参数,带返回值,返回值为string类型 返回给Unity
    public static extern string RtCallSDKFunc(string funcName, string args);

    [DllImport("__Internal")]
    // 给iOS传string参数,带返回值,返回值为float类型 返回给Unity
    public static extern float RtFloatCallSDKFunc(string funcName, string args);

    [DllImport("__Internal")]
    // 给iOS传string参数,带返回值,返回值为int类型 返回给Unity
    public static extern int RtIntCallSDKFunc(string funcName, string args);



    public override void IsHaveSDK()
    {
        string havesdk = RtCallSDKFunc("SDK_ISHave", string.Empty);
        IsHaveSdk = havesdk.Equals("True");
    }

    public override void Login()
    {
        CallSDKFunc("SDK_Login", string.Empty);
    }

    public override void Logout()
    {
        CallSDKFunc("SDK_Logout", string.Empty);
    }

    public override void Pay(string payjson)
    {
        CallSDKFunc("SDK_Pay", payjson);
    }

    public override void PayAutoRepairRecoder_RegistDelete()
    {
        CallSDKFunc("SDK_PayAutoRepairRecoder_RegistDelete", string.Empty);
    }

    public override void PayRepairOrder(string payjson)
    {
        CallSDKFunc("SDK_PayRepairOrder", payjson);
    }

    public override void SendGameData(string rolejson)
    {
        CallSDKFunc("SDK_SendGameData", rolejson);
    }

    public override void ExitGame()
    {
        CallSDKFunc("SDK_ExitGame", string.Empty);
    }

    public override void UserCenter()
    {
        CallSDKFunc("SDK_UserCenter", string.Empty);
    }

    public override void ShowWindow(bool isShow)
    {
        CallSDKFunc("SDK_ShowWindow", System.Convert.ToInt32(isShow).ToString());
    }

    public override void UserAgreement()
    {
        //string url = string.Format("https://www.gamekuaishou.com/policy?channel={0}&appid={1}", Channel,Appid);
        CallSDKFunc("SDK_UserAgreement", string.Empty);
    }

    public override void PrivacyPolicy()
    {
        //string url = string.Format("https://www.gamekuaishou.com/privacy?channel={0}&appid={1}", Channel, Appid);
        CallSDKFunc("SDK_PrivacyPolicy", string.Empty);
    }

    public override void ReportHitPoint(string actionName, string recordJson)
    {
        CallSDKFunc2Args("SDK_ReportPoint", actionName, recordJson);
    }

    public override void SetGameFightStatus(bool isFighting)
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc("SDK_SetGameFightStatus", isFighting.ToString());
    }

    public override void SyncAntiAddictStatus()
    {
        CallSDKFunc("SDK_SyncAntiAddictStatus", string.Empty);
    }

    //public override string GetHitPointBaseData()
    //{
    //    if (string.IsNullOrEmpty(HitPointBaseData))
    //    {
    //        // appId = str[0],device_id = str[1],channel = str[2],packagechannel = str[3],platform = str[4],operator_type = str[5],app_version = str[6]
    //        HitPointBaseData = RtCallSDKFunc("SDK_GetHitPointBaseData", string.Empty);
    //        string[] str = HitPointBaseData.Split('|');
    //        Appid = str[0];
    //        SetDeviceId(str[1]);
    //        SetChannel(str[2]);
    //        SetPublishAppMarket(str[3]);
    //        SetAppVersion(str[6]);
    //    }
    //    return HitPointBaseData;
    //}

    public override void OpenH5Questionnaire(string urlStr)
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc("SDK_OpenWebURL", urlStr);
    }

    public override int GetBatteryLevel()
    {//出ios包需要修改 batteryLevel * 100--by yd 
        int batteryLevel = 100;
        batteryLevel = RtIntCallSDKFunc("SDK_GetBatteryLevel", string.Empty);
        return batteryLevel;
    }

    public override int JoinQQGroup()
    {
        if (!CheckIsInitSDK()) return -1;
        return RtIntCallSDKFunc("SDK_joinQQGroup", string.Empty);
    }

    public override void CallRealNameUI()
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc("SDK_CallRealNameUI", string.Empty);
    }


    public override int GetLeftMemorySize()
    {
        if (!CheckIsInitSDK()) return 0;
        return RtIntCallSDKFunc("SDK_GetLeftMemorySize", string.Empty);
    }

    public override void SDKPrintLog(SDKManager.ESDKLogLevel eSDKLogLevel, string msg)
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc2Args("SDK_PrintLog", eSDKLogLevel.ToString(), msg);
    }

    public override void SDKScanQRCode()
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc("SDK_ScanQRCode", string.Empty);
    }

    public override void SDKCustomService()
    {
        if (!CheckIsInitSDK()) return;
        CallSDKFunc("SDK_CustomService", string.Empty);
    }

    //public override void SDKCommunityService()
    //{
    //    if (!CheckIsInitSDK()) return;
    //    CallSDKFunc("SDK_CommunityService", string.Empty);
    //}


    //public override bool SDKInternetStatus()
    //{
    //    return Call<bool>("SDK_GetInternetStatus");
    //}


    public override void SDKCommonConfigHitPoint(string sValue)
    {
        CallSDKFunc("SDK_CommonConfigHitPoint", sValue);
    }

    public override void SDKReportErrorToChannel(string flagName)
    {
        CallSDKFunc("SDK_ReportErrorToChannel", string.Empty);
    }

    public override void SDKPaiLianTu()
    {
        CallSDKFunc("SDK_PaiLianTu", string.Empty);
    }

    public override float GetBrightnessNative()
    {
        float value = RtFloatCallSDKFunc("SDK_GetBrightnessNative", string.Empty);
        return value;
    }

    public override void SetBrightnessNative(float value)
    {
        string arg = value.ToString();
        CallSDKFunc("SDK_SetBrightnessNative", arg);
    }

    public override void SDKSetPhoneBind()
    {
        CallSDKFunc("SDK_SetPhoneBind", string.Empty);
    }

    public override void SDKWarmTips()
    {
        CallSDKFunc("SDK_WarmTips", string.Empty);
    }

    public override bool SDKPreWarmActivity()
    {
        string havePreWarm = RtCallSDKFunc("SDK_PreWarmActivity", string.Empty);
        bool isOpen = havePreWarm.Equals("True");
        return isOpen;
    }

    public override bool SDKGetSCoreInAppStore()
    {
        string haveScore = RtCallSDKFunc("SDK_GetSCoreInAppStore", string.Empty);
        bool isOpen = haveScore.Equals("True");
        return isOpen;
    }


    //public override void SDKGetWangYiToken()
    //{
    //    CallSDKFunc("SDK_GetWangYiToken", string.Empty);
    //}

    //以下是新接入的sdk
    public override bool QQIsInstalled()
    {
        string haveScore = RtCallSDKFunc("SDK_QQIsInstalled", string.Empty);
        bool isOpen = haveScore.Equals("True");
        return isOpen;
    }

    public override string SDKGetAppVersion()
    {
        return RtCallSDKFunc("SDK_GetAppVersion", string.Empty);
    }


    public override string SDKGetDeviceType()
    {
        return RtCallSDKFunc("SDK_GetDeviceType", string.Empty);
    }

    public override bool SDKiOSPhaseCanPlay()
    {
        if(string.IsNullOrEmpty(_iOSPhaseCanPlay))
            _iOSPhaseCanPlay = RtCallSDKFunc("SDK_iOSPhaseCanPlay", string.Empty);
        bool isOpen = _iOSPhaseCanPlay.Equals("True",StringComparison.Ordinal);
        return isOpen;
    }

}
#endif