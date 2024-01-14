using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary> 接收拖拽控制 </summary>
public class SlotControl : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        eventData.pointerDrag.GetComponent<ScrollOrDragControl>().targetTransform.name = gameObject.name;
    }
}
