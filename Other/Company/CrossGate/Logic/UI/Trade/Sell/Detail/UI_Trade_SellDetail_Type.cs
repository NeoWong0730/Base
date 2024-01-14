using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_SellDetail_Type
    {
        private class DeitalType_Cell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private Sys_Trade.SellDetailType m_DetailType = Sys_Trade.SellDetailType.None; //    

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClickToggle);
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    Sys_Trade.Instance.SetSellDeitalType(m_DetailType);
                }
            }

            public void SetType(uint type)
            {
                m_DetailType = (Sys_Trade.SellDetailType)(type + 1);
            }

            public void OnToggleSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<DeitalType_Cell> list = new List<DeitalType_Cell>();

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                DeitalType_Cell cell = new DeitalType_Cell();
                cell.Init(transform.GetChild(i));
                cell.SetType((uint)i);
                list.Add(cell);
            }
        }

        public void OnSelect(Sys_Trade.SellDetailType type)
        {
            int index = (int)type - 1;
            if (index < list.Count)
            {
                list[index].OnToggleSelect(true);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }
    }
}


