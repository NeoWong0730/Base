using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;

/// <summary>
/// NPC好感解锁
/// </summary>
public class TriggerCondition_NPCFavorability : TriggerSlot
{
    public bool isFavorability = false;
    public TriggerCondition_NPCFavorability() { }

    public TriggerCondition_NPCFavorability(bool value)
    {
        SetArg(value);
    }

    public void SetArg(bool value)
    {
        isFavorability = value;
    }
    protected override void DoChange()
    {
        isFinish = Sys_NPCFavorability.Instance.AnyNpcUnlocked;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_NPCFavorability.Instance.eventEmitter.Handle<uint, uint>(Sys_NPCFavorability.EEvents.OnNPCUnlock, OnNPCUnlock, toListen);
    }

    private void OnNPCUnlock(uint zoneId, uint npcId)
    {
        OnChange();
    }

    public override string GetFailReason()
    {
        return string.Format("不符合条件");
    }
}
