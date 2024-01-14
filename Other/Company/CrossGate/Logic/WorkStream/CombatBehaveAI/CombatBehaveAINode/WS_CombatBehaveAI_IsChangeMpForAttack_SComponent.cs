using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsChangeMpForAttack)]
public class WS_CombatBehaveAI_IsChangeMpForAttack_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobCombatComponent mobCombatComponent = ((MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;
        
        if (behaveAIController.m_CurHpChangeData == null)
        {
            if (mobCombatComponent.m_HpChangeDataQueue.Count == 0)
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            CombatHpChangeData chcd = mobCombatComponent.m_HpChangeDataQueue.Peek();

            if (chcd.m_ChangeType == 1)
            {
                behaveAIController.m_CurHpChangeData = mobCombatComponent.m_HpChangeDataQueue.Dequeue();
#if DEBUG_MODE
                DLogManager.Log(ELogType.eCombat, $"HpChangeDataQueue.Dequeue-----<color=yellow>{mobCombatComponent.m_CurUseEntity.m_Go?.name}--Count:{mobCombatComponent.m_HpChangeDataQueue.Count.ToString()}    {MobCombatComponent.GetHpChangeDataDebugLog(behaveAIController.m_CurHpChangeData)}</color>");
#endif
            }
        }

        if (behaveAIController.m_CurHpChangeData != null && behaveAIController.m_CurHpChangeData.m_ChangeType == 1)
            m_CurUseEntity.TranstionMultiStates(this, 1, 1);
        else
            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
    }
}