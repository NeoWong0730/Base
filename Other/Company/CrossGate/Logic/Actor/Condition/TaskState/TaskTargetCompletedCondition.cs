using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:任务目标已完成///
    /// </summary>
    public class TaskTargetCompletedCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TaskTargetCompleted;
            }
        }

        uint taskID;
        int targetIndex;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
            targetIndex = data[1];
        }

        public override bool IsValid()
        {
            if (Sys_Task.Instance.GetTaskGoalState(taskID, targetIndex - 1) == ETaskGoalState.Finish)
            {
                return true;
            }
            return false;
        }

        protected override void OnDispose()
        {
            taskID = 0;
            targetIndex = 0;
        }
    }
}