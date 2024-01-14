using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework
{
    // Application.internetReachabilityֻ��������ʹ�ã��������Լ�⵽wifi/4g/��������������������������£����һֱ��NotReachable״̬
    // ������Ҫ��������������£��ṩһ������ӿڣ�ͳһ�������������
    // ��Ϊ����������ʱ��������ʵһֱ�ǹرյģ����Իᱻ��Ϊ�ǣ� NetworkReachability.NotReachable
    // https://blog.csdn.net/weixin_30807677/article/details/96313781
    public static class NetworkHelper
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        [DllImport("sensapi.dll", SetLastError = true)]
        private static extern bool IsNetworkAlive(out int connectionDescription);
#endif

        /// <summary>
        /// ������ �Ƿ���
        /// </summary>
        public static bool IsWanOpen()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        // ������/������/������ �Ƿ���
        public static bool IsWanOrLanOpen()
        {
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
        // ʣ�µ�ʹ��unity�ӿ��ж�
        return IsWanOpen();
#endif
        }

        /// <summary>
        /// ��⵱ǰ�����Ƿ�ΪWIFI����
        /// </summary>
        public static bool IsWifi()
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }

        /// <summary>
        /// ��⵱ǰ�����Ƿ�Ϊ�ƶ�����
        /// </summary>
        public static bool IsCarrier()
        {
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
                    netStatus = "�ƶ�";
                    break;

            }

            return netStatus;
        }

    }
}