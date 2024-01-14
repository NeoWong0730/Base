using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using System;

namespace Logic
{
    public class UI_Equipment_Inlay_JewelIconRoot : UIParseCommon
    {
        //public ulong UId { get; private set; } = 0;
        private Sys_Equip.JewelGroupData jewelGroup;
        //private ItemData curItem;

        private Button btnCell;
        private PropItem propItem;
        private Text textName;

        private List<GameObject> propList = new List<GameObject>();

        protected override void Parse()
        {
            btnCell = transform.Find("IconRoot").GetComponent<Button>();
            btnCell.onClick.AddListener(OnClickCell);

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);

            textName = transform.Find("Text_Name").GetComponent<Text>();

            for (int i = 0; i < 2; ++i)
            {
                string strGoName = string.Format("Text_Property{0}", i);
                GameObject propGo = transform.Find(strGoName).gameObject;
                propGo.SetActive(false);
                propList.Add(propGo);
            }
        }

        private void OnClickCell()
        {
            Sys_Equip.Instance.InlayJewelReq(jewelGroup.itemId);
        }

        private void OnClickJewel(PropItem item)
        {
            Sys_Equip.Instance.InlayJewelReq(jewelGroup.itemId);
        }

        public void UpdateJewel(Sys_Equip.JewelGroupData groupData)
        {
            jewelGroup = groupData;

            PropIconLoader.ShowItemData propData = new PropIconLoader.ShowItemData(jewelGroup.itemId, jewelGroup.count, true, false, false, false, false, true, false, true, OnClickJewel, false, false );
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Equipment, propData));
            propItem.OnEnableLongPress(true);
            textName.text = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(jewelGroup.itemId).name_id);

            CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelGroup.itemId);

            if (jewelInfoData.percent != 0)
            {
                propList[0].SetActive(true);

                Text propName = propList[0].GetComponent<Text>();
                Text propValue = propList[0].transform.Find("Text_Num").GetComponent<Text>();

                propName.text = LanguageHelper.GetTextContent(4085, jewelInfoData.percent.ToString());
                propValue.text = "";
            }
            else
            {
                for (int i = 0; i < jewelInfoData.attr.Count; ++i)
                {
                    if (i < propList.Count)
                    {
                        propList[i].SetActive(true);

                        Text propName = propList[i].GetComponent<Text>();
                        Text propValue = propList[i].transform.Find("Text_Num").GetComponent<Text>();

                        CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(jewelInfoData.attr[i][0]);
                        propName.text = LanguageHelper.GetTextContent(attrData.name);
                        propValue.text = Sys_Attr.Instance.GetAttrValue(attrData, jewelInfoData.attr[i][1]);
                    }
                    else
                    {
                        Debug.LogError("jewel attr count is larger!!! max=2");
                    }
                }
            }
        }
    }
}