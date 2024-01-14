using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能经验道具
    /// </summary>
    public class ItemUsePetSoul : ItemUseBase
    {
        public ItemUsePetSoul(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if(!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(222))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680003049));
                return false;
            }

            uint uiid = 7;
            MessageEx practiceEx = new MessageEx
            {
                messageState = (EPetMessageViewState)uiid
            };
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            return true;
        }
    }
}


