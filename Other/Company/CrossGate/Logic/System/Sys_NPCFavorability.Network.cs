using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {
    public partial class Sys_NPCFavorability : SystemModuleBase<Sys_NPCFavorability> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents {
            OnPlayerFavorabilityChanged, // 体力改变
            OnNpcFavorabilityChanged, // 好感值改变
            OnFavorabilityStageChanged, // 好感阶段改变
            OnZoneRewardGot, // 区域奖励获取
            OnGiftUnlock, // 礼物解锁
            OnNPCUnlock, // npc解锁
            OnZoneUnlock, // 区域解锁
            OnAllInfoReceived, // 登录信息数据
        }

        public override void Init() {
            EventDispatcher.Instance.AddEventListener((ushort)CmdFavorability.InfoReq, (ushort)CmdFavorability.InfoAck, this.OnCmdFavorabilityInfoAck, CmdFavorabilityInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFavorability.AddValueReq, (ushort)CmdFavorability.AddValueAck, this.OnCmdFavorabilityAddValueAck, CmdFavorabilityAddValueAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFavorability.StageUpReq, (ushort)CmdFavorability.StageUpAck, this.OnCmdFavorabilityStageUpAck, CmdFavorabilityStageUpAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdFavorability.GetZoneRewardReq, (ushort)CmdFavorability.GetZoneRewardAck, this.OnCmdFavorabilityGetZoneRewardAck, CmdFavorabilityGetZoneRewardAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFavorability.UnlockGiftTypeNtf, this.OnCmdFavorabilityUnlockGiftTypeNtf, CmdFavorabilityUnlockGiftTypeNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFavorability.UnlockNpcNtf, this.OnCmdFavorabilityUnlockNpcNtf, CmdFavorabilityUnlockNpcNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdFavorability.InfoNtf, this.OnCmdFavorabilityInfoNtf, CmdFavorabilityInfoNtf.Parser);

            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, true);
        }

        private void OnTimeNtf(uint oldTime, uint newTime) {
            // 强制重新设置timer
            uint now = newTime;
            bool over = false;
            while (now > this.NextNPCRefreshTime) {
                this._lastNPCRefreshTime += 86400;
                over = true;
            }
            if (over) {
                this.ReqCmdFavorabilityInfo();
            }

            this.lastNPCRefreshTime = this._lastNPCRefreshTime;

            over = false;
            //int gap = (int)(newTime - oldTime) / (int)this.FavorabilityRecoveryTime;
            while (now > this.NextFavorabilityRefreshTime) {
                ++this._favorability;
                this.lastFavorabilityRecoveryTime += this.FavorabilityRecoveryTime;
                over = true;
            }
            if (over) {

            }
            this.Favorability = this._favorability;
        }

        public void ReqCmdFavorabilityInfo() {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: CmdFavorabilityInfoReq");

            CmdFavorabilityInfoReq req = new CmdFavorabilityInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdFavorability.InfoReq, req);
        }
        // 登录数据包
        private void OnCmdFavorabilityInfoAck(NetMsg msg) {
            CmdFavorabilityInfoAck response = NetMsgUtil.Deserialize<CmdFavorabilityInfoAck>(CmdFavorabilityInfoAck.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityInfoAck");

            this.ProcessAll(response.Info);
        }

        private void ProcessAll(NpcFavorability info) {
            this.Clear();
            //UnityEngine.Debug.LogError(info.FItems.Count.ToString());
            for (int i = 0, length = info.FItems.Count; i < length; ++i) {
                var serverNpc = info.FItems[i];
                DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityInfoAck {0}", serverNpc.NpcInfoId);
                this.TryGetNpc(serverNpc.NpcInfoId, out FavorabilityNPC npc, true);
                npc.Refresh(serverNpc);
                npc.IsNew = false;

                this.npcs.Add(serverNpc.NpcInfoId, npc);
                if (!this.zoneNpcs.TryGetValue(serverNpc.ZoneId, out ZoneFavorabilityNPC container)) {
                    container = new ZoneFavorabilityNPC(serverNpc.ZoneId);
                    this.zoneNpcs.Add(serverNpc.ZoneId, container);
                }
                container.Add(npc);
            }

            this.lastFavorabilityRecoveryTime = info.LastRecoverTime;
            this.lastNPCRefreshTime = info.LastRefreshTime;

            this.Favorability = info.RoleValue;
            for (int i = 0, length = info.UnlockZone.Zones.Count; i < length; ++i) {
                uint zoneId = info.UnlockZone.Zones[i].ZoneId;
                this.zoneNpcs[zoneId].gotRewads = info.UnlockZone.Zones[i].Reward;
                // this.zoneNpcs[zoneId].unlockTime = info.UnlockZone.Zones[i].UnlockTime;
            }

            this.selectedZoneId = info.UnlockZone.ZoneId;

            this.eventEmitter.Trigger(EEvents.OnAllInfoReceived);
        }

        public void ReqCmdFavorabilityAddValue(uint actTypeId, uint npcId, uint csvId = 0, ulong guid = 0, uint feteId = 0) {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: ReqCmdFavorabilityAddValue");

            CmdFavorabilityAddValueReq req = new CmdFavorabilityAddValueReq();
            req.ActType = actTypeId;
            req.NpcInfoId = npcId;
            req.InfoId = csvId;
            req.EquipUid = guid;
            req.FeastId = feteId;
            NetClient.Instance.SendMessage((ushort)CmdFavorability.AddValueReq, req);
        }
        // NPC好感度提升
        private void OnCmdFavorabilityAddValueAck(NetMsg msg) {
            CmdFavorabilityAddValueAck response = NetMsgUtil.Deserialize<CmdFavorabilityAddValueAck>(CmdFavorabilityAddValueAck.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityAddValueAck");

            this.lastFavorabilityRecoveryTime = response.LastRecoverTime;
            this.Favorability = response.RoleValue;

            uint npcId = response.NpcInfoId;
            if (this.TryGetNpc(npcId, out var npc)) {
                uint old = npc.favorability;
                npc.RefreshHealth(response.HealthValue, response.SickId);
                npc.RefreshFavorability(response.FavorabilityValue);
                npc.RefreshMood(response.MoodValue, response.MoodId);
                npc.acts[response.ActType] = response.ActTimes;

                if (old != response.FavorabilityValue) {
                    // id:stageId:from:to
                    this.eventEmitter.Trigger<uint, uint, uint, uint>(EEvents.OnNpcFavorabilityChanged, npcId, npc.favorabilityStage, old, response.FavorabilityValue);

                    UIScheduler.Push((int)EUIID.UI_FavorabilityNPCChanged, new System.Tuple<uint, uint, uint, uint>(npcId, npc.favorabilityStage, old, response.FavorabilityValue), null, false, () => {
                        return !UIManager.IsVisibleAndOpen(EUIID.UI_PureHint);
                    });
                }
            }
        }

        public void ReqCmdFavorabilityStageUp(uint npcId) {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: ReqCmdFavorabilityStageUp");

            CmdFavorabilityStageUpReq req = new CmdFavorabilityStageUpReq();
            req.NpcInfoId = npcId;
            NetClient.Instance.SendMessage((ushort)CmdFavorability.StageUpReq, req);
        }
        // 好感度阶段提升
        private void OnCmdFavorabilityStageUpAck(NetMsg msg) {
            CmdFavorabilityStageUpAck response = NetMsgUtil.Deserialize<CmdFavorabilityStageUpAck>(CmdFavorabilityStageUpAck.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityStageUpAck");

            if (this.TryGetNpc(response.NpcInfoId, out FavorabilityNPC npc)) {
                uint old = npc.favorabilityStage;
                npc.RefreshFavorabilityStage(response.NewStatgId);

                if (old != response.NewStatgId) {
                    // id:from:to
                    Sys_NPCFavorability.Instance.eventEmitter.Trigger<uint, uint, uint>(Sys_NPCFavorability.EEvents.OnFavorabilityStageChanged, response.NpcInfoId, old, response.NewStatgId);

                    UpdateFavirabilityEvt et = new UpdateFavirabilityEvt();
                    et.npcId = response.NpcInfoId;
                    et.val = response.NewStatgId;
                    Sys_HUD.Instance.eventEmitter.Trigger<UpdateFavirabilityEvt>(Sys_HUD.EEvents.OnUpdateFavirability, et);

                    CSVFavorabilityStageName.Data csvStage = CSVFavorabilityStageName.Instance.GetConfData(npc.csvNPCFavorabilityStage.Stage);
                    if (csvStage != null) {
                        string npcName = LanguageHelper.GetNpcTextContent(npc.csvNPC.name);
                        string stageName = LanguageHelper.GetTextContent(csvStage.name);

                        UIManager.OpenUI(EUIID.UI_PureHint, false, LanguageHelper.GetTextContent(2010615, npcName, stageName));
                    }
                }
            }
        }

        public void ReqCmdFavorabilityGetZoneReward(uint zoneId) {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: ReqCmdFavorabilityGetZoneReward");

            CmdFavorabilityGetZoneRewardReq req = new CmdFavorabilityGetZoneRewardReq();
            req.ZoneId = zoneId;
            NetClient.Instance.SendMessage((ushort)CmdFavorability.GetZoneRewardReq, req);
        }
        // 区域奖励
        private void OnCmdFavorabilityGetZoneRewardAck(NetMsg msg) {
            CmdFavorabilityGetZoneRewardAck response = NetMsgUtil.Deserialize<CmdFavorabilityGetZoneRewardAck>(CmdFavorabilityGetZoneRewardAck.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityGetZoneRewardAck");

            this.zoneNpcs[response.ZoneId].gotRewads = true;
            this.eventEmitter.Trigger<uint>(EEvents.OnZoneRewardGot, response.ZoneId);
        }

        // 礼物解锁
        private void OnCmdFavorabilityUnlockGiftTypeNtf(NetMsg msg) {
            CmdFavorabilityUnlockGiftTypeNtf response = NetMsgUtil.Deserialize<CmdFavorabilityUnlockGiftTypeNtf>(CmdFavorabilityUnlockGiftTypeNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityUnlockGiftTypeNtf");

            if (this.TryGetNpc(response.NpcId, out FavorabilityNPC npc)) {
                npc.RefreshUnlock(response.GiftType);

                CSVFavorabilityGiftType.Data csvGift = CSVFavorabilityGiftType.Instance.GetConfData(response.GiftType);
                if (csvGift != null) {
                    string giftType = LanguageHelper.GetTextContent(csvGift.name);
                    string npcName = LanguageHelper.GetNpcTextContent(npc.csvNPC.name);
                    string content = LanguageHelper.GetTextContent(2010616, npcName, giftType);

                    UIManager.OpenUI(EUIID.UI_PureHint, false, content);
                }

                this.eventEmitter.Trigger<uint, uint>(EEvents.OnGiftUnlock, response.NpcId, response.GiftType);
            }
        }
        // NPC调查解锁
        private void OnCmdFavorabilityUnlockNpcNtf(NetMsg msg) {
            CmdFavorabilityUnlockNpcNtf response = NetMsgUtil.Deserialize<CmdFavorabilityUnlockNpcNtf>(CmdFavorabilityUnlockNpcNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityUnlockNpcNtf");

            uint npcId = response.NpcId;
            if (!this.TryGetNpc(npcId, out FavorabilityNPC npc, true)) {
                DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityUnlockNpcNtf {0}", npcId);
                npc.Refresh(response.NpcInfo);
                npc.IsNew = true;

                this.npcs.Add(npcId, npc);

                uint zoneId = response.NpcInfo.ZoneId;
                if (!this.zoneNpcs.TryGetValue(zoneId, out ZoneFavorabilityNPC container)) {
                    container = new ZoneFavorabilityNPC(zoneId);
                    this.zoneNpcs.Add(zoneId, container);

                    this.selectedZoneId = zoneId;
                    this.eventEmitter.Trigger<uint>(EEvents.OnZoneUnlock, zoneId);
                }
                container.Add(npc);

                this.eventEmitter.Trigger<uint, uint>(EEvents.OnNPCUnlock, zoneId, response.NpcId);

                UpdateFavirabilityEvt et = new UpdateFavirabilityEvt();
                et.npcId = npcId;
                et.val = 1;
                Sys_HUD.Instance.eventEmitter.Trigger<UpdateFavirabilityEvt>(Sys_HUD.EEvents.OnUpdateFavirability, et);
            }
        }
        private void OnCmdFavorabilityInfoNtf(NetMsg msg) {
            CmdFavorabilityInfoNtf response = NetMsgUtil.Deserialize<CmdFavorabilityInfoNtf>(CmdFavorabilityInfoNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: OnCmdFavorabilityInfoNtf");

            this.ProcessAll(response.Info);
        }

        public void ReqReceiveTask(uint npcId, uint taskId) {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: ReqReceiveTask");

            CmdFavorabilityAcceptTaskReq req = new CmdFavorabilityAcceptTaskReq();
            req.NpcId = npcId;
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort)CmdFavorability.AcceptTaskReq, req);
        }
        
        public void ReqSelectTargetZone(uint zoneId) {
            DebugUtil.LogFormat(ELogType.eRelation, "Sys_NPCFavorability: ReqSelectTargetZone");

            CmdFavorabilityChangeZoneReq req = new CmdFavorabilityChangeZoneReq();
            req.ZoneId = zoneId;
            NetClient.Instance.SendMessage((ushort)CmdFavorability.ChangeZoneReq, req);
        }
    }
}