using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 创建勇士团功能///
    /// </summary>
    public class CreateWarriorGroupFunction : FunctionBase
    {
        protected override bool CanExecute(bool CheckVisual = true)
        {
            return Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID == 0;
        }

        protected override void OnCantExecTip()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13538));
        }

        protected override void OnExecute()
        {
            UIManager.OpenUI(EUIID.UI_CreateWarriorGroup);
        }
    }
}