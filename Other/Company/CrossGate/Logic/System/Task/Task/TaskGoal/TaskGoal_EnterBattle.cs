using System.Collections.Generic;
using Table;

namespace Logic {
    /// 进入战斗的基类
    public class TaskGoal_EnterBattle : TaskGoal {
        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;
            return this.InitTaskEntry(taskEntry, csv, goalIndex);
        }

        protected override void DoExec(bool auto = true) {
            if (this.csv.PathfindingType == 1) {
                this.NavToNpc(this.csv.PathfindingTargetID, auto);
            }
            else {
                this.PartrolNpc(this.csv.PathfindingTargetID, this.csv.PathfindingMap, auto);
            }
        }
    }
}