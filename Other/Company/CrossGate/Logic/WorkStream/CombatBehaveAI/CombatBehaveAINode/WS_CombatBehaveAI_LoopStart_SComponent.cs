[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.LoopStart)]
public class WS_CombatBehaveAI_LoopStart_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        m_CurUseEntity.GetNeedComponent<WS_CombatBehaveAIDataComponent>().Mark(m_DataNodeId, str);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}