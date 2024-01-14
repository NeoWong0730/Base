using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.RotationYAngle)]
public class WS_CombatBehaveAI_RotationYAngle_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        var trans = ((MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent).m_Trans;
        trans.eulerAngles = new Vector3(trans.eulerAngles.x, trans.eulerAngles.y + float.Parse(str), trans.eulerAngles.z);
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}