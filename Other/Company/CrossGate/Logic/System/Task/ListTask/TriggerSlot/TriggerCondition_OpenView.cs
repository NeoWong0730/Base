using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发条件打开界面
/// 在打开界面动画之前，关闭界面动画之后。
/// </summary>
public class TriggerCondition_OpenView : TriggerSlot
{
    public uint UIId = 0;
    public TriggerCondition_OpenView() { }

    public TriggerCondition_OpenView(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        UIId = id;
    }
    protected override void DoChange()
    {
        isFinish = UIManager.IsOpenState((EUIID)UIId);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnUIChange, toListen);
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIChange, toListen);
    }

    private void OnUIChange(uint stack, int id)
    {
        OnChange();
    }

    public override string GetFailReason()
    {
        return string.Format("不符合界面条件");
    }
}