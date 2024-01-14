[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MatchMonsterId)]
public class WS_CombatBehaveAI_MatchMonsterId_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint tbId = uint.Parse(str);
        
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            MobEntity me = kv.Value;
            if (me == null || me.m_MobCombatComponent == null)
                continue;

            if (me.m_MobCombatComponent.m_BattleUnit.UnitInfoId == tbId)
            {
                m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                return;
            }
        }

        m_CurUseEntity.TranstionMultiStates(this, 1, 0);
    }
}