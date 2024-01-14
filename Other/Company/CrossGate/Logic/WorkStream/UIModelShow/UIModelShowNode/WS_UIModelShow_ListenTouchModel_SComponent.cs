[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ListenTouchModel)]
public class WS_UIModelShow_ListenTouchModel_SComponent : StateBaseComponent
{
    public override void Dispose()
    {
        if (m_CurUseEntity != null && m_CurUseEntity.m_StateControllerEntity != null)
        {
            WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
            uiModelShowManager.m_TouchModelAction = null;
        }
        
        base.Dispose();
    }

    public override void Init(string str)
	{
        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        uiModelShowManager.m_TouchModelAction = TouchModelAction_evt;
	}

    private void TouchModelAction_evt()
    {
        if (m_CurUseEntity == null)
            return;
        
        m_CurUseEntity.TranstionMultiStates(this);
    }
}