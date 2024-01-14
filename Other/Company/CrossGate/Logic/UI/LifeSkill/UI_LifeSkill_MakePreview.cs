using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class OpenLifeSkill_MakePreviewParm
    {
        public uint formulaId;
        public bool lucky;
    }

    public class UI_LifeSkill_MakePreview : UIBase
    {
        private Image m_FormulaIcon;
        private Text m_FormulaName;
        private Text m_FormulaLevel;
        private Text m_EquipLevel;
        private Text m_Career;
        private Button m_CloseButton;

        private Transform m_AttrTitleParent;
        private Transform m_QualityParent;
        private Transform m_AttrRightGreenParent;
        private Transform m_AttrRightEffectParent;
        private Transform m_AttrEmpty;
        private Transform m_AttrNotEmpty;
        private GameObject m_AttrRightGreenRoot;
        private GameObject m_AttrRightEffectRoot;
        private Text m_EmptyContent;

        private OpenLifeSkill_MakePreviewParm m_OpenLifeSkill_MakePreviewParm;
        private uint m_FormulaId;
        private bool b_Lucky;
        private CSVFormula.Data cSVFormulaData;
        private CSVEquipment.Data cSVEquipmentData;

        private List<Toggle> m_Toggles = new List<Toggle>();

        protected override void OnLoaded()
        {
            m_FormulaIcon = transform.Find("Animator/Content/Image_Equip/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            m_FormulaName = transform.Find("Animator/Content/Image_Equip/Name").GetComponent<Text>();
            m_FormulaLevel = transform.Find("Animator/Content/Image_Equip/LV").GetComponent<Text>();
            m_EquipLevel = transform.Find("Animator/Content/Image_Equip/LV2").GetComponent<Text>();
            m_Career = transform.Find("Animator/Content/Image_Equip/Job").GetComponent<Text>(); ;
            m_CloseButton = transform.Find("Animator/Image_BG/Btn_Close").GetComponent<Button>();

            m_AttrTitleParent = transform.Find("Animator/Content/List/Layout_Title");
            m_QualityParent = transform.Find("Animator/Content/List/Scroll View/Viewport/Content");
            m_AttrRightGreenParent = transform.Find("Animator/Content/Attribute/Content/Scroll View/Viewport/Content");
            m_AttrRightEffectParent = transform.Find("Animator/Content/Attribute/Content/Scroll View2/Viewport/Content");
            m_AttrEmpty = transform.Find("Animator/Content/Attribute/Empty");
            m_AttrNotEmpty = transform.Find("Animator/Content/Attribute/Content");
            m_AttrRightGreenRoot = transform.Find("Animator/Content/Attribute/Content/Scroll View").gameObject;
            m_AttrRightEffectRoot = transform.Find("Animator/Content/Attribute/Content/Scroll View2").gameObject;
            m_EmptyContent = transform.Find("Animator/Content/Attribute/Empty/View_Tips/Image_Bg03/Text_Tips").GetComponent<Text>();

            for (int i = 0; i < 5; i++)
            {
                Toggle toggle = m_QualityParent.GetChild(i).GetChild(3).Find("Tg").GetComponent<Toggle>();
                m_Toggles.Add(toggle);

                Toggle toggle1 = m_QualityParent.GetChild(i).GetChild(4).Find("Tg").GetComponent<Toggle>();
                m_Toggles.Add(toggle1);
            }

            for (int i = 0; i < m_Toggles.Count; i++)
            {
                int index = i;
                m_Toggles[i].onValueChanged.AddListener((arg) => OnValueChanged(arg, index));
            }

            m_CloseButton.onClick.AddListener(OnCloseUI);
        }

        private void OnCloseUI()
        {
            UIManager.CloseUI(EUIID.UI_LifeSkill_MakeTips);
        }

        protected override void OnOpen(object arg)
        {
            m_OpenLifeSkill_MakePreviewParm = arg as OpenLifeSkill_MakePreviewParm;
            m_FormulaId = m_OpenLifeSkill_MakePreviewParm.formulaId;
            b_Lucky = m_OpenLifeSkill_MakePreviewParm.lucky;
            cSVFormulaData = CSVFormula.Instance.GetConfData(m_FormulaId);
            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
        }

        protected override void OnShow()
        {
            RefreshTitle();
            SetProbabilty();
            RefreshAttr();
            if (cSVEquipmentData.green_id != 0)
            {
                m_Toggles[0].isOn = true;
                m_Toggles[0].transform.Find("Mask").gameObject.SetActive(true);
                ShowGreen(1);
            }
        }

        private void OnValueChanged(bool arg, int i)
        {
            if (arg)
            {
                m_Toggles[i].transform.Find("Mask").gameObject.SetActive(true);
                int index = i % 2;
                if (index == 0)//偶数 绿字
                {
                    index = i / 2;
                    ShowGreen((uint)index + 1);
                }
                else
                {
                    index = i / 2;
                    ShowSpecial(index + 1);
                }
            }
            else
            {
                m_Toggles[i].transform.Find("Mask").gameObject.SetActive(false);
            }
        }

        private void RefreshTitle()
        {
            ImageHelper.SetIcon(m_FormulaIcon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
            TextHelper.SetText(m_FormulaName, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id);
            TextHelper.SetText(m_FormulaLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, cSVFormulaData.level_formula.ToString()));
            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
            TextHelper.SetText(m_EquipLevel, LanguageHelper.GetTextContent(2010141, cSVEquipmentData.equipment_level.ToString()));

            string careerName = string.Empty;
            if (cSVEquipmentData.career_condition == null)
            {
                m_Career.text = string.Format(string.Format(CSVLanguage.Instance.GetConfData(4012).words, CSVLanguage.Instance.GetConfData(4183).words));
            }
            else
            {
                for (int i = 0; i < cSVEquipmentData.career_condition.Count; i++)
                {
                    uint careerId = cSVEquipmentData.career_condition[i];
                    string str = CSVLanguage.Instance.GetConfData(CSVCareer.Instance.GetConfData(careerId).name).words;
                    if (i == 0)
                    {
                        careerName += string.Format("{0}", str);
                    }
                    else
                    {
                        careerName += string.Format(".{0}", str);
                    }
                }
                m_Career.text = string.Format(CSVLanguage.Instance.GetConfData(4012).words, careerName);
            }
        }

        private void RefreshAttr()
        {

            int attrCount = cSVEquipmentData.attr.Count;
            if (attrCount == 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    m_AttrTitleParent.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        m_QualityParent.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else if (attrCount == 1)
            {
                for (int i = 0; i < 3; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    if (i == 0)
                    {
                        attrTrans.gameObject.SetActive(true);
                        TextHelper.SetText(name, CSVAttr.Instance.GetConfData(cSVEquipmentData.attr[i][0]).name);
                    }
                    else
                    {
                        attrTrans.gameObject.SetActive(false);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 0)
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                        else
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                    }
                }
            }
            else if (attrCount == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    if (i == 2)
                    {
                        attrTrans.gameObject.SetActive(false);
                    }
                    else
                    {
                        attrTrans.gameObject.SetActive(true);
                        TextHelper.SetText(name, CSVAttr.Instance.GetConfData(cSVEquipmentData.attr[i][0]).name);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (j == 2)
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                        else
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (attrCount == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    attrTrans.gameObject.SetActive(true);
                    TextHelper.SetText(name, CSVAttr.Instance.GetConfData(cSVEquipmentData.attr[i][0]).name);
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                CSVQualityParameter.Data cSVQualityParameterData = CSVQualityParameter.Instance.GetConfData((uint)i + 1);
                uint lower_Limit = cSVQualityParameterData.base_cor[0];//下限系数
                uint upper_Limit = cSVQualityParameterData.base_cor[1];//上限系数
                for (int j = 0; j < attrCount; j++)
                {
                    List<uint> attr = cSVEquipmentData.attr[j];
                    string lowStr = GetAttrStr(attr[0], attr[1] * lower_Limit / 10000);
                    string highStr = GetAttrStr(attr[0], attr[2] * upper_Limit / 10000);
                    Text attrText = m_QualityParent.GetChild(i).GetChild(j).Find("Text").GetComponent<Text>();
                    attrText.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, lowStr, highStr);
                }
            }

            if (cSVEquipmentData.green_id == 0)
            {
                m_AttrTitleParent.GetChild(3).gameObject.SetActive(false);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(3).gameObject.SetActive(false);
                }
            }
            else
            {
                m_AttrTitleParent.GetChild(3).gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(3).gameObject.SetActive(true);
                    Text green = m_QualityParent.GetChild(i).GetChild(3).Find("Text").GetComponent<Text>();
                    CSVQualityParameter.Data cSVQualityParameterData = CSVQualityParameter.Instance.GetConfData((uint)i + 1);
                    TextHelper.SetText(green, LanguageHelper.GetTextContent(2010039, cSVQualityParameterData.green_weight[0][0].ToString(),
                        cSVQualityParameterData.green_weight[cSVQualityParameterData.green_weight.Count - 1][0].ToString()));
                }
            }

            if (cSVEquipmentData.special_id == 0)
            {
                m_AttrTitleParent.GetChild(4).gameObject.SetActive(false);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(4).gameObject.SetActive(false);
                }
            }
            else
            {
                m_AttrTitleParent.GetChild(4).gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(4).gameObject.SetActive(true);
                    Text effect = m_QualityParent.GetChild(i).GetChild(4).Find("Text").GetComponent<Text>();
                    CSVQualityParameter.Data cSVQualityParameterData = CSVQualityParameter.Instance.GetConfData((uint)i + 1);
                    TextHelper.SetText(effect, LanguageHelper.GetTextContent(2010039, cSVQualityParameterData.special_weight[0][0].ToString(),
                        cSVQualityParameterData.special_weight[cSVQualityParameterData.special_weight.Count - 1][0].ToString()));
                }
            }

            if (cSVEquipmentData.green_id == 0 && cSVEquipmentData.special_id == 0)
            {
                m_AttrNotEmpty.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 2010167);
            }
            else
            {
                m_AttrNotEmpty.gameObject.SetActive(true);
                m_AttrEmpty.gameObject.SetActive(false);
            }
            FrameworkTool.ForceRebuildLayout(m_AttrTitleParent.gameObject);
            FrameworkTool.ForceRebuildLayout(m_QualityParent.gameObject);
        }

        private void SetProbabilty()
        {
            uint normalEquipParam = cSVFormulaData.forge_success_equip[1];
            uint intensifyEquipParam = 0;
            //if (b_Lucky)
            //{
            //    intensifyEquipParam = cSVFormulaData.lucky_forge_equip[1];
            //}
            //else
            //{
            //    intensifyEquipParam = cSVFormulaData.intensify_forge_equip[1];
            //}
            intensifyEquipParam = cSVFormulaData.intensify_forge_equip[1];
            CSVEquipmentParameter.Data cSVEquipmentParameterData_normal = CSVEquipmentParameter.Instance.GetConfData(normalEquipParam);
            CSVEquipmentParameter.Data cSVEquipmentParameterData_intensify = CSVEquipmentParameter.Instance.GetConfData(intensifyEquipParam);

            for (int i = 0; i < 5; i++)
            {
                int qualityIndex = i;
                Text rateNormal = m_QualityParent.GetChild(i).GetChild(5).Find("Rate0").GetComponent<Text>();
                Text rateIntensify = m_QualityParent.GetChild(i).GetChild(5).Find("Rate1").GetComponent<Text>();

                float sum_Normal = 0f;
                for (int j = 0; j < cSVEquipmentParameterData_normal.quality_weight.Count; j++)
                {
                    sum_Normal += cSVEquipmentParameterData_normal.quality_weight[j];
                }
                float rate_Normal = (float)cSVEquipmentParameterData_normal.quality_weight[qualityIndex] / sum_Normal;
                rate_Normal *= 100;
                rateNormal.text = rate_Normal.ToString(1) + "%";

                float sum_Intensify = 0f;
                for (int j = 0; j < cSVEquipmentParameterData_intensify.quality_weight.Count; j++)
                {
                    sum_Intensify += cSVEquipmentParameterData_intensify.quality_weight[j];
                }
                float rate_Intensify = (float)cSVEquipmentParameterData_intensify.quality_weight[qualityIndex] / sum_Intensify;
                rate_Intensify *= 100;
                rateIntensify.text = rate_Intensify.ToString(1) + "%";
            }
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

        private void ShowGreen(uint quality)
        {
            m_AttrRightEffectRoot.SetActive(false);
            m_AttrRightGreenRoot.SetActive(true);

            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
            CSVQualityParameter.Data cSVQualityParameterData = CSVQualityParameter.Instance.GetConfData(quality);
            List<uint> greenId = new List<uint>();
            foreach (var item in CSVGreen.Instance.GetAll())
            {
                if (item.group_id == cSVEquipmentData.green_id)
                {
                    greenId.Add(item.id);
                }
            }
            int needCount = greenId.Count;
            if (needCount == 0)
            {
                m_AttrNotEmpty.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 2010168);
                return;
            }
            if (cSVQualityParameterData.green_weight[0][0] == 0 && cSVQualityParameterData.green_weight[cSVQualityParameterData.green_weight.Count - 1][0] == 0)
            {
                m_AttrNotEmpty.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 2010168);
                return;
            }

            m_AttrEmpty.gameObject.SetActive(false);
            m_AttrNotEmpty.gameObject.SetActive(true);

            uint a = cSVQualityParameterData.green_range[0];
            uint b = cSVQualityParameterData.green_range[1];
            CSVGreen.Data cSVGreenData = null;
            FrameworkTool.CreateChildList(m_AttrRightGreenParent, needCount);
            for (int i = 0; i < m_AttrRightGreenParent.childCount; i++)
            {
                Transform child = m_AttrRightGreenParent.GetChild(i);
                Text attrName = child.Find("Name").GetComponent<Text>();
                Text attrNum = child.Find("Num").GetComponent<Text>();
                cSVGreenData = CSVGreen.Instance.GetConfData(greenId[i]);
                TextHelper.SetText(attrName, CSVAttr.Instance.GetConfData(cSVGreenData.attr_id).name);
                int low = cSVGreenData.low;
                int up = cSVGreenData.up;
                int showLow = low + (up - low) * (int)a / 10000;
                int showUp = low + (up - low) * (int)b / 10000;
                attrNum.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, showLow.ToString(), showUp.ToString());
            }
        }

        private void ShowSpecial(int quality)
        {
            m_AttrRightEffectRoot.SetActive(true);
            m_AttrRightGreenRoot.SetActive(false);
            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
            uint specialId = cSVEquipmentData.special_range[quality - 1];
            List<uint> effectId = new List<uint>();
            foreach (var item in CSVEquipmentEffect.Instance.GetAll())
            {
                if (item.group_id == specialId)
                {
                    effectId.Add(item.id);
                }
            }
            int needCount = effectId.Count;
            if (needCount == 0)
            {
                m_AttrNotEmpty.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 2010169);
                return;
            }

            m_AttrEmpty.gameObject.SetActive(false);
            m_AttrNotEmpty.gameObject.SetActive(true);

            FrameworkTool.CreateChildList(m_AttrRightEffectParent, needCount);
            for (int i = 0; i < needCount; i++)
            {
                uint effectid = effectId[i];
                Transform go = m_AttrRightEffectParent.GetChild(i);
                Text name = go.transform.Find("Basis_Property01").GetComponent<Text>();
                Text des = go.transform.Find("Number").GetComponent<Text>();
                TextHelper.SetText(name, CSVEquipmentEffect.Instance.GetConfData(effectid).name);
                TextHelper.SetText(des, CSVEquipmentEffect.Instance.GetConfData(effectid).descripe);
            }
        }
    }
}


