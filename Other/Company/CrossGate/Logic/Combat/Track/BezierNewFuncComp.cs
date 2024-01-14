using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierNewFuncComp : BezierCurvesFuncComponent, IUpdate
#if UNITY_EDITOR
    , IDrawGizmos
#endif
{
#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (m_BezierTrackData == null)
            return;

        for (int bpdIndxe = 0, bpdLen = m_BezierTrackData.m_BezierPosDatas.Length; bpdIndxe < bpdLen; bpdIndxe++)
        {
            #region 画bezier曲线
            Color oldColor = UnityEditor.Handles.color;
            if (bpdIndxe < bpdLen - 1)
            {
                var curBpd = m_BezierTrackData.m_BezierPosDatas[bpdIndxe];
                var nextBpd = m_BezierTrackData.m_BezierPosDatas[bpdIndxe + 1];
                Vector3 curPos;
                Vector3 curRightHelpPos = curBpd.RightHelpPos + curBpd.Pos;
                Vector3 nextPos;
                Vector3 nextLeftHelpPos = nextBpd.LeftHelpPos + nextBpd.Pos;
                if (m_IsNeedAxis)
                {
                    curPos = Axis_x * curBpd.Pos.x + Axis_y * curBpd.Pos.y + Axis_z * curBpd.Pos.z + m_OriginPos;
                    curRightHelpPos = Axis_x * curRightHelpPos.x + Axis_y * curRightHelpPos.y + Axis_z * curRightHelpPos.z + m_OriginPos;
                    nextPos = Axis_x * nextBpd.Pos.x + Axis_y * nextBpd.Pos.y + Axis_z * nextBpd.Pos.z + m_OriginPos;
                    nextLeftHelpPos = Axis_x * nextLeftHelpPos.x + Axis_y * nextLeftHelpPos.y + Axis_z * nextLeftHelpPos.z + m_OriginPos;
                }
                else
                {
                    curPos = curBpd.Pos + m_OriginPos;
                    curRightHelpPos += m_OriginPos;
                    nextPos = nextBpd.Pos + m_OriginPos;
                    nextLeftHelpPos += m_OriginPos;
                }
                
                float t = 1f / nextBpd.Segment;
                Vector3[] points = new Vector3[nextBpd.Segment + 1];
                float curverSumLen = 0f;
                for (int pIndex = 0, pCount = (int)nextBpd.Segment + 1; pIndex < pCount; pIndex++)
                {
                    var curV3 = BezierCurverController_3D.GetBezierCurverPos((int)nextBpd.Segment, pIndex, t,
                        curPos, curRightHelpPos, nextLeftHelpPos, nextPos);

                    //Gizmos.DrawWireSphere(v3, 0.05f);

                    points[pIndex] = curV3;

                    float preSumLen = curverSumLen;

                    Vector3 preV3;
                    if (pIndex > 0)
                    {
                        preV3 = points[pIndex - 1];
                        float len = (curV3 - preV3).magnitude;
                        if (len < 0)
                        {
                            Debug.LogError("bezier曲线有段<0");
                        }
                        curverSumLen += len;
                    }
                    else
                        preV3 = curV3;
                }
                nextBpd.BezierCurverLen = curverSumLen;
                UnityEditor.Handles.DrawPolyLine(points);
                UnityEditor.Handles.color = oldColor;
            }
            #endregion
        }
    }
#endif

    private Transform _target;

    /// <summary>
    /// =0正在运行，=1运行到终点，=2轨迹结束处理
    /// </summary>
    private int _overState;
    private float _funcTime;
    private float _overStayTime;
    private bool _isNeedForward;

    public void SetData(Transform target, uint trackId, bool isNeedForward, float overStayTime)
    {
        DLogManager.Log(Lib.Core.ELogType.eCombat, $"开始运行Bezier曲线trackId：{trackId.ToString()}");

        _target = target;

        SetBezierData(trackId);

        Init(target.position);

        _overState = 0;
        _funcTime = 0f;
        _overStayTime = overStayTime;
        _isNeedForward = isNeedForward;
    }

    public void Update()
    {
        if (_target == null || m_BezierTrackData == null)
        {
            m_CurUseEntity.TrackOver();
            return;
        }

        if (_overState == 1)
        {
            _funcTime += Time.deltaTime;
            if (_funcTime > _overStayTime)
            {
                _funcTime = 0f;
                _overState = 2;
            }

            return;
        }
        else if (_overState == 2)
        {
            if (_funcTime == 0f)
                _moveForward = _moveForward.normalized;

            _funcTime += Time.deltaTime;
            if (_funcTime > 3f)
                m_CurUseEntity.TrackOver();
            else
                _target.position += _moveSpeed * Time.deltaTime * _moveForward;

            return;
        }

        float x = 0;
        float y = 0;
        float z = 0;
        bool isCanForward = false;
        if (!DoBezierCurves(ref x, ref y, ref z, ref isCanForward))
        {
            if (_target != null)
            {
                Vector3 targetPos = new Vector3(x, y, z);
                if (isCanForward && _isNeedForward)
                    _target.LookAt(targetPos);

                _target.position = targetPos;
            }
        }
        else
        {
            m_CurUseEntity.Hit();
            _overState = 1;
            _funcTime = 0f;
        }
    }
}
