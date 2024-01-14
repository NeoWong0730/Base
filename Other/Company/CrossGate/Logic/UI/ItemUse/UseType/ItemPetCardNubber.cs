
using Logic.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 符文附魔
    /// </summary>
    public class ItemPetCardNubber : ItemUseBase
    {
        public ItemPetCardNubber(ItemData itemData) : base(itemData) { }

        public override bool OnUse()
        {
            uint petId = _itemData.cSVItemData.fun_value[1];
            bool active = Sys_Pet.Instance.GetPetIsActive(petId);
            if (!active)
            {
                UIManager.OpenUI(EUIID.UI_Eraser, false, new Tuple<uint, object>(0, _itemData.cSVItemData.fun_value[2]));
            }
            else
            {
                if (_itemData.cSVItemData.fun_parameter == "petCard")
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(10545, true))//宠物功能开启条件
                        return false;
                    if (_itemData.cSVItemData.fun_value.Count >= 2)
                    {
                        uint uiid = _itemData.cSVItemData.fun_value[0];
                        PetBookListPar petBookListPar = new PetBookListPar();
                        petBookListPar.petId = petId;
                        if (Sys_Pet.Instance.GetPetIsActive(petId))
                        {
                            petBookListPar.eviewType = EPetReviewViewType.Friend;
                            petBookListPar.ePetReviewPageType = EPetBookPageType.Friend;
                        }
                        else
                        {
                            petBookListPar.eviewType = EPetReviewViewType.Book;
                            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
                        }
                        UIManager.OpenUI((EUIID)uiid, false, petBookListPar);
                    }
                }
            }
            return true;
        }
    }
}


