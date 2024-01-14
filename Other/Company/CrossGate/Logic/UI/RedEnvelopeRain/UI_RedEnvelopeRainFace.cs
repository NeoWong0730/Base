using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Table;

namespace Logic
{
    public class UI_RedEnvelopeRainFace : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        #endregion
        #region 组件
        Button btnClose;
        Button btnDetails;
        Text textTime;
        Image imgBack,imgEffect,imgTitle;
        #endregion
        #region 数据
        bool isDirty;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnDetails = transform.Find("Animator/RawImage/Btn_Details").GetComponent<Button>();
            textTime = transform.Find("Animator/RawImage/Text_Time/Text_Value").GetComponent<Text>();
            imgBack = transform.Find("Animator/Image2").GetComponent<Image>();
            imgEffect = transform.Find("Animator/Image2/Image1").GetComponent<Image>();
            imgTitle = transform.Find("Animator/Image1").GetComponent<Image>();

            btnClose.onClick.AddListener(OnClickClose);
            btnDetails.onClick.AddListener(()=> {
                OnClickClose();
                UIManager.OpenUI(EUIID.UI_RedEnvelopeRain);
            });
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            CSVRedEnvelopRain.Data csvData = Sys_RedEnvelopeRain.Instance.GetFirstDayRedEnvelopRainData();
            if (csvData != null)
            {
                ImageHelper.SetIcon(imgBack, null, csvData.Bottom_Back, false);
                ImageHelper.SetIcon(imgEffect, null, csvData.Bottom_Effect, false);
                ImageHelper.SetIcon(imgTitle, null, csvData.Bottom_Tittle, false);
            }
            textTime.text = string.Format("{0}-{1}", Sys_RedEnvelopeRain.Instance.activityStartTime, Sys_RedEnvelopeRain.Instance.activityEndTime);//活动时间
        }
        #endregion
        private void OnClickClose()
        {
            UIManager.HitButton(EUIID.UI_RedEnvelopeRain_Face, "OnClickClose");
            CloseSelf();
        }
    }
}