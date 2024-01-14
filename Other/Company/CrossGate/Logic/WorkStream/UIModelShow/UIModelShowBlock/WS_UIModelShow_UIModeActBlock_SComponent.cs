[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.UIModeActBlock)]
public class WS_UIModelShow_UIModeActBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}