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
    public class UI_Mall_Left_Cell : UIComponent
    {
        private List<UI_Mall_ShopItem> listItem = new List<UI_Mall_ShopItem>();
        private int shopCount = 0;

        protected override void Loaded()
        {
            shopCount = transform.childCount;
            string format = "ShopItem0{0}";
            for (int i = 0; i < shopCount; ++i)
            {
                string itemStr = string.Format(format, (i + 1).ToString());
                UI_Mall_ShopItem shopItem = AddComponent<UI_Mall_ShopItem>(transform.Find(itemStr));
                listItem.Add(shopItem);
            }
        }

        public override void OnDestroy()
        {
            foreach (var shopItem in listItem)
            {
                shopItem.OnDestroy();
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


