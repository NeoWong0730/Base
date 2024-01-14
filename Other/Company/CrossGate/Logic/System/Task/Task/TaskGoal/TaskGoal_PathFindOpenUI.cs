using System.Collections.Generic;
using Table;

namespace Logic {
    /// PathFindOpenUI = 9,   // openUI之前先寻路
    public class TaskGoal_PathFindOpenUI : TaskGoal {
        public uint npcId;

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;

            this.npcId = csv.PathfindingTargetID;
            return base.Init(taskEntry, csv, goalIndex);
        }

        public override void Refresh(uint crt) {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex]);
        }

        protected override void DoExec(bool auto = true) {
            this.NavToNpc(this.npcId, auto);
        }
    }
}