#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Disposer
{
    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
    [UnityEngine.HideInInspector]
    internal short Base_DrawGizmosEventCount;
}

public interface IDrawGizmos
{
    void OnDrawGizmos();
}

public class GizmosEvents
{
    protected ObjectEvents _objectEvents;

    private List<Disposer> _gizmosEvents = new List<Disposer>();

    public Dictionary<Type, int> m_GizmosTypeDic = new Dictionary<Type, int>();

    public void Init(ObjectEvents objectEvents)
    {
        _objectEvents = objectEvents;
    }

    public void Dispose()
    {
        int gCount = _gizmosEvents.Count;
        if (gCount > 0)
        {
            for (int gIndex = 0; gIndex < gCount; gIndex++)
            {
                var disposer = _gizmosEvents[gIndex];
                if (disposer == null)
                    continue;

                disposer.Base_DrawGizmosEventCount = 0;
            }
            _gizmosEvents.Clear();
        }
    }

    public void AddEvent(Disposer disposer)
    {
        if (disposer is IDrawGizmos)
        {
            _gizmosEvents.Add(disposer);

            ++disposer.Base_DrawGizmosEventCount;
        }
    }

    public void OnDrawGizmos(bool isForce = false)
    {
        int i = 0;
        while (i < _gizmosEvents.Count)
        {
            Disposer disposer = _gizmosEvents[i];
            if (disposer == null)
            {
                _gizmosEvents.RemoveAll(x => x == null);
                continue;
            }

            if (disposer.Id == 0)
            {
                _gizmosEvents.RemoveAt(i);

                --disposer.Base_DrawGizmosEventCount;

#if DEBUG_MODE
                CheckDrawGizmosEventDisposer(disposer);
#endif

                continue;
            }

            IDrawGizmos gizmos = disposer as IDrawGizmos;
            if (gizmos == null)
            {
                _gizmosEvents.RemoveAt(i);

                --disposer.Base_DrawGizmosEventCount;

#if DEBUG_MODE
                CheckDrawGizmosEventDisposer(disposer);
#endif

                continue;
            }

            if (disposer.Base_DrawGizmosEventCount > 1)
            {
                _gizmosEvents.RemoveAt(i);

                --disposer.Base_DrawGizmosEventCount;

#if DEBUG_MODE
                CheckDrawGizmosEventDisposer(disposer);
#endif

                continue;
            }

            i++;

            try
            {
                gizmos.OnDrawGizmos();
            }
            catch (Exception e)
            {
                Lib.Core.DebugUtil.LogError(e.ToString());
            }
        }
    }

    private void CheckDrawGizmosEventDisposer(Disposer disposer)
    {
        if (disposer.Base_DrawGizmosEventCount > 0)
        {
            if (!_gizmosEvents.Contains(disposer))
            {
                Lib.Core.DebugUtil.LogError($"{disposer.GetType()}计数DrawGizmosEventCount:{disposer.Base_DrawGizmosEventCount.ToString()}不在_gizmosEvents中");
                disposer.Base_DrawGizmosEventCount = 0;
            }
        }
        else if (disposer.Base_DrawGizmosEventCount < 0)
        {
            Lib.Core.DebugUtil.LogError($"{disposer.GetType()}计数DrawGizmosEventCount:{disposer.Base_DrawGizmosEventCount.ToString()}");
            disposer.Base_DrawGizmosEventCount = 0;
        }
    }
}
#endif