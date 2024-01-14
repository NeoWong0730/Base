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
    public class UI_PointMall_Left : UIComponent
    {
        private GameObject priceTip;
        private Text textPrice;
        private GameObject timeTip;
        private Text textTimer;

        private InfinityGrid _infinityGrid;
        private Lib.Core.CoroutineHandler handler;
        private Dictionary<GameObject, UI_PointMall_ShopItem> dicCells = new Dictionary<GameObject, UI_PointMall_ShopItem>();

        private List<ShopItem> listShopItems = new List<ShopItem>();

        private CSVShop.Data shopInfo;

        protected override void Loaded()
        {
            priceTip = transform.Find("View_Tips/Tips1").gameObject;
            textPrice = priceTip.transform.Find("Text").GetComponent<Text>();

            timeTip = FrameworkTool.CreateGameObject(textPrice.gameObject);
            timeTip.transform.SetParent(priceTip.transform);
            timeTip.transform.localPosition = new Vector3(textPrice.transform.localPosition.x, textPrice.transform.localPosition.y, 0f);
            timeTip.transform.localScale = Vector3.one;
            textTimer = timeTip.transform.GetComponent<Text>();

            textPrice.gameObject.SetActive(false);
            timeTip.SetActive(false);

            _infinityGrid = transform.Find("Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;


            //gridGroup = transform.Find("Scroll_View/Viewport").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 12;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //layoutGroup = gridGroup.GetComponent<GridLayoutGroup>();
            ////rectTransform = layoutGroup.GetComponent<RectTransform>();

            //CP_ToggleRegistry group = gridGroup.gameObject.GetComponent<CP_ToggleRegistry>();
            //group.allowSwitchOff = false;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform tran = gridGroup.transform.GetChild(i);
            //    UI_PointMall_ShopItem cell = AddComponent<UI_PointMall_ShopItem>(tran);
            //    //cell.AddListener(OnSelectIndex);
            //    dicCells.Add(tran.gameObject, cell);
            //}
        }

        public override void OnDestroy()
        {
            foreach (var cell in dicCells)
            {
                cell.Value.OnDestroy();
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_PointMall_ShopItem entry = new UI_PointMall_ShopItem();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_PointMall_ShopItem entry = cell.mUserData as UI_PointMall_ShopItem;
            entry.UpdateInfo(listShopItems[index], shopInfo != null && shopInfo.price_type != 0);
        }

        //private void UpdateChildrenCallback(int index, Transform trans)
        //{
        //    if (index < 0 || index >= visualGridCount)
        //        return;

        //    if (dicCells.ContainsKey(trans.gameObject))
        //    {
        //        UI_PointMall_ShopItem cell = dicCells[trans.gameObject];
        //        cell.UpdateInfo(listShopItems[index], shopInfo != null && shopInfo.price_type != 0);
        //    }
        //}

        public void UpdateShop(uint shopId, uint ItemId = 0)
        {
            shopInfo = CSVShop.Instance.GetConfData(shopId);

            listShopItems.Clear();
            listShopItems = Sys_Mall.Instance.GetShopItems(shopId);

            //计算选中
            if (ItemId != 0)
            {
                for (int i = 0; i < listShopItems.Count; ++i)
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
                //未选中时，默认选中第一个
                if (listShopItems.Count > 0)
                    Sys_Mall.Instance.SelectShopItemId = listShopItems[0].ShopItemId;
            }

            //visualGridCount = listShopItems.Count;
            //gridGroup.SetAmount(visualGridCount);
            _infinityGrid.CellCount = listShopItems.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            //CheckNeedScroll(ItemId);
            UpdateTip(shopId);

            if (handler != null)
            {
                CoroutineManager.Instance.Stop(handler);
                handler = null;
            }

            handler = CoroutineManager.Instance.StartHandler(CheckNeedScroll(ItemId));
        }

        private IEnumerator CheckNeedScroll(uint itemId)
        {
            yield return null;

            //calTargetPos 
            if (itemId != 0)
            {
                bool isFind = false;
                int i;
                for (i = 0; i < listShopItems.Count; ++i)
                {
                    CSVShopItem.Data shopItemInfo = CSVShopItem.Instance.GetConfData(listShopItems[i].ShopItemId);
                    if (shopItemInfo.item_id == itemId)
                    {
                        isFind = true;
                        break;
                    }
                }

                if (isFind)
                {
                    _infinityGrid.MoveToIndex(i);
                }
                else
                {
                    DebugUtil.LogErrorFormat("找不到该商品 -- {0}", itemId);
                }
            }
        }


        //private void CheckNeedScroll(uint ItemId)
        //{
        //    //calTargetPos 
        //    if (ItemId != 0)
        //    {
        //        int cellIndex = 0;
        //        int i;
        //        for (i = 0; i < listShopItems.Count; ++i)
        //        {
        //            CSVShopItem.Data shopItemInfo = CSVShopItem.Instance.GetConfData(listShopItems[i].ShopItemId);
        //            if (shopItemInfo.item_id == ItemId)
        //                break;
        //        }

        //        cellIndex = i / 3;

        //        if (cellIndex > 1)
        //        {
        //            RectTransform rectTransform = gridGroup.GetComponent<RectTransform>();
        //            float positionY = (cellIndex) * (layoutGroup.cellSize.y + layoutGroup.spacing.y);
        //            rectTransform.anchoredPosition = new Vector3(0, positionY, 0);
        //        }
        //    }
        //}

        private void UpdateTip(uint shopId)
        {
            //计算时间
            textPrice.gameObject.SetActive(false);
            timeTip.SetActive(false);

            if (shopInfo != null)
            {
                if (shopInfo.price_type == 2)
                {
                    textPrice.gameObject.SetActive(true);
                    textPrice.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2009810));
                }

                if (shopInfo.refresh_time != null)
                {
                    timeTip.SetActive(true);

                    System.Text.StringBuilder strDay = new System.Text.StringBuilder();
                    for (int i = 0; i < shopInfo.refresh_time[0].Count; ++i)
                    {
                        strDay.Append(CSVLanguage.Instance.GetConfData(2009811 + shopInfo.refresh_time[0][i]).words);
                        if (i != shopInfo.refresh_time[0].Count - 1)
                            strDay.Append("、");
                    }

                    System.Text.StringBuilder strHour = new System.Text.StringBuilder();
                    for (int i = 0; i < shopInfo.refresh_time[1].Count; ++i)
                    {
                        strHour.Append(shopInfo.refresh_time[1][i]);
                        if (i != shopInfo.refresh_time[1].Count - 1)
                            strHour.Append("、");
                    }

                    textTimer.text = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(2009811), strDay.ToString(), strHour.ToString());
                }
            }
        }
    }
}


