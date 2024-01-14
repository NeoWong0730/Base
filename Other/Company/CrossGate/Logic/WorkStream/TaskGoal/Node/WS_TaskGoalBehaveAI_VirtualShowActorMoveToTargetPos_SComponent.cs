namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor移动到指定位置///
    /// 0: UID///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4：Speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorMoveToTargetPos)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorMoveToTargetPos_SComponent : WS_NPCBehaveAI_VirtualShowActorMoveToTargetPos_SComponent
    {
        
    }
}