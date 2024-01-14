using UnityEngine;
using Logic.Core;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Pet_Right_Tabs
    {
        private List<CP_Toggle> listToggles = new List<CP_Toggle>();
        private IListener listener;
        public string subPStr = "";
        public int toggleCount;
        public UI_Pet_Right_Tabs(string _subPStr, int toggleCoutn)
        {
            toggleCount = toggleCoutn;
            subPStr = _subPStr;            
        }

        public void Init(Transform transform)
        {
            int length = toggleCount;

            for (int i = 0; i < length; ++i)
            {
                CP_Toggle toggle = transform.Find(string.Format("{0}{1}", subPStr.ToString(), i.ToString())).GetComponent<CP_Toggle>();
                listToggles.Add(toggle);
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    OnToggleClick(isOn, listToggles.IndexOf(toggle));
                });
            }
        }


        public void ShowEx(int index)
        {
            if(index < listToggles.Count && index >= 0)
                listToggles[index].SetSelected(true, true);
        }

        public void Hide()
        {
            for (int i = 0; i < listToggles.Count; i++)
            {
                listToggles[i].SetSelected(false, true);
            }            
        }

        private void OnToggleClick(bool _isOn, int _index)
        {
            if (_isOn)
            {
                listener?.OnClickTabType(_index);
            }
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnClickTabType(int _type);
        }
    }

}

