using Lib.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public sealed class ObjectEvents
{
    private static ObjectEvents _instance;
    public static ObjectEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ObjectEvents();
                _instance.Init();
            }
            return _instance;
        }
    }

    public uint m_CurRunFrame;

    private Queue<Disposer> _startEvents = new Queue<Disposer>();

    private UpdateEvents _updateEvents = new UpdateEvents();

#if UNITY_EDITOR
    private GizmosEvents _gizmosEvents = new GizmosEvents();
#endif

    public void Dispose()
    {
        _startEvents.Clear();

        _updateEvents.Dispose();

#if UNITY_EDITOR
        _gizmosEvents.Dispose();
#endif
    }

    private void Init()
    {
        _updateEvents.Init(this);

#if UNITY_EDITOR
        _gizmosEvents.Init(this);
#endif
    }

    public void AddEvent(Disposer disposer)
    {
        IAwake awake = disposer as IAwake;
        if (awake != null)
            awake.Awake();

        if (disposer is IStart)
            _startEvents.Enqueue(disposer);

        _updateEvents.AddEvent(disposer);

#if UNITY_EDITOR
        _gizmosEvents.AddEvent(disposer);
#endif
    }

    public void AddSystemUpdateEvent(Disposer disposer)
    {
        _updateEvents.AddEvent(disposer);
    }

    private void Start()
    {
        while (_startEvents.Count > 0)
        {
            Disposer disposer = _startEvents.Dequeue();
            if (disposer == null || disposer.Id == 0)
                continue;
            IStart start = disposer as IStart;
            if (start == null)
                continue;

            try
            {
                start.Start();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }
    
    public void Update()
    {
        m_CurRunFrame++;

        Start();

        _updateEvents.Update();
    }

#if UNITY_EDITOR
    public void OnDrawGizmos(bool isForce = false)
    {
        _gizmosEvents.OnDrawGizmos(isForce);
    }

    public Dictionary<Type, int> GetGizmosTypeDic()
    {
        return _gizmosEvents.m_GizmosTypeDic;
    }
#endif
}

public interface IAwake
{
    void Awake();
}

public interface IStart
{
    void Start();
}

public interface ILimitFrame
{
    uint LimitIntervalFrame { get; set; }
}
