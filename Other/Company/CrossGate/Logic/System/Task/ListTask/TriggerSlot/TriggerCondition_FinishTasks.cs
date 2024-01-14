using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_FinishTasks : TriggerSlot
{
    public Dictionary<uint, bool> taskIds = new Dictionary<uint, bool>();
    public int finishedCount;
    public int count { get { return taskIds.Count; } }

    public TriggerCondition_FinishTasks() { }
    public TriggerCondition_FinishTasks(List<uint> ids) 
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
        isFinish = true;
        finishedCount = 0;
        dic.Clear();
        foreach (var kvp in taskIds)
        {
            TaskEntry taskEntry = Sys_Task.Instance.GetTask(kvp.Key);
            if (taskEntry != null)
            {
                bool t = taskEntry.taskState >= ETaskState.Submited;
                dic.Add(kvp.Key, t);
                isFinish &= t;
                if (isFinish)
                    ++finishedCount;
            }
            // 不存在的任务
            //else
            //{
            //    isFinish = false;
            //    break;
            //}
        }

        foreach (var kvp in dic)
        {
            taskIds[kvp.Key] = kvp.Value;
        }
    }
    protected override void DoChange()
    {
        Check();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toListen);

    }
    public override string GetFailReason()
    {
        return string.Format("前置任务未完成");
    }
    private void Check()
    {
        isFinish = true;
        finishedCount = 0;
        foreach (var kvp in taskIds)
        {
            isFinish &= kvp.Value;
            if (!isFinish) { break; }
            ++finishedCount;
        }
    }
    private void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldStatus, ETaskState newStatus)
    {
        // 提交状态
        if (newStatus == ETaskState.Submited && taskIds.ContainsKey(taskEntry.id))
        {
            taskIds[taskEntry.id] = true;
            OnChange();
        }
    }
}