using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;

public static class EntityFactory
{
    /// <summary>
    /// 用完必须保证为Count为0或者clear，以确保下次使用不会出错
    /// </summary>
    public static Queue<long> Temporary_RemoveIdQueue = new Queue<long>();

    public static T Create<T>(bool isFromPool = true) where T : Disposer
    {
        T dis = CombatObjectPool.Instance.Fetch<T>(isFromPool);
        if (dis == null)
            return null;
        
        dis.Base_IsFromPool = isFromPool;
        ObjectEvents.Instance.AddEvent(dis);
        return dis;
    }

    public static T CreateWithId<T>(long id, bool isFromPool = true) where T : Disposer
    {
        T dis = CombatObjectPool.Instance.Fetch<T>(isFromPool);
        if (dis == null)
            return null;
        
        dis.Base_IsFromPool = isFromPool;
        dis.Id = id;
        ObjectEvents.Instance.AddEvent(dis);
        return dis;
    }
}

public static class ComponentFactory
{
    public static T Create<T>(AEntity entity, bool isFromPool = true) where T : AComponent
    {
        T dis = CombatObjectPool.Instance.Fetch<T>(isFromPool);
        if (dis == null)
            return null;

        dis.m_Entity = entity;
        dis.Base_IsFromPool = isFromPool;

        return dis;
    }

    public static AComponent Create(AEntity entity, Type type, bool isFromPool = true)
    {
        AComponent dis = (AComponent)CombatObjectPool.Instance.Fetch(type, isFromPool);
        if (dis == null)
            return null;

        dis.m_Entity = entity;
        dis.Base_IsFromPool = isFromPool;

        return dis;
    }

    /// <summary>
    /// 创建子entity，系统内部使用，外部不要调用
    /// </summary>
    public static T Base_CreateChildEntity<T>(AEntity parent, bool isFromPool = true) where T : AEntity
    {
        T dis = CombatObjectPool.Instance.Fetch<T>(isFromPool);
        if (dis == null)
            return null;
        
        dis.Parent = parent;
        dis.Base_IsFromPool = isFromPool;

        return dis;
    }
}

public class AEntity : Disposer
{
    public AEntity Parent;
    private Dictionary<Type, AEntity> _entityDict = new Dictionary<Type, AEntity>();
    private Dictionary<Type, AComponent> _componentDict = new Dictionary<Type, AComponent>();

#if UNITY_EDITOR
    public Dictionary<Type, AEntity> m_ChildEntityDic { get { return _entityDict; } }
    public Dictionary<Type, AComponent> m_ComponentDict { get { return _componentDict; } }
#endif

    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    protected bool BaseAEntity_NoNeedRecycleInDispose = false;

    public override void Dispose()
    {
        if (Id == 0)
            return;

        //这里设置的原因是该Entity必须把身上挂载的组件全都卸载干净了才能被回收利用，
        //否者会出现自身被回收，卸载组件的时候又被从池里拿出来利用导致很多奇怪的错误
        Base_NoNeedRecycleInDispose = true;
        //这里是为了把状态设置为正在Dispose了，防止卸载组件的时候又执行Dispose
        base.Dispose();

        if (Parent != null)
        {
            Parent.RemoveEntity(GetType(), this, false);
            Parent = null;
        }

        if (_componentDict.Count > 0)
        {
            foreach (var keyValue in _componentDict)
            {
                AComponent comp = keyValue.Value;
                if (comp == null)
                    continue;

                try
                {
                    comp.BaseKernelTrustRelease();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            _componentDict.Clear();
        }

        if (_entityDict.Count > 0)
        {
            foreach (var kv in _entityDict)
            {
                AEntity entity = kv.Value;
                if (entity == null)
                    continue;

                try
                {
                    entity.BaseKernelTrustRelease();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            _entityDict.Clear();
        }

        if (BaseAEntity_NoNeedRecycleInDispose)
            BaseAEntity_NoNeedRecycleInDispose = false;
        else
            Base_PushToPool();
    }

    /// <summary>
    /// 系统内部调用，外部不要使用
    /// </summary>
    public virtual void BaseKernelTrustRelease()
    {
        if (Id == 0)
            return;

        Parent = null;

        Dispose();
    }

    public void RemoveEntity(Type type, AEntity child, bool dispose = true)
    {
        if (dispose || this is AEntityRepeat)
        {
            AEntity entity;
            if (!_entityDict.TryGetValue(type, out entity) || (entity != child))
                return;

            _entityDict.Remove(type);

            if (dispose)
                entity.Dispose();
        }
        else
            _entityDict.Remove(type);
    }

    public E GetChildEntity<E>() where E : AEntity
    {
        AEntity entity;
        if (!_entityDict.TryGetValue(typeof(E), out entity))
            return null;

        if (entity == null)
        {
            _entityDict.Remove(typeof(E));
            return null;
        }

        return (E)entity;
    }

    public E GetNeedChildEntity<E>(bool isFromPool = true) where E : AEntity
    {
        Type type = typeof(E);
        AEntity entity;
        if (!_entityDict.TryGetValue(type, out entity))
        {
            entity = ComponentFactory.Base_CreateChildEntity<E>(this, isFromPool);
            if (entity == null)
                return null;

            _entityDict.Add(type, entity);

            ObjectEvents.Instance.AddEvent(entity);
        }

        return (E)entity;
    }

    public E GetNeedChildEntityForEvent<E>(bool isFromPool = true, bool isAddEvent = true) where E : AEntity
    {
        Type type = typeof(E);
        AEntity entity;
        if (!_entityDict.TryGetValue(type, out entity))
        {
            entity = ComponentFactory.Base_CreateChildEntity<E>(this, isFromPool);
            if (entity == null)
                return null;

            _entityDict.Add(type, entity);

            if (isAddEvent)
                ObjectEvents.Instance.AddEvent(entity);
            else
                entity.Base_IsBaseKernelOffUpdate = true;
        }

        return (E)entity;
    }

    public K GetComponent<K>() where K : AComponent
    {
        AComponent component;
        if (!_componentDict.TryGetValue(typeof(K), out component))
            return null;

        if (component == null)
        {
            RemoveComponent(typeof(K));
            return null;
        }

        return (K)component;
    }

    public AComponent GetComponent(Type type)
    {
        AComponent component;
        if (!_componentDict.TryGetValue(type, out component))
            return null;

        if (component == null)
        {
            RemoveComponent(type);
            return null;
        }

        return component;
    }

    public K GetNeedComponent<K>(bool isFromPool = true) where K : AComponent
    {
        Type type = typeof(K);
        AComponent component;
        if (!_componentDict.TryGetValue(type, out component))
        {
            component = ComponentFactory.Create<K>(this, isFromPool);
            if (component == null)
                return null;

            _componentDict.Add(type, component);

            ObjectEvents.Instance.AddEvent(component);
        }

        return (K)component;
    }

    public AComponent GetNeedComponent(Type type, bool isFromPool = true)
    {
        AComponent component;
        if (!_componentDict.TryGetValue(type, out component))
        {
            component = ComponentFactory.Create(this, type, isFromPool);
            if (component == null)
                return null;

            _componentDict.Add(type, component);

            ObjectEvents.Instance.AddEvent(component);
        }

        return component;
    }

    public K GetNeedComponentForEvent<K>(bool isFromPool = true, bool isAddEvent = true) where K : AComponent
    {
        Type type = typeof(K);
        AComponent component;
        if (!_componentDict.TryGetValue(type, out component))
        {
            component = ComponentFactory.Create<K>(this, isFromPool);
            if (component == null)
                return null;

            _componentDict.Add(type, component);

            if (isAddEvent)
                ObjectEvents.Instance.AddEvent(component);
            else
                component.Base_IsBaseKernelOffUpdate = true;
        }

        return (K)component;
    }

    public void RemoveComponent(Type type)
    {
        AComponent component;
        if (!_componentDict.TryGetValue(type, out component))
            return;

        _componentDict.Remove(type);

        component?.Dispose();
    }

    public void RemoveComponent<K>() where K : AComponent
    {
        RemoveComponent(typeof(K));
    }

    public void RemoveComponentNoDispose(Type type)
    {
        _componentDict.Remove(type);
    }
}

public class AComponent : Disposer
{
    public AEntity m_Entity;

    public override void Dispose()
    {
        if (Id == 0)
            return;

        base.Dispose();

        if (m_Entity != null)
            m_Entity.RemoveComponentNoDispose(GetType());

        m_Entity = null;
    }

    /// <summary>
    /// 系统内部调用，外部不要使用
    /// </summary>
    internal virtual void BaseKernelTrustRelease()
    {
        if (Id == 0)
            return;

        m_Entity = null;

        Dispose();
    }

    public T GetEntity<T>() where T : AEntity
    {
        return m_Entity as T;
    }

    public T GetComponent<T>() where T : AComponent
    {
        if (m_Entity == null)
            return null;

        return m_Entity.GetComponent<T>();
    }

    public T GetNeedComponent<T>() where T : AComponent
    {
        if (m_Entity == null)
            return null;

        return m_Entity.GetNeedComponent<T>();
    }
}

public class AEntityRepeat : AEntity
{
    public Dictionary<long, AEntityRepeat> m_RepeatEntityDic = new Dictionary<long, AEntityRepeat>();
    public Dictionary<long, AComponentRepeat> m_RepeatComponentDic = new Dictionary<long, AComponentRepeat>();

    public override void Dispose()
    {
        if (Id == 0)
            return;

        long entityId = Id;
        AEntityRepeat aParentEntityRepeat = null;
        if (Parent != null)
            aParentEntityRepeat = (AEntityRepeat)Parent;

        BaseAEntity_NoNeedRecycleInDispose = true;
        base.Dispose();

        if (aParentEntityRepeat != null)
            aParentEntityRepeat.RemoveEntity_Repeat(entityId, false);

        ClearAllEntity_Repeat();
        ClearAllComponent_Repeat();

        Base_PushToPool();
    }

    /// <summary>
    /// 系统内部调用，外部不要使用
    /// </summary>
    public override void BaseKernelTrustRelease()
    {
        if (Id == 0)
            return;

        Parent = null;

        Dispose();
    }

    public E GetChildEntity_Repeat<E>(long entityId) where E : AEntityRepeat
    {
        AEntityRepeat aEntityRepeat;
        if (!m_RepeatEntityDic.TryGetValue(entityId, out aEntityRepeat))
            return null;

        if (aEntityRepeat == null)
        {
            m_RepeatEntityDic.Remove(entityId);
            return null;
        }

        return (E)aEntityRepeat;
    }

    public E GetNeedChildEntity_Repeat<E>(bool isFromPool = true) where E : AEntityRepeat
    {
        AEntityRepeat entityRepeat = ComponentFactory.Base_CreateChildEntity<E>(this, isFromPool);
        if (entityRepeat == null)
            return null;

        m_RepeatEntityDic.Add(entityRepeat.Id, entityRepeat);

        ObjectEvents.Instance.AddEvent(entityRepeat);

        return (E)entityRepeat;
    }

    public E GetNeedChildEntityForEvent_Repeat<E>(bool isFromPool = true, bool isAddEvent = true) where E : AEntityRepeat
    {
        AEntityRepeat entityRepeat = ComponentFactory.Base_CreateChildEntity<E>(this, isFromPool);
        if (entityRepeat == null)
            return null;

        m_RepeatEntityDic.Add(entityRepeat.Id, entityRepeat);

        if (isAddEvent)
            ObjectEvents.Instance.AddEvent(entityRepeat);
        else
            entityRepeat.Base_IsBaseKernelOffUpdate = true;

        return (E)entityRepeat;
    }

    public K GetComponent_Repeat<K>(long componentId) where K : AComponentRepeat
    {
        AComponentRepeat componentRepeat;
        if (!m_RepeatComponentDic.TryGetValue(componentId, out componentRepeat))
            return null;

        if (componentRepeat == null)
        {
            RemoveComponent_Repeat(componentId);
            return null;
        }

        return (K)componentRepeat;
    }

    public AComponentRepeat GetComponent_Repeat(long componentId)
    {
        AComponentRepeat componentRepeat;
        if (!m_RepeatComponentDic.TryGetValue(componentId, out componentRepeat))
            return null;

        if (componentRepeat == null)
        {
            RemoveComponent_Repeat(componentId);
            return null;
        }

        return componentRepeat;
    }

    public K GetNeedComponent_Repeat<K>(bool isFromPool = true) where K : AComponentRepeat
    {
        AComponentRepeat componentRepeat = ComponentFactory.Create<K>(this, isFromPool);
        if (componentRepeat == null)
            return null;

        m_RepeatComponentDic.Add(componentRepeat.Id, componentRepeat);

        ObjectEvents.Instance.AddEvent(componentRepeat);

        return (K)componentRepeat;
    }

    public AComponentRepeat GetNeedComponent_Repeat(Type type, bool isFromPool = true)
    {
        AComponentRepeat componentRepeat = (AComponentRepeat)ComponentFactory.Create(this, type, isFromPool);
        if (componentRepeat == null)
            return null;

        m_RepeatComponentDic.Add(componentRepeat.Id, componentRepeat);

        ObjectEvents.Instance.AddEvent(componentRepeat);

        return componentRepeat;
    }

    public K GetNeedComponentForEvent_Repeat<K>(bool isFromPool = true, bool isAddEvent = true) where K : AComponentRepeat
    {
        AComponentRepeat componentRepeat = ComponentFactory.Create<K>(this, isFromPool);
        if (componentRepeat == null)
            return null;

        m_RepeatComponentDic.Add(componentRepeat.Id, componentRepeat);

        if (isAddEvent)
            ObjectEvents.Instance.AddEvent(componentRepeat);
        else
            componentRepeat.Base_IsBaseKernelOffUpdate = true;

        return (K)componentRepeat;
    }

    public void RemoveEntity_Repeat(long entityId, bool dispose = true)
    {
        if (dispose)
        {
            AEntityRepeat aEntityRepeat;
            if (!m_RepeatEntityDic.TryGetValue(entityId, out aEntityRepeat))
                return;

            m_RepeatEntityDic.Remove(entityId);

            aEntityRepeat.Dispose();
        }
        else
            m_RepeatEntityDic.Remove(entityId);
    }

    public void RemoveComponent_Repeat(long componentId)
    {
        AComponentRepeat componentRepeat;
        if (!m_RepeatComponentDic.TryGetValue(componentId, out componentRepeat))
            return;

        m_RepeatComponentDic.Remove(componentId);

        componentRepeat.Dispose();
    }

    public void RemoveComponentNoDispose_Repeat(long componentId)
    {
        m_RepeatComponentDic.Remove(componentId);
    }

    public void ClearAllEntity_Repeat()
    {
        if (m_RepeatEntityDic.Count > 0)
        {
            foreach (var kv in m_RepeatEntityDic)
            {
                var entityRepeat = kv.Value;
                if (entityRepeat == null)
                    continue;

                try
                {
                    entityRepeat.BaseKernelTrustRelease();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            m_RepeatEntityDic.Clear();
        }
    }

    public void ClearAllComponent_Repeat()
    {
        if (m_RepeatComponentDic.Count > 0)
        {
            foreach (var keyValue in m_RepeatComponentDic)
            {
                AComponentRepeat compRepeat = keyValue.Value;
                if (compRepeat == null)
                    continue;

                try
                {
                    compRepeat.BaseKernelTrustRelease();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            m_RepeatComponentDic.Clear();
        }
    }

    public void ClearTypeComponents_Repeat<T>() where T : AComponentRepeat
    {
        if (m_RepeatComponentDic.Count > 0)
        {
            foreach (var keyValue in m_RepeatComponentDic)
            {
                AComponentRepeat compRepeat = keyValue.Value;
                if (compRepeat == null || !(compRepeat is T))
                    continue;

                try
                {
                    EntityFactory.Temporary_RemoveIdQueue.Enqueue(keyValue.Key);

                    compRepeat.BaseKernelTrustRelease();
                }
                catch (Exception e)
                {
                    DebugUtil.LogError(e.ToString());
                }
            }

            while (EntityFactory.Temporary_RemoveIdQueue.Count > 0)
            {
                m_RepeatComponentDic.Remove(EntityFactory.Temporary_RemoveIdQueue.Dequeue());
            }
        }
    }
}

public class AComponentRepeat : AComponent
{
    public override void Dispose()
    {
        if (Id == 0)
            return;

        long componentId = Id;
        AEntityRepeat aEntityRepeat = null;
        if (m_Entity != null)
            aEntityRepeat = (AEntityRepeat)m_Entity;

        base.Dispose();

        if (aEntityRepeat != null)
            aEntityRepeat.RemoveComponentNoDispose_Repeat(componentId);
    }

    /// <summary>
    /// 系统内部调用，外部不要使用
    /// </summary>
    internal override void BaseKernelTrustRelease()
    {
        if (Id == 0)
            return;

        m_Entity = null;

        Dispose();
    }
}

public partial class Disposer : Object, IDisposable
{
    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    internal bool Base_IsFromPool;

    /// <summary>
    /// 外部不能设置，底层设置
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    public long Id;

    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    internal bool Base_IsBaseKernelOffUpdate;
    /// <summary>
    /// 外部调用：
    /// true=在已存在IUpdate接口的情况下移除IUpdate接口；
    /// false=在已移除IUpdate接口的情况下重新添加IUpdate接口；
    /// </summary>
    public bool ObjectEvent_IsStopUpdate
    {
        get { return Base_IsBaseKernelOffUpdate; }
        set
        {
            if (Base_IsBaseKernelOffUpdate != value)
            {
                if (Base_IsBaseKernelOffUpdate)
                {
                    ObjectEvents.Instance.AddSystemUpdateEvent(this);
                }

                Base_IsBaseKernelOffUpdate = value;
            }
        }
    }

    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    internal bool Base_IsPause;

    /// <summary>
    /// 外部不能设置使用，底层使用
    /// </summary>
#if UNITY_EDITOR
    [UnityEngine.HideInInspector]
#endif
    protected bool Base_NoNeedRecycleInDispose = false;

    public virtual void Dispose()
    {
        if (Id == 0)
        {
            DebugUtil.LogError($"该{this.GetType()}实列的id已经为0");
        }

        //Log.Debug($"----------Dispose这个{GetType().ToString()}--------");

        Id = 0;
        Base_IsBaseKernelOffUpdate = false;
        Base_IsPause = false;

        if (Base_NoNeedRecycleInDispose)
            Base_NoNeedRecycleInDispose = false;
        else
            Base_PushToPool();
    }

    public void AddEvent()
    {
        ObjectEvents.Instance.AddEvent(this);
    }

    /// <summary>
    /// 外部不能调用，底层使用
    /// </summary>
    protected void Base_PushToPool()
    {
        if (Base_IsFromPool)
            CombatObjectPool.Instance.Recycle(this);
        Base_IsFromPool = false;
    }
}

public class BaseComponent<T> : AComponent where T : AEntity
{
    private T _t;
    public T m_CurUseEntity
    {
        get
        {
            if (_t == null)
                _t = m_Entity as T;

            return _t;
        }
    }


    public override void Dispose()
    {
        base.Dispose();

        if (_t != null)
            _t = null;
    }
}

public class BaseEntity<T> : AEntity where T : AEntity
{
    private T _t;
    public T m_CurUseParent
    {
        get
        {
            if (_t == null)
                _t = Parent as T;

            return _t;
        }
    }


    public override void Dispose()
    {
        base.Dispose();

        if (_t != null)
            _t = null;
    }
}

public class BaseComponentRepeat<T> : AComponentRepeat where T : AEntityRepeat
{
    private T _t;
    public T m_CurUseEntity
    {
        get
        {
            if (_t == null)
                _t = m_Entity as T;

            return _t;
        }
    }


    public override void Dispose()
    {
        base.Dispose();

        if (_t != null)
            _t = null;
    }
}
