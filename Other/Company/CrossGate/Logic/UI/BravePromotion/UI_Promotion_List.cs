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
    public class UI_Promotion_List : UIBase
    {
        #region 系统函数        
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            if (arg is ClickPromotionListData)
            {
                clickPromotionListData = arg as ClickPromotionListData;
            }
        }
        protected override void OnShow()
        {
            OnProwShow();
            if (clickPromotionListData != null)
            {
                SetPosition(clickPromotionListData);
            }
        }        
        #endregion
        #region 组件
        Button close;
        Transform buttonScrollParent;

        RectTransform GroupList;
        RectTransform Image_BG;
        #endregion
        #region 定义数据
        private List<SubAssessMainData> curSubAssessMainData;
        private List<ButtonItem> buttonItems = new List<ButtonItem>();
        private int visualGridCount;
        private ClickPromotionListData clickPromotionListData;
        #endregion
        #region 初始化
        private void OnParseComponent()
        {
            close = transform.Find("close").GetComponent<Button>();
            GroupList = transform.Find("GroupList").GetComponent<RectTransform>() ;
            Image_BG= transform.Find("GroupList/Image_BG").GetComponent<RectTransform>();
            buttonScrollParent = transform.Find("GroupList/Image_BG/Viewport");

            close.onClick.AddListener(() => { CloseSelf(); });
        }
        private void OnProwShow()
        {
            curSubAssessMainData = Sys_Promotion.Instance.subAssessMainDatas;
            if (curSubAssessMainData != null && curSubAssessMainData.Count>0)
            {
                visualGridCount = curSubAssessMainData.Count;
                for (int i = 0; i < buttonItems.Count; i++)
                {
                    ButtonItem ceil = buttonItems[i];
                    ceil.RemoveListener();
                    PoolManager.Recycle(buttonItems[i]);
                }
                buttonItems.Clear();
                FrameworkTool.CreateChildList(buttonScrollParent, visualGridCount);
                for (int i = 0; i < visualGridCount; i++)
                {
                    Transform tran = buttonScrollParent.transform.GetChild(i);
                    ButtonItem item = PoolManager.Fetch<ButtonItem>();
                    item.Init(tran);
                    item.RefreshData(curSubAssessMainData[i]);
                    buttonItems.Add(item);
                }
            }
        }
        #endregion

        public void SetPosition(ClickPromotionListData clickPromotionListData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(clickPromotionListData.parent, clickPromotionListData.clickTarget.position, null, out Vector2 localPos);
            Vector3 newPos=Vector3.zero;
            if (clickPromotionListData.eUIID == EUIID.UI_BravePromotion)
            {
                newPos = new Vector3(localPos.x + clickPromotionListData.clickTarget.sizeDelta.x / 2, localPos.y + clickPromotionListData.clickTarget.sizeDelta.y / 2, 0);
            }
            else if (clickPromotionListData.eUIID == EUIID.UI_PromotionDefeat)
            {
                newPos = new Vector3(localPos.x + clickPromotionListData.clickTarget.sizeDelta.x, localPos.y + clickPromotionListData.clickTarget.sizeDelta.y, 0);
            }
            GroupList.localPosition = newPos;
        }
    }
    public class ButtonItem
    {
        Transform trans;
        Button btn;
        Text name;
        SubAssessMainData data;
        public void Init(Transform trans)
        {
            this.trans = trans;
            btn = trans.Find("Button").GetComponent<Button>();
            name = trans.Find("Button/Text").GetComponent<Text>();

            btn.onClick.AddListener(MoveToView);
        }
        private void MoveToView()
        {
            if (this.data != null)
            {
                Sys_Promotion.Instance.MoveToView(this.data.jump, this.data.bar);
                UIManager.CloseUI(EUIID.UI_PromotionList);
            }
        }
        public void RefreshData(SubAssessMainData data)
        {
            this.data = data;
            TextHelper.SetText(name, LanguageHelper.GetTextContent(data.subName));
        }
        public void RemoveListener()
        {
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
            }
        }
    }
}