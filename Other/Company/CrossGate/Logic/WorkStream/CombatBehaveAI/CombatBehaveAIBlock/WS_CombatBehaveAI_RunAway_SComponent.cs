using Lib.Core;
using Logic;
using Packet;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RunAway)]
public class WS_CombatBehaveAI_RunAway_SComponent : StateBaseComponent
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

        var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(behaveAIController.m_BehaveAIControllParam == null ? -1 : behaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex);
        if (excuteTurnData == null)
        {
            if (MobManager.Instance.IsPlayer(mobCombatComponent.m_BattleUnit))
            {
                Net_Combat.Instance.m_BattleOverType = 1u;
            }
            mobCombatComponent.m_IsRunAway = true;
            m_CurUseEntity.TranstionMultiStates(this, 1, 1);
            return;
        }
        else if (excuteTurnData.ExcuteData != null && excuteTurnData.ExcuteData.Count > 0)
        {
            for (int i = 0, dataCount = excuteTurnData.ExcuteData.Count; i < dataCount; i++)
            {
                var data = excuteTurnData.ExcuteData[i];
                if (data.UnitsChange != null)
                {
                    if (data.UnitsChange.EscapeUnitId != null && data.UnitsChange.EscapeUnitId.Count > 0)
                    {
                        for (int j = 0, delCount = data.UnitsChange.EscapeUnitId.Count; j < delCount; j++)
                        {
                            if (mobCombatComponent.m_BattleUnit.UnitId == data.UnitsChange.EscapeUnitId[j])
                            {
                                DLogManager.Log(ELogType.eCombat, $"<color=yellow>RunAway is Success</color>");

                                Net_Combat.Instance.OnAddOrDelUnit(true, mobCombatComponent.m_BattleUnit, false);

                                if (MobManager.Instance.IsPlayer(mobCombatComponent.m_BattleUnit))
                                {
                                    Net_Combat.Instance.m_BattleOverType = 1u;
                                }
                                mobCombatComponent.m_IsRunAway = true;
                                m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                                return;
                            }
                        }
                    }
                    if (data.UnitsChange.EscapeFailUnitId != null && data.UnitsChange.EscapeFailUnitId.Count > 0)
                    {
                        for (int ucIndex = 0, ucCount = data.UnitsChange.EscapeFailUnitId.Count; ucIndex < ucCount; ucIndex++)
                        {
                            if (mobCombatComponent.m_BattleUnit.UnitId == data.UnitsChange.EscapeFailUnitId[ucIndex])
                            {
                                DLogManager.Log(ELogType.eCombat, $"<color=yellow>RunAway is Fail</color>");

                                mobCombatComponent.m_IsRunAway = false;
                                m_CurUseEntity.TranstionMultiStates(this, 1, 0);
                                return;
                            }
                        }
                    }
                }
            }
        }

        DLogManager.Log(ELogType.eCombat, $"<color=yellow>RunAway is Fail</color>");

        mobCombatComponent.m_IsRunAway = false;
        m_CurUseEntity.TranstionMultiStates(this, 1, 0);
    }
}