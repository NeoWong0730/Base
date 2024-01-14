using Lib.Core;
using System.Collections.Generic;
using UnityEngine;

public enum BezierCurverLoopEnum
{
    NoLoop = 0,
    Loop_PingPong = 1,
    Loop_ReStart = 2,
    Loop_ReEnd = 3,
}

public interface IBezierEvent
{
    void OnBezierEvent(int bezierIndex, int eventId, string eventFlagName);
}

[System.Serializable]
public class BezierCurvesFuncComponent : BaseComponent<BaseTrackEntity>
{
    public Bezier3CurvesData m_BezierTrackData;

    public List<Bezier1CurvesData> m_BezierRatioDataList;

    public List<IBezierEvent> m_BezierEventList;

    public Vector3 Axis_x;
    public Vector3 Axis_y;
    public Vector3 Axis_z;

#if UNITY_EDITOR
    public float BezierCurverRatio;
#endif

    public bool IsStop;

    public int m_CurBezierIndex;
    private int _pointIndex;
    private float _segmentDis;

    protected Vector3 _moveForward;

    private float _moveLen;
    private bool _refreshMoveLen;

    public Vector3 m_OriginPos;

    public bool m_IsNeedAxis;

    public BezierCurverLoopEnum m_LoopType;
    public uint m_MoveCount;
    public bool m_IsFront;

    [SerializeField]
    protected float _moveSpeed = 2f;
    private float _moveSumLen;

    //4位1为正向运动2为反向运动，1-3位的数为第几段Bezier
    private int _preMoveState;
    private float _oldRatio;
    private uint _oldMoveCount;
    private float _curCurverSumLen;

    private bool _isStartDo;

    public Dictionary<int, int> m_EventTimesDic = new Dictionary<int, int>();

    public override void Dispose()
    {
        Clear();

        base.Dispose();
    }

    public void Clear()
    {
        m_BezierTrackData = null;
        if (m_BezierRatioDataList != null)
            m_BezierRatioDataList.Clear();

        if (m_BezierEventList != null)
            m_BezierEventList.Clear();

        ResetMoveData();

        m_LoopType = 0;
        m_MoveCount = 1u;
    }

    public void Init(Vector3 originPos, BezierCurverLoopEnum loopType = BezierCurverLoopEnum.NoLoop, uint moveCount = 0)
    {
        m_LoopType = loopType;
        m_MoveCount = moveCount;

        if (m_BezierEventList != null)
            m_BezierEventList.Clear();

        ResetMoveData();

        SetAxis();

        m_OriginPos = originPos;
    }

    public bool DoBezierCurves(ref float x, ref float y, ref float z, ref bool isCanForward)
    {
        if (IsStop)
            return true;

        if (m_BezierTrackData == null || m_BezierTrackData.m_BezierPosDatas == null ||
            m_BezierTrackData.m_BezierPosDatas.Length == 0)
            return true;

        if (m_LoopType == BezierCurverLoopEnum.NoLoop)
        {
            if (m_CurBezierIndex >= m_BezierTrackData.m_BezierPosDatas.Length)
                return true;
        }
        else if (m_MoveCount == 0u)
        {
            if (m_CurBezierIndex >= m_BezierTrackData.m_BezierPosDatas.Length ||
                m_CurBezierIndex < 0)
                return true;
        }
        else if (m_LoopType == BezierCurverLoopEnum.Loop_PingPong)
        {
            if (m_CurBezierIndex >= m_BezierTrackData.m_BezierPosDatas.Length)
            {
                m_IsFront = false;
                --m_MoveCount;

                m_CurBezierIndex = m_BezierTrackData.m_BezierPosDatas.Length - 1;
            }
            else if (m_CurBezierIndex < 0)
            {
                m_IsFront = true;
                --m_MoveCount;

                m_CurBezierIndex = 0;
            }
        }
        else if (m_LoopType == BezierCurverLoopEnum.Loop_ReStart)
        {
            if (m_CurBezierIndex >= m_BezierTrackData.m_BezierPosDatas.Length ||
                m_CurBezierIndex < 0)
            {
                --m_MoveCount;

                m_CurBezierIndex = 0;
            }
            m_IsFront = true;
        }
        else if (m_LoopType == BezierCurverLoopEnum.Loop_ReEnd)
        {
            if (m_CurBezierIndex < 0)
            {
                --m_MoveCount;

                m_CurBezierIndex = m_BezierTrackData.m_BezierPosDatas.Length - 1;
            }

            m_IsFront = false;
        }

        Bezier3PosData curBpd = m_BezierTrackData.m_BezierPosDatas[m_CurBezierIndex];
        Bezier3PosData nextBpd;
        if (m_IsFront)
        {
            if (m_CurBezierIndex + 2 > m_BezierTrackData.m_BezierPosDatas.Length)
            {
                Vector3 curBpdPos;
                if (m_IsNeedAxis)
                {
                    curBpdPos = Axis_x * curBpd.Pos.x + Axis_y * curBpd.Pos.y + Axis_z * curBpd.Pos.z + m_OriginPos;
                }
                else
                    curBpdPos = curBpd.Pos + m_OriginPos;

                x = curBpdPos.x;
                y = curBpdPos.y;
                z = curBpdPos.z;

                ++m_CurBezierIndex;

                _moveSumLen = 0f;

                DoBezierEvent();

                return false;
            }

            nextBpd = m_BezierTrackData.m_BezierPosDatas[m_CurBezierIndex + 1];

            _curCurverSumLen = nextBpd.BezierCurverLen;
        }
        else
        {
            if (m_CurBezierIndex < 1)
            {
                Vector3 curBpdPos;
                if (m_IsNeedAxis)
                {
                    curBpdPos = Axis_x * curBpd.Pos.x + Axis_y * curBpd.Pos.y + Axis_z * curBpd.Pos.z + m_OriginPos;
                }
                else
                    curBpdPos = curBpd.Pos + m_OriginPos;

                x = curBpdPos.x;
                y = curBpdPos.y;
                z = curBpdPos.z;

                --m_CurBezierIndex;

                _moveSumLen = 0f;

                DoBezierEvent();

                return false;
            }

            nextBpd = m_BezierTrackData.m_BezierPosDatas[m_CurBezierIndex - 1];

            _curCurverSumLen = curBpd.BezierCurverLen;
        }

        uint segm = m_IsFront ? nextBpd.Segment : curBpd.Segment;
        if (_pointIndex >= segm)
        {
            _pointIndex = 0;
            _segmentDis = -1;

#if DEBUG_MODE
            if (_moveSumLen < _curCurverSumLen)
            {
                DebugUtil.LogError($"段落结束，总长度不太对，_moveSumLen:{_moveSumLen.ToString()}    _curCurverSumLen:{_curCurverSumLen.ToString()}");
            }
#endif

            if (m_IsFront)
            {
                ++m_CurBezierIndex;
                _moveSumLen = _moveSumLen > _curCurverSumLen ? _moveSumLen - _curCurverSumLen : 0f;
            }
            else
            {
                --m_CurBezierIndex;
                _moveSumLen = _moveSumLen > _curCurverSumLen ? _moveSumLen - _curCurverSumLen : 0f;
            }

            return DoBezierCurves(ref x, ref y, ref z, ref isCanForward); ;
        }
        else if (_segmentDis < 0)
        {
            _segmentDis = 1f / segm;
        }

        Vector3 curPos;
        Vector3 curHelpPos = (m_IsFront ? curBpd.RightHelpPos : curBpd.LeftHelpPos) + curBpd.Pos;
        Vector3 nextPos;
        Vector3 nextHelpPos = (m_IsFront ? nextBpd.LeftHelpPos : nextBpd.RightHelpPos) + nextBpd.Pos;
        if (m_IsNeedAxis)
        {
            curPos = Axis_x * curBpd.Pos.x + Axis_y * curBpd.Pos.y + Axis_z * curBpd.Pos.z + m_OriginPos;
            curHelpPos = Axis_x * curHelpPos.x + Axis_y * curHelpPos.y + Axis_z * curHelpPos.z + m_OriginPos;
            nextPos = Axis_x * nextBpd.Pos.x + Axis_y * nextBpd.Pos.y + Axis_z * nextBpd.Pos.z + m_OriginPos;
            nextHelpPos = Axis_x * nextHelpPos.x + Axis_y * nextHelpPos.y + Axis_z * nextHelpPos.z + m_OriginPos;
        }
        else
        {
            curPos = curBpd.Pos + m_OriginPos;
            curHelpPos += m_OriginPos;
            nextPos = nextBpd.Pos + m_OriginPos;
            nextHelpPos += m_OriginPos;
        }

        if (_isStartDo)
        {
            x = curPos.x;
            y = curPos.y;
            z = curPos.z;

            _isStartDo = false;

            DoBezierEvent();

            return false;
        }

        Vector3 point01 = CombatHelp.Calculate3BezierPoint_3D(_pointIndex * _segmentDis, curPos, curHelpPos, nextHelpPos, nextPos);
        Vector3 point02 = CombatHelp.Calculate3BezierPoint_3D((_pointIndex + 1) * _segmentDis, curPos, curHelpPos, nextHelpPos, nextPos);

        _moveForward = point02 - point01;
        if (_refreshMoveLen)
        {
            int bParam = m_BezierTrackData.m_BezierParams[m_IsFront ? m_CurBezierIndex : (m_CurBezierIndex - 1)];
            if (bParam < 0)
                _moveSpeed = -bParam;
            else
            {
                if (_curCurverSumLen == 0f)
                    _moveSpeed = 0f;
                else
                    _moveSpeed = GetValFrom1CurverData(m_IsFront ? (_moveSumLen / _curCurverSumLen) : (1 - (_moveSumLen / _curCurverSumLen)), m_IsFront);
            }

            float moveLen = _moveSpeed * Time.deltaTime;

            _moveSumLen += moveLen;

            _moveLen += moveLen;
        }
        else if (!isCanForward)
        {
            isCanForward = true;
        }

        if (_moveLen * _moveLen > Vector3.Dot(_moveForward, _moveForward))
        {
            float ppDis = _moveForward.magnitude;

            _moveLen -= ppDis;
            if (_moveLen < 0f)
            {
                DebugUtil.LogError($"BezierCurvesFuncComponent运动有问题减到小于0");

                _moveLen = 0f;
            }

            _refreshMoveLen = false;
            ++_pointIndex;

            return DoBezierCurves(ref x, ref y, ref z, ref isCanForward);
        }
        else
        {
            _refreshMoveLen = true;

            Vector3 goPos = point01 + _moveForward.normalized * _moveLen;
            x = goPos.x;
            y = goPos.y;
            z = goPos.z;

            DoBezierEvent();

            return false;
        }
    }

    private void ResetMoveData()
    {
        IsStop = false;

        if (m_LoopType == BezierCurverLoopEnum.NoLoop)
        {
            m_CurBezierIndex = 0;
        }
        else if (m_LoopType == BezierCurverLoopEnum.Loop_PingPong ||
                    m_LoopType == BezierCurverLoopEnum.Loop_ReStart ||
                    m_LoopType == BezierCurverLoopEnum.Loop_ReEnd)
        {
            m_CurBezierIndex = -1;
        }

        _pointIndex = 0;

        _moveLen = 0f;
        _refreshMoveLen = true;

        _segmentDis = -1;

        _moveSumLen = 0f;

        m_IsFront = true;

        _preMoveState = 0;
        _oldRatio = 0f;
        _oldMoveCount = 0u;

        _isStartDo = true;

        m_EventTimesDic.Clear();
    }

    public void SetAxis()
    {
        if (m_BezierTrackData == null)
            m_BezierTrackData = new Bezier3CurvesData();

        m_IsNeedAxis = false;

        Axis_x = Vector3.right;
        Axis_y = Vector3.up;
        Axis_z = Vector3.forward;

        if (m_BezierTrackData.EulerAngleX > 0.1f ||
            m_BezierTrackData.EulerAngleX < -0.1f)
        {
            m_IsNeedAxis = true;
        }
        else
            m_BezierTrackData.EulerAngleX = 0f;

        if (m_BezierTrackData.EulerAngleY > 0.1f ||
            m_BezierTrackData.EulerAngleY < -0.1f)
        {
            m_IsNeedAxis = true;
        }
        else
            m_BezierTrackData.EulerAngleY = 0f;

        if (m_BezierTrackData.EulerAngleZ > 0.1f ||
            m_BezierTrackData.EulerAngleZ < -0.1f)
        {
            m_IsNeedAxis = true;
        }
        else
            m_BezierTrackData.EulerAngleZ = 0f;

        if (m_IsNeedAxis)
        {
            Quaternion q = Quaternion.Euler(m_BezierTrackData.EulerAngleX, m_BezierTrackData.EulerAngleY, m_BezierTrackData.EulerAngleZ);
            Axis_x = q * Axis_x;
            Axis_y = q * Axis_y;
            Axis_z = q * Axis_z;
        }
    }

    public float GetValFrom1CurverData(float t, bool isFront)
    {
        if (m_BezierRatioDataList == null || m_BezierRatioDataList.Count == 0)
            return 0f;

        if (t > 1f)
        {
            DebugUtil.LogError($"BezierCurvesFuncComponent运动有问题 t：{t}");

            t = 1f;
        }
        else if (t < 0f)
        {
            DebugUtil.LogError($"BezierCurvesFuncComponent运动有问题 t：{t}");

            t = 0f;
        }

#if UNITY_EDITOR
        BezierCurverRatio = t;
#endif

        float val = 0f;
        Bezier1CurvesData bezier1CurvesData = m_BezierRatioDataList[isFront ? m_CurBezierIndex : (m_CurBezierIndex - 1)];
        if (bezier1CurvesData == null || bezier1CurvesData.Bezier1GroupPosDataArray == null)
        {
            return 10f;
        }

        for (int i = 0, length = bezier1CurvesData.Bezier1GroupPosDataArray.Length; i < length; i++)
        {
            Bezier1GroupPosData bpd = bezier1CurvesData.Bezier1GroupPosDataArray[i];
            if (t < bpd.PosRatioInTotal || (i == length - 1))
            {
                Bezier1GroupPosData preBpd = bezier1CurvesData.Bezier1GroupPosDataArray[i - 1];

                float ratioInSegm = bpd.PosRatioInTotal == preBpd.PosRatioInTotal ? 0f :
                                       ((t - preBpd.PosRatioInTotal) / (bpd.PosRatioInTotal - preBpd.PosRatioInTotal));
                float coordY = CombatHelp.Calculate3BezierPoint_1D(ratioInSegm, preBpd.Pos, preBpd.RightPos, bpd.LeftPos, bpd.Pos);

                val = bezier1CurvesData.Y_MaxVal - (bezier1CurvesData.Y_MaxVal - bezier1CurvesData.Y_MinVal) * (coordY - bezier1CurvesData.LeftTopY) / bezier1CurvesData.Hight;

                break;
            }
        }

        if (val < 0f)
        {
            DebugUtil.LogError($"获取的速度为负数：{val.ToString()}");
            val = 0f;
        }

        return val;
    }

    public void SetBezierData(uint bezierTrackId)
    {
        m_BezierTrackData = CombatConfigManager.Instance.GetBezier3CurvesData(bezierTrackId);
        if (m_BezierTrackData == null)
            return;

        if (m_BezierRatioDataList == null)
            m_BezierRatioDataList = new List<Bezier1CurvesData>();
        else
            m_BezierRatioDataList.Clear();

        if (m_BezierTrackData.m_BezierParams == null)
        {
            DebugUtil.LogError($"新的Bezier中SetBezierData的bezierTrackId：{bezierTrackId.ToString()}没有m_BezierParams数据");
            return;
        }

        for (int i = 0; i < m_BezierTrackData.m_BezierParams.Length; i++)
        {
            int paramId = m_BezierTrackData.m_BezierParams[i];
            if (paramId < 0)
            {
                m_BezierRatioDataList.Add(null);
                continue;
            }
            else if (paramId == 0)
            {
                DebugUtil.LogError($"bezierTrackId:{bezierTrackId.ToString()}   index:{i.ToString()}   获取RatioId为0，会导致给个默认速度10");
                m_BezierRatioDataList.Add(null);
                continue;
            }

            Bezier1CurvesData bezier1CurvesData = CombatConfigManager.Instance.GetBezier1CurvesData((uint)paramId);
            if (bezier1CurvesData == null)
            {
                DebugUtil.LogError($"bezierTrackId:{bezierTrackId.ToString()}   index:{i.ToString()}   数据读取RatioId：{paramId}为null，会导致给个默认速度10");
                m_BezierRatioDataList.Add(null);
                continue;
            }

            m_BezierRatioDataList.Add(bezier1CurvesData);
        }
    }

    public void AddEvent(IBezierEvent bezierEvent)
    {
        if (bezierEvent == null)
            return;

        if (m_BezierEventList == null)
            m_BezierEventList = new List<IBezierEvent>();

        for (int i = 0, count = m_BezierEventList.Count; i < count; i++)
        {
            if (m_BezierEventList[i] == bezierEvent)
            {
                DebugUtil.LogError($"IBezierEvent事件加入已经存在，不要重复加入");
                return;
            }
        }

        m_BezierEventList.Add(bezierEvent);
    }

    public void RemoveEvent(IBezierEvent bezierEvent)
    {
        m_BezierEventList.Remove(bezierEvent);
    }

    private void DoBezierEvent()
    {
        if (m_BezierEventList == null || m_BezierEventList.Count <= 0)
        {
            float cRatio = _curCurverSumLen == 0f ? 0f : CombatHelp.GetFloat3Decimal(_moveSumLen / _curCurverSumLen);
            if (cRatio > 1f)
            {
                DebugUtil.LogError($"获取的移动总长度比当前曲线总长度要长：{cRatio.ToString()}");
                cRatio = 1f;
            }

            if (m_IsFront)
            {
                _preMoveState = 1000 + m_CurBezierIndex;
            }
            else
            {
                _preMoveState = 2000 + m_CurBezierIndex;
            }

            _oldRatio = cRatio;
            _oldMoveCount = m_MoveCount;

            return;
        }

        int oldFront;
        int oldCurverIndex;
        if (_preMoveState != 0)
        {
            oldFront = _preMoveState / 1000;
            oldCurverIndex = _preMoveState - oldFront * 1000;
        }
        else
        {
            _oldMoveCount = m_MoveCount;
            oldFront = 0;
            if (m_IsFront)
            {
                oldCurverIndex = 0;
                _oldRatio = -0.1f;
            }
            else
            {
                oldCurverIndex = m_BezierTrackData.m_BezierPosDatas.Length - 1;
                _oldRatio = 1.1f;
            }
        }

        float curRatio = _curCurverSumLen == 0f ? 0f : CombatHelp.GetFloat3Decimal(_moveSumLen / _curCurverSumLen);
        if (curRatio > 1f)
        {
            DebugUtil.LogError($"获取的移动总长度比当前曲线总长度要长：{curRatio.ToString()}");
            curRatio = 1f;
        }

        if (m_IsFront)
        {
            _preMoveState = 1000 + m_CurBezierIndex;

            if (oldFront == 2)
            {
                for (int i = oldCurverIndex; i > 0; --i)
                {
                    DoEvent(i, oldCurverIndex, _oldRatio, curRatio, false, 1);
                }

                for (int i = 1; i < m_CurBezierIndex + 2; i++)
                {
                    if (i >= m_BezierTrackData.m_BezierPosDatas.Length)
                        break;

                    DoEvent(i, oldCurverIndex, _oldRatio, curRatio, true, 2);
                }
            }
            else
            {
                if (_oldMoveCount != m_MoveCount)
                {
                    for (int i = oldCurverIndex + 1; i < m_BezierTrackData.m_BezierPosDatas.Length; i++)
                    {
                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, true, 3);
                    }

                    for (int i = 1; i < m_CurBezierIndex + 2; i++)
                    {
                        if (i >= m_BezierTrackData.m_BezierPosDatas.Length)
                            break;

                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, true, 4);
                    }
                }
                else
                {
                    for (int i = oldCurverIndex + 1; i < m_CurBezierIndex + 2; i++)
                    {
                        if (i >= m_BezierTrackData.m_BezierPosDatas.Length)
                            break;

                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, true, 5);
                    }
                }
            }
        }
        else
        {
            curRatio = 1 - curRatio;

            _preMoveState = 2000 + m_CurBezierIndex;

            if (oldFront == 1)
            {
                for (int i = oldCurverIndex + 1; i < m_BezierTrackData.m_BezierPosDatas.Length; i++)
                {
                    DoEvent(i, oldCurverIndex, _oldRatio, curRatio, true, 6);
                }

                for (int i = m_BezierTrackData.m_BezierPosDatas.Length - 1; i >= m_CurBezierIndex; --i)
                {
                    if (i < 1)
                        break;

                    DoEvent(i, oldCurverIndex, _oldRatio, curRatio, false, 7);
                }
            }
            else
            {
                if (_oldMoveCount != m_MoveCount)
                {
                    for (int i = oldCurverIndex; i > 0; --i)
                    {
                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, false, 8);
                    }

                    for (int i = m_BezierTrackData.m_BezierPosDatas.Length - 1; i >= m_CurBezierIndex; --i)
                    {
                        if (i < 1)
                            break;

                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, false, 9);
                    }
                }
                else
                {
                    for (int i = oldCurverIndex; i >= m_CurBezierIndex; --i)
                    {
                        if (i < 1)
                            break;

                        DoEvent(i, oldCurverIndex, _oldRatio, curRatio, false, 10);
                    }
                }
            }
        }

        _oldRatio = curRatio;
        _oldMoveCount = m_MoveCount;
    }

    private void DoEvent(int posDataIndex, int oldCurverIndex, float oldRatio, float curRatio, bool isFront, int type)
    {
        Bezier3PosData bezier3PosData = m_BezierTrackData.m_BezierPosDatas[posDataIndex];
        if (bezier3PosData.EventInfoArray != null && bezier3PosData.EventInfoArray.Length > 0)
        {
            int eventCount = bezier3PosData.EventInfoArray.Length;
            int eventIndex = isFront ? 0 : (eventCount - 1);
            while ((isFront && eventIndex < eventCount) ||
                (!isFront && eventIndex > -1))
            {
                BezierCurverEvent bezierCurverEvent = bezier3PosData.EventInfoArray[eventIndex];

                //Log.Error($"oldRatio:{oldRatio} oldCurverIndex:{oldCurverIndex} posDataIndex:{posDataIndex} curRatio:{curRatio}==={eventIndex}");

                if (isFront)
                    ++eventIndex;
                else
                    --eventIndex;

                if (m_EventTimesDic.TryGetValue(bezierCurverEvent.EventId, out int curTimes))
                {
                    if (curTimes >= bezierCurverEvent.EventCount)
                        continue;
                }

                if (type == 1)
                {
                    if (posDataIndex == oldCurverIndex &&
                        oldRatio <= bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 2)
                {
                    if ((posDataIndex == m_CurBezierIndex + 1) &&
                        curRatio < bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 3)
                {
                    if ((posDataIndex == oldCurverIndex + 1) &&
                        oldRatio >= bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 4)
                {
                    if ((posDataIndex == m_CurBezierIndex + 1) &&
                        curRatio < bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 5)
                {
                    if ((posDataIndex == oldCurverIndex + 1) &&
                        oldRatio >= bezierCurverEvent.EventRatio)
                        continue;

                    if ((posDataIndex == m_CurBezierIndex + 1) &&
                        curRatio < bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 6)
                {
                    if ((posDataIndex == oldCurverIndex + 1) &&
                        oldRatio >= bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 7)
                {
                    if ((posDataIndex == m_CurBezierIndex) &&
                        curRatio > bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 8)
                {
                    if ((posDataIndex == oldCurverIndex) &&
                        oldRatio <= bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 9)
                {
                    if ((posDataIndex == m_CurBezierIndex) &&
                        curRatio > bezierCurverEvent.EventRatio)
                        continue;
                }
                else if (type == 10)
                {
                    if ((posDataIndex == oldCurverIndex) &&
                        oldRatio <= bezierCurverEvent.EventRatio)
                        continue;

                    if ((posDataIndex == m_CurBezierIndex) &&
                        curRatio > bezierCurverEvent.EventRatio)
                        continue;
                }

                for (int registerIndex = 0, registerCount = m_BezierEventList.Count; registerIndex < registerCount; registerIndex++)
                {
                    var registerEvent = m_BezierEventList[registerIndex];
                    if (registerEvent == null)
                    {
                        DebugUtil.LogError($"执行Bezier事件时{registerIndex.ToString()}出现m_BezierEventList有null元素");
                        continue;
                    }
                    registerEvent.OnBezierEvent(posDataIndex - 1, bezierCurverEvent.EventId, bezierCurverEvent.EventFlagName);
                }

                ++curTimes;
                m_EventTimesDic[bezierCurverEvent.EventId] = curTimes;
            }
        }
    }
}
