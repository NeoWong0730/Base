using System.Collections.Generic;
using Table;

namespace Logic {
    /// FinishOtherTask = 5,
    public class TaskGoal_FinishOtherTask : TaskGoal {
        private readonly List<uint> ids = new List<uint>();
        private readonly List<TaskEntry> _toDos = new List<TaskEntry>();

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter1;

            this.ids.Clear();
            if (csv.TargetParameter2 != 0) { this.ids.Add(csv.TargetParameter2); }
            if (csv.TargetParameter3 != 0) { this.ids.Add(csv.TargetParameter3); }
            if (csv.TargetParameter4 != 0) { this.ids.Add(csv.TargetParameter4); }
            if (csv.TargetParameter5 != 0) { this.ids.Add(csv.TargetParameter5); }
            if (csv.TargetParameter6 != 0) { this.ids.Add(csv.TargetParameter6); }
            if (csv.TargetParameter7 != 0) { this.ids.Add(csv.TargetParameter7); }

            return base.Init(taskEntry, csv, goalIndex);
        }
        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex], this.current.ToString(), this.limit.ToString());
        }

        protected override void DoExec(bool auto = true) {
            TaskEntry subEntry = this.Find(this.taskEntry, this.ids);
            if (subEntry != null) {
                // this.taskEntry.childTaskEntry = subEntry;
                // subEntry.parentTaskEntry = this.taskEntry;

                if(subEntry.taskState == ETaskState.UnFinished && !subEntry.isTraced) {
                    Sys_Task.Instance.ReqTrace(subEntry.id, true, false);
                }
                Sys_Task.Instance.TryDoTask(subEntry, true, auto);
            }
            else {
                this.InterruptDoing();
            }
        }

        private TaskEntry Find(TaskEntry mainTaskEntry, List<uint> toDoIds) {
            this._toDos.Clear();
            for (int i = 0, length = toDoIds.Count; i < length; ++i) {
                var id = toDoIds[i];
                TaskEntry entry = Sys_Task.Instance.GetTask(id, true);
                if (entry != null && entry.taskState != ETaskState.Submited) {
                    this._toDos.Add(entry);
                }
            }

            return this._toDos.Count > 0 ? this._toDos[0] : null;
        }
    }
}