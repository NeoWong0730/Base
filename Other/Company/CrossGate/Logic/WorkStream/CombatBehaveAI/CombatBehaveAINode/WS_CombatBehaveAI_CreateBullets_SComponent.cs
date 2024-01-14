using Lib.Core;
using System.Collections.Generic;
using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CreateBullets)]
public class WS_CombatBehaveAI_CreateBullets_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        if (cbace.m_SourcesTurnBehaveSkillInfo == null)
        {
            DebugUtil.LogError($"WorkId:{((WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent).m_WorkId.ToString()}生成飞弹时没有m_SourcesTurnBehaveSkillInfo数据");

            if (m_CurUseEntity != null)
                m_CurUseEntity.TranstionMultiStates(this);

            return;
        }

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        int turnBehaveSkillTargetInfoIndex;
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob, out turnBehaveSkillTargetInfoIndex);

        int isCanProcessTypeAfterHit = dataComponent.m_IsCanProcessTypeAfterHit;

#if DEBUG_MODE
        DLogManager.Log(ELogType.eCombatBehave, $"CreateBullets目标状态----<color=yellow>{mob.m_Go?.name.ToString()}   dc.IsDoPollingNodeMarkNode:{dataComponent.IsDoPollingNodeMarkNode}  skill:{cbace.m_SourcesTurnBehaveSkillInfo.SkillId}  数量：{cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount.ToString()}</color>");
        if (cbace.m_SourcesTurnBehaveSkillInfo != null && cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount > 1)
        {
            if (!dataComponent.IsDoPollingNodeMarkNode)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null)
                {
                    for (int tbstiIndex = 0, tbstiCount = cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; tbstiIndex < tbstiCount; tbstiIndex++)
                    {
                        TurnBehaveSkillTargetInfo tbsti = cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[tbstiIndex];
                        if (tbsti != null)
                        {
                            MobEntity errorTbstiMob = MobManager.Instance.GetMob(tbsti.TargetUnitId);
                            if (errorTbstiMob != null)
                                sb.Append($"{errorTbstiMob.m_Go?.name}");
                            sb.Append($"[{tbsti.TargetUnitId.ToString()}]---");
                        }
                    }
                }
                DebugUtil.LogError($"{mob.m_Go?.name.ToString()}有多个目标{sb.ToString()}   skill:{cbace.m_SourcesTurnBehaveSkillInfo.SkillId.ToString()}   数量：{cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount.ToString()}  但没有填“轮询数量标记”节点");
            }
        }
#endif

        bool isLogicWorkId = false;
        uint bulletId = 0u;
        float delayTime = 0f;
        uint bulletWorkId = 0u;
        #region 处理效果逻辑
        if (cbace.m_SourcesTurnBehaveSkillInfo.ExtendType != 1 && cbace.m_SourcesTurnBehaveSkillInfo.ExtendType != 2 && 
            turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.ExtendId > 0u)
        {
            CSVActiveSkillEffective.Data skillEffectTb = CSVActiveSkillEffective.Instance.GetConfData(turnBehaveSkillTargetInfo.ExtendId);
            if (skillEffectTb != null)
            {
                if (skillEffectTb.weapon_type != null && skillEffectTb.behavior_id != null)
                {
                    CSVEquipment.Data edTb = CSVEquipment.Instance.GetConfData(mob.m_MobCombatComponent.m_WeaponId);
                    if (edTb != null)
                    {
                        int weaponIndex = (int)edTb.equipment_type - 1;
                        if (weaponIndex < skillEffectTb.weapon_type.Count)
                        {
                            int behaviorInfoIndex = skillEffectTb.weapon_type[weaponIndex] * 3;
                            if (behaviorInfoIndex > -1)
                            {
                                if (behaviorInfoIndex < skillEffectTb.behavior_id.Count)
                                {
                                    isLogicWorkId = true;
                                    bulletId = skillEffectTb.behavior_id[behaviorInfoIndex];
                                    delayTime = skillEffectTb.behavior_id[behaviorInfoIndex + 1] * 0.0001f;
                                    bulletWorkId = skillEffectTb.behavior_id[behaviorInfoIndex + 2];

                                    DebugUtil.Log(ELogType.eCombatBehave, $"飞弹执行了turnBehaveSkillTargetInfo.ExtendId:{turnBehaveSkillTargetInfo.ExtendId.ToString()}   isLogicWorkId：{isLogicWorkId.ToString()}   bulletId:{bulletId.ToString()}  delayTime:{delayTime.ToString()}    bulletWorkId:{bulletWorkId.ToString()}");
                                }
#if DEBUG_MODE
                                else
                                    DebugUtil.LogError($"CSVActiveSkillEffective逻辑效果表Id：{turnBehaveSkillTargetInfo.ExtendId}数据equipment_type:{edTb.equipment_type.ToString()}   behaviorInfoIndex:{behaviorInfoIndex.ToString()}   skillEffectTb.behavior_id.Count：{skillEffectTb.behavior_id.Count.ToString()}不对");
#endif
                            }
                        }
#if DEBUG_MODE
                        else
                            DebugUtil.LogError($"CSVActiveSkillEffective逻辑效果表Id：{turnBehaveSkillTargetInfo.ExtendId}数据equipment_type:{edTb.equipment_type.ToString()}   skillEffectTb.weapon_type.Count：{skillEffectTb.weapon_type.Count.ToString()}不对");
#endif
                    }
#if DEBUG_MODE
                    else
                        DebugUtil.LogError($"CSVEquipment表中没有Id ：{mob.m_MobCombatComponent.m_WeaponId.ToString()}");
#endif
                }
            }
#if DEBUG_MODE
            else
                DebugUtil.LogError($"未找到CSVActiveSkillEffective逻辑效果表Id：{turnBehaveSkillTargetInfo.ExtendId}数据");
#endif
        }
        #endregion

        if ((isLogicWorkId && bulletId == 0u) || string.IsNullOrEmpty(str))
        {
            if (dataComponent == null || dataComponent.m_CurTarget == null)
                DebugUtil.LogError($"str1：{str}，直接攻击但没有目标，是否未配置MoveToTargetPos节点");
            else
            {
                bool isPlayBehave = true;
                bool isConverAttack = cbace.m_SourcesTurnBehaveSkillInfo != null ? cbace.m_SourcesTurnBehaveSkillInfo.IsConverAttack : false;
                if (isConverAttack && cbace.m_BehaveAIControllParam != null)
                {
                    NetExcuteTurnInfo excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnInfo(cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
                    if (excuteTurnData.CombineAttack_ScrIndex != 0)
                    {
                        WS_CombatBehaveAIManagerEntity wS_CombatBehaveAIManagerEntity = dataComponent.m_CurTarget.GetChildEntity<WS_CombatBehaveAIManagerEntity>();
                        if (wS_CombatBehaveAIManagerEntity != null && wS_CombatBehaveAIManagerEntity.m_MainStreamController != null)
                        {
                            WS_CombatBehaveAIControllerEntity wS_CombatBehaveAIControllerEntity = wS_CombatBehaveAIManagerEntity.m_MainStreamController as WS_CombatBehaveAIControllerEntity;
                            if (wS_CombatBehaveAIControllerEntity != null)
                            {
                                if (wS_CombatBehaveAIControllerEntity.m_BehaveAIControllParam != null)
                                {
                                    wS_CombatBehaveAIControllerEntity.m_BehaveAIControllParam.SrcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
                                }

                                wS_CombatBehaveAIControllerEntity.SkipFirstStateMachineCombatBehaveAIByState();
                            }
                        }

                        isPlayBehave = false;
                    }
                }
                if (isPlayBehave)
                {
                    BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                    behaveAIControllParam.SrcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
                    behaveAIControllParam.SkillId = cbace.m_BehaveAIControllParam == null ? 
                        (cbace.m_SkillTb == null ? 0u : cbace.m_SkillTb.id) : cbace.m_BehaveAIControllParam.SkillId;
                    behaveAIControllParam.TargetUnitId = dataComponent.m_CurTarget.m_MobCombatComponent.m_BattleUnit.UnitId;
                    behaveAIControllParam.TurnBehaveSkillTargetInfoIndex = turnBehaveSkillTargetInfoIndex;
                    if (cbace.m_BehaveAIControllParam != null)
                        behaveAIControllParam.ExcuteTurnIndex = cbace.m_BehaveAIControllParam.ExcuteTurnIndex;

                    if (isLogicWorkId && bulletWorkId > 0u)
                        dataComponent.m_CurTarget.m_MobCombatComponent.DoBehave(bulletWorkId, cbace.m_SkillTb, 1, cbace.m_SourcesTurnBehaveSkillInfo, behaveAIControllParam);
                    else
                        dataComponent.m_CurTarget.m_MobCombatComponent.DoBehave(cbace.m_SkillTb, 1, mob, cbace.m_SourcesTurnBehaveSkillInfo, behaveAIControllParam);

                    WS_CombatBehaveAIControllerEntity.BulletHitToProcess(mob, dataComponent.m_CurTarget,
                            cbace.m_BehaveAIControllParam == null ? -1 : cbace.m_BehaveAIControllParam.ExcuteTurnIndex,
                            cbace.m_SourcesTurnBehaveSkillInfo, turnBehaveSkillTargetInfoIndex, isCanProcessTypeAfterHit);
                }
            }

            if (m_CurUseEntity != null)
                m_CurUseEntity.TranstionMultiStates(this);
            
            return;
        }

        if (!isLogicWorkId)
        {
            string[] strs = CombatHelp.GetStrParse1Array(str);

            bulletId = uint.Parse(strs[0]);
            delayTime = float.Parse(strs[1]);
            bulletWorkId = 0u;
            if (strs.Length > 2)
            {
                uint bwi = uint.Parse(strs[2]);
                if (bwi > 0u)
                    bulletWorkId = bwi;
            }
        }
        
        if (turnBehaveSkillTargetInfo != null && turnBehaveSkillTargetInfo.TargetUnitId > 0)
        {
            var bulletTarget = MobManager.Instance.GetMob(turnBehaveSkillTargetInfo.TargetUnitId);
            int bulletTargetClientNum = bulletTarget == null ? 0 : bulletTarget.m_MobCombatComponent.m_ClientNum;

            if (bulletWorkId == 0u && cbace.m_SkillTb == null)
            {
                bulletWorkId = ((WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent).m_WorkId;
            }

            BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
            behaveAIControllParam.SrcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
            behaveAIControllParam.SkillId = cbace.m_BehaveAIControllParam == null ?
                (cbace.m_SkillTb == null ? 0u : cbace.m_SkillTb.id) : cbace.m_BehaveAIControllParam.SkillId;
            behaveAIControllParam.TargetUnitId = turnBehaveSkillTargetInfo.TargetUnitId;
            behaveAIControllParam.TurnBehaveSkillTargetInfoIndex = turnBehaveSkillTargetInfoIndex;
            if (cbace.m_BehaveAIControllParam != null)
                behaveAIControllParam.ExcuteTurnIndex = cbace.m_BehaveAIControllParam.ExcuteTurnIndex;

            BulletManager.Instance.CreateBullet(bulletId, cbace.m_SkillTb, cbace.m_SourcesTurnBehaveSkillInfo, behaveAIControllParam, 
                mob, bulletTarget, bulletTargetClientNum, null, delayTime, bulletWorkId, isCanProcessTypeAfterHit);
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}