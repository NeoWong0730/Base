[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsHaveFightBackBehave)]
public class WS_CombatBehaveAI_IsHaveFightBackBehave_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        if (mob != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_BattleUnit != null)
        {
            int turnBehaveSkillTargetInfoIndex;
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.
                GetTurnBehaveSkillTargetInfo(m_CurUseEntity, out turnBehaveSkillTargetInfoIndex);

            if (turnBehaveSkillTargetInfo != null &&
                Net_Combat.Instance.DoFightBackTurnBehaveInfo(cbace.m_BehaveAIControllParam.ExcuteTurnIndex,
                    turnBehaveSkillTargetInfo.SNodeId, turnBehaveSkillTargetInfo.SNodeLayer,
                    turnBehaveSkillTargetInfo.TargetUnitId,
                    cbace.m_AttachType == 0 ? mob.m_MobCombatComponent.m_BattleUnit.UnitId : cbace.m_BehaveAIControllParam.SrcUnitId, 
                    string.IsNullOrWhiteSpace(str)))
            {
                m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                return;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}