[StateComponent((int)StateCategoryEnum.CommunalAI, (int)CommunalAIEnum.CommonBlock)]
public class WS_CommunalAI_CommonBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}