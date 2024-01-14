[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsSetOriginState)]
public class WS_CombatBehaveAI_IsSetOriginState_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        mob.m_MobCombatComponent.ResetTrans(true);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}