using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using System;
using Table;

namespace Logic
{
    public class PointRuleEvt
    {
        public uint titleLan;
        public uint contentLan;
    }

    public class UI_PointRuleTips : UIBase
    {
        private Text content;
        private Button closeBtn;
        private Text title;

        private PointRuleEvt evt;

        protected override void OnLoaded()
        {
            title= transform.Find("Animator/View_Tips/Name/Text_Name").GetComponent<Text>();
            content = transform.Find("Animator/View_Tips/Text_Describe").GetComponent<Text>();
            closeBtn = transform.Find("Blank").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtn);
        }
        protected override void OnOpen(object arg)
        {
            evt = (PointRuleEvt)arg;
        }

        protected override void OnShow()
        {
            title.text = LanguageHelper.GetTextContent(evt.titleLan);
            content.text = LanguageHelper.GetTextContent(evt.contentLan);
        }

        #region 响应事件
        private void OnCloseBtn()
        {
            UIManager.CloseUI(EUIID.UI_PointRuleTips);
        }

        #endregion
    }

}
