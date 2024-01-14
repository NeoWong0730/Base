namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor移动到指定NPC位置///
    /// 0: UID///
    /// 1: TargetNPCID///
    /// 2: Speed///
    /// 3：OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorMoveToNPC)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorMoveToNPC_SComponent : WS_NPCBehaveAI_VirtualShowActorMoveToNPC_SComponent
    {
        
    }
}
