using System;
using UnityEngine;
using Lib;

namespace Framework
{
    public class AppState_WaitSDK : IAppState
    {
        public EAppState State { get { return EAppState.WaitSdDK; } }


        private bool bWait = true;

        public void OnEnter()
        {
#if UNITY_ANDROID || UNITY_IOS
            SDKManager.bInit = true;
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
            //if (bWait && SDKManager.bInit)
            //{
            //    bWait = false;
            //    DebugUtil.LogFormat(ELogType.eNone, "Initialize Completed");

            //    if (CheckDeviceLimit())
            //    {
            //        AppManager.NextAppState = EAppState.CheckVersion;
            //    }
            //    else
            //    {
                    
            //    }
            //}
        }

        public void OnApplicationPause(bool pause)
        {

        }

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
            int limit = 8;
            if (!deviceModel.StartsWith(deviceName, StringComparison.Ordinal))
            {
                deviceName = "iPad";
                limit = 5;
                if (!deviceModel.StartsWith(deviceName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            string s = deviceModel.Remove(0, deviceName.Length);
            string[] ss = s.Split(",");

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
}
