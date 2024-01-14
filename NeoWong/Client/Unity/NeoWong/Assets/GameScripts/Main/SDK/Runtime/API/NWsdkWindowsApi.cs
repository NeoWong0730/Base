using System;
using System.Reflection;
using UnityEngine;

namespace GameMain.NWsdk
{
    public class NWsdkWindowsApi : INWsdkApi
    {
        private object Invoke(object obj, Type type, string method, params object[] objects)
        {
            MethodInfo methodInfo = type.GetMethod(method,
                BindingFlags.Static |
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.Public);

            if (method == null)
                return default;

            return methodInfo.Invoke(obj, objects);
        }

        public void CallPlugin(string pluginName, string methodName, params object[] parameters)
        {
            Invoke(this, GetType(), methodName, parameters);
        }

        public T CallPlugin<T>(string pluginName, string methodName, params object[] parameters)
        {
            object ret = Invoke(this, GetType(), methodName, parameters);
            
            if (ret == null)
                ret = default(T);

            return (T)ret;
        }

        public bool IsExistPlugin(string pluginName)
        {
            return true;
        }

        public bool IsExistField(string pluginName, string fieldName)
        {
            return true;
        }

        public bool IsExistMethod(string pluginName, string methodName)
        {
            return true;
        }

        public void SetCallBack(NWsdkCallBackListener callback)
        {
        }
    }
}