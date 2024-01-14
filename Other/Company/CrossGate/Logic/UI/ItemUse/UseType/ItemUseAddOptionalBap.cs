using Logic.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 技能经验道具
    /// </summary>
    public class ItemUseAddOptionalBap : ItemUseBase
    {
        public ItemUseAddOptionalBap(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            Tuple<ItemData, uint> tuple = new Tuple<ItemData, uint>(_itemData, 1);
            UIManager.OpenUI(EUIID.UI_OptionalGift, false, tuple);
            return true;
        }
    }
}


