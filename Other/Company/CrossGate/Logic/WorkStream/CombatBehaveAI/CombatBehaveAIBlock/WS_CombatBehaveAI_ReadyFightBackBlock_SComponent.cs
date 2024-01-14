[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ReadyFightBackBlock)]
public class WS_CombatBehaveAI_ReadyFightBackBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}