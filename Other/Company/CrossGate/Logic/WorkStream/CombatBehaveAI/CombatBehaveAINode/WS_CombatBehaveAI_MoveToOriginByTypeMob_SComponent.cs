﻿using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MoveToOriginByTypeMob)]
public class WS_CombatBehaveAI_MoveToOriginByTypeMob_SComponent : StateBaseComponent, IUpdate
{
    private float _moveSpeed;
    private Vector3 _moveV3;
    private Vector3 _moveNormalize;
    private Vector3 _originV3;
    private MobEntity _mob;

    private float _len;

    public override void Dispose()
    {
        _mob = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);

        _mob = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, (int)fs[0]);
        if (_mob == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _moveSpeed = fs[1];
        _originV3 = _mob.m_Trans.position;

        if (CombatHelp.IsAllowDistanceFor2Point(_originV3.x, _mob.m_MobCombatComponent.m_OriginPos.x) &&
            CombatHelp.IsAllowDistanceFor2Point(_originV3.z, _mob.m_MobCombatComponent.m_OriginPos.z))
        {
            _mob.m_MobCombatComponent.ResetTrans(true);
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        else
            _mob.m_Trans.LookAt(_mob.m_MobCombatComponent.m_OriginPos);

        _moveV3 = _mob.m_MobCombatComponent.m_OriginPos - _originV3;
        _moveV3.y = 0f;
        _moveNormalize = _moveV3.normalized;

        _len = 0f;
    }

    public void Update()
    {
        _len += Time.deltaTime * _moveSpeed;

        if (_len * _len < CombatHelp.SimulateDot(_moveV3, _moveV3) && _moveSpeed > 0f)
        {
            _mob.m_Trans.position = _originV3 + _len * _moveNormalize;
        }
        else
        {
            _mob.m_MobCombatComponent.ResetTrans(false, true);

            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}