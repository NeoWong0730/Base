using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPassEvent : MonoBehaviour, IPointerClickHandler
{
    /// <summary> 点击事件 </summary>
    public Action<GameObject> onClick;
    /// <summary> 摇杆输入 </summary>
    private JoystickInput joystickInput;
    /// <summary> 触摸输入 </summary>
    private TouchInput touchInput;
    /// <summary> 目标 </summary>
    private Transform target;

    public void SetTarget(Transform tr)
    {
        joystickInput = GameObject.FindObjectOfType<JoystickInput>();
        touchInput = GameObject.FindObjectOfType<TouchInput>();
        target = tr;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (null != target)
        {
            Button button = target.GetComponent<Button>();
            if (null != button)
            {
                button.onClick.Invoke();
            }
            Toggle toggle = target.GetComponent<Toggle>();
            if (null != toggle)
            {
                toggle.isOn = !toggle.isOn;
            }
            CP_Toggle cP_Toggle = target.GetComponent<CP_Toggle>();
            if (null != cP_Toggle)
            {
                cP_Toggle.OnPointerClick(eventData);
            }
            Lib.Core.EventTrigger eventTrigger = target.GetComponent<Lib.Core.EventTrigger>();
            if (null != eventTrigger)
            {
                eventTrigger.OnPointerClick(eventData);
            }
            Collider collider = target.GetComponent<Collider>();
            if (null != collider)
            {
                joystickInput?.OnPointerUp(eventData);
                touchInput?.OnPointerUp(eventData);
            }
        }
        //当前事件处理
        if (null != onClick)
            onClick(gameObject);
    }
}
