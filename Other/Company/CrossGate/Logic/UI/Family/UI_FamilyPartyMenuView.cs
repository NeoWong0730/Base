using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System;
using Framework;

namespace Logic
{
    public class UI_FamilyPartyMenuView : UIComponent
    {
        private Button btnArrow;
        private bool arrowShow = true;
        private bool isOpened = false;//重复打开检测
        private GameObject goLeftArrow;
        private GameObject goRightArrow;
        private GameObject goLeftViewOne;
        private GameObject goLeftViewTwo;

        private Text txtTimeOutside;
        private Text txtTimeInside;
        private Text txtStage;//酒会阶段
        private Text txtFoodName;
        private Text txtStageTime;
        private Text txtExp;
        private PropItem propItem;
        private Slider sliderExp;
        private Text txtSliderExp;
        private Text txtStarNum;
        private Button btnGetMat;//领食材
        private Button btnCook;
        private Button btnSubmit;

        private Text txtTimeInside2;
        private Text txtStage2;//酒会阶段
        private Text txtStageTime2;
        private Text txtTasteNum;//享用菜肴次数
        private Text txtStarNum2;

        private Timer timer;
        private float countDownTime = 0;
        private Timer hideTimer;//隐藏倒计时，跑阶段节点
        private float countDownHideTime;//隐藏倒计时的时间
        private int partyStage = -1;//酒会阶段

        private Timer popupTimer;//弹窗倒计时(弹场景外提示窗的倒计时)
        private float countDownPopupTime = 0;//弹窗倒计时的时间

        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            hideTimer?.Cancel();
            popupTimer?.Cancel();
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, false);
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            Sys_Family.Instance.GetCuisineInfoReq();
        }
        public override void Hide()
        {
            timer?.Cancel();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartyDataUpdate, UpdateView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnPartyValueNtfUpdate, OnPartyValueNtfUpdate, toRegister);
            Sys_CollectItem.Instance.eventEmitter.Handle<uint>(Sys_CollectItem.EEvents.OnCollectSuccess, OnCollectSuccess, toRegister);

        }
        #endregion
        #region func
        private void Parse()
        {
            btnArrow = transform.Find("Button").GetComponent<Button>();
            btnArrow.onClick.AddListener(OnBtnArrowClick);
            goLeftArrow = transform.Find("Button/Image1").gameObject;
            goRightArrow = transform.Find("Button/Image2").gameObject;
            goLeftViewOne = transform.Find("Left").gameObject;
            goLeftViewTwo = transform.Find("Left1").gameObject;

            txtTimeOutside = transform.Find("Text_Time").GetComponent<Text>();

            txtTimeInside = transform.Find("Left/Content/Time").GetComponent<Text>();
            txtStage = transform.Find("Left/Content/Stage").GetComponent<Text>();
            txtStageTime = transform.Find("Left/Content/Stage_Time").GetComponent<Text>();
            txtFoodName = transform.Find("Left/Content/Taste_Num").GetComponent<Text>();
            txtExp = transform.Find("Left/Content/EXP").GetComponent<Text>();
            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Left/Content/PropItem").gameObject);
            sliderExp = transform.Find("Left/Slider_Exp").GetComponent<Slider>();
            txtSliderExp = transform.Find("Left/Slider_Exp/Text_Num").GetComponent<Text>();
            txtStarNum = transform.Find("Left/Star_Num").GetComponent<Text>();
            btnGetMat = transform.Find("Left/Buttons/Btn_01").GetComponent<Button>();
            btnGetMat.onClick.AddListener(OnBtnGetMatClick);
            btnCook = transform.Find("Left/Buttons/Btn_02").GetComponent<Button>();
            btnCook.onClick.AddListener(OnBtnCookClick);
            btnSubmit = transform.Find("Left/Buttons/Btn_03").GetComponent<Button>();
            btnSubmit.onClick.AddListener(OnBtnSubmitClick);

            txtTimeInside2 = transform.Find("Left1/Content/Time").GetComponent<Text>();
            txtStage2 = transform.Find("Left1/Content/Stage").GetComponent<Text>();
            txtStageTime2 = transform.Find("Left1/Content/Stage_Time").GetComponent<Text>();
            txtTasteNum = transform.Find("Left1/Content/Taste_Num").GetComponent<Text>();
            txtStarNum2 = transform.Find("Left1/Star_Num").GetComponent<Text>();

            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, true);
        }
        public void Init()
        {
            Sys_Family.Instance.PopupFamilyPartyOutsideHint();
            Sys_Family.Instance.PopupFamilyPartyGetTeamInsideHint();
            StartPopupTimer();
            bool show = Sys_Family.Instance.CheckFamilyPartyMenuViewCanShow();
            if (show)
            {
                partyStage = Sys_Family.Instance.GetFamilyPartyStage();
                if (partyStage >= 0)
                {
                    this.Show();
                    if (!isOpened)
                    {
                        arrowShow = true;//策划要求，默认打开界面
                    }
                    isOpened = true;
                    UpdateView();
                }
                StartHideTimer();
            }
            else
            {
                isOpened = false;
                hideTimer?.Cancel();
            }
        }
        private void UpdateArrowState(bool state)
        {
            goLeftArrow.SetActive(!state);
            goRightArrow.SetActive(state);
        }
        private void UpdateView()
        {
            partyStage = Sys_Family.Instance.GetFamilyPartyStage();
            if (partyStage >= 0)
            {
                UpdateArrowState(arrowShow);
                UpdateTimerView();
                UpdateLeftView();
            }
            else
            {
                //加个检测，不在酒会时间内，直接隐藏
                this.Hide();
            }
        }

        private void UpdateLeftView()
        {
            if (arrowShow)
            {
                if (partyStage == 1 || partyStage == 0)
                {
                    goLeftViewOne.gameObject.SetActive(true);
                    goLeftViewTwo.gameObject.SetActive(false);
                    UpdateLeftViewOne();
                }
                else if (partyStage == 2)
                {
                    goLeftViewOne.gameObject.SetActive(false);
                    goLeftViewTwo.gameObject.SetActive(true);
                    UpdateLeftViewTwo();
                }
            }
            else
            {
                goLeftViewOne.gameObject.SetActive(false);
                goLeftViewTwo.gameObject.SetActive(false);
            }
        }
        /// <summary> 刷新1阶段面板 </summary>
        private void UpdateLeftViewOne()
        {
            if (partyStage == 0)
            {
                txtStage.text = LanguageHelper.GetTextContent(6265);//准备阶段标题
            }
            else if(partyStage == 1)
            {
                txtStage.text = LanguageHelper.GetTextContent(6238);//1阶段标题
            }

            CSVCharacterAttribute.Data levelData = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level);
            var expRate = levelData.FamilyReceptionSecondExp * 60;
            txtExp.text = LanguageHelper.GetTextContent(6241, expRate.ToString());//{0}EXP/{1}秒

            var id = Sys_Family.Instance.GetFashionFoodId();
            var itemData = new PropIconLoader.ShowItemData(id, 0, true, false, false, false, false);
            var foodItemData = CSVItem.Instance.GetConfData(id);
            if (foodItemData != null)
            {
                txtFoodName.text = LanguageHelper.GetTextContent(foodItemData.name_id);
            }
            propItem.SetData(itemData, EUIID.UI_Menu);

            var curValue = Sys_Family.Instance.familyData.familyPartyInfo.partyValue;
            var maxValue = Sys_Family.Instance.GetPartyMaxValue();
            sliderExp.value = (float)curValue / (float)maxValue;
            txtSliderExp.text = LanguageHelper.GetTextContent(4733, curValue.ToString(), maxValue.ToString());
            txtStarNum.text = LanguageHelper.GetTextContent(6230, Sys_Family.Instance.GetPartyStarNumByPartyValue(curValue).ToString());
            var isGetMat = Sys_Family.Instance.familyData.familyPartyInfo.isFoodMatGet;
            ImageHelper.SetImageGray(btnGetMat.GetComponent<Image>(), isGetMat, true);
        }
        /// <summary> 刷新2阶段面板 </summary>
        private void UpdateLeftViewTwo()
        {
            var stageTime = uint.Parse(CSVParam.Instance.GetConfData(1037).str_value) - uint.Parse(CSVParam.Instance.GetConfData(1031).str_value) - uint.Parse(CSVParam.Instance.GetConfData(1035).str_value) * 60;
            txtStage2.text = LanguageHelper.GetTextContent(6239);//2阶段标题
            txtTasteNum.text = LanguageHelper.GetTextContent(6242, Sys_Family.Instance.GetPartyTasteNum().ToString());//{0}次
            var curValue = Sys_Family.Instance.familyData.familyPartyInfo.partyValue;
            txtStarNum2.text = LanguageHelper.GetTextContent(6230, Sys_Family.Instance.GetPartyStarNumByPartyValue(curValue).ToString());
        }
        private void UpdateTimerView()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var partyEndTime = Sys_Family.Instance.GetPartyEndTimestamp();
            countDownTime = partyEndTime - nowtime;
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        /// <summary> 隐藏倒计时启动 </summary>
        private void StartHideTimer()
        {
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var targetTime = Sys_Family.Instance.GetPartyNextStageStartTimeStamp();
            countDownHideTime = targetTime - nowtime;
            hideTimer?.Cancel();
            hideTimer = Timer.Register(countDownHideTime, OnHideTimerComplete, OnHideTimerUpdate, false, false);
        }
        /// <summary>
        /// 酒会阶段开始的弹框
        /// </summary>
        private void PartyStageStartPopup()
        {
            UIManager.CloseUI(EUIID.UI_FamilyParty_Popup);
            if (partyStage == 1)
            {
                //酒会开始
                Sys_Family.Instance.PopupFamilyPartyStageView(EFamilyPartyPopupType.PartyStart);
            }
            else if (partyStage == 2)
            {
                //酒会2阶段
                Sys_Family.Instance.PopupFamilyPartyStageView(EFamilyPartyPopupType.FoodBattle);
            }
            else if (partyStage == -1)
            {
                //酒会结束
                Sys_Family.Instance.PopupFamilyPartyStageView(EFamilyPartyPopupType.PartyEnd);
            }
        }
        /// <summary> 弹窗倒计时启动 </summary>
        private void StartPopupTimer()
        {
            var partyStartTime = Sys_Family.Instance.GetPartyStartTimestamp();
            uint nowtime = Sys_Time.Instance.GetServerTime();
            var cd = partyStartTime - nowtime;
            countDownPopupTime = cd > 0 ? cd : cd + 86400;
            popupTimer?.Cancel();
            popupTimer = Timer.Register(countDownPopupTime, OnPopupTimerComplete, null, false, false);
        }
        #endregion
        #region event
        private void OnBtnArrowClick()
        {
            arrowShow = !arrowShow;
            UpdateView();
        }
        private void OnBtnGetMatClick()
        {
            //直接领食材
            if (Sys_Family.Instance.familyData.familyPartyInfo.isFoodMatGet)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6240));//今日食材已领取
            }
            else
            {
                Sys_Family.Instance.GetCuisineIngredientReq();
            }
        }
        private void OnBtnCookClick()
        {
            uint npcInfoId = 1521009u;
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcInfoId, true);
        }
        private void OnBtnSubmitClick()
        {
            Sys_Family.Instance.GoToFamilyParty();
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            this.Hide();
        }
        private void OnTimerUpdate(float time)
        {
            if (countDownTime >= time)
            {
                if (txtTimeOutside != null)
                {
                    txtTimeOutside.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
                }
                if (arrowShow)
                {
                    if ((partyStage == 1 || partyStage == 0) && txtTimeInside != null)
                    {
                        txtTimeInside.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
                    }
                    else if (partyStage == 2 && txtTimeInside2 != null)
                    {
                        txtTimeInside2.text = LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1);
                    }
                }
            }
        }
        private void OnHideTimerComplete()
        {
            hideTimer?.Cancel();
            partyStage = Sys_Family.Instance.GetFamilyPartyStage();
            if (partyStage == 0 || partyStage == 1)
            {
                arrowShow = true;
                this.Show();
            }
            UpdateView();
            PartyStageStartPopup();
            StartHideTimer();
        }
        private void OnHideTimerUpdate(float time)
        {
            if (countDownTime >= time && arrowShow)
            {
                if ((partyStage == 1 || partyStage == 0) && txtStageTime != null)
                {
                    txtStageTime.text = LanguageHelper.TimeToString((uint)(countDownHideTime - time), LanguageHelper.TimeFormat.Type_1);
                }
                else if (partyStage == 2 && txtStageTime2 != null)
                {
                    txtStageTime2.text = LanguageHelper.TimeToString((uint)(countDownHideTime - time), LanguageHelper.TimeFormat.Type_1);
                }
            }
        }
        private void OnPartyValueNtfUpdate()
        {
            if (partyStage == 1 || partyStage == 0)
            {
                var curValue = Sys_Family.Instance.familyData.familyPartyInfo.partyValue;
                var maxValue = Sys_Family.Instance.GetPartyMaxValue();
                sliderExp.value = (float)curValue / (float)maxValue;
                txtSliderExp.text = LanguageHelper.GetTextContent(4733, curValue.ToString(), maxValue.ToString());
                txtStarNum.text = LanguageHelper.GetTextContent(6230, Sys_Family.Instance.GetPartyStarNumByPartyValue(curValue).ToString());
            }
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            StartPopupTimer();
            bool show = Sys_Family.Instance.CheckFamilyPartyMenuViewCanShow();
            if (show)
            {
                partyStage = Sys_Family.Instance.GetFamilyPartyStage();
                if (partyStage >= 0)
                {
                    UpdateView();
                }
                else
                {
                    this.Hide();
                }
                StartHideTimer();
            }
        }

        private void OnCollectSuccess(uint npcId)
        {
            UpdateLeftView();
        }

        private void OnPopupTimerComplete()
        {
            popupTimer?.Cancel();
            Sys_Family.Instance.PopupFamilyPartyOutsideHint();
            StartPopupTimer();
        }
        #endregion
    }
}
