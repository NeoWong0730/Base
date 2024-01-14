using System.Collections.Generic;
using Table;

namespace Logic {
    /// FinishBattle = 15, // 完成战斗
    public class TaskGoal_FinishBattle : TaskGoal_EnterBattle {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter2;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }
        public override string GetTaskContent() {
            // 填充 个数
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex], this.csv.TargetParameter2.ToString());
        }
    }
}