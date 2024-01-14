using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PenetrationClick : MonoBehaviour, IPointerClickHandler
{
    public Action onCloseClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.submitHandler);
    }

    private void PassEvent<T>(PointerEventData data,ExecuteEvents.EventFunction<T> funtion) where T:IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        GameObject propItem=null;
        bool clickOnPropItem = false;
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.GetComponent<PropItemFalg>())
            {
                clickOnPropItem = true;
                propItem = results[i].gameObject;
                break;
            }
            //if (current != results[i].gameObject)
            //{
            //    if (results[i].gameObject.GetComponent<PropItemFalg>())
            //    {
            //        ExecuteEvents.Execute(results[i].gameObject, data, funtion);
            //    }
            //    else
            //    {
            //        onClick?.Invoke();
            //    }
            //}
        }
        if (clickOnPropItem)
        {
            ExecuteEvents.Execute(propItem, data, funtion);
        }
        else
        {
            onCloseClick?.Invoke();
        }
    }
}
