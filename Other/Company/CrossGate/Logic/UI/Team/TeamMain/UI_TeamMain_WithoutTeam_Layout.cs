using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;
using System;

namespace Logic
{
    public class UI_TeamMain_WithoutTeam_Layout
    {
        //无队伍
        private Transform m_WithoutTeam;
        private Button m_BtnCreate;
        private Button m_BtnJoin;

        Toggle m_tCreate;
        Toggle m_tJion;

        Action onCreate;
        Action onJion;
        public void Show()
        {
            m_WithoutTeam.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_WithoutTeam.gameObject.SetActive(false);
        }

        public void Load(Transform parent)
        {
            m_WithoutTeam = parent.Find("TeamShow/Team_Create");

            m_tCreate = m_WithoutTeam.Find("Toggle_Task").GetComponent<Toggle>();
            m_tJion = m_WithoutTeam.Find("Toggle_Team").GetComponent<Toggle>();

            m_BtnCreate = m_WithoutTeam.Find("Btn_Create").GetComponent<Button>();
            m_BtnJoin = m_WithoutTeam.Find("Btn_Find").GetComponent<Button>();

        }
        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
            //m_tCreate.onValueChanged.AddListener(switchCreate);
            //m_tJion.onValueChanged.AddListener(switchJoin);

            //onCreate = listener.Create;
            //onJion = listener.Join;

            m_BtnCreate.onClick.AddListener(listener.Create);
            m_BtnJoin.onClick.AddListener(listener.Join);
        }

        private void switchCreate(bool value)
        {
            if(value)
            {
                onCreate?.Invoke();
            }
        }

        private void switchJoin(bool value)
        {
            if (value)
            {
                onJion?.Invoke();
            }
        }
    }

}
