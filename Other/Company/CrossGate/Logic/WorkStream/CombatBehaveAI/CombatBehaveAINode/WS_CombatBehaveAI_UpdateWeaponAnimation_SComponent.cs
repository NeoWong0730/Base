[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.UpdateWeaponAnimation)]
public class WS_CombatBehaveAI_UpdateWeaponAnimation_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint[] us = CombatHelp.GetStrParseUint1Array(str);

        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            if (kv.Value == null || kv.Value.m_MobCombatComponent == null)
                continue;
            
            if (kv.Value.m_MobCombatComponent.m_ClientNum == (int)us[0])
            {
                kv.Value.m_MobCombatComponent.m_AnimationComponent.StopAll();
                //kv.Value.m_MobCombatComponent.m_AnimationComponent.SetAnimationEnable(false);
                kv.Value.m_MobCombatComponent.m_AnimationComponent.UpdateHoldingAnimations(us[1], us[2]);
                break;
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}