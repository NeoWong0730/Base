using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Inlay_Middle_Attr : UIParseCommon
    {
        private GameObject attrTemplate;

        private Dictionary<uint, uint> dicAttr = new Dictionary<uint, uint>();

        protected override void Parse()
        {
            attrTemplate = transform.Find("Attr_Grid/Attr").gameObject;
            attrTemplate.SetActive(false);
        }

        public override void Show()
        {

        }

        public override void Hide()
        {

        }

        public override void OnDestroy()
        {
  
        }

        public override void UpdateInfo(ItemData _item)
        {
            Lib.Core.FrameworkTool.DestroyChildren(attrTemplate.transform.parent.gameObject, attrTemplate.name);
            dicAttr.Clear();

            bool isPercent = false;
            uint percentValue = 0;

            foreach (uint jewelInfoId in _item.Equip.JewelinfoId)
            {
                if (jewelInfoId == 0)
                    continue;

                //ItemData jewelItem = Sys_Equip.Instance.GetItemData(jewelUId);
                CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(jewelInfoId);

                if (jewelInfo.percent != 0) //策划确认，百分比属性只有一个
                {
                    isPercent = true;
                    percentValue += jewelInfo.percent;
                }
                else
                {
                    foreach (var attrData in jewelInfo.attr)
                    {
                        uint attrId = attrData[0];
                        uint attrValue = attrData[1];

                        if (dicAttr.ContainsKey(attrId))
                        {
                            dicAttr[attrId] += attrValue;
                        }
                        else
                        {
                            dicAttr.Add(attrId, attrValue);
                        }
                    }
                }
            }

            if (isPercent)
            {
                GameObject attrGo = GameObject.Instantiate<GameObject>(attrTemplate, attrTemplate.transform.parent);
                attrGo.SetActive(true);

                Text name = attrGo.transform.Find("Text_Property").GetComponent<Text>();
                Text value = attrGo.transform.Find("Text_Num").GetComponent<Text>();

                name.text = LanguageHelper.GetTextContent(4050);
                value.text = string.Format("+{0}%", percentValue);
            }

            foreach (var data in dicAttr)
            {
                GameObject attrGo = GameObject.Instantiate<GameObject>(attrTemplate, attrTemplate.transform.parent);
                attrGo.SetActive(true);

                Text name = attrGo.transform.Find("Text_Property").GetComponent<Text>();
                Text value = attrGo.transform.Find("Text_Num").GetComponent<Text>();

                CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(data.Key);
                name.text = LanguageHelper.GetTextContent(attrData.name);
                value.text = Sys_Attr.Instance.GetAttrValue(attrData, data.Value);
            }
        }
    }
}


