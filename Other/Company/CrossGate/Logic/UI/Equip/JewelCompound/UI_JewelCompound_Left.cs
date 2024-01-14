using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_JewelCompound_Left : UIParseCommon, UI_JewelCompound_Left_TypeSwitch.IListener
    {
        private InfinityGrid _infinityGrid;

        private List<Sys_Equip.JewelGroupData> listJewels;

        private IListener listener;

        private UI_JewelCompound_Left_TypeSwitch typeSwitch;

        protected override void Parse()
        {
            _infinityGrid = transform.Find("Scroll_View_Gem").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            typeSwitch = new UI_JewelCompound_Left_TypeSwitch();
            typeSwitch.Init(transform.Find("Button_Switch"));
            typeSwitch.ReisterListener(this);
        }

        public override void Show()
        {
            typeSwitch.Show();
        }

        public override void Hide()
        {
            typeSwitch.Hide();
        }

        public void UpdateType(ItemData itemData)
        {
            if (itemData != null)
            {
                Table.CSVJewel.Data jewelInfo = Table.CSVJewel.Instance.GetConfData(itemData.Id);
                typeSwitch.UpdateType((EJewelType)jewelInfo.jewel_type);
            }
            else
            {
                typeSwitch.UpdateType(EJewelType.All);
            }
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_JewelIconRoot entry = new UI_JewelIconRoot();
            entry.AddListener(OnSelectJewel);
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_JewelIconRoot entry = cell.mUserData as UI_JewelIconRoot;
            entry.UpdateJewelInfo(listJewels[index], index);
        }

        public void ReisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public void UpdateJewelList(List<Sys_Equip.JewelGroupData> list, int selectIndex)
        {
            Sys_Equip.Instance.SelectJewelGroupIndex = selectIndex;
            if (list.Count == 0)
            {
                listener?.SelectedJewel(null);
            }

            listJewels = list;
            _infinityGrid.CellCount = listJewels.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(selectIndex);
           
            //visualGridCount = listJewels.Count;
            //gridGroup.SetAmount(visualGridCount);
        }

        private void OnSelectJewel(Sys_Equip.JewelGroupData groupData)
        {
            listener?.SelectedJewel(groupData);
        }

        public void SwitchJewelType(EJewelType _jewelType)
        {
            listener?.SwitchJewelType(_jewelType);
        }

        public interface IListener
        {
            void SelectedJewel(Sys_Equip.JewelGroupData _jewelData);
            void SwitchJewelType(EJewelType _type);
        }
    }
}


