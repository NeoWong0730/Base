[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoSkillBehave)]
public class WS_CombatBehaveAI_DoSkillBehave_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        MobCombatComponent mobCombatComponent = ((MobEntity)cbace.m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

        uint[] us = CombatHelp.GetStrParseUint1Array(str);

        int attachType = (int)us[0];
        uint skillId = us[1];

        BehaveAIControllParam behaveAIControllParam = BehaveAIControllParam.DeepClone(cbace.m_BehaveAIControllParam);
        behaveAIControllParam.SkillId = skillId;
        if (!mobCombatComponent.StartBehave(skillId, cbace.m_SourcesTurnBehaveSkillInfo, false,
            behaveAIControllParam, attachType))
        {
            m_CurUseEntity.TranstionMultiStates(this);
        }
    }
}