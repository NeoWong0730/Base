using Logic;
using Table;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ShowSkillContent)]
public class WS_CombatBehaveAI_ShowSkillContent_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        Packet.BattleUnit unit = ((MobEntity)cbace.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent.m_BattleUnit;

        //技能名称飘字
        TriggerSkillEvt triggerSkillEvt = CombatObjectPool.Instance.Get<TriggerSkillEvt>();
        triggerSkillEvt.id = unit.UnitId;
        triggerSkillEvt.clientNum = CombatHelp.ServerToClientNum(unit.Pos, CombatManager.Instance.m_IsNotMirrorPos);

        if (cbace.m_SkillTb != null && cbace.m_SkillTb.show_skill_name != 0u)
        {
            CSVLanguage.Data lanTb = CSVLanguage.Instance.GetConfData(cbace.m_SkillTb.show_skill_name);
            if (lanTb != null)
                triggerSkillEvt.skillcontent = lanTb.words;
            else
                triggerSkillEvt.skillcontent = string.Empty;
        }
        else
            triggerSkillEvt.skillcontent = string.Empty;

        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerSkill, triggerSkillEvt);
        CombatObjectPool.Instance.Push(triggerSkillEvt);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}