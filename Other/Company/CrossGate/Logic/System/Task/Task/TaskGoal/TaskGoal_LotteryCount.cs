using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic {
    /// LotteryCount = 25, // 抽奖次数
    public class TaskGoal_LotteryCount : TaskGoal {
        public uint npcID = 0;

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;
            this.npcID = csv.PathfindingTargetID;
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
            this.NavToNpc(this.npcID, auto);
        }
    }
}