using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Record_Type
    {
        private class Cell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;
            private uint m_Type;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClick);
            }

            public void SetType(uint type)
            {
                m_Type = type;
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                {
                    Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnRecordType, m_Type);
                }
            }

            public void OnSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        Transform transform;

        private List<Cell> listCells = new List<Cell>();

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Cell cell = new Cell();
                cell.Init(transform.GetChild(i));
                cell.SetType((uint)i);
                listCells.Add(cell);
            }
        }

        public void OnSelect(uint type)
        {
            if (type < listCells.Count)
            {
                listCells[(int)type].OnSelect(true);
            }
        }
    }
}


