namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：记录初始点位置///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.CacheStartTransform)]
    public class WS_NPCBehaveAI_CacheStartTransform_SComponent : StateBaseComponent
    {
        WS_NPCControllerEntity entity;
        WS_NPCDataComponent dataComponent;

        public override void Init(string str)
        {
            entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;
            dataComponent = GetNeedComponent<WS_NPCDataComponent>();
            dataComponent.cacheStartPos = entity.m_Go.transform.position;
            m_CurUseEntity.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            entity = null;
            dataComponent = null;

            base.Dispose();
        }
    }
}
