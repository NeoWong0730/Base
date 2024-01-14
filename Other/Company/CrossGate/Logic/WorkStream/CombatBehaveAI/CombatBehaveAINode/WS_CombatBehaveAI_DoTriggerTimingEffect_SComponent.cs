using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoTriggerTimingEffect)]
public class WS_CombatBehaveAI_DoTriggerTimingEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (string.IsNullOrWhiteSpace(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        
        WS_CombatBehaveAIControllerEntity combatBehaveAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatBehaveAIControllerEntity;
        if (combatBehaveAIController == null)
        {
            var combatSceneAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatSceneAIControllerEntity;
            if (combatSceneAIController != null && combatSceneAIController.m_OutBoInfo != null &&
                combatSceneAIController.m_ExcuteIndex > -1)
            {
                Net_Combat.Instance.DoTriggerTimingEffect(combatSceneAIController.m_ExcuteIndex, combatSceneAIController.m_OutBoInfo.Stage, combatSceneAIController.m_OutBoInfo.Bo, uint.Parse(str));
            }
        }
        else
        {
            MobEntity mob = (MobEntity)combatBehaveAIController.m_WorkStreamManagerEntity.Parent;
            if (combatBehaveAIController.m_BehaveAIControllParam != null && combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex > -1)
            {
                WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(combatBehaveAIController, mob);

                if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.SNodeLayer == 1)
                    Net_Combat.Instance.DoTriggerTimingEffect(combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex, 
                        Net_Combat.Instance.GetExcuteTurnStage(combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex), 
                        turnBehaveSkillTargetInfo.Bo, uint.Parse(str));
                else
                    DebugUtil.Log(ELogType.eCombatBehave, $"{(turnBehaveSkillTargetInfo == null ? "TurnBehaveSkillTargetInfoList[0] is null" : $"ExtendId:{turnBehaveSkillTargetInfo.ExtendId.ToString()}   TimingLifeId:{turnBehaveSkillTargetInfo.SNodeId.ToString()}   TimingLayer:{turnBehaveSkillTargetInfo.SNodeLayer.ToString()}")}");
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}