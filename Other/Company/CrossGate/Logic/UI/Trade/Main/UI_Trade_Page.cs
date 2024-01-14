using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Page
    {
        private class UI_Trade_Page_Cell
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private Sys_Trade.PageType m_PageType;

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
                    Sys_Trade.Instance.SetPageType(m_PageType);
                }
            }

            public void SetType(uint pageType)
            {
                m_PageType = (Sys_Trade.PageType)pageType;
            }

            public void OnToggleSelect(bool isSelect)
            {
                m_Toggle.SetSelected(isSelect, true);
            }
        }

        private Transform transform;

        private List<UI_Trade_Page_Cell> listCells = new List<UI_Trade_Page_Cell>();

        public void Init(Transform trans)
        {
            transform = trans;

            Transform tranParent = transform.Find("Toggles_button");
            int count = tranParent.childCount;
            for (int i = 0; i < count; ++i)
            {
                UI_Trade_Page_Cell cell = new UI_Trade_Page_Cell();
                cell.Init(tranParent.GetChild(i));
                cell.SetType((uint)i + 1);
                listCells.Add(cell);
            }
        }

        public void OnSelectPage(Sys_Trade.PageType pageType)
        {
            int index = (int)pageType - 1;
            if (index < listCells.Count)
            {
                listCells[index].OnToggleSelect(true);
            }
        }
    }
}


