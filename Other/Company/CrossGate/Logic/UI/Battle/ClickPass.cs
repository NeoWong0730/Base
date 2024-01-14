using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic
{
    public class ClickPass : MonoBehaviour,  IPointerClickHandler,IPointerDownHandler,IPointerUpHandler
    {
        private bool hasPassed;
        public void OnPointerDown(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
           // PassEvent(eventData, ExecuteEvents.submitHandler);
        }

        //渗透事件
        public void PassEvent<T>(PointerEventData eventData, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
        {
            if (hasPassed)
            {
                return;
            }
            hasPassed = true;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            GameObject current = eventData.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (current != results[i].gameObject)
                {
                    ExecuteEvents.Execute(results[i].gameObject, eventData, function);
                }
            }
            results.Clear();
            hasPassed = false;
        }
    }
}
