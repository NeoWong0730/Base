﻿[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.B_BeHit_DivineShield)]
public class WS_CombatBehaveAI_B_BeHit_DivineShield_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
		m_CurUseEntity.TranstionMultiStates(this);
	}
}