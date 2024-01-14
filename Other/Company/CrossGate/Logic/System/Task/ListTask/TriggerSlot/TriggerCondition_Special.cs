using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

// 特殊条件
public class TriggerCondition_Special : TriggerSlot
{
    public uint id;

    public TriggerCondition_Special() { }
    public TriggerCondition_Special(uint id)
    {
        SetArg(id);
    }
    public void SetArg(uint id)
    {
        this.id = id;
    }

    protected override void DoChange()
    {
        // 暂时设置为true，后续条件功能完成的时候再去修改
        isFinish = true;
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {

    }
    public override string GetFailReason()
    {
        return string.Format("特殊条件为达标{0}", id.ToString());
    }
}