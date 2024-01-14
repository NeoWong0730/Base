using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 打开宠物洗练界面
    /// </summary>
    public class ItemUseOpenPetRefineUI : ItemUseBase
    {
        public ItemUseOpenPetRefineUI(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uiid = 3;
            MessageEx practiceEx = new MessageEx();
            practiceEx.messageState = (EPetMessageViewState)uiid;
            Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            return true;
        }
    }
}


