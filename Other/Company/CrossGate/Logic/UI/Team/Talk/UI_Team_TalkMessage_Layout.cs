using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Team_TalkMessage_Layout 
    {
        private Button m_BtnSend;

        private Button m_BtnClose;

        private InputField m_IFMessage;

        public interface IListener
        {
            void OnClickClose();
            void OnClickSend();

            void OnInputEnd(string value);
        }
        public void Load(Transform root)
        {
            m_BtnSend = root.Find("Animator/Button_Modify").GetComponent<Button>();
            m_BtnClose = root.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            m_IFMessage = root.Find("Animator/InputField_Describe").GetComponent<InputField>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnSend.onClick.AddListener(listener.OnClickSend);
            m_IFMessage.onEndEdit.AddListener(listener.OnInputEnd);
        }

        public void SetDefaultTex(string value)
        {
            m_IFMessage.text = value;
        }
    }
}
