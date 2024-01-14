using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ScreenBlackTransition)]
public class WS_CombatBehaveAI_ScreenBlackTransition_SComponent : StateBaseComponent, IUpdate
{
    private float[] fs;
    private float _time;

    public override void Dispose()
    {
        fs = null;

        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnUIBlackSrceen, false, 0);

        base.Dispose();
    }

    public override void Init(string str)
	{
        fs = CombatHelp.GetStrParseFloat1Array(str);

        _time = 0f;
    }

    public void Update()
    {
        if (_time > (fs[0] + fs[1] + fs[2]))
        {
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnUIBlackSrceen, false, 0);

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        int alpha = 0;
        if (_time < fs[0])
        {
            alpha = (int)(_time / fs[0] * 100f);
        }
        else if (_time < (fs[0] + fs[1]))
        {
            alpha = 100;
        }
        else if (_time < (fs[0] + fs[1] + fs[2]))
        {
            alpha = (int)((fs[0] + fs[1] + fs[2] - _time) / fs[2] * 100f);
        }

        Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnUIBlackSrceen, true, alpha);

        _time += Time.deltaTime;
    }
}