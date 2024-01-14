
namespace Logic
{
    /// <summary>
    /// 符文附魔
    /// </summary>
    public class ItemUseEnchant : ItemUseBase
    {
        public ItemUseEnchant(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            Sys_Equip.Instance.OnUseRunel(_itemData);
            return true;
        }
    }
}


