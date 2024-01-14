using System.Collections.Generic;
using Table;

namespace Logic {
    /// KillMonster = 16, // 击杀怪物
    public class TaskGoal_KillMonster : TaskGoal_EnterBattle {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = (int)csv.TargetParameter3;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }
        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            // 杀死{0 count}

            // 策划手写文字内容，程序不去填充字段
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex], this.csv.TargetParameter3.ToString());
        }
    }
}