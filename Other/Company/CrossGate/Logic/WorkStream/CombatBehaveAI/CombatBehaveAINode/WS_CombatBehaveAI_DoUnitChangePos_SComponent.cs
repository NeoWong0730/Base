using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoUnitChangePos)]
public class WS_CombatBehaveAI_DoUnitChangePos_SComponent : StateBaseComponent
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

            behaveAIController.m_CurHpChangeData = mobCombatComponent.m_HpChangeDataQueue.Dequeue();
#if DEBUG_MODE
            DLogManager.Log(ELogType.eCombat, $"HpChangeDataQueue.Dequeue-----<color=yellow>{mobCombatComponent.m_CurUseEntity.m_Go?.name}--Count:{mobCombatComponent.m_HpChangeDataQueue.Count.ToString()}    {MobCombatComponent.GetHpChangeDataDebugLog(behaveAIController.m_CurHpChangeData)}</color>");
#endif
        }

        if (behaveAIController.m_CurHpChangeData.m_UnitChangePos > -1)
        {
            mobCombatComponent.ChangeUnitServerPos(behaveAIController.m_CurHpChangeData.m_UnitChangePos);
        }
        m_CurUseEntity.TranstionMultiStates(this);
	}
}