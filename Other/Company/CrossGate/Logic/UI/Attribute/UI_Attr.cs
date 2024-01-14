using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System;

namespace Logic
{
    public class UI_Attr : UIComponent
    {
        private uint id;
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Image icon;
        private Button button;
        private Image bg;
        private bool isBasicAttr;

        public UI_Attr(uint _id, bool _isBasicAttr)
            : base()
        {
            id = _id;
            isBasicAttr = _isBasicAttr;
        }

        protected override void Loaded()
        {
            attrname = transform.Find("Attr").GetComponent<Text>();
            number = transform.Find("Attr/Text/Text_Number").GetComponent<Text>();
            addpoint = transform.Find("Attr/Text").GetComponent<Text>();
            bg = transform.Find("Image_Attr (2)").GetComponent<Image>();
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips,false, tip);
        }

        public void ShowBg(bool isShowBg)
        {
            bg.enabled = isShowBg;
        }

        public void RefreshItem( )
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            if (!isBasicAttr)
            {
                if(CSVAttr.Instance.GetConfData(id).show_type==1)
                    number.text = Sys_Attr.Instance.pkAttrs[id].ToString();
                else
                    number.text = ((float)Sys_Attr.Instance.pkAttrs[id]/100).ToString() + "%";
                addpoint.text = string.Empty;
            }
            else 
            {
                number.text = Sys_Attr.Instance.pkAttrs[id].ToString();
                int curIndex = (int)Sys_Attr.Instance.curIndex;
                int assighnum = 0;
                if (Sys_Attr.Instance.listBasePlans.Count > curIndex)
                {
                    assighnum = Sys_Attr.Instance.GetAssighPointByAttrId(id, (uint)curIndex);
                }
                int origion =(int) Sys_Attr.Instance.pkAttrs[id] - assighnum;
                if (origion >= 0)
                {
                    addpoint.text = LanguageHelper.GetTextContent(4418, assighnum.ToString(), origion.ToString());
                }
                else
                {
                    addpoint.text = LanguageHelper.GetTextContent(10013902, assighnum.ToString(), Mathf.Abs(origion).ToString());

                }
            }
        }
    }
}
