using Net;
using Packet;
using Table;

namespace Logic {
    /// Follow = 49 跟踪
    public class TaskGoal_FollowTrack : TaskGoal {
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
            Sys_Track.Instance.ClearTrackVirtualNpcs();
        }
        public override void OnFinished() {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTaskTextContent(CSVTaskLanguage.Instance.GetConfData(csv.TracingCompleteTips)));
            if (this.csv.TaskAfterPerform != 0) {
                WS_TaskGoalManagerEntity.StartTaskGoal<WS_TaskGoalControllerEntity>(this.csv.TaskAfterPerform, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () => {
                    if (Sys_Team.Instance.HaveTeam) {
                        if (Sys_Team.Instance.isCaptain()) {
                            CmdTeamSyncTrackEnd cmdTeamSyncTrackEnd = new CmdTeamSyncTrackEnd();
                            cmdTeamSyncTrackEnd.TaskId = this.taskEntry.id;
                            cmdTeamSyncTrackEnd.Index = (uint)this.goalIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncTrackEnd, cmdTeamSyncTrackEnd);
                        }
                    }
                    else {
                        Sys_Track.Instance.ClearTrackVirtualNpcs();
                        Sys_Task.Instance.TryContinueDoCurrentTask();
                    }
                }, true, (int)TaskGoalEnum.B_TaskGoalCompleted);
            }
            else {
                Sys_Track.Instance.ClearTrackVirtualNpcs();
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }
    }
}