using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace Logic
{
    public enum ETransfigurationViewType
    {
        None = 0,
        ViewLevelUp = 1,   
        ViewSkillRebuild = 2,   
        AllRaceAdd=3,
        Plan = 4,
        Max = 5,
    }

    public class UI_Transfiguration_Study : UIBase
    {
        private  Button close_Btn;
        private Transform levelUpGo;
        private Transform skillGo;
        private Transform allRaceAddGo;
        private Transform planGo;
        private CP_Toggle cP_Toggle_LevelUp;
        private CP_Toggle cP_Toggle_Skill;
        private CP_Toggle cP_Toggle_AllRaceAdd;
        private CP_Toggle cP_Toggle_Plan;
        private UI_Transfiguration_LevelUp UI_Transfiguration_LevelUp;
        private UI_Tansform_Skill_ReStudy UI_Tansform_Skill_ReStudy;
        private UI_Transfiguration_Add UI_Transfiguration_Add;
        private UI_Transfigutation_Plan UI_Transfigutation_Plan;

        private int curView;

        protected override void OnLoaded()
        {
            close_Btn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            close_Btn.onClick.AddListener(On_CloseBtn_Clicked);
            levelUpGo = transform.Find("Animator/View_LevelUp").transform;
            skillGo = transform.Find("Animator/View_SkillRebuild").transform;
            allRaceAddGo = transform.Find("Animator/View_AllBonus").transform;
            planGo = transform.Find("Animator/View_Plan").transform;

            cP_Toggle_LevelUp = transform.Find("Animator/Label_Scroll01/TabList/TabItem0").GetComponent<CP_Toggle>();
            cP_Toggle_LevelUp.onValueChanged.AddListener(OnLevelUpValueChanged);
            cP_Toggle_Skill = transform.Find("Animator/Label_Scroll01/TabList/TabItem1").GetComponent<CP_Toggle>();
            cP_Toggle_Skill.onValueChanged.AddListener(OnSkillValueChanged);
            cP_Toggle_AllRaceAdd = transform.Find("Animator/Label_Scroll01/TabList/TabItem2").GetComponent<CP_Toggle>();
            cP_Toggle_AllRaceAdd.onValueChanged.AddListener(OnAllRaceValueChanged);
            cP_Toggle_Plan = transform.Find("Animator/Label_Scroll01/TabList/TabItem3").GetComponent<CP_Toggle>();
            cP_Toggle_Plan.onValueChanged.AddListener(OnPlanValueChanged);

            UI_Transfiguration_LevelUp = new UI_Transfiguration_LevelUp();
            UI_Transfiguration_LevelUp.Init(levelUpGo);
            UI_Tansform_Skill_ReStudy = new UI_Tansform_Skill_ReStudy();
            UI_Tansform_Skill_ReStudy.Init(skillGo);
            UI_Transfiguration_Add = new UI_Transfiguration_Add();
            UI_Transfiguration_Add.Init(allRaceAddGo);
            UI_Transfigutation_Plan = new UI_Transfigutation_Plan();
            UI_Transfigutation_Plan.Init(planGo);
        }

        private void OnSkillValueChanged(bool isOn)
        {
            if (isOn)
            {
                if (curView != (int)ETransfigurationViewType.ViewSkillRebuild)
                {
                    curView = (int)ETransfigurationViewType.ViewSkillRebuild;
                    if (UI_Transfiguration_LevelUp.csvGenusData != null)
                    {
                        UI_Tansform_Skill_ReStudy.curRaceId = UI_Transfiguration_LevelUp.csvGenusData.id;
                    }
                    UI_Tansform_Skill_ReStudy.Show();
                    UI_Transfiguration_LevelUp.Hide();
                    UI_Transfigutation_Plan.Hide();
                    UI_Transfiguration_Add.Hide();
                }
            }
        }

        private void OnLevelUpValueChanged(bool isOn)
        {
            if (isOn)
            {
                curView = (int)ETransfigurationViewType.ViewLevelUp;
                UI_Tansform_Skill_ReStudy.Hide();
                UI_Transfiguration_Add.Hide();
                UI_Transfigutation_Plan.Hide();
                UI_Transfiguration_LevelUp.Show();
            }
        }

        private void OnAllRaceValueChanged(bool isOn)
        {
            if (isOn)
            {
                curView = (int)ETransfigurationViewType.AllRaceAdd;
                UI_Tansform_Skill_ReStudy.Hide();
                UI_Transfiguration_LevelUp.Hide();
                UI_Transfigutation_Plan.Hide();
                UI_Transfiguration_Add.Show();
            }
        }

        private void OnPlanValueChanged(bool isOn)
        {
            if (isOn)
            {
                curView = (int)ETransfigurationViewType.Plan;
                UI_Tansform_Skill_ReStudy.Hide();
                UI_Transfiguration_Add.Hide();
                UI_Transfiguration_LevelUp.Hide();
                UI_Transfigutation_Plan.Show();
            }
        }

        private void On_CloseBtn_Clicked()
        {
            UIManager.CloseUI(EUIID.UI_Transfiguration_Study);
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                curView = 1;
            }
            else
            {
                curView = Convert.ToInt32(arg);
            }
        }

        protected override void OnShow()
        {
            if (curView == (int)ETransfigurationViewType.ViewLevelUp)
            {
                cP_Toggle_LevelUp.SetSelected(true, true);
            }
            else
            {
                cP_Toggle_Skill.SetSelected(true, false);
                curView = (int)ETransfigurationViewType.ViewSkillRebuild;
                UI_Tansform_Skill_ReStudy.Show();
                UI_Transfiguration_LevelUp.Hide();
            }
        }

        protected override void OnHide()
        {
            UI_Transfiguration_LevelUp.Hide();
            UI_Tansform_Skill_ReStudy.Hide();
            UI_Transfiguration_Add.Hide();
        }

        protected override void OnDestroy()
        {
            UI_Tansform_Skill_ReStudy.OnDestroy();
        }
    }
}
