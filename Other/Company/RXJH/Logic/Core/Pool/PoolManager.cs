using Lib.Core;
using Logic;
using System;
using System.Collections.Generic;

public static class PoolManager
{
    private static Dictionary<Type, DisposablePool> allPools = new Dictionary<Type, DisposablePool>();

    public static object Fetch(string fullName)
    {
        Type type = Type.GetType(fullName);
        if (type == null)
        {
            DebugUtil.LogErrorFormat("ERROR!!! PoolManager Fetch failed, TypeName:{0}", fullName);
            return null;
        }
        return Fetch(type);
    }

    public static object Fetch(Type type)
    {
        object item = null;
        if (allPools.TryGetValue(type, out DisposablePool pool) && pool.TryGet(out object rlt))
        {
            item = rlt;
        }
        else
        {
            item = Activator.CreateInstance(type);

#if DEBUG_MODE
            if (pool == null)
            {
                pool = new DisposablePool();
                allPools.Add(type, pool);
            }

            if (pool.allocatorCount < ulong.MaxValue)
            {
                ++pool.allocatorCount;
            }
#endif
        }

#if DEBUG_MODE
        ++pool.fetch;
#endif
        return item;
    }

    public static TClass Fetch<TClass>() where TClass : class, new()
    {
        Type type = typeof(TClass);
        return Fetch(type) as TClass;
    }

    public static void Recycle(object item, bool autoDisposable = true)
    {
        if (item == null)
            return;

        Type type = item.GetType();
        if (!allPools.TryGetValue(type, out DisposablePool pool))
        {
            pool = new DisposablePool();
            allPools.Add(type, pool);
        }

        pool.Recovery(item, autoDisposable);
    }

    public static void SetSize(string fullName, int size)
    {
        Type type = Type.GetType(fullName);
        SetSize(type, size);
    }

    public static void SetSize(Type type, int size)
    {        
        if (type == null)
        {
            return;
        }
        if (!allPools.TryGetValue(type, out DisposablePool pool))
        {
            pool = new DisposablePool();
            allPools.Add(type, pool);
        }

        pool.SetSize(size);
    }

    public static void DisposeAll()
    {
        foreach(var obj in allPools.Values)
        {
            obj.Dispose();
        }
        allPools.Clear();
    }

    public static void Dispose(string fullName)
    {
        Type type = Type.GetType(fullName);
        Dispose(type);
    }

    public static void Dispose(Type type)
    {
        if (type == null)
        {
            return;
        }
        if (allPools.TryGetValue(type, out DisposablePool pool))
        {
            pool.Dispose();
        }
    }

    public static void OnGUI_Pool(UnityEngine.Vector2 size, bool hideNormal)
    {
#if DEBUG_MODE
        foreach (var obj in allPools)
        {
            Type type = obj.Key;
            DisposablePool pool = obj.Value;

            UnityEngine.Color color = UnityEngine.GUI.color;
            if (pool.collectCount > 0)
            {
                UnityEngine.GUI.color = UnityEngine.Color.red;
            }
            else
            {
                if (hideNormal)
                {
                    continue;
                }
            }

            UnityEngine.GUILayout.BeginHorizontal();
            UnityEngine.GUILayout.Label(type.ToString(), UnityEngine.GUILayout.Width(size.x * 0.4f));
            UnityEngine.GUILayout.Label(pool.fetch.ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
            UnityEngine.GUILayout.Label(pool.Count().ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
            UnityEngine.GUILayout.Label(pool.Capacity().ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
            UnityEngine.GUILayout.Label(pool.allocatorCount.ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
            UnityEngine.GUILayout.Label(pool.collectCount.ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
            UnityEngine.GUILayout.EndHorizontal();

            UnityEngine.GUI.color = color;
        }
#endif
    }
}

public class DisposablePool : IDisposable
{
#if DEBUG_MODE
    public ulong allocatorCount = 0;
    public int fetch = 0;
    public ulong collectCount = 0;
    private HashSet<object> hashSet;
#endif
    private int nSize = 64;
    private List<object> pool;

    public DisposablePool(int size = 64)
    {
        nSize = size;
        pool = new List<object>(4);
#if DEBUG_MODE
        hashSet = new HashSet<object>();
#endif
    }

    public void SetSize(int size)
    {
        if (size < 4)
        {
            nSize = 4;
        }
        pool.Capacity = nSize;
    }

    public bool TryGet(out object item)
    {
        if (pool.Count > 0)
        {
            item = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
#if DEBUG_MODE
            hashSet.Remove(item);
#endif
            return true;
        }
        else
        {
            item = null;
            return false;
        }
    }

    public void Recovery(object item, bool autoDisposable)
    {
        if (autoDisposable)
        {
            IDisposable disposable = item as IDisposable;
            disposable?.Dispose();
        }

#if DEBUG_MODE
        /*
        if (hashSet.Contains(item))
        {
            DebugUtil.LogErrorFormat("{0} 重复回收", item.GetType());
            return;
        }

        if (fetch <= 0)
        {
            Type type = item.GetType();
            DebugUtil.LogErrorFormat("{0} 回收次数大于 创建次数", type.ToString());
            EffectUtil.EffectBase effectBase = item as EffectUtil.EffectBase;
            if (effectBase != null)
            {
                DebugUtil.LogErrorFormat($"EEffectTag: {effectBase.EffectTag.ToString()}, EffectGo: {effectBase.EffectGo.name}");
            }
            return;
        }

        --fetch;
        */
#endif

        if (pool.Count >= nSize)
        {
#if DEBUG_MODE
            if (collectCount < ulong.MaxValue)
            {
                ++collectCount;
            }
#endif
            return;
        }

        pool.Add(item);

#if DEBUG_MODE
        hashSet.Add(item);
#endif
    }

    public void Dispose()
    {        
#if DEBUG_MODE
        allocatorCount = 0;
        collectCount = 0;
        fetch = 0;
        hashSet.Clear();
#endif
        pool.Clear();
    }

    public int Count()
    {
        return pool.Count;
    }

    public int Size()
    {
        return nSize;
    }

    public int Capacity()
    {
        return pool.Capacity;
    }
}
