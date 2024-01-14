using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Equipment_Make_Right_Property_Before : UIParseCommon
    {
        private GameObject template;
        private GameObject noneTip;

        //private uint itemPaperId;

        protected override void Parse()
        {
            template = transform.Find("Attr_Grid/Attr").gameObject;
            template.SetActive(false);

            noneTip = transform.Find("Text_None").gameObject;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void UpdateInfo(ItemData itemEquip)
        {
            noneTip.SetActive(false);
            Lib.Core.FrameworkTool.DestroyChildren(template.transform.parent.gameObject, template.name);

            if (itemEquip.Equip.SuitTypeId == 0)
            {
                noneTip.SetActive(true);
                return;
            }

            foreach (var attrInfo in itemEquip.Equip.SuitAttr)
            {
                uint attrId = attrInfo.Attr2.Id;
                int attrValue = attrInfo.Attr2.Value;

                GameObject propGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                propGo.SetActive(true);

                Text propName = propGo.transform.Find("Text_Property").GetComponent<Text>();
                Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                propName.text = LanguageHelper.GetTextContent(attrData.name);
                propValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrValue);
            }

            //套装属性
            uint suitNum = Sys_Equip.Instance.CalSuitNumber(itemEquip.Equip.SuitTypeId);

            List<CSVSuitEffect.Data>  allEffects = CSVSuitEffect.Instance.GetSuitEffectList(itemEquip.Equip.SuitTypeId, Sys_Role.Instance.Role.Career);
            foreach (CSVSuitEffect.Data effectData in allEffects)
            {
                bool isActive = effectData.num <= suitNum;
                uint colorId = Sys_Equip.Instance.IsEquiped(itemEquip) && isActive ? 4097u : 4098u;

                if (effectData.effect != 0)
                {
                    GameObject propGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                    propGo.SetActive(true);

                    Text propName = propGo.transform.Find("Text_Property").GetComponent<Text>();
                    Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(effectData.effect);

                    string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                    propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);
                    string valueContent = LanguageHelper.GetTextContent(skillInfo.desc);
                    propValue.text = LanguageHelper.GetLanguageColorWordsFormat(valueContent, colorId);
                }
                else
                {
                    foreach (var attrInfo in effectData.attr)
                    {
                        uint attrId = attrInfo[0];
                        uint attrValue = attrInfo[1];

                        GameObject propGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                        propGo.SetActive(true);

                        Text propName = propGo.transform.Find("Text_Property").GetComponent<Text>();
                        Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                        string nameContent = LanguageHelper.GetTextContent(4096, effectData.num.ToString());
                        propName.text = LanguageHelper.GetLanguageColorWordsFormat(nameContent, colorId);
                        string valueContent = string.Format("{0} {1}", LanguageHelper.GetTextContent(attrData.name), Sys_Attr.Instance.GetAttrValue(attrData, attrValue));
                        propValue.text = LanguageHelper.GetLanguageColorWordsFormat(valueContent, colorId);
                    }
                }
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(template.transform.parent.gameObject);
        }
    }
}


