[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.JumpToWorkStreamBlock)]
public class WS_CombatBehaveAI_JumpToWorkStreamBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        int[] us = CombatHelp.GetStrParseInt1Array(str, StrParseEnum.VerticalLine);
        int attachType = us[0];
        int blockType = us[1];

        WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;

        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

        MobEntity mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        WS_CombatBehaveAIDataComponent data = m_CurUseEntity.GetComponent<WS_CombatBehaveAIDataComponent>();
        if (data == null || data.Target == null || data.Target == mob)
        {
            m_CurUseEntity.SkipBlock(blockType);
            return;
        }

        WS_CombatBehaveAIManagerEntity combatBehaveAIManagerEntity = data.Target.GetNeedChildEntity<WS_CombatBehaveAIManagerEntity>();
        BehaveAIControllParam behaveAIControllParam = BehaveAIControllParam.DeepClone(cbace.m_BehaveAIControllParam);
        combatBehaveAIManagerEntity.StartWorkId(wstc.m_WorkId, attachType, cbace.m_SkillTb, cbace.m_SourcesTurnBehaveSkillInfo, behaveAIControllParam, blockType);

        m_CurUseEntity.TranstionMultiStates(this);
    }
}