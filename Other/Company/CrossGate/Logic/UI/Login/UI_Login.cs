using System;
using System.IO;
using Framework;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_Login : UIBase, UI_Login_Layout.IListener
    {
        public static bool toBtn2 = false;

        public UI_Login_Layout Layout;
        public AppHooker hooker;

        private bool isSelectState = false;

        private float m_WaitCallBacktimer = 0;

        protected override void OnLoaded()
        {
            this.Layout = new UI_Login_Layout();
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);

            this.hooker = this.gameObject.AddComponent<AppHooker>();
            //this.Layout.SecondButton.gameObject.SetActive(false);
            //this.Layout.FirstButton.gameObject.SetActive(true);
            //Layout.Button_Logout_Button.gameObject.SetActive(false);

            this.Layout.Input_Account_InputField.text = Sys_Login.Instance.Account;

            //int key = PlayerPrefs.GetInt("AcceptAgreement", 0);
            //var show = key == 0 ? false : true;
            //this.Layout.toggleRead.isOn = show;
            //this.Layout.toggleRead.gameObject.SetActive(!show);
            //this.Layout.readtip.gameObject.SetActive(!show);

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Login.Instance.eventEmitter.Handle(Sys_Login.EEvents.OnSelectServerChanged, this.OnSelectServerChange, toRegister);
            Sys_Login.Instance.eventEmitter.Handle(Sys_Login.EEvents.OnServerListChanged, this.OnServerListChanged, toRegister);
            //Sys_Login.Instance.eventEmitter.Handle<string>(Sys_Login.EEvents.OnLoginFail, this.OnLoginFail, toRegister);
            SDKManager.eventEmitter.Handle<String>(SDKManager.ESDKLoginStatus.OnSDKLoginFail, this.OnSDKLoginFail, toRegister);
        }
        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Login.Instance.eventEmitter.Handle<bool>(Sys_Login.EEvents.OnLoginGot, this.OnGetServerList, toRegister);

            //SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKExitGame, this.onExitGameNotify, toRegister);
            SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKLoginSucced, this.OnSDKLoginSucced, toRegister);
            SDKManager.eventEmitter.Handle<String>(SDKManager.ESDKLoginStatus.OnSDKLogout, this.OnSDKLogout, toRegister);
            //SDKManager.eventEmitter.Handle<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, this.OnSDKHintMsg, toRegister);
            //Sys_Net.Instance.eventEmitter.Handle<int>(Sys_Net.EEvents.OnKickOff,this.OnBindPhoneKickOff, toRegister);
            SDKManager.eventEmitter.Handle<int>(SDKManager.ESDKLoginStatus.OnSDKBindIphoneStatus, OnSDKBindIphoneStatus, toRegister);
            if (this.hooker != null)
            {
                if (toRegister)
                    this.hooker.onNetworkStatusChanged += this.OnNetworkStatusChanged;
                else
                {
                    if (this.hooker.onNetworkStatusChanged != null)
                        this.hooker.onNetworkStatusChanged -= this.OnNetworkStatusChanged;
                }
            }
        }

        protected override void OnOpened()
        {
            if (toBtn2 || SDKManager.GetEnableSwitchAccount())
            {
                toBtn2 = false;
                isSelectState = true;
                SDKManager.bSwitchAccount = false;
                this.Layout.FirstButton.gameObject.SetActive(false);
                this.Layout.SecondButton.gameObject.SetActive(true);
                UpdatePreHeatActivityState();
                Sys_Login.Instance.LoginWeb();
            }
            else
            {
                isSelectState = false;
                this.Layout.FirstButton.gameObject.SetActive(true);
                this.Layout.SecondButton.gameObject.SetActive(false);
            }
        }
        protected override void OnShow()
        {

            //使用sdk登录，关闭账号密码的输入
            if (SDKManager.sdk.IsHaveSdk)
            {
                this.Layout.Input_Account_InputField.gameObject.SetActive(false);
                this.Layout.Input_Password_InputField.gameObject.SetActive(false);

                //登出按钮，根据渠道有无登出功能进行屏蔽
                //if (!SDKManager.channelFlag.ContainsKey(SDKManager.GetChannel())) {
                //    this.Layout.Button_Logout_Button.gameObject.SetActive(false);
                //}

                bool isEnable = SDKManager.SDKApiAvailable("logoff");
                this.Layout.Button_Logout_Button.gameObject.SetActive(isEnable);

                //渠道 （客服，扫码）进行屏蔽
                bool officialFlag = SDKManager.GetChannel().Equals(SDKManager.officialChannel);
                this.Layout.Btn_Geren_Button.gameObject.SetActive(officialFlag);
                if (officialFlag)
                {
#if UNITY_IPHONE
                    this.Layout.Button_ScanQRCode_Button.gameObject.SetActive(true);
#else
                    this.Layout.Button_ScanQRCode_Button.gameObject.SetActive(false);
#endif
                }
                else
                {
                    this.Layout.Button_CustomService_Button.gameObject.SetActive(false);
                    this.Layout.Button_ScanQRCode_Button.gameObject.SetActive(false);
                }
            }

            string ext = string.Empty;
            if ((VersionHelper.eChannelFlags & EChannelFlags.DisplayName) == EChannelFlags.DisplayName)
            {
                // Todo
                ext = "(" + VersionHelper.ChannelName + ")";
            }

            if (VersionHelper.eHotFixMode == EHotFixMode.Test)
            {
                // Todo
                ext += "(更新测试)";
            }

            //版本号 
            string appversion = SDKManager.GetAppVersion();
            TextHelper.SetText(this.Layout.Text_Version_Text, 1000000, appversion);

            //资源号
            if (!string.Equals(VersionHelper.StreamingBuildVersion, VersionHelper.PersistentBuildVersion, StringComparison.Ordinal))
            {
                string[] strStream = VersionHelper.StreamingBuildVersion.Split('.');
                int streamversion = int.Parse(strStream[0]) * 1000000 + int.Parse(strStream[1]) * 10000;
                string[] strPersist = VersionHelper.PersistentBuildVersion.Split('.');
                int Persistversion = int.Parse(strPersist[0]) * 1000000 + int.Parse(strPersist[1]) * 10000;
                if (Persistversion < streamversion)
                    TextHelper.SetText(this.Layout.Text_Resources_Text, 1000001, string.Format("{0}.{1}{2}", VersionHelper.StreamingBuildVersion, VersionHelper.StreamingAssetVersion, ext));
            }
            else
            {
                TextHelper.SetText(this.Layout.Text_Resources_Text, 1000001, string.Format("{0}.{1}{2}", VersionHelper.PersistentBuildVersion, VersionHelper.PersistentAssetVersion, ext));
            }

            //协议号
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(529);
            TextHelper.SetText(this.Layout.Text_Protocal_Text, 1000042, cSVParamData.str_value);

            this.RefreshServer();

            //this.isSelectState = false;
        }

        protected override void OnUpdate()
        {
            if (this.isSelectState)
            {
                this.m_WaitCallBacktimer += Time.deltaTime;
                if (this.m_WaitCallBacktimer > 15)
                {
                    this.m_WaitCallBacktimer = 0;
                    this.isSelectState = false;
                    this.Layout.FirstButton.gameObject.SetActive(true);
                    this.Layout.SecondButton.gameObject.SetActive(false);
                }
            }

            if (Sys_Login.Instance.NeedRefresh())
            {
                Sys_Login.Instance.GetServerList();
            }
        }

        private ServerEntry serverData;
        private ServerState serverState;
        private void RefreshServer()
        {
            this.serverData = Sys_Login.Instance.mSelectedServer;
            string serverName = string.Empty;
            if (this.serverData != null)
            {
                serverName = this.serverData.mServerInfo.ServerName;
                var color = this.serverData.mServerInfo.Color.ToStringUtf8();
                this.serverData.GetState(out uint stateIcon, out uint stateText);
                this.Layout.ServerName_Text.text = serverName;
                ImageHelper.SetIcon(this.Layout.State_Icon_Image, stateIcon, true);
                TextHelper.SetText(this.Layout.State_Text, stateText);
            }
        }

        #region UICallback

        public void OnLogin_ButtonClicked()
        {
            //if (!this.Layout.toggleRead.isOn) {
            //    // "请阅读以及同意协议"
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10714));
            //    return;
            //}
            if (!SDKManager.sdk.IsHaveSdk)
            {
                string account = this.Layout.Input_Account_InputField.text;
                if (string.IsNullOrEmpty(account))
                {
                    // "No Account"
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(100015));
                    return;
                }
                if (Sys_RoleName.Instance.HasBadNames(account))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
                    return;
                }
            }

            this.LoginByNetWork();
        }


        public void LoginByNetWork()
        {
            if (NetworkHelper.IsWanOrLanOpen())
            {
                HitPointManager.HitPoint("game_btn_login");

                this.isSelectState = true;
                this.Layout.FirstButton.gameObject.SetActive(false);

                if (SDKManager.sdk.IsHaveSdk)
                    SDKManager.SDKLogin();
                else
                {
                    if (Sys_Login.Instance.mSelectedServer != null)
                        this.Layout.SecondButton.gameObject.SetActive(true);

                    UIManager.OpenUI(EUIID.UI_BlockClickHttp, false, 2f);
                    Sys_Login.Instance.LoginWeb();
                }
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(10709).words;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    //重新走登录的流程
                    this.LoginByNetWork();

                }, 10710);
                PromptBoxParameter.Instance.SetCancel(false, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        // 返回一般是切换账号，所以这里Clear当前账号请求的 目录服信息
        // 防止后者看到前者的Server信息，尤其是白名单
        public void OnLogout_ButtonClicked()
        {
            this.isSelectState = false;
            this.Layout.SecondButton.gameObject.SetActive(false);
            this.Layout.Login_Animator.speed = 5f;
            this.Layout.FirstButton.gameObject.SetActive(true);

            Sys_Login.Instance.ClearServerList();


            if (SDKManager.sdk.IsHaveSdk)
            {
                //1.登出sdk
                SDKManager.SDKLogout();

                //2.自动拉起sdk的登录
                SDKManager.SDKLogin();
            }
        }
        public void OnHrefClick(string arg)
        {
            if (arg == "1")
            {
                this.UserAgreement_ButtonClicked();
            }
            else if (arg == "2")
            {
                this.PrivacyPolicy_ButtonClicked();
            }
        }
        public void OnValueChanged(bool status)
        {
            PlayerPrefs.SetInt("AcceptAgreement", status ? 1 : 0);
            //this.Layout.toggleRead.gameObject.SetActive(!status);
            //this.Layout.readtip.gameObject.SetActive(!status);
        }

        public void OnStart_ButtonClicked()
        {
            /*同意隐私政策和用户协议相关 已经有快手做了*/
            //if (!this.Layout.toggleRead.isOn) {
            //    // Todo
            //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10714));
            //    return;
            //}

            if (!NetworkHelper.IsWanOrLanOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10638));
                return;
            }

            if (this.serverData != null)
            {
                if (Sys_Login.Instance.HasMainTitle(this.serverData, true))
                {
                    return;
                }

#if UNITY_EDITOR && SKIP_SDK_Login
                Sys_Net.Instance.Connect(SkipSDKLogin.Instance.ServerIp, SkipSDKLogin.Instance.ServerPort);//"82.156.32.70", 11001

#else
                string ServerIp = this.serverData.mServerInfo.ServerIp;
                int ServerPort = (int)this.serverData.mServerInfo.ServerPort;
                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, string.Format("开始连接游戏服, ServerIp:{0}, Serverport:", ServerIp, ServerPort));
                Sys_Net.Instance.Connect(ServerIp, ServerPort);
#endif

                HitPointManager.HitPoint("game_btn_entergame");
                UIManager.OpenUI(EUIID.UI_BlockClickNetwork, false, 10f);
            }
            else
            {
                // "请先选择服务器"
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(100016));
            }
        }

        public void OnSelectServer_ButtonClicked()
        {
            HitPointManager.HitPoint("game_server_confirm");
            UIManager.OpenUI(EUIID.UI_Server);
        }

        public void OnBoard_ButtonClicked()
        {
            Sys_ExternalNotice.Instance.GetExternalNotice();
        }

        public void PrivacyPolicy_ButtonClicked()
        {
            //隐私政策：https://www.gamekuaishou.com/privacy?channel=**&appid=****
            SDKManager.SDKPrivacyPolicy();
        }

        public void UserAgreement_ButtonClicked()
        {
            //用户协议：https://www.gamekuaishou.com/policy?channel=**&appid=****
            SDKManager.SDKUserAgreement();
        }

        public void OnFixHotUpdate_ButtonClicked()
        {
            //修复按钮 -- 前提：EAppState.Game 退出逻辑需要完善
            //Framework.AppManager.ClearCachingData();
            //Framework.AppManager.NextAppState = Framework.EAppState.CheckVersion;

            //说明是首包，不用检测 沙盒路径热更新资源正确与否
            if (string.Equals(VersionHelper.StreamingAssetVersion, VersionHelper.PersistentAssetVersion, StringComparison.Ordinal))
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.title = LanguageHelper.GetTextContent(2106003);
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.TitleText;
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2106007).words; //"资源没有热更，不需要修复";
                PromptBoxParameter.Instance.SetConfirm(true, null);//2106001
                PromptBoxParameter.Instance.SetCancel(true, null);//2106002
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.title = LanguageHelper.GetTextContent(2106003);
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.TitleText;
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2106000).words;//"补丁缺失或其他显示异常等原因导致无法登陆时，可尝试点击下方“开始修复”按钮,随后重新启动游戏";
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    this.FixClient();
                });//2106001
                PromptBoxParameter.Instance.SetCancel(true, null);//2106002

                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void OnScanQRCode_ButtonClicked()
        {
            SDKManager.SDKScanQRCode();
        }
        public void FixClient()
        {

            //修复按钮 -- 前提：EAppState.Game 退出逻辑需要完善,这个完善成本太大
            //Framework.AppManager.ClearCachingData();
            //Framework.AppManager.NextAppState = Framework.EAppState.HotFix;

            //获取对应版本远端的热更文件信息，校验一遍本地的资源 md5码，不一致的删除
            HotFixStateManager.Instance.CheckPersistentAssetMd5(CheckMD5AssetList);
        }

        public void OnCustomService_ButtonClicked()
        {
            SDKManager.SDKCustomService();
        }


        public void OnXieYi_ButtonClicked()
        {
            SDKManager.SDKSDKWarmTips();
        }

        public void OnGeRen_ButtonClicked()
        {
            SDKManager.SDKOpenUserCenter();
        }
        public void OnBtnHuiguifuliClick()
        {
            UIManager.OpenUI(EUIID.UI_BackWalfare);
        }
        public void OnRightAge_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_PrompBox_Long, false);
        }

        public void OnAccount_InputFieldValueChanged(string arg)
        {
            string account = this.Layout.Input_Account_InputField.text.Trim();
            Sys_Login.Instance.SetAccount(account);
        }
        public void OnPassword_InputFieldValueChanged(string arg) { }

        public void OnBtnPreHeatClick()
        {
            //按钮点击的埋点
            UIManager.HitGameKeyPoint("BtnPreHeatClick");
            Sys_PreHeatActivity.Instance.JumpToPreHeatActivityPage();
        }

        private void UpdatePreHeatActivityState()
        {
            if (Layout.Btn_PreHeat_Button != null)
            {
                if (SDKManager.sdk.IsHaveSdk)
                {
                    if (Sys_PreHeatActivity.Instance.CheckPreHeatActivityIsOpen())
                    {
                        bool isShow = Sys_PreHeatActivity.Instance.JumpToPreHeatActivityPage();
                        Layout.Btn_PreHeat_Button.gameObject.SetActive(isShow);
                        if (isShow)
                        {
                            //按钮展示的埋点
                            UIManager.HitGameKeyPoint("PreHeatButtonShow");
                        }
                    }
                    else
                    {
                        Layout.Btn_PreHeat_Button.gameObject.SetActive(false);
                    }
                }
            }
        }
        #endregion UICallback

        #region SystemEvent
        private void OnGetServerList(bool success)
        {
            DebugUtil.LogFormat(ELogType.eNone, string.Format("OnLoginGot:{0} {1}", this.isSelectState, success));

            if (this.isSelectState && success)
            {
                this.isSelectState = false;
                this.Layout.FirstButton.gameObject.SetActive(false);
                this.Layout.SecondButton.gameObject.SetActive(true);
            }
            else
            {
                this.isSelectState = false;
                this.Layout.FirstButton.gameObject.SetActive(true);
                this.Layout.SecondButton.gameObject.SetActive(false);
            }
        }


        private void OnResetSDKLoginUIStatus()
        {
            this.isSelectState = false;
            this.Layout.SecondButton.gameObject.SetActive(false);
            this.Layout.Login_Animator.speed = 5f;
            this.Layout.FirstButton.gameObject.SetActive(true);

            Sys_Login.Instance.ClearServerList();

            UIManager.CloseUI(EUIID.UI_ExternalNotice);
            UIManager.CloseUI(EUIID.UI_Server);
        }

        private void OnServerListChanged()
        {
            this.RefreshServer();
        }

        private void OnSelectServerChange()
        {
            this.RefreshServer();
        }

        private void OnSDKLoginFail(string errorMsg)
        {
            Debug.LogError(errorMsg);
            this.isSelectState = false;
            this.Layout.SecondButton.gameObject.SetActive(false);
            this.Layout.Login_Animator.speed = 5f;
            this.Layout.FirstButton.gameObject.SetActive(true);

            Sys_Login.Instance.ClearServerList();
            UIManager.CloseUI(EUIID.UI_ExternalNotice);
            UIManager.CloseUI(EUIID.UI_Server);

            if (SDKManager.sdk.IsHaveSdk)
                SDKManager.SDKLogout();

            int errorCode = 0;
            string msg = errorMsg;
            if (errorMsg.Contains("|"))
            {
                int.TryParse(errorMsg.Split('|')[0], out errorCode);
                msg = errorMsg.Split('|')[1];
            }

            switch (errorCode)
            {
                case 0: Sys_Hint.Instance.PushContent_Normal(msg); break;
                case 2002:
                    if (msg.Contains("100202111"))
                    {
                        Sys_Hint.Instance.PushContent_Normal("当前账号密码输入错误次数过多，为了您的账号安全，请24小时后再尝试重新登录");
                    }
                    else if (msg.Contains("100201002"))
                    {
                        Sys_Hint.Instance.PushContent_Normal("当前设备已被撤销账号授权，禁止自动登录");
                    }
                    else if(msg.Contains("100216002"))
                    {
                        Sys_Hint.Instance.PushContent_Normal("当前登录的账号未获得测试资");
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(msg);
                    }
                    break;
                case 2003: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2001).words); break;//登录超时
                case 2004: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2002).words); break;//"没有登录"
                case 2005: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2003).words); break;//"登出成功"
                case 2006: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2004).words); break;//"登出失败"
                case 2007: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2005).words); break;//登录取消
                case 2009: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2006).words); break;//"切换登录取消"
                case 2010: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2007).words); break;//"登录服务异常"
                case 2011: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2008).words); break;//切换账号失败或者未获取相关权限
                case 2012: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2009).words); break;//绑定失败
                case 2013: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2010).words); break;//"注销账号时，取消注销返回";
                case 2014: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2011).words); break;//"注销完成后，重新登录时，用户触发取消登录"
                case 2100: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2012).words); break;//"之前没有账户"
                case 2101: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2013).words); break;//无账号信息，需要登录
                case 2102: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2014).words); break;//当前系统不支持此登录方式
                case 2103: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2015).words); break;//"GameCenter需要重新授权"
                case 2104: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2016).words); break;//不支持的三方授权形式"
                case 2105: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2017).words); break;//不支持GuidedAccess
                case 2106: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2018).words); break;//操作被用户取消
                case 2107: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2019).words); break;//不支持的礼包活动
                case 2108: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2020).words); break;//未开启礼包活动
                case 2109:
                case 2110: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2021).words); break;//实名认证身份证信息输入错误
                case 2111: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2022).words); break;//实名认证身份证信息网络错误
                case 2112: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2023).words); break;//实名认证二次授权失败
                case 2113: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2024).words); break;//实名认证身份证信息其他错误
                case 2114: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2025).words); break;//用户触发防沉迷无法登录
                case 2115: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2026).words); break;//账号已提交注销
                case 2116: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2027).words); break;//服务器内部错误
                case 2117: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2028).words); break;//授权失败
                case 2118: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2029).words); break;//绑定错误，游戏账号已绑定其他快手账号
                case 2119: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2030).words); break;//绑定错误,快手账号已绑定其他游戏账号
                case 2120: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2031).words); break;//登录账号不存在
                case 2121: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2032).words); break;//登录密码错误
                case 2122: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2033).words); break;//token 内容错误
                case 2123: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2034).words); break;//token 过期
                case 2124: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2035).words); break;//撤销设备授权
                case 2125: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2036).words); break;//账号已经注销
                case 2126: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2037).words); break;//需要继续休息
                case 2127: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2038).words); break;//没有完成实名认证导致登录失败
                case 2128: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2039).words); break;//用户被封禁
                case 2129: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2040).words); break;//邮箱格式错误
                case 2130: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2041).words); break;//邮箱未绑定账户
                case 2131: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2042).words); break;//验证码发送失败
                case 2132: Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(2043).words); break;//邮箱验证调用异常，不代表验证码不对
                default: Sys_Hint.Instance.PushContent_Normal(string.Format("登录相关错误码：({0}),{1}", errorCode, msg)); break;
            }
        }
        private void OnSDKLoginSucced()
        {
            this.isSelectState = true;
            //this.Layout.FirstButton.gameObject.SetActive(false);
            if (Sys_Login.Instance.mSelectedServer != null)
                this.Layout.SecondButton.gameObject.SetActive(true);

            UpdatePreHeatActivityState();

            DebugUtil.LogFormat(ELogType.eSdk, "OnSDKLoginSucced:{0}", "登录成功，执行回调");
            HitPointManager.HitPoint("game_sdk_login_success");

            UIManager.OpenUI(EUIID.UI_BlockClickHttp, false, 2f);
            Sys_Login.Instance.LoginWeb();
            Sys_ExternalNotice.Instance.GetExternalNotice();
        }

        private void OnSDKLogout(string errorMsg)
        {
            DebugUtil.LogFormat(ELogType.eNone, "OnSDKLoginCancel:{0}", "sdk登录失败，用户取消登录");
            Sys_Login.Instance.ClearServerList();

            if (!errorMsg.Equals("NULL"))
                Sys_Hint.Instance.PushContent_Normal(errorMsg);

            this.isSelectState = false;
            this.Layout.FirstButton.gameObject.SetActive(true);
            this.Layout.SecondButton.gameObject.SetActive(false);
        }

        private void OnSDKHintMsg(string hintMsg)
        {
            Sys_Hint.Instance.PushContent_Normal(hintMsg);//LanguageHelper.GetTextContent(lanId)
        }

        private void OnSDKBindIphoneStatus(int _type)
        {
            if (Sys_Role.Instance.IsNeedBindPhone)
            {
                OnLogout_ButtonClicked();
                Sys_Role.Instance.IsNeedBindPhone = false;
            }

        }

        #endregion SystemEvent

        private void OnNetworkReachabilityChanged(NetworkReachability oldState, NetworkReachability newState)
        {
            if (newState == NetworkReachability.NotReachable)
            {
            }
            else
            {
                Sys_Login.Instance.GetServerList();
            }
        }

        private void OnNetworkStatusChanged(bool oldState, bool newState)
        {
            if (newState)
            {
                Sys_Login.Instance.GetServerList();
            }
        }

        public void CheckMD5AssetList()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2106004).words;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {

                HotFixStateManager.CheckMD5AssetList = null;
                //if (HotFixManager.IsExitErrorAsset)
                //    HotFixManager.Instance.DeleteErrorAssetList();

                AppManager.Quit();
            }); //"重启游戏"2106001
            PromptBoxParameter.Instance.SetCancel(false, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

    }
}