using UnityEngine;

namespace GameMain.NWsdk
{
    public class NWsdkApi
    {
        public const string PLATFORMNAME = "PlatformProxy";
#if UNITY_ANDROID
        private static INWsdkApi api = new NWsdkAndroidApi();
#elif UNITY_IOS
        private static INWsdkApi api = new NWsdkiOSApi();
#else
        private static INWsdkApi api = new NWsdkWindowsApi();
#endif

        static NWsdkApi()
        {
            SetCallBack(new NWsdkCallBackListener(new NWsdkCallBack()));
        }

        public static void SetCallBack(NWsdkCallBackListener callback)
        {
            api.SetCallBack(callback);
        }

        public static string GetConfig(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            return CallPluginT<string>(PLATFORMNAME, "getConfig", key);
        }

        public static void CallPlugin(string pluginName, string methodName, params object[] parameters)
        {
            api.CallPlugin(pluginName, methodName, parameters);
        }

        public static T CallPluginT<T>(string pluginName, string methodName, params object[] parameters)
        {
            return api.CallPlugin<T>(pluginName, methodName, parameters);
        }

        public static object CallPluginT(string type, string pluginName, string methodName, params object[] parameters)
        {
            if (type == "int")
                return CallPluginT<int>(pluginName, methodName, parameters);
            else if (type == "float")
                return CallPluginT<float>(pluginName, methodName, parameters);
            else if (type == "long")
                return CallPluginT<long>(pluginName, methodName, parameters);
            else if (type == "bool")
                return CallPluginT<bool>(pluginName, methodName, parameters);
            else if (type == "string")
                return CallPluginT<string>(pluginName, methodName, parameters);
            else if (type == "double")
                return CallPluginT<double>(pluginName, methodName, parameters);
            else if (type == "object")
                return CallPluginT<AndroidJavaObject>(pluginName, methodName, parameters);

            return null;
        }

        public bool IsExistPlugin(string pluginName)
        {
            return api.IsExistPlugin(pluginName);
        }

        public bool IsExistField(string pluginName, string fieldName)
        {
            return api.IsExistField(pluginName, fieldName);
        }

        public bool IsExistMethod(string pluginName, string methodName)
        {
            return api.IsExistMethod(pluginName, methodName);
        }
    }
}