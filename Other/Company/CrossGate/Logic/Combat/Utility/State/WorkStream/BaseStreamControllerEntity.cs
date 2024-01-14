using System.Collections.Generic;

public abstract class BaseStreamControllerEntity : StateControllerEntity
{
    public WorkStreamManagerEntity m_WorkStreamManagerEntity;

    /// <summary>
    /// =0正常状态，=1等待重启状态，=2暂停状态
    /// </summary>
    public int m_ControllerState;
    public int m_StartBlockType;

    private bool _isOverNoToNext;

    public override void Dispose()
    {
        var wse = m_WorkStreamManagerEntity;
        m_WorkStreamManagerEntity = null;
        m_ControllerState = 0;
        m_StartBlockType = 0;

        base.Dispose();

        if (wse != null)
            wse.WorkStreamOverOperation(_isOverNoToNext, this);
        _isOverNoToNext = false;
    }

    public void OnOver(bool isOverNoToNext)
    {
        _isOverNoToNext = isOverNoToNext;

        Dispose();
    }
    
    public abstract bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0);

    public virtual bool StartController(int blockType = 0)
    {
        m_ControllerState = 0;

        WorkStreamTranstionComponent workStreamTranstionComponent = m_FirstMachine.m_StateTranstionComponent as WorkStreamTranstionComponent;

        if (blockType == 0)
            return workStreamTranstionComponent.StartWorkStream();
        else
            return workStreamTranstionComponent.StartWorkStream(blockType);
    }

    public virtual void RevertToWaitForStartControllerStatus()
    {
        m_ControllerState = 1;

        WorkStreamTranstionComponent workStreamTranstionComponent = m_FirstMachine.m_StateTranstionComponent as WorkStreamTranstionComponent;
        workStreamTranstionComponent.ClearBlockData();
    }

    public override bool PauseStateMachine(bool isPause)
    {
        bool p = base.PauseStateMachine(isPause);

        if (p)
        {
            if (isPause)
                m_ControllerState = 2;
            else
                m_ControllerState = 0;
        }

        return p;
    }

    /// <summary>
    /// StateBaseComponent节点附加上一个StateMachine,当该StateMachine结束该节点才结束
    /// </summary>
    public virtual bool StartChildMachine(StateBaseComponent stateBaseComponent, uint workId, int attachType, int blockType = 0)
    {
        List<WorkBlockData> workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas(GetType(), workId, attachType);
        if (workBlockDatas == null || workBlockDatas.Count == 0)
        {
            Lib.Core.DebugUtil.LogError($"{GetType()}类型没有数据WorkId：{workId}    AttachType : 0");
            return false;
        }

        StartChildMachine(stateBaseComponent, workId, workBlockDatas, blockType);

        return true;
    }

    /// <summary>
    /// StateBaseComponent节点附加上一个StateMachine,当该StateMachine结束该节点才结束
    /// </summary>
    protected virtual bool StartChildMachine(StateBaseComponent stateBaseComponent, uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)
    {
        WorkStreamTranstionComponent workStreamTranstionComponent = PrepareChildMachineInStateComponent(stateBaseComponent, workId, workBlockDatas);
        if (workStreamTranstionComponent == null)
            return false;

        if (blockType == 0)
            return workStreamTranstionComponent.StartWorkStream();
        else
            return workStreamTranstionComponent.StartWorkStream(blockType);
    }

    private WorkStreamTranstionComponent PrepareChildMachineInStateComponent(StateBaseComponent stateBaseComponent, uint workId, List<WorkBlockData> workBlockDatas)
    {
        if (stateBaseComponent == null || workId == 0u || workBlockDatas == null || workBlockDatas.Count <= 0)
            return null;

        stateBaseComponent.CreateChildMachineEntity();

        WorkStreamTranstionComponent workStreamTranstionComponent = stateBaseComponent.ChildMachineEntity.AddTranstion<WorkStreamTranstionComponent>();
        workStreamTranstionComponent.InitWorkBlockDatas(workId, workBlockDatas);

        return workStreamTranstionComponent;
    }

    /// <summary>
    /// 进入该Controller接口
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// 退出该Controller接口
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// 暂停该Controller接口
    /// </summary>
    public virtual void OnPause() { }
}
