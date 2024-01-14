using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using Framework;

namespace Logic
{
    public class UI_ServerActivityMenu : UIComponent
    {
        private Animator animator01;
        private Timer time;
        private Button menuButton;
        private GameObject menu;
        private Image m_Open_Icon;
        //private GameObject m_redPoint;

        //private Button qaButton;
        private Button m_OperationalActivity;
        private Button m_SevenDaysSign;
        private Text text_SevenDaysSignName;
        private Image image_SevenDaysSignItem;
        private Text text_SevenDaysSignTime;
        private GameObject go_SevenDaysSignState;
        private GameObject go_SevenDaysSignEffect;
        private Timer timer_SevenDaysSign;
        private GameObject m_OPActivtyRedPoint;
        private Button btnFirstCharge;
        private Button btnPromotion;
        private GameObject goFirstChargeRedPoint;

        private Button btn_Lotto;
        private GameObject goLottoRedPoint;

        /// <summary> 七日目标按钮 </summary>
        private Button btnSevendaysTarget;
        private GameObject goSevendaysTargetRedPoint;
        /// <summary> 限时礼包按钮(水蓝鼠宝藏) </summary>
        private Button btnTimelimitGift;
        private Text txtLimitGiftTime;
        private Text txtLimitGiftNum;
        /// <summary> 家族兽训练按钮 </summary>
        private Button btnFamilyTrain;

        private Timer limitGiftTimer;
        private float limitGiftCD;
        /// <summary> 天降红包 </summary>
        private Button btnRedEnvelopeRain;
        private Animator redEnvelopeRainAnimator;
        private GameObject Fx_RainObj;
        private Text textRainTime;
        //private Timer rainTimer;
        //private float rainTimeCD;
        private Timer rainHintTimer;
        private float rainHintTimeCD;       
        
        /// <summary> 限时活动 </summary>
        //private Button SummerActivityButton;
        //private GameObject limitTimeActivityRedPoint;
        //private Timer SummerActivityTimer;

        /// <summary> 通用活动 </summary>
        private Button TopicActivityButton;
        private Timer TopicActivityTimer;
        private GameObject limitTimeActivityRedPoint;
        private uint TopicActivityUid=0;//界面euid

        /// <summary> 回归活动 </summary>
        private Button BtnBackActivity;
        private GameObject goBackActivityRedPoint;
        private Timer backActivityTimer;
        private Text txtBackActivityTime;
        private float backActivityCD;

        /// <summary> 活动2048 </summary>
        private Button BtnActivity2048;
        private GameObject goActivity2048RedPoint;
        private Timer activity2048Timer;
        private Text txtActivity2048Time;
        private float activity2048CD;

        /// <summary> pk大赛 </summary>
        private Button BtnPKMatch;
        private Timer pkMatchTimer;
        private Text txtpkMatchTime;
        private float pkMatchCD;

        private Button BtnPKWeb;
        private Timer pkWebTimer;

        public class ButtonActivity
        {
            private Transform transform;
            private Image imgIcon;
            private Text txtName;
            private Transform transRed;

            //private uint m_IndexId;
            private CSVActivityMainUi.Data csvData;
            //private CSVOperationalActivityRuler.Data csvRulerData;
            private ActivityInfo svrInfo;
            private uint _indexId;

            public void Init(Transform trans)
            {
                transform = trans;

                Button btn = transform.GetComponent<Button>();
                btn.onClick.AddListener(OnClick);
                
                imgIcon = transform.Find("Image_Icon1").GetComponent<Image>();
                txtName = transform.Find("Text").GetComponent<Text>();
                transRed = transform.Find("Image_Dot");
            }

            public void SetData(uint indexId)
            {
                _indexId = indexId;
                RefreshActivityIndex();
            }

            public void RefreshActivityIndex()
            {
                csvData = CSVActivityMainUi.Instance.GetConfData(_indexId);
                if (csvData != null)
                {
                    svrInfo = Sys_ActivityOperationRuler.Instance.GetOpenActivity(csvData.Activity_Id);
                    if (svrInfo != null)
                    {
                        int index = csvData.Activity_Id.IndexOf(svrInfo.infoId);
                        ImageHelper.SetIcon(imgIcon, csvData.IntIcon[index]);
                        if (csvData.IconTitle[index] == 0u)
                            txtName.text = "";
                        else 
                            txtName.text = LanguageHelper.GetTextContent(csvData.IconTitle[index]);
                    }
                }
            }

            private void OnClick()
            {
                if (csvData != null && svrInfo != null)
                {
                    if (csvData.UiParam != null && csvData.UiParam.Count > 0)
                    {
                        int index = csvData.Activity_Id.IndexOf(svrInfo.infoId);
                        UIManager.OpenUI((EUIID)csvData.UiId, false, csvData.UiParam[index]);
                    }
                    else
                    {
                        UIManager.OpenUI((EUIID)csvData.UiId);
                    }
                }
            }

            public void RfreshState()
            {
                if (csvData != null)
                {
                    if (svrInfo != null)
                    {
                        bool isCanShow = true;
                        //鼠王存钱罐入口紧急开关
                        if (svrInfo.csvData.Product_Type == 13)
                            isCanShow = Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(213);
                        if (Sys_FunctionOpen.Instance.IsOpen(csvData.Function_Id) && isCanShow)
                        {
                            uint nowTime = Sys_Time.Instance.GetServerTime();
                            uint beginTime = TimeManager.ConvertFromZeroTimeZone(svrInfo.csvData.Begining_Date);
                            uint endTime = beginTime + svrInfo.csvData.Duration_Day * 3600 * 24;
                            
                            if (nowTime >= beginTime && nowTime <= endTime)
                            {
                                transform.gameObject.SetActive(true);
                                bool red = false;
                                if (svrInfo.csvData.Product_Type == 13)
                                {
                                    red = Sys_ActivitySavingBank.Instance.CheckRedPoint();
                                }
                                else if (svrInfo.csvData.Product_Type == 8 || svrInfo.csvData.Product_Type == 9)
                                {
                                    if (Sys_ItemExChange.Instance.isActivity)
                                        red = red || Sys_ItemExChange.Instance.hasRed();
                                    if (Sys_ActivityQuest.Instance.isActivity)
                                        red = red || Sys_ActivityQuest.Instance.hasRed();
                                }
                                else if (svrInfo.csvData.Product_Type == 103 || svrInfo.csvData.Product_Type == 104)
                                {
                                    if (Sys_ItemExChange.Instance.isHeFuActivity)
                                        red = red || Sys_ItemExChange.Instance.hasHeFuRed();
                                    if (Sys_ActivityQuest.Instance.isHeFuActivity)
                                        red = red || Sys_ActivityQuest.Instance.hasHeFuRed();
                                }else if (svrInfo.csvData.Product_Type == 102)
                                {
                                    red = Sys_ActivityTopic.Instance.CheckActivitySignRedPoint(1);
                                }
                                //if (svrInfo.csvData.Product_Type == 8)
                                //    red = Sys_ItemExChange.Instance.hasRed();
                                //else if(svrInfo.csvData.Product_Type == 9)
                                //    red = Sys_ActivityQuest.Instance.hasRed();
                                //else if(svrInfo.csvData.Product_Type == 13)
                                    
                                //if (svrInfo.csvData.Product_Type == 8 || svrInfo.csvData.Product_Type == 9 || svrInfo.csvData.Product_Type == 13)
                                //{
                                //    red = Sys_ItemExChange.Instance.hasRed() | Sys_ActivityQuest.Instance.hasRed() | Sys_ActivitySavingBank.Instance.CheckRedPoint();
                                //}

                                transRed.gameObject.SetActive(red);
                                return;
                            }
                        }
                    
                        transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        svrInfo = Sys_ActivityOperationRuler.Instance.GetOpenActivity(csvData.Activity_Id);
                        if (svrInfo != null)
                        {
                            int index = csvData.Activity_Id.IndexOf(svrInfo.infoId);
                            ImageHelper.SetIcon(imgIcon, csvData.IntIcon[index]);
                            if (csvData.IconTitle[index] == 0u)
                                txtName.text = "";
                            else 
                                txtName.text = LanguageHelper.GetTextContent(csvData.IconTitle[index]);
                        }
                    }
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }
        
        public List<ButtonActivity> listBtnActivitys = new List<ButtonActivity>();
        
        protected override void Loaded()
        {
            base.Loaded();
            menuButton = transform.Find("Button01").GetComponent<Button>();
            //qaButton = transform.Find("Grid01/Button_Qa").GetComponent<Button>();
            menu = transform.Find("Grid01").gameObject;
            m_Open_Icon = menuButton.transform.Find("Image_Icon").GetComponent<Image>();
            //m_redPoint = qaButton.transform.Find("Image_Dot").gameObject;
            menuButton.onClick.AddListener(OnClickOpen);
            //qaButton.onClick.AddListener(OnQaButtonClick);

            m_OperationalActivity = transform.Find("Grid01/Button_Activity_Operation").GetComponent<Button>();
            m_OperationalActivity.gameObject.SetActive(false);
            m_OperationalActivity.onClick.AddListener(OnClickOperationalActivity);
            m_OPActivtyRedPoint = m_OperationalActivity.transform.Find("Image_Dot").gameObject;

            m_SevenDaysSign = transform.Find("Grid01/Button_SevenDays").GetComponent<Button>();
            m_SevenDaysSign.gameObject.SetActive(false);
            m_SevenDaysSign.onClick.AddListener(OnClickSevenDaysSign);

            text_SevenDaysSignName = m_SevenDaysSign.transform.Find("Text").GetComponent<Text>();
            image_SevenDaysSignItem = m_SevenDaysSign.transform.Find("Image_Icon1").GetComponent<Image>();
            text_SevenDaysSignTime = m_SevenDaysSign.transform.Find("Text_Time").GetComponent<Text>();
            go_SevenDaysSignState = m_SevenDaysSign.transform.Find("Text_State").gameObject;
            go_SevenDaysSignEffect = m_SevenDaysSign.transform.Find("Fx_ui_yindao_ring").gameObject;


            btnFirstCharge = transform.Find("Grid01/Button_FirstCharge").GetComponent<Button>();
            btnFirstCharge.onClick.AddListener(OnClickFirstCharge);
            //btnPromotion = transform.Find("Grid01/Button_Promotion").GetComponent<Button>();
            //btnPromotion.onClick.AddListener(OnClickPromotionCharge);

            btn_Lotto = transform.Find("Grid01/Button_Lotto").GetComponent<Button>();
            btn_Lotto.onClick.AddListener(onClickLotto);
            goLottoRedPoint = btn_Lotto.transform.Find("Image_Dot").gameObject;

            goFirstChargeRedPoint = btnFirstCharge.transform.Find("Image_Dot").gameObject;

            btnSevendaysTarget = transform.Find("Grid01/Button_SevenTarget").GetComponent<Button>();
            btnSevendaysTarget.onClick.AddListener(OnClickSevenDaysTarget);
            goSevendaysTargetRedPoint = btnSevendaysTarget.transform.Find("Image_Dot").gameObject;

            btnTimelimitGift = transform.Find("Grid01/Button_Condition_Gift").GetComponent<Button>();
            btnTimelimitGift.onClick.AddListener(OnClickBtnTimelimitGift);
            txtLimitGiftTime = transform.Find("Grid01/Button_Condition_Gift/Text_Time").GetComponent<Text>();
            txtLimitGiftNum = transform.Find("Grid01/Button_Condition_Gift/Tips/Text").GetComponent<Text>();

            btnRedEnvelopeRain = transform.Find("Grid01/Button_RedEnvelopeRain").GetComponent<Button>();
            btnRedEnvelopeRain.onClick.AddListener(OnClickRedEnvelopeRain);
            textRainTime= transform.Find("Grid01/Button_RedEnvelopeRain/Text_Time").GetComponent<Text>();
            redEnvelopeRainAnimator = btnRedEnvelopeRain.GetComponent<Animator>();
            Fx_RainObj = btnRedEnvelopeRain.transform.Find("Fx_ui").gameObject;
            Fx_RainObj.SetActive(false);

            btnFamilyTrain = transform.Find("Grid01/Button_Familycreatures").GetComponent<Button>();
            btnFamilyTrain.onClick.AddListener(OnFamilyTrainBtnClicked);

            TopicActivityButton = transform.Find("Grid01/Button_RedEnvelopeRain (1)").GetComponent<Button>();
            TopicActivityButton.onClick.AddListener(OnClickActivityTopic);
            limitTimeActivityRedPoint= TopicActivityButton.transform.Find("Image_Dot").gameObject;

            BtnBackActivity = transform.Find("Grid01/Button_ComeBackGift").GetComponent<Button>();
            BtnBackActivity.onClick.AddListener(OnBtnBackActivityClick);
            goBackActivityRedPoint = transform.Find("Grid01/Button_ComeBackGift/Image_Dot").gameObject;
            txtBackActivityTime = transform.Find("Grid01/Button_ComeBackGift/Text_Time").GetComponent<Text>();

            BtnActivity2048 = transform.Find("Grid01/Button_LittleGame").GetComponent<Button>();
            BtnActivity2048.onClick.AddListener(OnBtnActivity2048Click);
            goActivity2048RedPoint = transform.Find("Grid01/Button_LittleGame/Image_Dot").gameObject;
            txtActivity2048Time = transform.Find("Grid01/Button_LittleGame/Text_Time").GetComponent<Text>();

            BtnPKMatch = transform.Find("Grid01/Button_PKCompetition").GetComponent<Button>();
            txtpkMatchTime = transform.Find("Grid01/Button_PKCompetition/Text_Time").GetComponent<Text>();
            BtnPKMatch.onClick.AddListener(OnClickPKMatch);

            BtnPKWeb= transform.Find("Grid01/Button_PKWeb").GetComponent<Button>();
            BtnPKWeb.onClick.AddListener(OnClickPKWeb);

            listBtnActivitys.Clear();
            for (int i = 1; i < 10; ++i)
            {
                string str = string.Format("Grid01/Button_Activity_{0}", i);
                Transform trans = transform.Find(str);
                if (trans != null)
                {
                    ButtonActivity temp = new ButtonActivity();
                    temp.Init(trans);
                    temp.SetData((uint)i);
                    listBtnActivitys.Add(temp);
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, this.OnSubmited, toRegister);
            Sys_Qa.Instance.eventEmitter.Handle(Sys_Qa.EEvents.OnRefreshQARedPoint, OnUpdateQaData, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnRoleLevelUp, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnFunctionOpen, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSignSevenDayData, OnUpdateSignSevenDayData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle<uint>(Sys_OperationalActivity.EEvents.ReceiveSignReward, OnReceiveSignReward, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLevelGiftData, OnUpdateLevelGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateGrowthFundData, OnUpdateGrowthFundData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateTotalChargeData, OnUpdateTotalChargeData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSpecialCardData, OnUpdateSpecialCardData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateFirstChargeGiftData, RefreshFirstChargeState, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateLotteryActivityData, OnUpdateLotteryActivityData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateRankActivityData, OnUpdateRankActivityData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateSevenDaysTargetData, OnUpdateSevenDaysTargetData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateTimelimitGiftData, OnUpdateTimelimitGiftData, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalCharge, UpdateActivityTotalCharge, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalConsume, UpdateActivityTotalConsume, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, toRegister);
            Sys_Sign.Instance.eventEmitter.Handle(Sys_Sign.EEvents.UpdateDailySignAwardCount, OnUpdateDailySignAwardCount, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnCrossDay, OnCrossDay, toRegister);

            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, OnBeginEnter, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, OnUIEndExit, toRegister);
            
            Sys_ActivitySubscribe.Instance.eventEmitter.Handle(Sys_ActivitySubscribe.EEvents.OnActivityBegin, RefreshOPActivityRedPoint, toRegister);
            Sys_ActivitySubscribe.Instance.eventEmitter.Handle(Sys_ActivitySubscribe.EEvents.OnActivityEnd, RefreshOPActivityRedPoint, toRegister);
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshActivityPreviewData, OnRefreshRedEnvelopRainState, toRegister);
            Sys_RedEnvelopeRain.Instance.eventEmitter.Handle(Sys_RedEnvelopeRain.EEvents.OnRefreshRainStartBeforeHint, OnRefreshRainStartBeforeHint, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyPetTrainStarOrEnd, OnFamilyPetStarOrEndRefresh, toRegister);
            Sys_ActivityTopic.Instance.eventEmitter.Handle(Sys_ActivityTopic.EEvents.OnCommonActivityUpdate,OnActivityTopicUpdate, toRegister);
            Sys_PetExpediton.Instance.eventEmitter.Handle(Sys_PetExpediton.EEvents.OnPetExpeditonDataUpdate, OnActivityTopicUpdate, toRegister);
            Sys_ItemExChange.Instance.eventEmitter.Handle(Sys_ItemExChange.EEvents.e_UpdateRedState, OnActivityTopicUpdate, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle(Sys_ActivityQuest.EEvents.e_UpdateRedState, OnActivityTopicUpdate, toRegister);
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityRedPointUpdate, RefreshBackActivityState, toRegister);
            Sys_BackActivity.Instance.eventEmitter.Handle(Sys_BackActivity.EEvents.OnBackActivityInfoUpdate, RefreshBackActivityState, toRegister);
            Sys_Activity_2048.Instance.eventEmitter.Handle(Sys_Activity_2048.EEvents.OnActivity2048DataUpdate, RefreshActivity2048State, toRegister);
        }


        public override void Show()
        {
            base.Show();
            OnRefreshRedPoint();
            DebugUtil.LogFormat(ELogType.eQa, string.Format("OnShow==>hasQa:{0}", Sys_Qa.Instance.HasQa()));
            SetOperationalActivity();
            RefreshSevenDaysTargetState();
            RefreshTimelimitGiftState();
            OnRefreshRedEnvelopRainState();
            OnRefreshRainStartBeforeHint();
            CheckExpRetrieveFaceTips();
            OnFamilyPetStarOrEndRefresh();
            CheckActivityTopic();
            OnRefreshBtnActivitys();
            Sys_RedEnvelopeRain.Instance.CheckRedEnvelopeRainFaceIsShow();
            RefreshBackActivityState();
            RefreshActivity2048State();
            RefreshPKMatchBtn();
            RefreshPKWebBtn();
            OnRefreshActivityIds();
        }

        public override void Hide()
        {
            base.Hide();
            timer_SevenDaysSign?.Cancel();
            limitGiftTimer?.Cancel();
            TopicActivityTimer?.Cancel();
            //rainTimer?.Cancel();
            backActivityTimer?.Cancel();
            activity2048Timer?.Cancel();
            pkWebTimer?.Cancel();
            pkMatchTimer?.Cancel();
        }

        private void OnRefreshActivityIds() //针对活动商城id会变化
        {
            for (int i = 0; i < listBtnActivitys.Count; i++)
            {
                listBtnActivitys[i].RefreshActivityIndex();
            }
        }

        public void OnRefreshBtnActivitys()
        {
            for (int i = 0; i < listBtnActivitys.Count; i++)
            {
                listBtnActivitys[i].RfreshState();
            }
        }

        private void OnSubmited(int menuId, uint taskId, TaskEntry taskEntry)
        {
            RefreshQaShowState();
            OnRefreshRedPoint();
            SetOperationalActivity();
            RefreshSevenDaysTargetState();
            DebugUtil.LogFormat(ELogType.eQa, string.Format("OnSubmited taskId{0} ==>hasQa:{1}", taskId, Sys_Qa.Instance.HasQa()));
        }

        private void OnRoleLevelUp()
        {
            RefreshQaShowState();
            OnRefreshRedPoint();
            OnRefreshRedEnvelopRainState();
            OnRefreshRainStartBeforeHint();
            RefreshPKMatchBtn();
            DebugUtil.LogFormat(ELogType.eQa, string.Format("OnRoleLevelUp  ==>hasQa:{0}", Sys_Qa.Instance.HasQa()));
        }

        private void RefreshQaShowState()
        {
            Sys_Qa.Instance.hasShowRedPoint = false;
            int oldQaCount = Sys_Qa.Instance.Questionnaires.Count;
            bool flag = Sys_Qa.Instance.HasQa() && SDKManager.IsOpenGetExtJsonParam(SDKManager.EThirdSdkType.QuestionSurvey.ToString(), out string paramsValue);
            if (flag)
            {
                int curQaCount = Sys_Qa.Instance.Questionnaires.Count;
                if (curQaCount == oldQaCount)
                {
                    Sys_Qa.Instance.hasShowRedPoint = true;
                }
            }
        }

        private void OnCrossDay()
        {
            OnRefreshRedPoint();
            DebugUtil.LogFormat(ELogType.eQa, string.Format("OnCrossDay  ==>hasQa:{0}", Sys_Qa.Instance.HasQa()));
        }

        private void OnRefreshRedPoint()
        {
            //qaButton.gameObject.SetActive(Sys_Qa.Instance.HasQa() && SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.QuestionSurvey));
            //m_redPoint.SetActive(Sys_Qa.Instance.HasQa() && SDKManager.GetThirdSdkStatus(SDKManager.EThirdSdkType.QuestionSurvey));
            RefreshOPActivityRedPoint();
            RefreshFirstChargeState();
            RefreshLottoRedPoint();
        }

        public void OnClickOpen()
        {
            //animator01.enabled = true;
            if (menu.activeInHierarchy)
            {
                //animator01.Play("Close", -1, 0);
                time?.Cancel();
                time = Timer.Register(0.11f, () =>
                {
                    menu.SetActive(false);
                    SetOpenIconRoll();
                });

            }
            else
            {
                menu.SetActive(true);
                SetOpenIconRoll();
                //animator01.Play("Open", -1, 0);
            }
        }

        private void OnQaButtonClick()
        {
            Sys_Qa.Instance.CheckConditionVaild();
            //m_redPoint.SetActive(false);
        }

        public void SetOpenIconRoll()
        {
            if (menu.activeInHierarchy)
                m_Open_Icon.transform.localEulerAngles = new Vector3(0, 0, 180);
            else
                m_Open_Icon.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        public void OnClickOperationalActivity()
        {
            UIManager.OpenUI(EUIID.UI_OperationalActivity);
            
        }
        public void OnClickFirstCharge()
        {
            Sys_OperationalActivity.Instance.ReportFirstChargeClickEventHitPoint("Open");
            Sys_OperationalActivity.Instance.OpenFirstCharge();
        }
        public void OnClickPromotionCharge()
        {
            UIManager.OpenUI(EUIID.UI_BravePromotion);
            
        }

        public void onClickLotto()
        {
            UIManager.OpenUI(EUIID.UI_Lotto);
        }
        public void OnClickSevenDaysSign()
        {
            UIManager.OpenUI(EUIID.UI_OperationalActivity);
        }

        public void OnClickSevenDaysTarget()
        {
            UIManager.OpenUI(EUIID.UI_SevenDaysTarget);
        }
        public void OnClickBtnTimelimitGift()
        {
            UIManager.HitButton(EUIID.UI_TimelimitGift, "OpenClick");
            UIManager.OpenUI(EUIID.UI_TimelimitGift);
        }
        public void OnClickRedEnvelopeRain()
        {
            UIManager.OpenUI(EUIID.UI_RedEnvelopeRain);
        }

        public void OnClickActivityTopic()
        {
            if (TopicActivityUid!=0)
            {
                UIManager.OpenUI((EUIID)TopicActivityUid,false,Sys_ActivityTopic.Instance.menuId);
            }
            else
            {
                Debug.LogError("主活动ui表中无对应id");
            }

        }
        private void OnBtnBackActivityClick()
        {
            UIManager.OpenUI(EUIID.UI_BackActivity);
        }
        private void OnBtnActivity2048Click()
        {
            UIManager.OpenUI(EUIID.UI_Activity2048);
        }
        private void OnClickPKMatch()
        {
            UIManager.OpenUI(EUIID.UI_PKCompetition);
        }
        private void OnClickPKWeb()
        {
            Sys_PKCompetition.Instance.OpenPKWeb();
        }

        private void RefreshPKWebBtn()
        {
            bool isShow = Sys_PKCompetition.Instance.CheckWatchPKIsOpen();
            BtnPKWeb.gameObject.SetActive(isShow);

            pkWebTimer?.Cancel();
            if (isShow)
            {
                var pkWebCD = Sys_PKCompetition.Instance.GetCurRemainTime(true);
                pkWebTimer = Timer.Register(pkWebCD, RefreshPKWebBtn);
            }
        }

        private void RefreshPKMatchBtn()
        {
            bool isShow = Sys_PKCompetition.Instance.CheckIsOpen() && Sys_PKCompetition.Instance.IsGetLevelLimite() != 0;
            BtnPKMatch.gameObject.SetActive(isShow);

            pkMatchTimer?.Cancel();
            if (isShow)
            {
                pkMatchCD = Sys_PKCompetition.Instance.GetCurRemainTime();
                pkMatchTimer = Timer.Register(pkMatchCD, PKMatchTimeOver, OnPKMatchTimerUpdate, false, false);
            }
        }

        private void PKMatchTimeOver()
        {
            RefreshPKMatchBtn();        
            Sys_PKCompetition.Instance.eventEmitter.Trigger(Sys_PKCompetition.EEvents.Event_TimeOver);
        }

        private void OnPKMatchTimerUpdate(float time)
        {
            //float pkMatchCD = Sys_PKCompetition.Instance.GetCurRemainTime();
            if (pkMatchCD >= time && txtpkMatchTime != null)
            {
                txtpkMatchTime.text = LanguageHelper.TimeToString((uint)(pkMatchCD - time), LanguageHelper.TimeFormat.Type_4);
            }
        }

        /// <summary>
        /// 设置福利按钮
        /// </summary>
        private void SetWelfareButton()
        {
            bool isShow = Sys_FunctionOpen.Instance.IsOpen(50902) && Sys_OperationalActivity.Instance.IsShowActivity();
            m_OperationalActivity.gameObject.SetActive(isShow);
        }
        /// <summary>
        /// 设置七日登入按钮
        /// </summary>
        private void SetSignSevenDayButton()
        {
            Sys_OperationalActivity.SevenDaySignState sevenDaySignState = Sys_OperationalActivity.Instance.GetSevenDaySignData();
            m_SevenDaysSign.gameObject.SetActive(sevenDaySignState.state > 0);
            uint Id = Sys_OperationalActivity.Instance.GetNextSignID();
            CSVSigninReward.Data cSVSigninRewardData = CSVSigninReward.Instance.GetConfData(Id);
            if (null != cSVSigninRewardData)
            {
                List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVSigninRewardData.itemId);
                if (list_drop.Count > 0)
                {
                    ItemIdCount itemIdCount = list_drop[0];
                    ImageHelper.SetIcon(image_SevenDaysSignItem, itemIdCount.CSV.icon_id);
                    text_SevenDaysSignName.text = LanguageHelper.GetTextContent(itemIdCount.CSV.name_id);
                }
            }
            timer_SevenDaysSign?.Cancel();
            if (sevenDaySignState.nextTime > 0)
            {
                text_SevenDaysSignTime.gameObject.SetActive(true);
                go_SevenDaysSignState.SetActive(false);
                go_SevenDaysSignEffect.SetActive(false);
                Action updateTimeAction = () =>
                {
                    var state = Sys_OperationalActivity.Instance.GetSevenDaySignData();
                    if (state.nextTime > 0)
                    {
                        text_SevenDaysSignTime.text = LanguageHelper.TimeToString((uint)state.nextTime, LanguageHelper.TimeFormat.Type_1);
                    }
                    else
                    {
                        OnUpdateSignSevenDayData();
                        updateTimeAction = null;
                    }
                };
                updateTimeAction?.Invoke();
                timer_SevenDaysSign = Timer.Register(1f, () =>
                {
                    updateTimeAction?.Invoke();
                }, null, true);
            }
            else if (sevenDaySignState.nextTime == 0)
            {
                text_SevenDaysSignTime.gameObject.SetActive(false);
                go_SevenDaysSignState.SetActive(true);
                go_SevenDaysSignEffect.SetActive(true);
            }
            else
            {
                text_SevenDaysSignTime.gameObject.SetActive(false);
                go_SevenDaysSignState.SetActive(false);
                go_SevenDaysSignEffect.SetActive(false);
            }
        }
        /// <summary> 打开家族兽训练界面 </summary>
        private void OnFamilyTrainBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_FamilyCreatures_Train);
        }
        /// <summary>
        /// 功能开启
        /// </summary>
        /// <param name="functionOpenData"></param>
        public void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            uint id = functionOpenData.id;
            SetWelfareButton();
            RefreshSevenDaysTargetState();
            RefreshActivity2048State();
            //首充51301
            if (id == 51301)
            {
                //UIManager.OpenUI(EUIID.UI_FirstCharge);
            }
            if (id == 52401)
            {
                CheckActivityTopic();
            }
        }
        /// <summary>
        /// 设置福利相关按钮
        /// </summary>
        public void SetOperationalActivity()
        {
            SetWelfareButton();
            SetSignSevenDayButton();
        }
        /// <summary>
        /// 七日登入更新
        /// </summary>
        public void OnUpdateSignSevenDayData()
        {
            SetOperationalActivity();
        }
        public void OnReceiveSignReward(uint id)
        {
            OnUpdateSignSevenDayData();
        }

        private void OnUpdateLevelGiftData()
        {
            RefreshOPActivityRedPoint();
        }

        private void OnUpdateGrowthFundData()
        {
            RefreshOPActivityRedPoint();
        }
        private void OnUpdateTotalChargeData()
        {
            RefreshOPActivityRedPoint();
        }
        private void OnUpdateSpecialCardData()
        {
            RefreshOPActivityRedPoint();
        }
        private void OnUpdateLotteryActivityData()
        {
            RefreshOPActivityRedPoint();
        }
        private void OnUpdateRankActivityData()
        {
            RefreshOPActivityRedPoint();
        }

        private void OnUpdateQaData()
        {
            RefreshOPActivityRedPoint();
        }

        private void OnUpdateSevenDaysTargetData()
        {
            RefreshSevenDaysTargetState();
        }
        private void OnUpdateTimelimitGiftData()
        {
            RefreshTimelimitGiftState();
        }
        private void UpdateActivityTotalCharge()
        {
            RefreshOPActivityRedPoint();
        }
        private void UpdateActivityTotalConsume()
        {
            RefreshOPActivityRedPoint();
        }
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            //后台开关控制按钮显示隐藏刷新
            //福利按钮、红点
            SetWelfareButton();
            RefreshOPActivityRedPoint();
            //七日目标
            RefreshSevenDaysTargetState();
            //限时礼包(条件礼包)
            RefreshTimelimitGiftState();
            //首充
            RefreshFirstChargeState();
            //七日签到
            SetSignSevenDayButton();
            //红包雨
            OnRefreshRedEnvelopRainState();

            RefreshLottoRedPoint();
            //回归活动按钮
            RefreshBackActivityState();
            RefreshActivity2048State();
        }
        private void RefreshOPActivityRedPoint()
        {
            m_OPActivtyRedPoint.SetActive(Sys_OperationalActivity.Instance.CheckOperationalActivityRedPoint());
        }
        private void RefreshFirstChargeState()
        {
            btnFirstCharge.gameObject.SetActive(Sys_OperationalActivity.Instance.CheckFirstChargeIsShow());
            goFirstChargeRedPoint.SetActive(Sys_OperationalActivity.Instance.CheckFirstChargeRedPoint());
        }

        private void RefreshLottoRedPoint()
        {
            btn_Lotto.gameObject.SetActive(Sys_OperationalActivity.Instance.CheckLotteryActivityIsOpen());
            goLottoRedPoint.SetActive(Sys_pub.Instance.CheckLottoRedPoint());
        }

        private void OnUpdateDailySignAwardCount()
        {
            RefreshOPActivityRedPoint();
        }
        /// <summary> 刷新七日目标按钮显示及红点 </summary>
        private void RefreshSevenDaysTargetState()
        {
            bool isShow = Sys_OperationalActivity.Instance.CheckSevenDaysTargetIsOpen();
            btnSevendaysTarget.gameObject.SetActive(isShow);
            goSevendaysTargetRedPoint.SetActive(isShow && Sys_OperationalActivity.Instance.CheckSevenDaysTargetAllRedPoint());
        }
        /// <summary> 刷新限时礼包(水蓝鼠宝藏)按钮以及倒计时 </summary>
        private void RefreshTimelimitGiftState()
        {
            bool isShow = Sys_OperationalActivity.Instance.CheckTimelimitMainBtnIsShow();
            btnTimelimitGift.gameObject.SetActive(isShow);
            limitGiftTimer?.Cancel();
            if (isShow)
            {
                var minGift = Sys_OperationalActivity.Instance.GetTimelimitMinCDValidGift();
                if (minGift != null)
                {
                    limitGiftCD = Sys_OperationalActivity.Instance.GetTimelimitGiftCountdown(minGift.Id);
                    limitGiftTimer = Timer.Register(limitGiftCD, OnLimitGiftTimerComplete, OnLimitGiftTimerUpdate, false, false);
                }
                txtLimitGiftNum.text = Sys_OperationalActivity.Instance.GetTimelimitGiftsNum().ToString();
            }
        }
        /// <summary> 经验找回提示 </summary>
        private void CheckExpRetrieveFaceTips()
        {
            Sys_OperationalActivity.Instance.CheckShowFaceTip();
        }
        /// <summary> 公测献礼提示 </summary>
        //private void CheckTestGiftFaceTips()
        //{
        //    if (Sys_OperationalActivity.Instance.isTestGiftOpen&& Sys_OperationalActivity.Instance.CheckTestGiftFaceTip())
        //    {
        //        UIManager.OpenUI(EUIID.UI_Face_GoddessGift);
        //    }

        //}
        /// <summary> 刷新限时回归活动状态 </summary>
        private void RefreshBackActivityState()
        {
            bool isShow = Sys_BackActivity.Instance.CheckBackActivityIsOpen();
            BtnBackActivity.gameObject.SetActive(isShow);
            goBackActivityRedPoint.SetActive(Sys_BackActivity.Instance.CheckBackActivityAllRedPoint());
            backActivityTimer?.Cancel();
            if (isShow)
            {
                Sys_BackActivity.Instance.CheckToPopupFace();
                backActivityCD = Sys_BackActivity.Instance.GetBackActivityBtnCD();
                backActivityTimer = Timer.Register(backActivityCD, OnBackActivityTimerComplete, OnBackActivityTimerUpdate, false, false);
            }
        }
        /// <summary> 刷新活动2048按钮状态 </summary>
        private void RefreshActivity2048State()
        {
            bool isShow = Sys_Activity_2048.Instance.CheckActivity2048IsOpen();
            BtnActivity2048.gameObject.SetActive(isShow);
            goActivity2048RedPoint.SetActive(Sys_Activity_2048.Instance.CheckAllRedPoint());
            activity2048Timer?.Cancel();
            if (isShow)
            {
                activity2048CD = Sys_Activity_2048.Instance.GetActivity2048BtnCD();
                activity2048Timer = Timer.Register(activity2048CD, OnActivity2048TimerComplete, OnActivity2048TimerUpdate, false, false);
            }
        }
        /// <summary> 通用活动按钮 </summary>
        private void CheckActivityTopic()
        {
            bool _isOpen = Sys_ActivityTopic.Instance.CheckLimitedActivityOpen();
            bool _isShow = Sys_ActivityTopic.Instance.CheckLimitedActivityShow();
            TopicActivityButton.gameObject.SetActive(_isShow);
            
            if (!_isOpen)
            {//活动没开
                return;
            }
            if (Sys_ActivityTopic.Instance.signRefreshTime==0)
            {
                Sys_ActivityTopic.Instance.OnLimitedActivityDataReq();
            }
            ActivityTopicMenuShow();
            if (_isShow)
            {//活动开了且显示
                limitTimeActivityRedPoint.gameObject.SetActive(Sys_ActivityTopic.Instance.CheckLimitedActivityRedPoint());
                DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                DateTime _end = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivityTopic.Instance.nowMenuTime[1]));
                TimeSpan sp = _end.Subtract(nowtime);
                TopicActivityTimer?.Cancel();
                TopicActivityTimer = Timer.Register((float)sp.TotalSeconds, CheckActivityTopic);
                if (Sys_ActivityTopic.Instance.CheckCommonActivityFaceTips())
                {
                    UIManager.OpenUI(EUIID.UI_Activity_Topic);
                    Sys_ActivityTopic.Instance.SetCommonActivityFaceTip();
                }
            }
            else
            {//活动开了未到显示时间
                
                DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                DateTime _start = TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(Sys_ActivityTopic.Instance.nowMenuTime[0]));
                TimeSpan sp = _start.Subtract(nowtime);
                TopicActivityTimer?.Cancel();
                TopicActivityTimer = Timer.Register((float)sp.TotalSeconds, CheckActivityTopic);
            }
        }

        private void ActivityTopicMenuShow()
        {
            var csvData = CSVActivityMainnUi.Instance.GetAll();
            for (int i = 0; i < csvData.Count; i++)
            {
                var _singleData = csvData[i];
                TopicActivityUid = Sys_ActivityTopic.Instance.menuId;
                int index = _singleData.Activity_Id.IndexOf(TopicActivityUid);
                if (_singleData != null&&index>=0)
                {
                    TopicActivityUid = _singleData.UiId;
                    TopicActivityButton.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(_singleData.IconTitle[index]);
                    ImageHelper.SetIcon(TopicActivityButton.transform.Find("Image_Icon1").GetComponent<Image>(), _singleData.IntIcon[index]);
                    return;
                }
            }
        }
        private void OnLimitGiftTimerComplete()
        {
            RefreshTimelimitGiftState();
        }
        private void OnLimitGiftTimerUpdate(float time)
        {
            if (limitGiftCD >= time && txtLimitGiftTime != null)
            {
                txtLimitGiftTime.text = LanguageHelper.TimeToString((uint)(limitGiftCD - time), LanguageHelper.TimeFormat.Type_1);
            }
        }
        private void OnBackActivityTimerComplete()
        {
            RefreshBackActivityState();
        }
        private void OnBackActivityTimerUpdate(float time)
        {
            if (backActivityCD >= time && txtBackActivityTime != null)
            {
                txtBackActivityTime.text = LanguageHelper.TimeToString((uint)(backActivityCD - time), LanguageHelper.TimeFormat.Type_4);
            }
        }
        private void OnActivity2048TimerComplete()
        {
            RefreshActivity2048State();
        }
        private void OnActivity2048TimerUpdate(float time)
        {
            if (activity2048CD >= time && txtActivity2048Time != null)
            {
                txtActivity2048Time.text = LanguageHelper.TimeToString((uint)(activity2048CD - time), LanguageHelper.TimeFormat.Type_4);
            }
        }
        private void OnBeginEnter(uint stackID, int nID)
        {
            if (nID == (int)EUIID.UI_Chat)
            {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                gameObject.SetActive(AspectRotioController.IsExpandState);
#else
                gameObject.SetActive(false);
#endif
            }
        }

        private void OnUIEndExit(uint stackID, int nID)
        {
            if (nID == (int)EUIID.UI_Chat)
            {
                gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 刷新红包雨按钮以及倒计时
        /// </summary>
        private void OnRefreshRedEnvelopRainState()
        {
            bool isShow = Sys_RedEnvelopeRain.Instance.ActivityIsOpen();
            bool isDefault = Sys_RedEnvelopeRain.Instance.CheckIsDefaultRedEnvelopeRain();
            btnRedEnvelopeRain.gameObject.SetActive(isShow && isDefault);
            //rainTimer?.Cancel();
            //if (Sys_RedEnvelopeRain.Instance.CheckCurRainIsUnderway())
            //{
            //    Fx_RainObj.SetActive(false);
            //    redEnvelopeRainAnimator.Play("Empty", -1, 0);
            //    redEnvelopeRainAnimator.speed = 0;
            //    textRainTime.gameObject.SetActive(true);
            //    rainTimeCD = Sys_RedEnvelopeRain.Instance.GetResidueSecond(false);
            //    rainTimer = Timer.Register(rainTimeCD, OnRedEnvelopRainComplete, OnRedEnvelopRainUpdate, false, false);
            //}
            //else
            //{
            //    textRainTime.gameObject.SetActive(false);
            //}
        }
        //private void OnRedEnvelopRainComplete()
        //{
        //    OnRefreshRedEnvelopRainState();
        //}
        //private void OnRedEnvelopRainUpdate(float time)
        //{
        //    textRainTime.color = Color.white;
        //    textRainTime.text = LanguageHelper.TimeToString((uint)(rainTimeCD - time), LanguageHelper.TimeFormat.Type_1);
        //}

        private void OnRefreshRainStartBeforeHint()
        {
            if (Sys_RedEnvelopeRain.Instance.ActivityIsOpen() && Sys_RedEnvelopeRain.Instance.CheckIsDefaultRedEnvelopeRain())
            {
                rainHintTimeCD = Sys_RedEnvelopeRain.Instance.GetRainStartBeforHintDiffTime();
                bool isShow = Sys_RedEnvelopeRain.Instance.CheckRainStartBeforHint();
                if (Fx_RainObj != null && redEnvelopeRainAnimator != null)
                {
                    if (isShow)
                    {
                        Fx_RainObj.SetActive(true);
                        redEnvelopeRainAnimator.speed = 1;
                        redEnvelopeRainAnimator.Play("Open", -1, 0);
                    }
                    else
                    {
                        Fx_RainObj.SetActive(false);
                        redEnvelopeRainAnimator.Play("Empty", -1, 0);
                        redEnvelopeRainAnimator.speed = 0;
                    }
                }
                rainHintTimer?.Cancel();
                if (textRainTime != null)
                {
                    if (rainHintTimeCD != -1)
                    {
                        textRainTime.gameObject.SetActive(true);
                        rainHintTimer = Timer.Register(rainHintTimeCD, OnRedEnvelopRainBeforeHintComplete, OnRedEnvelopRainBeforeHintUpdate, false, false);
                    }
                    else
                    {
                        textRainTime.gameObject.SetActive(false);
                        textRainTime.text = "";
                    }
                }
            }
        }
        private void OnRedEnvelopRainBeforeHintComplete()
        {
            OnRefreshRainStartBeforeHint();
        }
        private void OnRedEnvelopRainBeforeHintUpdate(float time)
        {
            rainHintTimeCD = Sys_RedEnvelopeRain.Instance.GetRainStartBeforHintDiffTime();
            if (rainHintTimeCD <= 0)
                rainHintTimeCD = 0;
            if (textRainTime != null)
                textRainTime.text = LanguageHelper.TimeToString((uint)(rainHintTimeCD), LanguageHelper.TimeFormat.Type_1);
        }
        private void OnFamilyPetStarOrEndRefresh()
        {
            btnFamilyTrain.gameObject.SetActive(Sys_Family.Instance.IsNeedShowMenuTrainBtn());
        }
        private void OnActivityTopicUpdate()
        {
            CheckActivityTopic();
        }
      
    }
}


