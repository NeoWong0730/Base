using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Framework;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_Nien  : UIBase
    {
        private Text txtChallengeDate;
        private Text txtActvityDate;
        
        private PropItem srcItem;
        private PropItem desItem;

        private Button btnGetItem;
        private Button btnFindNpc;

        private uint npcId;
        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
            
            txtChallengeDate = transform.Find("Animator/Text_Nien/Text_Time").GetComponent<Text>();
            txtActvityDate = transform.Find("Animator/Image_Time/Text").GetComponent<Text>();
            
            srcItem = new PropItem();
            srcItem.BindGameObject(transform.Find("Animator/PropItem").gameObject);
            
            desItem = new PropItem();
            desItem.BindGameObject(transform.Find("Animator/PropItem (1)").gameObject);
            
            btnGetItem = transform.Find("Animator/Btn_01").GetComponent<Button>();
            btnGetItem.onClick.AddListener(OnClickGetItem);
            
            btnFindNpc = transform.Find("Animator/Btn_02").GetComponent<Button>();
            btnFindNpc.onClick.AddListener(OnClickFindNpc);
        }

        protected override void OnOpen(object arg)
        {
            CSVNienParameters.Data data = CSVNienParameters.Instance.GetConfData(6);
            uint.TryParse(data.str_value, out npcId);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnFillShopData, OnFillShopData, toRegister);
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {

        }

        protected override void OnDestroy()
        {

        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnClickGetItem()
        {
            Debug.LogError("OnClickGetItem");
        }

        private void OnClickFindNpc()
        {
            Debug.LogError("OnClickFindNpc=" + npcId);
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcId);
            this.CloseSelf();
        }

        private void UpdateInfo()
        {

        }
    }
}


