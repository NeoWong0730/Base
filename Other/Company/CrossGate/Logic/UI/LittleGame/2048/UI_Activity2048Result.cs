using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;
using Packet;

namespace Logic
{
    public class Activity2048ResultPrama
    {
        public bool IsWin;
        public uint Time;
    }
    public class UI_Activity2048Result : UIBase
    {
        private Button btnClose;
        private GameObject goFail;
        private GameObject goSucc;
        private GameObject goContent;
        private Text txtTime;
        private Text txtDesc;
        private Slider slider;

        private bool isWin;
        private uint useTime;
        #region 系统函数

        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(Activity2048ResultPrama))
            {
                Activity2048ResultPrama prama = arg as Activity2048ResultPrama;
                isWin = prama.IsWin;
                useTime = prama.Time;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnHide()
        {
        }
        protected override void OnDestroy()
        {
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Image_mask").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            goFail = transform.Find("Animator/Result/Fail").gameObject;
            goSucc = transform.Find("Animator/Result/Success").gameObject;
            goContent = transform.Find("Animator/Content").gameObject;
            txtTime = transform.Find("Animator/Content/Time/Text_Level").GetComponent<Text>();
            txtDesc = transform.Find("Animator/Content/Rank/Text_Level").GetComponent<Text>();
            slider = transform.Find("Animator/Content/Slider_Eg").GetComponent<Slider>();
        }
        private void UpdateView()
        {
            goFail.SetActive(!isWin);
            goSucc.SetActive(isWin);
            goContent.SetActive(isWin);
            if (isWin)
            {
                txtTime.text = LanguageHelper.TimeToString(useTime, LanguageHelper.TimeFormat.Type_4);
                var value = Sys_Activity_2048.Instance.GetActivity2048RankPercent(useTime);
                txtDesc.text = LanguageHelper.GetTextContent(2026201, value.ToString());
                slider.value = (float)value / 100;
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            if (!Sys_OperationalActivity.Instance.CheckTimelimitFunctionIsOpen())
            {
                this.CloseSelf();
            }
        }
        #endregion
    }
}
