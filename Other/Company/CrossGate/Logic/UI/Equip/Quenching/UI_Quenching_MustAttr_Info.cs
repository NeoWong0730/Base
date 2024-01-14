using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Table;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    public class UI_Quenching_MustAttr_Info : TipEquipInfoBase
    {
        private GameObject template;

        private ItemData item;

        protected override void Parse()
        {
            template = transform.Find("View_Must_Property").gameObject;
            template.SetActive(false);
        }

        public override void OnDestroy()
        {

        }

        public void UpdateMustAttrInfo(ItemData _item, Essence mustAttr)
        {
            this.item = _item;

            Lib.Core.FrameworkTool.DestroyChildren(gameObject, template.name, lineStr);

            if (mustAttr.MustGreenID != 0)
            {
                GenAttr(LanguageHelper.GetTextContent(4081, CSVAttr.Instance.GetConfData(mustAttr.MustGreenID).name.ToString()));
            }

            if (mustAttr.GreenPlusN != 0)
            {
                GenAttr(LanguageHelper.GetTextContent(4082, mustAttr.GreenPlusN.ToString()));
            }
        }

        private void GenAttr(string strAttr)
        {
            GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);
            entryGo.SetActive(true);
            Text des = entryGo.transform.Find("Text_Property01").GetComponent<Text>();
            des.text = strAttr;
        }
    }
}



