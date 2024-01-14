[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ProjectIsOver)]
public class WS_CombatBehaveAI_ProjectIsOver_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        //WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        //int turnBehaveSkillTargetInfoIndex;
        //TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.
        //    GetTurnBehaveSkillTargetInfo(m_CurUseEntity, out turnBehaveSkillTargetInfoIndex);

        //if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.IsProtectOver)
        //    m_CurUseEntity.TranstionMultiStates(this, 1, 1);
        //else
        //    m_CurUseEntity.TranstionMultiStates(this);

        m_CurUseEntity.TranstionMultiStates(this, 1, 1);
    }
}