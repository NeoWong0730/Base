namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：返回循环节点标记///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.BackToLoopNodeMark)]
    public class WS_NPCBehaveAI_BackToLoopNodeMark_SComponent : StateBaseComponent
    {
        WS_NPCDataComponent dataComponent;
    
        public override void Init(string str)
        {
            dataComponent = GetNeedComponent<WS_NPCDataComponent>();
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
