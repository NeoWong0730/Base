using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 洗点
    /// </summary>
    public class ItemUseResetPoint : ItemUseBase
    {
        public ItemUseResetPoint(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            UIManager.OpenUI(EUIID.UI_Reset);

            return true;
        }
    }
}


