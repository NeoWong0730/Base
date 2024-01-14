using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{ 
    public class UI_PvpSingle_Rule_Layout
    {
        private Text m_TexDetail;

        private Button m_BtnClose;

        private Canvas m_Canvas;
        public void Load(Transform root)
        {
            m_TexDetail = root.Find("Image_BG/Content/Text_Content").GetComponent<Text>();

            m_BtnClose = root.Find("close").GetComponent<Button>();

            m_Canvas = root.GetComponent<Canvas>();
        }

        public void SetRuleDetailTex(uint tex)
        {
            TextHelper.SetText(m_TexDetail, tex);
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }
        public interface IListener
        {
            void OnClickClose();
        }

        public void SetSort(int sort)
        {
            m_Canvas.sortingOrder = sort;
        }
    }

}
