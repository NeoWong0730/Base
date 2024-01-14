using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;
using System.Linq;
using logic;

namespace Logic
{

    public class UI_AwakenImprint_RightView : UIComponent
    {
        #region 界面
        private Image img_SkillIcon;
        private Image img_Frame;
        private Text txt_Name;
        private Text txt_Lv;
        private GameObject go_atrrItem;
        private Text txt_Lock;
        private GameObject scrollPanel;
        private GameObject go_PartSec;
        private GameObject go_AwardItem;
        private GameObject go_LabelOne;
        private GameObject go_LabelSec;
        private GameObject go_LabelThird;
        private Image img_Icon;
        private Text txt_Icon;
        private Button btn_Update;
        private ToggleGroup tg_Group;
        private Toggle tg_One;
        private Toggle tg_Sec;
        private GameObject img_Full;

        #endregion
        private ImprintNode iNode;
        private ImprintEntry iEntry;
        private Toggle[] toggles;
        private int costType = 1;
        public bool firstState;
        private bool secondState;
        bool isBright;
        private long excessCoinNum;
        private uint needCoinNum;
        protected override void Loaded()
        {
            img_SkillIcon = transform.Find("Part1/Skill_Icon").GetComponent<Image>();
            img_Frame = transform.Find("Part1/Image1").GetComponent<Image>();
            txt_Name = transform.Find("Part1/Text1").GetComponent<Text>();
            txt_Lv = transform.Find("Part1/Text2").GetComponent<Text>();
            go_atrrItem = transform.Find("Part1/Scroll View_1/Viewport/Content/Object/Item").gameObject;
            txt_Lock = transform.Find("Part1/Scroll View_1/Viewport/Content/Text_Property").GetComponent<Text>();
            scrollPanel = transform.Find("Part1/Scroll View_1/Viewport/Content").gameObject;
            go_LabelOne = transform.Find("Part1/Lable/Image1").gameObject;
            go_LabelSec = transform.Find("Part1/Lable/Image2").gameObject;
            go_LabelThird = transform.Find("Part1/Lable/Image3").gameObject;
            go_PartSec = transform.Find("Part2").gameObject;
            go_AwardItem = transform.Find("Part2/Item/Award/Item").gameObject;
            img_Icon = transform.Find("Part2/Coin/Text_Cost/Image_Coin").GetComponent<Image>();
            txt_Icon = transform.Find("Part2/Coin/Text_Cost").GetComponent<Text>();
            btn_Update = transform.Find("Btn_01").GetComponent<Button>();
            tg_Group = transform.Find("Part2/Toggle_Choice").GetComponent<ToggleGroup>();
            tg_One = transform.Find("Part2/Toggle_Choice/toggle1").GetComponent<Toggle>();
            tg_Sec = transform.Find("Part2/Toggle_Choice/toggle2").GetComponent<Toggle>();
            img_Full = transform.Find("Image_Full").gameObject;
            btn_Update.onClick.AddListener(OnUpdateButtonSelect);
            toggles = tg_Group.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length; i++)
            {
                int k = i;
                toggles[k].onValueChanged.AddListener((bool value) => SetEverToggle(value, k));
            }
        }



        public override void Show()
        {
            base.Show();
            SetPanelData();


        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        public void RefreshCoinShow()
        {
            if (Sys_Bag.Instance.GetItemCount(3)>needCoinNum)
            {
                secondState = true;
                txt_Icon.color = Sys_TravellerAwakening.Instance.FrameShow(118);
                UpdateButtonState();
            }
        }
        public void SetPanelData()
        {
            iEntry = Sys_TravellerAwakening.Instance.GetImprintEntry(Sys_TravellerAwakening.Instance.SelectIndex + 1);
            iNode = Sys_TravellerAwakening.Instance.nowNode;
            ImageHelper.SetIcon(img_SkillIcon, iNode.csv.Node_Icon);
            ImageHelper.SetImageGray(img_SkillIcon, !iNode.isActive);
            
            if (iNode.csv.Node_Type == 3)
            {
                txt_Lock.text = LanguageHelper.GetTextContent(3910010165);
            }
            else
            {
                uint _pid = iNode.GetPreNodeId(0);
                CSVImprintNode.Data _node = null;
                if (_pid != 0)
                {
                    _node = CSVImprintNode.Instance.GetConfData(_pid);
                }
                if (_node != null)
                {
                    txt_Lock.text = LanguageHelper.GetTextContent(3910010164, LanguageHelper.GetTextContent(_node.Imprint_Name), iNode.GetPreActiveGrade(0).ToString());
                }

            }
            txt_Lock.gameObject.SetActive(!iNode.isActive);
            txt_Name.text =LanguageHelper.GetTextContent(iNode.csv.Imprint_Name);
            txt_Lv.text = "Lv." + iNode.level + "/" + iNode.csv.Level_Cap;
            SetAttribute();
            LabelShow(iNode.csv.Farme);
            img_Frame.color= Sys_TravellerAwakening.Instance.FrameShow(iNode.csv.Farme);
            

        }

        private void LabelShow(uint type)
        {
            go_LabelOne.SetActive(type == 117 ? true : false);
            go_LabelSec.SetActive(type == 115 ? true : false);
            go_LabelThird.SetActive(type == 116 ? true : false);
        }
        private void SetAttribute()
        {
            FrameworkTool.DestroyChildren(go_atrrItem.transform.parent.gameObject, go_atrrItem.transform.name);
            CSVImprintUpgrade.Data uData;
            CSVImprintUpgrade.Data nData;
            bool isShow = true;
            if (iNode.level==0)
            {
                uData = null;
            }
            else
            {
                uData = CSVImprintUpgrade.Instance.GetConfData(iNode.id*100+ iNode.level);
            }
            if (iNode.level==iNode.csv.Level_Cap)
            {
                nData = null;
                isShow=false;
            }
            else
            {
                nData = CSVImprintUpgrade.Instance.GetConfData(iNode.id * 100 + iNode.level+1);
            }
            CSVImprintUpgrade.Data _iData = (nData == null) ? uData: nData;
            FrameworkTool.CreateChildList(go_atrrItem.transform.parent, _iData.Attribute_Bonus.Count);
            
            for (int i=0;i < _iData.Attribute_Bonus.Count; i++)
            {
                GameObject go = go_atrrItem.transform.parent.GetChild(i).gameObject;
                Text attrName= go.transform.Find("Text_Property").GetComponent<Text>();
                Text attrTextOne = go.transform.Find("Text_Num").GetComponent<Text>();
                Text attrTextSec = go.transform.Find("Text_Num2").GetComponent<Text>();
                GameObject go_arrow = go.transform.Find("Image_Arrow").gameObject;
                CSVAttr.Data aData = CSVAttr.Instance.GetConfData(_iData.Attribute_Bonus[i][0]);
                attrName.text = DescribeShow(((_iData.Target_Type.Count==1)? _iData.Target_Type[0]:4), aData.name);
                attrTextOne.text = TextFrame(aData.show_type,((uData == null) ?0:uData.Attribute_Bonus[i][1]));
                int styleId = iNode.ThisNodeIsMaxGrade() ? 74 : 118;
                attrTextOne.color = Sys_TravellerAwakening.Instance.FrameShow((uint)(iNode.ThisNodeIsMaxGrade() ? 74 : 118));
                txt_Lv.color = Sys_TravellerAwakening.Instance.FrameShow((uint)(iNode.ThisNodeIsMaxGrade() ? 74 : 65));
                attrTextSec.gameObject.SetActive(nData != null);
                go_arrow.SetActive(nData!=null);
                if (nData != null)
                {
                    attrTextSec.text = TextFrame(aData.show_type,nData.Attribute_Bonus[i][1]); ;
                }
            }
            for (int j=0;j<= _iData.Attribute_Bonus.Count;j++)
            {
                scrollPanel.GetComponent<VerticalLayoutGroup>().spacing = -10.5f+0.05f*j;
            }
            UpdatePaneShow(isShow,nData);

        }
        private string TextFrame(uint _type,uint _value)
        {
            string str = "";
            if (_type == 1)
            {
                str= "+" + _value;
            }
            else
            {
                str = "+" + (_value / 100) + "%";
            }
            return str;
        }
        private string DescribeShow(uint type,uint id)
        {
            StringBuilder str = new StringBuilder();
            str.Append(SetRoleText(type)).Append(LanguageHelper.GetTextContent(id));

            return str.ToString();
            
        }
        private string SetRoleText(uint type)
        {
            string str = "";
            switch(type)
            {
                case 1:
                    str = LanguageHelper.GetTextContent(3910010160);
                    break;
                   case 2:
                    str = LanguageHelper.GetTextContent(3910010161);
                    break;
                case 3:
                    str = LanguageHelper.GetTextContent(3910010162);
                    break;
                case 4:
                    str = LanguageHelper.GetTextContent(3910010163);
                    break;

            }
            return str;

        }
        private void UpdatePaneShow(bool _isShow, CSVImprintUpgrade.Data _nData)
        {
            firstState = true;
            secondState = true;
            btn_Update.gameObject.SetActive(_isShow);
            go_PartSec.SetActive(_isShow);
            img_Full.gameObject.SetActive(!_isShow);
            if (!_isShow)
            {
                return;
            }
            FrameworkTool.CreateChildList(go_AwardItem.transform.parent, _nData.Consume_Item.Count);
            for (int i=0;i<_nData.Consume_Item.Count;i++)
            {
                GameObject go = go_AwardItem.transform.parent.GetChild(i).gameObject;
                PropItem propItem = new PropItem();
                propItem.BindGameObject(go);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_AwakenImprint, new PropIconLoader.ShowItemData(_nData.Consume_Item[i][0], _nData.Consume_Item[i][1], true, false, false, false, false,
                       _bShowCount: true, _bUseClick: true, _bShowBagCount: true)));
                if (Sys_Bag.Instance.GetItemCount(_nData.Consume_Item[i][0]) < _nData.Consume_Item[i][1])
                {
                    firstState = false;
                }
            }
            ImageHelper.SetIcon(img_Icon, 992503, true);
            txt_Icon.text = _nData.Consume_Coin.ToString();
            if (Sys_Bag.Instance.GetItemCount(3)< _nData.Consume_Coin)
            {
                secondState = false;
                needCoinNum = _nData.Consume_Coin;
                excessCoinNum = _nData.Consume_Coin - Sys_Bag.Instance.GetItemCount(3);
                txt_Icon.color = Sys_TravellerAwakening.Instance.FrameShow(75);//75红色，118紫色
            }
            else
            {
                txt_Icon.color = Sys_TravellerAwakening.Instance.FrameShow(118);
            }
            if (firstState)
            {
                tg_One.isOn = true;
                costType = 0;
            }else
            {
                tg_Sec.isOn = true;
                costType = 1;
            }
            UpdateButtonState();

        }
        private void SetEverToggle(bool value, int j)
        {
            if (value)
            {
                costType = j;
                UpdateButtonState();
            }
            
        }
        private void UpdateButtonState()
        {
            isBright = iNode.isActive;
            ImageHelper.SetImageGray(btn_Update.GetComponent<Image>(), !isBright);
            btn_Update.enabled = isBright;
        }

        private void OnUpdateButtonSelect()
        {
            if (costType == 0 && !firstState)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                return;
            }
            if (costType == 1 && !secondState)
            {
                Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.SilverCoin, excessCoinNum);
                return;
                //跳转兑换银币
            }
            Sys_TravellerAwakening.Instance.OnOnAwakenImprintUpdateReq(iNode.id,(uint)costType);
        }

    }


}