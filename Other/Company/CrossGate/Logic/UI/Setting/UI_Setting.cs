using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public enum ESettingPage
    {
        Settings,
        TZ,
        Code,
        SaftyLock,
    }

    public enum ESetting
    {
        Graphics,
        Audio,
        Game,
        Seal,
        Hangup,
        HotKey,
    }

    public class UI_Setting : UIBase, UI_Setting_Layout.IListener
    {

        #region Setting
        public interface ISettingEntry
        {
            void Refresh();
            void Init(Transform root, int settingID);
        }

        public class Setting_ToggleGroup : ISettingEntry
        {
            public int _settingID { get; private set; }
            public CP_ToggleRegistry _toggleGroup { get; private set; }
            public bool _bRefresh;

            public void Init(Transform root, int settingID)
            {
                _toggleGroup = root.GetComponent<CP_ToggleRegistry>();
                _settingID = settingID;
                Refresh();

                _toggleGroup.onToggleChange += OnToggleChange;
            }

            private void OnToggleChange(int to, int from)
            {
                if (!_bRefresh)
                {
                    OptionManager.Instance.SetInt(_settingID, to, false);
                }
            }

            public void Refresh()
            {
                _bRefresh = true;
                _toggleGroup.SwitchTo(OptionManager.Instance.GetInt(_settingID, false), false, false);
                _bRefresh = false;
            }
        }

        public class Setting_Switch : ISettingEntry
        {
            private int _settingID;
            private Toggle _toggle;
            private bool _bRefresh;

            public void Init(Transform root, int settingID)
            {
                _toggle = root.GetComponent<Toggle>();
                _settingID = settingID;
                Refresh();
                _toggle.onValueChanged.AddListener(OnValueChanged);
            }

            private void OnValueChanged(bool v)
            {
                if (!_bRefresh)
                {
                    OptionManager.Instance.SetBoolean(_settingID, v, false);
                }
            }

            public void Refresh()
            {
                _bRefresh = true;
                _toggle.SetIsOnWithoutNotify(OptionManager.Instance.GetBoolean(_settingID, false));
                _bRefresh = false;
            }
        }

        public class Setting_Slider : ISettingEntry
        {
            public int _settingID { get; private set; }
            public Slider _slider { get; private set; }
            public bool _bRefresh;

            public void Init(Transform root, int settingID)
            {
                _slider = root.GetComponent<Slider>();
                _settingID = settingID;
                Refresh();
                _slider.onValueChanged.AddListener(OnValueChanged);
            }

            private void OnValueChanged(float v)
            {
                if (!_bRefresh)
                {
                    OptionManager.Instance.SetFloat(_settingID, v, false);
                }
            }

            public void Refresh()
            {
                _bRefresh = true;
                _slider.SetValueWithoutNotify(OptionManager.Instance.GetFloat(_settingID, false));
                _bRefresh = false;
            }
        }

        public class Setting_Dropdown : ISettingEntry
        {
            private int _settingID;
            private Dropdown _dropdown;
            private bool _bRefresh;
            private List<uint> _optionsLan = new List<uint>();

            public void Init(Transform root, int settingID)
            {
                _dropdown = root.GetComponent<Dropdown>();
                _dropdown.ClearOptions();
                if (settingID == (int)OptionManager.EOptionID.AbandonAutoPet)
                {
                    _optionsLan = ReadHelper.ReadArray_ReadUInt(CSVPetNewParam.Instance.GetConfData(25).str_value, '|');
                }
                else if (settingID == (int)OptionManager.EOptionID.AutoPetCatchCardQuality)
                {
                    _optionsLan = ReadHelper.ReadArray_ReadUInt(CSVPetNewParam.Instance.GetConfData(26).str_value, '|');
                }
                else
                {
                    Lib.Core.DebugUtil.LogErrorFormat("未添加该Dropdown选项，id = {1}", root.name, settingID);
                }
                _dropdown.options.Clear();
                for (int i = 0; i < _optionsLan.Count; ++i)
                {
                    Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = LanguageHelper.GetTextContent(_optionsLan[i]);
                    _dropdown.options.Add(op);
                }

                _settingID = settingID;

                Refresh();
                _dropdown.onValueChanged.AddListener(OnValueChanged);
            }

            private void OnValueChanged(int index)
            {
                if (!_bRefresh)
                {
                    OptionManager.Instance.SetInt(_settingID, index, false);
                }
            }

            public void Refresh()
            {
                _bRefresh = true;
                int index = OptionManager.Instance.GetInt(_settingID, false);
                _dropdown.SetValueWithoutNotify(index);
                _bRefresh = false;
            }
        }

        #endregion


        private UI_Setting_Layout Layout;
        private Dictionary<int, ISettingEntry> _settings = new Dictionary<int, ISettingEntry>();
        private string exchangeCodeStr;

        private Tuple<ESettingPage, ESetting> tuple;

        private ESettingPage _currentSettings = ESettingPage.Settings;
        private ESetting _currentSetting = ESetting.Graphics;

        private UI_SetKeyCode SetKeyCodeView = new UI_SetKeyCode();
        //安全锁
        private bool hasPassward;//密码
        private bool lockState;//加锁状态
        private int errorCount;//密码错误次数
        private uint enforceUnlockTick;//强制解锁到期时间戳       
        private bool isEnforce = false;

        protected override void OnLoaded()
        {
            Layout = new UI_Setting_Layout();
            Layout.Parse(gameObject);

            Layout.rt_recommend_0.gameObject.SetActive(OptionManager.Instance.RecommendQuality == EQuality.Low);
            Layout.rt_recommend_1.gameObject.SetActive(OptionManager.Instance.RecommendQuality == EQuality.Middle);
            Layout.rt_recommend_2.gameObject.SetActive(OptionManager.Instance.RecommendQuality == EQuality.High);

            CreateSettingEntry<Setting_ToggleGroup>(Layout.tg_graphicsSetting.transform, (int)OptionManager.EOptionID.Quality);
            ParseSettingEntry(Layout.rt_graphicsSettingLayout0);
            //ParseSettingEntry(Layout.rt_graphicsSettingLayout2);
            ParseSettingEntry(Layout.rt_audioSettingLayout0);
            ParseSettingEntry(Layout.rt_audioSettingLayout1);
            ParseSettingEntry(Layout.rt_audioSettingLayout2);
            ParseSettingEntry(Layout.rt_audioSettingLayout3);
            ParseSettingEntry(Layout.rt_audioSettingLayout4);
            ParseSettingEntry(Layout.rt_gameSettingLayout0);
            ParseSettingEntry(Layout.rt_pushSettingLayout0);
            //ParseSettingEntry(Layout.rt_audioSettingLayout0);
            ParseSettingEntry(Layout.rt_sealSettingLayout0);
            ParseSettingEntry(Layout.rt_sealSettingLayout1);
            ParseSettingEntry(Layout.rt_sealSettingLayout2);
            ParseSettingEntry(Layout.rt_sealSettingLayout3);

            // 挂机
            ParseSettingEntry(Layout.rt_hangupSettingLayout0);
            ParseSettingEntry(Layout.rt_hangupSettingLayout1);
            ParseSettingEntry(Layout.rt_hangupSettingLayout2);
            ParseSettingEntry(Layout.rt_hangupSettingLayout3);

            SetKeyCodeView.Init(Layout.ScrollView_rejian.transform);

            Layout.RegisterEvents(this);


            if (SDKManager.sdk.IsHaveSdk)
            {
                bool officialFlag = SDKManager.GetChannel().Equals(SDKManager.officialChannel);
                bool cdKeyFalg = SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.CDKey.ToString(), out string paramsValue); 
                this.Layout.rt_TabItem2_CDKey.gameObject.SetActive(cdKeyFalg);
                //this.Layout.btn_community.gameObject.SetActive(officialFlag);
#if UNITY_IOS
                this.Layout.btn_scanCode.gameObject.SetActive(true);
#else
                this.Layout.btn_scanCode.gameObject.SetActive(false);
#endif

#if UNITY_STANDALONE_WIN && USE_PCSDK
                this.Layout.btn_person.gameObject.SetActive(!officialFlag);
                this.Layout.btn_bugReport.gameObject.SetActive(officialFlag);
                this.Layout.btn_scanCode.gameObject.SetActive(false);
#else
                this.Layout.btn_person.gameObject.SetActive(officialFlag);
                this.Layout.btn_bugReport.gameObject.SetActive(!officialFlag);
#endif

            }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            //PC-CBT3版本SDK未提供以下功能，暂时屏蔽
            this.Layout.btn_privacyPolicy.gameObject.SetActive(false);
            this.Layout.btn_userAgreement.gameObject.SetActive(false);
            //this.Layout.btn_community.gameObject.SetActive(false);
#endif
            //Sys_SecureLock.Instance.SecureLockInitReq();
        }

        protected override void OnOpen(object arg)
        {
            tuple = arg as Tuple<ESettingPage, ESetting>;
        }

        public void CreateSettingEntry<T>(Transform root, int settingID) where T : ISettingEntry, new()
        {
            if (!_settings.ContainsKey(settingID))
            {
                T entry = new T();
                entry.Init(root, settingID);
                _settings.Add(settingID, entry);
            }
            else
            {
                Lib.Core.DebugUtil.LogErrorFormat("添加设置项{0}失败，已经存在设置项 id = {1}", root.name, settingID);
            }
        }

        public void CreateSettingEntry<T>(Transform root, string sid) where T : ISettingEntry, new()
        {
            if (int.TryParse(sid, out int settingID))
            {
                if (!_settings.ContainsKey(settingID))
                {
                    T entry = new T();
                    entry.Init(root, settingID);
                    _settings.Add(settingID, entry);
                }
                else
                {
                    Lib.Core.DebugUtil.LogErrorFormat("添加设置项{0}失败，已经存在设置项 id = {1}", root.name, settingID);
                }
            }
            else
            {
                Lib.Core.DebugUtil.LogErrorFormat("{0} id 不是数字", root.name);
            }
        }

        protected void ParseSettingEntry(RectTransform root)
        {
            string toggleGroup_StartWith = "ToggleGroup_";
            string toggle_StartWith = "Toggle_";
            string slider_StartWith = "Slider_";
            string dropdown_StartWith = "Dropdown_";

            int count = root.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform node = root.GetChild(i);
                string nodeName = node.name;
                if (nodeName.StartsWith(toggleGroup_StartWith, StringComparison.Ordinal))
                {
                    CreateSettingEntry<Setting_ToggleGroup>(node, nodeName.Remove(0, toggleGroup_StartWith.Length));
                }
                else if (nodeName.StartsWith(toggle_StartWith, StringComparison.Ordinal))
                {
                    CreateSettingEntry<Setting_Switch>(node, nodeName.Remove(0, toggle_StartWith.Length));
                }
                else if (nodeName.StartsWith(slider_StartWith, StringComparison.Ordinal))
                {
                    CreateSettingEntry<Setting_Slider>(node, nodeName.Remove(0, slider_StartWith.Length));
                }
                else if (nodeName.StartsWith(dropdown_StartWith, StringComparison.Ordinal))
                {
                    CreateSettingEntry<Setting_Dropdown>(node, nodeName.Remove(0, dropdown_StartWith.Length));
                }
            }
        }

        protected override void OnShow()
        {
            Sys_Head.Instance.SetHeadAndFrameData(Layout.img_headIcon);

            Layout.txt_level.text = Sys_Role.Instance.Role.Level.ToString();
            Layout.txt_name.text = Sys_Role.Instance.sRoleName;
            Layout.txt_id.text = Sys_Role.Instance.RoleId.ToString();

            Layout.txt_server.text = Sys_Login.Instance.mSelectedServer == null ? null : Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Layout.rt_graphicsSettingLayout0_ToggleGroup_11.gameObject.SetActive(true);
            Layout.tg_HotKeySetting.gameObject.SetActive(true);
            this.Layout.btn_scanCode.gameObject.SetActive(false);
#elif UNITY_STANDALONE_WIN && OPEN_PC_KEYCODE_FUN
            Layout.rt_graphicsSettingLayout0_ToggleGroup_11.gameObject.SetActive(false);
            Layout.tg_HotKeySetting.gameObject.SetActive(true);
#else
            Layout.rt_graphicsSettingLayout0_ToggleGroup_11.gameObject.SetActive(false);
            Layout.tg_HotKeySetting.gameObject.SetActive(false);
#endif
            RefreshAllOption();
            RefreshHangup();

            if (tuple != null)
            {
                Layout.tg_Tables.SwitchTo((int)tuple.Item1);
                if (tuple.Item1 == ESettingPage.Settings)
                {
                    Layout.tg_SettingTable.SwitchTo((int)tuple.Item2);
                }
            }

            //if (tuple != null)
            //{
            //    Layout.tg_SettingTable.SwitchTo((int)tuple.Item1);
            //    Layout.tg_SettingTable.SwitchTo((int)tuple.Item2);
            //}
            TextShow();
            SaftyLockShow();
            EnforceUnlockCheck();
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            if (Layout.tg_HotKeySetting.IsOn)
                SetKeyCodeView.Show();
#endif
            Layout.goClosePowersaving.SetActive(Sys_PowerSaving.Instance.bAllowPowerSaving);
        }

        protected override void OnHide()
        {
            tuple = null;
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            SetKeyCodeView.Hide();
#endif
        }

        private void RefreshHangup()
        {
            float.TryParse(CSVParam.Instance.GetConfData(262).str_value, out float hangupDuration);
            TextHelper.SetText(Layout.autoHangupTip, 2021913, hangupDuration.ToString());

            bool isPrivilegeUsing = Sys_Attr.Instance.privilegeBuffIdList.Contains(3);
            if (isPrivilegeUsing)
            {
                Layout.lockedHangup.SetActive(false);
                Layout.unlockedHangup.SetActive(true);

                float.TryParse(CSVParam.Instance.GetConfData(1009).str_value, out float arg1);
                float.TryParse(CSVParam.Instance.GetConfData(1025).str_value, out float arg2);
                TextHelper.SetText(Layout.unlockedHangupTip, 2021919, arg1.ToString(), arg2.ToString());
            }
            else
            {
                Layout.lockedHangup.SetActive(true);
                Layout.unlockedHangup.SetActive(false);
            }
        }
        private void RefreshAllOption()
        {
            Dictionary<int, ISettingEntry>.Enumerator enumerator = _settings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Refresh();
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionValueChange, OnOptionValueChange, toRegister);
            SDKManager.eventEmitter.Handle<int>(SDKManager.ESDKLoginStatus.onSDKReportErrorToChannel, OnReportErrorToChannel, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnStateUpdate, OnStateUpdate, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnErrorCountUpdate, OnErrorCountUpdate, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnVlueInit, OnValueInit, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnUnLockRes, OnUnLock, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnResetPwdRes, OnResetPassWard, toRegister);
            Sys_SecureLock.Instance.eventEmitter.Handle(Sys_SecureLock.EEvents.OnSecureLockEnforce, OnSecureLockEnforce, toRegister);

        }

        private void OnOptionValueChange(int optionID)
        {
            if (_settings.TryGetValue(optionID, out ISettingEntry settingEntry))
            {
                settingEntry.Refresh();
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (optionID == (int)OptionManager.EOptionID.PCAspectRatioOpt)
                    ResolutionChangeTips();
#endif
            }
        }

        private void OnReportErrorToChannel(int code)
        {
            if (code == 0)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021923));//日志上报成功
            else
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2021924));//日志上报失败，请稍后再试
        }


        public void OnBtnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Setting);
        }

        public void OnBtnPerson_ButtonClicked()
        {
            SDKManager.SDKOpenUserCenter();
        }

        public void OnBtnLockScreen_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Setting);
            UIManager.OpenUI(EUIID.UI_ScreenLock);
        }


        public void OnBtnPrivacyPolicy_ButtonClicked()
        {
            //隐私政策：https://www.gamekuaishou.com/privacy?channel=**&appid=****
            SDKManager.SDKPrivacyPolicy();
        }

        public void OnUserAgreement_ButtonClicked()
        {
            //用户协议：https://www.gamekuaishou.com/policy?channel=**&appid=****
            SDKManager.SDKUserAgreement();
        }


        public void OnCDKey_InputFieldValueChanged(string arg)
        {
            exchangeCodeStr = this.Layout.InputField_CDKey.text.Trim();
        }

        public void OnBtnExchangeCode_ButtonClicked()
        {
            Sys_Role.Instance.ReqExchangeGift(exchangeCodeStr, SDKManager.GetToken());
        }

        public void OnBtnBugReport_ButtonClicked()
        {
            SDKManager.SDKReportErrorToChannel(string.Format("{0}-{1}", SDKManager.GetChannel(), SDKManager.GetAppid()));
        }

        public void OnBtnCommunity_ButtonClicked()
        {
            //SDKManager.SDKCommunityService();
        }

        public void OnBtnScanCode_ButtonClicked()
        {
            SDKManager.SDKScanQRCode();
        }

        public void OnBtnSwitchRole_ButtonClicked()
        {
            //返回登录，重新选角
            UI_Login.toBtn2 = true;
            Sys_Role.Instance.ExitGameReq();
        }

        public void OnPetTips_ButtonClicked()
        {
            UISetingTipParam param = new UISetingTipParam();
            param.contentLanId = 11657;
            UIManager.OpenUI(EUIID.UI_Setting_Tips, false, param);
        }

        public void OnRecharge_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_OperationalActivity, false, new OperationalActivityPrama { pageType = (uint)EOperationalActivity.SpecialCard, openValue = 2 });
        }

        public void OnTg_Tables_ToggleChange(int from, int to)
        {

        }

        public void OnTg_SettingTable_ToggleChange(int from, int to)
        {

        }

        #region 安全锁

        private void ValueInit()
        {
            hasPassward = Sys_SecureLock.Instance.hasPwd;
            lockState = Sys_SecureLock.Instance.lockState;
            errorCount = Sys_SecureLock.Instance.errorCount;
            enforceUnlockTick = Sys_SecureLock.Instance.enforceUnlockTick;
        }
        private void SaftyLockShow()
        {
            ValueInit();
            if (enforceUnlockTick == 0)
            {
                isEnforce = false;
                LockShow();
            }
            else
            {//强制解锁
                isEnforce = true;
                ForceUnLockShow();
            }
        }
        public void PanelOpen(bool openCotent, bool closeContent)
        {
            Layout.openContent.SetActive(openCotent);
            Layout.closeContent.SetActive(closeContent);
            Layout.timePanel.SetActive(isEnforce);

        }
        private void EnforceUnlockCheck()
        {
            if (enforceUnlockTick == 0) return;
            var nowTime = Sys_Time.Instance.GetServerTime();
            if (nowTime> enforceUnlockTick)
            {
                Sys_SecureLock.Instance.SecureLockInitReq();
            }
        }
        public void TextShow()
        {
            
            FrameworkTool.CreateChildList(Layout.prohibitItem.transform.parent, 2);
            for (uint i=0;i<2;i++)
            {
                GameObject go = Layout.prohibitItem.transform.parent.GetChild((int)i).gameObject;
                Text content = go.transform.Find("Text").GetComponent<Text>();
                content.text = LanguageHelper.GetTextContent(i + 590001100);
            }
            Layout.explainContent.text = LanguageHelper.GetTextContent(590001102);
            Layout.leftPanel.GetComponent<VerticalLayoutGroup>().spacing = 20f;
            Layout.enforceContent.text = string.Format(LanguageHelper.GetTextContent(590001103), 10.ToString(), 10.ToString());
            

        }

        public void LockShow()
        {
            Layout.stateText.gameObject.SetActive(true);
            Layout.title.text = LanguageHelper.GetTextContent(2021926);
            PanelOpen(true, false);
            ButtonShow();
            if (lockState)
            {//上锁
                Layout.stateText.text = LanguageHelper.GetTextContent(2021927);
            }
            else
            {//未上锁
                Layout.stateText.text = LanguageHelper.GetTextContent(2021928);
            }
        }

        public void ForceUnLockShow()
        {//强制解锁

            Layout.stateText.gameObject.SetActive(false);
            Layout.title.text = LanguageHelper.GetTextContent(2021929);
            PanelOpen(false, true);
            ButtonShow();
            DateTime startTime = TimeManager.GetDateTime(enforceUnlockTick - 864000);
            DateTime endTime = TimeManager.GetDateTime(enforceUnlockTick);
            Layout.dateCommit.text = DateShow(startTime.Year, startTime.Month, startTime.Day, "-");
            Layout.timeCommit.text = DateShow(startTime.Hour, startTime.Minute, startTime.Second, ":");
            Layout.dateRelieve.text = DateShow(endTime.Year, endTime.Month, endTime.Day, "-");
            Layout.timeRelieve.text = DateShow(endTime.Hour, endTime.Minute, endTime.Second, ":");
            Layout.imageClose.SetActive(true);
        }

        //时间显示
        public string DateShow(int first, int second, int third, string breakStr)
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0:00}", first).Append(breakStr).AppendFormat("{0:00}", second).Append(breakStr).AppendFormat("{0:00}", third);
            return str.ToString();

        }

        public void ButtonShow()
        {//未上锁|上锁|强制解锁

            isEnforce = enforceUnlockTick == 0 ? false : true;
            Layout.imageClose.SetActive(!lockState);
            Layout.lockBtn.gameObject.SetActive(!isEnforce && !lockState);
            Layout.unLockBtn.gameObject.SetActive(!isEnforce && lockState);
            Layout.forceUnLockBtn.gameObject.SetActive(!isEnforce && lockState);
            Layout.changePwdBtn.gameObject.SetActive(!isEnforce && lockState);
            Layout.cancelBtn.gameObject.SetActive(isEnforce && lockState);

        }
        #region 事件

        public void OnValueInit()
        {
            SaftyLockShow();
        }

        public void OnStateUpdate()
        {
            SaftyLockShow();
        }

        public void OnSecureLockEnforce()
        {
            SaftyLockShow();
        }
        public void OnErrorCountUpdate()
        {
            string str = "";
            errorCount = Sys_SecureLock.Instance.errorCount;
            if (UIManager.IsOpen(EUIID.UI_ChangePassword_Prop))
            {
                str = CSVErrorCode.Instance.GetConfData(592000006).words;
            }else
            {

                str = CSVErrorCode.Instance.GetConfData(592000004).words;
            }
            if (errorCount <=5)
            {
                Sys_Hint.Instance.PushContent_Normal(string.Format(str, 5 - errorCount));
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);//解锁密码输入错误次数过多，请明日再试。
            }


        }
        public void OnUnLock()
        {
            Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000007).words);//成功解锁
        }

        public void OnResetPassWard()
        {
            if (UIManager.IsOpen(EUIID.UI_ChangePassword_Prop))
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000002).words);//解锁密码设置成功
                UIManager.CloseUI(EUIID.UI_ChangePassword_Prop);
            }

        }

        #endregion

        #region 按钮
        public void OnLockBtnClicked()
        {
            if (hasPassward)
            {
                Sys_SecureLock.Instance.SecureLockLockReq();
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_SetPassword_Prop);
            }


        }
        public void OnUnLockBtnClicked()
        {
            errorCount = Sys_SecureLock.Instance.errorCount;
            if (errorCount<5)
            {
                UIManager.OpenUI(EUIID.UI_Unlock_Prop);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);
            }
        }
        public void OnForceUnLockBtnClicked()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(590001104, 10.ToString());
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_SecureLock.Instance.SecureLockEnforceResetReq(1);
                UIManager.CloseUI(EUIID.UI_PromptBox);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        public void OnChangePwdBtnClicked()
        {
            errorCount = Sys_SecureLock.Instance.errorCount;
            if (errorCount <5)
            {
                UIManager.OpenUI(EUIID.UI_ChangePassword_Prop);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(592000005).words);
            }
            
        }
        public void OnCancelBtnClicked()
        {
            Sys_SecureLock.Instance.SecureLockEnforceResetReq(0);
        }
        #endregion

        #endregion

        public void OnBtnDefaultKey_ButtonClicked()
        {
            Sys_SettingHotKey.Instance.DefaultKeySetting();
            SetKeyCodeView.ShowHotKeyList();
        }

        /// <summary>
        /// 切换PC分辨率确认框
        /// </summary>
        private void ResolutionChangeTips()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(1000999);
            PromptBoxParameter.Instance.SetConfirm(true, null);
            PromptBoxParameter.Instance.SetCancel(false, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
#endif
        }

        public void OnClickToggle(bool isOn)
        {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            if (isOn)
                SetKeyCodeView.Show();
            else
                SetKeyCodeView.Hide();
#endif
        }

        protected override void OnUpdate()
        {
#if UNITY_STANDALONE_WIN && (OPEN_PC_KEYCODE_FUN || !UNITY_EDITOR)
            SetKeyCodeView?.ExecUpdate();
#endif
        }

        public void OnBtnSealSetting_ButtonClicked()
        {
            Sys_Pet.Instance.PetCatchSettingsReq();
            UIManager.OpenUI(EUIID.UI_Pet_SealSetting);
        }
    }
}