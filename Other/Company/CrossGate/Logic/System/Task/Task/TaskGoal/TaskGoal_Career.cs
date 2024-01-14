using System.Collections.Generic;
using Table;
using Logic.Core;

namespace Logic
{
    /// Career = 18, // 就职
    public class TaskGoal_Career : TaskGoal
    {
        public uint npcID = 0;

        public override TaskGoal Init(TaskEntry taskEntry, CSVTaskGoal.Data csv, int goalIndex)
        {
            this.limit = 1;
            this.npcID = csv.PathfindingTargetID;
            return base.Init(taskEntry, csv, goalIndex);
        }

        public override void Refresh(uint crt)
        {
            this.current = (int)crt;
        }

        public override string GetTaskContent() {
            // 策划手写文字内容，程序不去填充字段
            int finalIndex = this.taskEntry.csvTask.conditionType ? 0 : this.goalIndex;
            return LanguageHelper.GetTaskTextContent(this.taskEntry.csvTask.taskContent[finalIndex]);
        }

        protected override void DoExec(bool auto = true)
        {
            if (this.CanAutoPathFind(auto)) {
                // 只是打开UI，不请求server，server统计进度，然后同步给客户端
                UIManager.OpenUI(EUIID.UI_Career, false);

                Sys_Task.Instance.StopAutoTask(true);
            }
            else {
                this.InterruptDoing();
            }
        }
    }
}