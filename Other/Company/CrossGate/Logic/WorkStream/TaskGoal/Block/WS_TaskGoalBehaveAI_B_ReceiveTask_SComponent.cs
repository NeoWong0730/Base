namespace Logic
{
    /// <summary>
    /// 任务表演块：接受任务///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.B_ReceiveTask)]
    public class WS_TaskGoalBehaveAI_B_ReceiveTask_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}
