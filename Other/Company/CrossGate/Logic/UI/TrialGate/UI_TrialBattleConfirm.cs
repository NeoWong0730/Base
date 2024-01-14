using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Framework;

namespace Logic
{
    //试炼进入战斗确认界面
    public class UI_TrialBattleConfirm : UIBase
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
        protected override void OnUpdate()
        {
            Update();
        }
        //protected override void OnClose()
        //{
        //    if (Sys_ActivityTrialGate.Instance.curEnterBattleState != Sys_ActivityTrialGate.EEnterBattleState.Nono)
        //        Sys_ActivityTrialGate.Instance.OnChallengeVoteReq(false);
        //}
        #endregion
        #region 组件
        Button btnClose;
        Slider timeSlider;
        Text textSlider;
        Button btnConfirm, btnCancel;
        InfinityGrid infinityGrid;
        Transform readyTrans;
        Transform enterTrans;
        GameObject objTips;
        #endregion
        #region 数据
        TrialTeamCommonDetails trialTeamCommonDetails = new TrialTeamCommonDetails();
        TrialRightCommonDetails trialRightCommonDetails = new TrialRightCommonDetails();
        List<SkillColumnDeploy> skillColumnDeployList = new List<SkillColumnDeploy>();
        List<PetSkillItem> petSkillItemList = new List<PetSkillItem>();
        bool cdDirty;
        float timeCD;
        static bool defaultShow;
        static uint curSelectType;
        static uint curSelectBarId;
        uint curExpireTime;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            enterTrans = transform.Find("Animator/Scroll View");
            trialRightCommonDetails.InitSkillAndPet(transform.Find("Animator/View_Skill"),1);
            trialTeamCommonDetails.Init(enterTrans, trialRightCommonDetails, 2);
            timeSlider = transform.Find("Animator/Slider_Exp").GetComponent<Slider>();
            textSlider = transform.Find("Animator/Slider_Exp/Handle/Time/Text").GetComponent<Text>();
            transform.Find("Animator/Slider_Exp/Text_num").gameObject.SetActive(false);
            btnConfirm = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnCancel = transform.Find("Animator/Btn_02").GetComponent<Button>();

            objTips = transform.Find("Animator/Text_Tips").gameObject;
            readyTrans = transform.Find("Animator/Scroll View2");
            infinityGrid = readyTrans.GetComponent<InfinityGrid>();

            btnClose.onClick.AddListener(() => {
                Sys_ActivityTrialGate.Instance.OnChallengeVoteReq(false);
            });
            btnConfirm.onClick.AddListener(() => { VoteOnClick(true); });
            btnCancel.onClick.AddListener(() => { VoteOnClick(false); });

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshChallengeVote, OnRefreshChallengeVote, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle<uint>(Sys_ActivityTrialGate.EEvents.OnRefreshSkillColumnDeploy, OnRefreshSkillColumnDeploy, toRegister);
            Sys_ActivityTrialGate.Instance.eventEmitter.Handle(Sys_ActivityTrialGate.EEvents.OnRefreshReadyState, OnRefreshReadyState, toRegister);
            if (toRegister)
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
            else
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
        }
        private void OnRefreshChallengeVote()
        {
            bool isVote = Sys_ActivityTrialGate.Instance.CheckIsVote(Sys_Role.Instance.RoleId);
            if (isVote)//自己已投票
            {
                btnConfirm.interactable = false;
                btnConfirm.GetComponent<ButtonScaler>().enabled=false;
                InitEnterView();
            }
        }
        private void OnRefreshSkillColumnDeploy(uint type)
        {
            if (Sys_ActivityTrialGate.Instance.curEnterBattleState == Sys_ActivityTrialGate.EEnterBattleState.Ready)
            {
                type = type == 2 ? 1 : type;
                RefreshPetSkillItem();
                trialRightCommonDetails.RefreshRighShowDefault(type);
            }
        }
        private void VoteOnClick(bool isAgree)
        {
            Sys_ActivityTrialGate.Instance.OnChallengeVoteReq(isAgree);
        }
        /// <summary>
        /// 备战时间结束，刷新备战状态
        /// </summary>
        private void OnRefreshReadyState()
        {
            bool isVote = Sys_ActivityTrialGate.Instance.CheckIsVote(Sys_Role.Instance.RoleId);
            //备战期间已投票，刷新组队界面
            if (isVote)
                trialTeamCommonDetails.RefreshTrialTeamData();
            else//未投票，强制变为已准备状态，并初始化刷新组队界面
            {
                btnConfirm.interactable = false;
                btnConfirm.GetComponent<ButtonScaler>().enabled = false;
                InitEnterView();
            }
            SetSlider();
        }
        private void OnEnterBattle(CSVBattleType.Data cSVBattleTypeTb)
        {
            if (Sys_ActivityTrialGate.Instance.curEnterBattleState != Sys_ActivityTrialGate.EEnterBattleState.Nono)
            {
                Sys_ActivityTrialGate.Instance.curEnterBattleState = Sys_ActivityTrialGate.EEnterBattleState.Nono;
                CloseSelf();
                if (UIManager.IsVisibleAndOpen(EUIID.UI_TrialGateMain))
                    UIManager.CloseUI(EUIID.UI_TrialGateMain);
            }
        }
        #endregion
        private void InitView()
        {
            SetSlider();
            InitReadyView();
        }
        private void Update()
        {
            if (cdDirty)
            {
                timeCD -= deltaTime;
                if (curExpireTime > TimeManager.GetServerTime())
                {
                    RefreshSlider(timeCD);
                }
                else
                {
                    cdDirty = false;
                }
            }
        }
        #region slider
        private void SetSlider()
        {
            if (Sys_ActivityTrialGate.Instance.curEnterBattleState == Sys_ActivityTrialGate.EEnterBattleState.Confirm)
            {
                curExpireTime = Sys_ActivityTrialGate.Instance.confirmExpireTime;
                timeCD = (int)(curExpireTime - Sys_ActivityTrialGate.Instance.confirmStartTime);
            }
            else if(Sys_ActivityTrialGate.Instance.curEnterBattleState == Sys_ActivityTrialGate.EEnterBattleState.Ready)
            {
                curExpireTime = Sys_ActivityTrialGate.Instance.readyExpireTime;
                timeCD = (int)(curExpireTime - Sys_ActivityTrialGate.Instance.readyStartTime);
            }
            timeSlider.minValue = 0;
            timeSlider.maxValue = timeCD;
            RefreshSlider(timeCD);
            cdDirty = true;
        }
        private void RefreshSlider(float value)
        {
            timeSlider.value = timeSlider.maxValue - value;
            textSlider.text = ((int)value).ToString(); //LanguageHelper.GetTextContent(1, value.ToString());//TODO
        }
        #endregion
        #region  进战确认
        private void InitEnterView()
        {
            objTips.SetActive(false);
            readyTrans.gameObject.SetActive(false);
            enterTrans.gameObject.SetActive(true);
            trialRightCommonDetails.RefreshEmptyPos(2);
            trialTeamCommonDetails.InitData(1);
            trialTeamCommonDetails.RefreshTrialTeamData();
        }
        #endregion
        #region 备战
        private void InitReadyView()
        {
            objTips.SetActive(true);
            readyTrans.gameObject.SetActive(true);
            enterTrans.gameObject.SetActive(false);
            trialRightCommonDetails.RefreshEmptyPos(1);
            btnConfirm.interactable = true;
            btnConfirm.GetComponent<ButtonScaler>().enabled = true;
            defaultShow = true;
            skillColumnDeployList = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployList(1);
            curSelectType = 1;
            if (skillColumnDeployList.Count > 0)
                curSelectBarId = skillColumnDeployList[0].barId;
            RefreshPetSkillItem();
        }
        private void RefreshPetSkillItem()
        {
            skillColumnDeployList = Sys_ActivityTrialGate.Instance.GetSkillColumnDeployList(1);
            infinityGrid.CellCount = skillColumnDeployList.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillItem entry = new PetSkillItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
            petSkillItemList.Add(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PetSkillItem entry = cell.mUserData as PetSkillItem;
            entry.SetData(skillColumnDeployList[index], RefreshRightShow);//索引数据
        }
        private void RefreshRightShow(uint type, SkillColumnDeploy data)
        {
            curSelectType = type;
            curSelectBarId = data.barId;
            if (petSkillItemList != null && petSkillItemList.Count > 0)
            {
                for (int i = 0; i < petSkillItemList.Count; i++)
                {
                    if (data.barId == petSkillItemList[i].data.barId)
                    {
                        petSkillItemList[i].Select(type);
                    }
                    else
                    {
                        petSkillItemList[i].Release();
                    }
                }
            }

            trialRightCommonDetails.RefreshRightShow(type, data);
        }
        #endregion
        #region item
        public class PetSkillItem
        {
            Button btnSkill;
            Image skillIcon;
            Image mask;
            GameObject objSkillSelect;
            GameObject objSkillLock;

            Button btnPet,btnPetAdd;
            Image petIcon;
            GameObject objPetSelect;
            GameObject objAdd;
            GameObject objPetLock;

            public SkillColumnDeploy data;
            Action<uint, SkillColumnDeploy> action;
            public void Init(Transform trans)
            {
                btnSkill = trans.Find("Skill/Icon").GetComponent<Button>();
                skillIcon = trans.Find("Skill/Icon").GetComponent<Image>();
                mask = trans.Find("Skill/Mask").GetComponent<Image>();
                objSkillSelect = trans.Find("Skill/Select").gameObject;
                objSkillLock = trans.Find("Skill/Lock").gameObject;

                btnPet = trans.Find("Pet/Icon").GetComponent<Button>();
                petIcon = trans.Find("Pet/Icon").GetComponent<Image>();
                objPetSelect = trans.Find("Pet/Select").gameObject;
                objAdd = trans.Find("Pet/Button_Add").gameObject;
                btnPetAdd = trans.GetNeedComponent<Button>();
                objPetLock = trans.Find("Pet/Lock").gameObject;

                btnSkill.onClick.AddListener(SkillOnClick);
                btnPet.onClick.AddListener(PetOnClick);
                btnPetAdd.onClick.AddListener(PetOnClick);
            }
            public void SetData(SkillColumnDeploy data, Action<uint, SkillColumnDeploy> action)
            {
                this.data = data;
                this.action = action;
                SetSkillData();
                SetPetData();
                SetSelect();
            }
            public void SetSkillData()
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(data.superSkill.skillId);
                if (skillInfo != null)
                    ImageHelper.SetIcon(skillIcon, skillInfo.icon);
                mask.fillAmount = data.GetFirstSkillActivateRatio();
                if (data.CheckSuperSkillIsActivate())
                {
                    objSkillLock.SetActive(false);
                    mask.gameObject.SetActive(false);
                }
                else
                {
                    if (data.GetFirstSkillActivateCount() <= 0)
                    {
                        objSkillLock.SetActive(true);
                        mask.gameObject.SetActive(false);
                    }
                    else
                    {
                        objSkillLock.SetActive(false);
                        mask.gameObject.SetActive(true);
                        mask.fillAmount = data.GetFirstSkillActivateRatio();
                    }
                }
            }
            public void SetPetData()
            {
                objPetLock.SetActive(false);
                if (data.petUid != 0)
                {
                    petIcon.gameObject.SetActive(true);
                    objAdd.SetActive(false);
                    ClientPet clientPet = Sys_ActivityTrialGate.Instance.GetClientPet(data.petUid);
                    ImageHelper.SetIcon(petIcon, clientPet.petData.icon_id);
                }
                else
                {
                    objAdd.SetActive(true);
                    petIcon.gameObject.SetActive(false);
                }
            }
            private void SkillOnClick()
            {
                defaultShow = false;
                action?.Invoke(1, data);
            }
            private void PetOnClick()
            {
                defaultShow = false;
                action?.Invoke(0, data);
            }
            private void SetSelect()
            {
                Release();
                if (data.barId == curSelectBarId)
                {
                    if (curSelectType == 0)
                        objPetSelect.SetActive(true);
                    else if (curSelectType == 1)
                        objSkillSelect.SetActive(true);
                    if (defaultShow)
                        action?.Invoke(curSelectType, data);
                }
            }
            public void Select(uint type)
            {
                if (type == 0)
                {
                    objPetSelect.SetActive(true);
                    objSkillSelect.SetActive(false);
                }
                else
                {
                    objPetSelect.SetActive(false);
                    objSkillSelect.SetActive(true);
                }
            }
            public void Release()
            {
                objPetSelect.SetActive(false);
                objSkillSelect.SetActive(false);
            }
        }
        #endregion
    }
}