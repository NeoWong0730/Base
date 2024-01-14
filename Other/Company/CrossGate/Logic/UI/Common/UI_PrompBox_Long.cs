using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_PrompBox_Long : UIBase
    {
        private Text m_TitleText;

        private Text m_ContentText;

        private Button m_BtnCancel = null;
        private Text m_TextCancel = null;

        private Button m_BtnConfirm = null;
        private Text m_TextConfirm = null;

    

        protected override void OnLoaded()
        {
            ParseComponent();
        }


        protected override void OnShow()
        {
            m_BtnCancel.gameObject.SetActive(false);
            m_TitleText.text = LanguageHelper.GetTextContent(2106005);
            m_ContentText.text = CSVLanguage.Instance.GetConfData(2106006).words;
        }

        private void ParseComponent()
        {
            m_TitleText = transform.Find("Animator/Image_Titlebg01/Text_Title").GetComponent<Text>();
            m_ContentText = transform.Find("Animator/GameObject/Grid/Text_Tip").GetComponent<Text>();


            m_BtnConfirm = transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            m_TextConfirm = transform.Find("Animator/Buttons/Button_Sure/Text").GetComponent<Text>();

            m_BtnCancel = transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            m_TextCancel = transform.Find("Animator/Buttons/Button_Cancel/Text").GetComponent<Text>();

            m_BtnCancel.onClick.AddListener(OnBtnCancel);
            m_BtnConfirm.onClick.AddListener(OnBtnConfirm);
  
        }

        private void OnBtnCancel()
        {
            UIManager.CloseUI(EUIID.UI_PrompBox_Long, false);
        }

        private void OnBtnConfirm()
        {
            UIManager.CloseUI(EUIID.UI_PrompBox_Long, false);
        }
    }



}

