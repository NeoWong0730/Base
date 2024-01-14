using DG.Tweening;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_HintBottomNotice : UIComponent
    {
        #region 界面组件
        private Image Image_Notice_BG;
        private Text content;
        private RectTransform mask;
        #endregion
        #region 数据定义
        private Vector3 posDownStart;//底部出现位置
        private float moveDistance;
        private float duration;
        private Timer updateTimer;
        private float posDiff;
        private bool isFromBottom;
        private float rightMoveDistance;
        private float rightDuration;
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            OnParseComponent();
            Init();
        }

        public override void SetData(params object[] arg)
        {

        }
        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            base.Hide();
        }
        protected override void Update()
        {
        }
        protected override void Refresh()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion
        #region 初始化
        private void OnParseComponent()
        {
            Image_Notice_BG = transform.Find("Image_Notice_BG").GetComponent<Image>();
            mask = transform.Find("Mask").GetComponent<RectTransform>();
            content = transform.Find("Mask/Text").GetComponent<Text>();
            gameObject.SetActive(false);
            Sys_Hint.Instance.BottomCommonObjIsShow = false;

        }
        private void Init()
        {
            posDiff = (Image_Notice_BG.rectTransform.anchoredPosition.y / 2 + Image_Notice_BG.rectTransform.sizeDelta.y / 2 + 20);
            posDownStart = new Vector3(Image_Notice_BG.rectTransform.localPosition.x, Image_Notice_BG.rectTransform.localPosition.y- posDiff, 0);
            Image_Notice_BG.rectTransform.localPosition = posDownStart;
            mask.localPosition = posDownStart;
            moveDistance = posDownStart.y+ posDiff;
            duration = 1;
            isFromBottom = true;
        }
        #endregion
        #region 功能函数
        public void RefreshCommonInfoData(HitElement_CommonInfo data)
        {
            TextHelper.SetText(content, data.content);
            // 根据文本内容是否超出单行长度，超出按跑马灯滚动方式播放
            isFromBottom = content.preferredWidth <= mask.rect.width;
            if (isFromBottom)
            {
                mask.localPosition = posDownStart;
                content.rectTransform.localPosition = new Vector3(0, mask.anchoredPosition.y / 2 + mask.sizeDelta.y / 2, 0);
            }
            else
            {
                mask.localPosition = new Vector3(posDownStart.x, moveDistance, 0);
                rightMoveDistance = content.preferredWidth / 2 + mask.rect.width / 2+100;
                rightDuration = rightMoveDistance / float.Parse(CSVParam.Instance.GetConfData(1056).str_value);
                content.rectTransform.localPosition = new Vector3(content.preferredWidth / 2 + mask.rect.width / 2, 0, 0);
            }
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                Sys_Hint.Instance.BottomCommonObjIsShow = true;
                if (isFromBottom)
                {
                    Image_Notice_BG.transform.DOLocalMoveY(moveDistance, duration).SetEase(Ease.Linear);
                }
                else
                {
                    Image_Notice_BG.rectTransform.localPosition = new Vector3(posDownStart.x, moveDistance, 0);
                }
            }
            content.gameObject.SetActive(true);
            if (isFromBottom)
            {
                mask.transform.DOLocalMoveY(moveDistance, duration).SetEase(Ease.Linear).onComplete += () =>
                {
                    bool isHaveNext = Sys_Hint.Instance.hintCommonInfo.Count > 0;
                    updateTimer = Timer.Register((float)Math.Round(float.Parse(CSVParam.Instance.GetConfData(1070).str_value) / 1000,2) , () =>
                    {
                        updateTimer?.Cancel();
                        if (isHaveNext)
                        {
                            content.gameObject.SetActive(false);
                            Sys_Hint.Instance.GetNextCommonInfoData();
                        }
                        else
                        {
                            SetHide(isFromBottom);
                        }
                    }, null, false,true);
                };

            }
            else
            {
                content.transform.DOLocalMoveX(-rightMoveDistance, rightDuration).SetEase(Ease.Linear).onComplete += () => {
                    content.text = "";
                    bool isHaveNext = Sys_Hint.Instance.hintCommonInfo.Count > 0;
                    if (isHaveNext)
                    {
                        Sys_Hint.Instance.GetNextCommonInfoData();
                    }
                    else
                    {
                        SetHide(isFromBottom);
                    }
                };
            }
        }
        public void SetHide()
        {
            gameObject.SetActive(false);
            content.text = "";
            content.rectTransform.localPosition = new Vector3(0, mask.anchoredPosition.y / 2 + mask.rect.height / 2, 0);
            mask.localPosition = posDownStart;
            Image_Notice_BG.rectTransform.localPosition = posDownStart;
            DOTween.Kill(Image_Notice_BG);
            DOTween.Kill(content);
            DOTween.Kill(mask);
            Sys_Hint.Instance.BottomCommonObjIsShow = false;
        }
        public void SetHide(bool isBottom)
        {
            if (isBottom)
            {
                Image_Notice_BG.transform.DOLocalMoveY(Image_Notice_BG.rectTransform.localPosition.y - posDiff, duration).SetEase(Ease.Linear);
                mask.transform.DOLocalMoveY(mask.localPosition.y - posDiff, duration).SetEase(Ease.Linear).onComplete += () => {
                    SetHide();
                };
            }
            else
            {
                SetHide();
            }
        }
        #endregion
    }
}
