using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Message_EquipItem
    {
        private Transform transform;

        private TipEquipLeftCompare _tipEquip;
        public Button _btnMessage;
        private ItemData item;

        public void Init(Transform trans)
        {
            transform = trans;

            _tipEquip = new TipEquipLeftCompare();
            _tipEquip.Init(transform);
            _btnMessage = _tipEquip.backgroundFirst.BtnMsg;
            _btnMessage.onClick.AddListener(OnBtnMessage);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }
        
        public void OnDestroy()
        {
            _tipEquip?.OnDestroy();
        }

        public void UpdateInfo(TradeItem tradeItem)
        {
            item = new ItemData(99, tradeItem.GoodsUid, tradeItem.InfoId, tradeItem.Count, 0, false, false, tradeItem.Item.Equipment, tradeItem.Item.Essence, 0, null);

            _tipEquip.UpdateInfo(item);
        }

        public void UpdateInfo(ItemData itemEquip)
        {
            _tipEquip.UpdateInfo(itemEquip);
        }

        private void OnBtnMessage()
        {
            Sys_Equip.Instance.eventEmitter.Trigger<ItemData>(Sys_Equip.EEvents.OnShowMsg, item);
        }
    }
 
}


