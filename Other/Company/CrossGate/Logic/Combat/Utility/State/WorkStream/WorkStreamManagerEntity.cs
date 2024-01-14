using System;
using System.Collections.Generic;

public enum StartControllerStyleEnum
{
    None = 0,
    Parallel = 1,           //战斗单位控制器并行执行
    Insert_LastQueue = 2,   //战斗单位插入到最后一个控制器队列中
    StopAll = 3,            //战斗单位停止所有控制器
    Insert_MainQueue = 4,   //战斗单位插入到主控制器队列中
}

public enum SwitchWorkStreamEnum
{
    None = 0,

    Stop_AllWorkStream = 1,

    #region 处理主容器，不处理并行容器   100~199
    Stop_AllMain_NoParallels = 100,
    Stop_MainNoStack_NoParallels = 101,
    Pause_MainNoStack_NoParallels = 102,
    WaitStart_MainNoStack_NoParallels = 103,    //等待重启
    #endregion

    #region 不处理主容器，处理并行容器  300~399
    Stop_NoMain_AllParallels = 300,
    #endregion
}

public class WorkStreamManagerEntity : AEntity
{
    public class StreamControllerData
    {
        public BaseStreamControllerEntity BaseStreamController;
        public Queue<BaseStreamControllerEntity> BaseStreamControllerStack;
        
        public BaseStreamControllerEntity Push(bool isNeedDelFromManager = false, BaseStreamControllerEntity filterController = null)
        {
            bool isHaveFilter = false;

            if (BaseStreamController != filterController)
            {
                if (BaseStreamController != null)
                {
                    if (!isNeedDelFromManager)
                        BaseStreamController.m_WorkStreamManagerEntity = null;

                    var bsc = BaseStreamController;
                    BaseStreamController = null;

                    bsc.OnOver(true);
                }
            }
            else
            {
                BaseStreamController = null;
                isHaveFilter = true;
            }

            if (BaseStreamControllerStack != null)
            {
                while (BaseStreamControllerStack.Count > 0)
                {
                    var bsc = BaseStreamControllerStack.Dequeue();
                    if (bsc != null)
                    {
                        if (!isNeedDelFromManager)
                            bsc.m_WorkStreamManagerEntity = null;
                        bsc.OnOver(true);
                    }
                }
            }

            CombatObjectPool.Instance.Push(this);

            return isHaveFilter ? filterController : null;
        }
    }

    protected bool _isSelfDestroy;

    public bool m_Parallel2Main;
    
    #region 主控制器容器
    public BaseStreamControllerEntity m_MainStreamController;
    private Queue<BaseStreamControllerEntity> _mainStreamControllerStack = new Queue<BaseStreamControllerEntity>();
    #endregion

    #region 并行控制器容器
    private List<StreamControllerData> _parallelStreamControllerDataList;
    #endregion

    private Queue<BaseStreamControllerEntity> _cacheStack;

    public override void Dispose()
    {
        _isSelfDestroy = false;

        m_Parallel2Main = false;

        StopAll();

        base.Dispose();
    }

    /// <summary>
    /// isSelfDestroy=true,不用手动销毁创建WorkStreamManagerEntity的子类
    /// </summary>
    protected static T CreateWorkStreamManagerEntity<T>(bool isSelfDestroy = true) where T : WorkStreamManagerEntity
    {
        T t = EntityFactory.Create<T>();
        t._isSelfDestroy = isSelfDestroy;

        return t;
    }
    
    public T CreateController<T>(uint workId, int attachType = 0, SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
            Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, ulong uid = 0,
            List<WorkBlockData> workBlockDatas = null)
            where T : BaseStreamControllerEntity
    {
        if (workBlockDatas == null)
            workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<T>(workId, attachType);
        if (workBlockDatas == null || workBlockDatas.Count == 0)
        {
            Lib.Core.DebugUtil.LogError($"{typeof(T)}类型的数据中没有workId ：{workId.ToString()}, attachType : {attachType.ToString()}");
            return null;
        }

        bool isd = _isSelfDestroy;
        _isSelfDestroy = false;

        bool isP2M = m_Parallel2Main;
        m_Parallel2Main = false;

        if (m_MainStreamController != null)
        {
            switch (switchWorkStreamEnum)
            {
                case SwitchWorkStreamEnum.Stop_AllWorkStream:
                    StopAll();
                    break;

                case SwitchWorkStreamEnum.Stop_AllMain_NoParallels:
                    StopAllMain_NoParallels();
                    break;

                case SwitchWorkStreamEnum.Stop_MainNoStack_NoParallels:
                default:
                    StopMainNoStack_NoParallel(true);
                    break;

                case SwitchWorkStreamEnum.Pause_MainNoStack_NoParallels:
                    if (m_MainStreamController.PauseStateMachine(true))
                        _mainStreamControllerStack.Enqueue(m_MainStreamController);
                    m_MainStreamController = null;

                    break;

                case SwitchWorkStreamEnum.WaitStart_MainNoStack_NoParallels:
                    m_MainStreamController.RevertToWaitForStartControllerStatus();
                    _mainStreamControllerStack.Enqueue(m_MainStreamController);
                    m_MainStreamController = null;

                    break;
            }
        }

        _isSelfDestroy = isd;

        m_Parallel2Main = isP2M;

        if (m_MainStreamController == null)
        {
            m_MainStreamController = EntityFactory.Create<T>();
            m_MainStreamController.m_ControllerBeginAction = controllerBeginAction;
            m_MainStreamController.m_ControllerOverAction = controllerOverAction;

            if (m_MainStreamController.PrepareControllerEntity(this, workId, workBlockDatas, uid))
                return (T)m_MainStreamController;
            else
            {
                m_MainStreamController.OnOver(false);
                m_MainStreamController = null;
                return null;
            }
        }

        return null;
    }

    public bool StartController<T>(uint workId, int attachType = 0, SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, int blockType = 0, ulong uid = 0)
        where T : BaseStreamControllerEntity
    {
        T t = CreateController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction, uid);
        if (t != null && t.StartController(blockType))
            return true;
        else
            return false;
    }

    public T CreateParallelController<T>(uint workId, int attachType = 0, SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.None,
            Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null,
            List<WorkBlockData> workBlockDatas = null)
            where T : BaseStreamControllerEntity
    {
        if (workBlockDatas == null)
            workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<T>(workId, attachType);
        if (workBlockDatas == null || workBlockDatas.Count == 0)
        {
            Lib.Core.DebugUtil.LogError($"{typeof(T)}类型的数据中没有workId ：{workId.ToString()}, attachType : {attachType.ToString()}");
            return null;
        }
        
        bool isd = _isSelfDestroy;
        _isSelfDestroy = false;

        bool isP2M = m_Parallel2Main;
        m_Parallel2Main = false;

        switch (switchWorkStreamEnum)
        {
            case SwitchWorkStreamEnum.Stop_AllWorkStream:
                StopAll();
                break;

            case SwitchWorkStreamEnum.Stop_NoMain_AllParallels:
                StopNoMain_AllParallels();
                break;
        }

        _isSelfDestroy = isd;

        m_Parallel2Main = isP2M;

        T parallelController = EntityFactory.Create<T>();
        parallelController.m_ControllerBeginAction = controllerBeginAction;
        parallelController.m_ControllerOverAction = controllerOverAction;

        if (parallelController.PrepareControllerEntity(this, workId, workBlockDatas))
        {
            if (m_MainStreamController == null)
            {
                m_MainStreamController = parallelController;
            }
            else
            {
                if (_parallelStreamControllerDataList == null)
                    _parallelStreamControllerDataList = new List<StreamControllerData>();
                StreamControllerData streamControllerData = CombatObjectPool.Instance.Get<StreamControllerData>();
                streamControllerData.BaseStreamController = parallelController;
                _parallelStreamControllerDataList.Add(streamControllerData);
            }
            
            return parallelController;
        }
        else
        {
            parallelController.OnOver(false);
            return null;
        }
    }

    public T InsertController<T>(uint workId, StartControllerStyleEnum startControllerStyleEnum, out bool isEnqueue, int attachType = 0, SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.None,
            Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null,
            List<WorkBlockData> workBlockDatas = null)
            where T : BaseStreamControllerEntity
    {
        isEnqueue = false;

        if (workBlockDatas == null)
            workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<T>(workId, attachType);
        if (workBlockDatas == null || workBlockDatas.Count == 0)
        {
            Lib.Core.DebugUtil.LogError($"{typeof(T)}类型的数据中没有workId ：{workId.ToString()}, attachType : {attachType.ToString()}");
            return null;
        }

        bool isd = _isSelfDestroy;
        _isSelfDestroy = false;

        bool isP2M = m_Parallel2Main;
        m_Parallel2Main = false;

        _isSelfDestroy = isd;

        m_Parallel2Main = isP2M;

        T controller = EntityFactory.Create<T>();
        controller.m_ControllerBeginAction = controllerBeginAction;
        controller.m_ControllerOverAction = controllerOverAction;

        controller.m_ControllerState = 1;
        
        if (controller.PrepareControllerEntity(this, workId, workBlockDatas))
        {
            if (startControllerStyleEnum == StartControllerStyleEnum.Insert_LastQueue)
            {
                if (_parallelStreamControllerDataList != null)
                {
                    for (int pscdIndex = _parallelStreamControllerDataList.Count - 1; pscdIndex > -1; --pscdIndex)
                    {
                        StreamControllerData streamControllerData = _parallelStreamControllerDataList[pscdIndex];
                        if (streamControllerData == null || streamControllerData.BaseStreamController == null)
                            continue;

                        if (streamControllerData.BaseStreamControllerStack == null)
                            streamControllerData.BaseStreamControllerStack = new Queue<BaseStreamControllerEntity>();

                        streamControllerData.BaseStreamControllerStack.Enqueue(controller);

                        isEnqueue = true;
                        break;
                    }
                }

                if (!isEnqueue)
                {
                    if (m_MainStreamController != null)
                    {
                        _mainStreamControllerStack.Enqueue(controller);
                        isEnqueue = true;
                    }
                    else
                        m_MainStreamController = controller;
                }
            }
            else
            {
                if (m_MainStreamController != null)
                {
                    _mainStreamControllerStack.Enqueue(controller);
                    isEnqueue = true;
                }
                else
                    m_MainStreamController = controller;
            }
            
            return controller;
        }
        else
        {
            controller.OnOver(false);
            return null;
        }
    }

    public void DoImmediateMainStreamControllerStack(Func<BaseStreamControllerEntity, bool> conditionAction = null)
    {
        if (_mainStreamControllerStack == null || _mainStreamControllerStack.Count < 1)
            return;

        if (_cacheStack == null)
            _cacheStack = new Queue<BaseStreamControllerEntity>();

        while (_mainStreamControllerStack.Count > 0)
        {
            BaseStreamControllerEntity baseStreamControllerEntity = _mainStreamControllerStack.Dequeue();
            if (baseStreamControllerEntity == null)
                continue;

            if (conditionAction == null || conditionAction(baseStreamControllerEntity))
            {
                if (_parallelStreamControllerDataList == null)
                    _parallelStreamControllerDataList = new List<StreamControllerData>();
                StreamControllerData streamControllerData = CombatObjectPool.Instance.Get<StreamControllerData>();
                streamControllerData.BaseStreamController = baseStreamControllerEntity;
                _parallelStreamControllerDataList.Add(streamControllerData);

                baseStreamControllerEntity.StartController(baseStreamControllerEntity.m_StartBlockType);
            }
            else
            {
                _cacheStack.Enqueue(baseStreamControllerEntity);
            }
        }

        var s = _mainStreamControllerStack;
        _mainStreamControllerStack = _cacheStack;
        _cacheStack = s;
    }

    public void StopAll()
    {
        m_Parallel2Main = false;

        StopAllMain_NoParallels();

        StopNoMain_AllParallels();
    }

    private void StopAllMain_NoParallels(BaseStreamControllerEntity filterController = null)
    {
        if (m_MainStreamController != filterController && m_MainStreamController != null)
        {
            var msc = m_MainStreamController;
            m_MainStreamController = null;

            msc.OnOver(true);
        }

        while (_mainStreamControllerStack.Count > 0)
        {
            _mainStreamControllerStack.Dequeue()?.OnOver(true);
        }
    }

    public void StopMainNoStack_NoParallel(bool isOverNoToNext)
    {
        if (m_MainStreamController != null)
            m_MainStreamController.OnOver(isOverNoToNext);
        else if (!isOverNoToNext)
            WorkStreamOverOperation(isOverNoToNext, null);
    }

    private void StopNoMain_AllParallels()
    {
        if (_parallelStreamControllerDataList != null)
        {
            for (int i = _parallelStreamControllerDataList.Count - 1; i > -1; --i)
            {
                StreamControllerData streamControllerData = _parallelStreamControllerDataList[i];
                if (streamControllerData == null)
                    continue;

                streamControllerData.Push();
            }
            _parallelStreamControllerDataList.Clear();
        }
    }

    public void StopAllByFilter(BaseStreamControllerEntity filterController)
    {
        StopAllMain_NoParallels(filterController);

        if (_parallelStreamControllerDataList != null)
        {
            for (int i = _parallelStreamControllerDataList.Count - 1; i > -1; --i)
            {
                StreamControllerData streamControllerData = _parallelStreamControllerDataList[i];
                if (streamControllerData == null)
                    continue;

                var filter = streamControllerData.Push(false, filterController);
                if (filter != null)
                    m_MainStreamController = filter;
            }
            _parallelStreamControllerDataList.Clear();
        }
    }

    public void WorkStreamOverOperation(bool isOverNoToNext, BaseStreamControllerEntity baseStreamControllerEntity)
    {
        if (m_MainStreamController == baseStreamControllerEntity || (baseStreamControllerEntity == null && !isOverNoToNext))
        {
            m_MainStreamController = null;

            if (!isOverNoToNext)
            {
                while (_mainStreamControllerStack.Count > 0)
                {
                    m_MainStreamController = _mainStreamControllerStack.Dequeue();
                    if (m_MainStreamController != null)
                    {
                        if (m_MainStreamController.m_ControllerState == 1)
                        {
                            m_MainStreamController.StartController(m_MainStreamController.m_StartBlockType);
                            break;
                        }
                        else if (m_MainStreamController.m_ControllerState == 2)
                        {
                            if (m_MainStreamController.PauseStateMachine(false))
                                break;
                            else
                                m_MainStreamController = null;
                        }

                        break;
                    }
                }
            }

            if (m_Parallel2Main && m_MainStreamController == null && _mainStreamControllerStack.Count <= 0)
            {
                if (_parallelStreamControllerDataList != null)
                {
                    for (int i = 0, count = _parallelStreamControllerDataList.Count; i < count; i++)
                    {
                        StreamControllerData streamControllerData = _parallelStreamControllerDataList[i];
                        if (streamControllerData == null ||
                            (streamControllerData.BaseStreamController == null &&
                            (streamControllerData.BaseStreamControllerStack == null || streamControllerData.BaseStreamControllerStack.Count <= 0)))
                            continue;

                        m_MainStreamController = streamControllerData.BaseStreamController;
                        streamControllerData.BaseStreamController = null;

                        var mainStack = _mainStreamControllerStack;
                        _mainStreamControllerStack = streamControllerData.BaseStreamControllerStack;
                        if (_mainStreamControllerStack == null)
                            _mainStreamControllerStack = new Queue<BaseStreamControllerEntity>();
                        streamControllerData.BaseStreamControllerStack = mainStack;

                        _parallelStreamControllerDataList.RemoveAt(i);
                        streamControllerData.Push();
                        
                        break;
                    }
                }
            }
        }
        else if (baseStreamControllerEntity != null)
        {
            if (_parallelStreamControllerDataList != null)
            {
                for (int i = 0, count = _parallelStreamControllerDataList.Count; i < count; i++)
                {
                    StreamControllerData streamControllerData = _parallelStreamControllerDataList[i];
                    if (streamControllerData == null || streamControllerData.BaseStreamController != baseStreamControllerEntity)
                        continue;

                    streamControllerData.BaseStreamController = null;

                    if (!isOverNoToNext && streamControllerData.BaseStreamControllerStack != null)
                    {
                        while (streamControllerData.BaseStreamControllerStack.Count > 0)
                        {
                            streamControllerData.BaseStreamController = streamControllerData.BaseStreamControllerStack.Dequeue();
                            if (streamControllerData.BaseStreamController != null)
                            {
                                if (streamControllerData.BaseStreamController.m_ControllerState == 1)
                                {
                                    streamControllerData.BaseStreamController.StartController(streamControllerData.BaseStreamController.m_StartBlockType);
                                    break;
                                }
                                else if (streamControllerData.BaseStreamController.m_ControllerState == 2)
                                {
                                    if (streamControllerData.BaseStreamController.PauseStateMachine(false))
                                        break;
                                    else
                                        streamControllerData.BaseStreamController = null;
                                }

                                break;
                            }
                        }
                    }

                    if (streamControllerData.BaseStreamController == null &&
                        (streamControllerData.BaseStreamControllerStack == null || streamControllerData.BaseStreamControllerStack.Count <= 0))
                    {
                        _parallelStreamControllerDataList.RemoveAt(i);
                        streamControllerData.Push();
                    }

                    break;
                }
            }
        }

        if (_isSelfDestroy && IsOverWorkStream())
        {
            Dispose();
        }
    }

    private bool IsOverWorkStream()
    {
        if (m_MainStreamController != null || _mainStreamControllerStack.Count > 0)
            return false;

        if (_parallelStreamControllerDataList != null && _parallelStreamControllerDataList.Count > 0)
        {
            for (int i = 0, count = _parallelStreamControllerDataList.Count; i < count; i++)
            {
                StreamControllerData streamControllerData = _parallelStreamControllerDataList[i];
                if (streamControllerData != null &&
                    (streamControllerData.BaseStreamController != null ||
                    (streamControllerData.BaseStreamControllerStack != null && streamControllerData.BaseStreamControllerStack.Count > 0)))
                    return false;
            }
        }

        return true;
    }

#if UNITY_EDITOR
    public BaseStreamControllerEntity[] GetMainStreamControllerStack()
    {
        return _mainStreamControllerStack.ToArray();
    }

    public List<StreamControllerData> GetParallelStreamControllerDataList()
    {
        return _parallelStreamControllerDataList;
    }

    public int GetParallelStreamControllerDataListCount()
    {
        if (_parallelStreamControllerDataList == null)
            return 0;

        return _parallelStreamControllerDataList.Count;
    }

    public StreamControllerData GetStreamControllerData(int i)
    {
        if (i < _parallelStreamControllerDataList.Count)
            return _parallelStreamControllerDataList[i];

        return null;
    }
#endif
}
