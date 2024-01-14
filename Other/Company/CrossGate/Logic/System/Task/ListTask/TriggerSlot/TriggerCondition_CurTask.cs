using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Logic;

public class TriggerCondition_CurTask : TriggerSlot
{
    public uint TaskId = 0;

    public TriggerCondition_CurTask() { }

    public TriggerCondition_CurTask(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        TaskId = id;
    }
    protected override void DoChange()
    {
        isFinish = Sys_Task.Instance.GetReceivedTask(TaskId) != null;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toListen);
    }

    public override string GetFailReason()
    {
        return string.Format("不符合任务条件");
    }

    private void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldStatus, ETaskState newStatus)
    {
        if (TaskId == taskEntry.id)
        {
            OnChange();
        }
    }
}
