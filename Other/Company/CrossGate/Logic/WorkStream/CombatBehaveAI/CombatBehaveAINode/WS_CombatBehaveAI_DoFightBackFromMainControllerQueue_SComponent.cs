[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoFightBackFromMainControllerQueue)]
public class WS_CombatBehaveAI_DoFightBackFromMainControllerQueue_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        cbace.m_WorkStreamManagerEntity.DoImmediateMainStreamControllerStack((BaseStreamControllerEntity baseStreamControllerEntity) =>
        {
            WS_CombatBehaveAIControllerEntity c = baseStreamControllerEntity as WS_CombatBehaveAIControllerEntity;
            if (c == null || c.m_SourcesTurnBehaveSkillInfo == null || c.m_SourcesTurnBehaveSkillInfo.ExtendType != 5u)
                return false;
            
            return true;
        });

        m_CurUseEntity.TranstionMultiStates(this);
	}
}