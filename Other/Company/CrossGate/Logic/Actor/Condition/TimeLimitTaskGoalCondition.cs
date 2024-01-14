using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 条件: 限时任务目标///
    /// </summary>
    public class TimeLimitTaskGoalOnCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TimeLimitTaskGoalOn;
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
            var goal = Sys_Task.Instance.GetTaskGoal(taskID, targetIndex - 1);            if (goal != null && goal.IsTimeLimitOpen)
                return true;
            return false;
        }

        protected override void OnDispose()
        {
            taskID = 0;
            targetIndex = 0;
        }
    }

    public class TimeLimitTaskGoalOffCondition : ConditionBase
    {
        public override EConditionType ConditionType
        {
            get
            {
                return EConditionType.TimeLimitTaskGoalOff;
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
            var goal = Sys_Task.Instance.GetTaskGoal(taskID, targetIndex - 1);            if (goal != null && goal.IsTimeLimitOpen)
                return false;
            return true;
        }

        protected override void OnDispose()
        {
            taskID = 0;
            targetIndex = 0;
        }
    }
}
