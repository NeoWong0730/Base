using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateControllerEntity : AEntity
{
    public int m_StateCategory;

    public StateECManager m_StateECManager;

    //最顶层(即第一层)的状态Machine
    public StateMachineEntity m_FirstMachine;

    public Action<StateControllerEntity> m_ControllerBeginAction;

    public Action m_ControllerOverAction;

    //最顶层(即第一层)的状态Machine不写入该容器中
    private Dictionary<int, List<StateMachineEntity>> _machineDic;
    
    //使用必须调用
    protected void OnInit(int stateCategory)
    {
        if (_machineDic == null)
            _machineDic = new Dictionary<int, List<StateMachineEntity>>();
        
        m_StateCategory = stateCategory;
        m_StateECManager = StateCategoryManager.Instance.GetStateECManager(stateCategory);
        
        if (m_FirstMachine != null)
            m_FirstMachine.Dispose();

        if (_machineDic.Count > 0)
        {
            DebugUtil.LogError($"初始化状态控制器状态Machine容器大小不为0");
        }
    }

    public override void Dispose()
    {
        m_StateCategory = -1;
        m_StateECManager = null;

        if (m_FirstMachine != null)
        {
            var fm = m_FirstMachine;
            m_FirstMachine = null;

            fm.Dispose();
        }

        //这里的machine都是m_FirstMachine组件下的childMachine，m_FirstMachine.Dispose的时候这里的machine都会Dispose
        _machineDic.Clear();

        base.Dispose();

        if (m_ControllerOverAction != null)
        {
            var overAction = m_ControllerOverAction;
            m_ControllerOverAction = null;
            overAction.Invoke();
        }
    }

    public StateMachineEntity CreateFirstStateMachineEntity()
    {
        StateMachineEntity machineEntity = EntityFactory.Create<StateMachineEntity>();
        machineEntity.Init(this, 0);

        machineEntity.m_ParentMachine = null;
        machineEntity.m_ParentStateId = 0;
        machineEntity.m_ParentStateComponentId = 0L;

        return machineEntity;
    }

    public StateMachineEntity CreateStateMachineEntity(int layerIndex, StateMachineEntity parentMachine = null, int parentStateId = 0)
    {
        StateMachineEntity machineEntity = EntityFactory.Create<StateMachineEntity>();
        machineEntity.Init(this, layerIndex);

        AddStateMachineEntity(machineEntity);

        machineEntity.m_ParentMachine = parentMachine;
        machineEntity.m_ParentStateId = parentStateId;

        return machineEntity;
    }

    public void AddStateMachineEntity(StateMachineEntity stateMachineEntity)
    {
        if (stateMachineEntity.m_LayerIndex == 0)
            return;

        List<StateMachineEntity> machineList;
        if (!_machineDic.TryGetValue(stateMachineEntity.m_LayerIndex, out machineList) || machineList == null)
        {
            machineList = new List<StateMachineEntity>();
            _machineDic[stateMachineEntity.m_LayerIndex] = machineList;
        }

        machineList.Add(stateMachineEntity);
    }

    public StateMachineEntity CreateChildStateMachineEntity(int layerIndex, StateMachineEntity parentMachine, int parentStateId, long parentStateComponentId)
    {
        StateMachineEntity machineEntity = EntityFactory.Create<StateMachineEntity>();
        machineEntity.Init(this, layerIndex);

        AddChildStateMachineEntity(machineEntity);

        machineEntity.m_ParentMachine = parentMachine;
        machineEntity.m_ParentStateId = parentStateId;
        machineEntity.m_ParentStateComponentId = parentStateComponentId;

        return machineEntity;
    }

    public void AddChildStateMachineEntity(StateMachineEntity stateMachineEntity)
    {
        if (stateMachineEntity.m_LayerIndex == 0)
            return;

        List<StateMachineEntity> machineList;
        if (!_machineDic.TryGetValue(stateMachineEntity.m_LayerIndex, out machineList) || machineList == null)
        {
            machineList = new List<StateMachineEntity>();
            _machineDic[stateMachineEntity.m_LayerIndex] = machineList;
        }

        machineList.Add(stateMachineEntity);
    }

    public virtual bool PauseStateMachine(bool isPause)
    {
        if (Id == 0L)
            return false;

        DebugUtil.Log(ELogType.eCombatBehave, $"<color=yellow>{Id.ToString()}--StateController暂停状态未：{isPause.ToString()}</color>");

        m_FirstMachine.PauseAllStateComponent(isPause);

        foreach (var kv in _machineDic)
        {
            if (kv.Value != null && kv.Value.Count > 0)
            {
                for (int i = 0, machineCount = kv.Value.Count; i < machineCount; i++)
                {
                    kv.Value[i].PauseAllStateComponent(isPause);
                }
            }
        }

        return true;
    }

    public void RemoveStateMachineEntity(int layerIndex, StateMachineEntity stateMachineEntity)
    {
        if (layerIndex < 0)
            return;

        if (layerIndex == 0)
        {
            if (m_FirstMachine.Id == 0L)
                m_FirstMachine = null;
            else
                DebugUtil.LogError($"RemoveStateMachineEntity的layerIndex为0时，m_FirstMachine.Id:{m_FirstMachine.Id}不为0，不应该！！！");

            Dispose();
            return;
        }

        List<StateMachineEntity> machineList;
        if (!_machineDic.TryGetValue(layerIndex, out machineList) || machineList == null)
            return;

        machineList.Remove(stateMachineEntity);
    }

#if UNITY_EDITOR
    public Dictionary<int, List<StateMachineEntity>> GetOtherMachineDic()
    {
        return _machineDic;
    }
#endif
}
