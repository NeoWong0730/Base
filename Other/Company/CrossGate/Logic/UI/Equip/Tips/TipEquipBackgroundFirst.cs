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
    public class TipEquipBackgroundFirst : UIComponent
    {
        private RawImage imgQuality;
        private Text EquipName;
        private Text EquipLevel;
        private Text DressLevel;
        private Text EquipProfession;
        private Text EquipStatus;

        private PropItem propItem;

        public Button BtnMsg;
        public Button BtnSwitch;

        protected override void Loaded()
        {
            base.Loaded();

            imgQuality = transform.Find("Image_Quality").GetComponent<RawImage>();
            EquipName = transform.Find("Text_Name").GetComponent<Text>();
            EquipLevel = transform.Find("Text_Type").GetComponent<Text>();
            DressLevel = transform.Find("Text_Lv").GetComponent<Text>();
            EquipProfession = transform.Find("Text_Profession").GetComponent<Text>();
            EquipStatus = transform.Find("Image_Equip/Text").GetComponent<Text>();

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

            EquipName.text = LanguageHelper.GetLanguageColorWordsFormat(Sys_Equip.Instance.GetEquipmentName(itemEquip), itemEquip.Quality + 2007416);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(itemEquip.Id);
            EquipLevel.text = LanguageHelper.GetTextContent(4011, equipData.TransLevel().ToString());

            //穿戴等级计算
            uint minusLevel = 0;
            if (itemEquip.Equip.EffectAttr != null)
            {
                for (int i = 0; i < itemEquip.Equip.EffectAttr.Count; ++i)
                {
                    if (itemEquip.Equip.EffectAttr[i].Attr2.Id == 4101001) //写死该id 等级减10
                    {
                        minusLevel = 10;
                        break;
                    }
                }
            }
            if (minusLevel != 0)
                DressLevel.text = LanguageHelper.GetTextContent(4198, string.Format("{0}-{1}", equipData.equipment_level.ToString(), minusLevel.ToString()));
            else
                DressLevel.text = LanguageHelper.GetTextContent(4198, equipData.equipment_level.ToString());

            EquipStatus.gameObject.SetActive(Sys_Equip.Instance.IsEquiped(itemEquip));
            TextHelper.SetText(EquipStatus, 4014);

            if (equipData.career_condition == null)
            {
                EquipProfession.text = LanguageHelper.GetTextContent(4012, LanguageHelper.GetTextContent(4183));
            }
            else
            {
                System.Text.StringBuilder strBuilder = Lib.Core.StringBuilderPool.GetTemporary();

                for (int i = 0; i < equipData.career_condition.Count; ++i)
                {
                    strBuilder.Append(LanguageHelper.GetTextContent(OccupationHelper.GetTextID(equipData.career_condition[i])));
                    if (i != equipData.career_condition.Count - 1)
                    {
                        strBuilder.Append(".");
                    }
                }

                EquipProfession.text = LanguageHelper.GetTextContent(4012, Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(strBuilder));
            }
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
