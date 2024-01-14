[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SetBattleStage)]
public class WS_CombatBehaveAI_SetBattleStage_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        Net_Combat.Instance.m_CurClientBattleStage = uint.Parse(str);

		m_CurUseEntity.TranstionMultiStates(this);
	}
}