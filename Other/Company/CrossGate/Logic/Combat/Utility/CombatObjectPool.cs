using Lib.Core;
using Logic;
using System;
using System.Collections.Generic;

public class CombatObjectPool : Logic.Singleton<CombatObjectPool>
{
    private bool _isWork;

    public void OnAwake()
    {
        _isWork = true;

#if DEBUG_MODE
        foreach (var kv in _dic)
        {
            var val = kv.Value;
            if (val == null)
                continue;

            var disposerArray = kv.Value.GetArray();
            for (int i = 0, length = disposerArray.Length; i < length; i++)
            {
                var d = disposerArray[i];
                if (d == null)
                    continue;

                if (d.Base_UpdateEventCount > 0 || d.Base_IsBaseKernelOffUpdate)
                {
                    Lib.Core.DebugUtil.LogError($"{d.GetType().ToString()}在CombatObjectPool清除的时候未正确把数据清除掉，Base_UpdateEventCount:{d.Base_UpdateEventCount.ToString()}    Base_IsBaseKernelOffUpdate:{d.Base_IsBaseKernelOffUpdate.ToString()}");
                }
            }
        }
#endif
    }

    public void OnEnable()
    {
        _isWork = true;
    }

    public void OnDisable()
    {
        Queue<Type> q = new Queue<Type>();

        foreach (var kv in _dic)
        {
            TQueue<Disposer> disQueue = kv.Value;
            if (disQueue.CheckExpire())
                q.Enqueue(kv.Key);
        }

        while (q.Count > 0)
        {
            _dic.Remove(q.Dequeue());
        }

        foreach (var kv in _freeDic)
        {
            TQueue<object> objQueue = kv.Value;
            if (objQueue.CheckExpire())
                q.Enqueue(kv.Key);
        }

        while (q.Count > 0)
        {
            _freeDic.Remove(q.Dequeue());
        }
    }

    public void OnDestroy()
    {
        _isWork = false;

        Clear();
    }

    public void Clear()
    {
        _dic.Clear();
        _freeDic.Clear();
    }

    #region 针对Disposer的对象池 
    private readonly Dictionary<Type, TQueue<Disposer>> _dic = new Dictionary<Type, TQueue<Disposer>>();

#if UNITY_EDITOR && DEBUG_MODE01
    public List<Disposer> m_NowUseDisposerList = new List<Disposer>();
#endif

    public T Fetch<T>(bool isFromPool) where T : Disposer
    {
        Type type = typeof(T);

        T obj;
        TQueue<Disposer> queue;
        if (isFromPool && _dic.TryGetValue(type, out queue) && queue != null && queue.Count > 0)
            obj = (T)queue.Dequeue();
        else
            obj = (T)Activator.CreateInstance(type);

        if (obj != null)
            obj.Id = IdGenerater.GenerateId();
        else
            Lib.Core.DebugUtil.LogError($"Fetch Disposer Type : {type.ToString()} is null");

#if UNITY_EDITOR && DEBUG_MODE01
            if (m_NowUseDisposerList.Contains(obj))
                Lib.Core.DebugUtil.LogError($"{type.ToString()}在正在使用的List中已经存在了，却在回收队列里面拿来使用");
            else
                m_NowUseDisposerList.Add(obj);
#endif

        return obj;
    }

    public Disposer Fetch(Type type, bool isFromPool)
    {
        Disposer obj;
        TQueue<Disposer> queue;
        if (isFromPool && _dic.TryGetValue(type, out queue) && queue != null && queue.Count > 0)
            obj = queue.Dequeue();
        else
            obj = (Disposer)Activator.CreateInstance(type);

        if (obj != null)
            obj.Id = IdGenerater.GenerateId();
        else
            Lib.Core.DebugUtil.LogError($"Fetch Disposer Type : {type.ToString()} is null");

#if UNITY_EDITOR && X_Debug && X_Developer
        if (m_NowUseDisposerList.Contains(obj))
            Lib.Core.DebugUtil.LogError($"{type.ToString()}在正在使用的List中已经存在了，却在回收队列里面拿来使用");
        else
            m_NowUseDisposerList.Add(obj);
#endif

        return obj;
    }

//    public void Recycle(Disposer obj)
//    {
//        Type type = obj.GetType();
//        TQueue<Disposer> queue;
//        if (!_dic.TryGetValue(type, out queue))
//        {
//            queue = new TQueue<Disposer>(120f);
//            _dic.Add(type, queue);
//        }
//#if X_Developer || X_Debug
//            else if (queue.Count > 0 && queue.IsContain(obj))
//            {
//                Log.Error($"该{type.ToString()}已经被回收过！！！");
//                return;
//            }
//#endif

//#if !UNITY_EDITOR
//            if (queue.Count < queue.MaxCount)
//#endif
//        queue.Enqueue(obj);

//#if UNITY_EDITOR && X_Debug
//            if (queue.Count >= queue.MaxCount)
//            {
//                Log.Error($"回收{type.ToString()}缓存数量已经超量为{queue.Count.ToString()}");
//            }
//#endif

//#if UNITY_EDITOR && X_Debug && X_Developer
//            if (!m_NowUseDisposerList.Remove(obj))
//                Log.Error($"{type.ToString()}被回收却不在使用List中");
//#endif
//    }
    public void Recycle(Disposer obj)
    {
        if (!_isWork)
            return;

        Type type = obj.GetType();
        TQueue<Disposer> queue;
        if (!_dic.TryGetValue(type, out queue))
        {
            queue = new TQueue<Disposer>(120f);
            _dic.Add(type, queue);
        }
        queue.Enqueue(obj);

#if UNITY_EDITOR && DEBUG_MODE01
        if (!m_NowUseDisposerList.Remove(obj))
            DebugUtil.LogError($"{type.ToString()}被回收却不在使用List中");
#endif
    }
    #endregion

    #region 针对所有对象的对象池
    private Dictionary<Type, TQueue<object>> _freeDic = new Dictionary<Type, TQueue<object>>();

#if UNITY_EDITOR && DEBUG_MODE
    private Dictionary<Type, int> _newClassCountDic = new Dictionary<Type, int>();
#endif

    public T Get<T>() where T : class, new()
    {
        Type type = typeof(T);

        T t = null;

        TQueue<object> queue;
        if (_freeDic.TryGetValue(type, out queue) && queue != null && queue.Count > 0)
        {
            t = queue.Dequeue() as T;
        }

        if (t == null)
        {
            t = new T();

#if UNITY_EDITOR && DEBUG_MODE
            if (!_newClassCountDic.TryGetValue(type, out int newCount))
                _newClassCountDic[type] = 1;
            else
                _newClassCountDic[type] = ++newCount;
#endif
        }

        return t;
    }

    public void Push<T>(T obj)
    {
        if (obj == null)
            return;

        Type type = obj.GetType();

        TQueue<object> queue;
        if (!_freeDic.TryGetValue(type, out queue) || queue == null)
        {
            queue = new TQueue<object>(120f);
            _freeDic[type] = queue;
        }
#if DEBUG_MODE
        else if (queue.Count > 0 && queue.IsContain(obj))
        {
            Lib.Core.DebugUtil.LogError($"该{type.ToString()}已经被回收过！！！");
            return;
        }
#endif

#if !UNITY_EDITOR
        if (queue.Count < 127)
#endif
        {
            queue.Enqueue(obj);
        }

#if UNITY_EDITOR && DEBUG_MODE
        if (!_newClassCountDic.TryGetValue(type, out int newCount) || queue.Count > newCount)
            DebugUtil.LogError($"{type.Name}创建的对象数为：{newCount.ToString()}    回收的对象数为：{queue.Count}，可能是有地方直接使用new该类型");
#endif

#if DEBUG_MODE
        if (queue.Count > 30)
        {
            if (queue.Count < 40 && type == typeof(AnimData))
                return;
            
            if (queue.Count < 60 && (type == typeof(WorkStreamTranstionComponent.TranstionStateInfo) ||
                type == typeof(CombatHpChangeData)))
                return;
            
            if (queue.Count < 80 && (type == typeof(BehaveAIControllParam) || type == typeof(ActiveTurnInfo) || 
                type == typeof(SNodeInfo) ||
                type == typeof(TurnBehaveInfo) || type == typeof(TurnBehaveSkillInfo)))
                return;
            
            if (queue.Count < 120 && (type == typeof(WS_BA_EffectData) || type == typeof(CombatModelManager.ModelUseStateStruct) || 
                type == typeof(CombatModelManager.AssetDataClass) ||
                type == typeof(TurnBehaveSkillTargetInfo)))
                return;

            if (queue.Count < 160 && (type == typeof(MobBuffComponent.BuffData) || type == typeof(CombatModelManager.ModelFreeStruct)))
                return;

            if (queue.Count < 180 && (type == typeof(MobBuffComponent.NeedProcessBuffData) || 
                type == typeof(Net_Combat.CacheMobRoundBeginState_Video)))
                return;

            Lib.Core.DebugUtil.LogError($"回收{type.ToString()}缓存数量已经超量为{queue.Count.ToString()}");
        }
#endif
    }
#endregion
}

public abstract class BasePoolClass
{
    private bool _isUse;

    public static T Get<T>() where T : BasePoolClass, new()
    {
        T t = CombatObjectPool.Instance.Get<T>();

        if (t._isUse)
        {
            DebugUtil.LogError($"获取{t.GetType()}类型实例未使用Push()回收");
            return null;
        }

        t.Clear();

        t._isUse = true;

        return t;
    }
    
    public abstract void Clear();

    public void Push()
    {
        if (!_isUse)
        {
            Lib.Core.DebugUtil.LogError($"{GetType()}回收的时候发现已经被回收过");
            return;
        }

        Clear();

        _isUse = false;

        CombatObjectPool.Instance.Push(this);
    }
}
