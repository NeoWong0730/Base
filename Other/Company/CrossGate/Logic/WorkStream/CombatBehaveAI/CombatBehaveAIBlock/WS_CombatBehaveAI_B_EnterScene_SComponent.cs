[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_EnterScene)]
public class WS_CombatBehaveAI_B_EnterScene_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent workStreamTranstionComponent = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;

        m_CurUseEntity.TranstionMultiStates(this);

        if (workStreamTranstionComponent != null)
            workStreamTranstionComponent.Update();
    }
}