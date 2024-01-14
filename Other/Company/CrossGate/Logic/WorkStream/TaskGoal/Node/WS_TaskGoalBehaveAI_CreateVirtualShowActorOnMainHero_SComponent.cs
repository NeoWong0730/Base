namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家位置生成虚拟Actor///
    /// 0: UID///
    /// 1: Type///
    /// 2: InfoID///
    /// 3: OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// 6: RotX///
    /// 7: RotY///
    /// 8: RotZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.CreateVirtualShowActorOnMainHero)]
    public class WS_TaskGoalBehaveAI_CreateVirtualShowActorOnMainHero_SComponent : WS_NPCBehaveAI_CreateVirtualShowActorOnMainHero_SComponent
    {
        
    }
}