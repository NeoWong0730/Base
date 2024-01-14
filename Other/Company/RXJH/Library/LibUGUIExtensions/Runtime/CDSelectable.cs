using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 按钮点击的cd控制
[DisallowMultipleComponent]
[RequireComponent(typeof(Selectable))]
public class CDSelectable : MonoBehaviour, IPointerClickHandler {
    public Selectable selectable;
    public CDSelectableRegistry registry;

    [Header("如果有registry，会使用registry的cd")] [Range(0.1f, 5f)]
    public float selfCd = 0.4f;
    
    public bool status {
        get { return selectable.interactable; }
        set {
            if (selectable.interactable != value) {
                selectable.interactable = value;

                GraphicGrayer grayer = null;
                if (!value) {
                    if (!transform.TryGetComponent<GraphicGrayer>(out grayer)) {
                        grayer = gameObject.AddComponent<GraphicGrayer>();
                    }
                }

                if (grayer != null) {
                    grayer.Status = value;
                }
            }
        }
    }

    protected virtual void Awake() {
        if (selectable == null) {
            selectable = GetComponent<Selectable>();
        }
    }

    protected virtual void OnEnable() {
        if (registry != null) {
            registry.Register(this);
        }
    }

    protected virtual void OnDisable() {
        if (registry != null) {
            registry.Unregister(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Left) {
            return;
        }

        OnClicked();
    }

    protected virtual void OnClicked() {
        if (status) {
            if (registry == null) {
                DisableImmediately(selfCd);
            }
            else {
                registry.OnAnyClicked();
            }
        }
    }

    public void DisableImmediately(float cdTime) {
        CancelInvoke(nameof(_CDCtrl));
        Invoke(nameof(_CDCtrl), cdTime);
        status = false;
    }

    private void _CDCtrl() {
        status = true;
    }
}