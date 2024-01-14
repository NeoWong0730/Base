[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CheckCurMonsterGroup)]
public class WS_CombatBehaveAI_CheckCurMonsterGroup_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this, 1, Logic.Sys_Fight.curMonsterGroupId == uint.Parse(str) ? 1 : 0);
	}
}