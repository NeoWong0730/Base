[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.EnterSceneEffectInBattleType)]
public class WS_CombatBehaveAI_EnterSceneEffectInBattleType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this, 1, (int)CombatManager.Instance.m_BattleTypeTb.enter_battle_effect);
	}
}