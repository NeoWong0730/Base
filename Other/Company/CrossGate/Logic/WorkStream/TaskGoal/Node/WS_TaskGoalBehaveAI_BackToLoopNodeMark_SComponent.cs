namespace Logic
{
    /// <summary>
    /// 任务表演节点：返回循环节点标记///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.BackToLoopNodeMark)]
    public class WS_TaskGoalBehaveAI_BackToLoopNodeMark_SComponent : StateBaseComponent
    {
        WS_TaskGoalDataComponent dataComponent;

        public override void Init(string str)
        {
            dataComponent = GetNeedComponent<WS_TaskGoalDataComponent>();
            if (dataComponent == null)
                return;

            dataComponent.m_LoopNodeMarkCount--;

            if (dataComponent.m_LoopNodeMarkCount > 0)
                m_CurUseEntity.SkipState(this, dataComponent.m_LoopNodeMarkNodeId);
            else
            {
                dataComponent.m_LoopNodeMarkCount = 0;
                dataComponent.m_LoopNodeMarkNodeId = 0;
                m_CurUseEntity.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            dataComponent = null;

            base.Dispose();
        }
    }
}
