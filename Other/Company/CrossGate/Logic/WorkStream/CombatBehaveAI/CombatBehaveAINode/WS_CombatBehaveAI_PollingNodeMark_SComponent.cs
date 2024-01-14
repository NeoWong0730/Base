[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PollingNodeMark)]
public class WS_CombatBehaveAI_PollingNodeMark_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();

#if DEBUG_MODE
        dataComponent.IsDoPollingNodeMarkNode = true;
#endif

        if (dataComponent.m_LoopNodeMarkCount == 0)
        {
            if (string.IsNullOrEmpty(str))
            {
                WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
                MobEntity mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
                int loopNodeMarkCount = 0;
                if (cbace.m_SourcesTurnBehaveSkillInfo != null && cbace.m_SourcesTurnBehaveSkillInfo.TurnBehaveSkillTargetInfoList != null)
                    loopNodeMarkCount = cbace.m_SourcesTurnBehaveSkillInfo.TargetUnitCount;
                dataComponent.m_LoopNodeMarkCount = loopNodeMarkCount;
            }
            else
            {
                dataComponent.m_LoopNodeMarkCount = int.Parse(str);
            }

            dataComponent.m_LoopNodeMarkNodeId = m_DataNodeId;

            WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;
            dataComponent.m_LoopWorkBlockType = wstc.m_CurWorkBlockData.CurWorkBlockType;
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}