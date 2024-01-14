#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierController_3D : MonoBehaviour
{
    public bool EditorBezier;

    public bool AddBezier;

    public bool DelBezier;

    public bool BezierFuncType = true;

    public bool ShowBezierModel = true;

    [System.Serializable]
    public class EditorTransClass
    {
        public int m_LeftIndex;
        public Transform m_LeftHelpTrans;
        public int m_Index;
        public Transform m_Trans;
        public int m_RightIndex;
        public Transform m_RightHelpTrans;
    }

    public List<EditorTransClass> EditorPointTrans = new List<EditorTransClass>();

    private List<Vector3> _bpList = new List<Vector3>();

    private void OnDrawGizmos()
    {
        //for (int i = 0; i < EditorPointTrans.Count - 1; i++)
        //{
        //    var curEPT = EditorPointTrans[i];
        //    var nextEPT = EditorPointTrans[i + 1];
        //    UnityEditor.Handles.DrawBezier(curEPT.m_Trans.position, nextEPT.m_Trans.position, curEPT.m_RightHelpTrans.position, nextEPT.m_LeftHelpTrans.position, new Color(0.8f, 0.1f, 0.1f), null, 5f);
        //}

        if (AddBezier)
        {
            AddBezier = false;

            BezierPointClass bpc = new BezierPointClass();
            
            var end = FindBezierPoint(m_EndPointIndex);
            if (end != null)
            {
                end.m_RightHelpPoint = end.m_Point;
                end.m_RightIndex = m_EndPointIndex + 1;

                bpc.m_LeftIndex = m_EndPointIndex;
                bpc.m_LeftHelpPoint = end.m_Point;
                bpc.m_Index = m_EndPointIndex + 1;
                bpc.m_Point = end.m_Point;


                var ts = EditorPointTrans.Find(x => x.m_Index == m_EndPointIndex);
                if (ts != null)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    Transform rightTf = Clone(go.transform, ts.m_Trans);
                    rightTf.name = "RightHelpPoint_" + (m_EndPointIndex + 1).ToString();

                    ts.m_RightHelpTrans = rightTf;
                    ts.m_RightIndex = m_EndPointIndex + 1;

                    DestroyImmediate(go);
                }
            }
            else
            {
                bpc.m_Index = m_EndPointIndex + 1;
            }

            m_BezierPointList.Add(bpc);


            m_EndPointIndex++;
        }

        if (DelBezier)
        {
            DelBezier = false;
            
            int preIndex = -1;
            var bpc = FindBezierPoint(m_EndPointIndex);
            if (bpc != null)
            {
                preIndex = bpc.m_LeftIndex;
            }
            m_BezierPointList.RemoveAll(x => x.m_Index == m_EndPointIndex);


            if (preIndex > 0)
            {
                var preBpc = FindBezierPoint(preIndex);
                if (preBpc != null)
                {
                    preBpc.m_RightIndex = 0;
                    preBpc.m_RightHelpPoint = Vector3.zero;
                }

                var preTs = EditorPointTrans.Find(x => x.m_Index == preIndex);
                if (preTs != null)
                {
                    if (preTs.m_RightHelpTrans != null)
                    {
                        DestroyImmediate(preTs.m_RightHelpTrans.gameObject, true);
                    }

                    preTs.m_RightHelpTrans = null;
                    preTs.m_RightIndex = 0;
                }
            }

            var ts = EditorPointTrans.Find(x => x.m_Index == m_EndPointIndex);
            if (ts != null)
            {
                DelTransStruct(ts);
            }
            EditorPointTrans.RemoveAll(x => x.m_Index == m_EndPointIndex);

            if(m_EndPointIndex > 0)
                m_EndPointIndex--;
        }

        if (EditorBezier)
        {
            foreach (var val in m_BezierPointList)
            {
                var ts = EditorPointTrans.Find(x => x.m_Index == val.m_Index);
                if (ts == null)
                {
                    ts = new EditorTransClass();

                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                    Transform pt = Clone(go.transform, transform);
                    pt.position = val.m_Point;
                    pt.name = "BezierPoint_" + val.m_Index.ToString();

                    ts.m_Index = val.m_Index;
                    ts.m_Trans = pt;

                    if (val.m_LeftIndex > 0)
                    {
                        Transform left = Clone(go.transform, pt);
                        left.position = val.m_LeftHelpPoint;
                        left.name = "LeftHelpPoint_" + val.m_LeftIndex.ToString();

                        ts.m_LeftIndex = val.m_LeftIndex;
                        ts.m_LeftHelpTrans = left;
                    }

                    if (val.m_RightIndex > 0)
                    {
                        Transform right = Clone(go.transform, pt);
                        right.position = val.m_RightHelpPoint;
                        right.name = "RightHelpPoint_" + val.m_RightIndex.ToString();

                        ts.m_RightIndex = val.m_RightIndex;
                        ts.m_RightHelpTrans = right;
                    }

                    EditorPointTrans.Add(ts);

                    DestroyImmediate(go);
                }
            }

            foreach (var ts in EditorPointTrans)
            {
                var bpc = FindBezierPoint(ts.m_Index);
                if (bpc == null)
                {
                    DelTransStruct(ts);
                    continue;
                }

                bpc.m_Point = ts.m_Trans.position;
                if (bpc.m_LeftIndex > 0)
                    bpc.m_LeftHelpPoint = ts.m_LeftHelpTrans.position;
                if (bpc.m_RightIndex > 0)
                    bpc.m_RightHelpPoint = ts.m_RightHelpTrans.position;
            }
        }
        else
        {
            foreach (var ts in EditorPointTrans)
            {
                DelTransStruct(ts);
            }
            EditorPointTrans.Clear();
        }

        _bpList.Clear();
        foreach (var curBpc in m_BezierPointList)
        {
            var nextBpc = FindBezierPoint(curBpc.m_RightIndex);
            if (curBpc.m_RightIndex <= 0 || nextBpc == null)
                continue;
            
            int count = 50;
            float t = 1f / count;
            Vector3[] points = new Vector3[count + 1];
            for (int i = 0; i < count + 1; i++)
            {
                float f = t * i;
                var v3 = BezierToolHelper.Calculate3BezierPoint_3D(f, curBpc.m_Point, curBpc.m_RightHelpPoint, nextBpc.m_LeftHelpPoint, nextBpc.m_Point);

                points[i] = v3;

                _bpList.Add(v3);
            }
            UnityEditor.Handles.DrawPolyLine(points);
        }
        m_PathPoints = _bpList.ToArray();

        if (EditorPointTrans[0].m_Trans.GetComponent<SphereCollider>().enabled != ShowBezierModel)
        {
            foreach (var ept in EditorPointTrans)
            {
                if (ept.m_LeftHelpTrans != null)
                {
                    ept.m_LeftHelpTrans.GetComponent<SphereCollider>().enabled = ShowBezierModel;
                    ept.m_LeftHelpTrans.GetComponent<MeshRenderer>().enabled = ShowBezierModel;
                }
                if (ept.m_Trans != null)
                {
                    ept.m_Trans.GetComponent<SphereCollider>().enabled = ShowBezierModel;
                    ept.m_Trans.GetComponent<MeshRenderer>().enabled = ShowBezierModel;
                }
                if (ept.m_RightHelpTrans != null)
                {
                    ept.m_RightHelpTrans.GetComponent<SphereCollider>().enabled = ShowBezierModel;
                    ept.m_RightHelpTrans.GetComponent<MeshRenderer>().enabled = ShowBezierModel;
                }
            }
        }
    }
    
    private void DelTransStruct(EditorTransClass ts)
    {
        if (ts.m_Trans != null)
        {
            DestroyImmediate(ts.m_Trans.gameObject, true);
        }

        if (ts.m_LeftHelpTrans != null)
        {
            DestroyImmediate(ts.m_LeftHelpTrans.gameObject, true);
        }

        if (ts.m_RightHelpTrans != null)
        {
            DestroyImmediate(ts.m_RightHelpTrans.gameObject, true);
        }
    }
    
    [System.Serializable]
    public class BezierPointClass
    {
        public int m_LeftIndex;
        public Vector3 m_LeftHelpPoint;
        public int m_Index;
        public Vector3 m_Point;
        public int m_RightIndex;
        public Vector3 m_RightHelpPoint;
    }

    public List<BezierPointClass> m_BezierPointList = new List<BezierPointClass>();
    public int m_EndPointIndex;
    public Vector3[] m_PathPoints;

    public Transform m_MoveTrans;
    
    public BezierPointClass FindBezierPoint(int bezierIndex)
    {
        for (int j = 0; j < m_BezierPointList.Count; j++)
        {
            if (m_BezierPointList[j].m_Index == bezierIndex)
            {
                return m_BezierPointList[j];
            }
        }

        return null;
    }

    public BezierPointClass FindBezierPoint(int bezierIndex, out int i)
    {
        i = -1;

        for (int j = 0; j < m_BezierPointList.Count; j++)
        {
            if (m_BezierPointList[j].m_Index == bezierIndex)
            {
                i = j;
                return m_BezierPointList[j];
            }
        }

        return null;
    }

    public int m_Index;
    
    private void Awake()
    {
        
    }

    //public Transform m_Trans02;
    //public Transform m_Trans03;
    //private float _time02;
    private void Update()
    {
        if(BezierFuncType)
            SmoothMoveBezier03(m_MoveTrans);
        else
            SmoothMoveBezier02(m_MoveTrans);

        //if (m_Trans02 != null)
        //{
        //    _time02 += Time.deltaTime;
        //    m_Trans02.position = m_Trans02.right * m_Speed * _time02;
        //}

        //if (m_Trans03 != null)
        //    m_Trans03.position += m_Trans03.right * m_Speed * Time.deltaTime;
    }

    #region 调试用
    public void SmoothMoveBezier01(Transform trans)
    {
        if (trans == null || m_PathPoints == null || (m_Index + 1 >= m_PathPoints.Length))
            return;

        Vector3 v1 = m_PathPoints[m_Index];
        Vector3 v2 = m_PathPoints[m_Index + 1];
        Vector3 v12 = v2 - v1;
        Vector3 v12Nor = v12.normalized;

        trans.LookAt(v2);

        _time += Time.deltaTime;
        Vector3 disV = v12Nor * m_Speed * _time;

        if (SimulateDot(disV, disV) >= SimulateDot(v12, v12))
        {
            trans.position = v2;
            _time = 0f;
            m_Index++;
        }
        else
        {
            trans.position = v1 + disV;
        }
    }

    public void SmoothMoveBezier02(Transform trans)
    {
        if (trans == null || m_PathPoints == null || (m_Index + 1 >= m_PathPoints.Length))
            return;

        _time += Time.deltaTime;
        float moveLen = m_Speed * _time;

        int i = m_Index;
        while (i < m_PathPoints.Length - 1)
        {
            Vector3 v1 = m_PathPoints[i];
            Vector3 v2 = m_PathPoints[i + 1];
            Vector3 v12 = v2 - v1;

            float mm = moveLen * moveLen;
            float v12v12 = SimulateDot(v12, v12);

            if (mm > v12v12)
            {
                moveLen = moveLen - v12.magnitude;
                _time = 0f;
                i++;
                continue;
            }
            else if (mm == v12v12)
            {
                trans.position = v2;
                trans.LookAt(v2);
                _time = 0f;
                i++;
                m_Index = i;
                return;
            }
            else
            {
                if (i > m_Index)
                {
                    trans.position = v1;
                    _time = 0f;
                }
                else
                {
                    Vector3 v12Nor = v12.normalized;
                    Vector3 disV = v12Nor * moveLen;
                    trans.position = v1 + disV;
                }

                trans.LookAt(v2);

                m_Index = i;
                return;
            }
        }

        trans.position = m_PathPoints[i];
        m_Index = i;
    }
    #endregion
    


    public float m_Speed = 2f;
    public int m_BezierIndex;

    public bool m_Repeat03;

    private float _time;
    private int _pointIndex;
    private float _moveLen;
    private bool _refreshMoveLen = true;
    public void SmoothMoveBezier03(Transform trans)
    {
        if (trans == null)
            return;

        if (_pointIndex >= 20)
        {
            _pointIndex = 0;
            m_BezierIndex++;
        }

        if (m_BezierIndex > EditorPointTrans.Count - 2)
        {
            trans.position = EditorPointTrans[m_BezierIndex].m_Trans.position;

            if(m_Repeat03)
                m_BezierIndex = 0;

            _pointIndex = 0;
            _time = 0f;

            return;
        }

        var curBpc = EditorPointTrans[m_BezierIndex];
        var nextBpc = EditorPointTrans[m_BezierIndex + 1];
        Vector3 point01 = BezierToolHelper.Calculate3BezierPoint_3D(_pointIndex * 0.05f, curBpc.m_Trans.position, curBpc.m_RightHelpTrans.position, nextBpc.m_LeftHelpTrans.position, nextBpc.m_Trans.position);
        Vector3 point02 = BezierToolHelper.Calculate3BezierPoint_3D((_pointIndex + 1) * 0.05f, curBpc.m_Trans.position, curBpc.m_RightHelpTrans.position, nextBpc.m_LeftHelpTrans.position, nextBpc.m_Trans.position);
        //Debug.LogError($"{point01.ToString()}-------{point02.ToString()}-------{_pointIndex04.ToString()}");

        Vector3 pp = point02 - point01;
        if (_refreshMoveLen)
        {
            _time += Time.deltaTime;
            _moveLen = m_Speed * _time;
        }
        
        if (_moveLen * _moveLen > SimulateDot(pp, pp))
        {
            float ppDis = pp.magnitude;

            _moveLen -= ppDis;
            _time = 0f;

            _refreshMoveLen = false;
            _pointIndex++;

            SmoothMoveBezier03(trans);
            return;
        }
        else
        {
            _refreshMoveLen = true;
            trans.position = point01 + pp.normalized * _moveLen;
            return;
        }
    }

    public float SimulateDot(Vector3 a, Vector3 b)
    {
        return (a.x * b.x + a.y * b.y + a.z * b.z);
    }

    public Transform Clone(Transform obj, Transform parent)
    {
        GameObject go = Instantiate(obj.gameObject);

        Transform trans = go.transform;

        trans.SetParent(parent);

        trans.localPosition = Vector3.zero;
        trans.localEulerAngles = Vector3.zero;
        trans.localScale = Vector3.one;

        return trans;
    }
}
#endif
