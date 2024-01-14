[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.HitEffect)]
public class WS_CombatBehaveAI_HitEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mobCombatComponent = ((MobEntity)cbace.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

        if (cbace.m_BehaveAIControllParam == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
        if (excuteTurnData == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        if (excuteTurnData.ExcuteData != null && excuteTurnData.ExcuteData.Count > 0)
        {
            for (int i = dataComponent.HitEffectIndex, dataCount = excuteTurnData.ExcuteData.Count; i < dataCount; i++)
            {
                ++dataComponent.HitEffectIndex;

                var data = excuteTurnData.ExcuteData[i];
                if (data == null)
                    continue;

                var hc = data.HpChange;
                if (hc == null)
                    continue;

                if (hc.HitEffect < 3)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                MobEntity mob = MobManager.Instance.GetMob(hc.UnitId);
                if(mob == null)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                dataComponent.Target = mob;

                m_CurUseEntity.TranstionMultiStates(this, 1, (int)hc.HitEffect);
                return;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}