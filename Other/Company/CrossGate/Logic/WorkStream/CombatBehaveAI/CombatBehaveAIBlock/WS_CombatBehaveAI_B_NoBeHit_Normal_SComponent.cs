﻿[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_NoBeHit_Normal)]
public class WS_CombatBehaveAI_B_NoBeHit_Normal_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}