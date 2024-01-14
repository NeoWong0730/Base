using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Table;
using System;

namespace Logic
{
    public class UI_Skill_Tips : UIBase
    {
        private Button btnClose;

        private PetSkillItem02 skill;
        private GameObject objPassvieTag;
        private Text textName;
        private Text textdesc;
        private Text textTip;
        private Text textMp;
        private Text textMpValue;

        private uint skillId = 0;
        private uint tipsId = 0;
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if (arg is Tuple<uint, uint> tuple)
                {
                    skillId = tuple.Item1;
                    tipsId = tuple.Item2;
                }
                else
                {
                    skillId = 0;
                    tipsId = 0;
                }
            }
            else
            {
                skillId = 0;
                tipsId = 0;
            }
        }

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Animator/Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            skill = new PetSkillItem02();
            skill.Bind(transform.Find("Animator/View_SkillTips/PetSkillItem02").gameObject);
            skill.EnableLongPress(false);
            objPassvieTag = transform.Find("Animator/View_SkillTips/Text_Passive").gameObject;
            textName = transform.Find("Animator/View_SkillTips/Text_Name").GetComponent<Text>();
            textdesc = transform.Find("Animator/View_SkillTips/bg/Text_Describe").GetComponent<Text>();
            textTip = transform.Find("Animator/View_SkillTips/bg/Text_Tips").GetComponent<Text>();
            textMp = transform.Find("Animator/View_SkillTips/Text_Mp").GetComponent<Text>();
            textMpValue = transform.Find("Animator/View_SkillTips/Text_Mp/Text").GetComponent<Text>();
        }

        protected override void OnShow()
        {            
            UpdatePanel();
        }

        protected override void OnShowEnd()
        {
            //强刷刷新布局：处理嵌套layout 自动布局与content size fitter 时，布局刷新失败。后续有更好方法可以再进行优化
            if (textdesc.transform.parent != null)
            {
                FrameworkTool.ForceRebuildLayout(textdesc.transform.parent.gameObject);
            }
        }

        private void UpdatePanel()
        {
            bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(skillId);

            textMp.gameObject.SetActive(isActiveSkill);
            objPassvieTag.SetActive(!isActiveSkill);
            if (isActiveSkill) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    skill.SetDate(skillId);
                    textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                    TextHelper.SetText(textdesc, Sys_Skill.Instance.GetSkillDesc(skillId));                  
                    CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(skillId);
                    if (null != cSVActiveSkillData)
                    {
                        textMpValue.text = cSVActiveSkillData.mana_cost.ToString();
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in activeSkill", skillId);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in skillInfo", skillId);
                }
            }
            else
            {
                
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    skill.SetDate(skillId);
                    textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                    textdesc.text = LanguageHelper.GetTextContent(skillInfo.desc);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0}", skillId);
                }
            }

            bool isShowTips = tipsId != 0;
            if (isShowTips)
            {
                textTip.text = LanguageHelper.GetTextContent(tipsId);
            }
            textTip.gameObject.SetActive(isShowTips);
        }


        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Skill_Tips);
        }
    }
}


