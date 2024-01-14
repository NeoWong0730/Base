using Table;
using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MoveToTargetPos)]
public class WS_CombatBehaveAI_MoveToTargetPos_SComponent : StateBaseComponent, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
    private float _moveSpeed;
    private Vector3 _moveV3;
    private Vector3 _moveNormalize;
    private Vector3 _originV3;
    private MobEntity _mob;

    private MobEntity _target;
    private MobEntity _destTarget;

    private float _len;

    private int _pType;

    private bool _isConverAttack;

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_originV3 + _moveV3, 0.5f);
    }
#endif

    public override void Dispose()
    {
        _mob = null;
        _target = null;
        _destTarget = null;

        _pType = 0;

        base.Dispose();
    }

    public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        if (cbace.m_SourcesTurnBehaveSkillInfo == null || cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        _isConverAttack = cbace.m_SourcesTurnBehaveSkillInfo.IsConverAttack;

        float[] fs = CombatHelp.GetStrParseFloat1Array(str);
        _moveSpeed = fs[0];

        if (fs.Length > 1)
            _pType = (int)fs[1];
        else
            _pType = 0;
        
        GetNeedComponent<WS_CombatBehaveAIDataComponent>().m_AttackMoveSpeed = _moveSpeed;
        _mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, _mob);

        uint targetUnitId = turnBehaveSkillTargetInfo == null ? 0u : turnBehaveSkillTargetInfo.TargetUnitId;

        uint pointType = 3u;
        if (_pType == 0 || pointType == 3u)
        {
            if (targetUnitId == 0u)
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }
            MobEntity targetMob = MobManager.Instance.GetMob(targetUnitId);
            if (targetMob == null)
            {
                bool isExistNewBattleUnit = Net_Combat.Instance.IsExistNewBattleUnit(targetUnitId);
                m_CurUseEntity.TranstionMultiStates(this, 1, isExistNewBattleUnit ? 0 : -1);
                return;
            }
            dataComponent.m_CurTarget = targetMob;
        }
        
        if (_pType == 0)
        {
            if (_isConverAttack)
            {
                pointType = 5u;
            }
            else if (dataComponent.m_AttackTargetIndex == 0)
            {
                pointType = 4u;
                //if (dataComponent.m_AttackTargetIndex + 1 < targetList.Count &&
                //Mathf.Abs(dataComponent.m_CurTarget.m_MobCombatComponent.m_ClientNum - targetList[dataComponent.m_AttackTargetIndex + 1].m_MobCombatComponent.m_ClientNum) == 1)
                //{
                //    pointType = 3u;
                //}
            }
        }
        else if (_pType == 7)
        {
            pointType = 7u;
        }

        if (turnBehaveSkillTargetInfo == null || turnBehaveSkillTargetInfo.BeProtectUnitId == 0u)
        {
            _target = dataComponent.m_CurTarget;
            _destTarget = _target;
        }
        else
        {
            _target = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, 1);
            _destTarget = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, -1);
        }

        if (cbace.m_SourcesTurnBehaveSkillInfo.ExtendType == 5u)
        {
            MoveOver();
            return;
        }

        SetMove(pointType, _destTarget, dataComponent);

        _len = 0f;
    }

    public void SetMove(uint pointType, MobEntity target, WS_CombatBehaveAIDataComponent dataComponent)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        
        CombatPosData combatPosData = null;
        if (pointType == 3u)
        {
            uint curClientNum = (uint)target.m_MobCombatComponent.m_ClientNum;
            TurnBehaveSkillTargetInfo preTbsti = dataComponent.GetTurnBehaveSkillTargetInfoByIndex(cbace, dataComponent.m_AttackTargetIndex - 1);
            if (preTbsti != null)
            {
                uint preTargetUnitId = preTbsti.BeProtectUnitId > 0u ? preTbsti.BeProtectUnitId : 
                    (preTbsti.TargetUnitId > 0u ? preTbsti.TargetUnitId : 0u);
                if (preTargetUnitId > 0u)
                {
                    MobEntity preTargetMob = MobManager.Instance.GetMob(preTargetUnitId);
                    if (preTargetMob != null && curClientNum == preTargetMob.m_MobCombatComponent.m_ClientNum)
                    {
                        MoveOver();
                        return;
                    }
                }
            }

            if (dataComponent.m_AttackTargetIndex > 1)
            {
                if ((int)dataComponent.m_SelectMoveToPointNum - 1 == (int)curClientNum ||
                    (int)dataComponent.m_SelectMoveToPointNum - 5 == (int)curClientNum ||
                    (int)dataComponent.m_SelectMoveToPointNum - 6 == (int)curClientNum ||
                    dataComponent.m_SelectMoveToPointNum == curClientNum)
                {
                    Vector3 disPos = _mob.m_Trans.position - dataComponent.m_SelectMoveToPointPos;
                    if (disPos.sqrMagnitude < 0.01f)
                    {
                        MoveOver();
                        return;
                    }
                }
            }

            uint pointNum = curClientNum;
            if (curClientNum % 5u == 0u)
                pointNum = curClientNum + 1u;
            else
                pointNum = curClientNum;

            dataComponent.m_SelectMoveToPointNum = pointNum;
            uint selectMoveToClientNum = CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + pointNum * 100u + 1u;
            combatPosData = CombatConfigManager.Instance.GetCombatPosData(selectMoveToClientNum);
        }
        else if (pointType == 4u)
        {
            CSVPositionPoint.Data pointTb = CSVPositionPoint.Instance.GetConfData(CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + (uint)target.m_MobCombatComponent.m_ClientNum * 100u + 1u);
            int pointNum = Random.Range(1, (int)pointTb.totol_point_number + 1);

            uint selectMoveToClientNum = CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + (uint)target.m_MobCombatComponent.m_ClientNum * 100u + (uint)pointNum;
            combatPosData = CombatConfigManager.Instance.GetCombatPosData(selectMoveToClientNum);
        }
        else if (pointType == 5u)
        {
            CSVPositionPoint.Data pointTb = CSVPositionPoint.Instance.GetConfData(CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + (uint)target.m_MobCombatComponent.m_ClientNum * 100u + 1u);
            int pointNum = 1;
            if (pointTb.totol_point_number > 1)
            {
                if (cbace.m_BehaveAIControllParam != null)
                {
                    NetExcuteTurnInfo excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnInfo(cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
                    int count = 0;
                    for (int i = 0; i < excuteTurnData.CombineAttack_SrcUnits.Count; i++)
                    {
                        if (excuteTurnData.CombineAttack_SrcUnits[i] == _mob.m_MobCombatComponent.m_BattleUnit.UnitId)
                        {
                            pointNum = i + 1 - count * (int)pointTb.totol_point_number;
                            int m = (i + 1) % (int)pointTb.totol_point_number;
                            if (m == 0)
                                count++;
                            break;
                        }
                    }
                }
            }

            uint selectMoveToClientNum;
            if (pointNum == 1)
                selectMoveToClientNum = pointTb.id;
            else
                selectMoveToClientNum = CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + (uint)target.m_MobCombatComponent.m_ClientNum * 100u + (uint)pointNum;

            combatPosData = CombatConfigManager.Instance.GetCombatPosData(selectMoveToClientNum);
        }
        else if (pointType == 7u)
        {
            uint selectMoveToClientNum = CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + (CombatHelp.GetClientCampSide(_mob.m_MobCombatComponent.m_ClientNum) == 0 ?  100u : 200u) + 1u;
            combatPosData = CombatConfigManager.Instance.GetCombatPosData(selectMoveToClientNum);
        }

        _originV3 = _mob.m_Trans.position;
        if (CombatManager.Instance.CombatSceneCenterPos.y != _originV3.y)
        {
            CombatManager.Instance.CombatSceneCenterPos.y = _originV3.y;
            _mob.m_MobCombatComponent.m_OriginPos.y = _originV3.y;
        }
        Vector3 targetPos = CombatManager.Instance.CombatSceneCenterPos;
        if (combatPosData != null)
        {
            if (CombatManager.Instance.PosFollowSceneCamera)
                targetPos += CombatManager.Instance.m_AdjustSceneViewAxiss[0] * combatPosData.PosX + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * combatPosData.PosZ;
            else
                targetPos += new Vector3(combatPosData.PosX, 0f, combatPosData.PosZ);
        }

        dataComponent.m_SelectMoveToPointPos = targetPos;

        _mob.m_Trans.LookAt(targetPos);

        _moveV3 = targetPos - _originV3;
        _moveNormalize = _moveV3.normalized;
        _len = 0f;
    }
    
    public void Update()
    {
        _len += Time.deltaTime * _moveSpeed;

        if (_len * _len < CombatHelp.SimulateDot(_moveV3, _moveV3))
        {
            _mob.m_Trans.position = _originV3 + _len * _moveNormalize;
        }
        else
        {
            _mob.m_Trans.position = _originV3 + _moveV3;

            MoveOver();
        }
    }

    private void MoveOver()
    {
        if (_pType == 0 && _target != null)
        {
            Transform targetTrans = _target.m_Trans;
            if (_target == _destTarget)
            {
                Vector3 lookTargetPos = targetTrans.position;
                lookTargetPos.y = _mob.m_Trans.position.y;
                _mob.m_Trans.LookAt(lookTargetPos);
            }
            if (!_isConverAttack)
            {
                targetTrans.LookAt(_mob.m_Trans.position);
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
    }
}