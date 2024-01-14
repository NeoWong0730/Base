using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_TerroristWeek_Info : UIComponent
    {
        #region UI
        private Text m_TextDes;
        private Image m_ImgPic;
        #endregion
        
        protected override void Loaded()
        {
            m_TextDes = transform.Find("GameObject/Image_textbg/Text").GetComponent<Text>();
            m_ImgPic = transform.Find("GameObject/Image_mask/Image_pic").GetComponent<Image>();
        }

        public void UpdateInfo(CSVInstance.Data instanceData)
        {
            //和日常是一个描述
            CSVTerrorSeries.Data terrData = CSVTerrorSeries.Instance.GetConfData(instanceData.id);
            if (terrData != null)
            {
                m_TextDes.text = LanguageHelper.GetTextContent(terrData.instance_des);
                ImageHelper.SetIcon(m_ImgPic, instanceData.bg);
            }
        }
    }
}


