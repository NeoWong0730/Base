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
    public class PetExpeditionTaskParam
    {
        public uint taskId;
        public bool isUnderway;
    }
    public class UI_PetExpedition_Task : UIBase
    {
        private uint taskId;
        private bool isUnderway;

        private Button btnClose;
        private Text txtActivityTime;
        private Image imgBgQuality;
        private Text txtBgQuality;
        private Text txtTaskTitle;
        private Text txtTaskPoint;//调查点数
        private GameObject goGoing;
        private GameObject goFinish;
        private Text txtTaskLv;
        private Text txtTaskScore;//推荐评分
        private Text txtTaskTime;
        private Text txtStrPercent;//强度
        private Text txtElementPercent;//元素
        private Text txtRacePercent;//种族
        private Button btnGetReward;//领奖
        private GameObject goCanGet;
        private Text txtBaseReward;
        private Text txtSuccessPercent;//成功率
        private Button btnBeginTask;//开始探险
        private Button btnBack;//返回
        private Animator ani;
        private Button btnRaceRule;//种族规则
        private GameObject goRewardUp;//奖励提升
        private GameObject goPetEmpty;//宠物列表为空的提示
        private Button btnAutoExpedition;//自动派遣

        private InfinityGrid infinity;
        private List<GameObject> listStar = new List<GameObject>();
        private List<UI_PetExpeditionTaskMonsterCell> listBoss = new List<UI_PetExpeditionTaskMonsterCell>();
        private List<PropItem> listRewards = new List<PropItem>();
        private List<ClientPet> listPet = new List<ClientPet>();
        private List<ClientPet> listSelectedPetData = new List<ClientPet>();
        private List<UI_PetExpeditionTaskSelectedPetCell> listSelectedPetCell = new List<UI_PetExpeditionTaskSelectedPetCell>();

        private Timer timer;
        private float countDownTime = 0;
        private Timer btnCDTimer;   //按钮cd 防止多次请求
        private bool isBtnCD;
        private bool isSelectState = false;//是否是选择状态
        private int selectedIndex = -1;//选中的格子下标
        private bool isFinish = false;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(PetExpeditionTaskParam))
            {
                PetExpeditionTaskParam param = arg as PetExpeditionTaskParam;
                taskId = param.taskId;
                Debug.Log(taskId);
                isUnderway = param.isUnderway;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            Sys_PetExpediton.Instance.curSelectedPetUid.Clear();
            UpdateView();
        }
        protected override void OnHide()
        {

        }
        protected override void OnDestroy()
        {
            timer?.Cancel();
            btnCDTimer?.Cancel();
            Sys_PetExpediton.Instance.curSelectedPetUid.Clear();
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
            txtActivityTime = transform.Find("Animator/Time").GetComponent<Text>();
            imgBgQuality = transform.Find("Animator/Task/Image_Quality").GetComponent<Image>();
            txtBgQuality = transform.Find("Animator/Task/Text").GetComponent<Text>();
            txtTaskTitle = transform.Find("Animator/Task/Text_Title").GetComponent<Text>();
            txtTaskPoint = transform.Find("Animator/Task/Text_Title/Text").GetComponent<Text>();
            goGoing = transform.Find("Animator/Task/Detail/Image_Going").gameObject;
            goFinish = transform.Find("Animator/Task/Detail/Image_End").gameObject;
            txtTaskLv = transform.Find("Animator/Task/Detail/Text_Lv").GetComponent<Text>();
            txtTaskScore = transform.Find("Animator/Task/Detail/Text_Score").GetComponent<Text>();
            txtTaskTime = transform.Find("Animator/Task/Detail/Text_Time").GetComponent<Text>();
            txtStrPercent = transform.Find("Animator/LayoutBuff/Buff0/Text_Add").GetComponent<Text>();
            txtElementPercent = transform.Find("Animator/LayoutBuff/Buff1/Text_Add").GetComponent<Text>();
            txtRacePercent = transform.Find("Animator/LayoutBuff/Buff2/Text_Add").GetComponent<Text>();
            btnGetReward = transform.Find("Animator/Reward/Image_Icon").GetComponent<Button>();
            btnGetReward.onClick.AddListener(OnBtnGetRewardClick);
            goCanGet = transform.Find("Animator/Reward/Image_Icon/Get").gameObject;
            txtBaseReward = transform.Find("Animator/Reward/Text_Reward").GetComponent<Text>();
            txtSuccessPercent = transform.Find("Animator/Reward/Text02/Text_Rate").GetComponent<Text>();
            btnBeginTask = transform.Find("Animator/Bottom/Btn_01").GetComponent<Button>();
            btnBeginTask.onClick.AddListener(OnBtnBeginTaskClick);
            btnBack = transform.Find("Animator/Bottom/Btn_02").GetComponent<Button>();
            btnBack.onClick.AddListener(OnBtnBackClick);
            ani = transform.Find("Animator/Reward/Image_Icon/Icon").GetComponent<Animator>();
            btnRaceRule = transform.Find("Animator/LayoutBuff/Buff2/Button").GetComponent<Button>();
            btnRaceRule.onClick.AddListener(OnBtnRaceRule);
            goRewardUp = transform.Find("Animator/Reward/RewardUp").gameObject;
            goPetEmpty = transform.Find("Animator/Empty").gameObject;
            btnAutoExpedition = transform.Find("Animator/Task/Button_Dispatch").GetComponent<Button>();
            btnAutoExpedition.onClick.AddListener(OnBtnAutoExpeditionClick);

            var starParent = transform.Find("Animator/Task/Layout_Star");
            listStar.Clear();
            for (int i = 0; i < starParent.childCount; i++)
            {
                var star = starParent.GetChild(i);
                listStar.Add(star.gameObject);
            }
            infinity = transform.Find("Animator/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
            listBoss.Clear();
            for (int i = 1; i < 3; i++)
            {
                Transform monster = transform.Find("Animator/Task/MonsterItem0" + i.ToString());
                UI_PetExpeditionTaskMonsterCell cellMon = new UI_PetExpeditionTaskMonsterCell();
                cellMon.Init(monster);
                listBoss.Add(cellMon);
            }
            var rewardParent = transform.Find("Animator/Reward/Layout_Reward");
            listRewards.Clear();
            for (int i = 0; i < rewardParent.childCount; i++)
            {
                var reward = rewardParent.GetChild(i);
                PropItem itemCell = new PropItem();
                itemCell.BindGameObject(reward.gameObject);
                listRewards.Add(itemCell);
            }
            listSelectedPetCell.Clear();
            for (int i = 0; i < 3; i++)
            {
                Transform pet = transform.Find("Animator/Task/LayoutPet/Item" + i.ToString());
                UI_PetExpeditionTaskSelectedPetCell cellPet = new UI_PetExpeditionTaskSelectedPetCell();
                cellPet.Init(pet, i);
                cellPet.RegisterSelectAction(OnCenterPetSelected);
                cellPet.RegisterDownAction(OnCenterPetDown);
                listSelectedPetCell.Add(cellPet);
            }
        }
        private void UpdateView()
        {
            timer?.Cancel();
            txtActivityTime.text = Sys_PetExpediton.Instance.GetActivityDateText();
            var petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(taskId / 1000);
            if (petTaskData != null)
            {
                uint qualityBgId = Sys_PetExpediton.Instance.GetTaskInfoBgImageIconId(petTaskData.Difficulty);
                ImageHelper.SetIcon(imgBgQuality, qualityBgId);
                txtBgQuality.text = Sys_PetExpediton.Instance.GetTaskBgTitleText(petTaskData.Difficulty);
                for (int i = 0; i < listStar.Count; i++)
                {
                    var star = listStar[i];
                    star.SetActive(i < petTaskData.Difficulty);
                }
                txtTaskTitle.text = LanguageHelper.GetTextContent(petTaskData.Title);
                txtTaskPoint.text = LanguageHelper.GetTextContent(2025628, petTaskData.Point.ToString());//（{0}调查点）;
                txtTaskLv.text = Sys_PetExpediton.Instance.GetTaskRichText(petTaskData.Difficulty, LanguageHelper.GetTextContent(2025606, petTaskData.Level_Condition.ToString()));//推荐等级:{0}
                txtTaskScore.text = Sys_PetExpediton.Instance.GetTaskRichText(petTaskData.Difficulty, LanguageHelper.GetTextContent(2025607, petTaskData.Score_Condition.ToString()));//推荐评分:{0}
                txtTaskTime.text = Sys_PetExpediton.Instance.GetTaskRichText(petTaskData.Difficulty, LanguageHelper.GetTextContent(2025608, Sys_PetExpediton.Instance.GetTaskAllTimeText(petTaskData.Times))); //耗时:{0}
                txtBaseReward.text = LanguageHelper.GetTextContent(2025622, petTaskData.Point.ToString());//基础奖励:{0}冒险点
                btnBeginTask.gameObject.SetActive(!isUnderway);
                btnAutoExpedition.gameObject.SetActive(!isUnderway);
                if (isUnderway)
                {
                    //进行中/已完成
                    var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                    if (taskInfo != null)
                    {
                        uint nowtime = Sys_Time.Instance.GetServerTime();
                        isFinish = taskInfo.EndTick < nowtime;
                        bool isGet = taskInfo.AwardState != 0;
                        goGoing.SetActive(!isFinish);
                        goFinish.SetActive(isFinish);
                        goCanGet.SetActive(isFinish && !isGet);
                        btnBack.gameObject.SetActive(!isFinish);
                        if (!isFinish)
                        {
                            StartTimer(taskInfo.EndTick - nowtime);
                        }
                        if (isFinish && !isGet)
                        {
                            ani.Play("Open");
                        }
                        else
                        {
                            ani.Play("Empty");
                        }
                    }
                }
                else
                {
                    goGoing.SetActive(false);
                    goFinish.SetActive(false);
                    goCanGet.SetActive(false);
                    btnBack.gameObject.SetActive(true);
                    ani.Play("Empty");
                }
                for (int i = 0; i < listBoss.Count; i++)
                {
                    var cellBoss = listBoss[i];
                    if (i < petTaskData.Chieftain.Count)
                    {
                        cellBoss.UpdateCellView(petTaskData.Chieftain[i]);
                    }
                    else
                    {
                        cellBoss.SetActive(false);
                    }
                }
                var rewardItems = CSVDrop.Instance.GetDropItem(petTaskData.Success_Reward);
                for (int i = 0; i < listRewards.Count; i++)
                {
                    PropItem itemCell = listRewards[i];
                    if (i < rewardItems.Count)
                    {
                        itemCell.transform.gameObject.SetActive(true);
                        var itemData = new PropIconLoader.ShowItemData(rewardItems[i].id, rewardItems[i].count, true, false, false, false, false, true);
                        itemCell.SetData(itemData, EUIID.UI_PetExpedition_RewardList);
                    }
                    else
                    {
                        itemCell.transform.gameObject.SetActive(false);
                    }
                }
                UpdateLeftPetList();
                UpdateCenterPet();
                UpdatePresentView();
            }
        }
        /// <summary> 刷新加成百分比 </summary>
        private void UpdatePresentView()
        {
            txtStrPercent.text = Sys_PetExpediton.Instance.GetStrPresentText(taskId, isUnderway);
            txtElementPercent.text = Sys_PetExpediton.Instance.GetElementPresentText(taskId, isUnderway);
            txtRacePercent.text = Sys_PetExpediton.Instance.GetRacePresentText(taskId, isUnderway);
            uint allPre = Sys_PetExpediton.Instance.GetAllPresentNum(taskId, isUnderway);
            goRewardUp.SetActive(allPre > 100);
            txtSuccessPercent.text = Sys_PetExpediton.Instance.GetSuccessPresentText(allPre);
        }
        /// <summary> 刷新左边宠物列表 </summary>
        private void UpdateLeftPetList()
        {
            listPet = Sys_PetExpediton.Instance.GetValidPetList();
            goPetEmpty.SetActive(listPet.Count <= 0);
            infinity.CellCount = listPet.Count;
            infinity.ForceRefreshActiveCell();
        }
        /// <summary> 刷新中间派遣宠物 </summary>
        private void UpdateCenterPet()
        {
            for (int i = 0; i < listSelectedPetCell.Count; i++)
            {
                var cell = listSelectedPetCell[i];
                List<uint> listPetId;
                if (isUnderway)
                {
                    listPetId = new List<uint>();
                    var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                    if (taskInfo != null)
                    {
                        for (int j = 0; j < taskInfo.PetInfos.Count; j++)
                        {
                            var petInfo = taskInfo.PetInfos[j];
                            listPetId.Add(petInfo.PetInfoId);
                        }
                    }
                }
                else
                {
                    listPetId = Sys_PetExpediton.Instance.curSelectedPetUid;
                }
                uint id = i < listPetId.Count ? listPetId[i] : 0;
                cell.UpdateCellView(id, !isUnderway, isSelectState, selectedIndex);
            }
        }
        private void StartTimer(uint cd)
        {
            countDownTime = cd;
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        #endregion

        #region event

        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnPetExpeditonDataUpdate()
        {
            UpdateView();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetExpeditionTaskPetCell mCell = new UI_PetExpeditionTaskPetCell();
            mCell.Init(cell.mRootTransform.transform);
            mCell.RegisterAction(OnLeftPetSelected);
            cell.BindUserData(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PetExpeditionTaskPetCell mCell = cell.mUserData as UI_PetExpeditionTaskPetCell;
            mCell.UpdateCellView(listPet[index], isSelectState);
        }
        private void OnBtnGetRewardClick()
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

            if (isFinish)
            {
                //领奖
                Sys_PetExpediton.Instance.ReqPetExploreFinishGetAwardReq(taskId);
            }
        }
        private void OnBtnBeginTaskClick()
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
            //接任务
            if (Sys_PetExpediton.Instance.curSelectedPetUid.Count > 0)
            {
                Sys_PetExpediton.Instance.ReqPetExploreStart(taskId);
                this.CloseSelf();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025647));//请选择进行探险的宠物
            }
        }
        private void OnBtnBackClick()
        {
            this.CloseSelf();
        }
        private void OnBtnRaceRule()
        {
            //弹种族规则界面
            UIManager.OpenUI(EUIID.UI_Element);
        }
        private void OnBtnAutoExpeditionClick()
        {
            Sys_PetExpediton.Instance.AutoSetExplorePetList();
            UpdateView();
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            UpdateView();
        }
        private void OnTimerUpdate(float time)
        {
            //if (countDownTime >= time && txtRemainingTime != null)
            //{
            //    txtRemainingTime.text = LanguageHelper.GetTextContent(2025631, LanguageHelper.TimeToString((uint)(countDownTime - time), LanguageHelper.TimeFormat.Type_4));
            //}
        }
        /// <summary>
        /// 左边列表宠物被选中
        /// </summary>
        private void OnLeftPetSelected(uint uid)
        {
            isSelectState = false;
            if(selectedIndex < Sys_PetExpediton.Instance.curSelectedPetUid.Count)
            {
                //替换
                Sys_PetExpediton.Instance.curSelectedPetUid[selectedIndex] = uid;
            }
            else
            {
                //新增
                Sys_PetExpediton.Instance.curSelectedPetUid.Add(uid);
            }
            //刷新左边、中间宠物，以及属性加成
            UpdateLeftPetList();
            UpdateCenterPet();
            UpdatePresentView();
        }
        /// <summary>
        /// 中间宠物格子被选中
        /// </summary>
        private void OnCenterPetSelected(int index)
        {
            isSelectState = true;
            selectedIndex = index;
            //刷新左边列表
            UpdateLeftPetList();
            UpdateCenterPet();
        }
        /// <summary>
        /// 中间选中的宠物被取消选择
        /// </summary>
        private void OnCenterPetDown(int index)
        {
            isSelectState = false;
            List<uint> newList = new List<uint>();
            var oldList = Sys_PetExpediton.Instance.curSelectedPetUid;
            for (int i = 0; i < oldList.Count; i++)
            {
                if(index != i)
                {
                    newList.Add(oldList[i]);
                }
            }
            Sys_PetExpediton.Instance.curSelectedPetUid = newList;
            UpdateLeftPetList();
            UpdateCenterPet();
            UpdatePresentView();
        }
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        #endregion


        #region class
        public class UI_PetExpeditionTaskPetCell
        {
            private ClientPet clientPetData;

            private Transform transform;
            private Image imgIcon;
            private Text txtLv;
            private Text txtName;
            private Text txtRace;//种族
            private Transform attrParent;
            private Text txtScore;//评分
            private GameObject goArrowUp;
            private Button btnArrowUp;
            private Button btnSelf;
            private bool isSelectState;
            private Action<uint> selectAct;
            public void Init(Transform trans)
            {
                transform = trans;
                imgIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
                txtLv = transform.Find("PetItem/Image_Level/Text_Level/Text_Num").GetComponent<Text>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtRace = transform.Find("Text_Race").GetComponent<Text>();
                attrParent = transform.Find("Image_Attr");
                txtScore = transform.Find("Image_Point/Text_Num").GetComponent<Text>();
                goArrowUp = transform.Find("PetItem/Image_Blank").gameObject;
                btnArrowUp = transform.Find("PetItem/Image_Blank").GetComponent<Button>();
                btnArrowUp.onClick.AddListener(OnBtnSelfClick);
                btnSelf = transform.Find("Image").GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnSelfClick);
            }
            public void UpdateCellView(ClientPet _clientPetData,bool _isSelectState)
            {
                clientPetData = _clientPetData;
                isSelectState = _isSelectState;
                CSVPetNew.Data petData = clientPetData.petData;
                if (petData != null)
                {
                    ImageHelper.SetIcon(imgIcon, petData.icon_id);
                    txtName.text = LanguageHelper.GetTextContent(petData.name);
                    CSVGenus.Data genusData = CSVGenus.Instance.GetConfData(petData.race);
                    if (genusData != null)
                    {
                        txtRace.text = LanguageHelper.GetTextContent(genusData.rale_name);
                    }
                    var petAttrList = petData.init_attr;
                    FrameworkTool.CreateChildList(attrParent, petAttrList.Count);
                    for (int i = 0; i < petAttrList.Count; i++)
                    {
                        Transform child = attrParent.GetChild(i);
                        uint id = petData.init_attr[i][0];
                        if (CSVAttr.Instance.TryGetValue(id, out CSVAttr.Data data) && id != 101)
                        {
                            ImageHelper.SetIcon(child.Find("Image_Attr").GetComponent<Image>(), data.attr_icon);
                            child.Find("Image_Attr/Text").GetComponent<Text>().text = petData.init_attr[i][1].ToString();
                        }
                        else
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
                txtLv.text = clientPetData.petUnit.SimpleInfo.Level.ToString();
                txtScore.text = clientPetData.petUnit.SimpleInfo.Score.ToString();
                bool isSelected = Sys_PetExpediton.Instance.CheckPetIsSelected(clientPetData.petUnit.Uid);
                goArrowUp.SetActive(isSelectState && !isSelected);
            }

            public void RegisterAction(Action<uint> act)
            {
                selectAct = act;
            }
            private void OnBtnSelfClick()
            {
                bool isSelected = Sys_PetExpediton.Instance.CheckPetIsSelected(clientPetData.petUnit.Uid);
                if (isSelectState && !isSelected)
                {
                    selectAct?.Invoke(clientPetData.petUnit.Uid);
                }
            }
        }

        public class UI_PetExpeditionTaskMonsterCell
        {
            private Transform transform;

            private Image imgIcon;
            private Text txtName;
            private Text txtRace;//种族
            private Transform attrParent;

            public void Init(Transform trans)
            {
                transform = trans;
                imgIcon = transform.Find("Item/Image_Icon").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtRace = transform.Find("Text_Race").GetComponent<Text>();
                attrParent = transform.Find("Image_Attr");
            }

            public void UpdateCellView(uint petId)
            {
                transform.gameObject.SetActive(true);
                CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(petId);
                if (petData != null)
                {
                    ImageHelper.SetIcon(imgIcon, petData.icon_id);
                    txtName.text = LanguageHelper.GetTextContent(petData.name);
                    CSVGenus.Data genusData = CSVGenus.Instance.GetConfData(petData.race);
                    if (genusData != null)
                    {
                        txtRace.text = LanguageHelper.GetTextContent(genusData.rale_name);
                    }
                    var petAttrList = petData.init_attr;
                    FrameworkTool.CreateChildList(attrParent, petAttrList.Count);
                    for (int i = 0; i < petAttrList.Count; i++)
                    {
                        Transform child = attrParent.GetChild(i);
                        uint id = petData.init_attr[i][0];
                        if (CSVAttr.Instance.TryGetValue(id, out CSVAttr.Data data) && id != 101)
                        {
                            ImageHelper.SetIcon(child.Find("Image_Attr").GetComponent<Image>(), data.attr_icon);
                            child.Find("Image_Attr/Text").GetComponent<Text>().text = petData.init_attr[i][1].ToString();
                        }
                        else
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                }
            }

            public void SetActive(bool isActive)
            {
                transform.gameObject.SetActive(isActive);
            }
        }
        /// <summary> 派遣位置的宠物模块 </summary>
        public class UI_PetExpeditionTaskSelectedPetCell
        {
            private Transform transform;
            private uint Uid;
            private int cellIndex;

            private Image imgIcon;
            private Text txtName;
            private Button btnAdd;
            private Button btnArrow;
            private GameObject goArrowDown;
            private GameObject goselect;
            private Action<int> selectAct;
            private Action<int> downAct;
            private bool isSelectState;
            private int curSelectIndex = -1;
            public void Init(Transform trans, int index)
            {
                transform = trans;
                cellIndex = index;
                imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                btnAdd = transform.Find("ImageBG").GetComponent<Button>();
                btnAdd.onClick.AddListener(OnBtnAddClick);
                btnArrow = transform.Find("Image_Blank").GetComponent<Button>();
                btnArrow.onClick.AddListener(OnBtnArrowClick);
                goArrowDown = transform.Find("Image_Blank/Image_ArrowDown").gameObject;
                goselect = transform.Find("Image_Select").gameObject;
            }

            public void UpdateCellView(uint petId, bool isUsable = false, bool _isSelectState = false, int _selectIndex = -1)
            {
                isSelectState = _isSelectState;
                curSelectIndex = _selectIndex;
                CSVPetNew.Data petData = null;
                if (isUsable)
                {
                    //UID
                    var clientPetData = Sys_PetExpediton.Instance.GetClientPetBuyUid(petId);
                    if (clientPetData != null)
                    {
                        petData = clientPetData.petData;
                    }
                }
                else
                {
                    //宠物id
                    petData = CSVPetNew.Instance.GetConfData(petId);
                }
                if (petData != null)
                {
                    imgIcon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgIcon, petData.icon_id);
                    txtName.text = LanguageHelper.GetTextContent(petData.name);
                }
                else
                {
                    imgIcon.gameObject.SetActive(false);
                    txtName.text = "";
                }
                btnAdd.gameObject.SetActive(isUsable && cellIndex <= Sys_PetExpediton.Instance.curSelectedPetUid.Count);
                goselect.SetActive(isUsable && isSelectState && curSelectIndex == cellIndex);
                bool showArrow = isUsable && isSelectState && petData != null && curSelectIndex == cellIndex;
                btnArrow.gameObject.SetActive(showArrow);
                goArrowDown.SetActive(showArrow);
            }


            public void RegisterSelectAction(Action<int> act)
            {
                selectAct = act;
            }
            public void RegisterDownAction(Action<int> act)
            {
                downAct = act;
            }
            private void OnBtnAddClick()
            {
                selectAct?.Invoke(cellIndex);
            }

            private void OnBtnArrowClick()
            {
                if (isSelectState && curSelectIndex == cellIndex)
                {
                    downAct?.Invoke(cellIndex);
                }
            }

        }
        #endregion
    }
}
