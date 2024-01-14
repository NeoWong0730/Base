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
    public class UI_Trade_Message_OrnamentItem
    {
        private Transform transform;

        public TipOrnamentCompare _tipOrnament;

        public void Init(Transform trans)
        {
            transform = trans;

            _tipOrnament = new TipOrnamentCompare();
            _tipOrnament.Init(transform);
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(TradeItem tradeItem)
        {
            ItemData item = new ItemData(99, tradeItem.GoodsUid, tradeItem.InfoId, tradeItem.Count, 0, false, false, tradeItem.Item.Equipment, tradeItem.Item.Essence, 0, null, null, tradeItem.Item.Ornament);

            _tipOrnament.UpdateEquipInfo(item);
        }

        public void UpdateInfo(ItemData itemEquip)
        {
            _tipOrnament.UpdateEquipInfo(itemEquip);
        }

        public void OnDestroy()
        {
            _tipOrnament?.OnDestroy();
        }
    }
}


