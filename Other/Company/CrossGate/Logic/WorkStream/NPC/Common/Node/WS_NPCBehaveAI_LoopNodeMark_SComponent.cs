namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：循环节点标记///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.LoopNodeMark)]
    public class WS_NPCBehaveAI_LoopNodeMark_SComponent : StateBaseComponent
    {
        WS_NPCDataComponent dataComponent;

        public override void Init(string str)
        {
            dataComponent = GetNeedComponent<WS_NPCDataComponent>();
            if (dataComponent.m_LoopNodeMarkCount == 0)
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

