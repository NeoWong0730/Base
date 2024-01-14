using System;
using UnityEngine;
using NWFramework;

namespace GameMain.NWsdk
{
    public class NWsdkAndroidApi : INWsdkApi
    {
        const string platformName = "PlatformProxy";
        const string pluginPreName = "com.nwsdk.plugin.";
#if UNITY_ANDROID
        private static AndroidJavaClass nwsdk
        {
            get
            {
                try
                {
                    AndroidJavaClass sdk = new AndroidJavaClass("com.nwsdk.sdk.NWsdk");
                    return sdk;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    return null;
                }
            }
        }
#endif

        private void SendAndroidMessage(string pluginName, string methodName, params object[] parameters)
        {
#if UNITY_ANDROID
            try
            {
                pluginName = pluginPreName + pluginName;

                if (nwsdk != null)
                {
                    AndroidJavaObject context = nwsdk.CallStatic<AndroidJavaObject>("getPlugin", pluginName);
                    context?.Call(methodName, parameters);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
#endif
        }

        private T SendAndroidMessage<T>(string pluginName, string methodName, params object[] parameters)
        {
#if UNITY_ANDROID
            try
            {
                pluginName = pluginPreName + pluginName;

                if (nwsdk != null)
                {
                    AndroidJavaObject context = nwsdk.CallStatic<AndroidJavaObject>("getPlugin", pluginName);
                    if (nwsdk != null)
                    {
                        return context.Call<T>(methodName, parameters);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return default(T);
            }
#endif
            return default(T);
        }

        public void CallPlugin(string pluginName, string methodName, params object[] parameters)
        {
            SendAndroidMessage(pluginName, methodName, parameters);
        }

        public T CallPlugin<T>(string pluginName, string methodName, params object[] parameters)
        {
            return SendAndroidMessage<T>(pluginName, methodName, parameters);
        }

        public bool IsExistPlugin(string pluginName)
        {
#if UNITY_ANDROID
            pluginName = pluginPreName + pluginName;

            if (nwsdk != null) {
                AndroidJavaObject context = nwsdk.CallStatic<AndroidJavaObject> ("getPlugin", pluginName);
                return context != null ? true : false;
            }
#endif
            return true;
        }

        public bool IsExistField(string pluginName, string fieldName)
        {
#if UNITY_ANDROID
            pluginName = pluginPreName + pluginName;

            if (nwsdk != null) {
                return nwsdk.CallStatic<bool> ("isExistField", pluginName, fieldName);
            }
#endif
            return true;
        }

        public bool IsExistMethod(string pluginName, string methodName)
        {
#if UNITY_ANDROID
            pluginName = pluginPreName + pluginName;

            if (nwsdk != null)
            {
                return nwsdk.CallStatic<bool>("isExistMethod", pluginName, methodName);
            }
#endif
            return true;
        }

        public void SetCallBack(NWsdkCallBackListener callback)
        {
#if UNITY_ANDROID
            nwsdk?.CallStatic("setCallBack", callback);
#endif
        }
    }
}