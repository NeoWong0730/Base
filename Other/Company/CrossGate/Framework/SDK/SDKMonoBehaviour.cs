using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SDKMonoBehaviour : MonoBehaviour
{
    public static SDKMonoBehaviour instance;
    void Awake()
    {
        instance = this;
    }

    //单个执行单元（无延迟）
    struct NoDelayedQueueItem
    {
        public Action<object> action;
        public object param;
    }

    //全部执行列表（无延迟）
    List<NoDelayedQueueItem> listNoDelayActions = new List<NoDelayedQueueItem>();


    //单个执行单元（有延迟）
    struct DelayedQueueItem
    {
        public Action<object> action;
        public object param;
        public float time;
    }
    //全部执行列表（有延迟）
    List<DelayedQueueItem> listDelayedActions = new List<DelayedQueueItem>();


    //加入到主线程执行队列（无延迟）
    public static void QueueOnMainThread(Action<object> taction, object param)
    {
        QueueOnMainThread(taction, param, 0f);
    }

    //加入到主线程执行队列（有延迟）
    public static void QueueOnMainThread(Action<object> action, object param, float time)
    {
        if (time != 0)
        {
            lock (instance.listDelayedActions)
            {
                instance.listDelayedActions.Add(new DelayedQueueItem { time = Time.time + time, action = action, param = param });
            }
        }
        else
        {
            lock (instance.listNoDelayActions)
            {
                instance.listNoDelayActions.Add(new NoDelayedQueueItem { action = action, param = param });
            }
        }
    }


    //当前执行的无延时函数链
    List<NoDelayedQueueItem> currentActions = new List<NoDelayedQueueItem>();
    //当前执行的有延时函数链
    List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();

    void Update()
    {
        if (listNoDelayActions.Count > 0)
        {
            lock (listNoDelayActions)
            {
                currentActions.Clear();
                currentActions.AddRange(listNoDelayActions);
                listNoDelayActions.Clear();
            }
            for (int i = 0; i < currentActions.Count; i++)
            {
                currentActions[i].action(currentActions[i].param);
            }
        }

        if (listDelayedActions.Count > 0)
        {
            lock (listDelayedActions)
            {
                currentDelayed.Clear();
                currentDelayed.AddRange(listDelayedActions.Where(d => Time.time >= d.time));
                for (int i = 0; i < currentDelayed.Count; i++)
                {
                    listDelayedActions.Remove(currentDelayed[i]);
                }
            }

            for (int i = 0; i < currentDelayed.Count; i++)
            {
                currentDelayed[i].action(currentDelayed[i].param);
            }
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    #region SDK事件回调

    /// <summary>
    /// 只有同意隐私政策，才能进行接下来的游戏，-- Android暂时不在使用 ios也不在使用
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKInitCallback(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKInitCallback:{0}", result);
     
        if (!result.Equals("error"))
        {
            SDKManager.bInit = true;
            SDKManager.sdk.sChannel = result;
            //HitPointManager.HitPoint("game_sdk_launch"); 已在android/ios sdk中打印
        }
        else
        {
            UI_Box.Create(UseUIBOxType.InitFailure);
        }
    }

    /// <summary>
    /// SDK登入回调
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKLoginCallback(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKLoginCallback:{0}", result);
        string[] strs = result.Split('|');
        if (strs.Length >= 2)
        {
            //0 = UID,1 = Token,2 = Gameid,3 = Channelid
            SDKManager.sdk.sdkUid = strs[0];
            SDKManager.sdk.token = strs[1];
            //sdk.Gameid = strs[2];
            //sdk.Channelid = strs[3];
            SDKManager.MTPUserLogin(0, SDKManager.sdk.sdkUid, "");
        }

        //请求Web服务器列表
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginSucced);

    }


    public void OnSDKWangYiTokenCallBack(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKWangYiTokenCallBack:{0}", result);
        //SDKManager.sdk.WangYiToken = result;
    }



    /// <summary>
    /// SDK登出回调 返回登录界面
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKLogoutCallback(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKLogoutCallback");
        SDKManager.sdk.sdkUid = string.Empty;
        SDKManager.sdk.token = string.Empty;

        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);

        //try
        //{
        //    bool flag = Convert.ToBoolean(result);
        //    if (flag)
        //        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);
        //}
        //catch (System.Exception)
        //{
        //    Debug.LogError("OnSDKLogoutCallback convert fail !!!");
        //    throw;
        //}
    }

    /// <summary>
    /// SDK登出回调
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKLoginCancelByUser(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKLoginCancelByUser");

       // SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginCancel, result);
    }


    public void OnSDKSwitchAccount(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKSwitchAccount:{0}", result);
        string[] strs = result.Split('|');
        if (strs.Length >= 2)
        {
            //0 = UID,1 = Token,2 = Gameid,3 = Channelid
            SDKManager.bSwitchAccount = true;
            SDKManager.sdk.sdkUid = strs[0];
            SDKManager.sdk.token = strs[1];
            //sdk.Gameid = strs[2];
            //sdk.Channelid = strs[3];
        }
    }



    /// <summary>
    /// SDK支付回调
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKPayCallback(string result)
    {

    }
    /// <summary>
    /// SDK退出游戏回调
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKExitGameCallback(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKExitGameCallback");
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKExitGame);
        //SDKManager.AutoCallSDKLogin = true;
    }
    /// <summary>
    /// SDK绑定手机回调
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKBindPhoneCallback(string result)
    {

    }

    /// <summary>
    /// SDK点击返回键退出游戏
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKClickBackToExitGame(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKClickBackToExitGame");
        UI_Box.Create(UseUIBOxType.ClickBackToExitGame);
    }



    public void OnSDKUserInfoRealName(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKUserInfoRealName: " + result);
        try
        {
            bool flag = Convert.ToBoolean(result);
            //SDKManager.SetRealNameStatus(flag);
        }
        catch (System.Exception)
        {
            Debug.LogError("OnSDKUserInfoRealName convert fail !!!");
            throw;
        }

    }



    public void OnSDKPayFinish(string result)
    {
        //支付完成（从三方支付页面返回时回调，不代表支付成功，游戏需要去服务端验证支付结果

        DebugUtil.LogFormat(ELogType.eNone, "OnSDKPayFinish: " + result);
        SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "sdk支付请求完成");
    }


    public void OnSDKPayFailure(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKPayFailure: " + result);
        SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, string.Format("sdk支付请求失败:{0}", result));
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKPayFailure, result);
    }

    public void OnSDKExitReportExtraData(string result)
    {
        //这种强制sdk登出，需要上报玩家角色退出打点上报后，要登出sdk --IOS 不调用
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKExitReportExtraData: " + result);
        SDKManager.ExitGameState exitGameState = SDKManager.ExitGameState.LogoutBackToLogin;
        exitGameState = (SDKManager.ExitGameState)Enum.Parse(typeof(SDKManager.ExitGameState), result);
        SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.onSDKExitReportExtraData, SDKManager.SDKReportState.EXIT, exitGameState);
    }


    public void OnSDKAllinSDKExitApp(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKAllinSDKExitApp:" + result);
#if !USE_OLDSDK
        SDKManager.SDKExitApp();
#endif
    }

    public void OnSDKReportErrorToChannel(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKReportErrorToChannel: " + result);
        int.TryParse(result, out int code);
        SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.onSDKReportErrorToChannel, code);
    }

    public void OnSDKPermissionAgree(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKPermissionAgree: " + result);
        //HitPointManager.HitPoint("game_privacy_agree"); 已在android/ios sdk中打印
        //SDKManager.SDKBuglyInit();
    }

    public void OnSDKAutoRepairOrder(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKAutoRequireOrder: " + result);
        //SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKPayAutoRepairOrder, result);
    }


    public void OnSDKPushNotify(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKPushNotify: " + result);
        UI_Box.Create(UseUIBOxType.PushNotify);
    }


    /// <summary>
    /// 绑定手机送礼包 以后由GM控制活动是否开启
    /// </summary>
    /// <param name="result">true:活动开启，false:活动关闭</param>
    public void OnSDKBindIphoneActivityIsOpen(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKBindIphoneActivityIsOpen: " + result);
        try
        {
            bool flag = Convert.ToBoolean(result);
            //SDKManager.bBindIphoneActivityIsOpen = flag;
        }
        catch (System.Exception)
        {
            Debug.LogError("OnSDKBindIphoneActivityIsOpen convert fail !!!");
            throw;
        }
    }

    /// <summary>
    /// 用户是否绑定手机号
    /// </summary>
    /// <param name="result">1未绑定 2已绑定</param>
    public void OnSDKGetPhoneBindStatus(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKGetPhoneBindStatus: " + result);
        int.TryParse(result, out int code);
        SDKManager.eventEmitter.Trigger<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, code);
    }

    /// <summary>
    /// sdk返回的飘字信息
    /// </summary>
    /// <param name="result"></param>
    public void OnSDKHintMsg(string result)
    {
        DebugUtil.LogFormat(ELogType.eNone, "OnSDKHintMsg: " + result);
        SDKManager.eventEmitter.Trigger<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, result);
    }
  
#endregion
}
