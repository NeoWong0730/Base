using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_ActivityTrialGate : SystemModuleBase<Sys_ActivityTrialGate>
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
            OnRefreshTrialGateReset,    //试炼活动重置刷新
            OnRefreshStage,             //刷新当前阶段
            OnRefreshSkillColumnDeploy, //刷新技能栏节点数据
            OnRefreshTeamDeployData,    //刷新队伍配置数据
            OnRefreshChallengeVote,     //刷新挑战投票
            OnRefreshReadyState,        //刷新备战状态
            OnRefreshRedPoint,          //刷新红点
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary>
        /// 进战状态
        /// </summary>
        public enum EEnterBattleState
        {
            Nono,
            Ready,   //准备期间
            Confirm  //确认期间按
        }
        //徽章对应的数量、表数据
        public List<BadgeData> badgeDataList = new List<BadgeData>();
        //宠物出战技能配置列表
        List<SkillColumnDeploy> skillColumnDeployList = new List<SkillColumnDeploy>();
        //按等级段分类阶段Boss
        Dictionary<uint, List<TrialStage>> trialStageDic = new Dictionary<uint, List<TrialStage>>();
        //按等级段分类阶段Boss特性
        Dictionary<uint, List<CSVTrialCharacteristic.Data>> trialCharacteristicDic = new Dictionary<uint, List<CSVTrialCharacteristic.Data>>();
        //玩家自己的特性排行数据
        Dictionary<uint, RoleTrialFeature> roleTrialFeatureDic = new Dictionary<uint, RoleTrialFeature>();
        //玩家队伍技能配置数据
        List<TrialTeamRoleData> trialTeamRoleDataList = new List<TrialTeamRoleData>();
        //投票环节参加的角色
        public List<ulong> voteRoleList = new List<ulong>();
        //下次活动重置时间
        public uint nextResetTime { get; private set; }
        //队伍配置查看请求CD
        public uint teamDeployRefreshTime { get; private set; }
        //本周当前特性Tid
        public uint curTrialFeatureTid { get; private set; }
        //进行到的阶段
        public uint curStage { get; private set; }
        //当前试炼准备状态
        public EEnterBattleState curEnterBattleState;
        //备战开始时间
        public uint readyStartTime;
        //备战结束时间
        public uint readyExpireTime;
        //确认进入战斗开始时间
        public uint confirmStartTime;
        //确认进入战斗结束时间
        public uint confirmExpireTime;
        //当前是否在试炼之门
        bool curIsOnTrialGate = false;
        //试炼之门所在地图id
        public uint trialMapId = CSVTrialParameters.Instance.GetConfData(4).value;
        //最近一次阶段
        uint lastStage;

        bool isLogin;
        #endregion
        #region 事件注册
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrialGate.DataNty, OnDataNty, CmdTrialGateDataNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrialGate.UpdateStageNty, OnUpdateStageNty, CmdTrialGateUpdateStageNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTrialGate.UnlockSkillReq, (ushort)CmdTrialGate.UnlockSkillNty, OnUnlockSkillNty, CmdTrialGateUnlockSkillNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTrialGate.SetNodePetReq, (ushort)CmdTrialGate.UpdateNodePetNty, OnUpdateNodePetNty, CmdTrialGateUpdateNodePetNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTrialGate.ChallengeReadyReq, (ushort)CmdTrialGate.ChallengeReadyNty, OnChallengeReadyNty, CmdTrialGateChallengeReadyNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrialGate.ChallengeConfirmNty, OnChallengeConfirmNty, CmdTrialGateChallengeConfirmNty.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTrialGate.ChallengeCancelNty, OnChallengeCancelNty, CmdTrialGateChallengeCancelNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTrialGate.WatchMemberConfigReq, (ushort)CmdTrialGate.WatchMemberConfigNty, OnWatchMemberConfigNty, CmdTrialGateWatchMemberConfigNty.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdTrialGate.ChallengeVoteReq, (ushort)CmdTrialGate.ChallengeVoteNty, OnChallengeVoteNty, CmdTrialGateChallengeVoteNty.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.DataNty, OnDataNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.UpdateStageNty, OnUpdateStageNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.UnlockSkillNty, OnUnlockSkillNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.UpdateNodePetNty, OnUpdateNodePetNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.ChallengeReadyNty, OnChallengeReadyNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.ChallengeConfirmNty, OnChallengeConfirmNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.ChallengeCancelNty, OnChallengeCancelNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.WatchMemberConfigNty, OnWatchMemberConfigNty);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTrialGate.ChallengeVoteNty, OnChallengeVoteNty);
            }
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, OnLoadMapOk, toRegister);
            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, OnChangeMap, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChange, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, toRegister);
        }
        private void OnReconnectResult(bool result)
        {
            UIManager.CloseUI(EUIID.UI_TrialGateMain);
            UIManager.CloseUI(EUIID.UI_TrialBattleConfirm);
            UIManager.CloseUI(EUIID.UI_TrialTeamDeploy);
            UIManager.CloseUI(EUIID.UI_TrialRank);
            UIManager.CloseUI(EUIID.UI_TrialSkillDeploy);
            UIManager.CloseUI(EUIID.UI_TrialResult);
            UIManager.CloseUI(EUIID.UI_TrialBadgeTips);
        }
        private void OnChangeMap(uint lastMapId, uint curMapId)
        {
            if (curIsOnTrialGate)
            {
                if (Sys_Map.Instance.CurMapId != trialMapId)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000033));
                    UIManager.CloseUI(EUIID.UI_TrialGateMain);
                    UIManager.CloseUI(EUIID.UI_TrialBattleConfirm);
                    UIManager.CloseUI(EUIID.UI_TrialTeamDeploy);
                    UIManager.CloseUI(EUIID.UI_TrialRank);
                    UIManager.CloseUI(EUIID.UI_TrialSkillDeploy);
                    UIManager.CloseUI(EUIID.UI_TrialResult);
                    UIManager.CloseUI(EUIID.UI_TrialBadgeTips);
                }
            }
        }
        private void OnLoadMapOk()
        {
            curIsOnTrialGate = Sys_Map.Instance.CurMapId == trialMapId;
        }
        private void OnEndBattle(CmdBattleEndNtf ntf)
        {
            if (ntf != null && ntf.TrialGate!=null)
            {
                TrialResultParam trialResultParam = new TrialResultParam();
                trialResultParam.cmdBattleEndNtf = ntf;
                //int rlt = Net_Combat.Instance.GetBattleOverResult();
                //战斗通过阶段大于当前阶段为胜利
                if (lastStage < ntf.TrialGate.Stage)
                {
                    if (lastStage != curStage)
                        lastStage = curStage;
                    trialResultParam.type = 1;
                }
                else
                {
                    //有阶段未完成
                    if (ntf.TrialGate.Stage < GetAllTrialStageNum())
                        trialResultParam.type = 3;
                    else
                        trialResultParam.type = 2;
                }
                UIManager.OpenUI(EUIID.UI_TrialResult, false, trialResultParam);
            }
        }
        private void OnCurrencyChange(uint id, long value)
        {
            if (badgeDataList.Find(o => o.badgeId == id) != null)
            {
                eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
            }
        }
        private void OnBeMember()
        {
            UIManager.CloseUI(EUIID.UI_TrialGateMain);
        }
        #endregion
        #region Nty
        /// <summary>
        /// 上线推送
        /// </summary>
        /// <param name="msg"></param>
        private void OnDataNty(NetMsg msg)
        {
            SetDefaultData();
            CmdTrialGateDataNty ntf = NetMsgUtil.Deserialize<CmdTrialGateDataNty>(CmdTrialGateDataNty.Parser, msg);
            if (ntf != null)
            {
                if (ntf.System != null)
                    curTrialFeatureTid = ntf.System.Featureid;
                if (ntf.Role != null)
                {
                    nextResetTime = ntf.Role.Resettime;
                    curStage = ntf.Role.Stage;
                    lastStage = curStage;
                    teamDeployRefreshTime = ntf.Role.Refreshtime;
                    if (ntf.Role.Nodes != null && ntf.Role.Nodes.Count > 0)
                    {
                        int count = ntf.Role.Nodes.Count;
                        for (int i = 0; i < count; i++)
                        {
                            RoleTrialGateNode node = ntf.Role.Nodes[i];
                            SkillColumnDeploy data= GetSkillColumnDeployByBarId(node.Nodeid);
                            if (data != null)
                            {
                                data.petUid = node.Petuid;
                                if (node.Skills != null && node.Skills.Count > 0)
                                {
                                    for (int j = 0; j < node.Skills.Count; j++)
                                    {
                                        SkillColumnDeploy.FirstSkillCell skillData =data.GetFirstSkillCellBySkillId(node.Skills[j]);
                                        if (skillData != null)
                                        {
                                            skillData.activateState = true;
                                        }
                                    }
                                }
                            }
                            else
                                DebugUtil.LogError("CSVTrialSkillBar not found id："+ node.Nodeid);
                        }
                    }
                    roleTrialFeatureDic.Clear();
                    if (ntf.Role.Features != null && ntf.Role.Features.Count > 0)
                    {
                        int count = ntf.Role.Features.Count;
                        for (int i = 0; i < count; i++)
                        {
                            RoleTrialGateFeature data = ntf.Role.Features[i];
                            RoleTrialFeature cell = new RoleTrialFeature() {
                                featureId = data.Featureid,
                                maxStage = data.Maxstage,
                                minRound = data.Minround,
                                timestamp=data.Timestamp
                            };
                            roleTrialFeatureDic[data.Featureid] = cell;
                        }
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshTrialGateReset);
            if (UIManager.IsVisibleAndOpen(EUIID.UI_TrialGateMain) || UIManager.IsVisibleAndOpen(EUIID.UI_TrialSkillDeploy))
            {
                //重置数据提示
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000028));
            }
        }
        /// <summary>
        /// 通知更新通过阶段
        /// </summary>
        /// <param name="msg"></param>
        private void OnUpdateStageNty(NetMsg msg)
        {
            CmdTrialGateUpdateStageNty ntf = NetMsgUtil.Deserialize<CmdTrialGateUpdateStageNty>(CmdTrialGateUpdateStageNty.Parser, msg);
            if (ntf != null)
            {
                lastStage = curStage;
                curStage = ntf.Stage;
                if (ntf.Feature != null)
                {
                    if (roleTrialFeatureDic.ContainsKey(ntf.Feature.Featureid))
                    {
                        roleTrialFeatureDic[ntf.Feature.Featureid].maxStage = ntf.Feature.Maxstage;
                        roleTrialFeatureDic[ntf.Feature.Featureid].minRound = ntf.Feature.Minround;
                        roleTrialFeatureDic[ntf.Feature.Featureid].timestamp = ntf.Feature.Timestamp;
                    }
                    else
                    {
                        RoleTrialFeature cell = new RoleTrialFeature()
                        {
                            featureId = ntf.Feature.Featureid,
                            maxStage = ntf.Feature.Maxstage,
                            minRound = ntf.Feature.Minround,
                            timestamp = ntf.Feature.Timestamp
                        };
                        roleTrialFeatureDic[ntf.Feature.Featureid] = cell;
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshStage);
        }
        /// <summary>
        /// 通知解锁技能
        /// </summary>
        /// <param name="msg"></param>
        private void OnUnlockSkillNty(NetMsg msg)
        {
            CmdTrialGateUnlockSkillNty ntf = NetMsgUtil.Deserialize<CmdTrialGateUnlockSkillNty>(CmdTrialGateUnlockSkillNty.Parser, msg);
            if (ntf != null)
            {
                for (int i = 0; i < skillColumnDeployList.Count; i++)
                {
                    SkillColumnDeploy.FirstSkillCell skillData = skillColumnDeployList[i].GetFirstSkillCellBySkillId(ntf.Skillid);
                    if (skillData != null)
                    {
                        skillData.activateState = true;
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillData.skillId);
                        if (skillInfo != null)
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000010, LanguageHelper.GetTextContent(skillInfo.name)));
                    }
                }
            }
            
            eventEmitter.Trigger(EEvents.OnRefreshSkillColumnDeploy, 2u);
            eventEmitter.Trigger(EEvents.OnRefreshRedPoint);
        }
        /// <summary>
        /// 通知更新节点宠物
        /// </summary>
        /// <param name="msg"></param>
        private void OnUpdateNodePetNty(NetMsg msg)
        {
            CmdTrialGateUpdateNodePetNty ntf = NetMsgUtil.Deserialize<CmdTrialGateUpdateNodePetNty>(CmdTrialGateUpdateNodePetNty.Parser, msg);
            if (ntf != null)
            {
                SkillColumnDeploy data = GetSkillColumnDeployByBarId(ntf.Nodeid);
                if (data != null)
                {
                    data.petUid = ntf.Petuid;
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshSkillColumnDeploy, 0u);
        }

        /// <summary>
        /// 通知挑战准备
        /// </summary>
        /// <param name="msg"></param>
        private void OnChallengeReadyNty(NetMsg msg)
        {
            CmdTrialGateChallengeReadyNty ntf = NetMsgUtil.Deserialize<CmdTrialGateChallengeReadyNty>(CmdTrialGateChallengeReadyNty.Parser, msg);
            if (ntf != null)
            {
                readyStartTime = ntf.Starttime;
                readyExpireTime = ntf.Expiretime;
                if (ntf.Briefs != null && ntf.Briefs.Count > 0)
                {
                    trialTeamRoleDataList.Clear();
                    SetBriefs(ntf.Briefs);
                }
            }
            curEnterBattleState = EEnterBattleState.Ready;
            voteRoleList.Clear();
            if (!UIManager.IsVisibleAndOpen(EUIID.UI_TrialBattleConfirm))
                UIManager.OpenUI(EUIID.UI_TrialBattleConfirm);
        }
        /// <summary>
        /// 通知挑战确认
        /// </summary>
        /// <param name="msg"></param>
        private void OnChallengeConfirmNty(NetMsg msg)
        {
            CmdTrialGateChallengeConfirmNty ntf = NetMsgUtil.Deserialize<CmdTrialGateChallengeConfirmNty>(CmdTrialGateChallengeConfirmNty.Parser, msg);
            if (ntf != null)
            {
                confirmStartTime = ntf.Starttime;
                confirmExpireTime = ntf.Expiretime;
                if (ntf.Briefs != null && ntf.Briefs.Count > 0)
                {
                    SetBriefs(ntf.Briefs);
                }
            }
            curEnterBattleState = EEnterBattleState.Confirm;
            eventEmitter.Trigger(EEvents.OnRefreshReadyState);
        }
        /// <summary>
        /// 通知挑战取消
        /// </summary>
        /// <param name="msg"></param>
        private void OnChallengeCancelNty(NetMsg msg)
        {
            CmdTrialGateChallengeCancelNty ntf = NetMsgUtil.Deserialize<CmdTrialGateChallengeCancelNty>(CmdTrialGateChallengeCancelNty.Parser, msg);
            if (ntf != null)
            {
                TeamMem teamMem = Sys_Team.Instance.teamMems.Find(o => o.MemId == ntf.Roleid);
                if (teamMem != null)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000027, teamMem.Name.ToStringUtf8()));
                }
            }
            if (GetActivityRestTimeDiff() <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000039));
            }
            curEnterBattleState = EEnterBattleState.Nono;
            voteRoleList.Clear();
            UIManager.CloseUI(EUIID.UI_TrialBattleConfirm);
        }
        /// <summary>
        /// 通知查看队员配置
        /// </summary>
        /// <param name="msg"></param>
        private void OnWatchMemberConfigNty(NetMsg msg)
        {
            CmdTrialGateWatchMemberConfigNty ntf = NetMsgUtil.Deserialize<CmdTrialGateWatchMemberConfigNty>(CmdTrialGateWatchMemberConfigNty.Parser, msg);
            if (ntf != null)
            {
                teamDeployRefreshTime = ntf.Refreshtime;
                trialTeamRoleDataList.Clear();
                SetBriefs(ntf.Briefs);
            }
            eventEmitter.Trigger(EEvents.OnRefreshTeamDeployData);
        }

        /// <summary>
        /// 通知投票结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnChallengeVoteNty(NetMsg msg)
        {
            CmdTrialGateChallengeVoteNty ntf = NetMsgUtil.Deserialize<CmdTrialGateChallengeVoteNty>(CmdTrialGateChallengeVoteNty.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Brief != null)
                {
                    voteRoleList.Add(ntf.Brief.Roleid);
                    SetBrief(ntf.Brief);
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshChallengeVote);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="briefs"></param>
        /// <param name="type">1(玩家备战投票数据) 2(玩家确认进入战斗投票数据) 3(查看队伍配置数据)</param>
        private void SetBriefs(RepeatedField<RoleTrialGateBrief> briefs)
        {
            for (int i = 0; i < briefs.Count; i++)
            {
                SetBrief(briefs[i]);
            }
        }
        private void SetBrief(RoleTrialGateBrief brief)
        {
            if (brief == null)
                return;
            TrialTeamRoleData data = trialTeamRoleDataList.Find(o => o.roleId == brief.Roleid);
            if (data != null)
            {
                if (brief.Nodes != null && brief.Nodes.Count > 0)
                {
                    List<TrialTeamRoleData.BarNode> nodeList = new List<TrialTeamRoleData.BarNode>();
                    for (int j = 0; j < brief.Nodes.Count; j++)
                    {
                        TrialTeamRoleData.BarNode node = data.barNodeList.Find(o => o.barId == brief.Nodes[j].Nodeid);
                        if (node != null)
                        {
                            node.skillList.Clear();
                            if (brief.Nodes[j].Skills != null && brief.Nodes[j].Skills.Count > 0)
                                node.skillList.AddRange(brief.Nodes[j].Skills);
                        }
                        else
                        {
                            node = new TrialTeamRoleData.BarNode();
                            node.barId = brief.Nodes[j].Nodeid;
                            if (brief.Nodes[j].Skills != null && brief.Nodes[j].Skills.Count > 0)
                                node.skillList.AddRange(brief.Nodes[j].Skills);
                            bool isRecommend = CheckRoleSkillIsRecommend(node.barId);
                            if (isRecommend)
                                data.barNodeList.Insert(0, node);
                            else
                                nodeList.Add(node);
                        }
                    }
                    nodeList.Sort((a, b) => { return (int)(a.barId - b.barId); });
                    data.barNodeList.AddRange(nodeList);
                    nodeList.Clear();
                }
            }
            else
            {
                TeamMem teamMem = Sys_Team.Instance.teamMems.Find(o => o.MemId == brief.Roleid);
                if (teamMem != null)
                {
                    data = new TrialTeamRoleData();
                    data.roleId = brief.Roleid;
                    data.teamMem = teamMem;
                    if (brief.Nodes != null && brief.Nodes.Count > 0)
                    {
                        List<TrialTeamRoleData.BarNode> nodeList = new List<TrialTeamRoleData.BarNode>();
                        for (int j = 0; j < brief.Nodes.Count; j++)
                        {
                            TrialTeamRoleData.BarNode node = new TrialTeamRoleData.BarNode();
                            node.barId = brief.Nodes[j].Nodeid;
                            if (brief.Nodes[j].Skills != null && brief.Nodes[j].Skills.Count > 0)
                                node.skillList.AddRange(brief.Nodes[j].Skills);
                            bool isRecommend = CheckRoleSkillIsRecommend(node.barId);
                            if (isRecommend)
                                data.barNodeList.Insert(0, node);
                            else
                                nodeList.Add(node);
                        }
                        nodeList.Sort((a,b)=> { return (int)(a.barId - b.barId); });
                        data.barNodeList.AddRange(nodeList);
                        nodeList.Clear();
                    }
                    trialTeamRoleDataList.Add(data);
                }
            }
        }
        #endregion

        #region Req
        /// <summary>
        /// 请求解锁技能
        /// </summary>
        /// <param name="skillId"></param>
        public void OnUnlockSkillReq(SkillColumnDeploy.FirstSkillCell firstSkillCell)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(20218, true))
                return;
            if (Sys_Fight.Instance.IsFight())
            {
                LanguageHelper.GetTextContent(3899000041);
                return;
            }
            string str=string.Empty;
            uint rlt = CheckUnlock(firstSkillCell);
            CSVTrialBadge.Data badgeData = CSVTrialBadge.Instance.GetConfData(firstSkillCell.csv_trialPreSkill.badge_type);
            if (rlt == 1)
                str=LanguageHelper.GetTextContent(3899000013, firstSkillCell.csv_trialPreSkill.badge_number.ToString(), LanguageHelper.GetTextContent(badgeData.badge_name));
            else if (rlt == 2)
                str = LanguageHelper.GetTextContent(3899000011, LanguageHelper.GetTextContent(badgeData.badge_name));
            else
                str = LanguageHelper.GetTextContent(3899000012, firstSkillCell.csv_trialPreSkill.badge_number.ToString(), LanguageHelper.GetTextContent(badgeData.badge_name));
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = str;
            PromptBoxParameter.Instance.SetConfirm(true, () => {
                if (rlt != 3)
                {
                    CmdTrialGateUnlockSkillReq req = new CmdTrialGateUnlockSkillReq();
                    req.Skillid = firstSkillCell.skillId;
                    NetClient.Instance.SendMessage((ushort)CmdTrialGate.UnlockSkillReq, req);
                }
                else
                {
                    BadgeData curBadgeData = GeBadgeDataByTid(firstSkillCell.badgeType);
                    List <BadgeData.BossData> bossDataList = curBadgeData.GetCurLevelStageBossData(GetCurLevelGradeId());
                    if (bossDataList != null && bossDataList.Count > 0)
                    {
                        TrialBadgeOpenParam param = new TrialBadgeOpenParam();
                        param.type = 2;
                        //param.Pos = CameraManager.mUICamera.WorldToScreenPoint(btnCheckFrom.GetComponent<RectTransform>().position);
                        param.badgeId = firstSkillCell.badgeType;
                        UIManager.OpenUI(EUIID.UI_TrialBadgeTips, false, param);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000026));
                    }
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 检查解锁条件
        /// </summary>
        /// <param name="firstSkillCell"></param>
        /// <returns></returns>
        private uint CheckUnlock(SkillColumnDeploy.FirstSkillCell firstSkillCell)
        {
            uint rlt;
            long badgeNum = GeBadgeNum(firstSkillCell.badgeType);
            long anyTokenNum = GeBadgeNum(33);
            if (badgeNum >= firstSkillCell.csv_trialPreSkill.badge_number)//徽章足够
            {
                rlt = 1;
            }
            else
            {
                if (badgeNum + anyTokenNum >= firstSkillCell.csv_trialPreSkill.badge_number)//徽章+万能徽章足够
                {
                    rlt = 2;
                }
                else//徽章不足
                {
                    rlt = 3;
                }
            }
            return rlt;
        }
        /// <summary>
        /// 请求设置节点宠物
        /// </summary>
        /// <param name="barId"></param>
        /// <param name="petUid"></param>
        public void OnSetNodePetReq(uint barId,uint petUid)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(20218, true))
                return;
            if (Sys_Fight.Instance.IsFight())
            {
                LanguageHelper.GetTextContent(3899000041);
                return;
            }
            CmdTrialGateSetNodePetReq req = new CmdTrialGateSetNodePetReq();
            req.Nodeid = barId;
            req.Petuid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdTrialGate.SetNodePetReq, req);
        }
        /// <summary>
        /// 请求挑战准备
        /// </summary>
        public void OnChallengeReadyReq()
        {
            uint rlt= ChechIsCanEnterTrial();
            if (rlt == 0)
            {
                CmdTrialGateChallengeReadyReq req = new CmdTrialGateChallengeReadyReq();
                NetClient.Instance.SendMessage((ushort)CmdTrialGate.ChallengeReadyReq, req);
            }
        }
        /// <summary>
        /// 请求查看队员配置
        /// </summary>
        public void OnWatchMemberConfigReq()
        {
            int diff = (int)(teamDeployRefreshTime - TimeManager.GetServerTime());
            if (diff <= 0)
            {
                CmdTrialGateWatchMemberConfigReq req = new CmdTrialGateWatchMemberConfigReq();
                NetClient.Instance.SendMessage((ushort)CmdTrialGate.WatchMemberConfigReq, req);
            }
        }
        /// <summary>
        /// 请求挑战投票
        /// </summary>
        /// <param name="isAgree"></param>
        public void OnChallengeVoteReq(bool isAgree)
        {
            CmdTrialGateChallengeVoteReq req = new CmdTrialGateChallengeVoteReq();
            req.Agree = isAgree;
            NetClient.Instance.SendMessage((ushort)CmdTrialGate.ChallengeVoteReq, req);
        }
        #endregion
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            if (!isLogin)
            {
                isLogin = true;
                curEnterBattleState = EEnterBattleState.Nono;
                curStage = 0;
                lastStage = 0;
                curIsOnTrialGate = false;
                var trialStageDatas = CSVTrialStage.Instance.GetAll();
                for (int i = 0; i < trialStageDatas.Count; i++)
                {
                    TrialStage trialStage = new TrialStage();
                    trialStage.tid = trialStageDatas[i].id;
                    trialStage.csv_trialStage = trialStageDatas[i];
                    if (trialStageDic.ContainsKey(trialStageDatas[i].levelGrade_id))
                        trialStageDic[trialStageDatas[i].levelGrade_id].Add(trialStage);
                    else
                        trialStageDic[trialStageDatas[i].levelGrade_id] = new List<TrialStage>() { trialStage };
                }
                var trialCharacteristicDatas = CSVTrialCharacteristic.Instance.GetAll();
                for (int i = 0; i < trialCharacteristicDatas.Count; i++)
                {
                    if (trialCharacteristicDic.ContainsKey(trialCharacteristicDatas[i].levelGrade_id))
                        trialCharacteristicDic[trialCharacteristicDatas[i].levelGrade_id].Add(trialCharacteristicDatas[i]);
                    else
                        trialCharacteristicDic[trialCharacteristicDatas[i].levelGrade_id] = new List<CSVTrialCharacteristic.Data>() { trialCharacteristicDatas[i] };
                }
                var trialSkillBarDatas = CSVTrialSkillBar.Instance.GetAll();
                var trialPreSkillDatas = CSVTrialPreSkill.Instance.GetAll();
                for (int i = 0; i < trialSkillBarDatas.Count; i++)
                {
                    SkillColumnDeploy petSkillDeploy = new SkillColumnDeploy();
                    petSkillDeploy.petUid = 0;
                    petSkillDeploy.barId = trialSkillBarDatas[i].id;
                    petSkillDeploy.csv_trialSkillBar = trialSkillBarDatas[i];
                    petSkillDeploy.superSkill = new SkillColumnDeploy.SuperSkill()
                    {
                        skillId = trialSkillBarDatas[i].superSkill_id,
                    };
                    for (int j = 0; j < trialPreSkillDatas.Count; j++)
                    {
                        if (trialSkillBarDatas[i].id == trialPreSkillDatas[j].skillBar_id)
                        {
                            SkillColumnDeploy.FirstSkillCell skillCell = new SkillColumnDeploy.FirstSkillCell();
                            skillCell.barId = trialSkillBarDatas[i].id;
                            skillCell.skillId = trialPreSkillDatas[j].id;
                            skillCell.activateState = false;
                            skillCell.badgeType = trialPreSkillDatas[j].badge_type;
                            skillCell.csv_trialPreSkill = trialPreSkillDatas[j];
                            petSkillDeploy.firstSkillCellList.Add(skillCell);
                        }
                    }
                    petSkillDeploy.firstSkillCellList.Sort((a, b) => { return (int)(a.csv_trialPreSkill.sort_id - b.csv_trialPreSkill.sort_id); });
                    skillColumnDeployList.Add(petSkillDeploy);
                }
                skillColumnDeployList.Sort((a, b) => { return (int)(a.csv_trialSkillBar.sort_id - b.csv_trialSkillBar.sort_id); });

                var trialBadgeDatas = CSVTrialBadge.Instance.GetAll();
                for (int i = 0; i < trialBadgeDatas.Count; i++)
                {
                    BadgeData data = new BadgeData();
                    data.csv_trialBadge = trialBadgeDatas[i];
                    data.badgeId = trialBadgeDatas[i].id;
                    for (int j = 0; j < CSVTrialLevelGrade.Instance.Count; j++)
                    {
                        uint levelId = CSVTrialLevelGrade.Instance.GetByIndex(j).id;
                        List<List<uint>> list = new List<List<uint>>();
                        if (levelId == 1)
                            list = trialBadgeDatas[i].firstLevelGradeBoss;
                        else if (levelId == 2)
                            list = trialBadgeDatas[i].secendLevelGradeBoss;
                        else if (levelId == 3)
                            list = trialBadgeDatas[i].thirdLevelGradeBoss;
                        if (list != null && list.Count > 0)
                        {
                            for (int k = 0; k < list.Count; k++)
                            {
                                BadgeData.BossData bossData = new BadgeData.BossData();
                                bossData.bossId = list[k][0];
                                bossData.giveBadgeNum = list[k][1];
                                if (data.badgeBossDic.ContainsKey(levelId))
                                    data.badgeBossDic[levelId].Add(bossData);
                                else
                                    data.badgeBossDic[levelId] = new List<BadgeData.BossData>() { bossData };
                            }
                        }
                    }
                    badgeDataList.Add(data);
                }
            }
        }
        private void ClearData()
        {
            badgeDataList.Clear();
            skillColumnDeployList.Clear();
            petDataCellList.Clear();
            trialStageDic.Clear();
            trialCharacteristicDic.Clear();
            trialTeamRoleDataList.Clear();
            roleTrialFeatureDic.Clear();
            voteRoleList.Clear();
        }
        private void SetDefaultData()
        {
            if (skillColumnDeployList != null && skillColumnDeployList.Count > 0)
            {
                for (int i = 0; i < skillColumnDeployList.Count; i++)
                {
                    skillColumnDeployList[i].ClearData();
                }
            }
            trialTeamRoleDataList.Clear();
            roleTrialFeatureDic.Clear();
            voteRoleList.Clear();
            curStage = 0;
            lastStage = 0;
            curIsOnTrialGate = false;
            curEnterBattleState = EEnterBattleState.Nono;
        }

        #region function
        /// <summary>
        /// 获取活动下次刷新剩余时间
        /// </summary>
        /// <returns></returns>
        public int GetActivityRestTimeDiff()
        {
            return (int)(nextResetTime - TimeManager.GetServerTime());
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
        /// 检查所属等级段
        /// </summary>
        /// <param name="level">level=0(自己的等级，否则为其他人传入的等级)</param>
        /// <param name="checkType">checkType=0(返回目标等级段id) checkType=1(返回目标等级段等级)</param>
        /// <returns></returns>
        public uint CheckLevelGrade(uint level = 0, uint checkType = 0)
        {
            level = level != 0 ? level : Sys_Role.Instance.Role.Level;
            uint value = 0;
            var trialLevelGradeDatas = CSVTrialLevelGrade.Instance.GetAll();
            for (int i = trialLevelGradeDatas.Count - 1; i >= 0; i--)
            {
                if (level >= trialLevelGradeDatas[i].levelGrade)
                {
                    value = checkType == 0 ? trialLevelGradeDatas[i].id : trialLevelGradeDatas[i].levelGrade;
                    break;
                }
            }
            return value;
        }
        /// <summary>
        /// 获取等级段区间
        /// </summary>
        /// <param name="levelId">等级段id</param>
        /// <returns></returns>
        private uint[] GetLevelSection(uint levelId)
        {
            uint[] section = new uint[2];
            CSVTrialLevelGrade.Data levelData = CSVTrialLevelGrade.Instance.GetConfData(levelId);
            CSVTrialLevelGrade.Data nextLevelData = CSVTrialLevelGrade.Instance.GetConfData(levelId + 1);
            if (levelData != null)
            {
                section[0] = levelData.levelGrade;
                if (nextLevelData != null)
                    section[1] = nextLevelData.levelGrade - 1;
                else
                    section[1] = 99;
            }
            return section;
        }
        /// <summary>
        /// 获取当前等级段数据
        /// </summary>
        /// <returns></returns>
        public CSVTrialLevelGrade.Data GetTrialLevelGrade()
        {
            return CSVTrialLevelGrade.Instance.GetConfData(GetCurLevelGradeId());
        }
        /// <summary>
        /// 获取等级段对应的阶段数据
        /// </summary>
        /// <param name="levelId">levelId=0是自己的，否则为他人的</param>
        /// <returns></returns>
        public List<TrialStage> GetTrialStageList(uint levelId = 0)
        {
            levelId = levelId != 0 ? levelId : CheckLevelGrade();
            if (trialStageDic.ContainsKey(levelId))
                return trialStageDic[levelId];
            return null;
        }
        /// <summary>
        /// 获取当前等级段对应的试炼阶段总数
        /// </summary>
        /// <returns></returns>
        public int GetAllTrialStageNum()
        {
            List<TrialStage> dataList = GetTrialStageList();
            if (dataList != null)
                return dataList.Count;
            return 0;
        }
        /// <summary>
        /// 获取本等级段的所有Boss特性数据
        /// </summary>
        /// <param name="levelId">等级段id</param>
        /// <returns></returns>
        public List<CSVTrialCharacteristic.Data> GetTrialCharacteristicList(uint levelId = 0)
        {
            levelId = levelId != 0 ? levelId : CheckLevelGrade();
            if (trialCharacteristicDic.ContainsKey(levelId))
                return trialCharacteristicDic[levelId];
            return null;
        }
        /// <summary>
        /// 根据徽章类型获取徽章数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BadgeData GeBadgeDataByTid(uint id)
        {
            return badgeDataList.Find(o => o.badgeId == id);
        }
        /// <summary>
        /// 获取徽章对应的数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public long GeBadgeNum(uint id)
        {
            return Sys_Bag.Instance.GetCurrencyCount(id);
        }
        /// <summary>
        /// 获取本周试炼特性数据
        /// </summary>
        /// <returns></returns>
        public CSVTrialCharacteristic.Data GetTrialCharacteristic()
        {
            CSVTrialCharacteristic.Data data= CSVTrialCharacteristic.Instance.GetConfData(curTrialFeatureTid);
            if (data == null)
                DebugUtil.LogError("CSVTrialCharacteristic not found id :"+ curTrialFeatureTid);
            return data;
        }
        /// <summary>
        /// 检测是否可以进入试炼战斗
        /// </summary>
        /// <returns></returns>
        public uint ChechIsCanEnterTrial()
        {
            uint rlt = 0;
            uint teamCount = CSVTrialParameters.Instance.GetConfData(1).value;
            List<string> teamRoleNameList = new List<string>();
            uint[] levelSection = null;
            if (!Sys_FunctionOpen.Instance.IsOpen(20218, true))
                return 6;
            if (GetActivityRestTimeDiff() <= 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000039));
                return 7;
            }
            //在试炼之门地图
            if (Sys_Map.Instance.CurMapId == trialMapId)
            {
                if (Sys_Team.Instance.HaveTeam)
                {
                    if (Sys_Team.Instance.TeamMemsCount >= teamCount)
                    {
                        int count = Sys_Team.Instance.teamMems.Count;
                        TeamMem captainData = Sys_Team.Instance.teamMems.Find(o => Sys_Team.Instance.isCaptain(o.MemId));
                        //uint levelTarget = CheckLevelGrade(captainData.Level, 1);//队伍目标等级
                        levelSection = GetLevelSection(CheckLevelGrade(captainData.Level));
                        for (int i = 0; i < count; i++)
                        {
                            TeamMem teamMem = Sys_Team.Instance.teamMems[i];
                            if (!TeamMemHelper.IsLeave(teamMem) && !TeamMemHelper.IsOffLine(teamMem))//没有离队、离线
                            {
                                if (!(teamMem.Level >= levelSection[0] && teamMem.Level <= levelSection[1]))
                                {
                                    rlt = 4;
                                    teamRoleNameList.Add(teamMem.Name.ToStringUtf8());
                                }
                            }
                            else
                            {
                                rlt = 3;
                                break;
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
            switch (rlt)
            {
                case 1://当前没有队伍
                    str = LanguageHelper.GetTextContent(101811);
                    break;
                case 2://队伍人数不足
                    str = LanguageHelper.GetTextContent(3899000005, teamCount.ToString());
                    break;
                case 3://有队员离线或者暂离
                    str = LanguageHelper.GetTextContent(3899000003);
                    break;
                case 4://有队员等级不满足
                    if (teamRoleNameList != null && teamRoleNameList.Count > 0)
                    {
                        System.Text.StringBuilder nameStr = new System.Text.StringBuilder();
                        for (int i = 0; i < teamRoleNameList.Count; i++)
                        {
                            nameStr.Append(teamRoleNameList[i]);
                            if (i < teamRoleNameList.Count - 1)
                            {
                                nameStr.Append("、");
                            }
                        }
                        str = LanguageHelper.GetTextContent(3899000004, levelSection[0].ToString(), levelSection[1].ToString(), nameStr.ToString());
                    }
                    break;
                case 5://是否在试炼之门的地图中
                    str = LanguageHelper.GetTextContent(3899000034);
                    break;
            }
            if (str != string.Empty)
                Sys_Hint.Instance.PushContent_Normal(str);
            if (teamRoleNameList != null && teamRoleNameList.Count > 0)
                teamRoleNameList.Clear();
            return rlt;
        }
        /// <summary>
        /// 获取宠物数据
        /// </summary>
        List<PetDataCell> petDataCellList = new List<PetDataCell>();
        public List<PetDataCell> GetPetList()
        {
            petDataCellList.Clear();
            List<ClientPet> petList = Sys_Pet.Instance.petsList;
            List<SkillColumnDeploy> deployList = GetSkillColumnDeployList(2);
            if (deployList != null && deployList.Count > 0)
            {
                for (int i = 0; i < deployList.Count; i++)
                {
                    ClientPet clientPet = petList.Find(o => o.petUnit.Uid == deployList[i].petUid);
                    if (clientPet != null)
                    {
                        PetDataCell cell = new PetDataCell();
                        cell.barId = deployList[i].barId;
                        cell.clientPet = clientPet;
                        petDataCellList.Add(cell);
                    }
                }
            }
            if (petList != null && petList.Count > 0)
            {
                List<PetDataCell> list = new List<PetDataCell>();
                int petCount = petList.Count;
                for (int i = 0; i < petCount; i++)
                {
                    PetDataCell petDataCell = petDataCellList.Find(o => o.clientPet.petUnit.Uid == petList[i].petUnit.Uid);
                    if (petDataCell == null)
                    {
                        PetDataCell cell = new PetDataCell();
                        cell.barId = 0;
                        cell.clientPet = petList[i];
                        list.Add(cell);
                    }
                }
                //未装配按宠物评分进行排序
                list.Sort((a, b) =>
                {
                    return (int)(b.clientPet.petUnit.SimpleInfo.Score - a.clientPet.petUnit.SimpleInfo.Score);
                });
                petDataCellList.AddRange(list);
                list.Clear();
            }
            return petDataCellList;
        }
        public ClientPet GetClientPet(uint petUid)
        {
            return Sys_Pet.Instance.petsList.Find(o => o.petUnit.Uid == petUid);
        }
        public SkillColumnDeploy GetSkillColumnDeployByBarId(uint barId)
        {
            return skillColumnDeployList.Find(o => o.barId == barId);
        }
        /// <summary>
        /// 获取技能栏位对应的数据，推荐技能默认排在第一位
        /// </summary>
        /// <param name="type">type=0(全部) type=1(仅激活的超级技能(包括部分激活)) type=2(仅装配宠物)</param>
        /// <returns></returns>
        public List<SkillColumnDeploy> GetSkillColumnDeployList(uint type=0)
        {
            List<SkillColumnDeploy> list = new List<SkillColumnDeploy>();
            for (int i = 0; i < skillColumnDeployList.Count; i++)
            {
                if (type == 0)
                {
                    if (skillColumnDeployList[i].CheckSuperSkillIsRecommend())
                    {
                        list.Insert(0, skillColumnDeployList[i]);
                    }
                    else
                    {
                        list.Add(skillColumnDeployList[i]);
                    }
                }
                else if (type == 1)
                {
                    if (skillColumnDeployList[i].CheckBarIsActivate())
                    {
                        if (skillColumnDeployList[i].CheckSuperSkillIsRecommend())
                        {
                            list.Insert(0, skillColumnDeployList[i]);
                        }
                        else
                        {
                            list.Add(skillColumnDeployList[i]);
                        }
                    }
                }
                else if (type == 2)
                {
                    if (skillColumnDeployList[i].petUid != 0)
                    {
                        if (skillColumnDeployList[i].CheckSuperSkillIsRecommend())
                        {
                            list.Insert(0, skillColumnDeployList[i]);
                        }
                        else
                        {
                            list.Add(skillColumnDeployList[i]);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 检查角色超级技能是否为本周推荐
        /// </summary>
        /// <returns></returns>
        public bool CheckRoleSkillIsRecommend(uint barId)
        {
            CSVTrialSkillBar.Data csvData= CSVTrialSkillBar.Instance.GetConfData(barId);
            if (csvData != null && csvData.characteristic_id != null && csvData.characteristic_id.Count > 0)
            {
                for (int i = 0; i < csvData.characteristic_id.Count; i++)
                {
                    if (csvData.characteristic_id[i] == curTrialFeatureTid)
                        return true;
                }
            }
            return false;
        }
        public RoleTrialFeature GetRoleTrialFeature(uint id)
        {
            if (roleTrialFeatureDic.ContainsKey(id))
                return roleTrialFeatureDic[id];
            return null;
        }
        /// <summary>
        /// 是否有可激活的前置技能红点
        /// </summary>
        /// <returns></returns>
        public bool CheckRedPoint()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(20218))
                return false;
            for (int i = 0; i < skillColumnDeployList.Count; i++)
            {
                if (skillColumnDeployList[i].CheckFirstSkillIsCanActivate())
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查该宠物是否装配
        /// </summary>
        /// <param name="petUid"></param>
        /// <returns></returns>
        public bool CheckPetIsDeploy(uint petUid)
        {
            for (int i = 0; i < skillColumnDeployList.Count; i++)
            {
                if (skillColumnDeployList[i].petUid != 0)
                {
                    if (skillColumnDeployList[i].petUid == petUid)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取队伍配置界面
        /// </summary>
        /// <returns></returns>
        public List<TrialTeamRoleData> GetTrialTeamRoleDataList()
        {
            if (Sys_Team.Instance.HaveTeam)
            {
                List<SkillColumnDeploy> skillColumnDeployList = GetSkillColumnDeployList(2);
                TrialTeamRoleData data = trialTeamRoleDataList.Find(o => o.roleId == Sys_Role.Instance.RoleId);
                if (data != null)
                {
                    for (int i = 0; i < skillColumnDeployList.Count; i++)
                    {
                        TrialTeamRoleData.BarNode node = data.barNodeList.Find(o => o.barId == skillColumnDeployList[i].barId);
                        if (node != null)
                        {
                            node.skillList.Clear();
                            if (skillColumnDeployList[i].firstSkillCellList != null && skillColumnDeployList[i].firstSkillCellList.Count > 0)
                            {
                                for (int j = 0; j < skillColumnDeployList[i].firstSkillCellList.Count; j++)
                                {
                                    if (skillColumnDeployList[i].firstSkillCellList[j].activateState)
                                        node.skillList.Add(skillColumnDeployList[i].firstSkillCellList[j].skillId);
                                }
                            }
                        }
                        else
                        {
                            node = new TrialTeamRoleData.BarNode();
                            node.barId = skillColumnDeployList[i].barId;
                            for (int j = 0; j < skillColumnDeployList[i].firstSkillCellList.Count; j++)
                            {
                                if (skillColumnDeployList[i].firstSkillCellList[j].activateState)
                                    node.skillList.Add(skillColumnDeployList[i].firstSkillCellList[j].skillId);
                            }
                            data.barNodeList.Add(node);
                        }
                    }
                }
                else
                {
                    TeamMem teamMem = Sys_Team.Instance.teamMems.Find(o => o.MemId == Sys_Role.Instance.RoleId);
                    int teamIndex = Sys_Team.Instance.teamMems.IndexOf(teamMem);
                    bool isCaptain = Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId);
                    if (teamMem != null)
                    {
                        data = new TrialTeamRoleData();
                        data.roleId = Sys_Role.Instance.RoleId;
                        data.teamMem = teamMem;
                        for (int i = 0; i < skillColumnDeployList.Count; i++)
                        {
                            TrialTeamRoleData.BarNode node = new TrialTeamRoleData.BarNode();
                            node.barId = skillColumnDeployList[i].barId;
                            if (skillColumnDeployList[i].firstSkillCellList != null && skillColumnDeployList[i].firstSkillCellList.Count > 0)
                            {
                                for (int j = 0; j < skillColumnDeployList[i].firstSkillCellList.Count; j++)
                                {
                                    if (skillColumnDeployList[i].firstSkillCellList[j].activateState)
                                        node.skillList.Add(skillColumnDeployList[i].firstSkillCellList[j].skillId);
                                }
                            }
                            data.barNodeList.Add(node);
                        }
                        if (isCaptain)
                            trialTeamRoleDataList.Insert(0, data);
                        else
                            trialTeamRoleDataList.Insert(teamIndex, data);
                    }
                }
            }
            return trialTeamRoleDataList;
        }
        /// <summary>
        /// 检查玩家是否投票(同意)
        /// </summary>
        /// <param name="roldId"></param>
        /// <returns></returns>
        public bool CheckIsVote(ulong roldId)
        {
            if (voteRoleList != null && voteRoleList.Count > 0)
            {
                for (int i = 0; i < voteRoleList.Count; i++)
                {
                    if (roldId == voteRoleList[i])
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检查所有阶段是否完成
        /// </summary>
        /// <returns></returns>
        public bool CheckStageIsAllFinished()
        {
            List<TrialStage> dataList= GetTrialStageList();
            return curStage >= dataList.Count;
        }
        #endregion
    }
    #region classData
    /// <summary>
    /// 宠物出战技能栏配置
    /// </summary>
    public class SkillColumnDeploy
    {
        public class SkillBase
        {
            public uint skillId;         //技能id
        }
        /// <summary>
        /// 前置技能
        /// </summary>
        public class FirstSkillCell : SkillBase
        {
            public uint barId;
            public CSVTrialPreSkill.Data csv_trialPreSkill;
            public uint badgeType; //对应徽章类型id
            public bool activateState;   //激活状态

            /// <summary>
            /// 检查是否可以激活
            /// </summary>
            /// <returns></returns>
            public bool CheckIsCanActivate()
            {
                long haveNum = Sys_Bag.Instance.GetCurrencyCount(badgeType);
                long anyTokenNum = Sys_Bag.Instance.GetCurrencyCount(33);
                long totalNum = haveNum + anyTokenNum;
                if (totalNum >= csv_trialPreSkill.badge_number && !activateState)//未激活且徽章足够(本体徽章+万能徽章)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 超级技能
        /// </summary>
        public class SuperSkill : SkillBase
        {

        }

        public uint petUid;
        public uint barId;
        public CSVTrialSkillBar.Data csv_trialSkillBar;
        public SuperSkill superSkill;
        public List<FirstSkillCell> firstSkillCellList = new List<FirstSkillCell>();
        /// <summary>
        /// 获取已激活前置技能的个数
        /// </summary>
        /// <returns></returns>
        public uint GetFirstSkillActivateCount()
        {
            uint count = 0;
            if (firstSkillCellList != null && firstSkillCellList.Count > 0)
            {
                for (int i = 0; i < firstSkillCellList.Count; i++)
                {
                    if (firstSkillCellList[i].activateState)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public FirstSkillCell GetFirstSkillCellBySkillId(uint skillId)
        {
            return firstSkillCellList.Find(o => o.skillId == skillId);
        }
        public float GetFirstSkillActivateRatio()
        {
            if (firstSkillCellList != null && firstSkillCellList.Count > 0)
            {
                return 1 - (float)Math.Round((double)GetFirstSkillActivateCount() / firstSkillCellList.Count, 3);
            }
            return 0;
        }
        /// <summary>
        /// 检查超级技能是否为推荐技能 由本周boss特性决定
        /// </summary>
        /// <returns></returns>
        public bool CheckSuperSkillIsRecommend()
        {
            if (csv_trialSkillBar != null && csv_trialSkillBar.characteristic_id != null && csv_trialSkillBar.characteristic_id.Count > 0)
            {
                if (csv_trialSkillBar.characteristic_id.Contains(Sys_ActivityTrialGate.Instance.curTrialFeatureTid))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 检查超级技能是否激活，需要所有后置技能全部激活
        /// </summary>
        /// <returns></returns>
        public bool CheckSuperSkillIsActivate()
        {
            return GetFirstSkillActivateCount() >= firstSkillCellList.Count;
        }
        /// <summary>
        /// 检查宠物技能栏是否解锁，有一个前置技能激活即为解锁
        /// </summary>
        /// <returns></returns>
        public bool CheckBarIsActivate()
        {
            return GetFirstSkillActivateCount() > 0;
        }
        /// <summary>
        /// 检查是否有可激活的前置技能
        /// </summary>
        /// <returns></returns>
        public bool CheckFirstSkillIsCanActivate()
        {
            if (firstSkillCellList != null && firstSkillCellList.Count > 0)
            {
                for (int i = 0; i < firstSkillCellList.Count; i++)
                {
                    if (firstSkillCellList[i].CheckIsCanActivate())
                        return true;
                }
            }
            return false;
        }
        public void ClearData()
        {
            petUid = 0;
            if (firstSkillCellList != null && firstSkillCellList.Count > 0)
            {
                for (int i = 0; i < firstSkillCellList.Count; i++)
                {
                    firstSkillCellList[i].activateState = false;
                }
            }
        }
    }
    /// <summary>
    /// 徽章数据
    /// </summary>
    public class BadgeData
    {
        //徽章对应的Boss数据
        public class BossData
        {
            public uint bossId;
            public uint giveBadgeNum;
        }
        public uint badgeId;
        public CSVTrialBadge.Data csv_trialBadge;
        public Dictionary<uint, List<BossData>> badgeBossDic = new Dictionary<uint, List<BossData>>();

        /// <summary>
        /// 获取当前等级段对应的Boss数据
        /// </summary>
        /// <returns></returns>
        public List<BossData> GetCurLevelStageBossData(uint stage)
        {
            if (badgeBossDic.ContainsKey(stage))
                return badgeBossDic[stage];
            return null;
        }
    }
    /// <summary>
    /// 试炼奖励
    /// </summary>
    public class TrialStage
    {
        public uint tid;
        public CSVTrialStage.Data csv_trialStage;//表格数据
    }
    public class PetDataCell
    {
        public uint barId;
        public ClientPet clientPet;
    }
    /// <summary>
    /// 玩家自身试炼不同特性对应数据
    /// </summary>
    public class RoleTrialFeature
    {
        public uint featureId;//特性id
        public uint maxStage;//最大阶段
        public uint minRound;//最小回合
        public uint timestamp;//时间
    }
    public class TrialTeamRoleData
    {
        public class BarNode
        {
            public uint barId;
            public List<uint> skillList = new List<uint>();
        }
        public ulong roleId;
        public TeamMem teamMem;
        public List<BarNode> barNodeList = new List<BarNode>();
    }
    #endregion
}