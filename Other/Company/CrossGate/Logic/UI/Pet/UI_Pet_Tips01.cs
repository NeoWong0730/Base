using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Pet_Tips01 : UIBase
    {
        private uint langId;
        private Text desc;
        private RectTransform backImageRect;
        protected override void OnOpen(object arg)
        {
            langId = System.Convert.ToUInt32(arg);
        }

        protected override void OnLoaded()
        {
            desc = transform.Find("Animator/Image_BG/Text_Name").GetComponent<Text>();
            backImageRect = transform.Find("Animator/Image_BG").GetComponent<RectTransform>();
        }

        protected override void OnShow()
        {
            desc.text = LanguageHelper.GetTextContent(langId);

            RectTransform textRect = desc.gameObject.GetComponent<RectTransform>();
            float h = LayoutUtility.GetPreferredHeight(textRect);
            backImageRect.sizeDelta = new Vector2(backImageRect.sizeDelta.x, h + 16);
        }
    }
}
