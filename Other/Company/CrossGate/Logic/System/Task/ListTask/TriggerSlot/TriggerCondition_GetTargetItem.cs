using System.Collections.Generic;
using Logic.Core;
using Logic;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public class TriggerCondition_GetTargetItem : TriggerSlot
{
    public uint itemId;
    public uint itemCount;

    public TriggerCondition_GetTargetItem() { }
    public TriggerCondition_GetTargetItem(uint itemId, uint itemCount = 1) : base()
    {
        SetArg(itemId, itemCount);
    }
    public void SetArg(uint itemId, uint itemCount = 1)
    {
        this.itemId = itemId;
        this.itemCount = itemCount;
    }

    protected override void DoChange()
    {
    }
    protected override void Listen(bool toListen)
    {
        // 监听物品获取事件
    }
    public override string GetFailReason()
    {
        return string.Format("未获取道具{0}", itemId.ToString());
    }

    private void OnItemGot(uint itemId, uint itemCount)
    {
        if (this.itemId == itemId)
        {
            OnChange();
        }
    }
}