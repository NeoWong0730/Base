[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BackToPollingNodeMark)]
public class WS_CombatBehaveAI_BackToPollingNodeMark_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        dataComponent.m_LoopNodeMarkCount--;
        dataComponent.m_AttackTargetIndex++;

        if (dataComponent.m_LoopNodeMarkCount > 0)
            m_CurUseEntity.SkipState(this, dataComponent.m_LoopNodeMarkNodeId, dataComponent.m_LoopWorkBlockType);
        else
        {
            dataComponent.m_LoopNodeMarkCount = 0;
            dataComponent.m_LoopNodeMarkNodeId = 0;
            m_CurUseEntity.TranstionMultiStates(this);
        }
	}
}