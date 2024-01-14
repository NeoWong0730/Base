namespace Logic
{
    /// <summary>
    /// 任务表演块：任务目标完成///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.B_TaskGoalCompleted)]
    public class WS_TaskGoalBehaveAI_B_TaskGoalCompleted_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}
