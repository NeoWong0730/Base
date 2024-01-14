using System;
using System.Collections.Generic;

public partial class Disposer
{
    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    internal short Base_UpdateEventCount;

    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    internal uint Base_UpdateExecuteFrame;
}

public interface IUpdate
{
    void Update();
}

//使用这个模板，只需要对Base_UpdateEventCount，Base_UpdateExecuteFrame，IUpdate三个参数进行修改
internal class UpdateEvents : BaseSystemEvents
{
    public void Dispose()
    {
#if UPDATE_LIST
            int ueCount = _systemEvents.Count;
            if (ueCount > 0) 
            {
                for (int ueIndex = 0; ueIndex < ueCount; ueIndex++)
                {
                    var disposer = _systemEvents[ueIndex];
                    if (disposer == null)
                        continue;

                    disposer.Base_UpdateEventCount = 0;
                }
                _systemEvents.Clear();
            }
#else
        while (_systemQueue01.Count > 0)
        {
            var disposer = _systemQueue01.Dequeue();
            if (disposer != null)
            {
                disposer.Base_UpdateEventCount = 0;
            }
        }
        while (_systemQueue02.Count > 0)
        {
            var disposer = _systemQueue02.Dequeue();
            if (disposer != null)
            {
                disposer.Base_UpdateEventCount = 0;
            }
        }
#endif
    }

    public void AddEvent(Disposer disposer)
    {
        if (disposer is IUpdate)
        {
            //1.当继承了IUpdate接口，但在Update之前直接Dispose会出现这种情况。也就是已经放进_updateEvents，但马上就回收掉了
#if UPDATE_LIST
                _systemEvents.Add(disposer);
#else
            _systemQueue01.Enqueue(disposer);
#endif

            ++disposer.Base_UpdateEventCount;

            disposer.Base_UpdateExecuteFrame = _objectEvents.m_CurRunFrame + 1;
        }
    }

    public void Update()
    {
#if UPDATE_LIST
            int i = 0;
            while (i < _systemEvents.Count)
            {
                Disposer disposer = _systemEvents[i];
                if (disposer == null)
                {
                    _systemEvents.RemoveAll(x => x == null);
                    continue;
                }

                if (disposer.Id == 0)
                {
                    _systemEvents.RemoveAt(i);

                    --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                    CheckSystemEventDisposer(disposer);
#endif

                    continue;
                }

                IUpdate systemEvent = disposer as IUpdate;
                if (systemEvent == null)
                {
                    _systemEvents.RemoveAt(i);

                    --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                    CheckSystemEventDisposer(disposer);
#endif

                    continue;
                }

                if (disposer.Base_UpdateEventCount > 1)
                {
                    _systemEvents.RemoveAt(i);

                    --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                    CheckSystemEventDisposer(disposer);
#endif

                    continue;
                }

                if (disposer.Base_IsBaseKernelOffUpdate)
                {
                    _systemEvents.RemoveAt(i);

                    --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                    CheckSystemEventDisposer(disposer);
#endif

                    continue;
                }

                i++;

                try
                {
                    if (disposer.Base_UpdateExecuteFrame == _objectEvents.m_CurRunFrame)
                    {
                        systemEvent.Update();

                        ILimitFrame limitFrame = disposer as ILimitFrame;
                        if (limitFrame != null && limitFrame.LimitIntervalFrame > 1 &&
                            disposer.Base_UpdateExecuteFrame == _objectEvents.m_CurRunFrame //这步判断的情景是disposer在Update中进行了销毁之后再进行创建
                            ) //限制帧数更新
                        {
                            disposer.Base_UpdateExecuteFrame += limitFrame.LimitIntervalFrame;
                        }
                        else
                            disposer.Base_UpdateExecuteFrame = _objectEvents.m_CurRunFrame + 1;
                    }
                    else if (disposer.Base_UpdateExecuteFrame < _objectEvents.m_CurRunFrame)
                    {
                        Log.Error($"{disposer.GetType().ToString()}的帧为{disposer.Base_UpdateExecuteFrame.ToString()}，当前帧为{_objectEvents.m_CurRunFrame.ToString()}");

                        disposer.Dispose();
                        --i;
                        _systemEvents.RemoveAt(i);
                        --disposer.Base_UpdateEventCount;
                        continue;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());

                    disposer.Dispose();
                    --i;
                    _systemEvents.RemoveAt(i);
                    --disposer.Base_UpdateEventCount;
                    continue;
                }
            }
#else
        while (_systemQueue01.Count > 0)
        {
            Disposer disposer = _systemQueue01.Dequeue();
            if (disposer == null)
            {
                continue;
            }

            if (disposer.Id == 0)
            {
                --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer);
#endif

                continue;
            }

            IUpdate systemEvent = disposer as IUpdate;
            if (systemEvent == null)
            {
                --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer);
#endif

                continue;
            }

            if (disposer.Base_UpdateEventCount > 1)
            {
                --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer);
#endif

                continue;
            }

            if (disposer.Base_IsBaseKernelOffUpdate)
            {
                --disposer.Base_UpdateEventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer);
#endif

                continue;
            }

            try
            {
                if (disposer.Base_UpdateExecuteFrame == _objectEvents.m_CurRunFrame)
                {
                    systemEvent.Update();

                    ILimitFrame limitFrame = disposer as ILimitFrame;
                    if (limitFrame != null && limitFrame.LimitIntervalFrame > 1 &&
                        disposer.Base_UpdateExecuteFrame == _objectEvents.m_CurRunFrame //这步判断的情景是disposer在Update中进行了销毁之后再进行创建
                        ) //限制帧数更新
                    {
                        disposer.Base_UpdateExecuteFrame += limitFrame.LimitIntervalFrame;
                    }
                    else
                        disposer.Base_UpdateExecuteFrame = _objectEvents.m_CurRunFrame + 1;
                }
                else if (disposer.Base_UpdateExecuteFrame < _objectEvents.m_CurRunFrame)
                {
                    Lib.Core.DebugUtil.LogError($"{disposer.GetType().ToString()}的帧为{disposer.Base_UpdateExecuteFrame.ToString()}，当前帧为{_objectEvents.m_CurRunFrame.ToString()}");

                    disposer.Dispose();
                    --disposer.Base_UpdateEventCount;
                    continue;
                }
            }
            catch (Exception e)
            {
                WorkStreamTranstionComponent wstc = disposer as WorkStreamTranstionComponent;
                if (wstc != null && wstc.m_CurUseEntity != null && wstc.m_CurUseEntity.m_StateControllerEntity != null)
                {
#if DEBUG_MODE
                    var sceType = wstc.m_CurUseEntity.m_StateControllerEntity.GetType();
                    string logErrorStr = $"{sceType.ToString()}的[WorkStreamTranstionComponent]---WorkId:[{wstc.m_WorkId.ToString()}]";
                    if (sceType == typeof(WS_NPCControllerEntity) || sceType == typeof(WS_TaskGoalControllerEntity))
                        logErrorStr += $"-----{DLogManager.GetLastLog((int)DLogManager.DLogEnum.WorkStream)}-----{e.ToString()}";
                    else
                        logErrorStr += $"-----{DLogManager.GetLastLog()}-----{e.ToString()}";
                    Lib.Core.DebugUtil.LogError(logErrorStr);
#endif
                }
                else
                    Lib.Core.DebugUtil.LogError($"[{disposer.ToString()}]-----{e.ToString()}");

                disposer.Dispose();
                --disposer.Base_UpdateEventCount;
                continue;
            }

            _systemQueue02.Enqueue(disposer);
        }

        var uq = _systemQueue01;
        _systemQueue01 = _systemQueue02;
        _systemQueue02 = uq;
#endif
    }

#if DEBUG_MODE
    private void CheckSystemEventDisposer(Disposer disposer)
    {
        CheckSystemEventDisposer(disposer, ref disposer.Base_UpdateEventCount);
    }
#endif
}