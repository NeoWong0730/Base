using System;

internal abstract class BaseUpdateTemplateSystemEvents<T> : BaseSystemEvents where T : class
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

                DoDisposer(disposer);
            }
            _systemEvents.Clear();
        }
#else
        while (_systemQueue01.Count > 0)
        {
            var disposer = _systemQueue01.Dequeue();
            if (disposer != null)
            {
                DoDisposer(disposer);
            }
        }
        while (_systemQueue02.Count > 0)
        {
            var disposer = _systemQueue02.Dequeue();
            if (disposer != null)
            {
                DoDisposer(disposer);
            }
        }
#endif
    }

    protected abstract void DoDisposer(Disposer disposer);

    public abstract void AddEvent(Disposer disposer);

    protected void AddEvent(Disposer disposer, ref short eventCount, ref uint executeFrame)
    {
        if (disposer is T)
        {
            //1.当继承了T接口，但在更新之前直接Dispose会出现这种情况。也就是已经放进_systemEvents，但马上就回收掉了
#if UPDATE_LIST
            _systemEvents.Add(disposer);
#else
            _systemQueue01.Enqueue(disposer);
#endif

            ++eventCount;

            executeFrame = _objectEvents.m_CurRunFrame + 1;
        }
    }

    public abstract void SystemUpdate();

#if UPDATE_LIST
    protected bool DoSystemData(Disposer disposer, ref short eventCount, ref uint executeFrame, ref int i) 
#else
    protected bool DoSystemData(Disposer disposer, ref short eventCount, ref uint executeFrame)
#endif
    {
#if UPDATE_LIST
            if (disposer == null)
            {
                _systemEvents.RemoveAll(x => x == null);
                return true;
            }

            if (disposer.Id == 0)
            {
                _systemEvents.RemoveAt(i);

                --eventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer, ref eventCount);
#endif

                return true;
            }

            T systemEvent = disposer as T;
            if (systemEvent == null)
            {
                _systemEvents.RemoveAt(i);

                --eventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer, ref eventCount);
#endif

                return true;
            }

            if (eventCount > 1)
            {
                _systemEvents.RemoveAt(i);

                --eventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer, ref eventCount);
#endif

                return true;
            }

            if (disposer.Base_IsBaseKernelOffUpdate)
            {
                _systemEvents.RemoveAt(i);

                --eventCount;

#if DEBUG_MODE
                CheckSystemEventDisposer(disposer, ref eventCount);
#endif

                return true;
            }

            i++;

            try
            {
                if (executeFrame == _objectEvents.m_CurRunFrame)
                {
                    DoSystemInterface(systemEvent);

                    ILimitFrame limitFrame = disposer as ILimitFrame;
                    if (limitFrame != null && limitFrame.LimitIntervalFrame > 1 &&
                        executeFrame == _objectEvents.m_CurRunFrame //这步判断的情景是disposer在Update中进行了销毁之后再进行创建
                        ) //限制帧数更新
                    {
                        executeFrame += limitFrame.LimitIntervalFrame;
                    }
                    else
                        executeFrame = _objectEvents.m_CurRunFrame + 1;
                }
                else if (executeFrame < _objectEvents.m_CurRunFrame)
                {
                    Log.Error($"{disposer.GetType().ToString()}的帧为{executeFrame.ToString()}，当前帧为{_objectEvents.m_CurRunFrame.ToString()}");

                    disposer.Dispose();
                    --i;
                    _systemEvents.RemoveAt(i);
                    --eventCount;
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());

                disposer.Dispose();
                --i;
                _systemEvents.RemoveAt(i);
                --eventCount;
                return true;
            }

#else
        if (disposer == null)
        {
            return true;
        }

        if (disposer.Id == 0)
        {
            --eventCount;

#if DEBUG_MODE
            CheckSystemEventDisposer(disposer, ref eventCount);
#endif

            return true;
        }

        T systemEvent = disposer as T;
        if (systemEvent == null)
        {
            --eventCount;

#if DEBUG_MODE
            CheckSystemEventDisposer(disposer, ref eventCount);
#endif

            return true;
        }

        if (eventCount > 1)
        {
            --eventCount;

#if DEBUG_MODE
            CheckSystemEventDisposer(disposer, ref eventCount);
#endif

            return true;
        }

        if (disposer.Base_IsBaseKernelOffUpdate)
        {
            --eventCount;

#if DEBUG_MODE
            CheckSystemEventDisposer(disposer, ref eventCount);
#endif

            return true;
        }

        try
        {
            if (executeFrame == _objectEvents.m_CurRunFrame)
            {
                DoSystemInterface(systemEvent);

                ILimitFrame limitFrame = disposer as ILimitFrame;
                if (limitFrame != null && limitFrame.LimitIntervalFrame > 1 &&
                    executeFrame == _objectEvents.m_CurRunFrame //这步判断的情景是disposer在Update中进行了销毁之后再进行创建
                    ) //限制帧数更新
                {
                    executeFrame += limitFrame.LimitIntervalFrame;
                }
                else
                    executeFrame = _objectEvents.m_CurRunFrame + 1;
            }
            else if (executeFrame < _objectEvents.m_CurRunFrame)
            {
                Lib.Core.DebugUtil.LogError($"很严重的bug，发现者需要把error下“所有”的log记录下来：{disposer.GetType().ToString()}的帧为{executeFrame.ToString()}，当前帧为{_objectEvents.m_CurRunFrame.ToString()}");

                disposer.Dispose();
                --eventCount;
                return true;
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
            --eventCount;
            return true;
        }
#endif

        return false;
    }

    protected abstract void DoSystemInterface(T systemEvent);
}