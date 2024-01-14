using Logic.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 时装
    /// </summary>
    public class ItemUseFashion : ItemUseBase
    {
        public ItemUseFashion(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            Tuple<uint, uint> tuple = new Tuple<uint, uint>(_itemData.cSVItemData.fun_value[0], _itemData.Id);
            UIManager.OpenUI(EUIID.UI_Fashion, false, tuple);
            return true;
        }
    }
}


