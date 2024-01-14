namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家移动到指定NPC位置///
    /// 0: TargetNPCID///
    /// 1: Speed///
    /// 2：OffsetPosX///
    /// 3: OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.MainHeroMoveToNPC)]
    public class WS_TaskGoalBehaveAI_MainHeroMoveToNPC_SComponent : WS_NPCBehaveAI_MainHeroMoveToNPC_SComponent
    {
        
    }
}
