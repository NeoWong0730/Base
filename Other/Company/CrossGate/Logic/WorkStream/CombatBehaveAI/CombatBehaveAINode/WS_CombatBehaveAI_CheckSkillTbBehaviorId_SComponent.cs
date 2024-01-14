[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CheckSkillTbBehaviorId)]
public class WS_CombatBehaveAI_CheckSkillTbBehaviorId_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        if(cbace == null || cbace.m_SkillTb == null)
        {
            m_CurUseEntity.TranstionMultiStates(this, 1, 0);
            return;
        }

        uint skillBehaviorId = uint.Parse(str);
        m_CurUseEntity.TranstionMultiStates(this, 1, cbace.m_SkillTb.active_skill_behavior_id == skillBehaviorId ? 1 : 0);
    }
}