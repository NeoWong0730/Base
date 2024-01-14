[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BattlePosType)]
public class WS_CombatBehaveAI_BattlePosType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this, 1, (int)CombatManager.Instance.m_BattlePosType);
	}
}