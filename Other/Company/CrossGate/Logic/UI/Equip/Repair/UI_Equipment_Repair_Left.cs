using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Equipment_Repair_Left : UIParseCommon
    {
        private ToggleGroup toggleGroup;
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Repair_EquipRoot> dicEquipments = new Dictionary<GameObject, UI_Repair_EquipRoot>();
        private int visualGridCount;

        private IListener listener;
        private int curSelectIndex;

        protected override void Parse()
        {
            toggleGroup = transform.Find("SmeltListRoot/Scroll_Equip/Grid").gameObject.GetComponent<ToggleGroup>();
            gridGroup = transform.Find("SmeltListRoot/Scroll_Equip/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 7;
            gridGroup.GetComponent<ToggleGroup>().allowSwitchOff = false;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);

                UI_Repair_EquipRoot iconRoot = new UI_Repair_EquipRoot();
                iconRoot.Init(trans);
                iconRoot.AddListener(OnSelectEquipment);
                dicEquipments.Add(trans.gameObject, iconRoot);
            }
        }

        public override void Hide()
        {
            foreach (UI_Repair_EquipRoot node in dicEquipments.Values)
            {
                node.ToggleOff();
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicEquipments.ContainsKey(trans.gameObject))
            {
                UI_Repair_EquipRoot iconRoot = dicEquipments[trans.gameObject];
                iconRoot.UpdateEquipInfo(Sys_Equip.Instance.EquipListUIds[index], index);

                if (curSelectIndex == index)
                {
                    iconRoot.OnSelect(true);
                }
                else
                {
                    iconRoot.OnSelect(false);
                }
            }
        }

        public override void UpdateInfo(ItemData _item)
        {
            curSelectIndex = -1;

            ////cal sort equipments
            //Sys_Equip.Instance.SortEquipments(Sys_Equip.EquipmentOperations.Repair, _item);
            //visualGridCount = Sys_Equip.Instance.EquipListUIds.Count;
            //gridGroup.SetAmount(visualGridCount);
            //toggleGroup.EnsureValidState();

            //if (visualGridCount == 0)
            //    listener?.OnSelectEquipment(0);
        }

        private void OnSelectEquipment(UI_Repair_EquipRoot _equipNode)
        {
            curSelectIndex = _equipNode.GridIndex;
            _equipNode.OnSelect(true);

            ulong equipUId = 0;
            if (_equipNode != null)
            {
                equipUId = _equipNode.UId;
            }

            listener?.OnSelectEquipment(equipUId);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectEquipment(ulong _uId);
        }
    }
}


