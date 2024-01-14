using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CP_Toggle : Selectable, IPointerClickHandler {

    public enum ToggleType
    {
        None,
        Single
    }

    public class ToggleEvent : UnityEvent<bool> { }
    public ToggleEvent onValueChanged = new ToggleEvent();

    public int id = 0;
    public CP_TransformContainer transformContainer;
    public GameObject single;
    public CP_ToggleRegistry ownerRegistry;
    public ToggleType toggleType = ToggleType.None;

    private Func<bool> condition;
    private bool isChangeState = true;

    protected override void Awake() {
        base.Awake();
        if (this.ownerRegistry == null) {
            this.ownerRegistry = this.GetComponentInParent<CP_ToggleRegistry>();
        }
    }

    protected override void OnEnable() {
        if (this.ownerRegistry != null) {
            this.ownerRegistry.RegisterToggle(this);
            this.ownerRegistry.RegisterCondition(this.id);
        }
    }
    protected override void OnDisable() {
        if (this.ownerRegistry != null) {
            this.ownerRegistry.UnregisterToggle(this);
            this.ownerRegistry.UnRegisterCondition(this.id);
        }
    }

    public bool IsOn { get; protected set; }

    public void RegisterCondition(Func<bool> _condition) {
        this.condition = _condition;
    }

    public void UnRegisterCondition() {
        this.condition = null;
    }

    public void SetToggleIsNotChange(bool value)
    {
        isChangeState = value;
    }

    public virtual void SetSelected(bool value, bool sendMessage) {
        // keep first
        this.IsOn = value;

        if (sendMessage) {
            this.onValueChanged.Invoke(this.IsOn);
        }
        // 当前选中的时候，设置其他tab未选中
        if (this.IsOn && this.ownerRegistry != null && isChangeState) {
            this.ownerRegistry.NotifyOtherToggleOff(this, true);
        }

        this.Highlight(this.IsOn && isChangeState);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) { return; }
        if (!this.IsActive() || !this.IsInteractable()) { return; }

        if (this.condition != null && !this.condition())
            return;

        if (this.ownerRegistry != null && this.ownerRegistry.allowSwitchOff)
        {
            this.SetSelected(!this.IsOn, true);
        }
        else
        {
            if (toggleType == ToggleType.Single)
            {
                this.SetSelected(!this.IsOn, true);
            }
            else
            {
                this.SetSelected(true, true);
            }
        }
    }

    #region 表现
    // Toggle控制Graphic 显/隐
    public void Highlight(bool highlight) {
        if (this.transformContainer != null) {
            this.transformContainer.ShowHideBySetActive(highlight);
        }
        if (this.single != null) {
            this.single.SetActive(highlight);
        }
    }
    #endregion
}
