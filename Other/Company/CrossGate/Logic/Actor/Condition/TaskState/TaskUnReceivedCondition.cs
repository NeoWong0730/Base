using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 条件:任务状态是未接受///
    /// </summary>
    public class TaskUnReceivedCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TaskUnReceived;
            }
        }

        uint taskID;

        public override void DeserializeObject(List<int> data)
        {
            taskID = (uint)data[0];
        }

        public override bool IsValid()
        {
            ETaskState eTaskState = Sys_Task.Instance.GetTaskState(taskID);
            if (eTaskState == ETaskState.UnReceived || eTaskState == ETaskState.UnReceivedButCanReceive)
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
