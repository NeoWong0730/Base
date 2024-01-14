using Lib.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RemoveFightBackHinder)]
public class WS_CombatBehaveAI_RemoveFightBackHinder_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = WS_CombatBehaveAIControllerEntity.GetTurnBehaveSkillTargetInfo(m_CurUseEntity);
        if (turnBehaveSkillTargetInfo != null)
        {
#if DEBUG_MODE
            if (cbace.m_AttachType == 1 && cbace.m_CurHpChangeData == null)
                DebugUtil.LogError($"{mob.m_Go?.name}目标执行FightBackIsOver时没有HpChangeData数据");
#endif

            uint fightBackUnitId = cbace.m_AttachType == 0 ? mob.m_MobCombatComponent.m_BattleUnit.UnitId : cbace.m_CurHpChangeData.m_SrcUnitId;

            string[] strs = CombatHelp.GetStrParse1Array(str);

            CombatManager.Instance.m_EventEmitter.Trigger(CombatManager.EEvents.WaitFightBackOver, 
                turnBehaveSkillTargetInfo.TargetUnitId, fightBackUnitId, strs.Length > 1 ? uint.Parse(strs[1]) : 0u, strs[0]);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}