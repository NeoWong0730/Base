using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoPassiveTiming)]
public class WS_CombatBehaveAI_DoPassiveTiming_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (string.IsNullOrWhiteSpace(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        if (cbace.m_BehaveAIControllParam != null && cbace.m_BehaveAIControllParam.ExcuteTurnIndex > -1 && 
            cbace.m_SourcesTurnBehaveSkillInfo != null &&
            cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null)
        {
            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);
            if (turnBehaveSkillTargetInfo != null)
                Net_Combat.Instance.DoPassive(cbace.m_BehaveAIControllParam.ExcuteTurnIndex, turnBehaveSkillTargetInfo.SNodeId, turnBehaveSkillTargetInfo.SNodeLayer, uint.Parse(str));
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}