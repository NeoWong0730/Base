[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DelGhostShadow)]
public class WS_CombatBehaveAI_DelGhostShadow_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).GetComponent<PlayGhostShadowComponent>().Dispose();

        m_CurUseEntity.TranstionMultiStates(this);
	}
}