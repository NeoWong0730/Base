namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor朝向指定NPC位置///
    /// 0: UID///
    /// 1: TargetNPCID///
    /// 2: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorLookToNPC)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorLookToNPC_SComponent : WS_NPCBehaveAI_VirtualShowActorLookToNPC_SComponent
    {
        
    }
}
