using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CP_ToggleP : CP_Toggle
{
    public override void SetSelected(bool value, bool sendMessage) {
        // keep first
        this.IsOn = value;

        this.Highlight(this.IsOn);

        if (sendMessage) {
            this.onValueChanged.Invoke(this.IsOn);
        }
        // 当前选中的时候，设置其他tab未选中
        if (this.IsOn && this.ownerRegistry != null) {
            this.ownerRegistry.NotifyOtherToggleOff(this, true);
        } 
    }

}
