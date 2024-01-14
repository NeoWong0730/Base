[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.BackToLoopStart)]
public class WS_CombatBehaveAI_BackToLoopStart_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var nodeId = m_CurUseEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>().DequeueMarkNodeId(str);

        if (nodeId > 0)
            m_CurUseEntity.SkipState(this, nodeId);
        else
            m_CurUseEntity.TranstionMultiStates(this);
    }
}