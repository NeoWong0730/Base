using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MobShake)]
public class WS_CombatBehaveAI_MobShake_SComponent : StateBaseComponent, IUpdate
{
    public float m_Speed;
    public float m_Range;
    public float m_TotalTime;
    public Vector3 _originV3;

    private float _time;

    private bool _isObstruction;

    public bool _isForward;

    public override void Init(string str)
	{
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        m_Speed = fs[0];
        m_Range = fs[1];
        m_TotalTime = fs[2];
        _isObstruction = (fs.Length < 4 || fs[3] == 0) ? true : false;
        _isForward = (fs.Length < 5 || fs[4] == 0) ? true : false;
        _time = 0f;

        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        Transform trans = mob.m_Trans;

        _originV3 = trans.position;

        if (!_isObstruction)
        {
            m_CurUseEntity.GetNeedComponent<MobShakeComponent>().Init(trans, this);
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }

    public void Update()
    {
        if (!_isObstruction)
            return;

        MobEntity mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        if (_time > m_TotalTime)
        {
            mobEntity.m_Trans.position = _originV3;
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;

        mobEntity.m_Trans.position = _originV3 + Mathf.Cos(_time * m_Speed) * m_Range * (_isForward ? mobEntity.m_Trans.forward : mobEntity.m_Trans.up);
    }
}

public class MobShakeComponent : BaseComponent<StateMachineEntity>, IUpdate
{
    private float _speed;
    private float _range;
    private float _totalTime;
    private Vector3 _originV3;

    private float _time;

    private bool _isForward;

    private Transform _trans;
    
    public void Init(Transform trans, WS_CombatBehaveAI_MobShake_SComponent mobShake_SComponent)
    {
        _trans = trans;
        _speed = mobShake_SComponent.m_Speed;
        _range = mobShake_SComponent.m_Range;
        _totalTime = mobShake_SComponent.m_TotalTime;
        _originV3 = mobShake_SComponent._originV3;
        _isForward = mobShake_SComponent._isForward;

        _time = 0f;
    }

    public void Update()
    {
        if (_time > _totalTime)
        {
            _trans.position = _originV3;
            Dispose();
            return;
        }

        _time += Time.deltaTime;

        _trans.position = _originV3 + Mathf.Cos(_time * _speed) * _range * (_isForward ? _trans.forward : _trans.up);
    }
}