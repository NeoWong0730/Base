using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Equipment_ViewLeftTabs : UIParseCommon
    {
        private uint funcTypeId = 10300;
        private class TabNode
        {
            private Transform transform;

            private CP_Toggle _toggle;

            private Sys_Equip.EquipmentOperations _opType = Sys_Equip.EquipmentOperations.Inlay;
            private System.Action<Sys_Equip.EquipmentOperations> _action;

            public void Init(Transform trans)
            {
                transform = trans;
                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClick);
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_opType);
            }

            public void SetType(Sys_Equip.EquipmentOperations opType)
            {
                _opType = opType;
                transform.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(10300 + (uint)opType, false));
            }

            public void RegistAction(System.Action<Sys_Equip.EquipmentOperations> action)
            {
                _action = action;
            }

            public void OnSelect(bool isOn)
            {
                _toggle.SetSelected(isOn, true);
            }
        }

        private List<TabNode> tabList = new List<TabNode>();

        protected override void Parse()
        {
            tabList.Clear();

            for (int i = 1; i < (int)Sys_Equip.EquipmentOperations.Max; ++i)
            {
                string tabName = string.Format("Label_Scroll01/TabList/TabItem0{0}", i);

                TabNode node = new TabNode();
                node.Init(transform.Find(tabName));
                node.SetType((Sys_Equip.EquipmentOperations)i);
                node.RegistAction(OnSelect);

                tabList.Add(node);
            }
        }

        private void OnSelect(Sys_Equip.EquipmentOperations opType)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(funcTypeId + (uint)opType, true)) { return; }

            Sys_Equip.Instance.eventEmitter.Trigger(Sys_Equip.EEvents.OnOperationType, opType);
        }

        public void OnDefaultSelect(Sys_Equip.EquipmentOperations opType)
        {
            int index = (int) opType - 1;
            if (!Sys_FunctionOpen.Instance.IsOpen(funcTypeId + (uint)opType, true)) { return; }
            
            if (index < tabList.Count)
                tabList[index].OnSelect(true);
        }
    }
}


