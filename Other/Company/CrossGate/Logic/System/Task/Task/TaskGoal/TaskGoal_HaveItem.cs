using Table;

namespace Logic {
    /// HaveItem = 12,
    public class TaskGoal_HaveItem : TaskGoal {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter2;
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
            this.NavToNpc(this.csv.PathfindingTargetID, auto);
        }
    }
}