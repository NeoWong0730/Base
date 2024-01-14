using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.LookAtTarget)]
public class WS_CombatBehaveAI_LookAtTarget_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        MobEntity targetMob = null;
        if (cbace.m_AttachType == 0)
        {
            if (cbace.m_SourcesTurnBehaveSkillInfo != null && cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount > 0)
            {
                WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);

                if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.TargetUnitId > 0u)
                    targetMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
            }
        }
        else
        {
            if (cbace.m_BehaveAIControllParam != null && cbace.m_BehaveAIControllParam.SrcUnitId > 0u)
                targetMob = MobManager.Instance.GetMob(cbace.m_BehaveAIControllParam.SrcUnitId);
        }

        if (targetMob != null)
        {
            Vector3 lookTargetPos = targetMob.m_Trans.position;
            lookTargetPos.y = mob.m_Trans.position.y;
            mob.m_Trans.LookAt(lookTargetPos);
            if (!string.IsNullOrWhiteSpace(str))
                targetMob.m_Trans.LookAt(mob.m_Trans.position);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}