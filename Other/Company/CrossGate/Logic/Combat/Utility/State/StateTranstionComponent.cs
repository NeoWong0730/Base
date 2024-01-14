public abstract class StateTranstionComponent : BaseComponent<StateMachineEntity>
{
    /// <summary>
    /// 会获取一个或多个state
    /// type=-1获取前一个状态，=0获取当前状态，=1获取下一个状态
    /// selectIndex是oldState需要转换到选择种类的states
    /// </summary>
    public abstract void GetStates(int type, ushort oldNodeId, int selectType);

    public abstract bool SkipBlock(int newBlockState);

    public abstract void SkipState(ushort oldNodeId, ushort newNodeId, int newWorkBlockType = 0);
    
    //public abstract void DisposeState(uint nodeId);
} 
