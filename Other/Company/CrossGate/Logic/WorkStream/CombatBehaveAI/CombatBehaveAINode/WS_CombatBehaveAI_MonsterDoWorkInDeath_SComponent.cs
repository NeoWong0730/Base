using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MonsterDoWorkInDeath)]
public class WS_CombatBehaveAI_MonsterDoWorkInDeath_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mob.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
        if (cSVMonsterData == null || cSVMonsterData.dead_behavior == 0u)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = mob.GetChildEntity<WS_CombatBehaveAIManagerEntity>();
        if (combatBehaveAIManagerEntity != null)
        {
            if (combatBehaveAIManagerEntity.StartWorkId(cSVMonsterData.dead_behavior, 0, null, null,
                null, 0, StartControllerStyleEnum.Parallel))
            {
                m_CurUseEntity.StateMachineOver();
                return;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}