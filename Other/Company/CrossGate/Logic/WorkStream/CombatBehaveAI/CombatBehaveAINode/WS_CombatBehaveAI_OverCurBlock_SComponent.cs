[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.OverCurBlock)]
public class WS_CombatBehaveAI_OverCurBlock_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;
        wstc.OverCurBlock();
    }
}