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
    public class UI_Promotion_Defeat : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }        
        protected override void OnShow()
        {
            OnProShow();
        }        
        #endregion
        #region 组件
        Transform partent;
        Button closeBtnMask;
        #endregion
        #region 数据定义
        private List<PromotionItem> promotionList = new List<PromotionItem>();
        List<CheckAssessData> checkDataList;
        public static RectTransform targetParent;
        int visualGridCount;
        #endregion
        #region 初始化
        private void OnParseComponent()
        {
            targetParent = transform.Find("Animator").GetComponent<RectTransform>();
            closeBtnMask = transform.Find("Animator/Image_mask").GetComponent<Button>();
            partent = transform.Find("Animator/View_Promote/Scroll_View/Viewport/Content");

            closeBtnMask.onClick.AddListener(() => CloseSelf());
        }
        public void OnProShow()
        {
            checkDataList = Sys_Promotion.Instance.GetSortAssessMainData();
            visualGridCount = checkDataList.Count;
            for (int i = 0; i < promotionList.Count; i++)
            {
                PromotionItem ceil = promotionList[i];
                ceil.RemoveListener();
                PoolManager.Recycle(promotionList[i]);
            }
            promotionList.Clear();
            FrameworkTool.CreateChildList(partent, visualGridCount);
            for (int i = 0; i < visualGridCount; i++)
            {
                Transform tran = partent.transform.GetChild(i);
                PromotionItem ceil = PoolManager.Fetch<PromotionItem>();
                ceil.Init(tran);
                ceil.RefreshData(checkDataList[i]);
                promotionList.Add(ceil);
            }
            FrameworkTool.ForceRebuildLayout(partent.gameObject);
        }
        #endregion

    }
    public class PromotionItem
    {
        Transform trans;
        Image icon;
        Text title;
        Image img_red;
        Image img_yellow;
        Button btn;
        CheckAssessData data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            btn = trans.GetComponent<Button>();
            icon = trans.Find("Image_Icon_01").GetComponent<Image>();
            title = trans.Find("Text").GetComponent<Text>();
            img_red = trans.Find("Image_Red").GetComponent<Image>();
            img_yellow = trans.Find("Image_Yellow").GetComponent<Image>();

            btn.onClick.AddListener(MoveToView);
        }

        private void MoveToView()
        {
            if (data.mainData != null)
            {
                if (data.mainData.Jump.Count > 1)
                {
                    List<SubAssessMainData> subList = Sys_Promotion.Instance.GetSubAssessMainData(data.mainData);
                    if (subList.Count > 1)
                    {
                        ClickPromotionListData clickData = new ClickPromotionListData();
                        clickData.eUIID = EUIID.UI_PromotionDefeat;
                        clickData.clickTarget = btn.GetComponent<RectTransform>();
                        clickData.parent = UI_Promotion_Defeat.targetParent;
                        UIManager.OpenUI(EUIID.UI_PromotionList, false, clickData);
                    }
                    else
                    {
                        Sys_Promotion.Instance.MoveToView(subList[0].jump, subList[0].bar);
                    }
                }
                else
                {
                    Sys_Promotion.Instance.MoveToView(data.mainData.Jump[0], data.mainData.Bar);
                }
            }
        }

        public void RefreshData(CheckAssessData data)
        {
            this.trans.gameObject.SetActive(true);
            this.data = data;
            if (data != null)
            {
                CheckStart(data.type);
                ImageHelper.SetIcon(icon, data.mainData.Icon);
                TextHelper.SetText(title, LanguageHelper.GetTextContent(data.mainData.Name));
            }
        }
        public void CheckStart(Sys_Promotion.PromotionLve prolv)
        {
            switch (prolv)
            {
                case Sys_Promotion.PromotionLve.LvUp:
                    img_red.gameObject.SetActive(true);
                    img_yellow.gameObject.SetActive(false);
                    break;
                case Sys_Promotion.PromotionLve.Great:
                    img_red.gameObject.SetActive(false);
                    img_yellow.gameObject.SetActive(true);
                    break;
                case Sys_Promotion.PromotionLve.Normal:
                case Sys_Promotion.PromotionLve.Special:
                    img_red.gameObject.SetActive(false);
                    img_yellow.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        public void RemoveListener()
        {
            if (btn != null)
                btn.onClick.RemoveAllListeners();
        }
    }
}