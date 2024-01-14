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
    public class UI_Trade_Message_PetEquipItem
    {
        private Transform transform;

        private TipPetEquipLeftCompare _tipPetEquip;

        public void Init(Transform trans)
        {
            transform = trans;

            _tipPetEquip = new TipPetEquipLeftCompare();
            _tipPetEquip.Init(transform);
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
            ItemData item = new ItemData(99, tradeItem.GoodsUid, tradeItem.InfoId, tradeItem.Count, 0, false, false, tradeItem.Item.Equipment, tradeItem.Item.Essence, 0, null, null, null, tradeItem.Item.PetEquip);

            _tipPetEquip.UpdateInfo(item, 0);
        }

        public void UpdateInfo(ItemData item)
        {
            _tipPetEquip.UpdateInfo(item, 0);
        }

        public void OnDestroy()
        {
            _tipPetEquip?.OnDestroy();
        }
    }
}


