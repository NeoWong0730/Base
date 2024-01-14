namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor移动到玩家位置///
    /// 0: UID///
    /// 1: Speed///
    /// 2: OffsetPosX///
    /// 3：OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorMoveToMainHero)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorMoveToMainHero_SComponent : WS_NPCBehaveAI_VirtualShowActorMoveToMainHero_SComponent
    {
        
    }
}
