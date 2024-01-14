using System.Collections.Generic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CreateChildWorkStream)]
public class WS_CombatBehaveAI_CreateChildWorkStream_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint workId;
        int attachType = 0;
        if (!uint.TryParse(str, out workId))
        {
            uint[] us = CombatHelp.GetStrParseUint1Array(str);
            workId = us[0];
            attachType = (int)us[1];
        }

        if (!((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).StartChildMachine(this, workId, attachType))
            m_CurUseEntity.TranstionMultiStates(this);
    }
}