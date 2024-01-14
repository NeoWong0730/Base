using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Gleanings_Items_Info
    {
        private Transform transform;

        private Image _imgIcon;
        private Text _textName;
        private Text _textDetail;

        public void Init(Transform trans)
        {
            transform = trans;

            _imgIcon = transform.Find("Item/Image_icon_bg/Image_icon").GetComponent<Image>();
            _textName = transform.Find("Item/Text").GetComponent<Text>();
            _textDetail = transform.Find("Text_Detail").GetComponent<Text>();
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {
            //for (int i = 0; i < listEvents.Count; ++i)
            //    listEvents[i].OnDestroy();
        }

        public void UpdateInfo(uint gleaningId)
        {
            CSVGleanings.Data data = CSVGleanings.Instance.GetConfData(gleaningId);
            if (data != null)
            {
                ImageHelper.SetIcon(_imgIcon, data.icon_id);
                _textName.text = LanguageHelper.GetTextContent(data.name_id);
                _textDetail.text = LanguageHelper.GetTextContent(data.describe_id);
            }
        }
    }
}


