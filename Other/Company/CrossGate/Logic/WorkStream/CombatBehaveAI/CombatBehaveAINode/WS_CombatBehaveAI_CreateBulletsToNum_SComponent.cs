using Logic;
using Packet;
using System.Collections.Generic;
using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CreateBulletsToNum)]
public class WS_CombatBehaveAI_CreateBulletsToNum_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity combatBehaveAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatBehaveAIControllerEntity;
        if (combatBehaveAIController == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;;
        }

        CSVActiveSkill.Data skillTb = combatBehaveAIController.m_SkillTb;
        MobEntity mob = (MobEntity)combatBehaveAIController.m_WorkStreamManagerEntity.Parent;
        TurnBehaveSkillInfo turnBehaveSkillInfo = combatBehaveAIController.m_SourcesTurnBehaveSkillInfo;

        if (turnBehaveSkillInfo != null && turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null)
        {
            string[] strs = CombatHelp.GetStrParse1Array(str);

            uint bulletId = uint.Parse(strs[0]);
            float delayTime = float.Parse(strs[1]);

            uint bulletWorkId = 0u;
            if (skillTb == null)
            {
                bulletWorkId = ((WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent).m_WorkId;
            }

            float dt = -delayTime;
            for (int i = 0, count = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList.Count; i < count; i++)
            {
                TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = turnBehaveSkillInfo.TurnBehaveSkillTargetInfoList[i];
                if (turnBehaveSkillTargetInfo == null || turnBehaveSkillTargetInfo.CallingTargetBattleUnit == null)
                    continue;

                dt += delayTime;

                var targetBattleUnit = turnBehaveSkillTargetInfo.CallingTargetBattleUnit;

                BehaveAIControllParam behaveAIControllParam = BasePoolClass.Get<BehaveAIControllParam>();
                if (mob != null)
                    behaveAIControllParam.SrcUnitId = mob.m_MobCombatComponent.m_BattleUnit.UnitId;
                if (combatBehaveAIController != null)
                {
                    behaveAIControllParam.SkillId = combatBehaveAIController.m_BehaveAIControllParam == null ?
                        (combatBehaveAIController.m_SkillTb == null ? 0u : combatBehaveAIController.m_SkillTb.id) : 
                        combatBehaveAIController.m_BehaveAIControllParam.SkillId;

                    if (combatBehaveAIController.m_BehaveAIControllParam != null)
                    {
                        behaveAIControllParam.ExcuteTurnIndex = combatBehaveAIController.m_BehaveAIControllParam.ExcuteTurnIndex;
                        if (behaveAIControllParam.ExcuteTurnIndex > -1)
                        {
                            var excuteTurnData = Net_Combat.Instance.GetNetExcuteTurnData(behaveAIControllParam.ExcuteTurnIndex);
                            if (excuteTurnData != null && excuteTurnData.ExcuteData != null && excuteTurnData.ExcuteData.Count > 0 &&
                                turnBehaveSkillTargetInfo.ExcuteDataIndex > -1 && 
                                turnBehaveSkillTargetInfo.ExcuteDataIndex < excuteTurnData.ExcuteData.Count)
                            {
                                ExcuteData excuteData = excuteTurnData.ExcuteData[turnBehaveSkillTargetInfo.ExcuteDataIndex];
                                if (excuteData != null && excuteData.UnitsChange != null)
                                {
                                    behaveAIControllParam.m_BattleReplaceType = excuteData.UnitsChange.ReplaceType;
                                }
                            }
                        }
                        
                    }
                }
                behaveAIControllParam.TargetUnitId = targetBattleUnit.UnitId;
                behaveAIControllParam.TurnBehaveSkillTargetInfoIndex = i;

                BulletManager.Instance.CreateBullet(bulletId, skillTb, turnBehaveSkillInfo, behaveAIControllParam, mob, null, CombatHelp.ServerToClientNum(targetBattleUnit.Pos, CombatManager.Instance.m_IsNotMirrorPos), delegate (MobEntity attack, MobEntity target, BulletEntity bulletEntity)
                {
                    if (Sys_Fight.Instance != null && targetBattleUnit != null)
                    {
                        Net_Combat.Instance.CreateNewUnit(turnBehaveSkillInfo,
                                bulletEntity.m_BehaveAIControllParam, bulletEntity.m_BehaveAIControllParam.m_BattleReplaceType, 
                                targetBattleUnit);
                    }
                }, dt, bulletWorkId);
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}