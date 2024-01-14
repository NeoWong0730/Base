using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class UI_JewelIconRoot : UIParseCommon
    {
        public Sys_Equip.JewelGroupData jewelGroupData { get; private set; } = null;
        public int GridIndex  { get; private set; }

        private CP_Toggle toggle;
        private Item0_Layout layout;

        private Text jewelNum;

        private Image jewelSelect;

        private Text jewelName;
        private List<GameObject> propList = new List<GameObject>();

        private Action<Sys_Equip.JewelGroupData> selectAction;

        protected override void Parse()
        {
            toggle = gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);

            layout = new Item0_Layout();
            layout.BindGameObject(transform.Find("PropItem/Btn_Item").gameObject);
            layout.btnItem.onClick.AddListener(OnClickJewel);

            jewelNum = transform.Find("PropItem/Text_Number").GetComponent<Text>();
            jewelNum.gameObject.SetActive(true);

            jewelSelect = transform.Find("Image_Select").GetComponent<Image>();

            jewelName = transform.Find("Text_Name").GetComponent<Text>();

            propList.Add(transform.Find("Text_Property").gameObject);
            propList.Add(transform.Find("Text_Property1").gameObject);
        }

        private void OnClickJewel()
        {

        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                Sys_Equip.Instance.SelectJewelGroupIndex = GridIndex;
                selectAction?.Invoke(jewelGroupData);
            }
        }


        public void AddListener(Action<Sys_Equip.JewelGroupData> _action)
        {
            selectAction = _action;
        }

        public  void UpdateJewelInfo(Sys_Equip.JewelGroupData _groupData, int _index)
        {
            jewelGroupData = _groupData;
            GridIndex = _index;

            CSVItem.Data itemData = CSVItem.Instance.GetConfData(jewelGroupData.itemId);

            layout.SetData(itemData, false);
            TextHelper.SetText(jewelName, itemData.name_id);

            if (jewelGroupData.count > 1)
            {
                jewelNum.text = string.Format("x{0}", jewelGroupData.count.ToString());
            }
            else
            {
                jewelNum.text = "";
            }

            for (int i = 0; i < propList.Count; ++i)
                propList[i].SetActive(false);

            CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelGroupData.itemId);
            if (jewelInfoData.percent != 0)
            {
                GameObject propGo = propList[0];
                propGo.SetActive(true);
                Text propName = propGo.GetComponent<Text>();
                Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                propName.text = LanguageHelper.GetTextContent(4050);
                propValue.text = string.Format("+{0}%", jewelInfoData.percent);
            }
            else
            {
                for (int i = 0; i < jewelInfoData.attr.Count; ++i)
                {
                    GameObject propGo = propList[i];
                    propGo.SetActive(true);
                    Text propName = propGo.GetComponent<Text>();
                    Text propValue = propGo.transform.Find("Text_Num").GetComponent<Text>();

                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(jewelInfoData.attr[i][0]);
                    propName.text = LanguageHelper.GetTextContent(attrData.name);
                    propValue.text = Sys_Attr.Instance.GetAttrValue(attrData, jewelInfoData.attr[i][1]);
                }
            }

            toggle.SetSelected(Sys_Equip.Instance.SelectJewelGroupIndex == GridIndex, true);
        }
    }
}


