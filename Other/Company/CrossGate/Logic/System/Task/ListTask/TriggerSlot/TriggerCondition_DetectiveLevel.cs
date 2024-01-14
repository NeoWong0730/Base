using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_DetectiveLevel : TriggerSlot
{
    public int downLimit;
    public int upLimit;

    public TriggerCondition_DetectiveLevel() { }
    public TriggerCondition_DetectiveLevel(int down, int up) : base()
    {
        SetArg(down, up);
    }
    public void SetArg(int down, int up)
    {
        this.downLimit = down;
        this.upLimit = up;
    }

    protected override void DoChange()
    {
        int level = (int)Sys_ClueTask.Instance.detectiveLevel;
        isFinish = downLimit <= level && level <= upLimit;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_ClueTask.Instance.eventEmitter.Handle<uint, uint>(Sys_ClueTask.EEvents.OnDetectiveLevelChanged, OnLevelChanged, toListen);
    }
    public override string GetFailReason()
    {
        return string.Format("等级需要在{0}~{1}֮之间", downLimit.ToString(), upLimit.ToString());
    }

    private void OnLevelChanged(uint oldLevel, uint newLevel)
    {
        OnChange();
    }
}