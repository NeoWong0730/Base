using System.Collections.Generic;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;
using UnityEngine;
using System;

public abstract class TriggerSlot
{
    // 条件关心的事件
    private bool hasListened = false;

    public bool isFinish { get; protected set; } = false;
    public Action<TriggerSlot, bool> onCondition;

    public TriggerSlot()
    {
    }
    public void Reset()
    {
        isFinish = false;
        onCondition = null;
        TryListen(false);
    }
    public void TryListen(bool toListen)
    {
        if (!hasListened && toListen)
        {
            Listen(true);
            hasListened = true;
        }
        else if (!toListen)
        {
            Listen(false);
            hasListened = false;
        }
    }
    protected virtual void Listen(bool toListen)
    {

    }
    public virtual void PreCheck()
    {

    }

    // 每个事件的监听函数
    protected void OnChange()
    {
        // set flag
        DoChange();
        onCondition?.Invoke(this, isFinish);
    }
    protected virtual void DoChange() { }
    public abstract string GetFailReason();
}

public class TriggerSlotGroup
{
    public class ConditionWarpper
    {
        public bool isFinish = false;
        public ConditionWarpper(bool isFinish) { this.isFinish = isFinish; }
    }

    public Dictionary<TriggerSlot, ConditionWarpper> triggers { get; private set; } = new Dictionary<TriggerSlot, ConditionWarpper>(4);
    public int trueCount { get; private set; } = 0;
    public int count { get { return triggers.Count; } }

    public Action onFull;
    public Action<int> onTrueCountChanged;

    public bool isFinish { get { return trueCount >= triggers.Count; } }

    public TriggerSlotGroup()
    {
    }

    public TriggerSlotGroup(List<TriggerSlot> list)
    {
        SetTriggerList(list);
    }
    public void SetTriggerList(List<TriggerSlot> list)
    {
        trueCount = 0;
        triggers.Clear();

        if (triggers != null && list.Count > 0)
        {
            for (int i = 0, count = list.Count; i < count; ++i)
            {
                // if reuse may be error
                list[i].onCondition = OnCondition;
                triggers.Add(list[i], new ConditionWarpper(false));
            }
        }
    }
    public TriggerSlotGroup TryListen(bool toListen)
    {
        foreach (var t in triggers)
        {
            t.Key.TryListen(toListen);
        }
        return this;
    }
    public TriggerSlotGroup PreCheck()
    {
        foreach (var t in triggers)
        {
            t.Key.PreCheck();
            OnCondition(t.Key, t.Key.isFinish);
        }
        return this;
    }
    private void OnCondition(TriggerSlot condition, bool conditionFinish)
    {
        bool oldFlag = triggers[condition].isFinish;
        triggers[condition].isFinish = conditionFinish;

        if (oldFlag != conditionFinish)
        {
            trueCount = conditionFinish ? trueCount + 1 : trueCount - 1;
            onTrueCountChanged?.Invoke(trueCount);
        }

        if (trueCount >= triggers.Count)
        {
            onFull?.Invoke();
        }
    }

    public bool Can(out TriggerSlot triggerCondition)
    {
        bool can = true;
        triggerCondition = default;
        foreach (var e in triggers)
        {
            triggerCondition = e.Key;
            if (!triggerCondition.isFinish)
            {
                can = false;
                break;
            }
        }
        return can;
    }
    public void Clear()
    {
        TryListen(false);
        triggers?.Clear();
    }
}