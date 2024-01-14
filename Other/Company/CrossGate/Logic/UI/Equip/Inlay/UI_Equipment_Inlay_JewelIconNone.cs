using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using System;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Inlay_JewelIconNone : UIParseCommon
    {
        //private Button btnCell;
        private PropItem propItem;
        private Text textName;

        private List<GameObject> propList = new List<GameObject>();
        private uint jewelType;

        protected override void Parse()
        {
            //btnCell = transform.Find("IconRoot").GetComponent<Button>();
            //btnCell.onClick.AddListener(OnClickCell);

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

        //private void OnClickCell()
        //{
        //    //ItemData itemData = new ItemData(0,)
        //    //UIManager.OpenUI(EUIID.UI_Prop_Message, false, new Tuple<ItemData, bool>(bagCeilGrid.mItemData, true));
        //}

        public void ConstructJewel(uint jewelType)
        {
            this.jewelType = jewelType;

            uint jewelId = 0;
            switch ((EJewelType)this.jewelType)
            {
                case EJewelType.One:
                    jewelId = 200101;
                    break;
                case EJewelType.Two:
                    jewelId = 200121;
                    break;
                case EJewelType.Three:
                    jewelId = 200141;
                    break;
                case EJewelType.Four:
                    jewelId = 200161;
                    break;
                case EJewelType.Five:
                    jewelId = 200181;
                    break;
                case EJewelType.Six:
                    jewelId = 200201;
                    break;
                case EJewelType.Seven:
                    jewelId = 200221;
                    break;
                case EJewelType.Eight:
                    jewelId = 200241;
                    break;
                case EJewelType.Nine:
                    jewelId = 200261;
                    break;
                case EJewelType.Ten:
                    jewelId = 200281;
                    break;
                case EJewelType.Eleven:
                    jewelId = 200301;
                    break;
                case EJewelType.Twelve:
                    jewelId = 200321;
                    break;
                default:
                    break;
            }

            CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(jewelId);

            propItem.SetData(new MessageBoxEvt( EUIID.UI_Equipment, new PropIconLoader.ShowItemData(jewelId, 1, true, false, false, false, false, false, true, true)));
            textName.text = LanguageHelper.GetTextContent(jewelItem.name_id);

            CSVJewel.Data jewelInfoData = CSVJewel.Instance.GetConfData(jewelId);

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