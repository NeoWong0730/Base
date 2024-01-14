namespace Logic
{
    /// <summary>
    /// 任务表演节点：循环节点标记///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.LoopNodeMark)]
    public class WS_TaskGoalBehaveAI_LoopNodeMark_SComponent : StateBaseComponent
    {
        WS_TaskGoalDataComponent dataComponent;

        public override void Init(string str)
        {
            dataComponent = GetNeedComponent<WS_TaskGoalDataComponent>();
            if (dataComponent != null && dataComponent.m_LoopNodeMarkCount == 0)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    dataComponent.m_LoopNodeMarkCount = int.Parse(str);
                }
                dataComponent.m_LoopNodeMarkNodeId = m_DataNodeId;
            }

            m_CurUseEntity.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            dataComponent = null;

            base.Dispose();
        }
    }
}
