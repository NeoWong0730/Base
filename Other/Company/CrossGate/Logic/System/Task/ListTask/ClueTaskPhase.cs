using System.Collections.Generic;
using Table;

namespace Logic {
    public class ClueTaskPhase {
        public uint id;
        public CSVCluePhasedTasksGroup.Data csv;
        public ClueTask owner;

        public ClueTaskPhase prePhase;
        public ClueTaskPhase nextPhase;

        private readonly QueueDictionary<uint, TaskEntry> tasks = new QueueDictionary<uint, TaskEntry>();

        private readonly TriggerSlotGroup triggers = new TriggerSlotGroup();

        public bool isFinish { get; private set; } = false;
        public bool CanVisible {
            get {
                return this.tasks.FirstValue.taskState >= ETaskState.UnFinished;
            }
        }
        public bool anyTracing {
            get {
                bool result = this.GetCurrent(out int index, out uint taskId);
                if (result) {
                    result = Sys_Task.Instance.GetTask(taskId).isTraced;
                }
                return result;
            }
        }

        public ClueTaskPhase(CSVCluePhasedTasksGroup.Data csv) {
            this.csv = csv;
            this.id = csv.id;
            this.tasks.Clear();
            this.triggers.Clear();

            if (csv.SubTask != null) {
                for (int i = 0, length = csv.SubTask.Count; i < length; ++i) {
                    var id = csv.SubTask[i];
                    TaskEntry taskEntry = Sys_Task.Instance.GetTask(id);
                    if (taskEntry != null) {
                        (taskEntry as TaskEntry_Clue).SetOwner(this);
                        this.tasks.Enqueue(id, taskEntry);
                    }
                }

                TriggerCondition_FinishTasks trigger = new TriggerCondition_FinishTasks(csv.SubTask);
                this.triggers.SetTriggerList(new List<TriggerSlot> { trigger });
                this.triggers.onFull = () => {
                    this.isFinish = true;
                    Sys_ClueTask.Instance.eventEmitter.Trigger<ClueTaskPhase>(Sys_ClueTask.EEvents.OnClueTaskPhaseFinished, this);
                };
                //triggers.onTrueCountChanged = (trueCount) =>
                //{
                //    UnityEngine.Debug.LogError(id + " phase " + trueCount);
                //};
            }
        }
        public ClueTaskPhase(uint id) : this(CSVCluePhasedTasksGroup.Instance.GetConfData(id)) { }
        public ClueTaskPhase SetOwner(ClueTask clueTask) {
            this.owner = clueTask;
            return this;
        }
        public void PreCheck() {
            // œ»prechec£¨»ª∫Ûlisten
            this.triggers.PreCheck();
            this.triggers.TryListen(true);
        }

        public bool GetCurrent(out int index, out uint id) {
            bool ret = false;
            index = 0;
            id = this.tasks.LastKey;
            if (!this.isFinish) {
                for (int i = 0; i < this.tasks.Count; ++i) {
                    if (!(this.tasks.GetValueByIndex(i).taskState >= ETaskState.Submited)) {
                        index = i;
                        id = this.tasks.GetKeyByIndex(i);
                        ret = true;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}