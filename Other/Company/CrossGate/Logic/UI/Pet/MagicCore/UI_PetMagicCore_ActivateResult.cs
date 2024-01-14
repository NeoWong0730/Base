using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using System.Text;
namespace Logic
{
    public class UI_PetMagicCore_ActivateResult_Layout
    {
        private Button closeBtn;
        private GameObject fashionGo;
        private Text suitSkillNameText;
        private Text suitSkillDesText;
        private Text suitFashionNameText;
        private Text suitFashionDesText;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Image").GetComponent<Button>();
            fashionGo = transform.Find("Animator/Image_BG2/Content/Fashion").gameObject;

            suitSkillNameText =transform.Find("Animator/Image_BG2/Content/Set/Text_Value").GetComponent<Text>();
            suitSkillDesText = transform.Find("Animator/Image_BG2/Content/Set/Set_Des").GetComponent<Text>();

            suitFashionNameText = transform.Find("Animator/Image_BG2/Content/Fashion/Text_Value").GetComponent<Text>();
            suitFashionDesText = transform.Find("Animator/Image_BG2/Content/Fashion/Fashion_Des").GetComponent<Text>();
        }
        

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
        }

        public void SetView(uint suitSkillId, uint suitAppearanceId)
        {
            CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
            if (null != suitSkillData)
            {
                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(suitSkillData.upgrade_skill);
                if (isActiveSkill) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(suitSkillNameText, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name)));
                        TextHelper.SetText(suitSkillDesText, 680000912, LanguageHelper.GetTextContent(skillInfo.name), Sys_Skill.Instance.GetSkillDesc(suitSkillData.upgrade_skill));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(suitSkillNameText, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name)));
                        TextHelper.SetText(suitSkillDesText, 680000912, LanguageHelper.GetTextContent(skillInfo.name), LanguageHelper.GetTextContent(skillInfo.desc));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                    }
                }
            }
            bool hasSuitAppearanceId = suitAppearanceId > 0;
            if (hasSuitAppearanceId)
            {
                CSVPetEquipSuitAppearance.Data suitAppearanceData = CSVPetEquipSuitAppearance.Instance.GetConfData(suitAppearanceId);

                if (null != suitAppearanceData)
                {
                    StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
                    for (int i = 0; i < suitAppearanceData.pet_id.Count; i++)
                    {
                        CSVPetNew.Data pet = CSVPetNew.Instance.GetConfData(suitAppearanceData.pet_id[i]);
                        stringBuilder.Append(LanguageHelper.GetTextContent(pet.name));
                        if (i != suitAppearanceData.pet_id.Count - 1)
                        {
                            stringBuilder.Append("、");
                        }
                    }

                    TextHelper.SetText(suitFashionNameText, LanguageHelper.GetTextContent(680000913, LanguageHelper.GetTextContent(suitAppearanceData.name)));
                    TextHelper.SetText(suitFashionDesText, 680000914, LanguageHelper.GetTextContent(suitAppearanceData.name), StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));
                }
            }
            fashionGo.SetActive(hasSuitAppearanceId);
        }

        public interface IListener
        {
            void CloseBtnClicked();
        }
    }

    public class UI_PetMagicCore_ActivateResult : UIBase, UI_PetMagicCore_ActivateResult_Layout.IListener
    {
        private UI_PetMagicCore_ActivateResult_Layout layout = new UI_PetMagicCore_ActivateResult_Layout();
        private uint petEquipSuitSkillId;
        private uint petEquipSuitAppearanceId;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                Tuple<uint, uint> tuple = arg as Tuple<uint, uint>;
                petEquipSuitSkillId = tuple.Item1;
                petEquipSuitAppearanceId = tuple.Item2;
            }
        }

        protected override void OnShow()
        {
            layout.SetView(petEquipSuitSkillId, petEquipSuitAppearanceId);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_PetMagicCore_ActivateResult);
        }

    }
}