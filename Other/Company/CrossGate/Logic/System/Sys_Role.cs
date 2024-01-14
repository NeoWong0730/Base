using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Lib.AssetLoader;
using Lib.Core;
using LitJson;
using Logic.Core;
using Net;
using Packet;
using Table;

#if !USE_OLDSDK
using com.kwai.game.features;
#endif

namespace Logic
{
    #region 上报角色相关

    public enum ReportRelation
    {
        SPOUSE = 0,
        SWORN,
        LOVERS,
        MENTORSHIP,
        ENEMY,
        OTHER
    }

    public class ReportFriend
    {
        //角色关系、亲密度、角色ID
        public ReportRelation relation;
        public long intimacy;
        public long roleId;

        public ReportFriend(ReportRelation relation, long intimacy, long roleId)
        {
            this.relation = relation;
            this.intimacy = intimacy;
            this.roleId = roleId;
        }
    }

    public class ReportCoin
    {
        //货币ID、货币数量、货币名称
        public int coinId;
        public int coinNum;
        public String coinName;

        public ReportCoin(int coinId, int coinNum, String coinName)
        {
            this.coinId = coinId;
            this.coinNum = coinNum;
            this.coinName = coinName;
        }
    }

    public class ReportModel
    {
        public string reportState;
        public String serverId;
        public String serverName;
        public String roleId;
        public String roleName;
        public String roleLevel;

        //必填 帮会相关
        public long gangId;
        //public String gangName;
        //public long gangTitleId;
        //public String gangTitle;


        //必填 职业相关
        public long roleProfId;
        //public String roleProf;
        //public long roleProfTitleId;
        //public String roleProfTitle;


        // 以下内容为选填，如游戏中有相关数据，建议上报
        public long roleCreateTime; //选填 创建时间
        public long roleLevelUpTime; // 选填 等级升级时间

        //游戏角色上报接口新增字段 --社区需要，但不是必须的
        //nickName   //玩家昵称
        //avatarUrl  //玩家头像icon url 地址
    }

    public class IOSReportModel
    {
        public string gameUserId;
        public string serverId;
        public string zoneId;
        //public string gameScene;
        public string level;
        public string serverName;
        public string roleId;
        public string roleName;
        //public string rolePower;
        //public string avatarUrl;
        public string reportType;
        public string roleCreateTime = "0";
        public string roleLevelUpTime = "0";
    }
    public enum IOSReportType
    {
        KwaiGameReportTypeCreate, // 角色创建
        KwaiGameReportTypeLogin, // 角色登录
        KwaiGameReportTypeLevel, // 角色升级
        KwaiGameReportTypeExit // 角色退出
    }

    #endregion


    public class Sys_Role : SystemModuleBase<Sys_Role>, ISystemModuleApplicationPause
    {
        public bool hasEnterGame = false;
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnUpdateCareerRank,
            OnUpdateOpenServiceDay,
            OnLoginQueueNtf,
            OnLoginRes,
            OnSyncFinished,
            OnCrossDay,
            OnReName,
            OnClickWorldLevel,
            OnUpdateCrossSrvState,
        }

        public enum EClientState
        {
            None = 0, //无
            CatchPet = 1, //捕捉宠物
            Guide = 2, //引导开关
            Hangup = 3, //挂机巡逻
            ExecTask = 4, //执行任务
        }

        #region 临时数据,进入teamplayer方式

        public enum EEnterTeamPlayerType
        {
            Scene,
            Friend,
        }

        public EEnterTeamPlayerType enterType = EEnterTeamPlayerType.Scene;

        #endregion

        /// <summary>
        /// 断线重连key
        /// </summary>
        public uint ReconnKey = 0;

        /// <summary>
        /// 角色数据
        /// </summary>
        public RoleBase Role;

        /// <summary>
        /// 开服天数
        /// </summary>
        public uint openServiceDay = 0;

        /// <summary>
        /// 开服时间（不带时区）
        /// </summary>
        public uint openServiceTime = 0;

        /// <summary>
        /// 开服时间（带服务器时区）
        /// </summary>
        public uint openServiceGameTime = 0;

        /// <summary>
        /// 上次离线时间
        /// </summary>
        public uint lastOffTime = 0;

        /// <summary>
        /// 职业等级
        /// </summary>
        public uint CareerRank = 0;

        /// <summary>
        /// Proto Version
        /// </summary>
        public uint ProtoVersion = 0;

        /// <summary>
        /// 记录是否已经进入游戏
        /// </summary>
        public bool FirstEnterGame = false;

        /// <summary>
        /// 服务器发送的在线总时常
        /// </summary>
        public uint OnlineTime = 0u;

        /// <summary>
        /// 用户分层奖励是否领取
        /// </summary>
        public bool UserPartitionGiftIsGet = true;

        /// <summary>
        /// 玩家是否处于跨服
        /// </summary>
        public bool isCrossSrv = false;

        /// <summary>
        /// 是否在后台运行
        /// </summary>
        private bool IsPause = false;

        public bool IsMenuActivityExpand = true;

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public uint lastLoginTime = 0;

        /// <summary>
        /// 改名数据
        /// </summary>
        public RoleRenameData reNameData;

        public bool isReNameSend = false;

        /// <summary>
        /// 需要绑定手机bool
        /// </summary>
        public bool IsNeedBindPhone = false;
        public bool IsBackRole=false;

        /// <summary>
        /// 上一次点击时候的开服天数 一开始没有数据时是0
       /// </summary>
    public uint lastClickOpenServiceDay ;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.LoginReq, (ushort) CmdRole.LoginRes,
                this.OnLoginRes, CmdRoleLoginRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.EnterGameNtf, this.OnEnterGameNtf,
                CmdRoleEnterGameNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.ExitGameReq, (ushort) CmdRole.ExitGameRes,
                this.OnExitGameRes, null);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.PromoteCareerRankReq,
                (ushort) CmdRole.PromoteCareerRankRes, this.PromoteCareerRankRes, CmdRolePromoteCareerRankRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.CareerUpInfoReq,
                (ushort) CmdRole.CareerUpInfoRes, this.CareerUpInfoRes, CmdRoleCareerUpInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.FinishEnterGameNtf, this.OnFinishEnterGameNtf,
                CmdRoleFinishEnterGameNtf.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.SettingNtf, OnSettingNtf, CmdRoleSettingNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.ReconnectReq, (ushort) CmdRole.ReconnectRes,
                this.OnReconnectRes, CmdRoleReconnectRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.OpenServiceDayNtf, this.OnOpenServiceDayNtf,
                CmdRoleOpenServiceDayNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.WjxinfoNtf, this.OnWjxinfoNtf,
                CmdRoleWJXInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.ExchangeGiftReq,
                (ushort) CmdRole.ExchangeGiftRes, this.OnExchangeGiftReq, CmdRoleExchangeGiftRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.ClientStateNtf, this.OnClientStateNtf,
                CmdRoleClientStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.LoginQueueInfoReq,
                (ushort) CmdRole.LoginQueueNtf, this.OnLoginQueueNtf, CmdRoleLoginQueueNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.CrossDayNtf, this.OnCrossDay,
                CmdRoleCrossDayNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.UpdateCrossState, this.OnUpdateCrossState,
                CmdRoleUpdateCrossState.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.BindPhoneSwitchNtf, this.BindPhoneSwitchNtf,
                CmdRoleBindPhoneSwitchNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.RenameReq, (ushort) CmdRole.RenameRes,
                this.OnReNameRes, CmdRoleRenameRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdRole.RenameExpireReq,
                (ushort) CmdRole.RenameExpireRes, this.OnReNameExpireRes, CmdRoleRenameExpireRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdRole.NeedBindPhoneNtf, this.NeedBindPhoneNtf,
                CmdRoleBindPhoneSwitchNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.ChangeCareerReq, (ushort)CmdRole.ChangeCareerNty, this.OnChangeCareerNtf, CmdRoleChangeCareerNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.ClickWorldLevelReq,
          (ushort)CmdRole.ClickWorldLevelRes, this.OnClickWorldLevelRes, CmdRoleClickWorldLevelRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.TeamLeaderInvitePromoteCareerNtf, this.TeamLeaderInvitePromoteCareerNtf,
          CmdRoleTeamLeaderInvitePromoteCareerNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.TeamPromoteCareerAgreeOrCancelNtf, this.TeamPromoteCareerAgreeOrCancelNtf,
          CmdRoleTeamPromoteCareerAgreeOrCancelNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdRole.TeamLeaderCheckPromoteCareerReq, (ushort)CmdRole.TeamLeaderCheckPromoteCareerRes, this.OnTeamLeaderCheckPromoteCareerRes, 
                CmdRoleTeamLeaderCheckPromoteCareerRes.Parser);

            SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKExitGame, this.ExitGameReq, true);
            SDKManager.eventEmitter.Handle<SDKManager.SDKReportState>(SDKManager.ESDKLoginStatus.onSDKExitReportExtraData, this.SDKAccountReportExtraData, true);
            SDKManager.eventEmitter.Handle<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, this.OnSDKHintMsg, true);
            SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKJailBreakIOS, OnSDKJailBreakIOS, true);

            this.ProtoVersion = uint.Parse(CSVParam.Instance.GetConfData(529).str_value);
            //uint.Parse(CSVParam.Instance.GetConfData(529).str_value);
            //this.ProtoVersion = 82;
            this.Role = null;
            this.IsNeedBindPhone = false;
        }

        public override void OnLogin()
        {
            
            #if !UNITY_EDITOR
                RandomFivePercent();
                //GetGMWebData();
#endif
            this.IsBackRole = false;
        }

        public override void OnLogout()
        {
            //this.Role = null;
        }

        public override void Dispose()
        {
            //this._timerFPS?.Cancel();
            //this._timerFPS = null;

            this._timerHitPoint?.Cancel();
            this._timerHitPoint = null;
            this.IsNeedBindPhone = false;
            SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKExitGame, this.ExitGameReq, false);
            SDKManager.eventEmitter.Handle<SDKManager.SDKReportState>(SDKManager.ESDKLoginStatus.onSDKExitReportExtraData, this.SDKAccountReportExtraData, false);
            SDKManager.eventEmitter.Handle<string>(SDKManager.ESDKLoginStatus.OnSDKHintMsg, this.OnSDKHintMsg, false);
            SDKManager.eventEmitter.Handle(SDKManager.ESDKLoginStatus.OnSDKJailBreakIOS, OnSDKJailBreakIOS, false);
            base.Dispose();
        }

        public void OnApplicationPause(bool pause)
        {
            this.IsPause = pause;
        }


        #region login

        public bool GotLoginRes { get; private set; } = false;

        public void ReqLogin(string account, ulong roleId, int serverId)
        {
            //Start Game
            if (!string.IsNullOrWhiteSpace(account))
            {
#if UNITY_EDITOR && SKIP_SDK_Login
                CmdRoleLoginReq req = new CmdRoleLoginReq();
                req.AccountType = SDKManager.iAccountType;// 默认填写16即可
                req.Signature = FrameworkTool.ConvertToGoogleByteString(SkipSDKLogin.Instance.Signature);//签名通过服务器工具获取
                req.Time = SkipSDKLogin.Instance.Time;//时间通过服务器工具获取
                req.Account = FrameworkTool.ConvertToGoogleByteString(SkipSDKLogin.Instance.Account);//以机器人身份登入
                req.ServerId = SkipSDKLogin.Instance.ServerId; //serverId 通过链接获取
                req.Protover = ProtoVersion;
                req.RoleId = SkipSDKLogin.Instance.Roleid;
               //req.Duntoken = FrameworkTool.ConvertToGoogleByteString(SDKManager.sdk.WangYiToken);
#else
                CmdRoleLoginReq req = new CmdRoleLoginReq();
                req.AccountType = SDKManager.iAccountType;
                req.Signature = Sys_Login.Instance.Signature;
                req.Extensiondata = Sys_Login.Instance.Extensiondata;
                req.Time = Sys_Login.Instance.SdkLoginTIme;
                req.Account = FrameworkTool.ConvertToGoogleByteString(account);
                req.ServerId = serverId; // (int)Sys_Login.Instance.mSelectedServer.mServerInfo.ServerId;
                req.Protover = this.ProtoVersion;
                req.RoleId = roleId;
                //req.Duntoken = FrameworkTool.ConvertToGoogleByteString(SDKManager.sdk.WangYiToken);
#if UNITY_IOS
                req.IsIOSBreak = FrameworkTool.ConvertToGoogleByteString(SDKManager.sdk.bIsJailbreak);//判断是否越狱（ios平台）
#endif

#endif

                //客户端需要发给服务器的打点信息
                HitPointManager.UnityHitPointBaseData unityBaseData = HitPointManager.GetUnityHitPointBaseData();
                unityBaseData.test_id = CSVParam.Instance.GetConfData(1364).str_value;
                req.NetWork = FrameworkTool.ConvertToGoogleByteString(unityBaseData.network);
                req.SystemVersion = FrameworkTool.ConvertToGoogleByteString(unityBaseData.system_version);
                req.PhoneModel = FrameworkTool.ConvertToGoogleByteString(unityBaseData.phone_model);
                req.ScreenWidth = unityBaseData.screen_width;
                req.PixelDensity = unityBaseData.pixel_density;
                req.Cpu = FrameworkTool.ConvertToGoogleByteString(unityBaseData.cpu);
                req.MemorySize = unityBaseData.memory_size;
                req.IsTestAccount = FrameworkTool.ConvertToGoogleByteString(unityBaseData.account_type);
                req.TestId = FrameworkTool.ConvertToGoogleByteString(unityBaseData.test_id);
                DebugUtil.LogFormat(ELogType.eHotFix,
                    string.Format(
                        "NetWork:{0} SystemVersion:{1} PhoneModel:{2} ScreenWidth:{3} PixelDensity:{4} Cpu:{5} MemorySize:{6} IsTestAccount:{7} TestId:{8}",
                        unityBaseData.network,
                        unityBaseData.system_version,
                        unityBaseData.phone_model,
                        unityBaseData.screen_width,
                        unityBaseData.pixel_density,
                        unityBaseData.cpu,
                        unityBaseData.memory_size,
                        unityBaseData.account_type,
                        unityBaseData.test_id
                    ));


                HitPointManager.SDKHitPointBaseData sdkBaseData = HitPointManager.GetSDKHitPointBaseData();
                req.PackageChannel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.markert_channel);
                req.GameUserId = FrameworkTool.ConvertToGoogleByteString(SDKManager.GetUID());
                req.DeviceId = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.device_id);
                req.AppVersion = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.app_version);
                req.Platform = uint.Parse(sdkBaseData.platform);
                req.Operation = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.operator_type);
                req.Channel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.channel);
                req.DeviceType = (uint) SDKManager.GetDeviceType();

                DebugUtil.LogFormat(ELogType.eHotFix,
                    string.Format(
                        "PackageChannel:{0} GameUserId:{1} DeviceId:{2} AppVersion:{3} Platform:{4} Operation:{5} Channel:{6} DeviceType:{7}",
                        sdkBaseData.markert_channel,
                        SDKManager.GetUID().ToString(),
                        sdkBaseData.device_id,
                        sdkBaseData.app_version,
                        sdkBaseData.platform,
                        sdkBaseData.operator_type,
                        sdkBaseData.channel,
                        req.DeviceType
                    ));

                SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "发起游戏登录");
                NetClient.Instance.SendMessage((ushort) CmdRole.LoginReq, req);
                DebugUtil.LogFormat(ELogType.eNone, "Login Server:{0}", account);
            }
            else
            {
                DebugUtil.LogError("No Account");
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000015));
            }

            this.GotLoginRes = false;
            this.syncCount = 0;
        }

        private void OnLoginRes(NetMsg msg)
        {
            //初次同步时间 和 时区信息
            CmdRoleLoginRes res = NetMsgUtil.Deserialize<CmdRoleLoginRes>(CmdRoleLoginRes.Parser, msg);
            Framework.TimeManager.CorrectServerTime(res.ServerTime);
            Framework.TimeManager.CorrectServerTimeZone(res.Timezone);

            //赋值加密的key
            //NetMsgUtil.CryptKey = (byte)res.OriginCryptoKey;

            this.hasEnterGame = false;
            this.FirstEnterGame = Sys_Login.Instance.selectedRoleId == 0u;

            Sys_Login.Instance.RealServerID = res.InsSvrId;

            if (this.FirstEnterGame)
            {
                // 进入创角场景，进行创角
                LevelManager.EnterLevel(typeof(LvCreateCharacter));
            }
            else
            {
                //打点上报：非创建角色进入游戏
                //HitPointManager.HitPoint("game_server_login");
            }

            SDKManager.SDKPrintLog(SDKManager.ESDKLogLevel.INFO, "游戏登录成功");
            this.eventEmitter.Trigger(EEvents.OnLoginRes);

            IsMenuActivityExpand = true;
        }

        public void ReconnectReq()
        {
            CmdRoleReconnectReq req = new CmdRoleReconnectReq();
            req.ReconnKey = this.ReconnKey;
            req.RoleId = Sys_Role.Instance.Role.RoleId;
            req.SvrId = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerId;
            req.Protover = Sys_Role.Instance.ProtoVersion;

            //客户端需要发给服务器的打点信息
            HitPointManager.UnityHitPointBaseData unityBaseData = HitPointManager.GetUnityHitPointBaseData();
            unityBaseData.test_id = CSVParam.Instance.GetConfData(1364).str_value;
            req.ExtraData = new RoleExtraData();
            req.ExtraData.NetWork = FrameworkTool.ConvertToGoogleByteString(unityBaseData.network);
            req.ExtraData.SystemVersion = FrameworkTool.ConvertToGoogleByteString(unityBaseData.system_version);
            req.ExtraData.PhoneModel = FrameworkTool.ConvertToGoogleByteString(unityBaseData.phone_model);
            req.ExtraData.ScreenWidth = unityBaseData.screen_width;
            req.ExtraData.PixelDensity = unityBaseData.pixel_density;
            req.ExtraData.Cpu = FrameworkTool.ConvertToGoogleByteString(unityBaseData.cpu);
            req.ExtraData.MemorySize = unityBaseData.memory_size;
            req.ExtraData.IsTestAccount = FrameworkTool.ConvertToGoogleByteString(unityBaseData.account_type);
            req.ExtraData.TestId = FrameworkTool.ConvertToGoogleByteString(unityBaseData.test_id);
            DebugUtil.LogFormat(ELogType.eHotFix,
                string.Format(
                    "NetWork:{0} SystemVersion:{1} PhoneModel:{2} ScreenWidth:{3} PixelDensity:{4} Cpu:{5} MemorySize:{6} IsTestAccount:{7} TestId:{8}",
                    unityBaseData.network,
                    unityBaseData.system_version,
                    unityBaseData.phone_model,
                    unityBaseData.screen_width,
                    unityBaseData.pixel_density,
                    unityBaseData.cpu,
                    unityBaseData.memory_size,
                    unityBaseData.account_type,
                    unityBaseData.test_id
                ));

            HitPointManager.SDKHitPointBaseData sdkBaseData = HitPointManager.GetSDKHitPointBaseData();
            req.ExtraData.PackageChannel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.markert_channel);
            //req.ExtraData.GameUserId = FrameworkTool.ConvertToGoogleByteString(SDKManager.GetUID());
            req.ExtraData.DeviceId = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.device_id);
            req.ExtraData.AppVersion = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.app_version);
            req.ExtraData.Platform = uint.Parse(sdkBaseData.platform);
            req.ExtraData.Operation = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.operator_type);
            req.ExtraData.Channel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.channel);
            req.ExtraData.DeviceType = (uint) SDKManager.GetDeviceType();

            DebugUtil.LogFormat(ELogType.eHotFix,
                string.Format(
                    "PackageChannel:{0} GameUserId:{1} DeviceId:{2} AppVersion:{3} Platform:{4} Operation:{5} Channel:{6} DeviceType:{7}",
                    sdkBaseData.markert_channel,
                    SDKManager.GetUID().ToString(),
                    sdkBaseData.device_id,
                    sdkBaseData.app_version,
                    sdkBaseData.platform,
                    sdkBaseData.operator_type,
                    sdkBaseData.channel,
                    req.ExtraData.DeviceType
                ));

            NetClient.Instance.SendMessage((ushort) CmdRole.ReconnectReq, req);
        }

        private void OnReconnectRes(NetMsg msg)
        {
            CmdRoleReconnectRes res = NetMsgUtil.Deserialize<CmdRoleReconnectRes>(CmdRoleReconnectRes.Parser, msg);

            //修正时间
            Framework.TimeManager.CorrectServerTime(res.ServerTime);

            bool result = res.Ret == 0;
            Sys_Net.Instance.ReconnectResult(result);
        }

        private void OnOpenServiceDayNtf(NetMsg msg)
        {
            CmdRoleOpenServiceDayNtf res =
                NetMsgUtil.Deserialize<CmdRoleOpenServiceDayNtf>(CmdRoleOpenServiceDayNtf.Parser, msg);
            this.openServiceDay = res.OpenServiceDay;
            this.eventEmitter.Trigger(Sys_Role.EEvents.OnUpdateOpenServiceDay);
            DebugUtil.LogFormat(ELogType.eNone, "OpenServiceDayUpdata{0}", res.OpenServiceDay);
        }

        #endregion

        #region EnterGame

        public void ReqCreateCharacter(string accountName, uint heroId)
        {
            CmdRoleCreateReq req = new CmdRoleCreateReq();
            req.Name = FrameworkTool.ConvertToGoogleByteString(accountName);
            req.HeroId = heroId;
#if UNITY_IOS
            req.IsIOSBreak = FrameworkTool.ConvertToGoogleByteString(SDKManager.sdk.bIsJailbreak);//判断是否越狱（ios平台）
#endif

            ////客户端需要发给服务器的打点信息
            //HitPointManager.UnityHitPointBaseData unityBaseData = HitPointManager.GetUnityHitPointBaseData();
            //unityBaseData.test_id = CSVParam.Instance.GetConfData(1364).str_value;
            //req.ExtraData = new RoleExtraData();
            //req.ExtraData.NetWork = FrameworkTool.ConvertToGoogleByteString(unityBaseData.network);
            //req.ExtraData.SystemVersion = FrameworkTool.ConvertToGoogleByteString(unityBaseData.system_version);
            //req.ExtraData.PhoneModel = FrameworkTool.ConvertToGoogleByteString(unityBaseData.phone_model);
            //req.ExtraData.ScreenWidth = unityBaseData.screen_width;
            //req.ExtraData.PixelDensity = unityBaseData.pixel_density;
            //req.ExtraData.Cpu = FrameworkTool.ConvertToGoogleByteString(unityBaseData.cpu);
            //req.ExtraData.MemorySize = unityBaseData.memory_size;
            //req.ExtraData.IsTestAccount = FrameworkTool.ConvertToGoogleByteString(unityBaseData.account_type);
            //req.ExtraData.TestId = FrameworkTool.ConvertToGoogleByteString(unityBaseData.test_id);
            //DebugUtil.LogFormat(ELogType.eHotFix,
            //    string.Format(
            //        "NetWork:{0} SystemVersion:{1} PhoneModel:{2} ScreenWidth:{3} PixelDensity:{4} Cpu:{5} MemorySize:{6} IsTestAccount:{7} TestId:{8}",
            //        unityBaseData.network,
            //        unityBaseData.system_version,
            //        unityBaseData.phone_model,
            //        unityBaseData.screen_width,
            //        unityBaseData.pixel_density,
            //        unityBaseData.cpu,
            //        unityBaseData.memory_size,
            //        unityBaseData.account_type,
            //        unityBaseData.test_id
            //    ));

            //HitPointManager.SDKHitPointBaseData sdkBaseData = HitPointManager.GetSDKHitPointBaseData();
            //req.ExtraData.PackageChannel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.packagechannel);
            ////req.ExtraData.GameUserId = FrameworkTool.ConvertToGoogleByteString(SDKManager.GetUID());
            //req.ExtraData.DeviceId = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.device_id);
            //req.ExtraData.AppVersion = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.app_version);
            //req.ExtraData.Platform = uint.Parse(sdkBaseData.platform);
            //req.ExtraData.Operation = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.operator_type);
            //req.ExtraData.Channel = FrameworkTool.ConvertToGoogleByteString(sdkBaseData.channel);
            //req.ExtraData.DeviceType = (uint) SDKManager.GetDeviceType();

            //DebugUtil.LogFormat(ELogType.eHotFix,
            //    string.Format(
            //        "PackageChannel:{0} GameUserId:{1} DeviceId:{2} AppVersion:{3} Platform:{4} Operation:{5} Channel:{6} DeviceType:{7}",
            //        sdkBaseData.packagechannel,
            //        SDKManager.GetUID().ToString(),
            //        sdkBaseData.device_id,
            //        sdkBaseData.app_version,
            //        sdkBaseData.platform,
            //        sdkBaseData.operator_type,
            //        sdkBaseData.channel,
            //        req.ExtraData.DeviceType
            //    ));

            NetClient.Instance.SendMessage((ushort) CmdRole.CreateReq, req);
        }

        private void OnCreateRes(NetMsg msg)
        {
        }

        private void OnEnterGameNtf(NetMsg msg)
        {
            CmdRoleEnterGameNtf res = NetMsgUtil.Deserialize<CmdRoleEnterGameNtf>(CmdRoleEnterGameNtf.Parser, msg);
            this.Role = res.Role;
            this.ReconnKey = res.ReconnKey;
            this.sRoleName = this.Role.Name.ToStringUtf8();
            this.openServiceDay = res.OpenServiceDay;
            this.openServiceTime = res.OpenTime;
            this.openServiceGameTime = res.OpenTimeGame;
            this.lastOffTime = res.LastOffTime;
            this.CareerRank = res.Role.CareerRank;
            this.exp = this.Role.Exp;
            this.LoginDay = res.LoginDay;
            this.OnlineTime = res.OnlineTime;
            this.UserPartitionGiftIsGet = res.LayerReward;
            this.isCrossSrv = res.BCrossSvr;
            this.lastLoginTime = res.LastLoginTime;
            this.lastClickOpenServiceDay = res.LastClickOpenServiceDay;
            //Sys_OperationalActivity.Instance.BindPhoneSwitch = res.BindPhoneSwitch;
            Sys_OperationalActivity.Instance.BindPhoneTakeWard = res.BindPhone;
            DebugUtil.LogFormat(ELogType.eNone, "OpenServiceDayEnterGame{0}", res.OpenServiceDay);

            if (res.CreateEnter)
                HitPointManager.HitPoint("game_createrole_success");
            HitPointManager.HitPoint("game_server_login");

            SDKSetCommonReportParam();

#if UNITY_ANDROID || UNITY_IOS
            //渠道要求登陆上报人物信息
            if (res.CreateEnter)
                this.SDKAccountReportExtraData(SDKManager.SDKReportState.CREATE);
       
            this.SDKAccountReportExtraData(SDKManager.SDKReportState.LOGIN);
#endif
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnAddExp);
            Sys_Attr.Instance.SetExpContent();
            Sys_SecureLock.Instance.SecureLockInitReq(); //安全锁请求数据

            if (!this.hasEnterGame)
            {
                SystemModuleManager.OnLogin();
                SDKManager.bPaiLianTu = true;
#if UNITY_IPHONE
                SDKManager.SDKPay_AutoRepairRecoder_RegistDelete();
#endif
#if UNITY_STANDALONE_WIN
                SDKManager.ThirdACESdkLogin(this.Role.RoleId.ToString(), Sys_Login.Instance.selectedServerId);
#endif
            }

            Logic.Core.UIScheduler.OnLogin();

            this.hasEnterGame = true;
            
            Sys_Login.Instance.WWWGetIpDistapch();
            Sys_BackAssist.Instance.LovePointDataInit();
#if UNITY_IOS
            //进入游戏判断IOS越狱
            OnSDKJailBreakIOS();
#endif
        }

        public void ExitGameReq()
        {
            //切换角色，推出游戏等都需要重新获取网易Tokken
            //SDKManager.SDKGetWangYiToken();
            Sys_Role.Instance.SDKAccountReportExtraData(SDKManager.SDKReportState.EXIT);

            CmdRoleExitGameReq req = new CmdRoleExitGameReq();
            NetClient.Instance.SendMessage((ushort) CmdRole.ExitGameReq, req);

            if (this.hasEnterGame)
            {
                SystemModuleManager.OnLogout();
                Sys_Login.Instance.ClearServerList();
                AudioUtil.StopAll();
#if UNITY_STANDALONE_WIN
                SDKManager.ACESDKLoginOff();
#endif
            }

            GameCenter.Dispose();
            //UIManager.ClearUI();
            this.CloseStackIgnoreUI();

            LevelManager.EnterLevel(typeof(LvLogin));

            this.hasEnterGame = false;
            this._timerHitPoint?.Cancel();
            this._timerHitPoint = null;
        }

        /// <summary>
        /// 角色信息更新（可以选择退出方式）
        /// </summary>
        /// <param name="reportState">登录、创角、升级、退出</param>
        /// <param name="exitIsLogout"></param>
        public void SDKAccountReportExtraData(SDKManager.SDKReportState reportState)
            //SDKManager.ExitGameState exitGameState = SDKManager.ExitGameState.None)
        {
            //说明：exitIsLogout 退出只有2种
            //1.ForceLogout 需要登出sdk,并且回到登录界面
            //2.杀掉进程app 需要上报
            //3.所有角色的退出 android平台都要上报
            if (!SDKManager.sdk.IsHaveSdk) return;
#if UNITY_ANDROID || UNITY_IOS
            DebugUtil.LogFormat(ELogType.eNone, string.Format("SDKAccountReportExtraData:{0}", reportState));
            //进入游戏大状态
            if (reportState != SDKManager.SDKReportState.EXIT)
            {
                RoleData role = new RoleData();
                role.RoleId = this.RoleId.ToString();
                role.ServerId = Sys_Login.Instance.selectedServerId.ToString();
                role.ServerName = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;
                role.RoleLevel = this.Role.Level.ToString();
                role.RoleName = this.sRoleName;
                //role.RolePower = this.Role.战斗力

                switch (reportState)
                {
                    case SDKManager.SDKReportState.CREATE:
                        role.UpdateTiming = UpdateRoleDataTiming.createRole;
                        break;
                    case SDKManager.SDKReportState.LOGIN:
                        //优先上报 公共埋点 用于在打点参数中携带 服务器、游戏用户id 等数据,传入后SDK上报埋点信息时会携带这些公共参数
                        role.UpdateTiming = UpdateRoleDataTiming.login;
                        break;
                    case SDKManager.SDKReportState.LEVEL:
                        role.UpdateTiming = UpdateRoleDataTiming.levelUpgrade;
                        break;
                }
                SDKManager.SDKUpdateRoleData(role);
                DebugUtil.LogFormat(ELogType.eNone, "SDKAccountReportExtraData:" + JsonMapper.ToJson(role));
            }
            else
            {
                if ((LevelManager.mCurrentLevelType != typeof(LvLogin) && LevelManager.mCurrentLevelType != typeof(LvCreateCharacter)))
                {
                    RoleData role = new RoleData();
                    role.RoleId = this.RoleId.ToString();
                    role.ServerId = Sys_Login.Instance.selectedServerId.ToString();
                    role.ServerName = Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;
                    role.RoleLevel = this.Role.Level.ToString();
                    role.RoleName = this.sRoleName;
                    //role.RolePower = this.Role.战斗力
                    role.UpdateTiming = UpdateRoleDataTiming.exitGame;
                    SDKManager.SDKUpdateRoleData(role);
                    DebugUtil.LogFormat(ELogType.eNone, "SDKAccountReportExtraData:" + JsonMapper.ToJson(role));
                }

                //switch (exitGameState)
                //{
                //    case SDKManager.ExitGameState.None:
                //        break;
                //    case SDKManager.ExitGameState.LogoutBackToLogin:
                //        ExitGameReq();
                //        SDKManager.SDKLogout();
                //        break;
                //    case SDKManager.ExitGameState.ExitApp:
                //        SDKManager.SDKExitGame();
                //        break;
                //}
            }
#endif
        }


        private void OnSDKHintMsg(string hintMsg)
        {
            Sys_Hint.Instance.PushContent_Normal(hintMsg);//LanguageHelper.GetTextContent(lanId)
        }

        /// <summary>
        /// IOS 越狱
        /// </summary>
        private void OnSDKJailBreakIOS()
        {
#if UNITY_IOS
            DebugUtil.LogErrorFormat(string.Format("hasEnterGame:{0} bIsJailbreak:{1}", hasEnterGame, SDKManager.sdk.bIsJailbreak));
            if (this.hasEnterGame && SDKManager.sdk.bIsJailbreak == "1")
                RoleKickReq(0, SDKManager.sdk.bIsJailbreak);
#endif
        }

        /// <summary>
        /// 请求踢人
        /// </summary>
        /// <param name="kickType"></param>
        /// <param name="param"></param>
        private void RoleKickReq(uint kickType, string param)
        {
            CmdRoleKickReq req = new CmdRoleKickReq();
            req.KickType = kickType;
            req.Param = FrameworkTool.ConvertToGoogleByteString(param);
            NetClient.Instance.SendMessage((ushort)CmdRole.KickReq, req);
        }

        /// <summary>
        /// 公共参数上报，角色登陆后上报
        /// </summary>
        public void SDKSetCommonReportParam()
        {
            DebugUtil.LogFormat(ELogType.eSdk, "SDKCommonHitPoint:");
#if UNITY_ANDROID || UNITY_IOS
            CommonReport commonReport = new CommonReport();
            commonReport.UserId = RoleId.ToString();
            commonReport.ServerId = Sys_Login.Instance.selectedServerId.ToString();
            commonReport.ZoneId = SDKManager.GetZoneid().ToString();
            commonReport.Level = Role.Level.ToString();
            //commonReport.Scenes

            SDKManager.SDKSetCommonReportParam(commonReport);
#endif
        }


        private void OnExitGameRes(NetMsg msg)
        {
            //ExitToLogin();
        }

        public void PromoteCareerRankReq(uint agree)   //1-同意  0- 不同意
        {
            CmdRolePromoteCareerRankReq req = new CmdRolePromoteCareerRankReq();
            req.AgreePromote = agree;
            NetClient.Instance.SendMessage((ushort) CmdRole.PromoteCareerRankReq, req);
        }

        public void CareerUpInfoReq(uint index)
        {
            CmdRoleCareerUpInfoReq req = new CmdRoleCareerUpInfoReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort) CmdRole.CareerUpInfoReq, req);
        }

        private void PromoteCareerRankRes(NetMsg msg)
        {
            CmdRolePromoteCareerRankRes res =
                NetMsgUtil.Deserialize<CmdRolePromoteCareerRankRes>(CmdRolePromoteCareerRankRes.Parser, msg);
            if (this.Role.CareerRank == res.NowRank)
            {
                return;
            }
            this.Role.CareerRank = res.NowRank;
            uint curPromoteCareerId = Sys_Advance.Instance.SetAdvanceRank();
            CSVPromoteCareer.Data csvData = CSVPromoteCareer.Instance.GetConfData(curPromoteCareerId);
            if (csvData.teamCondition != 0)
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(csvData.advanceDia);
                if (null == cSVDialogueData)
                    return;
                List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
                ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                resetDialogueDataEventData.Init(datas, () =>
                {
                    GameCenter.mainHero.heroFxComponent.UpdateAdvanceFx(true);
                    PlayActorAdvanceUpHudEvt evt = new PlayActorAdvanceUpHudEvt();
                    evt.actorId = GameCenter.mainHero.UID;
                    Sys_HUD.Instance.eventEmitter.Trigger<PlayActorAdvanceUpHudEvt>(Sys_HUD.EEvents.OnPlayActorAdvanceUpFx,
                        evt);
                }, cSVDialogueData);
                Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
            }
            else
            {
                GameCenter.mainHero.heroFxComponent.UpdateAdvanceFx(true);
                PlayActorAdvanceUpHudEvt evt = new PlayActorAdvanceUpHudEvt();
                evt.actorId = GameCenter.mainHero.UID;
                Sys_HUD.Instance.eventEmitter.Trigger<PlayActorAdvanceUpHudEvt>(Sys_HUD.EEvents.OnPlayActorAdvanceUpFx,
                    evt);
            }
            this.eventEmitter.Trigger(Sys_Role.EEvents.OnUpdateCareerRank, res.NowRank);

            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.OccipationAdvance);
        }

        private void CareerUpInfoRes(NetMsg msg)
        {
            CmdRoleCareerUpInfoRes res =
                NetMsgUtil.Deserialize<CmdRoleCareerUpInfoRes>(CmdRoleCareerUpInfoRes.Parser, msg);
            Sys_Advance.Instance.careerUpData = res;
            DebugUtil.LogFormat(ELogType.eNone, "CareerUpInfoRes:{0}", Sys_Advance.Instance.careerUpData.Index);
            Sys_Advance.Instance.eventEmitter.Trigger(Sys_Advance.EEvents.OnUpdateCareerUpInfoEvent);
        }

        private void OnWjxinfoNtf(NetMsg netMsg)
        {
            DebugUtil.Log(ELogType.eQa, "OnWjxinfoNtf");
            CmdRoleWJXInfoNtf res = NetMsgUtil.Deserialize<CmdRoleWJXInfoNtf>(CmdRoleWJXInfoNtf.Parser, netMsg);
            List<uint> ids = new List<uint>();
            for (int i = 0; i < res.WjxId.Count; i++)
            {
                ids.Add(res.WjxId[i]);
                DebugUtil.Log(ELogType.eQa, string.Format("已答完调查问卷{0}", ids[i].ToString()));
            }

            Sys_Qa.Instance.CacheWjxIds(ids);
        }

        public void ReqExchangeGift(string code, string token)
        {
            DebugUtil.Log(ELogType.eNone, string.Format("兑换码：code: {0},token: {1}", code, token));
            CmdRoleExchangeGiftReq req = new CmdRoleExchangeGiftReq();
            req.Code = FrameworkTool.ConvertToGoogleByteString(code);
            req.Gametoken = FrameworkTool.ConvertToGoogleByteString(token);
            NetClient.Instance.SendMessage((ushort) CmdRole.ExchangeGiftReq, req);
        }

        private void OnExchangeGiftReq(NetMsg netMsg)
        {
            CmdRoleExchangeGiftRes res =
                NetMsgUtil.Deserialize<CmdRoleExchangeGiftRes>(CmdRoleExchangeGiftRes.Parser, netMsg);
            DebugUtil.Log(ELogType.eNone, string.Format("兑换码返回:{0} ErrorstrLength:{1}", res.Ret, res.Errorstr.Length));
            if (res.Ret == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10684));
            }
            else
            {
                if (res.Errorstr.IsEmpty)
                {
                }
                else
                {
                    string errorMsg = res.Errorstr.ToStringUtf8();
                    DebugUtil.Log(ELogType.eNone, string.Format("兑换码返回:{0} {1}", res.Ret, errorMsg));
                    Sys_Hint.Instance.PushContent_Normal("兑换码不存在"); //LanguageHelper.GetTextContent(10684);需要策划配置语言表
                }
            }
        }

        void CloseStackIgnoreUI()
        {
            UIManager.CloseUI(EUIID.UI_NPC);
            UIManager.CloseUI(EUIID.UI_Dialogue);
        }

        public int syncCount { get; private set; } = 0;

        public bool hasSyncFinished
        {
            get { return this.syncCount > 0; }
        }

        // 是否有过短线重连
        public bool hasReconnectSync
        {
            get { return this.syncCount > 1; }
        }

        /// <summary>
        /// 通知玩家已完成各模块消息推送
        /// </summary>
        /// <param name="msg"></param>
        private void OnFinishEnterGameNtf(NetMsg msg)
        {
            CmdRoleFinishEnterGameNtf roleFinishEnterGameNtf =
                NetMsgUtil.Deserialize<CmdRoleFinishEnterGameNtf>(CmdRoleFinishEnterGameNtf.Parser, msg);
            Sys_Guide.Instance.OnSealStatusChange(System.Convert.ToBoolean(roleFinishEnterGameNtf.ClientState));

            ++syncCount;
            SystemModuleManager.OnSyncFinished();
            this.reNameData = roleFinishEnterGameNtf.RenameData;
            isReNameSend = false;
            this.eventEmitter.Trigger(EEvents.OnSyncFinished);
        }

        /// <summary>
        /// 设置客户端状态标识请求
        /// </summary>
        /// <param name="clientStateId"></param>
        /// <param name="status"></param>
        public void RoleClientStateReq(uint clientStateId, bool status, uint id = 0)
        {
            DebugUtil.LogFormat(ELogType.eTask, "clientStateId: {0} status: {1} arg:{2}", clientStateId, status, id);
            CmdRoleClientStateReq cmdRoleClientStateReq = new CmdRoleClientStateReq();
            cmdRoleClientStateReq.ClientStateId = clientStateId;
            cmdRoleClientStateReq.Status = status;
            NetClient.Instance.SendMessage((ushort) CmdRole.ClientStateReq, cmdRoleClientStateReq);

            //if (clientStateId == (uint)EClientState.ExecTask) {
            //    Sys_Hangup.Instance.SendHangUpEnemySwitchReq(status);
            //}
            if (id != 0)
            {
                this.ReqHangUpReportHangUpStatus(clientStateId, status, id);
            }
        }

        public void ReqHangUpReportHangUpStatus(uint clientStateId, bool status, uint id)
        {
            CmdHangUpReportHangUpStatusReq req = new CmdHangUpReportHangUpStatusReq();
            req.HangUpInfoId = id;
            req.PetInfoId = id;

            if (clientStateId == (uint) EClientState.Hangup && status)
            {
                Sys_Hangup.Instance.hangUpId = id;
            }

            NetClient.Instance.SendMessage((ushort) CmdHangUp.ReportHangUpStatusReq, req);
        }

        /// <summary>
        /// 客户端状态更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnClientStateNtf(NetMsg msg)
        {
            CmdRoleClientStateNtf ntf =
                NetMsgUtil.Deserialize<CmdRoleClientStateNtf>(CmdRoleClientStateNtf.Parser, msg);
            if (ntf.BEnter) //登入
            {
                var status = Convert.ToString(ntf.SetClientStatus, 2).ToCharArray();
                Array.Reverse(status);

                var values = System.Enum.GetValues(typeof(EClientState));
                var length = values.Length - 1;
                for (int i = 0; i < length; i++)
                {
                    uint id = (uint) (i + 1);
                    if (i < status.Length)
                    {
                        if (status[i].CompareTo('1') == 0)
                        {
                            this.SetClientState((EClientState) id, true);
                        }
                        else
                        {
                            this.SetClientState((EClientState) id, false);
                        }
                    }
                    else
                    {
                        this.SetClientState((EClientState) id, false);
                    }
                }
            }
            else //更新
            {
                //设置开启
                if (ntf.SetClientStatus != 0)
                {
                    var status = Convert.ToString(ntf.SetClientStatus, 2).ToCharArray();
                    Array.Reverse(status);

                    for (int i = 0; i < status.Length; i++)
                    {
                        uint id = (uint) (i + 1);
                        if (status[i].CompareTo('1') == 0)
                        {
                            this.SetClientState((EClientState) id, true);
                        }
                    }
                }

                //设置关闭
                if (ntf.ClearClientStatus != 0)
                {
                    var status = Convert.ToString(ntf.ClearClientStatus, 2).ToCharArray();
                    Array.Reverse(status);

                    for (int i = 0; i < status.Length; i++)
                    {
                        uint id = (uint) (i + 1);
                        if (status[i].CompareTo('1') == 0)
                        {
                            this.SetClientState((EClientState) id, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置客户端状态
        /// </summary>
        /// <param name="ClientStateId"></param>
        /// <param name="value"></param>
        private void SetClientState(EClientState ClientStateId, bool value)
        {
            switch (ClientStateId)
            {
                case EClientState.CatchPet: //捕捉宠物
                {
                    if (value)
                    {
                        Sys_Pet.Instance.clientStateId = ClientStateId;
                    }

                    Sys_Pet.Instance.OnSealStatusChange(ClientStateId, value);
                }
                    break;
                case EClientState.Guide: //开关引导
                {
                    Sys_Guide.Instance.OnSealStatusChange(value);
                }
                    break;
                case EClientState.Hangup: //挂机巡逻
                {
                    if (value)
                    {
                        Sys_Pet.Instance.clientStateId = ClientStateId;
                    }

                    Sys_Pet.Instance.OnSealStatusChange(ClientStateId, value);
                }
                    break;
                case EClientState.ExecTask: //执行任务
                {
                    if (value)
                    {
                        Sys_Pet.Instance.clientStateId = ClientStateId;
                    }

                    Sys_Pet.Instance.OnSealStatusChange(ClientStateId, value);
                }
                    break;
            }
        }

        // 登录排队信息
        public void ReqLoginQueue()
        {
            NetClient.Instance.SendMessage((ushort) CmdRole.LoginQueueInfoReq, new CmdRoleLoginQueueInfoReq());
        }

        // 取消排队
        public void ReqCancelLoginQueue()
        {
            NetClient.Instance.SendMessage((ushort) CmdRole.CancelLoginQueueReq, new CmdRoleCancelLoginQueueReq());
        }

        public void UpdateRoleBackInfo(bool isBack)
        {
            IsBackRole = isBack;
            //HUD更新
            ActorHUDNameUpdateEvt evt = new ActorHUDNameUpdateEvt();
            evt.id = Role.RoleId;
            evt.name = Role.Name.ToStringUtf8();
            evt.eFightOutActorType = EFightOutActorType.MainHero;
            evt.upBack = isBack;
            Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorName, evt);
        }
        private void OnLoginQueueNtf(NetMsg msg)
        {
            //if (GotLoginRes) {
            //    return;
            //}

            if (LevelManager.mCurrentLevelType != typeof(LvLogin) &&
                LevelManager.mCurrentLevelType != typeof(LvCreateCharacter))
            {
                return;
            }

            CmdRoleLoginQueueNtf res = NetMsgUtil.Deserialize<CmdRoleLoginQueueNtf>(CmdRoleLoginQueueNtf.Parser, msg);

            if (res.Ret == 1)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(11693).words;
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);

                void OnConform()
                {
                    this.ReqCancelLoginQueue();
                    //NetClient.Instance.Disconnect();
                }
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_LoginLineUp, false,
                    new Tuple<uint, CmdRoleLoginQueueNtf>((uint) Sys_Login.Instance.selectedServerId, res));
            }

            this.eventEmitter.Trigger<CmdRoleLoginQueueNtf>(EEvents.OnLoginQueueNtf, res);
        }

        private void OnCrossDay(NetMsg netMsg)
        {
            this.LoginDay++;
            this.eventEmitter.Trigger(EEvents.OnCrossDay);
        }


        private void OnUpdateCrossState(NetMsg netMsg)
        {
            CmdRoleUpdateCrossState info =
                NetMsgUtil.Deserialize<CmdRoleUpdateCrossState>(CmdRoleUpdateCrossState.Parser, netMsg);

            this.isCrossSrv = info.BCrossSvr;
            this.eventEmitter.Trigger(EEvents.OnUpdateCrossSrvState);
        }

        private void BindPhoneSwitchNtf(NetMsg msg)
        {
            CmdRoleBindPhoneSwitchNtf ntf =
                NetMsgUtil.Deserialize<CmdRoleBindPhoneSwitchNtf>(CmdRoleBindPhoneSwitchNtf.Parser, msg);
            //Sys_OperationalActivity.Instance.BindPhoneSwitch = ntf.Setting;
        }

        private void NeedBindPhoneNtf(NetMsg msg)
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(16000);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                IsNeedBindPhone = true;
                //打开手机绑定
                SDKManager.SDKSetPhoneBind();
                UIManager.CloseUI(EUIID.UI_Server);
            }, 11828);
            PromptBoxParameter.Instance.SetCancel(false, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnReNameReq(string _name)
        {
            CmdRoleRenameReq req = new CmdRoleRenameReq();
            req.NewName = ByteString.CopyFromUtf8(_name);
            NetClient.Instance.SendMessage((ushort) CmdRole.RenameReq, req);
        }

        private void OnReNameRes(NetMsg netMsg)
        {
            CmdRoleRenameRes res = NetMsgUtil.Deserialize<CmdRoleRenameRes>(CmdRoleRenameRes.Parser, netMsg);
            reNameData = res.Data;
            Role.Name = res.NewName;
            //聊天框推送
            string _content = LanguageHelper.GetErrorCodeContent(770000004, res.NewName.ToStringUtf8());
            Sys_Chat.Instance.PushMessage(ChatType.Person, null, _content, Sys_Chat.EMessageProcess.None);
            //HUD更新
            ActorHUDNameUpdateEvt evt = new ActorHUDNameUpdateEvt();
            evt.id = Role.RoleId;
            evt.eFightOutActorType = EFightOutActorType.MainHero;
            evt.name = res.NewName.ToStringUtf8();
            Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDNameUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorName, evt);
            this.eventEmitter.Trigger(EEvents.OnReName);
        }

        public void OnReNameExpireReq()
        {
            CmdRoleRenameExpireReq req = new CmdRoleRenameExpireReq();
            NetClient.Instance.SendMessage((ushort) CmdRole.RenameExpireReq, req);
        }

        private void OnReNameExpireRes(NetMsg netMsg)
        {
            CmdRoleRenameExpireRes res =
                NetMsgUtil.Deserialize<CmdRoleRenameExpireRes>(CmdRoleRenameExpireRes.Parser, netMsg);
        }

        public void OnChangeCareerReq(uint careerId)
        {
            CmdRoleChangeCareerReq req = new CmdRoleChangeCareerReq();
            req.Career = careerId;
            NetClient.Instance.SendMessage((ushort)CmdRole.ChangeCareerReq, req);
        }

        private void OnChangeCareerNtf(NetMsg netMsg)
        {
            CmdRoleChangeCareerNty ntf = NetMsgUtil.Deserialize<CmdRoleChangeCareerNty>(CmdRoleChangeCareerNty.Parser, netMsg);
            
            PromptBoxParameter.Instance.Clear();
            string name = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(ntf.Career).name).words;
            string content = CSVLanguage.Instance.GetConfData(5180 ).words;
            PromptBoxParameter.Instance.content = string.Format(content, name);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
               ExitGameReq();
            });
            //PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickWorldLevelReq()
        {
            CmdRoleClickWorldLevelReq req = new CmdRoleClickWorldLevelReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.ClickWorldLevelReq, req);
        }

        private void OnClickWorldLevelRes(NetMsg msg)
        {
            CmdRoleClickWorldLevelRes res =  NetMsgUtil.Deserialize<CmdRoleClickWorldLevelRes>(CmdRoleClickWorldLevelRes.Parser, msg);
            lastClickOpenServiceDay = res.OpenServiceDay;
            this.eventEmitter.Trigger(Sys_Role.EEvents.OnClickWorldLevel);
        }

        private void TeamLeaderInvitePromoteCareerNtf(NetMsg msg)
        {
            CmdRoleTeamLeaderInvitePromoteCareerNtf ntf = NetMsgUtil.Deserialize<CmdRoleTeamLeaderInvitePromoteCareerNtf>(CmdRoleTeamLeaderInvitePromoteCareerNtf.Parser, msg);
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId))
            {
                UIManager.OpenUI(EUIID.UI_Advance_TeamTips);
            }      
            Sys_Advance.Instance.eventEmitter.Trigger<uint, ulong>(Sys_Advance.EEvents.OnTeamPromoteCareerAgreeOrCancel, 1, Sys_Team.Instance.Captain.MemId);
        }

        private void TeamPromoteCareerAgreeOrCancelNtf(NetMsg msg)
        {
            CmdRoleTeamPromoteCareerAgreeOrCancelNtf ntf = NetMsgUtil.Deserialize<CmdRoleTeamPromoteCareerAgreeOrCancelNtf>(CmdRoleTeamPromoteCareerAgreeOrCancelNtf.Parser, msg);
            Sys_Advance.Instance.eventEmitter.Trigger<uint, ulong>(Sys_Advance.EEvents.OnTeamPromoteCareerAgreeOrCancel, ntf.Agree, ntf.RoleId);
        }

        public void TeamLeaderCheckPromoteCareerReq()
        {
            CmdRoleTeamLeaderCheckPromoteCareerReq req = new CmdRoleTeamLeaderCheckPromoteCareerReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.TeamLeaderCheckPromoteCareerReq, req);
        }

        private void OnTeamLeaderCheckPromoteCareerRes(NetMsg msg)
        {
            CmdRoleTeamLeaderCheckPromoteCareerRes res = NetMsgUtil.Deserialize<CmdRoleTeamLeaderCheckPromoteCareerRes>(CmdRoleTeamLeaderCheckPromoteCareerRes.Parser, msg);
            if (!UIManager.IsOpen(EUIID.UI_Attribute))
            {
                uint nextId = (uint)Sys_Advance.Instance.NextAdvanceRank();
                int count = CSVPromoteCareer.Instance.GetConfData(nextId).teamCondition;
                if (Sys_Advance.Instance.CanAdvanceInTeam(count))
                {
                    UIManager.OpenUI(EUIID.UI_Advance_TeamTips, false);
                }
            }
            else
            {
                Sys_Advance.Instance.eventEmitter.Trigger(Sys_Advance.EEvents.OnTeamLeaderCheckPromoteCareer);
            }
        }
        #endregion

        #region RoleUtil

        public bool IsSelfRoleName(string name)
        {
            return string.Equals(this.sRoleName, name, StringComparison.Ordinal);
        }

        public bool IsSelfRole(ulong id)
        {
            return this.Role.RoleId == id;
        }

        public ulong RoleId
        {
            get { return this.Role.RoleId; }
        }

        public uint HeroId
        {
            get { return this.Role.HeroId; }
        }

        public string sRoleName { get; private set; }

        public ulong exp { get; private set; }

        public uint LoginDay { get; private set; }

        public uint GetWorldLv()
        {
            if (CSVWorldLevel.Instance.TryGetValue(Sys_Role.Instance.openServiceDay,
                out CSVWorldLevel.Data openServiceDay) && openServiceDay != null)
            {
                return openServiceDay.world_level;
            }
            else
            {
                return CSVWorldLevel.Instance.GetKeys().Max();
            }
        }

        #endregion

        /// <summary>
        /// 新手埋点
        /// </summary>
        //private uint _newTraceLeftTime = 0;
        //private uint TraceTimeCount = 3 * 3600u;
        private uint clientOnlineTime = 0u;

        //private System.Threading.Thread _thread;
        private Timer _timerHitPoint;
        //private Timer _timerFPS;

        public void CheckNewTrace()
        {
            this.clientOnlineTime = this.OnlineTime;
            Framework.TimeManager.StartFPSCalculate();

            //this._timerFPS?.Cancel();
            //this._timerFPS = Timer.Register(10f, this.OnHitPointFPSEvent, null, true);

            if (this.Role != null && this.Role.Level < 41)
            {
                this._timerHitPoint?.Cancel();
                this._timerHitPoint = Timer.Register(1f, this.OnTracePoint, null, true);
            }
            else
            {
#if !DEBUG_MODE
                //Framework.TimeManager.StopFPSCalculate();
#endif
                this._timerHitPoint?.Cancel();
                this._timerHitPoint = null;
            }
        }

        private HitPointNewTrace trace = new HitPointNewTrace();

        private void OnTracePoint()
        {
            if (!SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.ReportPoint.ToString(), out string paramsValue))
                return;

            if (this.IsPause)
                return;

            if (!NetClient.Instance.IsConnected)
                return;

            if (this.Role == null || this.Role.Level >= 41)
                return;

            if (Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.Hangup) //挂机不上报
                return;

            int inBattle = 0;
            if (GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType ==
                ProcedureManager.EProcedureType.Fight)
                inBattle = 1;

            trace.AppendBaseData();
            trace.scene_id = Sys_Map.Instance.CurMapId;
            trace.total_online_duration = (this.clientOnlineTime++);
            trace.team_status = Sys_Team.Instance.GetPlayerTeamState();
            trace.inbattle = inBattle;

            //taksId
            uint mainTaskId = 0;
            List<uint> branchIds = Sys_Task.Instance.GetTaskIds(ref mainTaskId);
            trace.main_task_id = mainTaskId;
            System.Text.StringBuilder sbranchIds = StringBuilderPool.GetTemporary();
            sbranchIds.Clear();

            for (int i = 0; i < branchIds.Count; ++i)
            {
                if (i == branchIds.Count - 1)
                {
                    sbranchIds.Append(branchIds[i]);
                }
                else
                {
                    sbranchIds.Append(branchIds[i]).Append("|");
                }
            }

            trace.branch_task_id = sbranchIds.ToString();
            trace.current_task_id = Sys_Task.Instance.currentTaskId;

            UnityEngine.Vector2 pos = Sys_Map.Instance.GetPointInTexture();
            trace.coordinate_x = UnityEngine.Mathf.RoundToInt(pos.x);
            trace.coordinate_z = UnityEngine.Mathf.RoundToInt(pos.y);
            trace.fps = Framework.TimeManager.FPS;

            HitPointManager.HitPoint(HitPointNewTrace.Key, trace);

            StringBuilderPool.ReleaseTemporary(sbranchIds);
        }

        //HitPointFPSEvent fpsEvent = new HitPointFPSEvent();

        //private void OnHitPointFPSEvent()
        //{
        //    if (!IsGMHitOpen("fps"))
        //        return;
            
        //    fpsEvent.fps = Framework.TimeManager.FPS;
        //    fpsEvent.instance_id = Sys_Login.Instance.selectedServerId;
            
        //    HitPointManager.HitPoint(HitPointFPSEvent.Key, fpsEvent);
        //}

        //private Dictionary<string, string> gmSwitchs = new Dictionary<string, string>();
        private bool isFivePercent = false;

        public bool IsGMHitOpen(string key)
        {
            uint open = 0;
            SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.fps.ToString(), out string paramsValue);
            open = paramsValue == null ? open = 0 : uint.Parse(paramsValue);

            if (open == 0)
            {
                return true;
            }
            else if (open == 1)
            {
                return false;
            }
            else if (open == 2)
            {
                return isFivePercent;
            }
            
            return false;
        }

        private void RandomFivePercent()
        {
            Random rand = new Random(5);
            int value = rand.Next(0, 10000);
            isFivePercent = value < 500;
        }

        //UnityEngine.Networking.UnityWebRequest req;
        //private void GetGMWebData()
        //{
        //    string remoteVersionUrl = string.Format("{0}?app_id={1}&game_version={2}&ts={3}", VersionHelper.VersionUrl,
        //        SDKManager.GetAppid(), SDKManager.GetGameVersion(), Framework.TimeManager.ClientNowMillisecond());
        //    HitPointManager.SDKHitPointBaseData pointBaseData = HitPointManager.GetSDKHitPointBaseData();

        //    req = UnityEngine.Networking.UnityWebRequest.Get(remoteVersionUrl);
        //    req.timeout = 10;
        //    req.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
        //    req.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
        //    req.SetRequestHeader("Pragma", "no-cache");
        //    req.SetRequestHeader("device-id", pointBaseData.device_id);
        //    req.SetRequestHeader("os", AssetPath.sPlatformName);
        //    req.SetRequestHeader("app-version", pointBaseData.app_version);
        //    req.SetRequestHeader("channel", pointBaseData.channel);
        //    req.SetRequestHeader("channel-id", pointBaseData.markert_channel);
        //    req.SendWebRequest().completed += AsyncOperationAppealCompleted;
        //}
        //private void AsyncOperationAppealCompleted(UnityEngine.AsyncOperation asyncOperation)
        //{
        //    if (req.isDone)
        //    {
        //        System.IO.MemoryStream ms = new System.IO.MemoryStream(req.downloadHandler.data);
        //        System.IO.StreamReader sr = new System.IO.StreamReader(ms);
        //        string content = sr.ReadToEnd();
        //        sr.Close();
        //        ms.Close();

        //        VersionInfoRemote versionInfo = LitJson.JsonMapper.ToObject<VersionInfoRemote>(content);
        //        gmSwitchs.Clear();
        //        gmSwitchs = versionInfo.ext_json;
        //    }
        //    req.Dispose();
        //    req = null;
        //}
        //private System.Collections.IEnumerator ParseGMSwitchsData(UnityEngine.Networking.UnityWebRequest req)
        //{
        //    yield return req.SendWebRequest();
        //    if (!req.isDone || req.isNetworkError || req.isHttpError || req.responseCode != 200)
        //    {
        //    }
        //    else
        //    {
        //        System.IO.MemoryStream ms = new System.IO.MemoryStream(req.downloadHandler.data);
        //        System.IO.StreamReader sr = new System.IO.StreamReader(ms);
        //        string content = sr.ReadToEnd();
        //        DebugUtil.LogFormat(ELogType.eNone, "接口获取内容：\n {0}", content);
        //        sr.Close();
        //        ms.Close();

        //        VersionInfoRemote versionInfo = LitJson.JsonMapper.ToObject<VersionInfoRemote>(content);
        //        gmSwitchs.Clear();
        //        gmSwitchs = versionInfo.ext_json;
        //    }
        //}
    }
}