[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.OverShow)]
public class WS_CombatBehaveAI_OverShow_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        Net_Combat.Instance.DoBattleResult();

        if(m_CurUseEntity != null)
            m_CurUseEntity.TranstionMultiStates(this);
	}
}