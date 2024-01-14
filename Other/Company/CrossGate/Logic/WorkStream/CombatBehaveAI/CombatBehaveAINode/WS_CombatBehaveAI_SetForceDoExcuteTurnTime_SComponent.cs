[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SetForceDoExcuteTurnTime)]
public class WS_CombatBehaveAI_SetForceDoExcuteTurnTime_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        Net_Combat.Instance.SetForceDoExcuteTurnTime(float.Parse(str));

        m_CurUseEntity.m_StateMachineOverAction += SetStateMachineOverAction;
        
        m_CurUseEntity.TranstionMultiStates(this);
	}

    public void SetStateMachineOverAction()
    {
        Net_Combat.Instance.SetForceDoExcuteTurnTime(5f);
    }
}