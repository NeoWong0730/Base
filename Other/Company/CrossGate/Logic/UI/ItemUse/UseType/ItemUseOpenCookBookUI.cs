using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能经验道具
    /// </summary>
    public class ItemUseOpenCookBookUI : ItemUseBase
    {
        public ItemUseOpenCookBookUI(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            UIManager.OpenUI(EUIID.UI_Knowledge_Cooking, false, _itemData.cSVItemData.fun_value[0]);
            return true;
        }
    }
}


