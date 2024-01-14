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
    //天梯竞技场
    public partial class Sys_LadderPvp : SystemModuleBase<Sys_LadderPvp>,ISystemModuleUpdate
    {
        static uint PvpMapID = 1433;//1401;
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
            GetTaskAward,//获得任务奖励
            RankList,//排行榜
            MySelfLoadOk,
            GetCumulateAward,//累胜奖励
            ShowSeasonAward,
            TeamMemberInfo,

        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init()
        {
            
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.InfoRes, Notify_ArneaInfo, CmdTTDanLvInfoRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.FightEndNtf, Notify_FightEnd, CmdTTDanLvFightEndNtf.Parser);
 
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.ShowBoxNtf, Notify_ShowSeasonReward, CmdTTDanLvShowBoxNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.BoxAwardRes, Notify_ShowBoxAwardRes, CmdTTDanLvBoxAwardRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.TeamMemberInfoRes, Notify_TeamMembersInfo, CmdTTDanLvTeamMemberInfoRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.FastGetTaskAwardRes, Notify_FastGetTaskAward, CmdTTDanLvFastGetTaskAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.GetTaskAwardRes, Notify_GetTaskAward, CmdTTDanLvGetTaskAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTTDanLv.LeaderOpenMainPanelRes, Notify_LeaderOpenMainPanelRes, CmdTTDanLvLeaderOpenMainPanelRes.Parser);

            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_RankList, true);

            Sys_Rank.Instance.eventEmitter.Handle(Sys_Rank.EEvents.RankNextTimeReset, Notify_RankReset, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelOpen>(Sys_Match.EEvents.MatchPanelOpen, Notify_StartMatch, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchPanelClose, Notify_CloseMatcherPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchCancle, Notify_CancleMatch, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherInfo>(Sys_Match.EEvents.MatcherInfo, Notify_MatchResult, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherLoadOk>(Sys_Match.EEvents.MatcherLoadOk, Notify_MatcherLoadOK, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelOpen>(Sys_Match.EEvents.LoadPanelOpen, Notify_OpenLoadPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelClose>(Sys_Match.EEvents.LoadPanelClose, Notify_CloseLoadPanel, true);

            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.InitFinish, OnFuncInitFinish, true);

            ParseLevelStage();
        }

        private void OnFuncInitFinish()
        {
            Apply_Info();
        }
        public override void Dispose()
        {
            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_RankList, false);
            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.InitFinish, OnFuncInitFinish, false);
        }
        public override void OnLogin()
        {
            LoginTime = Time.time;       
        }

        public override void OnLogout()
        {
            m_InfoRes = new CmdTTDanLvInfoRes();
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

                    UIManager.OpenUI(EUIID.UI_LadderPvp_FightResult);
                }
                  
            }

            //if (m_bWaitReMatch)
            //{
            //    if (Sys_Map.Instance.CurMapId != PvpMapID && GameCenter.nLoadStage == 3)
            //    {
            //        if (m_ReMatchMsgID != 0)
            //        {
            //            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(m_ReMatchMsgID));
            //            m_ReMatchMsgID = 0;
            //        }
            //        m_bWaitReMatch = false;

            //        Apply_Mathch();

            //        UIManager.OpenUI(EUIID.UI_Pvp_SingleMatch);
            //    }
            //}
        }
    }

    #region single pvp

    /// <summary>
    /// 数据
    /// </summary>
    public partial class Sys_LadderPvp : SystemModuleBase<Sys_LadderPvp>
    {
        public class PvpItem
        {
            public ulong roleID { get; set; }

            public bool LoadOK { get; set; } = false;

            public FightPlayer Info { get; set; }

            public bool IsPlayerSelf { get { return Sys_Role.Instance.RoleId == roleID; } }

            public uint TeamNum;
        }

        public class PvpTeam
        {
            public List<PvpItem> Items = new List<PvpItem>(5);
            public uint TeamNum;
        }


        public List<PvpTeam> PvpTeamList = new List<PvpTeam>();

        private float LoginTime;
        public bool IsPvpLoading { get; private set; } = false;

        public float LoadingStartTimePoint { get; private set; }
        /// <summary>
        /// 排行榜信息
        /// </summary>
        public CmdRankQueryRes RankInfo { get; private set; }

        /// <summary>
        /// 赛季信息
        /// </summary>
        private CmdTTDanLvInfoRes m_InfoRes = new CmdTTDanLvInfoRes();
        public CmdTTDanLvInfoRes MyInfoRes
        {
            get { return m_InfoRes; }
        }
        /// <summary>
        /// 第几赛季
        /// </summary>
        public uint SeasonNum { get { return m_InfoRes.Season.SeasonId; } }

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


        public uint PvpEndTimesampe { get { return m_InfoRes.Season.EndTime; } }
        public uint PvpStartTimesampe { get { return m_InfoRes.Season.StartTime; } }


        public uint ActivityOpenTime { get {

                if (MyInfoRes == null || MyInfoRes.DayDanLv == null)
                    return 0;

                return MyInfoRes.DayDanLv.StartTime;
            
            } }
        public uint ActivityCloseTime { get {

                if (MyInfoRes == null || MyInfoRes.DayDanLv == null)
                    return 0;

                return MyInfoRes.DayDanLv.EndTime;
            } }

        /// <summary>
        /// 段位ID
        /// </summary>
        public uint LevelID { get; private set; } = 0;
        public uint LastLevelID{get;private set;}

        public uint NextLevelID { get; private set; } = 0;
        public bool IsLevelEffect { get; set; } = false;

        public bool IsStarEffect { get; set; } = false;

        /// <summary>
        /// 排名
        /// </summary>
        public uint MineRank { get { return m_InfoRes.RoleInfo.Base.Rank; } }


        public int SeasonWeek { get; private set; }


        /// <summary>
        /// 匹配结果
        /// </summary>
        public CmdMatchNotifyMatcherInfo MatchDataNtf { get; set; }

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

        public bool IsNewSeason { get;  set; } = false;
        /// <summary>
        /// pvp结果
        /// </summary>
        public CmdTTDanLvFightEndNtf FinghtEndInfo { get;  set; }


        public CmdTTDanLvTeamMemberInfoRes TeamMembersInfo { get; private set; } = null;
        private bool m_bWaitResult { get; set; } = false;

        private bool m_bWaitReMatch = false;
        private uint m_ReMatchMsgID = 0;

        private Dictionary<uint, CmdRankQueryRes> m_DicRank = new Dictionary<uint, CmdRankQueryRes>();
        private Dictionary<uint, uint> m_DicRankAppling = new Dictionary<uint, uint>();

        public CmdTTDanLvBoxAwardRes SewasonAwardInfo { get; private set; } = null;

        /// <summary>
        /// 0 为休闲 1 段位
        /// </summary>
        public uint PvpType { get; set; } = 1;

        private int CacleSeasonWeek()
        {
            var serverTime = TimeManager.GetServerTime();

            var serverDate = TimeManager.GetDateTime(serverTime);

            return (serverDate - PvpStarTime).Days / 7 + 1;
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


        private void ResetPvpItemDic()
        {
            m_PvpItemDic.Clear();

            m_PvpItemDic.Add(Sys_Role.Instance.RoleId, new PvpItem() { roleID = Sys_Role.Instance.RoleId });
        }
        private void AddPvpItem(CmdMatchNotifyMatcherInfo info)
        {
            int count = info.Red.Players.Count;

            PvpTeamList.Clear();

            PvpTeamList.Add(new PvpTeam() { TeamNum = 0 });
            for (int i = 0; i < count; i++)
            {
                var value = info.Red.Players[i];

                if (m_PvpItemDic.ContainsKey(value.RoleId) == false)
                {
                    var item = new PvpItem() { roleID = value.RoleId, Info = value, TeamNum = 0 };

                    m_PvpItemDic.Add(value.RoleId, item);

                    PvpTeamList[0].Items.Add(item);

                }
                else
                {
                    m_PvpItemDic[value.RoleId].Info = value;
                }
                   
            }

            count = info.Blue.Players.Count;
            PvpTeamList.Add(new PvpTeam() { TeamNum = 1 });
            for (int i = 0; i < count; i++)
            {
                var value = info.Blue.Players[i];

                if (m_PvpItemDic.ContainsKey(value.RoleId) == false)
                {
                    var item = new PvpItem() { roleID = value.RoleId, Info = value, TeamNum = 1 };

                    m_PvpItemDic.Add(value.RoleId, item);

                    PvpTeamList[1].Items.Add(item);

                }
                else
                {
                    m_PvpItemDic[value.RoleId].Info = value;
                }
                    
            }


        }

        public uint GetDanLvIDByScore(uint score)
        {
            int count = CSVTianTiSegmentInformation.Instance.Count;

            uint result = 0;
            for (int i = 1; i < count; i++)
            {
                var lastdata = CSVTianTiSegmentInformation.Instance.GetByIndex(i-1);
                var data = CSVTianTiSegmentInformation.Instance.GetByIndex(i);

                if (score >= lastdata.Score && data.Score > score)
                {
                    result = lastdata.id;
                    break;
                }
            }

            if (result == 0)
            {
                var maxdata = CSVTianTiSegmentInformation.Instance.GetByIndex(count - 1);

                if (maxdata.Score <= score)
                {
                    result = maxdata.id;
                }
            }

            return result;
        }

        public uint GetNextDanLvIDByScore(uint score)
        {
            int count = CSVTianTiSegmentInformation.Instance.Count;

            int result = 0;
            for (int i = 0; i < count; i++)
            {
                var data = CSVTianTiSegmentInformation.Instance.GetByIndex(i);

                if (data.Score > score)
                {
                    result = i;
                    break;
                }
            }

            var nextdata = CSVTianTiSegmentInformation.Instance.GetByIndex(result);

            return nextdata == null ? 0 : nextdata.id;
        }
        private void UpdataInfo(CmdTTDanLvInfoRes info)
        {

            uint lastLevelID = LevelID;

            var nowtime = Sys_Time.Instance.GetServerTime();

            m_InfoRes = info;

            LevelID = GetDanLvIDByScore(info.RoleInfo.Base.Score);

            NextLevelID = GetNextDanLvIDByScore(info.RoleInfo.Base.Score);

            m_PvpStarTime = TimeManager.GetDateTime(PvpStartTimesampe+8*3600u);
            m_PvpEndTime = TimeManager.GetDateTime(PvpEndTimesampe+8*3600u);



            var infoData = CSVTianTiSegmentInformation.Instance.GetConfData((uint)LevelID);

            SeasonWeek = PvpStartTimesampe <= nowtime ? CacleSeasonWeek() : 0;

            UpdatePvpLevelIcon();

            InfoRefreshTime = nowtime;

            if (lastLevelID != LevelID && lastLevelID != 0)
            {
                if (!IsLevelEffect)
                    IsLevelEffect = true;
 
                LastLevelID = lastLevelID;
            }

        }

        /// <summary>
        /// 获取赛季剩余时间
        /// </summary>
        /// <returns></returns>
        public uint GetRemainingTime()
        {
            var matchData = CSVMatch.Instance.GetConfData(5);

            var nowtime = Sys_Time.Instance.GetServerTime();

            uint endtime = PvpEndTimesampe;//结束的时间戳
            uint starttime = endtime - matchData.Duration;

            uint time = 0;

            if (nowtime >= starttime && nowtime < endtime)
                time = endtime - nowtime;

            return time;
        }

        public uint GetNextStartTime()
        {
            var matchData = CSVMatch.Instance.GetConfData(5);

            var nowtime = Sys_Time.Instance.GetServerTime();

            uint endtime = PvpEndTimesampe;

            uint starttime =  PvpStartTimesampe;

            uint time = 0;

            var todayZero = Sys_Time.Instance.GetDayZeroTimestamp();

            if (PvpEndTimesampe - (uint)todayZero > 86400)
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
        /// 获得活动的时间
        /// </summary>
        /// <param name="remaintime">未开启时为还有多久开启时间,进行中为结束时间 ，结束 下次开启时间</param>
        /// <returns>0 未开启 1 进行中 2 已结束</returns>
        public uint GetActivityRemainingTime(out uint remaintime)
        {
            var matchData = CSVMatch.Instance.GetConfData(5);

            var nowtime = Sys_Time.Instance.GetServerTime();

            uint endtime = ActivityCloseTime;//结束的时间戳
            uint starttime = endtime - matchData.Duration;

            uint result = 0;
            remaintime = 0;

            if (nowtime < ActivityOpenTime)
            {
                remaintime = ActivityOpenTime - nowtime;
            }
            else if (nowtime >= ActivityOpenTime && nowtime <= ActivityCloseTime)
            {
                remaintime = ActivityCloseTime - nowtime;
                result = 1u;
            }
            else
            {
                remaintime = GetActivityNextTime();
                result = 2u;
            }


            return result;
        }

        public uint GetActivityNextTime()
        {
            var matchData = CSVMatch.Instance.GetConfData(5);
            var date = Sys_Daily.Instance.mTodayDay;

            uint dayofweek =(uint)( date.DayOfWeek == 0 ? date.DayOfWeek + 7 : date.DayOfWeek);

            int count = matchData.GameStar.Count;

            
            uint nowtime = Sys_Time.Instance.GetServerTime();

            uint result = 0;
            bool hadfind = false;

            for (int i = 0; i < count; i++)
            {
                if (matchData.GameStar[i] >= dayofweek)
                {
                    int ocount = matchData.OpeningTime.Count;

                    var dayoffset = matchData.GameStar[i] - dayofweek;

                    if (dayoffset == 0)
                    {
                        for (int oi = 0; oi < ocount; oi++)
                        {
                            uint time = (uint)(matchData.OpeningTime[oi][0] * 3600 + matchData.OpeningTime[oi][1] * 60) + (uint)Sys_Daily.Instance.mTodayZeroTimePoint;

                            if (time > nowtime)
                            {
                                hadfind = true;
                                result = time - nowtime;
                                break;
                            }
                        }
                    }

                    else
                    {
                        uint time = (uint)(matchData.OpeningTime[0][0] * 3600 + matchData.OpeningTime[0][1] * 60) + (uint)Sys_Daily.Instance.mTodayZeroTimePoint;
                        time += (86400 * dayoffset);
                        hadfind = true;
                    }

                    if (hadfind)
                        break;
                }
            }


            if (hadfind == false && count > 0)
            {
                int minday = (int)matchData.GameStar[0];

                int today = (int)dayofweek;

                int hadday = (minday - today) + 7;

                uint nexttime =  (uint)Sys_Daily.Instance.mTodayZeroTimePoint + (uint)hadday * 86400 + (uint)(matchData.OpeningTime[0][0] * 3600 + matchData.OpeningTime[0][1] * 60);
                result = nexttime - nowtime;
            }
            return result;
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


        public uint GetDailyTimes()
        {
            if (MyInfoRes == null || MyInfoRes.RoleInfo == null)
                return 0;

            return MyInfoRes.RoleInfo.Today.Join;
        }

        public bool IsHaveLeveUpReward()
        {
            if (MyInfoRes == null || MyInfoRes.Task1 == null)
                return false;
            int count = MyInfoRes.Task1.Tasks.Count;

            for (int i = 0; i < count; i++)
            {
                if (MyInfoRes.Task1.Tasks[i].Status == (uint)TTDanLvTaskStatus.NotGet)
                    return true;
            }

            return false;
        }

        public bool IsHaveTaskReward()
        {
            if (MyInfoRes == null || MyInfoRes.Task2 == null)
                return false;
            int count = MyInfoRes.Task2.Tasks.Count;

            for (int i = 0; i < count; i++)
            {
                if (MyInfoRes.Task2.Tasks[i].Status == (uint)TTDanLvTaskStatus.NotGet)
                    return true;
            }

            return false;
        }
    }
    public partial class Sys_LadderPvp : SystemModuleBase<Sys_LadderPvp>
    {
        public uint MatchStartTime { get; private set; }
        /// <summary>
        /// 收到PVP信息
        /// </summary>
        /// <param name="msg"></param>
        private void Notify_ArneaInfo(NetMsg msg)
        {
            // Sys_Hint.Instance.PushContent_Normal("收到服务器返回：CmdArenaInfoRes");

            CmdTTDanLvInfoRes info = NetMsgUtil.Deserialize<CmdTTDanLvInfoRes>(CmdTTDanLvInfoRes.Parser, msg);

            UpdataInfo(info);

            eventEmitter.Trigger(EEvents.PvpInfoRefresh);

            float timeoffset =  Time.time - LoginTime;

            if (info.IsBox  && timeoffset > 5)
            {
                IsNewSeason = true;

                UIManager.OpenUI(EUIID.UI_LadderPvp_NewSeason);

                IsNewSeason = false;
            }
           
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
            if (msg.Matchtype != (uint)EMatchType.MatchTypeTtdanLv && msg.Matchtype != (uint)EMatchType.MatchTypeTtleisure)
                return;

            PvpType = msg.Matchtype == (uint)EMatchType.MatchTypeTtdanLv ? 1u : 0u;

            MatchStartTime = Sys_Time.Instance.GetServerTime();

            eventEmitter.Trigger(EEvents.StartMatch);

            UIManager.OpenUI(EUIID.UI_LadderPvp_Match);
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
            if (Sys_Match.Instance.MatchType != EMatchType.MatchTypeTtdanLv && Sys_Match.Instance.MatchType != EMatchType.MatchTypeTtleisure)
                return;

            MatchDataNtf = msg;

            AddPvpItem(msg);

            StartPvpLoading();

            eventEmitter.Trigger(EEvents.MatchSuccess);

            if (UIManager.IsOpen(EUIID.UI_LadderPvp_Match))
                UIManager.CloseUI(EUIID.UI_LadderPvp_Match, true);

            UIManager.OpenUI(EUIID.UI_LadderPvp_Laoding);
        }

        private void Notify_MatcherLoadOK(CmdMatchNotifyMatcherLoadOk msg)
        {
            if (Sys_Match.Instance.MatchType != EMatchType.MatchTypeTtdanLv && Sys_Match.Instance.MatchType != EMatchType.MatchTypeTtleisure)
                return;

            int count = msg.Oks.Roles.Count;

            for (int i = 0; i < count; i++)
            {
                var value = msg.Oks.Roles[i];

                if (m_PvpItemDic.ContainsKey(value) && m_PvpItemDic[value].LoadOK == false)
                {
                    m_PvpItemDic[value].LoadOK = true;              

                }
            }

            eventEmitter.Trigger(EEvents.OneReadyOk);
        }

        private void Notify_CloseLoadPanel(CmdMatchNotifyLoadPanelClose msg)
        {

            eventEmitter.Trigger(EEvents.LoadCompl);
        }

        private void Notify_OpenLoadPanel(CmdMatchNotifyLoadPanelOpen msg)
        {
            if (msg.Matchtype != (uint)EMatchType.MatchTypeTtdanLv && msg.Matchtype != (uint)EMatchType.MatchTypeTtleisure)
                return;

            UIManager.OpenUI(EUIID.UI_LadderPvp_Laoding);
        }


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

        private void Notify_RankList(CmdRankQueryRes msg)
        {
            // CmdRankQueryRes info = NetMsgUtil.Deserialize<CmdRankQueryRes>(CmdRankQueryRes.Parser, msg);
            if (msg.Type != (uint)RankType.TianTi)
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
                        if (RankInfo.Type == (uint)RankType.TianTi)
                        {
                            if (RankInfo.Units[i].TiantiData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                rankUnitData = RankInfo.Units[i];
                                break;
                            }
                        }
                    }
                }
            }

            if (rankUnitData == null && MyInfoRes != null)
            {
                rankUnitData = new RankUnitData();

                rankUnitData.TiantiData = new RankDataTianTi();
                rankUnitData.TiantiData.Score = (int)MyInfoRes.RoleInfo.Base.Score;

                rankUnitData.TiantiData.GlobalRank = MyInfoRes.RoleInfo.Base.Rank;
                rankUnitData.TiantiData.Name = Sys_Role.Instance.Role.Name;
                rankUnitData.TiantiData.WinNum = (int)MyInfoRes.RoleInfo.Total.Win;
                rankUnitData.TiantiData.TotalNum = (int)MyInfoRes.RoleInfo.Total.Join;
                rankUnitData.TiantiData.ServerId = (uint)Sys_Login.Instance.RealServerID;
            }
            return rankUnitData;
        }



        private void Notify_FightEnd(NetMsg msg)
        {

            CmdTTDanLvFightEndNtf info = NetMsgUtil.Deserialize<CmdTTDanLvFightEndNtf>(CmdTTDanLvFightEndNtf.Parser, msg);

            FinghtEndInfo = info;

            m_bWaitResult = true;

            if (MyInfoRes != null)
            {
                MyInfoRes.RoleInfo.Today = info.Today;
                MyInfoRes.RoleInfo.Total = info.Total;
            }
        }


        private void Notify_ShowSeasonReward(NetMsg msg)
        {
            CmdTTDanLvShowBoxNtf info = NetMsgUtil.Deserialize<CmdTTDanLvShowBoxNtf>(CmdTTDanLvShowBoxNtf.Parser, msg);

            IsNewSeason = true;

            UIManager.OpenUI(EUIID.UI_LadderPvp_NewSeason);

            IsNewSeason = false;
            

        }

        private void Notify_ShowBoxAwardRes(NetMsg msg)
        {
            CmdTTDanLvBoxAwardRes info = NetMsgUtil.Deserialize<CmdTTDanLvBoxAwardRes>(CmdTTDanLvBoxAwardRes.Parser, msg);

            SewasonAwardInfo = info;

            eventEmitter.Trigger(EEvents.ShowSeasonAward);
        }
        private void Notify_TeamMembersInfo(NetMsg msg)
        { 
            CmdTTDanLvTeamMemberInfoRes info = NetMsgUtil.Deserialize<CmdTTDanLvTeamMemberInfoRes>(CmdTTDanLvTeamMemberInfoRes.Parser, msg);

            TeamMembersInfo = info;

            eventEmitter.Trigger(EEvents.TeamMemberInfo);
        }


        private void Notify_FastGetTaskAward(NetMsg msg)
        {
            CmdTTDanLvFastGetTaskAwardRes info = NetMsgUtil.Deserialize<CmdTTDanLvFastGetTaskAwardRes>(CmdTTDanLvFastGetTaskAwardRes.Parser, msg);

            int count = info.TaskIds.Count;

            for (int i = 0; i < count; i++)
            {
               var task = MyInfoRes.Task1.Tasks.Find(o => o.TaskId == info.TaskIds[i]);
                if (task != null)
                {
                    task.Status =(uint)TTDanLvTaskStatus.Got;
                }
            }

            eventEmitter.Trigger(EEvents.GetAllDanLvUpAward);
        }

        private void Notify_GetTaskAward(NetMsg msg)
        {
            CmdTTDanLvGetTaskAwardRes info = NetMsgUtil.Deserialize<CmdTTDanLvGetTaskAwardRes>(CmdTTDanLvGetTaskAwardRes.Parser, msg);


            var data = CSVTianTiTask.Instance.GetConfData(info.TaskId);

            if (data == null)
                return;

            var tasks = data.MissionType == 1 ? MyInfoRes.Task1 : MyInfoRes.Task2;

            TTDanLvTask task = null;

             if(tasks != null)
             task =  tasks.Tasks.Find(o => o.TaskId == info.TaskId);

            if (task != null)
            {
                task.Status = (uint)TTDanLvTaskStatus.Got;
            }

            eventEmitter.Trigger(data.MissionType == 1 ? EEvents.GetDanLvUpAward : EEvents.GetTaskAward);
        }

        private void Notify_LeaderOpenMainPanelRes(NetMsg msg)
        {
            CmdTTDanLvLeaderOpenMainPanelRes info = NetMsgUtil.Deserialize<CmdTTDanLvLeaderOpenMainPanelRes>(CmdTTDanLvLeaderOpenMainPanelRes.Parser, msg);

            if (Sys_Team.Instance.HaveTeam == false || Sys_Team.Instance.isCaptain())
                return;

            bool isleave = Sys_Team.Instance.isPlayerLeave();

            if (isleave)
                return;

            if (UIManager.IsOpen(EUIID.UI_LadderPvp_NewSeason))
                return;

            UIManager.OpenUI(EUIID.UI_LadderPvp);
        }
    }


    public partial class Sys_LadderPvp : SystemModuleBase<Sys_LadderPvp>
    {
        uint LastPvpSendTime = 0;
        EMatchType LastPvpSendType  = EMatchType.MatchTypeNone;

        /// <summary>
        /// 申请PVP信息
        /// </summary>
        public void Apply_Info()
        {

            if (Sys_FunctionOpen.Instance.IsOpen(22150) ==false)
            {
                return;
            }

            CmdTTDanLvInfoReq info = new CmdTTDanLvInfoReq();


            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.InfoReq, info);

            if (MyInfoRes == null)
            {
                CmdTTDanLvInfoRes resinfo = new CmdTTDanLvInfoRes();
                resinfo.Season = new TTDanLvSeason();

                resinfo.DayDanLv = new TTDanLvDay();
                resinfo.DayLeisure = new TTDanLvDay();
                resinfo.RoleInfo = new TTDanLvRoleSeason();
                resinfo.RoleInfo.Base = new TTDanLvRoleBase();
                resinfo.RoleInfo.Total = new TTDanLvRoleTotal();
                resinfo.RoleInfo.Today = new TTDanLvRoleDay();

                resinfo.Season.SeasonId = 1u;
                resinfo.Season.StartTime = Sys_Time.Instance.GetServerTime() - 86400u;
                resinfo.Season.EndTime = Sys_Time.Instance.GetServerTime() + 10 * 86400u;

                resinfo.DayDanLv.StartTime = Sys_Time.Instance.GetServerTime() + 3600;
                resinfo.DayDanLv.EndTime = resinfo.DayDanLv.StartTime + 3600;

                UpdataInfo(resinfo);
            }


        }

        /// <summary>
        /// 申请匹配
        /// type 1 休闲匹配 2 段位匹配
        /// </summary>
        public void Apply_Mathch(uint type)
        {
            if (Sys_Team.Instance.isCaptain() == false && Sys_Team.Instance.HaveTeam)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590003102));
                return;
            }
               

            if (Sys_Team.Instance.HaveLeaveOrOfflineMem())
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590003103));
                return;
            }

            var nowtime = Sys_Time.Instance.GetServerTime();
            EMatchType matchtype = type == 1 ? EMatchType.MatchTypeTtleisure : EMatchType.MatchTypeTtdanLv;

            if (matchtype == LastPvpSendType && nowtime - LastPvpSendTime < 2)
                return;

            ClearCahce();

            LastPvpSendTime = nowtime;
            LastPvpSendType = matchtype;

            Sys_Match.Instance.SendStartMatch(matchtype);
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

        public void Apply_GetBoxAward()
        {
            CmdTTDanLvBoxAwardReq info = new CmdTTDanLvBoxAwardReq();
            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.BoxAwardReq, info);
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
            info.Type = (uint)RankType.TianTi;
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

        public void Apply_GetTeamMembersInfo()
        {
            CmdTTDanLvTeamMemberInfoReq info = new CmdTTDanLvTeamMemberInfoReq();

            //CmdTTDanLvTeamMemberInfoRes resinfo = new CmdTTDanLvTeamMemberInfoRes();
           
            //int count = Sys_Team.Instance.TeamMemsCount;

            //for (int i = 0; i < count; i++)
            //{
            //    TTDanLvTeamMember mem = new TTDanLvTeamMember();
            //    mem.RoleId = Sys_Team.Instance.getTeamMem(i).MemId;
            //    mem.Score = 1u;
            //    resinfo.Members.Add(mem);

            //}

            //TeamMembersInfo = resinfo;

            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.TeamMemberInfoReq, info);
        }

        public void Apply_GetTaskAward(uint taskid)
        {
            CmdTTDanLvGetTaskAwardReq info = new CmdTTDanLvGetTaskAwardReq();
            info.TaskId = taskid;
            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.GetTaskAwardReq, info);
        }

        /// <summary>
        /// type 1 晋级 ，其他的是 
        /// </summary>
        /// <param name="type"></param>
        public void Apply_GetAllTaskAward(uint type)
        {
            CmdTTDanLvFastGetTaskAwardReq info = new CmdTTDanLvFastGetTaskAwardReq();
            info.TaskType = type;
            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.FastGetTaskAwardReq, info);
        }

        public void Apply_LeaderOpenMainPanelReq()
        {
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount <= 1)
                return;



            CmdTTDanLvLeaderOpenMainPanelReq info = new CmdTTDanLvLeaderOpenMainPanelReq();

            NetClient.Instance.SendMessage((ushort)CmdTTDanLv.LeaderOpenMainPanelReq, info);
        }
    }
    #endregion

    #region 配置
    public partial class Sys_LadderPvp : SystemModuleBase<Sys_LadderPvp>
    {
        public string LevelIcon { get; private set; } = string.Empty;

        public uint LevelIconID { get; private set; } = 0;
        public uint LevelString { get; private set; } = 0;

        private List<string> m_ServerNameList = new List<string>();
        public void UpdatePvpLevelIcon()
        {
            var item = CSVTianTiSegmentInformation.Instance.GetConfData((uint)LevelID);

            LevelString = item == null ? 0 : item.RankDisplay;
            LevelIcon = item == null ? string.Empty : item.RankIcon;
            LevelIconID = item == null ? 0 : item.RankIcon1;
        }

        public string GetCurLevelServerName()
        {
            int stage = GetLevelStage();

            if (m_ServerNameList.Count == 0)
            {
                var values = LevelStageValue;

                int count = values.Length - 1;

                for (int i = 0; i < count; i++)
                {
                    
                    var data = CSVTianTiWinAward.Instance.GetConfData((uint)(i + 1));

                    string name = LanguageHelper.GetTextContent(data.CourtId);
                    m_ServerNameList.Add(name);
                }
            }

            if (stage - 1 >= m_ServerNameList.Count)
                return string.Empty;

            return m_ServerNameList[stage - 1];

        }

        public string GetDanLvLevelString(uint levleid)
        {
            var item = CSVTianTiSegmentInformation.Instance.GetConfData(levleid);

            if (item == null)
                return string.Empty;

            return LanguageHelper.GetTextContent(item.RankDisplay);
        }
    }
    #endregion
}
