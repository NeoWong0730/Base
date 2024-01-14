[StateComponent((int)StateCategoryEnum.CommunalAI, (int)CommunalAIEnum.ShowModel)]
public class WS_CommunalAI_ShowModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CommunalAIManagerEntity communalAIManagerEntity = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_CommunalAIManagerEntity;
        if (communalAIManagerEntity.m_Go != null)
            communalAIManagerEntity.m_Go.SetActive(int.Parse(str) == 1);

        m_CurUseEntity.TranstionMultiStates(this);
    }
}