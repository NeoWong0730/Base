using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 宠物改造功能///
    /// </summary>
    public class PetTransformationFunction : FunctionBase
    {
        protected override void OnExecute()
        {
            base.OnExecute();

            UIManager.CloseUI(EUIID.UI_NPC);
            UIManager.CloseUI(EUIID.UI_Dialogue);

            Sys_Pet.Instance.OnGetPetInfoReq(_ePetUiType: EPetUiType.UI_Remake);
        }
    }
}
