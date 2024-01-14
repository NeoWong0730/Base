using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发条件显示界面(完全显示界面)
/// 在打开界面动画之后，关闭界面动画之前。
/// </summary>
public class TriggerCondition_ShowView : TriggerSlot
{
    public uint UIId = 0;
    public TriggerCondition_ShowView() { }

    public TriggerCondition_ShowView(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        UIId = id;
    }
    protected override void DoChange()
    {
        isFinish = UIManager.IsShowState((EUIID)UIId);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndEnter, OnUIChange, toListen);
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginExit, OnUIChange, toListen);
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