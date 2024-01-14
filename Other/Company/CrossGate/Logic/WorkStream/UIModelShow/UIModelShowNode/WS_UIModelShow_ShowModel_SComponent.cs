[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ShowModel)]
public class WS_UIModelShow_ShowModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        if (uiModelShowManager.m_Go != null)
            uiModelShowManager.m_Go.SetActive(int.Parse(str) == 1);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}