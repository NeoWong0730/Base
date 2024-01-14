using System.Collections.Generic;

internal class BaseSystemEvents
{
    protected ObjectEvents _objectEvents;

#if UPDATE_LIST
    protected List<Disposer> _systemEvents = new List<Disposer>();
#else
    protected Queue<Disposer> _systemQueue01 = new Queue<Disposer>();
    protected Queue<Disposer> _systemQueue02 = new Queue<Disposer>();
#endif

    public void Init(ObjectEvents objectEvents)
    {
        _objectEvents = objectEvents;
    }

#if DEBUG_MODE
    protected void CheckSystemEventDisposer(Disposer disposer, ref short eventCount)
    {
#if UPDATE_LIST
            if (eventCount > 0)
            {
                if (!_systemEvents.Contains(disposer))
                {
                    Lib.Core.DebugUtil.LogError($"{disposer.GetType()}����eventCount:{eventCount.ToString()}����_systemEvents��");
                    eventCount = 0;
                }
            }
            else if (eventCount < 0)
            {
                Lib.Core.DebugUtil.LogError($"{disposer.GetType()}����eventCount:{eventCount.ToString()}");
                eventCount = 0;
            }
#else
        if (eventCount > 0)
        {
            if (!_systemQueue01.Contains(disposer) && !_systemQueue02.Contains(disposer))
            {
                Lib.Core.DebugUtil.LogError($"{disposer.GetType()}����eventCount:{eventCount.ToString()}����_systemQueue01��_systemQueue02��");
                eventCount = 0;
            }
        }
        else if (eventCount < 0)
        {
            Lib.Core.DebugUtil.LogError($"{disposer.GetType()}����eventCount:{eventCount.ToString()}");
            eventCount = 0;
        }
#endif
    }
#endif
}