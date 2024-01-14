using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Pet_DemonSuccessParam
    {
        public uint petSoulBeadOpType;
        public uint skill1;
        public uint skill2;
        public PetSoulBeadInfo petSoulBeadInfo;
    }
    public class UI_Pet_DemonSuccess_Layout
    {
        private Button closeBtn;
        public Transform topTransform;
        public Text titleText;

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Image_Black").GetComponent<Button>();
            topTransform = transform.Find("Animator/View_BG");
            titleText = transform.Find("Animator/View_BG/Text_Title").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
        }
    }

    public class UI_Pet_DemonSuccessSkill : UIComponent
    {
        private GameObject leftPresentSkillGo;
        private GameObject rightPresentSkillGo;
        private PetSoulBeadInfo sphereTemp;
        protected CSVSoulBead.Data sphereData;

        protected override void Loaded()
        {
            leftPresentSkillGo = transform.Find("Skill1").gameObject;
            rightPresentSkillGo = transform.Find("Skill2").gameObject;
        }

        public void SetView(PetSoulBeadInfo sphereTemp,uint _skill1, uint _skill2)
        {
            if (null != sphereTemp)
            {
                this.sphereTemp = sphereTemp;
                uint level = sphereTemp.Level;
                var sphereId = sphereTemp.Type * 10000 + level;
                sphereData = CSVSoulBead.Instance.GetConfData(sphereId);

                uint skill1 = sphereTemp.SkillIds[0];
                uint skill2 = sphereTemp.SkillIds[1];
                SetPreSentSkill(leftPresentSkillGo, skill1, _skill1, 1);
                SetPreSentSkill(rightPresentSkillGo, skill2, _skill2, 2);
            }
        }

        private void SetPreSentSkill(GameObject skillGo, uint skillId, uint oldSkillId, uint type)
        {
            bool hasSkill = skillId > 0;
            var levelText = skillGo.transform.Find("Unlock/Image_Level/Text_Level").GetComponent<Text>();
            var lockGo = skillGo.transform.Find("Lock").gameObject;
            var UnlockGo = skillGo.transform.Find("Unlock").gameObject;

            var lable = skillGo.transform.Find("Unlock/Lable").gameObject;

            if (hasSkill)
            {
                var preData = CSVSoulBead.Instance.GetConfData(sphereData.id - 1);
                bool isNew = false;
                bool isLevelUp = false;
                if(type == 1)
                {
                    isNew = null == preData;
                }
                else
                {
                    isNew = null == preData.special_skill_group;
                }
                isLevelUp = !isNew && (skillId > oldSkillId);
                lable.SetActive(isNew || isLevelUp);
                var lableGreen = lable.transform.Find("Image_Green").gameObject;
                var lableYellow = lable.transform.Find("Image_Yellow").gameObject;
                lableGreen.SetActive(isNew);
                lableYellow.SetActive(isLevelUp);
                if (isLevelUp)
                {
                    lable.transform.Find("Text_Level").GetComponent<Text>().text = LanguageHelper.GetTextContent(680002050);//等级提升
                }
                else if(isNew)
                {
                    lable.transform.Find("Text_Level").GetComponent<Text>().text = LanguageHelper.GetTextContent(680002051);//新解锁
                }
                 

                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);

                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skillGo.transform.Find("Unlock/Text_Name").GetComponent<Text>(), skillInfo.name.ToString());
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("Unlock/PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("Unlock/PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                        TextHelper.SetText(skillGo.transform.Find("Unlock/Text_Name/Text").GetComponent<Text>(), Sys_Skill.Instance.GetSkillDesc(skillId));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        skillGo.transform.Find("Unlock/Text_Name").GetComponent<Text>().text = LanguageHelper.GetTextContent(skillInfo.name);
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("Unlock/PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("Unlock/PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                        TextHelper.SetText(skillGo.transform.Find("Unlock/Text_Name/Text").GetComponent<Text>(), LanguageHelper.GetTextContent(skillInfo.desc));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                lockGo?.SetActive(false);
            }
            else
            {
                var lockData = CSVSoulBead.Instance.GetSkillLockLevelData(sphereData.type);
                if (null != lockData)
                {
                    TextHelper.SetText(skillGo.transform.Find("Lock/Text_Level").GetComponent<Text>(), 680003040, LanguageHelper.GetTextContent(680003010u + sphereData.type - 1), lockData.level.ToString());//解锁等级
                    lockGo?.SetActive(true);
                }
            }
            UnlockGo.SetActive(hasSkill);
        }
    }


    public class UI_Pet_DemonSuccess : UIBase, UI_Pet_DemonSuccess_Layout.IListener
    {
        private UI_Pet_DemonSuccess_Layout layout = new UI_Pet_DemonSuccess_Layout();
        private UI_Pet_DemonSuccessSkill demonDetailTop;
        private UI_Pet_DemonSuccessParam param;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            demonDetailTop = AddComponent<UI_Pet_DemonSuccessSkill>(layout.topTransform);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            param = arg as UI_Pet_DemonSuccessParam;
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(null != param.petSoulBeadInfo)
            {
                if (param.petSoulBeadOpType == (uint)PetSoulBeadOpType.SoulBeadOpTypeActive)
                {
                    layout.titleText.text = LanguageHelper.GetTextContent(680002053);
                }
                else 
                {
                    layout.titleText.text = LanguageHelper.GetTextContent(680002052);
                }
                demonDetailTop.SetView(param.petSoulBeadInfo, param.skill1, param.skill2);
            }
        }


        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_DemonSuccess);
        }
    }
}