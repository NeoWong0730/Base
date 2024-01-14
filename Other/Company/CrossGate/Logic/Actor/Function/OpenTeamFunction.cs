using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 打开快捷组队功能///
    /// </summary>
    public class OpenTeamFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(ID);
        }
    }
}