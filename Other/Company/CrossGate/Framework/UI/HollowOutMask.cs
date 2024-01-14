using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary> 镂空遮罩 </summary>
public class HollowOutMask : MonoBehaviour, IPointerUpHandler
{
    /// <summary> 点击事件 </summary>
    public Action onClick;
    /// <summary> 摇杆输入 </summary>
    private JoystickInput joystickInput;
    /// <summary> 触摸输入 </summary>
    private TouchInput touchInput;
    /// <summary> 目标 </summary>
    private Transform target;
    /// <summary> UI目标 </summary>
    private RectTransform rt_target;
    /// <summary> 碰撞盒 </summary>
    private Collider c_target;
    /// <summary> 父节点UI坐标系 </summary>
    private RectTransform rt_Parent;
    /// <summary> 当前脚本UI坐标系 </summary>
    private RectTransform rt_Mask;
    /// <summary> UI坐标 </summary>
    private Vector3 pos;
    void Awake()
    {
        rt_Parent = transform.parent as RectTransform;
        rt_Mask = transform as RectTransform;
    }

    void OnDisable()
    {
        joystickInput = null;
        touchInput = null;
        target = null;
        rt_target = null;
        c_target = null;
    }

    void Update()
    {
        if (null == target) return;

        if (null != rt_target)//UI目标
        {
            if (rt_Mask.sizeDelta != rt_target.sizeDelta)
                rt_Mask.sizeDelta = rt_target.sizeDelta;
            if (rt_Mask.position != rt_target.position)
                rt_Mask.position = rt_target.position;
            if (rt_Mask.localEulerAngles != rt_target.localEulerAngles)
                rt_Mask.localEulerAngles = rt_target.localEulerAngles;
            if (rt_Mask.localScale != rt_target.localScale)
                rt_Mask.localScale = rt_target.localScale;
        }
        else if (null != c_target)//碰撞盒目标
        {
            Vector2 screenPoint = CameraManager.mCamera.WorldToScreenPoint(c_target.bounds.center);
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt_Parent, screenPoint, CameraManager.mUICamera, out pos))
            {
                rt_Mask.position = pos;
            }
        }
    }
    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="size"></param>
    public void SetTarget(Transform tr, Vector2 size)
    {
        joystickInput = GameObject.FindObjectOfType<JoystickInput>();
        touchInput = GameObject.FindObjectOfType<TouchInput>();
        target = tr;
        gameObject.SetActive(target != null);
        if (null == target)
        {
            //强制结束
            onClick?.Invoke();
            return;
        }
        rt_target = target as RectTransform;
        c_target = target.GetComponent<Collider>();

        if (null != rt_target)//UI目标
        {
            rt_Mask.pivot = rt_target.pivot;
            rt_Mask.anchorMin = rt_target.anchorMin;
            rt_Mask.anchorMax = rt_target.anchorMax;
            rt_Mask.sizeDelta = rt_target.sizeDelta;
            rt_Mask.position = rt_target.position;
            rt_Mask.localEulerAngles = rt_target.localEulerAngles;
            rt_Mask.localScale = rt_target.localScale;
        }
        else if (null != c_target)//碰撞盒目标
        {
            rt_Mask.pivot = new Vector2(0.5f, 0.5f);
            rt_Mask.anchorMin = new Vector2(0.5f, 0.5f);
            rt_Mask.anchorMax = new Vector2(0.5f, 0.5f);
            rt_Mask.sizeDelta = size;
            rt_Mask.localEulerAngles = Vector3.zero;
            rt_Mask.localScale = Vector3.one;

            Vector2 screenPoint = CameraManager.mCamera.WorldToScreenPoint(c_target.bounds.center);
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt_Parent, screenPoint, CameraManager.mUICamera, out pos))
            {
                rt_Mask.position = pos;
            }
        }
    }
    /// <summary>
    /// 遮罩坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMaskPosition()
    {
        if (null == target) return Vector3.zero;
        else return rt_Mask.position;
    }

    public void SetMaskPivot(Vector2 pivot)
    {
        if (rt_Mask != null)
            rt_Mask.pivot = pivot;
    }

    /// <summary>
    /// 点击抬起
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (null == target) return;
        //穿透事件处理
        if (null != rt_target)
        {
            Button button = target.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.Invoke();
                UtlFunc();
                return;
            }
            Toggle toggle = target.GetComponent<Toggle>();
            if (null != toggle)
            {
                toggle.isOn = !toggle.isOn;
                UtlFunc();
                return;
            }
            CP_Toggle cP_Toggle = target.GetComponent<CP_Toggle>();
            if (null != cP_Toggle)
            {
                cP_Toggle.OnPointerClick(eventData);
                UtlFunc();
                return;
            }
            Lib.Core.EventTrigger eventTrigger = target.GetComponent<Lib.Core.EventTrigger>();
            if (null != eventTrigger)
            {
                eventTrigger.OnPointerClick(eventData);
                UtlFunc();
                return;
            }
        }
        else if (null != c_target && null != eventData)
        {
            //修正点击坐标，必然点击到物体
            Vector2 screenPoint = CameraManager.mCamera.WorldToScreenPoint(c_target.bounds.center);
            eventData.position = screenPoint;
            joystickInput?.OnPointerUp(eventData);
            touchInput?.OnPointerUp(eventData);
        }

        UtlFunc();
    }

    private void UtlFunc()
    {
        //当前事件处理
        onClick?.Invoke();
        //关闭遮罩
        gameObject.SetActive(false);
    }
}
