using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyLineComponent : BaseComponent<BaseTrackEntity>, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawPolyLine(_originV3, _targetPos);
    }
#endif

    public float m_SpeedX;
    public float m_Gx;

    public bool IsPauseTrack;

    private Transform _trans;
    private bool _isForward;
    private Vector3 _originV3;
    private float _time;
    private Vector3 _targetPos;
    private float _overStayTime;
    private bool _over;

    public override void Dispose()
    {
        _trans = null;
        IsPauseTrack = false;

        base.Dispose();
    }

    public void Init(float speedX, float gX, Transform trans, bool isForward, float overStayTime, Vector3 targetPos)
    {
        m_SpeedX = speedX;
        m_Gx = gX;

        _trans = trans;
        _isForward = isForward;
        _originV3 = _trans.position;
        _time = 0;
        _overStayTime = overStayTime;
        
        _targetPos = targetPos;
        _targetPos.y += 0.6f;
        _trans.LookAt(_targetPos);

        _over = false;
    }
    
    public void Update()
    {
        if (IsPauseTrack)
            return;

        if (_over)
        {
            _time += Time.deltaTime;
            if (_time > _overStayTime)
            {
                m_CurUseEntity.TrackOver();
            }

            return;
        }

        Vector3 forward = _trans.forward;
        Vector3 moveV3 = CombatHelp.CalLineFormula(m_SpeedX, m_Gx, _time, forward);
        _trans.position = _originV3 + moveV3;

        Vector3 targetDir = _targetPos - _trans.position;
        if (CombatHelp.SimulateDot(targetDir, forward) <= 0f && CombatHelp.SimulateDot(targetDir, targetDir) > 0.001f)
        {
            _trans.position = _targetPos;
            _time = 0f;
            _over = true;
            m_CurUseEntity.Hit();
            return;
        }

        _time += Time.deltaTime;
    }
}
