using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Message_ComItem
    {
        private Transform transform;

        private Image _imgIcon;
        private Image _imgIconBg;

        private RawImage _imgQualityBg;
        private Text _textName;
        private Text _textLevel;
        private Text _textContent;
        private Button _btnPath;

        private CSVItem.Data _itemData; 

        public void Init(Transform trans)
        {
            transform = trans;

            _imgIcon = transform.Find("ListItem/Image_Icon").GetComponent<Image>();
            _imgIconBg = transform.Find("ListItem/Image_BG").GetComponent<Image>();

            _imgQualityBg = transform.Find("Image_QualityBG").GetComponent<RawImage>();
            _textName = transform.Find("Text_Name").GetComponent<Text>();
            _textLevel = transform.Find("Text_Level").GetComponent<Text>();
            _textContent = transform.Find("Text_Ccontent").GetComponent<Text>();
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void SetItemInfoId(uint infoId)
        {
            _itemData = CSVItem.Instance.GetConfData(infoId);
        }

        public void UpdateInfo(TradeItem tradeItem)
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(tradeItem.InfoId);

            ImageHelper.SetIcon(_imgIcon, itemData.icon_id);
            ImageHelper.GetQualityColor_Frame(_imgIconBg, (int)itemData.quality);
            ImageHelper.SetBgQuality(_imgQualityBg, itemData.quality);

            TextHelper.SetText(_textName, itemData.name_id);
            TextHelper.SetText(_textContent, itemData.describe_id);
            TextHelper.SetText(_textLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, itemData.lv));
        }
    }
}


