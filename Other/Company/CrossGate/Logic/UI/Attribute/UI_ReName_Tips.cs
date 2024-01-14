using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using Logic;
using System;

namespace Logic {
    public class UI_ReName_Tips : UIBase
    {
        private Button btn_Close;
        private Button btn_Cancel;
        private Button btn_Sure;
        private Text oldName;
        private Text newName;
        private Text nowName;
        private Text backName;
        private GameObject typeOne;
        private GameObject typeSec;

        private Text typeOne_txtOne;
        private Text typeOne_txtSec;

        private Text backTipsText;

        private string newNameStr;
        uint renameCardId;
        uint rolelevel = 0;
        uint itemlevel = 0;
        uint price = 20;
        uint totalTime = 30;//改名后再次改名冷却时间（天）
        uint backTime = 7; //改名后可改回曾用名冷却时间（天）

        public void Init()
        {
            btn_Sure = transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            btn_Cancel = transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            typeOne= transform.Find("Animator/Type1").gameObject;
            typeSec = transform.Find("Animator/Type2").gameObject;
            newName = transform.Find("Animator/Type1/Name/Name2").GetComponent<Text>();
            oldName = transform.Find("Animator/Type1/Name/Name1").GetComponent<Text>();
            backName = transform.Find("Animator/Type2/Name/Name2").GetComponent<Text>();
             nowName= transform.Find("Animator/Type2/Name/Name1").GetComponent<Text>();
            typeOne_txtOne = transform.Find("Animator/Type1/Tips/Image1/GameObject").GetComponent<Text>();
            typeOne_txtSec = transform.Find("Animator/Type1/Tips/Image2/GameObject").GetComponent<Text>();
            backTipsText= transform.Find("Animator/Type2/Tips/Image1/GameObject").GetComponent<Text>();
        }

        protected override void OnOpen(object arg = null)
        {
            if (arg != null)
            {
                newNameStr = (string)arg;
            }
        }

        protected override void OnLoaded()
        {
            Init();
            InitParam();
            btn_Sure.onClick.AddListener(OnSureButtonClicked);
            btn_Cancel.onClick.AddListener(OnCancelButtonClicked);

        }
        protected override void OnShow()
        {
            PanelShow();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnReName, OnCancelButtonClicked, toRegister);
        }
        private void InitParam()
        {//初始化天数参数
            CSVParam.Data pData = CSVParam.Instance.GetConfData(1324);
            totalTime = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1325);
            backTime = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1327);
            price = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1353);
            itemlevel = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1328);
            rolelevel = Convert.ToUInt32(pData.str_value);
            pData = CSVParam.Instance.GetConfData(1329);
            renameCardId = Convert.ToUInt32(pData.str_value);
        }
        private void PanelShow()
        {
            typeOne.SetActive(Sys_Role.Instance.reNameData.Time == 0);
            typeSec.SetActive(Sys_Role.Instance.reNameData.Time != 0);

            if (Sys_Role.Instance.reNameData.Time == 0)
            {
                newName.text = newNameStr;
                oldName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                typeOne_txtOne.text = LanguageHelper.GetTextContent(2025113,totalTime.ToString(), backTime.ToString());
                typeOne_txtSec.text = LanguageHelper.GetTextContent(2025114,Sys_Role.Instance.Role.Name.ToStringUtf8(), totalTime.ToString());
            }
            else
            {
                backName.text = Sys_Role.Instance.reNameData.Name.ToStringUtf8();
                nowName.text =Sys_Role.Instance.Role.Name.ToStringUtf8();
                backTipsText.text = LanguageHelper.GetTextContent(2025118,totalTime.ToString(), Sys_Role.Instance.Role.Name.ToStringUtf8());
            }

        }
        
        
        private void OnSureButtonClicked()
        {
            if (rolelevel != 0 && Sys_Role.Instance.Role.Level >rolelevel)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025129, rolelevel.ToString()));//超过人物等级
                return;
            }
            if (CheckChangeNameItem() || Sys_Bag.Instance.GetItemCount(1) >= price)
            {
                if (Sys_Role.Instance.reNameData.Time == 0)
                {
                    Sys_Role.Instance.OnReNameReq(newNameStr);
                }
                else
                {
                    Sys_Role.Instance.OnReNameReq(Sys_Role.Instance.reNameData.Name.ToStringUtf8());
                }
            }
            else
            {//无改名卡或者魔币不够
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025125));
                return;

            }
            
        }

        private bool CheckChangeNameItem()
        {
            if (itemlevel != 0)
            {
                return Sys_Bag.Instance.GetItemCount(renameCardId) > 0 && Sys_Role.Instance.Role.Level <= itemlevel;
            }
            else
            {
                return Sys_Bag.Instance.GetItemCount(renameCardId) > 0;
            }
        }
        private void OnCancelButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_ReName_Tips);//改名成功，必然返回关闭确认面板,否则有提示
        }

    }

}
