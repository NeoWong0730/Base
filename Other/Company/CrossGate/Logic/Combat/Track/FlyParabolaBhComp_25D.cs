using UnityEngine;
using System;

/// <summary>
/// xz平面专用, 
/// 抛物线水平方向为xz平面的正前方，垂直方向为世界坐标的Y坐标轴
/// </summary>
public class FlyParabolaBhComp_25D : BaseComponent<BaseTrackEntity>, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        int count = (int)(m_TotalTime / 0.01f);
        if (count > 10000)
            count = 10000;
        Vector3[] points = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            float disT = i * 0.01f;
            float forwardLen = (m_SpeedX + 0.5f * m_Gx * disT) * disT;
            float upLen = (m_SpeedY + 0.5f * m_Gy * disT) * disT;
            points[i] = new Vector3(m_OrginV3.x + forwardLen * _moveX, m_OrginV3.y + upLen, m_OrginV3.z + forwardLen * _moveZ);
        }
        UnityEditor.Handles.DrawPolyLine(points);
    }
#endif

    public float m_SpeedX;
    public float m_Gx;
    public float m_SpeedY;
    public float m_Gy;
    public float m_TotalTime;
    public float m_NeedBounceInterval;

    public Func<bool> m_EndConditionFunc;

    public Vector3 m_OrginV3;

    /// <summary>
    /// =0时间结束，=1低于开始轨迹时的世界Y值
    /// </summary>
    public int m_TrackOverType;

    public bool IsPauseTrack;

    private Transform _trans;

    private float _moveX;
    private float _moveZ;

    private float _time;
    
    //private float _curSpeedX;
    //private float _curSpeedY;
    
    public override void Dispose()
    {
        _trans = null;
        m_EndConditionFunc = null;
        m_TrackOverType = 0;
        IsPauseTrack = false;

        base.Dispose();
    }
    
    public void Init(Transform trans, float speedX, float gx, float speedY, float gy, float totalTime, float startTime = 0f)
    {
        _trans = trans;

        m_SpeedX = speedX;
        m_Gx = gx;
        m_SpeedY = speedY;
        m_Gy = gy;
        m_TotalTime = totalTime;

        m_OrginV3 = _trans.position;
        
        Vector3 f = _trans.forward;
        _moveX = f.x;
        _moveZ = f.z;

        _time = 0f;
    }

    public void Update()
    {
        if (IsPauseTrack)
            return;

        if (m_TrackOverType == 1 && _trans.position.y < m_OrginV3.y)
        {
            _trans.position = new Vector3(_trans.position.x, m_OrginV3.y, _trans.position.z);
            m_CurUseEntity.TrackOver();
            return;
        }
        else if (m_EndConditionFunc != null)
        {
            if (m_EndConditionFunc.Invoke())
            {
                m_CurUseEntity.TrackOver();
                return;
            }
        }
        else if (_time > m_TotalTime)
        {
            m_CurUseEntity.TrackOver();
            return;
        }
        
        float forwardLen = (m_SpeedX + 0.5f * m_Gx * _time) * _time;
        float upLen = (m_SpeedY + 0.5f * m_Gy * _time) * _time;
        _trans.position = new Vector3(m_OrginV3.x + forwardLen * _moveX, m_OrginV3.y + upLen, m_OrginV3.z + forwardLen * _moveZ);
        
        //_curSpeedX = m_SpeedX + m_Gx * _time;
        //_curSpeedY = m_SpeedY + m_Gy * _time;

        _time += Time.deltaTime;
    }
}
