using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Table;
using System;
using Packet;

namespace Logic
{
    public class UI_Pet_DemonSkill_Tips : UIBase
    {
        private Button btnClose;
        private Button refreshBtn;

        private PetSkillItem02 skill;
        private GameObject objPassvieTag;
        private Text textName;
        private Text textdesc;
        private Text textTip;
        private Text textMp;
        private Text textMpValue;
        private uint sphereType = 0;
        private int skillIndex= 0;
        private uint sphereIndex = 0;
        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                if (arg is Tuple<uint, uint, uint> tuple)
                {
                    sphereType = tuple.Item1;
                    sphereIndex = tuple.Item2;
                    skillIndex = (int)tuple.Item3;
                }
                else
                {
                    sphereType = 0;
                    sphereIndex = 0;
                    skillIndex = 0;
                }
            }
            else
            {
                sphereType = 0;
                sphereIndex = 0;
                skillIndex = 0;
            }
        }

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            refreshBtn = transform.Find("Animator/Btn_Forget").GetComponent<Button>();
            refreshBtn.onClick.AddListener(OnClickRefresh);
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
            PetSoulBeadInfo petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(sphereType, sphereIndex);
            var skillId = petSoulBeadInfo.SkillIds[skillIndex - 1];
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
        }


        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonSkill_Tips);
        }

        private void OnClickRefresh()
        {
            UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Refresh, false, new Tuple<uint, uint, uint>(sphereType, sphereIndex, (uint)skillIndex));
            UIManager.CloseUI(EUIID.UI_Pet_DemonSkill_Tips);
        }
    }
}