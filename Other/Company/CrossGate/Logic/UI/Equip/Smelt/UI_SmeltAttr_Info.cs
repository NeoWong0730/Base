using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Packet;
using Google.Protobuf.Collections;

namespace Logic
{
    public class  UIAttrInfo
    {
        public AttributeElem attrElem;
        public long diff;
    }

    public class UI_SmeltAttr_Info : TipEquipInfoBase
    {
        private GameObject template;
        private List<InfoAttrEntry> listEntry = new List<InfoAttrEntry>();

        protected override void Parse()
        {
            template = transform.Find("PropertyTemplate").gameObject;
            template.SetActive(false);
        }

        public  void UpdateAttrInfo(ItemData item, List<UIAttrInfo> attrList)
        {
            listEntry.Clear();

            Lib.Core.FrameworkTool.DestroyChildren(gameObject, template.name, lineStr);

            int entryCount = CalEntryCount(attrList.Count);
            for (int i = 0; i < entryCount; ++i)
            {
                GameObject entryGo = GameObject.Instantiate<GameObject>(template, template.transform.parent);

                InfoAttrEntry entryleft = new InfoAttrEntry();
                entryleft.root = entryGo.transform.Find("Basis_PropertyLeft").gameObject;
                entryleft.name = entryleft.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entryleft.diffValue = entryleft.root.transform.Find("Change_Number").GetComponent<Text>();
                entryleft.value = entryleft.root.transform.Find("Change_Number/Number").GetComponent<Text>();
                entryleft.upArrow = entryleft.root.transform.Find("Image_UpArrow").gameObject;
                entryleft.downArrow = entryleft.root.transform.Find("Image_DownArrow").gameObject;

                listEntry.Add(entryleft);

                InfoAttrEntry entryright = new InfoAttrEntry();
                entryright.root = entryGo.transform.Find("Basis_PropertyRight").gameObject;
                entryright.name = entryright.root.transform.Find("Basis_Property01").GetComponent<Text>();
                entryright.diffValue = entryright.root.transform.Find("Change_Number").GetComponent<Text>();
                entryright.value = entryright.root.transform.Find("Change_Number/Number").GetComponent<Text>();
                entryright.upArrow = entryright.root.transform.Find("Image_UpArrow").gameObject;
                entryright.downArrow = entryright.root.transform.Find("Image_DownArrow").gameObject;

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

                    listEntry[i].upArrow.gameObject.SetActive(false);
                    listEntry[i].downArrow.gameObject.SetActive(false);

                    AttributeElem equipAttr = attrList[i].attrElem;
                    //TextHelper.SetText(listEntry[i].name, CSVAttr.Instance.GetConfData(equipAttr.Attr[0].Id).name);
                    //listEntry[i].value.text = equipAttr.Attr[0].Value.ToString();
                    if (attrList[i].diff > 0)
                    {
                        TextHelper.SetText(listEntry[i].diffValue, 4062, Mathf.Abs(attrList[i].diff).ToString());
                        listEntry[i].upArrow.gameObject.SetActive(true);
                    }
                    else if (attrList[i].diff < 0)
                    {
                        TextHelper.SetText(listEntry[i].diffValue, 4061, Mathf.Abs(attrList[i].diff).ToString());
                        listEntry[i].downArrow.gameObject.SetActive(true);
                    }
                    else
                    {
                        listEntry[i].diffValue.text = "";
                    }

                    //判断是否为绿字属性
                    bool isGreen = false;
                    foreach (AttributeElem greenAttr in item.Equip.GreenAttr)
                    {
                        if (greenAttr.Attr2.Id == equipAttr.Attr2.Id)
                        {
                            isGreen = true;
                            break;
                        }
                    }

                    string name = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(equipAttr.Attr2.Id).name);
                    if (isGreen)
                    {
                        TextHelper.SetText(listEntry[i].name, 4071, name);
                        TextHelper.SetText(listEntry[i].value, 4073, equipAttr.Attr2.Value.ToString());
                    }
                    else
                    {
                        TextHelper.SetText(listEntry[i].name, 4070, name);
                        TextHelper.SetText(listEntry[i].value, 4072, equipAttr.Attr2.Value.ToString());
                    }
                }
            }
        }
    }
}

