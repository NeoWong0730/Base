using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;

namespace Logic
{
    public class UI_Equipment_Make_Right_Property_After : UIParseCommon
    {
        private GameObject template;
        private ItemData curEquip;

        protected override void Parse()
        {
            template = transform.Find("Attr_Grid/Attr").gameObject;
            template.SetActive(false);

            transform.Find("Button_Replace").GetComponent<Button>().onClick.AddListener(OnClickReplaceAttr);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void OnClickReplaceAttr()
        {
            Sys_Equip.Instance.OnBuildReplaceAttrReq(curEquip.Uuid);
        }

        public  void UpdateInfo(ItemData itemEquip, uint suitId)
        {
            curEquip = itemEquip;
            Lib.Core.FrameworkTool.DestroyChildren(template.transform.parent.gameObject, template.name);

            foreach (AttributeElem attrEle in itemEquip.Equip.SuitAttrTmp)
            {
                uint attrId = attrEle.Attr2.Id;
                int attrValue = attrEle.Attr2.Value;

                GameObject propGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                propGo.SetActive(true);

                Text propName = propGo.transform.Find("Text_Property").GetComponent<Text>();
                Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                propName.text = LanguageHelper.GetTextContent(attrData.name);
                propValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrValue);
            }

            //套装属性
            uint suitNum = Sys_Equip.Instance.CalSuitNumber(suitId);

            List<CSVSuitEffect.Data>  allEffects = CSVSuitEffect.Instance.GetSuitEffectList(suitId, Sys_Role.Instance.Role.Career);
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


