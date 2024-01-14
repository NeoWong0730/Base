using Logic;
using Packet;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.ShowHitEffectTypeFont)]
public class WS_CombatBehaveAI_ShowHitEffectTypeFont_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
        BattleUnit unit = mob.m_MobCombatComponent.m_BattleUnit;

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
        
        TurnBehaveSkillTargetInfo turnBehaveSkillTargetInfo = dataComponent.GetTurnBehaveSkillTargetInfo(cbace, mob);

        int hitEffect = turnBehaveSkillTargetInfo != null ? (int)turnBehaveSkillTargetInfo.HitEffect :
            (cbace.m_BehaveAIControllParam == null ? 0 : cbace.m_BehaveAIControllParam.HitEffect);

        TriggerAnimEvt triggerAnimEvt = CombatObjectPool.Instance.Get<TriggerAnimEvt>();
        triggerAnimEvt.id = unit.UnitId;
        if(hitEffect == (int)HitEffectType.Mana)
            triggerAnimEvt.AnimType = AnimType.e_MagicShort;
        else if(hitEffect == (int)HitEffectType.Energe)
            triggerAnimEvt.AnimType = AnimType.e_EnergyShort;
        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerAnim, triggerAnimEvt);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}