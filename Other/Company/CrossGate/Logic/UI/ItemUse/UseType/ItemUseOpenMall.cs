using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能书
    /// </summary>
    public class ItemUseOpenMall : ItemUseBase
    {
        public ItemUseOpenMall(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            MallPrama param = new MallPrama();
            param.mallId = _itemData.cSVItemData.fun_value[1];
            param.isCharge = true;
            UIManager.OpenUI(EUIID.UI_Mall, false, param);
            return true;
        }
    }
}


