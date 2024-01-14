using System.Collections.Generic;
using Table;

namespace Logic {
    /// Pet = 21
    public class TaskGoal_NpcDetectFinished : TaskGoal_CollectItem {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }

        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            // 策划手写文字内容，程序不去填充字段
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex]);
        }
    }
}