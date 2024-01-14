[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CancelProcessAfterHit)]
public class WS_CombatBehaveAI_CancelProcessAfterHit_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        if (string.IsNullOrWhiteSpace(str))
            dataComponent.m_IsCanProcessTypeAfterHit = int.MaxValue;
        else
            dataComponent.m_IsCanProcessTypeAfterHit = int.Parse(str);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}