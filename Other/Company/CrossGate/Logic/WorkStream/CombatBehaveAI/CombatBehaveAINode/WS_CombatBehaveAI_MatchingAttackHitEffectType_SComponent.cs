[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MatchingAttackHitEffectType)]
public class WS_CombatBehaveAI_MatchingAttackHitEffectType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob, out turnBehaveSkillTargetInfoIndex);

        int hitEffect = turnBehaveSkillTargetInfo != null ? (int)turnBehaveSkillTargetInfo.HitEffect : 
            (cbace.m_BehaveAIControllParam == null ? 0 : cbace.m_BehaveAIControllParam.HitEffect);

        m_CurUseEntity.TranstionMultiStates(this, 1, hitEffect);
	}
}