using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Tips_Preview : UIBase
    {
        private class AttrRange
        {
            public uint attrId;
            public int a; //最小值
            public int b; //最大值
            public int attrValue; //
        }

        private Dictionary<uint, AttrRange> dictAttr = new Dictionary<uint, AttrRange>();

        private Button btnClose;
        private GameObject attrParent;
        private GameObject attrTemplate;
        private List<string> tempList = new List<string>();

        private ItemData curItem;

        //global param 
        private float m, n, p, q;

        protected override void OnLoaded()
        {            
            btnClose = transform.Find("Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            attrParent = transform.Find("Message/Message_Root/View_Basis_Property").gameObject;
            attrTemplate = attrParent.transform.Find("Basis_Property").gameObject;
            attrTemplate.SetActive(false);
            tempList.Add(attrTemplate.name);

            string[] param = CSVParam.Instance.GetConfData(201).str_value.Split('|');
            float.TryParse(param[0], out p);
            float.TryParse(param[1], out m);
            float.TryParse(param[2], out n);
            float.TryParse(param[3], out q);
        }

        protected override void OnOpen(object arg)
        {            
            curItem = (ItemData)arg;
        }

        protected override void OnShow()
        {            
            UpdatePanel();
        }        

        private void UpdatePanel()
        {
            if (curItem == null)
            {
                Debug.LogError("jewel is  null");
                return;
            }

            dictAttr.Clear();
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(curItem.Id);
            if (equipInfo.attr != null)
            {
                for (int i = 0; i < equipInfo.attr.Count; ++i)
                {
                    uint attrId = equipInfo.attr[i][0];
                    uint minValue = equipInfo.attr[i][1];
                    uint maxValue = equipInfo.attr[i][2];

                    AttrRange attr = new AttrRange();
                    attr.attrId = attrId;
                    attr.a = (int)minValue;
                    attr.b = (int)maxValue;

                    if (!dictAttr.ContainsKey(attr.attrId))
                        dictAttr.Add(attr.attrId, attr);
                    else
                        Debug.LogError("equip attr is null");
                }
            }

            attrParent.DestoryAllChildren(tempList, true);

            for (int i = 0; i < curItem.Equip.BaseAttr.Count; ++i)
            {
                AttributeElem elem = curItem.Equip.BaseAttr[i];
                if (dictAttr.ContainsKey(elem.Attr2.Id))
                {
                    dictAttr[elem.Attr2.Id].attrValue = elem.Attr2.Value;
                    GenAttr(elem.Attr2);
                }
            }

            //绿字属性
            for (int i = 0; i < curItem.Equip.GreenAttr.Count; ++i)
            {
                uint attrId = curItem.Equip.GreenAttr[i].Attr2.Id;
                CSVGreen.Data greenData = CSVGreen.Instance.GetGreenData(equipInfo.green_id, attrId);

                AttrRange attr = new AttrRange();
                attr.attrId = attrId;
                attr.a = greenData.low;
                attr.b = greenData.up;
                attr.attrValue = curItem.Equip.GreenAttr[i].Attr2.Value;

                if (dictAttr.ContainsKey(attr.attrId))
                    dictAttr[attr.attrId] = attr;
                else
                    dictAttr.Add(attr.attrId, attr);

                GenAttr(curItem.Equip.GreenAttr[i].Attr2);
            }
        }

        private void GenAttr(AttributeRow row)
        {
            long minValue = 0;
            long maxValue = 0;
            CalRange(row.Value, dictAttr[row.Id], ref minValue, ref maxValue);

            minValue -= dictAttr[row.Id].attrValue;
            maxValue -= dictAttr[row.Id].attrValue;

            GameObject attrGo = GameObject.Instantiate<GameObject>(attrTemplate, attrParent.transform);
            attrGo.SetActive(true);
            Text textName = attrGo.transform.Find("Basis_Property01").GetComponent<Text>();
            Text textNum = attrGo.transform.Find("Number01").GetComponent<Text>();

            CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(row.Id);
            textName.text = LanguageHelper.GetTextContent(attrData.name);
            textNum.text = LanguageHelper.GetTextContent(4042, Sys_Attr.Instance.GetAttrValue(attrData, minValue), Sys_Attr.Instance.GetAttrValue(attrData, maxValue));
        }

        private void CalRange(long x, AttrRange range, ref long minValue, ref long maxValue)
        {
            minValue = (long)(((1 - p) * x) + (range.a * p));
            double divisor = (n * range.a) + (m * range.b) - (q * range.b * (m + n));
            double dividend = ((long)range.a - (long)range.b) * ((long)range.a - (long)range.b) * (n + m);
            maxValue = (long)((divisor / dividend) * (x - (long)range.b) * (x - (long)range.b) + q * range.b);
        }

        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_Tips_Preview);
        }
    }
}
