using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic {
    /// Convoy = 23, // 护送任务
    public class TaskGoal_Convoy : TaskGoal {
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
            // 策划手写文字内容，程序不去填充字段
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex]);
        }

        protected override void DoExec(bool auto = true) {
            this.NavToNpc(this.npcID, auto);
        }

        protected override void OnIverLimitTime()
        {
            Sys_Escort.Instance.ClearEscortVirtualNpcs();
        }

        public override void OnFinished()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(csv.EscortCompleteTips)));
            if (this.csv.TaskAfterPerform != 0)
            {
                WS_TaskGoalManagerEntity.StartTaskGoal<WS_TaskGoalControllerEntity>(this.csv.TaskAfterPerform, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () =>
                {
                    if (Sys_Team.Instance.HaveTeam)
                    {
                        if (Sys_Team.Instance.isCaptain())
                        {
                            CmdTeamSyncConvoyEnd cmdTeamSyncConvoyEnd = new CmdTeamSyncConvoyEnd();
                            cmdTeamSyncConvoyEnd.TaskId = this.taskEntry.id;
                            cmdTeamSyncConvoyEnd.Index = (uint) this.goalIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncConvoyEnd, cmdTeamSyncConvoyEnd);
                        }
                    }
                    else
                    {
                        Sys_Escort.Instance.ClearEscortVirtualNpcs();
                        Sys_Task.Instance.TryContinueDoCurrentTask();
                    }
                }, true, (int)TaskGoalEnum.B_TaskGoalCompleted);
            }
            else
            {
                Sys_Escort.Instance.ClearEscortVirtualNpcs();
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }
    }
}