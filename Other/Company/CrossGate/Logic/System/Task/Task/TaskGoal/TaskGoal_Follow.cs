using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic {
    /// Follow = 48 跟随
    public class TaskGoal_Follow : TaskGoal {
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
        protected override void OnIverLimitTime() {
            Sys_NpcFollow.Instance.ClearNpcFollowVirtualNpcs();
        }
        public override void OnFinished()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(csv.FollowCompleteTips)));
            if (this.csv.TaskAfterPerform != 0)
            {
                WS_TaskGoalManagerEntity.StartTaskGoal<WS_TaskGoalControllerEntity>(this.csv.TaskAfterPerform, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () =>
                {
                    if (Sys_Team.Instance.HaveTeam)
                    {
                        if (Sys_Team.Instance.isCaptain())
                        {
                            CmdTeamSyncFollowEnd cmdTeamSyncFollowEnd = new CmdTeamSyncFollowEnd();
                            cmdTeamSyncFollowEnd.TaskId = this.taskEntry.id;
                            cmdTeamSyncFollowEnd.Index = (uint) this.goalIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncFollowEnd, cmdTeamSyncFollowEnd);
                        }
                    }
                    else
                    {
                        Sys_NpcFollow.Instance.ClearNpcFollowVirtualNpcs();
                        Sys_Task.Instance.TryContinueDoCurrentTask();
                    }
                }, true, (int)TaskGoalEnum.B_TaskGoalCompleted);
            }
            else
            {
                Sys_NpcFollow.Instance.ClearNpcFollowVirtualNpcs();
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }
    }
}