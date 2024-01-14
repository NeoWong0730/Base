[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_ReleaseSkillImmediate_Before)]
public class WS_CombatBehaveAI_B_ReleaseSkillImmediate_Before_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent workStreamTranstionComponent = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;

        m_CurUseEntity.TranstionMultiStates(this);

        if (workStreamTranstionComponent != null && workStreamTranstionComponent.Id != 0L)
            workStreamTranstionComponent.Update();
    }
}