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
    public class UI_Quenching_BasicAttr_Info : TipEquipInfoBase
    {
        private GameObject template;
        private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        private ItemData item;

        protected override void Parse()
        {
            template = transform.Find("View_Basis_Property").gameObject;
            template.SetActive(false);
        }

        public override void OnDestroy()
        {

        }

        public void UpdateQuenchingInfo(ItemData _item, RepeatedField<AttributeRow> attrList)
        {
            this.item = _item;
            listEntry.Clear();
            Lib.Core.FrameworkTool.DestroyChildren(gameObject, template.name, lineStr);

            int entryCount = (attrList.Count + 1) / 2; ;
            for (int i = 0; i < entryCount; ++i)
            {
                GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);

                InfoAttrEntry entryleft = new InfoAttrEntry();
                entryleft.root = entryGo.transform.Find("Basis_PropertyLeft").gameObject;
                entryleft.name = entryleft.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entryleft.value = entryleft.root.transform.Find("Number").GetComponent<Text>();

                listEntry.Add(entryleft);

                InfoAttrEntry entryright = new InfoAttrEntry();
                entryright.root = entryGo.transform.Find("Basis_PropertyRight").gameObject;
                entryright.name = entryright.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entryright.value = entryright.root.transform.Find("Number").GetComponent<Text>();

                listEntry.Add(entryright);

                entryGo.SetActive(true);
            }

            //refresh
            for (int i = 0; i < listEntry.Count; ++i)
            {
                if (i >= attrList.Count)
                {
                    listEntry[i].root.SetActive(false);
                }
                else
                {
                    listEntry[i].root.SetActive(true);

                    AttributeRow equipAttr = attrList[i];

                    listEntry[i].attrId = equipAttr.Id;
                    listEntry[i].attrValue = equipAttr.Value;

                    TextHelper.SetText(listEntry[i].name, CSVAttr.Instance.GetConfData(listEntry[i].attrId).name);
                    listEntry[i].value.text = listEntry[i].attrValue.ToString();
                }
            }
        }
    }
}



