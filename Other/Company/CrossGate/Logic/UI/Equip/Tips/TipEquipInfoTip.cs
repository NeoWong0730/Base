using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;

namespace Logic
{
    public class TipEquipInfoTip : UIComponent
    {        
        private Text Durability;
        private Text DurabilityNumber;

        private Text Score;
        private Text ScoreNumber;

        private Image _imgMake;
        private Text _textMakePlayer;
        private Text _textMakeType;

        private Image _imgSale;
        private Text _textSaleTip;

        protected override void Loaded()
        {
            base.Loaded();

            Durability = transform.Find("Image_Score/Text_Durability").GetComponent<Text>();
            DurabilityNumber = transform.Find("Image_Score/Durability_Number").GetComponent<Text>();

            Score = transform.Find("Image_Score/Text_Score").GetComponent<Text>();
            ScoreNumber = transform.Find("Image_Score/Number").GetComponent<Text>();

            _imgMake = transform.Find("Image_Make").GetComponent<Image>();
            _textMakePlayer = transform.Find("Image_Make/Name").GetComponent<Text>();
            _textMakeType = transform.Find("Image_Make/Text").GetComponent<Text>();

            _imgSale = transform.Find("Image_Lock").GetComponent<Image>();
            _textSaleTip = transform.Find("Image_Lock/Text_Lockdate").GetComponent<Text>();
        }

        public void UpdateInfo(ItemData item)
        {
            if (item != null && item.Equip.DurabilityData != null)
                DurabilityNumber.text = item.Equip.DurabilityData.CurrentDurability.ToString() + "/" + item.Equip.DurabilityData.MaxDurability.ToString();
            ScoreNumber.text = Sys_Equip.Instance.CalEquipTotalScore(item).ToString();

            //make
            if (item.Equip.BuildType != 0u)
            {
                _imgMake.gameObject.SetActive(true);
                _textMakePlayer.text = item.Equip.BuildName.ToStringUtf8();

                uint lanId = item.Equip.BuildType == 1u ? 4205u : 4204u;
                _textMakeType.text = LanguageHelper.GetTextContent(lanId);
            }
            else
            {
                _imgMake.gameObject.SetActive(false);
            }

            _imgSale.gameObject.SetActive(false);
            if (!item.bMarketEnd)
            {
                _imgSale.gameObject.SetActive(true);
                if (item.MarketendTime == -1)
                {
                    _textSaleTip.text = LanguageHelper.GetTextContent(1009002);
                }
                else
                {
                    _textSaleTip.text = item.marketendTimer.GetMarkendTimeFormat();
                }
            }
        }
    }
}