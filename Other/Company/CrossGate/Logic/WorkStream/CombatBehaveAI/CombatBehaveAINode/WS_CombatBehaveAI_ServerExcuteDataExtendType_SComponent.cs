[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ServerExcuteDataExtendType)]
public class WS_CombatBehaveAI_ServerExcuteDataExtendType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        if (cbace.m_SourcesTurnBehaveSkillInfo != null)
            m_CurUseEntity.TranstionMultiStates(this, 1, (int)cbace.m_SourcesTurnBehaveSkillInfo.ExtendType);
        else
            m_CurUseEntity.TranstionMultiStates(this);
	}
}