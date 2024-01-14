using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> 滑动或拖拽控制 </summary>
public class ScrollOrDragControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary> 克隆数据 </summary>
    public delegate void OnCopyDataCallback(Transform curTransform, Transform copyTransform);
    public static OnCopyDataCallback onCopyDataCallback = null;
    /// <summary> 交互数据 </summary>
    public delegate void OnChangeDataCallback(Transform curTransform, Transform copyTransform);
    public static OnChangeDataCallback onChangeDataCallback = null;
    /// <summary> 画布 </summary>
    private Canvas canvas;
    /// <summary> 滑动区域 </summary>
    private ScrollRect scrollRect;
    /// <summary> 目标 </summary>
    [SerializeField]
    public RectTransform targetTransform;
    /// <summary> 目标区域 </summary>
    [SerializeField]
    public GameObject targetRect;
    /// <summary> 是否拖拽 </summary>
    private bool isDragged;

    private void Start()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        scrollRect = transform.GetComponentInParent<ScrollRect>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = null == targetTransform ? false : Mathf.Abs(eventData.delta.normalized.x) > 0.8f;//横向拖动
        if (isDragged)
        {
            targetTransform.position = transform.position;
            targetTransform.name = transform.name;
            targetTransform.gameObject.SetActive(true);
            targetRect.SetActive(true);
            onCopyDataCallback?.Invoke(transform, targetTransform);
        }
        else
        {
            scrollRect.OnBeginDrag(eventData);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            targetTransform.anchoredPosition += eventData.delta;
        }
        else
        {
            scrollRect.OnDrag(eventData);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragged)
        {
            targetTransform.gameObject.SetActive(false);
            targetRect.SetActive(false);

            if (targetTransform.name != transform.name)
            {
                onChangeDataCallback?.Invoke(transform, targetTransform);
            }
        }
        else
        {
            scrollRect.OnEndDrag(eventData);
        }
    }
}
