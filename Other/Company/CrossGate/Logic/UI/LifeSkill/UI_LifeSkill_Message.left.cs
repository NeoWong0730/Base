using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace Logic
{

    public partial class UI_LifeSkill_Message : UIBase
    {
        private Transform m_Content1;
        private Transform m_Content2;
        //private Button levelUpBtn;
        //private Button describeBtn;
        //private GameObject fx_skillGradeUp;
        //private GameObject fx_skillCanGradeUp;
        private Button m_GoToShopButton;
        //private Text curGrade;
        //private Text nextGrade;
        private Image m_CostIcon;
        private Text m_Cost;
        private Button m_CostButton;
        private Text m_CanLearn;
        private Button m_CanLearnButton;
        private List<GameObject> m_LifeSkills = new List<GameObject>();
        //private Timer m_GradeTimer;

        private CP_ToggleRegistry m_CP_ToggleRegisterLeft;
        private UI_CurrencyTitle m_UI_CurrencyTitle;
        private LifeSkillOpenParm m_LifeSkillOpenParm;
        private uint m_CurSkillId;
        private uint m_ItemId;

        private void ParceLeftCp()
        {
            m_CP_ToggleRegisterLeft = transform.Find("Animator/BG/Image_BG").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegisterLeft.onToggleChange = OnToggleLeftValueChanged;

            m_Content1 = transform.Find("Animator/BG/Image_BG/Content");
            m_Content2 = transform.Find("Animator/BG/Image_BG/Content2");
            m_CostButton = transform.Find("Animator/View_Message/Image_Property01/Button_Add").GetComponent<Button>();
            m_CanLearn = transform.Find("Animator/BG/Image_BG/Learn_Num/Text_Number").GetComponent<Text>();
            m_CanLearnButton = transform.Find("Animator/BG/Image_BG/Learn_Num/Button_Add").GetComponent<Button>();
            //describeBtn = transform.Find("Animator/View_Message/Image_Title/Btn_Detail").GetComponent<Button>();
            m_GoToShopButton = transform.Find("Animator/View_Message/Image_Title/Btn_Shop").GetComponent<Button>();
            //fx_skillGradeUp = transform.Find("Animator/View_Message/Image_Title/Btn_Up/Fx_ui_fire_life").gameObject;
            //fx_skillCanGradeUp = transform.Find("Animator/View_Message/Image_Title/Btn_Up/Fx_ui_lifeskill_leveup_Loop").gameObject;
            //levelUpBtn = transform.Find("Animator/View_Message/Image_Title/Btn_Up").GetComponent<Button>();
            //nextGrade = transform.Find("Animator/View_Message/Image_Title/Btn_Up/Text").GetComponent<Text>();
            //curGrade = transform.Find("Animator/View_Message/Image_Title/Text/Text_Num").GetComponent<Text>();

            m_UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            for (int i = 0; i < 6; i++)
            {
                m_LifeSkills.Add(m_Content2.GetChild(i).gameObject);
            }

            for (int i = 0; i < 3; i++)
            {
                m_LifeSkills.Add(m_Content1.GetChild(i).gameObject);
            }
            //levelUpBtn.onClick.AddListener(OnGradeUpButtonClicked);
            //describeBtn.onClick.AddListener(OnDescribeButtonClicked);
            m_GoToShopButton.onClick.AddListener(OnGoToShopButtonClicked);
            m_CostButton.onClick.AddListener(OnCostButtonClicked);
            m_CanLearnButton.onClick.AddListener(OnCanLearnButtonClicked);
        }

        private void OnToggleLeftValueChanged(int curToggle, int oldToggle)
        {
            if (curToggle == oldToggle)
            {
                return;
            }
            m_CurSkillId = (uint)curToggle;
            Sys_LivingSkill.Instance.bIntensify = false;
            UpdateSkillData();
            RefreshView();
            CancelMakeAnim();
        }

        private void OnRefreshChangeData(int a, int b)
        {
            UpdateCost();

            if (cSVFormulaData != null)
            {
                if (cSVFormulaData.forge_type == 1)
                {
                    int needForgeNum = (int)cSVFormulaData.normal_forge.Count;
                    SetCostRoot(needForgeNum, true, cSVFormulaData.can_intensify, cSVFormulaData.can_harden, true, true);
                }
                else if (cSVFormulaData.forge_type == 2)
                {
                    int needForgeNum = (int)cSVFormulaData.forge_num;
                    SetCostRoot(needForgeNum, false, false, false, false);
                }
            }
        }

        private void UpdateCost()
        {
            ImageHelper.SetIcon(m_CostIcon, CSVItem.Instance.GetConfData(5).icon_id);
            TextHelper.SetText(m_Cost, string.Format("{0}/{1}", Sys_Bag.Instance.GetItemCount(5), Sys_Vitality.Instance.GetMaxVitality()));
        }

        /// <summary>
        /// 学习状态
        /// </summary>
        private void UpdateLearnState()
        {
            for (int i = 1; i < 10; i++)
            {
                uint skillId = (uint)i;
                OnUpdateLeftLevelUp(skillId);
            }
        }

        private void OnUpdateLeftLevelUp(uint skillid)
        {
            int childIndex = (int)skillid - 1;
            GameObject gameObject = m_LifeSkills[childIndex];
            LivingSkill livingSkill = Sys_LivingSkill.Instance.livingSkills[skillid];

            GameObject learn = gameObject.transform.Find("Image_Learn").gameObject;
            GameObject unlearn = gameObject.transform.Find("Image_UnLearn").gameObject;
            Image skillIcon = gameObject.transform.Find("Image_Icon").GetComponent<Image>();
            Text name = gameObject.transform.Find("Text").GetComponent<Text>();
            Text text_Light = gameObject.transform.Find("Text_Light").GetComponent<Text>();
            ImageHelper.SetIcon(skillIcon, livingSkill.cSVLifeSkillData.icon_id);

            if (livingSkill.Unlock)
            {
                unlearn.SetActive(false);
                learn.SetActive(true);
                TextHelper.SetText(name, 2010143, livingSkill.name, livingSkill.Level.ToString());
                TextHelper.SetText(text_Light, 2010143, livingSkill.name, livingSkill.Level.ToString());
            }
            else
            {
                learn.SetActive(false);
                unlearn.SetActive(true);
                TextHelper.SetText(name, livingSkill.name);
                TextHelper.SetText(text_Light, livingSkill.name);
            }
        }

        private void OnGoToShopButtonClicked()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.itemId = 0;
            mallPrama.mallId = 601;
            mallPrama.shopId = 1005;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }


        #region Grade

        //private void OnDescribeButtonClicked()
        //{
        //    CSVLifeSkillRank.Data cSVLifeSkillRankData = CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade);
        //    UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(cSVLifeSkillRankData.desc_id) });
        //}

        //private void OnGradeUpButtonClicked()
        //{
        //    CSVLifeSkillRank.Data cSVLifeSkillRankData = CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade);
        //    if (Sys_Role.Instance.Role.Level < cSVLifeSkillRankData.role_level)
        //    {
        //        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010137, cSVLifeSkillRankData.role_level.ToString()));
        //        return;
        //    }

        //    PromptItemData promptItemData = new PromptItemData();
        //    uint npcId = cSVLifeSkillRankData.npc_id;
        //    uint taskId = cSVLifeSkillRankData.task_id;
        //    if (taskId == 0)
        //    {
        //        List<uint> needitems = new List<uint>();
        //        List<uint> needcounts = new List<uint>();
        //        if (cSVLifeSkillRankData.cost_item == null)
        //        {
        //            //最高等级
        //            return;
        //        }
        //        foreach (var item in cSVLifeSkillRankData.cost_item)
        //        {
        //            needitems.Add(item[0]);
        //            needcounts.Add(item[1]);
        //        }
        //        promptItemData.onConfire = () =>
        //        {
        //            Sys_LivingSkill.Instance.SegmentLevelUpReq();
        //            UIManager.CloseUI(EUIID.UI_PromptItemBox);
        //        };
        //        string content = CSVLanguage.Instance.GetConfData(2010030).words;
        //        promptItemData.notEnough = CSVLanguage.Instance.GetConfData(2010104).words;   //需要设置道具不足的语言表
        //        promptItemData.content = string.Format(content, Sys_LivingSkill.Instance.Grade + 1);    //需要设置content语言表
        //        promptItemData.needCount = needcounts;
        //        promptItemData.items = needitems;
        //        promptItemData.titleId = 2010136u;
        //        promptItemData.type = 0;
        //        UIManager.OpenUI(EUIID.UI_PromptItemBox, false, promptItemData);
        //    }

        //    #region TaskFun
        //    else
        //    {
        //        if (Sys_Task.Instance.GetTaskState(taskId) == ETaskState.Finished || Sys_Task.Instance.GetTaskState(taskId) == ETaskState.Submited)
        //        {
        //            List<uint> needitems = new List<uint>();
        //            List<uint> needcounts = new List<uint>();
        //            if (cSVLifeSkillRankData.cost_item == null)
        //            {
        //                //最高等级
        //                return;
        //            }
        //            foreach (var item in cSVLifeSkillRankData.cost_item)
        //            {
        //                needitems.Add(item[0]);
        //                needcounts.Add(item[1]);
        //            }
        //            promptItemData.onConfire = () =>
        //            {
        //                Sys_LivingSkill.Instance.SegmentLevelUpReq();
        //                UIManager.CloseUI(EUIID.UI_PromptItemBox);
        //            };
        //            string content = CSVLanguage.Instance.GetConfData(2010030).words;
        //            promptItemData.notEnough = CSVLanguage.Instance.GetConfData(2010104).words;   //需要设置道具不足的语言表
        //            promptItemData.content = string.Format(content, Sys_LivingSkill.Instance.Grade + 1);    //需要设置content语言表
        //            promptItemData.needCount = needcounts;
        //            promptItemData.items = needitems;
        //            promptItemData.titleId = 2010136u;
        //            promptItemData.type = 0;
        //            UIManager.OpenUI(EUIID.UI_PromptItemBox, false, promptItemData);
        //        }
        //        else
        //        {
        //            if (taskId == 0)
        //            {
        //                DebugUtil.LogErrorFormat("配置任务为0");
        //            }
        //            else
        //            {
        //                PromptBoxParameter.Instance.Clear();
        //                string content = CSVLanguage.Instance.GetConfData(2010103).words;
        //                PromptBoxParameter.Instance.content = string.Format(content, CSVLanguage.Instance.GetConfData(CSVNpc.Instance.GetConfData(npcId).name).words);
        //                PromptBoxParameter.Instance.SetConfirm(true, () =>
        //                {
        //                    UIManager.CloseUI(EUIID.UI_PromptBox);
        //                    //UIManager.CloseUI(EUIID.UI_LifeSkill);
        //                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);
        //                });
        //                PromptBoxParameter.Instance.SetCancel(true, null);
        //                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        //            }
        //        }
        //    }


        //    #endregion
        //}

        //private void OnUpdateGrade()
        //{
        //    curGrade.text = Sys_LivingSkill.Instance.Grade.ToString();
        //    CSVLifeSkillRank.Data cSVLifeSkillLevelData = CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade);
        //    bool can = Sys_LivingSkill.Instance.CanGradeLevelUp && cSVLifeSkillLevelData.npc_id == 0;
        //    bool fx_can = true;
        //    CSVLifeSkillRank.Data cSVLifeSkillRankData = CSVLifeSkillRank.Instance.GetConfData(Sys_LivingSkill.Instance.Grade);
        //    if (Sys_Role.Instance.Role.Level < cSVLifeSkillRankData.role_level)
        //    {
        //        fx_can = false;
        //    }
        //    List<uint> needitems = new List<uint>();
        //    List<uint> needcounts = new List<uint>();
        //    foreach (var item in cSVLifeSkillRankData.cost_item)
        //    {
        //        needitems.Add(item[0]);
        //        needcounts.Add(item[1]);
        //    }
        //    for (int i = 0; i < needitems.Count; i++)
        //    {
        //        uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(needitems[i]);
        //        if (itemCount < needcounts[i])
        //        {
        //            fx_can = false;
        //            continue;
        //        }
        //    }
        //    levelUpBtn.gameObject.SetActive(can);
        //    fx_skillCanGradeUp.SetActive(can & fx_can);
        //    if (can)
        //    {
        //        TextHelper.SetText(nextGrade, LanguageHelper.GetTextContent(2010156, (Sys_LivingSkill.Instance.Grade + 1).ToString()));
        //    }
        //    //ButtonHelper.Enable(levelUpBtn, can);
        //}

        //private void OnPlayGradeUpFx()
        //{
        //    m_GradeTimer?.Cancel();
        //    fx_skillGradeUp.SetActive(true);
        //    m_GradeTimer = Timer.Register(5, () =>
        //    {
        //        fx_skillGradeUp.SetActive(false);
        //    });
        //}

        #endregion

        private void OnCostButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Vitality);
        }

        private void OnCanLearnButtonClicked()
        {
            PromptItemData promptItemData = new PromptItemData();
            List<uint> needitems = new List<uint>();
            List<uint> needcounts = new List<uint>();
            uint level = Sys_LivingSkill.Instance.canLearnNum > 0 ? Sys_LivingSkill.Instance.canLearnNum : 0;
            needitems.Add((uint)Sys_LivingSkill.Instance.cost[(int)level][0]);
            needcounts.Add((uint)Sys_LivingSkill.Instance.cost[(int)level][1]);
            promptItemData.onConfire = () =>
            {
                Sys_LivingSkill.Instance.LifeSkillCanLearnNumAddReq();
                UIManager.CloseUI(EUIID.UI_PromptItemBox);
            };
            string content = CSVLanguage.Instance.GetConfData(2010149).words;
            promptItemData.notEnough = CSVLanguage.Instance.GetConfData(2010154).words;   //需要设置道具不足的语言表
            promptItemData.content = string.Format(content, Sys_LivingSkill.Instance.canLearnNum + Sys_LivingSkill.Instance.freeLearnNum + 1);    //需要设置content语言表
            promptItemData.needCount = needcounts;
            promptItemData.items = needitems;
            promptItemData.titleId = 2010147u;
            promptItemData.type = 2;
            UIManager.OpenUI(EUIID.UI_PromptItemBox, false, promptItemData);
        }

        private void OnUpdateCanLearn()
        {
            uint num = Sys_LivingSkill.Instance.freeLearnNum + Sys_LivingSkill.Instance.canLearnNum;
            TextHelper.SetText(m_CanLearn, num.ToString());
            ButtonHelper.Enable(m_CanLearnButton, num < 6);
        }

        private void OnForgetSkill()
        {
            UpdateSkillData();
            RefreshView();
            CancelMakeAnim();
        }

        private void OnLearnedSkill()
        {
            UpdateSkillData();
            RefreshView();
        }
    }
}

