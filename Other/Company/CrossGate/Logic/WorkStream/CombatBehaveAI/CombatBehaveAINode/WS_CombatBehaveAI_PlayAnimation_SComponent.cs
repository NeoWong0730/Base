[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayAnimation)]
public class WS_CombatBehaveAI_PlayAnimation_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        //播放动作
        if (string.IsNullOrEmpty(str))
        {
            Lib.Core.DebugUtil.LogError($"播放动作节点动作填的为空");
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        string[] strs = CombatHelp.GetStrParse1Array(str);

        var mobCombatComponent = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_MobCombatComponent;

        if (strs[0] == mobCombatComponent.m_CurAnimationName && strs.Length > 1)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        mobCombatComponent.m_AnimationComponent.Stop(strs[0]);
        mobCombatComponent.m_AnimationComponent.CrossFade(strs[0], 0.1f);
        mobCombatComponent.m_CurAnimationName = strs[0];

        WS_CombatBehaveAIControllerEntity.PlayMoveAnimationAudio(m_CurUseEntity,
            mobCombatComponent.m_BattleUnit.UnitType,
            mobCombatComponent.m_BattleUnit.UnitInfoId,
            mobCombatComponent.m_WeaponId, strs[0]);

        m_CurUseEntity.TranstionMultiStates(this);
	}
}