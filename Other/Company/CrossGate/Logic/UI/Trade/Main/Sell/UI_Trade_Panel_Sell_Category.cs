using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Panel_Sell_Category
    {
        private class Category
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private Sys_Trade.SellCategory m_sellCategory = Sys_Trade.SellCategory.None;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClick);
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                    Sys_Trade.Instance.SetSellCategory(m_sellCategory);
            }

            public void SetType(uint type)
            {
                m_sellCategory = (Sys_Trade.SellCategory)(type + 1);
            }

            public void OnSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<Category> listCells = new List<Category>();

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Category cell = new Category();
                cell.Init(transform.GetChild(i));
                cell.SetType((uint)i);
                listCells.Add(cell);
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

        public void OnSelectCategory(Sys_Trade.SellCategory sellCategory)
        {
            int index = (int)sellCategory - 1;
            if (index < listCells.Count)
            {
                listCells[index].OnSelect(true);
            }
        }
    }
}


