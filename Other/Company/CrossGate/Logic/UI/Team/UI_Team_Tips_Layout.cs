using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Team_Tips_Layout
    {
        Button m_BtnCancle;
        Button m_BtnSure;
        Button m_BtnSet;

        Text m_TexSure;
        Text m_TexCancle;

        Text m_TexMessage;
        public void Load(Transform root)
        {
            m_BtnCancle = root.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            m_BtnSure = root.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            m_BtnSet = root.Find("Animator/Button_Setting").GetComponent<Button>();

            m_TexSure = m_BtnSure.transform.Find("Text").GetComponent<Text>();
            m_TexCancle = m_BtnCancle.transform.Find("Text").GetComponent<Text>();

            m_TexMessage = root.Find("Animator/Text_Tip").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnSure.onClick.AddListener(listener.OnClickSure);
            m_BtnCancle.onClick.AddListener(listener.OnClickCancle);
            m_BtnSet.onClick.AddListener(listener.OnClickSet);
        }

        public void SetBtnSureTex(string tex)
        {
            m_TexSure.text = tex;
        }

        public void SetBtnCancleTex(string tex)
        {
            m_TexCancle.text = tex;
        }

        public void SetMessage(string tex)
        {
            m_TexMessage.text = tex;
        }
    }

    public partial class UI_Team_Tips_Layout
    {
        public interface IListener
        {
            void OnClickCancle();
            void OnClickSure();

            void OnClickSet();
        }
    }
}
