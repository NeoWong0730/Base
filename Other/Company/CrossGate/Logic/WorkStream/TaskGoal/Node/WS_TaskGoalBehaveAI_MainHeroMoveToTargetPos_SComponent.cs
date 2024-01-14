namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家移动到指定位置///
    /// 0: TargetPosX///
    /// 1: TargetPosY///
    /// 2: TargetPosZ///
    /// 3：Speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.MainHeroMoveToTargetPos)]
    public class WS_TaskGoalBehaveAI_MainHeroMoveToTargetPos_SComponent : WS_NPCBehaveAI_MainHeroMoveToTargetPos_SComponent
    {
        
    }
}
