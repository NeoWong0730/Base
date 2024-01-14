[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.EnableAnimation)]
public class WS_CombatBehaveAI_EnableAnimation_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        int[] ints = CombatHelp.GetStrParseInt1Array(str);

        if (ints.Length > 1)
        {
            foreach (var kv in MobManager.Instance.m_MobDic)
            {
                MobEntity mob = kv.Value;
                if (mob == null || mob.m_MobCombatComponent == null)
                    continue;

                if (mob.m_MobCombatComponent.m_ClientNum == ints[1])
                {
                    mob.m_MobCombatComponent.m_AnimationComponent.SetAnimationEnable(ints[0] == 1);
                    break;
                }
            }
        }
        else
        {
            MobEntity mob = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
            mob.m_MobCombatComponent.m_AnimationComponent.SetAnimationEnable(ints[0] == 1);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}