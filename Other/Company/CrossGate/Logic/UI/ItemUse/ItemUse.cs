
namespace Logic
{
    public class ItemUse
    {
        public static bool ParseItem(ItemData itemData)
        {
            ItemUseBase useBase = null;
            switch (itemData.cSVItemData.fun_parameter)
            {
                case "enchant":
                    useBase = new ItemUseEnchant(itemData);
                    break;
                case "skillBook":
                    useBase = new ItemUseSkillBook(itemData);
                    break;
                case "openpetfeedUI":
                    useBase = new ItemUseOpenPetFeedUI(itemData);
                    break;
                case "openpetrefineUI":
                    useBase = new ItemUseOpenPetRefineUI(itemData);
                    break;
                case "fashion":
                    useBase = new ItemUseFashion(itemData);
                    break;
                case "openUi":
                    useBase = new ItemUseOpenUI(itemData);
                    break;
                case "opendevice":
                case "adddeviceexp":
                case "adddevicesexp":
                    useBase = new ItemUsePetDevice(itemData);
                    break;
                case "addpartnerExp":
                    useBase = new ItemUsePartnerExp(itemData);
                    break;
                case "addproficiency":
                    useBase = new ItemUseLifeSkill(itemData);
                    break;
                case "addstoneexp":
                    useBase = new ItemUseStoneExp(itemData);
                    break;
                case "seekNpc":
                    useBase = new ItemUseSeekNpc(itemData);
                    break;
                case "rePoint":
                    useBase = new ItemUseResetPoint(itemData);
                    break;
                case "openRelationUi":
                    useBase = new ItemUseOpenRelationUI(itemData);
                    break;
                case "success_rate":
                    useBase = new ItemUseSuccessRate(itemData);
                    break;
                case "openchangepet":
                    useBase = new ItemUseChangePet(itemData);
                    break;
                case "addPassiveSkill":
                    useBase = new ItemUsePassiveSkill(itemData);
                    break;
                case "PetskillBook":
                    useBase = new ItemUsePetSkillExp(itemData);
                    break;
                case "OpenCookBookUI":
                    useBase = new ItemUseOpenCookBookUI(itemData);
                    break;
                case "addOptionalBap":
                    useBase = new ItemUseAddOptionalBap(itemData);
                    break;
                case "extendfriend":
                    useBase = new ItemUseExtendFriend(itemData);
                    break;
                case "petCard":
                    useBase = new ItemPetCardNubber(itemData);
                    break;
                case "openMountEnergy":
                    useBase = new OpenMountEnergy(itemData);
                    break;
                case "ChangeNameLimit":
                    useBase = new ChangeNameLimit(itemData);
                    break;
                case "openWelfare":
                    useBase = new ItemUseOpenWelfare(itemData);
                    break;
                case "openMall":
                    useBase = new ItemUseOpenMall(itemData);
                    break;
                case "openPetEquipUi":
                    useBase = new OpenPetEquipUi(itemData);
                    break;
                case "openPetSoul":
                     useBase = new ItemUsePetSoul(itemData);
                    break;
                default:
                    Sys_Bag.Instance.UsetItem(itemData.Id, itemData.Uuid, 1);
                    Sys_Bag.Instance.useItemReq = true;
                    break;
            }

            if (useBase != null)
                return useBase.OnUse();
            else
                return true;
        }
    }
}


