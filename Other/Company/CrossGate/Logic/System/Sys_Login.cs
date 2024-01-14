using System;
using System.Collections;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using Framework;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.Networking;
using pb = global::Google.Protobuf;

namespace Logic
{
    public class ServerEntry
    {
        public List<DirRoleInfo> mRoleInfo;
        public ServerInfo mServerInfo { get; private set; }
        public bool isWhiteList = false;

        private List<string> _tags;
        public List<string> tags
        {
            get
            {
                if (this._tags == null)
                {
                    string arrayStr = this.mServerInfo.Tags.ToStringUtf8();
                    if (string.IsNullOrEmpty(arrayStr))
                    {
                        this._tags = new List<string>(0);
                    }
                    else
                    {
                        try
                        {
                            string[] array = JsonUtilityArrayPacker.FromJson<string[]>(arrayStr);
                            if (array == null)
                            {
                                this._tags = new List<string>(0);
                            }
                            else
                            {
                                this._tags = array.ToList<string>();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.Message + "  " + this.mServerInfo.ServerName + "--> " + arrayStr);
                            this._tags = new List<string>(0);
                        }
                    }
                }
                return this._tags;
            }
        }

        private string _subTitle;

        public string SubTitle {
            get {
                if (this._subTitle == null) {
                    _subTitle = mServerInfo.SubTitle.ToStringUtf8();
                }

                return _subTitle;
            }
        }
        
        private string _mainTitle;

        public string MainTitle {
            get {
                if (this._mainTitle == null) {
                    _mainTitle = mServerInfo.MainTainMessage.ToStringUtf8();
                }

                return _mainTitle;
            }
        }

        public ServerEntry(ServerInfo serverInfo, List<DirRoleInfo> roleInfo)
        {
            this.mServerInfo = serverInfo;
            this.mRoleInfo = roleInfo;
        }
        
        public void GetState(out uint stateIcon, out uint stateText)
        {
            stateIcon = 990002;
            stateText = 1000005;

            var color = this.mServerInfo.Color.ToStringUtf8();
            if (color == "GREEN")
            {
                stateIcon = 990002;
                stateText = 1000005;
            }
            else if (color == "YELLOW")
            {
                stateIcon = 990003;
                stateText = 1000004;
            }
            else if (color == "RED")
            {
                stateIcon = 990004;
                stateText = 1000003;
            }
            else if (color == "GRAY")
            {
                stateIcon = 990001;
                stateText = 1000006;
            }
        }
    }

    public class Sys_Login : SystemModuleBase<Sys_Login>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            OnGetServerList,
            OnLoginGot,
            OnServerListChanged,
            OnSelectServerChanged,
            //OnLoginFail
        }

        public readonly Dictionary<uint, ServerEntry> allServers = new Dictionary<uint, ServerEntry>();
        public readonly List<ServerEntry> mServerEntries = new List<ServerEntry>();
        public readonly List<ServerEntry> mMyServerEntries = new List<ServerEntry>();
        public readonly List<ServerEntry> mRecommendServerEntries = new List<ServerEntry>();
        
        // groupId : list
        public SortedDictionary<uint, ServerGroup> tabs = new SortedDictionary<uint, ServerGroup>();

        public static bool SwitchRoleUiSetting = false;

        public List<uint> FilterSvrIds(IList<uint> ls)
        {
            List<uint> rlt = new List<uint>(0);
            for (int i = 0, length = ls.Count; i < length; ++i)
            {
                var one = ls[i];
                if (this.allServers.ContainsKey(one))
                {
                    rlt.Add(one);
                }
            }
            return rlt;
        }

        // 上次登录时间
        public long PreLoginFailTime
        {
            get
            {
                return PlayerPrefs.GetInt("PreLoginTime");
            }
            set
            {
                PlayerPrefs.SetInt("PreLoginTime", (int)value);
                //PlayerPrefs.Save();
            }
        }
        // 连续登录失败的次数
        public int seriesLoginFailCount = 0;

        private string _account;
        public string Account
        {
            get
            {
                if (string.IsNullOrEmpty(this._account))
                {
                    this._account = PlayerPrefs.GetString("account");
                }
                return this._account;
            }
        }

        /// <summary>
        /// 在登录成功后设置
        /// </summary>
        /// <param name="account"></param>
        public void SetAccount(string account)
        {
            this._account = account;
            PlayerPrefs.SetString("account", this._account);
        }

        public ulong selectedRoleId { get; set; } = 0;

        private int _selectedServerId = 0;
        public int selectedServerId
        {
            get
            {
                return this._selectedServerId;
            }
            set
            {
                if (this._selectedServerId != value)
                {
                    Sys_Login.Instance.seriesLoginFailCount = 0;
                    Sys_Login.Instance.PreLoginFailTime = 0;

                    this._selectedServerId = value;
                }
            }
        }
        /// <summary>
        /// 真实进入的服务器ID,由于合服导致玩家选择的serverid不一定是真实的serverid
        /// </summary>
        public uint RealServerID { get; set; }


        public ServerEntry mSelectedServer { get; private set; }

        public pb::ByteString Signature { get; private set; }
        public pb::ByteString Extensiondata { get; private set; }
        public uint SdkLoginTIme { get; private set; }
        private float _webLoginTime;
        public string AccountByChannel(string account)
        {
            string ret;
        
            if (SDKManager.sdk.IsQRCODE)
            {
                if (SDKManager.sdk.IsHaveSdk)
                {
                    SDKManager.iAccountType = (int)AccountType.IOs;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType, SDKManager.GetUID());
                }
                else
                {
                    SDKManager.iAccountType = (int)AccountType.Android;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType, account);
                }
            }
            else
            {
                if (SDKManager.sdk.IsHaveSdk)
                {
#if UNITY_IOS
                    SDKManager.iAccountType = (int)AccountType.IOs;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType, SDKManager.GetUID());
#else
                    SDKManager.iAccountType = (int)AccountType.Android;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType , SDKManager.GetUID());
#endif
                }
                else
                {
#if UNITY_EDITOR && SKIP_SDK_Login
                    SDKManager.iAccountType = SkipSDKLogin.Instance.accountType;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType, SkipSDKLogin.Instance.Account);
#else
                    SDKManager.iAccountType = (int)AccountType.Android;
                    ret = string.Format("{0}_{1}", SDKManager.iAccountType, account);
#endif
                }
            }
            return ret;
        }

        public int FirstWaitTime = 15;
        public string ChannelAccount
        {
            get
            {
                return this.AccountByChannel(this.Account);
            }
        }
        public string urlEncodeAccount
        {
            get
            {
                return UnityWebRequest.EscapeURL(this.Account, Encoding.UTF8);
            }
        }

        public float fRefreshInterval = 60;
        private float fTimePoint;

        //private int nTryLoginWebIndex = 0;
        private UnityWebRequest webLoginWebRequest;

        //private int nTryGotListIndex = 0;
        private UnityWebRequest serverListWebRequest;

        private UnityWebRequest ipDispatchWebRequest;
        private UnityWebRequest pkServerWebRequest;

        public override void Init()
        {
            var csv = CSVParam.Instance.GetConfData(961);
            if (csv != null)
            {
                int.TryParse(csv.str_value, out this.FirstWaitTime);
            }
        }

#region 登录
        private void LoginTransfer()
        {
            string account = this.urlEncodeAccount;
            string url = string.Format("{0}?account={1}&token={2}&device-id={3}&packagechannel={4}&ts={5}", VersionHelper.LoginUrl, this.AccountByChannel(account), SDKManager.GetToken(), SDKManager.GetDeviceId(), SDKManager.GetPublishAppMarket(), TimeManager.ClientNowMillisecond());
            Debug.Log($"url: {VersionHelper.LoginUrl}");

            this.webLoginWebRequest = UnityWebRequest.Get(url);
            this.webLoginWebRequest.timeout = 10;
            DebugUtil.LogFormat(ELogType.eNone, "WebLogin:{0}", url);

            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "开始获取服务器列表");
            UnityWebRequestAsyncOperation requestAsyncOperation = this.webLoginWebRequest.SendWebRequest();
            requestAsyncOperation.completed += this.OnWebLoginGot;
        }

        public bool needExtUrlRequest = true;

        public void LoginWeb()
        {
            if (this.webLoginWebRequest != null) return;

            //this.nTryLoginWebIndex = 0;
            // 判断VersionHelper.DirsvrUrl是否合法应该在启动的阶段检验，校验不合法则直接不会进入logic阶段，所以这里代码逻辑不合理
            // 因为editor存在一种情况就是：断网一直可以进入到logic阶段的loginUI，然后此时发现请求服务器列表地的时候存在问题，就是这个url不存在导致的，这是不合理的！

            HitPointManager.HitPoint("game_serverlist_start");
            if (VersionHelper.LoginUrl != null)//&& VersionHelper.LoginUrls.Length > this.nTryLoginWebIndex)
            {
                this.LoginTransfer();
            }
            else
            {
                //"获取服务器列表失败"
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(42622));
                this.eventEmitter.Trigger(EEvents.OnLoginGot, false);

                Hashtable hashtable = new Hashtable();
                hashtable.Add("reason", "登录服 LoginUrl 为空");
                HitPointManager.HitPoint("game_serverlist_fail", hashtable);
            }
        }

        private void OnWebLoginGot(AsyncOperation operation)
        {
            if (this.webLoginWebRequest.isHttpError || this.webLoginWebRequest.isNetworkError)
            {
                Debug.LogError(string.Format("LoginTransfer:{0}", this.webLoginWebRequest.error));

                //++this.nTryLoginWebIndex;
                //if (VersionHelper.LoginUrls.Length > this.nTryLoginWebIndex) {
                //    this.LoginTransfer();
                //}
                //else
                {
                    //"获取服务器列表失败"
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("reason", this.webLoginWebRequest.error);
                    HitPointManager.HitPoint("game_serverlist_fail", hashtable);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(42622));
                    SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, string.Format("获取服务器列表失败:{0}", this.webLoginWebRequest.error));
                    this.webLoginWebRequest.Dispose();
                    this.webLoginWebRequest = null;
                    // SetSelectedServerInvalid();
                    this.eventEmitter.Trigger(EEvents.OnLoginGot, false);
                    this.eventEmitter.Trigger(EEvents.OnServerListChanged);
                }
            }
            else
            {

                this.ClearServerList();
                NetMsgUtil.TryDeserialize(AccountLoginRes.Parser, this.webLoginWebRequest.downloadHandler.data, out AccountLoginRes accountRes);

#if UNITY_EDITOR && SKIP_SDK_Login
                this.Signature = accountRes.Signature;
                this.Extensiondata = accountRes.Extensiondata;
                this.SdkLoginTIme = accountRes.Time;
                this._webLoginTime = Time.unscaledTime;
                //this.SetServerList(accountRes.Serverlist);
                GetServerList();
                this.eventEmitter.Trigger(EEvents.OnLoginGot, true);
                HitPointManager.HitPoint("game_get_serverlist_success");
                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "获取服务器列表成功");
               
#else
                if (accountRes.Ret == 0) //校验成功
                {
                    this.Signature = accountRes.Signature;
                    this.Extensiondata = accountRes.Extensiondata;
                    this.SdkLoginTIme = accountRes.Time;
                    this._webLoginTime = Time.unscaledTime;
                    this.SetServerList(accountRes.Serverlist);
                    this.eventEmitter.Trigger(EEvents.OnLoginGot, true);
                    HitPointManager.HitPoint("game_serverlist_succss");
                    SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "获取服务器列表成功");
                }
                else //校验失败
                {
                    string errorMsg = string.Format("服务器校验Tokken失败,获取服务器列表失败,错误码:{0}, 错误消息:{1}", accountRes.Ret, accountRes.Errmsg);
                    Hashtable hashtable = new Hashtable();
                    hashtable.Add("reason", errorMsg);
                    HitPointManager.HitPoint("game_serverlist_fail", hashtable);
                    SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.ERROR, errorMsg);

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(12415).words;//"服务器校验用户信息失败,请重试";
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        //重新走登录的流程
                        if (SDKManager.sdk.IsHaveSdk)
                            SDKManager.SDKLogin();
                        else
                            this.LoginWeb();
                    });
                    PromptBoxParameter.Instance.SetCancel(true, () =>
                    {
                        //重新走登录的流程
                        this.eventEmitter.Trigger(EEvents.OnLoginGot, false);
                    });
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

                    //this.eventEmitter.Trigger(EEvents.OnLoginGot, false);
                    Debug.LogError(errorMsg);
                }
#endif

                if (needExtUrlRequest) {
                    WWWGetPkServer();
                }
                
                this.webLoginWebRequest.Dispose();
                this.webLoginWebRequest = null;
            }
        }
#endregion

#region 获取列表服
        private void ServerListTransfer()
        {
            string account = this.urlEncodeAccount;
            string url = string.Format("{0}?account={1}&token={2}&device-id={3}&packagechannel={4}&ts={5}", VersionHelper.DirsvrUrl, this.AccountByChannel(account), SDKManager.GetToken(), SDKManager.GetDeviceId(), SDKManager.GetPublishAppMarket(), TimeManager.ClientNowMillisecond());
            this.serverListWebRequest = UnityWebRequest.Get(url);
            this.serverListWebRequest.timeout = 10;
            DebugUtil.LogFormat(ELogType.eNone, "ServerList:{0}", url);
            UnityWebRequestAsyncOperation requestAsyncOperation = this.serverListWebRequest.SendWebRequest();
            requestAsyncOperation.completed += this.OnServerListGot;
        }
        public void GetServerList()
        {
            if (this.serverListWebRequest != null)
                return;

            //this.nTryGotListIndex = 0;
            // 判断VersionHelper.DirsvrUrl是否合法应该在启动的阶段检验，校验不合法则直接不会进入logic阶段，所以这里代码逻辑不合理
            // 因为editor存在一种情况就是：断网一直可以进入到logic阶段的loginUI，然后此时发现请求服务器列表地的时候存在问题，就是这个url不存在导致的，这是不合理的！
            if (VersionHelper.DirsvrUrl != null)// && VersionHelper.DirsvrUrls.Length > this.nTryGotListIndex)
            {
                this.ServerListTransfer();
            }
            else
            {
                //"获取服务器列表失败"
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(42622));
                this.eventEmitter.Trigger(EEvents.OnGetServerList, false);
            }
        }

        private void OnServerListGot(AsyncOperation operation)
        {
            if (this.serverListWebRequest.isHttpError || this.serverListWebRequest.isNetworkError)
            {
                Debug.LogError(string.Format("OnServerListGot:{0}", this.serverListWebRequest.error));

                //++this.nTryGotListIndex;
                //if (VersionHelper.DirsvrUrls.Length > this.nTryGotListIndex)
                //{
                //    this.ServerListTransfer();
                //}
                //else
                {
                    //"获取服务器列表失败"
                    //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(42622));
                    this.serverListWebRequest.Dispose();
                    this.serverListWebRequest = null;
                    // SetSelectedServerInvalid();
                    this.eventEmitter.Trigger(EEvents.OnGetServerList, false);
                    this.eventEmitter.Trigger(EEvents.OnServerListChanged);
                }
            }
            else
            {
                this.ClearServerList();
                NetMsgUtil.TryDeserialize(ServerListRes.Parser, this.serverListWebRequest.downloadHandler.data, out ServerListRes serverListRes);
                this.SetServerList(serverListRes);
                this.eventEmitter.Trigger(EEvents.OnGetServerList, true);
                this.serverListWebRequest.Dispose();
                this.serverListWebRequest = null;
                
                if (needExtUrlRequest) {
                    WWWGetPkServer();
                }
            }
        }

#endregion

        #region IPDistapch url操作
        
        public void WWWGetIpDistapch() {
            if (ipDispatchWebRequest != null) {
                return;
            }
            
            int key = PlayerPrefs.GetInt("AcceptAgreement", 0);
            if (key != 0) {
                // 首次安装才执行
                return;
            }

            string url = "https://login.movergames.com/activty/getVoicePacketLanguage.shtml";
            this.ipDispatchWebRequest = UnityWebRequest.Get(url);
            this.ipDispatchWebRequest.timeout = 10;

            UnityWebRequestAsyncOperation requestAsyncOperation = this.ipDispatchWebRequest.SendWebRequest();
            requestAsyncOperation.completed += this.OnIpDispatchGot;
        }

        private void OnIpDispatchGot(AsyncOperation operation) {
            (operation).completed -= this.OnIpDispatchGot;
            
            void DoDefault() {
                // 默认设置 1
                OptionManager.Instance.SetInt(OptionManager.EOptionID.VoiceLanguage, 1, false);
            }

            try // 解析
            {
                JsonObject jo = (JsonObject)JsonSerializer.Deserialize(this.ipDispatchWebRequest.downloadHandler.text);
                if (jo.TryGetValue("code", out var jv))
                {
                    string code = ((string)jv);
                    if (code.Equals("e1000", StringComparison.Ordinal))
                    {
                        var lang = (string)(jo["data"]["language"]);
                        if (lang.Equals("zh_HK", StringComparison.Ordinal))
                        {
                            OptionManager.Instance.SetInt(OptionManager.EOptionID.VoiceLanguage, 2, false);
                        }
                        else if (lang.Equals("zh_CN", StringComparison.Ordinal))
                        {
                            DoDefault();
                        }
                    }
                    else
                    {
                        DoDefault();
                    }
                }
                else {
                    DoDefault();
                }
            }
            catch (Exception ex) {
                DoDefault();
            }

            this.ipDispatchWebRequest.Dispose();
            this.ipDispatchWebRequest = null;
            
            PlayerPrefs.SetInt("AcceptAgreement", 1);
        }

        #endregion
        
        #region 其他 url操作

        public void WWWGetPkServer() {
            if (pkServerWebRequest != null) {
                return;
            }

            SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.pkserver.ToString(), out string retUrl);
            if (retUrl == null)
            {
                return;
            }

            string account = this.urlEncodeAccount;
            string url = string.Format("{0}/userlogin?account={1}&token={2}&device-id={3}&packagechannel={4}&ts={5}", retUrl, this.AccountByChannel(account), SDKManager.GetToken(), SDKManager.GetDeviceId(), SDKManager.GetPublishAppMarket(), TimeManager.ClientNowMillisecond());
            Debug.Log($"url: {retUrl}");
            
            this.pkServerWebRequest = UnityWebRequest.Get(url);
            this.pkServerWebRequest.timeout = 10;

            UnityWebRequestAsyncOperation requestAsyncOperation = this.pkServerWebRequest.SendWebRequest();
            requestAsyncOperation.completed += this.OnPkServerGot;
        }

        private void OnPkServerGot(AsyncOperation operation) {
            (operation).completed -= this.OnPkServerGot;
            
            if (this.pkServerWebRequest.isHttpError || this.pkServerWebRequest.isNetworkError) {
                Debug.LogError(string.Format("OnPkServerGot:{0}", this.pkServerWebRequest.error));
            }
            else {
                // 解析
                NetMsgUtil.TryDeserialize(AccountLoginRes.Parser, this.pkServerWebRequest.downloadHandler.data, out AccountLoginRes serverListRes);
                if (serverListRes.Ret == 0) { //校验成功
                    this.MergeServerList(serverListRes.Serverlist);
                }
                else {
                    Debug.LogError("OnPkServerGot 校验失败");
                }
            }
            
            this.pkServerWebRequest.Dispose();
            this.pkServerWebRequest = null;
        }

        #endregion

        // 设置选中的server为维护状态
        private void SetSelectedServerInvalid()
        {
            if (this.mSelectedServer != null)
            {
                this.SetSelected(this.mSelectedServer);
            }
        }
        public void ClearServerList()
        {
            this.mSelectedServer = null;
            this.mServerEntries.Clear();
            this.allServers.Clear();
            this.tabs.Clear();
            this.mRecommendServerEntries.Clear();
            this.mMyServerEntries.Clear();
        }

        public static readonly int COUNT_PER_PAGE = 10;

        public bool IsNewCharacter = true;
        public void SetServerList(ServerListRes serverListRes)
        {
            uint selectedID = this.mSelectedServer == null ? 0 : this.mSelectedServer.mServerInfo.ServerId;
            if (serverListRes != null)
            {
                Dictionary<uint, List<DirRoleInfo>> hasRoleServer = new Dictionary<uint, List<DirRoleInfo>>();
                if (serverListRes.Roles != null)
                {
                    this.IsNewCharacter = serverListRes.Roles.Count <= 0;
                    DebugUtil.LogFormat(ELogType.eNone, "RoleServer Count:{0}", serverListRes.Roles.Count.ToString());
                    for (int i = 0; i < serverListRes.Roles.Count; ++i)
                    {
                        DirRoleInfo dirRoleInfo = serverListRes.Roles[i];
                        if (!hasRoleServer.TryGetValue(dirRoleInfo.RoleServer, out var list))
                        {
                            list = new List<DirRoleInfo>();
                            hasRoleServer.Add(dirRoleInfo.RoleServer, list);
                        }
                        list.Add(dirRoleInfo);
                    }
                }

                void Process(ServerList serverList, bool whitelist)
                {
                    this.mServerEntries.Capacity += serverList.ServerList_.Count;
                    for (int i = 0; i < serverList.ServerList_.Count; ++i)
                    {
                        ServerInfo serverInfo = serverList.ServerList_[i];
                        ServerEntry entry = null;

                        //有角色的服务器
                        if (hasRoleServer.TryGetValue(serverInfo.ServerId, out var list) && list != null)
                        {
                            entry = new ServerEntry(serverInfo, list);
                        }
                        else
                        {
                            entry = new ServerEntry(serverInfo, EmptyList<DirRoleInfo>.Value);
                        }
                        entry.isWhiteList = whitelist;
                        this.mServerEntries.Add(entry);
                        this.allServers[serverInfo.ServerId] = entry;

                        //我的服务器列表插入 （越近的在越前面）
                        if (entry.mRoleInfo.Count > 0)
                        {
                            this.mMyServerEntries.Add(entry);
                        }

                        if (entry.tags.Contains("RECOMMEND"))
                        {
                            this.mRecommendServerEntries.Add(entry);
                        }

                        //重新复制当前选择的服务器
                        if (entry.mServerInfo.ServerId == selectedID)
                        {
                            this.mSelectedServer = entry;
                        }
                    }
                }
                
                if (serverListRes.ServerList != null)
                {
                    NetMsgUtil.TryDeserialize(ServerList.Parser, serverListRes.ServerList, out ServerList serverList);
                    Process(serverList, false);
                }
                if (serverListRes.SeverList2 != null)
                {
                    Process(serverListRes.SeverList2, true);
                }
            }

            // 角色服 登录时间进行排序
            int length = this.mMyServerEntries.Count;
            for (int i = 0; i < length; ++i)
            {
                var ls = this.mMyServerEntries[i].mRoleInfo;
                ls.Sort((ll, rr) =>
                {
                    return (int)(rr.RoleLogin - (long)ll.RoleLogin);
                });
            }
            this.mMyServerEntries.Sort((lEntry, rEntry) =>
            {
                return (int)(rEntry.mRoleInfo[0].RoleLogin - ((long)lEntry.mRoleInfo[0].RoleLogin));
            });

            // 推荐服 开启时间排序
            this.mRecommendServerEntries.Sort((lEntry, rEntry) =>
            {
                return (int)(rEntry.mServerInfo.OpenTime - (int)lEntry.mServerInfo.OpenTime);
            });
            
            NetMsgUtil.TryDeserialize(ServerGroups.Parser, serverListRes.GroupInfo, out ServerGroups groups);
            length = groups.Groups.Count;
            for (int i = 0; i < length; ++i)
            {
                var one = groups.Groups[i];
                this.tabs[one.SortNum] = one;
            }

            if (this.mSelectedServer == null)
            {
                this.SetDefaultServer();
            }

            this.eventEmitter.Trigger(EEvents.OnServerListChanged);
        }

        public void MergeServerList(ServerListRes serverListRes) {
            SetServerList(serverListRes);
        }

        public void SetDefaultServer()
        {
            if (this.mMyServerEntries.Count > 0)
            {
                this.SetSelected(this.mMyServerEntries[0]);
            }
            else if (this.mRecommendServerEntries.Count > 0)
            {
                this.SetSelected(this.mRecommendServerEntries[0]);
            }
            else if (this.mServerEntries.Count > 0)
            {
                this.SetSelected(this.mServerEntries[this.mServerEntries.Count - 1]);
            }
        }

        public void SetSelected(ServerEntry entry)
        {
            //if (mSelectedServer != entry)
            {
                this.mSelectedServer = entry;
                this.selectedServerId = (int)entry.mServerInfo.ServerId;
                if (entry.mRoleInfo.Count > 0)
                {
                    // 默认选中第一个角色
                    this.selectedRoleId = entry.mRoleInfo[0].RoleId;
                }
                else
                {
                    this.selectedRoleId = 0;
                }

                this.eventEmitter.Trigger(EEvents.OnSelectServerChanged);
            }
        }

        public ServerEntry FindServerEntryByID(uint id)
        {
            for (int i = 0; i < this.mServerEntries.Count; ++i)
            {
                var serverInfo = this.mServerEntries[i].mServerInfo;
                if (serverInfo != null && serverInfo.ServerId == id)
                    return this.mServerEntries[i];
            }
            return null;
        }
        public ServerInfo FindServerInfoByID(uint id)
        {
            return this.FindServerEntryByID(id)?.mServerInfo;
        }

        public ServerInfo GetServerInfo()
        {
            return this.FindServerEntryByID((uint)this.selectedServerId)?.mServerInfo;
        }

        public bool NeedRefresh()
        {
            if (this.serverListWebRequest != null)
                return false;

            float time = Time.time;
            if (time >= this.fTimePoint + this.fRefreshInterval)
            {
                this.fTimePoint = time;
                return true;
            }
            return false;
        }

        public bool HasMainTitle(ServerEntry serverEntry, bool toTipWhenValid = false) {
            var mainTitle = serverEntry.MainTitle;
            bool valid = (!string.IsNullOrEmpty(mainTitle));
            valid &= (serverEntry.mServerInfo.Status == (int)ServerStatus.Maintaining);
            valid &= (Sys_Login.Instance.Extensiondata != null &&
                      Sys_Login.Instance.Extensiondata.Length >= 1 && 
                      Sys_Login.Instance.Extensiondata[0] == '0');

            if (valid && toTipWhenValid) {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = mainTitle;
                PromptBoxParameter.Instance.SetConfirm(true, null);
                PromptBoxParameter.Instance.SetCancel(false, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }

            return valid;
        }
    }
}