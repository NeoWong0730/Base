using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Knowledge_Annals_Bottom
    {
        private class TogglePoint
        {
            private Transform transform;

            private CP_Toggle _toggle;

            private int _index;
            private System.Action<int> _action;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            public void SetIndex(int index)
            {
                _index = index;
            }

            public void Register(System.Action<int> action)
            {
                _action = action;
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                    _action?.Invoke(_index);
            }
        }

        private Transform transform;

        private List<TogglePoint> listPoints = new List<TogglePoint>(8);

        private IListener _listener;

        public void Init(Transform trans)
        {
            transform = trans;

            for (int i = 0; i < 5; ++i)
            {
                Transform temp = transform.Find(string.Format("Toggle{0}", i));

                TogglePoint point = new TogglePoint();
                point.Init(temp);
                point.SetIndex(i);
                point.Register(OnClickPoint);

                listPoints.Add(point);
            }
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {

        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        private void OnClickPoint(int index)
        {
            _listener?.OnClickPoint(index);
        }

        public interface IListener
        {
            void OnClickPoint(int index);
        }
    }
}


