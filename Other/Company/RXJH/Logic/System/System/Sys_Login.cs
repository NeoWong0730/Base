using Client;
using Common;
using Lib.Core;
using Logic.Core;
using Logic.UI;
using Net;
using UnityEngine;

namespace Logic {
    public partial class Sys_Login : SystemModuleBase<Sys_Login> {
        public string _account;

        public string Account {
            get {
                if (string.IsNullOrWhiteSpace(this._account)) {
                    this._account = PlayerPrefs.GetString("rxjh-account");
                }

                return this._account;
            }
            set {
                if (!this._account.Equals(value)) {
                    this._account = value;
                    PlayerPrefs.SetString("rxjh-account", value);
                }
            }
        }

        public ulong accountId;

        public uint selectZoneExpire; // 用于显示 选区超时 倒计时

        public uint randToken;
        public uint actionExpire;
    }

    public partial class Sys_Login : ISystemModuleUpdate {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private readonly SystemModuleMsgProcesser _msgProcesser = new SystemModuleMsgProcesser(enClientFirst.Login);

        private NetClient _loginTransfer;

        public NetClient LoginTransfer {
            get {
                if (_loginTransfer == null) {
                    _loginTransfer = new NetClient(NetClient.EType.Login);
                }

                return _loginTransfer;
            }
            private set { _loginTransfer = value; }
        }

        public enum EEvents {
            OnLoginGot,
        }

        public override void Init() {
            _msgProcesser.Listen((ushort)enSecondLogin.ReqVerifyAccountPasswd, (ushort)enSecondLogin.NtfVerifyAccountPasswd, _OnVerifyAccount, SecondLogin_Ntf_VerifyAccountPasswd.Parser, true);
            _msgProcesser.Listen((ushort)enSecondLogin.ReqChooseZone, (ushort)enSecondLogin.NtfChooseZone, _OnChooseZone, SecondLogin_Ntf_ChooseZone.Parser, true);
            _msgProcesser.Listen((ushort)enSecondLogin.ReqLoginGateway, (ushort)enSecondLogin.NtfLoginGateway, _OnLoginGateway, SecondLogin_Ntf_LoginGateway.Parser, true);
            _msgProcesser.Listen((ushort)enSecondLogin.ReqCreateRole, (ushort)enSecondLogin.NtfCreateRole, _OnCreateRole, SecondLogin_Ntf_CreateRole.Parser, true);
            _msgProcesser.Listen((ushort)enSecondLogin.ReqSelectRole, (ushort)enSecondLogin.NtfSelectRole, _OnSelectRole, SecondLogin_Ntf_SelectRole.Parser, true);

            Sys_Net.Instance.eventEmitter.Handle(Sys_Net.EEvents.OnConnectSuccess, _OnConnectSuccess, true);
        }

        #region web登录选服

        public void OnUpdate() {
            OnWebUpdate();
        }

        public Sys_Login OnWebInit() {
            _loginTransfer = null;

            LoginTransfer.RemoveStateListener(_OnStatusChanged);
            LoginTransfer.AddStateListener(_OnStatusChanged);
            return this;
        }

        public void OnWebUpdate() {
            // not loginTransfer?.Update();
            _loginTransfer?.Update();
        }

        public void OnWebLogin() {
        }

        public void OnWebBeginLogout() {
            _loginTransfer.RemoveStateListener(_OnStatusChanged);
            _loginTransfer.Dispose(); // 清理socket以及事件监听
            _loginTransfer = null;
        }

        public void OnWebEndLogout() {
            randToken = 0;
        }

        private void _OnStatusChanged(NetClient.ENetState from, NetClient.ENetState to) {
            DebugUtil.LogFormat(ELogType.eNone, "OnStatusChanged: {0} -> {1}", from, to);
            if (to == NetClient.ENetState.Connected) {
                UIManager.CloseUI(EUIID.UI_BlockClick);
                
                OnWebLogin();
                ReqVerifyAccount();
            }
            else if (to == NetClient.ENetState.ConnectFail) {
            }
            else if (to == NetClient.ENetState.Ready) {
            }
        }

        #endregion

        private void _OnConnectSuccess() {
            UIManager.CloseUI(EUIID.UI_BlockClick);
            
            ReqLoginGateway(randToken, accountId, 0);
            OnWebEndLogout();
        }

        public void ReqVerifyAccount() {
            DebugUtil.Log(ELogType.eNetSendMSG, "ReqVerifyAccount");
            var req = new SecondLogin_Req_VerifyAccountPasswd();
            req.Account = Account;
            _msgProcesser.SendMessage(_loginTransfer, (int)(enSecondLogin.ReqVerifyAccountPasswd), req);
        }

        private void _OnVerifyAccount(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "OnVerifyAccount");
            var res = msg.data as SecondLogin_Ntf_VerifyAccountPasswd;
            accountId = res.AccId;
            selectZoneExpire = res.TimeoutExpire;

            Sys_Time.Instance.CorrectServerTime(res.ServerGameTime);
            Sys_Server.Instance.SetServerList(res.Tabs);

            eventEmitter.Trigger<bool>(EEvents.OnLoginGot, true);
        }

        public void ReqChooseZone(uint zoneId) {
            DebugUtil.LogFormat(ELogType.eNetSendMSG, "ReqChooseZone zondId:{0}", zoneId.ToString());
            var req = new SecondLogin_Req_ChooseZone();
            req.ZoneId = zoneId;
            _msgProcesser.SendMessage(_loginTransfer, (int)(enSecondLogin.ReqChooseZone), req);
        }

        private void _OnChooseZone(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "OnChooseZone");
            var res = msg.data as SecondLogin_Ntf_ChooseZone;
            var gateway = res.Gatway;
            randToken = gateway.RandToken;
            actionExpire = gateway.ActionExpire;

            OnWebBeginLogout(); // 清理login阶段的数据

            void __Listen(bool toListen) {
                
            }

            UIManager.OpenUI(EUIID.UI_BlockClick, false, new UI_BlockClick.TimeCtrl(2.5f, 10f, __Listen));
            // 开始game的socket链接
            NetClient.Instance.Connect(gateway.Ip.ToStringUtf8(), (int)gateway.Port);
        }

        public void ReqLoginGateway(uint token, ulong accountId, ulong clientTime) {
            DebugUtil.LogFormat(ELogType.eNetSendMSG, "ReqLoginGateway clientTime:{0}", clientTime.ToString());
            var req = new SecondLogin_Req_LoginGateway();
            req.RankToken = token;
            req.AccId = accountId;
            req.ClientGameTime = clientTime;
            _msgProcesser.SendMessage((int)(enSecondLogin.ReqLoginGateway), req);
        }

        private void _OnLoginGateway(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "OnLoginGateway");
            var res = msg.data as SecondLogin_Ntf_LoginGateway;
            Sys_Time.Instance.CorrectServerTime(res.ServerGameTime);
            LevelManager.EnterLevel(typeof(LvCreateCharacter));
        }

        public void ReqCreateRole(ZoneCreateRole role) {
            DebugUtil.Log(ELogType.eNetSendMSG, "ReqCreateRole");
            var req = new SecondLogin_Req_CreateRole();
            req.Role = role;
            _msgProcesser.SendMessage((int)(enSecondLogin.ReqCreateRole), req);

            // todo 暂时强制让进去
            _EnterGame(false);
        }

        private void _OnCreateRole(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "OnCreateRole");
            var res = msg.data as SecondLogin_Ntf_CreateRole;

            _EnterGame(false);
        }

        public void ReqSelectRole(ulong roleId) {
            DebugUtil.LogFormat(ELogType.eNetSendMSG, "ReqSelectRole  roleId{0}", roleId.ToString());
            var req = new SecondLogin_Req_SelectRole();
            req.RoleId = roleId;
            _msgProcesser.SendMessage((int)(enSecondLogin.ReqSelectRole), req);

            // todo 暂时强制让进去
            _EnterGame(true);
        }

        private void _OnSelectRole(NetMsg msg) {
            DebugUtil.Log(ELogType.eNetSendMSG, "OnSelectRole");
            // var res = msg.data as SecondLogin_Ntf_SelectRole;

            _EnterGame(true);
        }

        public void ExitGame(bool toLogin) {
            var req = new SecondLogin_Req_QuitGame();
            _msgProcesser.SendMessage((int)(enSecondLogin.ReqQuitGame), req);

            if (toLogin) {
                LevelManager.EnterLevel(typeof(LvLogin));
            }
            else {
                Application.Quit();
            }
        }

        private void _EnterGame(bool selectOrCreate) {
            // 开启主动心跳
            NetClient.Instance.PositiveHeartBeat(true);

            // 进入游戏
            LevelManager.EnterLevel(typeof(LvPlay), new LevelParams() {
                eLoadingType = ELoadingType.eUnuseLoading,
                arg = selectOrCreate,
                bCanSwitchToEqualLevelType = false
            });
        }
    }
}