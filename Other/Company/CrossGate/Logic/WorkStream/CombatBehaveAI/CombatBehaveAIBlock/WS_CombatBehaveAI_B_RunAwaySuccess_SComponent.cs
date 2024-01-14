using Lib.Core;
using Logic;
using Packet;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_RunAwaySuccess)]
public class WS_CombatBehaveAI_B_RunAwaySuccess_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mobCombatComponent = ((MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;
        if ((behaveAIController.m_CurHpChangeData != null && behaveAIController.m_CurHpChangeData.m_Death) || (behaveAIController.m_CurHpChangeData == null && mobCombatComponent.m_Death))
        {
            m_CurUseEntity.StateMachineOver();
            return;
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}