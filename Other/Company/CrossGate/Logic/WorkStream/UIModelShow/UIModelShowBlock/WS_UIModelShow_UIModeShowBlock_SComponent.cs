[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.UIModeShowBlock)]
public class WS_UIModelShow_UIModeShowBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}