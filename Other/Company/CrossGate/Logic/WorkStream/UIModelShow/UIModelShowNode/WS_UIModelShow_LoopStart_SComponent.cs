[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.LoopStart)]
public class WS_UIModelShow_LoopStart_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        m_CurUseEntity.GetNeedComponent<WS_UIModelShowDataComponent>().Mark(((WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent).GetWorkNodeDataFromCurWorkNodeDataList(m_DataNodeId), str);

		m_CurUseEntity.TranstionMultiStates(this);
	}
}