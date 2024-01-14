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
    public class UI_RedEnvelopeRainRemind : UIBase
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
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            OnLateUpdate();
        }
        #endregion
        #region 组件
        Text textTitle;
        Text textTime;
        Button btn;
        Button black;
        #endregion
        #region 数据
        bool isDirty;
        int curTime;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            black = transform.Find("Black").gameObject.AddComponent<Button>();
            textTitle = transform.Find("Animator/Image_Bg/Text_Title").GetComponent<Text>();
            textTime = transform.Find("Animator/Image_Bg/Text_Time").GetComponent<Text>();
            btn = transform.Find("Animator/Image_Bg/Btn_Participation").GetComponent<Button>();

            btn.onClick.AddListener(OnClick);
            black.onClick.AddListener(OnClickClose);
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshRainStartHint, OnRefreshRainStartHint, toRegister);
        }

        private void OnRefreshRainStartHint()
        {
            RefreshData();
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            RefreshData();
        }
        private void RefreshData()
        {
            isDirty = true;
            textTitle.text = LanguageHelper.GetTextContent(1001914, Sys_RedEnvelopeRain.Instance.curRainNum.ToString());
            RefreshTime();
        }
        #endregion

        #region function
        private void OnClickClose()
        {
            UIManager.HitButton(EUIID.UI_RedEnvelopeRain_Remind, "OnClickClose");
            CloseSelf();
        }
        private void OnClick()
        {
            OnClickClose();
            if (Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway())
            {
                if(UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain))
                    UIManager.CloseUI(EUIID.UI_RedEnvelopeRain);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1001921).words);
            }
        }
        private void RefreshTime()
        {
            int scened= Sys_RedEnvelopeRain.Instance.GetResidueSecond(false);
            if (scened <= 0)
            {
                scened = 0;
                isDirty = false;
            }
            textTime.text = LanguageHelper.TimeToString((uint)(scened), LanguageHelper.TimeFormat.Type_1);
        }
        #endregion
        private void OnLateUpdate()
        {
            if(isDirty)
            {
                RefreshTime();
            }
        }
    }
}