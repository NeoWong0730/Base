using Framework;
using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public enum ERedPacketType
    {
        Normal = 0,  //普通红包
        Voice = 1,   //语音红包
    }
    public enum ERedPacketGetWayType
    {
        TopUp = 1,//付费充值
        Achievement = 2,//完成成就
        LuckyDraw = 3,//抽奖
    }
    public enum ERedPacketState
    {
        Unclaimed = 1,//待领取
        Opened = 2,   //已打开
    }
    public enum ESystemRedPacketState
    {
        Unsent = 1,//未发送
        Send = 2,  //已发送
    }
    public enum ERefreshTimerType
    {
        DayLimit=1,   //红包日领额度刷新
        WeekLimit=2,  //红包周发额度刷新
    }

    /// <summary> 家族红包 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        #region 数据
        //拥有的系统可发送红包集合
        public List<SystemRedPacketData> systemRedPacketList = new List<SystemRedPacketData>();
        //展示最新的红包数据(最多八个)
        public List<ShowRedPacketData> showRedPacketList = new List<ShowRedPacketData>();
        //红包历史记录集合
        public List<RedPacketHistoryData> historyRedPacketList = new List<RedPacketHistoryData>();
        //当前收到的红包id集合
        public List<uint> getRedPacketIds = new List<uint>();
        //已打开的红包id集合
        private List<uint> openedPacketIdList = new List<uint>();
        //当前选择的红包id
        public uint curSelectRedPacketId;
        //当前选择返回的红包数据
        public RedPacketData curSelectRedPacketData;
        private bool _curIsHaveRedPacket;
        public bool curIsHaveRedPacket
        {
            get
            {
                return _curIsHaveRedPacket;
            }
            set
            {
                if (_curIsHaveRedPacket != value)
                {
                    _curIsHaveRedPacket = value;
                    eventEmitter.Trigger(EEvents.OnRefreshRedPacketPoint);
                    //RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyRedPacketRedPoint, null);
                }
            }
        }
        //红包日抢限额
        public uint todayMoney;
        public uint todayLastRefresh;
        //红包周发限额
        public uint weekMoney;
        public uint weekLastRefresh;
        //累计发放的红包总金额
        public uint totalSendMoney;
        //累计发放的红包个数
        public uint totalSendCount;

        public bool getRedPacketViewIsShow;
        //主界面抢红包页面显隐时间
        public Timer showTime;
        public Timer hideTime;
        public bool voiceIsCanSend;
        //计时器字典
        Dictionary<ERefreshTimerType, Timer> timerDic = new Dictionary<ERefreshTimerType, Timer>();
        //已发送的聊天红包链接 红包id|聊天语句id
        Dictionary<uint, SendPacketData> sendPackerChatIdDic = new Dictionary<uint, SendPacketData>();

        List<FamilyPacketLimitData> packetLimitDataList = new List<FamilyPacketLimitData>();
        public class SendPacketData
        {
            public ulong messageId;
            public string newContent;
        }
        public class FamilyPacketLimitData
        {
            public uint id;
            public CSVFamilyPacketLimit.Data data;
        }
        #endregion
        public void FamilyRedPacketInit()
        {
            curSelectRedPacketId = 0;
            curIsHaveRedPacket = false;
            todayMoney=0;
            todayLastRefresh=0;
            weekMoney=0;
            weekLastRefresh=0;
            totalSendMoney=0;
            totalSendCount=0;
            getRedPacketViewIsShow = false;
            voiceIsCanSend = false;
            sendPacketAction = null;

            packetLimitDataList.Clear();
            var datas= CSVFamilyPacketLimit.Instance.GetAll();
            for (int i = 0; i < datas.Count; i++)
            {
                FamilyPacketLimitData data = new FamilyPacketLimitData();
                data.id = datas[i].id;
                data.data = datas[i];
                packetLimitDataList.Add(data);
            }
            packetLimitDataList.Sort((a, b) => {
                return (int)(a.id - b.id);
            });
        }

        public void FamilyRedPacketLogout()
        {
            systemRedPacketList.Clear();
            showRedPacketList.Clear();
            historyRedPacketList.Clear();
            getRedPacketIds.Clear();
            openedPacketIdList.Clear();
            subRedPacketDataList.Clear();
            sendPackerChatIdDic.Clear();
            curSelectRedPacketData = null;
            CancelTime();
            ClearAllDicTime();
            sendPacketAction = null;
        }
        private void ProcessEvents_RedPacket(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.SendRedEnvelopeReq, (ushort)CmdGuild.SendRedEnvelopeAck, OnSendRedEnvelopeAck, CmdGuildSendRedEnvelopeAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.SendRedEnvelopeNtf, OnSendRedEnvelopeNtf, CmdGuildSendRedEnvelopeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.MyRedEnvelopeNtf, OnMyRedEnvelopeAck, CmdGuildMyRedEnvelopeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.NewEnvelopeNtf, OnNewEnvelopeNtf, CmdGuildNewEnvelopeNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.GetRedEnvelopeInfoAck, OnGetRedEnvelopeInfoAck, CmdGuildGetRedEnvelopeInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.QueryEnvelopeInfoReq, (ushort)CmdGuild.QueryEnvelopeInfoAck, OnQueryEnvelopeInfoAck, CmdGuildQueryEnvelopeInfoAck.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.OpenEnvelopeReq, (ushort)CmdGuild.OpenEnvelopeAck, OnOpenEnvelopeAck, CmdGuildOpenEnvelopeAck.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.TodayEnvelopeMoneyNtf, OnTodayEnvelopeMoneyNtf, CmdGuildTodayEnvelopeMoneyNtf.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuild.EnvelopeLogNtf, OnEnvelopeLogNtf, CmdGuildEnvelopeLogNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdGuild.GetOnlineNumReq, (ushort)CmdGuild.GetOnlineNumRes, GetOnlineNumRes, CmdGuildGetOnlineNumRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SendRedEnvelopeAck, OnSendRedEnvelopeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.SendRedEnvelopeNtf, OnSendRedEnvelopeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.MyRedEnvelopeNtf, OnMyRedEnvelopeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.NewEnvelopeNtf, OnNewEnvelopeNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetRedEnvelopeInfoAck, OnGetRedEnvelopeInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.QueryEnvelopeInfoAck, OnQueryEnvelopeInfoAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.OpenEnvelopeAck, OnOpenEnvelopeAck);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.TodayEnvelopeMoneyNtf, OnTodayEnvelopeMoneyNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.EnvelopeLogNtf, OnEnvelopeLogNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuild.GetOnlineNumRes, GetOnlineNumRes);
            }
        }
        #region 服务器消息
        /// <summary>
        /// 用于更新自己的周发红包，红包总金额、总个数,系统红包红包状态
        /// </summary>
        /// <param name="msg"></param>
        private void OnSendRedEnvelopeAck(NetMsg msg)
        {
            CmdGuildSendRedEnvelopeAck res = NetMsgUtil.Deserialize<CmdGuildSendRedEnvelopeAck>(CmdGuildSendRedEnvelopeAck.Parser, msg);
            weekMoney = res.WeekSend;
            weekLastRefresh = res.WeekRefresh;
            totalSendMoney = res.TotalMoney;
            totalSendCount = res.TotalCount;
            RefreshPacketTime(ERefreshTimerType.WeekLimit);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMyTotalRedPacket);
            if (res.Info != null)
            {
                for (int i = 0; i < systemRedPacketList.Count; i++)
                {
                    if (res.Info.EnvelopeId == systemRedPacketList[i].packetId)
                    {
                        systemRedPacketList[i].state = res.Info.State == 0 ? ESystemRedPacketState.Unsent : ESystemRedPacketState.Send;
                        systemRedPacketList[i].aliveTime = res.Info.AliveTime;
                        break;
                    }
                }
                CheckRedPoint();
                CheckSystemRedPacketActive();
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMySystemRedPacket);
            }
        }
        /// <summary>
        /// 发送红包返回 (同时也会通知其他玩家收到红包)
        /// </summary>
        /// <param name="msg"></param>
        private void OnSendRedEnvelopeNtf(NetMsg msg)
        {
            CmdGuildSendRedEnvelopeNtf ntf = NetMsgUtil.Deserialize<CmdGuildSendRedEnvelopeNtf>(CmdGuildSendRedEnvelopeNtf.Parser, msg);
            ShowRedPacketData showData = new ShowRedPacketData();
            showData.sendName = ntf.SenderName.ToStringUtf8();
            showData.state = ERedPacketState.Unclaimed;
            showData.content = ntf.Blessing.ToStringUtf8();
            showData.packetId = ntf.EnvelopId;
            //bool isPastDue = (int)(ntf.SendTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
            //if (isPastDue)
            //{
            //    if (showData.state == ERedPacketState.Unclaimed)
            //        showData.state = ERedPacketState.Opened;
            //}
            if (showRedPacketList.Count >= 8)
            {
                showRedPacketList.RemoveAt(showRedPacketList.Count-1);
            }
            showRedPacketList.Insert(0, showData);
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateShowRedPacket);
            //是谁发放的红包
            if (ntf.RoleId == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11939));
            }

            //Sys_Chat.ChatBaseInfo chatBaseInfo = new Sys_Chat.ChatBaseInfo();
            //chatBaseInfo.nRoleID = ntf.RoleId;
            //chatBaseInfo.nHeroID = Sys_Role.Instance.HeroId;
            //chatBaseInfo.sSenderName = ntf.SenderName.ToStringUtf8();
            //chatBaseInfo.SenderChatFrame = Sys_Head.Instance.clientHead.chatFrameId;
            //chatBaseInfo.SenderChatText = Sys_Head.Instance.clientHead.chatTextId;
            //chatBaseInfo.SenderHead = Sys_Head.Instance.clientHead.headId;
            //chatBaseInfo.SenderHeadFrame = Sys_Head.Instance.clientHead.headFrameId;

            string sendName = string.Format("<color=#{0}>[{1}]</color>", Constants.gChatColor_Name, ntf.SenderName.ToStringUtf8());
            string content = LanguageHelper.GetTextContent(11972, sendName, ntf.Blessing.ToStringUtf8(), CSVParam.Instance.GetConfData(1292).str_value, ntf.EnvelopId.ToString());
            string newContent = LanguageHelper.GetTextContent(11972, sendName, ntf.Blessing.ToStringUtf8(), CSVParam.Instance.GetConfData(1293).str_value, ntf.EnvelopId.ToString());
            ulong messageId = Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, Sys_Chat.EMessageProcess.AddUID, Sys_Chat.EExtMsgType.Normal);
            sendPackerChatIdDic[ntf.EnvelopId] = new SendPacketData { messageId = messageId, newContent = newContent };

            CheckRedPoint();
            AddRedPacketId(ntf.EnvelopId);
        }
        /// <summary>
        /// 上线推送(已发放总金币、总个数，已发放红包周额，周发红包刷新时间，拥有的系统红包)
        /// </summary>
        /// <param name="msg"></param>
        private void OnMyRedEnvelopeAck(NetMsg msg)
        {
            CmdGuildMyRedEnvelopeNtf ntf = NetMsgUtil.Deserialize<CmdGuildMyRedEnvelopeNtf>(CmdGuildMyRedEnvelopeNtf.Parser, msg);
            totalSendMoney = ntf.TotalSendMoney;
            totalSendCount = ntf.TotalSendCount;
            weekMoney = ntf.WeekMoney;
            weekLastRefresh = ntf.LastRefresh;
            RefreshPacketTime(ERefreshTimerType.WeekLimit);
            if (ntf.Myenvelopes != null && ntf.Myenvelopes.Count > 0)
            {
                systemRedPacketList.Clear();
                foreach (var item in ntf.Myenvelopes)
                {
                    SystemRedPacketData systemData = new SystemRedPacketData();
                    systemData.packetId = item.EnvelopeId;
                    systemData.state = item.State == 0 ? ESystemRedPacketState.Unsent : ESystemRedPacketState.Send;
                    systemData.aliveTime = item.AliveTime;
                    systemData.expireTime = item.ExpireTime;
                    systemData.packetData = CSVFamilyPacket.Instance.GetConfData(item.InfoId);
                    systemRedPacketList.Add(systemData);
                }
            }
            CheckRedPoint();
        }
        /// <summary>
        /// 系统红包增量
        /// </summary>
        /// <param name="msg"></param>
        private void OnNewEnvelopeNtf(NetMsg msg)
        {
            CmdGuildNewEnvelopeNtf ntf = NetMsgUtil.Deserialize<CmdGuildNewEnvelopeNtf>(CmdGuildNewEnvelopeNtf.Parser, msg);
            if (ntf != null && ntf.NewEnvelopes.Count > 0)
            {
                for (int i = 0; i < ntf.NewEnvelopes.Count; i++)
                {
                    SystemRedPacketData systemData = new SystemRedPacketData();
                    systemData.packetId = ntf.NewEnvelopes[i].EnvelopeId;
                    systemData.state = ntf.NewEnvelopes[i].State == 0 ? ESystemRedPacketState.Unsent : ESystemRedPacketState.Send;
                    systemData.aliveTime = ntf.NewEnvelopes[i].AliveTime;
                    systemData.expireTime = ntf.NewEnvelopes[i].ExpireTime;
                    systemData.packetData = CSVFamilyPacket.Instance.GetConfData(ntf.NewEnvelopes[i].InfoId);
                    systemRedPacketList.Add(systemData);
                }
                if (systemRedPacketList.Count > 100)
                    systemRedPacketList.RemoveRange(0, systemRedPacketList.Count - 100);
                CheckRedPoint();
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateMySystemRedPacket);
            }
        }
        /// <summary>
        /// 公会所有红包日志和最近八个红包详细信息（上线通知）
        /// </summary>
        /// <param name="msg"></param>
        private void OnGetRedEnvelopeInfoAck(NetMsg msg)
        {
            CmdGuildGetRedEnvelopeInfoAck ntf = NetMsgUtil.Deserialize<CmdGuildGetRedEnvelopeInfoAck>(CmdGuildGetRedEnvelopeInfoAck.Parser, msg);
            //八个展示红包
            if (ntf.RedEnvelopes != null)
            {
                showRedPacketList.Clear();
                foreach (var item in ntf.RedEnvelopes)
                {
                    ShowRedPacketData showData = new ShowRedPacketData();
                    showData.packetId = item.EnvelopeId;
                    showData.sendName = item.SenderName.ToStringUtf8(); 
                    showData.state = item.BOpen ? ERedPacketState.Opened : ERedPacketState.Unclaimed;
                    showData.content = item.Blessing.ToStringUtf8();
                    showData.sendTime = item.SendTime;
                    //bool isPastDue = (int)(item.SendTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
                    //if (isPastDue)
                    //{
                    //    if (showData.state == ERedPacketState.Unclaimed)
                    //        showData.state = ERedPacketState.Opened;
                    //}
                    showRedPacketList.Add(showData);
                }
            }
            //历史红包
            if (ntf.EnvelopeLogs != null)
            {
                historyRedPacketList.Clear();
                for (int i = ntf.EnvelopeLogs.Count-1; i >=0; i--)
                {
                    RedEnvelopeLog item = ntf.EnvelopeLogs[i];
                    RedPacketHistoryData historyData = new RedPacketHistoryData();
                    historyData.packetId = item.EnvelopeId;
                    historyData.logTime = item.LogTime;
                    historyData.sendName = item.SenderName.ToStringUtf8();
                    historyData.state = item.Checked ? ERedPacketState.Opened : ERedPacketState.Unclaimed;
                    historyData.curTime = item.FinishTime != 0 ? item.FinishTime : item.LogTime;
                    //bool isPastDue = (int)(item.LogTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
                    //if (isPastDue)
                    //{
                    //    if (historyData.state == ERedPacketState.Unclaimed)
                    //        historyData.state = ERedPacketState.Opened;
                    //}
                    //以下抢完才有值
                    if (item.FinishTime != 0)
                    {
                        historyData.finishTime = item.FinishTime;
                        historyData.currencyAllValue = item.TotalMoney;
                        historyData.luckyBoy = item.LuckyBoy.ToStringUtf8();
                    }
                    historyData.Init();
                    historyRedPacketList.Add(historyData);
                }
                historyRedPacketList.Sort((a,b)=> {
                    return (int)(b.curTime - a.curTime);
                });
            }
            CheckRedPoint();
        }
        /// <summary>
        /// 根据玩家是否打开过返回不同信息
        /// </summary>
        /// <param name="msg"></param>
        private void OnQueryEnvelopeInfoAck(NetMsg msg)
        {
            CmdGuildQueryEnvelopeInfoAck ntf = NetMsgUtil.Deserialize<CmdGuildQueryEnvelopeInfoAck>(CmdGuildQueryEnvelopeInfoAck.Parser, msg);
            if (curSelectRedPacketData == null)
                curSelectRedPacketData = new RedPacketData();
            curSelectRedPacketData.packetId = ntf.Detail.EnvelopeId;
            curSelectRedPacketData.sendTime = ntf.Detail.SendTime;
            curSelectRedPacketData.state = ntf.Detail.BOpened ? ERedPacketState.Opened : ERedPacketState.Unclaimed;
            curSelectRedPacketData.content = ntf.Detail.Blessing.ToStringUtf8();
            curSelectRedPacketData.type = ntf.Detail.Voiceid == 0 ? ERedPacketType.Normal : ERedPacketType.Voice;
            curSelectRedPacketData.sendName = ntf.Detail.SnederName.ToStringUtf8();
            curSelectRedPacketData.headId = ntf.Detail.HeadIcon;
            curSelectRedPacketData.heroId = ntf.Detail.HeroId;
            if (curSelectRedPacketData.subRedPacketDataList == null)
                curSelectRedPacketData.subRedPacketDataList = new List<SubRedPacketData>();
            else
                curSelectRedPacketData.subRedPacketDataList.Clear();
            curSelectRedPacketData.currencyAllValue = ntf.Detail.TotalMoney;
            curSelectRedPacketData.costTime = ntf.Detail.CostTime == 0 ? ntf.Detail.CostTime : ntf.Detail.CostTime - ntf.Detail.SendTime;
            curSelectRedPacketData.currencyCopies = ntf.Detail.DivideCount;
            curSelectRedPacketData.currencyValue = 0;
            curSelectRedPacketData.isPastDue = (int)(ntf.Detail.SendTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
            if (curSelectRedPacketData.isPastDue)
            {
                if (curSelectRedPacketData.state == ERedPacketState.Unclaimed)
                    curSelectRedPacketData.state = ERedPacketState.Opened;
                ChangeRedPacketLineState(ntf.Detail.EnvelopeId);
            }
            RefreshMyRedPacketState(curSelectRedPacketData);
            CheckRedPoint();
            if (ntf.Detail.Moneylist.Count > 0)
            {
                foreach (var item in ntf.Detail.Moneylist)
                {
                    if (Sys_Role.Instance.sRoleName.Equals(item.Name.ToStringUtf8()))
                        curSelectRedPacketData.currencyValue = item.Money;
                    SubRedPacketData subData = new SubRedPacketData();
                    subData.roleName = item.Name.ToStringUtf8();
                    subData.currencyValue = item.Money;
                    subData.headId = item.HeadIcon;
                    subData.heroId = item.HeroId;
                    curSelectRedPacketData.subRedPacketDataList.Add(subData);
                }
            }
            if (!UIManager.IsVisibleAndOpen(EUIID.UI_Family_RedPacket))
            {
                UIManager.OpenUI(EUIID.UI_Family_RedPacket);
            }
        }
        /// <summary>
        /// 抢红包返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnOpenEnvelopeAck(NetMsg msg)
        {
            CmdGuildOpenEnvelopeAck ntf = NetMsgUtil.Deserialize<CmdGuildOpenEnvelopeAck>(CmdGuildOpenEnvelopeAck.Parser, msg);
            if (curSelectRedPacketData == null)
                curSelectRedPacketData = new RedPacketData();
            openedPacketIdList.Add(ntf.Detail.EnvelopeId);
            curSelectRedPacketData.currencyValue = ntf.GotMoney != 0 ? ntf.GotMoney : 0;
            curSelectRedPacketData.packetId = ntf.Detail.EnvelopeId;
            curSelectRedPacketData.sendTime = ntf.Detail.SendTime;
            curSelectRedPacketData.state = ntf.Detail.BOpened ? ERedPacketState.Opened : ERedPacketState.Unclaimed;
            curSelectRedPacketData.content = ntf.Detail.Blessing.ToStringUtf8();
            curSelectRedPacketData.type = ntf.Detail.Voiceid == 0 ? ERedPacketType.Normal : ERedPacketType.Voice;
            curSelectRedPacketData.sendName = ntf.Detail.SnederName.ToStringUtf8();
            curSelectRedPacketData.headId = ntf.Detail.HeadIcon;
            curSelectRedPacketData.heroId = ntf.Detail.HeroId;
            todayMoney = ntf.TodayLimit;
            if(ntf.BSelfEnvelope)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12463));
            todayLastRefresh = ntf.LastRefreshTime;
            RefreshPacketTime(ERefreshTimerType.DayLimit);
            if (curSelectRedPacketData.subRedPacketDataList == null)
                curSelectRedPacketData.subRedPacketDataList = new List<SubRedPacketData>();
            else
                curSelectRedPacketData.subRedPacketDataList.Clear();
            curSelectRedPacketData.currencyAllValue = ntf.Detail.TotalMoney;
            curSelectRedPacketData.costTime = ntf.Detail.CostTime==0? ntf.Detail.CostTime : ntf.Detail.CostTime - ntf.Detail.SendTime;
            curSelectRedPacketData.currencyCopies = ntf.Detail.DivideCount;
            curSelectRedPacketData.isPastDue = (int)(ntf.Detail.SendTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
            RefreshMyRedPacketState(curSelectRedPacketData);
            if (ntf.Detail.Moneylist.Count > 0)
            {
                foreach (var item in ntf.Detail.Moneylist)
                {
                    SubRedPacketData subData = new SubRedPacketData();
                    subData.roleName = item.Name.ToStringUtf8();
                    subData.currencyValue = item.Money;
                    subData.headId = item.HeadIcon;
                    subData.heroId = item.HeroId;
                    curSelectRedPacketData.subRedPacketDataList.Add(subData);
                }
            }
            CheckRedPoint();
            Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnOpendRedPacketBack);
            ChangeRedPacketLineState(ntf.Detail.EnvelopeId);
        }
        /// <summary>
        /// 今日已抢红包
        /// </summary>
        /// <param name="msg"></param>
        private void OnTodayEnvelopeMoneyNtf(NetMsg msg)
        {
            CmdGuildTodayEnvelopeMoneyNtf ntf = NetMsgUtil.Deserialize<CmdGuildTodayEnvelopeMoneyNtf>(CmdGuildTodayEnvelopeMoneyNtf.Parser, msg);
            todayMoney = ntf.TodayMoney;
            todayLastRefresh = ntf.LastRefresh;
            RefreshPacketTime(ERefreshTimerType.DayLimit);
        }
        /// <summary>
        /// 红包日志增量
        /// </summary>
        /// <param name="msg"></param>
        private void OnEnvelopeLogNtf(NetMsg msg)
        {
            CmdGuildEnvelopeLogNtf ntf = NetMsgUtil.Deserialize<CmdGuildEnvelopeLogNtf>(CmdGuildEnvelopeLogNtf.Parser, msg);
            if (ntf.Info != null)
            {
                RedPacketHistoryData historyData = new RedPacketHistoryData();
                historyData.packetId = ntf.Info.EnvelopeId;
                historyData.logTime = ntf.Info.LogTime;
                historyData.sendName = ntf.Info.SenderName.ToStringUtf8();
                historyData.state = ntf.Info.Checked ? ERedPacketState.Opened : ERedPacketState.Unclaimed;
                historyData.curTime = ntf.Info.FinishTime != 0 ? ntf.Info.FinishTime : ntf.Info.LogTime;
                //bool isPastDue = (int)(ntf.Info.LogTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime()) <= 0;
                //if (isPastDue)
                //{
                //    if (historyData.state == ERedPacketState.Unclaimed)
                //        historyData.state = ERedPacketState.Opened;
                //}
                //以下抢完才有值
                if (ntf.Info.FinishTime != 0)
                {
                    historyData.finishTime = ntf.Info.FinishTime;
                    historyData.currencyAllValue = ntf.Info.TotalMoney;
                    historyData.luckyBoy = ntf.Info.LuckyBoy.ToStringUtf8();
                }
                historyData.Init();
                if (historyRedPacketList.Count >= 99)
                {
                    historyRedPacketList.RemoveAt(historyRedPacketList.Count - 1);
                }
                historyRedPacketList.Insert(0, historyData);
                CheckRedPoint();
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateHistoryRedPacket,false);
            }
        }
        private void GetOnlineNumRes(NetMsg msg)
        {
            CmdGuildGetOnlineNumRes ntf = NetMsgUtil.Deserialize<CmdGuildGetOnlineNumRes>(CmdGuildGetOnlineNumRes.Parser, msg);
            if (ntf != null)
            {
                onlineMember = ntf.OnlineNum;
                guidLvl = ntf.GuildLv;
            }
            sendPacketAction?.Invoke();
        }
        #endregion
        #region 客户端请求
        Action sendPacketAction;
        /// <summary>
        /// 发送红包请求
        /// </summary>
        /// <param name="packetId">红包id</param>
        /// <param name="content">祝福语</param>
        /// <param name="divideCount">红包份数</param>
        /// <param name="talkId">0为普通红包，1为语音红包</param>
        /// <param name="totalMoney">自发红包的金额，系统红包不必赋值</param>
        public void OnSendRedPacketReq(uint packetId, string content, uint divideCount, uint talkId = 0, uint totalMoney = 0, bool isSystemRedPacket = false)
        {
            sendPacketAction = () =>
            {
                if (CheckSendRedPacketCondition(isSystemRedPacket))
                {
                    if (!isSystemRedPacket)
                    {
                        //金币不足
                        if (Sys_Bag.Instance.GetItemCount(2) < totalMoney)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12090));
                            return;
                        }
                    }
                    CmdGuildSendRedEnvelopeReq req = new CmdGuildSendRedEnvelopeReq();
                    req.EnvelopId = packetId;
                    req.Blessing = ByteString.CopyFromUtf8(content);
                    req.DivideCount = divideCount;
                    req.TalkId = talkId;
                    req.TotalMoney = totalMoney;
                    NetClient.Instance.SendMessage((ushort)CmdGuild.SendRedEnvelopeReq, req);
                }
            };
            GetOnlineNumReq();
        }
        private void GetOnlineNumReq()
        {
            CmdGuildGetOnlineNumReq req = new CmdGuildGetOnlineNumReq();
            NetClient.Instance.SendMessage((ushort)CmdGuild.GetOnlineNumReq, req);
        }
        /// <summary>
        /// 根据id查询某红包
        /// </summary>
        public void QueryEnvelopeInfoReq(uint packetId = 0)
        {
            CmdGuildQueryEnvelopeInfoReq req = new CmdGuildQueryEnvelopeInfoReq();
            req.EnvelopeId = packetId == 0 ? curSelectRedPacketId : packetId;
            NetClient.Instance.SendMessage((ushort)CmdGuild.QueryEnvelopeInfoReq, req);
        }
        /// <summary>
        /// 请求抢红包
        /// </summary>
        public void OpenEnvelopeReq(RedPacketData data)
        {
            //抢的是自己的红包不用检查今日已抢额度
            if (Sys_Role.Instance.sRoleName.Equals(data.sendName) || CheckGetRedPacketCondition())
            {
                CmdGuildOpenEnvelopeReq req = new CmdGuildOpenEnvelopeReq();
                req.EnvelopeId = data.packetId;
                NetClient.Instance.SendMessage((ushort)CmdGuild.OpenEnvelopeReq, req);
            }
        }
        #endregion
        #region Function
        /// <summary>
        /// 改变聊天中红包链接的状态
        /// </summary>
        public void ChangeRedPacketLineState(uint packetId)
        {
            if (sendPackerChatIdDic.ContainsKey(packetId))
            {
                SendPacketData data = sendPackerChatIdDic[packetId];
                Sys_Chat.Instance.SetChatContent(data.messageId, data.newContent);
            }
        }
        /// <summary>
        /// 检查系统红包存活状态
        /// </summary>
        public void CheckSystemRedPacketActive()
        {
            for (int i = systemRedPacketList.Count-1; i >=0 ; i--)
            {
                if ((int)(systemRedPacketList[i].aliveTime - TimeManager.GetServerTime()) <= 0)
                {
                    systemRedPacketList.Remove(systemRedPacketList[i]);
                }
            }
        }
        /// <summary>
        /// 检查是否有未发送、未过期的系统红包,以及有未领取的红包
        /// </summary>
        public void CheckRedPoint()
        {
            bool isHave = false;
            if (familyData.isInFamily)
            {
                if (systemRedPacketList.Count > 0)
                {
                    for (int i = 0; i < systemRedPacketList.Count; i++)
                    {
                        if (systemRedPacketList[i].state == ESystemRedPacketState.Unsent)
                        {
                            if ((int)(systemRedPacketList[i].expireTime - TimeManager.GetServerTime()) > 0)
                            {
                                isHave= true;
                                break;
                            }
                        }
                    }
                }
                if (todayMoney < GetRedPacketLimit())
                {
                    if (showRedPacketList.Count > 0)
                    {
                        for (int i = 0; i < showRedPacketList.Count; i++)
                        {
                            if (showRedPacketList[i].state == ERedPacketState.Unclaimed)
                            {
                                isHave = true;
                                break;
                            }
                        }
                    }
                }
            }
            curIsHaveRedPacket = isHave;
        }
        public bool IsCanSendSystemRedPacket()
        {
            return systemRedPacketList.Count > 0;
        }
        /// <summary>
        /// 是否有家族红包红点
        /// </summary>
        //public bool IsRedPoint_RedPacket()
        //{
        //    return CheckIsUnsentSystemPacket();
        //}
        /// <summary>
        /// 根据累计充值积分获取红包发放限额数据
        /// </summary>
        /// <returns></returns>
        public CSVFamilyPacketLimit.Data GetRedPacketLimitData()
        {
            CSVFamilyPacketLimit.Data limitData = null;
            for (int i = 0; i < packetLimitDataList.Count; i++)
            {
                if (i + 1 < packetLimitDataList.Count)
                {
                    if (Sys_OperationalActivity.Instance.NewChargeActExp >= packetLimitDataList[i].data.Cumulative_Amount_Dowm && Sys_OperationalActivity.Instance.NewChargeActExp < packetLimitDataList[i+1].data.Cumulative_Amount_Dowm)
                    {
                        limitData = packetLimitDataList[i].data;
                        break;
                    }
                }
            }
            //未找到说明充值积分已超过最大值，取最大值
            if (limitData == null)
            {
                limitData = packetLimitDataList[packetLimitDataList.Count - 1].data;
            }
            return limitData;
        }
        /// <summary>
        /// 根据当前要发送的红包金额获取红包份数数据
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CSVFamilyPacketSection.Data GetCurPacketSectionData(uint value)
        {
            CSVFamilyPacketSection.Data sectionData = null;

            for (int i = 0, len = CSVFamilyPacketSection.Instance.Count; i < len; i++)
            {
                CSVFamilyPacketSection.Data tmp = CSVFamilyPacketSection.Instance.GetByIndex(i);
                if (tmp.Packet_Mix <= value && tmp.Packet_Max >= value)
                {
                    sectionData = tmp;
                    break;
                }
            }
            return sectionData;
        }
        /// <summary>
        /// 获取红包上限额度
        /// </summary>
        public uint GetRedPacketLimit()
        {
            uint totalValue = Sys_OperationalActivity.Instance.NewChargeActExp;
            uint targetValue = uint.Parse(CSVParam.Instance.GetConfData(1134).str_value);
            if(totalValue>0)
            {
                if (totalValue < 20000u)
                    targetValue = uint.Parse(CSVParam.Instance.GetConfData(1135).str_value);
                else
                {
                    string[] strValue = CSVParam.Instance.GetConfData(1391).str_value.Split('|');
                    for (int i = 0; i < strValue.Length; i++)
                    {
                        if (totalValue >= uint.Parse(strValue[i].Split('&')[0]))
                        {
                            targetValue = uint.Parse(strValue[i].Split('&')[1]);
                            break;
                        }
                    }
                }
            }
            return targetValue;
        }
        /// <summary>
        /// 检查抢红包条件
        /// </summary>
        /// <returns></returns>
        public bool CheckGetRedPacketCondition()
        {
            bool isCan;
            if (todayMoney < GetRedPacketLimit())
            {
                isCan = true;
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11944));
                isCan = false;
            }
            return isCan;
        }
        uint onlineMember, guidLvl;
        /// <summary>
        /// 检查发红包条件
        /// </summary>
        /// <returns></returns>
        public bool CheckSendRedPacketCondition(bool isSystemRedPacket=false)
        {
            bool isCan;
            uint familyLv = isSystemRedPacket ? 1279u : 1129u;
            uint familyOnLine = isSystemRedPacket ? 1281u : 1131u;
            uint levelId = isSystemRedPacket ? 1280u : 1130u;
            uint totalActivityId = isSystemRedPacket ? 1282u : 1132u;
            //七日内无限制
            if (Sys_Role.Instance.openServiceDay <= 7)
            {
                if (onlineMember >= uint.Parse(CSVParam.Instance.GetConfData(familyOnLine).str_value))
                {
                    //系统红包不计入周发红包额度，不用限制
                    if (isSystemRedPacket)
                    {
                        isCan = true;
                    }
                    else
                    {
                        //周发红包额度
                        if (weekMoney < GetRedPacketLimitData().Packet_Week_Quota)
                        {
                            isCan = true;
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11977));
                            isCan = false;
                        }
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11980));
                    isCan = false;
                }
            }
            else
            {
                if (guidLvl >= uint.Parse(CSVParam.Instance.GetConfData(familyLv).str_value))
                {
                    if (onlineMember >= uint.Parse(CSVParam.Instance.GetConfData(familyOnLine).str_value))
                    {
                        //角色等级
                        if (Sys_Role.Instance.Role.Level >= uint.Parse(CSVParam.Instance.GetConfData(levelId).str_value))
                        {
                            //活跃度
                            if (Sys_Daily.Instance.TotalActivity >= uint.Parse(CSVParam.Instance.GetConfData(totalActivityId).str_value))
                            {
                                //系统红包不计入周发红包额度，不用限制
                                if (isSystemRedPacket)
                                {
                                    isCan = true;
                                }
                                else
                                {
                                    //周发红包额度
                                    if (weekMoney < GetRedPacketLimitData().Packet_Week_Quota)
                                    {
                                        isCan = true;
                                    }
                                    else
                                    {
                                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11977));
                                        isCan = false;
                                    }
                                }
                            }
                            else
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11978));
                                isCan = false;
                            }
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11979));
                            isCan = false;
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11980));
                        isCan = false;
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11981));
                    isCan = false;
                }
            }
            return isCan;
        }
        //排序list
        List<SubRedPacketData> subRedPacketDataList = new List<SubRedPacketData>();
        /// <summary>
        /// 获取当前已抢完红包的最佳或最差者
        /// </summary>
        /// <param name="packetData">当前红包</param>
        /// <param name="roleName">比对角色名</param>
        /// <returns></returns>
        public int GetCurRedPacketBsetOrWorst(RedPacketData packetData, string roleName)
        {
            int type = 0;//type=0 默认啥也不是，type=1为最佳，type=2为最差
            //红包抢完了
            if (packetData.subRedPacketDataList.Count == packetData.currencyCopies)
            {
                subRedPacketDataList.Clear();
                subRedPacketDataList.AddRange(packetData.subRedPacketDataList);
                subRedPacketDataList.Sort((a, b) => {
                    return (int)(a.currencyValue - b.currencyValue);
                });
                string worstRoleName = subRedPacketDataList[0].roleName;
                string bestRoleName = subRedPacketDataList[subRedPacketDataList.Count - 1].roleName;
                type = roleName.Equals(worstRoleName) ? 2 : roleName.Equals(bestRoleName) ? 1 : 0;
            }
            return type;
        }
        public string oldBless = string.Empty;
        /// <summary>
        /// 获取语音祝福语
        /// </summary>
        public uint GetVoicePacketBlessing(int type = 0)
        {
            string bless = null;
            string[] blessArray = CSVParam.Instance.GetConfData(1137).str_value.Split('|');
            if (blessArray.Length > 0)
            {
                if (type == 0)
                {
                    bless = blessArray[0];
                }
                else//随机刷新一个
                {
                    System.Random random = new System.Random();
                    bless = blessArray[random.Next(0, blessArray.Length - 1)];
                    if (oldBless != string.Empty)
                    {
                        if (bless == oldBless)//本次随机与上次相同
                        {
                            List<string> arrayList = new List<string>();
                            foreach (var item in blessArray)
                            {
                                arrayList.Add(item);
                            }
                            arrayList.Remove(oldBless);
                            bless = arrayList[random.Next(0, arrayList.Count - 1)];
                        }
                    }
                }
            }
            oldBless = bless;
            return uint.Parse(bless);
        }

        private void RefreshMyRedPacketState(RedPacketData envelopeDetail)
        {
            bool isChange1 = false;
            bool isChange2 = false;
            int num = 0;
            for (int i = 0; i < historyRedPacketList.Count; i++)
            {
                //int diffTime = (int)(historyRedPacketList[i].logTime + uint.Parse(CSVParam.Instance.GetConfData(1125).str_value) - TimeManager.GetServerTime());
                //bool isHave = diffTime > 0 && historyRedPacketList[i].logTime == envelopeDetail.sendTime;
                if (envelopeDetail.packetId == historyRedPacketList[i].packetId)//&& isHave
                {
                    num++;
                    isChange1 = historyRedPacketList[i].state != envelopeDetail.state;
                    historyRedPacketList[i].state = envelopeDetail.state;
                    if(num==2)
                        break;
                }
            }
            for (int i = 0; i < showRedPacketList.Count; i++)
            {
                if (envelopeDetail.packetId == showRedPacketList[i].packetId)
                {
                    isChange2 = historyRedPacketList[i].state != envelopeDetail.state;
                    showRedPacketList[i].state = envelopeDetail.state;
                    break;
                }
            }
            if(isChange1 || isChange2)
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.UpdateHistoryRedPacket,true);
        }
        /// <summary>
        /// 刷新日抢红包、周发红包限额时间
        /// </summary>
        private void RefreshPacketTime(ERefreshTimerType type)
        {
            int diff=0;
            Action action=null;
            if (type == ERefreshTimerType.DayLimit)
            {
                diff = (int)(todayLastRefresh - TimeManager.GetServerTime());
                action = () => {
                    if ((int)(todayLastRefresh - TimeManager.GetServerTime()) <= 0)
                    {
                        todayMoney = 0;
                        if (timerDic.ContainsKey(ERefreshTimerType.DayLimit))
                        {
                            timerDic[ERefreshTimerType.DayLimit]?.Cancel();
                            timerDic.Remove(ERefreshTimerType.DayLimit);
                        }
                    }
                };
            }
            else if(type == ERefreshTimerType.WeekLimit)
            {
                diff = (int)(weekLastRefresh - TimeManager.GetServerTime());
                action = () => {
                    if ((int)(weekLastRefresh - TimeManager.GetServerTime()) <= 0)
                    {
                        weekMoney = 0;
                        if (timerDic.ContainsKey(ERefreshTimerType.WeekLimit))
                        {
                            timerDic[ERefreshTimerType.WeekLimit]?.Cancel();
                            timerDic.Remove(ERefreshTimerType.WeekLimit);
                        }
                    }
                };
            }
            CreateTimer(type, diff, action);
        }
        /// <summary>
        /// 创建刷新倒计时
        /// </summary>
        /// <returns></returns>
        private void CreateTimer(ERefreshTimerType type, float duration, Action action)
        {
            if (timerDic.ContainsKey(type))
            {
                timerDic[type]?.Cancel();
                timerDic.Remove(type);
            }
            if (duration <= 0)
            {
                if (type == ERefreshTimerType.DayLimit)
                {
                    todayMoney = 0;
                    return;
                }
                else
                {
                    weekMoney = 0;
                    return;
                }
            }
            Timer timer = Timer.Register(duration, ()=> {
                action?.Invoke();
            }, null, false, true);
            timerDic[type] = timer;
        }
        private void ClearAllDicTime()
        {
            foreach (var item in timerDic.Values)
            {
                item?.Cancel();
            }
            timerDic.Clear();
        }
        /// <summary>
        /// 检查语音红包是否检查通过
        /// </summary>
        public string CheckVoiceIsPass(string content)
        {
            string str = string.Empty;
            char[] charTarget = curSelectRedPacketData.content.ToCharArray();
            char[] charStr = content.ToCharArray();
            int passCount = 0;
            for (int i = 0; i < charTarget.Length; i++)
            {
                for (int j = 0; j < charStr.Length; j++)
                {
                    if (charTarget[i] == charStr[j])
                    {
                        passCount++;
                    }
                }
            }
            if ((float)Math.Round((double)(passCount / charTarget.Length), 4) * 10000 > float.Parse(CSVParam.Instance.GetConfData(1136).str_value))
            {
                //内容比对通过
                str = curSelectRedPacketData.content;
                voiceIsCanSend = true;
            }
            else
                voiceIsCanSend = false;

            Sys_Family.Instance.eventEmitter.Trigger(Sys_Family.EEvents.OnVoiceRecordIsPass);
            return str;
        }
 
        public uint RedPacketIdDequeue()
        {
            List<uint> commonPacketIdList = new List<uint>();
            for (int i = 0; i < getRedPacketIds.Count; i++)
            {
                for (int j = 0; j < openedPacketIdList.Count; j++)
                {
                    //此红包已被打开 剔除
                    if (getRedPacketIds[i] == openedPacketIdList[j])
                    {
                        commonPacketIdList.Add(getRedPacketIds[i]);
                    }
                }
            }
            for (int i = 0; i < commonPacketIdList.Count; i++)
            {
                getRedPacketIds.Remove(commonPacketIdList[i]);
            }
            commonPacketIdList.Clear();
            if (getRedPacketIds.Count > 0)
            {
                curSelectRedPacketId = getRedPacketIds[0];
                getRedPacketIds.RemoveAt(0);
            }
            else
                curSelectRedPacketId = 0;
            return curSelectRedPacketId;
        }
        /// <summary>
        /// 添加收到的红包id到队列
        /// </summary>
        public void AddRedPacketId(uint id)
        {
            //今日抢红包未达上限
            if (todayMoney < GetRedPacketLimit())
            {
                getRedPacketIds.Add(id);
                //界面未打开
                if (!getRedPacketViewIsShow)
                {
                    RedPacketIdDequeue();
                    Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnReceiveRedPacket);
                }
            }
        }
        public void HideTime()
        {
            hideTime = Timer.Register((float)Math.Round(float.Parse(CSVParam.Instance.GetConfData(1128).str_value) / 1000, 2), () => {
                RedPacketIdDequeue();
                Sys_Family.Instance.eventEmitter.Trigger(EEvents.OnReceiveRedPacket);
            }, null, false, true);
        }
        public void ShowTime(UnityEngine.GameObject obj)
        {
            showTime = Timer.Register((float)Math.Round(float.Parse(CSVParam.Instance.GetConfData(1127).str_value) / 1000, 2), () =>
            {
                obj.SetActive(false);
                if (getRedPacketIds.Count > 0)
                    HideTime();
                else
                {
                    getRedPacketViewIsShow = false;
                    CancelTime();
                }
            }, null, false, true);
        }
        public void CancelTime()
        {
            showTime?.Cancel();
            hideTime?.Cancel();
        }
        #endregion
    }
    /// <summary>
    /// 系统红包数据
    /// </summary>
    public class SystemRedPacketData
    {
        public uint packetId;                   //红包id
        public ESystemRedPacketState state;     //系统红包状态
        public uint aliveTime;                  //存活时间
        public uint expireTime;                 //未发送的过期时间
        public CSVFamilyPacket.Data packetData;
    }
    /// <summary>
    /// 家族红包大厅八个红包展示数据
    /// </summary>
    public class ShowRedPacketData
    {
        public uint packetId;                       //红包id  
        public string sendName;                     //发送者
        public uint sendTime;                       //发送时间
        public ERedPacketState state;               //红包状态
        public string content;                      //红包祝福语
    }
    /// <summary>
    /// 所有的红包历史数据
    /// </summary>
    public class RedPacketHistoryData
    {
        public uint packetId;                       //红包id  
        public uint logTime;                        //发送时间
        public string sendName;                     //发送者
        public ERedPacketState state;               //红包状态

        public uint finishTime;                     //红包抢完所需时间
        public uint currencyAllValue;               //红包总金额
        public string luckyBoy;                     //手气最佳者
        public uint curTime;

        public string content;

        public void Init()
        {
            DateTime dateTime = TimeManager.GetDateTime(curTime);
            string timeStr = dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            //抢完了
            if (finishTime != 0)
            {
                uint diff = finishTime - logTime;
                string strDiff;

                uint hour = diff / 3600;
                uint min = (diff - hour * 3600) / 60;
                uint sen = diff - hour * 3600 - min * 60;

                if (diff < 60)
                {
                    strDiff = string.Format("{0}{1}", diff, LanguageHelper.GetTextContent(11994));
                }
                else if (diff < 3600)
                {
                    if (sen != 0)
                        strDiff = string.Format("{0}{1}{2}{3}", min, LanguageHelper.GetTextContent(11995), sen, LanguageHelper.GetTextContent(11994));
                    else
                        strDiff = string.Format("{0}{1}", min, LanguageHelper.GetTextContent(11995));
                }
                else
                {
                    if (min != 0)
                    {
                        if (sen != 0)
                            strDiff = string.Format("{0}{1}{2}{3}{4}{5}", hour, LanguageHelper.GetTextContent(11996), min, LanguageHelper.GetTextContent(11995), sen, LanguageHelper.GetTextContent(11994));
                        else
                            strDiff = string.Format("{0}{1}{2}{3}", hour, LanguageHelper.GetTextContent(11996), min, LanguageHelper.GetTextContent(11995));
                    }
                    else
                    {
                        if (sen != 0)
                            strDiff = string.Format("{0}{1}{2}{3}", hour, LanguageHelper.GetTextContent(11996), sen, LanguageHelper.GetTextContent(11994));
                        else
                            strDiff = string.Format("{0}{1}", hour, LanguageHelper.GetTextContent(11996));
                    }
                }
                content = LanguageHelper.GetTextContent(11937, timeStr, sendName, currencyAllValue.ToString(), strDiff, luckyBoy);
            }
            else
                content = LanguageHelper.GetTextContent(11935, timeStr, sendName);
        }
    }
    /// <summary>
    /// 单个红包数据
    /// </summary>
    public class RedPacketData
    {
        public string sendName;
        public uint heroId;
        public uint headId;
        public uint packetId;                           //红包id
        public uint sendTime;                           //发送时间
        public ERedPacketState state;                   //红包状态
        public ERedPacketType type;                     //红包类型
        public uint cover;                              //红包封面(预留，目前固定封面)
        public uint currencyValue;                      //抢到的红包金额   
        //一下打开红包才有的数据
        public uint currencyAllValue;                   //红包总金额   
        public uint currencyCopies;                     //红包份数 
        public string content;                          //红包祝福语
        public ulong costTime;                          //红包领取完所需时间
        public bool isPastDue;                          //是否过期
        public List<SubRedPacketData> subRedPacketDataList;  //红包领取记录
    }
    /// <summary>
    /// 抢红包列表展示数据
    /// </summary>
    public class SubRedPacketData
    {
        public string roleName;
        public uint heroId;
        public uint headId;
        public uint currencyValue;                      //红包金额   
    }
}