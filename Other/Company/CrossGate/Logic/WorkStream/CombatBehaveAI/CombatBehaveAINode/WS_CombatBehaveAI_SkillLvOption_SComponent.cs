[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SkillLvOption)]
public class WS_CombatBehaveAI_SkillLvOption_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var skillTb = ((WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_SkillTb;
        int selectType = -1;
        if (skillTb != null)
            selectType = (int)skillTb.active_skill_lv;

        m_CurUseEntity.TranstionMultiStates(this, 1, selectType);
    }
}