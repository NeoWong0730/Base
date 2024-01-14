using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:任务状态是已提交///
    /// </summary>
    public class TaskSubmittedCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TaskSubmitted;
            }
        }

        uint taskID;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Task.Instance.GetTaskState(taskID) == ETaskState.Submited)
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
