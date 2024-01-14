using System.Collections.Generic;
using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayGhostShadow)]
public class WS_CombatBehaveAI_PlayGhostShadow_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (!string.IsNullOrEmpty(str))
        {
            float[] fs = CombatHelp.GetStrParseFloat1Array(str);

            ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).
                GetNeedComponent<PlayGhostShadowComponent>().Init(fs[0], fs[1], fs.Length > 2 ? (int)fs[2] : 0);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}