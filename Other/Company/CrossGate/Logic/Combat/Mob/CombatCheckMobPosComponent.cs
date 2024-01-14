using Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 0.5秒之后检测Mob离战斗中心点是否大于15，用以检测是否被NavMesh拉走
/// </summary>
public class CombatCheckMobPosComponent : BaseComponent<MobEntity>, IAwake, IUpdate
{
    private Vector3 _resetPos;
    private float _time;

    private int _frameCount;

    private bool _isNeedOffset;

    public void Awake()
    {
        _resetPos = m_CurUseEntity.m_MobCombatComponent.m_OriginPos;

        _time = 0f;

        _frameCount = 0;

        _isNeedOffset = true;
    }

    public void SetResetPos(Vector3 pos, bool isNeedOffset)
    {
        if (m_CurUseEntity.m_MobCombatComponent != null && m_CurUseEntity.m_MobCombatComponent.m_DeathType == 1)
        {
            Dispose();
            return;
        }

        _resetPos = pos;

        _isNeedOffset = isNeedOffset;
    }

    public void Update()
    {
        if (m_CurUseEntity.m_MobCombatComponent != null && m_CurUseEntity.m_MobCombatComponent.m_DeathType == 1)
        {
            Dispose();
            return;
        }

//#if DEBUG_MODE
//        if ((_frameCount < 5 && _time > 0.4f) || _frameCount == 5)
//        {
//            Vector3 pos2 = CombatManager.Instance.CombatSceneCenterPos - m_CurUseEntity.m_Trans.position;
//            if (CombatHelp.SimulateDot(pos2, pos2) > 225)
//            {
//                DLogManager.Log(Lib.Core.ELogType.eCombat, $"[{Time.frameCount.ToString()}][{Time.realtimeSinceStartup.ToString()}]--------Mob：{m_CurUseEntity.m_Go.name}位置：{m_CurUseEntity.m_Trans.position.ToString()}----{GameCenter.mainHero?.movementComponent?.mNavMeshAgent.enabled.ToString()}");
//            }

//            m_CurUseEntity.m_Trans.position = _resetPos;

//            DLogManager.Log(Lib.Core.ELogType.eCombat, $"CombatCheckMobPosComponent 01   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");
//        }
//#endif

        if (_time > 0.5f && m_CurUseEntity.m_Trans != null)
        {
            Vector3 pos = CombatManager.Instance.CombatSceneCenterPos - m_CurUseEntity.m_Trans.position;
            if (CombatHelp.SimulateDot(pos, pos) > 324)
            {
                Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eCombat, $"<color=red>Mob：{m_CurUseEntity.m_Go.name}位置：{m_CurUseEntity.m_Trans.position.ToString()},中心位置：{CombatManager.Instance.CombatSceneCenterPos.ToString()}, 离中心点距离为：{Mathf.Sqrt(CombatHelp.SimulateDot(pos, pos)).ToString()}，大于15</color>");
                if (m_CurUseEntity.m_MobCombatComponent.m_NotSetTransPos)
                {
                    if (_isNeedOffset)
                    {
                        float mobPosY = _resetPos.y;
                        _resetPos -= m_CurUseEntity.m_Trans.forward * 15f;
                        _resetPos.y = mobPosY;
                        m_CurUseEntity.m_Trans.position = _resetPos;
                    }
                    else
                        m_CurUseEntity.m_Trans.position = _resetPos;

                    Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eCombat, $"<color=red>Mob：{m_CurUseEntity.m_Go.name} 被强制拉到：{_resetPos.ToString()}</color>");
                }
            }
            Dispose();
            return;
        }

        _time += Time.deltaTime;
        _frameCount++;
    }
}
