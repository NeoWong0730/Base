using Lib.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;

public class BehaveAIControllParam : BasePoolClass
{
    public uint SrcUnitId;
    public uint TargetUnitId;
    public uint SkillId;
    public int TurnBehaveSkillTargetInfoIndex;
    public int ExcuteTurnIndex;
    public int HitEffect;
    /// <summary>
    /// 战斗协议中BattleUnitChange.replace_type
    /// </summary>
    public uint m_BattleReplaceType;
    /// <summary>
    /// 控制器行为结束的时候需要做Idle动作
    /// </summary>
    public bool IsIdleInControlOver;
    /// <summary>
    /// 表演中目标需要额外独立启动一个行为时，给目标行为数加1
    /// （如：护卫中施法者通知目标进行护卫准备工作就是要目标独立启动护卫准备行为，行为需要加1）
    /// </summary>
    public bool IsAddBehaveCount;
    /// <summary>
    /// 该次行为目标不需要从m_HpChangeDataQueue队列中拿出HpChangeData，
    /// 因为目标启动行为时会自动从m_HpChangeDataQueue队列中拿出HpChangeData
    /// </summary>
    public bool IsNotNeedDequeueChangeData;
    /// <summary>
    /// 该次行为结束时不更新buff
    /// </summary>
    public bool IsNotUpdateBuffInOverBehave;
    /// <summary>
    /// 该次行为开启的类型
    /// </summary>
    public StartControllerStyleEnum m_StartControllerStyleEnum;
    /// <summary>
    /// 该次行为不需要更新目标任何类型的行为数量
    /// </summary>
    public bool IsNotUpdateEveryTypeBehaveCount;
    
    public static BehaveAIControllParam DeepClone(BehaveAIControllParam clone)
    {
        BehaveAIControllParam t = Get<BehaveAIControllParam>();

        if (clone != null)
        {
            t.SrcUnitId = clone.SrcUnitId;
            t.TargetUnitId = clone.TargetUnitId;
            t.SkillId = clone.SkillId;
            t.TurnBehaveSkillTargetInfoIndex = clone.TurnBehaveSkillTargetInfoIndex;
            t.ExcuteTurnIndex = clone.ExcuteTurnIndex;
            t.HitEffect = clone.HitEffect;
            t.m_BattleReplaceType = clone.m_BattleReplaceType;
            t.IsIdleInControlOver = clone.IsIdleInControlOver;
            t.IsAddBehaveCount = clone.IsAddBehaveCount;
            t.IsNotNeedDequeueChangeData = clone.IsNotNeedDequeueChangeData;
            t.IsNotUpdateBuffInOverBehave = clone.IsNotUpdateBuffInOverBehave;
            t.m_StartControllerStyleEnum = clone.m_StartControllerStyleEnum;
            t.IsNotUpdateEveryTypeBehaveCount = clone.IsNotUpdateEveryTypeBehaveCount;
        }
        
        return t;
    }

    public override void Clear()
    {
        SrcUnitId = 0u;
        TargetUnitId = 0u;
        SkillId = 0u;
        TurnBehaveSkillTargetInfoIndex = -1;
        ExcuteTurnIndex = -1;
        HitEffect = -1;
        m_BattleReplaceType = 0u;
        IsIdleInControlOver = false;
        IsAddBehaveCount = false;
        IsNotNeedDequeueChangeData = false;
        IsNotUpdateBuffInOverBehave = false;
        m_StartControllerStyleEnum = StartControllerStyleEnum.None;
        IsNotUpdateEveryTypeBehaveCount = false;
    }
}

public class WS_CombatBehaveAIManagerEntity : WorkStreamManagerEntity
{
    public float m_CommonWaitTime;

	#region 示例
	public static WS_CombatBehaveAIManagerEntity StartCombatBehaveAI02<T>(uint workId, int attachType = 0, 
		SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, 
		Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity
	{
		WS_CombatBehaveAIManagerEntity me = CreateWorkStreamManagerEntity<WS_CombatBehaveAIManagerEntity>(isSelfDestroy);
		T t = me.CreateController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);
		if (t != null && t.StartController())
			return me;
		else
		{
			me.Dispose();
			return null;
		}
	}

    public static T CreateCombatAI<T>(uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity
    {
        WS_CombatBehaveAIManagerEntity me = CreateWorkStreamManagerEntity<WS_CombatBehaveAIManagerEntity>(isSelfDestroy);
        T t = me.CreateController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);
        if (t != null)
            return t;
        else
        {
            me.Dispose();
            return null;
        }
    }
    #endregion

    public bool StartAI(CSVActiveSkill.Data skillTb, int attachType, MobEntity behaveDataSources, 
        TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo, int blockType, BehaveAIControllParam behaveAIControllParam)
    {
        uint workId = skillTb.behavior_tool;
        int hitEffect = sourcesTurnBehaveSkillInfo != null ? (int)sourcesTurnBehaveSkillInfo.HitEffect : (behaveAIControllParam == null ? -1 : behaveAIControllParam.HitEffect);
        if (hitEffect == (int)HitEffectType.Mana || hitEffect == (int)HitEffectType.Energe)
        {
            workId = skillTb.no_mana_behavior;
            if(workId == 0u)
            {
                DebugUtil.LogError($"HitEffect : {hitEffect.ToString()}的CSVActiveSkillData.no_mana_behavior为0");
                if (behaveAIControllParam != null)
                    behaveAIControllParam.Push();
                return false;
            }
        }
        else
        {
            if (skillTb.new_behavior != 0u)
            {
                uint activeSkillBehaviorId = skillTb.id;
                if (skillTb.active_skill_behavior_id > 0u)
                    activeSkillBehaviorId = skillTb.active_skill_behavior_id;

                uint asbId = 0u;
                if (behaveDataSources.m_MobCombatComponent.m_BattleUnit.IsUseShapeShift == 0u && 
                    behaveDataSources.m_MobCombatComponent.m_BattleUnit.ShapeShiftId > 0u)
                {
                    CSVTransform.Data transformTb = CSVTransform.Instance.GetConfData(behaveDataSources.m_MobCombatComponent.m_BattleUnit.ShapeShiftId);
                    if (transformTb == null)
                    {
                        DebugUtil.LogError($"CreateFightHero 变身表中没有数据ShapeShiftId:{behaveDataSources.m_MobCombatComponent.m_BattleUnit.ShapeShiftId.ToString()}");
                        return false;
                    }

                    asbId = transformTb.action_id * 1000000u + activeSkillBehaviorId;
                }
                else if (behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
                {
                    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
                    if (cSVMonsterData == null)
                    {
                        DebugUtil.LogError($"CSVMonsterData表中没有Id ：{behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId.ToString()}");
                        if (behaveAIControllParam != null)
                            behaveAIControllParam.Push();
                        return false;
                    }
                    asbId = cSVMonsterData.monster_id * 10000u + activeSkillBehaviorId;
                }
                else if (behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
                {
                    CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
                    if (cSVPetData == null)
                    {
                        DebugUtil.LogError($"CSVPetData表中没有Id ：{behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId.ToString()}");
                        if (behaveAIControllParam != null)
                            behaveAIControllParam.Push();
                        return false;
                    }
                    asbId = cSVPetData.action_id * 1000000u + activeSkillBehaviorId;
                }
                else
                {
                    asbId = behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId * 1000000u + activeSkillBehaviorId;
                }
                var asbTb = CSVActiveSkillBehavior.Instance.GetConfData(asbId);
                if (asbTb == null)
                {
                    DebugUtil.LogError($"CSVActiveSkillBehavior表不存在unitInfoId：{behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitInfoId.ToString()}加上技能Id：{skillTb.id.ToString()}, " +
                        $"该服务器定义类型为{((UnitType)behaveDataSources.m_MobCombatComponent.m_BattleUnit.UnitType).ToString()}  编辑器调表现的id ：{asbId.ToString()}");
                    if (behaveAIControllParam != null)
                        behaveAIControllParam.Push();
                    return false;
                }

                CSVEquipment.Data edTb = CSVEquipment.Instance.GetConfData(behaveDataSources.m_MobCombatComponent.m_WeaponId);
                if (edTb == null)
                {
                    DebugUtil.LogError($"CSVEquipment表中没有Id ：{behaveDataSources.m_MobCombatComponent.m_WeaponId.ToString()}");
                    if (behaveAIControllParam != null)
                        behaveAIControllParam.Push();
                    return false;
                }

                if (asbTb.behavior_tool == null || edTb.equipment_type > asbTb.behavior_tool.Count)
                {
                    DebugUtil.LogError($"编辑器调表现的id ：{asbId.ToString()}, edTb.equipment_type : {edTb.equipment_type.ToString()} 重组不对！");
                    if (behaveAIControllParam != null)
                        behaveAIControllParam.Push();
                    return false;
                }

                workId = asbTb.behavior_tool[(int)edTb.equipment_type - 1];
                if (workId == 0u)
                {
                    DebugUtil.LogError($"CSVActiveSkillBehavior表Id：{asbTb.id.ToString()}数据中，没有武器Id：{edTb.id.ToString()}的行为Id");
                    if (behaveAIControllParam != null)
                        behaveAIControllParam.Push();
                    return false;
                }
            }
        }
        
        if (StartWorkId(workId, attachType, skillTb, sourcesTurnBehaveSkillInfo, behaveAIControllParam, blockType))
        {
            DLogManager.Log(ELogType.eCombat, $"{behaveDataSources.m_MobCombatComponent.m_ClientNum.ToString()}号位释放技能:{skillTb.id.ToString()}；<color=yellow>{((MobEntity)Parent).m_MobCombatComponent.m_ClientNum.ToString()}号位{(attachType == 0 ? "释放者" : "受击者")}处理行为Id:{workId.ToString()}</color>");
            return true;
        }
        
        return false;
    }

    public bool StartWorkId(uint workId, int attachType, CSVActiveSkill.Data skillTb, 
        TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo, BehaveAIControllParam behaveAIControllParam, 
        int blockType = 0, StartControllerStyleEnum startControllerStyleEnum = StartControllerStyleEnum.Parallel)
    {
        List<WorkBlockData> workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<WS_CombatBehaveAIControllerEntity>(workId, attachType);
        if (workBlockDatas == null || workBlockDatas.Count == 0)
        {
            DebugUtil.LogError($"{typeof(WS_CombatBehaveAIControllerEntity)}类型的数据中没有workId ：{workId.ToString()}, attachType : {attachType.ToString()}");
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return false;
        }

        if (behaveAIControllParam != null && behaveAIControllParam.m_StartControllerStyleEnum != StartControllerStyleEnum.None)
            startControllerStyleEnum = behaveAIControllParam.m_StartControllerStyleEnum;

        m_Parallel2Main = true;
        
        string startCSNodeContent = WorkStreamTranstionComponent.GetWorkNodeContentByType(workBlockDatas, (int)CombatBehaveAIEnum.B_SwitchWorkStreamController, (int)CombatBehaveAIEnum.SwitchWorkStreamStyle);
        if (!string.IsNullOrWhiteSpace(startCSNodeContent))
        {
            int startCSNodeType;
            if (int.TryParse(startCSNodeContent, out startCSNodeType))
                startControllerStyleEnum = (StartControllerStyleEnum)startCSNodeType;
        }
        DebugUtil.Log(ELogType.eCombat, $"WorkId:{workId.ToString()}    attachType:{attachType.ToString()}以{startControllerStyleEnum.ToString()}类型执行");
        
        WS_CombatBehaveAIControllerEntity t = null;
        if (startControllerStyleEnum == StartControllerStyleEnum.Parallel)
        {
            t = CreateParallelController<WS_CombatBehaveAIControllerEntity>(workId, attachType,
                    SwitchWorkStreamEnum.None, null, null, workBlockDatas);
        }
        else if (startControllerStyleEnum == StartControllerStyleEnum.Insert_LastQueue)
        {
            bool isEnqueue;
            t = InsertController<WS_CombatBehaveAIControllerEntity>(workId, startControllerStyleEnum, out isEnqueue, attachType,
                    SwitchWorkStreamEnum.None, null, null, workBlockDatas);

            if (!isEnqueue)
                startControllerStyleEnum = StartControllerStyleEnum.Parallel;
        }
        else if (startControllerStyleEnum == StartControllerStyleEnum.Insert_MainQueue)
        {
            bool isEnqueue;
            t = InsertController<WS_CombatBehaveAIControllerEntity>(workId, startControllerStyleEnum, out isEnqueue, attachType,
                    SwitchWorkStreamEnum.None, null, null, workBlockDatas);

            if (!isEnqueue)
                startControllerStyleEnum = StartControllerStyleEnum.Parallel;
        }
        else
        {
            t = CreateController<WS_CombatBehaveAIControllerEntity>(workId, attachType,
                    SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, 0ul, workBlockDatas);
        }
        if (t != null)
        {
            t.m_StartBlockType = blockType;

#if UNITY_EDITOR
            if (Parent != null)
            {
                MobEntity mobEntity = Parent as MobEntity;
                if (mobEntity != null && mobEntity.m_Go != null)
                {
                    Workstream_Test.AddWorkstream_Test(this, mobEntity.m_Go);
                }
            }
#endif

            if (skillTb != null)
            {
                t.m_IsMelee = skillTb.attack_range == 1;
            }
            t.m_SkillTb = skillTb;
            t.m_AttachType = attachType;
            t.m_SourcesTurnBehaveSkillInfo = sourcesTurnBehaveSkillInfo;
            t.m_BehaveAIControllParam = behaveAIControllParam;
#if DEBUG_MODE
            t.m_WorkId = workId;
#endif

            if (startControllerStyleEnum == StartControllerStyleEnum.Insert_LastQueue ||
                startControllerStyleEnum == StartControllerStyleEnum.Insert_MainQueue)
                return true;

            if (t.StartController(blockType))
                return true;
            else
                return false;
        }

        if (behaveAIControllParam != null)
            behaveAIControllParam.Push();
        return false;
    }
}