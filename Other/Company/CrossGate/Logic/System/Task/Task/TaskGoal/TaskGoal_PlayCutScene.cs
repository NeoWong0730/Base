using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic {
    /// PlayCutScene = 26
    public class TaskGoal_PlayCutScene : TaskGoal {
        public uint npcID = 0;
        public uint cutsceneId = 0;

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex) {
            this.limit = 1;
            this.npcID = csv.PathfindingTargetID;
            this.cutsceneId = csv.TargetParameter1;
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
            this.NavToNpc(this.npcID, auto);
            //Sys_CutScene.Instance.TryDoCutScene(cutsceneId);
        }
    }
}