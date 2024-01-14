namespace Logic
{
    /// <summary>
    /// 任务表演节点：NPC移动到指定虚拟Actor位置///
    /// 0: npcinfoid///
    /// 1: TargetUID///
    /// 2: Speed///
    /// 3：OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.NPCMoveToTargetVirtualShowActor)]
    public class WS_TaskGoalBehaveAI_NPCMoveToTargetVirtualShowActor_SComponent : WS_NPCBehaveAI_NPCMoveToTargetVirtualShowActor_SComponent
    {
        
    }
}