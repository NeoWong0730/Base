using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Equipment_Quenching_Left : UIParseCommon
    {
        private InfinityGridLayoutGroup gridGroup;
        private Dictionary<GameObject, UI_Quenching_EquipRoot> dicEquipments = new Dictionary<GameObject, UI_Quenching_EquipRoot>();
        private int visualGridCount;

        private IListener listener;
        private int curSelectIndex;

        protected override void Parse()
        {
            gridGroup = transform.Find("SmeltListRoot/Scroll_Equip/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.minAmount = 7;
            gridGroup.GetComponent<ToggleGroup>().allowSwitchOff = true;
            gridGroup.updateChildrenCallback = UpdateChildrenCallback;

            for (int i = 0; i < gridGroup.transform.childCount; ++i)
            {
                Transform trans = gridGroup.transform.GetChild(i);

                UI_Quenching_EquipRoot iconRoot = new UI_Quenching_EquipRoot();
                iconRoot.Init(trans);
                iconRoot.AddListener(OnSelectEquipment);
                dicEquipments.Add(trans.gameObject, iconRoot);
            }
        }

        private void UpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= visualGridCount)
                return;

            if (dicEquipments.ContainsKey(trans.gameObject))
            {
                UI_Quenching_EquipRoot iconRoot = dicEquipments[trans.gameObject];
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

            //cal sort equipments
            //Sys_Equip.Instance.SortEquipments(Sys_Equip.EquipmentOperations.Quenching, _item);
            //visualGridCount = Sys_Equip.Instance.EquipListUIds.Count;
            //gridGroup.SetAmount(visualGridCount);
        }

        private void OnSelectEquipment(UI_Quenching_EquipRoot _equipNode)
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


