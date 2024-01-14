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
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_MerchantFleet : UIBase,UI_MerchantFleet_Right.IListener
    {
        private Button btn_Close;
        private UI_MerchantFleet_Left m_LeftPanel;
        private UI_MerchantFleet_Right m_RightPanel;
        #region 系统函数
        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/View_Title02/Btn_Close").GetComponent<Button>();
            m_LeftPanel = new UI_MerchantFleet_Left();
            m_LeftPanel.BindGameObject(transform.Find("Animator/View_Left"));
            m_RightPanel = new UI_MerchantFleet_Right();
            m_RightPanel.RegisterListener(this);
            m_RightPanel.BindGameObject(transform.Find("Animator/View_Right"));
            btn_Close.onClick.AddListener(OnCloseButtonClicked);

        }
        protected override void OnDestroy()
        {
            m_LeftPanel.Destory();
            m_RightPanel.Destory();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateMerchantInfo, OnUpdateMerchantInfo, toRegister);
            Sys_MerchantFleet.Instance.eventEmitter.Handle(Sys_MerchantFleet.EEvents.UpdateLevelAward, OnUpdateMerchantInfo, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChanged, toRegister);
        }

        protected override void OnShow()
        {
            Sys_MerchantFleet.Instance.OnMerchantGetInfoReq();
            m_LeftPanel.Show();
            m_RightPanel.Show();
        }
        protected override void OnUpdate()
        {

        }
        protected override void OnHide()
        {
            m_LeftPanel.Destory();
        }
        #endregion
        private void OnUpdateMerchantInfo()
        {
            m_LeftPanel.Update();
            m_RightPanel.Update();
        }
        private void OnCurrencyChanged(uint id, long value)
        {
            m_RightPanel.Update();
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MerchantFleet);
        }
        public void OnTradeButtonClicked()
        {
            m_LeftPanel.PlaySailAnimation();
        }
    }
   
}
