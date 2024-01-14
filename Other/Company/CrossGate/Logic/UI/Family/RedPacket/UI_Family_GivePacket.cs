using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using Lib.Core;
using System;
using UnityEngine;
using Packet;

namespace Logic
{
    /// <summary> 家族红包发送 </summary>
    public class UI_Family_GivePacket : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
                InitData();
        }
        protected override void OnOpen(object arg)
        {

        }
        protected override void OnClose()
        {
            Sys_Family.Instance.oldBless = string.Empty;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btn_Close;
        Image packetIcon;
        Transform toggle;
        CP_ToggleRegistry toggleRegistry;
        Button btn_Detail;

        Text moneyTextTitle;
        InputField moneyInputField;
        Button moneyBtnAdd;
        Button moneyBtnMinus;

        Text numTextTitle;
        InputField numInputField;
        Button numBtnAdd;
        Button numBtnMinus;

        Text minTextTitle;
        Text minValue;
        Text maxTextTitle;
        Text maxValue;

        InputField contentInputField;
        Text tips;
        Button btn_Roll;
        Button btn_Send;
        #endregion
        #region 数据
        CSVFamilyPacketLimit.Data curOwnerData;
        ERedPacketType curRedPacketType;
        uint curMoneyValue;
        uint curNumValue;
        string curContent;
        uint moneyMinValue;
        uint numMinValue;
        uint moneyMaxValue;
        uint numMaxValue;
        #endregion
        #region 组件查找、事件
        private void OnParseComponent()
        {
            btn_Close = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            packetIcon = transform.Find("Animator/Content/Image/Image").GetComponent<Image>();
            toggle= transform.Find("Animator/Content/Toggle");
            toggleRegistry = toggle.GetComponent<CP_ToggleRegistry>();
            btn_Detail = transform.Find("Animator/Content/Btn_Detail").GetComponent<Button>();
            moneyTextTitle = transform.Find("Animator/Content/Money/Text_Title").GetComponent<Text>();
            moneyInputField = transform.Find("Animator/Content/Money/InputField").GetComponent<InputField>();
            moneyBtnAdd = transform.Find("Animator/Content/Money/Btn_Add").GetComponent<Button>();
            moneyBtnMinus = transform.Find("Animator/Content/Money/Btn_Minus").GetComponent<Button>();
            numTextTitle = transform.Find("Animator/Content/Num/Text_Title").GetComponent<Text>();
            numInputField = transform.Find("Animator/Content/Num/InputField").GetComponent<InputField>();
            numBtnAdd = transform.Find("Animator/Content/Num/Btn_Add").GetComponent<Button>();
            numBtnMinus = transform.Find("Animator/Content/Num/Btn_Minus").GetComponent<Button>();
            minTextTitle = transform.Find("Animator/Content/Min/Text_Title").GetComponent<Text>();
            minValue = transform.Find("Animator/Content/Min/Text").GetComponent<Text>();
            maxTextTitle = transform.Find("Animator/Content/Max/Text_Title").GetComponent<Text>();
            maxValue = transform.Find("Animator/Content/Max/Text").GetComponent<Text>();
            contentInputField = transform.Find("Animator/Content/InputField").GetComponent<InputField>();
            tips = transform.Find("Animator/Content/Text").GetComponent<Text>();
            btn_Roll = transform.Find("Animator/Content/Btn_Roll").GetComponent<Button>();
            btn_Send = transform.Find("Animator/Content/Btn_01").GetComponent<Button>();

            btn_Close.onClick.AddListener(()=>CloseSelf());
            btn_Detail.onClick.AddListener(()=> { });//红包类型提示

            moneyInputField.onEndEdit.AddListener((str)=> { SetValue(0, str); });
            moneyBtnAdd.onClick.AddListener(()=> AddValue(0));
            moneyBtnMinus.onClick.AddListener(()=> MinusValue(0));

            numInputField.onEndEdit.AddListener((str) => { SetValue(1, str); });
            numBtnAdd.onClick.AddListener(() => AddValue(1));
            numBtnMinus.onClick.AddListener(() => MinusValue(1));

            contentInputField.onValueChanged.AddListener((str) => { curContent = str; });
            btn_Roll.onClick.AddListener(RefreshContent);//刷新祝福语
            btn_Send.onClick.AddListener(SendPacket);//发送

            UI_LongPressButton money_LongPressAddButton = moneyBtnAdd.gameObject.AddComponent<UI_LongPressButton>();
            money_LongPressAddButton.onLongPress.AddListener((arg) => AddValue(0));
            money_LongPressAddButton.interval = 0.5f;
            UI_LongPressButton money_LongPressMinusButton = moneyBtnMinus.gameObject.AddComponent<UI_LongPressButton>();
            money_LongPressMinusButton.onLongPress.AddListener((arg) => MinusValue(0));
            money_LongPressMinusButton.interval = 0.5f;

            UI_LongPressButton num_LongPressAddButton = numBtnAdd.gameObject.AddComponent<UI_LongPressButton>();
            num_LongPressAddButton.onLongPress.AddListener((arg) => AddValue(1));
            num_LongPressAddButton.interval = 0.5f;
            UI_LongPressButton num_LongPressMinusButton = numBtnMinus.gameObject.AddComponent<UI_LongPressButton>();
            num_LongPressMinusButton.onLongPress.AddListener((arg) => MinusValue(1));
            num_LongPressMinusButton.interval = 0.5f;

            toggleRegistry.onToggleChange += ToggleChange;
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        #endregion

        #region 界面显示
        private void InitData()
        {
            toggle.Find("tg0/Text").GetComponent<Text>().text=LanguageHelper.GetTextContent(2023966);
            toggle.Find("tg1/Text").GetComponent<Text>().text=LanguageHelper.GetTextContent(11962);
            moneyTextTitle.text = LanguageHelper.GetTextContent(2023967);
            numTextTitle.text = LanguageHelper.GetTextContent(2023968);
            minTextTitle.text = LanguageHelper.GetTextContent(2023969);
            maxTextTitle.text = LanguageHelper.GetTextContent(2023970);

            curOwnerData = Sys_Family.Instance.GetRedPacketLimitData();
            toggleRegistry.SwitchTo((int)ERedPacketType.Normal, false, false);
            //SetDefault();
        }
        private void SetDefault()
        {
            moneyMinValue = curOwnerData.Item_Num;
            moneyMaxValue = curOwnerData.Packet_Day_Quota;
            curMoneyValue = moneyMinValue;
            SetCurMoneyValue();
            ImageHelper.SetIcon(packetIcon, "");//红包封面 读配置
            minValue.text = moneyMinValue.ToString();//红包最小金额
            maxValue.text = moneyMaxValue.ToString();//红包最大金额
            contentInputField.text = curContent;
            contentInputField.placeholder.GetComponent<Text>().text = LanguageHelper.GetTextContent(11991);
        }
        private void ToggleChange(int curId, int oldId)
        {
            curRedPacketType = (ERedPacketType)curId;
            if (curRedPacketType == ERedPacketType.Normal)
            {
                curContent = LanguageHelper.GetTextContent(uint.Parse(CSVParam.Instance.GetConfData(1138).str_value));
                tips.text = LanguageHelper.GetTextContent(2023971);
                btn_Roll.gameObject.SetActive(false);
            }
            else
            {
                curContent = LanguageHelper.GetTextContent(Sys_Family.Instance.GetVoicePacketBlessing());
                tips.text = LanguageHelper.GetTextContent(2023972);
                btn_Roll.gameObject.SetActive(true);
            }
            SetDefault();
        }
        private void RefreshContent()
        {
            curContent = LanguageHelper.GetTextContent(Sys_Family.Instance.GetVoicePacketBlessing(1));
            contentInputField.text = curContent;
        }
        private void SetValue(int type,string str)
        {
            if (type == 0)
            {
                if (str == string.Empty)
                    curMoneyValue = moneyMinValue;
                else
                {
                    curMoneyValue = uint.Parse(str);
                    if (curMoneyValue >= moneyMaxValue)
                        curMoneyValue = moneyMaxValue;
                    else if (curMoneyValue <= moneyMinValue)
                        curMoneyValue = moneyMinValue;
                }
                SetCurMoneyValue();
            }
            else
            {
                if (str == string.Empty)
                    curNumValue = numMinValue;
                else
                {
                    curNumValue = uint.Parse(str);
                    if (curNumValue >= numMaxValue)
                        curNumValue = numMaxValue;
                    else if (curNumValue <= numMinValue)
                        curNumValue = numMinValue;
                }
                numInputField.text = curNumValue.ToString();
            }
        }
        public void AddValue(int type)
        {
            if (type == 0)//红包金额加
            {
                curMoneyValue += curMoneyValue * uint.Parse(CSVParam.Instance.GetConfData(1139).str_value) / 10000;
                if (curMoneyValue >= moneyMaxValue)
                    curMoneyValue = moneyMaxValue;
                SetCurMoneyValue();
            }
            else//红包份数加
            {
                curNumValue += 1;
                if (curNumValue >= numMaxValue)
                    curNumValue = numMaxValue;
                numInputField.text = curNumValue.ToString();
            }
        }
        public void MinusValue(int type)
        {
            if (type == 0)//红包金额减
            {
                curMoneyValue -= curMoneyValue * uint.Parse(CSVParam.Instance.GetConfData(1139).str_value)/10000;
                if (curMoneyValue <= moneyMinValue)
                    curMoneyValue = moneyMinValue;
                SetCurMoneyValue();
            }
            else//红包份数减
            {
                curNumValue -= 1;
                if (curNumValue <= numMinValue)
                    curNumValue = numMinValue;
                numInputField.text = curNumValue.ToString();
            }
        }
        private void SetCurMoneyValue()
        {
            moneyInputField.text = curMoneyValue.ToString();
            CSVFamilyPacketSection.Data packetSectionData=Sys_Family.Instance.GetCurPacketSectionData(curMoneyValue);
            numMinValue = packetSectionData.Packet_Part;
            numMaxValue = packetSectionData.Packet_Part + packetSectionData.Packet_Extra_Part;
            curNumValue = numMinValue;
            numInputField.text = curNumValue.ToString();
        }
        #endregion

        #region Function
        private void SendPacket()
        {
            if (curContent.Length < 2)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11993));
                return;
            }
            if (curContent.Length > 16)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11992));
                return;
            }
            Sys_Family.Instance.OnSendRedPacketReq(0, curContent, curNumValue, (uint)curRedPacketType, curMoneyValue, false);
            CloseSelf();
        }
        #endregion
    }
}