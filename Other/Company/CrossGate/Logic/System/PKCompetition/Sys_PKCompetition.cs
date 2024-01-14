using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using Net;
using Packet;
using System.Collections.Generic;
using System;
using Framework;

namespace Logic
{
    public class Sys_PKCompetition : SystemModuleBase<Sys_PKCompetition>
    {
        //事件列表
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public readonly int OnePageDatasNum = 10;
        public readonly int MaxMemNum = 6;
        public readonly int MinMemNum = 5;
        public enum EEvents : int
        {
            Event_UpdateView,//更新切换页面
            Event_SignSuccess,//报名成功更新
            Event_SearchSuccess,//搜索
            Event_TimeOver, //活动时间到，关闭界面
        }
        public enum MemberState
        {
            None,
            Enum_Checking, //审核中
            Enum_Refused, //已拒绝
            Enum_Passed, //已通过
        }
        #region 数据定义
        public uint MatchID = 1;
        private CSVPKMatch.Data csvPKMatchData;

        public uint SignUpSucessCount;//报名成功队伍数量
        public uint JoinTeamID;//已加入队伍的ID
        public string TeamName;//已加入队伍的名字

        private ulong LeaderID; //战队队长ID 
        private uint MyTeamNum; //战队人数
        private bool IsSignSuccess; //战队是否报名成功

        private uint totlaPages; //战队列表总页数
        public uint MaxPage
        {
            get
            {
                return totlaPages;
            }
        }

        public List<TeamInfo> _teamInfoList = new List<TeamInfo>();

        public Dictionary<ulong, MemberInfo> _memberInfoDic = new Dictionary<ulong, MemberInfo>();
        public List<ulong> _memberInfoList = new List<ulong>();

        public List<TeamInfo> SearchData = new List<TeamInfo>();

        public class TeamInfo
        {
            public SimplePKTeam team;
            public string pkAdvance;
            public uint pageNum;
        }

        public class MemberInfo
        {
            public PKTeamRole role;
            public MemberState state;
        }
        #endregion

        #region 系统函数
        public override void Init()
        {
            ProcessEvents(true);
        }

        public override void Dispose()
        {
            ProcessEvents(false);
        }

        public override void OnLogin()
        {
            var csv = CSVParam.Instance.GetConfData(1531);
            if (csv != null)
                MatchID = uint.Parse(csv.str_value);
            csvPKMatchData = CSVPKMatch.Instance.GetConfData(MatchID);
        }

        public override void OnLogout()
        {
            ClearData();
        }

        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.SignUpReq, (ushort)PKCompete.SignUpRes, SignUpRes, PKCompeteSignUpRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.TeamListReq, (ushort)PKCompete.TeamListRes, TeamListRes, PKCompeteTeamListRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.ApplyReq, (ushort)PKCompete.ApplyRes, ApplyRes, PKCompeteApplyRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.CancelApplyReq, (ushort)PKCompete.CancelApplyRes, CancelApplyRes, PKCompeteCancelApplyRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.HandleApplyReq, (ushort)PKCompete.HandleApplyRes, HandleApplyRes, PKCompeteHandleApplyRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.InfoReq, (ushort)PKCompete.InfoRes, InfoRes, PKCompeteInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)PKCompete.SearchReq, (ushort)PKCompete.SearchRes, SearchRes, PKCompeteSearchRes.Parser);

            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.SignUpRes, SignUpRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.TeamListRes, TeamListRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.ApplyRes, ApplyRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.CancelApplyRes, CancelApplyRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.HandleApplyRes, HandleApplyRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.InfoRes, InfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)PKCompete.SearchRes, SearchRes);
            }

            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, toRegister);
        }

        #endregion
        #region Ntf
        /// <summary>
        /// 报名返回
        /// </summary>
        private void SignUpRes(NetMsg msg)
        {
            PKCompeteSignUpRes ntf = NetMsgUtil.Deserialize<PKCompeteSignUpRes>(PKCompeteSignUpRes.Parser, msg);
            uint result = ntf.Ret; //0 成功 1 报名队伍数已满 2 人数不够
            IsSignSuccess = result == 0;
            SetErrorPKCompeteData(ntf.Ret);
            if (IsSignSuccess)
            {
                UpdateSignSuccessData();
                eventEmitter.Trigger(EEvents.Event_SignSuccess);
            }
        }
        /// <summary>
        /// 战队列表返回
        /// </summary>
        /// <param name="msg"></param>
        private void TeamListRes(NetMsg msg)
        {
            PKCompeteTeamListRes ntf = NetMsgUtil.Deserialize<PKCompeteTeamListRes>(PKCompeteTeamListRes.Parser, msg);
            totlaPages = ntf.TotalPages;
            //SetTeamData(ntf.Teams);

            //int index = (int)ntf.PageNum * OnePageDatasNum;
            InsertTeamData(ntf.PageNum, ntf.Teams);           
            eventEmitter.Trigger(EEvents.Event_UpdateView);
            //刷新team列表
        }
        /// <summary>
        /// 申请入队返回
        /// </summary>
        /// <param name="msg"></param>
        private void ApplyRes(NetMsg msg)
        {
            PKCompeteApplyRes ntf = NetMsgUtil.Deserialize<PKCompeteApplyRes>(PKCompeteApplyRes.Parser, msg);
            uint result = ntf.Ret; //0 成功 1 已经申请过别的队伍
            SetMemData(ntf.Myteam);
            if (result == 0)
                eventEmitter.Trigger(EEvents.Event_UpdateView);
            else
                SetErrorPKCompeteData(result);
        }

        /// <summary>
        /// 取消申请返回
        /// </summary>
        /// <param name="msg"></param>
        private void CancelApplyRes(NetMsg msg)
        {
            PKCompeteCancelApplyRes ntf = NetMsgUtil.Deserialize<PKCompeteCancelApplyRes>(PKCompeteCancelApplyRes.Parser, msg);
            JoinTeamID = 0;
            Req_Info();
        }
        /// <summary>
        /// 队长操作返回
        /// </summary>
        /// <param name="msg"></param>
        private void HandleApplyRes(NetMsg msg)
        {
            PKCompeteHandleApplyRes ntf = NetMsgUtil.Deserialize<PKCompeteHandleApplyRes>(PKCompeteHandleApplyRes.Parser, msg);
            if (_memberInfoDic.ContainsKey(ntf.ApplyRoleId))
            {
                if (ntf.Ret == 0)
                    Req_Info();
                //    _memberInfoDic[ntf.ApplyRoleId].state = MemberState.Enum_Passed;
                //else
                //    _memberInfoDic[ntf.ApplyRoleId].state = MemberState.Enum_Refused;
                //eventEmitter.Trigger(EEvents.Event_UpdateView);
            }
        }

        /// <summary>
        /// 打开界面队伍信息返回
        /// </summary>
        /// <param name="msg"></param>
        private void InfoRes(NetMsg msg)
        {
            DebugUtil.Log(ELogType.eNone, "PK:InfoRes");
            PKCompeteInfoRes ntf = NetMsgUtil.Deserialize<PKCompeteInfoRes>(PKCompeteInfoRes.Parser, msg);
            SignUpSucessCount = ntf.Signupnums;
            ClearMemData();
            if (ntf.Myteam != null)                //已有战队
            {
                if (UIManager.IsOpen(EUIID.UI_PKCompetitionCreate))
                    UIManager.CloseUI(EUIID.UI_PKCompetitionCreate);
                DebugUtil.Log(ELogType.eNone, "PK:Myteam");
                SetMemData(ntf.Myteam);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "PK:Teams");
                totlaPages = ntf.TotalPages;
                uint pageNum = ntf.PageNum;
                SetTeamData(ntf.Teams, true);
            }
            eventEmitter.Trigger(EEvents.Event_UpdateView);
        }

        private void SearchRes(NetMsg msg)
        {
            PKCompeteSearchRes ntf = NetMsgUtil.Deserialize<PKCompeteSearchRes>(PKCompeteSearchRes.Parser, msg);
            ClearSearchData();
            if (ntf.Team != null)
            {
                TeamInfo info = PoolManager.Fetch<TeamInfo>();
                info.team = ntf.Team;
                SearchData.Add(info);
                eventEmitter.Trigger(EEvents.Event_SearchSuccess);
            }
            else if (ntf.Team == null)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001016));
        }
        #endregion
        #region Req
        /// <summary>
        /// 确认报名
        /// </summary>
        public void Req_SignUp()
        {
            if (!IsFightTeamLeader()|| !CheckIsOpen(true))
                return;
            PKCompeteSignUpReq req = new PKCompeteSignUpReq();
            NetClient.Instance.SendMessage((ushort)PKCompete.SignUpReq, req);
        }
        /// <summary>
        /// 请求战队列表 分页
        /// </summary>
        /// <param name="pageNum"></param>
        public void Req_TeamList(uint pageNum)
        {
            if (pageNum >= totlaPages)
                return;
            PKCompeteTeamListReq req = new PKCompeteTeamListReq();
            req.PageNum = pageNum;
            NetClient.Instance.SendMessage((ushort)PKCompete.TeamListReq, req);
            DebugUtil.LogFormat(ELogType.eNone, "PK:Req_TeamList:{0}", pageNum);
        }

        /// <summary>
        /// 请求进入队伍
        /// </summary>
        /// <param name="teamid"></param>
        public void Req_ApplyJoinTeam(uint teamid)
        {
            if (!CheckIsOpen(true))
                return;
            PKCompeteApplyReq req = new PKCompeteApplyReq();
            req.TeamId = teamid;
            NetClient.Instance.SendMessage((ushort)PKCompete.ApplyReq, req);
        }
        /// <summary>
        /// 创建战队
        /// </summary>
        /// <param name="teamName"></param>
        /// <param name="teamAnnounce"></param>
        public void Req_CreateFightTeam(string teamName,string teamAnnounce)
        {
            if (!CheckIsOpen(true))
                return;
            PKCompeteCreateTeamReq req = new PKCompeteCreateTeamReq();
            req.TeamName = FrameworkTool.ConvertToGoogleByteString(teamName);
            req.TeamAnnounce = FrameworkTool.ConvertToGoogleByteString(teamAnnounce);
            NetClient.Instance.SendMessage((ushort)PKCompete.CreateTeamReq, req);
        }

        /// <summary>
        /// 取消申请
        /// </summary>
        public void Req_CancelApply()
        {
            if (!CheckIsOpen(true))
                return;
            PKCompeteCancelApplyReq req = new PKCompeteCancelApplyReq();
            NetClient.Instance.SendMessage((ushort)PKCompete.CancelApplyReq, req);
        }
        /// <summary>
        /// 队长操作申请
        /// </summary>
        public void Req_HandleApply(ulong roleiid,bool isAgree=true)
        {
            if (!IsFightTeamLeader()|| !CheckIsOpen(true))
                return;
            PKCompeteHandleApplyReq req = new PKCompeteHandleApplyReq();
            req.ApplyRoleId = roleiid;
            req.Agree = isAgree;
            NetClient.Instance.SendMessage((ushort)PKCompete.HandleApplyReq, req);
        }
        /// <summary>
        /// 打开界面队伍信息
        /// </summary>
        public void Req_Info()
        {
            PKCompeteInfoReq req = new PKCompeteInfoReq();
            req.RoleId = Sys_Role.Instance.RoleId;
            req.Level = Sys_Role.Instance.Role.Level;
            NetClient.Instance.SendMessage((ushort)PKCompete.InfoReq, req);
            DebugUtil.Log(ELogType.eNone, "PK:Req_Info");
        }

        /// <summary>
        /// 解散战队
        /// </summary>
        public void Req_FightDissolve()
        {
            if (!IsFightTeamLeader()|| !CheckIsOpen(true))
                return;
            PKCompeteDisbandReq req = new PKCompeteDisbandReq();
            NetClient.Instance.SendMessage((ushort)PKCompete.DisbandReq, req);
            DebugUtil.Log(ELogType.eNone, "PK:Req_FightDissolve");
        }

        /// <summary>
        /// 搜索战队
        /// </summary>
        /// <param name="teamid"></param>
        public void Req_Search(uint teamid)
        {
            PKCompeteSearchReq req = new PKCompeteSearchReq();
            req.TeamId = teamid;
            NetClient.Instance.SendMessage((ushort)PKCompete.SearchReq, req);
            DebugUtil.Log(ELogType.eNone, "PK:Req_Search");
        }

        #endregion
        #region SetData

        private void SetTeamData(Google.Protobuf.Collections.RepeatedField<SimplePKTeam> teams,bool isClearData=false)
        {
            if(isClearData)
                ClearTeamData();
            if (teams != null)
            {
                for (int i = 0; i < teams.Count; i++)
                {
                    TeamInfo info = PoolManager.Fetch<TeamInfo>();
                    if (teams[i] == null)
                        continue;
                    info.team = teams[i];
                    _teamInfoList.Add(info);
                }
            }
        }

        private void InsertTeamData(uint pageNum, Google.Protobuf.Collections.RepeatedField<SimplePKTeam> teams)
        {
            int index = (int)pageNum * OnePageDatasNum;
            if (index >= _teamInfoList.Count)
            {
                SetTeamData(teams);
            }
            else
            {
                if (pageNum >= totlaPages - 1)
                {
                    int count = _teamInfoList.Count % OnePageDatasNum;
                    if (count < OnePageDatasNum)
                        _teamInfoList.RemoveRange(index, count);
                    else
                        _teamInfoList.RemoveRange(index, OnePageDatasNum);
                }
                else
                    _teamInfoList.RemoveRange(index, OnePageDatasNum);
                int tempIndex = 0;
                for (int i = 0; i < teams.Count; i++)
                {
                    TeamInfo info = PoolManager.Fetch<TeamInfo>();
                    if (teams[i] == null)
                        continue;
                    info.team = teams[i];
                    _teamInfoList.Insert(index + tempIndex, info);
                    tempIndex++;
                }
            }            
        }

        private void SetMemData(PKTeam Myteam, bool isSignSuccess = false)
        {
            if (Myteam == null)
                return;
            JoinTeamID = Myteam.TeamId;
            TeamName = Myteam.TeamName.ToStringUtf8();
            IsSignSuccess = Myteam.HasSignUp;
            if (Myteam.Mems != null)
            {
                for (int i = 0; i < Myteam.Mems.Count; i++)
                {
                    MemberInfo info = PoolManager.Fetch<MemberInfo>();
                    if (Myteam.Mems[i] == null)
                        continue;
                    info.role = Myteam.Mems[i];
                    info.state = MemberState.Enum_Passed;
                    if (_memberInfoDic.ContainsKey(info.role.RoleId))
                    {
                        DebugUtil.LogErrorFormat("Myteam.Mems 存在相同的roleid:{0}", info.role.RoleId);
                        continue;
                    }
                    _memberInfoDic.Add(info.role.RoleId, info);
                    _memberInfoList.Add(info.role.RoleId);
                    LeaderID = i == 0 ? info.role.RoleId : LeaderID;
                }
                MyTeamNum = (uint)Myteam.Mems.Count;
            }
            if (Myteam.ApplyRoles != null)
            {
                for (int i = 0; i < Myteam.ApplyRoles.Count; i++)
                {
                    MemberInfo info = PoolManager.Fetch<MemberInfo>();
                    if (Myteam.ApplyRoles[i] == null)
                        continue;
                    info.role = Myteam.ApplyRoles[i];
                    info.state = MemberState.Enum_Checking;
                    if (_memberInfoDic.ContainsKey(info.role.RoleId))
                    {
                        DebugUtil.LogErrorFormat("Myteam.ApplyRoles 存在相同的roleid:{0}", info.role.RoleId);
                        continue;
                    }
                    if(IsSignSuccess)
                    {
                        DebugUtil.LogErrorFormat("战队报名成功，不再显示申请列表roleid:{0}", info.role.RoleId);
                        continue;
                    }
                    _memberInfoDic.Add(info.role.RoleId, info);
                    _memberInfoList.Add(info.role.RoleId);
                }
            }
        }

        private void ClearMemData()
        {
            JoinTeamID = 0;
            TeamName = "";
            //IsSignSuccess ;
            for (int i = 0; i < _memberInfoList.Count; ++i)
            {
                MemberInfo info = _memberInfoDic[_memberInfoList[i]];
                if (info == null)
                    continue;
                _memberInfoDic.Remove(_memberInfoList[i]);
                PoolManager.Recycle(info);
            }
            _memberInfoDic.Clear();
            _memberInfoList.Clear();
        }
        private void ClearTeamData()
        {
            for (int i = 0; i < _teamInfoList.Count; ++i)
            {
                TeamInfo info = _teamInfoList[i];
                if (info == null)
                    continue;
                PoolManager.Recycle(info);
            }
            _teamInfoList.Clear();
        }
        private void ClearSearchData()
        {
            for (int i = 0; i < SearchData.Count; i++)
            {
                PoolManager.Recycle(SearchData[i]);
            }
            SearchData.Clear();
        }
        private void UpdateSignSuccessData()
        {
            for (int i = 0; i < _memberInfoList.Count; ++i)
            {
                MemberInfo info = _memberInfoDic[_memberInfoList[i]];
                if (info == null)
                    continue;
                if (info.state != MemberState.Enum_Passed)
                {
                    _memberInfoDic.Remove(_memberInfoList[i]);
                    PoolManager.Recycle(info);
                }
            }
            _memberInfoList.Clear();
            foreach (var item in _memberInfoDic.Keys)
            {
                _memberInfoList.Add(item);
            }
        }
        public void ClearData()
        {
            ClearTeamData();
            ClearMemData();
            ClearSearchData();
        }

        private void SetErrorPKCompeteData(uint ret)
        {
            switch ((ErrorPKCompete)ret)
            {
                case ErrorPKCompete.None:
                    break;
                case ErrorPKCompete.Creating:
                    break;
                case ErrorPKCompete.DiamondNotEnough:
                    break;
                case ErrorPKCompete.TeamNone:
                    break;
                case ErrorPKCompete.ApplyFull:
                    break;
                case ErrorPKCompete.NameRepeated:
                    break;
                case ErrorPKCompete.MastersvrConnect:
                    break;
                default:
                    break;
            }
            if (ret != 0)
            {
                uint languageID = 100000u + ret;
                string content = LanguageHelper.GetErrorCodeContent(languageID);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
        }
        #endregion
        #region function
        /// <summary>
        /// 是否为战队队长
        /// </summary>
        /// <returns></returns>
        public bool IsFightTeamLeader()
        {
            return LeaderID == Sys_Role.Instance.RoleId;
        }
        /// <summary>
        /// 战队是否报名成功
        /// </summary>
        /// <returns></returns>
        public bool IsFightTeamSignSuccess()
        {
            return IsSignSuccess;
        }
        /// <summary>
        /// 战队人数
        /// </summary>
        /// <returns></returns>
        public uint GetMemNum()
        {
            return MyTeamNum;
        }

        /// <summary>
        /// 获取成员状态
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public MemberState GetMemState(ulong uid)
        {
            if (_memberInfoDic.ContainsKey(uid))
            {
                return _memberInfoDic[uid].state;
            }
            return MemberState.Enum_Checking;
        }

        /// <summary>
        /// 等级是否达到
        /// </summary>
        /// <returns></returns>
        public int IsGetLevelLimite()
        {
            uint roleLv = Sys_Role.Instance.Role.Level;
            var csvMatch = CSVPKMatch.Instance.GetConfData(MatchID);
            if (csvMatch == null)
                return 0;
            var level= csvMatch?.level;
            for (int i = 0; i < level.Count; i++)
            {
                if (roleLv >= level[i][0] && roleLv <= level[i][1])
                    return i + 1;
            }
            return 0;
        }

        /// <summary>
        /// 显示时间转换
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void ReturnTime(ref DateTime startTime, ref DateTime endTime)
        {
            CSVPKMatch.Data csv = CSVPKMatch.Instance.GetConfData(MatchID);
            if (csv != null)
            {
                startTime = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(csv.startTime));
                endTime = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(csv.endTime));
            }
        }

        /// <summary>
        /// 活动是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckIsOpen(bool isTip = false)
        {
            CSVPKMatch.Data csv = CSVPKMatch.Instance.GetConfData(MatchID);
            uint nowtime = Sys_Time.Instance.GetServerTime();
            if (nowtime >= TimeManager.ConvertFromZeroTimeZone(csv.startTime) && nowtime <= TimeManager.ConvertFromZeroTimeZone(csv.endTime))
                return true;
            if (isTip)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1001015));
            return false;
        }

        /// <summary>
        /// 获取PK按钮或观看PKweb按钮倒计时
        /// </summary>
        public uint GetCurRemainTime(bool isPKWeb = false)
        {
            CSVPKMatch.Data csv = CSVPKMatch.Instance.GetConfData(MatchID);
            var nowTime = Sys_Time.Instance.GetServerTime();
            var endTime = TimeManager.ConvertFromZeroTimeZone(csv.endTime);
            if (isPKWeb)
                endTime = TimeManager.ConvertFromZeroTimeZone(csv.pkOver);
            if (endTime > nowTime)
            {
                return endTime - nowTime;
            }
            return 0;
        }

        public void OpenSureTip(string msg,Action action)
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = msg;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                action?.Invoke();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OpenPKWeb()
        {
            if (csvPKMatchData != null)
                Application.OpenURL(csvPKMatchData.pkURL);
        }

        /// <summary>
        /// 观看PK赛是否开启
        /// </summary>
        /// <returns></returns>
        public bool CheckWatchPKIsOpen()
        {
            if (csvPKMatchData == null)
                return false;
            uint nowtime = Sys_Time.Instance.GetServerTime();
            if (nowtime >= TimeManager.ConvertFromZeroTimeZone(csvPKMatchData.pkStart) && nowtime <= TimeManager.ConvertFromZeroTimeZone(csvPKMatchData.pkOver))
                return true;
            return false;
        }
        #endregion

        private void OnEndBattle(CmdBattleEndNtf obj)
        {
            CSVBattleType.Data cSVBattleTypeTb = CSVBattleType.Instance.GetConfData(obj.BattleTypeId);
            if (cSVBattleTypeTb == null)
                return;
            if (cSVBattleTypeTb.battle_type != 13)
                return;
            int battleResult = Net_Combat.Instance.GetBattleOverResult();
            //不是在观战中
            if (Net_Combat.Instance.IsRealCombat() && battleResult != 0)
            {
                UIManager.OpenUI(EUIID.UI_PvP_Result, false, battleResult);
            }
        }
    }
}