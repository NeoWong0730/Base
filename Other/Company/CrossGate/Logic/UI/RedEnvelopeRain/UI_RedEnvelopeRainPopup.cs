using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_RedEnvelopeRainPopup : UIBase
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
        protected override void OnOpen(object arg)
        {
            curData = null;
            if (arg != null)
            {
                curData = arg as EnvelopeData;
            }
        }
        protected override void OnClose()
        {
            updateTimer?.Cancel();
        }
        #endregion
        #region 组件
        Button btnNormal;
        Button btnGolden;
        Text textNormal;
        Text textGolden;
        GameObject objNormal;
        GameObject objGolden;
        Button btnClose;
        Animator open;
        Animator openNormal;
        Animator openGolden;
        #endregion
        #region 数据
        EnvelopeData curData;

        Timer updateTimer;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            btnClose= transform.Find("Black").gameObject.AddComponent<Button>();
            openNormal = transform.Find("Animator/Btn_Normal").GetComponent<Animator>();
            openGolden = transform.Find("Animator/Btn_Golden").GetComponent<Animator>();
            objNormal = transform.Find("Animator/Btn_Normal").gameObject;
            objGolden = transform.Find("Animator/Btn_Golden").gameObject;
            btnNormal= transform.Find("Animator/Btn_Normal").GetComponent<Button>();
            btnGolden = transform.Find("Animator/Btn_Golden").GetComponent<Button>();
            textNormal = transform.Find("Animator/Btn_Normal/Text_Get").GetComponent<Text>();
            textGolden = transform.Find("Animator/Btn_Golden/Text_Get").GetComponent<Text>();

            btnClose.onClick.AddListener(OnClickOpen); 

            btnNormal.onClick.AddListener(OnClickOpen);
            btnGolden.onClick.AddListener(OnClickOpen);
        }
        #endregion
        #region 初始化
        private void InitView()
        {
            if (curData != null)
            {
                if (curData.quality== RedEnvelopeQuality.Golden)
                {
                    objGolden.SetActive(true);
                    objNormal.SetActive(false);
                    open = openGolden;
                }
                else
                {
                    objGolden.SetActive(false);
                    objNormal.SetActive(true);
                    open = openNormal;
                }
                RefreshData();
                SetTimer();
            }
        }
        #endregion
        #region function
        private void SetTimer()
        {
            updateTimer?.Cancel();
            updateTimer = Timer.Register(0f, OnUpdateData, null, true);
        }
        private void OnUpdateData()
        {
            if (IsCompleteAnimator())
            {
                updateTimer?.Cancel();
                OnClickOpen();
            }
        }
        private bool IsCompleteAnimator()
        {
            if (null == open) return true;
            if (!open.gameObject.activeInHierarchy) return false;
            AnimatorStateInfo animatorStateInfo = open.GetCurrentAnimatorStateInfo(0);
            return animatorStateInfo.normalizedTime >= 0.95f;
        }
        private void OnClickOpen()
        {
            if (curData != null)
            {
                UIManager.HitButton(EUIID.UI_RedEnvelopeRain_Popup, "OnClickClose");
                CloseSelf();
                Sys_RedEnvelopeRain.Instance.OnTakeAwardReq(curData.dropId);
                curData = null;
            }
        }
        private void RefreshData()
        {
            int index = Sys_RedEnvelopeRain.Instance.GetCurIndexByCurRainNum();
            uint awardCount = Sys_RedEnvelopeRain.Instance.GetCurAwardByIndex(index);
            uint max = Sys_RedEnvelopeRain.Instance.curCSVRainData.Limit_Max[index];
            string str = LanguageHelper.GetTextContent(1001911, (awardCount + 1).ToString(), max.ToString());
            if (curData.quality == RedEnvelopeQuality.Golden)
            {
                textGolden.text = str;
            }
            else
            {
                textNormal.text = str;
            }
        }
        #endregion
    }
}