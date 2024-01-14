using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_SpecialCardRule : UIBase
    {
        private uint languageId;
        private RectTransform _rectTransform;
        private Button btnClose;
        private Text txtDesc;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(uint))
            {
                languageId = (uint)arg;
            }
        }
        protected override void OnLoaded()
        {
            btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtDesc = transform.Find("Animator/View_Rule/Image_Bg/Text_Tips").GetComponent<Text>();
            _rectTransform = transform.Find("Animator/View_Rule/Image_Bg").GetComponent<RectTransform>();
        }
        protected override void OnShow()
        {
            txtDesc.text = LanguageHelper.GetTextContent(languageId);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
        }
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        #endregion
    }
}
