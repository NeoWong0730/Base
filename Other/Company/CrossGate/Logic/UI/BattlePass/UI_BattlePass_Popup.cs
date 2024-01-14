using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class UI_BattlePass_Popup: UIBase, UI_BattlePass_Popup_Layout.IListener
    {
        UI_BattlePass_Popup_Layout m_Layout = new UI_BattlePass_Popup_Layout();

        private int m_FocusIndex = 0;

        List<CSVPublicityMap.Data> m_Datas;
        public void OnClickLeft()
        {
            m_FocusIndex = Mathf.Max(0,m_FocusIndex - 1);

            m_Layout.SetFocus(m_FocusIndex);

            m_Layout.SetLeftActive(m_FocusIndex > 0);
            m_Layout.SetRightActive(m_FocusIndex < m_Datas.Count - 1);
        }

        public void OnClickRight()
        {
            m_FocusIndex = Mathf.Min(m_Datas.Count - 1, m_FocusIndex + 1);
            m_Layout.SetFocus(m_FocusIndex);

            m_Layout.SetLeftActive(m_FocusIndex > 0);
            m_Layout.SetRightActive(m_FocusIndex < m_Datas.Count - 1);
        }

        public void OnClickStart()
        {
            CloseSelf();

            UIManager.OpenUI(EUIID.UI_BattlePass);
        }

        public string OnItemUpdate(int index)
        {
            if (index >= m_Datas.Count)
                return string.Empty;

            return m_Datas[index].Picture;
        }

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            m_Datas = Sys_BattlePass.Instance.GetPublicityTableData();
        }

        protected override void OnShow()
        {
            m_FocusIndex = 0;

            m_Layout.SetCount(m_Datas.Count);
            m_Layout.SetFocus(0);

            Sys_BattlePass.Instance.SetFristEnterTime();

            m_Layout.SetLeftActive(false);
            m_Layout.SetRightActive(m_Datas.Count > 1);
        }
    }
}
