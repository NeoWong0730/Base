using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Logic;
using System.Text;
using Framework;
using System;

namespace Logic
{
    public class UI_ReName : UIBase
    {
        private InputField inputField;
        private Button closeBtn;
        private Button confirmBtn;
        private Button changeBackBtn;
        private GameObject go_CD;
        private GameObject go_OldName;
        private Text oldName;
        private Text txt_CD;
        private Text tips_txtOne;
        private string _newName;
        bool isShow=false;//改回按钮及曾用名是否显示
        bool isInputTick=false;//输入框CD是否开启倒计时
        uint rolelevel = 0;
        uint itemlevel = 0;
        uint price=20;
        uint totalTime=30;//改名后再次改名冷却时间（天）
        uint backTime=7; //改名后可改回曾用名冷却时间（天）
        uint renameId = 550008;
        bool isChangeBackEnable;
        bool isConfirmEnable;

        DateTime startDate;
        DateTime endTime;

        public void Init()
        {
            inputField = transform.Find("Animator/New/InputField").GetComponent<InputField>();
            closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            confirmBtn = transform.Find("Animator/Btn_01").GetComponent<Button>();
            changeBackBtn = transform.Find("Animator/Btn_02").GetComponent<Button>();
            go_CD = transform.Find("Animator/CD").gameObject;
            go_OldName = transform.Find("Animator/Old").gameObject;
            txt_CD = transform.Find("Animator/CD/Text").GetComponent<Text>();
            oldName = transform.Find("Animator/Old/Text").GetComponent<Text>();
            tips_txtOne= transform.Find("Animator/Tips/Image1/GameObject").GetComponent<Text>();
        }

        protected override void OnLoaded()
        {
            Init();
            InitParam();
            CheckExpire();
            closeBtn.onClick.AddListener(OnCloseButtonClicked);
            confirmBtn.onClick.AddListener(OnComfiremButtonClicked);
            changeBackBtn.onClick.AddListener(OnChangeBackButtonClicked);
        }
        protected override void OnShow()
        {
            ShowData();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnReName, OnReNameUpdate, toRegister);
            
        }
        protected override void OnLateUpdate(float dt, float usdt)
        {
            if (isShow)
            {
                BackNameButtonShow();
            }
            CheckExpire();
            if (Sys_Role.Instance.reNameData.Time != 0)
            {
                
                isInputTick = CheckTick(totalTime);
                isConfirmEnable = false;
                if (isInputTick)
                {
                    txt_CD.text = LanguageHelper.GetTextContent(2025123, TickNumberShow(totalTime));//输入框——{0}后可改回(具体时间)
                }
                else
                {
                    int temp = ReNameTimeShow(totalTime).Days;
                    if (temp == totalTime)
                    {
                        temp--;
                    }
                    txt_CD.text = LanguageHelper.GetTextContent(2025124, temp.ToString());
                }
            }
            


        }

        private void CheckExpire()
        {//检查是否到期
            if (Sys_Role.Instance.reNameData.Time != 0)
            {
                UpDateNameTime();
                DateTime nowtime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
                if (nowtime > endTime)
                {
                    Sys_Role.Instance.OnReNameExpireReq();//这里发到期协议
                    Sys_Role.Instance.reNameData.Time = 0;
                    Sys_Role.Instance.reNameData.IsBack = false;
                    isInputTick = false;
                    ShowData();

                }
            }

        }
        public void UpDateNameTime()
        {
            startDate = TimeManager.GetDateTime(Sys_Role.Instance.reNameData.Time);
            endTime = startDate.AddDays(totalTime);

        }
        private void ShowData()
        {
            UpDateNameTime();
            bool isTime = (Sys_Role.Instance.reNameData.Time == 0);
            bool isGoBack = Sys_Role.Instance.reNameData.IsBack;
            isShow = !isTime && !isGoBack;
            //改名且未改回时，显示改回按钮及曾用名
            changeBackBtn.gameObject.SetActive(isShow);
            go_OldName.SetActive(isShow);
            if (isShow)
            {
                oldName.text = Sys_Role.Instance.reNameData.Name.ToStringUtf8();
                BackNameButtonShow();
            }
            //时间戳不为0时，确认按钮失效,输入框cd开启
            go_CD.SetActive(!isTime);
            isConfirmEnable = isTime;
            ImageHelper.SetImageGray(confirmBtn.gameObject.GetComponent<Image>(), !isTime);
            tips_txtOne.text = LanguageHelper.GetTextContent(2025105, price.ToString());

        }

        private void InitParam()
        {//初始化参数
            
            CSVParam.Data pData = CSVParam.Instance.GetConfData(1324);
            totalTime = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1325);
            backTime = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1327);
            price= Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1353);
            itemlevel= Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1329);
            renameId = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1328);
            rolelevel = Convert.ToUInt32(pData.str_value);
        }
        private TimeSpan ReNameTimeShow(uint _index)
        {
            DateTime nowtime =  TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            DateTime ThisEndTime = startDate.AddDays(_index);
            TimeSpan sp = ThisEndTime.Subtract(nowtime);

            return sp;
            
        }
        private bool CheckTick(uint _time)
        {
            bool isStart=false;
            TimeSpan sp = ReNameTimeShow(_time);
            if ((sp.Days<1)&&(sp.TotalSeconds>0))
            {
                isStart = true;
            }
            return isStart;
        }
        private void BackNameButtonShow()
        {//改回按钮显示

            TimeSpan spback = ReNameTimeShow(backTime);
            int temp = spback.Days;
            if (spback.Days >=1)
            {
                if (temp == backTime)
                {
                    temp--;
                }
                changeBackBtn.gameObject.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(2025111, temp.ToString());//{0}天后可改回
                isChangeBackEnable = false;
                ImageHelper.SetImageGray(changeBackBtn.gameObject.GetComponent<Image>(), true);
            }
            else if(spback.TotalSeconds<=0)
            {
                changeBackBtn.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(2025109);//点击改回
                isChangeBackEnable = true;
                ImageHelper.SetImageGray(changeBackBtn.gameObject.GetComponent<Image>(), false);
            }
            else
            {
                changeBackBtn.gameObject.transform.Find("Text").GetComponent<Text>().fontSize = 19;
                changeBackBtn.gameObject.transform.Find("Text").GetComponent<Text>().text = LanguageHelper.GetTextContent(2025110, TickNumberShow(backTime));//改回按钮——{0}后可改回(具体时间)
                isChangeBackEnable = false;
                ImageHelper.SetImageGray(changeBackBtn.gameObject.GetComponent<Image>(), true);
            }

        }

        private string TickNumberShow(uint _time)
        {
            TimeSpan sp = ReNameTimeShow(_time);
            uint lastSecond = (uint)sp.TotalSeconds;
            string strTime = LanguageHelper.TimeToString(lastSecond, LanguageHelper.TimeFormat.Type_1);
            return strTime;
        }
        

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ReName);
        }

        private void OnComfiremButtonClicked()
        {
            if (!isConfirmEnable)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025128,totalTime.ToString()));//不能改回提示
                return;
            }
            if (rolelevel != 0&&Sys_Role.Instance.Role.Level> rolelevel)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025129, rolelevel.ToString()));//超过人物等级
                return;
            }
            _newName = inputField.text.Trim();
            CSVParam.Data csv = CSVParam.Instance.GetConfData(1);
            uint nameLenLimit = csv == null ? 10 : System.Convert.ToUInt32(csv.str_value);
            if (_newName == "")
            {//输入为空
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025103));
                return;
            }else if (IsRepeatName(_newName))
            {//和原名相同
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025126));
                return;
            }
            else if (_newName.Length > nameLenLimit)
            {//超出名字最大长度
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101001));
                return;
            }
            else if (Sys_RoleName.Instance.HasBadNames(_newName))
            {//名字内含有违禁字、特殊字符
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101011));
                return;
            }


            if (CheckChangeNameItem()|| Sys_Bag.Instance.GetItemCount(1) >= price)
            {//有改名卡且没超过等级或者魔币足够
                UIManager.OpenUI(EUIID.UI_ReName_Tips, false, _newName);
                inputField.text = string.Empty;
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025125));//魔币不足提示
            }

        }

        private bool CheckChangeNameItem()
        {
            if (itemlevel!=0)
            {
                return Sys_Bag.Instance.GetItemCount(renameId) > 0 && Sys_Role.Instance.Role.Level <=itemlevel;
            }
            else
            {
                return Sys_Bag.Instance.GetItemCount(renameId) > 0;
            }
            
        }

        private bool IsRepeatName(string str)
        {
            return str.Equals(Sys_Role.Instance.Role.Name.ToStringUtf8());
        }
        private void OnChangeBackButtonClicked()
        {
            if (!isChangeBackEnable)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025127,backTime.ToString()));//不能改回提示
                return;
            }
            UIManager.OpenUI(EUIID.UI_ReName_Tips);
        }

        private void OnReNameUpdate()
        {
           ShowData();
        }
    }

}
