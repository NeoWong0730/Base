using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ModelScale)]
public class WS_CombatBehaveAI_ModelScale_SComponent : StateBaseComponent, IUpdate
{
    private float _startScaleFac;
    private float _endScaleFac;
    private float _totalTime;
    private float _speed;

    private float _time;

    public override void Init(string str)
	{
        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        dataComponent.m_IsAdustLocalScale = true;

        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        _startScaleFac = fs[0];
        _endScaleFac = fs[1];
        _totalTime = fs[2];

        MobEntity mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        if (_startScaleFac == _endScaleFac || _totalTime == 0f)
        {
            mobEntity.m_Trans.localScale = dataComponent.m_OrginLocalScale * _endScaleFac;
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        mobEntity.m_Trans.localScale = dataComponent.m_OrginLocalScale * _startScaleFac;

        _speed = (_endScaleFac - _startScaleFac) / _totalTime;

        _time = 0f;
    }

    public void Update()
    {
        MobEntity mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        if (_time >= _totalTime)
        {
            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            if (dataComponent != null)
                mobEntity.m_Trans.localScale = dataComponent.m_OrginLocalScale * _endScaleFac;
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;

        float scale = _speed * Time.deltaTime;
        mobEntity.m_Trans.localScale += new Vector3(scale, scale, scale);
    }
}