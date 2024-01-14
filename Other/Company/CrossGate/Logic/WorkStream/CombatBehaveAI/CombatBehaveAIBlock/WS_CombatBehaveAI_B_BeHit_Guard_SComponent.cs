[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_BeHit_Guard)]
public class WS_CombatBehaveAI_B_BeHit_Guard_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.GetTurnBehaveSkillTargetInfo(m_CurUseEntity);
        if (turnBehaveSkillTargetInfo != null)
        {
            if (cbace.m_AttachType == 0 || turnBehaveSkillTargetInfo.TargetUnitId != mob.m_MobCombatComponent.m_BattleUnit.UnitId)
                Lib.Core.DebugUtil.LogError($"{mob.m_Go?.name}执行WorkStream时m_AttachType{cbace.m_AttachType}获取TurnBehaveSkillTargetInfo时TargetUnitId：{turnBehaveSkillTargetInfo.TargetUnitId}    m_BattleUnit.UnitId:{mob.m_MobCombatComponent.m_BattleUnit.UnitId}");
            else
                cbace.SetProtectedInfo(true, mob.m_MobCombatComponent.m_BattleUnit.UnitId, turnBehaveSkillTargetInfo.BeProtectUnitId);
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}