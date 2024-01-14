using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Energyspar_Shop_Left
    {
        private InfinityGridLayoutGroup gridGroup;
        private GridLayoutGroup layoutGroup; //特殊处理
        private Dictionary<GameObject, UI_Energyspar_Shop_ShopItem> dicCells = new Dictionary<GameObject, UI_Energyspar_Shop_ShopItem>();
        private int visualGridCount;

        private List<ShopItem> listShopItems = new List<ShopItem>();

        private CSVShop.Data shopInfo;        
        public void Init(Transform transform)
        {
            gridGroup = transform.Find("Scroll_View/Viewport").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 12;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            layoutGroup = gridGroup.GetComponent<GridLayoutGroup>();

            CP_ToggleRegistry group = gridGroup.gameObject.GetComponent<CP_ToggleRegistry>();
            group.allowSwitchOff = false;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform tran = gridGroup.transform.GetChild(i);
                UI_Energyspar_Shop_ShopItem cell = new UI_Energyspar_Shop_ShopItem();
                cell.Init(tran);
                dicCells.Add(tran.gameObject, cell);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicCells.ContainsKey(trans.gameObject))
            {
                UI_Energyspar_Shop_ShopItem cell = dicCells[trans.gameObject];
                cell.UpdateInfo(listShopItems[index], shopInfo != null && shopInfo.price_type != 0);
            }
        }

        public void UpdateShop(uint shopId, uint ItemId = 0)
        {
            shopInfo = CSVShop.Instance.GetConfData(shopId);

            listShopItems.Clear();
            listShopItems = Sys_Mall.Instance.GetShopItems(shopId);

            //计算选中
            if (ItemId != 0)
            {
                for (int i = 0; i < listShopItems.Count; i++)
                {
                    CSVShopItem.Data shopItemInfo = CSVShopItem.Instance.GetConfData(listShopItems[i].ShopItemId);
                    if (shopItemInfo.item_id == ItemId)
                    {
                        Sys_Mall.Instance.SelectShopItemId = listShopItems[i].ShopItemId;
                        break;
                    }
                }
            }
            else
            {
                if (listShopItems.Count > 0)
                    Sys_Mall.Instance.SelectShopItemId = listShopItems[0].ShopItemId;
            }

            visualGridCount = listShopItems.Count;
            gridGroup.SetAmount(visualGridCount);
            CheckNeedScroll(ItemId);
        }

        private void CheckNeedScroll(uint ItemId)
        {
            //calTargetPos 
            if (ItemId != 0)
            {
                int cellIndex = 0;
                int i;
                for (i = 0; i < listShopItems.Count; ++i)
                {
                    CSVShopItem.Data shopItemInfo = CSVShopItem.Instance.GetConfData(listShopItems[i].ShopItemId);
                    if (shopItemInfo.item_id == ItemId)
                        break;
                }

                cellIndex = i / 3;

                cellIndex -= 1; //初始2行，不用滑动

                if (cellIndex > 0)
                {
                    gridGroup.MoveToCellIndex(cellIndex, (cellIndex / 2 * 0.2f));
                }
            }
        }
    }
}


