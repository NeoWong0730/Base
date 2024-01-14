public class StateBaseComponent : BaseComponentRepeat<StateMachineEntity>
{
    public ushort m_DataNodeId;

    /// <summary>
    /// StateBaseComponent节点上附加的一个子StateMachine,当该子StateMachine结束了该节点才结束
    /// </summary>
    public StateMachineEntity ChildMachineEntity;

    public void CreateChildMachineEntity()
    {
        if (ChildMachineEntity != null)
        {
            Lib.Core.DebugUtil.LogError($"{GetType().ToString()}组件重复生成ChildMachineEntity");
            return;
        }

        ChildMachineEntity = m_CurUseEntity.m_StateControllerEntity.CreateChildStateMachineEntity(m_CurUseEntity.m_LayerIndex + 1,
                                m_CurUseEntity, m_CurUseEntity.m_StateControllerEntity.m_StateECManager.GetStateId(GetType()), Id);
    }

    public override void Dispose()
    {
        if(m_CurUseEntity != null && m_CurUseEntity.m_EndStateAction != null)
            m_CurUseEntity.m_EndStateAction.Invoke(this);
        
        if (ChildMachineEntity != null)
        {
            ChildMachineEntity.Free();
            ChildMachineEntity = null;
        }

        m_DataNodeId = 0;

        base.Dispose();
    }

    public virtual void Init(string str) { }
}
