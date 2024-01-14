namespace Logic
{
    /// <summary>
    /// 任务表演节点：等待一定时间///
    /// Str:等待时间///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.WaitTime)]
    public class WS_TaskGoalBehaveAI_WaitTime_SComponent : WS_NPCBehaveAI_WaitTime_SComponent
    {
    }
}
