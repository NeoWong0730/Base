using Lib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class BulletEntity : BaseTrackEntity
{
    public GameObject m_BulletGo;
    public Transform m_BulletTrans;
    public CSVBullet.Data m_BulletTb;
    public CSVActiveSkill.Data m_SkillTb;

    public MobEntity m_Attach;
    public MobEntity m_Target;

    public ulong m_ModelId;

    public uint m_BulletWorkId;

    public int m_ExcuteTurnIndex;

    public Action<MobEntity, MobEntity, BulletEntity> HitAction;

    public TurnBehaveSkillInfo m_SourcesTurnBehaveSkillInfo;
    public BehaveAIControllParam m_BehaveAIControllParam;

    private int _isCanProcessTypeAfterHit;

    private int _bezierEventCount;

    private BulletBezierEvent _bulletBezierEvent;

    public void Init(ulong modelId, CSVActiveSkill.Data skillTb, GameObject bulletGo, CSVBullet.Data bulletTb, MobEntity attach, MobEntity target, 
        int targetClientNum, TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo, BehaveAIControllParam behaveAIControllParam, 
        Action<MobEntity, MobEntity, BulletEntity> hitAction = null, uint bulletWorkId = 0u, int isCanProcessTypeAfterHit = 0)
    {
        m_BulletGo = bulletGo;
        m_BulletTrans = bulletGo.transform;
        m_BulletTb = bulletTb;
        m_Attach = attach;
        m_Target = target;
        m_ModelId = modelId;
        m_SkillTb = skillTb;

        if (behaveAIControllParam == null)
            m_ExcuteTurnIndex = -1;
        else 
            m_ExcuteTurnIndex = behaveAIControllParam.ExcuteTurnIndex;

        HitAction = hitAction;
        m_SourcesTurnBehaveSkillInfo = sourcesTurnBehaveSkillInfo;
        m_BehaveAIControllParam = behaveAIControllParam;

        m_BulletWorkId = bulletWorkId;

        _isCanProcessTypeAfterHit = isCanProcessTypeAfterHit;

        if (m_BulletTb.track_type == 1)
        {
            uint bezierId = CombatManager.Instance.m_BattlePosType * 1000000u + bulletTb.bullet_change_type * 10000u + (target == null ? (uint)targetClientNum : (uint)target.m_MobCombatComponent.m_ClientNum) * 100u + (uint)UnityEngine.Random.Range(1, (int)m_BulletTb.parma2 + 1);

            BezierCurveComponent bcc = GetNeedComponent<BezierCurveComponent>();
            bcc.Init(bezierId, m_BulletTrans, m_BulletTb.is_forward == 1, m_BulletTb.parma3 * 0.001f);
        }
        else if (m_BulletTb.track_type == 2)
        {
            FlyLineComponent flc = GetNeedComponent<FlyLineComponent>();
            flc.Init(m_BulletTb.parma1 * 0.0001f, m_BulletTb.parma2 * 0.0001f, m_BulletTrans, m_BulletTb.is_forward == 1, m_BulletTb.parma3 * 0.001f, target == null ? MobManager.Instance.GetPosInClientNum(targetClientNum) : target.m_Trans.position);
        }
        else if (m_BulletTb.track_type == 3)
        {
            FlyParabolaBhComp_25D flyParabolaBhComp_25D = GetNeedComponent<FlyParabolaBhComp_25D>();
            //flyParabolaBhComp_25D.Init(m_BulletGo.transform)
        }
        else if (m_BulletTb.track_type == 4 || m_BulletTb.track_type == 5)
        {
            uint bezierId = CombatManager.Instance.m_BattlePosType * 1000000u + bulletTb.bullet_change_type * 10000u + (target == null ? (uint)targetClientNum : (uint)target.m_MobCombatComponent.m_ClientNum) * 100u + (uint)UnityEngine.Random.Range(1, (int)m_BulletTb.parma2 + 1);

            BezierNewFuncComp bnfc = GetNeedComponent<BezierNewFuncComp>();
            bnfc.SetData(m_BulletTrans, bezierId, m_BulletTb.is_forward == 1, m_BulletTb.parma3 * 0.001f);

            if (m_BulletTb.track_type == 5)
            {
                _bulletBezierEvent = BasePoolClass.Get<BulletBezierEvent>();
                _bulletBezierEvent.SetBulletEntity(this);
                bnfc.AddEvent(_bulletBezierEvent);
            }
        }
    }

    public override void Dispose()
    {
        m_BulletGo = null;
        m_BulletTrans = null;
        m_BulletTb = null;
        m_Attach = null;
        m_Target = null;
        m_ModelId = 0ul;
        HitAction = null;
        m_BulletWorkId = 0u;
        m_SkillTb = null;
        m_ExcuteTurnIndex = -1;
        m_SourcesTurnBehaveSkillInfo = null;
        if (m_BehaveAIControllParam != null)
        {
            m_BehaveAIControllParam.Push();
            m_BehaveAIControllParam = null;
        }

        _isCanProcessTypeAfterHit = 0;

        _bezierEventCount = 0;

        if (_bulletBezierEvent != null)
        {
            var bbe = _bulletBezierEvent;
            _bulletBezierEvent = null;

            bbe.Push();
        }

        base.Dispose();
    }

    public override void TrackOver()
    {
        if (!BulletManager.Instance.FreeBullet(m_ModelId))
            Dispose();
    }

    public override void Hit(bool isForce = false)
    {
        if (!isForce && m_BulletTb != null && m_BulletTb.track_type == 5)
            return;

        #region 飞弹击中目标
        if (HitAction == null)
        {
            BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
            if (m_Attach != null && m_Attach.m_MobCombatComponent != null && m_Attach.m_MobCombatComponent.m_BattleUnit != null)
                behaveAIControllParam.SrcUnitId = m_Attach.m_MobCombatComponent.m_BattleUnit.UnitId;
            behaveAIControllParam.SkillId = m_BehaveAIControllParam == null ? (m_SkillTb == null ? 0u : m_SkillTb.id) : m_BehaveAIControllParam.SkillId;
            if (m_Target != null && m_Target.m_MobCombatComponent != null && m_Target.m_MobCombatComponent.m_BattleUnit != null)
                behaveAIControllParam.TargetUnitId = m_Target.m_MobCombatComponent.m_BattleUnit.UnitId;
            if (m_BehaveAIControllParam != null)
            {
                behaveAIControllParam.TurnBehaveSkillTargetInfoIndex = m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex;
                behaveAIControllParam.ExcuteTurnIndex = m_BehaveAIControllParam.ExcuteTurnIndex;
            }
            
            if (m_BulletWorkId == 0u)
                m_Target?.m_MobCombatComponent.DoBehave(m_SkillTb, 1, m_Attach, m_SourcesTurnBehaveSkillInfo, behaveAIControllParam);
            else
                m_Target?.m_MobCombatComponent.DoBehave(m_BulletWorkId, m_SkillTb, 1, m_SourcesTurnBehaveSkillInfo, behaveAIControllParam);
        }
        else
            HitAction.Invoke(m_Attach, m_Target, this);

        WS_CombatBehaveAIControllerEntity.BulletHitToProcess(m_Attach, m_Target,
                        m_ExcuteTurnIndex, m_SourcesTurnBehaveSkillInfo,
                        m_BehaveAIControllParam == null ? -1 : m_BehaveAIControllParam.TurnBehaveSkillTargetInfoIndex,
                        _isCanProcessTypeAfterHit);
        #endregion

        if (m_Target != null)
        {
            var bcc = GetComponent<BezierCurveComponent>();
            if (bcc != null)
            {
                m_Target.m_Trans.LookAt(m_Target.m_Trans.position - bcc.m_MoveForward);
            }
            else if(m_BulletTb != null && m_BulletTb.track_type == 4)
            {
                if (m_Target.m_Trans != null && m_BulletTrans != null)
                    m_Target.m_Trans.LookAt(m_BulletTrans.position);
            }
        }
    }

    public void OnBezierEvent(int bezierIndex, int eventId, string eventFlagName)
    {
        if (m_BulletTb == null)
            return;

        ++_bezierEventCount;

        if (m_BulletTb.track_type == 5)
        {
            if (_bezierEventCount > 1)
            {
                DebugUtil.LogError($"飞弹Id:{m_BulletTb.id.ToString()}  track_type:{m_BulletTb.track_type.ToString()}生成的贝塞尔曲线有多个事件，该track_type类型只能配置一个事件");
                return;
            }

            Hit(true);
        }
    }
}

public class BulletBezierEvent : BasePoolClass, IBezierEvent
{
    private long _bulletId;
    private BulletEntity _bulletEntity;

    public override void Clear()
    {
        SetBulletEntity(null);
    }

    public void SetBulletEntity(BulletEntity bulletEntity)
    {
        if (bulletEntity == null || bulletEntity.Id == 0L)
        {
            _bulletId = 0L;
            _bulletEntity = null;
        }
        else
        {
            _bulletId = bulletEntity.Id;
            _bulletEntity = bulletEntity;
        }
    }

    public BulletEntity GetBulletEntity()
    {
        if (_bulletEntity == null)
        {
            return null;
        }
        else if (_bulletEntity.Id != _bulletId)
        {
            _bulletEntity = null;
            _bulletId = 0L;
            return null;
        }

        return _bulletEntity;
    }

    public void OnBezierEvent(int bezierIndex, int eventId, string eventFlagName)
    {
        BulletEntity bulletEntity = GetBulletEntity();
        if (bulletEntity == null)
            return;

        bulletEntity.OnBezierEvent(bezierIndex, eventId, eventFlagName);
    }
}
