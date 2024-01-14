using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using pbc = global::Google.Protobuf.Collections;

namespace Logic {
    public partial class Sys_Talent : SystemModuleBase<Sys_Talent> {
        public override void Init() {
            // 升级
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.UpdateTalentLevelReq, (ushort) CmdTalent.UpdateTalentLevelRes, this.OnUpdateTalentLevelRes, CmdTalentUpdateTalentLevelRes.Parser);
            // 重置
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.ResetTalentPointReq, (ushort) CmdTalent.ResetTalentPointRes, this.OnResetTalentPointRes, CmdTalentResetTalentPointRes.Parser);
            // 兑换
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.ExchangeTalentPointReq, (ushort) CmdTalent.ExchangeTalentPointRes, this.OnExchangeTalentPointRes, CmdTalentExchangeTalentPointRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.EtpexpireTimeReq, (ushort) CmdTalent.EtpexpireTimeRes, this.OnTalentExpireTime, CmdTalentETPExpireTimeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTalent.TalentListNtf, this.OnTalentListNtf, CmdTalentTalentListNtf.Parser);
            // 天赋点数上限更新
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTalent.UpdateTalentLimitNtf, this.OnUpdateTalentLimitNtf, CmdTalentUpdateTalentLimitNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTalent.UpdateFreePointNtf, this.OnUpdateFreePointNtf, CmdTalentUpdateFreePointNtf.Parser);
            
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.CreateTalentSchemeReq, (ushort) CmdTalent.CreateTalentSchemeRes, this.CreateTalentSchemeRes, CmdTalentCreateTalentSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.SwitchSchemeReq, (ushort) CmdTalent.SwitchSchemeRes, this.SwitchSchemeRes, CmdTalentSwitchSchemeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTalent.ModifyNameReq, (ushort) CmdTalent.ModifyNameRes, this.ModifyNameRes, CmdTalentModifyNameRes.Parser);

            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.ChangePlan, (playType, index) => {
                if (playType == (uint) Sys_Plan.EPlanType.Talent) {
                    SwitchSchemeReq(index);
                }
            }, true);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, true);
        }

        public int MaxLianHua = 0;

        public override void OnLogin() {
            this.SelectedTalentId = 0;
            this.MaxLianHua = int.Parse(CSVParam.Instance.GetConfData(257).str_value);
        }

        private void OnTalentListNtf(NetMsg msg) {
            CmdTalentTalentListNtf response = NetMsgUtil.Deserialize<CmdTalentTalentListNtf>(CmdTalentTalentListNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTalent, "天赋全量包 回包{0}", response.TalentSchemeList.Count);
            
            this.currentLimitTalentPoint = response.TalentPointLimit;
            this.curSchemeIndex = (int)response.CurIndex;

            this.eventEmitter.Trigger(EEvents.OnCanRemainTalentChanged);
            this.eventEmitter.Trigger(EEvents.OnTalentLimitChanged);

            // 炼化信息
            this.lianhuaTimeStamps.Clear();
            this.ClearTimer();
            for (int i = 0, length = response.ExchangeTalentList.Count; i < length; ++i) {
                uint endTime = response.ExchangeTalentList[i];
                long now = Sys_Time.Instance.GetServerTime();
                long diff = endTime - now;
                //UnityEngine.Debug.LogError("Now:" + now + "  endTime:" + endTime);
                if (diff >= 0) {
                    this.lianhuaTimeStamps.Add(endTime);

                    Timer timer = Timer.Register(diff, () => { this.ReqTalentExpireTime(); }, null, false, true);
                    this.lianhuaTimers.Add(timer);
                }
            }

            this.eventEmitter.Trigger(EEvents.OnLianghuaChanged);
            
            this.schemes.Clear();
            for (int i = 0, length = response.TalentSchemeList.Count; i < length; ++i) {
                var sch = CreateOneScheme(i);
                sch.canUseTalentPoint = response.TalentSchemeList[i].FreePointNum;
                schemes.Add(sch);
                if (response.TalentSchemeList[i].Name == null || response.TalentSchemeList[i].Name.IsEmpty) {
                    sch.name = LanguageHelper.GetTextContent(10013501, (i + 1).ToString());
                }
                else {
                    sch.name = response.TalentSchemeList[i].Name.ToStringUtf8();
                }
                
                // server下发的非0级天赋
                for (int j = 0, lengthJ = response.TalentSchemeList[i].TalentList.Count; j < lengthJ; ++j) {
                    uint talentId = response.TalentSchemeList[i].TalentList[j].TalentId;
                    if (!sch.talents.TryGetValue(talentId, out TalentEntry entry)) {
                        continue;
                    }

                    entry.Refresh(response.TalentSchemeList[i].TalentList[j].TalentLevel);
                }
            }
        }

        private Scheme CreateOneScheme(int index) {
            Scheme sch = new Scheme();
            for (int j = 0, lengthJ = CSVTalent.Instance.Count; j < lengthJ; ++j) {
                var one = CSVTalent.Instance.GetByIndex(j);
                // 只保留一个分支id=1的天赋分支
                if (one.career_id == Sys_Role.Instance.Role.Career && one.branch_id == 1) {
                    var entry = new TalentEntry(one.id);
                    entry.schemeIndex = index;
                    sch.talents.Add(one.id, entry);

                    uint branchId = one.branch_id;
                    if (!sch.branchs.TryGetValue(branchId, out TalentBranch branch)) {
                        branch = new TalentBranch(branchId, entry.csv.branch_lan, entry.csv.branchIconId);
                        branch.schemeIndex = index;
                        sch.branchs.Add(branchId, branch);
                    }
                }
            }
            return sch;
        }

        private void OnUpdateTalentLimitNtf(NetMsg msg) {
            CmdTalentUpdateTalentLimitNtf response = NetMsgUtil.Deserialize<CmdTalentUpdateTalentLimitNtf>(CmdTalentUpdateTalentLimitNtf.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTalent, "天赋点数上限 回包{0}", response.TalentPointLimit);

            this.currentLimitTalentPoint = response.TalentPointLimit;
            this.eventEmitter.Trigger(EEvents.OnTalentLimitChanged);
        }

        public void ReqUpdateTalentLevel(uint index, uint talentId) {
            DebugUtil.LogFormat(ELogType.eTalent, "升级天赋 {0}", talentId.ToString());

            CmdTalentUpdateTalentLevelReq req = new CmdTalentUpdateTalentLevelReq();
            req.TalentId = talentId;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort) CmdTalent.UpdateTalentLevelReq, req);
        }

        private void OnUpdateTalentLevelRes(NetMsg msg) {
            CmdTalentUpdateTalentLevelRes response = NetMsgUtil.Deserialize<CmdTalentUpdateTalentLevelRes>(CmdTalentUpdateTalentLevelRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTalent, "接受升级天赋 回包 {0}", response.TalentId.ToString());
            int index = (int)response.Index;
            var sch = this.schemes[index];
            TalentEntry entry = sch.GetTalent(response.TalentId);
            if (entry != null) {
                sch.canUseTalentPoint = (uint) ((int) sch.canUseTalentPoint - Sys_Talent.UpgradeTalentLevelCostPoint);
                entry.Refresh(response.SkillLevel);
                this.eventEmitter.Trigger<uint, uint>(EEvents.OnUpdateTalentLevel, response.TalentId, response.Index);
            }
            else {
                DebugUtil.LogErrorFormat("接受升级天赋 server发送不合理的id {0}", response.TalentId.ToString());
            }
        }

        public void ReqResetTalentPoint(uint index) {
            DebugUtil.LogFormat(ELogType.eTalent, "重置天赋");
            CmdTalentResetTalentPointReq req = new CmdTalentResetTalentPointReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort) CmdTalent.ResetTalentPointReq, req);
        }

        private void OnResetTalentPointRes(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eTalent, "重置天赋 回包");
            CmdTalentResetTalentPointRes response = NetMsgUtil.Deserialize<CmdTalentResetTalentPointRes>(CmdTalentResetTalentPointRes.Parser, msg);
            int index = (int)response.Index;
            var sch = this.schemes[index];
            sch.canUseTalentPoint = sch.canUseTalentPoint + sch.usedTalentPoint;
            foreach (var kvp in sch.talents) {
                kvp.Value.Refresh(0);
            }

            this.eventEmitter.Trigger<uint>(EEvents.OnResetTalentPoint, response.Index);
        }

        public void ReqExchangeTalentPoint() {
            DebugUtil.LogFormat(ELogType.eTalent, "兑换天赋");
            CmdTalentExchangeTalentPointReq req = new CmdTalentExchangeTalentPointReq();
            NetClient.Instance.SendMessage((ushort) CmdTalent.ExchangeTalentPointReq, req);
        }

        private void OnExchangeTalentPointRes(NetMsg msg) {
            CmdTalentExchangeTalentPointRes response = NetMsgUtil.Deserialize<CmdTalentExchangeTalentPointRes>(CmdTalentExchangeTalentPointRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTalent, "兑换天赋 回包");

            uint timeStamp = response.ExchangeExpireTime;
            if (!this.lianhuaTimeStamps.Contains(timeStamp)) {
                this.lianhuaTimeStamps.Add(timeStamp);

                long now = Sys_Time.Instance.GetServerTime();
                float diff = (timeStamp - now);
                Timer timer = Timer.Register(diff, () => { this.ReqTalentExpireTime(); }, null, false, true);
                this.lianhuaTimers.Add(timer);

                this.eventEmitter.Trigger(EEvents.OnLianghuaChanged);
            }

            this.eventEmitter.Trigger(EEvents.OnExchangeTalentPoint);
        }

        // 每一个都需要进行请求
        public void ReqTalentExpireTime() {
            DebugUtil.LogFormat(ELogType.eTalent, "天赋炼化到期请求");

            CmdTalentETPExpireTimeReq req = new CmdTalentETPExpireTimeReq();
            NetClient.Instance.SendMessage((ushort) CmdTalent.EtpexpireTimeReq, req);
        }

        private void OnTalentExpireTime(NetMsg msg) {
            CmdTalentETPExpireTimeRes response = NetMsgUtil.Deserialize<CmdTalentETPExpireTimeRes>(CmdTalentETPExpireTimeRes.Parser, msg);
            DebugUtil.LogFormat(ELogType.eTalent, "天赋炼化到期 回复");

            uint timeStamp = response.ExchangeExpireTime;
            if (timeStamp != 0) {
                if (this.lianhuaTimeStamps.Count > 0) {
                    this.lianhuaTimeStamps[0] = timeStamp;
                    this.lianhuaTimers[0]?.Cancel();

                    long now = Sys_Time.Instance.GetServerTime();
                    float diff = timeStamp - now;
                    Timer timer = Timer.Register(diff, () => { this.ReqTalentExpireTime(); }, null, false, true);
                    this.lianhuaTimers[0] = timer;
                }
            }

            this.eventEmitter.Trigger(EEvents.OnLianghuaChanged);
            this.eventEmitter.Trigger(EEvents.OnCanRemainTalentChanged);
        }

        // 剩余天赋点更新
        // gm修改天赋点的时候，不能影响到后台的炼化点的逻辑
        private void OnUpdateFreePointNtf(NetMsg msg) {
            CmdTalentUpdateFreePointNtf response = NetMsgUtil.Deserialize<CmdTalentUpdateFreePointNtf>(CmdTalentUpdateFreePointNtf.Parser, msg);
            for (int i = 0, length = schemes.Count; i < length; ++i) {
                var cur = schemes[i];
                cur.canUseTalentPoint += response.AddPoint;
                DebugUtil.LogFormat(ELogType.eTalent, "index {0} canUseTalentPoint:{1}",i.ToString(),  cur.canUseTalentPoint);
            }
            
            int lengthIn = response.AddPoint;
            if (response.Type == 0) {
                for (int j = 0; j < lengthIn; j++) {
                    this.lianhuaTimers.RemoveAt(0);
                    this.lianhuaTimeStamps.RemoveAt(0);
                }
            }
                
            DebugUtil.LogFormat(ELogType.eTalent, "lianhuaTimersCount:{0} lianhuaTimeStamps {1} length:{2}", this.lianhuaTimers.Count, this.lianhuaTimeStamps.Count, lengthIn);
            
            this.eventEmitter.Trigger(EEvents.OnCanRemainTalentChanged);
            this.eventEmitter.Trigger(EEvents.OnLianghuaChanged);
        }

        public void CreateTalentSchemeReq() {
            DebugUtil.LogFormat(ELogType.eTalent, "CreateTalentSchemeReq");

            CmdTalentCreateTalentSchemeReq req = new CmdTalentCreateTalentSchemeReq();
            NetClient.Instance.SendMessage((ushort) CmdTalent.CreateTalentSchemeReq, req);
        }

        private void CreateTalentSchemeRes(NetMsg msg) {
            CmdTalentCreateTalentSchemeRes response = NetMsgUtil.Deserialize<CmdTalentCreateTalentSchemeRes>(CmdTalentCreateTalentSchemeRes.Parser, msg);
            var sch = CreateOneScheme((int)response.Index);
            sch.name = LanguageHelper.GetTextContent(10013501, (response.Index + 1).ToString());
            sch.canUseTalentPoint = usingScheme.withoutLianhuaing;
            schemes.Add(sch);
            
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.AddNewPlan, (uint)Sys_Plan.EPlanType.Talent, response.Index);
        }
        
        public void SwitchSchemeReq(uint index) {
            DebugUtil.LogFormat(ELogType.eTalent, "SwitchSchemeReq {0}", index.ToString());

            CmdTalentSwitchSchemeReq req = new CmdTalentSwitchSchemeReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort) CmdTalent.SwitchSchemeReq, req);
        }
        private void SwitchSchemeRes(NetMsg msg) {
            CmdTalentSwitchSchemeRes response = NetMsgUtil.Deserialize<CmdTalentSwitchSchemeRes>(CmdTalentSwitchSchemeRes.Parser, msg);
            curSchemeIndex = (int)response.Index;
            
            eventEmitter.Trigger<uint>(EEvents.ChangePlan, response.Index);
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, (uint)Sys_Plan.EPlanType.Talent, response.Index);
        }

        public void ModifyNameReq(uint index, string newName) {
            DebugUtil.LogFormat(ELogType.eTalent, "ModifyNameReq {0} {1}", index.ToString(), newName);

            CmdTalentModifyNameReq req = new CmdTalentModifyNameReq();
            req.Index = index;
            req.Name = FrameworkTool.ConvertToGoogleByteString(newName);
            NetClient.Instance.SendMessage((ushort) CmdTalent.ModifyNameReq, req);
        }
        private void ModifyNameRes(NetMsg msg) {
            CmdTalentModifyNameRes response = NetMsgUtil.Deserialize<CmdTalentModifyNameRes>(CmdTalentModifyNameRes.Parser, msg);
            var index = (int)response.Index;
            var name = "";
            if (response.Name == null || response.Name.IsEmpty) {
                name = LanguageHelper.GetTextContent(10013501, (response.Index + 1).ToString());
            }
            else {
                name = response.Name.ToStringUtf8();
            }

            schemes[index].name = name;
            Sys_Plan.Instance.eventEmitter.Trigger<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, (uint)Sys_Plan.EPlanType.Talent, response.Index, name);
        }

        private void OnTimeNtf(uint oldTime, uint newTime) {
            this.ClearTimer();
            bool hasTimeout = false;
            for (int i = 0, length = this.lianhuaTimeStamps.Count; i < length; ++i) {
                uint endTime = this.lianhuaTimeStamps[i];
                long now = newTime;
                long diff = endTime - now;

                if (diff <= 0) {
                    diff = uint.MaxValue; // 已经到期的为了进入timer池，但是timer还不生效，这里直接设置为无限大，让其不生效，因为后面网络回调中会删除失效的timer,所以这里必须入timer池
                    if (!hasTimeout) {
                        this.ReqTalentExpireTime();
                        hasTimeout = true;
                    }
                }

                // 更新timer的duration
                Timer timer = Timer.Register(diff, () => { this.ReqTalentExpireTime(); }, null, false, true);
                this.lianhuaTimers.Add(timer);
            }

            this.eventEmitter.Trigger(EEvents.OnLianghuaChanged);
        }
    }
}