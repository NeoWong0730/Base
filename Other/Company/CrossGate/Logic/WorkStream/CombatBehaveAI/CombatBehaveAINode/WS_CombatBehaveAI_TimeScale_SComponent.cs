using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.TimeScale)]
public class WS_CombatBehaveAI_TimeScale_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);

        if (fs[0] == 1 &&
            !((WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_CurHpChangeData.m_Death)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        if (dataComponent.m_TimeScale == 0f)
            dataComponent.m_TimeScale = 1f;

        float timeScale = fs[1];
        if (timeScale < dataComponent.m_TimeScale)
        {
            dataComponent.m_TimeScale = timeScale;

            m_CurUseEntity.GetNeedComponent<TimeScaleComponent>().Init(timeScale, fs[2]);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}

public class TimeScaleComponent : BaseComponent<StateMachineEntity>, IUpdate
{
    private float _totalTime;
    private float _time;

    public void Init(float timeScale, float totalTime)
    {
        Time.timeScale = timeScale * CombatManager.Instance.m_TimeScale;
        _totalTime = totalTime;
        _time = Time.time;
    }

    public override void Dispose()
    {
        Time.timeScale = CombatManager.Instance.m_TimeScale;

        base.Dispose();
    }

    public void Update()
    {
        if (Time.time > _time + _totalTime)
        {
            Dispose();
            return;
        }
    }
}