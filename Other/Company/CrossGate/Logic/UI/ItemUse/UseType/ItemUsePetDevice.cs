using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 宠物抵抗强化
    /// </summary>
    public class ItemUsePetDevice : ItemUseBase
    {
        public ItemUsePetDevice(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if (Sys_Pet.Instance.petsList.Count == 0)
            {
                //飘字提示 无宠物
                return false;
            }
            else
            {
                uint uiid = 3;
                MessageEx practiceEx = new MessageEx();
                practiceEx.messageState = (EPetMessageViewState)uiid;
                practiceEx.subPage = 1;
                Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            }
            return true;
        }
    }
}


