using Logic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CameraShake)]
public class WS_CombatBehaveAI_CameraShake_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (!string.IsNullOrEmpty(str))
            GameCenter.ShakeCamera(uint.Parse(str));

        m_CurUseEntity.TranstionMultiStates(this);
	}
}