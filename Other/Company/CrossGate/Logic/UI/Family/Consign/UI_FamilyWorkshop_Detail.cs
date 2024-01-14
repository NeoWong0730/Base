using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using DG.Tweening.Core;
using DG.Tweening;
using DG.Tweening.Plugins.Options;

namespace Logic
{
    public class OpenFamilyConsignDetailParm
    {
        public int type;   //1 协助  2 求助 
        public int fromType;    //1 来源于全部页签数据 用GuildConsignInfo结构  2 来源于我的页签数据 用GuildConsignSelfInfo结构
        public GuildConsignInfo guildConsignInfo;
        public GuildConsignSelfInfo guildConsignSelfInfo;
        public string name;
    }

    public class UI_FamilyWorkshop_Detail : UIBase
    {
        private GameObject m_Part1;
        private GameObject m_Part2;
        private Button m_CloseButton;
        private Text m_ItemName;
        private Image m_ItemIcon;
        private Button m_PreView;
        private Text m_Consigner;
        private Text m_RemainTime;
        private Text m_Skill;
        private Text m_Reward1;                 //熟练度
        private Text m_Reward2;                 //货币奖励
        private Text m_ConsignTodayCount;
        private Button m_HelpButton;            //协助按钮(其他人的委托)
        private Button m_CancelHelpButton;      //取消协助(自己的委托)
        private Button m_FamilyHelpButton;      //发布协助(自己的委托)
        private OpenFamilyConsignDetailParm m_OpenFamilyConsignDetailParm;
        private uint m_FormulaId;
        private CSVFormula.Data cSVFormulaData;
        private uint m_UID;

        private GameObject viewProcess;
        private Image sliderImage;
        private Transform makeAnimIconParent;
        private Image makeAnimBg;
        private Text animName;


        protected override void OnOpen(object arg)
        {
            m_OpenFamilyConsignDetailParm = arg as OpenFamilyConsignDetailParm;
            if (m_OpenFamilyConsignDetailParm.fromType == 1)
            {
                m_UID = m_OpenFamilyConsignDetailParm.guildConsignInfo.UId;
                m_FormulaId = m_OpenFamilyConsignDetailParm.guildConsignInfo.FormulaId;
                cSVFormulaData = CSVFormula.Instance.GetConfData(m_FormulaId);
            }
            else
            {
                m_UID = m_OpenFamilyConsignDetailParm.guildConsignSelfInfo.UId;
                m_FormulaId = m_OpenFamilyConsignDetailParm.guildConsignSelfInfo.FormulaId;
                cSVFormulaData = CSVFormula.Instance.GetConfData(m_FormulaId);
            }
        }

        protected override void OnLoaded()
        {
            m_Part1 = transform.Find("Animator/Detail/Part1").gameObject;
            m_Part2 = transform.Find("Animator/Detail/Part2").gameObject;
            m_CloseButton = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            m_ItemName = transform.Find("Animator/Detail/ImageBG/PropItem/Text_Name").GetComponent<Text>();
            m_ItemIcon = transform.Find("Animator/Detail/ImageBG/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            m_PreView = transform.Find("Animator/Detail/ImageBG/Button").GetComponent<Button>();
            m_Consigner = transform.Find("Animator/Detail/Image/Text").GetComponent<Text>();
            m_RemainTime = transform.Find("Animator/Detail/Image1/Text").GetComponent<Text>();
            m_Skill = transform.Find("Animator/Detail/Text_Title").GetComponent<Text>();
            m_Reward1 = transform.Find("Animator/Detail/Image2/Text1/Text").GetComponent<Text>();
            m_Reward2 = transform.Find("Animator/Detail/Image2/Text2/Text").GetComponent<Text>();
            m_ConsignTodayCount = transform.Find("Animator/Detail/Part1/Text_Amount").GetComponent<Text>();
            m_HelpButton = transform.Find("Animator/Detail/Part1/Btn_01").GetComponent<Button>();
            m_CancelHelpButton = transform.Find("Animator/Detail/Part2/Btn_01").GetComponent<Button>();
            m_FamilyHelpButton = transform.Find("Animator/Detail/Part2/Btn_01 (1)").GetComponent<Button>();

            viewProcess = transform.Find("Animator/Detail/Part1/View_process").gameObject;
            makeAnimBg = transform.Find("Animator/Detail/Part1/View_process/NPC_Collect/Root/Image1").GetComponent<Image>();
            sliderImage = viewProcess.transform.Find("NPC_Collect/Root/Image_Blank/Image3").GetComponent<Image>();
            makeAnimIconParent = viewProcess.transform.Find("NPC_Collect/Root/Button_Icon");
            animName = viewProcess.transform.Find("NPC_Collect/Root/Text").GetComponent<Text>();

            m_HelpButton.onClick.AddListener(OnHelpButtonClicked);
            m_CancelHelpButton.onClick.AddListener(OnCancelHelpButtonClicked);
            m_FamilyHelpButton.onClick.AddListener(OnFamilyHelpButtonClicked);
            m_PreView.onClick.AddListener(OnPreViewButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            if (m_OpenFamilyConsignDetailParm.type == 1) //协助
            {
                m_Part1.SetActive(true);
                m_Part2.SetActive(false);
                //m_HelpButton.gameObject.SetActive(true);
                uint skillId = cSVFormulaData.type;
                uint lv = cSVFormulaData.level_skill;
                bool can = Sys_LivingSkill.Instance.livingSkills[skillId].Level >= lv;
                ImageHelper.SetImageGray(m_HelpButton, !can, true);
                //ButtonHelper.Enable(m_HelpButton, can);

                //m_CancelHelpButton.gameObject.SetActive(false);
                //m_FamilyHelpButton.gameObject.SetActive(false);
                TextHelper.SetText(m_ConsignTodayCount, LanguageHelper.GetTextContent(590002036, Sys_Family.Instance.helpCountToday.ToString(), Sys_Family.Instance.maxHelpCount.ToString()));
            }
            else
            {
                m_Part1.SetActive(false);
                m_Part2.SetActive(true);
                //m_HelpButton.gameObject.SetActive(false);
                //m_CancelHelpButton.gameObject.SetActive(true);
                //m_FamilyHelpButton.gameObject.SetActive(true);
                TextHelper.SetText(m_ConsignTodayCount, string.Empty);
            }

            TextHelper.SetText(m_Consigner, m_OpenFamilyConsignDetailParm.name);

            if (null != cSVFormulaData)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(cSVFormulaData.view_item);
                if (null != cSVItemData)
                {
                    ImageHelper.SetIcon(m_ItemIcon, cSVItemData.icon_id);
                    TextHelper.SetText(m_ItemName, cSVItemData.name_id);
                }

                uint skillId = cSVFormulaData.type;
                TextHelper.SetText(m_Skill, 2010143, LanguageHelper.GetTextContent(CSVLifeSkill.Instance.GetConfData(skillId).name_id), cSVFormulaData.level_skill.ToString());
                TextHelper.SetText(m_Reward1, cSVFormulaData.proficiency.ToString());
                TextHelper.SetText(m_Reward2, cSVFormulaData.assist_reward.ToString());
            }
            uint endTick = 0;
            if (m_OpenFamilyConsignDetailParm.fromType == 1)
            {
                endTick = m_OpenFamilyConsignDetailParm.guildConsignInfo.EndTick;
            }
            else
            {
                endTick = m_OpenFamilyConsignDetailParm.guildConsignSelfInfo.EndTick;
            }
            uint remainTime = endTick - Sys_Time.Instance.GetServerTime();

            //由于服务器1分钟计算一次有没有过期 可能会导致 刷新的时候 该值小于0，而出现显示错误,所以给个随机的(0,60)的数字表示就行
            if (remainTime < 60)
            {
                remainTime = 1;
            }
            TextHelper.SetText(m_RemainTime, LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_6));
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_Detail);
        }

        private void OnHelpButtonClicked()
        {
            uint skillId = cSVFormulaData.type;
            uint lv = cSVFormulaData.level_skill;
            bool can = Sys_LivingSkill.Instance.livingSkills[skillId].Level >= lv;
            if (!can)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002043));
                return;
            }
            TryHelp();
        }

        private void OnCancelHelpButtonClicked()
        {
            Sys_Family.Instance.CancelConsignReq(m_UID);
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_Detail);
        }

        private void OnFamilyHelpButtonClicked()
        {
            //发送聊天消息
            Sys_Family.Instance.SeekHelpReq(m_UID);
        }

        private void OnPreViewButtonClicked()
        {
            CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(m_FormulaId);
            if (cSVFormulaData != null)
            {
                if (!cSVFormulaData.isequipment)
                {
                    UIManager.OpenUI(EUIID.UI_LifeSkill_MedicineTips, false, m_FormulaId);
                }
                else
                {
                    OpenLifeSkill_MakePreviewParm openLifeSkill_MakePreviewParm = new OpenLifeSkill_MakePreviewParm();
                    openLifeSkill_MakePreviewParm.formulaId = m_FormulaId;

                    UIManager.OpenUI(EUIID.UI_LifeSkill_MakeTips, false, openLifeSkill_MakePreviewParm);
                }
            }
        }

        private TweenerCore<float, float, FloatOptions> A;

        private void TryHelp()
        {
            viewProcess.SetActive(true);
            sliderImage.fillAmount = 0;
            TextHelper.SetText(animName, cSVFormulaData.animation_name);
            ImageHelper.SetIcon(makeAnimBg, cSVFormulaData.formula_animation[1]);
            ImageHelper.SetIcon(sliderImage, cSVFormulaData.formula_animation[0]);

            A?.Kill();
            A = DOTween.To(() => sliderImage.fillAmount, x => sliderImage.fillAmount = x, 1, 5);
            A.onComplete += OnMakeAnimationPlayOver;
        }


        private void OnMakeAnimationPlayOver()
        {
            DestroyMakeAnim();

            Sys_Family.Instance.HelpBuildReq(m_UID);
        }

        private void DestroyMakeAnim()
        {
            sliderImage.fillAmount = 0;
            viewProcess.SetActive(false);
        }
    }
}


