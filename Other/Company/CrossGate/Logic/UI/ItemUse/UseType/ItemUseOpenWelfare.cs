using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 技能书
    /// </summary>
    public class ItemUseOpenWelfare : ItemUseBase
    {
        public ItemUseOpenWelfare(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            OperationalActivityPrama operationalActivityPrama = new OperationalActivityPrama();
            operationalActivityPrama.pageType = _itemData.cSVItemData.fun_value[1];
            UIManager.OpenUI(EUIID.UI_OperationalActivity, false, operationalActivityPrama);
            return true;
        }
    }
}


