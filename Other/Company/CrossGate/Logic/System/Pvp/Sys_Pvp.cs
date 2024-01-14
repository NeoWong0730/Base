using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Net;
using Packet;
using Logic.Core;
using Lib.Core;
using Framework;

namespace Logic
{
    public partial class Sys_Pvp : SystemModuleBase<Sys_Pvp>,ISystemModuleUpdate
    {
        static uint PvpMapID = 1401;
        public enum EEvents
        {
            PvpInfoRefresh,//赛季信息更新
            StartMatch,//开始匹配
            CancleMatch,//取消匹配
            MatchSuccess,//匹配成功
            LoadCompl,//加载完成
            OneReadyOk,//一个加载完成
            DanLvUpAward,//晋级奖励列表
            GetDanLvUpAward,//获得晋级奖励
            GetAllDanLvUpAward,
            RankList,//排行榜
            MySelfLoadOk,
            GetCumulateAward,//累胜奖励
            ShowSeasonAward,

        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init()
        {
            #region 
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.InfoRes, Notify_ArneaInfo, CmdArenaInfoRes.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.StartMatchRes, Notify_StartMatch, CmdArenaStartMatchRes.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.CancelMatchRes, Notify_CancleMatch, CmdArenaCancelMatchRes.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.MatchDataNtf, Notify_MatchResult, CmdArenaMatchDataNtf.Parser);
          //  EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.LoadCompleteNtf, Notify_Complete, CmdArenaLoadCompleteNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.DanLvUpAwardRes, Notify_DanLvUpAward, CmdArenaDanLvUpAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.GetDanLvUpAwardRes, Notify_GetDanLvUpAward, CmdArenaGetDanLvUpAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.GetAllDanUpAwardRes, Notify_GetAllDanUpAward, CmdArenaGetAllDanUpAwardRes.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.RankListRes, Notify_RankList, CmdRankQueryRes.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.ReMatchNtf, Notify_ReMatch, CmdArenaReMatchNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.SeasonAwardNtf, Notify_SeasonAward, CmdArenaSeasonAwardNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.FightEndNtf, Notify_FightEnd, CmdArenaFightEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.GetCumulateAwardRes, Notify_GetCumulateAward, CmdArenaGetCumulateAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.ShowBoxNtf, Notify_ShowSeasonReward, CmdArenaShowBoxNtf.Parser);
           // EventDispatcher.Instance.AddEventListener(0, (ushort)CmdArena.CloseMatchPanelNtf, Notify_CloseMatchNtf, CmdArenaCloseMatchPanelNtf.Parser);

            #endregion

            #region
            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_RankList, true);
            Sys_Rank.Instance.eventEmitter.Handle(Sys_Rank.EEvents.RankNextTimeReset, Notify_RankReset, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelOpen>(Sys_Match.EEvents.MatchPanelOpen, Notify_StartMatch, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchPanelClose, Notify_CloseMatcherPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchCancle, Notify_CancleMatch, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherInfo>(Sys_Match.EEvents.MatcherInfo, Notify_MatchResult, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherLoadOk>(Sys_Match.EEvents.MatcherLoadOk, Notify_MatcherLoadOK, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelOpen>(Sys_Match.EEvents.LoadPanelOpen, Notify_OpenLoadPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelClose>(Sys_Match.EEvents.LoadPanelClose, Notify_CloseLoadPanel, true);

            #endregion

            ParseLevelStage();
        }

        public override void Dispose()
        {
            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_RankList, false);
        }
        public override void OnLogin()
        {

            base.OnLogin();

        }

        public override void OnLogout()
        {
            base.OnLogout();
        }

        public void OnUpdate()
        {
            if (IsPvpLoading)
            {
               var vlaue =  Time.time - LoadingStartTimePoint;
               if (vlaue >= 3f)
               {
                    if (Sys_Map.Instance.CurMapId == PvpMapID && GameCenter.nLoadStage == 3)
                    {

                        IsPvpLoading = false;
                       // Apply_LoadOK();
                        
                    }
               }
            }


            if (m_bWaitResult)
            {
                if (Sys_Map.Instance.CurMapId != PvpMapID && GameCenter.nLoadStage == 3)
                {
                    m_bWaitResult = false;

                    UIManager.OpenUI(EUIID.UI_Pvp_SingleFinale);
                }
                  
            }

            if (m_bWaitReMatch)
            {
                if (Sys_Map.Instance.CurMapId != PvpMapID && GameCenter.nLoadStage == 3)
                {
                    if (m_ReMatchMsgID != 0)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(m_ReMatchMsgID));
                        m_ReMatchMsgID = 0;
                    }
                    m_bWaitReMatch = false;

                    Apply_Mathch();

                    UIManager.OpenUI(EUIID.UI_Pvp_SingleMatch);
                }
            }
        }
    }

    #region single pvp

    /// <summary>
    /// 数据
    /// </summary>
    public partial class Sys_Pvp : SystemModuleBase<Sys_Pvp>
    {
        public class PvpItem
        {
            public ulong roleID { get; set; }

            public bool LoadOK { get; set; } = false;

            public FightPlayer Info { get; set; }

            public bool IsPlayerSelf { get { return Sys_Role.Instance.RoleId == roleID; } }

             
        }



        public bool IsPvpLoading { get; private set; } = false;

        public float LoadingStartTimePoint { get; private set; }
        /// <summary>
        /// 排行榜信息
        /// </summary>
        public CmdRankQueryRes RankInfo { get; private set; }

        /// <summary>
        /// 赛季信息
        /// </summary>
        private CmdArenaInfoRes m_InfoRes = new CmdArenaInfoRes();
        public CmdArenaInfoRes MyInfoRes
        {
            get { return m_InfoRes; }
        }
        /// <summary>
        /// 第几赛季
        /// </summary>
        public int SeasonNum { get { return m_InfoRes.StageIndex; } }

        /// <summary>
        /// 赛季开始时间
        /// </summary>
        private DateTime m_PvpStarTime;

        public DateTime PvpStarTime { get { return m_PvpStarTime; } }

        /// <summary>
        /// 赛季结束时间
        /// </summary>
        private DateTime m_PvpEndTime;
        public DateTime PvpEndTime { get { return m_PvpEndTime; } }


        public uint PvpEndTimesampe { get { return m_InfoRes.EndTime; } }
        public uint PvpStartTimesampe { get { return m_InfoRes.StartTime; } }

        /// <summary>
        /// 段位
        /// </summary>
        /// 
        public int Level { get; private set; }

        public int LastLevel { get; private set; }


        public int LastLevelID{get;private set;}
        public bool IsLevelEffect { get; set; } = false;

        public bool IsStarEffect { get; set; } = false;
        /// <summary>
        /// 段位ID
        /// </summary>
        public int LevelID { get { return m_InfoRes.DanLv; } }

        /// <summary>
        /// 星数
        /// </summary>
        public int Star { get { return m_InfoRes.Star; } }

        public int LastStar { get; private set; }
        public bool isBigStar { get; private set; }
        /// <summary>
        /// 排名
        /// </summary>
        public int MineRank { get { return m_InfoRes.MyRank; } }


        public int SeasonWeek { get; private set; }


        /// <summary>
        /// 匹配结果
        /// </summary>
        private CmdMatchNotifyMatcherInfo MatchDataNtf { get; set; }

        public bool MatchOtherLoadOK { get; private set; } = false;

        private List<ulong> m_MatchLoadOKList = new List<ulong>();

        private Dictionary<ulong, PvpItem> m_PvpItemDic = new Dictionary<ulong, PvpItem>();

        public Dictionary<ulong, PvpItem> PvpObjects { get { return m_PvpItemDic; } }

        /// <summary>
        /// pvp 信息更新的本地时间点
        /// </summary>
        public uint InfoRefreshTime { get; private set; }
        /// <summary>
        /// 等级分段，配置文件
        /// </summary>
        int[] m_LevelStageValue;

        public int[] LevelStageValue { get { return m_LevelStageValue; } }

        /// <summary>
        /// 晋级奖励信息
        /// </summary>
        public CmdArenaDanLvUpAwardRes DanLvUpAward { get; private set; }

        /// <summary>
        /// 赛季奖励，开启新赛季前推送
        /// </summary>
        public CmdArenaSeasonAwardNtf SewasonAwardInfo { get; private set; }


        public bool IsNewSeason { get;  set; } = false;
        /// <summary>
        /// pvp结果
        /// </summary>
        public CmdArenaFightEndNtf FinghtEndInfo { get; private set; }

        private bool m_bWaitResult { get; set; } = false;

        private bool m_bWaitReMatch = false;
        private uint m_ReMatchMsgID = 0;
        public IList<ArenaDanInfo> DanLvUpAwardList {
            get {
                if (DanLvUpAward == null)
                    return null;

                return DanLvUpAward.AwardList;
            }
        }


        private Dictionary<uint, CmdRankQueryRes> m_DicRank = new Dictionary<uint, CmdRankQueryRes>();
        private Dictionary<uint, uint> m_DicRankAppling = new Dictionary<uint, uint>();
        private int CacleSeasonWeek()
        {
            var serverTime = TimeManager.GetServerTime();

            var serverDate = TimeManager.GetDateTime(serverTime);

            return (serverDate - PvpStarTime).Days / 7 + 1;
        }

        /// <summary>
        /// 获取累计胜利奖励状态 0 不可领 1 可以领 2 已经领
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public int GetCumulativeVictoryRewardState(int num)
        {
            var list =  m_InfoRes.WinInfo;

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {

                if (list[i].WinNum == num)
                {
                    if (list[i].IsGet)
                        return 2;

                    if (list[i].CanGet)
                        return 1;
                    else
                        return 0;
                }

            }

            return 0;
        }

        /// <summary>
        /// 读取等级分段
        /// </summary>
        /// <returns></returns>
        private void ParseLevelStage()
        {
            var levelstatges = PvpStageLvHelper.GetLevelStage();

            int length = levelstatges.Count;

            m_LevelStageValue = new int[length];

            for (int i = 0; i < length; i++)
            {
                m_LevelStageValue[i] = levelstatges[i];
            }
 
        }

        /// <summary>
        /// 获取当前的等级分段
        /// </summary>
        /// <returns></returns>
        public int GetLevelStage()
        {
            if (m_InfoRes == null)
                return 0;

            int stage = 0;

            int level = (int)Sys_Role.Instance.Role.Level;
            for (int i = m_LevelStageValue.Length-1; i > 0; i--)
            {
                if (level >= m_LevelStageValue[i])
                    break;

                stage = i;

            }

            return stage;
        }

        public List<int> GetCumulativeVictoryNum()
        {
             var levelStage  = GetLevelStage();

            var data = CSVArenaCumlativeRewards.Instance.GetConfData((uint)levelStage);

            List<int> value = new List<int>();

            if (data != null)
            {
                var datalist = data.CumulativeWin;

                int count = datalist.Count;

                for (int i = 0; i < count; i++)
                {
                    value.Add((int)datalist[i][0]);
                }
            }

            return value;
        }

        public List<ItemIdCount> GetCumulativeVictoryReward(int num)
        {
            int state =  GetLevelStage();

            var data = CSVArenaCumlativeRewards.Instance.GetConfData((uint)state);

            var datalist = data.CumulativeWin;

            int count = datalist.Count;

            uint resultID = 0;
            for (int i = 0; i < count; i++)
            {
                var item = datalist[i];

                if (item[0] == num)
                {
                    resultID = item[1];
                    break;
                }
            }

            var rewardlist = CSVDrop.Instance.GetDropItem(resultID);

            return rewardlist;
        }

        private void ResetPvpItemDic()
        {
            m_PvpItemDic.Clear();

            m_PvpItemDic.Add(Sys_Role.Instance.RoleId, new PvpItem() { roleID = Sys_Role.Instance.RoleId });
        }
        private void AddPvpItem(CmdMatchNotifyMatcherInfo info)
        {
            int count = info.Red.Players.Count;
            for (int i = 0; i < count; i++)
            {
                var value = info.Red.Players[i];

                if (m_PvpItemDic.ContainsKey(value.RoleId) == false)
                {
                    m_PvpItemDic.Add(value.RoleId, new PvpItem() { roleID = value.RoleId, Info = value });

                }
                else
                    m_PvpItemDic[value.RoleId].Info = value;
            }

            count = info.Blue.Players.Count;

            for (int i = 0; i < count; i++)
            {
                var value = info.Blue.Players[i];

                if (m_PvpItemDic.ContainsKey(value.RoleId) == false)
                {
                    m_PvpItemDic.Add(value.RoleId, new PvpItem() { roleID = value.RoleId, Info = value });

                }
                else
                    m_PvpItemDic[value.RoleId].Info = value;
            }
        }

        private void UpdataArenaInfo(CmdArenaInfoRes info)
        {
            int nlastLevel = Level;
            int lastStar = Star;

            int lastLevelID = LevelID;

            var nowtime = Sys_Time.Instance.GetServerTime();

            m_InfoRes = info;

            m_PvpStarTime = TimeManager.GetDateTime(info.StartTime+8*3600u);

            m_PvpEndTime = TimeManager.GetDateTime(info.EndTime+8*3600u);

            var infoData = CSVArenaSegmentInformation.Instance.GetConfData((uint)LevelID);

            Level = (int)infoData.Rank;

            var starData = CSVArenaMaxStars.Instance.GetConfData(infoData.Rank);

            isBigStar = (starData != null && starData.MaxStar == 999) ? true : false;


            SeasonWeek = info.StartTime <= nowtime ? CacleSeasonWeek() : 0;

            //            if (lastLevel != Level || Star != lastStar)
            UpdatePvpLevelIcon();

            InfoRefreshTime = nowtime;

            if (Level != nlastLevel && nlastLevel != 0)
            {
                if (!IsLevelEffect)
                    IsLevelEffect = true;
                LastLevel = nlastLevel;
                LastLevelID = lastLevelID;
            }

            if (Level == nlastLevel && nlastLevel != 0 && lastStar != Star)
            {
                if (!IsStarEffect)
                    IsStarEffect = true;

                LastStar = lastStar;
            }
        }

        /// <summary>
        /// 获取赛季剩余时间
        /// </summary>
        /// <returns></returns>
        public uint GetRemainingTime()
        {
            var matchData = CSVMatch.Instance.GetConfData(1);

            var nowtime = Sys_Time.Instance.GetServerTime();

            uint endtime = (uint)m_InfoRes.LeftSec;//结束的时间戳
            uint starttime = endtime - matchData.Duration;

            uint time = 0;

            if (nowtime >= starttime && nowtime < endtime)
                time = endtime - nowtime;

            return time;
        }

        public uint GetNextStartTime()
        {
            var matchData = CSVMatch.Instance.GetConfData(1);

            var nowtime = Sys_Time.Instance.GetServerTime();

            uint endtime = (uint)m_InfoRes.LeftSec;
            uint starttime = endtime - matchData.Duration;

            uint time = 0;

            var todayZero = Sys_Time.Instance.GetDayZeroTimestamp();

            if (m_InfoRes.LeftSec - (uint)todayZero > 86400)
            {
                return 0;

            }
            if (starttime > nowtime)
            {
                time = starttime - nowtime;
            }

            return time;
        }
        /// <summary>
        /// 距离下次赛季开始时间
        /// </summary>
        /// <returns></returns>
        public int GetNextSatrtTime()
        {
            var data = CSVDailyActivity.Instance.GetConfData(70);

            var todayNowSce = (int)Sys_Daily.Instance.GetTodayTimeSceond();

            if (data.OpeningTime == null)
            {
                return 0;
            }

            int count = data.OpeningTime.Count;

            for (int i = 0; i < count; i++)
            {
                var timeA = data.OpeningTime[i];

                int starsec = timeA[0] * 3600 + timeA[1] * 60;
                int endsec = starsec + (int)data.Duration;

                if (starsec > todayNowSce)
                {
                    return starsec - todayNowSce;
                }
            }

            return 0;
        }

        /// <summary>
        /// 今天的活动时间段都完成了
        /// </summary>
        /// <returns></returns>
        public bool TodayAcitityIsOver()
        {
            var data = CSVDailyActivity.Instance.GetConfData(70);

            if (data.OpeningTime == null)
                return false;

            DateTime dateTime = DateTime.Now;

            int count = data.OpeningTime.Count;

            if (count == 0)
                return false;

            var timeA = data.OpeningTime[count - 1];

            int hour = dateTime.Hour;
            int min = dateTime.Minute;

            int endhour = timeA[0] + ((timeA[1] + (int)data.Duration / 60) / 60);
            int endMin = ((timeA[1] + (int)data.Duration % 60) % 60);

            if (hour >= endhour && min > endMin)
                return true;

            return false;
        }
        public ArenaDanInfo GetDanInfo(int danid)
        {
            if (DanLvUpAwardList == null)
                return null;

            int count = DanLvUpAwardList.Count;

            for (int i = 0; i < count; i++)
            {
                if (DanLvUpAwardList[i].DanLv == danid)
                {
                    return DanLvUpAwardList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 清除临时数据，为匹配存储数据
        /// </summary>
        private void ClearCahce()
        {
            ResetPvpItemDic();

            m_MatchLoadOKList.Clear();
        }

        private void StartPvpLoading()
        {
            IsPvpLoading = true;
            LoadingStartTimePoint = Time.time;

        }

        private void EndPvpLoad()
        {
            IsPvpLoading = false;
            LoadingStartTimePoint = Time.time;
        }

        public bool IsPvpActive(bool tips = true)
        {
            //var result = Sys_Daily.Instance.isDailyReady(70, false);

            //if (tips && !result)
            //{
            //    var data = Sys_Daily.Instance.WillOpenDaily.Find(o => o.ID == 70);

            //    if (data != null)
            //    {
            //        var nowtime = Sys_Daily.Instance.GetTodayTimeSceond();

            //        int offset = data.OpenTime - (int)nowtime;

            //        if (offset > 0)
            //        {
            //            int hour = offset / 3600;
            //            int mins = (offset - hour * 3600) / 60;
            //            int senc = offset - hour * 3600 - mins * 60;

            //            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11620, hour.ToString(), mins.ToString(), senc.ToString()));
            //        }

            //    }
            //}

            // return true;
            return true;
        }
    }
    public partial class Sys_Pvp : SystemModuleBase<Sys_Pvp>
    {
        /// <summary>
        /// 收到PVP信息
        /// </summary>
        /// <param name="msg"></param>
        private void Notify_ArneaInfo(NetMsg msg)
        {
           // Sys_Hint.Instance.PushContent_Normal("收到服务器返回：CmdArenaInfoRes");

            CmdArenaInfoRes info = NetMsgUtil.Deserialize<CmdArenaInfoRes>(CmdArenaInfoRes.Parser, msg);

            UpdataArenaInfo(info);

            eventEmitter.Trigger(EEvents.PvpInfoRefresh);

           
        }


        /// <summary>
        /// 判断伙伴的配置是否符合PVP规则
        /// </summary>
        /// <param name="value"></param>
        private bool DoPartnerFormatWithRule(IList<uint> listValue)
        {
            int ruleCount = 0;

            foreach (var item in listValue)
            {
                var partnerData = CSVPartner.Instance.GetConfData(item);

                if (partnerData.profession == 401 || partnerData.profession == 601)
                    ruleCount += 1;
            }
            return ruleCount <= 3;
        }


        private void Notify_StartMatch(CmdMatchNotifyMatchPanelOpen msg)
        {
            if (msg.Matchtype != (uint)EMatchType.MatchTypeArena)
                return;

            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event34);

            eventEmitter.Trigger(EEvents.StartMatch);

            UIManager.OpenUI(EUIID.UI_Pvp_SingleMatch);
        }

        private void Notify_CancleMatch(CmdMatchNotifyMatchPanelClose msg)
        {

            eventEmitter.Trigger(EEvents.CancleMatch);
        }

        private void Notify_CloseMatcherPanel(CmdMatchNotifyMatchPanelClose msg)
        {

        }
        private void Notify_MatchResult(CmdMatchNotifyMatcherInfo msg)
        {
            if (Sys_Match.Instance.MatchType != EMatchType.MatchTypeArena)
                return;

            // CmdArenaMatchDataNtf info = NetMsgUtil.Deserialize<CmdArenaMatchDataNtf>(CmdArenaMatchDataNtf.Parser, msg);

            MatchDataNtf = msg;

            AddPvpItem(msg);

            StartPvpLoading();

            eventEmitter.Trigger(EEvents.MatchSuccess);

            if (UIManager.IsOpen(EUIID.UI_Pvp_SingleMatch))
                UIManager.CloseUI(EUIID.UI_Pvp_SingleMatch, true);

           

            UIManager.OpenUI(EUIID.UI_Pvp_SingleLoading);
        }

        private void Notify_MatcherLoadOK(CmdMatchNotifyMatcherLoadOk msg)
        {
            if (Sys_Match.Instance.MatchType != EMatchType.MatchTypeArena)
                return;

            int count = msg.Oks.Roles.Count;

            for (int i = 0; i < count; i++)
            {
                var value = msg.Oks.Roles[i];
                if (m_PvpItemDic.ContainsKey(value) && m_PvpItemDic[value].LoadOK == false)
                {
                    m_PvpItemDic[value].LoadOK = true;
                    eventEmitter.Trigger<ulong>(EEvents.OneReadyOk, value);

                }
            }
        }

        private void Notify_CloseLoadPanel(CmdMatchNotifyLoadPanelClose msg)
        {

            eventEmitter.Trigger(EEvents.LoadCompl);
        }

        private void Notify_OpenLoadPanel(CmdMatchNotifyLoadPanelOpen msg)
        {
            if (msg.Matchtype != (uint)EMatchType.MatchTypeArena)
                return;

            UIManager.OpenUI(EUIID.UI_Pvp_SingleLoading);
        }
        //private void Notify_Complete(NetMsg msg)
        //{
        //    CmdArenaLoadCompleteNtf info = NetMsgUtil.Deserialize<CmdArenaLoadCompleteNtf>(CmdArenaLoadCompleteNtf.Parser, msg);

        //    int count = info.RoleId.Count;

        //    for (int i = 0; i < count; i++)
        //    {
        //        var value = info.RoleId[i];
        //        if (m_PvpItemDic.ContainsKey(value) && m_PvpItemDic[value].LoadOK == false)
        //        {
        //            m_PvpItemDic[value].LoadOK = true;
        //            eventEmitter.Trigger<ulong>(EEvents.OneReadyOk, value);

        //            //Sys_Hint.Instance.PushContent_Normal("load ok :" + value);

        //            //Debug.LogError("load ok :" + value);
        //        }
        //    }

        //    if (isMatchAllLoadSingleOK())
        //    {
        //        EndPvpLoad();
        //    }
               
        //}

        private bool isMatchAllLoadSingleOK()
        {
            if (m_PvpItemDic.Count <= 1)
                return false;

            bool result = true;

            foreach (var kvp in m_PvpItemDic)
            {
                if (kvp.Value.LoadOK == false)
                {
                    result = false;
                }
            }

            return result;
        }

        public bool IsHaveDanLvUpAward()
        {
            if (DanLvUpAwardList == null)
                return false;

            int count = DanLvUpAwardList.Count;

            for (int i = 0; i < count; i++)
            {
                if (DanLvUpAwardList[i].IsGet == false && DanLvUpAwardList[i].CanGet)
                {
                    return true;
                }
            }

            return false;
        }

        private void Notify_DanLvUpAward(NetMsg msg)
        {
            CmdArenaDanLvUpAwardRes info = NetMsgUtil.Deserialize<CmdArenaDanLvUpAwardRes>(CmdArenaDanLvUpAwardRes.Parser, msg);

            DanLvUpAward = info;

            eventEmitter.Trigger(EEvents.DanLvUpAward);
        }


        private void Notify_GetDanLvUpAward(NetMsg msg)
        {
            CmdArenaGetDanLvUpAwardRes info = NetMsgUtil.Deserialize<CmdArenaGetDanLvUpAwardRes>(CmdArenaGetDanLvUpAwardRes.Parser, msg);


            int count = DanLvUpAwardList.Count;

            ArenaDanInfo value = null;

            for (int i = 0; i < count; i++)
            {
                if (DanLvUpAwardList[i].DanLv == info.DanLv)
                {
                    DanLvUpAwardList[i].IsGet = true;

                    value = DanLvUpAwardList[i];

                    break;
                }
                     
            }
            eventEmitter.Trigger<ArenaDanInfo>(EEvents.GetDanLvUpAward, value);
        }


        private void Notify_GetAllDanUpAward(NetMsg msg)
        {
            CmdArenaGetAllDanUpAwardRes info = NetMsgUtil.Deserialize<CmdArenaGetAllDanUpAwardRes>(CmdArenaGetAllDanUpAwardRes.Parser, msg);

            int count  = info.DanLv.Count;

            List<ArenaDanInfo> list = new List<ArenaDanInfo>(count);

            for (int i = 0; i < count; i++)
            {
                var item = GetDanInfo((int)info.DanLv[i]);

                if (item != null)
                {
                    item.IsGet = true;
                    list.Add(item);
                }
            
            }

            if(list.Count > 0)
               eventEmitter.Trigger<List<ArenaDanInfo>>(EEvents.GetAllDanLvUpAward, list);

            list.Clear();
        }


        private void Notify_RankList(CmdRankQueryRes msg)
        {
            // CmdRankQueryRes info = NetMsgUtil.Deserialize<CmdRankQueryRes>(CmdRankQueryRes.Parser, msg);
            if (msg.Type != (uint)RankType.Arena)
                return;


            if (m_DicRank.ContainsKey(msg.GroupType) == false)
            {
                m_DicRank.Add(msg.GroupType, msg);
            }
            else
            {
                m_DicRank[msg.GroupType] = msg;
            }

            if (m_DicRankAppling.ContainsKey(msg.GroupType))
            {
                m_DicRankAppling.Remove(msg.GroupType);
            }

            RankInfo = msg;

            eventEmitter.Trigger(EEvents.RankList);
        }

        private void Notify_RankReset()
        {
            foreach (var kvp in m_DicRank)
            {
                kvp.Value.NextReqTime = 0;
            }
        }
        public RankUnitData GetSelfArenaData()
        {
            RankUnitData rankUnitData = null;
            if (RankInfo != null && RankInfo.Units != null)
            {
                if (RankInfo.Units.Count > 0)
                {
                    for (int i = 0; i < RankInfo.Units.Count; i++)
                    {
                        if (RankInfo.Type == (uint)RankType.Arena)
                        {
                            if (RankInfo.Units[i].ArenaData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                rankUnitData = RankInfo.Units[i];
                                break;
                            }
                        }
                    }

                    if (rankUnitData == null && MyInfoRes != null)
                    {
                        rankUnitData = new RankUnitData();

                        rankUnitData.ArenaData = new RankDataArena();
                        rankUnitData.ArenaData.DanLv = MyInfoRes.DanLv;
                        rankUnitData.ArenaData.Star = MyInfoRes.Star;
                        rankUnitData.ArenaData.GlobalRank = (uint)MyInfoRes.MyRank;
                        rankUnitData.ArenaData.Name = Sys_Role.Instance.Role.Name;
                    }
                }
            }
            return rankUnitData;
        }
        ////private void Notify_ReMatch(NetMsg msg)
        ////{
        ////    CmdArenaReMatchNtf info  = NetMsgUtil.Deserialize<CmdArenaReMatchNtf>(CmdArenaReMatchNtf.Parser, msg);

        ////    m_bWaitReMatch = true;
        ////    m_ReMatchMsgID = info.MsgId ;
        ////    EndPvpLoad();

        ////    eventEmitter.Trigger(EEvents.LoadCompl);

           
        ////}

        private void Notify_SeasonAward(NetMsg msg)
        {
            CmdArenaSeasonAwardNtf info = NetMsgUtil.Deserialize<CmdArenaSeasonAwardNtf>(CmdArenaSeasonAwardNtf.Parser, msg);

            SewasonAwardInfo = info;

            // IsNewSeason = true;


            eventEmitter.Trigger(EEvents.ShowSeasonAward);

        }

        private void Notify_FightEnd(NetMsg msg)
        {

            //Sys_Hint.Instance.PushContent_Normal("PVP 收到结算");

            //Debug.LogError("PVP 收到结算");

            CmdArenaFightEndNtf info = NetMsgUtil.Deserialize<CmdArenaFightEndNtf>(CmdArenaFightEndNtf.Parser, msg);

            FinghtEndInfo = info;


            m_bWaitResult = true;

           
        }

        private void Notify_GetCumulateAward(NetMsg msg)
        {
            CmdArenaGetCumulateAwardRes info = NetMsgUtil.Deserialize<CmdArenaGetCumulateAwardRes>(CmdArenaGetCumulateAwardRes.Parser, msg);

            UpdataInfo(info);

            eventEmitter.Trigger(EEvents.GetCumulateAward);
        }

        private void Notify_ShowSeasonReward(NetMsg msg)
        {
            CmdArenaShowBoxNtf info = NetMsgUtil.Deserialize<CmdArenaShowBoxNtf>(CmdArenaShowBoxNtf.Parser, msg);


            IsNewSeason = true;
            
            UIManager.OpenUI(EUIID.UI_Pvp_SingleNewSeason);

            IsNewSeason = false;
            

        }

        private void UpdataInfo(CmdArenaGetCumulateAwardRes info)
        {
            var list = m_InfoRes.WinInfo;

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                if (list[i].WinNum == info.WinNum)
                {
                    list[i].IsGet = true;
                    break;

                }
            }
        }

        //private void Notify_CloseMatchNtf(NetMsg msg)
        //{
        //    CmdArenaCloseMatchPanelNtf info = NetMsgUtil.Deserialize<CmdArenaCloseMatchPanelNtf>(CmdArenaCloseMatchPanelNtf.Parser, msg);

        //    eventEmitter.Trigger(EEvents.LoadCompl);
        //}
    }


    public partial class Sys_Pvp : SystemModuleBase<Sys_Pvp>
    {
        /// <summary>
        /// 申请PVP信息
        /// </summary>
        public void Apply_ArneaInfo()
        {
            CmdArenaInfoReq info = new CmdArenaInfoReq();


            NetClient.Instance.SendMessage((ushort)CmdArena.InfoReq, info);

           // Sys_Hint.Instance.PushContent_Normal("发送请求：CmdArenaInfoReq");
        }

        /// <summary>
        /// 申请获得今日累胜奖励,num 累胜次数 
        /// 
        /// </summary>
        public void Apply_GetCumulateAward(int num)
        {
            uint coutbag = Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal);
            if (coutbag == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10195));
                return;
            }


            CmdArenaGetCumulateAwardReq info = new CmdArenaGetCumulateAwardReq() { WinNum = (uint)num};


            NetClient.Instance.SendMessage((ushort)CmdArena.GetCumulateAwardReq, info);
        }
        /// <summary>
        /// 申请匹配
        /// </summary>
        public void Apply_Mathch()
        {
            ClearCahce();

            //CmdArenaStartMatchReq info = new CmdArenaStartMatchReq();


            //NetClient.Instance.SendMessage((ushort)CmdArena.StartMatchReq, info);

            Sys_Match.Instance.SendStartMatch(EMatchType.MatchTypeArena);
        }

        /// <summary>
        /// 申请取消匹配
        /// </summary>
        public void Apply_CancleMathch()
        {
            //CmdArenaCancelMatchReq info = new CmdArenaCancelMatchReq();


            //NetClient.Instance.SendMessage((ushort)CmdArena.CancelMatchReq, info);

            Sys_Match.Instance.SendCancleMatch();
        }

        /// <summary>
        /// 申请晋级奖励
        /// </summary>
        public void Apply_RiseInRank()
        {
            CmdArenaDanLvUpAwardReq info = new CmdArenaDanLvUpAwardReq();

            NetClient.Instance.SendMessage((ushort)CmdArena.DanLvUpAwardReq, info);
        }

        /// <summary>
        /// 获取晋级奖励
        /// </summary>
        /// <param name="danID"></param>
        public void Apply_GetDanLvUpAward(uint danID)
        {
            if (IsHadGetDanLvUpAward((int)danID))
                return;


            uint coutbag = Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal);
            if (coutbag == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10195));
                return;
            }

            CmdArenaGetDanLvUpAwardReq info = new CmdArenaGetDanLvUpAwardReq() { DanLv = danID };

            NetClient.Instance.SendMessage((ushort)CmdArena.GetDanLvUpAwardReq, info);
        }


        public void Apply_GetBoxAward()
        {
            CmdArenaGetBoxAward info = new CmdArenaGetBoxAward();
            NetClient.Instance.SendMessage((ushort)CmdArena.GetBoxAward, info);
        }
        private bool IsHadGetDanLvUpAward(int danID)
        {
            bool result = false;

            int count = DanLvUpAwardList.Count;

            for (int i = 0; i < count; i++)
            {
                if (DanLvUpAwardList[i].DanLv == danID)
                    return DanLvUpAwardList[i].IsGet;
            }

            return result;
        }

        /// <summary>
        /// 一键获取晋级奖励
        /// </summary>
        public void Apply_GetAllDanUpAward()
        {

            if (IsHaveDanLvUpAward() == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10196));
                return;
            }

            uint coutbag = Sys_Bag.Instance.GetBagEmptyGrid(BoxIDEnum.BoxIdNormal);
            if (coutbag == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10195));
                return;
            }


            CmdArenaGetAllDanUpAwardReq info = new CmdArenaGetAllDanUpAwardReq();

            NetClient.Instance.SendMessage((ushort)CmdArena.GetAllDanUpAwardReq, info);
        }


        /// <summary>
        /// 获取排行榜列表
        /// </summary>
        /// <param name="type">赛区</param>
        public void Apply_GetRankList(int type)
        {

            uint grouptype = (uint)type;

            var nowTime = Sys_Time.Instance.GetServerTime();

            if (m_DicRankAppling.TryGetValue(grouptype, out uint lasttime))
            {
                if (nowTime >= lasttime && nowTime - lasttime < 3)
                {
                    return;
                }

                m_DicRankAppling.Remove(grouptype);
            }

            if (m_DicRank.TryGetValue(grouptype, out CmdRankQueryRes data))
            {
              
                if (nowTime < data.NextReqTime )
                {
                    RankInfo = data;
                    eventEmitter.Trigger(EEvents.RankList);
                    return;
                }
            }

            m_DicRankAppling.Add(grouptype, nowTime);
            
            CmdRankQueryReq info = new CmdRankQueryReq();
            info.Type = (uint)RankType.Arena;
            info.Notmain = true;
            info.SubType = (uint)RankTypeArena.Kf;
            info.GroupType = (uint)type;
            NetClient.Instance.SendMessage((ushort)CmdRank.QueryReq, info);
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        private void Apply_LoadOK()
        {
            //CmdArenaMapLoadOkReq info = new CmdArenaMapLoadOkReq();

            //NetClient.Instance.SendMessage((ushort)CmdArena.MapLoadOkReq, info);
        }
    }
    #endregion

    #region 配置
    public partial class Sys_Pvp : SystemModuleBase<Sys_Pvp>
    {
        public string LevelIcon { get; private set; } = string.Empty;

        public uint LevelIconID { get; private set; } = 0;
        public uint LevelString { get; private set; } = 0;

        private List<string> m_ServerNameList = new List<string>();
        public void UpdatePvpLevelIcon()
        {  
            CSVArenaSegmentInformation.Data item = CSVArenaSegmentInformation.Instance.GetConfData((uint)LevelID);

            LevelString = item == null ? 0 : item.RankDisplay;
            LevelIcon = item == null ? string.Empty : item.RankIcon;
            LevelIconID = item == null ? 0 : item.RankIcon1;
        }

        public string GetCurLevelServerName()
        {
            int stage = Sys_Pvp.Instance.GetLevelStage();

            if (m_ServerNameList.Count == 0)
            {
                var values = Sys_Pvp.Instance.LevelStageValue;

                int count = values.Length - 1;

                for (int i = 0; i < count; i++)
                {
                    var data = CSVArenaWinAward.Instance.GetConfData((uint)(i + 1));

                    string name = LanguageHelper.GetTextContent(data.CourtId);
                    m_ServerNameList.Add(name);
                }
            }

            if (stage - 1 >= m_ServerNameList.Count)
                return string.Empty;

            return m_ServerNameList[stage - 1];

        }
    }
    #endregion


    class PvpStageLvHelper
    {
         static bool IsOpenLevel_80 = false;

        public static bool IsOpenLevel80 { get { return IsOpenLevel_80; } }

        static List<int> levelstage = null;
        static public List<int> GetLevelStage()
        {
            if (levelstage == null)
            {
                var item = CSVParam.Instance.GetConfData(701);

                var otheritem = CSVParam.Instance.GetConfData(1576);

                string[] values = null;

               // if (string.Equals(item.str_value, otheritem.str_value))
              //  {
                    IsOpenLevel_80 = true;
                    values = item.str_value.Split('|');
               // }
                //else
                //{
                //    values = otheritem.str_value.Split('|');
                //}


                int length = values.Length;

                levelstage = new List<int>();

                for (int i = 0; i < length; i++)
                {
                    int result = 0;
                    if (int.TryParse(values[i], out result))
                    {
                        levelstage.Add(result);
                    }
                }
            }

            return levelstage;
        }
    }
}
