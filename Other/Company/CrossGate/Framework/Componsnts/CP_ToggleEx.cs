using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CP_ToggleEx : CP_Toggle {
    protected override void Awake() {
        base.Awake();
        if (this.ownerRegistry != null)
        {
            this.ownerRegistry.RegisterToggle(this);
            this.ownerRegistry.RegisterCondition(this.id);
        }
    }

    protected override void OnEnable()
    {
    }

    protected override void OnDisable()
    {
    }
}
