using System;
using System.Collections.Generic;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Partner_Property_HeadList
    {
        public class HeadCell
        {
            private Transform transform;

            private CP_Toggle toggle;
            private Image icon;
            private Image imgFight;

            private uint partnerId;
            private Action<uint> action;

            public void Init(Transform trans)
            {
                transform = trans;
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnSelect);
                icon = transform.Find("Icon").GetComponent<Image>();
                imgFight = transform.Find("Imag_Fight").GetComponent<Image>();
            }

            private void OnSelect(bool isOn)
            {
                if (isOn)
                {
                    action?.Invoke(partnerId);
                }
            }

            public void Register(Action<uint> actionParam)
            {
                this.action = actionParam;
            }

            public void UpdateInfo(uint id, int index, int selectIndex)
            {
                partnerId = id;
                CSVPartner.Data data = CSVPartner.Instance.GetConfData(id);
                ImageHelper.SetIcon(icon, data.battle_headID);
                toggle.SetSelected(index == selectIndex, true);

                bool isInform = Sys_Partner.Instance.IsInForm(partnerId);
                imgFight.gameObject.SetActive(isInform);
            }
        }
        
        private Transform transform;

        private InfinityGrid _infinityGrid;
        private IListener _listener;
        private List<uint> listPartners = new List<uint>();
        private int selectIndex = 0;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            HeadCell headCell = new HeadCell();
            headCell.Init(cell.mRootTransform);
            headCell.Register(OnSelectPartner);
            
            cell.BindUserData(headCell);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            HeadCell headCell = cell.mUserData as HeadCell;
            headCell.UpdateInfo(listPartners[index], index, selectIndex);
        }

        private void OnSelectPartner(uint id)
        {
            _listener?.OnSelectPartner(id);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public void UpdateInfo(uint selectId)
        {
            List<Partner> temp = Sys_Partner.Instance.GetUnlockPartners();
            listPartners.Clear();
            selectIndex = 0;
            if (selectId != 0)
                listPartners.Add(selectId);
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].InfoId != selectId)
                    listPartners.Add(temp[i].InfoId);
            }
            
            _infinityGrid.CellCount = listPartners.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(selectIndex);
        }

        public interface IListener
        {
            void OnSelectPartner(uint id);
        }
    }
}
