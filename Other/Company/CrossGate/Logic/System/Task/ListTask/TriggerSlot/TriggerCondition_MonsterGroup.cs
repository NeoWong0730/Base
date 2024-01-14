using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Logic;

public class TriggerCondition_MonsterGroup : TriggerSlot
{
    public uint MonsterGroupId = 0;

    public TriggerCondition_MonsterGroup() { }

    public TriggerCondition_MonsterGroup(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        MonsterGroupId = id;
    }

    public override void PreCheck()
    {
        OnChange();
    }

    protected override void DoChange()
    {
        isFinish = Sys_Fight.curMonsterGroupId == MonsterGroupId;
    }
    protected override void Listen(bool toListen)
    {
        ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnBeforeEnterFightEffect, OnChange, toListen);
    }

    public override string GetFailReason()
    {
        return string.Format("不符合怪物组条件");
    }
}
