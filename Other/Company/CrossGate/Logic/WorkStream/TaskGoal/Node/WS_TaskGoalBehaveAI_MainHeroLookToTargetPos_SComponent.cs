namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家朝向指定位置///
    /// 0: TargetPosX///
    /// 1: TargetPosY///
    /// 2: TargetPosZ///
    /// 3: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.MainHeroLookToTargetPos)]
    public class WS_TaskGoalBehaveAI_MainHeroLookToTargetPos_SComponent : WS_NPCBehaveAI_MainHeroLookToTargetPos_SComponent
    {
        
    }
}
