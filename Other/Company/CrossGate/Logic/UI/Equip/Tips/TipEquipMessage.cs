using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;

namespace Logic
{
    public class TipEquipMessage : UIParseCommon
    {
        private GameObject attrTypeTemplate;
        //private GameObject titleGo;
        //private GameObject template;

        protected override void Parse()
        {
            attrTypeTemplate = transform.Find("Message_Root/View_Basis_Property/BasicPropRoot01").gameObject;
            attrTypeTemplate.SetActive(false);
        }

        public override void UpdateInfo(ItemData item)
        {
            Lib.Core.FrameworkTool.DestroyChildren(attrTypeTemplate.transform.parent.gameObject, attrTypeTemplate.name);

            if (item.Equip.BaseAttr.Count > 0)
            {
                GameObject basic = GameObject.Instantiate<GameObject>(attrTypeTemplate, attrTypeTemplate.transform.parent);
                basic.SetActive(true);

                Text attrTypeName = basic.transform.Find("Title/Image_Title_Splitline/Text_Title").GetComponent<Text>();
                TextHelper.SetText(attrTypeName, 4015);

                GameObject basicProperty = basic.transform.Find("Basis_Property").gameObject;
                basicProperty.SetActive(false);

                foreach (AttributeElem attrElem in item.Equip.BaseAttr)
                {
                    GameObject attrGo = GameObject.Instantiate<GameObject>(basicProperty, basicProperty.transform.parent);
                    attrGo.SetActive(true);

                    Text attrName = attrGo.transform.Find("Basis_Property01").GetComponent<Text>();
                    Text attrValue = attrGo.transform.Find("Number01").GetComponent<Text>();
                    Text attrArea = attrGo.transform.Find("Number02").GetComponent<Text>();

                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrElem.Attr2.Id);
                    attrName.text = LanguageHelper.GetTextContent(attrData.name);
                    attrValue.text = Sys_Attr.Instance.GetAttrValue(attrData, attrElem.Attr2.Value);

                    //属性范围,临时处理
                    CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(item.Id);

                    string minValue = string.Empty;
                    string maxValue = string.Empty;

                    foreach (List<uint> attrConf in equipData.attr)
                    {
                        if (attrConf[0] == attrElem.Attr2.Id)
                        {
                            CSVAttr.Data tempAttrData = CSVAttr.Instance.GetConfData(attrConf[0]);
                            if (tempAttrData.show_type == 2)
                            {
                                minValue = (attrConf[1] / 10000f).ToString("P2");
                                maxValue = (attrConf[2] / 10000f).ToString("P2");
                            }
                            else
                            {
                                minValue = attrConf[1].ToString();
                                maxValue = attrConf[2].ToString();
                            }
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

            //ForceRebuildLayout(gameObject);
            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}
