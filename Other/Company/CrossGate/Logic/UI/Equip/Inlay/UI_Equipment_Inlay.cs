using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{

    public class UI_Equipment_Inlay : UIParseCommon, UI_Equipment_Inlay_Middle.IListener
    {
        private UI_Equipment_Inlay_Middle middle;
        private UI_Equipment_Inlay_JewelList jewelList;

        private Button btnCompose;

        protected override void Parse()
        {
            middle = new UI_Equipment_Inlay_Middle();
            middle.Init(transform.Find("View_Middle"));
            middle.RegisterListener(this);

            jewelList = new UI_Equipment_Inlay_JewelList();
            jewelList.Init(transform.Find("View_Right"));

            btnCompose = transform.Find("Button_Clear_Up").GetComponent<Button>();
            btnCompose.onClick.AddListener(OnClickCompose);

            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfUnload, OnNtfUnload, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelNtfQuickCompose, true);
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnJewelCompose, OnJewelCompose, true);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfUnload, OnNtfUnload, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfQuickCompose, OnJewelNtfQuickCompose, false);
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnJewelCompose, OnJewelCompose, false);

            middle.OnDestroy();
        }

        public override void UpdateInfo(ItemData _item)
        {
            middle.UpdateInfo(_item);
        }

        private void OnClickCompose()
        {
            UIManager.OpenUI(EUIID.UI_JewelCompound);
        }

        public void OnSelectInlaySlot(ItemData equipItem, uint jewelInfoId)
        {
            jewelList?.UpdateJewelList(equipItem, jewelInfoId);
        }

        #region NetNoty
        private void OnNtfInlay()
        {
            middle?.OnRefreshEquipment();
        }

        private void OnNtfUnload()
        {
            middle?.OnRefreshEquipment();
        }

        private void OnJewelNtfQuickCompose()
        {
            middle?.OnRefreshEquipment();
        }

        private void OnJewelCompose(bool isTen)
        {
            middle?.OnRefreshEquipment();
        }
        #endregion
    }
}


