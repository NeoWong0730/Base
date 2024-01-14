using NWFramework;

namespace GameMain.NWsdk
{
    public interface INWsdkCallBackListener
    {
        void OnCallBack(string eventName, string jsonMsg);
    }

    public class  NWsdkCallBack : INWsdkCallBackListener
    {
        public void OnCallBack(string eventName, string jsonMsg) { }
    }

#if UNITY_ANDROID
    public class NWsdkCallBackListener : AndroidJavaProxy
    {
        private INWsdkCallBackListener callback;
        public NWsdkCallBackListener(INWsdkCallBackListener callback) : base("com.nwsdk.sdk.NWsdkCallBackListener")
        {
            this.callback = callback;
        }

        public void OnCallBack(string eventName, string msg)
        {
            Log.Info($"[NWsdkCallBackListener] eventName = {eventName}  jsonMsg = {jsonMsg}");
            UnityDispatcher.PostTask(() =>
            {
                callback?.OnCallBack(eventName, jsonMsg);
            });
        }
    }
#else
    public class NWsdkCallBackListener
    {
        private INWsdkCallBackListener callback;
        public NWsdkCallBackListener(INWsdkCallBackListener callback)
        {
            this.callback = callback;
        }

        public void OnCallBack(string eventName, string jsonMsg)
        {
            Log.Info($"[NWsdkCallBackListener] eventName = {eventName}  jsonMsg = {jsonMsg}");

            UnityDispatcher.PostTask(() =>
            {
                callback?.OnCallBack(eventName, jsonMsg);
            });
        }
    }
#endif
}