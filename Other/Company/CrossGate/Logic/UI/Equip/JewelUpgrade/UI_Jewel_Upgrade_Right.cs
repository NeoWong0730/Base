using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Jewel_Upgrade_Right
    {
        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<uint> listJewelIds;
        public void Init(Transform trans)
        {
            transform = trans;
            _infinityGrid = transform.Find("Scroll_View_Gem").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Jewel_Upgrade_IconRoot entry = new UI_Jewel_Upgrade_IconRoot();
            //entry.AddListener(OnSelectJewel);
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Jewel_Upgrade_IconRoot entry = cell.mUserData as UI_Jewel_Upgrade_IconRoot;
            entry.UpdateInfo(listJewelIds[index]);
        }

        public void UpdateJewelList(uint targetJewelId)
        {
            listJewelIds = Sys_Equip.Instance.CalPreJewelList(targetJewelId);

            _infinityGrid.CellCount = listJewelIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}


