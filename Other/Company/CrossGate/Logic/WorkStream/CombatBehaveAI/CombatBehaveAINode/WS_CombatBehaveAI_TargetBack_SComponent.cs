using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.TargetBack)]
public class WS_CombatBehaveAI_TargetBack_SComponent : StateBaseComponent, IUpdate
{
    private float _speed;
    private float _totalTime;
    private float _time;
    private float _gx;

    private Vector3 _originV3;

    public override void Init(string str)
	{
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        _speed = fs[0];
        _totalTime = fs[1];
        if (fs.Length > 2)
            _gx = fs[2];
        else
            _gx = 0f;

        _originV3 = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_Trans.position;

        _time = 0f;
    }

    public void Update()
    {
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        if (_time > _totalTime)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _time += Time.deltaTime;

        if (_gx == 0f)
            mob.m_Trans.position = _originV3 - _speed * _time * mob.m_Trans.forward;
        else
            mob.m_Trans.position -= CombatHelp.CalLineFormula02(_speed, _gx, _time, Time.deltaTime, mob.m_Trans.forward);
    }
}