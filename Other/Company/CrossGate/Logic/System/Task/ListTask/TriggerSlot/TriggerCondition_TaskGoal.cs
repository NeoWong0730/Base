using Logic;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class TriggerCondition_TaskGoal : TriggerSlot
{
    public uint taskID = 0;
    public int targetIndex = 0;
    public TriggerCondition_TaskGoal() { }

    public TriggerCondition_TaskGoal(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        taskID = id / 10;
        targetIndex = (int)id % 10 - 1;
    }
    protected override void DoChange()
    {
        isFinish = Sys_Task.Instance.GetTaskGoalState(taskID, targetIndex) == ETaskGoalState.Finish;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Task.Instance.eventEmitter.Handle<uint, uint, bool, bool>(Sys_Task.EEvents.OnTaskGoalStatusChanged, OnTaskGoalStatusChanged, toListen);
    }

    public override string GetFailReason()
    {
        return string.Format("不符合任务条件");
    }

    void OnTaskGoalStatusChanged(uint taskId, uint taskGoalId, bool oldState, bool newState)
    {
        if (taskID == taskId)
        {
            OnChange();
        }   
    }
}
