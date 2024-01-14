using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:接受了任务///
    /// </summary>
    public class HaveTaskCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.HaveTask;
            }
        }

        uint taskID;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Task.Instance.GetTaskState(taskID) == ETaskState.UnFinished || Sys_Task.Instance.GetTaskState(taskID) == ETaskState.Finished)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            taskID = 0;
        }
    }
}
