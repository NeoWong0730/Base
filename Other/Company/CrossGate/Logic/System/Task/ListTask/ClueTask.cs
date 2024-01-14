using System;
using System.Collections.Generic;
using Table;

namespace Logic {
    public enum EClueTaskType {
        None = 0,
        Detective = 1,
        Adventure = 2,
        Experience = 3,
    }
    public enum EClueTaskStatus {
        Tracing,
        GotClue,
        NoClue,
        Finished,
        UnTriggered,
    }

    public class ClueTask {
        public uint id;

        private CSVClueTask.Data _csv;
        public CSVClueTask.Data csv {
            get {
                if (this._csv == null) {
                    this._csv = CSVClueTask.Instance.GetConfData(this.id);
                }
                return this._csv;
            }
        }

        public EClueTaskStatus taskStatus = EClueTaskStatus.UnTriggered;
        // 使用前刷新状态
        public EClueTaskStatus RefreshStatus() {
            if (this.isFinish) { this.taskStatus = EClueTaskStatus.Finished; }
            else if (this.isTracing) { this.taskStatus = EClueTaskStatus.Tracing; }
            else if (!this.CanVisible) { this.taskStatus = EClueTaskStatus.NoClue; }
            else { this.taskStatus = EClueTaskStatus.GotClue; }
            return this.taskStatus;
        }

        public QueueDictionary<uint, ClueTaskPhase> phases = new QueueDictionary<uint, ClueTaskPhase>();
        public ClueTaskPhase currentPhase {
            get {
                bool exist = this.GetCurrent(out int _, out uint currentPhaseId);
                if (exist) {
                    return this.phases[currentPhaseId];
                }
                return null;
            }
        }

        // 触发条件
        public TriggerSlotGroup triggerGroup = new TriggerSlotGroup();
        // 完成条件
        public TriggerSlotGroup finishGroup = new TriggerSlotGroup();

        // server data
        public uint finishTime;
        public bool isFirst = false;

        public bool isTriggered { get { return this.triggerGroup.isFinish; } }
        public bool isFinish { get { return this.finishGroup.isFinish; } }
        public bool isTracing {
            get {
                bool result = this.GetCurrent(out int index, out uint phaseId);
                if (result/*phaseId != 0*/) {
                    result = this.phases.GetValueByKey(phaseId).anyTracing;
                }
                return result;
            }
        }
        public bool CanVisible {
            get {
                return this.phases.FirstValue.CanVisible;
            }
        }
        public ClueTask(uint id) {
            this.id = id;
            this.taskStatus = EClueTaskStatus.UnTriggered;

            this.phases.Clear();
            this.triggerGroup.Clear();
            this.finishGroup.Clear();
            if (this.csv.PhasedTasksGroup != null) {
                ClueTaskPhase pre = null;
                //UnityEngine.Debug.LogError(id + "  " + (this.csv.PhasedTasksGroup.Count));
                for (int i = 0, length = this.csv.PhasedTasksGroup.Count; i < length; ++i) {
                    var phaseId = this.csv.PhasedTasksGroup[i];
                    ClueTaskPhase phase = new ClueTaskPhase(phaseId);
                    phase.SetOwner(this);

                    // setPre
                    phase.prePhase = pre;
                    pre = phase;

                    this.phases.Enqueue(phaseId, phase);
                }

                // setNext
                for (int i = 0, count = this.csv.PhasedTasksGroup.Count; i < count; ++i) {
                    ClueTaskPhase current = this.phases[this.csv.PhasedTasksGroup[i]];
                    ClueTaskPhase next = null;
                    if (i < count - 1) {
                        next = this.phases[this.csv.PhasedTasksGroup[i + 1]];
                    }
                    current.nextPhase = next;
                }

                //UnityEngine.Debug.LogError(id + "  " + this.phases.Count + " " + (phases.GetHashCode()));
            }

            List<TriggerSlot> triggers = new List<TriggerSlot>();

            TriggerSlot trigger;
            string typeName = nameof(this.csv.TriggerCondition_DetectiveLevel);
            Type type = Type.GetType(typeName);
            if (type != null) {
                trigger = Activator.CreateInstance(type) as TriggerSlot;
                (trigger as TriggerCondition_DetectiveLevel).SetArg((int)this.csv.TriggerCondition_DetectiveLevel, int.MaxValue);
                triggers.Add(trigger);
            }

            typeName = nameof(this.csv.TriggerCondition_AdventureLevel);
            type = Type.GetType(typeName);
            if (type != null) {
                trigger = Activator.CreateInstance(type) as TriggerSlot;
                (trigger as TriggerCondition_AdventureLevel).SetArg((int)this.csv.TriggerCondition_AdventureLevel, int.MaxValue);
                triggers.Add(trigger);
            }

            typeName = nameof(this.csv.TriggerCondition_PlayerLevel);
            type = Type.GetType(typeName);
            if (type != null) {
                trigger = Activator.CreateInstance(type) as TriggerSlot;
                (trigger as TriggerCondition_PlayerLevel).SetArg((int)this.csv.TriggerCondition_PlayerLevel, int.MaxValue);
                triggers.Add(trigger);
            }

            if (this.csv.TriggerCondition_FinishTasks != null) {
                typeName = nameof(this.csv.TriggerCondition_FinishTasks);
                type = Type.GetType(typeName);
                if (type != null) {
                    trigger = Activator.CreateInstance(type) as TriggerSlot;
                    (trigger as TriggerCondition_FinishTasks).SetArg(this.csv.TriggerCondition_FinishTasks);
                    triggers.Add(trigger);
                }
            }

            if (this.csv.TriggerCondition_UnlockMaps != null) {
                typeName = nameof(this.csv.TriggerCondition_UnlockMaps);
                type = Type.GetType(typeName);
                if (type != null) {
                    trigger = Activator.CreateInstance(type) as TriggerSlot;
                    (trigger as TriggerCondition_UnlockMaps).SetArg(this.csv.TriggerCondition_UnlockMaps);
                    triggers.Add(trigger);
                }
            }

            typeName = nameof(this.csv.TriggerCondition_Special);
            type = Type.GetType(typeName);
            if (type != null) {
                trigger = Activator.CreateInstance(type) as TriggerSlot;
                (trigger as TriggerCondition_Special).SetArg(this.csv.TriggerCondition_Special);
                triggers.Add(trigger);
            }

            this.triggerGroup.SetTriggerList(triggers);
            this.triggerGroup.onFull = () => {
                this.taskStatus = EClueTaskStatus.NoClue;
                Sys_ClueTask.Instance.eventEmitter.Trigger<ClueTask>(Sys_ClueTask.EEvents.OnClueTaskTriggered, this);
            };

            if (this.csv.PhasedTasksGroup != null) {
                List<TriggerSlot> conditions = new List<TriggerSlot>(2);
                for (int i = 0, length = this.csv.PhasedTasksGroup.Count; i < length; ++i) {
                    TriggerCondition_FinishClueTaskPhases fTrigger = new TriggerCondition_FinishClueTaskPhases(new List<uint>() { this.csv.PhasedTasksGroup[i] });
                    fTrigger.taskId = id;
                    conditions.Add(fTrigger);
                }
                this.finishGroup.SetTriggerList(conditions);
                this.finishGroup.onFull = () => {
                    this.taskStatus = EClueTaskStatus.Finished;
                    Sys_ClueTask.Instance.eventEmitter.Trigger<ClueTask>(Sys_ClueTask.EEvents.OnClueTaskFinished, this);
                };
                //finishGroup.onTrueCountChanged = (trueCount) =>
                //{
                //    UnityEngine.Debug.LogError(id + "  " + trueCount);
                //};
            }
        }

        public void PreCheck() {
            foreach (var kvp in this.phases.dictionary) {
                kvp.Value.PreCheck();
            }

            this.triggerGroup.PreCheck();
            this.finishGroup.PreCheck();

            this.triggerGroup.TryListen(true);
            this.finishGroup.TryListen(true);
        }

        // 为了定位
        public bool GetCurrent(out int index, out uint id) {
            //UnityEngine.Debug.LogError(this.id + " ==== " + phases.Count + "  " + (phases.GetHashCode()));
            bool ret = false;
            index = 0;
            id = this.phases.LastKey;
            if (this.taskStatus != EClueTaskStatus.Finished) {
                for (int i = 0; i < this.phases.Count; ++i) {
                    if (!this.phases.GetValueByIndex(i).isFinish) {
                        index = i;
                        id = this.phases.GetKeyByIndex(i);
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}