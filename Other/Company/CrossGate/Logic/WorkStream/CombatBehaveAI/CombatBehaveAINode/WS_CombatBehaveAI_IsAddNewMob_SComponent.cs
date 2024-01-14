using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsAddNewMob)]
public class WS_CombatBehaveAI_IsAddNewMob_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        int excuteIndex = -1;
        
        WS_CombatBehaveAIControllerEntity combatBehaveAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatBehaveAIControllerEntity;
        if (combatBehaveAIController == null)
        {
            var combatSceneAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatSceneAIControllerEntity;
            if (combatSceneAIController != null)
                excuteIndex = combatSceneAIController.m_ExcuteIndex;
        }
        else if (combatBehaveAIController.m_BehaveAIControllParam != null)
        {
            excuteIndex = combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex;
        }

        var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(excuteIndex);
        if (excuteTurnData == null)
        {
            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
            return;
        }
        else if (excuteTurnData.ExcuteData != null && excuteTurnData.ExcuteData.Count > 0)
        {
            for (int i = 0, dataCount = excuteTurnData.ExcuteData.Count; i < dataCount; i++)
            {
                var data = excuteTurnData.ExcuteData[i];
                if (data.UnitsChange != null)
                {
                    if (data.UnitsChange.NewUnits != null && data.UnitsChange.NewUnits.Count > 0)
                    {
                        DLogManager.Log(ELogType.eCombat, $"<color=yellow>NewUnits is Success</color>");

                        for (int newUnitIndex = 0, newUnitCount = data.UnitsChange.NewUnits.Count; newUnitIndex < newUnitCount; newUnitIndex++)
                        {
                            Net_Combat.Instance.OnAddOrDelUnit(true, data.UnitsChange.NewUnits[newUnitIndex], true);
                        }

                        m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                        return;
                    }
                }
            }
        }

        DLogManager.Log(ELogType.eCombat, $"<color=yellow>NewUnits is Fail</color>");

        m_CurUseEntity.TranstionMultiStates(this, 1, 0);
    }
}