using Framework;
using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppState_WaitSDK : IAppState
{
    public EAppState State { get { return EAppState.WaitSDK; } }

    private bool bWait = true;
  
    public void OnEnter()
    {
#if UNITY_ANDROID || UNITY_IOS
        /* android平台 能走到这里说明有sdk
         * unityPlayer的创建是在隐私合规同意后，所以能走到这里，说明sdk已经初始化成功*/
        //SDKManager.bInit = true;
#endif
    }

    public void OnExit()
    {
    }

    public void OnFixedUpdate()
    {

    }

    public void OnLowMemory() 
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public void OnGUI()
    {
        
    }

    public void OnLateUpdate()
    {

    }

    public void OnUpdate()
    {
        if (bWait && SDKManager.bInit)
        {
            bWait = false;
            DebugUtil.LogFormat(ELogType.eNone, "初始化完成");

            //获取打点信息
            //SDKManager.SDKGetHitPointBaseInfo();

            //TODO:版本临时 限制地段机型
            //int lv = SDKManager.GetDeviceLevel();
            //Debug.Log("Option Level " + lv);
            //if (lv < 0)
            //{
            //    HitPointManager.HitPoint("game_device_limit", null, null);
            //    UI_Box.Create(UseUIBOxType.NotPlayByLevel);
            //}
            //else
            //{
            //    AppManager.NextAppState = EAppState.CheckVersion;
            //}            

            //AppManager.NextAppState = EAppState.CheckVersion;

            if (CheckDeviceLimit())
            {
                AppManager.NextAppState = EAppState.CheckVersion;
            }
            else
            {
                HitPointManager.HitPoint("game_device_limit");
                UI_Box.Create(UseUIBOxType.NotPlayByLevel);
            }
        }
    }

    public void OnApplicationPause(bool pause) { }

    private static bool CheckDeviceLimit()
    {
#if UNITY_IOS
        return CheckIOSDeviceLimit();
#else
        return true;
#endif
    }

    private static bool CheckIOSDeviceLimit()
    {
        string deviceModel = SystemInfo.deviceModel;

        string deviceName = "iPhone";
        int limit = 8; //iPhone 6s (2015)
        if (!deviceModel.StartsWith(deviceName, StringComparison.Ordinal))
        {
            deviceName = "iPad";
            limit = 5; //iPad mini4 (2015)
            if (!deviceModel.StartsWith(deviceName, StringComparison.Ordinal))
            {
                return true;
            }
        }

        string s = deviceModel.Remove(0, deviceName.Length);
        string[] ss = s.Split(',');

        if (ss.Length != 2)
        {
            return true;

        }

        int.TryParse(ss[0], out int arg0);
        int.TryParse(ss[1], out int arg1);

        if (arg0 < limit)
            return false;

        return true;
    }
}
