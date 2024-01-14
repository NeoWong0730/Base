using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Equipment_Make_Right_Property : UIParseCommon
    {
        private GameObject template;

        private uint itemPaperId;

        protected override void Parse()
        {
            template = transform.Find("Attr").gameObject;
            template.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void UpdateInfo(ItemData itemEquip, uint  paperId)
        {
            itemPaperId = paperId;

            CSVSuit.Data suitInfo = CSVSuit.Instance.GetConfData(itemPaperId);

            Lib.Core.FrameworkTool.DestroyChildren(template.transform.parent.gameObject, template.name);

            //单个套装属性
            //if (suitInfo.effect != 0)
            {
                //GameObject propGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
                //propGo.SetActive(true);

                //Text propName = propGo.transform.Find("Text_Property").GetComponent<Text>();
                //Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                ////CSVPassiveSkill.Data skillInfo = CSVPassiveSkill.Instance.GetConfData(suitInfo.effect);

                //propName.text = LanguageHelper.GetTextContent(skillInfo.desc);
                //propValue.text = "";
            }
            //else
            
        }
    }
}


