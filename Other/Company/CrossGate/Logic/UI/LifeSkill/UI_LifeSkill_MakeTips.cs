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
    public class UI_LifeSkill_MakeTips : UIBase
    {
        private GameObject view1;
        private GameObject view2;
        private GameObject view3;
        private RectTransform imageBG1;
        private RectTransform imageBG2;
        private RectTransform imageBG3;
        private Transform view2parent;
        private Transform view3parnet;
        private Transform baseParentRoot;
        private Transform greenParentRoot;
        private Transform effectParentRoot;
        private GameObject imageArrow1_green;
        private GameObject imageArrow2_green;
        private GameObject imageArrow1_effect;
        private GameObject imageArrow2_effect;
        private Image formulaIcon;
        private Text formulaName;
        private Text formulaLevel;
        private Text equipLevel;
        private Button green;
        private Button effect;
        private Text greenNormal;
        private Text greenIntensity;
        private Text effectNormal;
        private Text effectIntensity;
        private Text career;
        private uint formulaId;
        private CSVFormula.Data cSVFormulaData;
        private CSVEquipment.Data cSVEquipmentData;
        private ScrollRect scrollRect;
        private bool showGreen;
        private bool ShowGreen
        {
            get { return showGreen; }
            set
            {
                if (showGreen != value)
                {
                    showGreen = value;
                    view2.SetActive(showGreen);
                    if (showGreen)
                    {
                        imageArrow1_green.SetActive(false);
                        imageArrow2_green.SetActive(true);
                        _ShowGreen();
                        ShowEffect = false;
                    }
                    else
                    {
                        imageArrow1_green.SetActive(true);
                        imageArrow2_green.SetActive(false);
                    }
                }
            }
        }

        private bool showeffect;
        private bool ShowEffect
        {
            get { return showeffect; }
            set
            {
                if (showeffect != value)
                {
                    showeffect = value;
                    view3.SetActive(showeffect);
                    if (showeffect)
                    {
                        imageArrow1_effect.SetActive(false);
                        imageArrow2_effect.SetActive(true);
                        _ShowEffect();
                        ShowGreen = false;
                    }
                    else
                    {
                        imageArrow1_effect.SetActive(true);
                        imageArrow2_effect.SetActive(false);
                    }
                }
            }
        }

        private GameObject arrowUp;
        private GameObject arrowDown;


        protected override void OnOpen(object arg)
        {
            formulaId = (uint)arg;
            cSVFormulaData = CSVFormula.Instance.GetConfData(formulaId);
        }

        protected override void OnLoaded()
        {
            view1 = transform.Find("Animator/Tips01").gameObject;
            view2 = transform.Find("Animator/Tips02").gameObject;
            view3 = transform.Find("Animator/Tips03").gameObject;

            arrowUp = transform.Find("Animator/Tips03/Image_BG/Image_Title_Splitline/Image_Arrow1").gameObject;
            arrowDown = transform.Find("Animator/Tips03/Image_BG/Image_Title_Splitline/Image_Arrow2").gameObject;

            imageBG1 = view1.transform.Find("Image_BG").transform as RectTransform;
            imageBG2 = view2.transform.Find("Image_BG").transform as RectTransform;
            imageBG3 = view3.transform.Find("Image_BG").transform as RectTransform;

            view2parent = view2.transform.Find("Image_BG/Scroll_View/GameObject");
            view3parnet = view3.transform.Find("Image_BG/Scroll_View/GameObject");


            baseParentRoot = view1.transform.Find("Image_BG/View_Basic_Prop");
            greenParentRoot = view1.transform.Find("Image_BG/View_Greed_Prop");
            effectParentRoot = view1.transform.Find("Image_BG/View_Effect_Prop");

            green = greenParentRoot.transform.Find("Basis_Property/Btn_Tips").GetComponent<Button>();
            effect = effectParentRoot.transform.Find("Basis_Property/Btn_Tips").GetComponent<Button>();

            imageArrow1_green = greenParentRoot.transform.Find("Basis_Property/Image_Arrow1").gameObject;
            imageArrow2_green = greenParentRoot.transform.Find("Basis_Property/Image_Arrow2").gameObject;
            imageArrow1_effect = effectParentRoot.transform.Find("Basis_Property/Image_Arrow1").gameObject;
            imageArrow2_effect = effectParentRoot.transform.Find("Basis_Property/Image_Arrow2").gameObject;

            scrollRect = transform.Find("Animator/Tips03/Image_BG/Scroll_View").GetComponent<ScrollRect>();
            scrollRect.onValueChanged.AddListener(OnValueChanged);
            formulaIcon = view1.transform.Find("Image_BG/Item_Message/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            formulaName = view1.transform.Find("Image_BG/Item_Message/Text_Name").GetComponent<Text>();
            formulaLevel = view1.transform.Find("Image_BG/Item_Message/Text_Type").GetComponent<Text>();
            equipLevel = view1.transform.Find("Image_BG/Item_Message/Text_Type2").GetComponent<Text>();
            greenNormal = greenParentRoot.transform.Find("Basis_Property/Number1").GetComponent<Text>();
            greenIntensity = greenParentRoot.transform.Find("Basis_Property/Number2").GetComponent<Text>();
            effectNormal = effectParentRoot.transform.Find("Basis_Property/Number1").GetComponent<Text>();
            effectIntensity = effectParentRoot.transform.Find("Basis_Property/Number2").GetComponent<Text>();
            career = view1.transform.Find("Image_BG/Item_Message/Text_Profession").GetComponent<Text>();
            green.onClick.AddListener(() => { ShowGreen = !ShowGreen; });
            effect.onClick.AddListener(() => { ShowEffect = !ShowEffect; });
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_LifeSkill_MakeTips); });

            arrowUp.SetActive(false);
            arrowDown.SetActive(true);
        }

        protected override void OnShow()
        {
            RefreshTitle();
            RefreshAttr();
            ShowGreen = false;
            ShowEffect = false;
        }

        private void RefreshTitle()
        {
            ImageHelper.SetIcon(formulaIcon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
            TextHelper.SetText(formulaName, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id);
            TextHelper.SetText(formulaLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, cSVFormulaData.level_formula.ToString()));
            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
            TextHelper.SetText(equipLevel, LanguageHelper.GetTextContent(2010141, cSVEquipmentData.equipment_level.ToString()));
        }

        private void RefreshAttr()
        {
            uint normalEquipId = cSVFormulaData.forge_success_equip[0];
            uint normalEquipParam = cSVFormulaData.forge_success_equip[1];
            uint intensifyEquipId = cSVFormulaData.intensify_forge_equip[0];
            uint intensifyEquipParam = cSVFormulaData.intensify_forge_equip[1];

            cSVEquipmentData = CSVEquipment.Instance.GetConfData(normalEquipId);
            CSVEquipmentParameter.Data cSVEquipmentParameterData_normal = CSVEquipmentParameter.Instance.GetConfData(normalEquipParam);
            CSVEquipmentParameter.Data cSVEquipmentParameterData_intensify = CSVEquipmentParameter.Instance.GetConfData(intensifyEquipParam);

            string careerName = string.Empty;
            if (cSVEquipmentData.career_condition == null)
            {
                career.text = string.Format(string.Format(CSVLanguage.Instance.GetConfData(4012).words, CSVLanguage.Instance.GetConfData(4183).words));
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
                career.text = string.Format(CSVLanguage.Instance.GetConfData(4012).words, careerName);
            }

            int needCount = cSVEquipmentData.attr.Count;
            FrameworkTool.CreateChildList(baseParentRoot, needCount);
            LayoutRebuilder.ForceRebuildLayoutImmediate(imageBG1);
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseParentRoot.transform as RectTransform);
            int childCount = baseParentRoot.childCount;
            for (int i = 0; i < needCount; i++)
            {
                List<uint> attr = cSVEquipmentData.attr[i];
                Transform trans = baseParentRoot.GetChild(i);
                Text name = trans.Find("Basis_Property01").GetComponent<Text>();
                Text normal = trans.Find("Number1").GetComponent<Text>();
                Text intensify = trans.Find("Number2").GetComponent<Text>();

                TextHelper.SetText(name, CSVAttr.Instance.GetConfData(attr[0]).name);

                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr[1]);

                string low_normal = GetAttrStr(attr[0], (uint)(cSVEquipmentParameterData_normal.lower_limit / 100f * attr[1]));
                string high_normal = GetAttrStr(attr[0], (uint)(cSVEquipmentParameterData_normal.upper_limit / 100f * attr[2]));
                normal.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, low_normal, high_normal);
                string low_intensify = GetAttrStr(attr[0], (uint)(cSVEquipmentParameterData_intensify.lower_limit / 100f * attr[1]));
                string high_intensify = GetAttrStr(attr[0], (uint)(cSVEquipmentParameterData_intensify.upper_limit / 100f * attr[2]));

                intensify.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, low_intensify, high_intensify);
            }

            if (cSVEquipmentData.green_id == 0)
            {
                greenParentRoot.gameObject.SetActive(false);
            }
            else
            {
                greenParentRoot.gameObject.SetActive(true);
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010039).words, cSVEquipmentParameterData_normal.green[0][0],
                    cSVEquipmentParameterData_normal.green[cSVEquipmentParameterData_normal.green.Count - 1][0]);
                TextHelper.SetText(greenNormal, content);

                content = string.Format(CSVLanguage.Instance.GetConfData(2010039).words, cSVEquipmentParameterData_intensify.green[0][0],
                  cSVEquipmentParameterData_intensify.green[cSVEquipmentParameterData_intensify.green.Count - 1][0]);
                TextHelper.SetText(greenIntensity, content);
            }
            if (cSVEquipmentData.special_id == 0)
            {
                effectParentRoot.gameObject.SetActive(false);
            }
            else
            {
                effectParentRoot.gameObject.SetActive(true);
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010039).words, cSVEquipmentParameterData_normal.effect[0][0],
                cSVEquipmentParameterData_normal.effect[cSVEquipmentParameterData_normal.effect.Count - 1][0]);
                TextHelper.SetText(effectNormal, content);

                content = string.Format(CSVLanguage.Instance.GetConfData(2010039).words, cSVEquipmentParameterData_intensify.effect[0][0],
                  cSVEquipmentParameterData_intensify.effect[cSVEquipmentParameterData_intensify.effect.Count - 1][0]);
                TextHelper.SetText(effectIntensity, content);
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


        private void _ShowGreen()
        {
            List<uint> greenId = new List<uint>();
            foreach (var item in CSVGreen.Instance.GetAll())
            {
                if (item.group_id == cSVEquipmentData.green_id)
                {
                    greenId.Add(item.id);
                }
            }
            //int row = greenId.Count / 2;
            //int remain = greenId.Count % 2;
            int total = greenId.Count;
            FrameworkTool.CreateChildList(view2parent, total);
            LayoutRebuilder.ForceRebuildLayoutImmediate(view2parent.transform as RectTransform);
            for (int i = 0; i < total; i++)
            {
                Transform parent = view2parent.GetChild(i);
                //bool isdouble=true;
                //if (i==total-1)
                //{
                //    isdouble = remain == 0;
                //    parent.GetChild(0).gameObject.SetActive(isdouble);
                //}
                GameObject child0 = parent.GetChild(0).gameObject;
                uint id = greenId[i];
                Text name = child0.transform.Find("Basis_Property01").GetComponent<Text>();
                Text num = child0.transform.Find("Number").GetComponent<Text>();
                TextHelper.SetText(name, CSVAttr.Instance.GetConfData(CSVGreen.Instance.GetConfData(id).attr_id).name);
                num.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, CSVGreen.Instance.GetConfData(id).low, CSVGreen.Instance.GetConfData(id).up);

                //if (isdouble)
                //{
                //    GameObject child1 = parent.GetChild(0).gameObject;
                //    uint _id = greenId[i * 2+1];
                //    Text _name = child1.transform.Find("Basis_Property01").GetComponent<Text>();
                //    Text _num = child1.transform.Find("Number").GetComponent<Text>();
                //    TextHelper.SetText(_name, CSVAttr.Instance.GetConfData(CSVGreen.Instance.GetConfData(_id).attr_id).name);
                //    _num.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words,CSVGreen.Instance.GetConfData(_id).low,CSVGreen.Instance.GetConfData(_id).up);
                //}
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(imageBG2);
        }

        private void _ShowEffect()
        {
            List<uint> effectId = new List<uint>();
            foreach (var item in CSVEquipmentEffect.Instance.GetAll())
            {
                if (item.group_id == cSVEquipmentData.special_id)
                {
                    effectId.Add(item.id);
                }
            }
            int needCount = effectId.Count;
            FrameworkTool.CreateChildList(view3parnet, needCount);
            LayoutRebuilder.ForceRebuildLayoutImmediate(view3parnet.transform as RectTransform);
            for (int i = 0; i < needCount; i++)
            {
                uint effectid = effectId[i];
                GameObject go = view3parnet.GetChild(i).gameObject;
                Text name = go.transform.Find("Basis_Property01").GetComponent<Text>();
                Text des = go.transform.Find("Number").GetComponent<Text>();
                TextHelper.SetText(name, CSVEquipmentEffect.Instance.GetConfData(effectid).name);
                TextHelper.SetText(des, CSVEquipmentEffect.Instance.GetConfData(effectid).descripe);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(imageBG3);
        }


        private void OnValueChanged(Vector2 vector2)
        {
            arrowUp.SetActive(true);
            arrowDown.SetActive(true);
            if (vector2.y >= 0.99f)
            {
                arrowUp.SetActive(false);
            }
            if (vector2.y <= 0.01f)
            {
                arrowDown.SetActive(false);
            }
        }
    }
}


