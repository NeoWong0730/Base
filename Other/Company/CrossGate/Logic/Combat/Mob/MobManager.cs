using Lib.Core;
using Logic;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum CheckMobUnitServerStateEnum
{
    None = 0,
    UnitId = 1,
    Pos = 1 << 1,
}

public class MobManager : Logic.Singleton<MobManager>
{
    public Dictionary<uint, MobEntity> m_MobDic;
    
    public void OnAwake()
    {
        if (m_MobDic == null)
            m_MobDic = new Dictionary<uint, MobEntity>();
    }

    public void OnEnable()
    {
        ClearAll();
    }
    
    public void Dispose()
    {
        ClearAll();
    }

    public void ClearAll()
    {
        if (null == m_MobDic)
            return;

        if (m_MobDic.Count > 0)
        {
            foreach (var kv in m_MobDic)
            {
                kv.Value.Clear(false);
            }
            m_MobDic.Clear();
        }
    }
    
    public MobEntity AddMob(BattleUnit unit, AnimationComponent animationComponent, GameObject go, uint weaponId, bool isCalling = false)
    {
        CombatManager.Instance.OnTransData();

        ClearMobByCheckUnitState(unit, CheckMobUnitServerStateEnum.UnitId | CheckMobUnitServerStateEnum.Pos);

        UnityEngine.AI.NavMeshAgent[] nmas = go.GetComponentsInChildren<UnityEngine.AI.NavMeshAgent>();
        foreach (var item in nmas)
        {
            if (item.enabled && item.gameObject.activeInHierarchy)
            {
                DebugUtil.LogError($"进入战斗时：{go.name}下的NavMeshAgent组件是开启的，有问题，强制关闭");
                item.enabled = false;
            }
        }

        MobEntity mobEntity = EntityFactory.Create<MobEntity>();

        m_MobDic[unit.UnitId] = mobEntity;

        mobEntity.Init(unit, animationComponent, go, weaponId, isCalling);
        if (CombatManager.Instance.m_IsLoadMobsOver)
        {
            mobEntity.StartEnterScene();

            DLogManager.Log(ELogType.eCombat, $"AddMob 01   {mobEntity.m_Trans.name}--- {mobEntity.m_Trans.position}");
        }

        if (!Net_Combat.Instance.m_IsReconnect && CombatManager.Instance.m_CombatStyleState == 0 &&
            CombatManager.Instance.m_IsRunEnterScene && !Net_Combat.Instance.m_IsVideo)
        {
            Vector3 mobPos = mobEntity.m_Trans.position;
            float mobPosY = mobPos.y;
            mobPos -= mobEntity.m_Trans.forward * 15f;
            mobPos.y = mobPosY;
            mobEntity.m_Trans.position = mobPos;

            mobEntity.m_MobCombatComponent.m_NotSetTransPos = true;
            mobEntity.m_MobCombatComponent.OnShowOrHideBD(false);

            DLogManager.Log(ELogType.eCombat, $"AddMob 02   {mobEntity.m_Trans.name}--- {mobEntity.m_Trans.position}");
        }

        if (Sys_Fight.Instance.GetServerCombatUnitCount() == m_MobDic.Count)
        {
            DLogManager.Log(ELogType.eCombat, $"Load Mob Over   {mobEntity.m_Trans.name}--- {mobEntity.m_Trans.position}");
            
            mobEntity.m_MobCombatComponent.m_NotSetTransPos = false;
            CombatManager.Instance.LoadMobsOver(3u, () => 
            {
                CombatManager.Instance.OnInitCombatData();

                if (Net_Combat.Instance.m_IsVideo)
                {
                    Net_Combat.Instance.PlayRoundVideo(1u);
                }
                else
                {
                    foreach (var kv in m_MobDic)
                    {
                        if (kv.Value == null)
                            continue;

                        kv.Value.StartEnterScene();

                        DLogManager.Log(ELogType.eCombat, $"LoadMobsOver 04   {kv.Value.m_Trans.name}--- {kv.Value.m_Trans.position}");
                    }
                }
            });

            Net_Combat.Instance.m_IsWaitDoExcute = false;
            if (Net_Combat.Instance.m_IsReconnect)
            {
                Net_Combat.Instance.DoDelayRoundData(false);
            }

            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnLoadMobsOver);
        }

        return mobEntity;
    }

    public MobEntity GetMob(uint unitId)
    {
        MobEntity mobEntity;
        m_MobDic.TryGetValue(unitId, out mobEntity);

        return mobEntity;
    }

    public void RemoveMob(uint unitId)
    {
        m_MobDic.Remove(unitId);
    }
    
    public MobEntity GetMobByServerNum(int serverNum) 
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            if (kv.Value.m_MobCombatComponent.m_BattleUnit.Pos == serverNum)
                return kv.Value;
        }

        return null;
    }

    public MobEntity GetMobByGameObject(GameObject go)
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            if (kv.Value.m_Go == go)
                return kv.Value;
        }

        return null;
    }

    public MobEntity GetMobByClientNum(int clientNum)
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            if (kv.Value.m_MobCombatComponent.m_ClientNum == clientNum)
                return kv.Value;
        }

        return null;
    }

    public MobEntity GetPlayerMob()
    {
        if (Sys_Role.Instance == null || Sys_Role.Instance.Role == null)
            return null;

        foreach (var kv in m_MobDic)
        {
            MobEntity me = kv.Value;
            if (me == null || me.m_MobCombatComponent == null)
                continue;

            var battleUnit = me.m_MobCombatComponent.m_BattleUnit;
            if (battleUnit == null)
                continue;

            if (battleUnit.UnitType == (uint)UnitType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                return me;
        }

        return null;
    }

    public BattleUnit GetPlayerBattleUnit()
    {
        if (Sys_Role.Instance == null || Sys_Role.Instance.Role == null)
            return null;

        foreach (var kv in m_MobDic)
        {
            MobEntity me = kv.Value;
            if (me == null || me.m_MobCombatComponent == null)
                continue;

            var battleUnit = me.m_MobCombatComponent.m_BattleUnit;
            if (battleUnit == null)
                continue;

            if (battleUnit.UnitType == (uint)UnitType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                return battleUnit;
        }

        return null;
    }
    
    public void ResetAllMobState()
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            if (kv.Value.m_MobCombatComponent != null)
            {
                kv.Value.m_MobCombatComponent.RefreshMobState();
            }
        }
    }

    public bool IsHaveBehaveMob()
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            if (kv.Value.m_MobCombatComponent != null && kv.Value.m_MobCombatComponent.m_IsStartBehave)
            {
                DLogManager.Log(ELogType.eCombat, $"检测------{kv.Value.m_MobCombatComponent.m_ClientNum.ToString()}号正在开始行为");
                return true;
            }
        }

        return false;
    }
    
    public void EndEnterSceneBehave()
    {
        foreach (var kv in m_MobDic)
        {
            if (kv.Value == null)
                continue;

            kv.Value.GetComponent<MobEnterSceneComponent>().Dispose();
        }
    }
    
    public void ResetMobsState(bool isForce = false)
    {
        if (m_MobDic == null)
            return;

        foreach (var kv in m_MobDic)
        {
            if (kv.Value != null && kv.Value.m_MobCombatComponent != null)
                kv.Value.m_MobCombatComponent.ResetMobState(false, -1, isForce);
        }
    }

    public Vector3 GetPosInClientNum(int clientNum)
    {
        CombatPosData posData = CombatConfigManager.Instance.GetCombatPosData(CombatManager.Instance.m_BattlePosType * 1000000 + 20000u + (uint)clientNum * 100u + 1u);
        if (posData == null)
        {
            Lib.Core.DebugUtil.LogError($"编辑数据不存在PosId：{(20000u + clientNum * 100u + 1u).ToString()}");
            return CombatManager.Instance.CombatSceneCenterPos;
        }

        if (CombatManager.Instance.PosFollowSceneCamera)
        {
            Vector3 pos = CombatManager.Instance.CombatSceneCenterPos + CombatManager.Instance.m_AdjustSceneViewAxiss[0] * posData.PosX + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * posData.PosZ;
            pos.y = CombatManager.Instance.CombatSceneCenterPos.y;
            return pos;
        }
        else
        {
            Vector3 pos = CombatManager.Instance.CombatSceneCenterPos + new Vector3(posData.PosX, 0f, posData.PosZ);
            return pos;
        }
    }
    
    private Queue<MobEntity> _existMobEntityQueue = null;
    public void ClearMobByCheckUnitState(BattleUnit battleUnit, CheckMobUnitServerStateEnum checkMobUnitServerStateEnum)
    {
        if (_existMobEntityQueue != null && _existMobEntityQueue.Count > 0)
        {
            _existMobEntityQueue.Clear();
        }

        foreach (var kv in m_MobDic)
        {
            var mob = kv.Value;
            if (mob == null || mob.m_MobCombatComponent == null)
                continue;

            var bu = mob.m_MobCombatComponent.m_BattleUnit;
            if (bu == null)
                continue;

            if (((checkMobUnitServerStateEnum & CheckMobUnitServerStateEnum.UnitId) > 0 && bu.UnitId == battleUnit.UnitId) ||
                ((checkMobUnitServerStateEnum & CheckMobUnitServerStateEnum.Pos) > 0 && bu.Pos == battleUnit.Pos))
            {
                if (_existMobEntityQueue == null)
                    _existMobEntityQueue = new Queue<MobEntity>();

                _existMobEntityQueue.Enqueue(mob);
            }
        }

        if (_existMobEntityQueue != null)
        {
            while (_existMobEntityQueue.Count > 0)
            {
                var mob = _existMobEntityQueue.Dequeue();
                if (mob != null)
                {
#if DEBUG_MODE
                    MobBuffComponent mobBuffComponent = mob.GetComponent<MobBuffComponent>();
                    if (mobBuffComponent != null)
                    {
                        mobBuffComponent.m_DisposeCause = 1;
                    }
#endif
                    mob.Clear();
                }
            }
        }
    }

    public void StopMobsCombatState()
    {
        if (m_MobDic == null)
            return;

        foreach (var kv in m_MobDic)
        {
            var mob = kv.Value;
            if (mob != null)
            {
                WS_CombatBehaveAIManagerEntity cbame = mob.GetChildEntity<WS_CombatBehaveAIManagerEntity>();
                if (cbame != null)
                    cbame.StopAll();
            }
        }
    }

    public Transform GetPosByMobBind(uint battleUnitId, uint bindType)
    {
        if (m_MobDic.TryGetValue(battleUnitId, out MobEntity mobEntity) && mobEntity != null && mobEntity.m_BindGameObjectDic != null)
        {
            if (mobEntity.m_BindGameObjectDic.TryGetValue(bindType, out Transform bindTrans) && bindTrans != null)
                return bindTrans;
            else
                return mobEntity.m_Trans;
        }
        return null;
    }

    public void HideMobEntitys(int side = -999, bool isSameCamp = false)
    {
        if (null == m_MobDic)
            return;
        
        if (m_MobDic.Count > 0)
        {
            foreach (var kv in m_MobDic)
            {
                var mob = kv.Value;
                if (mob == null || mob.m_MobCombatComponent == null || mob.m_MobCombatComponent.m_BattleUnit == null)
                    continue;

                if (side == -999 || CombatHelp.IsSameCamp(mob.m_MobCombatComponent.m_BattleUnit, side) == isSameCamp)
                {
                    WS_CombatBehaveAIManagerEntity cbame = mob.GetChildEntity<WS_CombatBehaveAIManagerEntity>();
                    if (cbame != null)
                        cbame.StopAll();

                    mob.m_Trans.position = CombatHelp.FarV3;
                }
            }
        }
    }

    public bool IsPlayer(MobEntity mob)
    {
        if (Sys_Role.Instance == null || Sys_Role.Instance.Role == null)
            return false;

        if (mob == null || mob.m_MobCombatComponent == null)
            return false;

        var battleUnit = mob.m_MobCombatComponent.m_BattleUnit;
        if (battleUnit == null)
            return false;

        if (battleUnit.UnitType == (uint)UnitType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
            return true;

        return false;
    }

    public bool IsPlayer(BattleUnit battleUnit)
    {
        if (Sys_Role.Instance == null || Sys_Role.Instance.Role == null)
            return false;

        if (battleUnit.UnitType == (uint)UnitType.Hero && battleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
            return true;

        return false;
    }

    public BattleUnit GetBeWatchBattleUnit()
    {
        if (Net_Combat.Instance.m_BeWatchRoleId == 0ul)
            return null;

        foreach (var kv in m_MobDic)
        {
            var mob = kv.Value;
            if (mob == null || mob.m_MobCombatComponent == null || mob.m_MobCombatComponent.m_BattleUnit == null)
                continue;

            if (mob.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero &&
                mob.m_MobCombatComponent.m_BattleUnit.RoleId == Net_Combat.Instance.m_BeWatchRoleId)
            {
                return mob.m_MobCombatComponent.m_BattleUnit;
            }
        }

        DebugUtil.LogError($"没有获取到被观战者RoleId：{Net_Combat.Instance.m_BeWatchRoleId.ToString()}的信息");
        return null;
    }
}
