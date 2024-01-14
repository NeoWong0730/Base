#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BezierTransInfo_e
{
    public uint Segment_e = 20;
    public Transform m_LeftHelpTrans;
    public Transform m_Trans;
    public Transform m_RightHelpTrans;
    public Color m_Color;

    public bool m_IsNeedUseColor_e;

    public List<int> m_CombineIndexList;

    public BezierRatioInfo_e m_BezierRatioInfo_e;

    public int m_SelectBezierEventIndex;
    public List<BezierCurverEvent_e> m_bezierCuverEvent_e_List = new List<BezierCurverEvent_e>();
}

[System.Serializable]
public class BezierRatioInfo_e
{
    //原始矩形范围
    public float x;
    public float y;
    public float width;
    public float height;
    public float scale;

    public float StartX;
    public float StartY;

    public float Y_MinVal;
    public float Y_MaxVal;

    //一个矩形框中包含的所有点列表
    public List<BezierRatioPointInfo_e> m_BezierRatioPointInfoList_e = new List<BezierRatioPointInfo_e>();
}
[System.Serializable]
public class BezierRatioPointInfo_e
{
    public float OriginX;
    public float OriginY;

    public float ShowX;
    public float ShowY;

    public float Width;
    public float Height;

    public float Y_Val;
}

[System.Serializable]
public class BezierCurverEvent_e
{
    public BezierCurverEvent EventInfo;
    public Transform EventTrans;
}

public enum BalanceHelpEnum
{
    None = 0,
    Direction = 1,
    Direction_And_Distance = 2,
}

public class BezierCurverController_3D : MonoBehaviour, IBezierEvent
{
    public bool m_IsDrawHelpLine = true;
    private void OnDrawGizmos()
    {
        if (m_BezierCurvesFuncComponent == null || m_BezierCurvesFuncComponent.m_BezierTrackData == null ||
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas == null ||
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length == 0)
            return;

        for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
        {
            var bti = BezierTransInfoList[i];
            bti.m_IsNeedUseColor_e = false;
        }

        Transform trans = transform;
        for (int bpdIndxe = 0, bpdLen = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length; bpdIndxe < bpdLen; bpdIndxe++)
        {
            #region 画辅助连线
            BezierTransInfo_e info = BezierTransInfoList[bpdIndxe];
            BezierTransInfo_e nextInfo = bpdIndxe < BezierTransInfoList.Count - 1 ? BezierTransInfoList[bpdIndxe + 1] : null;

            Color oldColor = Handles.color;
            if (m_SelectGo != null)
            {
                Transform selectTrans = m_SelectGo.transform;

                if (selectTrans == info.m_Trans ||
                    selectTrans == info.m_LeftHelpTrans ||
                    selectTrans == info.m_RightHelpTrans)
                {
                    info.m_IsNeedUseColor_e = true;

                    if (m_IsDrawHelpLine)
                    {
                        if (bpdIndxe > 0)
                        {
                            Handles.color = info.m_Color;
                            Handles.DrawLine(info.m_Trans.position, info.m_LeftHelpTrans.position);
                            Handles.color = oldColor;
                        }

                        if (nextInfo != null)
                        {
                            Handles.color = nextInfo.m_Color;
                            Handles.DrawLine(info.m_Trans.position, info.m_RightHelpTrans.position);
                            Handles.color = oldColor;
                        }
                    }
                }
                else if (nextInfo != null)
                {
                    if (selectTrans == nextInfo.m_Trans ||
                        selectTrans == nextInfo.m_LeftHelpTrans ||
                        selectTrans == nextInfo.m_RightHelpTrans)
                    {
                        info.m_IsNeedUseColor_e = true;
                    }
                }
            }
            #endregion

            #region 画bezier曲线
            if (bpdIndxe < bpdLen - 1)
            {
                var curBpd = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[bpdIndxe];
                var nextBpd = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[bpdIndxe + 1];
                Vector3 curPos;
                Vector3 curRightHelpPos = curBpd.RightHelpPos + curBpd.Pos;
                Vector3 nextPos;
                Vector3 nextLeftHelpPos = nextBpd.LeftHelpPos + nextBpd.Pos;
                if (m_BezierCurvesFuncComponent.m_IsNeedAxis)
                {
                    curPos = m_BezierCurvesFuncComponent.Axis_x * curBpd.Pos.x + m_BezierCurvesFuncComponent.Axis_y * curBpd.Pos.y + m_BezierCurvesFuncComponent.Axis_z * curBpd.Pos.z + trans.position;
                    curRightHelpPos = m_BezierCurvesFuncComponent.Axis_x * curRightHelpPos.x + m_BezierCurvesFuncComponent.Axis_y * curRightHelpPos.y + m_BezierCurvesFuncComponent.Axis_z * curRightHelpPos.z + trans.position;
                    nextPos = m_BezierCurvesFuncComponent.Axis_x * nextBpd.Pos.x + m_BezierCurvesFuncComponent.Axis_y * nextBpd.Pos.y + m_BezierCurvesFuncComponent.Axis_z * nextBpd.Pos.z + trans.position;
                    nextLeftHelpPos = m_BezierCurvesFuncComponent.Axis_x * nextLeftHelpPos.x + m_BezierCurvesFuncComponent.Axis_y * nextLeftHelpPos.y + m_BezierCurvesFuncComponent.Axis_z * nextLeftHelpPos.z + trans.position;
                }
                else
                {
                    curPos = curBpd.Pos + trans.position;
                    curRightHelpPos += trans.position;
                    nextPos = nextBpd.Pos + trans.position;
                    nextLeftHelpPos += trans.position;
                }

                if (info.m_IsNeedUseColor_e && nextInfo != null)
                {
                    Handles.color = nextInfo.m_Color;
                }
                float t = 1f / nextBpd.Segment;
                Vector3[] points = new Vector3[nextBpd.Segment + 1];
                float curverSumLen = 0f;
                for (int pIndex = 0, pCount = (int)nextBpd.Segment + 1; pIndex < pCount; pIndex++)
                {
                    var curV3 = GetBezierCurverPos((int)nextBpd.Segment, pIndex, t,
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

                    if (m_IsShowEventInfo)
                        UpdateEventInfoByRatio(bpdIndxe + 1, preV3, curV3, curverSumLen, preSumLen);
                }
                nextBpd.BezierCurverLen = curverSumLen;
                Handles.DrawPolyLine(points);
                Handles.color = oldColor;
            }
            #endregion
        }
    }

    public BezierCurvesFuncComponent m_BezierCurvesFuncComponent;

    [HideInInspector]
    public bool IsAddBezier;
    public bool IsResetMoveBezier;

    public List<BezierTransInfo_e> BezierTransInfoList = new List<BezierTransInfo_e>();

    public GameObject m_SelectGo;
    public BalanceHelpEnum m_BalanceHelpEnum = BalanceHelpEnum.Direction_And_Distance;

    public bool m_IsHideBezierSphere;

    public Transform Target;

    public BezierCurverLoopEnum m_LoopType;
    public uint m_MoveCount;

    [HideInInspector]
    public bool m_FollowLastBezier;

    public bool m_IsShowEventInfo;

    private void Awake()
    {
        m_BezierCurvesFuncComponent = new BezierCurvesFuncComponent();

        ResetConfigData();

        m_BezierCurvesFuncComponent.Init(transform.position);
    }

    public void Update()
    {
        if (m_BezierCurvesFuncComponent != null)
        {
            if (IsResetMoveBezier)
            {
                if (Target == null)
                {
                    Target = CreatePrimitiveTypeTrans(transform.position, null, PrimitiveType.Cube);
                }

                IsResetMoveBezier = false;
                ResetMove();
            }

            UpdateBezierData();

            float x = 0;
            float y = 0;
            float z = 0;
            bool isForward = false;
            if (!m_BezierCurvesFuncComponent.DoBezierCurves(ref x, ref y, ref z, ref isForward))
            {
                if (Target != null)
                {
                    Vector3 targetPos = new Vector3(x, y, z);
                    if (isForward)
                        Target.LookAt(targetPos);

                    Target.position = targetPos;
                }
            }
        }
        else
        {
            m_BezierCurvesFuncComponent = new BezierCurvesFuncComponent();
            ResetMove();
        }

        if (IsAddBezier)
        {
            IsAddBezier = false;

            //AddBezierFunc();
        }
    }

    public void AddBezierFunc()
    {
        AddBezier(GetBezierPosToOffset(), transform, BezierTransInfoList);

        if (m_FollowLastBezier && BezierTransInfoList.Count > 0)
            Selection.activeObject = BezierTransInfoList[BezierTransInfoList.Count - 1].m_Trans;
    }

    public void InsertBezierFunc()
    {
        if (m_SelectGo == null)
            return;

        for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
        {
            var info = BezierTransInfoList[i];
            var selectTrans = m_SelectGo.transform;
            if (selectTrans == info.m_LeftHelpTrans ||
                selectTrans == info.m_Trans ||
                selectTrans == info.m_RightHelpTrans)
            {
                AddBezier(GetBezierPosToOffset(i), transform,
                    BezierTransInfoList, i + 1);
                break;
            }
        }
    }

    public void DelBezier(GameObject delGo)
    {
        if (delGo == null)
            return;

        Transform delTrans = delGo.transform;

        for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
        {
            var info = BezierTransInfoList[i];
            if (info.m_Trans == delTrans ||
                info.m_LeftHelpTrans == delTrans ||
                info.m_RightHelpTrans == delTrans)
            {
                DelBezier(i);
                
                break;
            }
        }
    }

    private void DelBezier(int index)
    {
        if (index >= BezierTransInfoList.Count)
            return;

        Debug.Log($"删除贝塞尔节点第{index.ToString()}个数据");

        BezierTransInfo_e info = BezierTransInfoList[index];

        DelBezierData(index);

        BezierTransInfoList.RemoveAt(index);

        if (info.m_LeftHelpTrans != null)
            DestroyImmediate(info.m_LeftHelpTrans.gameObject);

        if (info.m_RightHelpTrans != null)
            DestroyImmediate(info.m_RightHelpTrans.gameObject);

        if (info.m_Trans != null)
            DestroyImmediate(info.m_Trans.gameObject);

        if (info.m_bezierCuverEvent_e_List.Count > 0)
        {
            for (int eventIndex = 0, eventCount = info.m_bezierCuverEvent_e_List.Count; eventIndex < eventCount; eventIndex++)
            {
                BezierCurverEvent_e bezierCurvesEvent_E = info.m_bezierCuverEvent_e_List[eventIndex];
                if (bezierCurvesEvent_E == null)
                    continue;

                if (bezierCurvesEvent_E.EventTrans != null)
                    DestroyImmediate(bezierCurvesEvent_E.EventTrans.gameObject);
            }
        }

        if (BezierTransInfoList.Count > 0)
        {
            var bti0 = BezierTransInfoList[0];
            bti0.m_LeftHelpTrans.gameObject.SetActive(false);

            var btiL = BezierTransInfoList[BezierTransInfoList.Count - 1];
            btiL.m_RightHelpTrans.gameObject.SetActive(false);
        }
    }

    private void UpdateBezierData()
    {
        if (m_BezierCurvesFuncComponent == null || BezierTransInfoList.Count == 0 ||
            m_BezierCurvesFuncComponent.m_BezierTrackData == null || m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas == null)
            return;

        for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
        {
            var info = BezierTransInfoList[i];
            if (info.m_Trans == null || info.m_LeftHelpTrans == null || info.m_RightHelpTrans == null)
            {
                DelBezier(i);
                
                --i;
                --count;
                continue;
            }

            var data = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[i];

            data.Segment = info.Segment_e;
            data.Pos = info.m_Trans.localPosition;
            data.LeftHelpPos = info.m_LeftHelpTrans.localPosition;
            data.RightHelpPos = info.m_RightHelpTrans.localPosition;

            if (m_BalanceHelpEnum != BalanceHelpEnum.None && m_SelectGo != null)
            {
                var selectTrans = m_SelectGo.transform;
                if (selectTrans == info.m_LeftHelpTrans)
                {
                    BalanceHelpTrans(info.m_LeftHelpTrans, info.m_RightHelpTrans, info.m_Trans, m_BalanceHelpEnum);
                }
                else if (selectTrans == info.m_RightHelpTrans)
                {
                    BalanceHelpTrans(info.m_RightHelpTrans, info.m_LeftHelpTrans, info.m_Trans, m_BalanceHelpEnum);
                }
            }

            if (info.m_bezierCuverEvent_e_List.Count > 0)
            {
                bool isNeedRefreshEvent = false;
                for (int eventIndex = 0, eventCount = info.m_bezierCuverEvent_e_List.Count; eventIndex < eventCount; eventIndex++)
                {
                    BezierCurverEvent_e bezierCurvesEvent_E = info.m_bezierCuverEvent_e_List[eventIndex];
                    if (bezierCurvesEvent_E == null)
                    {
                        info.m_bezierCuverEvent_e_List.RemoveAt(eventIndex);

                        isNeedRefreshEvent = true;

                        --eventIndex;
                        --eventCount;
                        continue;
                    }

                    if (bezierCurvesEvent_E.EventTrans == null)
                    {
                        info.m_bezierCuverEvent_e_List.RemoveAt(eventIndex);

                        isNeedRefreshEvent = true;

                        --eventIndex;
                        --eventCount;
                        continue;
                    }
                }

                if (isNeedRefreshEvent || info.m_bezierCuverEvent_e_List.Count > 0)
                    RefreshBezierEvents(data, info);
            }
        }

        if (m_SelectGo != null)
        {
            for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
            {
                var info = BezierTransInfoList[i];
                var selectTrans = m_SelectGo.transform;
                if (info.m_Trans == selectTrans ||
                    info.m_LeftHelpTrans == selectTrans ||
                    info.m_RightHelpTrans == selectTrans)
                {
                    if (info.m_CombineIndexList != null && info.m_CombineIndexList.Count > 0)
                    {
                        foreach (var setCombineIndxe in info.m_CombineIndexList)
                        {
                            var combineInfo = BezierTransInfoList[setCombineIndxe];
                            combineInfo.m_LeftHelpTrans.position = info.m_LeftHelpTrans.position;
                            combineInfo.m_Trans.position = info.m_Trans.position;
                            combineInfo.m_RightHelpTrans.position = info.m_RightHelpTrans.position;
                        }

                        break;
                    }
                }
            }
        }

        var trans = transform;
        m_BezierCurvesFuncComponent.m_OriginPos = trans.position;
        m_BezierCurvesFuncComponent.m_BezierTrackData.EulerAngleX = trans.eulerAngles.x;
        m_BezierCurvesFuncComponent.m_BezierTrackData.EulerAngleY = trans.eulerAngles.y;
        m_BezierCurvesFuncComponent.m_BezierTrackData.EulerAngleZ = trans.eulerAngles.z;
        m_BezierCurvesFuncComponent.SetAxis();
    }

    private void BalanceHelpTrans(Transform src, Transform des, Transform center, BalanceHelpEnum balanceHelpType)
    {
        if (balanceHelpType == BalanceHelpEnum.Direction)
        {
            float desDis = (des.position - center.position).magnitude;
            Vector3 srcNormalizer = (center.position - src.position).normalized;

            des.position = center.position + srcNormalizer * desDis;
        }
        else if (balanceHelpType == BalanceHelpEnum.Direction_And_Distance)
        {
            des.position = center.position + center.position - src.position;
        }
    }

    public void HideBezierSphere(bool isHide)
    {
        for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
        {
            var info = BezierTransInfoList[i];

            info.m_Trans.GetComponent<MeshRenderer>().enabled = !isHide;
            info.m_LeftHelpTrans.GetComponent<MeshRenderer>().enabled = !isHide;
            info.m_RightHelpTrans.GetComponent<MeshRenderer>().enabled = !isHide;
        }
    }

    #region 数据和表现接口
    public void SetBezierData(Bezier3CurvesData bezier3CurvesData, Transform parent, List<BezierTransInfo_e> bezierTransInfoList)
    {
        bezierTransInfoList.Clear();

        m_BezierCurvesFuncComponent.m_BezierTrackData = bezier3CurvesData;

        for (int bcIndex = 0, bcCount = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length; bcIndex < bcCount; bcIndex++)
        {
            Bezier3PosData bezier3PosData = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[bcIndex];

            BezierTransInfo_e bezierTransInfo = new BezierTransInfo_e();

            ++_bezierStartNum;
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);

            bezierTransInfo.Segment_e = bezier3PosData.Segment;

            bezierTransInfo.m_Trans = CreatePrimitiveTypeTrans(bezier3PosData.Pos, parent);
            bezierTransInfo.m_Trans.name = $"B_{_bezierStartNum}";
            bezierTransInfo.m_Trans.localPosition = bezier3PosData.Pos;

            bezierTransInfo.m_LeftHelpTrans = CreatePrimitiveTypeTrans(bezier3PosData.LeftHelpPos, bezierTransInfo.m_Trans);
            bezierTransInfo.m_LeftHelpTrans.name = $"L_{_bezierStartNum}";
            bezierTransInfo.m_LeftHelpTrans.localPosition = bezier3PosData.LeftHelpPos;

            bezierTransInfo.m_RightHelpTrans = CreatePrimitiveTypeTrans(bezier3PosData.RightHelpPos, bezierTransInfo.m_Trans);
            bezierTransInfo.m_RightHelpTrans.name = $"R_{_bezierStartNum}";
            bezierTransInfo.m_RightHelpTrans.localPosition = bezier3PosData.RightHelpPos;

            bezierTransInfo.m_Color = color;

            bezierTransInfo.m_BezierRatioInfo_e = new BezierRatioInfo_e();

            bezierTransInfoList.Add(bezierTransInfo);
        }
    }

    public void AddBezier(Vector3 pos, Transform parent, List<BezierTransInfo_e> bezierTransInfoList, int insertIndex = -1)
    {
        if (bezierTransInfoList.Count > 0)
        {
            var bti0 = bezierTransInfoList[0];
            bti0.m_LeftHelpTrans.gameObject.SetActive(true);
            bti0.m_RightHelpTrans.gameObject.SetActive(true);

            var btiL = bezierTransInfoList[bezierTransInfoList.Count - 1];
            btiL.m_LeftHelpTrans.gameObject.SetActive(true);
            btiL.m_RightHelpTrans.gameObject.SetActive(true);
        }

        if (m_BezierCurvesFuncComponent.m_BezierTrackData == null)
        {
            m_BezierCurvesFuncComponent.m_BezierTrackData = new Bezier3CurvesData();
        }

        if (m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas == null || m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length == 0)
        {
            bezierTransInfoList.Clear();

            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas = new Bezier3PosData[2];

            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[0] = GetBezierPosData(pos, parent, out BezierTransInfo_e bezierTransInfo);
            bezierTransInfoList.Add(bezierTransInfo);

            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[1] = GetBezierPosData(pos + new Vector3(2f, 0f, 0f), parent, out bezierTransInfo);
            bezierTransInfoList.Add(bezierTransInfo);

            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams = new int[1];
        }
        else
        {
            if (insertIndex > m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length || insertIndex < 0)
                insertIndex = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length;

            var bpdList = new List<Bezier3PosData>();
            bpdList.AddRange(m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas);

            bpdList.Insert(insertIndex, GetBezierPosData(pos, parent, out BezierTransInfo_e bezierTransInfo));
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas = bpdList.ToArray();

            bezierTransInfoList.Insert(insertIndex, bezierTransInfo);

            var paramList = new List<int>();
            paramList.AddRange(m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams);
            paramList.Insert(insertIndex - 1, 0);
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams = paramList.ToArray();
        }

        if (bezierTransInfoList.Count > 0)
        {
            var bti0 = bezierTransInfoList[0];
            bti0.m_LeftHelpTrans.gameObject.SetActive(false);

            var btiL = bezierTransInfoList[bezierTransInfoList.Count - 1];
            btiL.m_RightHelpTrans.gameObject.SetActive(false);
        }
    }

    public void DelBezierData(int delIndex)
    {
        if (m_BezierCurvesFuncComponent.m_BezierTrackData == null || m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas == null ||
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length == 0)
            return;

        List<Bezier3PosData> list = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.ToList();
        list.RemoveAt(delIndex);
        m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas = list.ToArray();

        var paramList = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams.ToList();
        paramList.RemoveAt(delIndex == 0 ? 0 : delIndex - 1);
        m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierParams = paramList.ToArray();
    }

    private static uint _bezierStartNum;
    private Bezier3PosData GetBezierPosData(Vector3 pos, Transform parent, out BezierTransInfo_e bezierTransInfo)
    {
        Bezier3PosData bpd = new Bezier3PosData();

        ++_bezierStartNum;

        Color color = new Color(Random.Range(0f, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);

        bezierTransInfo = new BezierTransInfo_e();
        bezierTransInfo.Segment_e = 20;

        bezierTransInfo.m_Trans = CreatePrimitiveTypeTrans(pos, parent);
        bezierTransInfo.m_Trans.name = $"B_{_bezierStartNum}";
        //bezierTransInfo.m_Trans.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        bezierTransInfo.m_LeftHelpTrans = CreatePrimitiveTypeTrans(pos, bezierTransInfo.m_Trans);
        bezierTransInfo.m_LeftHelpTrans.name = $"L_{_bezierStartNum}";
        //bezierTransInfo.m_LeftHelpTrans.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        bezierTransInfo.m_RightHelpTrans = CreatePrimitiveTypeTrans(pos, bezierTransInfo.m_Trans);
        bezierTransInfo.m_RightHelpTrans.name = $"R_{_bezierStartNum}";
        //bezierTransInfo.m_RightHelpTrans.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        bpd.LeftHelpPos = bezierTransInfo.m_LeftHelpTrans.localPosition;
        bpd.Pos = bezierTransInfo.m_Trans.localPosition;
        bpd.RightHelpPos = bezierTransInfo.m_RightHelpTrans.localPosition;

        bezierTransInfo.m_Color = color;

        bezierTransInfo.m_BezierRatioInfo_e = new BezierRatioInfo_e();

        return bpd;
    }

    private Transform CreatePrimitiveTypeTrans(Vector3 pos, Transform parent, PrimitiveType primitiveType = PrimitiveType.Sphere)
    {
        return CreatePrimitiveTypeTrans(pos, parent, Vector3.one, primitiveType);
    }

    private Transform CreatePrimitiveTypeTrans(Vector3 pos, Transform parent, Vector3 localScale, PrimitiveType primitiveType)
    {
        GameObject go = GameObject.CreatePrimitive(primitiveType);
        Transform trans = go.transform;
        trans.SetParent(parent);
        trans.position = pos;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = localScale;

        return trans;
    }

    public void CheckBezierData(List<BezierTransInfo_e> bezierTransInfoList)
    {
        if (m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas != null && m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length == bezierTransInfoList.Count)
            return;

        m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas = new Bezier3PosData[bezierTransInfoList.Count];
        for (int i = 0, count = bezierTransInfoList.Count; i < count; i++)
        {
            var info = bezierTransInfoList[i];

            Bezier3PosData bpd = new Bezier3PosData();
            bpd.Segment = info.Segment_e;
            bpd.Pos = info.m_Trans.localPosition;
            bpd.LeftHelpPos = info.m_LeftHelpTrans.localPosition;
            bpd.RightHelpPos = info.m_RightHelpTrans.localPosition;
        }
    }

    private Transform CreateTrans(Vector3 pos, Transform parent)
    {
        GameObject go = new GameObject();
        Transform trans = go.transform;
        trans.SetParent(parent);
        trans.position = pos;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;

        return trans;
    }

    public Vector3 GetBezierPosToOffset(int dataIndex = -1)
    {
        if (m_BezierCurvesFuncComponent.m_BezierTrackData == null || m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas == null ||
            m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length == 0)
            return Vector3.zero;

        if (dataIndex < 0)
            dataIndex = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas.Length - 1;

        return m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[dataIndex].Pos + new Vector3(1f, 0f, 0f);
    }

    public static Vector3 GetBezierCurverPos(int segment, float segIndex, float segDis, Vector3 curPos, Vector3 curRightHelpPos, Vector3 nextLeftHelpPos, Vector3 nextPos)
    {
        float f = segIndex == segment ? 1 : segDis * segIndex;
        return CombatHelp.Calculate3BezierPoint_3D(f, curPos, curRightHelpPos, nextLeftHelpPos, nextPos);
    }

    public void ResetMove()
    {
        m_BezierCurvesFuncComponent.Init(transform.position, m_LoopType, m_MoveCount);
        m_BezierCurvesFuncComponent.AddEvent(this);
    }

    public void CombineBezierPoints()
    {
        GameObject[] selects = Selection.gameObjects;
        if (selects == null || selects.Length <= 0)
            return;

        Dictionary<int, BezierTransInfo_e> dic = new Dictionary<int, BezierTransInfo_e>();
        for (int sIndex = 0, sCount = selects.Length; sIndex < sCount; sIndex++)
        {
            Transform selectTrans = selects[sIndex].transform;
            for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
            {
                var info = BezierTransInfoList[i];
                if (info.m_Trans == selectTrans ||
                    info.m_LeftHelpTrans == selectTrans ||
                    info.m_RightHelpTrans == selectTrans)
                {
                    dic[i] = info;

                    break;
                }
            }
        }

        if (dic.Count <= 0)
            return;

        foreach (var kv1 in dic)
        {
            if (kv1.Value.m_CombineIndexList == null)
                kv1.Value.m_CombineIndexList = new List<int>();
            else
                kv1.Value.m_CombineIndexList.Clear();
            foreach (var kv2 in dic)
            {
                if (kv2.Key != kv1.Key)
                {
                    kv1.Value.m_CombineIndexList.Add(kv2.Key);
                }
            }
        }

        UpdateBezierData();
    }

    public void DelCombineBezierPoints()
    {
        GameObject[] selects = Selection.gameObjects;
        if (selects == null || selects.Length <= 0)
            return;

        for (int sIndex = 0, sCount = selects.Length; sIndex < sCount; sIndex++)
        {
            Transform selectTrans = selects[sIndex].transform;
            for (int i = 0, count = BezierTransInfoList.Count; i < count; i++)
            {
                var info = BezierTransInfoList[i];
                if (info.m_Trans == selectTrans ||
                    info.m_LeftHelpTrans == selectTrans ||
                    info.m_RightHelpTrans == selectTrans)
                {
                    if (info.m_CombineIndexList != null)
                    {
                        foreach (var combineIndex in info.m_CombineIndexList)
                        {
                            BezierTransInfoList[combineIndex].m_CombineIndexList.Clear();
                        }

                        info.m_CombineIndexList.Clear();
                    }

                    break;
                }
            }
        }
    }

    private void ResetConfigData()
    {
        string name = gameObject.name.Replace("(Clone)", string.Empty);
        if (!uint.TryParse(name, out uint trackId))
            return;

        //if (HBSConfigManager.Instance == null)
        //    return;

        m_BezierCurvesFuncComponent.SetBezierData(trackId);
    }

    public void UpdateEventInfoByRatio(int curverIndex, Vector3 startPos, Vector3 endPos, float curMoveLen, float preMoveLen)
    {
        if (curverIndex <= 0)
            return;

        BezierTransInfo_e endTransInfo = BezierTransInfoList[curverIndex];
        Bezier3PosData bezier3PosData = m_BezierCurvesFuncComponent.m_BezierTrackData.m_BezierPosDatas[curverIndex];
        if (bezier3PosData.BezierCurverLen == 0f)
            return;

        int bcEventCount = endTransInfo.m_bezierCuverEvent_e_List.Count;
        if (bcEventCount > 0)
        {
            if (bcEventCount > 1)
            {
                endTransInfo.m_bezierCuverEvent_e_List.Sort(SortEventInfo);
            }
            
            if (bezier3PosData.EventInfoArray == null || bezier3PosData.EventInfoArray.Length != bcEventCount)
            {
                bezier3PosData.EventInfoArray = new BezierCurverEvent[bcEventCount];
            }

            for (int i = 0; i < bcEventCount; i++)
            {
                BezierCurverEvent_e eventInfo = endTransInfo.m_bezierCuverEvent_e_List[i];
                if (eventInfo.EventTrans == null)
                {
                    endTransInfo.m_bezierCuverEvent_e_List.RemoveAt(i);
                    return;
                }
                if (curMoveLen == 0f)
                {
                    if (eventInfo.EventInfo.EventRatio == 0)
                    {
                        eventInfo.EventTrans.position = startPos;
                    }
                }
                else
                {
                    float eventRatioLen = eventInfo.EventInfo.EventRatio * bezier3PosData.BezierCurverLen;
                    if (eventRatioLen == preMoveLen)
                    {
                        eventInfo.EventTrans.position = startPos;
                    }
                    else if (eventRatioLen == curMoveLen)
                    {
                        eventInfo.EventTrans.position = endPos;
                    }
                    else if (eventRatioLen > preMoveLen && eventRatioLen < curMoveLen)
                    {
                        eventInfo.EventTrans.position = startPos + (endPos - startPos).normalized * (eventRatioLen - preMoveLen);
                    }
                }

                if (m_SelectGo != null && m_SelectGo == eventInfo.EventTrans.gameObject)
                {
                    endTransInfo.m_SelectBezierEventIndex = i;
                }

                int eventId = curverIndex * 100 + i + 1;
                if (eventInfo.EventInfo.EventId != eventId)
                {
                    eventInfo.EventInfo.EventId = eventId;
                    eventInfo.EventTrans.name = $"{eventId.ToString()}_Event";
                }

                bezier3PosData.EventInfoArray[i] = eventInfo.EventInfo;
            }
        }
    }

    private void RefreshBezierEvents(Bezier3PosData bezier3PosData, BezierTransInfo_e endTransInfo)
    {
        int bcEventCount = endTransInfo.m_bezierCuverEvent_e_List.Count;
        if (bcEventCount > 0)
        {
            if (bcEventCount > 1)
            {
                endTransInfo.m_bezierCuverEvent_e_List.Sort(SortEventInfo);
            }

            if (bezier3PosData.EventInfoArray == null || bezier3PosData.EventInfoArray.Length != bcEventCount)
            {
                bezier3PosData.EventInfoArray = new BezierCurverEvent[bcEventCount];
            }

            for (int i = 0; i < bcEventCount; i++)
            {
                BezierCurverEvent_e eventInfo = endTransInfo.m_bezierCuverEvent_e_List[i];
                if (eventInfo.EventTrans != null)
                {
                    string eventTransName = eventInfo.EventTrans.name;
                    string[] ens = eventTransName.Split('_');
                    if (ens.Length < 2 || !int.TryParse(ens[ens.Length - 1], out int eventId))
                    {
                        eventInfo.EventInfo.EventFlagName = eventTransName;
                    }
                    else
                    {
                        int.TryParse(ens[ens.Length - 1], out eventId);
                        eventInfo.EventInfo.EventId = eventId;
                        eventInfo.EventInfo.EventFlagName = null;
                        for (int eventTransNamesIndex = 0; eventTransNamesIndex < ens.Length - 1; eventTransNamesIndex++)
                        {
                            eventInfo.EventInfo.EventFlagName += $"{(eventTransNamesIndex == 0 ? null : "_")}{ens[eventTransNamesIndex]}";
                        }
                    }

                    eventInfo.EventTrans.name = $"{eventInfo.EventInfo.EventFlagName}_{eventInfo.EventInfo.EventId}";
                }

                bezier3PosData.EventInfoArray[i] = eventInfo.EventInfo;
            }
        }
    }

    private int SortEventInfo(BezierCurverEvent_e a, BezierCurverEvent_e b)
    {
        if (a.EventInfo.EventRatio < b.EventInfo.EventRatio)
            return -1;
        else if (a.EventInfo.EventRatio > b.EventInfo.EventRatio)
            return 1;
        else
            return 0;
    }

    public void CreateEventInfo(int curverIndex)
    {
        BezierTransInfo_e bezierTransInfo = BezierTransInfoList[curverIndex];

        BezierCurverEvent_e bezierCurverEvent_E = new BezierCurverEvent_e();
        bezierCurverEvent_E.EventInfo = new BezierCurverEvent();
        bezierCurverEvent_E.EventInfo.EventId = curverIndex * 100 + bezierTransInfo.m_bezierCuverEvent_e_List.Count + 1;
        bezierCurverEvent_E.EventInfo.EventRatio = 0.1f;
        bezierCurverEvent_E.EventInfo.EventCount = int.MaxValue;
        CreateEventTrans(curverIndex, bezierCurverEvent_E);
        bezierCurverEvent_E.EventTrans.name = $"{bezierCurverEvent_E.EventTrans.name}_Event_{bezierCurverEvent_E.EventInfo.EventId.ToString()}";

        bezierTransInfo.m_bezierCuverEvent_e_List.Add(bezierCurverEvent_E);
    }

    private void CreateEventTrans(int curverIndex, BezierCurverEvent_e bezierCurverEvent_E)
    {
        bezierCurverEvent_E.EventTrans = CreatePrimitiveTypeTrans(Vector3.one, transform, Vector3.one * 0.5f, PrimitiveType.Sphere);
        UpdateEventTransPos(curverIndex, bezierCurverEvent_E);
    }

    public void UpdateEventTransPos(int curverIndex, BezierCurverEvent_e bezierCurverEvent_E)
    {
        if (curverIndex <= 0)
            return;

        BezierTransInfo_e startTransInfo = BezierTransInfoList[curverIndex - 1];
        BezierTransInfo_e endTransInfo = BezierTransInfoList[curverIndex];

        bezierCurverEvent_E.EventTrans.position = CombatHelp.Calculate3BezierPoint_3D(bezierCurverEvent_E.EventInfo.EventRatio,
            startTransInfo.m_Trans.position, startTransInfo.m_RightHelpTrans.position,
            endTransInfo.m_LeftHelpTrans.position, endTransInfo.m_Trans.position);
    }

    public void OnBezierEvent(int bezierIndex, int eventId, string eventFlagName)
    {
        Debug.LogError($"{Time.realtimeSinceStartup}    {bezierIndex}   {eventId}   {eventFlagName}");
    }
    #endregion
}
#endif