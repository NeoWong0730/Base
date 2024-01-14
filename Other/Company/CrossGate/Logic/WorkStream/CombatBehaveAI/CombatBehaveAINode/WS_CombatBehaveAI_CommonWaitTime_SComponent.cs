using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CommonWaitTime)]
public class WS_CombatBehaveAI_CommonWaitTime_SComponent : StateBaseComponent, IUpdate
{
    public override void Init(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = (WS_CombatBehaveAIManagerEntity)((WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity;

        combatBehaveAIManagerEntity.m_CommonWaitTime = float.Parse(str) + Time.time;
    }

    public void Update()
    {
        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = (WS_CombatBehaveAIManagerEntity)((WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity;

        if (Time.time < combatBehaveAIManagerEntity.m_CommonWaitTime)
            return;

        m_CurUseEntity.TranstionMultiStates(this);
    }
}