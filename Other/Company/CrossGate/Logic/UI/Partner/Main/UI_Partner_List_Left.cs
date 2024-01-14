using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Partner_List_Left : UIParseCommon
    {
        private CP_Toggle _toggleAll;

        private InfinityGrid _infinityGrid;

        private IListener listener;

        private List<uint> listIds = new List<uint>();
        private List<uint> listIcons = new List<uint>();

        protected override void Parse()
        {
            _toggleAll = transform.Find("Toggle_All").GetComponent<CP_Toggle>();
            _toggleAll.onValueChanged.AddListener(OnToggleAll);

            _infinityGrid = transform.Find("Scroll01").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
            //gridGroup = transform.Find("Scroll01/Content").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            //gridGroup.minAmount = 9;
            //gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            //for (int i = 0; i < gridGroup.transform.childCount; ++i)
            //{
            //    Transform trans = gridGroup.transform.GetChild(i);

            //    UI_Partner_List_Left_Cell cell = new UI_Partner_List_Left_Cell();
            //    cell.Init(trans);
            //    cell.AddListener(OnSelectIndex);
            //    dicCells.Add(trans.gameObject, cell);
            //}
        }

        public override void Show()
        {
            OnSiftingByType(EPartnerType.Career);
        }

        public override void Hide()
        {

        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Partner_List_Left_Cell entry = new UI_Partner_List_Left_Cell();
            entry.Init(cell.mRootTransform);
            entry.AddListener(OnSelectIndex);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            //cell.mRootTransform.name = index.ToString();

            UI_Partner_List_Left_Cell entry = cell.mUserData as UI_Partner_List_Left_Cell;
            entry.UpdateInfo(listIds[index], listIcons[index], index);
        }

        private void OnToggleAll(bool isOn)
        {
            if (isOn)
            {
                listener?.OnSelectListIndex(0, listIds);

                Sys_Partner.Instance.SelectIndex = -1;

                _infinityGrid.CellCount = listIds.Count;
                _infinityGrid.ForceRefreshActiveCell();
                _infinityGrid.MoveToIndex(0);
            }
        }

        private void OnSelectIndex(int index)
        {
            Sys_Partner.Instance.SelectIndex = index;
            listener?.OnSelectListIndex(listIds[index], listIds);

            _toggleAll.SetSelected(false, true);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public void OnSiftingByType(EPartnerType _type)
        {
            listIds.Clear();
            listIcons.Clear();
            //listIds.Add(0); //id为0代表全部
            if (_type == EPartnerType.Career)
            {
                var occDict = CSVPartnerOccupation.Instance.GetAll();
                foreach (var data in occDict)
                {
                    listIds.Add(data.list_occupation);
                    listIcons.Add(data.icon);
                }
            }
            else if (_type == EPartnerType.Area)
            {
                var occDict = CSVPartnerBelonging.Instance.GetAll();
                foreach (var data in occDict)
                {
                    listIds.Add(data.list_belonging);
                    listIcons.Add(data.icon);
                }
            }

            _infinityGrid.CellCount = listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);

            Sys_Partner.Instance.SelectIndex = -1;
            _toggleAll.SetSelected(true, true);
        }

        public interface IListener
        {
            void OnSelectListIndex(uint _typeId, List<uint> _listIds);
        }
    }
}
