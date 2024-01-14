using System;
using System.Collections.Generic;
using System.Reflection;

public class WeakDelegate
{
    public struct DelegateEntity
    {
        public bool isStatic;
        public WeakReference reference;
        public MethodInfo method;
    }

    private List<DelegateEntity> delegateEntities = new List<DelegateEntity>();

    public void Add(Action action)
    {
        if (action == null)
            return;

        DelegateEntity delegateEntity = new DelegateEntity()
        {
            isStatic = action.Target == null,
            reference = new WeakReference(action.Target),
            method = action.Method,
        };

        delegateEntities.Add(delegateEntity);
    }

    public void Remove(Action action)
    {
        if (action == null)
            return;

        int i = delegateEntities.Count - 1;
        for (; i >= 0; --i)
        {
            DelegateEntity delegateEntity = delegateEntities[i];
            if (delegateEntity.reference.Target == action.Target && delegateEntity.method == action.Method)
            {
                delegateEntities.RemoveAt(i);
                break;
            }
        }
    }

    public void Set(Action action)
    {
        delegateEntities.Clear();
        Add(action);
    }

    public void Invoke()
    {
        int i = delegateEntities.Count - 1;
        for (; i >= 0; --i)
        {
            DelegateEntity delegateEntity = delegateEntities[i];
            if (delegateEntity.isStatic || delegateEntity.reference.IsAlive)
            {
                delegateEntity.method.Invoke(delegateEntity.reference.Target, null);
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("{0}Method.Target 已经被销毁", delegateEntity.method.Name);
                delegateEntities.RemoveAt(i);
            }
        }
    }
}

public class WeakDelegate<T>
{
    public struct DelegateEntity<T1>
    {
        public bool isStatic;
        public WeakReference reference;
        public MethodInfo method;
    }

    private List<DelegateEntity<T>> delegateEntities = new List<DelegateEntity<T>>();
    
    public void Add(Action<T> action)
    {
        if (action == null)
            return;

        DelegateEntity<T> delegateEntity = new DelegateEntity<T>()
        {
            isStatic = action.Target == null,
            reference = new WeakReference(action.Target),
            method = action.Method,
        };

        delegateEntities.Add(delegateEntity);
    }

    public void Remove(Action<T> action)
    {
        if (action == null)
            return;

        int i = delegateEntities.Count - 1;
        for (; i >= 0; --i)
        {
            DelegateEntity<T> delegateEntity = delegateEntities[i];
            if(delegateEntity.reference.Target == action.Target && delegateEntity.method == action.Method)
            {
                delegateEntities.RemoveAt(i);
                break;
            }
        }
    }

    public void Set(Action<T> action)
    {
        delegateEntities.Clear();
        Add(action);
    }

    public void Invoke(T data)
    {
        int i = delegateEntities.Count - 1;
        for (; i >= 0; --i)
        {
            DelegateEntity<T> delegateEntity = delegateEntities[i];
            if (delegateEntity.isStatic || delegateEntity.reference.IsAlive)
            {
                delegateEntity.method.Invoke(delegateEntity.reference.Target, new object[] { data });
            }
            else
            {
                Lib.Core.DebugUtil.LogWarningFormat("{0}Method.Target 已经被销毁", delegateEntity.method.Name);
                delegateEntities.RemoveAt(i);
            }
        }
    }
}

