using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;


namespace Logic
{
    public class UI_FamilyBoss_RankingReward_Type
    {
        public class TypeToggle
        {
            private Transform transform;

            private CP_Toggle m_Toggle;

            private int m_index;
            private Action<int> m_Action;
            public void Init(Transform trans)
            {
                transform = trans;

                m_Toggle = transform.GetComponent<CP_Toggle>();
                m_Toggle.onValueChanged.AddListener(OnToggle);
            }

            private void OnToggle(bool isOn)
            {
                if (isOn)
                {
                    m_Action?.Invoke(m_index);
                }
            }

            public void Register(Action<int> action, int index)
            {
                m_Action = action;
                m_index = index;
            }

            public void OnSelect(bool isOn)
            {
                m_Toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<TypeToggle> m_listToggles = new List<TypeToggle>(2);

        private IListener m_Listener;

        public void Init(Transform trans)
        {
            transform = trans;

            Transform parent = transform.Find("List");
            int count = parent.childCount;
            for(int i = 0; i < count; ++i)
            {
                TypeToggle toggle = new TypeToggle();
                toggle.Init(parent.GetChild(i));
                toggle.Register(OnToggleType, i);

                m_listToggles.Add(toggle);
            }
        }

        private void OnToggleType(int index)
        {
            m_Listener?.OnType(index);
        }

        public void Register(IListener listener)
        {
            m_Listener = listener;
        }

        public void OnSelect(int index)
        {
            m_listToggles[index].OnSelect(true);
        }

        public interface IListener
        {
            void OnType(int index);
        }
    }
}