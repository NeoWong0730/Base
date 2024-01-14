namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家移动到指定虚拟Actor位置///
    /// 0: TargetUID///
    /// 1: Speed///
    /// 2：OffsetPosX///
    /// 3: OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.MainHeroMoveToTargetVirtualShowActor)]
    public class WS_TaskGoalBehaveAI_MainHeroMoveToTargetVirtualShowActor_SComponent : WS_NPCBehaveAI_MainHeroMoveToTargetVirtualShowActor_SComponent
    {
        
    }
}