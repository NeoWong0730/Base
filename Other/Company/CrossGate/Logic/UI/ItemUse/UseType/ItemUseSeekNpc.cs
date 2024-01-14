using Logic.Core;

namespace Logic
{
    /// <summary>
    /// NPC寻路
    /// </summary>
    public class ItemUseSeekNpc : ItemUseBase
    {
        public ItemUseSeekNpc(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(_itemData.cSVItemData.fun_value[0]);
            UIManager.CloseUI(EUIID.UI_Bag);
            return true;
        }
    }
}


