using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> 镂空背景 </summary>
public class HollowOutBg : MonoBehaviour, ICanvasRaycastFilter, IPointerClickHandler
{
    /// <summary> 点击目标事件 </summary>
    public Action action_ClickTarget;
    /// <summary> 点击背景事件 </summary>
    public Action<PointerEventData> action_ClickBg;
    /// <summary> 遮罩按键 </summary>
    private Button button_mask;
    [SerializeField]
    /// <summary> 遮罩组 </summary>
    private List<RectTransform> _masks = new List<RectTransform>();
    [SerializeField]
    /// <summary> 目标组</summary>
    private List<Transform> _targets = new List<Transform>();

    void Awake()
    {
        var allNode = transform.parent.GetComponentsInChildren<Transform>();
        foreach (Transform item in allNode)
        {
            if (item == transform) continue;

            Button button = item.GetComponent<Button>();
            if (null != button)
            {
                button_mask = button;
                button_mask.gameObject.SetActive(false);
                break;
            }
        }
    }
    /// <summary>
    /// 获取第一目标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetFirstTargetPos()
    {
        if (_targets.Count <= 0) return Vector3.zero;
        else return _masks[0].GetComponent<HollowOutMask>().GetMaskPosition();
    }
    /// <summary>
    /// 设置遮罩数量
    /// </summary>
    public void SetMaskCount()
    {
        while (_targets.Count > _masks.Count)
        {
            GameObject go = GameObject.Instantiate(button_mask.gameObject, button_mask.transform.parent);
            go.GetComponent<HollowOutMask>().onClick = OnClick_Mask;
            _masks.Add(go.transform as RectTransform);
        }
    }
    /// <summary>
    /// 设置目标组
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="size"></param>
    public void SetTargets(List<Transform> targets, Vector2 size)
    {
        _targets.Clear();
        if (null != targets)
            targets.ForEach(x => _targets.Add(x));
        SetMaskCount();

        for (int i = 0; i < _masks.Count; i++)
        {
            if (i >= _targets.Count)
            {
                _masks[i].gameObject.SetActive(false);
            }
            else
            {
                _masks[i].gameObject.SetActive(true);
                _masks[i].GetComponent<HollowOutMask>().SetTarget(_targets[i], size);
            }
        }
    }

    public void SetMaskPivot(Vector2 pivot)
    {
        for (int i = 0; i < _masks.Count; i++)
        {
            _masks[i].GetComponent<HollowOutMask>().SetMaskPivot(pivot);
        }
    }

    /// <summary>
    /// 清理目标
    /// </summary>
    public void ClearTargets()
    {
        _targets.Clear();
        for (int i = 0; i < _masks.Count; i++)
        {
            _masks[i].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 点击遮罩
    /// </summary>
    public void OnClick_Mask()
    {
        _targets.Clear();
        //影藏所有遮罩
        _masks.ForEach(x => x.gameObject.SetActive(false));
        //传递事件
        action_ClickTarget?.Invoke();
    }
    /// <summary>
    /// 射线穿透目标
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns></returns>
    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (_masks.TrueForAll(x => x.gameObject.activeInHierarchy == false)) return true;

        foreach (var mask in _masks)
        {
            if (!mask.gameObject.activeInHierarchy) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(mask, sp, eventCamera))
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        action_ClickBg?.Invoke(eventData);
    }
    /// <summary>
    /// 直接点击目标
    /// </summary>
    public void OnClick_Target(PointerEventData eventData)
    {
        if (_targets.Count <= 0)
            return;

        _masks[0].GetComponent<HollowOutMask>().OnPointerUp(eventData);
    }
}
