using Logic.Core;
using System;
using Table;

namespace Logic
{
    public class ItemUseExtendFriend : ItemUseBase
    {
        public ItemUseExtendFriend(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            if (Sys_Society.Instance.friendsMaxCount >= uint.Parse(CSVParam.Instance.GetConfData(971).str_value))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13002));
                return false;
            }

            Sys_Bag.Instance.UsetItem(_itemData.Id, _itemData.Uuid, 1);
            Sys_Bag.Instance.useItemReq = true;
            return true;
        }
    }
}
