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
using System.Json;

namespace Logic
{
    public partial class Sys_SurvivalPvp : SystemModuleBase<Sys_SurvivalPvp>
    {
        readonly uint PvpMapID0= 1430;
        readonly uint PvpMapID1= 1431;
        readonly uint PvpMapID2= 1432;

        public uint MapID { get { return GetMapID(); } }

        private uint GetMapID()
        {
           var level = Sys_Role.Instance.Role.Level;

            uint mapID = 0;

            if (level >= 40 && level <= 59)
                mapID = PvpMapID0;
            else if (level >= 60 && level <= 79)
                mapID = PvpMapID1;
            else if (level >= 80)
                mapID = PvpMapID2;

            return mapID;
        }


        public int GetPvpLevel()
        {
            int pvplevel = 0;
            var level = Sys_Role.Instance.Role.Level;

            var stages = PvpStageLvHelper.GetLevelStage();
            int stagescount = stages.Count;
            for (int i = 0; i < stagescount; i++)
            {
                if (level >= stages[i] && (i >= stagescount - 1 || (level < stages[i + 1])))
                {
                    pvplevel = i + 1;
                    break;
                }
            }

            //if (level >= 35 && level <= 59)
            //    pvplevel = 1;
            //else if (level >= 60 && level <= 79)
            //    pvplevel = 2;
            //else if (level >= 80)
            //    pvplevel = 3;

            return pvplevel;
        }
        public bool isSurvivalPvpMap(uint mapID)
        {
            if (mapID == PvpMapID0 || mapID == PvpMapID1 || mapID == PvpMapID2)
                return true;

            return false;
        }
        public enum EEvents
        {
            InfoRefresh,
            FightEnd,
            MatchOk,
            MatchCancle,
            MatchPanleOpen,
            MatchPanleClose,
            MatchLoadPanleClose,
            MatchLoadPanleOpen,
            RankInfo,
            MemberScore,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();


        public CmdSurvivalInfoRes Info { get; private set; }

        public CmdSurvivalNotifyFightEnd FightEndInfo { get; private set; }

        public CmdMatchNotifyMatcherInfo MatcherInfo { get; private set; }

        public CmdRankQueryRes RankInfo { get; private set; }

        public CmdSurvivalNotifyTeamMemberScore MemberScore { get; private set; }

        private float RankUpdateTimePoint = 0;
        public bool isMatching { get; private set; } = false;

        public float MatchTimePoint { get; private set; } = 0;

        private Dictionary<uint, CmdRankQueryRes> m_DicRank = new Dictionary<uint, CmdRankQueryRes>();

        //public bool isFristInMap { get; private set; }=false;
        public uint LastInMapTime { get; private set; } = 0;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSurvival.InfoRes, Notify_Info, CmdSurvivalInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSurvival.NotifyFightEnd, Notify_FightEng, CmdSurvivalNotifyFightEnd.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSurvival.NotifyTeamMemberScore, Notify_MemberScore, CmdSurvivalNotifyTeamMemberScore.Parser);

            Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_RankList, true);
            Sys_Rank.Instance.eventEmitter.Handle(Sys_Rank.EEvents.RankNextTimeReset, Notify_RankReset, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelOpen>(Sys_Match.EEvents.MatchPanelOpen, Notify_StartMatch, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchPanelClose, Notify_MatchPanelClose, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherInfo>(Sys_Match.EEvents.MatcherInfo, Notify_MatchResult, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatcherLoadOk>(Sys_Match.EEvents.MatcherLoadOk, Notify_MatcherLoadOK, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelOpen>(Sys_Match.EEvents.LoadPanelOpen, Notify_OpenLoadPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyLoadPanelClose>(Sys_Match.EEvents.LoadPanelClose, Notify_CloseLoadPanel, true);

            Sys_Match.Instance.eventEmitter.Handle<CmdMatchNotifyMatchPanelClose>(Sys_Match.EEvents.MatchCancle, Notify_MatchCancle, true);

            //  Sys_Rank.Instance.eventEmitter.Handle<CmdRankQueryRes>(Sys_Rank.EEvents.RankQueryRes, Notify_Rank, true);
        }

        public override void OnLogin()
        {
            ReadRecordFile();
        }
        public override void OnLogout()
        {
            Info = null;
        }

       
        public string GetMatchTimeString()
        {
            float time = Time.time - MatchTimePoint;

            int min = (int)(time / 60);
            int sec = (int)(time - min * 60);

            string value = GetTimeMinAndSecString(min) + ":" + GetTimeMinAndSecString(sec);

            return value;
        }

        private string GetTimeMinAndSecString(int time)
        {
            string minstr = time <= 0 ? ("00") : (time < 10 ? ("0" + time.ToString()) : time.ToString());

            return minstr;
        }


        public string GetTimeString(float time)
        {
            int min = (int)(time / 60);
            int sec = (int)(time - min * 60);

            string value = GetTimeMinAndSecString(min) + ":" + GetTimeMinAndSecString(sec);

            return value;
        }
        public uint GetScore(ulong roleid)
        {
            if (Info != null && roleid == Sys_Role.Instance.RoleId)
            {
                return Info.Base.Score;
            }

            if (MemberScore == null)
                return 0;

            var value = MemberScore.Members.Find(o => o.RoleId == roleid);

            if (value == null)
                return 0;

            return value.Score;           
        }


        public uint GetDailyTimes()
        {
            if (Info == null)
                return 0;

            return Info.Base.Join;
        }

        public bool isCanOpenMainUI()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();

            if (nowTime >= MatchResultTime && nowTime - MatchResultTime <= 5)
            {
                return false;
            }
           // DebugUtil.LogError("open su pvp  nowtime + " + nowTime + " matchresultime +" + MatchResultTime);
            return true;
        }

        public bool IsGroupPvp()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            if(Info == null || Info.Time.Grouptime > nowtime || Info.Time.Grouptime == 0)
                return false;

            return true;
        }
        class InPvpTime
        {
           public uint time = 0;
        }
        public void SaveRecordFile()
        {
            string name = "SurvivalpvpFT";

            var nowTime = Sys_Time.Instance.GetServerTime();

            InPvpTime time = new InPvpTime();
            time.time = nowTime;

            FileStore.WriteJson(name, time);

        }

        public void ReadRecordFile()
        {
            string name = "SurvivalpvpFT";

            var jsonValue = FileStore.ReadJson(name);

            if (jsonValue == null)
            {
                LastInMapTime = 0;
                return;
            }

            InPvpTime time = new InPvpTime();
            JsonHeler.DeserializeObject(jsonValue, time); 
            
            LastInMapTime = time.time;
        }
    }


    public partial class Sys_SurvivalPvp : SystemModuleBase<Sys_SurvivalPvp>
    {
        private bool isSuvivalMatch(EMatchType type)
        {
            if (type == EMatchType.MatchTypeSurvival || type == EMatchType.MatchTypeSurvivalGp)
                return true;

            return false;
        }
        private void Notify_MatchCancle(CmdMatchNotifyMatchPanelClose msg)
        {
            if (isSuvivalMatch(Sys_Match.Instance.MatchType) == false)
                return;

            isMatching = false;

            MatchTimePoint = 0;

            eventEmitter.Trigger(EEvents.MatchCancle);

           // Debug.LogError("Notify_MatchCancle");
        }
        private void Notify_CloseLoadPanel(CmdMatchNotifyLoadPanelClose obj)
        {
            if (isSuvivalMatch(Sys_Match.Instance.MatchType) == false)
                return;

            eventEmitter.Trigger(EEvents.MatchLoadPanleClose);

           // Debug.LogError("Notify_CloseLoadPanel");
        }

        private void Notify_OpenLoadPanel(CmdMatchNotifyLoadPanelOpen obj)
        {

            if (isSuvivalMatch((EMatchType)obj.Matchtype) == false)
                return;

            eventEmitter.Trigger(EEvents.MatchLoadPanleOpen);

            //Debug.LogError("Notify_OpenLoadPanel");
        }

        private void Notify_MatcherLoadOK(CmdMatchNotifyMatcherLoadOk obj)
        {

            if (isSuvivalMatch(Sys_Match.Instance.MatchType) == false)
                return;

            int totalCount = MatcherInfo.Blue.Players.Count + MatcherInfo.Red.Players.Count;

            if (obj.Oks.Roles.Count >= totalCount)
            {
                eventEmitter.Trigger(EEvents.MatchOk);
            }

           // Debug.LogError("Notify_MatcherLoadOK");
        }

        private uint MatchResultTime = 0;
        private void Notify_MatchResult(CmdMatchNotifyMatcherInfo obj)
        {

            if (isSuvivalMatch(Sys_Match.Instance.MatchType) == false)
                return;
            MatcherInfo = obj;

            eventEmitter.Trigger(EEvents.MatchPanleClose);

            isMatching = false;

            MatchResultTime = Sys_Time.Instance.GetServerTime();

            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event51);

            // Debug.LogError("Notify_MatchResult");
        }

        private void Notify_MatchPanelClose(CmdMatchNotifyMatchPanelClose obj)
        {
            if (isSuvivalMatch(Sys_Match.Instance.MatchType) == false)
                return;

            //MatchResultTime = 0;
            // eventEmitter.Trigger(EEvents.MatchPanleClose);

            // Debug.LogError("Notify_MatchPanelClose");
        }

        private void Notify_StartMatch(CmdMatchNotifyMatchPanelOpen obj)
        {
            if (isSuvivalMatch( (EMatchType)obj.Matchtype) == false)
                return;

           // Debug.LogError("Notify_StartMatch");

            MatchTimePoint = Time.time;

            isMatching = true;

            eventEmitter.Trigger(EEvents.MatchPanleOpen);
        }

        private void Notify_RankList(CmdRankQueryRes obj)
        {
            if (obj.Type != (uint)RankType.Survival)
                return;

            if (m_DicRank.ContainsKey(obj.GroupType) == false)
            {
                m_DicRank.Add(obj.GroupType, obj);
            }
            else
            {
                m_DicRank[obj.GroupType] = obj;
            }

            RankInfo = obj;

            eventEmitter.Trigger(EEvents.RankInfo);
        }


        private void Notify_RankReset()
        {
            foreach (var kvp in m_DicRank)
            {
                kvp.Value.NextReqTime = 0;
            }
        }

        private void Notify_FightEng(NetMsg msg)
        {
            CmdSurvivalNotifyFightEnd info = NetMsgUtil.Deserialize<CmdSurvivalNotifyFightEnd>(CmdSurvivalNotifyFightEnd.Parser, msg);

            FightEndInfo = info;
           

            UIManager.OpenUI(EUIID.UI_SurvivalPvp_Result);

            eventEmitter.Trigger(EEvents.FightEnd);


        }

        public uint GetHightRewardTimes()
        {
            int hadtimes = 0;

            hadtimes = 5 - (FightEndInfo == null ? 0 : (int)FightEndInfo.Join);

            return hadtimes < 0 ? 0 : (uint)hadtimes;
        }
        private void Notify_Info(NetMsg msg)
        {
            CmdSurvivalInfoRes info = NetMsgUtil.Deserialize<CmdSurvivalInfoRes>(CmdSurvivalInfoRes.Parser, msg);

            Info = info;

            var nowtime = Sys_Time.Instance.GetServerTime();

            if (Info.Time.Grouptime > 0 && Info.Time.Grouptime >= LastInMapTime && Info.Time.Grouptime <= nowtime)
            {
                LastInMapTime = nowtime;
                SaveRecordFile();
                Sys_Team.Instance.OpenTips(0, LanguageHelper.GetTextContent(2022444),null,null,null,false);
            }
           
            eventEmitter.Trigger(EEvents.InfoRefresh);
        }


        private void Notify_MemberScore(NetMsg msg)
        {
            CmdSurvivalNotifyTeamMemberScore info = NetMsgUtil.Deserialize<CmdSurvivalNotifyTeamMemberScore>(CmdSurvivalNotifyTeamMemberScore.Parser, msg);

            MemberScore = info;

            eventEmitter.Trigger(EEvents.MemberScore);
        }
        public void SendStartMatch()
        {
            if (Info == null)
                return;

            var nowtime = Sys_Time.Instance.GetServerTime();

            EMatchType matchtype = (Info.Time.Grouptime > nowtime || Info.Time.Grouptime == 0) ? EMatchType.MatchTypeSurvival : EMatchType.MatchTypeSurvivalGp;

            Sys_Match.Instance.SendStartMatch(matchtype);
        }

        public void SendCancleMatch()
        {
            isMatching = false;

            Sys_Match.Instance.SendCancleMatch();
        }


        public void SendInfoReq()
        {

            CmdSurvivalInfoReq info = new CmdSurvivalInfoReq();

            NetClient.Instance.SendMessage((ushort)CmdSurvival.InfoReq, info);   
        }

        public void SendSetMsgFlagReq(bool flag)
        {
            if (Info != null && Info.Flag == flag)
                return;

            Info.Flag = flag;

            CmdSurvivalSetMsgFlagReq info = new CmdSurvivalSetMsgFlagReq();

            info.Flag = flag;
            NetClient.Instance.SendMessage((ushort)CmdSurvival.SetMsgFlagReq, info);
        }


        public void SendRankInfo()
        {
            uint grouptype = (uint)GetPvpLevel();

            var nowTime = Sys_Time.Instance.GetServerTime();

            if (m_DicRank.TryGetValue(grouptype, out CmdRankQueryRes data))
            {
                if (nowTime < data.NextReqTime)
                {
                    RankInfo = data;
                    eventEmitter.Trigger(EEvents.RankInfo);
                    return;
                }
            }

            RankTypeSurvival rankType = (Info.Time.Grouptime > nowTime || Info.Time.Grouptime == 0) ? RankTypeSurvival.Score : RankTypeSurvival.Group;

            CmdRankQueryReq info = new CmdRankQueryReq();
            info.Type = (uint)RankType.Survival;
            info.Notmain = true;
            info.SubType = (uint)rankType;
            info.GroupType = grouptype;
            NetClient.Instance.SendMessage((ushort)CmdRank.QueryReq, info);

           // Sys_Rank.Instance.RankQueryReq((uint)RankType.Survival, (uint)RankTypeSurvival.Score);
        }


        public void SendTeamScore()
        {
            CmdSurvivalTeamMemberScoreReq info = new CmdSurvivalTeamMemberScoreReq();

            NetClient.Instance.SendMessage((ushort)CmdSurvival.TeamMemberScoreReq, info);
        }

        public void SendFinish()
        {
            CmdMatchActiveFinishReq info = new CmdMatchActiveFinishReq();

            NetClient.Instance.SendMessage((ushort)CmdMatch.ActiveFinishReq, info);

            DebugUtil.LogError("生存竞技请求奖励邮件");
        }

        public RankUnitData GetSelfSurvivalData()
        {
            RankUnitData rankUnitData = null;
            if (RankInfo != null && RankInfo.Units != null)
            {
                if (RankInfo.Units.Count > 0)
                {
                    for (int i = 0; i < RankInfo.Units.Count; i++)
                    {
                        if (RankInfo.Type == (uint)RankType.Survival)
                        {
                            if (RankInfo.Units[i].SurvivalData.RoleId == Sys_Role.Instance.RoleId)
                            {
                                rankUnitData = RankInfo.Units[i];
                                break;
                            }
                        }
                    }


                }
            }

            if (rankUnitData == null && Info != null && Info.Base.Rank > 0)
            {
                rankUnitData = new RankUnitData();
                rankUnitData.Rank = Info.Base.Rank;
                rankUnitData.Score = Info.Base.Score;
                rankUnitData.SurvivalData = new RankDataSurvival();
                rankUnitData.SurvivalData.Name = Sys_Role.Instance.Role.Name;
                rankUnitData.SurvivalData.Career = Sys_Role.Instance.Role.Career;
                rankUnitData.SurvivalData.ServerId = Sys_Login.Instance.RealServerID;
            }

            return rankUnitData;
        }
    }


}
