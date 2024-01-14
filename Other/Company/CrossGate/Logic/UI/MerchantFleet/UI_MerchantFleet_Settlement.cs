using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using System.Text;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;
using Framework;
using System.Collections.Generic;

namespace Logic
{
    public class MerchantResultParam
    {
        public int TypeIndex;//1-2-3-4
        public List<ItemIdCount> AwardList;
    }
    //结算提示UI_Rewards_Result
    public class UI_MerchantFleet_Settlement : UIBase
    {
        private Button btnClose;
        private GameObject TypeFinishTask;//1
        private GameObject prop1;
        private GameObject TypeFinishTrade;//2
        private GameObject prop2;
        private GameObject TypeHelpTrade;//3
        private GameObject TypeFightFail;//4
        private MerchantResultParam param;
        private Timer m_timer;
        protected override void OnOpen(object arg)
        {
            if (arg!=null)
            {
                param = (MerchantResultParam)arg;
            }
        }
        protected override void OnLoaded()
        {
            btnClose = transform.Find("Image").GetComponent<Button>();
            TypeFinishTask = transform.Find("Animator/Image_BG2/Type1").gameObject;
            prop1 = transform.Find("Animator/Image_BG2/Type1/Grid/PropItem").gameObject;
            TypeFinishTrade = transform.Find("Animator/Image_BG2/Type2").gameObject;
            prop2 = transform.Find("Animator/Image_BG2/Type2/Grid/PropItem").gameObject;
            TypeHelpTrade = transform.Find("Animator/Image_BG2/Type3").gameObject;
            TypeFightFail = transform.Find("Animator/Image_BG2/Type4").gameObject;
            btnClose.onClick.AddListener(OnCloseButtonClicked);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        protected override void OnShow()
        {
            PanelShow();
        }
        protected override void OnDestroy()
        {

        }
        protected override void OnHide()
        {
            m_timer?.Cancel();
        }
        private void PanelShow()
        {
            DefaultType();
            m_timer?.Cancel();
            m_timer = Timer.Register(2.0f, OnCloseButtonClicked);
            if (param==null) return;
            switch (param.TypeIndex)
            {
                case 1:
                    TypeFinishTask.SetActive(true);
                    Sys_MerchantFleet.Instance.InitPropItem(prop1, param.AwardList,EUIID.UI_MerchantFleet_Settlement,false);
                    break;
                case 2:
                    TypeFinishTrade.SetActive(true);
                    Sys_MerchantFleet.Instance.InitPropItem(prop2, param.AwardList,EUIID.UI_MerchantFleet_Settlement, false);
                    break;
                case 3:
                    TypeHelpTrade.SetActive(true);
                    break;
                case 4:
                    TypeFightFail.SetActive(true);
                    break;
            }
            
        }
        private void DefaultType()
        {
            TypeFinishTask.SetActive(false);
            TypeFinishTrade.SetActive(false);
            TypeHelpTrade.SetActive(false);
            TypeFightFail.SetActive(false);
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MerchantFleet_Settlement);
        }
    }


}