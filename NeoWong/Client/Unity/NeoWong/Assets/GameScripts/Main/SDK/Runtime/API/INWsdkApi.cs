using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameMain.NWsdk
{
    public interface INWsdkApi
    {
        void CallPlugin(string pluginName, string methodName, params object[] parameters);

        T CallPlugin<T>(string pluginName, string methodName, params object[] parameters);

        bool IsExistPlugin(string pluginName);

        bool IsExistField(string pluginName, string fieldName);

        bool IsExistMethod(string pluginName, string methodName);

        void SetCallBack(NWsdkCallBackListener callback);
    }
}