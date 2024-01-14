using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MoveToPosType)]
public class WS_CombatBehaveAI_MoveToPosType_SComponent : StateBaseComponent, IUpdate
{
    private MobEntity _mobEntity;
    private float _speed;

    private Vector3 _pos;
    private Vector3 _moveV3;
    private Vector3 _moveNormalize;
    private Vector3 _originV3;

    private float _len;

    public override void Dispose()
    {
        _mobEntity = null;

        base.Dispose();
    }

    public override void Init(string str)
	{
        _mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        
        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        _speed = fs[0];
        _pos = CombatManager.Instance.CombatSceneCenterPos;
        if (fs.Length > 1)
        {
            int posType = (int)fs[1];
            if (posType > 0)
            {
                uint cellNum = (uint)_mobEntity.m_MobCombatComponent.m_ClientNum;
                if (posType == 7 || posType == 8)
                {
                    cellNum = CombatHelp.GetClientCampSide(_mobEntity.m_MobCombatComponent.m_ClientNum) == 0 ? 1u : 2u;
                }
                uint pointNum = 1u;
                if (fs.Length > 2)
                    pointNum = (uint)fs[2];
                uint clientNum = CombatManager.Instance.m_BattlePosType * 1000000 + (uint)posType * 10000u + cellNum * 100u + pointNum;
                CombatPosData combatPosData = CombatConfigManager.Instance.GetCombatPosData(clientNum);
                if (combatPosData != null)
                {
                    _pos.x += combatPosData.PosX;
                    _pos.z += combatPosData.PosZ;
                }
            }
            else 
            {
                MobEntity typeMob = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, posType);
                if (typeMob == null)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                _pos = typeMob.m_MobCombatComponent.m_OriginPos;
            }
        }

        _mobEntity.m_Trans.LookAt(_pos);

        _originV3 = _mobEntity.m_Trans.position;
        _moveV3 = _pos - _originV3;
        _moveNormalize = _moveV3.normalized;
        _len = 0f;
    }

    public void Update()
    {
        _len += Time.deltaTime * _speed;

        if (_len * _len < CombatHelp.SimulateDot(_moveV3, _moveV3))
        {
            _mobEntity.m_Trans.position = _originV3 + _len * _moveNormalize;
        }
        else
        {
            _mobEntity.m_Trans.position = _pos;

            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}