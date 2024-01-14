[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoProcessAfterHit)]
public class WS_CombatBehaveAI_DoProcessAfterHit_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob, out turnBehaveSkillTargetInfoIndex);

        if (turnBehaveSkillTargetInfo != null)
        {
            var target = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);

            int isCanProcessTypeAfterHit;
            if (string.IsNullOrWhiteSpace(str))
                isCanProcessTypeAfterHit = 0;
            else
                isCanProcessTypeAfterHit = ~int.Parse(str);

            WS_CombatBehaveAIControllerEntity.BulletHitToProcess(mob, target,
                            cbace.m_BehaveAIControllParam == null ? -1 : cbace.m_BehaveAIControllParam.ExcuteTurnIndex,
                            cbace.m_SourcesTurnBehaveSkillInfo, turnBehaveSkillTargetInfoIndex, isCanProcessTypeAfterHit);
        }

        dataComponent.m_IsCanProcessTypeAfterHit = 0; 

        m_CurUseEntity.TranstionMultiStates(this);
	}
}