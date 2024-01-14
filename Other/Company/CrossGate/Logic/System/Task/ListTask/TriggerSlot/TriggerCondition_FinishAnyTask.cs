using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_FinishAnyTask : TriggerSlot
{
    public Dictionary<uint, bool> taskIds = new Dictionary<uint, bool>();
    public int count { get { return taskIds.Count; } }

    public TriggerCondition_FinishAnyTask() { }
    public TriggerCondition_FinishAnyTask(List<uint> ids) 
    {
        SetArg(ids);
    }
    public void SetArg(List<uint> ids)
    {
        for (int i = 0, length = ids.Count; i < length; ++i) {
            taskIds.Add(ids[i], false);
        }
    }

    private Dictionary<uint, bool> dic = new Dictionary<uint, bool>();
    public override void PreCheck()
    {
        isFinish = false;
        dic.Clear();
        foreach (var kvp in taskIds)
        {
            TaskEntry taskEntry = Sys_Task.Instance.GetTask(kvp.Key);
            if (taskEntry != null)
            {
                bool t = taskEntry.taskState == ETaskState.Submited;
                dic.Add(kvp.Key, t);
                isFinish |= t;
                if (isFinish) { break; }
            }
            else
            {
                isFinish = true;
            }
        }
        foreach (var kvp in dic)
        {
            taskIds[kvp.Key] = kvp.Value;
        }
    }
    protected override void Listen(bool toListen)
    {
        Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toListen);

    }
    public override string GetFailReason()
    {
        return string.Format("前置任务未完成");
    }
    private void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldStatus, ETaskState newStatus)
    {
        // 提交状态
        if (newStatus == ETaskState.Submited && taskIds.ContainsKey(taskEntry.id))
        {
            taskIds[taskEntry.id] = true;
            isFinish = true;
            OnChange();
        }
    }
}