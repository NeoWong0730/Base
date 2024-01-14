[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MatchBuffId)]
public class WS_CombatBehaveAI_MatchBuffId_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = null;
        if (cbace.m_SourcesTurnBehaveSkillInfo != null)
        {
            MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);
        }

        int selectType = (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.ExtendId == uint.Parse(str)) ? 1 : 0;

        m_CurUseEntity.TranstionMultiStates(this, 1, selectType);
	}
}