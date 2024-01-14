using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;

namespace Logic
{
    /// <summary>
    /// 宠物装备底部信息-制作者 评分-交易限制
    /// </summary>
    public class TipPetEquipInfoTip : UIComponent
    {        
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

            Score = transform.Find("Image_Score/Text_Score").GetComponent<Text>();
            ScoreNumber = transform.Find("Image_Score/Number").GetComponent<Text>();

            _imgMake = transform.Find("Image_Make").GetComponent<Image>();
            _textMakePlayer = transform.Find("Image_Make/Name").GetComponent<Text>();

            _imgSale = transform.Find("Image_Lock").GetComponent<Image>();
            _textSaleTip = transform.Find("Image_Lock/Text_Lockdate").GetComponent<Text>();
        }

        public void UpdateInfo(ItemData item)
        {
            ScoreNumber.text = item.petEquip.Score.ToString();

            //make
            if (!item.petEquip.BuildName.IsEmpty)
            {
                _imgMake.gameObject.SetActive(true);
                _textMakePlayer.text = item.petEquip.BuildName.ToStringUtf8();
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