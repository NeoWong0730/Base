using System;

public class StateMachineEntity : AEntityRepeat
{
    public StateControllerEntity m_StateControllerEntity;

    public StateMachineEntity m_ParentMachine;
    public int m_ParentStateId;
    public long m_ParentStateComponentId;

    public int m_LayerIndex;

    public StateTranstionComponent m_StateTranstionComponent;

    public Action m_StateMachineOverAction;

    public Action<StateBaseComponent> m_StartStateAction;
    public Action<StateBaseComponent> m_EndStateAction;

    public void Init(StateControllerEntity stateControllerEntity, int layerIndex)
    {
        m_StateControllerEntity = stateControllerEntity;
        m_LayerIndex = layerIndex;
    }

    /// <summary>
    /// 外部需要StateMachineEntity手动释放的情况下必须只能调用StateMachineOver()
    /// </summary>
    public override void Dispose()
    {
        m_StateControllerEntity = null;

        m_ParentMachine = null;
        m_ParentStateId = 0;
        m_ParentStateComponentId = 0L;

        m_LayerIndex = -1;

        m_StateTranstionComponent = null;

        base.Dispose();

        if (m_StateMachineOverAction != null)
        {
            var overAction = m_StateMachineOverAction;
            m_StateMachineOverAction = null;
            overAction.Invoke();
        }

        m_StartStateAction = null;
        m_EndStateAction = null;
    }

    /// <summary>
    /// 外部需要StateMachineEntity手动释放的情况下必须只能调用StateMachineOver()
    /// </summary>
    public void Free()
    {
        if (Id == 0 && m_StateControllerEntity == null)
            return;

        int layerIndex = m_LayerIndex;
        StateControllerEntity stateControllerEntity = m_StateControllerEntity;

        Dispose();

        if (stateControllerEntity != null)
            stateControllerEntity.RemoveStateMachineEntity(layerIndex, this);
        else
            Lib.Core.DebugUtil.LogError($"*状态机没有控制器*");
    }

    public T AddTranstion<T>() where T : StateTranstionComponent
    {
        T t = GetNeedComponent<T>();
        m_StateTranstionComponent = t;

        return t;
    }
    
    #region 切换到一个或多个state
    /// <summary>
    /// 切换到一个或多个state；   
    /// oldState=-1表示没有旧状态处理；
    /// type参数表示更新的状态如何获取
    /// type=-1获取前一个状态，=0获取当前状态，=1获取下一个状态
    /// selectIndex是oldState需要转换到选择种类的states
    /// </summary>
    public void TranstionMultiStates(long stateComponentId = 0L, int oldStateId = -1, int type = 1, int selectType = -1)
    {
        ushort oldNodeId = 0;
        if (stateComponentId != 0L)
        {
            var oldStateComponent = (StateBaseComponent)GetComponent_Repeat(stateComponentId);
            if (oldStateComponent == null)
            {
                if (oldStateId > -1)
                {
                    Type oldStateType = m_StateControllerEntity.m_StateECManager.GetStateType(oldStateId);
                    if (oldStateType != null)
                        oldStateComponent = (StateBaseComponent)GetComponent(oldStateType);
                }
            }
            if (oldStateComponent != null)
            {
                oldNodeId = oldStateComponent.m_DataNodeId;
                //m_StateTranstionComponent.DisposeState(oldNodeId);
                oldStateComponent.Dispose();
            }
        }
        else
            Lib.Core.DebugUtil.LogError("TranstionMultiStates切换到一个或多个state传来的stateComponentId为0");

        m_StateTranstionComponent.GetStates(type, oldNodeId, selectType);
    }
    public void TranstionMultiStates(StateBaseComponent oldStateComponent, int type = 1, int selectType = -1)
    {
        ushort oldNodeId = 0;
        if (oldStateComponent != null)
        {
            oldNodeId = oldStateComponent.m_DataNodeId;
            //m_StateTranstionComponent.DisposeState(oldNodeId);
            oldStateComponent.Dispose();
        }

        m_StateTranstionComponent.GetStates(type, oldNodeId, selectType);
    }

    public StateBaseComponent GetStateComponent(int state)
    {
        if (state > -1)
        {
            Type newStateType = m_StateControllerEntity.m_StateECManager.GetStateType(state);
            if (newStateType != null)
                return (StateBaseComponent)GetNeedComponent_Repeat(newStateType);
        }

        return null;
    }

    public bool SkipBlock(int newBlockState)
    {
        if (m_StateTranstionComponent.SkipBlock(newBlockState))
            return true;

        StateMachineOver();
        return false;
    }

    public void SkipState(long oldStateComponentId, ushort newNodeId, int newWorkBlockType = 0)
    {
        ushort oldNodeId = 0;
        if (oldStateComponentId != 0L)
        {
            var oldStateComponent = (StateBaseComponent)GetComponent_Repeat(oldStateComponentId);
            if (oldStateComponent != null)
            {
                oldNodeId = oldStateComponent.m_DataNodeId;
                //m_StateTranstionComponent.DisposeState(oldNodeId);
                oldStateComponent.Dispose();
            }
        }
        else
            Lib.Core.DebugUtil.LogError("TranstionMultiStates切换到一个或多个state传来的stateComponentId为0");

        m_StateTranstionComponent.SkipState(oldNodeId, newNodeId, newWorkBlockType);
    }
    public void SkipState(StateBaseComponent oldStateComponent, ushort newNodeId, int newWorkBlockType = 0)
    {
        ushort oldNodeId = 0;
        if (oldStateComponent != null)
        {
            oldNodeId = oldStateComponent.m_DataNodeId;
            //m_StateTranstionComponent.DisposeState(oldNodeId);
            oldStateComponent.Dispose();
        }

        m_StateTranstionComponent.SkipState(oldNodeId, newNodeId, newWorkBlockType);
    }
    
    public void PauseAllStateComponent(bool isPause)
    {
        foreach (var kv in m_RepeatComponentDic)
        {
            if (kv.Value == null)
                continue;

            StateBaseComponent stateBaseComponent = kv.Value as StateBaseComponent;
            if (stateBaseComponent == null)
                continue;

            stateBaseComponent.Base_IsPause = isPause;
        }
    }
    #endregion

    public void StateMachineOver()
    {
        StateMachineEntity parentMachine = null;
        int parentStateId = 0;
        long parentStateComponentId = 0L;
        if (m_ParentMachine != null)
        {
            parentMachine = m_ParentMachine;
            parentStateId = m_ParentStateId;
            parentStateComponentId = m_ParentStateComponentId;
        }

        Free();

        if (parentMachine != null)
        {
            parentMachine.TranstionMultiStates(parentStateComponentId, parentStateId);
        }
    }
}
