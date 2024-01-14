using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
//#else
//using Lib.AssetLoader;
//#endif

namespace Logic
{
    public class TipPetEquipBackgroundFirst : UIComponent
    {
        private RawImage imgQuality;
        private Text EquipName;
        private Text EquipType;
        private Text DressLevel;
        private Text EquipStatus;
        private PropItem propItem;

        public Button BtnMsg;

        protected override void Loaded()
        {
            base.Loaded();

            imgQuality = transform.Find("Image_Quality").GetComponent<RawImage>();
            EquipName = transform.Find("Text_Name").GetComponent<Text>();
            EquipType = transform.Find("Text_Type").GetComponent<Text>();
            DressLevel = transform.Find("Text_Lv").GetComponent<Text>();
            EquipStatus = transform.Find("Image_Equip/Text").GetComponent<Text>();

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);

            BtnMsg = transform.Find("Button_Message").GetComponent<Button>();
        }

        public void UpdateInfo(ItemData itemEquip, uint petuid)
        {
            SetBgQuality(itemEquip.petEquip.Color);
            PropIconLoader.ShowItemData propData = new PropIconLoader.ShowItemData(itemEquip.Id, 1, true, itemEquip.bBind, false, false, false);
            propData.EquipPara = itemEquip.petEquip.Color;
            propData.SetMarketEnd(itemEquip.bMarketEnd);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Tips_PetMagicCore, propData));
            propItem.Layout.imgQuality.enabled = true;

            EquipName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id), itemEquip.petEquip.Color + 2007416);

            CSVPetEquip.Data petEquipData = CSVPetEquip.Instance.GetConfData(itemEquip.Id);
            DressLevel.text = LanguageHelper.GetTextContent(1018019, petEquipData.equipment_level.ToString());

            TextHelper.SetText(EquipType, 1018020, LanguageHelper.GetTextContent(680010000u + petEquipData.equipment_category));

            EquipStatus.gameObject.SetActive(Sys_Pet.Instance.IsEquiped(itemEquip.Uuid, petuid));
            TextHelper.SetText(EquipStatus, 4014);
        }
        
        public void SetBgQuality(uint quality)
        {
            string bgPath = null;
            switch ((EItemQuality)quality)
            {
                case EItemQuality.White:
                    bgPath = Constants.TipBgWhite;
                    break;
                case EItemQuality.Green:
                    bgPath = Constants.TipBgGreen;
                    break;
                case EItemQuality.Blue:
                    bgPath = Constants.TipBgBlue;
                    break;
                case EItemQuality.Purple:
                    bgPath = Constants.TipBgPurple;
                    break;
                case EItemQuality.Orange:
                    bgPath = Constants.TipBgOrange;
                    break;
                default:
                    break;
            }

            ImageHelper.SetTexture(imgQuality, bgPath);
        }        
    }
}
