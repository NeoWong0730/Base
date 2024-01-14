using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_UserPartition : UIBase
    {
        private Button btn01;
        private Button btn02;
        private Button btn03;
        private GameObject goItemCell;
        private GameObject goTips;
        private UI_UserPartition_Tips tips;

        private Timer btnCDTimer;   //按钮cd 防止多次请求
        private bool isBtnCD;

        #region 系统函数
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            //UIManager.HitPoint(EUIID.UI_UserPartition, true);
            UpdateView();
        }
        protected override void OnHide()
        {
            btnCDTimer?.Cancel();
        }
        protected override void OnDestroy()
        {
            btnCDTimer?.Cancel();
        }
        #endregion

        #region function
        private void Parse()
        {
            btn01 = transform.Find("Animator/View_Content/Button1").GetComponent<Button>();
            btn01.onClick.AddListener(OnBtn01Click);
            btn02 = transform.Find("Animator/View_Content/Button2").GetComponent<Button>();
            btn02.onClick.AddListener(OnBtn02Click);
            btn03 = transform.Find("Animator/View_Content/Button3").GetComponent<Button>();
            btn03.onClick.AddListener(OnBtn03Click);
            goItemCell = transform.Find("Animator/View_Content/Award/Scroll_View/Viewport/Item").gameObject;
            goItemCell.SetActive(false);
            goTips = transform.Find("Animator/Tips").gameObject;
            goTips.SetActive(false);
            tips = AddComponent<UI_UserPartition_Tips>(transform.Find("Animator/Tips"));
        }
        private void UpdateView()
        {
            FrameworkTool.DestroyChildren(goItemCell.transform.parent.gameObject, goItemCell.name);
            List<ItemData> items = Sys_UserPartition.Instance.GetRewardItems();
            int len = items.Count;
            for (int i = 0; i < len; i++)
            {
                GameObject go = GameObject.Instantiate<GameObject>(goItemCell, goItemCell.transform.parent);
                go.SetActive(true);
                CeilGrid bagCeilGrid = new CeilGrid();
                bagCeilGrid.BindGameObject(go);
                bagCeilGrid.AddClickListener(OnItemClick);
                bagCeilGrid.SetData(items[i], i, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_UserPartition);
            }
        }
        #endregion

        #region event
        /// <summary>
        /// 萌新玩家
        /// </summary>
        private void OnBtn01Click()
        {
            UIManager.HitPartition(EUIID.UI_UserPartition, 1);
            OnBtnClick(1u);
            ReqReceiveTask(1312);
        }
        /// <summary>
        /// 普通玩家
        /// </summary>
        private void OnBtn02Click()
        {
            UIManager.HitPartition(EUIID.UI_UserPartition, 2);
            OnBtnClick(2u);
            ReqReceiveTask(1313);
        }
        /// <summary>
        /// 资深玩家
        /// </summary>
        private void OnBtn03Click()
        {
            UIManager.HitPartition(EUIID.UI_UserPartition, 3);
            OnBtnClick(3u);
            ReqReceiveTask(1314);
        }
        private void OnBtnClick(uint typeId)
        {
            OptionManager.Instance.SetBoolean((int)OptionManager.EOptionID.UsePcStyleEnterFight, typeId == 3u, false);

            Sys_UserPartition.Instance.UserPartitionGetGiftReq();
            OnBtnClose();
        }
        private void OnBtnClose()
        {
            //UIManager.HitPoint(EUIID.UI_UserPartition, false);
            //UIManager.CloseUI(EUIID.UI_UserPartition);
            this.CloseSelf();
        }
        private void OnItemClick(CeilGrid bagCeilGrid)
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(bagCeilGrid.mItemData.Id, 0, false, false, false, false, false, false, true);
            tips.ShowView(itemData);
        }
        /// <summary> 请求接受任务 </summary>
        private void ReqReceiveTask(uint paramId)
        {
            if (isBtnCD)
                return;
            isBtnCD = true;
            btnCDTimer?.Cancel();
            btnCDTimer = null;
            btnCDTimer = Timer.Register(1f, () =>
            {
                isBtnCD = false;
            });

            CSVParam.Data param = CSVParam.Instance.GetConfData(paramId);
            if (param != null)
            {
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(uint.Parse(param.str_value));
                //策划需求，已经完成的任务不再接受
                if (taskData != null && !TaskHelper.HasSubmited(taskData.id))
                {
                    Sys_Task.Instance.ReqReceive(taskData.id, true);
                }
            }
        }
        #endregion

        #region Tips
        public class UI_UserPartition_Tips : UIComponent
        {
            private PropIconLoader.ShowItemData mItemData;

            private Image mIcon;
            private Text mItemName;
            private Text mItemContent;
            private Text mItemLevel;
            private Text mItemType;
            private Text mItem_CanDeal;
            private Text mItem_Bind;
            private Image mQuality;
            private RawImage mQualityBG;
            private Button mItemSourceButton;
            private GameObject mSourceViewRoot;

            protected override void Loaded()
            {
                mItemName = transform.Find("Animator/View_Message/Text_Name").GetComponent<Text>();
                mItemLevel = transform.Find("Animator/View_Message/Text_Level").GetComponent<Text>();
                mItemType = transform.Find("Animator/View_Message/Text_Type").GetComponent<Text>();
                mItem_CanDeal = transform.Find("Animator/View_Message/Text_Can_Deal").GetComponent<Text>();
                mItem_Bind = transform.Find("Animator/View_Message/Text_Bound").GetComponent<Text>();
                mItemContent = transform.Find("Animator/View_Message/Text_Ccontent").GetComponent<Text>();
                mQualityBG = transform.Find("Animator/View_Message/Image_QualityBG").GetComponent<RawImage>();
                mQuality = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_BG").GetComponent<Image>();
                mIcon = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();

                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, CloseView);

                mItemSourceButton = transform.Find("Animator/View_Message/Button").GetComponent<Button>();
                mItemSourceButton.gameObject.SetActive(false);
                mSourceViewRoot = transform.Find("Animator/View_Right").gameObject;
                mSourceViewRoot.SetActive(false);
            }

            public void ShowView(PropIconLoader.ShowItemData itemData)
            {
                Show();
                mItemData = itemData;
                mItemData.bShowBtnNo = false;
                UpdateView();
            }

            public void CloseView(BaseEventData eventData)
            {
                Hide();
            }

            private void UpdateView()
            {
                CSVItem.Data cSVItem = CSVItem.Instance.GetConfData(mItemData.id);
                ImageHelper.SetIcon(mIcon, cSVItem.icon_id);
                uint tempQuality = 0u;
                if (mItemData.Quality == 0u)
                    tempQuality = (uint)cSVItem.quality;
                else
                    tempQuality = mItemData.Quality;

                TextHelper.SetQuailtyText(mItemName, tempQuality, CSVLanguage.Instance.GetConfData(cSVItem.name_id).words);
                TextHelper.SetText(mItemContent, cSVItem.describe_id);
                if (cSVItem.lv == 0)
                {
                    mItemLevel.gameObject.SetActive(false);
                }
                else
                {
                    mItemLevel.gameObject.SetActive(true);
                    TextHelper.SetText(mItemLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, cSVItem.lv));
                }
                TextHelper.SetText(mItemType, string.Format(CSVLanguage.Instance.GetConfData(2007413).words, CSVLanguage.Instance.GetConfData(cSVItem.type_name).words));
                ImageHelper.GetQualityColor_Frame(mQuality, (int)tempQuality);
                ImageHelper.SetBgQuality(mQualityBG, tempQuality);
            }
        }
        #endregion
    }
}
