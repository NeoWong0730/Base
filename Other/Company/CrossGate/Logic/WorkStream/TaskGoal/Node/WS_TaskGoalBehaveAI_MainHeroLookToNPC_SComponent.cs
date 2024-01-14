namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家朝向指定NPC位置///
    /// 0: TargetNPCID///
    /// 1: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.MainHeroLookToNPC)]
    public class WS_TaskGoalBehaveAI_MainHeroLookToNPC_SComponent : WS_NPCBehaveAI_MainHeroLookToNPC_SComponent
    {
        
    }
}
