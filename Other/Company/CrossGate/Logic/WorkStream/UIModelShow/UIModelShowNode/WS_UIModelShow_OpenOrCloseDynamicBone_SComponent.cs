[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.OpenOrCloseDynamicBone)]
public class WS_UIModelShow_OpenOrCloseDynamicBone_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        if (uiModelShowManager.m_BehaviourCollector == null && uiModelShowManager.m_Go != null)
        {
            uiModelShowManager.m_BehaviourCollector = uiModelShowManager.m_Go.GetComponentInChildren<CP_BehaviourCollector>();
        }
        if (uiModelShowManager.m_BehaviourCollector != null)
        {
            uiModelShowManager.m_BehaviourCollector.Enable(int.Parse(str) == 1);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}