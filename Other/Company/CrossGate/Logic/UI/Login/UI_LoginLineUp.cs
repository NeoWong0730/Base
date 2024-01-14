using System;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 登录排队
    public class UI_LoginLineUp : UIBase {
        public const float GAP_TIME = 5f;

        public Text serverName;
        public Text lineupText;
        public Text remainText;
        public AppHooker hooker;

        private uint serverId;
        private CmdRoleLoginQueueNtf loginInfo = null;

        private Timer timer;

        protected override void OnLoaded() {
            this.serverName = this.transform.Find("Animator/Detail/Image_Server/SvrName").GetComponent<Text>();
            this.lineupText = this.transform.Find("Animator/Detail/Queue/Text").GetComponent<Text>();
            this.remainText = this.transform.Find("Animator/Detail/Queue/Time/Text").GetComponent<Text>();
            this.hooker = this.gameObject.AddComponent<AppHooker>();

            Button btn = this.transform.Find("Animator/View_BG/Btn_Close").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnExitClicked);

            btn = this.transform.Find("Animator/Detail/Btn_01").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnClicked);
        }
        protected override void OnDestroy() {
            this.timer?.Cancel();
            Sys_Role.Instance.ReqCancelLoginQueue();
        }

        private void OnBtnClicked() {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(11694).words;
            PromptBoxParameter.Instance.SetConfirm(true, OnConform);
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

            void OnConform() {
                this.OnBtnExitClicked();
            }
        }

        private void OnBtnExitClicked() {
            this.CloseSelf();
            Sys_Role.Instance.ReqCancelLoginQueue();
            //NetClient.Instance.Disconnect();

            // 如果再创角场景取消排队，则跳转到登陆场景
            if (LevelManager.mCurrentLevelType == typeof(LvCreateCharacter)) {
                UI_Login.toBtn2 = true;
                LevelManager.EnterLevel(typeof(LvLogin));
            }
        }

        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<uint, CmdRoleLoginQueueNtf>;
            if (tp != null) {
                this.serverId = Convert.ToUInt32(tp.Item1);
                this.loginInfo = tp.Item2;
            }
        }

        protected override void OnOpened() {
            ServerInfo info = Sys_Login.Instance.FindServerInfoByID(this.serverId);
            TextHelper.SetText(this.serverName, info?.ServerName);

            // 请求更新
            Sys_Role.Instance.ReqLoginQueue();

            // 刷新
            this.Refresh();

            this.timer = Timer.Register(GAP_TIME, this.OnTimeOut, this.OnTimeUpdate, true);
        }
        private void OnTimeOut() {
            Sys_Role.Instance.ReqLoginQueue();
        }
        private void OnTimeUpdate(float dt) {
            if (this.loginInfo != null) {
                float remain = this.loginInfo.LeftTime - dt;
                remain = remain < 0 ? 0 : remain;
                remain = remain > 3600 ? 3580 : remain;
                string remainContent = LanguageHelper.TimeToString((uint)remain, LanguageHelper.TimeFormat.Type_5);
                TextHelper.SetText(this.remainText, remainContent);
            }
        }

        private void Refresh() {
            if (this.loginInfo != null) {
                TextHelper.SetText(this.lineupText, 1000045, this.loginInfo.Myturn.ToString(), this.loginInfo.QueueNum.ToString());
            }
            else {
                Debug.LogError("error logininfo refresh");
                TextHelper.SetText(this.lineupText, 1000045, "0", "1");
                TextHelper.SetText(this.remainText, "---");
            }
        }

        #region 事件处理
        protected override void ProcessEvents(bool toRegister) {
            Sys_Role.Instance.eventEmitter.Handle<CmdRoleLoginQueueNtf>(Sys_Role.EEvents.OnLoginQueueNtf, this.OnLoginQueueNtf, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnLoginRes, this.OnLoginRes, toRegister);

            if (this.hooker != null) {
                // hook可能提前被destroy
                if (toRegister) {
                    this.hooker.onNetworkStatusChanged += this.OnNetworkStatusChanged;
                }
                else {
                    this.hooker.onNetworkStatusChanged += this.OnNetworkStatusChanged;
                }
            }
        }

        private void OnLoginQueueNtf(CmdRoleLoginQueueNtf info) {
            this.loginInfo = info;
            this.Refresh();
        }
        private void OnLoginRes() {
            this.timer?.Cancel();
            this.CloseSelf();
        }
        private void OnNetworkReachabilityChanged(NetworkReachability oldState, NetworkReachability newState) {
            if (newState != NetworkReachability.NotReachable) {
                this.OnTimeOut();
            }
            else {
                this.OnBtnExitClicked();
            }
        }
        private void OnNetworkStatusChanged(bool oldState, bool newState) {
            if (!newState) {
                this.OnBtnExitClicked();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10873));
            }
        }
        #endregion
    }
}
