using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;

public class TriggerCondition_RoleJob : TriggerSlot
{
    public List<uint> JobIds = new List<uint>();

    public TriggerCondition_RoleJob() { }

    public TriggerCondition_RoleJob(List<uint> ids)
    {
        SetArg(ids);
    }

    public void SetArg(List<uint> ids)
    {
        JobIds.Clear();
        for (int i = 0, length = ids.Count; i < length; ++i) {
            JobIds.Add(ids[i]);
        }
    }
    protected override void DoChange()
    {
        uint JobID = Sys_Role.Instance.Role.Career;
        isFinish = JobIds.Contains(JobID) || JobIds.Contains(0);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Inaugurate.Instance.eventEmitter.Handle(Sys_Inaugurate.EEvents.OnChangeCareer, OnChange, toListen);
    }

    public override string GetFailReason()
    {
        return string.Format("不符合职业条件");
    }
}
