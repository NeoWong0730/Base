using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.IsDelObjSuccess)]
public class WS_CombatBehaveAI_IsDelObjSuccess_SComponent : StateBaseComponent
{
    public override void Init(string str)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        if (cbace.m_BehaveAIControllParam == null)
        {
            Net_Combat.Instance.OnAddOrDelUnit(false, null, false);

            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
            return;
        }
        
        var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
        if (excuteTurnData == null)
        {
            Net_Combat.Instance.OnAddOrDelUnit(false, null, false);

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
                    if (data.UnitsChange.DelUnitId != null && data.UnitsChange.DelUnitId.Count > 0)
                    {
                        bool isExistDelUnit = false;
                        for (int delUnitIndex = 0, delUnitCount = data.UnitsChange.DelUnitId.Count; delUnitIndex < delUnitCount; delUnitIndex++)
                        {
                            uint delUnitId = data.UnitsChange.DelUnitId[delUnitIndex];
                            var delMob = MobManager.Instance.GetMob(delUnitId);
                            if (delMob != null)
                            {
                                Net_Combat.Instance.OnAddOrDelUnit(true, delMob.m_MobCombatComponent.m_BattleUnit, false);
                                isExistDelUnit = true;
                            }
                        }

                        if (isExistDelUnit)
                        {
                            DLogManager.Log(ELogType.eCombat, $"<color=yellow>DelUnitId is Success</color>");

                            m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                            return;
                        }
                    }
                    if (data.UnitsChange.DelFailUnitId != null && data.UnitsChange.DelFailUnitId.Count > 0)
                    {
                        bool isExistDelFail = false;
                        for (int delFailIndex = 0, delFailCount = data.UnitsChange.DelFailUnitId.Count; delFailIndex < delFailCount; delFailIndex++)
                        {
                            uint delFailUnitId = data.UnitsChange.DelFailUnitId[delFailIndex];
                            var delFailMob = MobManager.Instance.GetMob(delFailUnitId);
                            if (delFailMob != null)
                            {
                                Net_Combat.Instance.OnAddOrDelUnit(false, delFailMob.m_MobCombatComponent.m_BattleUnit, false);
                                isExistDelFail = true;
                            }
                        }

                        if (isExistDelFail)
                        {
                            DLogManager.Log(ELogType.eCombat, $"<color=yellow>DelUnitId is Fail</color>");
                            
                            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
                            return;
                        }
                    }
                }
            }
        }
        
        DLogManager.Log(ELogType.eCombat, $"<color=yellow>DelUnitId is Fail</color>");

        Net_Combat.Instance.OnAddOrDelUnit(false, null, false);

        m_CurUseEntity.TranstionMultiStates(this, 1, 0);
    }
}