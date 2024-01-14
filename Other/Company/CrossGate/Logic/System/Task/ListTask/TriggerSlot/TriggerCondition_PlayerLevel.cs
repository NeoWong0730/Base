using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_PlayerLevel : TriggerSlot
{
    public int downLimit = 0;
    public int upLimit;

    public TriggerCondition_PlayerLevel() { }
    public TriggerCondition_PlayerLevel(int down, int up) : base()
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
        int playerLevel = (int)Sys_Role.Instance.Role.Level;
        isFinish = downLimit <= playerLevel && playerLevel <= upLimit;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnChange, toListen);
    }
    public override string GetFailReason()
    {
        return string.Format("等级需要在{0}~{1}֮之间", downLimit.ToString(), upLimit.ToString());
    }
}