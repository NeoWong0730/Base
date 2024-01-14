using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using Random = System.Random;

namespace Logic {
    public partial class Sys_TownTask : SystemModuleBase<Sys_TownTask> {
        public enum EEvents {
            OnInfoNtf,
            OnCommitRes,
            OnRefreshTaskRes,
            OnTakeAwardRes,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init() {
            // 登录包
            EventDispatcher.Instance.AddEventListener((ushort) CmdTownTask.GetInfoReq, (ushort) CmdTownTask.InfoNtf, this.OnInfoNtf, CmdTownTaskInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTownTask.CommitReq, (ushort) CmdTownTask.CommitRes, this.OnCommitRes, CmdTownTaskCommitRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTownTask.RefreshTaskReq, (ushort) CmdTownTask.RefreshTaskRes, this.OnRefreshTaskRes, CmdTownTaskRefreshTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTownTask.TakeAwardReq, (ushort) CmdTownTask.TakeAwardRes, this.OnTakeAwardRes, CmdTownTaskTakeAwardRes.Parser);

            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, true);
        }

        public override void OnLogout() {
            this.towns.Clear();
            this.timer?.Cancel();
        }

        public void GetInfoReq() {
            // DebugUtil.LogFormat(ELogType.eTownTask, "请求刷新所有城镇任务");

            CmdTownTaskGetInfoReq req = new CmdTownTaskGetInfoReq();
            NetClient.Instance.SendMessage((ushort) CmdTownTask.GetInfoReq, req);
        }

        private void OnInfoNtf(NetMsg msg) {
            // DebugUtil.LogFormat(ELogType.eTownTask, "OnInfoNtf");
            CmdTownTaskInfoNtf response = NetMsgUtil.Deserialize<CmdTownTaskInfoNtf>(CmdTownTaskInfoNtf.Parser, msg);
            // 倒计时
            this.nextRefreshTime = response.LastRefreshTime;
            var now = Sys_Time.Instance.GetServerTime();
            this.OnTimeNtf(now, now);

            // 城镇
            this.towns.Clear();
            for (int i = 0, length = response.TownTaskList.Count; i < length; ++i) {
                var one = response.TownTaskList[i];
                var town = new Town {id = one.TownId, contributeLevel = one.Level, contributeExpInLevel = one.Exp, gotRewards = new List<uint>(one.AwardTake)};

                town.tasks.Clear();
                for (int j = 0, lengthJ = one.TaskList.Count; j < lengthJ; ++j) {
                    var task = one.TaskList[j];
                    var t = new Task {
                        id = task.InfoId, libraryId = task.TaskId, isFinish = task.Finish, town = town,
                    };

                    town.tasks.Add(t);
                }

                this.towns.Add(one.TownId, town);
            }
            
            this.eventEmitter.Trigger(EEvents.OnInfoNtf);
        }

        public void CommitReq(uint townId, uint bigTaskId, ulong guid = 0) {
            // DebugUtil.LogFormat(ELogType.eTownTask, "提交任务 {0} {1} {2}", townId.ToString(), bigTaskId.ToString(), guid.ToString());

            CmdTownTaskCommitReq req = new CmdTownTaskCommitReq();
            req.TownId = townId;
            req.InfoId = bigTaskId;
            req.ItemUuid = guid;
            NetClient.Instance.SendMessage((ushort) CmdTownTask.CommitReq, req);
        }

        private void OnCommitRes(NetMsg msg) {
            CmdTownTaskCommitRes response = NetMsgUtil.Deserialize<CmdTownTaskCommitRes>(CmdTownTaskCommitRes.Parser, msg);
            Sys_NPCFavorability.Instance.Favorability = response.RoleValue;
            
            // DebugUtil.LogFormat(ELogType.eTownTask, "OnCommitRes {0} {1} {2}", response.TownId.ToString(), response.Level.ToString(), response.Exp.ToString());
            if (this.towns.TryGetValue(response.TownId, out Town town)) {
                town.contributeLevel = response.Level;
                town.contributeExpInLevel = response.Exp;

                for (int i = 0, length = town.tasks.Count; i < length; ++i) {
                    if (town.tasks[i].id == response.InfoId) {
                        town.tasks[i].isFinish = true;

                        this.eventEmitter.Trigger<uint, uint, uint>(EEvents.OnCommitRes, town.id, response.InfoId, town.tasks[i].libraryId);
                        UIManager.OpenUI(EUIID.UI_TownTaskSubmitResult, false, town.tasks[i].libraryId);
                        break;
                    }
                }
            }
        }

        public void RefreshTaskReq(uint townId) {
            // DebugUtil.LogFormat(ELogType.eTownTask, "刷新任务 {0}", townId.ToString());

            CmdTownTaskRefreshTaskReq req = new CmdTownTaskRefreshTaskReq();
            req.TownId = townId;
            NetClient.Instance.SendMessage((ushort) CmdTownTask.RefreshTaskReq, req);
        }

        private void OnRefreshTaskRes(NetMsg msg) {
            CmdTownTaskRefreshTaskRes response = NetMsgUtil.Deserialize<CmdTownTaskRefreshTaskRes>(CmdTownTaskRefreshTaskRes.Parser, msg);
            // DebugUtil.LogFormat(ELogType.eTownTask, "OnRefreshTaskRes {0}", response.TownId.ToString());
            if (this.towns.TryGetValue(response.TownId, out Town town)) {
                town.tasks.Clear();
                for (int i = 0, length = response.TaskList.Count; i < length; ++i) {
                    var task = response.TaskList[i];
                    var t = new Task {
                        id = task.InfoId, libraryId = task.TaskId, isFinish = task.Finish,
                        town = town,
                    };
                    town.tasks.Add(t);
                }

                this.eventEmitter.Trigger<uint>(EEvents.OnRefreshTaskRes, town.id);
            }
        }

        public void TakeAwardReq(uint csvId, bool getAll = false) {
            // DebugUtil.LogFormat(ELogType.eTownTask, "领取贡献奖励 {0} {1}", csvId.ToString(), getAll.ToString());

            CmdTownTaskTakeAwardReq req = new CmdTownTaskTakeAwardReq();
            req.InfoId = csvId;
            req.GetAll = getAll;
            NetClient.Instance.SendMessage((ushort) CmdTownTask.TakeAwardReq, req);
        }

        private void OnTakeAwardRes(NetMsg msg) {
            CmdTownTaskTakeAwardRes response = NetMsgUtil.Deserialize<CmdTownTaskTakeAwardRes>(CmdTownTaskTakeAwardRes.Parser, msg);
            // DebugUtil.LogFormat(ELogType.eTownTask, "OnTakeAwardRes");

            uint townId = 0;
            int changedCount = response.InfoId.Count;
            for (int i = 0, length = changedCount; i < length; ++i) {
                var one = response.InfoId[i];
                var csv = CSVTownContributeAward.Instance.GetConfData(one);
                if (csv != null) {
                    if (this.towns.TryGetValue(csv.TownId, out Town town)) {
                        townId = csv.TownId;
                        town.gotRewards.Add(one);
                    }
                }
            }

            if (townId != 0) {
                this.eventEmitter.Trigger<uint, uint>(EEvents.OnTakeAwardRes, townId, (uint) changedCount);
            }
        }

        private void OnTimeNtf(uint oldTime, uint newTime) {
            if (newTime > oldTime) {
                // gm调时间
                bool over = (long) newTime > this.nextRefreshTime;
                if (over) {
                    this.GetInfoReq();

                    // 如果超出nextRefreshTime，则暂时修改newTime，防止频繁触发timer执行
                    newTime = (uint) this.nextRefreshTime - 1000;
                }
            }

            this.timer?.Cancel();
            // 7s的随机
            long diff = this.nextRefreshTime - (long) newTime + UnityEngine.Random.Range(0, 7);
            this.timer = Timer.RegisterOrReuse(ref this.timer, diff, this.GetInfoReq);
        }
    }

    public partial class Sys_TownTask : SystemModuleBase<Sys_TownTask> {
        public class Task {
            public uint id;

            public CSVTownTask.Data csv {
                get { return CSVTownTask.Instance.GetConfData(id); }
            }

            public uint libraryId;

            public CSVTownTaskLibrary.Data csvLibrary {
                get { return CSVTownTaskLibrary.Instance.GetConfData(libraryId); }
            }

            // 任务状态
            public bool isFinish = false;

            public enum EReason {
                Nil,
                HasFinish,
                LackFavority,
                InvalidPlayerLevel,
                InvalidFavorityLevel,
                LackItem,
                NotSelectTarget,
                ReachMaxLevel,
            }

            public Town town;

            public bool IsReachedMax {
                get { return town.IsReachedFull; }
            }

            public bool CanSimpleDo(out EReason reason) {
                reason = EReason.Nil;
                if (this.isFinish) {
                    reason = EReason.HasFinish;
                    return false;
                }

                // 和npc的好感度等级是否满足
                bool contains = Sys_NPCFavorability.Instance.npcs.TryGetValue(csv.FavorabilityNpc, out var favorabilityNpc);
                if (!contains ||
                    (favorabilityNpc.favorabilityStage < csvLibrary.NeedFavorabilityLv)) {
                    reason = EReason.InvalidFavorityLevel;
                    return false;
                }

                if (IsReachedMax) {
                    reason = EReason.ReachMaxLevel;
                    return false;
                }

                return true;
            }

            public bool CanDo(out EReason reason) {
                if (!CanSimpleDo(out reason)) {
                    return false;
                }

                // 好感度点数是否满足
                if (this.csvLibrary.ConsumePoints > Sys_NPCFavorability.Instance.Favorability) {
                    reason = EReason.LackFavority;
                    return false;
                }

                // 玩家等级是否腕满足
                var taskLv = this.csvLibrary.TaskLv;
                if (taskLv != null && taskLv.Count >= 2) {
                    if (Sys_Role.Instance.Role.Level < taskLv[0] || taskLv[1] < Sys_Role.Instance.Role.Level) {
                        reason = EReason.InvalidPlayerLevel;
                        return false;
                    }
                }

                // 装备是否满足
                if (csvLibrary.TaskType == 1) {
                    var equips = Sys_Equip.Instance.GetEequips(csvLibrary.TaskConsumeEqpt, csvLibrary.ConsumeEqptQuality, FavorabilityNPC.ItemsFilters);
                    if (equips.Count <= 0) {
                        reason = EReason.LackItem;
                        return false;
                    }
                }
                else {
                    // 道具是否满足
                    if (Sys_Bag.Instance.GetItemCount(csvLibrary.TaskConsumeItem) < csvLibrary.ConsumeItemNum) {
                        reason = EReason.LackItem;
                        return false;
                    }
                }

                return true;
            }
        }

        public class Town {
            public uint id;

            public CSVFavorabilityPlaceReward.Data csvFavority {
                get { return CSVFavorabilityPlaceReward.Instance.GetConfData(this.id); }
            }

            public CSVTownContributeAward.Data csvContribute {
                get { return CSVTownContributeAward.Instance.GetConfData(this.id); }
            }

            public ZoneFavorabilityNPC zoneNpcs {
                get {
                    Sys_NPCFavorability.Instance.zoneNpcs.TryGetValue(id, out var rlt);
                    return rlt;
                }
            }

            // serverdata
            public uint contributeLevel;
            public uint contributeExpInLevel;

            public uint contributeLevelId {
                get { return contributeLevel + 1; }
            }

            // 到达最大等级经验
            public bool IsReachedFull {
                get {
                    bool rlt = maxLevel <= contributeLevel;
                    return rlt;
                }
            }

            public int maxLevel {
                get {
                    uint max = 0;
                    var all = CSVTownContributeLv.Instance.GetAll();
                    for (int i = 0, length = all.Count; i < length; ++i) {
                        if(max < all[i].id) {
                            max = all[i].id;
                        }
                    }

                    return (int)max;
                }
            }
            
            public int max {
                get { return this.tasks.Count; }
            }

            public int canDoCount {
                get {
                    int count = 0;
                    for (int i = 0, length = this.tasks.Count; i < length; ++i) {
                        if (this.tasks[i].CanSimpleDo(out var _)) {
                            ++count;
                        }
                    }

                    return count;
                }
            }

            public bool Cando {
                get { return canDoCount > 0; }
            }

            public bool CanAnyGot {
                get {
                    var all = CSVTownContributeAward.Instance.GetAll();
                    for (int i = 0, length = all.Count; i < length; ++i) {
                        var one = all[i];
                        if (one.TownId == this.id) {
                            // 没有领取，并且可以领取
                            if (this.contributeLevel >= one.TownLV && !gotRewards.Contains(one.id)) {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            }

            // csvIds
            public List<uint> gotRewards;
            public List<Task> tasks = new List<Task>(8);
        }

        public readonly Dictionary<uint, Town> towns = new Dictionary<uint, Town>();

        public long nextRefreshTime;
        public Timer timer;

        public List<uint> GetTowns(bool includeLocked = false, bool needSort = false) {
            List<uint> ls = new List<uint>();
            foreach (var kvp in towns) {
                if (includeLocked || (Sys_NPCFavorability.Instance.zoneNpcs.TryGetValue(kvp.Key, out var _))) {
                    ls.Add(kvp.Key);
                }
            }

            if (needSort) {
                ls.Sort((left, right) => {
                    towns.TryGetValue(left, out var townLeft);
                    towns.TryGetValue(right, out var townRight);
                    if (townLeft != null && townRight != null) {
                        int diff = (int) ((long) townLeft.csvFavority.SortID - (long) townRight.csvFavority.SortID);
                        return diff;
                    }
                    else {
                        return 0;
                    }
                });

                SortStable(ls, (left, right) => {
                    towns.TryGetValue(left, out var townLeft);
                    towns.TryGetValue(right, out var townRight);
                    if (townLeft != null && townRight != null) {
                        var ld = townLeft.Cando ? 1 : 0;
                        var rd = townRight.Cando ? 1 : 0;
                        return ld - rd > 0;
                    }
                    else {
                        return false;
                    }
                });
            }

            return ls;
        }

        public static void SortStable(IList<uint> list, Func<uint, uint, bool> compare) {
            int j;
            for (int i = 1; i < list.Count; ++i) {
                var curr = list[i];

                j = i - 1;
                for (; j >= 0 && compare(curr, list[j]); --j)
                    list[j + 1] = list[j];

                list[j + 1] = curr;
            }
        }

        public bool CanAnyGot {
            get {
                foreach (var kvp in towns) {
                    if (kvp.Value.CanAnyGot) {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}