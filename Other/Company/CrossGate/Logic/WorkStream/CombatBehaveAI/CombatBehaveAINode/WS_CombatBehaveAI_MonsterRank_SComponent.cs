using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MonsterRank)]
public class WS_CombatBehaveAI_MonsterRank_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(mob.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
        if (cSVMonsterData == null)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        m_CurUseEntity.TranstionMultiStates(this, 1, (int)cSVMonsterData.monster_behavior_rank);
    }
}