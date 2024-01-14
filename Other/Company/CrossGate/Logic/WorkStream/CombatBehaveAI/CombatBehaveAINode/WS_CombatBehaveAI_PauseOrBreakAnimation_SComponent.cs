[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PauseOrBreakAnimation)]
public class WS_CombatBehaveAI_PauseOrBreakAnimation_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        string[] strs = CombatHelp.GetStrParse1Array(str);
        int style = int.Parse(strs[1]);
        if (style == 1)
            mob.m_MobCombatComponent.m_AnimationComponent.Pause(strs[0]);
        else
            mob.m_MobCombatComponent.m_AnimationComponent.RemovePause(strs[0]);
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}