using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;

namespace Logic
{
    public enum EEnergysparTabType
    {
        Main = 0,
        Shop = 1,
    }

    public class UI_Energyspar_Right_Tabs
    {
        private List<Toggle> listToggles = new List<Toggle>();
        private IListener listener;
        
        public void Init(Transform transform)
        {
            int length = Enum.GetValues(typeof(EEnergysparTabType)).Length;

            for (int i = 0; i < length; ++i)
            {
                Toggle toggle = transform.Find($"Scroll View/TabList/TabItem{i}").GetComponent<Toggle>();
                listToggles.Add(toggle);
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnToggleClick(isOn, listToggles.IndexOf(toggle));
                });
            }
        }

        public void Hide()
        {
            for (int i = 0; i < listToggles.Count; i++)
            {
                listToggles[i].isOn = false;
            }
        }

        public void OnTabIndex(int tabIndex)
        {
            if(!listToggles[tabIndex].isOn)
            {
                listToggles[tabIndex].isOn = true;
            }
            else
            {
                listener?.OnClickTabType((EEnergysparTabType)tabIndex);
            }
                
        }

        private void OnToggleClick(bool _isOn, int _index)
        {
            if (_isOn)
            {
                listener?.OnClickTabType((EEnergysparTabType)_index);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnClickTabType(EEnergysparTabType _type);
        }
    }
}