using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Terrorist_LineTemplate : UIComponent
    {
        #region UI
        private CP_Toggle m_Toggle;
        private Image m_ImgPaint;
        private Text m_TextLine;
        private Image m_ImgSelected;
        private Image m_ImgGoing;
        #endregion
        //private IListener m_Listener;    

        private uint m_InsId;
        private int m_Line;
        private uint m_NameId;

        protected override void Loaded()
        {
            m_Toggle = gameObject.GetComponent<CP_Toggle>();
            m_Toggle.onValueChanged.AddListener(OnClick);

            m_ImgPaint = transform.Find("Image_paint").GetComponent<Image>();
            m_TextLine = transform.Find("Text").GetComponent<Text>();
            m_ImgSelected = transform.Find("Image_select").GetComponent<Image>();
            m_ImgGoing = transform.Find("Image_ongoing").GetComponent<Image>();
        }

        private void OnClick(bool isOn)
        {
            if (isOn)
            {
                Sys_TerrorSeries.Instance.eventEmitter.Trigger(Sys_TerrorSeries.EEvents.OnSelectLine, m_Line);
            }

            //m_ImgSelected.gameObject.SetActive(isOn);
        }

        public void UpdateLineInfo(CSVTerrorSeries.Data data, int line)
        {
            m_InsId = data.id;
            m_Line = line;
            m_NameId = data.line_name[line];

            m_TextLine.text = LanguageHelper.GetTextContent(m_NameId);
            m_ImgGoing.gameObject.SetActive(Sys_TerrorSeries.Instance.IsDailyTaskLineOnGoing(m_InsId, (uint)m_Line));
            ImageHelper.SetIcon(m_ImgPaint, data.line_icon[line]);
        }

        public void OnDefaultSelect(bool select)
        {
            m_Toggle.SetSelected(select, true);
        }

        public void OnEnableLine(bool enable)
        {
            m_Toggle.enabled = enable;
            ImageHelper.SetImageGray(m_ImgPaint, !enable);
        }
    }
}


