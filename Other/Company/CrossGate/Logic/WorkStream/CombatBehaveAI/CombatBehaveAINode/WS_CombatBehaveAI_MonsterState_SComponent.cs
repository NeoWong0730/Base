using Packet;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.MonsterState)]
public class WS_CombatBehaveAI_MonsterState_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint tbId = uint.Parse(str);

        MobCombatComponent mobCombatComponent = null;
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            MobEntity me = kv.Value;
            if (me == null || me.m_MobCombatComponent == null)
                continue;

            if (me.m_MobCombatComponent.m_BattleUnit.UnitInfoId == tbId)
            {
                mobCombatComponent = me.m_MobCombatComponent;
                break;
            }
        }

        if (mobCombatComponent != null)
        {
            WS_CombatBehaveAIControllerEntity behaveAIController = m_CurUseEntity.m_StateControllerEntity as WS_CombatBehaveAIControllerEntity;
            if (behaveAIController != null)
            {
                if ((behaveAIController.m_CurHpChangeData != null && behaveAIController.m_CurHpChangeData.m_Death) ||
                    (behaveAIController.m_CurHpChangeData == null && mobCombatComponent.m_Death))
                {
                    m_CurUseEntity.TranstionMultiStates(this, 1, 0);
                }
                else
                {
                    m_CurUseEntity.TranstionMultiStates(this, 1, 1);
                }
            }
            else
            {
                m_CurUseEntity.TranstionMultiStates(this, 1, mobCombatComponent.m_Death ? 0 : 1);
            }

            return;
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
    }
}