[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_HitEffectTypeSpurting)]
public class WS_CombatBehaveAI_B_HitEffectTypeSpurting_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}