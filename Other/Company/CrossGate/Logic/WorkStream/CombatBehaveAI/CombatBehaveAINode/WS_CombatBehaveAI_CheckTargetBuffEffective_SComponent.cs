[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CheckTargetBuffEffective)]
public class WS_CombatBehaveAI_CheckTargetBuffEffective_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        int buffEffective = int.Parse(str);

        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        MobBuffComponent buffComponent = mob.GetComponent<MobBuffComponent>();

        m_CurUseEntity.TranstionMultiStates(this, 1, (buffComponent != null && buffComponent.CheckExistBuffEffective(buffEffective)) ? 1 : 0);
	}
}