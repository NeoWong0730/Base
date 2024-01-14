using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;

namespace Logic
{
    public class TipOrnamentInfoTip : UIComponent
    {
        private GameObject goScore;
        private Text ScoreNumber;

        private Image _imgMake;
        private Text _textMakePlayer;
        private Text _textMakeType;

        private Image _imgSale;
        private Text _textSaleTip;
        public Button btnSource;

        protected override void Loaded()
        {
            base.Loaded();

            goScore = transform.Find("Image_Score").gameObject;
            ScoreNumber = transform.Find("Image_Score/Number").GetComponent<Text>();

            _imgMake = transform.Find("Image_Make").GetComponent<Image>();
            _textMakePlayer = transform.Find("Image_Make/Name").GetComponent<Text>();
            _textMakeType = transform.Find("Image_Make/Text").GetComponent<Text>();

            _imgSale = transform.Find("Image_Lock").GetComponent<Image>();
            _textSaleTip = transform.Find("Image_Lock/Text_Lockdate").GetComponent<Text>();
            btnSource = transform.Find("Button").GetComponent<Button>();
            btnSource.gameObject.SetActive(false);
        }

        public void UpdateInfo(ItemData item)
        {
            if (item.ornament != null)
            {
                goScore.SetActive(true);
                ScoreNumber.text = item.ornament.Score.ToString();
            }
            else
            {
                goScore.SetActive(false);
            }
            _imgMake.gameObject.SetActive(false);
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