using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;

namespace Logic
{
    public class UI_Partner_List_Right : UIParseCommon
    {
        //private ToggleGroup toggleGroup;
        //private InfinityGridLayoutGroup gridGroup;
        //private Dictionary<GameObject, UI_Partner_List_Right_Cell> dicCells = new Dictionary<GameObject, UI_Partner_List_Right_Cell>();
        //private int visualGridCount;
        private Dictionary<GameObject, UI_Partner_List_Right_Cell> dicCells = new Dictionary<GameObject, UI_Partner_List_Right_Cell>();
        private InfinityGrid _infinityGrid;

        private List<uint> listInfoIds;

        protected override void Parse()
        {
            _infinityGrid = transform.Find("Scroll_Partner").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            //toggleGroup = transform.Find("Scroll_Partner/Content").gameObject.GetComponent<ToggleGroup>();
            //gridGroup = transform.Find("Scroll_Partner/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 20;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform trans = gridGroup.transform.GetChild(i);

            //    UI_Partner_List_Right_Cell cell = new UI_Partner_List_Right_Cell();
            //    cell.Init(trans);
            //    dicCells.Add(trans.gameObject, cell);
            //}
        }

        public override void Show()
        {
            //foreach (UI_Partner_List_Right_Cell cell in dicCells.Values)
            //{
            //    cell.Show();
            //}
        }

        public override void Hide()
        {
            //foreach (UI_Partner_List_Right_Cell cell in dicCells.Values)
            //{
            //    cell.Hide();
            //}
        }

        public override void OnDestroy()
        {
            foreach (UI_Partner_List_Right_Cell cell in dicCells.Values)
            {
                cell.OnDestroy();
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Partner_List_Right_Cell entry = new UI_Partner_List_Right_Cell();
            entry.Init(cell.mRootTransform);
            //entry.AddListener(OnSelectIndex);

            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_Partner_List_Right_Cell entry = cell.mUserData as UI_Partner_List_Right_Cell;
            entry.UpdateInfo(listInfoIds[index], index);
        }

        public void UpdatePartner(List<uint> _infoIds)
        {
            listInfoIds = _infoIds;

            _infinityGrid.CellCount = listInfoIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }

        //private void OnToggleClick(bool _select, int _index)
        //{
            
        //}
    }
}
