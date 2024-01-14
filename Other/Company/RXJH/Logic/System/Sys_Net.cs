using Framework;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using Net;
// using Packet;
using System;
using Table;

namespace Logic
{
    public class Sys_Net : SystemModuleBase<Sys_Net>, ISystemModuleUpdate, ISystemModuleApplicationPause
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnReconnectStartReq,

            OnReconnectStart,
            OnReconnectResult,
            OnPingNtf,

            OnConnectFail,
            OnServerCrash,
            OnKickOff,
            OnConnectSuccess,
        }

        [Flags]
        public enum EReconnectSwitchType
        {
            Login = 1,
            LevelSwitching = 2,
            CutScene = 4,
            Debug = 8,
        }

        #region 数据段
        private bool _reconnectActive = false;
        private int _reconnectDisable = 0;
        private bool _reconnectPause = false;
        public void DisableReconnect(EReconnectSwitchType switchType, bool disable)
        {
            if (disable)
            {
                _reconnectDisable |= (int)switchType;
            }
            else
            {
                _reconnectDisable &= (~((int)switchType));
            }

            RefreshReconnect();
        }

        private void RefreshReconnect()
        {
            if (_reconnectActive && _reconnectDisable == 0 && !NetClient.Instance.IsConnected)
            {
                StartDefaultReconnect();
            }
            //else if(nReconnectCount > 0)
            //{
            //    //TODO 这里处理不全面
            //    UIManager.CloseUI(EUIID.UI_Reconnection);
            //    nReconnectCount = 0;
            //}
        }

        /// <summary>
        /// 是否自动断线重连
        /// </summary>
        public bool bReconnectActive
        {
            get
            {
                return _reconnectActive && _reconnectDisable == 0;
            }
            set
            {
                _reconnectActive = value;
            }
        }

        public bool bDefaultConnected { get; set; } = false; //纯粹为了静默重连，不显示loading
        private bool bDefaultConnecting = false;
        private readonly float fDefaultReconnect_Interval = 1f; //静默重连间隔时间
        private readonly int nDefualtConnectMax = 2; //三次静默没连上，则正常重连
        private int nDefaultRecCount = 0;

        //public readonly int nMaxReconnectCount = 3;
        public readonly float fReconnect_Interval = 5f;
        public int nReconnectCount { get; private set; } = 0;
        public float fReconnectTime = 0;

        private string strIP;
        private string strPort;

        #endregion
        public override void Init()
        {
            //EventDispatcher.Instance.AddEventListener((ushort)CmdRole.HeartBeatReq, (ushort)CmdRole.HeartBeatRes, this.OnHeartBeatRes, null);
            // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.SysTipNtf, this.OnReceivedServerNotify, CmdSocialSysTipNtf.Parser);
            // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.KickedNtf, this.OnKickedNtf, CmdRoleKickedNtf.Parser);
            // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdError.ErrorMsg, this.OnReceivedErrorCode, ErrorMsg.Parser);
            // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.BanInfoNtf, this.OnRoleBanInfoNtf, CmdRoleBanInfoNtf.Parser);

            NetClient.Instance.AddStateListener(OnStatusChnage);
            NetClient.Instance.AddPingListener(OnPingChnage);
        }

        //HitPointNetWorkEvent netEvent = new HitPointNetWorkEvent();
        private void OnPingChnage(long ping)
        {
            eventEmitter.Trigger(EEvents.OnPingNtf, ping); //毫秒

            // if (ping >= 1500)
            //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1610000001));
            //

            //if (!Sys_Role.Instance.IsGMHitOpen("network"))
            //    return;
            
            //netEvent.delay_time = ping;
            //netEvent.net_address = strIP;
            //netEvent.ping_type = string.Empty;
            //netEvent.port = strPort;
            //netEvent.instance_id = Sys_Login.Instance.selectedServerId;

            //HitPointManager.HitPoint(HitPointNetWorkEvent.Key, netEvent);
        }

        private void OnStatusChnage(NetClient.ENetState from, NetClient.ENetState to)
        {
            DebugUtil.LogFormat(ELogType.eNone, "OnStatusChnage {0} -> {1} _reconnectActive = {2}", from, to, _reconnectActive);

            //DebugUtil.LogError(NetClient.Instance.eNetStatus.ToString());
            if (to == NetClient.ENetState.Connected)
            {
                // Sys_Login.Instance.seriesLoginFailCount = 0;
                // Sys_Login.Instance.PreLoginFailTime = 0;
                //
                // NetMsgUtil.CryptKey = 0;
                //
                // if (Sys_Role.Instance.hasEnterGame)
                // {
                //     InitReconnectParam();
                //
                //     Sys_Role.Instance.ReconnectReq();
                //     this.eventEmitter.Trigger(Sys_Net.EEvents.OnReconnectStartReq);
                //     //UIManager.CloseUI(EUIID.UI_Reconnection);
                // }
                // else
                {
                    OnReLogin();
                }
            }
            else if (to == NetClient.ENetState.ConnectFail)
            {
                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, string.Format("连接游戏服失败"));
                // ++Sys_Login.Instance.seriesLoginFailCount;
                // Sys_Login.Instance.PreLoginFailTime = Framework.TimeManager.GetLocalNow();
                // //UnityEngine.Debug.LogError("=============== 失败次数 " + Sys_Login.Instance.seriesLoginFailCount);
                // if (!_reconnectActive)
                // {
                //     //"网络连接失败，请重试"
                //     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10638));
                //
                //     //"服务器维护"
                //     //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000017));
                // }

                eventEmitter.Trigger<bool>(EEvents.OnConnectFail, _reconnectActive);
            }
            else if (to == NetClient.ENetState.Ready)
            {
                if (_reconnectActive && !_reconnectPause)
                    RefreshReconnect();
            }
        }

        public override void OnLogin()
        {
            base.OnLogin();

            InitReconnectParam();
            //nDefaultRecCount = 0;
        }

        private void InitReconnectParam()
        {
            bReconnectActive = true;
            _reconnectPause = false;
            nReconnectCount = 0;

            bDefaultConnected = false;
            bDefaultConnecting = false;
        }

        public override void OnLogout()
        {
            bReconnectActive = false;
            _reconnectPause = false;
            nReconnectCount = 0;

            bDefaultConnected = false;
            bDefaultConnecting = false;
            nDefaultRecCount = 0;

            base.OnLogout();
        }

        public override void Dispose()
        {
            bReconnectActive = false;
            _reconnectPause = false;

            NetClient.Instance.RemoveStateListener(OnStatusChnage);
            NetClient.Instance.RemovePingListener(OnPingChnage);
            base.Dispose();
        }

        public void OnUpdate()
        {
            //静默重连
            if (_reconnectActive && bDefaultConnecting)
            {
                fReconnectTime += GetUnscaledDeltaTime();
                if (fReconnectTime >= fDefaultReconnect_Interval)
                {
                    if (nDefaultRecCount >= nDefualtConnectMax)
                    {
                        bDefaultConnecting = false;
                        nDefaultRecCount = 0;
                        StartReconnect();
                    }
                    else
                    {
                        //DebugUtil.LogErrorFormat("nDefaultRecCount ={0}  time = {1}", nDefaultRecCount, UnityEngine.Time.time);
                        this.eventEmitter.Trigger(Sys_Net.EEvents.OnReconnectStart);
                        ++nDefaultRecCount;
                        //DebugUtil.LogError("bDefaultConnecting = " + nDefaultRecCount);
                        NetClient.Instance.Connect();
                        fReconnectTime = 0;
                    }
                }
                return;
            }

            if (nReconnectCount > 0 && _reconnectActive)
            {
                fReconnectTime += GetUnscaledDeltaTime();
                if (fReconnectTime >= fReconnect_Interval)
                {
                    ++nReconnectCount;
                    //DebugUtil.LogError("bConnecting = " + nReconnectCount);
                    NetClient.Instance.Connect();
                    fReconnectTime = 0;
                }
            }
            //else
            //{
            //    if (_reconnectActive && (NetClient.Instance.eNetStatus != NetClient.ENetState.Connected || NetClient.Instance.eNetStatus != NetClient.ENetState.Connecting))
            //    {
            //        DebugUtil.LogError(NetClient.Instance.eNetStatus.ToString());
            //        if (!_reconnectPause)
            //            RefreshReconnect();
            //    }
            //}
        }

        /// <summary> 进入后台时间 </summary>
        private int backTime;
        public void OnApplicationPause(bool pause)
        {
            //DebugUtil.LogError(pause.ToString());
            //if (pause)
            //{
            //    NetClient.Instance.Disconnect();
            //}
        }

//         private void OnReceivedServerNotify(NetMsg msg)
//         {
//             CmdSocialSysTipNtf response = NetMsgUtil.Deserialize<CmdSocialSysTipNtf>(CmdSocialSysTipNtf.Parser, msg);
//             ErrorCodeHelper.PushErrorCode(response);
//         }
//
//         private void OnKickedNtf(NetMsg msg)
//         {
//             CmdRoleKickedNtf response = NetMsgUtil.Deserialize<CmdRoleKickedNtf>(CmdRoleKickedNtf.Parser, msg);
//             //(LevelManager.mCurrentLevelType != typeof(LvLogin) && LevelManager.mCurrentLevelType != typeof(LvCreateCharacter))
//             {
//                 // 被踢的时候，广播给外部，不进行断线重连
//                 bReconnectActive = false;
//                 if (response.Reason != 0)
//                 { //resonBan, 服务器走的另一个协议
//                     if (response.Reason == (int)RoleOffReason.CancelQueue)
//                     {
//                         if (LevelManager.mCurrentLevelType == typeof(LvLogin) || LevelManager.mCurrentLevelType == typeof(LvCreateCharacter))
//                         {
//                             NetClient.Instance.Disconnect();
//                         }
//                     }
//                     else if (response.Reason == (int)RoleOffReason.PhoneNotBind)
//                     {
//                         UIManager.CloseUI(EUIID.UI_Server);
//                         this.eventEmitter.Trigger<int>(Sys_Net.EEvents.OnKickOff, response.Reason);
//                     }
//                     else if (response.Reason == (int)RoleOffReason.WrongDevice)
//                     {
//                         if (SDKManager.SDKISEmulator())
//                         {
//                             PromptBoxParameter.Instance.Clear();
//                             PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(8334).words;
//                             PromptBoxParameter.Instance.SetConfirm(true, () =>
//                             {
//                                 Sys_Role.Instance.SDKAccountReportExtraData(SDKManager.SDKReportState.EXIT);
//                                 AppManager.Quit();
//                             });
//                             PromptBoxParameter.Instance.SetCancel(false, null);
//                             UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
//                             //DebugUtil.LogFormat(ELogType.eNone,"OnKickedNtf Reason({0}) {1}", response.Reason.ToString(), "ISEmulator");
//                         }
//                     }
//                     else if (response.Reason == (int)RoleOffReason.DunCheckBan)
//                     {
//                         PromptBoxParameter.Instance.Clear();
//                         PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(8335).words;
//                         PromptBoxParameter.Instance.SetConfirm(true, () =>
//                         {
//                             Sys_Role.Instance.SDKAccountReportExtraData(SDKManager.SDKReportState.EXIT);
//                             AppManager.Quit();
//                         });
//                         PromptBoxParameter.Instance.SetCancel(false, null);
//                         UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
//                     }
//                     else if (response.Reason == (int)RoleOffReason.NoIosbreak)
//                     {
// #if UNITY_IOS
//                         if (SDKManager.sdk.bIsJailbreak == "1")
//                         {
//                             PromptBoxParameter.Instance.Clear();
//                             PromptBoxParameter.Instance.content = "为了您的账户安全，请使用正版系统进行游戏。";// CSVLanguage.Instance.GetConfData(8336).words;
//                             PromptBoxParameter.Instance.SetConfirm(true, () =>
//                             {
//                                 Sys_Role.Instance.SDKAccountReportExtraData(SDKManager.SDKReportState.EXIT);
//                                 AppManager.Quit();
//                             });
//                             PromptBoxParameter.Instance.SetCancel(false, null);
//                             UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
//                         }
// #endif
//                     }
//                     else if (response.Reason != (int)RoleOffReason.Ban)
//                     {
//                         string append = string.Empty;
//                         if (response.Comment != null)
//                         {
//                             append = response.Comment.ToStringUtf8();
//                         }
//
//                         DebugUtil.LogErrorFormat("OnKickedNtf Reason({0}) {1}", response.Reason.ToString(), append);
//                         if (response.Reason != (int)RoleOffReason.Kicked)
//                             UIManager.OpenUI(EUIID.UI_ServerNotify, true, new Tuple<int, string>(response.Reason, append));
//                         // 关闭排队UI
//                         UIManager.CloseUI(EUIID.UI_LoginLineUp);
//                     }
//                 }
//             }
//         }
        
//         private void OnReceivedErrorCode(NetMsg msg)
//         {
//             if (bDefaultConnecting) //静默重连状态，不处理errorcode
//                 return;
//
//             ErrorMsg res = NetMsgUtil.Deserialize<ErrorMsg>(ErrorMsg.Parser, msg);
//
//             uint errorCode = (uint)res.Err;
//             //TODO : 错误表文本最好和游戏文本分开
//             uint languageID = 100000u + errorCode;
//             string content = LanguageHelper.GetErrorCodeContent(languageID);
// #if DEBUG_MODE
//             content = string.Format("ServerError ：{0} ({1})", content, errorCode.ToString());
//             DebugUtil.LogError(content);
// #else
//             if (string.IsNullOrWhiteSpace(content))
//             {
//                 content = errorCode.ToString();
//             }
// #endif
//
//             bool __NeedFilter(uint code)
//             {
//                 return ((uint)ErrorWildBoss.FailToChangeScene == code ||
//                         (uint)ErrorWildBoss.BossNotFoundAfterChangingScene == code ||
//                         (uint)ErrorWildBoss.BossNotInFight == code
//                         );
//             }
//
//             if (!__NeedFilter(errorCode))
//             {
//                 Sys_Hint.Instance.PushContent_Normal(content);
//             }
//
//             //传送失败，玩家可以继续移动
//             if (Sys_Map.Instance.IsTelState)
//             {
//                 Sys_Map.Instance.IsTelState = false;
//                 GameCenter.EnableMainHeroMove(true);
//             }
//
//             if (res.ReqMsgType == (uint)CmdNpc.ResourceEndReq)
//             {
//                 Sys_CollectItem.Instance.eventEmitter.Trigger(Sys_CollectItem.EEvents.OnCollectFaild);
//             }
//
//             if (res.ReqMsgType == (uint)CmdRole.CreateReq)
//             {
//                 System.Collections.Hashtable hashtable = new System.Collections.Hashtable();
//                 hashtable.Add("reason", content);
//                 HitPointManager.HitPoint("game_createrole_fail", hashtable);
//             }
//
//             switch (errorCode)
//             {
//                 case 1033u: //参数错误
//                 case 1034u: //验签失败
//                 case 1035u: //登录签名过期无效,需要重新登录获取最新签名
//                     {
//                         string errorMsg = string.Format("0|登录游戏失败,消息号:{0},错误码:{1}",res.ReqMsgType, errorCode);
//                         SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, errorMsg);
//                         SDKManager.eventEmitter.Trigger(SDKManager.ESDKLoginStatus.OnSDKLoginFail, errorMsg);
//                     }
//                     break;
//                 case 1036u: //没有该服务器 HasNoGamesvr
//                 case 1039u: //没有该服务器 HasNoScenesvr
//                     if (LevelManager.mCurrentLevelType != typeof(LvLogin))
//                         NetClient.Instance.Connect();
//                     else
//                         Disconnect(); //不需要重连
//                     
//                     this.eventEmitter.Trigger(EEvents.OnServerCrash);
//                     break;
//                 case 1041u:
//                 case 1919u:
//                 case 2828u:
//                     {
//                         NetClient.Instance.Connect();
//                     }
//                     break;
//                 case (uint)ErrorWildBoss.FailToChangeScene:
//                     {
//                         PromptBoxParameter.Instance.Clear();
//                         PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(4157000025).words;
//                         PromptBoxParameter.Instance.SetConfirm(true, null);
//                         PromptBoxParameter.Instance.SetCancel(false, null);
//                         UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
//                     }
//                     break;
//                 case (uint)ErrorWildBoss.BossNotFoundAfterChangingScene:
//                 case (uint)ErrorWildBoss.BossNotInFight:
//                     {
//                         PromptBoxParameter.Instance.Clear();
//                         PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(4157000026).words;
//                         PromptBoxParameter.Instance.SetConfirm(true, null);
//                         PromptBoxParameter.Instance.SetCancel(false, null);
//                         UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
//                     }
//                     break;
//                 default:
//                     break;
//             }
//         }

        // public void OnRoleBanInfoNtf(NetMsg msg)
        // {
        //     CmdRoleBanInfoNtf res = NetMsgUtil.Deserialize<CmdRoleBanInfoNtf>(CmdRoleBanInfoNtf.Parser, msg);
        //
        //     if (res.BanType == (uint)BanType.Role)
        //     {
        //         //1000078
        //         //当前角色已被冻结，解冻时间{0}，如有疑问请联系客服
        //         //11862
        //         //"yyyy年-MM月-dd日 HH时:mm分:ss秒"
        //         if (LevelManager.mCurrentLevelType == typeof(LvPlay))
        //         {
        //             Framework.TimeManager.CorrectServerTimeZone(res.Timezone);
        //             DateTime dateTime = Sys_Time.ConvertToLocalTime(res.EndTime);
        //             string timePoint = dateTime.ToString(LanguageHelper.GetTextContent(11862));
        //             PromptBoxParameter promptBoxParameter = PromptBoxParameter.Instance;
        //             promptBoxParameter.Clear();
        //             promptBoxParameter.content = LanguageHelper.GetTextContent(1000078, timePoint);
        //             promptBoxParameter.SetConfirm(true, OnExitGame);
        //             UIManager.OpenUI(EUIID.UI_PromptBox, true, promptBoxParameter);
        //         }
        //         else
        //         {
        //             Framework.TimeManager.CorrectServerTimeZone(res.Timezone);
        //             DateTime dateTime = Sys_Time.ConvertToLocalTime(res.EndTime);
        //             string timePoint = dateTime.ToString(LanguageHelper.GetTextContent(11862));
        //             //string hint = LanguageHelper.GetTextContent(1000078, timePoint);
        //             //Sys_Hint.Instance.PushContent_Normal(hint);
        //
        //             //功能108330 角色封停 飘字换弹框
        //             PromptBoxParameter promptBoxParameter = PromptBoxParameter.Instance;
        //             promptBoxParameter.Clear();
        //             promptBoxParameter.content = LanguageHelper.GetTextContent(1000078, timePoint);
        //             promptBoxParameter.SetConfirm(true, null);
        //             UIManager.OpenUI(EUIID.UI_PromptBox, true, promptBoxParameter);
        //
        //         }
        //     }
        //     else if (res.BanType == (uint)BanType.Chat)
        //     {
        //         //102015
        //         //您已被禁言，解禁时间{0}
        //         DateTime dateTime = Sys_Time.ConvertToLocalTime(res.EndTime);
        //         string timePoint = dateTime.ToString(LanguageHelper.GetTextContent(11862));
        //         string hint = LanguageHelper.GetTextContent(102015, timePoint);
        //         Sys_Hint.Instance.PushContent_Normal(hint);
        //     }
        // }

        public bool Connect(string ip, int port)
        {
            strIP = ip;
            strPort = port.ToString();
            //long now = Framework.TimeManager.GetLocalNow();
            // DebugUtil.LogFormat(ELogType.eHeartBeat, "now: {0} preFail:{1} failCount{2}", now.ToString(),
            //     Sys_Login.Instance.PreLoginFailTime.ToString(), Sys_Login.Instance.seriesLoginFailCount.ToString());
            //
            // if (Sys_Login.Instance.seriesLoginFailCount >= 3)
            // {
            //     double preLoginTime = Sys_Login.Instance.PreLoginFailTime;
            //     if (now - preLoginTime <= Sys_Login.Instance.FirstWaitTime)
            //     {
            //         Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11635, Sys_Login.Instance.FirstWaitTime.ToString()));
            //         return false;
            //     }
            //     else
            //     {
            //         Sys_Login.Instance.seriesLoginFailCount = 0;
            //     }
            // }

            NetClient.Instance.Connect(ip, port);
            return true;
        }
        
        public void Disconnect()
        {
            bReconnectActive = false;
            NetClient.Instance.Disconnect();
        }

        public void StartDefaultReconnect()
        {
            //DebugUtil.LogError("StartDefaultReconnect");
            bDefaultConnecting = true;
            //++nDefaultRecCount;
            //Sys_Fight.Instance.OnReconnect();
            //this.eventEmitter.Trigger(Sys_Net.EEvents.OnReconnectStart);
            //NetClient.Instance.Connect();
            nDefaultRecCount = 0;
            fReconnectTime = 0;
            //DebugUtil.LogErrorFormat("time = {0}", UnityEngine.Time.time);
        }

        public void StartReconnect()
        {
            if (nReconnectCount > 0)
                return;

            // UIManager.OpenUI(EUIID.UI_Reconnection);
            // Sys_Fight.Instance.OnReconnect();
            this.eventEmitter.Trigger(Sys_Net.EEvents.OnReconnectStart);

            _reconnectPause = false;
            ++nReconnectCount;
            //DebugUtil.LogError("nReconnectCount = " + nReconnectCount);
            NetClient.Instance.Connect();
            fReconnectTime = 0;
        }

        private void OnReLogin()
        {
            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, string.Format("连接游戏服成功"));
            
            // if (SDKManager.sdk.IsHaveSdk)
            // {
            //     Sys_Role.Instance.ReqLogin(SDKManager.GetUID(), Sys_Login.Instance.selectedRoleId, Sys_Login.Instance.selectedServerId);
            // }
            // else
            // {
            //     Sys_Role.Instance.ReqLogin(Sys_Login.Instance.Account, Sys_Login.Instance.selectedRoleId, Sys_Login.Instance.selectedServerId);
            // }
            eventEmitter.Trigger(EEvents.OnConnectSuccess);
        }

        public void OnPauseReconnect()
        {
            nReconnectCount = 0;
            fReconnectTime = 0f;
            _reconnectPause = true;
        }

        public void ReconnectResult(bool result)
        {
            // if (result)
            // {
            //     UIManager.CloseUI(EUIID.UI_Reconnection, true);
            //     nReconnectCount = 0;
            //
            //     bDefaultConnected = bDefaultConnecting;
            //     bDefaultConnecting = false;
            //     nDefaultRecCount = 0;
            //
            //     //记录新手路径埋点
            //     Sys_Role.Instance.CheckNewTrace();
            // }
            // else
            // {
            //     UIManager.CloseUI(EUIID.UI_Reconnection, true);
            //     nReconnectCount = 0;
            //
            //     bDefaultConnected = bDefaultConnecting;
            //     bDefaultConnecting = false;
            //     nDefaultRecCount = 0;
            //
            //     PromptBoxParameter.Instance.Clear();
            //     PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(1012007);
            //     PromptBoxParameter.Instance.SetCancel(false, null);
            //     PromptBoxParameter.Instance.SetConfirm(true, () =>
            //     {
            //         Sys_Net.Instance.OnPauseReconnect();
            //         Sys_Role.Instance.ExitGameReq();
            //         //SDKManager.SDKLogout(false);
            //     });
            //     UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
            // }

            eventEmitter.Trigger(EEvents.OnReconnectResult, result);
        }

        private void OnExitGame()
        {
            // Sys_Role.Instance.ExitGameReq();
        }
    }
}
