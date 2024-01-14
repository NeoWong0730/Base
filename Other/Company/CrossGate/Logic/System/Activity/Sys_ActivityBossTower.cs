using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    public class BossTowerDailyFunc : DailyFunc
    {
        public override bool OnJoin()
        {
            uint lanId= Sys_ActivityBossTower.Instance.CheckCurBossTowerState();
            if (lanId != 0) return false;
            if (Sys_ActivityBossTower.Instance.bossTowerFeatureGroupDataList.Count <= 1)
            {
                CSVBOSSTower.Data curFeatureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature();
                if (curFeatureData != null)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(curFeatureData.npc_id);
                    UIManager.CloseUI(EUIID.UI_DailyActivites);
                }
            }
            else
                UIManager.OpenUI(EUIID.UI_BossTower_Feature);
            return false;
        }

        public override bool isUnLock()
        {
            bool result = Sys_FunctionOpen.Instance.IsOpen(Data.FunctionOpenid);

            if (!result)
                return false;

            uint state = Sys_ActivityBossTower.Instance.GetBossTowerActivityState();

            if (Data.id == 240 && state == 1)
                return true;

            if (Data.id == 250 && state == 2)
                return true;

            return false;
        }

    }
    public class Sys_ActivityBossTower : SystemModuleBase<Sys_ActivityBossTower>
    {
        #region 系统函数
        public override void Init()
        {
            isLogin = false;
            ProcessEvents(true);
        }
        public override void OnLogin()
        {
            InitData();
        }
        public override void OnLogout()
        {
            SetDefaultData();
        }
        public override void Dispose()
        {
            isLogin = false;
            ProcessEvents(false);
            ClearData();
        }
        #endregion
        #region 数据定义
        public enum EEvents
        {
            OnRefreshBossTowerReset,    //BossTower活动重置刷新
            OnRefreshSelfRankData,      //刷新个人排行数据
            OnRefreshRedPoint,          //刷新红点
            OnDoVote,                   //执行投票       
            OnUpdateVote,               //刷新投票  一般是新加了投票人    
            OnBossTowerStateChange,     //boss资格挑战赛状态改变
            OnRefreshRankList,          //刷新排行列表
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> boss特性 key:等级段id value:boss特性列表  </summary>
        Dictionary<uint, List<CSVBOSSTower.Data>> bossTowerFeatureDic = new Dictionary<uint, List<CSVBOSSTower.Data>>();
        /// <summary> boss特性组 key:轮询order value:boss特性列表  </summary>
        Dictionary<uint, List<CSVBOSSTower.Data>> bossTowerFeatureGroupDic = new Dictionary<uint, List<CSVBOSSTower.Data>>();
        /// <summary> boss特性组列表  </summary>
        public List<BossTowerFeatureGroupData> bossTowerFeatureGroupDataList = new List<BossTowerFeatureGroupData>();
        /// <summary> 资格赛 key:特性id value:对应资格赛列表 </summary>
        Dictionary<uint, List<BossTowerQualifierData>> bossQualifierDataDic = new Dictionary<uint, List<BossTowerQualifierData>>();
        /// <summary> Boss战 key:特性id value:对应boss阶段列表 </summary>
        Dictionary<uint, List<BossTowerStageData>> bossStageDataDic = new Dictionary<uint, List<BossTowerStageData>>();
        /// <summary> Boss战排行奖励 key:等级段id value:奖励列表  </summary>
        Dictionary<uint, List<CSVBOSSTowerRankReward.Data>> bossRankRewardDic = new Dictionary<uint, List<CSVBOSSTowerRankReward.Data>>();
        /// <summary>
        /// 不同等级段对应的个人排行数据(入围人数,基础排名数量)
        /// </summary>
        Dictionary<uint, QualifierRankData> qualifierRankDataDic = new Dictionary<uint, QualifierRankData>();
        /// <summary> 等级段 </summary>
        uint[] levelGrades;
        /// <summary> 等级段区间 key:等级段id value:等级区间 </summary>
        public Dictionary<uint, uint[]> levelGradeRangeDic = new Dictionary<uint, uint[]>();
        /// <summary> 投票ID </summary>
        public ulong VoteID { get; private set; }
        /// <summary> 投票开始时间 </summary>
        public uint VoteStartTime { get; private set; }
        /// <summary> 投票状态 </summary>
        private Dictionary<ulong, int> voteState = new Dictionary<ulong, int>();
        /// <summary> 投票成员数据 </summary>
        public BossTowerVoteData bossTowerVoteData;
        /// <summary> 是否参加过资格赛 </summary>
        public bool bJoinQualifier; 
        /// <summary> 当前已通过资格赛最大层数tid </summary>
        public uint curQualifierMaxLvlTid;
        /// <summary> 当前资格赛排名 </summary>
        public uint curQualifierRank;
        /// <summary> boss战阶段是否有挑战资格 </summary>
        public bool bBossUnlock;
        /// <summary> 当前已通过Boss战最大阶段tid </summary>
        public uint curBossMaxStageTid;
        /// <summary> 当前活动状态 </summary>
        public BossTowerState curBossTowerState;
        /// <summary> 当前活动状态结束时间 </summary>
        public uint curStateEndTime;
        /// <summary> 本周特性id </summary>
        public uint curFeatureId;
        /// <summary> 当前已获得资格人数 </summary>
        public uint curQualifierNum;
        /// <summary> 当前动态资格人数 </summary>
        public uint curDynamicQualifierNum;
        /// <summary> 资格赛基础排名数量(配置值+动态值) </summary>
        public uint baseRankNum;
        /// <summary> 资格赛额外排名名额 </summary>
        public uint extRankNum;
        /// <summary>资格赛最高层回合数</summary>
        public uint minPreRound;
        /// <summary>资格赛最高层回合时间</summary>
        public uint lastPreTime;
        /// <summary>Boss战最高层回合数</summary>
        public uint minBossRound;
        /// <summary>Boss战最高层回合数时间</summary>
        public uint lastBossTime;
        /// <summary> 个人排行数据刷新时间 </summary>
        public uint selfRankRefreshTime;
        /// <summary> 挑战冷却CD </summary>
        float challengeCD = float.Parse(CSVBOSSTowerParameter.Instance.GetConfData(201).str_value);
        /// <summary>资格赛最低通关层数</summary>
        public uint minPassLayer = uint.Parse(CSVBOSSTowerParameter.Instance.GetConfData(2).str_value);
        /// <summary> 挑战冷却计时器 </summary>
        Timer challengeCDTimer;
        /// <summary>挑战CD剩余时间 </summary>
        int challengeCDDiffTime;
        /// <summary> 挑战冷却时间是否结束 </summary>
        bool challengeCDIsOver;

        /// <summary> 单页排行数据量 </summary>
        public readonly int OnePageDatasNum = 50;
        public int MaxPageNum
        { 
            get 
            {
                int maxRankNum = int.Parse(CSVBOSSTowerParameter.Instance.GetConfData(8).str_value);
                return maxRankNum / OnePageDatasNum;
            }
        }
        /// <summary>不同等级段排行数据</summary>
        public Dictionary<bool, Dictionary<uint, List<BossTowerRankUnit>>> bossTowerRankUnitDic = new Dictionary<bool, Dictionary<uint, List<BossTowerRankUnit>>>();
        /// <summary>个人排行数据</summary>
        public BossTowerRankUnit selfRankUnit { get; private set; }

        //奖励展示界面的宽高
        public float rewardViewWidth = 466;
        public float rewardViewHeight = 152;

        bool isLogin;
        #endregion
        #region 事件注册
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBossTower.DataNtf, OnBossTowerDataNtf, CmdBossTowerDataNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdBossTower.SelfRankReq, (ushort)CmdBossTower.SelfRankNtf, OnSelfRankNtf, CmdBossTowerSelfRankNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdBossTower.ChallengeReq, (ushort)CmdBossTower.ChallengeRes, OnChallengeRes, CmdBossTowerChallengeRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdBossTower.RankListReq, (ushort)CmdBossTower.RankListRes, OnRankListRes, CmdBossTowerRankListRes.Parser);
                Sys_Vote.Instance.AddVoteLisitener((ushort)VoteType.BossTower, OnNotify_Vote);
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBossTower.DataNtf, OnBossTowerDataNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBossTower.SelfRankNtf, OnSelfRankNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdBossTower.ChallengeRes, OnChallengeRes);
                Sys_Vote.Instance.RemoveVoteLisitener((ushort)VoteType.BossTower, OnNotify_Vote);
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
            }
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, OnLoadMapOk, toRegister);
            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, OnChangeMap, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, toRegister);
        }
        #region 进战投票
        /// <summary>
        /// 投票通知
        /// </summary>
        /// <param name="eVote"></param>
        /// <param name="message"></param>
        void OnNotify_Vote(Sys_Vote.EVote eVote, object message)
        {
            if (eVote == Sys_Vote.EVote.Start)
                OnStartVoteNtf(message as CmdRoleStartVoteNtf);

            if (eVote == Sys_Vote.EVote.DoVote)
                OnDoVoteNtf(message as CmdRoleDoVoteNtf);

            if (eVote == Sys_Vote.EVote.End)
                OnVoteEndNtf(message as CmdRoleVoteEndNtf);

            if (eVote == Sys_Vote.EVote.Update)
                OnUpdateVoteNtf(message as CmdRoleVoteUpdateNtf);
        }
        /// <summary>
        /// 准备投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnStartVoteNtf(CmdRoleStartVoteNtf info)
        {
            NetMsgUtil.TryDeserialize(BossTowerVoteData.Parser, info.ClientData.ToByteArray(), out BossTowerVoteData data);
            VoteID = info.VoteId;
            bossTowerVoteData = data;
            VoteStartTime = info.StartTime;

            if (data.IsBoss)
                UIManager.CloseUI(EUIID.UI_BossTower_FightMain);
            else
                UIManager.CloseUI(EUIID.UI_BossTower_QualifierMain);

            BossTowerVoteParmas parmas = new BossTowerVoteParmas();
            parmas.isCaptain = Sys_Team.Instance.isCaptain();
            parmas.isBoss = data.IsBoss;
            parmas.stageId = data.StageId;
            parmas.featureId = data.FeatureId;
            parmas.voteState = Sys_Team.Instance.isCaptain() ? 3 : 2;
            UIManager.OpenUI(EUIID.UI_BossTower_EnterFightVote, false, parmas);
        }
        /// <summary>
        /// 队员投票通知
        /// </summary>
        /// <param name="msg"></param>
        void OnDoVoteNtf(CmdRoleDoVoteNtf info)
        {
            if (voteState.ContainsKey(info.RoleId))
                voteState[info.RoleId] = info.Op;
            else
                voteState.Add(info.RoleId, info.Op);
            eventEmitter.Trigger(EEvents.OnDoVote,info.RoleId);
        }
        /// <summary>
        /// 投票信息更新， 一般是新加了投票人
        /// </summary>
        /// <param name="info"></param>
        private void OnUpdateVoteNtf(CmdRoleVoteUpdateNtf info)
        {
            NetMsgUtil.TryDeserialize(BossTowerVoteData.Parser, info.ClientData.ToByteArray(), out BossTowerVoteData data);
            VoteID = info.VoteId;
            bossTowerVoteData = data;
            eventEmitter.Trigger(EEvents.OnUpdateVote);
        }
        /// <summary>
        /// 队员投票结果
        /// </summary>
        /// <param name="msg"></param>
        void OnVoteEndNtf(CmdRoleVoteEndNtf info)
        {
            if (info.ResultType == 2)
            {
                StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                switch ((VoteFailReason)info.FailReason)
                {
                    case VoteFailReason.Disagree:
                        {
                            for (int i = 0; i < info.DisagreeIds.Count; ++i)
                            {
                                if (info.DisagreeIds[i] == Sys_Role.Instance.RoleId)
                                    stringBuilder.Append(Sys_Role.Instance.sRoleName);
                                else
                                {
                                    TeamMem roleInfo = Sys_Team.Instance.getTeamMem(info.DisagreeIds[i]);
                                    if (roleInfo != null)
                                        stringBuilder.Append(roleInfo.Name.ToStringUtf8());
                                }
                                if (i < info.DisagreeIds.Count - 1)
                                    stringBuilder.Append("、");
                            }

                            if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009317, stringBuilder.ToString()));
                        }
                        break;
                    case VoteFailReason.ManualCancel:
                        {
                            if (info.CancelVoterId == Sys_Role.Instance.RoleId)
                                stringBuilder.Append(Sys_Role.Instance.sRoleName);
                            else
                            {
                                TeamMem roleInfo = Sys_Team.Instance.getTeamMem(info.CancelVoterId);
                                if (roleInfo != null)
                                    stringBuilder.Append(roleInfo.Name.ToStringUtf8());
                            }
                            if (!string.IsNullOrEmpty(stringBuilder.ToString()))
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009317, stringBuilder.ToString()));
                        }
                        break;
                    case VoteFailReason.SystemCancel:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105005));
                        }
                        break;
                    default:
                        break;
                }
                StringBuilderPool.ReleaseTemporary(stringBuilder);

            }
            voteState.Clear();
            VoteID = 0;
            UIManager.CloseUI(EUIID.UI_BossTower_EnterFightVote);
        }
        #endregion

        private void OnEnterBattle(CSVBattleType.Data cSVBattleTypeTb)
        {
            //播放录像不计CD
            if (Net_Combat.Instance.m_IsVideo)
                return;
            UIManager.CloseUI(EUIID.UI_BossTower_Result);
            bool isStart = false;
            if (isBossChallenge)
            {
                BossTowerStageData data = GetBossTowerStageData(curReqChallengeTid);
                if (data != null)
                    isStart = Sys_Fight.curMonsterGroupId == data.csvData.battle_id;
            }
            else
            {
                BossTowerQualifierData data = GetBossTowerQualifierData(curReqChallengeTid);
                if (data != null)
                    isStart = Sys_Fight.curMonsterGroupId == data.csvData.battle_id;
            }
            if (isStart)
                SetChallengeCDTime();
        }
        private void OnEndBattle(CmdBattleEndNtf ntf)
        {
            int rlt = Net_Combat.Instance.GetBattleOverResult();
            if (ntf != null && ntf.BossTower != null)
            {
                BattleEndBossTower battleEndBossTower = ntf.BossTower;
                BossTowerResultParam param = new BossTowerResultParam();
                param.isCanShowReward = false;
                param.isBoss = battleEndBossTower.IsBoss;
                param.stageId = battleEndBossTower.StageId;
                param.isCanShowReward = ntf.Rewards != null && ntf.Rewards.Items != null && ntf.Rewards.Items.Count > 0;
                param.battleReward = ntf.Rewards;
                if (battleEndBossTower.IsBoss)
                {
                    //未通过第一阶段 || 达到最后一阶段 即为战败
                    if (!CheckBossTowerShowState(battleEndBossTower.StageId))
                    {
                        UIManager.OpenUI(EUIID.UI_BossTower_Result, false, param);
                    }
                }
                else
                {
                    //胜利
                    if (rlt == 1)
                    {
                        UIManager.OpenUI(EUIID.UI_BossTower_Result, false, param);
                    }
                }
            }
        }
        /// <summary>
        /// 检查boss战结算显示状态
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public bool CheckBossTowerShowState(uint tid)
        {
            return (tid == 0 && curBossMaxStageTid == 0) || tid == GetBossTowerFirstOrLastDataTid(2, false);
        }
        /// <summary>
        /// 设置挑战冷却时间
        /// </summary>
        private void SetChallengeCDTime()
        {
            challengeCDIsOver = false;
            challengeCDTimer?.Cancel();
            challengeCDTimer = Timer.Register(challengeCD, () =>
            {
                challengeCDTimer?.Cancel();
                challengeCDIsOver = true;
            },
            (time) => {
                challengeCDDiffTime = (int)(challengeCD - time);
                challengeCDDiffTime = challengeCDDiffTime <= 0 ? 1 : challengeCDDiffTime;
            }
            , false, true);
        }
        /// <summary>
        /// 重连结果
        /// </summary>
        /// <param name="result"></param>
        private void OnReconnectResult(bool result)
        {
            UIManager.CloseUI(EUIID.UI_BossTower_Feature);
            UIManager.CloseUI(EUIID.UI_BossTower_FightMain);
            UIManager.CloseUI(EUIID.UI_BossTower_Stages);
            UIManager.CloseUI(EUIID.UI_BossTower_QualifierRank);
            UIManager.CloseUI(EUIID.UI_BossTower_BossFightRank);
            UIManager.CloseUI(EUIID.UI_BossTower_Result);
            UIManager.CloseUI(EUIID.UI_BossTower_EnterFightVote);
            UIManager.CloseUI(EUIID.UI_BossTower_QualifierMain);
            UIManager.CloseUI(EUIID.UI_BossTower_QualifierExplain);
        }
        private void OnChangeMap(uint lastMapId, uint curMapId)
        {
            if (bIsOnBossTowerMap)
            {
                CSVBOSSTower.Data featureData = GetBossTowerFeature();
                if (featureData != null && Sys_Map.Instance.CurMapId != featureData.map_id)
                {
                    CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(featureData.map_id);
                    if (mapData != null)
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009306, LanguageHelper.GetTextContent(mapData.name)));//离开地图提示
                    UIManager.CloseUI(EUIID.UI_BossTower_Feature);
                    UIManager.CloseUI(EUIID.UI_BossTower_FightMain);
                    UIManager.CloseUI(EUIID.UI_BossTower_Stages);
                    UIManager.CloseUI(EUIID.UI_BossTower_QualifierRank);
                    UIManager.CloseUI(EUIID.UI_BossTower_BossFightRank);
                    UIManager.CloseUI(EUIID.UI_BossTower_Result);
                    UIManager.CloseUI(EUIID.UI_BossTower_EnterFightVote);
                    UIManager.CloseUI(EUIID.UI_BossTower_QualifierMain);
                    UIManager.CloseUI(EUIID.UI_BossTower_QualifierExplain);
                }
            }
        }
        /// <summary>当前是否在boss资格挑战地图</summary>
        bool bIsOnBossTowerMap;
        private void OnLoadMapOk()
        {
            bIsOnBossTowerMap = CheckIsOnBossTowerMap();
        }
        private void OnBeMember()
        {
            UIManager.CloseUI(EUIID.UI_BossTower_QualifierMain);
            UIManager.CloseUI(EUIID.UI_BossTower_FightMain);
        }
        #endregion
        #region Nty
        /// <summary>
        /// 上线&&玩法阶段更新时下发
        /// </summary>
        /// <param name="msg"></param>
        private void OnBossTowerDataNtf(NetMsg msg)
        {
            CmdBossTowerDataNtf ntf = NetMsgUtil.Deserialize<CmdBossTowerDataNtf>(CmdBossTowerDataNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.RoleData != null)
                {
                    bJoinQualifier = ntf.RoleData.Join;
                    curQualifierMaxLvlTid = ntf.RoleData.PassedPreStageId;
                    curBossMaxStageTid = ntf.RoleData.PassedBossStageId;
                    minPreRound = ntf.RoleData.MinPreRound;
                    lastPreTime = ntf.RoleData.LastPreTime;
                    bBossUnlock = ntf.RoleData.BossUnlock;
                    minBossRound = ntf.RoleData.MinBossRound;
                    lastBossTime = ntf.RoleData.LastBossTime;
                }
                if (ntf.SysData != null)
                {
                    //curBossTowerState = (BossTowerState)ntf.SysData.State;
                    curStateEndTime = ntf.SysData.EndTime;
                    extRankNum = ntf.SysData.ExtRankNum;
                    //根据自身等级获取等级段id，再根据等级段id获取对应特性
                    if (ntf.SysData.Thirds != null && ntf.SysData.Thirds.Count > 0)
                    {
                        for (int i = 0; i < ntf.SysData.Thirds.Count; i++)
                        {
                            SysBossTowerData.Types.ThirdData data = ntf.SysData.Thirds[i];
                            if (qualifierRankDataDic.ContainsKey(data.Third))
                            {
                                qualifierRankDataDic[data.Third].featureId = data.FeatureId;
                                qualifierRankDataDic[data.Third].baseRankNum = data.BaseRankNum;
                                qualifierRankDataDic[data.Third].dynamicQualifierNum = data.DynRoleNum;
                            }
                            else
                            {
                                QualifierRankData qualifierRankData = new QualifierRankData();
                                qualifierRankData.levelGradeId = data.Third;
                                qualifierRankData.featureId = data.FeatureId;
                                qualifierRankData.baseRankNum = data.BaseRankNum;
                                qualifierRankData.dynamicQualifierNum = data.DynRoleNum;
                                qualifierRankDataDic.Add(data.Third, qualifierRankData);
                            }
                        }
                        uint levelGradeId = GetCurLevelGradeId();
                        SysBossTowerData.Types.ThirdData thirdData = ntf.SysData.Thirds.Find(o => o.Third == levelGradeId);
                        if (thirdData != null)
                        {
                            curFeatureId = thirdData.FeatureId;
                            baseRankNum = thirdData.BaseRankNum;
                            curDynamicQualifierNum = thirdData.DynRoleNum;
                        }
                    }
                    if (curBossTowerState != (BossTowerState)ntf.SysData.State)
                    {
                        curBossTowerState = (BossTowerState)ntf.SysData.State;
                        eventEmitter.Trigger(EEvents.OnBossTowerStateChange);
                        CheckTeamTarget();
                    }
                }
                SetDefaultData(false);
                HandleBossTowerData();
                eventEmitter.Trigger(EEvents.OnRefreshBossTowerReset);
            }
        }
        /// <summary>
        /// 检查队伍目标是否符合当前状态(不符合修改队伍目标)
        /// </summary>
        private void CheckTeamTarget()
        {
            if (Sys_Team.Instance.HaveTeam && Sys_Team.Instance.isCaptain())
            {
                CSVBOSSTower.Data featureData = GetBossTowerFeature();
                if (featureData != null && featureData.team_id != null && featureData.team_id.Count == 2)
                {
                    if (curBossTowerState != BossTowerState.None)
                    {
                        bool isBoss = curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver;
                        uint teamId = isBoss ? featureData.team_id[1] : featureData.team_id[0];
                        if (!Sys_Team.Instance.isTeamTarget(teamId))
                        {
                            CSVTeam.Data teamData = CSVTeam.Instance.GetConfData(teamId);
                            if (teamData != null)
                                Sys_Team.Instance.ApplyEditTarget(teamId, teamData.lv_min, teamData.lv_max, true, true);
                            else
                                DebugUtil.LogError("CSVTeam not found id:" + teamId);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 处理当前特性下的资格赛 || boss战数据
        /// </summary>
        public void HandleBossTowerData()
        {
            switch (curBossTowerState)
            {
                case BossTowerState.None:
                    break;
                case BossTowerState.Challenge:
                case BossTowerState.ChallengeOver:
                    BossTowerQualifierData qualifierData = GetBossTowerQualifierData(curQualifierMaxLvlTid);
                    if (qualifierData != null)
                    {
                        List<BossTowerQualifierData> qualifierDataList = GetBossTowerQualifierDataList();
                        if (qualifierDataList != null && qualifierDataList.Count > 0)
                        {
                            for (int i = 0; i < qualifierDataList.Count; i++)
                            {
                                if (qualifierDataList[i].csvData.floor_number <= qualifierData.csvData.floor_number)
                                    qualifierDataList[i].isPass = true;
                            }
                        }
                    }
                    break;
                case BossTowerState.Boss:
                case BossTowerState.BossOver:
                    BossTowerStageData bossData = GetBossTowerStageData(curBossMaxStageTid);
                    if (bossData != null)
                    {
                        List<BossTowerStageData> stageDataList = GetBossTowerStageDataList();
                        if (stageDataList != null && stageDataList.Count > 0)
                        {
                            for (int i = 0; i < stageDataList.Count; i++)
                            {
                                if (stageDataList[i].csvData.stage_number <= bossData.csvData.stage_number)
                                    stageDataList[i].isPass = true;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 个人排行返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnSelfRankNtf(NetMsg msg)
        {
            CmdBossTowerSelfRankNtf ntf = NetMsgUtil.Deserialize<CmdBossTowerSelfRankNtf>(CmdBossTowerSelfRankNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.NumData != null && ntf.NumData.Count > 0)
                {
                    for (int i = 0; i < ntf.NumData.Count; i++)
                    {
                        if (qualifierRankDataDic.ContainsKey(ntf.NumData[i].Third))
                        {
                            qualifierRankDataDic[ntf.NumData[i].Third].qualifierNum = ntf.NumData[i].CurQualifierNum;
                            qualifierRankDataDic[ntf.NumData[i].Third].baseRankNum = ntf.NumData[i].BaseRankNum;
                            qualifierRankDataDic[ntf.NumData[i].Third].dynamicQualifierNum = ntf.NumData[i].DynRoleNum;
                        }
                        else
                        {
                            QualifierRankData data = new QualifierRankData();
                            data.levelGradeId = ntf.NumData[i].Third;
                            data.qualifierNum = ntf.NumData[i].CurQualifierNum;
                            data.baseRankNum = ntf.NumData[i].BaseRankNum;
                            data.dynamicQualifierNum = ntf.NumData[i].DynRoleNum;
                            qualifierRankDataDic.Add(ntf.NumData[i].Third, data);
                        }
                    }
                    uint levelGradeId = GetCurLevelGradeId();
                    CmdBossTowerSelfRankNtf.Types.NumData numData = ntf.NumData.Find(o => o.Third == levelGradeId);
                    if (numData != null)
                    {
                        curQualifierNum = numData.CurQualifierNum;
                        baseRankNum = numData.BaseRankNum;
                        curDynamicQualifierNum = numData.DynRoleNum;
                    }
                }
                curQualifierRank = ntf.ChallengeRank;
                extRankNum = ntf.ExtRankNum;
                selfRankRefreshTime = ntf.CacheEndTime;
            }
            eventEmitter.Trigger(EEvents.OnRefreshSelfRankData);
        }
        /// <summary>
        /// 请求挑战返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnChallengeRes(NetMsg msg)
        {
            CmdBossTowerChallengeRes ntf = NetMsgUtil.Deserialize<CmdBossTowerChallengeRes>(CmdBossTowerChallengeRes.Parser, msg);
            if (ntf != null)
            {
            }
        }
        /// <summary>
        /// 排行数据列表
        /// </summary>
        /// <param name="msg"></param>
        private void OnRankListRes(NetMsg msg)
        {
            CmdBossTowerRankListRes ntf = NetMsgUtil.Deserialize<CmdBossTowerRankListRes>(CmdBossTowerRankListRes.Parser, msg);
            if (ntf != null)
            {
                if (ntf.SelfUnit != null)
                {
                    selfRankUnit = ntf.SelfUnit;
                }
                int count = (int)ntf.PageNum * OnePageDatasNum;
                if (bossTowerRankUnitDic.ContainsKey(ntf.IsBoss))
                {
                    if (bossTowerRankUnitDic[ntf.IsBoss].ContainsKey(ntf.Third))
                    {
                        if (count >= bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].Count)
                            bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].AddRange(ntf.List);
                        else
                        {
                            bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].RemoveRange(count, ntf.List.Count);
                            bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].InsertRange(count, ntf.List);
                        }
                    }
                    else
                    {
                        bossTowerRankUnitDic[ntf.IsBoss][ntf.Third] = new List<BossTowerRankUnit>();
                        bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].AddRange(ntf.List);
                    }
                }
                else
                {
                    bossTowerRankUnitDic[ntf.IsBoss] = new Dictionary<uint, List<BossTowerRankUnit>>();
                    bossTowerRankUnitDic[ntf.IsBoss][ntf.Third] = new List<BossTowerRankUnit>();
                    bossTowerRankUnitDic[ntf.IsBoss][ntf.Third].AddRange(ntf.List);
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshRankList, ntf.PageNum);
        }
        public List<BossTowerRankUnit> GetAllRankList(bool isBoss, uint third)
        {
            if (bossTowerRankUnitDic.ContainsKey(isBoss))
            {
                if (bossTowerRankUnitDic[isBoss].ContainsKey(third))
                    return bossTowerRankUnitDic[isBoss][third];
            }
            return null;
        }
        #endregion
        #region Req
        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        /// <param name="reuslt">投票结果 1 同意 ，2 反对</param>
        public void OnDoVoteReq(bool agree)
        {
            if (VoteID == 0)
                return;
            Sys_Vote.Instance.Send_DoVoteReq(VoteID, (uint)(agree ? 1 : 2));
        }
        /// <summary>
        /// 取消投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        public void OnDoVoteCancel()
        {
            if (VoteID == 0)
                return;
            Sys_Vote.Instance.Send_CancleVoteReq(VoteID);
        }
        /// <summary>
        /// 请求个人排行数据
        /// </summary>
        public void OnBossTowerSelfRankReq()
        {
            if (curBossTowerState == BossTowerState.Challenge || curBossTowerState == BossTowerState.ChallengeOver)
            {
                int diff = (int)(selfRankRefreshTime - TimeManager.GetServerTime());
                if (diff <= 0)
                {
                    CmdBossTowerSelfRankReq req = new CmdBossTowerSelfRankReq();
                    NetClient.Instance.SendMessage((ushort)CmdBossTower.SelfRankReq, req);
                }
            }
        }
        /// <summary>当前请求挑战的tid</summary>
        uint curReqChallengeTid;
        /// <summary>当前请求挑战的tid</summary>
        bool isBossChallenge;
        /// <summary>
        /// 请求挑战 资格赛 || Boss战
        /// </summary>
        /// <param name="isBoss">true boss战  false资格赛</param>
        /// <param name="tid">第几层tid 仅资格赛填充</param>
        public void OnBossTowerChallengeReq(bool isBoss, uint tid = 0)
        {
            isBossChallenge = isBoss;
            uint rlt = CheckIsCanEnterFight(isBoss,tid);
            if (rlt == 0)
            {
                uint stageId = 0;
                List<BossTowerStageData> dataList = GetBossTowerStageDataList();
                if (dataList != null && dataList.Count > 0)
                    stageId = dataList[0].tid;
                curReqChallengeTid = isBoss ? stageId : tid;
                CmdBossTowerChallengeReq req = new CmdBossTowerChallengeReq();
                req.IsBoss = isBoss;
                req.StageId = tid;
                NetClient.Instance.SendMessage((ushort)CmdBossTower.ChallengeReq, req);
            }
        }
        public void OnRankListReq(bool isBoss, uint third, uint pageNum)
        {
            CmdBossTowerRankListReq req = new CmdBossTowerRankListReq();
            req.IsBoss = isBoss;
            req.Third = third;
            req.PageNum = pageNum;
            NetClient.Instance.SendMessage((ushort)CmdBossTower.RankListReq, req);
        }
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            challengeCDIsOver = true;
            selfRankRefreshTime = 0;
            bossTowerVoteData = null;
            curBossTowerState = BossTowerState.None;
            if (!isLogin)
            {
                isLogin = true;
                var bossTowerData = CSVBOSSTower.Instance.GetAll();
                for (int i = 0; i < bossTowerData.Count; i++)
                {
                    if (bossTowerFeatureDic.ContainsKey(bossTowerData[i].levelGrade_id))
                        bossTowerFeatureDic[bossTowerData[i].levelGrade_id].Add(bossTowerData[i]);
                    else
                        bossTowerFeatureDic[bossTowerData[i].levelGrade_id] = new List<CSVBOSSTower.Data>() { bossTowerData[i] };

                    if (bossTowerData[i].order != 0)
                    {
                        if (bossTowerFeatureGroupDic.ContainsKey(bossTowerData[i].order))
                            bossTowerFeatureGroupDic[bossTowerData[i].order].Add(bossTowerData[i]);
                        else
                            bossTowerFeatureGroupDic[bossTowerData[i].order] = new List<CSVBOSSTower.Data>() { bossTowerData[i] };
                    }
                }
                foreach (var item in bossTowerFeatureDic.Values)
                {
                    item.Sort((a, b) => {
                        return (int)(a.id - b.id);
                    });
                }
                foreach (var item in bossTowerFeatureGroupDic)
                {
                    BossTowerFeatureGroupData data = new BossTowerFeatureGroupData();
                    data.groupId = item.Key;
                    data.csvDataList.AddRange(item.Value);
                    bossTowerFeatureGroupDataList.Add(data);
                }
                var bossQualifierData = CSVBOSSTowerQualifier.Instance.GetAll();
                for (int i = 0; i < bossQualifierData.Count; i++)
                {
                    BossTowerQualifierData data = new BossTowerQualifierData();
                    data.tid = bossQualifierData[i].id;
                    data.csvData = bossQualifierData[i];
                    data.isPass = false;
                    if (bossQualifierDataDic.ContainsKey(bossQualifierData[i].tower_id))
                        bossQualifierDataDic[bossQualifierData[i].tower_id].Add(data);
                    else
                        bossQualifierDataDic[bossQualifierData[i].tower_id] = new List<BossTowerQualifierData>() { data };
                }
                foreach (var item in bossQualifierDataDic.Values)
                {
                    item.Sort((a,b)=> {
                        return (int)(a.tid - b.tid);
                    });
                }
                var bossStageData = CSVBOSSTowerStage.Instance.GetAll();
                for (int i = 0; i < bossStageData.Count; i++)
                {
                    BossTowerStageData data = new BossTowerStageData();
                    data.tid = bossStageData[i].id;
                    data.csvData = bossStageData[i];
                    data.isPass = false;
                    if (bossStageDataDic.ContainsKey(bossStageData[i].tower_id))
                        bossStageDataDic[bossStageData[i].tower_id].Add(data);
                    else
                        bossStageDataDic[bossStageData[i].tower_id] = new List<BossTowerStageData>() { data };
                }
                foreach (var item in bossStageDataDic.Values)
                {
                    item.Sort((a, b) => {
                        return (int)(a.tid - b.tid);
                    });
                }
                var bossRankRewardData = CSVBOSSTowerRankReward.Instance.GetAll();
                for (int i = 0; i < bossRankRewardData.Count; i++)
                {
                    if (bossRankRewardDic.ContainsKey(bossRankRewardData[i].levelGrade_id))
                        bossRankRewardDic[bossRankRewardData[i].levelGrade_id].Add(bossRankRewardData[i]);
                    else
                        bossRankRewardDic[bossRankRewardData[i].levelGrade_id] = new List<CSVBOSSTowerRankReward.Data>() { bossRankRewardData[i] };
                }
                foreach (var item in bossRankRewardDic.Values)
                {
                    item.Sort((a, b) => {
                        return (int)(a.id - b.id);
                    });
                }
                var param = CSVParam.Instance.GetConfData(1533);
                string[] values = param.str_value.Split('|');
                int count = values.Length;
                levelGrades = new uint[count];
                for (int i = 0; i < count; i++)
                {
                    levelGrades[i]= uint.Parse(values[i]);
                }
                for (int i = 0; i < count - 1; i++)
                {
                    levelGradeRangeDic[(uint)(i + 1)] = new uint[2];
                    levelGradeRangeDic[(uint)(i + 1)][0]= uint.Parse(values[i]);
                    if (i < count - 2)
                        levelGradeRangeDic[(uint)(i + 1)][1] = uint.Parse(values[i + 1]) - 1;
                    else
                        levelGradeRangeDic[(uint)(i + 1)][1] = uint.Parse(values[i + 1]);
                }
            }
        }
        private void ClearData()
        {
            bossTowerFeatureDic.Clear();
            bossQualifierDataDic.Clear();
            bossStageDataDic.Clear();
            bossRankRewardDic.Clear();
            levelGrades = null;
            levelGradeRangeDic.Clear();
            bossTowerFeatureGroupDic.Clear();
            bossTowerFeatureGroupDataList.Clear();
            voteState.Clear();
            bossTowerVoteData = null;
            challengeCDTimer?.Cancel();
            bossTowerRankUnitDic.Clear();
            qualifierRankDataDic.Clear();
        }
        private void SetDefaultData(bool isAll = true)
        {
            foreach (var item in bossQualifierDataDic.Values)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].isPass = false;
                }
            }
            foreach (var item in bossStageDataDic.Values)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    item[i].isPass = false;
                }
            }
            if (isAll)
            {
                curQualifierNum = 0;
                baseRankNum = 0;
                curDynamicQualifierNum = 0;
                selfRankRefreshTime = 0;
                challengeCDTimer?.Cancel();
                challengeCDIsOver = true;
                voteState.Clear();
                bossTowerVoteData = null;
                curBossTowerState = BossTowerState.None;
                bossTowerRankUnitDic.Clear();
                qualifierRankDataDic.Clear();
            }
        }
        #region funcation
        /// <summary>
        /// 获取活动下次刷新剩余时间
        /// </summary>
        /// <returns></returns>
        public int GetActivityRestTimeDiff()
        {
            return (int)(curStateEndTime - TimeManager.GetServerTime());
        }
        public uint CheckCurBossTowerState()
        {
            uint lanId = 0;
            switch (curBossTowerState)
            {
                case BossTowerState.None:
                    break;
                case BossTowerState.Challenge:
                    break;
                case BossTowerState.ChallengeOver:
                    lanId = 1009313;
                    break;
                case BossTowerState.Boss:
                    if (!bBossUnlock)
                        lanId = 1009316;
                    break;
                case BossTowerState.BossOver:
                    lanId = 1009314;
                    break;
                default:
                    break;
            }
            if (lanId != 0) Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(lanId));
            return lanId;
        }
        /// <summary>
        /// 检测是否可以进入战斗
        /// </summary>
        /// <returns></returns>
        public uint CheckIsCanEnterFight(bool isBoss,uint tid)
        {
            uint rlt = 0;
            bool isOpen = CSVCheckseq.Instance.GetConfData((uint)(isBoss ? 12202 : 12201)).IsValid();
            //功能未解锁
            if (!isOpen)
            {
                if (isBoss)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15210));
                else
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(15211));
                return 6;
            }

            CSVBOSSTower.Data featureData = GetBossTowerFeature();
            uint curMapId = 0;
            if (featureData != null)
                curMapId = featureData.map_id;
            List<string> teamLevelList = new List<string>();
            List<string> teamLeaveOrOffLineList = new List<string>();
            //uint[] levelSection = null;
            //等级区间
            List<uint> levelRange = new List<uint>();
            //队伍人数区间
            List<uint> teamCountRange = new List<uint>(2);
            if (Sys_Map.Instance.CurMapId == curMapId)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (!Sys_Team.Instance.isCaptain())
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009340));
                        return 7;
                    }
                    if (isBoss)
                    {
                        if (curBossTowerState == BossTowerState.Challenge || curBossTowerState == BossTowerState.ChallengeOver)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009314));
                            return 8;
                        }
                    }
                    else
                    {
                        if (curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009313));
                            return 8;
                        }
                    }
                    uint lanId = CheckCurBossTowerState();
                    if (lanId != 0) return 8;

                    BossTowerQualifierData qualifierData = null;
                    bool isUnlock = true;
                    if (isBoss)
                    {
                        List<BossTowerStageData> dataList = GetBossTowerStageDataList();
                        if (dataList != null && dataList.Count > 0)
                        {
                            teamCountRange = dataList[0].csvData.playerLimit;
                            levelRange = dataList[0].csvData.levelGrade_taxt;
                        }
                    }
                    else
                    {
                        qualifierData = GetBossTowerQualifierData(tid);
                        BossTowerQualifierData qualifierData_1 = GetBossTowerQualifierData(GetBossTowerNextTid(1));
                        if (qualifierData != null)
                        {
                            teamCountRange = qualifierData.csvData.playerLimit;
                            levelRange = qualifierData.csvData.levelGrade_taxt;
                            if (qualifierData_1 != null)
                            {
                                isUnlock = qualifierData.csvData.floor_number <= qualifierData_1.csvData.floor_number;
                                //需通过上一层
                                if (!isUnlock)
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009318, LanguageHelper.GetTextContent(qualifierData_1.csvData.name)));
                                    return 9;
                                }
                            }
                        }
                        else
                        {
                            DebugUtil.LogError("CSVBOSSTowerQualifier not find id：" + tid);
                            return 11;
                        }
                    }
                    //挑战冷却时间未结束
                    if (!challengeCDIsOver)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1009315, challengeCDDiffTime.ToString()));
                        return 10;
                    }

                    if (Sys_Team.Instance.TeamMemsCount >= teamCountRange[0] && Sys_Team.Instance.TeamMemsCount<= teamCountRange[1])
                    {
                        int count = Sys_Team.Instance.teamMems.Count;
                        TeamMem captainData = Sys_Team.Instance.teamMems.Find(o => Sys_Team.Instance.isCaptain(o.MemId));
                        //levelSection = GetLevelSection(CheckLevelGrade(captainData.Level));
                        for (int i = 0; i < count; i++)
                        {
                            TeamMem teamMem = Sys_Team.Instance.teamMems[i];
                            if (TeamMemHelper.IsLeave(teamMem) || TeamMemHelper.IsOffLine(teamMem))//离队、离线
                            {
                                teamLeaveOrOffLineList.Add(teamMem.Name.ToStringUtf8());
                                rlt = 3;
                            }
                        }
                        if (teamLeaveOrOffLineList.Count <= 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                TeamMem teamMem = Sys_Team.Instance.teamMems[i];
                                if (!TeamMemHelper.IsLeave(teamMem) && !TeamMemHelper.IsOffLine(teamMem))//没有离队、离线
                                {
                                    if (!(teamMem.Level >= levelRange[0] && teamMem.Level <= levelRange[1]))
                                    {
                                        rlt = 4;
                                        teamLevelList.Add(teamMem.Name.ToStringUtf8());
                                    }
                                }
                            }
                        }
                    }
                    else
                        rlt = 2;
                }
                else
                    rlt = 1;
            }
            else
                rlt = 5;
            string str = string.Empty;
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            switch (rlt)
            {
                case 1://当前没有队伍
                    str = LanguageHelper.GetTextContent(101811);
                    break;
                case 2://队伍人数不足
                    str = LanguageHelper.GetTextContent(1009303, string.Format("{0}-{1}", teamCountRange[0], teamCountRange[1]));
                    break;
                case 3://有队员离线或者暂离
                    if (teamLeaveOrOffLineList != null && teamLeaveOrOffLineList.Count > 0)
                    {
                        for (int i = 0; i < teamLeaveOrOffLineList.Count; i++)
                        {
                            stringBuilder.Append(teamLeaveOrOffLineList[i]);
                            if (i < teamLeaveOrOffLineList.Count - 1)
                            {
                                stringBuilder.Append("、");
                            }
                        }
                        str = LanguageHelper.GetTextContent(1009301, stringBuilder.ToString());
                    }
                    break;
                case 4://有队员等级不满足
                    if (teamLevelList != null && teamLevelList.Count > 0)
                    {
                        for (int i = 0; i < teamLevelList.Count; i++)
                        {
                            stringBuilder.Append(teamLevelList[i]);
                            if (i < teamLevelList.Count - 1)
                            {
                                stringBuilder.Append("、");
                            }
                        }
                        str = LanguageHelper.GetTextContent(1009302, stringBuilder.ToString(), string.Format("{0}-{1}", levelRange[0], levelRange[1]));
                    }
                    break;
                case 5://是否在BossTower的地图中
                    CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(curMapId);
                    if (mapData != null)
                        str = LanguageHelper.GetTextContent(1009307, LanguageHelper.GetTextContent(mapData.name));
                    break;
            }
            if (str != string.Empty)
                Sys_Hint.Instance.PushContent_Normal(str);
            StringBuilderPool.ReleaseTemporary(stringBuilder);
            if (teamLeaveOrOffLineList != null && teamLeaveOrOffLineList.Count > 0)
                teamLeaveOrOffLineList.Clear();
            if (teamLevelList != null && teamLevelList.Count > 0)
                teamLevelList.Clear();
            return rlt;
        }
        /// <summary>
        /// 获取Boss特性
        /// </summary>
        /// <param name="featureId">特性id</param>
        /// <returns></returns>
        public CSVBOSSTower.Data GetBossTowerFeature(uint featureId = 0)
        {
            featureId = featureId == 0 ? curFeatureId : featureId;
            CSVBOSSTower.Data data = CSVBOSSTower.Instance.GetConfData(featureId);
            if (data == null)
                DebugUtil.LogError("CSVBOSSTower not found id :" + featureId);
            return data;
        }
        /// <summary>
        /// 获取等级段对应的boss特性数据
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public List<CSVBOSSTower.Data> GetBossTowerFeatureList(uint levelId = 0)
        {
            List<CSVBOSSTower.Data> dataList = new List<CSVBOSSTower.Data>();
            levelId = levelId != 0 ? levelId : CheckLevelGrade();
            if (bossTowerFeatureDic.ContainsKey(levelId))
            {
                dataList.AddRange(bossTowerFeatureDic[levelId]);
            }
            return dataList;
        }
        /// <summary>
        /// 获取当前特性对应的boss特性数据(仅本周特性所在的特性组)
        /// </summary>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public List<CSVBOSSTower.Data> GetCurBossTowerFeatureList(uint levelId)
        {
            List<CSVBOSSTower.Data> dataList = new List<CSVBOSSTower.Data>();
            CSVBOSSTower.Data featureData = GetBossTowerFeature();
            if (featureData != null)
            {
                if (bossTowerFeatureGroupDic.ContainsKey(featureData.order))
                {
                    CSVBOSSTower.Data data = bossTowerFeatureGroupDic[featureData.order].Find(o => o.levelGrade_id == levelId);
                    if (data != null)
                        dataList.Add(data);
                    else
                        DebugUtil.LogErrorFormat("CSVBOSSTower {0}没有第{1}等级段数据", LanguageHelper.GetTextContent(featureData.name), levelId);
                }
            }
            return dataList;
        }
        /// <summary>
        /// 检查所属等级段
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public uint CheckLevelGrade(uint level = 0)
        {
            uint stage = 0;
            level = level != 0 ? level : Sys_Role.Instance.Role.Level;
            for (int i = levelGrades.Length - 1; i > 0; i--)
            {
                if (level >= levelGrades[i])
                    break;
                stage = (uint)i;
            }
            return stage;
        }
        /// <summary>
        /// 获取当前等级段id
        /// </summary>
        /// <returns></returns>
        public uint GetCurLevelGradeId()
        {
            return CheckLevelGrade();
        }
        /// <summary>
        /// 根据等级段id获取等级段区间
        /// </summary>
        /// <param name="levelId">等级段id</param>
        /// <returns></returns>
        private uint[] GetLevelSection(uint levelId)
        {
            uint[] section = null;
            if (levelGradeRangeDic.ContainsKey(levelId))
                section = levelGradeRangeDic[levelId];
            return section;
        }
        /// <summary>
        /// 获取常量参数
        /// </summary>
        /// <returns></returns>
        public string GetBossTowerParameter(uint tid)
        {
            return CSVBOSSTowerParameter.Instance.GetConfData(tid).str_value;
        }
        /// <summary>
        /// 获取动态资格数
        /// </summary>
        /// <returns></returns>
        public uint GetDynamicQualifierNum(uint rankNum = 0)
        {
            rankNum = rankNum == 0 ? curQualifierNum : rankNum;
            uint extarNum = 0;
            string param = GetBossTowerParameter(3);
            string[] strValue = param.Split('|');
            for (int i = strValue.Length - 1; i >= 0; i--)
            {
                if (rankNum >= uint.Parse(strValue[i].Split('&')[1]))
                {
                    extarNum = uint.Parse(strValue[i].Split('&')[0]);
                    break;
                }
            }
            return extarNum;
        }
        /// <summary>
        /// 根据特性id获取资格赛数据列表
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public List<BossTowerQualifierData> GetBossTowerQualifierDataList(uint featureId = 0)
        {
            featureId = featureId != 0 ? featureId : curFeatureId;
            if (bossQualifierDataDic.ContainsKey(featureId))
                return bossQualifierDataDic[featureId];
            else
                DebugUtil.LogErrorFormat("by featureId({0}) not find CSVBOSSTowerQualifier data", featureId);
            return null;
        }
        /// <summary>
        /// 根据特性id获取Boss战阶段数据列表
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public List<BossTowerStageData> GetBossTowerStageDataList(uint featureId = 0)
        {
            featureId = featureId != 0 ? featureId : curFeatureId;
            if (bossStageDataDic.ContainsKey(featureId))
                return bossStageDataDic[featureId];
            else
                DebugUtil.LogErrorFormat("by featureId({0}) not find CSVBOSSTowerStage data", featureId);
            return null;
        }
        /// <summary>
        /// 获取资格赛指定特性对应的tid数据
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public BossTowerQualifierData GetBossTowerQualifierData(uint tid, uint featureId = 0)
        {
            BossTowerQualifierData data = null;
            List<BossTowerQualifierData> list = GetBossTowerQualifierDataList(featureId);
            if (list != null && list.Count > 0)
            {
                data = list.Find(o => o.tid == tid);
            }
            return data;
        }
        /// <summary>
        /// 获Boss战指定特性对应的tid数据
        /// </summary>
        /// <param name="tid"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public BossTowerStageData GetBossTowerStageData(uint tid, uint featureId = 0)
        {
            BossTowerStageData data = null;
            List<BossTowerStageData> list = GetBossTowerStageDataList(featureId);
            if (list != null && list.Count > 0)
            {
                data = list.Find(o => o.tid == tid);
            }
            return data;
        }
        /// <summary>
        /// 获取资格赛最大层数 || boss战最大阶段数
        /// </summary>
        /// <param name="type"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public int GetBossTowerMaxCount(uint type, uint featureId = 0)
        {
            featureId = featureId != 0 ? featureId : curFeatureId;
            int count = 0;
            if (type == 1)
            {
                if (bossQualifierDataDic.ContainsKey(featureId))
                    count = bossQualifierDataDic[featureId].Count;
            }
            else
            {
                if (bossStageDataDic.ContainsKey(featureId))
                    count = bossStageDataDic[featureId].Count;
            }
            return count;
        }
        /// <summary>
        /// 获取资格赛最第一层or后一层tid || boss战第一阶段or最后阶段tid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public uint GetBossTowerFirstOrLastDataTid(uint type, bool isFirst = true, uint featureId = 0)
        {
            featureId = featureId != 0 ? featureId : curFeatureId;
            uint tid = 0;
            int count = GetBossTowerMaxCount(type, featureId);
            if (type == 1)
            {
                if (bossQualifierDataDic.ContainsKey(featureId))
                {
                    if (isFirst)
                        tid = bossQualifierDataDic[featureId][0].tid;
                    else
                        tid = bossQualifierDataDic[featureId][count - 1].tid;
                }
            }
            else
            {
                if (bossStageDataDic.ContainsKey(featureId))
                {
                    if (isFirst)
                        tid = bossStageDataDic[featureId][0].tid;
                    else
                        tid = bossStageDataDic[featureId][count - 1].tid;
                }
            }
            return tid;
        }
        /// <summary>
        /// 获取资格赛系下一层tid || boss战下一阶段tid
        /// </summary>
        /// <param name="type"></param>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public uint GetBossTowerNextTid(uint type, uint featureId = 0)
        {
            featureId = featureId != 0 ? featureId : curFeatureId;
            uint tid = 0;
            if (type == 1)
            {
                List<BossTowerQualifierData> dataList= GetBossTowerQualifierDataList(featureId);
                if (dataList != null && dataList.Count > 0)
                {
                    if (curQualifierMaxLvlTid == 0)
                        tid = dataList[0].tid;
                    else
                    {
                        BossTowerQualifierData data = dataList.Find(o => o.tid == curQualifierMaxLvlTid);
                        if (data != null)
                            tid = data.csvData.nextFloor_id == 0 ? curQualifierMaxLvlTid : data.csvData.nextFloor_id;
                        else
                            DebugUtil.LogError("CSVBOSSTowerQualifier not find id:" + curQualifierMaxLvlTid);
                    }
                }
            }
            else
            {
                List<BossTowerStageData> dataList = GetBossTowerStageDataList(featureId);
                if (dataList != null && dataList.Count > 0)
                {
                    if (curBossMaxStageTid == 0)
                        tid = dataList[0].tid;
                    else
                    {
                        BossTowerStageData data = dataList.Find(o => o.tid == curBossMaxStageTid);
                        if (data != null)
                            tid = data.csvData.nextFloor_id == 0 ? curBossMaxStageTid : data.csvData.nextFloor_id;
                        else
                            DebugUtil.LogError("CSVBOSSTowerStage not find id:" + curBossMaxStageTid);
                    }
                }
            }
            return tid;
        }
        /// <summary>
        /// 获取所处等级段排名对应boss排行奖励数据
        /// </summary>
        /// <param name="levelId">等级段id</param>
        /// <param name="rankNum">排名</param>
        /// <returns></returns>
        public CSVBOSSTowerRankReward.Data GetBossTowerRankReward(uint rankNum, uint levelId = 0)
        {
            levelId = levelId != 0 ? levelId : CheckLevelGrade();
            CSVBOSSTowerRankReward.Data data = null;
            if (bossRankRewardDic.ContainsKey(levelId))
            {
                List<CSVBOSSTowerRankReward.Data> dataList = bossRankRewardDic[levelId];
                if (dataList != null && dataList.Count > 0)
                {
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        List<uint> rankRange = dataList[i].rankingRange;
                        if (rankRange != null && rankRange.Count == 2)
                        {
                            if (rankNum >= rankRange[0] && rankNum <= rankRange[1])
                            {
                                data = dataList[i];
                                break;
                            }
                        }
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// 快捷组队
        /// </summary>
        public void FastTeam(bool isBoss)
        {
            CSVBOSSTower.Data featureData = GetBossTowerFeature();
            if (featureData != null && featureData.team_id != null && featureData.team_id.Count == 2)
            {
                uint teamId = isBoss ? featureData.team_id[1] : featureData.team_id[0];
                if (Sys_Team.Instance.IsFastOpen(true))
                    Sys_Team.Instance.OpenFastUI(teamId);
            }
        }
        /// <summary>
        /// 检查资格赛是否通过最低规定层数
        /// </summary>
        /// <returns></returns>
        public bool CheckIsPassMinLayer()
        {
            BossTowerQualifierData data = GetBossTowerQualifierData(curQualifierMaxLvlTid);
            if (data != null)
            {
                if (data.csvData.floor_number >= minPassLayer)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查是否有Boss挑战资格（资格赛阶段）
        /// </summary>
        /// <returns></returns>
        public bool CheckIsHaveBossFightQualifier(uint rankNum = 0, uint layer = 0)
        {
            bool isHave = false;
            rankNum = rankNum == 0 ? curQualifierRank : rankNum;
            if (rankNum <= 0) return isHave;
            bool isCan = rankNum <= GetCurAllRankNum();
            if (layer == 0)
            {
                if (isCan && CheckIsPassMinLayer())
                    isHave = true;
            }
            else
            {
                List<BossTowerQualifierData> dataList = GetBossTowerQualifierDataList();
                if (dataList != null && dataList.Count > 0)
                {
                    BossTowerQualifierData data = dataList.Find(o => o.csvData.floor_number == layer);
                    if (data != null)
                    {
                        if (isCan && data.csvData.floor_number >= minPassLayer)
                            isHave = true;
                    }
                    else
                        DebugUtil.LogErrorFormat("CSVBOSSTowerQualifier not have {0} floor_number data", layer);
                }
            }
            return isHave;
        }
        /// <summary>
        /// 资格赛当前总资格数
        /// </summary>
        /// <returns></returns>
        public uint GetCurAllRankNum()
        {
            return baseRankNum + extRankNum;
        }
        /// <summary>
        /// 获取资格赛当前排名状态描述
        /// </summary>
        /// <param name="type">1当前排名{0}  2排名{0}</param>
        /// <returns></returns>
        public string GetCurRnakStateDescribe(uint type = 0)
        {
            string str = string.Empty;
            if (curBossTowerState == BossTowerState.Challenge || curBossTowerState == BossTowerState.ChallengeOver)
            {
                str = LanguageHelper.GetTextContent(1009304, minPassLayer.ToString());
                if (bJoinQualifier)
                {
                    bool isPass = CheckIsPassMinLayer();
                    if (isPass)
                    {
                        uint allRankNum = uint.Parse(GetBossTowerParameter(8));
                        if (curQualifierRank <= 0)
                        {
                            str = LanguageHelper.GetTextContent(1009060);
                        }
                        else
                        {
                            string str_1 = string.Empty;
                            if (curQualifierRank > allRankNum)//红色  
                                str_1 = string.Format("<color=#F1372D>{0}</color>/{1}", curQualifierRank, allRankNum);
                            else                             //绿色
                                str_1 = string.Format("<color=#79FF13>{0}</color>/{1}", curQualifierRank, allRankNum);

                            if (str_1 != string.Empty)
                            {
                                if (type == 1)
                                    str = LanguageHelper.GetTextContent(1009310, str_1);
                                else
                                    str = LanguageHelper.GetTextContent(1009311, str_1);
                            }
                        }

                    }
                }
            }
            else if(curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver)
            {
                str = LanguageHelper.GetTextContent(1009341, LanguageHelper.GetTextContent(bBossUnlock ? 1009342u : 1009343u));
            }
            return str;
        }
        /// <summary>
        /// 获取不同等级段的资格排行数据
        /// </summary>
        /// <param name="levelGradeId"></param>
        /// <returns></returns>
        public QualifierRankData GetQualifierRankData(uint levelGradeId)
        {
            QualifierRankData data = null;
            if (qualifierRankDataDic.ContainsKey(levelGradeId))
                data = qualifierRankDataDic[levelGradeId];
            return data;
        }
        #region vote
        /// <summary>
        /// 获取投票剩余时间
        /// </summary>
        /// <param name="isBoss"></param>
        /// <returns></returns>
        public float GetVoteDiffTime(bool isBoss)
        {
            if (VoteStartTime == 0)
                return 0;

            int realtime = isBoss ? int.Parse(GetBossTowerParameter(102)) : int.Parse(GetBossTowerParameter(7));
            uint servertime = Sys_Time.Instance.GetServerTime();
            float time = realtime;
            if (servertime > VoteStartTime)
            {
                uint distime = servertime - VoteStartTime;
                time = realtime - (float)distime;
                time = Mathf.Max(0, time);
            }
            return time;
        }
        /// <summary>
        /// 获取投票成员通关数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public uint GetRolePassStageId(ulong roleId=0)
        {
            roleId = roleId == 0 ? Sys_Role.Instance.RoleId : roleId;
            if (bossTowerVoteData != null)
            {
                BossTowerVoteData.Types.Player playerData = bossTowerVoteData.Players.Find(o => o.RoleId == roleId);
                if (playerData != null)
                {
                    return playerData.PassedId;
                }
            }
            return 0;
        }
        /// <summary>
        /// 获取成员投票状态
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public VoterOpType GetBossTowerVoteState(ulong roleId)
        {
            if (voteState.ContainsKey(roleId))
                return (VoterOpType)voteState[roleId];
            else
                return VoterOpType.None;
        }
        #endregion
        /// <summary>
        /// 打开当前状态下的排行榜
        /// </summary>
        public void OpenCurStateRank()
        {
            if (curBossTowerState == BossTowerState.Challenge || curBossTowerState == BossTowerState.ChallengeOver)
                UIManager.OpenUI(EUIID.UI_BossTower_QualifierRank);
            else if(curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver)
                UIManager.OpenUI(EUIID.UI_BossTower_BossFightRank);
        }
        /// <summary>
        /// 打开当前状态下对应的主界面
        /// </summary>
        public void OpenCurStateMainView()
        {
            if (curBossTowerState == BossTowerState.Challenge || curBossTowerState == BossTowerState.ChallengeOver)
                UIManager.OpenUI(EUIID.UI_BossTower_QualifierMain);
            else if (curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver)
                UIManager.OpenUI(EUIID.UI_BossTower_FightMain);
        }
        /// <summary>
        /// 检查是否在boss特性对应地图
        /// </summary>
        /// <returns></returns>
        public bool CheckIsOnBossTowerMap()
        {
            CSVBOSSTower.Data featureData = GetBossTowerFeature();
            if (featureData != null)
            {
                return Sys_Map.Instance.CurMapId == featureData.map_id;
            }
            return false;
        }
        /// <summary>
        /// 获取当boss资格赛当前活动状态 1入围阶段 2boss挑战阶段
        /// </summary>
        /// <returns></returns>
        public uint GetBossTowerActivityState()
        {
            uint state = 1;
            if (curBossTowerState == BossTowerState.Boss || curBossTowerState == BossTowerState.BossOver)
                state = 2;
            return state;
        }
        #endregion
    }
    #region ClassData
    //资格赛数据
    public class BossTowerQualifierData
    {
        public uint tid;
        public CSVBOSSTowerQualifier.Data csvData;
        public bool isPass;//是否通关
    }
    //Boss战阶段数据
    public class BossTowerStageData
    {
        public uint tid;
        public CSVBOSSTowerStage.Data csvData;
        public bool isPass;//是否通关
    }
    /// <summary>
    /// Boss特性组数据
    /// </summary>
    public class BossTowerFeatureGroupData
    {
        public uint groupId;
        public List<CSVBOSSTower.Data> csvDataList = new List<CSVBOSSTower.Data>();

        /// <summary>
        /// 获取本周特性所在特性组数据，其他组的默认拿一个数据
        /// </summary>
        /// <returns></returns>
        public CSVBOSSTower.Data GetCurBossTowerFeatureData()
        {
            CSVBOSSTower.Data featureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature();
            if (csvDataList != null)
            {
                if (featureData != null)
                {
                    CSVBOSSTower.Data data = csvDataList.Find(o => o.id == featureData.id);
                    if (data != null)
                        return data;
                    else
                        return csvDataList[0];
                }
                else
                    return csvDataList[0];
            }
            return null;
        }
    }
    /// <summary>
    /// 资格排行数据
    /// </summary>
    public class QualifierRankData
    {
        public uint levelGradeId;
        public uint featureId;
        public uint qualifierNum;
        public uint dynamicQualifierNum;
        public uint baseRankNum;
    }
    #endregion
}