using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {
    public class Sys_ClueTask : SystemModuleBase<Sys_ClueTask> {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents {
            OnAdventureLevelChanged,
            OnAdventureExpChanged,
            OnDetectiveLevelChanged,
            OnDetectiveExpChanged,

            OnFirstFinish,

            OnClueTaskStatusChanged,
            OnClueTaskPhaseFinished,
            OnClueTaskFinished,
            OnClueTaskTriggered,
        }

        public Dictionary<uint, List<uint>> map2ClueTask = new Dictionary<uint, List<uint>>();
        public Dictionary<uint, ClueTask> tasks = new Dictionary<uint, ClueTask>();
        public List<uint> allMaps = new List<uint>();

        public TaskEntry_Clue current;
        
        public uint _adventureLevel = 1;
        public uint adventureLevel {
            get { return this._adventureLevel; }
            private set {
                uint old = this._adventureLevel;
                if (old != value) {
                    this._adventureLevel = value;
                    this.eventEmitter.Trigger<uint, uint>(EEvents.OnAdventureLevelChanged, old, value);
                }
            }
        }

        public uint _adventureExp;
        public uint adventureExp {
            get { return this._adventureExp; }
            private set {
                uint old = this._adventureExp;
                if (old != value) {
                    this._adventureExp = value;
                    this.eventEmitter.Trigger<uint, uint>(EEvents.OnAdventureExpChanged, old, value);
                }
            }
        }

        public uint _detectiveLevel = 1;
        public uint detectiveLevel {
            get { return this._detectiveLevel; }
            private set {
                uint old = this._detectiveLevel;
                if (old != value) {
                    this._detectiveLevel = value;
                    this.eventEmitter.Trigger<uint, uint>(EEvents.OnDetectiveLevelChanged, old, value);
                }
            }
        }

        public uint _detectiveExp;
        public uint detectiveExp {
            get { return this._detectiveExp; }
            private set {
                uint old = this._detectiveExp;
                if (old != value) {
                    this._detectiveExp = value;
                    this.eventEmitter.Trigger<uint, uint>(EEvents.OnDetectiveExpChanged, old, value);
                }
            }
        }

        public override void Init() {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.ClueDataNtf, this.OnClueDataNtf, CmdTaskClueDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTask.ClueRankNtf, this.OnClueRankNtf, CmdTaskClueRankNtf.Parser);

            Sys_Task.Instance.eventEmitter.Handle<TaskEntry>(Sys_Task.EEvents.OnCurrentTaskChanged, this.OnSyncCurrentTask, true);
        }

        public override void OnLogin() {
            this.Build();
        }
        public override void OnSyncFinished() {
            this.PreCheck();
        }

        private void Build() {
            this.tasks.Clear();
            this.map2ClueTask.Clear();
            this.allMaps.Clear();

            this.allMaps.Add(0);

            var csvDict = CSVClueTask.Instance.GetAll();
            foreach (var kvp in csvDict) {
                ClueTask clue = new ClueTask(kvp.id);
                this.tasks.Add(kvp.id, clue);

                if (kvp.TriggerCondition_UnlockMaps != null) {
                    for (int i = 0, length = kvp.TriggerCondition_UnlockMaps.Count; i < length; ++i) {
                        var mapId = kvp.TriggerCondition_UnlockMaps[i];
                        List<uint> clueTasksInMap = null;
                        if (!this.map2ClueTask.TryGetValue(mapId, out clueTasksInMap)) {
                            clueTasksInMap = new List<uint>();
                            this.map2ClueTask.Add(mapId, clueTasksInMap);

                            this.allMaps.Add(mapId);
                        }

                        if (clueTasksInMap != null && !clueTasksInMap.Contains(kvp.id)) {
                            clueTasksInMap.Add(kvp.id);
                        }
                    }
                }
            }
        }

        public void PreCheck() {
            foreach (var kvp in this.tasks) {
                kvp.Value.PreCheck();
            }
        }

        private void OnClueDataNtf(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eClueTask, "OnClueDataNtf");
            CmdTaskClueDataNtf response = NetMsgUtil.Deserialize<CmdTaskClueDataNtf>(CmdTaskClueDataNtf.Parser, msg);

            for (int i = 0, length = response.ClueTaskInfo.Count; i < length; ++i) {
                var clueTaskUnit = response.ClueTaskInfo[i];
                this.tasks[clueTaskUnit.ClueTaskId].finishTime = clueTaskUnit.FinishTime;
                this.tasks[clueTaskUnit.ClueTaskId].isFirst = clueTaskUnit.Rank == 1;
            }
        }
        private void OnClueRankNtf(NetMsg msg) {
            DebugUtil.LogFormat(ELogType.eClueTask, "OnClueRankNtf");
            CmdTaskClueRankNtf response = NetMsgUtil.Deserialize<CmdTaskClueRankNtf>(CmdTaskClueRankNtf.Parser, msg);

            ClueTask clueTask = this.tasks[response.ClueTaskId];
            clueTask.isFirst = true;

            this.eventEmitter.Trigger<ClueTask>(EEvents.OnFirstFinish, clueTask);
        }

        public List<uint> GetVisibleMaps() {
            List<uint> ids = new List<uint>();
            for (int i = 0, length = this.allMaps.Count; i < length; ++i) {
                var mapId = this.allMaps[i];
                // if map is visible
                ids.Add(mapId);
            }
            return ids;
        }

        public List<uint> GetTasks(uint mapId, EClueTaskType taskType) {
            List<uint> ids = new List<uint>();
            if (mapId != 0) {
                for (int i = 0, length = this.map2ClueTask[mapId].Count; i < length; ++i) {
                    var clueTaskId = this.map2ClueTask[mapId][i];
                    if (this.tasks[clueTaskId].csv.TaskType == (uint)taskType) {
                        ids.Add(clueTaskId);
                    }
                }
            }
            else {
                foreach (var kvp in this.map2ClueTask) {
                    for (int i = 0, length = kvp.Value.Count; i < length; ++i) {
                        var taskId = kvp.Value[i];
                        if (this.tasks[taskId].csv.TaskType == (uint)taskType) {
                            ids.Add(taskId);
                        }

                    }
                }
            }
            return ids;
        }

        public bool IsAllFinish() {
            bool finish = true;
            foreach (var task in this.tasks) {
                if (!task.Value.isFinish) {
                    finish = false;
                    break;
                }
            }
            return finish;
        }

        // 解锁了的线索任务，是否存在未完成的
        public bool IsAnyVisibleButUnFinish() {
            bool exist = false;
            if (Sys_FunctionOpen.Instance.IsOpen(15, false)) {
                foreach (var task in this.tasks) {
                    if (task.Value.isTriggered) {
                        if (!task.Value.isFinish) {
                            exist = true;
                            break;
                        }
                    }
                }
            }

            return exist;
        }

        #region
        private void OnSyncCurrentTask(TaskEntry taskEntry) {
            TaskEntry_Clue clue = (taskEntry as TaskEntry_Clue);
            if (clue != null) {
                this.current = clue;
            }
        }
        #endregion
    }
}