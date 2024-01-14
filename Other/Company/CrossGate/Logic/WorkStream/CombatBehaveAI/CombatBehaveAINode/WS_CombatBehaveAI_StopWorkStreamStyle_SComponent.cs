[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.StopWorkStreamStyle)]
public class WS_CombatBehaveAI_StopWorkStreamStyle_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        if (string.IsNullOrWhiteSpace(str))
        {
            cbace.m_WorkStreamManagerEntity.StopAll();
            return;
        }
        else
        {
            cbace.m_WorkStreamManagerEntity.StopAllByFilter(cbace);
            m_CurUseEntity.TranstionMultiStates(this);
        }
	}
}