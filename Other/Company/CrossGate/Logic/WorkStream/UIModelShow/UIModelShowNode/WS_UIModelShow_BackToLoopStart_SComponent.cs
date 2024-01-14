[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.BackToLoopStart)]
public class WS_UIModelShow_BackToLoopStart_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var nodeData = m_CurUseEntity.GetNeedComponent<WS_UIModelShowDataComponent>().DequeueMarkNodeId(str);

        if (nodeData != null)
        {
            ushort nodeId = m_DataNodeId;
            WorkStreamTranstionComponent workStreamTranstionComponent = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;
            Dispose();
            workStreamTranstionComponent.SkipStateByWorkNodeData(nodeId, nodeData);
        }
        else
            m_CurUseEntity.TranstionMultiStates(this);
	}
}