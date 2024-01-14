[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RemoveHinderNode)]
public class WS_CombatBehaveAI_RemoveHinderNode_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.GetTurnBehaveSkillTargetInfo(m_CurUseEntity);

        CombatManager.Instance.m_EventEmitter.Trigger(CombatManager.EEvents.HinderNode, turnBehaveSkillTargetInfo == null ? 0 : (int)turnBehaveSkillTargetInfo.Id, str);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}