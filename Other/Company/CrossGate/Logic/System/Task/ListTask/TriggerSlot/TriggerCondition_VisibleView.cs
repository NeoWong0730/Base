using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic;
using Logic.Core;

/// <summary>
/// 触发条件可见界面(显示界面)
/// 在打开界面动画之前，关闭界面动画之后。
/// </summary>
public class TriggerCondition_VisibleView : TriggerSlot
{
    public uint UIId = 0;
    public TriggerCondition_VisibleView() { }

    public TriggerCondition_VisibleView(uint id)
    {
        SetArg(id);
    }

    public void SetArg(uint id)
    {
        UIId = id;
    }
    protected override void DoChange()
    {
        isFinish = UIManager.IsVisibleAndOpen((EUIID)UIId);
    }
    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        //Sys_Guide.Instance.eventEmitter.Handle(Sys_Guide.EEvents.OnSwitchView_VisibleView, OnChange, toListen);
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