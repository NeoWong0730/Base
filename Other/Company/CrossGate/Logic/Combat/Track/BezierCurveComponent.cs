using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveComponent : BaseComponent<BaseTrackEntity>, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
    public Vector3 m_MoveForward;

    private Transform _trans;
    private Vector3 _originPos;
    private bool _isForward;

    private BezierData _bezierData;

    private int _bezierIndex;
    private float _time;
    private int _pointIndex;
    private float _moveLen;
    private bool _refreshMoveLen;
    private float _overStayTime;
    
    /// <summary>
    /// =0正在运行，=1运行到终点，=2轨迹结束处理
    /// </summary>
    private int _overState;
    
    public void Init(uint bezierId, Transform trans, bool isForward, float overStayTime)
    {
        _trans = trans;
        _isForward = isForward;
        _originPos = trans.position;
        _overStayTime = overStayTime;

        _bezierData = CombatConfigManager.Instance.GetBezierData(bezierId);

        _bezierIndex = 0;
        _time = 0f;
        _pointIndex = 0;
        _refreshMoveLen = true;
        _overState = 0;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        for (int i = 0; i < _bezierData.BezierPosDatas.Length - 1; i++)
        {
            var curBpc = _bezierData.BezierPosDatas[i];
            var nextBpc = _bezierData.BezierPosDatas[i + 1];
            if (CombatManager.Instance.PosFollowSceneCamera)
            {
                Vector3 curPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * curBpc.Pos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * curBpc.Pos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * curBpc.Pos.z;
                Vector3 curRightHelpPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * curBpc.RightHelpPos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * curBpc.RightHelpPos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * curBpc.RightHelpPos.z;
                Vector3 nextPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * nextBpc.Pos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * nextBpc.Pos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * nextBpc.Pos.z;
                Vector3 nextLeftHelpPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * nextBpc.LeftHelpPos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * nextBpc.LeftHelpPos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * nextBpc.LeftHelpPos.z;
                UnityEditor.Handles.DrawBezier(curPos + _originPos, nextPos + _originPos, curRightHelpPos + _originPos, nextLeftHelpPos + _originPos, new Color(0.8f, 0.1f, 0.1f), null, 5f);

                Gizmos.DrawWireSphere(curPos + _originPos, 0.5f);
                Gizmos.DrawWireSphere(nextPos + _originPos, 0.5f);
            }
            else
            {
                UnityEditor.Handles.DrawBezier(curBpc.Pos + _originPos, nextBpc.Pos + _originPos, curBpc.RightHelpPos + _originPos, nextBpc.LeftHelpPos + _originPos, new Color(0.8f, 0.1f, 0.1f), null, 5f);

                Gizmos.DrawWireSphere(curBpc.Pos + _originPos, 0.5f);
                Gizmos.DrawWireSphere(nextBpc.Pos + _originPos, 0.5f);
            }
        }
    }
#endif

    public void Update()
    {
        if (_trans == null || _bezierData == null || _bezierData.BezierPosDatas == null || _bezierData.Speed <= 0f)
        {
            m_CurUseEntity.TrackOver();
            return;
        }

        if (_overState == 1)
        {
            _time += Time.deltaTime;
            if (_time > _overStayTime)
            {
                _time = 0f;
                _overState = 2;
            }

            return;
        }
        else if (_overState == 2)
        {
            if (_time == 0f)
                m_MoveForward = m_MoveForward.normalized;

            _time += Time.deltaTime;
            if (_time > 3f)
                m_CurUseEntity.TrackOver();
            else
                _trans.position += _bezierData.Speed * Time.deltaTime * m_MoveForward;

            return;
        }

        if (_pointIndex >= 20)
        {
            _pointIndex = 0;
            _bezierIndex++;
        }

        if (_bezierIndex > _bezierData.BezierPosDatas.Length - 2)
        {
            var bezierPosData = _bezierData.BezierPosDatas[_bezierIndex];
            if (CombatManager.Instance.PosFollowSceneCamera)
            {
                _trans.position = _originPos + CombatManager.Instance.m_AdjustSceneViewAxiss[0] * bezierPosData.Pos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * bezierPosData.Pos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * bezierPosData.Pos.z;
            }
            else
            {
                _trans.position = bezierPosData.Pos + _originPos;
            }
            
            m_CurUseEntity.Hit();
            _overState = 1;
            _time = 0f;
            return;
        }

        var curBpc = _bezierData.BezierPosDatas[_bezierIndex];
        var nextBpc = _bezierData.BezierPosDatas[_bezierIndex + 1];
        Vector3 point01 = Vector3.zero;
        Vector3 point02 = Vector3.zero;
        if (CombatManager.Instance.PosFollowSceneCamera)
        {
            Vector3 curPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * curBpc.Pos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * curBpc.Pos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * curBpc.Pos.z;
            Vector3 curRightHelpPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * curBpc.RightHelpPos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * curBpc.RightHelpPos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * curBpc.RightHelpPos.z;
            Vector3 nextPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * nextBpc.Pos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * nextBpc.Pos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * nextBpc.Pos.z;
            Vector3 nextLeftHelpPos = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * nextBpc.LeftHelpPos.x + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * nextBpc.LeftHelpPos.y + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * nextBpc.LeftHelpPos.z;
            point01 = BezierToolHelper.Calculate3BezierPoint_3D(_pointIndex * 0.05f, curPos + _originPos, curRightHelpPos + _originPos, nextLeftHelpPos + _originPos, nextPos + _originPos);
            point02 = BezierToolHelper.Calculate3BezierPoint_3D((_pointIndex + 1) * 0.05f, curPos + _originPos, curRightHelpPos + _originPos, nextLeftHelpPos + _originPos, nextPos + _originPos);
        }
        else
        {
            point01 = BezierToolHelper.Calculate3BezierPoint_3D(_pointIndex * 0.05f, curBpc.Pos + _originPos, curBpc.RightHelpPos + _originPos, nextBpc.LeftHelpPos + _originPos, nextBpc.Pos + _originPos);
            point02 = BezierToolHelper.Calculate3BezierPoint_3D((_pointIndex + 1) * 0.05f, curBpc.Pos + _originPos, curBpc.RightHelpPos + _originPos, nextBpc.LeftHelpPos + _originPos, nextBpc.Pos + _originPos);
        }
        
        m_MoveForward = point02 - point01;
        if (_refreshMoveLen)
        {
            _time += Time.deltaTime;
            _moveLen = _bezierData.Speed * _time;
        }

        if (_moveLen * _moveLen > CombatHelp.SimulateDot(m_MoveForward, m_MoveForward))
        {
            float ppDis = m_MoveForward.magnitude;

            _moveLen -= ppDis;
            _time = 0f;

            _refreshMoveLen = false;
            _pointIndex++;

            Update();
            return;
        }
        else
        {
            _refreshMoveLen = true;
            _trans.position = point01 + m_MoveForward.normalized * _moveLen;

            if (_isForward)
                _trans.LookAt(point02);

            return;
        }
    }
}
