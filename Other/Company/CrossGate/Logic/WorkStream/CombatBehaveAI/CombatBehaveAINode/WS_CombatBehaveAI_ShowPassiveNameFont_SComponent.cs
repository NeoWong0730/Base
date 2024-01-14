using Logic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ShowPassiveNameFont)]
public class WS_CombatBehaveAI_ShowPassiveNameFont_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        if (cbace.m_SourcesTurnBehaveSkillInfo != null)
        {
            MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
            Packet.BattleUnit unit = mob.m_MobCombatComponent.m_BattleUnit;

            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);

            if (cbace.m_SourcesTurnBehaveSkillInfo.ExtendType == 1u && turnBehaveSkillTargetInfo.ExtendId > 0u)
            {
                CombatManager.Instance.ShowPassiveName(unit.UnitId, turnBehaveSkillTargetInfo.ExtendId);
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}