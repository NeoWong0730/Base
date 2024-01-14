using Lib.Core;
using Logic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ShowDmg)]
public class WS_CombatBehaveAI_ShowDmg_SComponent : StateBaseComponent
{
    public override void Init(string str)
    {
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobEntity mob = (MobEntity)behaveAIController.m_WorkStreamManagerEntity.Parent;
        MobCombatComponent mobCombatComponent = mob.m_MobCombatComponent;
        Packet.BattleUnit unit = mobCombatComponent.m_BattleUnit;

        if (behaveAIController.m_AttachType == 1 && 
            behaveAIController.m_BehaveAIControllParam != null && behaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex > -1)
        {
            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(behaveAIController, mob);
            if (turnBehaveSkillTargetInfo != null)
            {
                Net_Combat.Instance.DoBuffChangeData(behaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex, turnBehaveSkillTargetInfo, 6);
            }
        }
        
        if (behaveAIController.m_CurHpChangeData == null)
        {
            if (mobCombatComponent.m_HpChangeDataQueue.Count == 0)
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            behaveAIController.m_CurHpChangeData = mobCombatComponent.m_HpChangeDataQueue.Dequeue();
#if DEBUG_MODE
            DLogManager.Log(ELogType.eCombat, $"HpChangeDataQueue.Dequeue-----<color=yellow>{mobCombatComponent.m_CurUseEntity.m_Go?.name}--Count:{mobCombatComponent.m_HpChangeDataQueue.Count.ToString()}    {MobCombatComponent.GetHpChangeDataDebugLog(behaveAIController.m_CurHpChangeData)}</color>");
#endif
        }

        if (behaveAIController.m_CurHpChangeData.m_Revive)
        {
            Net_Combat.Instance.CreateBloodHub(unit, mobCombatComponent.m_ClientNum, mobCombatComponent.m_CurUseEntity.m_Go);
        }

        bool isCanShowDmg = true;

        int strType = 0;
        if (!string.IsNullOrWhiteSpace(str))
        {
            if (!int.TryParse(str, out strType))
                isCanShowDmg = false;
        }
        
        if (behaveAIController.m_CurHpChangeData.m_ChangeType == 0 || behaveAIController.m_CurHpChangeData.m_ChangeType == 2 ||
            behaveAIController.m_CurHpChangeData.m_ChangeType == 4)
        {
            if (!CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Miss) &&
                    !CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Error))
            {
                Net_Combat.Instance.UpdateHp(unit, behaveAIController.m_CurHpChangeData.m_CurHp, unit.MaxHp);
                if (behaveAIController.m_CurHpChangeData.m_ChangeType == 0 && behaveAIController.m_CurHpChangeData.m_HpChange == 0)
                    isCanShowDmg = false;
            }
        }

        if (behaveAIController.m_CurHpChangeData.m_ChangeType == 3 || behaveAIController.m_CurHpChangeData.m_ChangeType == 4)
        {
            if (!CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Miss) &&
                    !CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Error))
            {
                Net_Combat.Instance.UpdateShield(unit, behaveAIController.m_CurHpChangeData.m_CurShield, unit.MaxShield);
            }
        }

        if (behaveAIController.m_CurHpChangeData.m_ChangeType == 5)
        {
            if (!CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Miss) &&
                    !CombatHelp.ContainAnimType(behaveAIController.m_CurHpChangeData.m_AnimType, AnimType.e_Error))
            {
                Net_Combat.Instance.UpdateGas(unit, behaveAIController.m_CurHpChangeData.m_CurGas, unit.MaxGas);
            }
        }

        //伤害飘字
        MobDeadComponent mobDeadComponent = m_CurUseEntity.GetComponent<MobDeadComponent>();
        if (isCanShowDmg && (mobDeadComponent == null || behaveAIController.m_CurHpChangeData.m_Revive) &&
            behaveAIController.m_CurHpChangeData.m_ChangeType > -1)
        {
            int triggerCount = 1;
            if (behaveAIController.m_CurHpChangeData.m_ChangeType == 2 || behaveAIController.m_CurHpChangeData.m_ChangeType == 4)
                triggerCount = 2;
            for (int triggerIndex = 0; triggerIndex < triggerCount; triggerIndex++)
            {
                TriggerAnimEvt triggerAnimEvt = CombatObjectPool.Instance.Get<TriggerAnimEvt>();
                triggerAnimEvt.id = unit.UnitId;

                AnimType animType = behaveAIController.m_CurHpChangeData.m_AnimType;

                if (behaveAIController.m_CurHpChangeData.m_ChangeType == 0)
                    triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_HpChange;
                else if (behaveAIController.m_CurHpChangeData.m_ChangeType == 1)
                    triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_MpChange;
                else if (behaveAIController.m_CurHpChangeData.m_ChangeType == 2)
                {
                    if (triggerIndex == 0)
                    {
                        triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_HpChange;
                        animType &= ~(AnimType.e_AddMp | AnimType.e_DeductMp | AnimType.e_ExtractMp);
                    }
                    else
                    {
                        triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_MpChange;
                        animType &= (AnimType.e_AddMp | AnimType.e_DeductMp | AnimType.e_ExtractMp);
                    }
                }
                else if (behaveAIController.m_CurHpChangeData.m_ChangeType == 3)
                    triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_ShieldChange;
                else if (behaveAIController.m_CurHpChangeData.m_ChangeType == 4)
                {
                    if (triggerIndex == 0)
                    {
                        triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_HpChange;
                    }
                    else
                    {
                        triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_ShieldChange;
                    }
                }
                else if (behaveAIController.m_CurHpChangeData.m_ChangeType == 5)
                {
                    triggerAnimEvt.finnaldamage = behaveAIController.m_CurHpChangeData.m_GasChange;
                }

                triggerAnimEvt.floatingdamage = behaveAIController.m_CurHpChangeData.m_damageAddon;
                if (strType == 0)
                {
                    triggerAnimEvt.AnimType = animType;
                    triggerAnimEvt.playType = 0u;
                }
                else if (strType > 0)
                {
                    triggerAnimEvt.AnimType = SwitchAnimType(animType, (AnimType)(1 << strType));
                    triggerAnimEvt.playType = 0u;
                }
                else
                {
                    triggerAnimEvt.AnimType = animType;
                    triggerAnimEvt.playType = (uint)(-strType);
                }

                triggerAnimEvt.converCount = behaveAIController.m_CurHpChangeData.m_ConverAttackCount;
                triggerAnimEvt.IsEnemy = !CombatHelp.IsSameCamp(mobCombatComponent.m_BattleUnit, MobManager.Instance.GetPlayerBattleUnit());

                MobEntity srcMob = MobManager.Instance.GetMob(behaveAIController.m_CurHpChangeData.m_SrcUnitId);
                if (srcMob != null && srcMob.m_MobCombatComponent != null &&
                    srcMob.m_MobCombatComponent.m_BattleUnit != null)
                {
                    triggerAnimEvt.attackInfoId = srcMob.m_MobCombatComponent.m_BattleUnit.UnitInfoId;
                    triggerAnimEvt.attackType = CombatHelp.SwitchCombatUnitType(srcMob.m_MobCombatComponent.m_BattleUnit.UnitType);
                    triggerAnimEvt.race_attack = srcMob.m_MobCombatComponent.m_BattleUnit.Race;
                }

                triggerAnimEvt.hitInfoId = mobCombatComponent.m_BattleUnit.UnitInfoId;
                triggerAnimEvt.hitType = CombatHelp.SwitchCombatUnitType(mobCombatComponent.m_BattleUnit.UnitType);
                triggerAnimEvt.race_hit = mobCombatComponent.m_BattleUnit.Race;
                Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerAnim, triggerAnimEvt);
            }
        }
        
        if (behaveAIController.m_CurHpChangeData.m_ChangeType == 1 || behaveAIController.m_CurHpChangeData.m_ChangeType == 2)
        {
            Net_Combat.Instance.UpdateMp(unit, behaveAIController.m_CurHpChangeData.m_CurMp, unit.MaxMp);
            if (behaveAIController.m_CurHpChangeData.m_ChangeType == 1)
            {
                if (behaveAIController.m_CurHpChangeData != null)
                {
                    CombatHpChangeData.Push(behaveAIController.m_CurHpChangeData);
                }
                behaveAIController.m_CurHpChangeData = null;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}

    private AnimType SwitchAnimType(AnimType animType, AnimType switchAnimType)
    {
        if (switchAnimType == AnimType.e_ExtractMp)
        {
            if ((animType & AnimType.e_DeductMp) > 0)
            {
                animType = animType & (~AnimType.e_DeductMp);
                animType |= switchAnimType;
            }
        }

        return animType;
    }
}