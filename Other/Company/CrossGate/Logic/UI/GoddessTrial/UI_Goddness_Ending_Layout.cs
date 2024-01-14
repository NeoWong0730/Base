using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_Ending_Layout
    {
        private Text m_TTitle;
        private Text m_TDiscrib;

        private RawImage m_IEnding;
        private Button m_BtnClose;


        public void SetTitle(uint langueID)
        {
            TextHelper.SetText(m_TTitle, LanguageHelper.GetTextContent(langueID));
        }

        public void SetTitle(string langueID)
        {
            TextHelper.SetText(m_TTitle, langueID);
        }

        public void Setdiscrib(uint langueID)
        {
            TextHelper.SetText(m_TDiscrib, langueID);
        }

        public void SetEndingImage(string path)
        {
            ImageHelper.SetTexture(m_IEnding, path);
        }
    }
    public partial class UI_Goddness_Ending_Layout
    {
        IListener m_Listener;
        public void Load(Transform root)
        {
            m_TTitle = root.Find("Animator/Text_Title").GetComponent<Text>();
            m_TDiscrib = root.Find("Animator/Text_Discrib").GetComponent<Text>();

            m_IEnding = root.Find("Animator/Image_Ending").GetComponent<RawImage>();
            m_BtnClose = root.Find("Animator/Image_mask").GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            m_Listener = listener;
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }
    }

    public partial class UI_Goddness_Ending_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }
}
