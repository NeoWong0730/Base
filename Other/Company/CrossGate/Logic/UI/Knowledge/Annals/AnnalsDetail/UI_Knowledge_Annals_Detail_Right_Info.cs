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
    public class UI_Knowledge_Annals_Detail_Right_Info
    {
        private Transform transform;

        private Text _textTitle;
        private Text _textContent;
        private Image _imgTexture;

        public void Init(Transform trans)
        {
            transform = trans;

            _textTitle = transform.Find("Text").GetComponent<Text>();
            _textContent = transform.Find("TextMask/Text_Story").GetComponent<Text>();
            _imgTexture = transform.Find("Mask/Image").GetComponent<Image>();
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

        }

        public void SetSliblingIndex(int index)
        {
            transform.SetSiblingIndex(index);
        }

        public void UpdateInfo(uint eventId)
        {
            CSVChronology.Data data = CSVChronology.Instance.GetConfData(eventId);
            if (data != null)
            {
                _textTitle.text = LanguageHelper.GetTextContent(data.event_titel);
                _textContent.text = LanguageHelper.GetTextContent(data.event_text);

                ImageHelper.SetIcon(_imgTexture, data.show_image);
            }
        }
    }
}


