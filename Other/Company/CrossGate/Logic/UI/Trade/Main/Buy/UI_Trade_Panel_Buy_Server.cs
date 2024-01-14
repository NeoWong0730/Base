using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Panel_Buy_Server
    {
        private class UI_Trade_Server_Cell 
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private Sys_Trade.ServerType m_ServerType;

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
                    Sys_Trade.Instance.SetBuyServerType(m_ServerType);
                }
            }

            public void SetServerType(uint serverType)
            {
                m_ServerType = (Sys_Trade.ServerType)(serverType);
            }

            public void OnToggleSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<UI_Trade_Server_Cell> listServerType = new List<UI_Trade_Server_Cell>();

        public void Init(Transform trans)
        {
            transform = trans;

            for (int i = 0; i < 2; ++i)
            {
                UI_Trade_Server_Cell serverType = new UI_Trade_Server_Cell();
                serverType.Init(transform.Find(string.Format("Toggle{0}", i)));
                serverType.SetServerType((uint)i);
                listServerType.Add(serverType);
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

        public void OnSelectServer(Sys_Trade.ServerType serverType)
        {
            for (int i = 0; i < listServerType.Count; ++i)
                listServerType[i].OnToggleSelect(false);

            int index = (int)serverType;
            if (index < listServerType.Count)
            {
                listServerType[index].OnToggleSelect(true);
            }
        }
    }
}


