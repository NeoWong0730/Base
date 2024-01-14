[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MatchingCombatUnitType)]
public class WS_CombatBehaveAI_MatchingCombatUnitType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        MobCombatComponent mobCombatComponent = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

        int combatUnitType = int.Parse(str);
        if ((combatUnitType & (int)CombatHelp.SwitchCombatUnitType(mobCombatComponent.m_BattleUnit.UnitType)) > 0)
            m_CurUseEntity.TranstionMultiStates(this, 1, 1);
        else
            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
	}
}