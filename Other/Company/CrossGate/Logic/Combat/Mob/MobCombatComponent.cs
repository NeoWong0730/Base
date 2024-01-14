using Lib.Core;
using Logic;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class CombatHpChangeData
{
    public int m_CurHp;
    public int m_HpChange;  //>0加血，<0扣血
    public int m_CurMp;
    public int m_MpChange;
    public int m_CurShield;
    public int m_ShieldChange;
    public int m_CurGas;
    public int m_GasChange;
    public AnimType m_AnimType;
    public bool m_Death;
    public bool m_Revive;
    public int m_damageAddon;
    public int m_ConverAttackCount = 1;//合击数量
    /// <summary>
    /// 未变化-1; 血变动0，魔法变动 1 ；都变化2，护盾变化3，血护盾都变4
    /// </summary>
    public int m_ChangeType;
    public uint m_ExtendId;
    public uint m_ExtendType;
    public uint m_BuffSkillId;
    public int m_ExcuteDataIndex;
    public uint m_ExtraHitEffect;
    public uint m_SrcUnitId;
    public int m_UnitChangePos;
    /// <summary>
    /// 飞弹击中后执行buff引起的hpchange时标记是否已被使用
    /// </summary>
    public bool m_IsBeUse;
    
    public static void Push(CombatHpChangeData combatHpChangeData)
    {
        if (combatHpChangeData == null)
            return;

        combatHpChangeData.m_CurHp = 0;
        combatHpChangeData.m_HpChange = 0;
        combatHpChangeData.m_CurMp = 0;
        combatHpChangeData.m_MpChange = 0;
        combatHpChangeData.m_CurShield = 0;
        combatHpChangeData.m_ShieldChange = 0;
        combatHpChangeData.m_CurGas = 0;
        combatHpChangeData.m_GasChange = 0;
        combatHpChangeData.m_AnimType = AnimType.e_None;
        combatHpChangeData.m_Death = false;
        combatHpChangeData.m_Revive = false;
        combatHpChangeData.m_damageAddon = 0;
        combatHpChangeData.m_ConverAttackCount = 1;
        combatHpChangeData.m_ChangeType = -1;
        combatHpChangeData.m_ExtendId = 0u;
        combatHpChangeData.m_ExtendType = 0u;
        combatHpChangeData.m_BuffSkillId = 0u;
        combatHpChangeData.m_ExcuteDataIndex = -1;
        combatHpChangeData.m_ExtraHitEffect = 0u;
        combatHpChangeData.m_SrcUnitId = 0u;
        combatHpChangeData.m_UnitChangePos = -1;
        combatHpChangeData.m_IsBeUse = false;

        CombatObjectPool.Instance.Push(combatHpChangeData);
    }
}

/// <summary>
/// 战斗单位状态
/// </summary>
[Flags]
public enum CombatUnitState
{
    None = 0,
    EnterState = 1, //进场状态
    CombatState = 1 << 1,    //战斗状态
    AttachmentState = 1 << 2,    //附身状态

    BeUsedSkillFail = 1 << 3, //被使用技能失败

    UnitPosChange = 1 << 4,  //换位

    LeaveState = 1 << 10, //离场状态

    DelSuccess = 1 << 11, //删除单位成功
    DelFail = 1 << 12,    //删除单位失败
    EscapeSuccess = 1 << 13, //逃跑成功
    EscapeFail = 1 << 14, //逃跑失败

    Death = 1 << 15,  //死亡

    FarDeath = 1 << 16,  //死远
    FallDeath = 1 << 17,  //倒地死
    BeHitFlyDeath = 1 << 18,  //击飞死
}

public class MobCombatComponent : BaseComponent<MobEntity>
{
    public int m_ClientNum;
    public BattleUnit m_BattleUnit;
    public bool m_Death;
    /// <summary>
    /// =0正常情况, =1已经设置为死出去了，=2已经设置为倒地死
    /// </summary>
    public int m_DeathType;
    public AnimationComponent m_AnimationComponent;
    public uint m_WeaponId;
    public Vector3 m_OriginPos;
    public Vector3 m_eulerAngles;

    public Queue<CombatHpChangeData> m_HpChangeDataQueue;
    public bool m_IsSetHpChange;
    
    public int m_ExcuteIndex = -1;

    private int _behaveCount;
    public bool m_IsStartBehave
    {
        get
        {
            return _behaveCount > 0;
        }
    }

    public int m_ReadlyBehaveCount;
    
    public string m_CurAnimationName;

    public bool m_BeHitToFlyState;

    public bool m_IsRunAway;

    public bool m_BeDelUnit;

    public CombatUnitState m_CombatUnitState;

    public bool m_IsCalling;
    
    public bool m_NotSetTransPos;

    public bool m_IsDoProtected;

    public List<dressData> m_DressDataList;

    private uint _curRecordRound;
    private uint _curRecordRoundCount;
    
#if DEBUG_MODE
    public bool m_isHaveChildLayerActive;
#endif

    public void Init(BattleUnit unit, AnimationComponent animationComponent, uint weaponId, bool isCalling = false)
    {
        if (m_HpChangeDataQueue == null)
            m_HpChangeDataQueue = new Queue<CombatHpChangeData>();
        
        m_DeathType = 0;

        m_BattleUnit = unit;
        if (unit.RoleId == Sys_Role.Instance.Role.RoleId && (UnitType)unit.UnitType == UnitType.Hero)
        {
            Net_Combat.Instance.m_IsAttackSide = CombatHelp.GetServerCampSide(unit.Pos) == 0;
        }

        DLogManager.Log(ELogType.eCombat, $"MobCombatComponent.Init----<color=yellow>{m_CurUseEntity.m_Trans.name}  UnitId:{m_BattleUnit.UnitId.ToString()}  ServerPos:{m_BattleUnit.Pos.ToString()}  MaxHp:{m_BattleUnit.MaxHp.ToString()}  CurHp:{m_BattleUnit.CurHp.ToString()}  MaxMp:{m_BattleUnit.MaxMp.ToString()}   CurMp:{m_BattleUnit.CurMp.ToString()}</color>");

        m_ClientNum = CombatHelp.ServerToClientNum(unit.Pos, CombatManager.Instance.m_IsNotMirrorPos);

        m_AnimationComponent = animationComponent;
        m_WeaponId = weaponId;
        m_IsCalling = isCalling;

        if (!RefreshPos())
            return;

        m_CombatUnitState = CombatUnitState.CombatState;

        //创建UI_Hub
        Net_Combat.Instance.CreateBloodHub(unit, m_ClientNum, m_CurUseEntity.m_Go);

        //设置数据状态
        if (Net_Combat.Instance.m_IsReconnect)
            ResetMobState(false);
        Net_Combat.Instance.UpdateHp(unit, unit.CurHp, unit.MaxHp);
        Net_Combat.Instance.UpdateMp(unit, unit.CurMp, unit.MaxMp);
        Net_Combat.Instance.UpdateShield(unit, unit.CurShield, unit.MaxShield);
        Net_Combat.Instance.UpdateGas(unit, unit.CurGas, unit.MaxGas);
        Net_Combat.Instance.UpdateBuff(m_CurUseEntity, unit);

        GetNeedComponent<CombatCheckMobPosComponent>();

        m_ExcuteIndex = -1;
        //特殊战斗生成血条
        if ((UnitType)unit.UnitType == UnitType.Monster && CombatManager.Instance.m_BattleTypeTb.show_UI_hp)
        {
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnUpdateBossBlood, unit, unit.CurHp, unit.MaxHp);
        }

        DLogManager.Log(ELogType.eCombat, $"Init 09   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");
    }

    public void StartEnterScene()
    {
        DLogManager.Log(ELogType.eCombat, $"StartEnterScene 06   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");

        if (m_DeathType == 1)
            return;

        DLogManager.Log(ELogType.eCombat, $"StartEnterScene 07   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");

        if (!RefreshPos())
            return;

        GetNeedComponent<CombatCheckMobPosComponent>().SetResetPos(m_OriginPos, true);

        DLogManager.Log(ELogType.eCombat, $"StartEnterScene 08   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");
    }

    public bool RefreshPos(bool isSetTransPos = true, bool isCheckMobPos = true)
    {
        CombatPosData posData;
        if (!CombatManager.Instance.GetPosByClientNum(m_ClientNum, out posData, ref m_OriginPos))
            return false;

        if (isSetTransPos && m_CurUseEntity.m_Trans != null)
            m_CurUseEntity.m_Trans.position = m_OriginPos;

        m_eulerAngles = new Vector3(posData.AngleX, posData.AngleY, posData.AngleZ);
        if (m_CurUseEntity.m_Trans != null)
        {
            m_CurUseEntity.m_Trans.eulerAngles = m_eulerAngles;
            if (CombatManager.Instance.PosFollowSceneCamera)
            {
                m_CurUseEntity.m_Trans.rotation = Quaternion.LookRotation(CombatManager.Instance.TransC2W_MultiplyVector(m_CurUseEntity.m_Trans.forward));
                m_eulerAngles = m_CurUseEntity.m_Trans.eulerAngles;
            }
        }
        
        if (isCheckMobPos)
            GetNeedComponent<CombatCheckMobPosComponent>().Awake();

        DLogManager.Log(ELogType.eCombat, $"RefreshPos 05   {m_CurUseEntity.m_Trans.name}--- {m_CurUseEntity.m_Trans.position}");

        return true;
    }

    public void SetBattleUnit(BattleUnit unit, uint weaponId = 0u)
    {
        Init(unit, m_AnimationComponent, weaponId);

        m_AnimationComponent?.UpdateHoldingAnimations(m_BattleUnit.UnitInfoId, m_WeaponId);
    }

    public override void Dispose()
    {
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnRemoveBattleUnit, m_BattleUnit.UnitId);
        
        m_BattleUnit = null;
        if (m_AnimationComponent != null)
        {
            m_AnimationComponent.StopAll();
            m_AnimationComponent = null;
        }
        m_ExcuteIndex = -1;
        _behaveCount = 0;
        m_ReadlyBehaveCount = 0;
        m_Death = false;
        if (m_HpChangeDataQueue != null)
        {
            while (m_HpChangeDataQueue.Count > 0)
            {
                CombatHpChangeData.Push(m_HpChangeDataQueue.Dequeue());
            }
        }
        m_IsSetHpChange = false;
        m_CurAnimationName = string.Empty;

        m_BeHitToFlyState = false;

        m_IsRunAway = false;

        m_BeDelUnit = false;

        _curRecordRound = 0;
        _curRecordRoundCount = 0;

        m_CombatUnitState = CombatUnitState.LeaveState;
        
        m_DeathType = 0;

        m_NotSetTransPos = false;

        m_IsDoProtected = false;

        m_DressDataList = null;
        
        base.Dispose();
    }

    public void RefreshMobState()
    {
        StopBehave();

        if (m_BattleUnit != null)
        {
            Net_Combat.Instance.UpdateHp(m_BattleUnit, m_BattleUnit.CurHp, m_BattleUnit.MaxHp);
            Net_Combat.Instance.UpdateMp(m_BattleUnit, m_BattleUnit.CurMp, m_BattleUnit.MaxMp);
            Net_Combat.Instance.UpdateShield(m_BattleUnit, m_BattleUnit.CurShield, m_BattleUnit.MaxShield);
            Net_Combat.Instance.UpdateGas(m_BattleUnit, m_BattleUnit.CurGas, m_BattleUnit.MaxGas);
        }
    }
    
    public bool StartBehave(uint skillId, TurnBehaveSkillInfo turnBehaveSkillInfo, bool isConverAttack, 
        BehaveAIControllParam behaveAIControllParam, int attachType = 0, int blockType = 0)
    {
        CSVActiveSkill.Data skillTb = CSVActiveSkill.Instance.GetConfData(skillId);
        if (skillTb == null)
        {
            Lib.Core.DebugUtil.LogError($"CSVActiveSkillData表中没有skillId : {skillId.ToString()}");
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return false;
        }

        return DoBehave(skillTb, attachType, m_CurUseEntity, turnBehaveSkillInfo, behaveAIControllParam, blockType);
    }

    public bool DoBehave(CSVActiveSkill.Data skillTb, int attachType, MobEntity behaveDataSources, TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo, 
        BehaveAIControllParam behaveAIControllParam, int blockType = 0)
    {
        if (skillTb == null || m_CurUseEntity == null)
        {
            DebugUtil.LogError($"StartBehave中skillTb : {skillTb} m_CurUseEntity : {m_CurUseEntity}");
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return false;
        }

        if (behaveAIControllParam != null && behaveAIControllParam.ExcuteTurnIndex > -1)
            m_ExcuteIndex = behaveAIControllParam.ExcuteTurnIndex;

        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = m_CurUseEntity.GetNeedChildEntity<WS_CombatBehaveAIManagerEntity>();
        if (combatBehaveAIManagerEntity == null)
        {
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return false;
        }

        return combatBehaveAIManagerEntity.StartAI(skillTb, attachType, behaveDataSources, sourcesTurnBehaveSkillInfo, blockType, behaveAIControllParam);
    }

    public bool DoBehave(uint workId, CSVActiveSkill.Data skillTb, int attachType, TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo, 
        BehaveAIControllParam behaveAIControllParam, int blockType = 0)
    {
        if (behaveAIControllParam != null && behaveAIControllParam.ExcuteTurnIndex > -1)
            m_ExcuteIndex = behaveAIControllParam.ExcuteTurnIndex;

        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = m_CurUseEntity.GetNeedChildEntity<WS_CombatBehaveAIManagerEntity>();
        if (combatBehaveAIManagerEntity == null)
        {
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return false;
        }

        return combatBehaveAIManagerEntity.StartWorkId(workId, attachType, skillTb, sourcesTurnBehaveSkillInfo, behaveAIControllParam, blockType);
    }

    public void BehaveOver(CombatHpChangeData controllHpChangeData, bool isUpdateBuff = true)
    {
        UpdateBehaveCount(-1);

        if (_behaveCount == 0 && isUpdateBuff)
        {
            GetComponent<MobBuffComponent>()?.UpdateReadyBuffData();
        }
        
        bool beMultiAttackOver = true;
        if (m_ExcuteIndex > -1)
        {
            var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnInfo(m_ExcuteIndex);
            if (excuteTurnData != null && excuteTurnData.CombineAttack_SrcUnits != null && 
                excuteTurnData.CombineAttack_SrcUnits.Count > 1 && excuteTurnData.CombineAttack_ScrIndex < excuteTurnData.CombineAttack_SrcUnits.Count)
                beMultiAttackOver = false;
            else if(_behaveCount == 0)
                m_ExcuteIndex = -1;
        }

        //当该Mob还在被被击当中，又被重新定位目标，比如连射，有可能重置之前有好几发箭已经被射出，
        //那么重置的m_HpChangeDataQueue会把之前的箭算上，相当于有重置的箭数加上重置之前已发射的箭数。
        if (m_HpChangeDataQueue.Count == 0 && !m_IsStartBehave)
            ResetTrans();

        Net_Combat.Instance.m_RefreshDoTurn = true;
        if (m_BattleUnit != null && CombatManager.Instance.m_BattleTypeTb != null && CombatManager.Instance.m_BattleTypeTb.is_quickfight)
            Net_Combat.Instance.m_RecordCombatBehaveUnitList.Remove(m_BattleUnit.UnitId);
        
        if (controllHpChangeData != null)
        {
            if (beMultiAttackOver && controllHpChangeData.m_Death)
            {
                m_CurUseEntity.GetNeedComponent<MobDeadComponent>();
                //m_CurUseEntity.RemoveComponent<MobBuffComponent>();
            }
            CombatHpChangeData.Push(controllHpChangeData);
        }

        //m_IsRunAway = false;

        Net_Combat.Instance.CheckCancelBattle();
    }

    public void StopBehave(bool isSetHpMp = false, bool stopCurrentWorkStream = true)
    {
        //Debug.LogError($"{m_CurUseEntity.m_Go.name}     _behaveCount:{_behaveCount.ToString()}   StopBehave");

        if (m_ReadlyBehaveCount > 0)
        {
            Debug.LogError($"{m_CurUseEntity.m_Go.name}   还有准备行为的次数m_ReadlyBehaveCount:{m_ReadlyBehaveCount.ToString()}未执行  StopBehave");
        }

        _behaveCount = 0;
        m_ReadlyBehaveCount = 0;

        ClearCombatHpChangeDataQueue(isSetHpMp);

        m_CurUseEntity.GetComponent<MobBuffComponent>()?.CheckCacheBuff();

        if (stopCurrentWorkStream)
        {
            var cbame = m_CurUseEntity.GetChildEntity<WS_CombatBehaveAIManagerEntity>();
            if(cbame != null)
                cbame.StopAll();
        }
    }
    
    public void ResetTrans(bool isForce = false, bool ResetPosInDead = false)
    {
        if (m_CurUseEntity != null && m_CurUseEntity.m_Trans != null)
        {
            if (isForce || (!m_Death && !m_IsRunAway) || (m_Death && ResetPosInDead))
                m_CurUseEntity.m_Trans.position = m_OriginPos;
            if (isForce || (!m_Death && !m_IsRunAway))
                m_CurUseEntity.m_Trans.eulerAngles = m_eulerAngles;
        }
    }

    public void ResetPos()
    {
        m_CurUseEntity.m_Trans.position = m_OriginPos;
    }

    /// <summary>
    /// setType小于0不设置, =0删除单位成功，=1删除单位失败，=2逃跑成功，=3逃跑失败，=4死亡
    /// </summary>
    public void ResetMobState(bool isFar = false, int setType = -1, bool isForce = false)
    {
        if (m_CurUseEntity == null || m_CurUseEntity.m_Trans == null)
            return;
        
        m_Death = m_BattleUnit.CurHp <= 0;
        if (CheckMobCanNotFight())
        {
            if (m_IsRunAway || m_BeDelUnit)
                isFar = true;

            if (isForce || isFar || !Net_Combat.Instance.NeedDoExcuteTurn(m_BattleUnit.UnitId))
            {
                bool isSetState = true;
                if (Net_Combat.Instance.m_IsReconnect && Net_Combat.Instance.m_ReconnectRoundNtf != null &&
                    Net_Combat.Instance.m_ReconnectRoundNtf.Turns != null && Net_Combat.Instance.m_ReconnectRoundNtf.Turns.Count > 0)
                {
                    isSetState = !Net_Combat.Instance.IsExistInExcuteTurns(Net_Combat.Instance.m_ReconnectRoundNtf.Turns, delegate (uint unitId)
                                {
                                    return m_BattleUnit.UnitId == unitId;
                                });
                }

                if (isSetState)
                {
                    bool isRefreshDieAnimation = true;

                    if (isFar)
                    {
                        m_DeathType = 1;
                    }
                    else
                    {
                        if (m_BeHitToFlyState)
                        {
                            m_DeathType = 1;
                        }
                        else if (m_BattleUnit.UnitType == (uint)UnitType.Monster)
                        {
                            CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(m_BattleUnit.UnitInfoId);
                            if (cSVMonsterData == null || cSVMonsterData.dead_behavior == 1005u)
                            {
                                m_DeathType = 1;
                            }
                            else
                            {
                                m_DeathType = 2;
                            }

                            if (cSVMonsterData != null && cSVMonsterData.refresh_action == 1u)
                                isRefreshDieAnimation = false;
                        }
                        else
                        {
                            m_DeathType = 2;
                        }
                    }

                    if (m_DeathType == 1)
                    {
                        m_AnimationComponent.StopAll();

                        m_CurUseEntity.m_Trans.position = CombatHelp.FarV3;

                        if (setType < 0)
                            SetCombatUnitState(CombatUnitState.Death | CombatUnitState.FarDeath);
                    }
                    else if (m_DeathType == 2)
                    {
                        m_CurUseEntity.m_Trans.position = m_OriginPos;
                        m_CurUseEntity.m_Trans.eulerAngles = m_eulerAngles;

                        if (isRefreshDieAnimation)
                            PlaySuccessAnimation("action_die1", true, true);

                        m_CurUseEntity.GetNeedComponent<MobDeadComponent>();

                        if (setType < 0)
                            SetCombatUnitState(CombatUnitState.Death | CombatUnitState.FallDeath);
                    }
                }
            }
        }
        else
        {
            m_DeathType = 0;

            m_CurUseEntity.RemoveComponent<MobDeadComponent>();

            m_CurUseEntity.m_Trans.position = m_OriginPos;
            m_CurUseEntity.m_Trans.eulerAngles = m_eulerAngles;
            
            PlaySuccessAnimation("action_idle", true, true);

            if (setType < 0)
                SetCombatUnitState(CombatUnitState.CombatState);
        }

        if (setType > -1)
            SetCombatUnitState(setType);
    }

    public bool CheckMobCanNotFight()
    {
        return m_Death || m_IsRunAway || m_BeDelUnit;
    }
    
    public void SetMobReviveState()
    {
        StopBehave();

        m_Death = false;
        m_CombatUnitState = CombatUnitState.CombatState;
        MobDeadComponent mobDeadComponent = m_CurUseEntity.GetComponent<MobDeadComponent>();
        if (mobDeadComponent != null)
        {
            mobDeadComponent.Remove();
            Net_Combat.Instance.CreateBloodHub(m_BattleUnit, m_ClientNum, m_CurUseEntity.m_Go);
        }

        m_CurUseEntity.GetComponent<MobBuffComponent>()?.ClearAllBuffs();
        
        ResetTrans();
    }
    
    public void SetRoundBattleCount(uint curRound)
    {
        if (_curRecordRound != curRound)
        {
            _curRecordRoundCount = 1u;
            _curRecordRound = curRound;
        }
        else
            _curRecordRoundCount = _curRecordRoundCount + 1u;

        if (_curRecordRoundCount > 1u)
        {
            //行动次数大于1
            Sys_HUD.Instance.eventEmitter.Trigger<uint>(Sys_HUD.EEvents.OnTriggerSecondAction, m_BattleUnit.UnitId);
        }
    }

    /// <summary>
    /// setType小于0不设置, =0删除单位成功，=1删除单位失败，=2逃跑成功，=3逃跑失败，=4死亡, =5被使用技能失败, =7 换位
    /// </summary>
    public void SetCombatUnitState(int setType)
    {
        if (setType < 0)
            return;

        if (setType == 0)
            m_CombatUnitState = CombatUnitState.DelSuccess | CombatUnitState.Death | CombatUnitState.LeaveState;
        else if (setType == 1)
            m_CombatUnitState = CombatUnitState.DelFail | CombatUnitState.CombatState;
        else if (setType == 2)
            m_CombatUnitState = CombatUnitState.EscapeSuccess | CombatUnitState.Death | CombatUnitState.LeaveState;
        else if (setType == 3)
            m_CombatUnitState = CombatUnitState.EscapeFail | CombatUnitState.CombatState;
        else if (setType == 4)
        {
            if (m_Death)
                m_CombatUnitState = CombatUnitState.Death;
            else
                DebugUtil.LogError($"SetCombatUnitState设置setType:{setType.ToString()}   m_Death:{m_Death.ToString()}数据设置不对");
        }
        else if (setType == 5)
            m_CombatUnitState = CombatUnitState.BeUsedSkillFail | CombatUnitState.CombatState;
        else if (setType == 7)
            m_CombatUnitState = CombatUnitState.UnitPosChange | CombatUnitState.CombatState;
        else
            m_CombatUnitState = CombatUnitState.CombatState;
    }

    public void SetCombatUnitState(CombatUnitState combatUnitState)
    {
        m_CombatUnitState = combatUnitState;
    }

    public void AddCombatUnitState(CombatUnitState combatUnitState)
    {
        m_CombatUnitState |= combatUnitState;
    }

    /// <summary>
    /// updateState =-1减behaveCount，=0强制清0，=1加behaveCount
    /// </summary>
    public void UpdateBehaveCount(int updateState)
    {
        DLogManager.Log(ELogType.eCombat, $"ClientNum:{m_ClientNum.ToString()}[{((m_Entity as MobEntity).m_Go == null ? null : (m_Entity as MobEntity).m_Go.name)}][{m_BattleUnit?.UnitId.ToString()}]  updateState:{updateState.ToString()}    处理前behaveCount:{_behaveCount.ToString()}");

        if (updateState == 0)
            _behaveCount = 0;
        else if (updateState == 1)
        {
            if (_behaveCount < 0)
                _behaveCount = 1;
            else
                _behaveCount++;
        }
        else
        {
            if (_behaveCount > 0)
                _behaveCount--;
            else
                _behaveCount = 0;
        }
    }

    /// <summary>
    /// updateReadlyState =-1减m_ReadlyBehaveCount，=0强制清0，=1加m_ReadlyBehaveCount
    /// </summary>
    public void UpdateReadlyBehaveCount(int updateReadlyState)
    {
        DLogManager.Log(ELogType.eCombat, $"ReadlyBehaveCount----ClientNum:{m_ClientNum.ToString()}[{((m_Entity as MobEntity).m_Go == null ? null : (m_Entity as MobEntity).m_Go.name)}]  updateReadlyState:{updateReadlyState.ToString()}    处理前m_ReadlyBehaveCount:{m_ReadlyBehaveCount.ToString()}");

        if (updateReadlyState == 0)
            m_ReadlyBehaveCount = 0;
        else if (updateReadlyState == 1)
        {
            if (m_ReadlyBehaveCount < 0)
                m_ReadlyBehaveCount = 1;
            else
                m_ReadlyBehaveCount++;
        }
        else
        {
            if (m_ReadlyBehaveCount > 0)
                m_ReadlyBehaveCount--;
            else
                m_ReadlyBehaveCount = 0;
        }
    }

    public int GetBehaveCount()
    {
        return _behaveCount;
    }

    public void ClearCombatHpChangeDataQueue(bool isSetHpMp = true)
    {
        if (m_HpChangeDataQueue != null)
        {
#if DEBUG_MODE
            if (m_HpChangeDataQueue.Count > 0)
            {
                DebugUtil.LogError($"{(m_CurUseEntity == null || m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}单位清除HpMpData时数据还残留数量：{m_HpChangeDataQueue?.Count.ToString()}");
                if (m_isHaveChildLayerActive)
                {
                    DebugUtil.LogError($"{(m_CurUseEntity == null || m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}    clientNum:{m_ClientNum.ToString()}   unitId;{m_BattleUnit.UnitId.ToString()}有可能是没有配置正确的被动时机节点，导致没有执行嵌套的子主动技能出现问题");
                    m_isHaveChildLayerActive = false;
                }
            }
            
#endif

            while (m_HpChangeDataQueue.Count > 0)
            {
                var hpChangeData = m_HpChangeDataQueue.Dequeue();

#if DEBUG_MODE
                DebugUtil.LogError($"HpChangeDataQueue.Dequeue-----{GetHpChangeDataDebugLog(hpChangeData)}");
#endif
                CombatHpChangeData.Push(hpChangeData);
            }

            if (isSetHpMp)
            {
                Net_Combat.Instance.UpdateHp(m_BattleUnit, m_BattleUnit.CurHp, m_BattleUnit.MaxHp);
                Net_Combat.Instance.UpdateMp(m_BattleUnit, m_BattleUnit.CurMp, m_BattleUnit.MaxMp);
                Net_Combat.Instance.UpdateShield(m_BattleUnit, m_BattleUnit.CurShield, m_BattleUnit.MaxShield);
                Net_Combat.Instance.UpdateGas(m_BattleUnit, m_BattleUnit.CurGas, m_BattleUnit.MaxGas);
            }
        }
    }

    public void OnShowOrHideBD(bool isShow)
    {
        DLogManager.Log(ELogType.eCombat, $"OnShowOrHideBD----OnHideBD----<color=yellow>{(m_CurUseEntity.m_Go == null ? null : m_CurUseEntity.m_Go.name)}  UnitId:{m_BattleUnit.UnitId.ToString()}  clientNum:{m_ClientNum.ToString()}</color>");
        ShowOrHideBDEvt showOrHideBDEvt = CombatObjectPool.Instance.Get<ShowOrHideBDEvt>();
        showOrHideBDEvt.id = m_BattleUnit.UnitId;
        showOrHideBDEvt.flag = isShow;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideBD, showOrHideBDEvt);
        CombatObjectPool.Instance.Push(showOrHideBDEvt);
    }

    public void OnShowMobUI(bool isShow)
    {
        OnShowOrHideBD(isShow);
        
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideBuffHUD, m_BattleUnit.UnitId, isShow);
    }

    public void SetMobDeath()
    {
        m_Death = true;
        m_DeathType = 1;

        m_BattleUnit.CurHp = 0;

        m_CurUseEntity.m_Trans.position = CombatHelp.FarV3;

        SetCombatUnitState(CombatUnitState.Death | CombatUnitState.FarDeath);
    }

    public void ChangeUnitServerPos(int newServerPos, bool isSetTransPos = false)
    {
        if (newServerPos < 0)
            return;

        if (m_BattleUnit.Pos == newServerPos)
            return;

        m_BattleUnit.Pos = newServerPos;
        m_ClientNum = CombatHelp.ServerToClientNum(m_BattleUnit.Pos, CombatManager.Instance.m_IsNotMirrorPos);
        RefreshPos(isSetTransPos, false);
    }

    public void PlaySuccessAnimation(string state, bool isCheck, bool isStopAnimationBeforePlay)
    {
        if (isCheck && state == m_CurAnimationName)
            return;

        if (isStopAnimationBeforePlay)
            m_AnimationComponent.StopAll();

        m_CurAnimationName = state;
        m_AnimationComponent.PlaySuccess(state, false);
    }

#if DEBUG_MODE
    public static string GetHpChangeDataDebugLog(CombatHpChangeData hpChangeData)
    {
        if (hpChangeData == null)
            return null;

        return $"m_ExcuteDataIndex:{hpChangeData.m_ExcuteDataIndex.ToString()}   ExtendId:{hpChangeData.m_ExtendId.ToString()}  ExtendType:{hpChangeData.m_ExtendType.ToString()}   m_BuffSkillId:{hpChangeData.m_BuffSkillId.ToString()}   CurHp:{hpChangeData.m_CurHp.ToString()}  HpChange:{hpChangeData.m_HpChange.ToString()}  CurMp:{hpChangeData.m_CurMp.ToString()}  MpChange:{hpChangeData.m_MpChange.ToString()}  CurShield:{hpChangeData.m_CurShield.ToString()}  ShieldChange:{hpChangeData.m_ShieldChange.ToString()}   m_CurGas:{hpChangeData.m_CurGas.ToString()}   m_GasChange:{hpChangeData.m_GasChange.ToString()}   ChangeType:{hpChangeData.m_ChangeType.ToString()}  m_Revive:{hpChangeData.m_Revive.ToString()}";
    }
#endif
}
