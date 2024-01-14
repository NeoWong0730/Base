[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.DoUpdateBehaveCount)]
public class WS_CombatBehaveAI_DoUpdateBehaveCount_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        MobEntity typeMob = WS_CombatBehaveAIControllerEntity.GetMobByType(m_CurUseEntity, int.Parse(str));
        if (typeMob != null)
        {
            typeMob.m_MobCombatComponent.UpdateReadlyBehaveCount(-1);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}