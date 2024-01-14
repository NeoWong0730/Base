using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelSystemBase
{
    //private bool isValid = true;
    public bool isActive = true;
    public bool isActivePrev = false;

    public int offsetFrame = 0;
    public int intervalFrame = 24;  //默认按照5帧运行 //最大帧率120帧 最小帧率5帧

#if DEBUG_MODE
    public int LastExecuteFrame = 0;
    public int LastExecuteTime = 0; //microsecond 微秒
    public string name;
#endif

    public virtual bool CanUpdate()
    {
        return Framework.TimeManager.CanExecute(offsetFrame, intervalFrame);
    }

    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void OnCreate() { }
    public virtual void OnDestroy() { }
    public virtual void OnUpdate() { }
}
