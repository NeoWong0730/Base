[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_HitEffectTypeAbsorb)]
public class WS_CombatBehaveAI_B_HitEffectTypeAbsorb_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}