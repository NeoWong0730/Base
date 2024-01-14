[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.ConrolSwitchModel)]
public class WS_UIModelShow_ConrolSwitchModel_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        bool canSwitch = int.Parse(str) == 1;

        WS_UIModelShowManagerEntity uiModelShowManager = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_UIModelShowManagerEntity;
        uiModelShowManager.RecordControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnSwitchModel, canSwitch);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}