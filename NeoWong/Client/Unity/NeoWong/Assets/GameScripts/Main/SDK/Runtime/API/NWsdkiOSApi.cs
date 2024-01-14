using System;
using System.Runtime.InteropServices;

namespace GameMain.NWsdk
{
    public class NWsdkiOSApi : INWsdkApi
    {
        private static NWsdkCallBackListener _callback;

        private delegate void NWsdkCallBackListenerCallBack(string callbackName, string jsonMsg);

        private static void NWsdkCallBackListenerMethod(string callbackName, string jsonMsg)
        {
            _callback?.OnCallBack(callbackName, jsonMsg);
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void __CallPlugin(string pluginName, string methodName, string[] parameters);
#else
        private static void __CallPlugin(string pluginName, string methodName, string[] parameters) { }
#endif

        public void CallPlugin(string pluginName, string methodName, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                parameters = new string[] { "" };
            string[] args = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = parameters[i].ToString();
            }
            __CallPlugin(pluginName, methodName, args);
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern string __CallPluginR (string pluginName, string methodName, string[] parameters);
#else
        private static string __CallPluginR(string pluginName, string methodName, string[] parameters)
        {
            return string.Empty;
        }
#endif

        public T CallPlugin<T>(string pluginName, string methodName, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                parameters = new string[] { "" };
            string[] args = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                args[i] = parameters[i].ToString();

            string ret = __CallPluginR(pluginName, methodName, args);
            T retR = (T)Convert.ChangeType(ret, typeof(T));
            return retR;
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern bool __IsExistPlugin (string pluginName);
#else
        private static bool __IsExistPlugin(string pluginName)
        {
            return true;
        }
#endif
        public bool IsExistPlugin(string pluginName)
        {
            return __IsExistPlugin(pluginName);
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern bool __IsExistMethod (string pluginName, string methodName);
#else
        private static bool __IsExistMethod(string pluginName, string methodName)
        {
            return true;
        }
#endif
        public bool IsExistMethod(string pluginName, string methodName)
        {
            return __IsExistMethod(pluginName, methodName);
        }

        public bool IsExistField(string pluginName, string fieldName)
        {
            return true;
        }

#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void __SetCallBack (NWsdkCallBackListenerCallBack callback);
#else
        private static void __SetCallBack(NWsdkCallBackListenerCallBack callback)
        {
        }
#endif
        public void SetCallBack(NWsdkCallBackListener callback)
        {
            _callback = callback;
            __SetCallBack(NWsdkCallBackListenerMethod);
        }
    }
}
