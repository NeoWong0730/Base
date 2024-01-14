using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:任务状态是已完成///
    /// </summary>
    public class TaskCompletedCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TaskCompleted;
            }
        }

        uint taskID;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            if (Sys_Task.Instance.GetTaskState(taskID) == ETaskState.Finished)
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