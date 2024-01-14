using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 增加晶石经验
    /// </summary>
    public class ItemUseStoneExp : ItemUseBase
    {
        public ItemUseStoneExp(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.itemId = _itemData.cSVItemData.id;
            mallPrama.mallId = _itemData.cSVItemData.fun_value[2];
            mallPrama.shopId = 0;
            EUIID eUIID = (EUIID)_itemData.cSVItemData.fun_value[0];
            UIManager.OpenUI(eUIID, false, mallPrama);

            return true;
        }
    }
}


