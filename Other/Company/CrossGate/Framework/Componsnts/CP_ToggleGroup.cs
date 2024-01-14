using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CP_ToggleGroup : MonoBehaviour {
    public enum EToggleType {
        HideOrShow,
        Loop,//循序切换激活态和非激活态
    }

    public Toggle toggle;
    public EToggleType toggleType = EToggleType.HideOrShow;
    public List<Transform> activeTransforms = new List<Transform>(0);
    public List<Transform> deActiveTransforms = new List<Transform>(0);

    private bool m_isActive = false;

    public void OnEnable() {
        if (this.toggle != null && this.toggle.isOn != this.m_isActive)
            this.OnValueChanged(this.toggle.isOn);
    }

    private void Awake() {
        this.toggle = this.GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(this.OnValueChanged);
    }
    private void OnValueChanged(bool active) {
        switch (this.toggleType) {
            case EToggleType.HideOrShow:
                this.OnValueChangedByHideOrShow(active);
                break;
            case EToggleType.Loop: {
                    this.OnValueChangedByLoop(active);
                }
                break;
        }
    }
    private void OnValueChangedByHideOrShow(bool active) {
        this.m_isActive = active;
        foreach (var trans in this.activeTransforms) {
            if(trans)
                trans.gameObject.SetActive(active);
        }
        foreach (var trans in this.deActiveTransforms) {
            if (trans)
                trans.gameObject.SetActive(!active);
        }
    }


    //重复点击同一个toggle，在两种状态下循环切换
    private void OnValueChangedByLoop(bool active) {
        if (this.m_isActive && active)
            this.OnValueChangedByHideOrShow(false);
        else
            this.OnValueChangedByHideOrShow(active);
    }
}
