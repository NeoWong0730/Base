using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using DG.Tweening;

namespace Logic
{
    /// <summary>
    /// 右边详情展示
    /// </summary>
    public class TrialRightCommonDetails
    {
        GameObject objWhiteBg;
        Transform transSkillRight;
        Image skillIcon;
        Text skillName;
        Text skillType;
        GameObject skillState;
        Text skillContent;
        Transform viewUnlock;
        Transform viewUnActivite;
        Transform viewActivite;

        Transform transPetRight;
        InfinityGrid petInfinityGrid;

        GameObject objEmpty;
        Text emptyTitle;

        List<PetDataCell> petDataCellList;
        SkillColumnDeploy curSelectSkillColumnDeploy;
        SkillColumnDeploy.FirstSkillCell curSelectFirstSkillCell;

        List<TrialFirstSkillItem> firstSkillItemList = new List<TrialFirstSkillItem>();
        Action<SkillColumnDeploy.FirstSkillCell> firstSkillAction;
        bool isShowBtn;
        public static uint curSelectPetUid;
        uint viewType;
        public void InitSkillAndPet(Transform view, uint viewType = 0)
        {
            this.viewType = viewType;
            objWhiteBg = view.Find("RawImage").gameObject;
            objEmpty = view.Find("Empty").gameObject;
            emptyTitle = view.Find("Empty/Text_Tips").GetComponent<Text>();
            transSkillRight = view.Find("Skill");
            skillIcon = transSkillRight.Find("Top/Image_Icon").GetComponent<Image>();
            skillName = transSkillRight.Find("Top/Text_Skill_Name").GetComponent<Text>();
            skillType = transSkillRight.Find("Top/Text").GetComponent<Text>();
            skillState = transSkillRight.Find("Top/Image_Label").gameObject;
            skillContent = transSkillRight.Find("Skill_Detail/Text/Content").GetComponent<Text>();

            viewUnlock = transSkillRight.Find("View_Unlock");
            viewUnActivite = transSkillRight.Find("View_UnActivite");
            viewActivite = transSkillRight.Find("View_Activite");

            transPetRight = view.Find("Pet");
            petInfinityGrid = transPetRight.Find("Scroll View").GetComponent<InfinityGrid>();

            petInfinityGrid.onCreateCell += OnPetCreateCell;
            petInfinityGrid.onCellChange += OnPetCellChange;
            transPetRight?.gameObject.SetActive(false);
            transSkillRight?.gameObject.SetActive(false);
            objWhiteBg?.SetActive(false);
            objEmpty?.SetActive(true);
            RefreshEmptyPos(viewType);
        }
        /// <summary>
        /// 刷新空提示位置
        /// </summary>
        /// <param name="viewType"></param>
        public void RefreshEmptyPos(uint viewType)
        {
            this.viewType = viewType;
            if (viewType == 1)
                objEmpty.transform.localPosition = new Vector3(100, objEmpty.transform.localPosition.y, objEmpty.transform.localPosition.z);
            else if (viewType == 2)
                objEmpty.transform.localPosition = new Vector3(485, objEmpty.transform.localPosition.y, objEmpty.transform.localPosition.z);
            emptyTitle.text = LanguageHelper.GetTextContent(3899000017);
        }
        public void InitSkill(Transform view)
        {
            objWhiteBg = view.Find("RawImage").gameObject;
            objEmpty = view.Find("Empty").gameObject;
            emptyTitle = view.Find("Empty/Text_Tips").GetComponent<Text>();
            transSkillRight = view.Find("Skill");
            skillIcon = transSkillRight.Find("Top/Image_Icon").GetComponent<Image>();
            skillName = transSkillRight.Find("Top/Text_Skill_Name").GetComponent<Text>();
            skillType = transSkillRight.Find("Top/Text").GetComponent<Text>();
            skillState = transSkillRight.Find("Top/Image_Label").gameObject;
            skillContent = transSkillRight.Find("Skill_Detail/Text/Content").GetComponent<Text>();

            viewUnlock = transSkillRight.Find("View_Unlock");
            viewUnActivite = transSkillRight.Find("View_UnActivite");
            viewActivite = transSkillRight.Find("View_Activite");
            transSkillRight?.gameObject.SetActive(false);
            objWhiteBg?.SetActive(false);
            objEmpty?.SetActive(true);
        }
        public void AddListener(Action<SkillColumnDeploy.FirstSkillCell> firstSkillAction)
        {
            this.firstSkillAction = firstSkillAction;
        }
        /// <summary>
        /// 按照点击展示右边详情
        /// </summary>
        /// <param name="type">type=0(展示宠物列表) type=1(展示超级技能) type=2(展示前置技能)</param>
        /// <param name="data"></param>
        /// <param name="data_1"></param>
        /// <param name="isShowBtn">是否显示前置技能的两个按钮</param>
        public void RefreshRightShow(uint type, SkillColumnDeploy data = null, SkillColumnDeploy.FirstSkillCell data_1 = null, bool isShowBtn = true)
        {
            curSelectSkillColumnDeploy = data;
            curSelectFirstSkillCell = data_1;
            this.isShowBtn = isShowBtn;
            if (type == 0)
            {
                transSkillRight?.gameObject.SetActive(false);
                if (curSelectSkillColumnDeploy != null)
                {
                    transPetRight?.gameObject.SetActive(true);
                    curSelectPetUid = curSelectSkillColumnDeploy.petUid;
                    petDataCellList = Sys_ActivityTrialGate.Instance.GetPetList();
                    if (petDataCellList != null && petDataCellList.Count > 0)
                    {
                        RefreshPetShow();
                        objWhiteBg?.SetActive(true);
                        objEmpty?.SetActive(false);
                    }
                    else
                    {
                        transPetRight?.gameObject.SetActive(false);
                        objWhiteBg?.SetActive(false);
                        objEmpty?.SetActive(true);
                        if (viewType == 1)
                        {
                            objEmpty.transform.localPosition = new Vector3(485, objEmpty.transform.localPosition.y, objEmpty.transform.localPosition.z);
                            emptyTitle.text = LanguageHelper.GetTextContent(3899000030);
                        }
                    }
                }
                else
                {
                    transPetRight?.gameObject.SetActive(false);
                    objWhiteBg?.SetActive(false);
                    objEmpty?.SetActive(true);
                    if (viewType == 1)
                    {
                        objEmpty.transform.localPosition = new Vector3(485, objEmpty.transform.localPosition.y, objEmpty.transform.localPosition.z);
                        emptyTitle.text = LanguageHelper.GetTextContent(3899000030);
                    }
                }
            }
            else
            {
                transPetRight?.gameObject.SetActive(false);
                if (curSelectSkillColumnDeploy != null || curSelectFirstSkillCell != null)
                {
                    transSkillRight?.gameObject.SetActive(true);
                    objWhiteBg?.SetActive(true);
                    objEmpty?.SetActive(false);
                    RefreshSkillShow(type);
                }
                else
                {
                    transSkillRight?.gameObject.SetActive(false);
                    objWhiteBg?.SetActive(false);
                    objEmpty?.SetActive(true);
                    if (viewType == 2)
                    {
                        objEmpty.transform.localPosition = new Vector3(485, objEmpty.transform.localPosition.y, objEmpty.transform.localPosition.z);
                        emptyTitle.text = LanguageHelper.GetTextContent(3899000017);
                    }
                }
            }
        }
        public void RefreshRighShowDefault(uint type)
        {
            if (type == 0)
                RefreshPetShow();
            else
                RefreshSkillShow(type);
        }
        #region skill
        private void RefreshSkillShow(uint type)
        {
            if (type == 1)
            {
                if (curSelectSkillColumnDeploy != null)
                    RefreshSupperSkill(curSelectSkillColumnDeploy);
            }
            else
            {
                if (curSelectFirstSkillCell != null)
                    RefreshFirstSkill(curSelectFirstSkillCell);
            }
        }
        //TODO
        private void SetCommonData(SkillColumnDeploy data, SkillColumnDeploy.FirstSkillCell data_1)
        {
            CSVPassiveSkillInfo.Data skillInfo;
            if (data != null)
            {
                skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data.superSkill.skillId);
                skillState.SetActive(!data.CheckSuperSkillIsActivate());
            }
            else
            {
                skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data_1.skillId);
                skillState.SetActive(!data_1.activateState);
            }
            if (skillInfo != null)
            {
                ImageHelper.SetIcon(skillIcon, skillInfo.icon);
                skillName.text = LanguageHelper.GetTextContent(skillInfo.name);
                skillContent.text = LanguageHelper.GetTextContent(skillInfo.desc);
            }
            //skillType.text = "";
        }
        private void RefreshSupperSkill(SkillColumnDeploy data)
        {
            viewUnlock.gameObject.SetActive(true);
            viewUnActivite.gameObject.SetActive(false);
            viewActivite.gameObject.SetActive(false);
            viewUnlock.gameObject.SetActive(true);
            SetCommonData(data, null);
            Transform parent = viewUnlock.Find("Skill_Layout");
            for (int i = 0; i < firstSkillItemList.Count; i++)
            {
                TrialFirstSkillItem cell = firstSkillItemList[i];
                cell.ClearAllEvent();
                PoolManager.Recycle(cell);
            }
            firstSkillItemList.Clear();
            FrameworkTool.CreateChildList(parent, data.firstSkillCellList.Count);
            for (int i = 0; i < data.firstSkillCellList.Count; i++)
            {
                Transform trans = parent.GetChild(i);
                TrialFirstSkillItem cell = PoolManager.Fetch<TrialFirstSkillItem>();
                cell.Init(trans,false);
                cell.SetData(data.firstSkillCellList[i], RefreshFirstSkill);
                firstSkillItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(parent.gameObject);
        }
        private void RefreshFirstSkill(SkillColumnDeploy.FirstSkillCell data,bool isInvoke=false)
        {
            if (data != null)
            {
                if (data.activateState)
                    SetViewActivite(data);
                else
                    SetViewUnActivite(data);
                if (isInvoke)
                    firstSkillAction?.Invoke(data);
            }
        }
        private void SetViewUnActivite(SkillColumnDeploy.FirstSkillCell data)
        {
            viewUnlock.gameObject.SetActive(false);
            viewUnActivite.gameObject.SetActive(true);
            viewActivite.gameObject.SetActive(false);
            SetCommonData(null, data);
            Text tip1, tip2, tip3, badgeNum;
            Image badgeIcon;
            Button btnCheckFrom, btnUnlock;
            tip1 = viewUnActivite.Find("Text_Tip01").GetComponent<Text>();
            tip2 = viewUnActivite.Find("Text_Tip02").GetComponent<Text>();
            tip3 = viewUnActivite.Find("Text_Tip03").GetComponent<Text>();
            badgeNum = viewUnActivite.Find("Text_Num").GetComponent<Text>();
            badgeIcon = viewUnActivite.Find("Image_Icon").GetComponent<Image>();
            btnCheckFrom = viewUnActivite.Find("Btn_02").GetComponent<Button>();
            btnUnlock = viewUnActivite.Find("Btn_01").GetComponent<Button>();

            if (!isShowBtn || Sys_Fight.Instance.IsFight())
            {
                btnCheckFrom.gameObject.SetActive(false);
                btnUnlock.gameObject.SetActive(false);
            }
            else
            {
                btnCheckFrom.gameObject.SetActive(true);
                btnUnlock.gameObject.SetActive(true);
            }

            uint supperSkillId= Sys_ActivityTrialGate.Instance.GetSkillColumnDeployByBarId(data.barId).superSkill.skillId;
            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(supperSkillId);
            string supperSkillName = string.Empty;
            if (skillInfo != null)
                supperSkillName = LanguageHelper.GetTextContent(skillInfo.name);
            BadgeData curBadgeData = Sys_ActivityTrialGate.Instance.GeBadgeDataByTid(data.badgeType);
            tip1.text = LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_source).Split('\n')[0];
            tip2.text = LanguageHelper.GetTextContent(3899000032, data.csv_trialPreSkill.badge_number.ToString(), LanguageHelper.GetTextContent(curBadgeData.csv_trialBadge.badge_name));
            tip3.text = LanguageHelper.GetTextContent(3899000025, supperSkillName) ;
            badgeNum.text = string.Format("{0}/{1}", Sys_ActivityTrialGate.Instance.GeBadgeNum(data.badgeType), data.csv_trialPreSkill.badge_number);
            ImageHelper.SetIcon(badgeIcon, curBadgeData.csv_trialBadge.bigIcon_id);
            btnCheckFrom.onClick.RemoveAllListeners();
            //查看来源
            btnCheckFrom.onClick.AddListener(() => {
                List<BadgeData.BossData> bossDataList = curBadgeData.GetCurLevelStageBossData(Sys_ActivityTrialGate.Instance.GetCurLevelGradeId());
                if (bossDataList != null && bossDataList.Count > 0)
                {
                    TrialBadgeOpenParam param = new TrialBadgeOpenParam();
                    param.type = 2;
                    //param.Pos = CameraManager.mUICamera.WorldToScreenPoint(btnCheckFrom.GetComponent<RectTransform>().position);
                    param.badgeId = data.badgeType;
                    UIManager.OpenUI(EUIID.UI_TrialBadgeTips, false, param);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000026));
                }
            });
            btnUnlock.onClick.RemoveAllListeners();
            //解锁
            btnUnlock.onClick.AddListener(() => {
                Sys_ActivityTrialGate.Instance.OnUnlockSkillReq(data);
            });
        }
        private void SetViewActivite(SkillColumnDeploy.FirstSkillCell data)
        {
            viewUnlock.gameObject.SetActive(false);
            viewUnActivite.gameObject.SetActive(false);
            viewActivite.gameObject.SetActive(true);
            SetCommonData(null, data);
            Text tip3;
            tip3 = viewUnActivite.Find("Text_Tip03").GetComponent<Text>();

            uint supperSkillId = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployByBarId(data.barId).superSkill.skillId;
            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(supperSkillId);
            string supperSkillName = string.Empty;
            if (skillInfo != null)
                supperSkillName = LanguageHelper.GetTextContent(skillInfo.name);
            tip3.text = LanguageHelper.GetTextContent(3899000025, supperSkillName);
        }

        #endregion

        #region pet
        /// <summary>
        /// 点击技能栏宠物显示右边宠物列表
        /// </summary>
        /// <param name="petUid"></param>
        private void RefreshPetShow()
        {
            petDataCellList = Sys_ActivityTrialGate.Instance.GetPetList();
            petInfinityGrid.CellCount = petDataCellList.Count;
            petInfinityGrid.ForceRefreshActiveCell();
        }
        List<TrialPetShowItem> petShowItemList = new List<TrialPetShowItem>();
        private void OnPetCreateCell(InfinityGridCell cell)
        {
            TrialPetShowItem entry = new TrialPetShowItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            petShowItemList.Add(entry);
        }
        private void OnPetCellChange(InfinityGridCell cell, int index)
        {
            TrialPetShowItem entry = cell.mUserData as TrialPetShowItem;
            entry.SetData(petDataCellList[index], curSelectSkillColumnDeploy, PetOnSelected);//索引数据
        }
        private void PetOnSelected(PetDataCell petDataCell)
        {
            curSelectPetUid = petDataCell.clientPet.petUnit.Uid;
            if (petShowItemList != null && petShowItemList.Count > 0)
            {
                for (int i = 0; i < petShowItemList.Count; i++)
                {
                    if (petDataCell.clientPet.petUnit.Uid == petShowItemList[i].data.clientPet.petUnit.Uid)
                    {
                        petShowItemList[i].Select();
                    }
                    else
                    {
                        petShowItemList[i].Release();
                    }
                }
            }
            //请求上阵宠物
            Sys_ActivityTrialGate.Instance.OnSetNodePetReq(curSelectSkillColumnDeploy.barId, petDataCell.clientPet.petUnit.Uid);
        }
        #endregion
    }
    #region classItem
    public class TrialPetShowItem
    {
        Button btnPet;
        GameObject objDeploy;
        GameObject objSelect;
        GameObject ObjIsFight;
        Image petIcon;
        Text textLevel;
        Text textName;
        Text textScore;

        Action<PetDataCell> action;
        public PetDataCell data;
        public void Init(Transform trans)
        {
            btnPet = trans.Find("Image").GetComponent<Button>();
            objDeploy = trans.Find("ImageMask").gameObject;
            petIcon = trans.Find("PetItem/Image_Icon").GetComponent<Image>();
            textLevel = trans.Find("PetItem/Image_Level/Text_Level/Text_Num").GetComponent<Text>();
            textName = trans.Find("Text_Name").GetComponent<Text>();
            textScore = trans.Find("Text_Num").GetComponent<Text>();
            objSelect = trans.Find("Image_Select").gameObject;
            ObjIsFight = trans.Find("PetItem/Image_Fight").gameObject;

            btnPet.onClick.AddListener(PetClick);
        }
        public void SetData(PetDataCell data, SkillColumnDeploy skillColumnDeploy, Action<PetDataCell> action)
        {
            this.data = data;
            this.action = action;
            ObjIsFight.SetActive(Sys_ActivityTrialGate.Instance.CheckPetIsDeploy(data.clientPet.petUnit.Uid));
            if (skillColumnDeploy!=null && skillColumnDeploy.petUid != 0)
                objDeploy.SetActive(skillColumnDeploy.petUid == data.clientPet.petUnit.Uid);
            else
                objDeploy.SetActive(false);

            ImageHelper.SetIcon(petIcon, data.clientPet.petData.icon_id);
            textLevel.text = data.clientPet.petUnit.SimpleInfo.Level.ToString();
            if (data.clientPet.petUnit.SimpleInfo.Name.ToStringUtf8() == string.Empty || data.clientPet.petUnit.SimpleInfo.Name.ToStringUtf8() == "")
                textName.text = LanguageHelper.GetTextContent(data.clientPet.petData.name);
            else
                textName.text = data.clientPet.petUnit.SimpleInfo.Name.ToStringUtf8();
            textScore.text = LanguageHelper.GetTextContent(3899000036, data.clientPet.petUnit.SimpleInfo.Score.ToString());
            SetSelect();
        }
        private void PetClick()
        {
            action?.Invoke(data);
        }
        private void SetSelect()
        {
            objSelect.SetActive(false);
            if (TrialRightCommonDetails.curSelectPetUid == data.clientPet.petUnit.Uid)
            {
                objSelect.SetActive(true);
            }
        }
        public void Select()
        {
            objSelect.SetActive(true);
        }
        public void Release()
        {
            objSelect.SetActive(false);
        }
    }
    public class TrialFirstSkillItem
    {
        Button btnSkill;
        Image skillIcon;
        GameObject objLock;
        GameObject objActive;
        GameObject objNum;
        Text badgeNum;
        Image badgeIcon;
        GameObject objSelect;
        public SkillColumnDeploy.FirstSkillCell data { get; private set; }
        Action<SkillColumnDeploy.FirstSkillCell,bool> action;
        bool isClick;
        public void Init(Transform trans, bool isSelect = true)
        {
            btnSkill = trans.Find("Image_Bottom").GetComponent<Button>();
            skillIcon = trans.Find("Icon").GetComponent<Image>();
            objLock = trans.Find("Image_Lock").gameObject;
            objActive = trans.Find("Text").gameObject;
            objNum = trans.Find("Num").gameObject;
            badgeIcon = trans.Find("Num/Image_Icon").GetComponent<Image>();
            badgeNum = trans.Find("Num/Text").GetComponent<Text>();
            if (isSelect)
                objSelect = trans.Find("Select").gameObject;

            btnSkill.onClick.AddListener(SkillClick);
        }
        public void SetData(SkillColumnDeploy.FirstSkillCell data, Action<SkillColumnDeploy.FirstSkillCell,bool> action)
        {
            this.data = data;
            this.action = action;
            CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data.skillId);
            if (skillInfo != null)
                ImageHelper.SetIcon(skillIcon, skillInfo.icon);
            if (data.activateState)
            {
                objLock.SetActive(false);
                objNum.SetActive(false);
                objActive.SetActive(true);
            }
            else
            {
                objLock.SetActive(true);
                objNum.SetActive(true);
                objActive.SetActive(false);
                long haveCount= Sys_ActivityTrialGate.Instance.GeBadgeNum(data.badgeType);
                long anyTokenNum = Sys_ActivityTrialGate.Instance.GeBadgeNum(33);
                long totalNum = haveCount + anyTokenNum;
                badgeNum.text = string.Format("{0}/{1}", haveCount, data.csv_trialPreSkill.badge_number);
                if (totalNum < data.csv_trialPreSkill.badge_number)
                    badgeNum.color = Color.red;
                else
                {
                    ColorUtility.TryParseHtmlString("#8B5A6B", out Color _color);
                    badgeNum.color = _color;
                }
                ImageHelper.SetIcon(badgeIcon, CSVItem.Instance.GetConfData(data.badgeType).icon_id);
            }
        }
        private void SkillClick()
        {
            if (UI_TrialSkillDeploy.curSelectSubSkillId == data.skillId && isClick)
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000040));
            action?.Invoke(data,true);
        }
        public void ClearAllEvent()
        {
            btnSkill.onClick.RemoveAllListeners();
        }
        public void Select()
        {
            objSelect?.SetActive(true);
            isClick = true;
        }
        public void Release()
        {
            objSelect?.SetActive(false);
            isClick = false;
        }
    }
    #endregion


    //试炼个人技能装配界面
    public class UI_TrialSkillDeploy : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            LateUpdate();
        }
        protected override void OnClose()
        {
            skillDeployItemList.Clear();
        }
        #endregion
        #region 组件
        Button btnClose;
        Transform logosTrans;
        Text textResetTime;
        Button btnTips;
        InfinityGrid infinityGrid;

        Button btnPet, btnSupperSkill, btnFirstSkill;
        #endregion
        #region 数据
        TrialRightCommonDetails rightCommonDetails = new TrialRightCommonDetails();
        List<BadgeItem> badgeItemList = new List<BadgeItem>();
        List<SkillColumnDeploy> skillColumnDeployList;
        List<SkillDeployItem> skillDeployItemList = new List<SkillDeployItem>();
        List<BadgeData> badgeDataList = new List<BadgeData>();
        static bool defaultShow;
        bool resetTimeDirty;
        static uint curSelectType;
        static uint curSelectBarId;
        public static uint curSelectSubSkillId;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            logosTrans = transform.Find("Animator/Top/Logos");
            textResetTime = transform.Find("Animator/Top/Text_Time").GetComponent<Text>();
            btnTips = transform.Find("Animator/Top/Button_Tip").GetComponent<Button>();
            infinityGrid = transform.Find("Animator/Skill_List/Scroll View").GetComponent<InfinityGrid>();

            rightCommonDetails.InitSkillAndPet(transform.Find("Animator/View_Skill"));
            btnPet = transform.Find("Animator/Skill_List/Title/Button01").GetComponent<Button>();
            btnSupperSkill = transform.Find("Animator/Skill_List/Title/Button02").GetComponent<Button>();
            btnFirstSkill = transform.Find("Animator/Skill_List/Title/Button03").GetComponent<Button>();

            btnClose.onClick.AddListener(() => { CloseSelf(); });
            btnTips.onClick.AddListener(() => { TipsShow(4); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;

            btnPet.onClick.AddListener(()=> { TipsShow(1); });
            btnSupperSkill.onClick.AddListener(()=> { TipsShow(2); });
            btnFirstSkill.onClick.AddListener(()=> { TipsShow(3); });

            rightCommonDetails.AddListener(FirstSkillOnClick);
        }
        private void TipsShow(uint type)
        {
            Button btn;
            uint lanId;
            float diffX = 0;
            if (type == 1)
            {
                lanId = 3899000007;
                btn = btnPet;
            }
            else if (type == 2)
            {
                lanId = 3899000008;
                btn = btnSupperSkill;
            }
            else if (type == 3)
            {
                lanId = 3899000009;
                btn = btnFirstSkill;
            }
            else
            {
                lanId = 3899000006;
                btn = btnTips;
                diffX = 2.5f;
            }
            Vector3 btnPos = btn.GetComponent<RectTransform>().position;
            Vector3 worldPos = new Vector3(btnPos.x - diffX, btnPos.y, btnPos.z);
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(lanId);
            param.Pos = CameraManager.mUICamera.WorldToScreenPoint(worldPos);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle<uint>(Sys_ActivityTrialGate.EEvents.OnRefreshSkillColumnDeploy, OnRefreshSkillColumnDeploy, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChange, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshTrialGateReset, OnRefreshTrialGateReset, toRegister);
            if (toRegister)
            {
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
                Sys_Fight.Instance.OnExitFight += OnExitFight;
            }
            else
            {
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
                Sys_Fight.Instance.OnExitFight -= OnExitFight;
            }
        }
        private void OnEnterBattle(CSVBattleType.Data obj)
        {
            OnRefreshSkillColumnDeploy(2u);
        }
        private void OnExitFight()
        {
            OnRefreshSkillColumnDeploy(2u);
        }
        /// <summary>
        /// 宠物装配 技能解锁通知
        /// </summary>
        /// <param name="type">type=0(展示宠物列表) type=1(展示超级技能) type=2(展示前置技能)</param>
        private void OnRefreshSkillColumnDeploy(uint type)
        {
            RefreshSkillDeploy();
            rightCommonDetails.RefreshRighShowDefault(type);
        }
        /// <summary>
        /// 刷新徽章数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        private void OnCurrencyChange(uint id, long value)
        {
            RefreshBadge();
            RefreshSkillDeploy();
            rightCommonDetails.RefreshRighShowDefault(curSelectType);
        }
        /// <summary>
        /// 重置刷新
        /// </summary>
        private void OnRefreshTrialGateReset()
        {
            InitBaseData();
        }
        private void FirstSkillOnClick(SkillColumnDeploy.FirstSkillCell data_1)
        {
            curSelectType = 2;
            defaultShow = false;
            int index =-1;
            if (data_1 != null)
            {
                curSelectBarId = data_1.barId;
                curSelectSubSkillId = data_1.skillId;
                for (int i = 0; i < skillColumnDeployList.Count; i++)
                {
                    if (data_1.barId== skillColumnDeployList[i].barId)
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (index != -1)
            {
                float posY = ((index - 1) * (infinityGrid.Spacing.y + infinityGrid.CellSize.y) + infinityGrid.Top + infinityGrid.CellSize.y * 0.5f - infinityGrid.ScrollView.viewport.sizeDelta.y * 0.5f);
                float maxContentY = skillColumnDeployList.Count <= 4 ? 0 : (skillColumnDeployList.Count - 4) * infinityGrid.CellSize.y + 35;
                posY = posY < 0 ? 0 : posY > maxContentY ? maxContentY : posY;
                RefreshSkillDeploy();
                infinityGrid.Content.DOLocalMoveY(Mathf.Min(infinityGrid.ContentSize.y < 0 ? 0 : infinityGrid.ContentSize.y, posY), 0.3f);
            }
        }
        #endregion
        private void InitView()
        {
            defaultShow = true;
            InitBaseData();
        }
        private void InitBaseData()
        {
            resetTimeDirty = true;
            SetBadge();
            curSelectType = 1;
            skillColumnDeployList = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployList();
            if (skillColumnDeployList.Count > 0)
                curSelectBarId = skillColumnDeployList[0].barId;
            SetResetTime();
            RefreshSkillDeploy();
        }
        private void LateUpdate()
        {
            if (resetTimeDirty)
            {
                SetResetTime();
            }
        }
        private void SetResetTime()
        {
            int second = Sys_ActivityTrialGate.Instance.GetActivityRestTimeDiff();
            if (second <= 0)
            {
                second = 0;
                resetTimeDirty = false;
            }
            textResetTime.text = LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_9);
        }
        private void SetBadge()
        {
            badgeDataList = Sys_ActivityTrialGate.Instance.badgeDataList;
            int transCount = logosTrans.childCount;
            for (int i = 0; i < transCount; i++)
            {
                if (i < badgeDataList.Count)
                {
                    Transform trans;
                    if (i == badgeDataList.Count - 1)
                    {
                        logosTrans.GetChild(i).gameObject.SetActive(false);
                        trans = logosTrans.GetChild(transCount - 1);
                    }
                    else
                        trans = logosTrans.GetChild(i);
                    trans.gameObject.SetActive(true);
                    BadgeItem badgeItem = new BadgeItem();
                    badgeItem.Init(trans);
                    badgeItem.SetData(badgeDataList[i]);
                    badgeItemList.Add(badgeItem);
                }
                else
                {
                    if (i != transCount - 1)
                        logosTrans.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
        private void RefreshBadge()
        {
            for (int i = 0; i < badgeDataList.Count; i++)
            {
                badgeItemList[i].SeBadgeNum();
            }
        }
        private void RefreshSkillDeploy()
        {
            skillColumnDeployList = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployList();
            infinityGrid.CellCount = skillColumnDeployList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            SkillDeployItem entry = new SkillDeployItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            skillDeployItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SkillDeployItem entry = cell.mUserData as SkillDeployItem;
            entry.SetData(skillColumnDeployList[index], RefreshRightShow);//索引数据
        }

        ///<param name = "type" > type = 0(展示宠物列表) type=1(展示超级技能) type = 2(展示前置技能) </ param >
        private void RefreshRightShow(uint type, SkillColumnDeploy data, SkillColumnDeploy.FirstSkillCell data_1)
        {
            curSelectType = type;
            curSelectBarId = data.barId;
            if (data_1 != null)
                curSelectSubSkillId = data_1.skillId;
            for (int i = 0; i < skillDeployItemList.Count; i++)
            {
                if (data.barId == skillDeployItemList[i].data.barId)
                {
                    skillDeployItemList[i].Select(type, data_1);
                }
                else
                {
                    skillDeployItemList[i].ReleaseAllOtherSelect();
                }
            }

            rightCommonDetails.RefreshRightShow(type, data, data_1);
        }
        #region Item
        public class BadgeItem
        {
            Image icon;
            Text num;
            BadgeData data;
            Button btn;
            public void Init(Transform trans)
            {
                icon = trans.Find("Image_Icon").GetComponent<Image>();
                num = trans.Find("Text_Num").GetComponent<Text>();
                btn = trans.Find("Image_Icon").GetComponent<Button>();
                btn.onClick.AddListener(OnClick);
            }
            public void SetData(BadgeData data)
            {
                this.data = data;
                ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(data.csv_trialBadge.id).icon_id);
                SeBadgeNum();
            }
            private void OnClick()
            {
                TrialBadgeOpenParam param = new TrialBadgeOpenParam();
                param.type = 1;
                //param.Pos = CameraManager.mUICamera.WorldToScreenPoint(btn.GetComponent<RectTransform>().position);
                param.badgeId = data.badgeId;
                UIManager.OpenUI(EUIID.UI_TrialBadgeTips, false, param);
            }
            public void SeBadgeNum()
            {
                num.text = Sys_ActivityTrialGate.Instance.GeBadgeNum(data.csv_trialBadge.id).ToString();
            }
        }
        public class SkillDeployItem
        {
            Button btnPet;
            Image petIcon;
            Text textPetRecommend;
            GameObject addObj;
            GameObject lockObj;
            GameObject petSelect;

            Image supperSkillIcon;
            Text supperSkillName;
            GameObject supperSkillLock;
            Image supperSkillMask;
            GameObject supperSkillSelect;
            GameObject supperSkillRecommend;
            Button btnSupperSkill;

            Transform transFirstSkill;
            List<TrialFirstSkillItem> firstSkillItemList = new List<TrialFirstSkillItem>();

            public SkillColumnDeploy data { get; private set; }
            Action<uint, SkillColumnDeploy, SkillColumnDeploy.FirstSkillCell> action;
            bool isClick;
            public void Init(Transform trans)
            {
                Transform transPet = trans.Find("Pet");
                Transform transSupperSkill = trans.Find("SuperSkill");
                transFirstSkill = trans.Find("Skill_Layout");
                petIcon = transPet.Find("Icon").GetComponent<Image>();
                textPetRecommend = transPet.Find("Text").GetComponent<Text>();
                addObj = transPet.Find("Button_Add").gameObject;
                lockObj = transPet.Find("Image_Lock").gameObject;
                btnPet = transPet.Find("Image_Bottom").GetComponent<Button>();
                petSelect= transPet.Find("Select").gameObject;

                supperSkillIcon = transSupperSkill.Find("Icon").GetComponent<Image>();
                supperSkillName = transSupperSkill.Find("Text").GetComponent<Text>();
                supperSkillLock = transSupperSkill.Find("Image_Lock").gameObject;
                supperSkillSelect = transSupperSkill.Find("Select").gameObject;
                supperSkillRecommend = transSupperSkill.Find("Image_Tuijian").gameObject;
                supperSkillMask= transSupperSkill.Find("Mask").GetComponent<Image>();
                btnSupperSkill = transSupperSkill.Find("Image_Bottom").GetComponent<Button>();


                btnPet.onClick.AddListener(PetOnClick);
                btnSupperSkill.onClick.AddListener(SupperSkillClick);
            }
            public void SetData(SkillColumnDeploy data, Action<uint, SkillColumnDeploy, SkillColumnDeploy.FirstSkillCell> action)
            {
                this.data = data;
                this.action = action;

                SetPetData();
                SetSupperSkillData();
                SetFirstSkillIData();
                SetSelect();
            }
            private void SetPetData()
            {
                if (data.petUid != 0)
                {
                    lockObj.SetActive(false);
                    addObj.SetActive(false);
                    petIcon.gameObject.SetActive(true);
                    ClientPet clientPet = Sys_ActivityTrialGate.Instance.GetClientPet(data.petUid);
                    ImageHelper.SetIcon(petIcon, clientPet.petData.icon_id);
                }
                else
                {
                    if (data.CheckBarIsActivate())
                    {
                        lockObj.SetActive(false);
                        addObj.SetActive(true);
                        petIcon.gameObject.SetActive(false);
                    }
                    else
                    {
                        lockObj.SetActive(true);
                        addObj.SetActive(false);
                        petIcon.gameObject.SetActive(false);
                    }
                }
                textPetRecommend.text = LanguageHelper.GetTextContent(data.csv_trialSkillBar.pet_type);
            }
            private void SetSupperSkillData()
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data.superSkill.skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(supperSkillIcon, skillInfo.icon);
                    supperSkillName.text = LanguageHelper.GetTextContent(skillInfo.name);
                }
                supperSkillRecommend.SetActive(data.CheckSuperSkillIsRecommend());
                if (data.CheckSuperSkillIsActivate())
                {
                    supperSkillLock.SetActive(false);
                    supperSkillMask.gameObject.SetActive(false);
                }
                else
                {
                    if (data.GetFirstSkillActivateCount() <= 0)
                    {
                        supperSkillLock.SetActive(true);
                        supperSkillMask.gameObject.SetActive(false);
                    }
                    else
                    {
                        supperSkillLock.SetActive(false);
                        supperSkillMask.gameObject.SetActive(true);
                        supperSkillMask.fillAmount = data.GetFirstSkillActivateRatio();
                    }
                }
            }
            private void SetFirstSkillIData()
            {
                for (int i = 0; i < firstSkillItemList.Count; i++)
                {
                    TrialFirstSkillItem cell = firstSkillItemList[i];
                    cell.ClearAllEvent();
                    PoolManager.Recycle(cell);
                }
                firstSkillItemList.Clear();
                FrameworkTool.CreateChildList(transFirstSkill, data.firstSkillCellList.Count);
                for (int i = 0; i < data.firstSkillCellList.Count; i++)
                {
                    Transform trans = transFirstSkill.GetChild(i);
                    TrialFirstSkillItem cell = PoolManager.Fetch<TrialFirstSkillItem>();
                    cell.Init(trans);
                    cell.SetData(data.firstSkillCellList[i], FirstSkillClick);
                    firstSkillItemList.Add(cell);
                }
                FrameworkTool.ForceRebuildLayout(transFirstSkill.gameObject);
            }
            private void FirstSkillClick(SkillColumnDeploy.FirstSkillCell data, bool isInvoke = false)
            {
                action?.Invoke(2, this.data, data);
                defaultShow = false;
                isClick = false;
            }
            private void PetOnClick()
            {
                if (!data.CheckBarIsActivate())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000014));
                    return;
                }
                action?.Invoke(0,data,null);
                defaultShow = false;
                isClick = false;
            }
            private void SupperSkillClick()
            {
                if(curSelectBarId == data.barId && isClick)
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000040));
                action?.Invoke(1, data,null);
                defaultShow = false;
                isClick = true;
            }
            public void Select(uint type, SkillColumnDeploy.FirstSkillCell data1)
            {
                ReleaseAllOtherSelect();
                if (type == 0)
                    petSelect.SetActive(true);
                else if (type == 1)
                {
                    supperSkillSelect.SetActive(true);
                    isClick = true;
                }
                else
                {
                    if (data1 != null)
                        FirstSkillSelect(data1.skillId);
                }
            }
            public void SetSelect()
            {
                ReleaseAllOtherSelect();
                if (data.barId == curSelectBarId)
                {
                    if (curSelectType == 0)
                        petSelect.SetActive(true);
                    else if (curSelectType == 1)
                        supperSkillSelect.SetActive(true);
                    else
                        FirstSkillSelect(curSelectSubSkillId);
                    if (defaultShow)
                        action?.Invoke(curSelectType, data, null);
                }
            }
            private void FirstSkillSelect(uint skillId)
            {
                if (firstSkillItemList != null && firstSkillItemList.Count > 0)
                {
                    for (int i = 0; i < firstSkillItemList.Count; i++)
                    {
                        if (skillId == firstSkillItemList[i].data.skillId)
                        {
                            firstSkillItemList[i].Select();
                        }
                    }
                }
            }
            public void ReleaseAllOtherSelect()
            {
                petSelect.SetActive(false);
                supperSkillSelect.SetActive(false);
                isClick = false;
                if (firstSkillItemList != null && firstSkillItemList.Count > 0)
                {
                    for (int i = 0; i < firstSkillItemList.Count; i++)
                    {
                        firstSkillItemList[i].Release();
                    }
                }
            }
        }

        #endregion
    }
}