using Logic;
using Logic.Core;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayCutScene)]
public class WS_CombatBehaveAI_PlayCutScene_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (UIManager.IsVisibleAndOpen(EUIID.UI_MainBattle)) {
            UIManager.CloseUI(EUIID.UI_MainBattle);
        }
        Sys_CutScene.Instance.TryDoCutScene(uint.Parse(str));

        m_CurUseEntity.TranstionMultiStates(this);
	}
}