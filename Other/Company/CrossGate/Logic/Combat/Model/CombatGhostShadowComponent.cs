using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadowData
{
    public Transform m_Trans;
    public Mesh m_Mesh;
    public MeshFilter m_MeshFilter;
    public MeshRenderer m_MeshRenderer;
    public bool m_IsSMR;
    public float m_LifeTime;

    public int m_GhostStyle;

    public bool m_IsNotFree;

    public static GhostShadowData GetGhostShadowData()
    {
        return CombatObjectPool.Instance.Get<GhostShadowData>();
    }

    public void Push()
    {
        if (m_MeshFilter != null)
        {
            m_MeshFilter = null;
        }
        if (m_GhostStyle == 0 && m_MeshRenderer != null)
        {
            Object.DestroyImmediate(m_MeshRenderer.material);
            m_MeshRenderer = null;
        }
        if (m_Mesh != null)
        {
            Object.DestroyImmediate(m_Mesh);
            m_Mesh = null;
        }
        if (m_Trans != null)
        {
            Object.DestroyImmediate(m_Trans.gameObject);
            m_Trans = null;
        }
        //m_IsSMR = false;

        m_GhostStyle = 0;

        m_IsNotFree = false;

        CombatObjectPool.Instance.Push(this);
    }
}

public class CombatGhostShadowComponent : AComponent, IAwake, IUpdate
{
    private TQueue<GhostShadowData> _freeSMRQueue = new TQueue<GhostShadowData>();
    private TQueue<GhostShadowData> _freeMFQueue = new TQueue<GhostShadowData>();

    private float _checkDelTime;

    public override void Dispose()
    {
        while (_freeSMRQueue.Count > 0)
        {
            GhostShadowData ghostShadowData = _freeSMRQueue.Dequeue();
            ghostShadowData.Push();
        }

        while (_freeMFQueue.Count > 0)
        {
            GhostShadowData ghostShadowData = _freeMFQueue.Dequeue();
            ghostShadowData.Push();
        }

        _checkDelTime = 0f;

        base.Dispose();
    }

    public void Awake()
    {
        _checkDelTime = Time.realtimeSinceStartup + 10f;
    }

    public void Update()
    {
        if (_freeSMRQueue.Count > 0 || _freeMFQueue.Count > 0)
        {
            if (Time.realtimeSinceStartup > _checkDelTime)
            {
                if (_freeSMRQueue.CheckExpire())
                {
                    while (_freeSMRQueue.Count > 0)
                    {
                        _freeSMRQueue.Dequeue().Push();
                    }
                }

                if (_freeMFQueue.CheckExpire())
                {
                    while (_freeMFQueue.Count > 0)
                    {
                        _freeMFQueue.Dequeue().Push();
                    }
                }

                _checkDelTime = Time.realtimeSinceStartup + 10f;
            }
        }
        else if (Time.realtimeSinceStartup > _checkDelTime)
            Dispose();
    }

    public GhostShadowData Get(bool isSMR)
    {
        return isSMR ? GetSMRGhostShadowData() : GetMFGhostShadowData();
    }

    private GhostShadowData GetSMRGhostShadowData()
    {
        GhostShadowData gsd;
        if (_freeSMRQueue.Count > 0)
        {
            gsd = _freeSMRQueue.Dequeue();
            if (gsd.m_Trans == null)
            {
                gsd.Push();
                return null;
            }
            else
                CombatManager.Instance.SetLayerByStyle(gsd.m_Trans.gameObject);
        }
        else
        {
            gsd = CombatObjectPool.Instance.Get<GhostShadowData>();
            GameObject go = new GameObject();
            //go.hideFlags = HideFlags.HideAndDontSave;
            gsd.m_Trans = go.transform;
            gsd.m_Trans.SetParent(CombatManager.Instance.m_WorkStreamTrans);
            gsd.m_Mesh = new Mesh();
            gsd.m_MeshFilter = go.AddComponent<MeshFilter>();
            gsd.m_MeshFilter.mesh = gsd.m_Mesh;
            gsd.m_MeshRenderer = go.AddComponent<MeshRenderer>();
            CombatManager.Instance.SetLayerByStyle(go);
        }

        if (gsd.m_IsNotFree)
            DebugUtil.LogError($"GetSMRGhostShadowData获取的没有被回收过");
        gsd.m_IsNotFree = true;

        gsd.m_IsSMR = true;
        
        return gsd;
    }

    private GhostShadowData GetMFGhostShadowData()
    {
        GhostShadowData gsd;
        if (_freeMFQueue.Count > 0)
        {
            gsd = _freeMFQueue.Dequeue();
            if (gsd.m_Trans == null)
            {
                gsd.Push();
                return null;
            }
            else
                CombatManager.Instance.SetLayerByStyle(gsd.m_Trans.gameObject);
        }
        else
        {
            gsd = CombatObjectPool.Instance.Get<GhostShadowData>();
            GameObject go = new GameObject();
            //go.hideFlags = HideFlags.HideAndDontSave;
            gsd.m_Trans = go.transform;
            gsd.m_Trans.SetParent(CombatManager.Instance.m_WorkStreamTrans);
            gsd.m_MeshFilter = go.AddComponent<MeshFilter>();
            gsd.m_MeshRenderer = go.AddComponent<MeshRenderer>();
            CombatManager.Instance.SetLayerByStyle(go);
        }

        if (gsd.m_IsNotFree)
            DebugUtil.LogError($"GetMFGhostShadowData获取的没有被回收过");
        gsd.m_IsNotFree = true;

        gsd.m_IsSMR = false;
        
        return gsd;
    }

    public void Push(GhostShadowData gsd, bool isSMR)
    {
        if (!gsd.m_IsNotFree)
            DebugUtil.LogError($"Push的已经被回收过    isSMR:{isSMR.ToString()}");
        gsd.m_IsNotFree = false;

        gsd.m_Trans.position = new Vector3(5000f, 5000f, 5000f);

        if (gsd.m_GhostStyle == 0 && gsd.m_MeshRenderer != null)
        {
            Object.DestroyImmediate(gsd.m_MeshRenderer.material);
        }

        if (isSMR)
            _freeSMRQueue.Enqueue(gsd);
        else
            _freeMFQueue.Enqueue(gsd);
    }
}
