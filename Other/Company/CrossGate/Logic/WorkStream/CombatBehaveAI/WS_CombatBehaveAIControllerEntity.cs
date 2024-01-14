using UnityEngine;using System.Collections.Generic;
using Table;
using Lib.Core;
using Logic;
using Packet;

[StateController((int)StateCategoryEnum.CombatBehaveAI, "Config/WorkStreamData/CombatBehaveAI{0}/CombatBehaveAI.txt")]
public class WS_CombatBehaveAIControllerEntity : BaseStreamControllerEntity
{
    public bool m_IsMelee;
    public CSVActiveSkill.Data m_SkillTb;
    public int m_AttachType;

    public TurnBehaveSkillInfo m_SourcesTurnBehaveSkillInfo;
    public BehaveAIControllParam m_BehaveAIControllParam;

    public CombatHpChangeData m_CurHpChangeData;
    
    private uint _protectedId;
    private uint _beProtectedId;

#if DEBUG_MODE
    public uint m_WorkId;
#endif

    public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
	{
        DLogManager.Log(ELogType.eCombat, $"<color=yellow>{Id.ToString()}--CombatBehaveAIController初始化</color>");

		if (m_WorkStreamManagerEntity != workStreamManagerEntity)
		{
			m_WorkStreamManagerEntity = workStreamManagerEntity;
			OnInit((int)StateCategoryEnum.CombatBehaveAI);
		}

		if (!PrepareCombatBehaveAI(workId, workBlockDatas))
		{
			OnOver(false);
			return false;
		}

		return true;
	}

	public override void Dispose()
	{
        MobEntity mob = null;
        if (m_WorkStreamManagerEntity != null)
            mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

#if DEBUG_MODE
        if (mob != null && mob.m_MobCombatComponent != null && mob.m_MobCombatComponent.m_IsStartBehave)
            DebugUtil.Log(ELogType.eCombatBehave, $"<color=yellow>{Id.ToString()}--{mob.m_MobCombatComponent.m_ClientNum.ToString()}--WorkId:{m_WorkId.ToString()}--CombatBehaveAIController行为结束</color>");
        else
            DLogManager.Log(ELogType.eCombat, $"<color=yellow>{Id.ToString()}--WorkId:{m_WorkId.ToString()}--CombatBehaveAIController结束</color>");
        
        if (m_CurHpChangeData != null && m_CurHpChangeData.m_Revive)
        {
            if (mob != null && mob.GetComponent<MobDeadComponent>() != null)
                DebugUtil.LogError($"{mob.m_Go?.name}执行m_WorkId:{m_WorkId.ToString()}时状态m_Revive：{m_CurHpChangeData.m_Revive.ToString()}确还有MobDeadComponent组件，估计是执行目标行为时目标的第一个块中没有配节点：{"收集行为数据"}");
        }

        m_WorkId = 0u;
#endif

        if (m_SourcesTurnBehaveSkillInfo != null && m_BehaveAIControllParam != null)
        {
            Net_Combat.Instance.DoBuffChangeData(m_BehaveAIControllParam.ExcuteTurnIndex, m_SourcesTurnBehaveSkillInfo, m_AttachType);
        }

        bool isUpdateBuff = true;
        if (m_BehaveAIControllParam != null)
        {
            isUpdateBuff = !m_BehaveAIControllParam.IsNotUpdateBuffInOverBehave;

            if (m_BehaveAIControllParam.IsIdleInControlOver &&
                mob != null && mob.m_MobCombatComponent.m_AnimationComponent != null)
            {
                mob.m_MobCombatComponent.PlaySuccessAnimation("action_idle", false, false);
            }

            m_BehaveAIControllParam.Push();
            m_BehaveAIControllParam = null;
        }

        SetProtectedInfo(false);

        if (mob != null)
        {
            if (!mob.m_MobCombatComponent.m_Death && mob.m_MobCombatComponent.m_BattleUnit != null)
            {
                //显示血条
                mob.m_MobCombatComponent.OnShowOrHideBD(true);

                //显示Buff
                Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideBuffHUD, mob.m_MobCombatComponent.m_BattleUnit.UnitId, true);
            }
            
            mob.m_MobCombatComponent.BehaveOver(m_CurHpChangeData, isUpdateBuff);
            m_CurHpChangeData = null;
        }
        else
        {
            if (m_CurHpChangeData != null)
            {
                CombatHpChangeData.Push(m_CurHpChangeData);
                m_CurHpChangeData = null;
            }
        }
        m_SkillTb = null;
        
        m_SourcesTurnBehaveSkillInfo = null;
        
        base.Dispose();
	}

	public bool PrepareCombatBehaveAI(uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)
	{
		if (workBlockDatas == null || workBlockDatas.Count == 0)
			return false;

		if (m_FirstMachine != null)
			m_FirstMachine.Dispose();

		m_FirstMachine = CreateFirstStateMachineEntity();
		WorkStreamTranstionComponent workStreamTranstionComponent = m_FirstMachine.AddTranstion<WorkStreamTranstionComponent>();
		workStreamTranstionComponent.InitWorkBlockDatas(workId, workBlockDatas);
		m_ControllerBeginAction?.Invoke(this);

		return true;
	}

    /// <summary>
	/// 自定义专属初始化函数，参数根据情况自定义添加
	/// 也可以依此拓展自己定义的函数
	/// </summary>
	public bool DoInit()
    {
        return true;
    }

    public override bool StartController(int blockType = 0)
    {
        MobEntity mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

        SetController(mob);

        return base.StartController(blockType);
    }
    
    private void SetController(MobEntity mob)
    {
        DLogManager.Log(ELogType.eCombat, $"<color=yellow>{(mob == null || mob.m_Go == null ? null : mob.m_Go.name)}--{Id.ToString()}--StartWorkId{((WorkStreamTranstionComponent)m_FirstMachine.m_StateTranstionComponent).m_WorkId.ToString()}</color>");

        if (m_AttachType == 1)    //处理被击者行为
        {
            if ((m_BehaveAIControllParam != null && m_BehaveAIControllParam.IsAddBehaveCount) || 
                !mob.m_MobCombatComponent.m_IsStartBehave)
                mob.m_MobCombatComponent.UpdateBehaveCount(1);

            if ((m_BehaveAIControllParam == null || !m_BehaveAIControllParam.IsNotNeedDequeueChangeData) && 
                mob.m_MobCombatComponent.m_HpChangeDataQueue.Count > 0)
            {
                CombatHpChangeData.Push(m_CurHpChangeData);

                m_CurHpChangeData = mob.m_MobCombatComponent.m_HpChangeDataQueue.Dequeue();
#if DEBUG_MODE
                DLogManager.Log(ELogType.eCombat, $"HpChangeDataQueue.Dequeue-----<color=yellow>{(mob == null || mob.m_Go == null ? null : mob.m_Go.name)}--Count:{mob.m_MobCombatComponent.m_HpChangeDataQueue.Count.ToString()}    {MobCombatComponent.GetHpChangeDataDebugLog(m_CurHpChangeData)}</color>");
#endif
            }
        }
        else    //处理释放着行为
        {
            //隐藏血条
            mob.m_MobCombatComponent.OnShowOrHideBD(false);

            //隐藏Buff
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnShowOrHideBuffHUD, mob.m_MobCombatComponent.m_BattleUnit.UnitId, false);

            mob.m_MobCombatComponent.UpdateBehaveCount(1);
            
            if (CombatManager.Instance.m_BattleTypeTb != null && CombatManager.Instance.m_BattleTypeTb.is_quickfight)
            {
                Net_Combat.Instance.m_RecordCombatBehaveUnitList.Add(mob.m_MobCombatComponent.m_BattleUnit.UnitId);
            }

            if (m_SourcesTurnBehaveSkillInfo != null && m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null)
            {
#if DEBUG_MODE
                int targetUnitCount = 0;
#endif

                for (int targetIndex = 0, targetCount = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; targetIndex < targetCount; targetIndex++)
                {
                    mob.m_MobCombatComponent.UpdateReadlyBehaveCount(-1);

                    TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[targetIndex];
                    if (turnBehaveSkillTargetInfo == null)
                        continue;
                    
                    if (turnBehaveSkillTargetInfo.TargetUnitId == 0u)
                        continue;
                    
#if DEBUG_MODE
                    ++targetUnitCount;
#endif

                    MobEntity targetMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
                    if (targetMob == null)
                        continue;

                    if (m_BehaveAIControllParam != null)
                    {
                        if (m_BehaveAIControllParam.IsNotUpdateEveryTypeBehaveCount)
                            continue;

                        targetMob.m_MobCombatComponent.m_ExcuteIndex = m_BehaveAIControllParam.ExcuteTurnIndex;
                    }

                    DLogManager.Log(ELogType.eCombat, $"{targetMob.m_MobCombatComponent.m_ClientNum.ToString()}号被击者被设为开始行为");
                    
                    targetMob.m_MobCombatComponent.UpdateBehaveCount(1);
                    
                    if (CombatManager.Instance.m_BattleTypeTb != null && CombatManager.Instance.m_BattleTypeTb.is_quickfight)
                    {
                        Net_Combat.Instance.m_RecordCombatBehaveUnitList.Add(targetMob.m_MobCombatComponent.m_BattleUnit.UnitId);
                    }

                    if (turnBehaveSkillTargetInfo.BeProtectUnitId > 0u)
                    {
                        MobEntity beProtectMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.BeProtectUnitId);
                        if(beProtectMob != null)
                        {
                            DLogManager.Log(ELogType.eCombat, $"{beProtectMob.m_Go?.name}    {beProtectMob.m_MobCombatComponent.m_ClientNum.ToString()}号被设为被护卫者");
                            beProtectMob.m_MobCombatComponent.UpdateReadlyBehaveCount(1);
                        }
                    }
                }

#if DEBUG_MODE
                if (m_SourcesTurnBehaveSkillInfo.TargetUnitCount != targetUnitCount)
                    DebugUtil.LogError($"释放技能{m_SourcesTurnBehaveSkillInfo.SkillId.ToString()}时m_SourcesTurnBehaveSkillInfo记录的TargetUnitCount:{m_SourcesTurnBehaveSkillInfo.TargetUnitCount.ToString()}和实际TargetUnitCount:{targetUnitCount.ToString()}不一致");
#endif
            }
            else
                mob.m_MobCombatComponent.UpdateReadlyBehaveCount(-1);
        }
    }

    public bool SkipFirstStateMachineCombatBehaveAIByState()
    {
        return SkipCombatBehaveAIByState((WorkStreamTranstionComponent)m_FirstMachine.m_StateTranstionComponent);
    }

    public bool SkipCombatBehaveAIByState(WorkStreamTranstionComponent workStreamTranstionComponent)
    {
        MobEntity mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

        if (m_CurHpChangeData == null)
        {
            DebugUtil.LogError($"ClientNum:{mob.m_MobCombatComponent.m_ClientNum.ToString()}执行被击行为时没有CurHpChangeData数据");
            return false;
        }
        
        int selectBehave = GetBlockTypeByState(workStreamTranstionComponent);
        if (selectBehave < 1)
            return false;

        uint workId = workStreamTranstionComponent.m_WorkId;
        if (!workStreamTranstionComponent.StartWorkStream(selectBehave))
        {
            DebugUtil.LogError($"强制结束行为，因为----workId:{workId.ToString()}   ClientPos:{mob.m_MobCombatComponent.m_ClientNum.ToString()} 复活状态:{m_CurHpChangeData.m_Revive.ToString()} 进行行为块:{((CombatBehaveAIEnum)selectBehave).ToString()}没有获取到");

            //m_MobEntity.m_MobCombatComponent.m_ExcuteIndex = -1;
            //m_MobEntity.m_MobCombatComponent.m_IsStartBehave = false;
            //m_MobEntity.m_MobCombatComponent.m_BehaveCount--;

            return false;
        }

        return true;
    }

    public int GetBlockTypeByState(WorkStreamTranstionComponent workStreamTranstionComponent, bool isCheckExtendType = true)
    {
        MobEntity mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

        if (m_CurHpChangeData == null)
        {
            DebugUtil.LogError($"ClientNum:{mob.m_MobCombatComponent.m_ClientNum.ToString()}执行被击行为时没有CurHpChangeData数据");
            return -1;
        }

        int selectBehave = 0;
        if (isCheckExtendType && m_CurHpChangeData.m_ExtendType == 5)
        {
            selectBehave = (int)CombatBehaveAIEnum.B_BeHit_FightBack;

            DebugUtil.Log(ELogType.eCombat, $"{mob.m_Go?.name}  is m_CurHpChangeData.m_ExtendType:{m_CurHpChangeData.m_ExtendType.ToString()}   selectBehave:{((CombatBehaveAIEnum)selectBehave).ToString()}");
        }
        else if (m_CurHpChangeData.m_Revive)
        {
            selectBehave = (int)CombatBehaveAIEnum.B_BeHit_Death;

            DebugUtil.Log(ELogType.eCombat, $"{mob.m_Go?.name}  is m_CurHpChangeData.m_Revive:{m_CurHpChangeData.m_Revive.ToString()}   selectBehave:{((CombatBehaveAIEnum)selectBehave).ToString()}");

            mob.RemoveComponent<MobDeadComponent>();
        }
        else
        {
            bool isDeathState = false;

            int buffState = 1;
            MobBuffComponent targetBuffComponent = mob.GetComponent<MobBuffComponent>();
            if (targetBuffComponent != null)
            {
                buffState = (int)targetBuffComponent.GetBuffMaxLevelPriority();
                if (buffState == 0)
                    buffState = 1;
                else if (buffState == 4 && m_CurHpChangeData.m_ExtraHitEffect != 6u)
                {
                    buffState = 1;
                }
            }
            else if (mob.GetComponent<MobDeadComponent>() != null)
            {
                isDeathState = true;
            }

            if (isDeathState)
            {
                if (CombatHelp.ContainAnimType(m_CurHpChangeData.m_AnimType, AnimType.e_Miss))
                {
                    selectBehave = (int)CombatBehaveAIEnum.B_NoBeHit_Death;
                }
                else
                {
                    selectBehave = (int)CombatBehaveAIEnum.B_BeHit_Death;
                }
            }
            else
            {
                if (CombatHelp.ContainAnimType(m_CurHpChangeData.m_AnimType, Logic.AnimType.e_Invalid))
                {
                    selectBehave = 3999;
                }
                else if (CombatHelp.ContainAnimType(m_CurHpChangeData.m_AnimType, Logic.AnimType.e_Miss) ||
                                CombatHelp.ContainAnimType(m_CurHpChangeData.m_AnimType, Logic.AnimType.e_Error))
                {
                    selectBehave = 3000 + buffState;
                }
                else
                {
                    selectBehave = 2000 + buffState;
                }
            }

            DebugUtil.Log(ELogType.eCombat, $"{mob.m_Go?.name}  is isDeathState:{isDeathState.ToString()}   selectBehave:{((CombatBehaveAIEnum)selectBehave).ToString()}");
        }

        return selectBehave;
    }

    public void GetSceneCombatPos(MobEntity mob, int offsetType, uint pointType, ref Vector3 pos)
    {
        if (offsetType == 0)
        {
            pos = CombatManager.Instance.CombatSceneCenterPos;
        }
        else
        {
            uint combatPosDataId = CombatManager.Instance.m_BattlePosType * 1000000 + pointType * 10000u + 1u;
            if (CombatHelp.GetClientCampSide(mob.m_MobCombatComponent.m_BattleUnit.Pos) == 0)
            {
                if (offsetType == 1)
                    combatPosDataId += 100u;
                else if (offsetType == -1)
                    combatPosDataId += 200u;
            }
            else
            {
                if (offsetType == 1)
                    combatPosDataId += 200u;
                else if (offsetType == -1)
                    combatPosDataId += 100u;
            }
            CombatPosData combatPosData = CombatConfigManager.Instance.GetCombatPosData(combatPosDataId);
            if (combatPosData != null)
            {
                pos = CombatManager.Instance.CombatSceneCenterPos;
                if (combatPosData != null)
                {
                    if (CombatManager.Instance.PosFollowSceneCamera)
                        pos += CombatManager.Instance.m_AdjustSceneViewAxiss[0] * combatPosData.PosX + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * combatPosData.PosZ;
                    else
                        pos += new Vector3(combatPosData.PosX, 0f, combatPosData.PosZ);
                }
            }
        }
    }

    public void GetAttackTypeTargetPos(ref Vector3 pos)
    {
#if UNITY_EDITOR
#if !ILRUNTIME_MODE
        if (CreateCombatDataTest.s_ClientType)
        {
            for (int i = 0, count = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; i < count; i++)
            {
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[i];
                if (turnBehaveSkillTargetInfo == null || turnBehaveSkillTargetInfo.TargetUnitId == 0u)
                    continue;

                MobEntity targetPosMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
                if (targetPosMob != null && targetPosMob.m_MobCombatComponent != null)
                {
                    pos = targetPosMob.m_MobCombatComponent.m_OriginPos;
                    return;
                }
            }
            return;
        }
#endif
#endif

        var mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

        if (m_BehaveAIControllParam != null)
        {
            ExcuteTurn excuteTurn = Net_Combat.Instance.GetNetExcuteTurnData(m_BehaveAIControllParam.ExcuteTurnIndex);
            if (excuteTurn != null)
            {
                MobEntity targetPosMob = MobManager.Instance.GetMobByServerNum(excuteTurn.TarPos);
                if (targetPosMob != null && targetPosMob.m_MobCombatComponent != null)
                {
                    if (!targetPosMob.m_MobCombatComponent.m_Death)
                        pos = targetPosMob.m_MobCombatComponent.m_OriginPos;
                    else if (m_SkillTb != null && m_SourcesTurnBehaveSkillInfo != null && m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null &&
                        m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count > 0)
                    {
                        if (m_SkillTb.attack_type == 3 || m_SkillTb.attack_type == 11 ||
                            m_SkillTb.attack_type == 14)
                        {
                            bool isExist = false;
                            int targetUnitIdIndex = 0;
                            MobEntity targetUnitIdMob = null;
                            for (int tbstiIndex = 0, tbstiCount = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
                            {
                                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbstiIndex];
                                if (turnBehaveSkillTargetInfo == null || turnBehaveSkillTargetInfo.TargetUnitId == 0u)
                                    continue;

                                MobEntity tbstiMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
                                if (tbstiMob == null || tbstiMob.m_MobCombatComponent == null || tbstiMob.m_MobCombatComponent.m_BattleUnit == null)
                                    continue;

                                if (targetUnitIdIndex == 0)
                                    targetUnitIdMob = tbstiMob;

                                ++targetUnitIdIndex;

                                if (m_SkillTb.attack_type == 3 ||
                                    m_SkillTb.attack_type == 14)
                                {
                                    int checkModServerPos = excuteTurn.TarPos % 20;

                                    if (tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos - 1 ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos + 1 ||
                                        (tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos + 10 * (checkModServerPos < 10 ? 1 : -1)))
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                                else if (m_SkillTb.attack_type == 11)
                                {
                                    if (tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos - 1 ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos - 2 ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos + 1 ||
                                        tbstiMob.m_MobCombatComponent.m_BattleUnit.Pos == excuteTurn.TarPos + 2)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }
                            }

                            if (isExist)
                                pos = targetPosMob.m_MobCombatComponent.m_OriginPos;
                            else
                                pos = targetUnitIdMob.m_MobCombatComponent.m_OriginPos;
                        }
                        else
                            pos = targetPosMob.m_MobCombatComponent.m_OriginPos;
                    }
                }

            }
        }
    }

    public static void BulletHitToProcess(MobEntity attack, MobEntity target, int excuteTurnIndex, 
        TurnBehaveSkillInfo turnBehaveSkillInfo, int turnBehaveSkillTargetInfoIndex, int isCanProcessTypeAfterHit)
    {
        if ((isCanProcessTypeAfterHit & 1) == 0)
        {
            DLogManager.Log(ELogType.eCombatBehave, $"BulletHitToProcess----<color=yellow>执行DoPassive ：attack:{(attack == null ? null : (attack.m_Go == null ? null : attack.m_Go.name))}   target:{(target == null ? null : (target.m_Go == null ? null : target.m_Go.name))}   effectTrigger:6u</color>");
            Net_Combat.Instance.DoPassive(excuteTurnIndex, turnBehaveSkillInfo, turnBehaveSkillTargetInfoIndex, 6u);
        }

        if ((isCanProcessTypeAfterHit & 2) == 0)
        {
            DLogManager.Log(ELogType.eCombatBehave, $"BulletHitToProcess----<color=yellow>执行DoTriggerBuffBehave->attack ：attack:{(attack == null ? null : (attack.m_Go == null ? null : attack.m_Go.name))}   target:{(target == null ? null : (target.m_Go == null ? null : target.m_Go.name))}   effectTrigger:6u</color>");
            Net_Combat.Instance.DoTriggerBuffBehave(attack, excuteTurnIndex, turnBehaveSkillInfo, turnBehaveSkillTargetInfoIndex);
        }

        if ((isCanProcessTypeAfterHit & 4) == 0)
        {
            DLogManager.Log(ELogType.eCombatBehave, $"BulletHitToProcess----<color=yellow>执行DoTriggerBuffBehave->target ：attack:{(attack == null ? null : (attack.m_Go == null ? null : attack.m_Go.name))}   target:{(target == null ? null : (target.m_Go == null ? null : target.m_Go.name))}   effectTrigger:6u</color>");
            Net_Combat.Instance.DoTriggerBuffBehave(target, excuteTurnIndex, turnBehaveSkillInfo, turnBehaveSkillTargetInfoIndex);
        }
    }

    /// <summary>
    /// type=0当前行为者，=1目标，=-1获取被护卫者，
    /// </summary>
    public static MobEntity GetMobByType(StateMachineEntity stateMachineEntity, int type)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)stateMachineEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        if (type == 0)
            return mob;

        WS_CombatBehaveAIDataComponent dataComponent = stateMachineEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);

        if (turnBehaveSkillTargetInfo != null)
        {
            if (type == -1 && turnBehaveSkillTargetInfo.BeProtectUnitId > 0u)
                return MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.BeProtectUnitId);
            else if (type == 1)
            {
                if (turnBehaveSkillTargetInfo.TargetUnitId > 0)
                    return MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
                else if (turnBehaveSkillTargetInfo.CallingTargetBattleUnit != null)
                    return MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.CallingTargetBattleUnit.UnitId);
            }
        }

        return null;
    }

    public static TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfo(StateMachineEntity stateMachineEntity)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)stateMachineEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = stateMachineEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        return dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);
    }

    public static TurnBehaveSkillTargetInfo GetTurnBehaveSkillTargetInfo(StateMachineEntity stateMachineEntity, out int turnBehaveSkillTargetInfoIndex)
    {
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)stateMachineEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = stateMachineEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        return dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob, out turnBehaveSkillTargetInfoIndex);
    }

    /// <summary>
    /// type=0当前行为者，=1目标，=-1获取被护卫者，
    /// </summary>
    public static uint GetUnitIdByType(StateMachineEntity stateMachineEntity, int type)
    {
        if (type == 0)
        {
            WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)stateMachineEntity.m_StateControllerEntity;
            var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
            return mob.m_MobCombatComponent.m_BattleUnit.UnitId;
        }

        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = GetTurnBehaveSkillTargetInfo(stateMachineEntity);
        if (turnBehaveSkillTargetInfo == null)
            return 0u;

        if (type == 1)
            return turnBehaveSkillTargetInfo.TargetUnitId > 0u ? turnBehaveSkillTargetInfo.TargetUnitId :
                (turnBehaveSkillTargetInfo.CallingTargetBattleUnit == null ? 0u : turnBehaveSkillTargetInfo.CallingTargetBattleUnit.UnitId);
        else if (type == -1)
            return turnBehaveSkillTargetInfo.BeProtectUnitId;

        return 0u;
    }
    public static uint GetUnitIdByType(TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo, int type)
    {
        if (turnBehaveSkillTargetInfo == null)
            return 0u;

        if (type == 1)
            return turnBehaveSkillTargetInfo.TargetUnitId > 0u ? turnBehaveSkillTargetInfo.TargetUnitId :
                (turnBehaveSkillTargetInfo.CallingTargetBattleUnit == null ? 0u : turnBehaveSkillTargetInfo.CallingTargetBattleUnit.UnitId);
        else if (type == -1)
            return turnBehaveSkillTargetInfo.BeProtectUnitId;

        return 0u;
    }

    public bool DoTargetBlock(StateMachineEntity stateMachineEntity, uint workId, int wsBlock,
        bool isAddBehaveCount, bool isNotNeedDequeueChangeData, bool isNotUpdateBuffInOverBehave)
    {
        MobEntity mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = stateMachineEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(this, mob, out turnBehaveSkillTargetInfoIndex);

        return DoTargetBlock(stateMachineEntity, turnBehaveSkillTargetInfo, turnBehaveSkillTargetInfoIndex,
            workId, wsBlock, 
            isAddBehaveCount, isNotNeedDequeueChangeData, isNotUpdateBuffInOverBehave);
    }

    public bool DoTargetBlock(StateMachineEntity stateMachineEntity, 
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo, int turnBehaveSkillTargetInfoIndex,
        uint workId, int wsBlock, 
        bool isAddBehaveCount, bool isNotNeedDequeueChangeData, bool isNotUpdateBuffInOverBehave)
    {
        if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.TargetUnitId > 0)
        {
            var targetMob = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
            if (targetMob == null)
                return false;

            MobEntity mob = (MobEntity)m_WorkStreamManagerEntity.Parent;

            uint bulletWorkId = workId == 0u ? ((WorkStreamTranstionComponent)stateMachineEntity.m_StateTranstionComponent).m_WorkId : workId;

            BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
            behaveAIControllParam.SrcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
            behaveAIControllParam.SkillId = m_BehaveAIControllParam == null ?
                (m_SkillTb == null ? 0u : m_SkillTb.id) : m_BehaveAIControllParam.SkillId;
            behaveAIControllParam.TargetUnitId = turnBehaveSkillTargetInfo.TargetUnitId;
            behaveAIControllParam.TurnBehaveSkillTargetInfoIndex = turnBehaveSkillTargetInfoIndex;
            if (m_BehaveAIControllParam != null)
                behaveAIControllParam.ExcuteTurnIndex = m_BehaveAIControllParam.ExcuteTurnIndex;
            behaveAIControllParam.IsAddBehaveCount = isAddBehaveCount;
            behaveAIControllParam.IsNotNeedDequeueChangeData = isNotNeedDequeueChangeData;
            behaveAIControllParam.IsNotUpdateBuffInOverBehave = isNotUpdateBuffInOverBehave;

            return targetMob.m_MobCombatComponent.DoBehave(bulletWorkId, m_SkillTb, 1,
                        m_SourcesTurnBehaveSkillInfo, behaveAIControllParam, wsBlock);
        }

        return false;
    }

    private static readonly string _animaMoveName = "action_battle_move";
    public static void PlayMoveAnimationAudio(StateMachineEntity stateMachineEntity,
        uint serverUnitType, uint unitInfoId, uint weaponId, string animationName)
    {
        if (animationName == _animaMoveName)
        {
            CSVSkillSE.Data skillSEDataTb = CombatManager.Instance.GetSkillAudioTb(serverUnitType, unitInfoId, weaponId);
            if (skillSEDataTb == null)
                return;

            uint audioId = skillSEDataTb.audio_tool[0];
            if (audioId == 0u)
                return;

            if (Random.Range(0, 10000) >= (int)skillSEDataTb.probability)
                return;

            if (stateMachineEntity.m_ParentMachine == null)
                stateMachineEntity.GetNeedComponent<PlayAudioComponent>().AddAudio(audioId, 0f);
            else
                stateMachineEntity.m_ParentMachine.GetNeedComponent<PlayAudioComponent>().AddAudio(audioId, 0f);
        }
    }

    public void SetProtectedInfo(bool isProtectedState, uint protectedId = 0u, uint beProtectedId = 0u)
    {
        uint pId;
        uint bpId;
        if (isProtectedState)
        {
            pId = _protectedId = protectedId;
            bpId = _beProtectedId = beProtectedId;
        }
        else
        {
            pId = _protectedId;
            _protectedId = 0u;

            bpId = _beProtectedId;
            _beProtectedId = 0u;
        }

        if (pId > 0u)
        {
            MobEntity protectedMob = MobManager.Instance.GetMob(pId);
            if (protectedMob != null)
                protectedMob.m_MobCombatComponent.m_IsDoProtected = isProtectedState;
        }

        if (bpId > 0u)
        {
            MobEntity beProtectedMob = MobManager.Instance.GetMob(bpId);
            if (beProtectedMob != null)
                beProtectedMob.m_MobCombatComponent.m_IsDoProtected = isProtectedState;
        }
    }
}