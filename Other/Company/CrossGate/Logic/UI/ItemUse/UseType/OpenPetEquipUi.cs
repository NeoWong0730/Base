using Logic.Core;

namespace Logic
{
    public class OpenPetEquipUi : ItemUseBase
    {
        public OpenPetEquipUi(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint uiType = _itemData.cSVItemData.fun_value[0];
            uint itemId = _itemData.cSVItemData.fun_value[1];
            MagicCorePrama magicCorePrama = new MagicCorePrama
            {
                pageType = 1,
                itemId = itemId,
                uiType = uiType
            };
            UIManager.OpenUI(EUIID.UI_PetMagicCore, false, magicCorePrama);
            return true;
        }
    }
}


