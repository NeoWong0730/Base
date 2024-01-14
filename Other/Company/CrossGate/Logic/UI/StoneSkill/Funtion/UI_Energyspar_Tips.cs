using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class EnergysparTipsParam
    {
        public uint stoneId;
        public uint index;        
        public bool isChao;
        public CSVPassiveSkillInfo.Data passiveSkillData;
    }


    public class UI_Energyspar_Tips: UIBase
    {
        private EnergysparTipsParam param;
        private Image iconImage;
        private Text chaoOrLDText;
        private Text typeText;
        private Text descText;
        private Button closeBtn;
        private Button moreInfoBtn;
        private Button changeoverbtn;

        private GameObject detailGo;
        private Text stoneAllText;
        private GameObject descGo;
        private GameObject descParentGo;
        protected override void OnLoaded()
        {
            iconImage = transform.Find("Animator/View_SkillTips/Image_Icon_Frame/Image_Icon").GetComponent<Image>();
            chaoOrLDText = transform.Find("Animator/View_SkillTips/Title_Tips/Text_Title").GetComponent<Text>();
            typeText = transform.Find("Animator/View_SkillTips/Text_Type").GetComponent<Text>();
            descText = transform.Find("Animator/View_SkillTips/Text_Describe").GetComponent<Text>();
            closeBtn = transform.Find("Black").GetComponent<Button>();
            closeBtn.onClick.AddListener(() => { CloseSelf(); });
            moreInfoBtn = transform.Find("Animator/View_SkillTips/Btn_Details").GetComponent<Button>();
            moreInfoBtn.onClick.AddListener(MoreBtnClick);
            changeoverbtn = transform.Find("Animator/View_SkillTips/Btn_Changeover").GetComponent<Button>();
            changeoverbtn.onClick.AddListener(ChangeoverBtnClick);
            detailGo = transform.Find("Animator/View_SkillTips/View_Details").gameObject;
            stoneAllText= transform.Find("Animator/View_SkillTips/View_Details/Text_Name").GetComponent<Text>();
            descGo = transform.Find("Animator/View_SkillTips/View_Details/Item_Attribute").gameObject;
            descParentGo = transform.Find("Animator/View_SkillTips/View_Details/AttributeGroup").gameObject;
        }

        protected override void OnOpen(object arg)
        {
            param = arg as EnergysparTipsParam;
        }

        protected override void OnShow()
        {
            detailGo.gameObject.SetActive(false);
            SetView();
        }

        private void SetView()
        {
            if (null != param)
            {
                if (null != param.passiveSkillData)
                {
                    ImageHelper.SetIcon(iconImage, param.passiveSkillData.icon);
                    TextHelper.SetText(descText, param.passiveSkillData.desc);

                    uint typeLd;
                    changeoverbtn.gameObject.SetActive(!param.isChao);
                    if (param.isChao)
                    {
                        typeLd = 2021051;
                        TextHelper.SetText(typeText, param.passiveSkillData.name);
                    }
                    else
                    {
                        typeLd = 2021037;
                        StageSkillUnit skillUnit = Sys_StoneSkill.Instance.GetStageSkillData(param.stoneId, param.index);
                        TextHelper.SetText(typeText, LanguageHelper.GetTextContent(skillUnit.SkillType == 0 ? 2021038u : 2021075u, (param.index + 1).ToString()));                        
                    }
                    TextHelper.SetText(chaoOrLDText, LanguageHelper.GetTextContent(typeLd));
                }
            }
        }

        private void MoreBtnClick()
        {
            bool isShow = detailGo.activeSelf;
            detailGo.gameObject.SetActive(!isShow);
            if(!isShow)
            {
                SetDetailView();
            }            
        }

        private void ChangeoverBtnClick()
        {
            if (null != param)
            {
                EnergysparChangeoverParam changeoverParam = new EnergysparChangeoverParam();
                changeoverParam.id = param.stoneId;
                changeoverParam.index = param.index;
                UIManager.OpenUI(EUIID.UI_Energyspar_Changeover, false, changeoverParam);
                CloseSelf();
            }
        }

        private void SetDetailView()
        {
            FrameworkTool.DestroyChildren(descParentGo);
            if (null != param)
            {                
                CSVStone.Data stoneData = CSVStone.Instance.GetConfData(param.stoneId);
                if (null != stoneData)
                {
                    uint langId = param.isChao ? 2021053u : 2021052u;
                    TextHelper.SetText(stoneAllText, LanguageHelper.GetTextContent(langId, LanguageHelper.GetTextContent(stoneData.stone_name)));
                    if(param.isChao)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            uint id = param.stoneId * 1000 + (uint)i;
                            CSVChaosAttr.Data chaoData = CSVChaosAttr.Instance.GetConfData(id);
                            if(null != chaoData)
                            {
                                CSVPassiveSkillInfo.Data skillData = CSVPassiveSkillInfo.Instance.GetConfData(chaoData.skill_id);
                                if(null != skillData)
                                {
                                    GameObject go = GameObject.Instantiate<GameObject>(descGo, descParentGo.transform);
                                    TextHelper.SetText(go.transform.Find("Text").GetComponent<Text>(), LanguageHelper.GetTextContent(2021068 , LanguageHelper.GetTextContent(skillData.name)));
                                    TextHelper.SetText(go.transform.Find("Value").GetComponent<Text>(), LanguageHelper.GetTextContent(skillData.desc));
                                    go.SetActive(true);
                                }
                                else
                                {
                                    DebugUtil.LogErrorFormat("CSVPassiveSkillInfo 表 未存在 id = {0}", chaoData.skill_id);
                                }
                            }
                            else
                            {
                                DebugUtil.LogErrorFormat("CSVChaosAttr 表 未存在 id = {0}", id);
                            }
                        }
                    }
                    else
                    {
                        CSVStoneStage.Data currenStageData = CSVStoneStage.Instance.GetConfData(param.stoneId * 1000 + param.index + 1);
                        if(null != currenStageData)
                        {
                            GameObject go = GameObject.Instantiate<GameObject>(descGo, descParentGo.transform);
                            TextHelper.SetText(go.transform.Find("Text").GetComponent<Text>(), LanguageHelper.GetTextContent(2021025));
                            TextHelper.SetText(go.transform.Find("Value").GetComponent<Text>(), LanguageHelper.GetTextContent(currenStageData.desc_light));
                            go.SetActive(true);

                            GameObject go2 = GameObject.Instantiate<GameObject>(descGo, descParentGo.transform);
                            TextHelper.SetText(go2.transform.Find("Text").GetComponent<Text>(), LanguageHelper.GetTextContent(2021026));
                            TextHelper.SetText(go2.transform.Find("Value").GetComponent<Text>(), LanguageHelper.GetTextContent(currenStageData.desc_dark));
                            go2.SetActive(true);
                        }
                    }
                }
            }
            FrameworkTool.ForceRebuildLayout(descParentGo);
        }
    }
}

