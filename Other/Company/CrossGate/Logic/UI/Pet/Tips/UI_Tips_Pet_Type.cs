using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Tips_Pet_Type
    {
        private class CellType
        {
            private Transform transform;

            private CP_Toggle _toggle;

            private int _index;
            private System.Action<int> _action;
            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnToggle);
            }

            private void OnToggle(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_index);
            }

            public void SetIndex(int index)
            {
                _index = index;
            }

            public void Register(System.Action<int> action)
            {
                _action = action;
            }

            public void OnSelect(bool isOn)
            {
                _toggle.SetSelected(isOn, true);
            }
        }

        private Transform transform;

        private List<CellType> listTypes = new List<CellType>();

        private IListener _listener;
        public void Init(Transform trans)
        {
            transform = trans;

            int count = transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                CellType cell = new CellType();
                cell.Init(transform.GetChild(i));
                cell.SetIndex(i);
                cell.Register(OnSelectType);

                listTypes.Add(cell);
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);

            UpdateInfo();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        private void OnSelectType(int index)
        {
            _listener?.OnSelectType(index);
        }

        private void UpdateInfo()
        {
            if (listTypes.Count > 0)
                listTypes[0].OnSelect(true);
        }

        public interface IListener
        {
            void OnSelectType(int index);
        }
    }
}
