using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class DragGameObject : MonoBehaviour
    {
        protected bool isDraging = false;
        public System.Action onDrag;

        private void Awake()
        {
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(gameObject);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            eventListener.onDragEnd += OnDragEnd;
        }

        public void OnDrag(BaseEventData eventData)
        {
            isDraging = true;
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(gameObject, angle);
            onDrag?.Invoke();
        }
        public void OnDragEnd(GameObject go)
        {
            isDraging = false;
        }

        public static void AddEulerAngles(GameObject go, Vector3 angle)
        {
            if (go != null)
            {
                Vector3 localAngle = go.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                go.transform.localEulerAngles = localAngle;
            }
        }
    }
}


