using UnityEngine;
using Logic.Core;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Safe_Right_Table : UIComponent
    {
        private List<CP_Toggle> listToggles = new List<CP_Toggle>();
        private IListener listener;        
        protected override void Loaded()
        {
            int length = Enum.GetValues(typeof(ESafeType)).Length;

            for (int i = 0; i < length; ++i)
            {
                CP_Toggle toggle = transform.Find($"Scroll View/TabList/TabItem{i}").GetComponent<CP_Toggle>();
                listToggles.Add(toggle);
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnToggleClick(isOn, listToggles.IndexOf(toggle));
                });
            }
        }

        public override void Show()
        {
            listToggles[0].SetSelected(true, true);
        }

        public override void Hide()
        {
            for (int i = 0; i < listToggles.Count; i++)
            {
                listToggles[i].SetSelected(false, false);
            }
        }

        private void OnToggleClick(bool _isOn, int _index)
        {
            if (_isOn)
            {
                listener?.OnClickTabType((ESafeType)_index);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnClickTabType(ESafeType _type);
        }
    }

}

