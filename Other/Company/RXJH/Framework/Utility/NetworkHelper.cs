using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Application.internetReachability只能在外网使用，外网可以检测到wifi/4g/断网等情况，但是在内网环境下，这个一直是NotReachable状态
// 所以需要在内内外网情况下，提供一个对外接口，统一处理这种情况。
// 因为内网开发的时候，网关其实一直是关闭的，所以会被认为是： NetworkReachability.NotReachable
// https://blog.csdn.net/weixin_30807677/article/details/96313781
public static class NetworkHelper {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN 
    [DllImport("sensapi.dll", SetLastError = true)]
    private static extern bool IsNetworkAlive(out int connectionDescription);
#endif

    /// <summary>
    /// 广域网 是否开启
    /// </summary>
    public static bool IsWanOpen() {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    // 局域网/广域网/虚拟网 是否开启
    public static bool IsWanOrLanOpen() {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN 
        bool isNetworkConnected = IsNetworkAlive(out int connectionDescription);
        int errCode = Marshal.GetLastWin32Error();
        return isNetworkConnected /*&& errCode == 0*/;
#elif UNITY_ANDROID 
        if (SDKManager.sdk.IsHaveSdk)
        {
            return SDKManager.SDKInternetStatus();
        }
        else
        {
            return IsWanOpen();
        }
#else
        // 剩下的使用unity接口判断
        return IsWanOpen();
#endif
    }

    /// <summary>
    /// 检测当前网络是否为WIFI网络
    /// </summary>
    public static bool IsWifi() {
        return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
    }

    /// <summary>
    /// 检测当前网络是否为移动网络
    /// </summary>
    public static bool IsCarrier() {
        return Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork;
    }


    public static string GetNetworkStatus()
    {
        string netStatus = "NULL";
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                netStatus = "Wifi";
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                netStatus = "移动";
                break;
            
        }

        return netStatus;
    }

}