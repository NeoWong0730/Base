using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SkillShout)]
public class WS_CombatBehaveAI_SkillShout_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (Time.time < CombatManager.Instance.m_SkillShoutStartTime)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        uint[] us = CombatHelp.GetStrParseUint1Array(str);
        int ratio = (int)us[2];
        if (Random.Range(0, 101) > ratio)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }

        CombatManager.Instance.m_SkillShoutStartTime = Time.time + us[1] * 0.001f;
        AudioUtil.PlayAudio(us[0]);
    }
}