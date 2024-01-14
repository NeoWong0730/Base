using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Lib.Core;
using System.Text;

namespace Logic
{
    /// <summary>
    /// 宠物装备装备详情信息
    /// </summary>
    public class TipPetEquipMessage : UIParseCommon
    {
        private GameObject attrTypeTemplate;//基础属性预览
        private GameObject specicalEffectTemplate;//特效属性预览
        private GameObject suitAppearanceTemplate;//套装属性预览
        //private GameObject titleGo;
        //private GameObject template;

        protected override void Parse()
        {
            attrTypeTemplate = transform.Find("Message_Root/View_Basis_Property/BasicPropRoot01").gameObject;
            
            attrTypeTemplate.SetActive(false);

            specicalEffectTemplate = transform.Find("Message_Root/View_SpecicalEffects_Prop/BasicPropRoot01").gameObject;

            specicalEffectTemplate.SetActive(false);

            suitAppearanceTemplate = transform.Find("Message_Root/View_Set_Prop/BasicPropRoot01").gameObject;

            suitAppearanceTemplate.SetActive(false);
        }

        public override void UpdateInfo(ItemData item)
        {
            Lib.Core.FrameworkTool.DestroyChildren(attrTypeTemplate.transform.parent.gameObject, attrTypeTemplate.name);
            //生成基础属性预览
            bool hasBaseAttr = item.petEquip.BaseAttr.Count > 0;
            if (hasBaseAttr)
            {
                GameObject basic = GameObject.Instantiate<GameObject>(attrTypeTemplate, attrTypeTemplate.transform.parent);
                basic.SetActive(true);

                Text attrTypeName = basic.transform.Find("Title/Image_Title_Splitline/Text_Title").GetComponent<Text>();
                TextHelper.SetText(attrTypeName, 4015);

                GameObject basicProperty = basic.transform.Find("Basis_Property").gameObject;
                basicProperty.SetActive(false);

                foreach (PetEquip.Types.BaseAttr attrElem in item.petEquip.BaseAttr)
                {
                    GameObject attrGo = GameObject.Instantiate<GameObject>(basicProperty, basicProperty.transform.parent);
                    attrGo.SetActive(true);

                    Text attrName = attrGo.transform.Find("Basis_Property01").GetComponent<Text>();
                    Text attrValue = attrGo.transform.Find("Number01").GetComponent<Text>();
                    Text attrArea = attrGo.transform.Find("Number02").GetComponent<Text>();

                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrElem.AttrId);
                    attrName.text = LanguageHelper.GetTextContent(attrData.name);
                    attrValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrElem.AttrValue);

                    List<List<uint>> attrNameId = Sys_Pet.Instance.GetPetEquipPreviewAttrIdById(item.Id);
                    //属性范围,临时处理
                    CSVPetEquip.Data equipData = CSVPetEquip.Instance.GetConfData(item.Id);

                    CSVPetEquipQualityParameter.Data cSVQualityParameterData = CSVPetEquipQualityParameter.Instance.GetConfData(item.petEquip.Color);
                    string minValue = string.Empty;
                    string maxValue = string.Empty;
                    uint lower_Limit = cSVQualityParameterData.base_cor[0];//下限系数
                    uint upper_Limit = cSVQualityParameterData.base_cor[1];//上限系数
                    for (int i = 0; i < attrNameId.Count; i++)
                    {
                        List<uint> attr = attrNameId[i];
                        if (attr[0] == attrElem.AttrId)
                        {
                            minValue = GetAttrStr(attr[0], attr[1] * lower_Limit / 10000);
                            maxValue = GetAttrStr(attr[0], attr[2] * upper_Limit / 10000);
                            break;
                        }
                    }

                    if (!string.IsNullOrEmpty(minValue) && !string.IsNullOrEmpty(maxValue))
                    {
                        TextHelper.SetText(attrArea, 4042, minValue, maxValue);
                    }
                    else
                    {
                        attrArea.text = "";
                    }
                }
            }
            attrTypeTemplate.transform.parent.gameObject.SetActive(hasBaseAttr);

            Lib.Core.FrameworkTool.DestroyChildren(specicalEffectTemplate.transform.parent.gameObject, specicalEffectTemplate.name);
            //生成特效属性预览
            bool hasEffectAttr = item.petEquip.EffectAttr.Count > 0;
            if (hasEffectAttr)
            {
                GameObject effect = GameObject.Instantiate<GameObject>(specicalEffectTemplate, specicalEffectTemplate.transform.parent);
                effect.SetActive(true);

                GameObject effectProperty = effect.transform.Find("Describle01").gameObject;
                effectProperty.SetActive(false);

                foreach (PetEquip.Types.EffectAttr attrElem in item.petEquip.EffectAttr)
                {
                    GameObject attrGo = GameObject.Instantiate<GameObject>(effectProperty, effectProperty.transform.parent);
                    attrGo.SetActive(true);

                    Text attrName = attrGo.transform.Find("Text0/Text_Name").GetComponent<Text>();
                    Text attrValue = attrGo.transform.Find("Text0").GetComponent<Text>();
                    var petEquipEffect = CSVPetEquipEffect.Instance.GetConfData(attrElem.InfoId);
                    if (null != petEquipEffect)
                    {
                        var passiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(petEquipEffect.effect);
                        if (null != passiveSkillInfo)
                        {
                            TextHelper.SetText(attrName, passiveSkillInfo.name);
                            TextHelper.SetText(attrValue, passiveSkillInfo.desc);
                        }
                        else
                        {
                            DebugUtil.LogError($"Not Find Id = {petEquipEffect.effect} In Table CSVPassiveSkillInfo");
                        }
                    }
                    else
                    {
                        DebugUtil.LogError($"Not Find Id = {attrElem.InfoId} In Table CSVPetEquipEffect");
                    }
                }
            }
            specicalEffectTemplate.transform.parent.gameObject.SetActive(hasEffectAttr);

            Lib.Core.FrameworkTool.DestroyChildren(suitAppearanceTemplate.transform.parent.gameObject, suitAppearanceTemplate.name);
            //生成套装外观预览
            bool hasSuitAppearanceSkill = item.petEquip.SuitSkill > 0;
            if (hasSuitAppearanceSkill)
            {
                GameObject suitapperanceSkill = GameObject.Instantiate<GameObject>(suitAppearanceTemplate, suitAppearanceTemplate.transform.parent);
                suitapperanceSkill.SetActive(true);

                GameObject suitapperanceSkillProperty = suitapperanceSkill.transform.Find("Describle01").gameObject;
                suitapperanceSkillProperty.SetActive(false);

                uint suitSkillId = item.petEquip.SuitSkill;

                GameObject attrGo = GameObject.Instantiate<GameObject>(suitapperanceSkillProperty, suitapperanceSkillProperty.transform.parent);
                attrGo.SetActive(true);

                Text skillName = attrGo.transform.Find("Text0").GetComponent<Text>();
                Text skillDes = attrGo.transform.Find("Text1").GetComponent<Text>();


                CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(suitSkillId);
                if (null != suitSkillData)
                {
                    bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(suitSkillData.upgrade_skill);
                    if (isActiveSkill) //主动技能
                    {
                        CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                        if (skillInfo != null)
                        {
                            TextHelper.SetText(skillName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
                            TextHelper.SetText(skillDes, 680000912, LanguageHelper.GetTextContent(skillInfo.name), Sys_Skill.Instance.GetSkillDesc(suitSkillData.upgrade_skill));
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
                            TextHelper.SetText(skillName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name))));
                            TextHelper.SetText(skillDes, 680000912, LanguageHelper.GetTextContent(skillInfo.name), LanguageHelper.GetTextContent(skillInfo.desc));
                        }
                        else
                        {
                            Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                        }
                    }
                }

                uint suitAppearanceId = item.petEquip.SuitAppearance;
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

                        GameObject fashionGo = GameObject.Instantiate<GameObject>(suitapperanceSkillProperty, suitapperanceSkillProperty.transform.parent);
                        fashionGo.SetActive(true);

                        Text fashionName = fashionGo.transform.Find("Text0").GetComponent<Text>();
                        Text fashionDes = fashionGo.transform.Find("Text1").GetComponent<Text>();

                        TextHelper.SetText(fashionName, LanguageHelper.GetTextContent(680000919, LanguageHelper.GetTextContent(680000913, LanguageHelper.GetTextContent(suitAppearanceData.name))));
                        TextHelper.SetText(fashionDes, 680000914, LanguageHelper.GetTextContent(suitAppearanceData.name), StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));
                    }
                }
            }
            suitAppearanceTemplate.transform.parent.gameObject.SetActive(hasSuitAppearanceSkill);
            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }

        private string GetAttrStr(uint attr, uint value)
        {
            CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr);
            if (cSVAttrData.show_type == 1)
            {
                return string.Format("{0}", value);
            }
            else
            {
                return string.Format("{0}%", value / 100f);
            }
        }
    }
}
