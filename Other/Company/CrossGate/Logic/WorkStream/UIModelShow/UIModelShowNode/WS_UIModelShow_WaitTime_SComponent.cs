using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.WaitTime)]
public class WS_UIModelShow_WaitTime_SComponent : StateBaseComponent, IUpdate
{
    private float _endTime;

    public override void Init(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        float time = float.Parse(str);
        if (time <= 0f)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _endTime = time + Time.time;
    }

    public void Update()
    {
        if (Time.time < _endTime)
            return;

        m_CurUseEntity.TranstionMultiStates(this);
    }
}