using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    /// <summary>
    /// 队伍列表详情
    /// </summary>
    public class TrialTeamCommonDetails
    {
        TrialRightCommonDetails trialRightCommonDetails;
        InfinityGrid infinityGrid;
        List<TrialTeamDataItem> trialTeamDataItemList = new List<TrialTeamDataItem>();
        List<TrialTeamRoleData> trialTeamRoleDataList = new List<TrialTeamRoleData>();
        uint type;
        public static ulong curSelectRoleId;
        public static uint curSelectBarId;
        public static bool defaultShow;
        uint viewType;
        int curRoleIndex;
        /// <param name="type">type=1(队伍配置)  type=2(进战确认)</param>
        public void Init(Transform trans, TrialRightCommonDetails trialRightCommonDetails,uint type)
        {
            this.trialRightCommonDetails = trialRightCommonDetails;
            this.type = type;
            infinityGrid = trans.GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        /// <summary>
        /// 队伍成员技能装配初始化
        /// </summary>
        /// <param name="viewType">0队伍配置，1进战</param>
        public void InitData(uint viewType=0)
        {
            this.viewType = viewType;
            defaultShow = true;
            trialTeamRoleDataList = Sys_ActivityTrialGate.Instance.GetTrialTeamRoleDataList();
            if (trialTeamRoleDataList != null && trialTeamRoleDataList.Count > 0)
            {
                if (viewType == 1)
                {
                    for (int i = 0; i < trialTeamRoleDataList.Count; i++)
                    {
                        if (trialTeamRoleDataList[i].roleId == Sys_Role.Instance.RoleId)
                        {
                            curSelectRoleId = trialTeamRoleDataList[i].roleId;
                            curRoleIndex = i;
                            if (trialTeamRoleDataList[i].barNodeList.Count > 0)
                            {
                                curSelectBarId = trialTeamRoleDataList[i].barNodeList[0].barId;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    curSelectRoleId = trialTeamRoleDataList[0].roleId;
                    if (trialTeamRoleDataList[0].barNodeList.Count > 0)
                        curSelectBarId = trialTeamRoleDataList[0].barNodeList[0].barId;
                }
            }
            trialRightCommonDetails.RefreshRightShow(1, null,null, false);
        }
        bool isMoveTo = false;
        public void RefreshTrialTeamData()
        {
            trialTeamRoleDataList = Sys_ActivityTrialGate.Instance.GetTrialTeamRoleDataList();
            infinityGrid.CellCount = trialTeamRoleDataList.Count;
            infinityGrid.ForceRefreshActiveCell();
            if (viewType == 1)
            {
                if (!isMoveTo)
                {
                    if (curSelectRoleId == Sys_Role.Instance.RoleId && curRoleIndex == trialTeamRoleDataList.Count - 1)
                    {
                        infinityGrid.MoveToIndex(curRoleIndex);
                        isMoveTo = true;
                    }
                }
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            TrialTeamDataItem entry = new TrialTeamDataItem();
            entry.Init(cell.mRootTransform, type);
            cell.BindUserData(entry);
            trialTeamDataItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            TrialTeamDataItem entry = cell.mUserData as TrialTeamDataItem;
            entry.SetData(trialTeamRoleDataList[index], RefreshRightShow);//索引数据
        }
        private void RefreshRightShow(ulong roleId, SkillColumnDeploy skillColumnDeploy)
        {
            curSelectRoleId = roleId;
            curSelectBarId = skillColumnDeploy.barId;
            if (trialTeamDataItemList != null && trialTeamDataItemList.Count > 0)
            {
                for (int i = 0; i < trialTeamDataItemList.Count; i++)
                {
                    if (roleId != trialTeamDataItemList[i].data.roleId)
                    {
                        trialTeamDataItemList[i].ReleaseAllSelect();
                    }
                }
            }
            trialRightCommonDetails.RefreshRightShow(1, skillColumnDeploy, null, false);
        }
    }
    #region clssItem
    public class TrialTeamDataItem
    {
        Image head;
        Image careerIcon;
        Text textCareer;
        Text textName;
        Text textLevel;
        Transform roleState;

        InfinityGrid infinityGrid;
        GameObject objText_Empty;
        GameObject objScroolView;
        GameObject objWait;

        public TrialTeamRoleData data;
        Action<ulong, SkillColumnDeploy> action;
        List<SupperSkillItem> supperSkillItemList = new List<SupperSkillItem>();
        uint type;
        public void Init(Transform trans,uint type)
        {
            this.type = type;
            head = trans.Find("Head").GetComponent<Image>();
            careerIcon = trans.Find("Image_Profession").GetComponent<Image>();
            textCareer = trans.Find("Image_Profession/Text").GetComponent<Text>();
            textName = trans.Find("Text_Name").GetComponent<Text>();
            textLevel = trans.Find("Text_Lv/Text_Num").GetComponent<Text>();
            if (type == 2)
            {
                roleState = trans.Find("Image_Stage");//0(ok) 1(no) 2(wait) 3(Leave) 4(LeaveMoment)
                objWait = trans.Find("Text_Tips").gameObject;
            }
            infinityGrid = trans.Find("Scroll View").GetComponent<InfinityGrid>();
            objScroolView = trans.Find("Scroll View").gameObject;
            objText_Empty = trans.Find("Text_Empty").gameObject;

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        public void SetData(TrialTeamRoleData data, Action<ulong, SkillColumnDeploy> action)
        {
            this.data = data;
            this.action = action;
            SetData();
            if (type == 2)
            {
                bool isVote = Sys_ActivityTrialGate.Instance.CheckIsVote(data.roleId);
                //已投票
                if (isVote || Sys_ActivityTrialGate.Instance.curEnterBattleState == Sys_ActivityTrialGate.EEnterBattleState.Confirm)
                {
                    objWait.SetActive(false);
                    SetRoleState(0);
                    //有配置宠物技能
                    if (data.barNodeList.Count > 0)
                    {
                        objScroolView.SetActive(true);
                        objText_Empty.SetActive(false);
                        RefreshSupperSkill();
                    }
                    else
                    {
                        objScroolView.SetActive(false);
                        objText_Empty.SetActive(true);
                    }
                }
                else
                {
                    objWait.SetActive(true);
                    objText_Empty.SetActive(false);
                    SetRoleState(2);
                }
            }
            else
            {
                if (data.barNodeList.Count > 0)
                {
                    objScroolView.SetActive(true);
                    objText_Empty.SetActive(false);
                    RefreshSupperSkill();
                }
                else
                {
                    objScroolView.SetActive(false);
                    objText_Empty.SetActive(true);
                }
            }
        }
        private void SetData()
        {
            TeamMem team = data.teamMem;
            CharacterHelper.SetHeadAndFrameData(head, team.HeroId, team.Photo, team.PhotoFrame);
            CSVCareer.Data csvCareerData = CSVCareer.Instance.GetConfData(team.Career);
            ImageHelper.SetIcon(careerIcon, csvCareerData.icon);
            textCareer.text = LanguageHelper.GetTextContent(csvCareerData.name);
            textName.text = team.Name.ToStringUtf8();
            textLevel.text = team.Level.ToString();
        }
        private void SetRoleState(uint index)
        {
            int count = roleState.childCount;
            for (int i = 0; i < count; i++)
            {
                if(index==i)
                    roleState.GetChild(i).gameObject.SetActive(true);
                else
                    roleState.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void RefreshSupperSkill()
        {
            infinityGrid.CellCount = data.barNodeList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            SupperSkillItem entry = new SupperSkillItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            supperSkillItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SupperSkillItem entry = cell.mUserData as SupperSkillItem;
            entry.SetData(data.roleId,data.barNodeList[index], SupperSkillClick);//索引数据
        }
        private void SupperSkillClick(SkillColumnDeploy skillColumnDeploy)
        {
            if (supperSkillItemList != null && supperSkillItemList.Count > 0)
            {
                for (int i = 0; i < supperSkillItemList.Count; i++)
                {
                    if (skillColumnDeploy.barId == supperSkillItemList[i].barId)
                    {
                        supperSkillItemList[i].Select();
                    }
                    else
                    {
                        supperSkillItemList[i].Release();
                    }
                }
            }
            action?.Invoke(data.roleId, skillColumnDeploy);
        }
        private void SetSelect()
        {
            ReleaseAllSelect();
        }
        public void ReleaseAllSelect()
        {
            if (supperSkillItemList != null && supperSkillItemList.Count > 0)
            {
                for (int i = 0; i < supperSkillItemList.Count; i++)
                {
                    supperSkillItemList[i].Release();
                }
            }
        }
        public class SupperSkillItem
        {
            Button btnClick;
            Image icon;
            Image mask;
            GameObject objSelect;

            Action<SkillColumnDeploy> action;
            SkillColumnDeploy skillColumnDeploy;
            TrialTeamRoleData.BarNode barNode;
            public uint barId;
            SkillColumnDeploy curSkillColumnDeploy;
            ulong roleId;
            public void Init(Transform trans)
            {
                btnClick = trans.Find("Image_Bottom").GetComponent<Button>();
                icon = trans.Find("Icon").GetComponent<Image>();
                mask = trans.Find("Mask").GetComponent<Image>();
                objSelect = trans.Find("Select").gameObject;

                btnClick.onClick.AddListener(OnClick);
            }
            public void SetData(ulong roleId, TrialTeamRoleData.BarNode barNode, Action<SkillColumnDeploy> action)
            {
                this.roleId = roleId;
                this.barNode = barNode;
                this.action = action;
                barId = barNode.barId;
                skillColumnDeploy = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployByBarId(barNode.barId);
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillColumnDeploy.superSkill.skillId);
                if (skillInfo != null)
                    ImageHelper.SetIcon(icon, skillInfo.icon);
                mask.gameObject.SetActive(true);
                mask.fillAmount = 1 - (float)Math.Round((double)barNode.skillList.Count / skillColumnDeploy.firstSkillCellList.Count, 3);
                SetSelect();
            }
            private void OnClick()
            {
                TrialTeamCommonDetails.defaultShow = false;
                SetCurSkillColumnDeploy();
                action?.Invoke(curSkillColumnDeploy);
            }
            private void SetCurSkillColumnDeploy()
            {
                if (barNode != null)
                {
                    if (curSkillColumnDeploy == null)
                        curSkillColumnDeploy = new SkillColumnDeploy();
                    curSkillColumnDeploy.barId = barNode.barId;
                    curSkillColumnDeploy.superSkill = skillColumnDeploy.superSkill;
                    curSkillColumnDeploy.csv_trialSkillBar = skillColumnDeploy.csv_trialSkillBar;
                    curSkillColumnDeploy.firstSkillCellList.Clear();
                    for (int i = 0; i < skillColumnDeploy.firstSkillCellList.Count; i++)
                    {
                        uint skillId = barNode.skillList.Find(o => o == skillColumnDeploy.firstSkillCellList[i].skillId);
                        SkillColumnDeploy.FirstSkillCell firstSkillCell = new SkillColumnDeploy.FirstSkillCell();
                        firstSkillCell.skillId = skillColumnDeploy.firstSkillCellList[i].skillId;
                        firstSkillCell.barId = skillColumnDeploy.firstSkillCellList[i].barId;
                        firstSkillCell.csv_trialPreSkill = skillColumnDeploy.firstSkillCellList[i].csv_trialPreSkill;
                        firstSkillCell.badgeType = skillColumnDeploy.firstSkillCellList[i].badgeType;
                        if (skillId != 0)
                            firstSkillCell.activateState = true;
                        else
                            firstSkillCell.activateState = false;
                        curSkillColumnDeploy.firstSkillCellList.Add(firstSkillCell);
                    }
                }
            }
            public void SetSelect()
            {
                if (TrialTeamCommonDetails.curSelectRoleId == roleId)
                {
                    if (TrialTeamCommonDetails.curSelectBarId == barId)
                    {
                        if (TrialTeamCommonDetails.defaultShow)
                        {
                            Select();
                            SetCurSkillColumnDeploy();
                            action?.Invoke(curSkillColumnDeploy);
                        }
                    }
                }
                else
                    Release();
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
    }
    #endregion
    //试炼队伍配置界面
    public class UI_TrialTeamDeploy : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            OnOpen();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        #endregion
        #region 数据
        TrialTeamCommonDetails trialTeamCommonDetails = new TrialTeamCommonDetails();
        TrialRightCommonDetails trialRightCommonDetails = new TrialRightCommonDetails();
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            trialRightCommonDetails.InitSkill(transform.Find("Animator/View_Skill"));
            trialTeamCommonDetails.Init(transform.Find("Animator/Scroll View"), trialRightCommonDetails,1);

            btnClose.onClick.AddListener(() => { CloseSelf(); });
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshTeamDeployData, OnRefreshTeamDeployData, toRegister);

        }
        private void OnRefreshTeamDeployData()
        {
            trialTeamCommonDetails.RefreshTrialTeamData();
        }
        #endregion
        private void OnOpen()
        {
            Sys_ActivityTrialGate.Instance.OnWatchMemberConfigReq();
        }
        private void InitView()
        {
            trialTeamCommonDetails.InitData();
            trialTeamCommonDetails.RefreshTrialTeamData();
        }
    }
}