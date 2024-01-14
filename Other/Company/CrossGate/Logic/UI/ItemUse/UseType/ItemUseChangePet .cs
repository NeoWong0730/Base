using Logic.Core;
using Table;


namespace Logic
{
    /// <summary>
    /// 宠物改造道具使用
    /// </summary>
    public class ItemUseChangePet : ItemUseBase
    {
        public ItemUseChangePet(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if(null != _itemData.cSVItemData.fun_value && _itemData.cSVItemData.fun_value.Count >= 2)
            {
                uint subToggle = _itemData.cSVItemData.fun_value[0];
                int subPage = (int)_itemData.cSVItemData.fun_value[1];
                MessageEx practiceEx = new MessageEx
                {
                    messageState = (EPetMessageViewState)subToggle,
                    subPage = subPage
                };
                Sys_Pet.Instance.OnGetPetInfoReq(practiceEx, EPetUiType.UI_Message);
            }
            return true;
        }
    }
}


