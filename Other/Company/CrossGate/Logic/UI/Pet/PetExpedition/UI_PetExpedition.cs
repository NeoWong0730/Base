using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Lib.Core;
using Table;

namespace Logic
{
    public enum EPetExpeditionPageType
    {
        /// <summary>可用</summary>
        Usable = 1,
        /// <summary>进行中</summary>
        Underway = 2,
    }
    public class UI_PetExpedition : UIBase
    {
        private Button btnClose;
        private Text txtActivityTime;
        private Slider slider;
        private Text txtScroeNum;//冒险点数量
        private Transform rewardParent;
        private GameObject goTaskEmpty;//任务列表为空的提示
        private Text txtTaskEmpty;//任务列表为空的提示文本

        private EPetExpeditionPageType pageType = EPetExpeditionPageType.Usable;
        private Dictionary<EPetExpeditionPageType, Toggle> dictToggles = new Dictionary<EPetExpeditionPageType, Toggle>();
        private bool isInitReward;
        private List<UI_PetExpeditionReward> listRewardCell = new List<UI_PetExpeditionReward>();
        private InfinityGrid infinity;
        private List<uint> ListIds;
        private List<UI_PetExpeditonTask> listTaskCell = new List<UI_PetExpeditonTask>();

        #region 系统函数
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnLoaded()
        {
            Sys_PetExpediton.Instance.ReqPetExploreInfo();
            Parse();
        }
        protected override void OnShow()
        {
            UpdatePageType();
        }
        protected override void OnHide()
        {
        }
        protected override void OnDestroy()
        {
            for (int i = 0; i < listTaskCell.Count; i++)
            {
                var cell = listTaskCell[i];
                cell?.Destroy();
            }
            for (int i = 0; i < listRewardCell.Count; i++)
            {
                var cell = listRewardCell[i];
                cell?.Destroy();
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_PetExpediton.Instance.eventEmitter.Handle(Sys_PetExpediton.EEvents.OnPetExpeditonDataUpdate, OnPetExpeditonDataUpdate, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/Image_bg/Button_off").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            Toggle tgUsable = transform.Find("Animator/Menu/Toggle0").GetComponent<Toggle>();
            tgUsable.onValueChanged.AddListener(OnClickUsableToggle);
            dictToggles[EPetExpeditionPageType.Usable] = tgUsable;
            Toggle tgUnderway = transform.Find("Animator/Menu/Toggle1").GetComponent<Toggle>();
            tgUnderway.onValueChanged.AddListener(OnClickUnderwayToggle);
            dictToggles[EPetExpeditionPageType.Underway] = tgUnderway;

            txtActivityTime = transform.Find("Animator/Time").GetComponent<Text>();
            slider = transform.Find("Animator/Point/Slider").GetComponent<Slider>();
            txtScroeNum = transform.Find("Animator/Point/Image/Num").GetComponent<Text>();
            rewardParent = transform.Find("Animator/Point/Grid");
            goTaskEmpty = transform.Find("Animator/Empty").gameObject;
            txtTaskEmpty = transform.Find("Animator/Empty/Text_Tips").GetComponent<Text>();

            infinity = transform.Find("Animator/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
        }
        private void UpdateView()
        {
            InitReward();
            txtScroeNum.text = Sys_PetExpediton.Instance.curScore.ToString();
            slider.value = Sys_PetExpediton.Instance.GetSliderValue();
            txtActivityTime.text = Sys_PetExpediton.Instance.GetActivityDateText();
            for (int i = 0; i < listRewardCell.Count; i++)
            {
                var cell = listRewardCell[i];
                cell.UpdateCellView();
            }
            bool isUnderway = pageType == EPetExpeditionPageType.Underway;
            ListIds = Sys_PetExpediton.Instance.GetTaskIds(isUnderway);
            infinity.CellCount = ListIds.Count;
            infinity.ForceRefreshActiveCell();
            bool isEmpty = ListIds.Count <= 0;
            goTaskEmpty.SetActive(isEmpty);
            if (isEmpty)
            {
                uint languaId = pageType == EPetExpeditionPageType.Underway ? 2025645u : 2025646u;
                txtTaskEmpty.text = LanguageHelper.GetTextContent(languaId);
            }
            SetMenuRedPoint(EPetExpeditionPageType.Underway, Sys_PetExpediton.Instance.CheckTaskFinishRedPoint());
        }
        private void UpdatePageType()
        {
            foreach (var key in dictToggles.Keys)
            {
                bool isActive = key == pageType;
                dictToggles[key].isOn = isActive;
                dictToggles[key].onValueChanged.Invoke(isActive);
            }
        }
        private void InitReward()
        {
            if (!isInitReward)
            {
                uint activityId = Sys_PetExpediton.Instance.curActivityId;
                CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(activityId);
                if (activityData != null)
                {
                    FrameworkTool.CreateChildList(rewardParent, (int)activityData.Date.Count);

                    for (int i = 0; i < rewardParent.childCount; i++)
                    {
                        Transform child = rewardParent.GetChild(i);
                        UI_PetExpeditionReward cell = new UI_PetExpeditionReward();
                        cell.Init(child, i);
                        listRewardCell.Add(cell);
                    }
                    isInitReward = true;
                }
            }
        }
        private void SetMenuRedPoint(EPetExpeditionPageType type,bool isShow)
        {
            Toggle curToggle = dictToggles[type];
            GameObject goRed = curToggle.transform.Find("Image_Dot").gameObject;
            goRed.SetActive(isShow);
        }
        #endregion

        #region event
        private void OnPetExpeditonDataUpdate()
        {
            UpdateView();
        }
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnClickUsableToggle(bool isOn)
        {
            if (isOn)
            {
                pageType = EPetExpeditionPageType.Usable;
                UpdateView();
            }
        }
        private void OnClickUnderwayToggle(bool isOn)
        {
            if (isOn)
            {
                pageType = EPetExpeditionPageType.Underway;
                UpdateView();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetExpeditonTask mCell = new UI_PetExpeditonTask();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            listTaskCell.Add(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PetExpeditonTask mCell = cell.mUserData as UI_PetExpeditonTask;
            mCell.UpdateCellView(ListIds[index], pageType == EPetExpeditionPageType.Underway);
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        #endregion

        #region class
        public class UI_PetExpeditionReward
        {
            private Transform transform;
            private Button btnReward;
            private Image imgReward;
            private GameObject goCanGet;
            private Text txtScore;
            private int cellIndex;

            private bool canGet;
            private bool isGet;

            private Timer btnCDTimer;   //按钮cd 防止多次请求
            private bool isBtnCD;
            public void Init(Transform trans,int index)
            {
                transform = trans;
                cellIndex = index;
                btnReward = transform.Find("Button").GetComponent<Button>();
                btnReward.onClick.AddListener(OnBtnRewardClick);
                imgReward = transform.Find("Button").GetComponent<Image>();
                goCanGet = transform.Find("Button/Image_Open").gameObject;
                txtScore = transform.Find("Text_Num").GetComponent<Text>();
            }

            public void UpdateCellView()
            {
                uint activityId = Sys_PetExpediton.Instance.curActivityId;
                CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(activityId);
                if (activityData != null)
                {
                    uint score = Sys_PetExpediton.Instance.curScore;
                    uint cellScore = activityData.Date[cellIndex];
                    canGet = score >= cellScore;
                    isGet = Sys_PetExpediton.Instance.CheckScoreRewardIsGet(cellIndex);
                    uint ImageId = Sys_PetExpediton.Instance.GetScoreRewardImageIconId(cellIndex, isGet);
                    ImageHelper.SetIcon(imgReward, ImageId);
                    goCanGet.SetActive(canGet && !isGet);
                    txtScore.text = cellScore.ToString();
                }
            }

            private void OnBtnRewardClick()
            {
                if (canGet)
                {
                    if (isGet)
                    {
                        //展示奖励
                        ShowReward();
                    }
                    else
                    {
                        //防止连点
                        if (isBtnCD)
                            return;
                        isBtnCD = true;
                        btnCDTimer?.Cancel();
                        btnCDTimer = null;
                        btnCDTimer = Timer.Register(1f, () =>
                        {
                            isBtnCD = false;
                        });
                        //领取奖励
                        Sys_PetExpediton.Instance.ReqPetExploreGetPointAwardReq((uint)cellIndex);
                    }
                }
                else
                {
                    //展示奖励
                    ShowReward();
                }
            }

            private void ShowReward()
            {
                uint activityId = Sys_PetExpediton.Instance.curActivityId;
                CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(activityId);
                if (activityData != null && activityData.Reward.Count > cellIndex)
                {
                    uint rewardId = activityData.Reward[cellIndex];

                    PetExpeditionRewardParam param = new PetExpeditionRewardParam();
                    param.rewardId = rewardId;
                    param.v3 = imgReward.transform.localToWorldMatrix.MultiplyPoint(imgReward.transform.localPosition);
                    UIManager.OpenUI(EUIID.UI_PetExpedition_RewardList, false, param);
                }
            }

            public  void Destroy()
            {
                btnCDTimer?.Cancel();
            }
        }

        public class UI_PetExpeditonTask
        {
            private uint petTaskId;
            private CSVPetexploreTaskGroup.Data petTaskData;
            private bool isUnderway = false;

            private Transform transform;
            private Button btnSelft;
            private Image imgBgQuality;
            private Text txtBgQuality;
            private Image imgIcon;
            private Text txtTaskTitle;
            private Text txtTaskPoint;
            private GameObject goNormal;
            private Text txtLevel;
            private Text txtAllTime;//任务总时间
            private GameObject goGoing;
            private Text txtRemainingTime;//剩余时间
            private GameObject goFinish;


            private Timer timer;
            private float countDownTime = 0;

            private List<GameObject> listStar = new List<GameObject>();
            private List<PropItem> listItem = new List<PropItem>();
            public void Init(Transform trans)
            {
                transform = trans;
                btnSelft = transform.Find("Button").GetNeedComponent<Button>();
                btnSelft.onClick.AddListener(OnBtnSelfClick);
                imgBgQuality = transform.Find("Image_Quality").GetComponent<Image>();
                txtBgQuality = transform.Find("Image_Quality/Text").GetComponent<Text>();
                imgIcon = transform.Find("Image_PetBottom/Image_Icon").GetComponent<Image>();
                txtTaskTitle = transform.Find("Text_Title").GetComponent<Text>();
                txtTaskPoint = transform.Find("Text_Title/Text").GetComponent<Text>();
                goNormal = transform.Find("State/Normal").gameObject;
                txtLevel = transform.Find("State/Normal/Text_Lv").GetComponent<Text>();
                txtAllTime = transform.Find("State/Normal/Text_Time/Text_Num").GetComponent<Text>();
                goGoing = transform.Find("State/Going").gameObject;
                txtRemainingTime = transform.Find("State/Going/Text_Time").GetComponent<Text>();
                goFinish = transform.Find("State/Finish").gameObject;
                

                var starParent = transform.Find("Image_Quality/Layout_Star");
                listStar.Clear();
                for (int i = 0; i < starParent.childCount; i++)
                {
                    var star = starParent.GetChild(i);
                    listStar.Add(star.gameObject);
                }
                var itemParent = transform.Find("Reward");
                listItem.Clear();
                for (int i = 0; i < itemParent.childCount; i++)
                {
                    var goItem = itemParent.GetChild(i).gameObject;
                    PropItem itemCell = new PropItem();
                    itemCell.BindGameObject(goItem);
                    listItem.Add(itemCell);
                }
            }

            public void UpdateCellView(uint id,bool _isUnderway = false)
            {
                timer?.Cancel();
                petTaskId = id;
                petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(petTaskId / 1000);
                isUnderway = _isUnderway;
                if (petTaskData != null)
                {
                    uint qualityBgId = Sys_PetExpediton.Instance.GetTaskCellBgImageIconId(petTaskData.Difficulty);
                    ImageHelper.SetIcon(imgBgQuality, qualityBgId);
                    txtBgQuality.text = Sys_PetExpediton.Instance.GetTaskBgTitleText(petTaskData.Difficulty);
                    for (int i = 0; i < listStar.Count; i++)
                    {
                        var star = listStar[i];
                        star.SetActive(i < petTaskData.Difficulty);
                    }
                    var rewardItems = CSVDrop.Instance.GetDropItem(petTaskData.Success_Reward);
                    for (int i = 0; i < listItem.Count; i++)
                    {
                        var propItem = listItem[i];
                        if (i < rewardItems.Count)
                        {
                            propItem.transform.gameObject.SetActive(true);
                            var itemData = new PropIconLoader.ShowItemData(rewardItems[i].id, rewardItems[i].count, true, false, false, false, false, true);
                            propItem.SetData(itemData, EUIID.UI_PetExpedition);
                        }
                        else
                        {
                            propItem.transform.gameObject.SetActive(false);
                        }
                    }
                    CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(petTaskData.Chieftain[petTaskData.Chieftain.Count - 1]);
                    ImageHelper.SetIcon(imgIcon, petData.icon_id);
                    txtTaskTitle.text = LanguageHelper.GetTextContent(petTaskData.Title);
                    txtTaskPoint.text = LanguageHelper.GetTextContent(2025628 ,petTaskData.Point.ToString());//（{0}调查点）;
                    if (isUnderway)
                    {
                        goNormal.SetActive(false);
                        var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(petTaskId);
                        if (taskInfo != null)
                        {
                            //进行中
                            uint nowtime = Sys_Time.Instance.GetServerTime();
                            bool isFinish = taskInfo.EndTick < nowtime;
                            if (isFinish)
                            {
                                goGoing.SetActive(false);
                                goFinish.SetActive(true);
                            }
                            else
                            {
                                goFinish.SetActive(false);
                                goGoing.SetActive(true);
                                StartTimer(taskInfo.EndTick - nowtime);
                            }
                        }
                    }
                    else
                    {
                        //可用
                        goGoing.SetActive(false);
                        goFinish.SetActive(false);
                        goNormal.SetActive(true);
                        txtLevel.text = LanguageHelper.GetTextContent(2025606, petTaskData.Level_Condition.ToString());//推荐等级:{0}
                        txtAllTime.text = Sys_PetExpediton.Instance.GetTaskAllTimeText(petTaskData.Times);
                    }
                }
            }
            private void StartTimer(uint cd)
            {
                countDownTime = cd;
                timer?.Cancel();
                timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
            }
            private void OnTimerComplete()
            {
                timer?.Cancel();
                UpdateCellView(petTaskId, isUnderway);
            }
            private void OnTimerUpdate(float time)
            {
                if (countDownTime >= time && txtRemainingTime != null)
                {
                    txtRemainingTime.text = LanguageHelper.GetTextContent(2025631, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4));
                }
            }
            public void Destroy()
            {
                timer?.Cancel();
            }

            private void OnBtnSelfClick()
            {
                UIManager.OpenUI(EUIID.UI_PetExpedition_Task, false, new PetExpeditionTaskParam() { taskId = petTaskId, isUnderway = isUnderway });
            }
        }
        #endregion
    }
}
