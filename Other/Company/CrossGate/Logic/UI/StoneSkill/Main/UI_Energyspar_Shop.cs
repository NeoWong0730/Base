using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using System;

namespace Logic
{

    public class UI_Energyspar_Shop : UIComponent
    {
        private UI_Energyspar_Shop_Left viewLeft;
        private UI_Energyspar_Shop_Middle viewMiddle;
        private uint selectId;        
        protected override void Loaded()
        {
            viewLeft = new UI_Energyspar_Shop_Left();
            viewLeft.Init(transform.Find("View_Left"));
            viewMiddle = new UI_Energyspar_Shop_Middle();
            viewMiddle.Init(transform.Find("View_Middle"));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_Mall.Instance.eventEmitter.Handle<uint>(Sys_Mall.EEvents.OnFillShopData, OnFillShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnSelectShopItem, OnSelectShopItem, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
            Sys_Mall.Instance.eventEmitter.Handle(Sys_Mall.EEvents.OnBuyScuccess, viewMiddle.OnBuyScuccess, toRegister);
        }

        public override void Show()
        {
            base.Show();
            OnSelectShop();
            OnFillShopData(2001);
        }

        public void ShowEx(uint id)
        {
            base.Show();
            selectId = id;
            OnSelectShop();
            OnFillShopData(2001);
        }

        public override void Hide()
        {
            base.Hide();
            Sys_Mall.Instance.ClearData();
        }


        public void OnSelectShop()
        {
            Sys_Mall.Instance.OnItemRecordReq(2001);
        }

        private void OnFillShopData(uint shopId)
        {
            viewLeft?.UpdateShop(shopId, selectId);
            selectId = 0;
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }

        private void OnSelectShopItem()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }

        private void OnRefreshShopData()
        {
            viewMiddle?.UpdateInfo(Sys_Mall.Instance.SelectShopItemId);
        }
    }
}


