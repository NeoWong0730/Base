using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Field_Upgrade : UIBase
    {
        private UI_CurrencyTitle currency;
        private UI_Filed_Upgrade_Left leftView;
        private UI_Filed_Upgrade_Right rightView;

        protected override void OnLoaded()
        {
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            
            Button btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                this.CloseSelf();
            });
            
            leftView = new UI_Filed_Upgrade_Left();
            leftView.Init(transform.Find("Animator/View_Left"));
            
            rightView = new UI_Filed_Upgrade_Right();
            rightView.Init(transform.Find("Animator/View_Right"));
        }

        protected override void OnOpen(object arg)
        {            

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfBodyUpgrade, OnNtfBodyUpgrade, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle<ulong>(Sys_Equip.EEvents.OnNtfFieldUpgradeMat, OnNtfFieldUpgradeMat, toRegister);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfBodyUpgradeSkillRefresh, OnNtfBodyUpgradeSkillRefresh, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }        

        protected override void OnDestroy()
        {
            currency?.Dispose();
        }

        private void UpdateInfo()
        {
            currency.InitUi();
            rightView.UpdateInfo();
            leftView.UpdateInfo();
        }

        private void OnNtfBodyUpgrade()
        {
            rightView.RefreshData();
            leftView.UpdateInfo();
        }

        private void OnNtfFieldUpgradeMat(ulong matUid)
        {
            rightView.SetData(matUid);
        }

        private void OnNtfBodyUpgradeSkillRefresh()
        {
            leftView.RefreshSelect();
        }
    }
}

