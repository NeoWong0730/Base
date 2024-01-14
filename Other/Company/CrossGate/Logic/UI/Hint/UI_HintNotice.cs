using DG.Tweening;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public enum ViewType
    {
        High,
        Low
    }
    public class UI_HintNotice : UIComponent
    {
        #region 界面组件
        private Transform View_High;
        private Transform View_Low;
        private Image Image_Notice_BG;
        private Image Image_Bell;
        private Text  content;
        private Image Mask;
        private Transform curTran;
        #endregion
        #region 数据定义
        private Vector3 posStart;
        private float speed;
        private float moveDistance;
        private float duration;
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
            View_High= transform.Find("View_High");
            View_Low = transform.Find("View_Low");
            View_High.gameObject.SetActive(false);
            View_Low.gameObject.SetActive(false);
            gameObject.SetActive(false);
            Sys_Hint.Instance.MarqueeObjIsShow = false;
        }
        private void Init()
        {
            speed = float.Parse(CSVParam.Instance.GetConfData(1056).str_value);
        }
        private Transform SetComponent(CSVAnnouncement.Data data)
        {
            Transform tran;
            if (data != null)
            {
                if (data.Type == 1)
                {
                    View_High.gameObject.SetActive(true);
                    View_Low.gameObject.SetActive(false);
                    tran = View_High;
                }
                else
                {
                    View_High.gameObject.SetActive(false);
                    View_Low.gameObject.SetActive(true);
                    tran = View_Low;
                }
            }
            else
            {
                View_High.gameObject.SetActive(false);
                View_Low.gameObject.SetActive(true);
                tran = View_Low;
            }
            Image_Notice_BG = tran.Find("Image_Notice_BG").GetComponent<Image>();
            Image_Bell = tran.Find("Image_Bell").GetComponent<Image>();
            content = tran.Find("Mask/Text").GetComponent<Text>();
            return tran;
        }
        #endregion
        #region 功能函数
        public void RefreshMarqueeData(HitElement_Marquee data)
        {
            speed = Sys_Hint.Instance.ChechMarqueeSpeed();
            curTran = SetComponent(data.announcementData);
            if (curTran!=null)
            {
                if (data.announcementData != null)// && data.announcementData.wordStyle != null
                {
                    TextHelper.SetText(content, data.content, CSVWordStyle.Instance.GetConfData(data.announcementData.wordStyle));
                }
                else
                {
                    TextHelper.SetText(content, data.content);
                }
                posStart = new Vector3(content.preferredWidth / 2 + Image_Notice_BG.rectTransform.rect.width / 2, content.rectTransform.localPosition.y, 0);
                content.rectTransform.localPosition = posStart;

                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                    Sys_Hint.Instance.MarqueeObjIsShow = true;
                }
                moveDistance = content.preferredWidth / 2 + Image_Notice_BG.rectTransform.rect.width / 2 - 100;
                duration = moveDistance / speed;
                content.transform.DOLocalMoveX(-moveDistance, duration).SetEase(Ease.Linear).onComplete += () => {
                    content.text = "";
                    bool isHaveNext = Sys_Hint.Instance.GetHintCount() > 0;
                    if (isHaveNext)
                    {
                        Sys_Hint.Instance.GetNextMarqueeData();
                    }
                    else
                    {
                        SetHide();
                    }
                };
            }
        }
        public void SetHide()
        {
            gameObject.SetActive(false);
            curTran.gameObject.SetActive(false);
            curTran = null;
            DOTween.Kill(content);
            Sys_Hint.Instance.MarqueeObjIsShow = false;
        }
        #endregion
    }
}