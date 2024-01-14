using Logic.Core;

namespace Logic
{
    /// <summary>
    /// Open PetMessage And AddMountEnergy UI
    /// </summary>
    public class OpenMountEnergy : ItemUseBase
    {
        public OpenMountEnergy(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            MessageEx practiceEx = new MessageEx();
            practiceEx.messageState = EPetMessageViewState.Mount;
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);

            UIManager.OpenUI(EUIID.UI_Pet_MountCharge);
            return true;
        }
    }
}


