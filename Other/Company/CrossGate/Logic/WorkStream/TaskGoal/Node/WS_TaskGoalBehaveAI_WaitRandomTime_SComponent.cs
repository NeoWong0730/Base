namespace Logic
{
    /// <summary>
    /// 任务表演节点：等待随机时间///
    /// 0：时间下限///
    /// 1：时间上限///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.WaitRandomTime)]
    public class WS_TaskGoalBehaveAI_WaitRandomTime_SComponent : WS_NPCBehaveAI_WaitRandomTime_SComponent
    {
    }
}
