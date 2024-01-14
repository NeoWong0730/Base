#if UNITY_ANDROID
using UnityEngine;
public class SDKForAndroid : SDKBase
{
    private AndroidJavaObject jo;
    public SDKForAndroid() : base()
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
    }

    private void Call(string funcName, params object[] args)
    {
        try
        {
            if (jo != null)
                jo.Call(funcName, args);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    /// <summary>
    /// Unity调用安卓方法 带返回值，只能返回一个值。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="funcName"></param>
    /// <param name="parms"></param>
    /// <returns></returns>
    private T Call<T>(string funcName, params object[] parms)
    {
        try
        {
            if (jo != null)
            {
                T returnVar = jo.Call<T>(funcName, parms);
                return returnVar;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        return default(T);
    }


    public override void IsHaveSDK()
    {
        IsHaveSdk = Call<bool>("SDK_ISHave");
    }
    public override void Init()
    {
        Call("SDK_Init");
    }

    public override void Login()
    {
        if (!CheckIsInitSDK()) return;

#if USE_SDK_QRCODE
        SDKManager.bIsQRCode = true;
        Call("SDK_QRLogin");
#else
        Call("SDK_Login");
#endif
    }

    public override void Logout()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_Logout");
    }

    public override void Pay(string payjson)
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_Pay", payjson);
    }

    public override void SendGameData(string rolejson)
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_SendGameData", rolejson);
    }

    public override void ExitGame()
    {
        Call("SDK_ExitGame");
    }

    public override void UserCenter()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_UserCenter");
    }

    public override void ShowWindow(bool isShow)
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_ShowWindow", System.Convert.ToInt32(isShow).ToString());
    }

    public override void UserAgreement()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_UserAgreement");
    }

    public override void PrivacyPolicy()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_PrivacyPolicy");
    }

    public override void ReportHitPoint(string actionName, string recordJson)
    {
        //if (!CheckIsInitSDK()) return;游戏启动上报打点 ，无需等初始化成功
        Call("SDK_ReportPoint", actionName, recordJson);
    }

    public override void SetGameFightStatus(bool isFighting)
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_SetGameFightStatus", isFighting);
    }

    public override void SyncAntiAddictStatus()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_SyncAntiAddictStatus");
    }

    public override string GetHitPointBaseData()
    {
        if (string.IsNullOrEmpty(HitPointBaseData))
        {
            HitPointBaseData = Call<string>("SDK_GetHitPointBaseData");
            // appId = str[0],device_id = str[1],channel = str[2],packagechannel = str[3],platform = str[4],operator_type = str[5],app_version = str[6]
            string[] str = HitPointBaseData.Split('|');
            Appid = str[0];
            SetDeviceId(str[1]);
            SetChannel(str[2]);
            SetPublishAppMarket(str[3]);
            SetAppVersion(str[6]);
        }

        return HitPointBaseData;
    }

    public override void OpenH5Questionnaire(string urlStr)
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_H5Questionnaire", urlStr);
    }

    public override int GetDeviceLevel()
    {
        if (!CheckIsInitSDK()) return 2;
        int rlt = Call<int>("GetDeviceLevel");

        return rlt;
    }

    public override int GetBatteryLevel()
    {
        int rlt = 100;
        if (GetIsHaveSdk())
            rlt = Call<int>("SDK_BatteryLevel");
        return rlt;
    }


    public override int JoinQQGroup()
    {
        if (!CheckIsInitSDK()) return -1;

        return Call<int>("SDK_joinQQGroup");
    }

    public override void CallRealNameUI()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_CallRealNameUI");
    }

    public override int GetLeftMemorySize()
    {
        if (!CheckIsInitSDK()) return 0;
        return Call<int>("SDK_GetLeftMemorySize");
    }

    public override void SDKPrintLog(SDKManager.ESDKLogLevel eSDKLogLevel, string msg)
    {
        Call("SDK_PrintLog", (int)eSDKLogLevel, msg);
    }

    public override void SDKScanQRCode()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_ScanQRCode");
    }

    public override void SDKCustomService()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_CustomService");
    }

    public override void SDKCommunityService()
    {
        if (!CheckIsInitSDK()) return;
        Call("SDK_CommunityService");
    }


    public override bool SDKInternetStatus()
    {
        return Call<bool>("SDK_GetInternetStatus");
    }


    public override void SDKReportExtraData(string reportJson)
    {
        Call("SDK_ReportExtraData", reportJson);
    }


    public override void SDKCommonConfigHitPoint(string sValue)
    {
        Call("SDK_CommonConfigHitPoint", sValue);
    }


    public override void SDKReportErrorToChannel(string flagName)
    {
        Call("SDK_ReportErrorToChannel", flagName);
    }

    public override void SDKPaiLianTu()
    {
        Call("SDK_PaiLianTu");
    }

    public override float GetBrightnessNative()
    {
        return Call<float>("SDK_GetBrightnessNative");
    }

    public override void SetBrightnessNative(float value)
    {
        Call("SDK_SetBrightnessNative", value.ToString());
    }


    public override void SDKPushOpenSystemSetting()
    {
        Call("SDK_PushOpenSystemSetting");
    }


    public override void SDKSetPhoneBind()
    {
        Call("SDK_SetPhoneBind");
    }


    public override void SDKRunGameShiled(string roleId)
    {
        Call("SDK_RunGameShiled", roleId);
    }

    public override void SDKWarmTips()
    {
        Call("SDK_WarmTips");
    }


    public override bool SDKApiAvailable(string funcName)
    {
        return Call<bool>("SDK_ApiAvailable", funcName);
    }

    public override bool SDKLeBianSmallApk()
    {
        return Call<bool>("SDK_LeBianSmallApk");
    }
    public override void SDKLeBianPlayLoadOpen()
    {
        Call("SDK_LeBianPlayLoadOpen");
    }

    public override void SDKLeBianDownloadFullRes()
    {
        Call("SDK_LeBianDownloadFullRes");
    }


    public override void SDKAppKillAndResart()
    {
        Call("SDK_AppKillAndRestart");
    }

    public override bool SDKISEmulator()
    {
        return Call<bool>("SDK_ISEmulator");
    }

    public override bool SDKPreWarmActivity()
    {
        return Call<bool>("SDK_PreWarmActivity");
    }

    public override void SDKSetCanExitVariable()
    {
        Call("SDK_SetCanExitVariable");
    }


    public override string SDKLeBianNeedDownLoadTotalSize()
    {
        return Call<string>("SDK_LeBianNeedDownLoadTotalSize");
    }


    public override void SDKGetWangYiToken()
    {
        Call("SDK_GetWangYiToken");
    }

}
#endif