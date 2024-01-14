using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragEventTrigger : MonoBehaviour, IDragHandler
{
    public TriggerEvent trigger = new TriggerEvent();

    public void OnDrag(PointerEventData eventData)
    {
        trigger.Invoke(eventData);
    }

    public class TriggerEvent : UnityEvent<PointerEventData>
    {
        public TriggerEvent() { }
    }
}
