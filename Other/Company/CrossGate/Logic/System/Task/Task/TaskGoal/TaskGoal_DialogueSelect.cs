using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic {
    /// DialogueSelect = 10,   // 对话选择
    public class TaskGoal_DialogueSelect : TaskGoal {
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
            string npcName = "";
            string mapName = "";
            CSVNpc.Data csvNpc = CSVNpc.Instance.GetConfData(this.npcID);
            if (csvNpc != null) {
                npcName = LanguageHelper.GetNpcTextContent(csvNpc.name);
                CSVMapInfo.Data csvMap = CSVMapInfo.Instance.GetConfData(csvNpc.mapId);
                if (csvMap != null) {
                    mapName = LanguageHelper.GetTextContent(csvMap.name);
                }
            }
            else {
                DebugUtil.LogErrorFormat("{0} npc is not exist!", this.npcID);
            }

            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex], mapName, npcName);
        }

        protected override void DoExec(bool auto = true) {
            this.NavToNpc(this.npcID, auto);
        }
    }
}