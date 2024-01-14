using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_PetDomesticate : UIBase
    {

        private Button btnClose;
        private Text txtRankName;   //段位
        private Slider sliderRank;
        private Text txtRankLevel;  //x段n级
        private Text txtRankValue;  //段位进度
        private Button btnRank;
        private Transform attrParent;//属性
        private Text txtTaskNum;
        private InfinityGrid infinity;
        private Button btnRefresh;
        private Image imgRefreshCost;
        private Text txtRefreshCost;
        private Button btnAddTask;
        private Image imgAddTaskCost;
        private Text txtAddTaskCost;
        private Text txtAddTaskImpose;  //加训限购

        private UI_CurrencyTitle currency;

        /// <summary>
        /// 属性列表
        /// </summary>
        private List<UI_PetDomesticateAttrCell> listAttr = new List<UI_PetDomesticateAttrCell>();

        private List<UI_PetDomesticateTaskCell> listTaskCell = new List<UI_PetDomesticateTaskCell>();

        private List<DomesticationTask> listTask;

        private bool isCheckToGetReward;

        private Timer btnCDTimer;   //按钮cd 防止多次请求
        private bool isBtnCD;

        private Timer hindTimer;    //最早结算的任务timer 用来领奖刷新界面
        private float hindCountDownTime = 0;

        #region 系统函数
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Sys_PetDomesticate.Instance.ReqGetPetDomesticationInfo();
            Parse();
        }
        protected override void OnShow()
        {
            isBtnCD = false;
            currency?.InitUi();
            UpdateView();
        }
        protected override void OnHide()
        {
            btnCDTimer?.Cancel();
            hindTimer?.Cancel();
        }
        protected override void OnDestroy()
        {
            currency?.Dispose();
            btnCDTimer?.Cancel();
            hindTimer?.Cancel();
            for (int i = 0; i < listTaskCell.Count; i++)
            {
                var cell = listTaskCell[i];
                cell?.Destroy();
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            Sys_PetDomesticate.Instance.eventEmitter.Handle(Sys_PetDomesticate.EEvents.OnPetDomesticateDataUpdate, OnPetDomesticateDataUpdate, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            txtRankName = transform.Find("Animator/View_Top/Text_Title").GetComponent<Text>();
            sliderRank = transform.Find("Animator/View_Top/Slider").GetComponent<Slider>();
            txtRankLevel = transform.Find("Animator/View_Top/Text_Level").GetComponent<Text>();
            txtRankValue = transform.Find("Animator/View_Top/Text_Value").GetComponent<Text>();
            btnRank = transform.Find("Animator/View_Top/Btn_Rank").GetComponent<Button>();
            btnRank.onClick.AddListener(OnBtnRankClick);
            attrParent = transform.Find("Animator/View_Left/View_Attr/Grid");
            txtTaskNum = transform.Find("Animator/View_Right/Text_Task/Text_Value").GetComponent<Text>();
            infinity = transform.Find("Animator/View_Right/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
            btnRefresh = transform.Find("Animator/View_Right/Btn_Reset").GetComponent<Button>();
            btnRefresh.onClick.AddListener(OnBtnRefreshClick);
            imgRefreshCost = transform.Find("Animator/View_Right/Consume_Reset/Icon").GetComponent<Image>();
            txtRefreshCost = transform.Find("Animator/View_Right/Consume_Reset/Value").GetComponent<Text>();
            btnAddTask = transform.Find("Animator/View_Right/Btn_Add").GetComponent<Button>();
            btnAddTask.onClick.AddListener(OnBtnAddTaskClick);
            imgAddTaskCost = transform.Find("Animator/View_Right/Consume_Add/Icon").GetComponent<Image>();
            txtAddTaskCost = transform.Find("Animator/View_Right/Consume_Add/Value").GetComponent<Text>();
            txtAddTaskImpose = transform.Find("Animator/View_Right/Text_Restrict").GetComponent<Text>();
        
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
        }
        private void UpdateView()
        {
            uint stage = Sys_PetDomesticate.Instance.Stage;
            uint level = Sys_PetDomesticate.Instance.Level;
            uint exp = Sys_PetDomesticate.Instance.Exp;
            uint rankId = stage * 1000 + level;
            CSVDomesticationRank.Data csvRank = CSVDomesticationRank.Instance.GetConfData(rankId);
            if (csvRank != null)
            {
                txtRankName.text = LanguageHelper.GetTextContent(csvRank.name);
                
                listTask = Sys_PetDomesticate.Instance.GetTaskList();
                infinity.CellCount = listTask.Count;
                infinity.ForceRefreshActiveCell();
                txtTaskNum.text = listTask.Count.ToString();
                uint refreshCostIcon = Sys_PetDomesticate.Instance.GetRefreshCostIconId();
                ImageHelper.SetIcon(imgRefreshCost, refreshCostIcon);
                txtRefreshCost.text = Sys_PetDomesticate.Instance.GetRefreshCostText();
                uint addTaskCostIcon = Sys_PetDomesticate.Instance.GetAddTaskCostIconId();
                ImageHelper.SetIcon(imgAddTaskCost, addTaskCostIcon);
                txtAddTaskCost.text = Sys_PetDomesticate.Instance.GetAddTaskCostText();
                txtAddTaskImpose.text = Sys_PetDomesticate.Instance.GetAddTaskTimesText();

                uint nextId = level == 99 ? (stage + 1) * 1000 : csvRank.id + 1;
                CSVDomesticationRank.Data nextData = CSVDomesticationRank.Instance.GetConfData(nextId);
                bool isMax = nextData == null;
                if(isMax)
                {
                    txtRankLevel.text = LanguageHelper.GetTextContent(2052037, LanguageHelper.GetTextContent(2052003, stage.ToString(), level.ToString()));//{0}段{1}级
                    sliderRank.value = 1;
                    txtRankValue.text = LanguageHelper.GetTextContent(2052048, exp.ToString());//{0}/--
                }
                else
                {
                    txtRankLevel.text = LanguageHelper.GetTextContent(2052003, stage.ToString(), level.ToString());//{0}段{1}级
                    sliderRank.value = (float)exp / csvRank.lvup_cost;
                    txtRankValue.text = LanguageHelper.GetTextContent(2052004, exp.ToString(), csvRank.lvup_cost.ToString());//{0}/{1}

                }
                int count = csvRank.lv_attr.Count;
                FrameworkTool.CreateChildList(attrParent, count);
                if (count > listAttr.Count)
                {
                    int startCount = listAttr.Count;
                    for (int i = startCount; i < count; i++)
                    {
                        var curChild = attrParent.GetChild(i);
                        UI_PetDomesticateAttrCell cell = new UI_PetDomesticateAttrCell();
                        cell.Init(curChild);
                        listAttr.Add(cell);
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    var curCell = listAttr[i];
                    uint attrId = csvRank.lv_attr[i][0];
                    uint arg0 = csvRank.lv_attr[i][1];
                    uint arg1 = isMax ? arg0 : nextData.lv_attr[i][1];
                    curCell.UpdateCellView(attrId, arg0, arg1);
                }
            }
            UpdateHindTimer();
        }

        private void UpdateHindTimer()
        {
            hindTimer?.Cancel();
            hindCountDownTime = Sys_PetDomesticate.Instance.GetNextWillFinishTaskCD();
            if (hindCountDownTime > 0)
            {
                hindTimer = Timer.Register(hindCountDownTime, OnHindTimerComplete, null, false, false);
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnHindTimerComplete()
        {
            hindTimer?.Cancel();
            //自动领奖
            Sys_PetDomesticate.Instance.CheckToGetReward();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetDomesticateTaskCell mCell = new UI_PetDomesticateTaskCell();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            listTaskCell.Add(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PetDomesticateTaskCell mCell = cell.mUserData as UI_PetDomesticateTaskCell;
            mCell.UpdateCellView(listTask[index]);
        }
        private void OnBtnRankClick()
        {
            if (Sys_FunctionOpen.Instance.IsOpen(50901, false))
            {
                OpenUIRankParam param = new OpenUIRankParam();
                param.initType = 6;
                param.initSubType = 602;
                UIManager.OpenUI(EUIID.UI_Rank, false, param);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6104));
            }
        }
        private void OnBtnRefreshClick()
        {
            //防止连点
            if (isBtnCD)
                return;
            isBtnCD = true;
            btnCDTimer?.Cancel();
            btnCDTimer = null;
            btnCDTimer = Timer.Register(0.3f, () =>
            {
                isBtnCD = false;
            });

            Sys_PetDomesticate.Instance.CheckToRefreshTask();
        }
        private void OnBtnAddTaskClick()
        {
            //防止连点
            if (isBtnCD)
                return;
            isBtnCD = true;
            btnCDTimer?.Cancel();
            btnCDTimer = null;
            btnCDTimer = Timer.Register(0.3f, () =>
            {
                isBtnCD = false;
            });

            Sys_PetDomesticate.Instance.CheckToAddTask();
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        private void OnPetDomesticateDataUpdate()
        {
            if(!isCheckToGetReward)
            {
                Sys_PetDomesticate.Instance.CheckToGetReward();
                isCheckToGetReward = true;
            }
            UpdateView();
        }
        private void OnCurrencyChanged(uint id, long value)
        {
            //货币更新 直接重新拉一次数据
            Sys_PetDomesticate.Instance.ReqGetPetDomesticationInfo();
            //txtRefreshCost.text = Sys_PetDomesticate.Instance.GetRefreshCostText();
            //txtAddTaskCost.text = Sys_PetDomesticate.Instance.GetAddTaskCostText();
        }
        #endregion

        #region class
        public class UI_PetDomesticateTaskCell
        {
            private Transform transform;
            private GameObject goBgS;
            private GameObject goBgA;
            private GameObject goBgB;
            private Text txtTaskName;
            private Text txtUseTime;    //驯养时长
            private GameObject goUsable;    //可用
            private GameObject goUnderway;  //进行中
            private Text txtCondition;  //驯养条件
            private Image imgRaceIcon;
            private Image imgSkillIcon;
            private Image imgPetIcon;
            private Text txtAllAdd;       //总加成
            private GameObject goAddUpArrow;    //加成上箭头
            private GameObject goAddDownArrow;  //加成下箭头
            //private Image imgRewardIcon;
            private Text txtRewardDecs; //奖励描述
            private Text txtRewardValue;
            private GameObject goRewardUpArrow;     //奖励上箭头
            private GameObject goRewardDownArrow;   //奖励下箭头
            private Button btnGo;

            private DomesticationTask taskData;
            private Timer timer;
            private float countDownTime = 0;
            public void Init(Transform _trans)
            {
                transform = _trans;
                goBgS = transform.Find("Imagebg_S").gameObject;
                goBgA = transform.Find("Imagebg_A").gameObject;
                goBgB = transform.Find("Imagebg_B").gameObject;
                txtTaskName = transform.Find("Text_Name").GetComponent<Text>();
                txtUseTime = transform.Find("Text_Time").GetComponent<Text>();
                goUsable = transform.Find("Not").gameObject;
                goUnderway = transform.Find("Begin").gameObject;
                txtCondition = transform.Find("Not/Text_Condition").GetComponent<Text>();
                imgRaceIcon = transform.Find("Not/Race/Image_Icon").GetComponent<Image>();
                imgSkillIcon = transform.Find("Not/Skill/Image_Icon").GetComponent<Image>();
                imgPetIcon = transform.Find("Begin/Image_head/Image_bg/Image_Icon").GetComponent<Image>();
                txtAllAdd = transform.Find("Begin/Text_Addition/Text_Value").GetComponent<Text>();
                //imgRewardIcon = transform.Find("grid/Award/Image_Icon").GetComponent<Image>();
                txtRewardValue = transform.Find("grid/Award/Text_Value").GetComponent<Text>();
                txtRewardDecs = transform.Find("grid/Award/Text_Award").GetComponent<Text>();
                btnGo = transform.Find("grid/Btn_01").GetComponent<Button>();
                btnGo.onClick.AddListener(OnBtnGoClick);
                goAddUpArrow = transform.Find("Begin/Text_Addition/Text_Value/Image_ArrowUp").gameObject;
                goAddDownArrow = transform.Find("Begin/Text_Addition/Text_Value/Image_ArrowDown").gameObject;
                goRewardUpArrow = transform.Find("grid/Award/Text_Value/Image_ArrowUp").gameObject;
                goRewardDownArrow = transform.Find("grid/Award/Text_Value/Image_ArrowDown").gameObject;
            }

            public void Destroy()
            {
                timer?.Cancel();
            }
            public void UpdateCellView(DomesticationTask _taskData)
            {
                timer?.Cancel();
                taskData = _taskData;
                bool isUsable = taskData.PetUid == 0;//是否可开始
                goUsable.SetActive(isUsable);
                goUnderway.SetActive(!isUsable);
                btnGo.gameObject.SetActive(isUsable);
                goBgS.SetActive(taskData.Grade == 1);
                goBgA.SetActive(taskData.Grade == 2);
                goBgB.SetActive(taskData.Grade == 3);
                var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
                if(csvTask!=null)
                {
                    txtTaskName.text = LanguageHelper.GetTextContent((uint)csvTask.name);
                    int gradeindex = (int)taskData.Grade - 1;
                    if (isUsable)
                    {
                        txtRewardDecs.text = LanguageHelper.GetTextContent(2052026);//基础奖励
                        goAddUpArrow.SetActive(false);
                        goAddDownArrow.SetActive(false);
                        goRewardUpArrow.SetActive(false);
                        goRewardDownArrow.SetActive(false);
                        txtUseTime.text = Sys_PetDomesticate.Instance.GetTaskAllTimeText(csvTask.duration[gradeindex]);
                        txtCondition.text = Sys_PetDomesticate.Instance.GetConditionText(csvTask.type, csvTask.condition[gradeindex]);
                        CSVGenus.Data csvGenus = CSVGenus.Instance.GetConfData((uint)csvTask.race);
                        ImageHelper.SetIcon(imgRaceIcon, csvGenus.rale_icon);
                        CSVPassiveSkillInfo.Data csvSkill = CSVPassiveSkillInfo.Instance.GetConfData(taskData.LuckySkill);
                        ImageHelper.SetIcon(imgSkillIcon, csvSkill.icon);
                        uint worldStyleId = 169u;
                        TextHelper.SetText(txtRewardValue, csvTask.reward[gradeindex].ToString(), CSVWordStyle.Instance.GetConfData(worldStyleId));
                    }
                    else
                    {
                        txtRewardDecs.text = LanguageHelper.GetTextContent(2052027);//最终奖励
                        StartTimer(taskData.EndTime);
                        CSVPetNew.Data csvPet = CSVPetNew.Instance.GetConfData(taskData.PetInfoId);
                        if (csvPet != null)
                        {
                            ImageHelper.SetIcon(imgPetIcon, csvPet.icon_id);
                        }
                        uint allAddition = taskData.AwardRatio;
                        uint worldStyleId = allAddition > 100 ? 170u : allAddition == 100 ? 169u : 171u;
                        TextHelper.SetText(txtAllAdd, LanguageHelper.GetTextContent(10882, allAddition.ToString()), CSVWordStyle.Instance.GetConfData(worldStyleId));
                        TextHelper.SetText(txtRewardValue, Mathf.Floor(csvTask.reward[gradeindex] * allAddition / 100).ToString(), CSVWordStyle.Instance.GetConfData(worldStyleId));
                        goAddUpArrow.SetActive(allAddition > 100);
                        goAddDownArrow.SetActive(allAddition < 100);
                        goRewardUpArrow.SetActive(allAddition > 100);
                        goRewardDownArrow.SetActive(allAddition < 100);
                    }
                }
            }

            private void OnBtnGoClick()
            {
                PetDomesticateTaskParam param = new PetDomesticateTaskParam()
                {
                    taskData = taskData
                };
                UIManager.OpenUI(EUIID.UI_PetDomesticateTask, false, param);
            }

            private void StartTimer(uint endTime)
            {
                timer?.Cancel();
                var nowTime = Sys_Time.Instance.GetServerTime();
                countDownTime = endTime - nowTime;
                if (countDownTime > 0)
                {
                    timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
                }
            }
            private void OnTimerComplete()
            {
                timer?.Cancel();
            }
            private void OnTimerUpdate(float time)
            {
                if (countDownTime >= time && txtUseTime != null)
                {
                    txtUseTime.text = LanguageHelper.GetTextContent(2052044, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_1));//剩余时长：{0}
                }
            }
        }

        public class UI_PetDomesticateAttrCell
        {
            private Transform transform;
            private Text txtName;
            private Text txtArg0;
            private Text txtArg1;

            public void Init(Transform _trans)
            {
                transform = _trans;
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtArg0 = transform.Find("Text1").GetComponent<Text>();
                txtArg1 = transform.Find("Text2").GetComponent<Text>();
            }

            public void UpdateCellView(uint AttrId, uint arg0, uint arg1)
            {
                CSVAttr.Data csvAttrData = CSVAttr.Instance.GetConfData(AttrId);
                if (csvAttrData != null)
                {
                    if (AttrId == 90 || AttrId == 91)
                    {
                        txtName.text = LanguageHelper.GetTextContent(csvAttrData.name);
                    }
                    else
                    {
                        txtName.text = LanguageHelper.GetTextContent(2052047, LanguageHelper.GetTextContent(csvAttrData.name));
                    }
                    txtArg0.text = Sys_Attr.Instance.GetAttrValue(csvAttrData, arg0);
                    uint worldStyleId = arg1 > arg0 ? 170u : 169u;
                    TextHelper.SetText(txtArg1, Sys_Attr.Instance.GetAttrValue(csvAttrData, arg1), CSVWordStyle.Instance.GetConfData(worldStyleId));
                }
            }
        }
        #endregion
    }
}
