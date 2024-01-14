[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.StopGhostShadow)]
public class WS_CombatBehaveAI_StopGhostShadow_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).GetComponent<PlayGhostShadowComponent>().SetCreateState(false);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}