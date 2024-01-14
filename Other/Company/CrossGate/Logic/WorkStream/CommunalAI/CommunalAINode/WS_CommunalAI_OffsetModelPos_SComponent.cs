using UnityEngine;

[StateComponent((int)StateCategoryEnum.CommunalAI, (int)CommunalAIEnum.OffsetModelPos)]
public class WS_CommunalAI_OffsetModelPos_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WS_CommunalAIManagerEntity communalAIManagerEntity = ((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity as WS_CommunalAIManagerEntity;
        if (communalAIManagerEntity.m_Go != null)
        {
            float[] fs = CombatHelp.GetStrParseFloat1Array(str);

            communalAIManagerEntity.m_Go.transform.position = new Vector3(communalAIManagerEntity.m_OriginPos.x + fs[0],
                communalAIManagerEntity.m_OriginPos.y + fs[1], communalAIManagerEntity.m_OriginPos.z + fs[2]);
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}