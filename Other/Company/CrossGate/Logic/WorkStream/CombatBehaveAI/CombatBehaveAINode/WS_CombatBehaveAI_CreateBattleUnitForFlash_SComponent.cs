using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CreateBattleUnitForFlash)]
public class WS_CombatBehaveAI_CreateBattleUnitForFlash_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        int excuteIndex = -1;

        WS_CombatBehaveAIControllerEntity combatBehaveAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatBehaveAIControllerEntity;
        if (combatBehaveAIController == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        
        if (combatBehaveAIController.m_BehaveAIControllParam != null)
        {
            excuteIndex = combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex;
        }

        var mobEntity = (MobEntity)combatBehaveAIController.m_WorkStreamManagerEntity.Parent;
        if (mobEntity == null || mobEntity.m_MobCombatComponent == null || mobEntity.m_MobCombatComponent.m_BattleUnit == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(excuteIndex);
        if (excuteTurnData != null && excuteTurnData.ExcuteData != null && excuteTurnData.ExcuteData.Count > 0)
        {
            for (int i = 0, dataCount = excuteTurnData.ExcuteData.Count; i < dataCount; i++)
            {
                var uc = excuteTurnData.ExcuteData[i].UnitsChange;
                if (uc != null && uc.ReplaceType == 3u)
                {
                    if (uc.NewUnits != null && uc.NewUnits.Count > 0)
                    {
                        for (int newUnitIndex = 0, newUnitCount = uc.NewUnits.Count; newUnitIndex < newUnitCount; newUnitIndex++)
                        {
                            var newBattleUnit = uc.NewUnits[newUnitIndex];
                            if (newBattleUnit.Pos != mobEntity.m_MobCombatComponent.m_BattleUnit.Pos)
                                continue;

                            DLogManager.Log(ELogType.eCombat, $"<color=yellow>闪现进场的 NewUnits is Success  ServerPos:{newBattleUnit.Pos.ToString()}</color>");

                            Net_Combat.Instance.CreateNewUnit(combatBehaveAIController.m_SourcesTurnBehaveSkillInfo,
                                combatBehaveAIController.m_BehaveAIControllParam, uc.ReplaceType, newBattleUnit);
                        }
                    }
                }
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
    }
}