[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SelectTargetState)]
public class WS_CombatBehaveAI_SelectTargetState_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var combatComponent = ((MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;
        if (behaveAIController.m_CurHpChangeData != null && behaveAIController.m_CurHpChangeData.m_Death)
        {
            //behaveAIController.m_WorkStreamManagerEntity.StopAllByFilter(behaveAIController);

            if (combatComponent.m_BeHitToFlyState)
                m_CurUseEntity.TranstionMultiStates(this, 1, 2);
            else
                m_CurUseEntity.TranstionMultiStates(this, 1, 0);
        }
        else
            m_CurUseEntity.TranstionMultiStates(this, 1, 1);
	}
}