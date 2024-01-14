using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using System.Text;
using Packet;
namespace Logic
{
    public class UI_PetMagicCore_ArtificeResult_Layout
    {
        private Button closeBtn;
        private GameObject fashionGo;
        private Text itemNameText;
        private Image itemIcon;
        private Text suitSkillNameText;
        private Text suitSkillDesText;
        private Text suitFashionNameText;
        private Text suitFashionDesText;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Image_Black").GetComponent<Button>();
            fashionGo = transform.Find("Animator/Attr_Fashion").gameObject;
            itemIcon = transform.Find("Animator/Image_Successbg/Image_Result/Icon").GetComponent<Image>();
            itemNameText = transform.Find("Animator/Image_Successbg/Image_Result/Name").GetComponent<Text>();
            suitSkillNameText =transform.Find("Animator/Attr_Set/Name/Value").GetComponent<Text>();
            suitSkillDesText = transform.Find("Animator/Attr_Set/Text_Des").GetComponent<Text>();
            suitFashionNameText = transform.Find("Animator/Attr_Fashion/Name/Value").GetComponent<Text>();
            suitFashionDesText = transform.Find("Animator/Attr_Fashion/Text_Des").GetComponent<Text>();
        }
        

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
        }

        public void SetView(Item itemData)
        {
            CSVItem.Data itemConfigData = CSVItem.Instance.GetConfData(itemData.Id);
            if(null != itemConfigData)
            {
                ImageHelper.SetIcon(itemIcon, itemConfigData.icon_id);
                TextHelper.SetText(itemNameText, itemConfigData.name_id);
            }
            uint suitSkillId = itemData.PetEquip.SuitSkill;
            uint suitAppearanceId = itemData.PetEquip.SuitAppearance;
            CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
            if(null != suitSkillData)
            {
                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(suitSkillData.upgrade_skill);
                if (isActiveSkill) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(suitSkillNameText, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
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
                        TextHelper.SetText(suitSkillNameText, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
                        TextHelper.SetText(suitSkillDesText, 680000912, LanguageHelper.GetTextContent(skillInfo.name), LanguageHelper.GetTextContent(skillInfo.desc));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                    }
                }
            }
            bool hasSuitAppearanceId = suitAppearanceId > 0;
            if(hasSuitAppearanceId)
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

                    TextHelper.SetText(suitFashionNameText, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000913, LanguageHelper.GetTextContent(suitAppearanceData.name))));
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

    public class UI_PetMagicCore_ArtificeResult : UIBase, UI_PetMagicCore_ArtificeResult_Layout.IListener
    {
        private UI_PetMagicCore_ArtificeResult_Layout layout = new UI_PetMagicCore_ArtificeResult_Layout();
        private Item itemData;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<Item>(Sys_Pet.EEvents.OnSmeltItemShow, ChangeShowItem, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if(null != arg)
            {
                itemData = arg as Item;
            }
        }

        private void ChangeShowItem(Item item)
        {
            itemData = item;
            if (null != itemData)
            {
                layout.SetView(itemData);
            }
        }

        protected override void OnShow()
        {
            if(null != itemData)
            {
                layout.SetView(itemData);
            }
            
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_PetMagicCore_ArtificeResult);
        }

    }
}