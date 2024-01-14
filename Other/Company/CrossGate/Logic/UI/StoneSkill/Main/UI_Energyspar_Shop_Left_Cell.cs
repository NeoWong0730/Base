using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Energyspar_Shop_Left_Cell
    {
        private List<UI_Energyspar_Shop_ShopItem> listItem = new List<UI_Energyspar_Shop_ShopItem>();
        private int shopCount = 0;        
        public void Init(Transform transform)
        {
            shopCount = transform.childCount;

            for (int i = 0; i < shopCount; ++i)
            {
                string itemStr = string.Format("ShopItem0{0}", i + 1);
                UI_Energyspar_Shop_ShopItem shopItem = new UI_Energyspar_Shop_ShopItem();
                shopItem.Init(transform.Find(itemStr));
                listItem.Add(shopItem);
            }
        }

        public void UpdateInfo(List<ShopItem> items, bool priceChange = false)
        {
            for (int i = 0; i < listItem.Count; ++i)
            {
                if (i < items.Count)
                {
                    listItem[i].gameObject.SetActive(true);
                    listItem[i].UpdateInfo(items[i], priceChange);
                }
                else
                {
                    listItem[i].gameObject.SetActive(false);
                }
            }
        }
    }
}


