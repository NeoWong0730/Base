#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatDrawGizmosTest : MonoBehaviour
{
    [Header("---------获取World空间到View空间的矩阵---------")]
    public bool IsGetW2C;

    [Space(15f)]
    public Transform m_Trans;

    [Header("---------轨迹调试---------")]
    private TrackEntity _trackEntity;
    public TrackType TrackType;
    public bool Init = true;
    public Vector3 OriginPos;
    public bool ReDoIt = true;
    public bool IsPause = true;
    public float m_SpeedX;
    public float m_Gx;
    public float m_SpeedY;
    public float m_Gy;
    public float m_TotalTime;
    public Vector3 TargetPos;

    public void OnDrawGizmos()
    {
        ObjectEvents.Instance.OnDrawGizmos();
        
        float y = CombatManager.Instance.CombatSceneCenterPos.y;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ), 0.5f);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ), 0.5f);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ), 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ), 0.5f);

        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ));

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_RightUpPosX, y, CombatManager.Instance.m_RightUpPosZ),
            new Vector3(CombatManager.Instance.m_RightUpPosX + CombatManager.Instance.m_Scene_U2D_AxisX, y, CombatManager.Instance.m_RightUpPosZ + CombatManager.Instance.m_Scene_U2D_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_RightDownPosX, y, CombatManager.Instance.m_RightDownPosZ),
            new Vector3(CombatManager.Instance.m_RightDownPosX + CombatManager.Instance.m_Scene_R2L_AxisX, y, CombatManager.Instance.m_RightDownPosZ + CombatManager.Instance.m_Scene_R2L_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftDownPosX, y, CombatManager.Instance.m_LeftDownPosZ),
            new Vector3(CombatManager.Instance.m_LeftDownPosX + CombatManager.Instance.m_Scene_D2U_AxisX, y, CombatManager.Instance.m_LeftDownPosZ + CombatManager.Instance.m_Scene_D2U_AxisZ));
        UnityEditor.Handles.DrawPolyLine(new Vector3(CombatManager.Instance.m_LeftUpPosX, y, CombatManager.Instance.m_LeftUpPosZ),
            new Vector3(CombatManager.Instance.m_LeftUpPosX + CombatManager.Instance.m_Scene_L2R_AxisX, y, CombatManager.Instance.m_LeftUpPosZ + CombatManager.Instance.m_Scene_L2R_AxisZ));

        UnityEditor.Handles.color = Color.white;
    }

    private void Update()
    {
        if (IsGetW2C)
        {
            IsGetW2C = false;

            CombatManager.Instance.OnTransData();

            CombatManager.Instance.PosFollowSceneCamera = true;
            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                kv.Value.m_MobCombatComponent.Init(kv.Value.m_MobCombatComponent.m_BattleUnit, kv.Value.m_MobCombatComponent.m_AnimationComponent, kv.Value.m_MobCombatComponent.m_WeaponId);
            }
        }
        
        Transform trans = m_Trans == null ? transform : m_Trans;
        if (Init)
        {
            Init = false;

            OriginPos = trans.position;
        }

        if (ReDoIt)
        {
            ReDoIt = false;

            transform.position = OriginPos;

            if (_trackEntity == null)
                _trackEntity = EntityFactory.Create<TrackEntity>();

            if (TrackType == TrackType.FlyParabola_25D)
            {
                FlyParabolaBhComp_25D flyParabolaBhComp_25D = _trackEntity.GetNeedComponent<FlyParabolaBhComp_25D>();
                float total = 1000f;
                if (m_TotalTime > 0f)
                {
                    total = m_TotalTime;
                    flyParabolaBhComp_25D.m_TrackOverType = 0;
                }
                else
                {
                    flyParabolaBhComp_25D.m_TrackOverType = 1;
                }

                flyParabolaBhComp_25D.Init(trans, m_SpeedX, m_Gx, m_SpeedY, m_Gy, total);
            }
            else if (TrackType == TrackType.FlyLine)
            {
                FlyLineComponent flyLineComponent = _trackEntity.GetNeedComponent<FlyLineComponent>();
                flyLineComponent.Init(m_SpeedX, m_Gx, trans, true, 0f, TargetPos);
            }
        }

        if (_trackEntity != null)
        {
            if (_trackEntity.Id == 0)
            {
                _trackEntity = null;
                return;
            }

            if (TrackType == TrackType.FlyParabola_25D)
            {
                FlyParabolaBhComp_25D flyParabolaBhComp_25D = _trackEntity.GetComponent<FlyParabolaBhComp_25D>();
                if (flyParabolaBhComp_25D != null)
                {
                    flyParabolaBhComp_25D.IsPauseTrack = IsPause;
                    if (IsPause)
                    {
                        float total = 1000f;
                        if (m_TotalTime > 0f)
                        {
                            total = m_TotalTime;
                            flyParabolaBhComp_25D.m_TrackOverType = 0;
                        }
                        else
                        {
                            flyParabolaBhComp_25D.m_TrackOverType = 1;
                        }

                        flyParabolaBhComp_25D.Init(trans, m_SpeedX, m_Gx, m_SpeedY, m_Gy, total);
                    }
                }
            }
            else if (TrackType == TrackType.FlyLine)
            {
                FlyLineComponent flyLineComponent = _trackEntity.GetComponent<FlyLineComponent>();
                if (flyLineComponent != null)
                {
                    flyLineComponent.IsPauseTrack = IsPause;
                    if (IsPause)
                    {
                        flyLineComponent.Init(m_SpeedX, m_Gx, trans, true, 0f, TargetPos);
                    }
                }
            }
        }
    }
}
#endif