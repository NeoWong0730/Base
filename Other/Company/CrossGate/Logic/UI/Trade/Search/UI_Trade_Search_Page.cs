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
    public class UI_Trade_Search_Page
    {
        private class PageCell
        {
            private Transform transform;

            private Sys_Trade.SearchPageType m_SearchType;
            private CP_Toggle m_Toggle;

            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = trans.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnClick);
            }

            public void SetPage(uint pageType)
            {
                m_SearchType = (Sys_Trade.SearchPageType)(pageType + 1);
            }

            private void OnClick(bool isOn)
            {
                if (isOn)
                {
                    Sys_Trade.Instance.SetSearchPageType(m_SearchType);
                }
            }

            public void OnSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<PageCell> listCells = new List<PageCell>();        

        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                PageCell cell = new PageCell();
                cell.Init(transform.GetChild(i));
                cell.SetPage((uint)i);
                listCells.Add(cell);
            }
        }

        public void OnSelect(Sys_Trade.SearchPageType type)
        {
            int index = (int)type - 1;
            if (index < listCells.Count)
            {
                listCells[index].OnSelect(true);
            }
        }
    }
}


