namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor移动到指定虚拟Actor位置///
    /// 0: UID///
    /// 1: TargetUID///
    /// 2: Speed///
    /// 3：OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorMoveToTargetVirtualShowActor)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorMoveToTargetVirtualShowActor_SComponent : WS_NPCBehaveAI_VirtualShowActorMoveToTargetVirtualShowActor_SComponent
    {
       
    }
}
