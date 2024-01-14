using System;
using Framework.UI;
using Lib.Core;
using Logic;
using Logic.Core;
using TMPro;
using UnityEngine;

namespace Logic.UI {
    // 基础UI逻辑
    public partial class UI_Login : UIBase {
        public enum EPageType {
            Page1 = 0,
            Page2,
        }

        public UI_Login_Layout layout;
        public EPageType pageType = EPageType.Page1;

        protected override void OnLoaded() {
            layout = UILayoutBase.GetLayout<UI_Login_Layout>(this.transform);
            layout.Init(this.transform);
            layout.BindEvents(this);

            layout.inputAccount.text = Sys_Login.Instance.Account;
        }

        protected override void OnOpen(object arg) {
            pageType = (EPageType)Convert.ToInt32(arg);
        }

        protected override void OnOpened() {
            CtrlPage(pageType);
        }

        private void CtrlPage(EPageType targetPageType) {
            bool valid = targetPageType == EPageType.Page2;
            layout.goPage1.gameObject.SetActive(!valid);
            layout.goPage2.gameObject.SetActive(valid);
        }

        protected override void OnShow() {
            RefreshServer();
        }

        private void RefreshServer() {
            var zone = Sys_Server.Instance.GetSelectedZone();
            if (zone != null) {
                ImageHelper.SetIcon(layout.imgServerState, UI_ServerList.GetServerStatusIcon(zone.svrZone.State));
                TextHelper.SetText(layout.serverName, zone.svrZone.ZoneName);
            }
            else {
                TextHelper.SetText(layout.serverName, "==**==");
            }
        }
    }

// UI事件
    public partial class UI_Login : UI_Login_Layout.IListener {
        public void OnValueChanged_readAgrement(bool status) {
        }

        public void OnValueChanged_inputAccount(string input) {
            string account = layout.inputAccount.text.Trim();
            Sys_Login.Instance.Account = account;
        }

        public void OnBtnClicked_btnFirst() {
            bool hasRead = layout.readAgrement.isOn;
            if (!hasRead) {
                // 同意协议
                DebugUtil.LogError("同意协议");
                return;
            }

            var account = Sys_Login.Instance.Account;
            if (string.IsNullOrWhiteSpace(account)) {
                // 账户名不能为空
                DebugUtil.LogError("账户名不能为空");
                return;
            }

            if (account.Length < 4) {
                // 账户名必须>=4长度
                DebugUtil.LogError("账户名必须>=4长度");
                return;
            }
            
            if (Sys_AccountName.Instance.IsIllegalName(account)) {
                // 账户名不合法
                DebugUtil.LogError("账户名不合法");
                return;
            }
            
            // UIManager.OpenUI(EUIID.UI_BlockClick, false, new UI_BlockClick.TimeCtrl(2.5f, 10f));
            // Sys_Login.Instance.OnWebInit().LoginTransfer.Connect("192.168.1.52", 60002);
            _OnWebLogin(true);
        }

        public void OnBtnClicked_btnSecond() {
            // if (Sys_Server.Instance.GetSelectedZone() != null) {
            //     Sys_Login.Instance.ReqChooseZone(Sys_Server.Instance.selectedZoneId);
            // }
            LevelManager.EnterLevel(typeof(LvCreateCharacter));
        }

        public void OnBtnClicked_btnChangeServer() {
            UIManager.OpenUI(EUIID.UI_ServerList);
        }

        public void OnTimeRefresh_cdText(TextMeshProUGUI text, float time, bool isEnd) {
        }
    }

// 逻辑事件
    public partial class UI_Login {
        protected override void ProcessEvents(bool toRegister) {
            Sys_Server.Instance.eventEmitter.Handle(Sys_Server.EEvents.OnServerListChanged, _OnServerListChanged, toRegister);
            Sys_Server.Instance.eventEmitter.Handle<uint>(Sys_Server.EEvents.OnSelectedServerChanged, _OnSelectedServerChanged, toRegister);
            Sys_Login.Instance.eventEmitter.Handle<bool>(Sys_Login.EEvents.OnLoginGot, _OnWebLogin, toRegister);
            
        }

        private void _OnServerListChanged() {
            RefreshServer();
        }

        private void _OnSelectedServerChanged(uint zoneId) {
            RefreshServer();
        }
        
        private void _OnWebLogin(bool success) {
            if (success) {
                CtrlPage(EPageType.Page2);
            }
        }
    }
}