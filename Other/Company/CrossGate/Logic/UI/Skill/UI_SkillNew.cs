using Table;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using System;
using Packet;
using Net;

namespace Logic
{
    public class UI_SkillNew_Layout
    {
        public class LimitDetailItem
        {
            public GameObject root;
            public Image icon;
            public Text name;
            public Text limit1;
            public Text limit2;
            public Text limit3;
            public Text limit4;
            public GameObject lightBg;
            public GameObject darkBg;

            public Sys_Skill.SkillInfo skillInfo;

            public LimitDetailItem(GameObject gameObject, Sys_Skill.SkillInfo _skillInfo)
            {
                skillInfo = _skillInfo;
                root = gameObject;

                icon = root.FindChildByName("SkillIcon").GetComponent<Image>();
                name = root.FindChildByName("SkillName").GetComponent<Text>();
                limit1 = root.FindChildByName("limit1").GetComponent<Text>();
                limit2 = root.FindChildByName("limit2").GetComponent<Text>();
                limit3 = root.FindChildByName("limit3").GetComponent<Text>();
                limit4 = root.FindChildByName("limit4").GetComponent<Text>();
                lightBg = root.FindChildByName("bg_Light");
                darkBg = root.FindChildByName("bg_Dark");
                Update();
            }

            void Update()
            {
                if (skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                {
                    CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                    TextHelper.SetText(name, cSVActiveSkillInfoData.name);
                    ImageHelper.SetIcon(icon, cSVActiveSkillInfoData.icon);
                    TextHelper.SetText(limit1, skillInfo.occUpgradeLimits[0].ToString());
                    TextHelper.SetText(limit2, skillInfo.occUpgradeLimits[1].ToString());
                    TextHelper.SetText(limit3, skillInfo.occUpgradeLimits[2].ToString());
                    TextHelper.SetText(limit4, skillInfo.occUpgradeLimits[3].ToString());
                }
                else if (skillInfo.ESkillType == Sys_Skill.ESkillType.Passive)
                {
                    CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                    TextHelper.SetText(name, cSVPassiveSkillInfoData.name);
                    ImageHelper.SetIcon(icon, cSVPassiveSkillInfoData.icon);
                    TextHelper.SetText(limit1, skillInfo.occUpgradeLimits[0].ToString());
                    TextHelper.SetText(limit2, skillInfo.occUpgradeLimits[1].ToString());
                    TextHelper.SetText(limit3, skillInfo.occUpgradeLimits[2].ToString());
                    TextHelper.SetText(limit4, skillInfo.occUpgradeLimits[3].ToString());
                }
            }

            public void SetBg(bool isLight)
            {
                lightBg.SetActive(isLight);
                darkBg.SetActive(!isLight);
            }
        }

        public class LevelLimitItem
        {
            public GameObject root;
            public Text carrerName;
            public Text limitNum;
            uint carrer;
            uint limit;

            public LevelLimitItem(GameObject gameObject, uint _carrer, uint _limit)
            {
                root = gameObject;
                carrerName = root.FindChildByName("Text_Job").GetComponent<Text>();
                limitNum = root.FindChildByName("Value").GetComponent<Text>();
                carrer = _carrer;
                limit = _limit;
                Update();
            }

            void Update()
            {
                TextHelper.SetText(carrerName, LanguageHelper.GetTextContent(carrer) + "：");
                TextHelper.SetText(limitNum, LanguageHelper.GetTextContent(2029363, limit.ToString()));
            }
        }

        public class SkillItem
        {
            public Sys_Skill.SkillInfo skillInfo;

            public GameObject root;
            public Toggle skillToggle;
            public Image skillBGImage;
            public GameObject selectImage;
            public GameObject forbidImage;
            public Text skillName;
            public Image skillIcon;
            public Image attckTip;
            public GameObject learnFx;
            public GameObject upFx;
            public GameObject lockTipRoot;
            public Text lockTipText;
            public GameObject redPoint;

            public SkillItem(GameObject gameObject, Sys_Skill.SkillInfo _skillInfo)
            {
                skillInfo = _skillInfo;
                root = gameObject;

                skillToggle = root.FindChildByName("Button_Normal").GetComponent<Toggle>();
                skillBGImage = skillToggle.gameObject.GetComponent<Image>();
                skillToggle.onValueChanged.AddListener(OnClickSkillItem);
                learnFx = skillToggle.gameObject.FindChildByName("Fx_ui_playskill");
                upFx = skillToggle.gameObject.FindChildByName("Fx_ui_Skillup");
                selectImage = root.FindChildByName("Image_Select");
                forbidImage = root.FindChildByName("Image_Forbid");
                skillName = root.FindChildByName("Text_Name").GetComponent<Text>();
                skillIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                attckTip = root.FindChildByName("Image_Tip").GetComponent<Image>();
                lockTipRoot = root.FindChildByName("Lock");
                lockTipText = lockTipRoot.GetComponentInChildren<Text>();
                redPoint = root.FindChildByName("Image_Dot");
                Update();
            }

            public void OnClickSkillItem(bool isOn)
            {
                skillToggle.isOn = isOn;
                if (isOn)
                {
                    selectImage.SetActive(true);
                    forbidImage.SetActive(false);
                    Sys_Skill.Instance.eventEmitter.Trigger<Sys_Skill.SkillInfo>(Sys_Skill.EEvents.ChooseSkillItem, skillInfo);
                    if (Sys_Skill.Instance.IsCanLearnSkill(skillInfo))
                    {
                        CmdSkillClickSkillReq req = new CmdSkillClickSkillReq();
                        req.SkillId = skillInfo.SkillID;

                        NetClient.Instance.SendMessage((ushort)CmdSkill.ClickSkillReq, req);
                        redPoint.SetActive(false);
                    }
                }
                else
                {
                    selectImage.gameObject.SetActive(false);
                    if (skillInfo.Level == 0)
                    {
                        forbidImage.SetActive(true);
                    }
                    else
                    {
                        forbidImage.SetActive(false);
                    }
                }
            }

            public void DoFx(bool needFx = false, bool isLearn = false)
            {
                if (needFx)
                {
                    if (isLearn)
                    {
                        learnFx.gameObject.SetActive(true);
                        upFx.gameObject.SetActive(false);
                    }
                    else
                    {
                        learnFx.gameObject.SetActive(false);
                        upFx.gameObject.SetActive(true);
                    }
                }
                else
                {
                    learnFx.gameObject.SetActive(false);
                    upFx.gameObject.SetActive(false);
                }
            }

            public void Update()
            {
                lockTipRoot.SetActive(false);
                root.name = $"SkillItem_{skillInfo.ESkillType}_{skillInfo.SkillID}";
                if (skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                {
                    CSVActiveSkillInfo.Data cSVActiveSkillInfoData;
                    if (skillInfo.Level == 0)
                    {
                        cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);

                        if (Sys_Role.Instance.Role.Level < cSVActiveSkillInfoData.need_lv)
                        {
                            TextHelper.SetText(lockTipText, LanguageHelper.GetTextContent(2028001, cSVActiveSkillInfoData.need_lv.ToString()));
                            lockTipRoot.SetActive(true);
                        }
                    }
                    else
                    {
                        cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                    }

                    TextHelper.SetText(skillName, cSVActiveSkillInfoData.name);
                    ImageHelper.SetIcon(skillIcon, cSVActiveSkillInfoData.icon);
                    ImageHelper.SetIcon(attckTip, cSVActiveSkillInfoData.typeicon);
                }
                else if (skillInfo.ESkillType == Sys_Skill.ESkillType.Passive)
                {
                    CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData;
                    if (skillInfo.Level == 0)
                    {
                        cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                        if (Sys_Role.Instance.Role.Level < cSVPassiveSkillInfoData.need_lv)
                        {
                            TextHelper.SetText(lockTipText, LanguageHelper.GetTextContent(2028001, cSVPassiveSkillInfoData.need_lv.ToString()));
                            lockTipRoot.SetActive(true);
                        }
                    }
                    else
                    {
                        cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                    }

                    TextHelper.SetText(skillName, cSVPassiveSkillInfoData.name);
                    ImageHelper.SetIcon(skillIcon, cSVPassiveSkillInfoData.icon);
                    ImageHelper.SetIcon(attckTip, cSVPassiveSkillInfoData.typeicon);
                }

                selectImage.SetActive(false);

                if (skillInfo.Level == 0)
                {
                    forbidImage.SetActive(true);
                    ImageHelper.SetImageGray(skillIcon, true, true);
                    ImageHelper.SetImageGray(skillName, true);
                    ImageHelper.SetImageGray(skillBGImage, true);
                }
                else
                {
                    forbidImage.SetActive(false);
                    ImageHelper.SetImageGray(skillIcon, false, true);
                    ImageHelper.SetImageGray(skillName, false);
                    ImageHelper.SetImageGray(skillBGImage, false);
                }

                if (Sys_Skill.Instance.IsCanLearnSkill(skillInfo))
                {
                    redPoint.SetActive(true);
                }
                else
                {
                    redPoint.SetActive(false);
                }
            }
        }

        public Transform transform;
        public Button closeButton;

        public Toggle toggleSkill;
        public Toggle toggleTalent;
        public GameObject toggleTalentRedDot;

        public Button activeSkillButton;
        public Button passiveSkillButton;
        public GameObject activeSkillButton_L;
        public GameObject activeSkillButton_D;
        public GameObject passiveSkillButton_L;
        public GameObject passiveSkillButton_D;
        public GameObject activeSkillButtonRedPoint;
        public GameObject passiveSkillButtonRedPoint;

        public GameObject activeSkillRoot;
        public GameObject passiveSkillRoot;
        public GameObject activeSkillContent;
        public GameObject passiveSkillContent;
        public GameObject skillItemPrefab;
        public GameObject skillInfoRoot;

        public GameObject skillMessageRoot;
        public Image selectSkillIcon;
        public Text selectSkillName;
        public Text selectSkillLevelAndRank;
        public GameObject selectSkillCostMPRoot;
        public Text selectSkillCostMP;
        public Text selectSkillProficiency;
        public Slider selectSkillProficiencySlider;

        public GameObject skillDescRoot;
        public Text upDescText;
        public GameObject downDescRoot;
        public Text downTextRoot;
        public Text downText;

        public GameObject upgradeLevelCondRoot;
        public Text upgradeLevelText;
        public GameObject levelCondRoot;
        public Text levelCondText;
        public GameObject careerCondRoot;
        public Text careerCondText;
        public GameObject preTaskCondRoot;
        public Text preTaskCondText;

        public Button learnButton;
        public Button upgradeLevelButton;
        public Button upgradeRankButton;
        public UI_LongPressButton upgradeRankButton_LongPress;
        public GameObject SuLianTip;

        public GameObject costItemRoot;
        public GameObject costItemPrefab;
        public GameObject costItemContent;

        public Transform talentNode;

        public GameObject levelLimitItemRoot;
        public GameObject levelLimitItemPrefab;
        public GameObject levelLimitItemContent;

        public Button limitDetailButton;

        public GameObject limitDetailRoot;
        public GameObject limitDetailItemPrefab;
        public GameObject limitDetailItemContent;
        public Button limitDetailCloseButton;

        public Text LimitTitle1;
        public Text LimitTitle2;
        public Text LimitTitle3;
        public Text LimitTitle4;

        public GameObject maxLevelRankGo;

        public ParticleSystem particleSystem;

        public GameObject learnRoot;
        public Text npcText;
        public Text coinCostText;

        public Button forgetButton;

        public Button quickButton;

        public void Init(Transform transform)
        {
            this.transform = transform;

            talentNode = transform.Find("Animator/View_Talent");

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();

            toggleSkill = transform.gameObject.FindChildByName("TabItem").GetComponent<Toggle>();
            toggleTalent = transform.gameObject.FindChildByName("TabItem (1)").GetComponent<Toggle>();
            toggleTalentRedDot = transform.gameObject.FindChildByName("TabItem (1)/RedDot")?.gameObject;
            toggleTalentRedDot.SetActive(false);

            activeSkillRoot = transform.gameObject.FindChildByName("ActiveSkill_Scroll");
            passiveSkillRoot = transform.gameObject.FindChildByName("PassiveSkill_Scroll");
            activeSkillButton = transform.gameObject.FindChildByName("ListItem").GetComponent<Button>();
            passiveSkillButton = transform.gameObject.FindChildByName("ListItem (1)").GetComponent<Button>();
            activeSkillButton_L = activeSkillButton.gameObject.FindChildByName("Image_Menu_Light");
            activeSkillButton_D = activeSkillButton.gameObject.FindChildByName("Btn_Menu_Dark");
            passiveSkillButton_L = passiveSkillButton.gameObject.FindChildByName("Image_Menu_Light");
            passiveSkillButton_D = passiveSkillButton.gameObject.FindChildByName("Btn_Menu_Dark");
            activeSkillButtonRedPoint = activeSkillButton.gameObject.FindChildByName("Image_Dot");
            passiveSkillButtonRedPoint = passiveSkillButton.gameObject.FindChildByName("Image_Dot");

            activeSkillContent = transform.gameObject.FindChildByName("ActiveSkiilContent");
            passiveSkillContent = transform.gameObject.FindChildByName("PassiveSkillContent");
            skillItemPrefab = transform.gameObject.FindChildByName("SkillItemPrefab");

            skillInfoRoot = transform.gameObject.FindChildByName("View_Skill_Right");
            skillMessageRoot = skillInfoRoot.FindChildByName("View_SkillMessage");
            selectSkillIcon = skillMessageRoot.FindChildByName("Skill_Icon").GetComponent<Image>();
            selectSkillName = skillMessageRoot.FindChildByName("Text_SkillName").GetComponent<Text>();
            selectSkillLevelAndRank = skillMessageRoot.FindChildByName("Text_Level_Num").GetComponent<Text>();
            selectSkillCostMPRoot = skillMessageRoot.FindChildByName("Text_Mp");
            selectSkillCostMP = skillMessageRoot.FindChildByName("Text_Mp_Num").GetComponent<Text>();
            selectSkillProficiency = skillMessageRoot.FindChildByName("Text_Percent").GetComponent<Text>();
            selectSkillProficiencySlider = skillMessageRoot.FindChildByName("Slider_Exp").GetComponent<Slider>();
            particleSystem = skillInfoRoot.FindChildByName("kuang").GetComponent<ParticleSystem>();

            skillDescRoot = skillInfoRoot.FindChildByName("Object_Des");
            upDescText = skillDescRoot.FindChildByName("Text_Tips01").GetComponent<Text>();
            downDescRoot = skillDescRoot.FindChildByName("View_SkillTips1");
            downTextRoot = downDescRoot.FindChildByName("Text_Title_down").GetComponent<Text>();
            downText = downDescRoot.FindChildByName("Text_Tips02").GetComponent<Text>();

            upgradeLevelCondRoot = skillInfoRoot.FindChildByName("View_Condition");
            upgradeLevelText = upgradeLevelCondRoot.FindChildByName("Grade_Condition").GetComponent<Text>();
            levelCondRoot = upgradeLevelCondRoot.FindChildByName("Text_NeedLevel");
            levelCondText = levelCondRoot.FindChildByName("Text_Num").GetComponent<Text>();
            careerCondRoot = upgradeLevelCondRoot.FindChildByName("Text_CareerLevel");
            careerCondText = careerCondRoot.FindChildByName("Text_Num").GetComponent<Text>();
            preTaskCondRoot = upgradeLevelCondRoot.FindChildByName("Text_NeedTask");
            preTaskCondText = preTaskCondRoot.FindChildByName("Text_Name").GetComponent<Text>();

            costItemRoot = skillInfoRoot.FindChildByName("Cost");
            costItemPrefab = costItemRoot.FindChildByName("Item");
            costItemContent = costItemRoot.FindChildByName("View_Cost");

            learnButton = skillInfoRoot.FindChildByName("LearnBtn").GetComponent<Button>();
            upgradeLevelButton = skillInfoRoot.FindChildByName("AddLevelBtn").GetComponent<Button>();
            upgradeRankButton = skillInfoRoot.FindChildByName("AddRankBtn").GetComponent<Button>();
            Text upgradeRankButtonText = upgradeRankButton.gameObject.FindChildByName("Text_01").GetComponent<Text>();
            string str = CSVParam.Instance.GetConfData(264).str_value;
            string[] strs = str.Split('|');
            TextHelper.SetText(upgradeRankButtonText, LanguageHelper.GetTextContent(2029319, strs[1]));
            upgradeRankButton_LongPress = upgradeRankButton.gameObject.AddComponent<UI_LongPressButton>();
            upgradeRankButton_LongPress.interval = 0.3f;
            SuLianTip = transform.gameObject.FindChildByName("SulianduRoot");

            levelLimitItemRoot = transform.gameObject.FindChildByName("StageLimit");
            levelLimitItemPrefab = levelLimitItemRoot.FindChildByName("Item");
            levelLimitItemContent = transform.gameObject.FindChildByName("StageLimitContent");

            limitDetailButton = transform.gameObject.FindChildByName("Btn_Details").GetComponent<Button>();

            limitDetailRoot = transform.gameObject.FindChildByName("View_StageLimit");
            limitDetailItemPrefab = limitDetailRoot.FindChildByName("LimitDetailItem");
            limitDetailItemContent = limitDetailRoot.FindChildByName("limitDetailItemContent");
            limitDetailCloseButton = limitDetailRoot.FindChildByName("Btn_Close").GetComponent<Button>();

            LimitTitle1 = limitDetailRoot.FindChildByName("Text1").GetComponent<Text>();
            LimitTitle2 = limitDetailRoot.FindChildByName("Text2").GetComponent<Text>();
            LimitTitle3 = limitDetailRoot.FindChildByName("Text3").GetComponent<Text>();
            LimitTitle4 = limitDetailRoot.FindChildByName("Text4").GetComponent<Text>();

            maxLevelRankGo = transform.gameObject.FindChildByName("Text_MaxLevel_Tips");

            learnRoot = skillInfoRoot.FindChildByName("Learn");
            npcText = learnRoot.FindChildByName("Text_Tips").GetComponent<Text>();
            coinCostText = learnRoot.FindChildByName("Text_Num").GetComponent<Text>();

            forgetButton = transform.gameObject.FindChildByName("Btn_Forget").GetComponent<Button>();

            quickButton = transform.gameObject.FindChildByName("QuickButton").GetComponent<Button>();
        }

        void UpdateActiveSkillMessage(Sys_Skill.SkillInfo skillInfo, CSVActiveSkillInfo.Data cSVActiveSkillInfoData, CSVActiveSkill.Data cSVActiveSkillData)
        {
            ImageHelper.SetIcon(selectSkillIcon, cSVActiveSkillInfoData.icon);
            TextHelper.SetText(selectSkillName, cSVActiveSkillInfoData.name);
            if (skillInfo.Level != 0)
            {
                TextHelper.SetText(selectSkillLevelAndRank, LanguageHelper.GetTextContent(2029317, skillInfo.Level.ToString(), skillInfo.Rank.ToString()));
            }
            else
            {
                TextHelper.SetText(selectSkillLevelAndRank, LanguageHelper.GetTextContent(2029317, "1", "1"));
            }
            selectSkillCostMPRoot.SetActive(true);
            if (skillInfo.ESkillSubType == Sys_Skill.ESkillSubType.Best)
            {
                TextHelper.SetText(selectSkillCostMP, $"{cSVActiveSkillData.mana_cost}");
            }
            else if (skillInfo.ESkillSubType == Sys_Skill.ESkillSubType.Common)
            {
                TextHelper.SetText(selectSkillCostMP, $"{cSVActiveSkillData.mana_cost * int.Parse(CSVParam.Instance.GetConfData(908).str_value) / 10}");
            }
            TextHelper.SetText(selectSkillProficiency, $"{skillInfo.CurProficiency}/{cSVActiveSkillInfoData.need_proficiency}");
            selectSkillProficiencySlider.value = (float)skillInfo.CurProficiency / cSVActiveSkillInfoData.need_proficiency;
        }

        void UpdatePassiveSkillMessageg(Sys_Skill.SkillInfo skillInfo, CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData)
        {
            ImageHelper.SetIcon(selectSkillIcon, cSVPassiveSkillInfoData.icon);
            TextHelper.SetText(selectSkillName, cSVPassiveSkillInfoData.name);
            if (skillInfo.Level != 0)
            {
                TextHelper.SetText(selectSkillLevelAndRank, LanguageHelper.GetTextContent(2029317, skillInfo.Level.ToString(), skillInfo.Rank.ToString()));
            }
            else
            {
                TextHelper.SetText(selectSkillLevelAndRank, LanguageHelper.GetTextContent(2029317, "1", "1"));
            }
            selectSkillCostMPRoot.SetActive(false);
            TextHelper.SetText(selectSkillProficiency, $"{skillInfo.CurProficiency}/{cSVPassiveSkillInfoData.max_adept}");
            selectSkillProficiencySlider.value = (float)skillInfo.CurProficiency / cSVPassiveSkillInfoData.max_adept;
        }

        void UpdateActiveSkillDesc(Sys_Skill.SkillInfo skillInfo, CSVActiveSkillInfo.Data currentRankSkillInfoData)
        {
            uint nextRankInfoID = skillInfo.GetNextRankInfoID();

            TextHelper.SetText(upDescText, Sys_Skill.Instance.GetSkillDesc(currentRankSkillInfoData.id));
            //未学习或满级//
            if (skillInfo.Level == 0 || skillInfo.Level > skillInfo.MaxLevel || (skillInfo.Level == skillInfo.MaxLevel && skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM))
            {
                downDescRoot.SetActive(false);
            }
            else
            {
                downDescRoot.SetActive(true);
                //某级最高阶(该升级了)///
                if (skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM)
                {
                    TextHelper.SetText(downTextRoot, LanguageHelper.GetTextContent(2029364));
                }
                else
                {
                    TextHelper.SetText(downTextRoot, LanguageHelper.GetTextContent(2029365));
                }
                TextHelper.SetText(downText, Sys_Skill.Instance.GetSkillDesc(nextRankInfoID));
            }
        }

        void UpdatePassiveSkillDesc(Sys_Skill.SkillInfo skillInfo, CSVPassiveSkillInfo.Data currentPassiveSkillInfoData)
        {
            uint nextRankInfoID = skillInfo.GetNextRankInfoID();
            CSVPassiveSkillInfo.Data nextPassiveSkillInfoData;

            TextHelper.SetText(upDescText, currentPassiveSkillInfoData.desc);
            //未学习或满级//
            if (skillInfo.Level == 0 || skillInfo.Level > skillInfo.MaxLevel || (skillInfo.Level == skillInfo.MaxLevel && skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM))
            {
                downDescRoot.SetActive(false);
            }
            else
            {
                nextPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(nextRankInfoID);

                downDescRoot.SetActive(true);
                //某级最高阶(该升级了)///
                if (skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM)
                {
                    TextHelper.SetText(downTextRoot, LanguageHelper.GetTextContent(2029364));
                }
                else
                {
                    TextHelper.SetText(downTextRoot, LanguageHelper.GetTextContent(2029365));
                }
                TextHelper.SetText(downText, nextPassiveSkillInfoData.desc);
            }
        }

        void UpdateActiveSkillConditions(Sys_Skill.SkillInfo skillInfo, CSVActiveSkillInfo.Data currentActiveSkillInfoData)
        {
            if (skillInfo.Level < skillInfo.MaxLevel || (skillInfo.Level == skillInfo.MaxLevel && skillInfo.Rank < Sys_Skill.EACHLEVELRANKNUM))
            {
                CSVActiveSkillInfo.Data nextActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.SkillID * 1000 + skillInfo.Level * Sys_Skill.EACHLEVELRANKNUM + 1);

                if (skillInfo.Level == 0 || (skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM && skillInfo.CurProficiency >= currentActiveSkillInfoData.need_proficiency))
                {
                    upgradeLevelCondRoot.SetActive(true);
                    if (nextActiveSkillInfoData.need_lv == 0)
                    {
                        levelCondRoot.SetActive(false);
                    }
                    else
                    {
                        levelCondRoot.SetActive(true);
                        if (nextActiveSkillInfoData.need_lv > Sys_Role.Instance.Role.Level)
                        {
                            TextHelper.SetText(levelCondText, 1000998, nextActiveSkillInfoData.need_lv.ToString());
                        }
                        else
                        {
                            TextHelper.SetText(levelCondText, 1000997, nextActiveSkillInfoData.need_lv.ToString());
                        }
                    }

                    if (!nextActiveSkillInfoData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                    {
                        careerCondRoot.SetActive(false);
                    }
                    else
                    {
                        careerCondRoot.SetActive(true);
                        if (nextActiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career] > Sys_Role.Instance.Role.CareerRank)
                        {
                            TextHelper.SetText(careerCondText, 1000998, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + nextActiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan));
                        }
                        else
                        {
                            TextHelper.SetText(careerCondText, 1000997, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + nextActiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan));
                        }
                    }

                    if (nextActiveSkillInfoData.pre_task == 0)
                    {
                        preTaskCondRoot.SetActive(false);
                    }
                    else
                    {
                        preTaskCondRoot.SetActive(true);
                        CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(nextActiveSkillInfoData.pre_task);
                        if (cSVTaskData != null)
                        {
                            if (Sys_Task.Instance.GetTaskState(nextActiveSkillInfoData.pre_task) != ETaskState.Submited)
                            {
                                TextHelper.SetText(preTaskCondText, 1000998, CSVTaskLanguage.Instance.GetConfData(cSVTaskData.taskName).words);
                            }
                            else
                            {
                                TextHelper.SetText(preTaskCondText, 1000997, CSVTaskLanguage.Instance.GetConfData(cSVTaskData.taskName).words);
                            }
                        }
                        else
                        {
                            DebugUtil.LogError($"Can not find NeedTask id: {nextActiveSkillInfoData.pre_task}");
                        }
                    }
                }
                else
                {
                    upgradeLevelCondRoot.SetActive(false);
                }

                if ((skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM && skillInfo.CurProficiency >= currentActiveSkillInfoData.need_proficiency) || skillInfo.Level == 0)
                {
                    if (nextActiveSkillInfoData.upgrade_cost.Count > 0)
                    {
                        costItemRoot.SetActive(true);
                        costItemContent.DestoryAllChildren();

                        for (int index = 0, len = nextActiveSkillInfoData.upgrade_cost.Count; index < len; index++)
                        {
                            GameObject costItemGo = GameObject.Instantiate(costItemPrefab);
                            costItemGo.SetActive(true);
                            costItemGo.transform.SetParent(costItemContent.transform, false);

                            PropItem costItem = new PropItem();
                            costItem.BindGameObject(costItemGo);
                            CSVItem.Data itemData = CSVItem.Instance.GetConfData(nextActiveSkillInfoData.upgrade_cost[index][0]);
                            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.id, nextActiveSkillInfoData.upgrade_cost[index][1], true, false, false, false, false, true, true, true);
                            costItem.SetData(new MessageBoxEvt(EUIID.UI_SkillUpgrade, showItem));
                        }
                    }
                    else
                    {
                        costItemRoot.SetActive(false);
                    }
                }
                else
                {
                    costItemRoot.SetActive(true);
                    costItemContent.DestoryAllChildren();

                    GameObject costItemGo = GameObject.Instantiate(costItemPrefab);
                    costItemGo.SetActive(true);
                    costItemGo.transform.SetParent(costItemContent.transform, false);

                    PropItem costItem = new PropItem();
                    costItem.BindGameObject(costItemGo);
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(202002);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.id, 1, true, false, false, false, false, true, true, true);
                    costItem.SetData(new MessageBoxEvt(EUIID.UI_SkillUpgrade, showItem));
                }
            }
            else
            {
                upgradeLevelCondRoot.SetActive(false);
                costItemRoot.SetActive(false);
            }
        }

        void UpdatePassiveSkillConditions(Sys_Skill.SkillInfo skillInfo, CSVPassiveSkillInfo.Data currentPassiveSkillInfoData)
        {
            if (skillInfo.Level < skillInfo.MaxLevel || (skillInfo.Level == skillInfo.MaxLevel && skillInfo.Rank < Sys_Skill.EACHLEVELRANKNUM))
            {
                CSVPassiveSkillInfo.Data nextPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.SkillID * 1000 + skillInfo.Level * Sys_Skill.EACHLEVELRANKNUM + 1);

                if (skillInfo.Level == 0 || (skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM && skillInfo.CurProficiency >= currentPassiveSkillInfoData.max_adept))
                {
                    upgradeLevelCondRoot.SetActive(true);

                    if (nextPassiveSkillInfoData.need_lv == 0)
                    {
                        levelCondRoot.SetActive(false);
                    }
                    else
                    {
                        levelCondRoot.SetActive(true);
                        if (nextPassiveSkillInfoData.need_lv > Sys_Role.Instance.Role.Level)
                        {
                            TextHelper.SetText(levelCondText, 1000998, nextPassiveSkillInfoData.need_lv.ToString());
                        }
                        else
                        {
                            TextHelper.SetText(levelCondText, 1000997, nextPassiveSkillInfoData.need_lv.ToString());
                        }
                    }

                    if (!nextPassiveSkillInfoData.SkillCareerLevelCond().ContainsKey(Sys_Role.Instance.Role.Career))
                    {
                        careerCondRoot.SetActive(false);
                    }
                    else
                    {
                        careerCondRoot.SetActive(true);
                        if (nextPassiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career] > Sys_Role.Instance.Role.CareerRank)
                        {
                            TextHelper.SetText(careerCondText, 1000998, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + nextPassiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan));
                        }
                        else
                        {
                            TextHelper.SetText(careerCondText, 1000997, LanguageHelper.GetTextContent(CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + nextPassiveSkillInfoData.SkillCareerLevelCond()[Sys_Role.Instance.Role.Career]).professionLan));
                        }
                    }

                    if (nextPassiveSkillInfoData.pre_task == 0)
                    {
                        preTaskCondRoot.SetActive(false);
                    }
                    else
                    {
                        preTaskCondRoot.SetActive(true);
                        CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(nextPassiveSkillInfoData.pre_task);
                        if (cSVTaskData != null)
                        {
                            if (Sys_Task.Instance.GetTaskState(nextPassiveSkillInfoData.pre_task) != ETaskState.Submited)
                            {
                                TextHelper.SetText(preTaskCondText, 1000998, CSVTaskLanguage.Instance.GetConfData(cSVTaskData.taskName).words);
                            }
                            else
                            {
                                TextHelper.SetText(preTaskCondText, 1000997, CSVTaskLanguage.Instance.GetConfData(cSVTaskData.taskName).words);
                            }
                        }
                        else
                        {
                            DebugUtil.LogError($"Can not find NeedTask id: {nextPassiveSkillInfoData.pre_task}");
                        }
                    }
                }
                else
                {
                    upgradeLevelCondRoot.SetActive(false);
                }

                if ((skillInfo.Rank >= Sys_Skill.EACHLEVELRANKNUM && skillInfo.CurProficiency >= currentPassiveSkillInfoData.max_adept) || skillInfo.Level == 0)
                {
                    if (nextPassiveSkillInfoData.upgrade_cost.Count > 0)
                    {
                        costItemRoot.SetActive(true);
                        costItemContent.DestoryAllChildren();

                        for (int index = 0, len = nextPassiveSkillInfoData.upgrade_cost.Count; index < len; index++)
                        {
                            GameObject costItemGo = GameObject.Instantiate(costItemPrefab);
                            costItemGo.SetActive(true);
                            costItemGo.transform.SetParent(costItemContent.transform, false);

                            PropItem costItem = new PropItem();
                            costItem.BindGameObject(costItemGo);
                            CSVItem.Data itemData = CSVItem.Instance.GetConfData(nextPassiveSkillInfoData.upgrade_cost[index][0]);
                            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.id, nextPassiveSkillInfoData.upgrade_cost[index][1], true, false, false, false, false, true, true, true);
                            costItem.SetData(new MessageBoxEvt(EUIID.UI_SkillUpgrade, showItem));
                        }
                    }
                    else
                    {
                        costItemRoot.SetActive(false);
                    }
                }
                else
                {
                    costItemRoot.SetActive(true);
                    costItemContent.DestoryAllChildren();

                    GameObject costItemGo = GameObject.Instantiate(costItemPrefab);
                    costItemGo.SetActive(true);
                    costItemGo.transform.SetParent(costItemContent.transform, false);
                    PropItem costItem = new PropItem();
                    costItem.BindGameObject(costItemGo);
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(202002);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.id, 1, true, false, false, false, false, true, true, true);
                    costItem.SetData(new MessageBoxEvt(EUIID.UI_SkillUpgrade, showItem));
                }
            }
            else
            {
                upgradeLevelCondRoot.SetActive(false);
                costItemRoot.SetActive(false);
            }
        }

        void UpdateButtons(Sys_Skill.SkillInfo skillInfo, uint MaxProficiency)
        {
            quickButton.gameObject.SetActive(false);
            CSVCheckseq.Data cSVCheckseqData = CSVCheckseq.Instance.GetConfData(30901);
            if (skillInfo.Level == 0)
            {
                learnButton.gameObject.SetActive(true);
                upgradeLevelButton.gameObject.SetActive(false);
                upgradeRankButton.gameObject.SetActive(false);
                maxLevelRankGo.SetActive(false);
                SuLianTip.gameObject.SetActive(false);
                forgetButton.gameObject.SetActive(false);            
            }
            else if (skillInfo.Level < skillInfo.MaxLevel || (skillInfo.Level == skillInfo.MaxLevel && skillInfo.Rank < Sys_Skill.EACHLEVELRANKNUM))
            {               
                forgetButton.gameObject.SetActive(true);
                if (skillInfo.CurProficiency >= MaxProficiency)
                {
                    learnButton.gameObject.SetActive(false);
                    upgradeLevelButton.gameObject.SetActive(true);
                    if (cSVCheckseqData.IsValid())
                        quickButton.gameObject.SetActive(true);
                    upgradeRankButton.gameObject.SetActive(false);
                    SuLianTip.gameObject.SetActive(false);
                }
                else
                {
                    learnButton.gameObject.SetActive(false);
                    upgradeLevelButton.gameObject.SetActive(false);
                    if (Sys_Role.Instance.Role.Level >= int.Parse(CSVParam.Instance.GetConfData(932).str_value))
                    {
                        upgradeRankButton.gameObject.SetActive(true);
                        if (cSVCheckseqData.IsValid())
                            quickButton.gameObject.SetActive(true);
                        SuLianTip.gameObject.SetActive(false);
                        costItemRoot.SetActive(true);
                    }
                    else
                    {
                        upgradeRankButton.gameObject.SetActive(false);
                        SuLianTip.gameObject.SetActive(true);
                        costItemRoot.SetActive(false);
                    }
                }
                maxLevelRankGo.SetActive(false);
            }
            else
            {
                learnButton.gameObject.SetActive(false);
                upgradeLevelButton.gameObject.SetActive(false);
                upgradeRankButton.gameObject.SetActive(false);
                maxLevelRankGo.SetActive(true);
                SuLianTip.gameObject.SetActive(false);
                forgetButton.gameObject.SetActive(true);
            }
        }

        public void UpdateInfo(Sys_Skill.SkillInfo skillInfo)
        {
            if (skillInfo == null)
                return;

            if (skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
            {
                CSVActiveSkill.Data currentSkillData;
                CSVActiveSkillInfo.Data currentRankSkillInfoData;
                CSVActiveSkillEffective.Data currentActiveSkillEffectiveData;
                if (skillInfo.Level == 0)
                {
                    currentSkillData = CSVActiveSkill.Instance.GetConfData(skillInfo.FirstInfoID);
                    currentRankSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                    currentActiveSkillEffectiveData = CSVActiveSkillEffective.Instance.GetConfData(skillInfo.FirstInfoID);
                    TextHelper.SetText(upgradeLevelText, LanguageHelper.GetTextContent(2029366));
                }
                else
                {
                    currentSkillData = CSVActiveSkill.Instance.GetConfData(skillInfo.CurInfoID);
                    currentRankSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                    currentActiveSkillEffectiveData = CSVActiveSkillEffective.Instance.GetConfData(skillInfo.CurInfoID);
                    TextHelper.SetText(upgradeLevelText, LanguageHelper.GetTextContent(2029367));
                }

                UpdateActiveSkillMessage(skillInfo, currentRankSkillInfoData, currentSkillData);

                UpdateActiveSkillDesc(skillInfo, currentRankSkillInfoData);

                UpdateActiveSkillConditions(skillInfo, currentRankSkillInfoData);

                UpdateButtons(skillInfo, currentRankSkillInfoData.need_proficiency);

                if (currentRankSkillInfoData.upgrade_cost != null)
                {
                    UpdateLearnInfo(skillInfo, currentRankSkillInfoData.learn_npc, currentRankSkillInfoData.upgrade_cost);
                }
                else
                {
                    UpdateLearnInfo(skillInfo, currentRankSkillInfoData.learn_npc);
                }
            }
            else if (skillInfo.ESkillType == Sys_Skill.ESkillType.Passive)
            {
                CSVPassiveSkillInfo.Data currentRankSkillInfoData;
                if (skillInfo.Level == 0)
                {
                    currentRankSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.FirstInfoID);
                }
                else
                {
                    currentRankSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(skillInfo.CurInfoID);
                }

                UpdatePassiveSkillMessageg(skillInfo, currentRankSkillInfoData);

                UpdatePassiveSkillDesc(skillInfo, currentRankSkillInfoData);

                UpdatePassiveSkillConditions(skillInfo, currentRankSkillInfoData);

                UpdateButtons(skillInfo, currentRankSkillInfoData.max_adept);

                if (currentRankSkillInfoData.upgrade_cost != null)
                {
                    UpdateLearnInfo(skillInfo, currentRankSkillInfoData.learn_npc, currentRankSkillInfoData.upgrade_cost);
                }
                else
                {
                    UpdateLearnInfo(skillInfo, currentRankSkillInfoData.learn_npc);
                }
            }

            UpdateLevelLimitInfo(skillInfo);
        }

        void UpdateLearnInfo(Sys_Skill.SkillInfo skillInfo, uint npcID, List<List<uint>> costs = null)
        {
            if (skillInfo.Rank == 0)
            {
                costItemRoot.SetActive(false);
                learnRoot.SetActive(true);

                CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(npcID);
                if (cSVNpcData != null)
                {
                    CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(cSVNpcData.mapId);
                    if (cSVMapInfoData != null)
                    {
                        TextHelper.SetText(npcText, $"{LanguageHelper.GetTextContent(cSVMapInfoData.name)}  {LanguageHelper.GetNpcTextContent(cSVNpcData.name)}");
                    }
                    else
                    {
                        DebugUtil.LogError($"cSVMapInfoData is null id:{npcID}");
                        TextHelper.SetText(npcText, LanguageHelper.GetNpcTextContent(cSVNpcData.name));
                    }

                    if (costs != null)
                    {
                        if (Sys_Bag.Instance.GetItemCount(costs[0][0]) >= costs[0][1])
                        {
                            TextHelper.SetText(coinCostText, costs[0][1].ToString(), LanguageHelper.GetTextStyle(21));
                        }
                        else
                        {
                            TextHelper.SetText(coinCostText, costs[0][1].ToString(), LanguageHelper.GetTextStyle(20));
                        }
                    }

                }
                else
                {
                    DebugUtil.LogError($"cSVNpcData is null id:{npcID}");
                }
            }
            else
            {
                learnRoot.SetActive(false);
            }
        }

        void UpdateLevelLimitInfo(Sys_Skill.SkillInfo skillInfo)
        {
            if (skillInfo.Level == 0)
            {
                levelLimitItemContent.DestoryAllChildren();

                for (int index = 0, len = skillInfo.occUpgradeLimits.Count; index < len; index++)
                {
                    GameObject levelLimitItemGo = GameObject.Instantiate(levelLimitItemPrefab);
                    levelLimitItemGo.SetActive(true);
                    levelLimitItemGo.transform.SetParent(levelLimitItemContent.transform, false);
                    CSVPromoteCareer.Data cSVPromoteCareerData = CSVPromoteCareer.Instance.GetConfData((uint)(Sys_Role.Instance.Role.Career * 100 + index));

                    LevelLimitItem levelLimitItem = new LevelLimitItem(levelLimitItemGo, cSVPromoteCareerData.professionLan, skillInfo.occUpgradeLimits[index]);
                }

                levelLimitItemRoot.SetActive(true);
            }
            else
            {
                levelLimitItemContent.DestoryAllChildren();
                levelLimitItemRoot.SetActive(false);
            }
        }

        public void UpdateLimitDetailInfo()
        {
            TextHelper.SetText(LimitTitle1, CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100).professionLan);
            TextHelper.SetText(LimitTitle2, CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + 1).professionLan);
            TextHelper.SetText(LimitTitle3, CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + 2).professionLan);
            TextHelper.SetText(LimitTitle4, CSVPromoteCareer.Instance.GetConfData(Sys_Role.Instance.Role.Career * 100 + 3).professionLan);

            int count = 0;
            limitDetailItemContent.DestoryAllChildren();

            foreach (var skillInfo in Sys_Skill.Instance.bestSkillInfos.Values)
            {
                GameObject limitDetailItemGo = GameObject.Instantiate(limitDetailItemPrefab);
                limitDetailItemGo.SetActive(true);
                limitDetailItemGo.transform.SetParent(limitDetailItemContent.transform, false);
                LimitDetailItem limitDetailItem = new LimitDetailItem(limitDetailItemGo, skillInfo);
                limitDetailItem.SetBg((count % 2 != 0));
                count++;
            }

            foreach (var skillInfo in Sys_Skill.Instance.commonSkillInfos.Values)
            {
                GameObject limitDetailItemGo = GameObject.Instantiate(limitDetailItemPrefab);
                limitDetailItemGo.SetActive(true);
                limitDetailItemGo.transform.SetParent(limitDetailItemContent.transform, false);
                LimitDetailItem limitDetailItem = new LimitDetailItem(limitDetailItemGo, skillInfo);
                limitDetailItem.SetBg((count % 2 != 0));
                count++;
            }
        }

        public void UpdateUseItemCount()
        {
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            learnButton.onClick.AddListener(listener.OnClickLearnButton);
            upgradeLevelButton.onClick.AddListener(listener.OnClickUpgradeLevelButton);
            upgradeRankButton.onClick.AddListener(listener.OnClickUpgradeRankButton);
            toggleSkill.onValueChanged.AddListener(listener.OnClickSkillToggle);
            toggleTalent.onValueChanged.AddListener(listener.OnValueChangedForTalent);
            upgradeRankButton_LongPress.onLongPress.AddListener(listener.OnLongPressed);
            upgradeRankButton_LongPress.onRelease.AddListener(listener.OnReleased);
            limitDetailButton.onClick.AddListener(listener.OnClickLimitDetailButton);
            limitDetailCloseButton.onClick.AddListener(listener.OnClickLimitDetailCloseButton);
            activeSkillButton.onClick.AddListener(listener.OnClickActiveSkillButton);
            passiveSkillButton.onClick.AddListener(listener.OnClickPassiveSkillButton);
            forgetButton.onClick.AddListener(listener.OnClickForgetButton);
            quickButton.onClick.AddListener(listener.OnClickQuickButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickLearnButton();

            void OnClickUpgradeLevelButton();

            void OnClickUpgradeRankButton();

            void OnValueChangedForTalent(bool isOn);

            void OnLongPressed(float dt);

            void OnReleased();

            void OnClickLimitDetailButton();

            void OnClickLimitDetailCloseButton();

            void OnClickActiveSkillButton();

            void OnClickPassiveSkillButton();

            void OnClickSkillToggle(bool isOn);

            void OnClickForgetButton();

            void OnClickQuickButton();
        }
    }

    public class UI_SkillNew : UIBase, UI_SkillNew_Layout.IListener
    {
        public enum ETab {
            Skill = 0,
            Talent = 1,
        }

        UI_SkillNew_Layout layout = new UI_SkillNew_Layout();
        UI_CurrencyTitle UI_CurrencyTitle;

        Sys_Skill.SkillInfo currentSelectSkill;
        Sys_Skill.SkillInfo initSelectSkill;

        List<UI_SkillNew_Layout.SkillItem> activeSkillItems = new List<UI_SkillNew_Layout.SkillItem>();
        List<UI_SkillNew_Layout.SkillItem> passiveSkillItems = new List<UI_SkillNew_Layout.SkillItem>();

        UI_Talent uiTalent;

        protected override void OnLoaded()
        {
            activeSkillItems.Clear();
            passiveSkillItems.Clear();

            layout.Init(transform);
            layout.RegisterEvents(this);

            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            transform.Find("Animator/UI_Property").gameObject.SetActive(true);
        }

        protected override void OnDestroy()
        {
            UI_CurrencyTitle.Dispose();
            uiTalent?.OnDestroy();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Skill.Instance.eventEmitter.Handle(Sys_Skill.EEvents.ForgetSkill, ForgetSkill, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle(Sys_Skill.EEvents.ClickedItem, OnClickedItem, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle<Sys_Skill.SkillInfo>(Sys_Skill.EEvents.ChooseSkillItem, OnChooseSkillItem, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle<Sys_Skill.SkillInfo>(Sys_Skill.EEvents.UpdateSkillLevel, OnUpdateSkillLevel, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle<Sys_Skill.SkillInfo>(Sys_Skill.EEvents.UpdateSkillRankExp, OnUpdateSkillRankExp, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle(Sys_Skill.EEvents.UseItem, OnUseItem, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.TriggerFunctionOpen, OnTriggerFunctionOpen, toRegister);
            Sys_Skill.Instance.eventEmitter.Handle(Sys_Skill.EEvents.SkillRankUp, OnSkillRankUp, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnExchangeTalentPoint, RefreshTalentDot, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnLianghuaChanged, RefreshTalentDot, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, RefreshTalentDot, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.OnSortOrderChanged, this.OnSortOrderChanged, toRegister);
        }

        private ETab currentTab = ETab.Skill;
        uint openSelectSkillID = 0;

        public void OnClickSkillToggle(bool isOn)
        {
            if (isOn)
            {
                currentTab = ETab.Skill;
                layout.activeSkillButton.onClick.Invoke();
                if (activeSkillItems != null && activeSkillItems.Count > 0)
                {
                    activeSkillItems[0].skillToggle.isOn = true;
                    activeSkillItems[0].skillToggle.onValueChanged.Invoke(true);
                }
            }
        }

        protected override void OnOpen(object arg)
        {
            List<int> paramsList = arg as List<int>;
            if (paramsList != null)
            {
                if (paramsList.Count > 0)
                {
                    currentTab = (ETab)Convert.ToInt32(paramsList[0]);
                    if (paramsList.Count > 1)
                    {
                        openSelectSkillID = (uint)paramsList[1];
                    }
                }
            }
        }

        protected override void OnHideStart()
        {
            if (currentSelectSkill != null)
                openSelectSkillID = currentSelectSkill.SkillID;
        }

        protected override void OnShow()
        {
            try
            {
                if (Sys_Role.Instance.Role.Career == Constants.NOCAREERID)
                {
                    layout.skillInfoRoot.SetActive(false);
                }
                else
                {
                    layout.skillInfoRoot.SetActive(true);
                }

                RefreshTab();
                DispatchTab();
                UI_CurrencyTitle.InitUi();

                UpdateCommonSkillList();
                UpdateBestSkillList();

                if (openSelectSkillID != 0)
                {
                    foreach (var skill in Sys_Skill.Instance.bestSkillInfos)
                    {
                        if (skill.Key == openSelectSkillID)
                        {
                            initSelectSkill = skill.Value;
                            break;
                        }
                    }

                    foreach (var skill in Sys_Skill.Instance.commonSkillInfos)
                    {
                        if (skill.Key == openSelectSkillID)
                        {
                            initSelectSkill = skill.Value;
                            break;
                        }
                    }
                    openSelectSkillID = 0;
                }

                if (initSelectSkill != null)
                {
                    if (Sys_Skill.Instance.bestSkillInfos.ContainsKey(initSelectSkill.SkillID))
                    {
                        layout.activeSkillButton.onClick.Invoke();
                    }
                    else if (Sys_Skill.Instance.commonSkillInfos.ContainsKey(initSelectSkill.SkillID))
                    {
                        layout.passiveSkillButton.onClick.Invoke();
                    }

                    if (initSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
                    {
                        for (int index = 0, len = activeSkillItems.Count; index < len; index++)
                        {
                            if (activeSkillItems[index].skillInfo.SkillID == initSelectSkill.SkillID)
                            {
                                activeSkillItems[index].skillToggle.isOn = true;
                                activeSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                                break;
                            }
                        }
                    }
                    else if (initSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
                    {
                        for (int index = 0, len = passiveSkillItems.Count; index < len; index++)
                        {
                            if (passiveSkillItems[index].skillInfo.SkillID == initSelectSkill.SkillID)
                            {
                                passiveSkillItems[index].skillToggle.isOn = true;
                                passiveSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                                break;
                            }
                        }
                    }
                }

                initSelectSkill = null;

                upgradeLevelButtonFlag = true;
                learnButtonFlag = true;

                layout.particleSystem.gameObject.SetActive(false);

                if (Sys_Skill.Instance.ExistedLearnShowTipBestSkill())
                {
                    layout.activeSkillButtonRedPoint.SetActive(true);
                }
                else
                {
                    layout.activeSkillButtonRedPoint.SetActive(false);
                }

                uint levelLimit = uint.Parse(CSVParam.Instance.GetConfData(953).str_value);
                if (Sys_Skill.Instance.ExistedLearnShowTipCommonSkill() && Sys_Role.Instance.Role.Level >= levelLimit)
                {
                    layout.passiveSkillButtonRedPoint.SetActive(true);
                }
                else
                {
                    layout.passiveSkillButtonRedPoint.SetActive(false);
                }
            }
            catch (System.Exception e)
            {
                DebugUtil.LogError(e.ToString());
            }
        }

        private void DispatchTab() {
            if (currentTab == ETab.Skill) {
                layout.toggleSkill.isOn = true;
            }
            else if (currentTab == ETab.Talent) {
                layout.toggleTalent.isOn = true;
            }
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            if (currentSelectSkill != null)
            {
                layout.UpdateInfo(currentSelectSkill);
            }

            RefreshTalentDot();
        }

        void OnItemCountChanged(int _, int __) {
            RefreshTalentDot();
        }

        void RefreshTalentDot() {
            layout.toggleTalentRedDot?.SetActive(Sys_Talent.Instance.CanLianhua());
        }

        void OnSortOrderChanged(uint stack, int uiId) {
            if (uiId == (int)(EUIID.UI_SkillUpgrade)) {
                uiTalent?.RefreshLayer(this);
            }
        }

        void ForgetSkill()
        {
            if (currentSelectSkill.ESkillSubType == Sys_Skill.ESkillSubType.Best)
                UpdateBestSkillList(currentSelectSkill);
            else
                UpdateCommonSkillList(currentSelectSkill);
            layout.UpdateInfo(currentSelectSkill);
        }

        void OnClickedItem()
        {
            if (Sys_Skill.Instance.ExistedLearnShowTipBestSkill())
            {
                layout.activeSkillButtonRedPoint.SetActive(true);
            }
            else
            {
                layout.activeSkillButtonRedPoint.SetActive(false);
            }

            uint levelLimit = uint.Parse(CSVParam.Instance.GetConfData(953).str_value);
            if (Sys_Skill.Instance.ExistedLearnShowTipCommonSkill() && Sys_Role.Instance.Role.Level >= levelLimit)
            {
                layout.passiveSkillButtonRedPoint.SetActive(true);
            }
            else
            {
                layout.passiveSkillButtonRedPoint.SetActive(false);
            }
        }

        void OnChooseSkillItem(Sys_Skill.SkillInfo skillInfo)
        {
            currentSelectSkill = skillInfo;
            layout.UpdateInfo(currentSelectSkill);
        }

        void OnUpdateSkillRankExp(Sys_Skill.SkillInfo skillInfo)
        {
            currentSelectSkill = skillInfo;
            layout.UpdateInfo(currentSelectSkill);
        }

        void OnSkillRankUp()
        {
            layout.particleSystem.gameObject.SetActive(true);
            layout.particleSystem.Play();
            AudioUtil.PlayAudio(4002);
        }

        void OnUpdateSkillLevel(Sys_Skill.SkillInfo skillInfo)
        {
            if (skillInfo.ESkillSubType == Sys_Skill.ESkillSubType.Best)
            {
                if (skillInfo.Level == 1)
                {
                    UpdateBestSkillList(skillInfo, true, true);
                }
                else
                {
                    UpdateBestSkillList(skillInfo, true, false);
                }
            }
            else if (skillInfo.ESkillSubType == Sys_Skill.ESkillSubType.Common)
            {
                if (skillInfo.Level == 1)
                {
                    UpdateCommonSkillList(skillInfo, true, true);
                }
                else
                {
                    UpdateCommonSkillList(skillInfo, true, false);
                }
            }
        }

        void UpdateBestSkillList(Sys_Skill.SkillInfo skillInfo = null, bool needFx = false, bool isLearn = false)
        {
            layout.activeSkillContent.DestoryAllChildren();
            activeSkillItems.Clear();

            foreach (Sys_Skill.SkillInfo info in Sys_Skill.Instance.bestSkillInfos.Values)
            {
                GameObject skillItemGo = GameObject.Instantiate(layout.skillItemPrefab);
                skillItemGo.SetActive(true);
                UI_SkillNew_Layout.SkillItem skillItem = new UI_SkillNew_Layout.SkillItem(skillItemGo, info);
                skillItem.Update();
                if (skillInfo != null)
                {
                    if (info.SkillID == skillInfo.SkillID)
                    {
                        skillItem.DoFx(needFx, isLearn);
                    }
                }
                else
                {
                    skillItem.DoFx(false, isLearn);
                }
                activeSkillItems.Add(skillItem);
            }

            activeSkillItems.Sort((itemA, itemB) =>
            {
                if (itemA.skillInfo.Level > 0)
                {
                    if (itemB.skillInfo.Level > 0)
                    {
                        if (itemA.skillInfo.SortIndex >= itemB.skillInfo.SortIndex)
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    uint needLVA;
                    if (itemA.skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                        needLVA = CSVActiveSkillInfo.Instance.GetConfData(itemA.skillInfo.FirstInfoID).need_lv;
                    else
                        needLVA = CSVPassiveSkillInfo.Instance.GetConfData(itemA.skillInfo.FirstInfoID).need_lv;
                    uint needLVB;
                    if (itemB.skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                        needLVB = CSVActiveSkillInfo.Instance.GetConfData(itemB.skillInfo.FirstInfoID).need_lv;
                    else
                        needLVB = CSVPassiveSkillInfo.Instance.GetConfData(itemB.skillInfo.FirstInfoID).need_lv;

                    if (needLVA > needLVB)
                        return 1;
                    else if (needLVA < needLVB)
                        return -1;
                    else
                    {
                        if (itemA.skillInfo.SortIndex >= itemB.skillInfo.SortIndex)
                            return 1;
                        else
                            return -1;
                    }
                }
            });

            for (int index = 0, len = activeSkillItems.Count; index < len; index++)
            {
                activeSkillItems[index].root.transform.SetParent(layout.activeSkillContent.transform, false);
                if (skillInfo != null && skillInfo.SkillID == activeSkillItems[index].skillInfo.SkillID)
                {
                    activeSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                }
            }

            if (skillInfo == null)
            {
                activeSkillItems[0].skillToggle.isOn = true;
                activeSkillItems[0].skillToggle.onValueChanged.Invoke(true);
            }
        }

        void UpdateCommonSkillList(Sys_Skill.SkillInfo skillInfo = null, bool needFx = false, bool isLearn = false)
        {
            layout.passiveSkillContent.DestoryAllChildren();
            passiveSkillItems.Clear();

            foreach (Sys_Skill.SkillInfo info in Sys_Skill.Instance.commonSkillInfos.Values)
            {
                GameObject skillItemGo = GameObject.Instantiate(layout.skillItemPrefab);
                skillItemGo.SetActive(true);
                UI_SkillNew_Layout.SkillItem skillItem = new UI_SkillNew_Layout.SkillItem(skillItemGo, info);
                skillItem.Update();
                if (skillInfo != null)
                {
                    if (info.SkillID == skillInfo.SkillID)
                    {
                        skillItem.DoFx(needFx, isLearn);
                    }
                }
                else
                {
                    skillItem.DoFx(false, isLearn);
                }
                passiveSkillItems.Add(skillItem);
            }

            passiveSkillItems.Sort((itemA, itemB) =>
            {
                if (itemA.skillInfo.Level > 0)
                {
                    if (itemB.skillInfo.Level > 0)
                    {
                        if (itemA.skillInfo.SortIndex >= itemB.skillInfo.SortIndex)
                            return 1;
                        else
                            return -1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    uint needLVA;
                    if (itemA.skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                        needLVA = CSVActiveSkillInfo.Instance.GetConfData(itemA.skillInfo.FirstInfoID).need_lv;
                    else
                        needLVA = CSVPassiveSkillInfo.Instance.GetConfData(itemA.skillInfo.FirstInfoID).need_lv;
                    uint needLVB;
                    if (itemB.skillInfo.ESkillType == Sys_Skill.ESkillType.Active)
                        needLVB = CSVActiveSkillInfo.Instance.GetConfData(itemB.skillInfo.FirstInfoID).need_lv;
                    else
                        needLVB = CSVPassiveSkillInfo.Instance.GetConfData(itemB.skillInfo.FirstInfoID).need_lv;
                    if (needLVA > needLVB)
                        return 1;
                    else if (needLVA < needLVB)
                        return -1;
                    else
                    {
                        if (itemA.skillInfo.SortIndex >= itemB.skillInfo.SortIndex)
                            return 1;
                        else
                            return -1;
                    }
                }
            });

            for (int index = 0, len = passiveSkillItems.Count; index < len; index++)
            {
                passiveSkillItems[index].root.transform.SetParent(layout.passiveSkillContent.transform, false);
                if (skillInfo != null && skillInfo.SkillID == passiveSkillItems[index].skillInfo.SkillID)
                {
                    passiveSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                }
            }
            if (skillInfo == null)
            {
                passiveSkillItems[0].skillToggle.isOn = true;
                passiveSkillItems[0].skillToggle.onValueChanged.Invoke(true);
            }
        }

        void OnUseItem()
        {
            layout.UpdateUseItemCount();
        }

        void OnTriggerFunctionOpen(Sys_FunctionOpen.FunctionOpenData funcData)
        {
            RefreshTab();
        }

        void RefreshTab()
        {
            bool isOpen = Sys_FunctionOpen.Instance.IsOpen(EFuncOpen.FO_Talent, false);
            layout.toggleTalent.gameObject.SetActive(isOpen);

            RefreshTalentDot();
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_SkillUpgrade);
        }

        bool learnButtonFlag;
        Timer learnButtonTimer;

        public void OnClickLearnButton()
        {
            if (currentSelectSkill != null)
            {
                if (learnButtonFlag)
                {
                    if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
                    {
                        Sys_Skill.Instance.ReqUpdateSkillLevel(currentSelectSkill);
                    }
                    else if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
                    {
                        Sys_Skill.Instance.ReqPassiveSkillUpdate(currentSelectSkill);
                    }
                    learnButtonFlag = false;
                    learnButtonTimer?.Cancel();
                    learnButtonTimer = Timer.Register(1f, LearnButtonTimerCallBack);
                }
            }
        }

        bool upgradeLevelButtonFlag;
        Timer upgradeLevelButtonTimer;

        public void OnClickUpgradeLevelButton()
        {
            if (currentSelectSkill != null)
            {
                if (upgradeLevelButtonFlag)
                {
                    if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
                    {
                        Sys_Skill.Instance.ReqUpdateSkillLevel(currentSelectSkill);
                    }
                    else if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
                    {
                        Sys_Skill.Instance.ReqPassiveSkillUpdate(currentSelectSkill);
                    }
                    upgradeLevelButtonFlag = false;
                    upgradeLevelButtonTimer?.Cancel();
                    upgradeLevelButtonTimer = Timer.Register(1f, UpgradeLevelButtonTimerCallBack);
                }
            }
        }

        void UpgradeLevelButtonTimerCallBack()
        {
            upgradeLevelButtonFlag = true;
        }

        void LearnButtonTimerCallBack()
        {
            learnButtonFlag = true;
        }

        public void OnClickUpgradeRankButton()
        {
            if (currentSelectSkill != null)
            {
                if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
                {
                    Sys_Skill.Instance.ReqUseItemAddASExp(currentSelectSkill);
                }
                else if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
                {
                    Sys_Skill.Instance.ReqUseItemAddPSExp(currentSelectSkill);
                }
            }
        }

        public void OnValueChangedForTalent(bool isOn)
        {
            if (isOn)
            {
                currentTab = ETab.Talent;
                if (uiTalent == null)
                {
                    uiTalent = new UI_Talent();
                    uiTalent.Init(layout.talentNode);
                    uiTalent.Refresh(this);
                }
            }

            for (int index = 0, len = activeSkillItems.Count; index < len; index++)
            {
                activeSkillItems[index].learnFx.SetActive(false);
                activeSkillItems[index].upFx.SetActive(false);
            }

            for (int index = 0, len = passiveSkillItems.Count; index < len; index++)
            {
                passiveSkillItems[index].learnFx.SetActive(false);
                passiveSkillItems[index].upFx.SetActive(false);
            }

            layout.particleSystem.gameObject.SetActive(false);
        }

        float longPressCd = 0.3f;
        float curDt;
        uint curNum = 1;
        uint maxNum = 20;

        public void OnLongPressed(float dt)
        {
            if (dt - curDt > longPressCd)
            {
                if (currentSelectSkill != null)
                {
                    if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
                    {
                        Sys_Skill.Instance.ReqUseItemAddASExp(currentSelectSkill, curNum);
                    }
                    else if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
                    {
                        Sys_Skill.Instance.ReqUseItemAddPSExp(currentSelectSkill, curNum);
                    }
                    if (curNum < maxNum)
                    {
                        curNum++;
                    }
                }
                curDt = dt;
            }
        }

        public void OnReleased()
        {
            curDt = 0f;
            curNum = 1;
        }

        public void OnClickLimitDetailButton()
        {
            layout.limitDetailRoot.SetActive(true);
            layout.UpdateLimitDetailInfo();
        }

        public void OnClickLimitDetailCloseButton()
        {
            layout.limitDetailRoot.SetActive(false);
        }

        public void OnClickActiveSkillButton()
        {
            layout.activeSkillRoot.SetActive(true);
            layout.passiveSkillRoot.SetActive(false);

            layout.activeSkillButton_L.SetActive(true);
            layout.activeSkillButton_D.SetActive(false);
            layout.passiveSkillButton_L.SetActive(false);
            layout.passiveSkillButton_D.SetActive(true);

            if (activeSkillItems != null && activeSkillItems.Count > 0)
            {
                if (initSelectSkill != null)
                {
                    for (int index = 0, len = activeSkillItems.Count; index < len; index++)
                    {
                        if (activeSkillItems[index].skillInfo.SkillID == initSelectSkill.SkillID)
                        {
                            activeSkillItems[index].skillToggle.isOn = true;
                            activeSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                        }
                    }
                }
                else
                {
                    activeSkillItems[0].skillToggle.isOn = true;
                    activeSkillItems[0].skillToggle.onValueChanged.Invoke(true);
                }
            }
        }

        public void OnClickPassiveSkillButton()
        {
            uint levelLimit = uint.Parse(CSVParam.Instance.GetConfData(953).str_value);
            //levelLimit = 100;
            if (Sys_Role.Instance.Role.Level < levelLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1002300, levelLimit.ToString()));
                return;
            }

            layout.activeSkillRoot.SetActive(false);
            layout.passiveSkillRoot.SetActive(true);

            layout.activeSkillButton_L.SetActive(false);
            layout.activeSkillButton_D.SetActive(true);
            layout.passiveSkillButton_L.SetActive(true);
            layout.passiveSkillButton_D.SetActive(false);

            if (passiveSkillItems != null && passiveSkillItems.Count > 0)
            {
                if (initSelectSkill != null)
                {
                    for (int index = 0, len = passiveSkillItems.Count; index < len; index++)
                    {
                        if (passiveSkillItems[index].skillInfo.SkillID == initSelectSkill.SkillID)
                        {
                            passiveSkillItems[index].skillToggle.isOn = true;
                            passiveSkillItems[index].skillToggle.onValueChanged.Invoke(true);
                        }
                    }
                }
                else
                {
                    passiveSkillItems[0].skillToggle.isOn = true;
                    passiveSkillItems[0].skillToggle.onValueChanged.Invoke(true);
                }
            }
        }

        public void OnClickForgetButton()
        {
            if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Active)
            {
                CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID);
                if (cSVActiveSkillInfoData != null)
                {
                    if (cSVActiveSkillInfoData.restitution == null)
                    {
                        ForgetActiveSkill(680000752, cSVActiveSkillInfoData);
                    }
                    else if (cSVActiveSkillInfoData.restitution.Count == 1)
                    {
                        ForgetActiveSkill(680000753, cSVActiveSkillInfoData);
                    }
                    else if (cSVActiveSkillInfoData.restitution.Count == 2)
                    {
                        ForgetActiveSkill(680000754, cSVActiveSkillInfoData);
                    }
                }
            }
            else if (currentSelectSkill.ESkillType == Sys_Skill.ESkillType.Passive)
            {
                CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID);
                if (cSVPassiveSkillInfoData != null)
                {
                    if (cSVPassiveSkillInfoData.restitution == null)
                    {
                        ForgetPassiveSkill(680000752, cSVPassiveSkillInfoData);
                    }
                    else if (cSVPassiveSkillInfoData.restitution.Count == 1)
                    {
                        ForgetPassiveSkill(680000753, cSVPassiveSkillInfoData);
                    }
                    else if (cSVPassiveSkillInfoData.restitution.Count == 2)
                    {
                        ForgetPassiveSkill(680000754, cSVPassiveSkillInfoData);
                    }
                }
            }
        }

        void ForgetActiveSkill(uint languageID, CSVActiveSkillInfo.Data cSVActiveSkillInfoData)
        {
            PromptBoxParameter.Instance.Clear();

            if (cSVActiveSkillInfoData.restitution == null)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name));
            }
            else if (cSVActiveSkillInfoData.restitution.Count == 1)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVActiveSkillInfoData.restitution[0][0]).name_id), cSVActiveSkillInfoData.restitution[0][1].ToString());
            }
            else if (cSVActiveSkillInfoData.restitution.Count == 2)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name),
                    LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVActiveSkillInfoData.restitution[0][0]).name_id),
                    cSVActiveSkillInfoData.restitution[0][1].ToString(),
                    LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVActiveSkillInfoData.restitution[1][0]).name_id),
                    cSVActiveSkillInfoData.restitution[1][1].ToString());
            }
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Skill.Instance.ReqForgetSKill(cSVActiveSkillInfoData.skill_id);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        void ForgetPassiveSkill(uint languageID, CSVPassiveSkillInfo.Data cSVPassiveSkillInfoData)
        {
            PromptBoxParameter.Instance.Clear();
            if (cSVPassiveSkillInfoData.restitution == null)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVPassiveSkillInfoData.name));
            }
            else if (cSVPassiveSkillInfoData.restitution.Count == 1)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVPassiveSkillInfoData.name), LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVPassiveSkillInfoData.restitution[0][0]).name_id), cSVPassiveSkillInfoData.restitution[0][1].ToString());
            }
            else if (cSVPassiveSkillInfoData.restitution.Count == 2)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(languageID, LanguageHelper.GetTextContent(cSVPassiveSkillInfoData.name),
                    LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVPassiveSkillInfoData.restitution[0][0]).name_id),
                    cSVPassiveSkillInfoData.restitution[0][1].ToString(),
                    LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(cSVPassiveSkillInfoData.restitution[1][0]).name_id),
                    cSVPassiveSkillInfoData.restitution[1][1].ToString());
            }
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Skill.Instance.ReqForgetSKill(cSVPassiveSkillInfoData.skill_id);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnClickQuickButton()
        {
            if (currentSelectSkill == null)
                return;

            uint levelCostNum;
            uint rankCostNum;
            uint finalLevel;
            uint finalRank;
            long needCoin = 0;
            Sys_Skill.Instance.Get(currentSelectSkill, out levelCostNum, out rankCostNum, out finalLevel, out finalRank);

            void OnConform()
            {
                if (Sys_Bag.Instance.GetItemCount(3) < needCoin)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001992));
                }
                else
                {
                    Sys_Skill.Instance.ReqOnSwitchUpdate(currentSelectSkill);
                }
            }

            if (levelCostNum == 0 && rankCostNum == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001993));
            }
            else
            {
                string strItemCost = string.Empty;
                var levelItemData = CSVItem.Instance.GetConfData(202001);
                var rankItemData = CSVItem.Instance.GetConfData(CSVParam.Instance.UpgradeSkillItemID);
                var coinItemData = CSVItem.Instance.GetConfData(3);

                long rankBagCount = Sys_Bag.Instance.GetItemCount(CSVParam.Instance.UpgradeSkillItemID);
                long levelBagCount = Sys_Bag.Instance.GetItemCount(202001);
                if (rankBagCount >= rankCostNum)
                {

                    if (levelBagCount >= levelCostNum)
                    {
                        if (levelCostNum != 0)
                            strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankCostNum}, {LanguageHelper.GetTextContent(levelItemData.name_id)}x{levelCostNum}";
                        else
                            strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankCostNum}";
                    }
                    else
                    {
                        needCoin = (levelCostNum - levelBagCount) * CSVParam.Instance.LevelCostCoinPrice;
                        if (levelBagCount != 0)
                            strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankCostNum}, {LanguageHelper.GetTextContent(levelItemData.name_id)}x{levelBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                        else
                            strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankCostNum}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                    }
                }
                else
                {
                    if (levelBagCount >= levelCostNum)
                    {
                        needCoin = (rankCostNum - rankBagCount) * CSVParam.Instance.RankCostCoinPrice;
                        if (rankBagCount != 0)
                        {
                            if (levelBagCount != 0)
                            {
                                if (levelCostNum != 0)
                                    strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankBagCount}, {LanguageHelper.GetTextContent(levelItemData.name_id)}x{levelCostNum}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                                else
                                    strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                            else
                            {
                                strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                        }
                        else
                        {
                            strItemCost = $"{LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                        }
                    }
                    else
                    {
                        needCoin = ((rankCostNum - rankBagCount) * CSVParam.Instance.RankCostCoinPrice) + ((levelCostNum - levelBagCount) * CSVParam.Instance.LevelCostCoinPrice);
                        if (rankBagCount != 0)
                        {
                            if (levelBagCount != 0)
                            {
                                strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankBagCount}, {LanguageHelper.GetTextContent(levelItemData.name_id)}x{levelBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                            else
                            {
                                strItemCost = $"{LanguageHelper.GetTextContent(rankItemData.name_id)}x{rankBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                        }
                        else
                        {
                            if (levelBagCount != 0)
                            {
                                strItemCost = $"{LanguageHelper.GetTextContent(levelItemData.name_id)}x{levelBagCount}, {LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                            else
                            {
                                strItemCost = $"{LanguageHelper.GetTextContent(coinItemData.name_id)}x{needCoin}";
                            }
                        }
                    }
                }

                PromptBoxParameter.Instance.Clear();
                string strSkillName = string.Empty;
                if (CSVActiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID) != null)
                {
                    strSkillName = LanguageHelper.GetTextContent(CSVActiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID).name);
                }
                else if (CSVPassiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID) != null)
                {
                    strSkillName = LanguageHelper.GetTextContent(CSVPassiveSkillInfo.Instance.GetConfData(currentSelectSkill.CurInfoID).name);
                }
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001990, strItemCost, strSkillName, finalLevel.ToString(), finalRank.ToString());
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                PromptBoxParameter.Instance.SetCancel(true, null);

                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }
    }
}