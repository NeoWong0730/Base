[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.HinderNode)]
public class WS_CombatBehaveAI_HinderNode_SComponent : StateBaseComponent
{
    private string _tag;

	public override void Init(string str)
	{
        _tag = str;

        CombatManager.Instance.m_EventEmitter.Handle<int, string>(CombatManager.EEvents.HinderNode, HinderNode_Evt, true);
	}

    public override void Dispose()
    {
        CombatManager.Instance.m_EventEmitter.Handle<int, string>(CombatManager.EEvents.HinderNode, HinderNode_Evt, false);

        _tag = null;

        base.Dispose();
    }

    private void HinderNode_Evt(int turnBehaveSkillTargetInfoId, string tag)
    {
        if (tag == _tag)
        {
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.GetTurnBehaveSkillTargetInfo(m_CurUseEntity);

            if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.Id == (uint)turnBehaveSkillTargetInfoId)
                m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}