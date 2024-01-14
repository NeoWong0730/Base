using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System.Text;
using System;
using logic;

namespace Logic
{
    public class UI_AwakenImprint : UIBase, UI_AwakenImprint_LeftView.IListener, UI_AwakenImprint_Middle.IListener
    {
        private Button btn_Close;
        private UI_AwakenImprint_LeftView awakenLeftView;
        private UI_AwakenImprint_RightView awakenRightView;
        private UI_AwakenImprint_Middle middlePanel;
        private UI_CurrencyTitle currency;
        private Button btn_AwakenAdd;        

        protected override void OnLoaded()
        {            
            awakenLeftView = new UI_AwakenImprint_LeftView();
            awakenLeftView.Init(transform.Find("Animator/View_Left"));
            awakenLeftView.RegisterListener(this);
            awakenRightView = new UI_AwakenImprint_RightView();
            awakenRightView.Init(transform.Find("Animator/View_Right"));
            middlePanel = new UI_AwakenImprint_Middle();
            middlePanel.Init(transform.Find("Animator/View_Middle"));
            middlePanel.RegisterListener(this);
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            btn_AwakenAdd = transform.Find("Animator/View_Left/Button").GetComponent<Button>();
            btn_Close = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            Sys_TravellerAwakening.Instance.panelOrder = transform.GetComponent<Canvas>().sortingOrder;
            //Sys_TravellerAwakening.Instance.ImprintLabelRedPointDayCheck();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            btn_AwakenAdd.onClick.AddListener(OnAwakenAdditionBtnClicked);
        }

        protected override void OnShow()
        {            
            awakenLeftView.Show();
            middlePanel.Show();
            awakenRightView.Show();
            currency.InitUi();
            Sys_TravellerAwakening.Instance.IsShowRedpoint = false;

        }
        protected override void OnUpdate()
        {
            if (Sys_TravellerAwakening.Instance.panelOrder != transform.GetComponent<Canvas>().sortingOrder)
            {
                Sys_TravellerAwakening.Instance.panelOrder = transform.GetComponent<Canvas>().sortingOrder;
                middlePanel.PanelOrderFresh();
            }

        }
        protected override void OnHideStart()
        {
            Sys_TravellerAwakening.Instance.ImprintLabelRedPointDayCheck();
        }
        protected override void OnHide()
        {            
            middlePanel.Hide();
            awakenRightView.Hide();
        }

        protected override void OnDestroy()
        {            
            awakenLeftView.OnDestroy();
            middlePanel.OnDestroy();
            awakenRightView.OnDestroy();
            currency.Dispose();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_TravellerAwakening.Instance.eventEmitter.Handle(Sys_TravellerAwakening.EEvents.OnAwakeImprintUpdate, OnAwakeImprintUpdate, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
        }
        public void OnSelectListIndex(uint selectLabel)
        {
            middlePanel.LabelRefresh(selectLabel);

        }
        public void OnSelectNode()
        {
            awakenRightView.SetPanelData();
        }
        private void OnCurrencyChanged(uint id, long value)
        {
            awakenRightView.RefreshCoinShow();
        }
        private void OnAwakeImprintUpdate()
        {
            awakenLeftView.RefreshRedPoint();
            middlePanel.RefreshPanel(Sys_TravellerAwakening.Instance.SelectIndex);
        }
        private void OnCloseButtonClicked()
        {

            UIManager.CloseUI(EUIID.UI_AwakenImprint);
        }
        private void OnAwakenAdditionBtnClicked()
        {

            UIManager.OpenUI(EUIID.UI_Awaken_Addition);
        }

        
    }

}
