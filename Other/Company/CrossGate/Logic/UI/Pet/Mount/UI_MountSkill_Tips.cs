using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using UnityEngine;
using Table;
using System;

namespace Logic
{
    public class UI_MountSkill_TipsParam
    {
        public ClientPet pet;
        public uint skillId;
        public uint tipsId;
        public bool isDetailShow;
    }

    public class UI_MountSkill_Tips : UIBase
    {
        private Button btnClose;
        private Button forgetBtn;
        private PetSkillItem02 skill;
        private Text textName;
        private Text textdesc;
        private Text textTip;
        //能量
        private Text textMp;
        private Text textMpValue;
        private Text limitValueText;
        private UI_MountSkill_TipsParam param;

        protected override void OnOpen(object arg)
        {
            if (null != arg)
                param = arg as UI_MountSkill_TipsParam;
        }

        protected override void OnLoaded()
        {
            btnClose = transform.Find("Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            forgetBtn = transform.Find("Animator/Btn_Forget").GetComponent<Button>();
            forgetBtn.onClick.AddListener(OnClickForget);
            skill = new PetSkillItem02();
            skill.Bind(transform.Find("Animator/View_SkillTips/PetSkillItem02").gameObject);
            skill.EnableLongPress(false);
            textName = transform.Find("Animator/View_SkillTips/Text_Name").GetComponent<Text>();
            textdesc = transform.Find("Animator/View_SkillTips/bg/Text_Describe").GetComponent<Text>();
            textTip = transform.Find("Animator/View_SkillTips/bg/Text_Tips").GetComponent<Text>();
            textMp = transform.Find("Animator/View_SkillTips/Text_Energy").GetComponent<Text>();
            textMpValue = transform.Find("Animator/View_SkillTips/Text_Energy/Text").GetComponent<Text>();
            limitValueText = transform.Find("Animator/View_SkillTips/Text_Limit/Text").GetComponent<Text>();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, UpdatePanel, toRegister);
        }
        
        protected override void OnShow()
        {
            UpdatePanel();
        }

        protected override void OnClose()
        {
            param = null;
        }

        protected override void OnShowEnd()
        {
            if (null != param)
            {
                //强刷刷新布局：处理嵌套layout 自动布局与content size fitter 时，布局刷新失败。后续有更好方法可以再进行优化
                if (textdesc.transform.parent != null)
                {
                    FrameworkTool.ForceRebuildLayout(textdesc.transform.parent.gameObject);
                }
            }
        }

        private void UpdatePanel()
        {
            if(null != param)
            {
                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(param.skillId);
                bool isHasPet = param.pet != null;
                forgetBtn.gameObject.SetActive(isHasPet&&!param.isDetailShow);
                
                if (isActiveSkill) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(param.skillId);
                    if (skillInfo != null)
                    {
                        skill.SetDate(param.skillId);
                        textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                        TextHelper.SetText(textdesc, Sys_Skill.Instance.GetSkillDesc(param.skillId));
                        CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(param.skillId);
                        if (null != cSVActiveSkillData)
                        {
                            textMpValue.text = cSVActiveSkillData.energy_cost.ToString();
                        }
                        else
                        {
                            Debug.LogErrorFormat("not found skillId={0} in activeSkill", param.skillId);
                        }
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in skillInfo", param.skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(param.skillId);
                    if (skillInfo != null)
                    {
                        skill.SetDate(param.skillId);
                        textMpValue.text = skillInfo.cost_energy.ToString();
                        if(isHasPet)
                        {
                            limitValueText.text = string.Format("{0}/{1}", param.pet.GetMountSkillDailyEnergy(param.skillId), skillInfo.cost_energy_limit.ToString());
                        }
                            
                        textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                        textdesc.text = LanguageHelper.GetTextContent(skillInfo.desc);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", param.skillId);
                    }
                }
                limitValueText.transform.parent.gameObject.SetActive(!isActiveSkill && isHasPet);
                bool isShowTips = param.tipsId != 0;
                if (isShowTips)
                {
                    textTip.text = LanguageHelper.GetTextContent(param.tipsId);
                }
                textTip.gameObject.SetActive(isShowTips);
            }
            
        }


        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_MountSkill_Tips);
        }

        private void OnClickForget()
        {
            if(param.pet == null)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000200u));
                return;
            }
            bool isUnique = param.pet.CheckIsMountUniqueSkillBySkillId(param.skillId);
            if (isUnique)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000223));//专属无法遗忘
                return;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680000224); //确定是否遗忘
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Pet.Instance.OnPetRidingSkillForgetReq(param.pet.GetPetUid(), (uint)param.skillId);
                OnClickClose();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }
}


