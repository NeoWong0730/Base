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
    public class TipOrnamentBackGroundFirst : UIComponent
    {
        private RawImage imgQuality;
        private Text EquipName;
        private Text EquipLevel;
        private Text EquipProfession;
        private Text EquipStatus;
        private Text CanEquipLevel;

        private PropItem propItem;

        public Button BtnMsg;
        public Button BtnSwitch;

        protected override void Loaded()
        {
            base.Loaded();

            imgQuality = transform.Find("Image_Quality").GetComponent<RawImage>();
            EquipName = transform.Find("Text_Name").GetComponent<Text>();
            EquipLevel = transform.Find("Text_Type").GetComponent<Text>();
            EquipProfession = transform.Find("Text_Profession").GetComponent<Text>();
            EquipStatus = transform.Find("Image_Equip/Text").GetComponent<Text>();
            CanEquipLevel = transform.Find("Text_Level").GetComponent<Text>();

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);

            BtnMsg = transform.Find("Button_Message").GetComponent<Button>();
            BtnSwitch = transform.Find("Button_Switch").GetComponent<Button>();
            BtnSwitch.gameObject.SetActive(false);
        }

        public void UpdateInfo(ItemData itemEquip)
        {
            SetBgQuality(itemEquip.Quality);

            PropIconLoader.ShowItemData propData = new PropIconLoader.ShowItemData(itemEquip.Id, 1, true, itemEquip.bBind, false, false, false);
            propData.SetQuality(itemEquip.Quality);
            propData.SetMarketEnd(itemEquip.bMarketEnd);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_TipsEquipment, propData));
            propItem.Layout.imgQuality.enabled = true;
            Debug.Log("Quality " + itemEquip.Quality);
            TextHelper.SetQuailtyText(EquipName, itemEquip.Quality, LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id));
            //EquipName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(itemEquip.cSVItemData.name_id), itemEquip.Quality + 2007416);

            CSVOrnamentsUpgrade.Data ornamentData = CSVOrnamentsUpgrade.Instance.GetConfData(itemEquip.Id);
            EquipLevel.text = LanguageHelper.GetTextContent(4011, ornamentData.lv.ToString());

            EquipStatus.gameObject.SetActive(Sys_Ornament.Instance.IsEquiped(itemEquip));
            TextHelper.SetText(EquipStatus, 4014);
            
            EquipProfession.text = LanguageHelper.GetTextContent(2007413, Sys_Ornament.Instance.GetOrnamentTypeName(ornamentData.type));//2007413 类型{0}
            CanEquipLevel.text = LanguageHelper.GetTextContent(1018019, Sys_Ornament.Instance.GetCanEquipLv(itemEquip).ToString());
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