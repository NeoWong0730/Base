using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 触发条件打开界面, 且page页签触发点击
/// </summary>
public class TriggerCondition_OpenViewPage : TriggerSlot
{
    private uint UIId;
    private List<string> tabPaths = new List<string>();
    private Lib.Core.Timer timer;

    public TriggerCondition_OpenViewPage() { }

    public TriggerCondition_OpenViewPage(uint uiId, List<string> tabs)
    {
        SetArg(uiId, tabs);
    }

    public void SetArg(uint uiId, List<string> tabs)
    {
        this.UIId = uiId;
        tabPaths = tabs;
    }
    protected override void DoChange()
    {
        bool achive = true;
        for (int i = 0; i < tabPaths.Count; ++i)
        {
            Transform target = UIManager.mRoot.Find(tabPaths[i]);
            if (null != target)
            {
                Toggle toggle = target.GetComponent<Toggle>();
                if (null != toggle)
                {
                    achive &= toggle.isOn;
                }
                CP_Toggle cP_Toggle = target.GetComponent<CP_Toggle>();
                if (null != cP_Toggle)
                {
                    achive &= cP_Toggle.IsOn;
                }
            }
            else
            {
                achive = false;
            }

            if (!achive)
                break;
        }

        isFinish = achive;
    }

    public override void PreCheck()
    {
        DoChange();
    }
    protected override void Listen(bool toListen)
    {
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndEnter, OnEnterUI, toListen);
        UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnExitUI, toListen);
    }

    private void OnEnterUI(uint stack, int id)
    {
        if (this.UIId == id)
        {
            timer?.Cancel();
            timer = Lib.Core.Timer.Register(0.1f, () =>
            {
                OnChange();
                if (isFinish)
                    timer?.Cancel();

            }, null, true);
        }
    }

    private void OnExitUI(uint stack, int id)
    {
        if (this.UIId == id)
        {
            timer?.Cancel();
            timer = null;
        }
    }

    public override string GetFailReason()
    {
        return string.Format("不符合界面条件");
    }
}