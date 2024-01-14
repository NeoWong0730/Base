using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tab : MonoBehaviour {
    public CP_TransformContainer container;

    private TabCollector _collector;
    public TabCollector collector {
        get {
            if (this._collector == null) {
                this._collector = this.GetComponentInParent<TabCollector>();
            }
            return this._collector;
        }
    }

    public uint id { get; set; }

    // 外部赋值
    public Action onSelected;
    public Action onRefreshed;

    public void SetSelected(bool toSelected) {
        container?.ShowHideBySetActive(toSelected);

        if (toSelected) {
            this.onSelected?.Invoke();
        }
    }
    public void Refresh() {
        this.onRefreshed?.Invoke();
    }
}