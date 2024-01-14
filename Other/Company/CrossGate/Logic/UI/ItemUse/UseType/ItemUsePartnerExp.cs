using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 升级伙伴
    /// </summary>
    public class ItemUsePartnerExp : ItemUseBase
    {
        public ItemUsePartnerExp(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uuid = _itemData.cSVItemData.fun_value[0];
            UIManager.OpenUI((EUIID)uuid);

            return true;
        }
    }
}


