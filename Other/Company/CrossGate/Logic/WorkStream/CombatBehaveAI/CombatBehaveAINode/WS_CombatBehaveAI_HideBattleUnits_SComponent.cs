[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.HideBattleUnits)]
public class WS_CombatBehaveAI_HideBattleUnits_SComponent : StateBaseComponent
{
    public override void Init(string str)
    {
        MobEntity mobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;

        int type = int.Parse(str);

        if (type == 0)
            MobManager.Instance.HideMobEntitys();
        else if (type == 1)
        {
            MobManager.Instance.HideMobEntitys(mobEntity.m_MobCombatComponent.m_BattleUnit.Side, true);
        }
        else
        {
            MobManager.Instance.HideMobEntitys(mobEntity.m_MobCombatComponent.m_BattleUnit.Side, false);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}