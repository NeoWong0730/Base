using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Sell_Option_Type
    {
        private class Option_Type_Cell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private Sys_Trade.SellOpType m_OpType = Sys_Trade.SellOpType.None; //
            
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
                    Sys_Trade.Instance.SetSellOptionType(m_OpType);
                }
            }

            public void SetServerType(uint serverType)
            {
                m_OpType = (Sys_Trade.SellOpType)(serverType + 1);
            }

            public void OnToggleSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<Option_Type_Cell> list = new List<Option_Type_Cell>();

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Option_Type_Cell serverType = new Option_Type_Cell();
                serverType.Init(transform.GetChild(i));
                serverType.SetServerType((uint)i);
                list.Add(serverType);
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

        public void OnSelect(Sys_Trade.SellOpType opType)
        {
            int index = (int)opType - 1;
            if (index < list.Count)
            {
                list[index].OnToggleSelect(true);
            }
        }
    }
}


