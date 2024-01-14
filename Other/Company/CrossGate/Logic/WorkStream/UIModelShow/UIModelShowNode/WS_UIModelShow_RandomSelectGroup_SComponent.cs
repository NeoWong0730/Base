using UnityEngine;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.RandomSelectGroup)]
public class WS_UIModelShow_RandomSelectGroup_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this, 1, Random.Range(0, int.Parse(str)));
	}
}