using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BeProtectedToBack)]
public class WS_CombatBehaveAI_BeProtectedToBack_SComponent : StateBaseComponent, IUpdate
{
    private Vector3 _moveV3;
    private Vector3 _originV3;

    private float _lenSum;
    private float _speed;

    private float _moveLen;

    private MobEntity _beProtectedMob;

    public override void Dispose()
    {
        _beProtectedMob = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        _beProtectedMob = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, -1);
        if (_beProtectedMob != null && !string.IsNullOrWhiteSpace(str))
        {
            _moveV3 = (_beProtectedMob.m_Trans.position - mob.m_MobCombatComponent.m_OriginPos).normalized;
            if (_moveV3 != Vector3.zero)
            {
                _originV3 = _beProtectedMob.m_Trans.position;
                _beProtectedMob.m_Trans.forward = -_moveV3;
                float[] fs = CombatHelp.GetStrParseFloat1Array(str);
                _lenSum = fs[0];
                _speed = fs[1];
                _moveLen = 0f;
                return;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}

    public void Update()
    {
        _moveLen += _speed * Time.deltaTime;

        if (_moveLen >= _lenSum)
        {
            _beProtectedMob.m_Trans.position = _originV3 + _moveV3 * _lenSum;
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _beProtectedMob.m_Trans.position = _originV3 + _moveV3 * _moveLen;
    }
}