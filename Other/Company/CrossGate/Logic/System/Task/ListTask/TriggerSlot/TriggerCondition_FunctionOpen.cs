using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_FunctionOpen : TriggerSlot
{
    public uint openID = 0;
    public TriggerCondition_FunctionOpen() { }
    public TriggerCondition_FunctionOpen(uint id) : base()
    {
        SetArg(id);
    }
    public void SetArg(uint id)
    {
        this.openID = id;
    }
    protected override void DoChange()
    {
        isFinish = Sys_FunctionOpen.Instance.IsOpen(openID, false);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnFunctionOpen, toListen);
    }
    public override string GetFailReason()
    {
        return string.Format("需要功能推送ID:{0}", openID.ToString());
    }
    public void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
    {
        if (openID != functionOpenData.id) return;
        OnChange();
    }
}