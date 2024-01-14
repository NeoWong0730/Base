using System.Collections.Generic;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.CreateChildWorkStream)]
public class WS_UIModelShow_CreateChildWorkStream_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint workId = uint.Parse(str);
        
        if (!((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).StartChildMachine(this, workId, 0))
            m_CurUseEntity.TranstionMultiStates(this);
    }
}