using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_SevenDaysTarget : UIBase
    {
        readonly uint SliderMaxWidth = 760;
        readonly uint SliderCellWidth = 102;

        private Button btnClose;
        private Text txtRemaningTime;
        private Timer timer;
        private float countDownTime = 0;
        private bool isTurned = false;//打开界面的时候滑动到选中位置
        //private RawImage imgBigReward;
        private GameObject goNpc1;
        private GameObject goNpc2;

        private InfinityGrid infinityMenu;
        private List<uint> menuList;
        private uint SelectedMenuId = 1;//id就是天数
        private Dictionary<GameObject, UI_SevenDaysTargetMenuCell> dictMenus = new Dictionary<GameObject, UI_SevenDaysTargetMenuCell>();

        private InfinityGrid infinityTarget;
        private List<CSVActivityTarget.Data>  TargetDataList = new List<CSVActivityTarget.Data>();

        private Text txtMyScore;
        private RectTransform SliderTrans;
        private Button btnRightArrow;
        private Button btnLeftArrow;
        private List<UI_SevenDaysTargetScoreItemCell> rewardItemList = new List<UI_SevenDaysTargetScoreItemCell>();
        private int rewardPage = 0;
        #region 系统函数

        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            SelectedMenuId = Sys_OperationalActivity.Instance.GetSevenDaysTaretPageNum();
            rewardPage = Sys_OperationalActivity.Instance.GetSevenDaysTaretRewardPageNum();
            UpdateView();
        }
        protected override void OnHide()
        {
            timer?.Cancel();
        }
        protected override void OnDestroy()
        {
            timer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSevenDaysTargetData, OnUpdateSevenDaysTargetData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtRemaningTime = transform.Find("Animator/View_Title/Text_Time").GetComponent<Text>();

            infinityMenu = transform.Find("Animator/Menu/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinityMenu.onCreateCell += OnCreateCell_MenuList;
            infinityMenu.onCellChange += OnCellChange_MenuList;

            infinityTarget = transform.Find("Animator/Content/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinityTarget.onCreateCell += OnCreateCell_TargetList;
            infinityTarget.onCellChange += OnCellChange_TargetList;

            txtMyScore = transform.Find("Animator/View_Bottom/Text_Goal").GetComponent<Text>();
            SliderTrans = transform.Find("Animator/View_Bottom/Slider_BG/Slider").GetComponent<RectTransform>();
            btnRightArrow = transform.Find("Animator/View_Bottom/Btn_Arrow1").GetComponent<Button>();
            btnRightArrow.onClick.AddListener(OnBtnRightArrowClick);
            btnLeftArrow = transform.Find("Animator/View_Bottom/Btn_Arrow2").GetComponent<Button>();
            btnLeftArrow.onClick.AddListener(OnBtnLeftArrowClick);
            //imgBigReward = transform.Find("Animator/Image_Npc").GetComponent<RawImage>();
            goNpc1 = transform.Find("Animator/Image_Npc").gameObject;
            goNpc2 = transform.Find("Animator/Image_Npc1").gameObject;
            var itemsParent = transform.Find("Animator/View_Bottom/Grid");
            rewardItemList.Clear();
            for (int i = 0; i < itemsParent.childCount; i++)
            {
                var goCell = itemsParent.GetChild(i);
                UI_SevenDaysTargetScoreItemCell itemCell = new UI_SevenDaysTargetScoreItemCell();
                itemCell.Init(goCell);
                rewardItemList.Add(itemCell);
            }
        }
        private void UpdateView()
        {
            UpdateTimer();
            UpdateMenu();
            UpdateTargetList();
            UpdateBottom();
        }
        private void UpdateTimer()
        {
            timer?.Cancel();
            countDownTime = Sys_OperationalActivity.Instance.GetSevenDaysTargetCountDownTime();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        private void UpdateMenu()
        {
            menuList = Sys_OperationalActivity.Instance.GetSevenDaysTargetDayList();
            infinityMenu.CellCount = menuList.Count;
            infinityMenu.ForceRefreshActiveCell();
            if (!isTurned)
            {
                isTurned = true;
                infinityMenu.MoveToIndex((int)SelectedMenuId - 1);
            }
        }
        private void UpdateTargetList()
        {
            TargetDataList = Sys_OperationalActivity.Instance.GetSevenDaysTargetList(SelectedMenuId);
            infinityTarget.CellCount = TargetDataList.Count;
            infinityTarget.ForceRefreshActiveCell();
        }
        private void UpdateBottom()
        {
            var showFirstBigReward = Sys_OperationalActivity.Instance.CheckSevenDaysTargetShowFirstBigReward();
            goNpc1.SetActive(showFirstBigReward);
            goNpc2.SetActive(!showFirstBigReward);
            //string strImgAddr = Sys_OperationalActivity.Instance.GetSevenDaysTargetBigRewardAddr();
            //if (imgBigReward != null && strImgAddr.Length > 0)
            //{
            //    RawImageLoader imageLoader = imgBigReward.GetNeedComponent<RawImageLoader>();
            //    imageLoader.Set(strImgAddr);
            //}
            uint allScore = Sys_OperationalActivity.Instance.SevenDaysTargetScore;
            txtMyScore.text = allScore.ToString();
            var rewardList = Sys_OperationalActivity.Instance.GetSevenDaysTargetRewardList(rewardPage);
            var itemsCount = rewardItemList.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                if (rewardItemList.Count > i && rewardList != null && rewardList.Count > i)
                {
                    rewardItemList[i].UpdateCellView(rewardList[i]);
                }
            }
            float width = Sys_OperationalActivity.Instance.GetSevenDaysTargetScoreSliderWidth(rewardPage, SliderCellWidth, SliderMaxWidth);
            SliderTrans.sizeDelta = new Vector2(width, SliderTrans.sizeDelta.y);
            bool isLeft = rewardPage == 0;
            btnRightArrow.gameObject.SetActive(isLeft);
            btnLeftArrow.gameObject.SetActive(!isLeft);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            //关闭界面？
            this.CloseSelf();
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time && txtRemaningTime != null)
            {
                txtRemaningTime.text = LanguageHelper.GetTextContent(2023908, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4));//剩余时间：{0}
            }
        }
        private void OnUpdateSevenDaysTargetData()
        {
            UpdateView();
        }
        private void OnCreateCell_MenuList(InfinityGridCell cell)
        {
            UI_SevenDaysTargetMenuCell mCell = new UI_SevenDaysTargetMenuCell();
            mCell.Init(cell.mRootTransform.transform);
            mCell.RegisterAction(OnMenuSelect);
            cell.BindUserData(mCell);
            dictMenus.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange_MenuList(InfinityGridCell cell, int index)
        {
            UI_SevenDaysTargetMenuCell mCell = cell.mUserData as UI_SevenDaysTargetMenuCell;
            mCell.UpdateCellView(menuList[index]);
            mCell.UpdateSelectState(SelectedMenuId);
        }
        private void OnMenuSelect(uint menuId)
        {
            SelectedMenuId = menuId;
            foreach (var cell in dictMenus)
            {
                cell.Value.UpdateSelectState(SelectedMenuId);
            }
            UpdateView();
            infinityTarget.MoveToIndex(0);
        }
        private void OnCreateCell_TargetList(InfinityGridCell cell)
        {
            UI_SevenDaysTargetCell mCell = new UI_SevenDaysTargetCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
        }
        private void OnCellChange_TargetList(InfinityGridCell cell, int index)
        {
            UI_SevenDaysTargetCell mCell = cell.mUserData as UI_SevenDaysTargetCell;
            mCell.UpdateCellView(TargetDataList[index]);
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        private void OnBtnRightArrowClick()
        {
            rewardPage = 1;
            UpdateBottom();
        }
        private void OnBtnLeftArrowClick()
        {
            rewardPage = 0;
            UpdateBottom();
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            if (!Sys_OperationalActivity.Instance.CheckSevenDaysTargetIsOpen())
            {
                this.CloseSelf();
            }
        }
        #endregion

        #region 模块Class
        public class UI_SevenDaysTargetMenuCell
        {
            private uint menuId;//这个id就是天数(第几天)

            private Transform transform;
            private Button btnSelf;
            private GameObject goUnSelected;
            private GameObject goSelected;
            private GameObject goRedPoint;
            private GameObject goLock;
            private Text txtUnSelectedText;
            private Text txtSelectedText;
            private GameObject goIsGetOnSelected;
            private GameObject goIsGetOnUnSelected;
            private Action<uint> action;

            public void Init(Transform trans)
            {
                transform = trans;
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnSelfClick);
                goUnSelected = transform.Find("Image_UnSelected").gameObject;
                goSelected = transform.Find("Image_Selected").gameObject;
                goRedPoint = transform.Find("Image_Dot").gameObject;
                goLock = transform.Find("Image_UnSelected/Image_Lock").gameObject;
                txtUnSelectedText = transform.Find("Image_UnSelected/Text").GetComponent<Text>();
                txtSelectedText = transform.Find("Image_Selected/Text_Select").GetComponent<Text>();
                goIsGetOnSelected = transform.Find("Image_Selected/Image_Get").gameObject;
                goIsGetOnUnSelected = transform.Find("Image_UnSelected/Image_Get").gameObject;
            }

            public void UpdateCellView(uint _menuId)
            {
                menuId = _menuId;
                txtUnSelectedText.text = txtSelectedText.text = LanguageHelper.GetTextContent(2023906, menuId.ToString());//第{0}天
                goRedPoint.SetActive(Sys_OperationalActivity.Instance.CheckSevenDaysTargetTaskRedPoint(menuId));
                var isLock = !Sys_OperationalActivity.Instance.CheckSevenDaysTargetDayTaskIsOpen(menuId);
                goLock.SetActive(isLock);
            }
            public void UpdateSelectState(uint SelectId)
            {
                bool isSelect = SelectId == menuId;
                bool isAllFinish = Sys_OperationalActivity.Instance.CheckSevenDaysTargetIsGetByDay(menuId);
                goIsGetOnSelected.SetActive(isAllFinish);
                goIsGetOnUnSelected.SetActive(isAllFinish);
                goSelected.SetActive(isSelect);
                goUnSelected.SetActive(!isSelect);
            }
            public void RegisterAction(Action<uint> act)
            {
                action = act;
            }
            private void OnSelfClick()
            {
                if (Sys_OperationalActivity.Instance.CheckSevenDaysTargetDayTaskIsOpen(menuId, true))
                {
                    action?.Invoke(menuId);
                }
            }
        }

        public class UI_SevenDaysTargetCell
        {
            private CSVActivityTarget.Data targetData;
            private bool isFinish;
            private bool isGet;

            private Transform transform;
            private PropItem item;
            private GameObject goItem;
            private Text txtTitle;
            private Text txtDesc;
            private Text txtNum;
            private Slider slider;
            private Button btnGet;
            private Text txtBtnText;
            private GameObject goIsGet;
            private GameObject goMark1;
            private GameObject goMark2;

            public void Init(Transform trans)
            {
                transform = trans;
                goItem = transform.Find("PropItem").gameObject;
                item = new PropItem();
                item.BindGameObject(goItem);
                txtTitle = transform.Find("Text").GetComponent<Text>();
                txtDesc = transform.Find("Text01").GetComponent<Text>();
                txtNum = transform.Find("Text_Num").GetComponent<Text>();
                slider = transform.Find("Slider_Exp").GetComponent<Slider>();
                btnGet = transform.Find("View_State/Btn_01").GetComponent<Button>();
                btnGet.onClick.AddListener(OnBtnGetClick);
                txtBtnText = transform.Find("View_State/Btn_01/Text_01").GetComponent<Text>();
                goIsGet = transform.Find("View_State/Image_Get").gameObject;
                goMark1 = transform.Find("View_Mark/Image_Mark01").gameObject;
                goMark2 = transform.Find("View_Mark/Image_Mark02").gameObject;
            }

            public void UpdateCellView(CSVActivityTarget.Data data)
            {
                targetData = data;
                var dropItems = CSVDrop.Instance.GetDropItem(data.Dropid);
                if (dropItems != null && dropItems.Count > 0)
                {
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItems[0].id, dropItems[0].count, true, false, false, false, false, true);
                    item.SetData(itemData, EUIID.UI_SevenDaysTarget);
                }
                txtTitle.text = LanguageHelper.GetTextContent(targetData.Titleid);
                txtDesc.text = Sys_OperationalActivity.Instance.GetSevenDaysTargetTaskDesc(targetData);

                var curNum = Sys_OperationalActivity.Instance.GetTargetTaskProgressCurNum(targetData.id);
                var maxNum = Sys_OperationalActivity.Instance.GetTargetTaskProgressMaxNum(targetData.id);
                txtNum.text = curNum.ToString() +"/" + maxNum.ToString();
                slider.value = (float)curNum / (float)maxNum;
                isFinish = Sys_OperationalActivity.Instance.CheckTargetIsFinish(targetData.id);
                isGet = Sys_OperationalActivity.Instance.CheckTargetTaskRewardIsGet(targetData.id);
                if (isGet)
                {
                    btnGet.gameObject.SetActive(false);
                    goIsGet.SetActive(true);
                    //ImageHelper.SetImageGray(transform.GetComponent<Image>(), true, true);
                }
                else
                {
                    btnGet.gameObject.SetActive(true);
                    goIsGet.SetActive(false);
                    //ImageHelper.SetImageGray(transform.GetComponent<Image>(), false, true);
                    if (isFinish)
                    {
                        txtBtnText.text = LanguageHelper.GetTextContent(4701);//领取
                    }
                    else
                    {
                        txtBtnText.text = LanguageHelper.GetTextContent(2023905);//前往参与
                    }
                }
                goMark1.SetActive(targetData.ActivityType == 1);
                goMark2.SetActive(targetData.ActivityType == 2);
            }

            private void OnBtnGetClick()
            {
                if (isFinish && !isGet)
                {
                    bool isOpen = Sys_OperationalActivity.Instance.CheckSevenDaysTargetDayTaskIsOpen(targetData.RankType);
                    if (isOpen)
                    {
                        Sys_OperationalActivity.Instance.GetSevenDaysTargetTaskRewardReq(targetData.id);
                    }
                }
                else
                {
                    Sys_OperationalActivity.Instance.SevenDaysTargetTaskToGo(targetData);
                }
            }
        }

        public class UI_SevenDaysTargetScoreItemCell
        {
            private CSVCumulativeReward.Data rewardData;
            private Transform transform;
            private PropItem item;
            private Text txtScore;
            private ItemIdCount itemIdCountData;
            private GameObject goSelect;
            public void Init(Transform trans)
            {
                transform = trans;
                item = new PropItem();
                item.BindGameObject(transform.Find("Image/PropItem").gameObject);
                txtScore = transform.Find("Text_Num").GetComponent<Text>();
                goSelect = transform.Find("Image/PropItem/Fx_ui_PropItem01")?.gameObject;
            }

            public void UpdateCellView(CSVCumulativeReward.Data data)
            {
                rewardData = data;
                var dropItems = CSVDrop.Instance.GetDropItem(rewardData.Dropid);
                bool canGet = Sys_OperationalActivity.Instance.CheckSevenDaysTargetRewardCanGet(rewardData.id);
                bool isGet = Sys_OperationalActivity.Instance.CheckSevenDaysTargetRewardIsGet(rewardData.id);
                if (dropItems != null && dropItems.Count > 0)
                {
                    var useTips = !canGet;
                    itemIdCountData = dropItems[0];
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItems[0].id, dropItems[0].count, true, false, false, false, false, true, false, true, OnItemClick, true, false);
                    item.SetData(itemData, EUIID.UI_SevenDaysTarget);
                }
                goSelect?.SetActive(canGet);
                item.SetGot(isGet);
                txtScore.text = rewardData.Requiredpoints.ToString();
            }

            private void OnItemClick(PropItem itemData)
            {
                uint rewardId = rewardData.id;
                bool canGet = Sys_OperationalActivity.Instance.CheckSevenDaysTargetRewardCanGet(rewardId);
                if (canGet)
                {
                    //请求领奖
                    Sys_OperationalActivity.Instance.GetSevenDaysTargetScoreRewardReq(rewardId);
                }
                else if (itemIdCountData != null)
                {
                    if (rewardData.id == 7)
                    {
                        CSVParam.Data paramData = CSVParam.Instance.GetConfData(1286);
                        if (paramData != null)
                        {
                            string[] strs = paramData.str_value.Split('|');
                            if (uint.Parse(strs[1]) == itemIdCountData.id)
                            {
                                //伙伴跳转
                                PropMessageParam propParam = new PropMessageParam();
                                ItemData mItemData = new ItemData(0, 0, itemIdCountData.id, (uint)itemIdCountData.count, 0, false, false, null, null, 0);
                                propParam.itemData = mItemData;
                                propParam.showBtnCheck = true;
                                propParam.targetEUIID = uint.Parse(strs[2]);
                                if (strs.Length >= 4)
                                {
                                    propParam.checkOpenParam = uint.Parse(strs[3]);
                                }
                                propParam.sourceUiId = EUIID.UI_SevenDaysTarget;
                                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
                            }
                        }
                    }
                    else
                    {

                        PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemIdCountData.id, itemIdCountData.count, false, false, false, false, false, false, true);
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Decompose, showItemData));
                    }
                }
            }
        }
        #endregion
    }

}
