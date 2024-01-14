using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Logic
{
    public class PetTipsEx
    {
        public bool isActive;
        public uint skillId;
    }
    //public class UI_Skill_Tips : UIBase
    //{
    //    private PetTipsEx petTipsEx;
    //    private Image iconImage;
    //    private Image qualityImage;
    //    private Button closeBtn;
    //    private Text nameText;
    //    private Text desc;
    //    private GameObject mana_cost;
    //    private Text manaCostText;
    //    protected override void OnOpen(object arg)
    //    {
    //        petTipsEx = arg as PetTipsEx;
    //    }

    //    protected override void OnLoaded()
    //    {
    //        iconImage = transform.Find("Animator/View_SkillTips/Image_Icon_Frame/Image_Icon").GetComponent<Image>();
    //        qualityImage = transform.Find("Animator/View_SkillTips/Image_Icon_Frame/Image_Quality").GetComponent<Image>();
    //        closeBtn = transform.Find("Animator/Black").GetComponent<Button>();
    //        closeBtn.onClick.AddListener(CloseTip);
    //        mana_cost = transform.Find("Animator/View_SkillTips/Text_Mp").gameObject;
    //        manaCostText = transform.Find("Animator/View_SkillTips/Text_Mp/Text").GetComponent<Text>();
    //        desc = transform.Find("Animator/View_SkillTips/Text_Describe").GetComponent<Text>();
    //        nameText = transform.Find("Animator/View_SkillTips/Text_Name").GetComponent<Text>();
    //    }

    //    private void CloseTip()
    //    {
    //        CloseSelf();
    //    }

    //    protected override void OnShow()
    //    {
    //        if(petTipsEx.isActive)
    //        {               
    //            CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(petTipsEx.skillId);
    //            if(null != cSVActiveSkillInfoData)
    //            {
    //                ImageHelper.SetIcon(iconImage, cSVActiveSkillInfoData.icon);
    //                ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)cSVActiveSkillInfoData.quality);
    //                nameText.text = LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name);
    //                desc.text = LanguageHelper.GetTextContent(cSVActiveSkillInfoData.desc);

    //                CSVActiveSkill.Data cSVActiveSkillData = CSVActiveSkill.Instance.GetConfData(cSVActiveSkillInfoData.active_skillid);
    //                if(null != cSVActiveSkillData)
    //                {
    //                    manaCostText.text = cSVActiveSkillData.mana_cost.ToString();
    //                }
    //            }

    //        }
    //        else
    //        {
    //            CSVPassiveSkillInfo.Data cSVPassiveSkillData = CSVPassiveSkillInfo.Instance.GetConfData(petTipsEx.skillId);
    //            if (null != cSVPassiveSkillData)
    //            {
    //                ImageHelper.SetIcon(iconImage, cSVPassiveSkillData.icon);
    //                ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)cSVPassiveSkillData.quality);
    //                nameText.text = LanguageHelper.GetTextContent(cSVPassiveSkillData.name);
    //                desc.text = LanguageHelper.GetTextContent(cSVPassiveSkillData.desc);
    //            }
    //        }
    //        mana_cost.SetActive(petTipsEx.isActive);
    //    }
    //}
}
