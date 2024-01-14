using Logic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SwitchCombatScene)]
public class WS_CombatBehaveAI_SwitchCombatScene_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        Sys_Fight.Instance.UpdateFightScene(uint.Parse(str), () =>
        {
            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                var mob = kv.Value;
                if (mob == null || mob.m_MobCombatComponent == null || mob.m_MobCombatComponent.Id == 0 || mob.m_MobCombatComponent.m_Death)
                    continue;

                mob.m_MobCombatComponent.RefreshPos();
            }

            m_CurUseEntity.TranstionMultiStates(this);
        });
	}
}