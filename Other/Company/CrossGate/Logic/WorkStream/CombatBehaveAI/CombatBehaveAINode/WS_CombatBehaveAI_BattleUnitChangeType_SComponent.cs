[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BattleUnitChangeType)]
public class WS_CombatBehaveAI_BattleUnitChangeType_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        if (cbace.m_BehaveAIControllParam == null || cbace.m_BehaveAIControllParam.m_BattleReplaceType == 0)
            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
        else
            m_CurUseEntity.TranstionMultiStates(this, 1, (int)cbace.m_BehaveAIControllParam.m_BattleReplaceType);
    }
}