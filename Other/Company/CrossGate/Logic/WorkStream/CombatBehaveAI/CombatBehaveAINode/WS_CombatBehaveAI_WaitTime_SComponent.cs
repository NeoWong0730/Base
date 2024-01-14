using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.WaitTime)]
public class WS_CombatBehaveAI_WaitTime_SComponent : StateBaseComponent, IUpdate
{
    private float _endTime;

    public override void Init(string str)
	{
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _endTime = float.Parse(str) + Time.time;
    }

    public void Update()
    {
        if (Time.time < _endTime)
            return;

        m_CurUseEntity.TranstionMultiStates(this);
    }
}