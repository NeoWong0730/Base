[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.AddCombatUnitBlock)]
public class WS_CombatBehaveAI_AddCombatUnitBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        mob.m_Trans.position = CombatHelp.FarV3;

        m_CurUseEntity.TranstionMultiStates(this);
	}
}