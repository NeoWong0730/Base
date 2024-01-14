using System;
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
    public class PetDomesticateTaskParam
    {
        public DomesticationTask taskData;
    }
    public class UI_PetDomesticateTask : UIBase
    {
        private Button btnClose;

        private InfinityGrid infinity;
        private GameObject goNoPet;
        private Text txtTaskName;
        private Text txtUseTime;    //驯养时长
        private Text txtCondition;  //驯养条件
        private Image imgRaceIcon;
        private Text txtRaceName;
        private Image imgSkillIcon;
        private Text txtSkillName;
        private Button btnDomesticate;  //驯养按钮
        private Text txtScoreAdd;       //评分加成
        private Text txtRaceAdd;        //种族加成
        private Text txtSkillAdd;       //技能加成
        private Text txtAllAdd;       //总加成
        private GameObject goAddUpArrow;    //加成上箭头
        private GameObject goAddDownArrow;  //加成下箭头

        private DomesticationTask taskData;
        private List<ClientPet> listPet;
        private List<UI_PetDomesticatePetCell> listCell = new List<UI_PetDomesticatePetCell>();
        /// <summary>
        /// 选中的宠物Uid
        /// </summary>
        private uint selectedPetUid;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if(arg!=null && arg.GetType() == typeof(PetDomesticateTaskParam))
            {
                PetDomesticateTaskParam param = arg as PetDomesticateTaskParam;
                taskData = param.taskData;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            UpdateView();
        }
        protected override void OnHide()
        {
        }
        protected override void OnDestroy()
        {
            for (int i = 0; i < listCell.Count; i++)
            {
                listCell[i].Destroy();
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        }
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            goNoPet = transform.Find("Animator/Text_Tips").gameObject;
            txtTaskName = transform.Find("Animator/Image_Frame/Text_Title").GetComponent<Text>();
            txtUseTime = transform.Find("Animator/Image_Frame/Text_Time").GetComponent<Text>();
            txtCondition = transform.Find("Animator/Image_Frame/Text_Condition/Text_Value").GetComponent<Text>();
            imgRaceIcon = transform.Find("Animator/Image_Frame/Text_Addition/Race/Image_Icon").GetComponent<Image>();
            imgSkillIcon = transform.Find("Animator/Image_Frame/Text_Addition1/Skill/Image_Icon").GetComponent<Image>();
            txtRaceName = transform.Find("Animator/Image_Frame/Text_Addition/Text_Value").GetComponent<Text>();
            txtSkillName = transform.Find("Animator/Image_Frame/Text_Addition1/Text_Value").GetComponent<Text>();
            txtScoreAdd = transform.Find("Animator/Addition_1/Text_Value").GetComponent<Text>();
            txtRaceAdd = transform.Find("Animator/Addition_2/Text_Value").GetComponent<Text>();
            txtSkillAdd = transform.Find("Animator/Addition_3/Text_Value").GetComponent<Text>();
            txtAllAdd = transform.Find("Animator/Addition_4/Text_Value").GetComponent<Text>();
            btnDomesticate = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnDomesticate.onClick.AddListener(OnBtnDomesticateClick);
            infinity = transform.Find("Animator/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;
            goAddUpArrow = transform.Find("Animator/Addition_4/Text_Value/Image_ArrowUp").gameObject;
            goAddDownArrow = transform.Find("Animator/Addition_4/Text_Value/Image_ArrowDown").gameObject;
        }
        private void UpdateView()
        {
            listPet = Sys_PetDomesticate.Instance.GetValidPetList(taskData);
            if (selectedPetUid == 0 && listPet.Count > 0)
            {
                selectedPetUid = listPet[0].GetPetUid();
            }
            infinity.CellCount = listPet.Count;
            infinity.ForceRefreshActiveCell();
            goNoPet.SetActive(listPet.Count <= 0);
            var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
            if (csvTask != null)
            {
                int gradeindex = (int)taskData.Grade - 1;
                txtTaskName.text = LanguageHelper.GetTextContent((uint)csvTask.name);
                txtUseTime.text = Sys_PetDomesticate.Instance.GetTaskAllTimeText(csvTask.duration[gradeindex]);
                txtCondition.text = Sys_PetDomesticate.Instance.GetConditionText(csvTask.type, csvTask.condition[gradeindex]);
                CSVGenus.Data csvGenus = CSVGenus.Instance.GetConfData((uint)csvTask.race);
                ImageHelper.SetIcon(imgRaceIcon, csvGenus.rale_icon);
                txtRaceName.text = LanguageHelper.GetTextContent(csvGenus.rale_name);
                CSVPassiveSkillInfo.Data csvSkill = CSVPassiveSkillInfo.Instance.GetConfData(taskData.LuckySkill);
                ImageHelper.SetIcon(imgSkillIcon, csvSkill.icon);
                txtSkillName.text = LanguageHelper.GetTextContent(csvSkill.name);

                uint scoreAdd = selectedPetUid > 0 ? Sys_PetDomesticate.Instance.GetScoreAddition(taskData, selectedPetUid) : 0;
                txtScoreAdd.text = LanguageHelper.GetTextContent(10882, scoreAdd.ToString());//+{0}%
                uint raceAdd = selectedPetUid > 0 ? Sys_PetDomesticate.Instance.GetRaceAddition(taskData, selectedPetUid) : 0;
                txtRaceAdd.text = LanguageHelper.GetTextContent(10882, raceAdd.ToString());//+{0}%
                uint skillAdd = selectedPetUid > 0 ? Sys_PetDomesticate.Instance.GetSkillAddition(taskData, selectedPetUid) : 0;
                txtSkillAdd.text = LanguageHelper.GetTextContent(10882, skillAdd.ToString());//+{0}%
                uint allValue = scoreAdd + raceAdd + skillAdd;
                uint worldStyleId = allValue > 100 ? 170u : allValue == 100 ? 169u : 171u;
                TextHelper.SetText(txtAllAdd, LanguageHelper.GetTextContent(10882, allValue.ToString()), CSVWordStyle.Instance.GetConfData(worldStyleId));
                goAddUpArrow.SetActive(allValue > 100);
                goAddDownArrow.SetActive(allValue < 100);
            }
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        private void OnBtnDomesticateClick()
        {
            if (selectedPetUid != 0)
            {
                Sys_PetDomesticate.Instance.ReqStartPetDomestication(taskData.InfoId, selectedPetUid);
                this.CloseSelf();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PetDomesticatePetCell mCell = new UI_PetDomesticatePetCell();
            mCell.Init(cell.mRootTransform.transform);
            mCell.RegisterEvent(OnCellSelected);
            cell.BindUserData(mCell);
            listCell.Add(mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PetDomesticatePetCell mCell = cell.mUserData as UI_PetDomesticatePetCell;
            mCell.UpdateCellView(listPet[index], selectedPetUid);
        }

        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            UpdateView();
        }
        private void OnCellSelected(uint petUid)
        {
            selectedPetUid = petUid;
            UpdateView();
        }
        #endregion
        #region class
        public class UI_PetDomesticatePetCell
        {
            private Transform transform;

            private Button btnSelf;
            private GameObject goSelected;
            private Image imgPetIcon;
            private Text txtRace;
            private Text txtName;
            private Text txtScore;

            private uint petUid;
            private Action<uint> actSelfClick;
            public void Init(Transform _trans)
            {
                transform = _trans;
                btnSelf = transform.GetComponent<Button>();
                btnSelf.onClick.AddListener(OnBtnSelfClick);
                goSelected = transform.Find("Image_Select").gameObject;
                imgPetIcon = transform.Find("Image_Head/Image_Icon").GetComponent<Image>();
                txtRace = transform.Find("Image_Race/Text").GetComponent<Text>();
                txtName = transform.Find("Text_Name").GetComponent<Text>();
                txtScore = transform.Find("Image_Score/Text").GetComponent<Text>();
            }

            public void UpdateCellView(ClientPet clientPet,uint selectedUid)
            {
                petUid = clientPet.GetPetUid();
                ImageHelper.SetIcon(imgPetIcon, clientPet.petData.icon_id);
                CSVGenus.Data csvGenus = CSVGenus.Instance.GetConfData(clientPet.petData.race);
                txtRace.text = LanguageHelper.GetTextContent(csvGenus.rale_name);
                txtName.text = Sys_Pet.Instance.GetPetName(clientPet);
                txtScore.text = LanguageHelper.GetTextContent(2052102, clientPet.petUnit.SimpleInfo.Score.ToString());//评分：{0}
                goSelected.SetActive(petUid == selectedUid);
            }
            private void OnBtnSelfClick()
            {
                actSelfClick?.Invoke(petUid);
            }
            public void RegisterEvent(Action<uint> act)
            {
                actSelfClick = act;
            }

            public void Destroy()
            {
                actSelfClick = null;
            }
        }
        #endregion
    }
}
