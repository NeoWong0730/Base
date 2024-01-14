using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:任务目标未完成///
    /// </summary>
    public class TaskTargetUnCompletedCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TaskTargetUnCompleted;
            }
        }

        uint taskID;

        /// <summary>
        /// 数字上的index///
        /// </summary>
        int targetIndex;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
            targetIndex = data[1];
        }

        public override bool IsValid()
        {
            if (Sys_Task.Instance.GetTaskGoalState(taskID, targetIndex - 1) == ETaskGoalState.UnFinish)
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
