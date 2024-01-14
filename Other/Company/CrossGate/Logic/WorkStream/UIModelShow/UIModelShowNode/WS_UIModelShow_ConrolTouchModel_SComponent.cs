[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ConrolTouchModel)]
public class WS_UIModelShow_ConrolTouchModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        bool canTouch = int.Parse(str) == 1;

        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        uiModelShowManager.RecordControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnTouchModel, canTouch);

        m_CurUseEntity.TranstionMultiStates(this);
    }
}