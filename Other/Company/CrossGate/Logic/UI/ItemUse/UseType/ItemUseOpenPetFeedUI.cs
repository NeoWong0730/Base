using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 打开宠物培养界面
    /// </summary>
    public class ItemUseOpenPetFeedUI : ItemUseBase
    {
        public ItemUseOpenPetFeedUI(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if (Sys_Pet.Instance.petsList.Count == 0)
            {
                //飘字提示 无宠物
                return false;
            }
            else
            {
                // Sys_Pet.Instance.OnGroomInfoReq();
                uint uiid = 3;
                MessageEx practiceEx = new MessageEx();
                practiceEx.messageState = (EPetMessageViewState)uiid;
                practiceEx.itemID = _itemData.Id;
                Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            }

            return true;
        }
    }
}


