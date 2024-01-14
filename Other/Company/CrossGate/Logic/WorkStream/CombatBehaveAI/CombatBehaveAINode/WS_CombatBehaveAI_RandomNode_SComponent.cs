using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RandomNode)]
public class WS_CombatBehaveAI_RandomNode_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        int[] ints = CombatHelp.GetStrParseInt1Array(str, StrParseEnum.VerticalLine);
        int minR = ints[0];
        int maxR = ints[1];

        int selectType = minR;
        if (minR != maxR)
            selectType = Random.Range(minR, maxR + 1);
        
		m_CurUseEntity.TranstionMultiStates(this, 1, selectType);
	}
}