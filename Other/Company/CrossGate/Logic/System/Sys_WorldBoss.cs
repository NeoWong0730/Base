using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {
    public class BossManualData {
        public uint id;

        private CSVBOSSManual.Data _csv;

        public CSVBOSSManual.Data csv {
            get {
                if (this._csv == null) {
                    this._csv = CSVBOSSManual.Instance.GetConfData(this.id);
                }

                return this._csv;
            }
        }

        // 解锁礼包是否解锁
        public EGiftStatus unlockRewardStatus { get; private set; }

        // 首杀礼包是否解锁
        public EGiftStatus firstSkillRewardStatus { get; private set; }

        // 传记等级，逐次开放，不能随机开放
        public int storyLevel { get; private set; }
        private bool hasBuildReward = false;
        public List<EGiftStatus> storyRewardStatus = new List<EGiftStatus>();

        public bool HasCardRewardUngot(int index) {
            bool rlt = false;
            if (0 <= index && index < this.storyRewardStatus.Count) {
                rlt |= (this.storyRewardStatus[index] == EGiftStatus.UnGot);
            }

            return rlt;
        }

        public bool HasRewardUnGot {
            get {
                bool rlt = false;
                rlt = this.unlockRewardStatus == EGiftStatus.UnGot;
                rlt |= (this.firstSkillRewardStatus == EGiftStatus.UnGot);
                for (int i = 0; i < this.storyRewardStatus.Count; i++) {
                    rlt |= (this.storyRewardStatus[i] == EGiftStatus.UnGot);
                    if (rlt) {
                        return rlt;
                    }
                }

                return rlt;
            }
        }

        public uint campId {
            get { return this.csv.camp_id; }
        }

        // 所屬陣營是否解锁
        public bool IsCampUnlocked {
            get { return Sys_WorldBoss.Instance.unlockedCamps.ContainsKey(this.campId); }
        }

        public void Refresh(BossInfo info, bool useTip = false) {
            this.unlockRewardStatus = (EGiftStatus) info.Unlock;
            this.firstSkillRewardStatus = (EGiftStatus) info.FirstDrop;

            if (!this.hasBuildReward) {
                this.storyRewardStatus.Clear();
                for (int i = 0; i < this.csv.biography.Count; i++) {
                    this.storyRewardStatus.Add(EGiftStatus.Locked);
                }

                this.hasBuildReward = true;
            }

            int min = (int) Math.Min(this.csv.biography.Count, info.Story);
            if (min != this.storyLevel) {
                if (useTip) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4157000016));
                }

                this.storyLevel = min;
            }

            for (int i = 0; i < min; ++i) {
                this.storyRewardStatus[i] = EGiftStatus.UnGot;
            }

            for (int i = 0, length = info.StoryReward.Count; i < length; ++i) {
                int index = (int) info.StoryReward[i] - 1;
                this.storyRewardStatus[index] = EGiftStatus.Got;
            }
        }

        public BossManualData(uint id) {
            this.id = id;
        }
    }

    public class BossCamp {
        public uint id;

        private CSVCampInformation.Data _csv;

        public CSVCampInformation.Data csv {
            get {
                if (this._csv == null) {
                    this._csv = CSVCampInformation.Instance.GetConfData(this.id);
                }

                return this._csv;
            }
        }

        public EGiftStatus rewardStatus { get; set; } = EGiftStatus.UnGot;

        public bool GotReward {
            get { return this.rewardStatus == EGiftStatus.Got; }
        }

        private List<uint> _allManualList = null;

        public List<uint> AllManualList {
            get {
                if (this._allManualList == null) {
                    this._allManualList = new List<uint>();
                    foreach (var kvp in CSVBOSSManual.Instance.GetAll()) {
                        if (this.id == kvp.camp_id) {
                            this._allManualList.Add(kvp.id);
                        }
                    }
                }

                return this._allManualList;
            }
        }

        public bool HasRewardUngot {
            get {
                bool rlt = false;
                rlt |= (this.rewardStatus == EGiftStatus.UnGot);
                if (rlt) {
                    return rlt;
                }

                for (int i = 0; i < this.AllManualList.Count; i++) {
                    if (Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue(this.AllManualList[i], out var bossManualData)) {
                        rlt |= bossManualData.HasRewardUnGot;
                        if (rlt) {
                            return rlt;
                        }
                    }
                }

                return rlt;
            }
        }

        public uint coreBossId {
            get { return this.csv.coreBoss_id; }
        }

        public BossCamp(uint id) {
            this.id = id;
        }

        public void Refresh(EGiftStatus rewardStatus) {
            this.rewardStatus = rewardStatus;
        }
    }

    public class BossPlayMode {
        public uint id;

        private CSVBOOSFightPlayMode.Data _csv;

        public CSVBOOSFightPlayMode.Data csv {
            get {
                if (this._csv == null) {
                    this._csv = CSVBOOSFightPlayMode.Instance.GetConfData(this.id);
                }

                return this._csv;
            }
        }

        public Timer dailyTimer;
        public Timer weeklyTimer;

        public uint dailyUsedCount;
        public uint weeklyUsedCount;

        public BossPlayMode(uint id) {
            this.id = id;
        }

        public void Refresh(RewardData reward) {
            this.dailyUsedCount = reward.DayLimit.PickTimes;
            this.weeklyUsedCount = reward.WeekLimit.PickTimes;

            ulong now = Sys_Time.Instance.GetServerTime();
            // 一周刷新次数
            // timer充当update使用
            this.dailyTimer?.Cancel();
            this.dailyTimer = Timer.Register(reward.DayLimit.ResetTime - now, () => { this.dailyUsedCount = 0; });

            this.weeklyTimer?.Cancel();
            this.weeklyTimer = Timer.Register(reward.WeekLimit.ResetTime - now, () => { this.weeklyUsedCount = 0; });
        }
    }

    // 世界boss系统
    public class Sys_WorldBoss : SystemModuleBase<Sys_WorldBoss> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents {
            OnBossStarLevelChanged, // 星级变化
            OnBossInfoChanged, // boss信息变化
            OnRewardGot, // 奖励领取
            OnBossManualUnlocked, // boss解锁
            OnCampUnlocked, // 阵营解锁
            OnBossCountGot, // boss个数获取
        }
        
        public class Rank {
            public long cd;
            public RankQueryWildBossFirstKill firstkillRecord;
            public RankQueryWildBossSelf mykillRecord;
            public CmdRankQueryRes rankList; 
        }
        public Dictionary<uint, Rank> cds = new Dictionary<uint, Rank>();

        // 所有的camp
        private List<uint> _allCamps;

        public List<uint> allCamps {
            get {
                if (this._allCamps == null) {
                    this._allCamps = new List<uint>();
                    foreach (var kvp in CSVCampInformation.Instance.GetAll()) {
                        this._allCamps.Add(kvp.id);
                    }
                }

                return this._allCamps;
            }
        }

        // 所有解锁的camp
        public Dictionary<uint, BossCamp> unlockedCamps = new Dictionary<uint, BossCamp>();

        // 所有解锁的bossManual, manualId:info
        public Dictionary<uint, BossManualData> unlockedBossManuales = new Dictionary<uint, BossManualData>();
        public Dictionary<uint, BossPlayMode> usedCount = new Dictionary<uint, BossPlayMode>();

        public override void Init() {
            EventDispatcher.Instance.AddEventListener((ushort) WildNPC.CmdWildBossInfoReq, (ushort) WildNPC.CmdWildBossInfoRes, this.OnAllBossReceived, CmdWildBossInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) WildNPC.CmdWildBossInfoNtf, this.OnRefreshOneBossManual, CmdWildBossInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) WildNPC.CmdWildBossUnlockReq, (ushort) WildNPC.CmdWildBossUnlockRes, this.OnBossManualUnlocked, CmdWildBossUnlockRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) WildNPC.CmdWildBossRewardPickReq, (ushort) WildNPC.CmdWildBossRewardPickRes, this.OnRewardGot, CmdWildBossRewardPickRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) WildNPC.CmdWildBossApplyReq, (ushort) WildNPC.CmdWildBossApplyRes, this.OnSignUp, CmdWildBossApplyRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) WildNPC.CmdWildBossMapInfoReq, (ushort) WildNPC.CmdWildBossMapInfoRes, this.OnBossCountReceived, CmdWildBossMapInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) WildNPC.CmdWildBossApplyNtf, this.OnBossApplyNtf, CmdWildBossApplyNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) WildNPC.CmdWildBossRewardDataNtf, this.OnBossRewardDataNtf, CmdWildBossRewardDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdNpc.BossBattleFailNtf, this.OnBossBattleFailNtf, CmdNpcBossBattleFailNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) WildNPC.CmdWildBossBattleIdNtf, this.OnCmdWildBossBattleIdNtf, CmdWildBossBattleIdNtf.Parser);

            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnBattleSettlement, this.OnEndBattle, true);
        }

        public override void OnSyncFinished() {
            // 请求 世界boss信息
            Sys_WorldBoss.Instance.ReqAllBoss();

            this.pendingRequests.Clear();
            this.cds.Clear();

            // 客户端 测试数据
            //RefreshCamps(new List<uint>() { 1, 3, 5, 7 });

            //List<uint> ls = new List<uint>() { 101781, 104782, 401785 };
            //foreach (var item in ls) {
            //    BossInfo bi = new BossInfo();
            //    bi.BossId = item;
            //    bi.Unlock = 0;
            //    bi.FirstDrop = 1;
            //    bi.Story = 2;

            //    for (int i = 0; i < bi.Story; i++) {
            //        bi.StoryReward.Add((uint)i);
            //    }

            //    AddBoss(item, bi);
            //}

            //Sys_Role.Instance.Role.Level = 56;

            unlockedCamps.Clear();
            unlockedBossManuales.Clear();
            usedCount.Clear();
        }

        #region 数据处理

        public void RefreshCamps(IList<uint> campIds) {
            this.unlockedCamps.Clear();
            for (int i = 0, length = campIds.Count; i < length; ++i) {
                BossCamp camp = new BossCamp(campIds[i]);
                this.unlockedCamps.Add(campIds[i], camp);
            }
        }

        public BossCamp AddCamp(uint campId) {
            if (!this.unlockedCamps.TryGetValue(campId, out BossCamp camp)) {
                camp = new BossCamp(campId);
                this.unlockedCamps.Add(campId, camp);
            }

            return camp;
        }

        public void RefreshBosseManuals(IList<BossInfo> infos) {
            this.unlockedBossManuales.Clear();
            for (int i = 0, length = infos.Count; i < length; ++i) {
                if (infos[i].ManualId == 0) {
                    continue;
                }

                BossManualData data = new BossManualData(infos[i].ManualId);
                this.unlockedBossManuales.Add(infos[i].ManualId, data);

                data.Refresh(infos[i]);
            }
        }

        public BossManualData AddBossManual(uint manualId, BossInfo info) {
            if (!this.unlockedBossManuales.TryGetValue(manualId, out BossManualData data)) {
                data = new BossManualData(manualId);
                this.unlockedBossManuales.Add(manualId, data);
            }

            data.Refresh(info);
            return data;
        }

        public bool IsUnlockedBossManual(uint manualId) {
            return this.unlockedBossManuales.TryGetValue(manualId, out var _);
        }

        public bool TryGetActivityIdByDailyId(uint dailyId, out uint actitityId) {
            actitityId = 0;
            foreach (var kvp in CSVBOOSFightPlayMode.Instance.GetAll()) {
                if (kvp.dailyActivites == dailyId) {
                    actitityId = kvp.id;
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 协议

        public void ReqAllBoss() {
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqAllBoss");

            CmdWildBossInfoReq req = new CmdWildBossInfoReq();
            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossInfoReq, req);
        }

        private void OnAllBossReceived(NetMsg msg) {
            CmdWildBossInfoRes response = NetMsgUtil.Deserialize<CmdWildBossInfoRes>(CmdWildBossInfoRes.Parser, msg);
            var data = response.Data;

            this.RefreshBosseManuals(data.Bosses);

            // 构建阵营
            for (int i = 0, length = data.Bosses.Count; i < length; ++i) {
                uint manualId = data.Bosses[i].ManualId;
                if (manualId == 0) {
                    continue;
                }

                uint campId = this.unlockedBossManuales[manualId].campId;
                this.AddCamp(campId);
            }

            // 刷新阵营奖励状态
            for (int i = 0, length = data.CampReward.Count; i < length; ++i) {
                uint campId = data.CampReward[i];
                this.unlockedCamps[campId].Refresh(EGiftStatus.Got);
            }

            // 刷新各种玩法次数
            this.usedCount.Clear();
            for (int i = 0, length = data.RewardData.Count; i < length; ++i) {
                BossPlayMode bpm = new BossPlayMode(data.RewardData[i].PlayType);
                bpm.Refresh(data.RewardData[i]);

                this.usedCount.Add(data.RewardData[i].PlayType, bpm);
            }

            DebugUtil.Log(ELogType.eWorldBoss, "OnAllBossReceived");
        }

        // 更新单个boss的信息
        private void OnRefreshOneBossManual(NetMsg msg) {
            CmdWildBossInfoNtf response = NetMsgUtil.Deserialize<CmdWildBossInfoNtf>(CmdWildBossInfoNtf.Parser, msg);
            BossInfo bmi = response.Data;
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnRefreshOneBoss: {0}", bmi.ManualId);
            if (this.unlockedBossManuales.TryGetValue(bmi.ManualId, out BossManualData bd)) {
                bd.Refresh(bmi, true);
            }
        }

        private readonly List<uint> pendingRequests = new List<uint>();

        // 解锁bossManualId
        public void ReqUnlockBossManual(uint manualId) {
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqUnlockBoss: {0}", manualId);

            if (this.pendingRequests.Contains(manualId)) {
                return;
            }

            this.pendingRequests.Add(manualId);

            CmdWildBossUnlockReq req = new CmdWildBossUnlockReq();
            req.ManualId = manualId;
            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossUnlockReq, req);
        }

        private void OnBossManualUnlocked(NetMsg msg) {
            CmdWildBossUnlockRes response = NetMsgUtil.Deserialize<CmdWildBossUnlockRes>(CmdWildBossUnlockRes.Parser, msg);
            BossInfo bi = response.BossInfo;

            this.pendingRequests.Remove(bi.ManualId);

            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnBossUnlocked: {0}", bi.ManualId);
            if (!this.unlockedBossManuales.TryGetValue(bi.ManualId, out BossManualData bd)) {
                bd = this.AddBossManual(bi.ManualId, bi);

                this.eventEmitter.Trigger<uint>(EEvents.OnBossManualUnlocked, bi.ManualId);
                bool isCampUnlocked = bd.IsCampUnlocked;
                uint campId = 0;
                if (!isCampUnlocked) {
                    campId = bd.campId;
                    this.AddCamp(campId);

                    this.eventEmitter.Trigger<uint>(EEvents.OnCampUnlocked, campId);
                }

                UIManager.OpenUI(EUIID.UI_WorldBossUnlockCampOrBoss, false, new Tuple<uint, uint>(campId, bi.ManualId));
            }
        }

        public void ReqReward(uint manualId, int rewardType, int index = 0) {
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqReward: {0} {1} {2}", manualId, rewardType, index);

            CmdWildBossRewardPickReq req = new CmdWildBossRewardPickReq();
            req.ManualId = manualId;
            req.RewardType = (uint) rewardType;
            req.Index = (uint) index;
            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossRewardPickReq, req);
        }

        private void OnRewardGot(NetMsg msg) {
            CmdWildBossRewardPickRes response = NetMsgUtil.Deserialize<CmdWildBossRewardPickRes>(CmdWildBossRewardPickRes.Parser, msg);
            BossInfo bi = response.BossInfo;
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnRewardGot: {0}", bi.ManualId);

            if (this.unlockedBossManuales.TryGetValue(bi.ManualId, out BossManualData bd)) {
                if (response.RewardType != 4) {
                    bd.Refresh(bi);
                }
                else {
                    // 阵营奖励
                    if (response.RewardType == 4 && this.unlockedCamps.TryGetValue(bd.campId, out BossCamp bc)) {
                        bc.Refresh(EGiftStatus.Got);
                    }
                }

                this.eventEmitter.Trigger(EEvents.OnRewardGot);
            }
        }

        // 报名/取消报名 0报名 1取消
        // 报名/取消报名/选中结果/首杀奖励通知/活动次数  都需要给每一个队员发送
        public void ReqSignUp(uint bossId, ulong teamId, ulong roleId, uint signOrCancel, ulong bossGuid) {
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqSignUp: bossId-{0} teamId-{1} roleId-{2} {3} bossGuid:{4}", bossId, teamId, roleId, signOrCancel, bossGuid);

            CmdWildBossApplyReq req = new CmdWildBossApplyReq();
            req.BossId = bossId;
            req.TeamId = (uint) teamId;
            req.RoleId = roleId;
            req.Cancel = signOrCancel;
            req.BossGuid = bossGuid;
            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossApplyReq, req);
        }

        private void OnSignUp(NetMsg msg) {
            CmdWildBossApplyRes response = NetMsgUtil.Deserialize<CmdWildBossApplyRes>(CmdWildBossApplyRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnSignUp: {0}", response);

            if (response.TimeStamp <= 0) {
                // 取消报名回包
                UIManager.CloseUI(EUIID.UI_WorldBossSignUp);
            }
            else {
                // 报名成功回包
                UIManager.OpenUI(EUIID.UI_WorldBossSignUp, false, response);
            }
        }
        
        // 观战
        public void ReqWatchBattle(ulong bossGuid) {
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqWatchBattle:bossGuid:{0}", bossGuid);

            CmdWildBossWatchReq req = new CmdWildBossWatchReq();
            req.BossGuid = bossGuid;
            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossWatchReq, req);
        }

        public void ReqBossCount(uint challengeId, IList<uint> ids) {
            CmdWildBossMapInfoReq req = new CmdWildBossMapInfoReq();
            req.ChallengeId = challengeId;
            DebugUtil.LogFormat(ELogType.eWorldBoss, "ReqBossCount: {0}", challengeId);

            for (int i = 0, length = ids.Count; i < length; ++i) {
                req.BossIds.Add(ids[i]);
            }

            NetClient.Instance.SendMessage((ushort) WildNPC.CmdWildBossMapInfoReq, req);
        }

        private void OnBossCountReceived(NetMsg msg) {
            CmdWildBossMapInfoRes response = NetMsgUtil.Deserialize<CmdWildBossMapInfoRes>(CmdWildBossMapInfoRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnBossCountReceived: {0}", response.ChallengeId);

            this.eventEmitter.Trigger<CmdWildBossMapInfoRes>(EEvents.OnBossCountGot, response);
        }

        private void OnBossApplyNtf(NetMsg msg) {
            CmdWildBossApplyNtf response = NetMsgUtil.Deserialize<CmdWildBossApplyNtf>(CmdWildBossApplyNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnBossApplyNtf: {0}, myTeamId {1}", response.TeamId, Sys_Team.Instance.teamID);
            if (response.TeamId != Sys_Team.Instance.teamID) {
                UIManager.OpenUI(EUIID.UI_WorldBossSignUpFail);
            }
        }

        // 领取奖励次数的通知
        private void OnBossRewardDataNtf(NetMsg msg) {
            CmdWildBossRewardDataNtf response = NetMsgUtil.Deserialize<CmdWildBossRewardDataNtf>(CmdWildBossRewardDataNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eWorldBoss, "OnBossRewardDataNtf: {0}", response);

            if (!this.usedCount.TryGetValue(response.Reward.PlayType, out BossPlayMode playMode)) {
                playMode = new BossPlayMode(response.Reward.PlayType);
                this.usedCount.Add(response.Reward.PlayType, playMode);
            }

            playMode.Refresh(response.Reward);
        }

        void OnBossBattleFailNtf(NetMsg netMsg) {
            CmdNpcBossBattleFailNtf ntf = NetMsgUtil.Deserialize<CmdNpcBossBattleFailNtf>(CmdNpcBossBattleFailNtf.Parser, netMsg);
            if (ntf != null) {
                //Npc npc = GameCenter.mainWorld.GetActor(typeof(Npc), ntf.NpcUid) as Npc;
                //if (npc != null) 
                if (GameCenter.TryGetSceneNPC(ntf.NpcUid, out Npc npc)) {
                    if (npc.cSVNpcData.type == (uint) ENPCType.WorldBoss) {
                        //World.GetComponent<WorldBossComponent>(npc).OnBossBattleFailNtf(ntf.Result);                       
                        (npc as WorldBossNpc).worldBossComponent.OnBossBattleFailNtf(ntf.Result);
                    }
                }
            }
        }

        void OnCmdWildBossBattleIdNtf(NetMsg netMsg) {
            CmdWildBossBattleIdNtf ntf = NetMsgUtil.Deserialize<CmdWildBossBattleIdNtf>(CmdWildBossBattleIdNtf.Parser, netMsg);
            if (ntf != null) {
                //Npc npc = GameCenter.mainWorld.GetActor(typeof(Npc), ntf.NpcUid) as Npc;
                //if (npc != null && npc.cSVNpcData != null) 
                if (GameCenter.TryGetSceneNPC(ntf.NpcUid, out Npc npc) && npc.cSVNpcData != null) {
                    if (npc.cSVNpcData.type == (uint) ENPCType.WorldBoss) {
                        //var compoonent = World.GetComponent<WorldBossComponent>(npc);
                        //var compoonent = npc.worldBossComponent;
                        //if (compoonent != null) {
                        //    compoonent.BattleID = ntf.BattleId;
                        //}

                        (npc as WorldBossNpc).worldBossComponent.BattleID = ntf.BattleId;
                    }
                }
            }
        }

        public Action PendingAction;

        private void OnEndBattle(CmdBattleEndNtf ntf) {
            // 世界boss类型
            if (ntf.BattleResult == 1 && CombatManager.Instance.m_BattleTypeTb.CloseReward == 2) {
                this.PendingAction = () => { UIManager.OpenUI(EUIID.UI_WorldbossBattleResult, false, ntf); };
            }
        }

        #endregion

        public bool HasRewardUngot {
            get {
                bool rlt = false;
                foreach (var kvp in unlockedCamps) {
                    rlt |= (kvp.Value.HasRewardUngot);
                    if (rlt) {
                        return rlt;
                    }
                }

                return rlt;
            }
        }

        public bool TryGetAnyUnlockedBossIdByCampId(uint campId, out uint bossId) {
            bossId = 0;
            foreach (var kvp in this.unlockedBossManuales) {
                if (kvp.Value.csv.camp_id == campId) {
                    bossId = kvp.Key;
                    return true;
                }
            }

            return false;
        }

        // 是否有足够次数参加玩法
        public bool IsActivityCountValid(uint activityId, out int uCount, out int limitCount) {
            limitCount = 0;
            uCount = 0;
            bool ret = this.usedCount.TryGetValue(activityId, out BossPlayMode playMode);
            DebugUtil.LogFormat(ELogType.eWorldBoss, "IsActivityCountValid {0}", activityId);
            if (ret) {
                if (playMode.csv.rewardLimit != 0) {
                    uCount = (int) playMode.weeklyUsedCount;
                    limitCount = (int) playMode.csv.rewardLimit;
                }
                else {
                    uCount = (int) playMode.dailyUsedCount;
                    limitCount = (int) playMode.csv.rewardLimitDay;
                }

                ret = uCount < limitCount;
            }
            else {
                // 第一次进来的时候，可能 usedCount还没有组装
                ret = true;
            }

            return ret;
        }

        // 能否参加某个boss玩法活动
        public bool CanEnter(uint bossId, uint targetLevel) {
            return this.CanEnter(CSVBOSSInformation.Instance.GetConfData(bossId), targetLevel);
        }

        public bool CanEnter(CSVBOSSInformation.Data csvBoss, uint targetLevel) {
            bool result = false;
            if (csvBoss != null) {
                result = csvBoss.limit_level[0] <= targetLevel && targetLevel <= csvBoss.limit_level[1];
            }

            return result;
        }

        public List<uint> GetSortedActivities(bool sort = false, bool limitLevel = false, bool filterInRank = false) {
            List<uint> ls = new List<uint>();
            foreach (var kvp in CSVBOOSFightPlayMode.Instance.GetAll()) {
                if (limitLevel) {
                    uint level = Sys_Role.Instance.Role.Level;
                    if (level >= kvp.playModeLevelLimit[0] && level <= kvp.playModeLevelLimit[1]) {
                        if ((filterInRank && kvp.playModeIsRanking) || !filterInRank) {
                            ls.Add(kvp.id);
                        }
                    }
                }
                else {
                    if ((filterInRank && kvp.playModeIsRanking) || !filterInRank) {
                        ls.Add(kvp.id);
                    }
                }
            }

            if (sort) {
                ls.Sort((lId, rId) => { return (int) (lId - rId); });
            }

            return ls;
        }

        public uint GetMatchChallengeId(IList<int> challengeIds, uint targetLevel, out int index) {
            index = 0;
            uint id = 0;

            for (int i = 0, length = challengeIds.Count; i < length; ++i) {
                bool isMatch = false;
                CSVChallengeLevel.Data csv = CSVChallengeLevel.Instance.GetConfData((uint) challengeIds[i]);
                if (csv != null) {
                    isMatch = csv.challengeLevelLimit[0] <= targetLevel && targetLevel <= csv.challengeLevelLimit[1];
                }

                if (isMatch) {
                    index = i;
                    id = (uint) challengeIds[i];
                    break;
                }
            }

            return id;
        }

        public uint GetMatchChallengeId(IList<uint> challengeIds, uint targetLevel, out int index) {
            index = 0;
            uint id = 0;

            for (int i = 0, length = challengeIds.Count; i < length; ++i) {
                bool isMatch = false;
                CSVChallengeLevel.Data csv = CSVChallengeLevel.Instance.GetConfData(challengeIds[i]);
                if (csv != null) {
                    isMatch = csv.challengeLevelLimit[0] <= targetLevel && targetLevel <= csv.challengeLevelLimit[1];
                }

                if (isMatch) {
                    index = i;
                    id = challengeIds[i];
                    break;
                }
            }

            return id;
        }
    }
}