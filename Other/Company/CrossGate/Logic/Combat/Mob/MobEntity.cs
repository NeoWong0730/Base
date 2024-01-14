using Lib.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class MobEntity : AEntity
{
    public GameObject m_Go;
    public Transform m_Trans;

    public Dictionary<uint, Transform> m_BindGameObjectDic;

    private MeshFilter[] _meshFilters;
    public MeshFilter[] MeshFilters
    {
        get
        {
            if (m_Go == null)
                return null;

            if (_meshFilters == null)
                _meshFilters = m_Go.GetComponentsInChildren<MeshFilter>();

            return _meshFilters;
        }
    }

    private SkinnedMeshRenderer[] _skinnedMeshRenderers;
    public SkinnedMeshRenderer[] SkinnedMeshRenderers
    {
        get
        {
            if (m_Go == null)
                return null;

            if (_skinnedMeshRenderers == null)
                _skinnedMeshRenderers = m_Go.GetComponentsInChildren<SkinnedMeshRenderer>();

            return _skinnedMeshRenderers;
        }
    }

    public MobCombatComponent m_MobCombatComponent;
    public MobSelectComponent m_MobSelectComponent;
       
    public void Init(BattleUnit unit, AnimationComponent animationComponent, GameObject go, uint weaponId, bool isCalling)
    {
        m_Go = go;
        m_Trans = go.transform;

        if(m_BindGameObjectDic == null)
            m_BindGameObjectDic = new Dictionary<uint, Transform>();
        CombatHelp.GetGameObjectBinds(m_Trans, m_BindGameObjectDic);

        m_MobCombatComponent = GetNeedComponent<MobCombatComponent>();
        m_MobCombatComponent.Init(unit, animationComponent, weaponId, isCalling);

        m_MobSelectComponent = GetNeedComponent<MobSelectComponent>();
        m_MobSelectComponent.Init(m_Go);
    }

    public void StartEnterScene()
    {
        m_MobCombatComponent.StartEnterScene();

        if (!Net_Combat.Instance.m_IsReconnect)
        {
            GetNeedComponent<MobEnterSceneComponent>().StartEnterScene(m_MobCombatComponent.m_IsCalling);
        }
    }

    public void Clear(bool removeFromManager = true)
    {
        if (removeFromManager)
            MobManager.Instance.RemoveMob(m_MobCombatComponent.m_BattleUnit.UnitId);

        Dispose();

        m_MobCombatComponent = null;
        m_MobSelectComponent = null;

        if (m_BindGameObjectDic != null)
            m_BindGameObjectDic.Clear();
        if (m_Trans != null)
        {
            m_Trans.position = CombatHelp.FarV3;
            m_Trans = null;
        }
        m_Go = null;

        _meshFilters = null;
        _skinnedMeshRenderers = null;
    }

    public void CacheProcessBuffChange(BattleBuffChange bc, bool isDelayUpdate = false)
    {
        MobBuffComponent mobBuffComponent = GetNeedComponent<MobBuffComponent>();
        mobBuffComponent.CacheProcessBuffChange(bc, isDelayUpdate);
    }

    public void DoProcessBuffChange(bool isForceUpdate = false)
    {
        MobBuffComponent mobBuffComponent = GetNeedComponent<MobBuffComponent>();
        mobBuffComponent.DoProcessBuffChange(isForceUpdate);
    }

    public void UpdateBuffComponentByBehave(BattleBuffChange bc)
    {
        MobBuffComponent mobBuffComponent = GetNeedComponent<MobBuffComponent>();
        mobBuffComponent.UpdateBuffChangeByBehave(bc);
    }

    public void UpdateBuffComponent(BattleBuffChange bc, bool isDelayUpdate)
    {
        DLogManager.Log(ELogType.eCombat, $"buff---{(m_Go == null ? null : m_Go.name)}-----执行buff UpdateBuffChange处理----<color=yellow>isDelayUpdate:{isDelayUpdate}   UnitId : {bc.UnitId.ToString()}  BuffId : {bc.BuffId.ToString()}  Num : {bc.OddNum.ToString()}   bc.CurHp:{bc.CurHp.ToString()}  bc.MaxHp:{bc.MaxHp.ToString()}  bc.CurMp:{bc.CurMp.ToString()}  bc.MaxMp:{bc.MaxMp.ToString()}</color>");

        MobBuffComponent mobBuffComponent = GetNeedComponent<MobBuffComponent>();
        mobBuffComponent.UpdateBuffChange(bc, isDelayUpdate);
    }

    public void UpdateBuffComponent(uint buffId, uint count, uint overlay = 0u,
        uint maxHp = 0u, uint curHp = 0u, uint maxMp = 0u, uint curMp = 0u, uint maxShield = 0u, uint curShield = 0u)
    {
        DLogManager.Log(ELogType.eCombat, $"buff---{(m_Go == null ? null : m_Go.name)}-----UpdateBuff处理----<color=yellow>BuffId : {buffId.ToString()}  count : {count.ToString()}   bc.CurHp:{curHp.ToString()}  bc.MaxHp:{maxHp.ToString()}  bc.CurMp:{curMp.ToString()}  bc.MaxMp:{maxMp.ToString()}</color>");

        MobBuffComponent mobBuffComponent = GetNeedComponent<MobBuffComponent>();
        mobBuffComponent.UpdateBuff(buffId, count, overlay, maxHp, curHp, maxMp, curMp, maxShield, curShield);
    }
}
