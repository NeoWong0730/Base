[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsPlayer)]
public class WS_CombatBehaveAI_IsPlayer_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mobCombatComponent = ((MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

        m_CurUseEntity.TranstionMultiStates(this, 1, MobManager.Instance.IsPlayer(mobCombatComponent.m_BattleUnit) ? 1 : 0);
	}
}