[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ConrolRotateModel)]
public class WS_UIModelShow_ConrolRotateModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        bool canRotate = int.Parse(str) == 1;

        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        uiModelShowManager.RecordControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnRotateModel, canRotate);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}