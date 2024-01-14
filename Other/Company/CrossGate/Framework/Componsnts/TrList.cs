using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class ActiverGroup {
    // 需要显示的objs
    public List<Transform> actives = new List<Transform>(0);

    // 需要隐藏的objs
    public List<Transform> deactives = new List<Transform>(0);

    public void SetActive(bool toActive) {
        foreach (Transform t in this.actives) {
            if (t != null)
                t.gameObject.SetActive(toActive);
        }

        foreach (Transform t in this.deactives) {
            if (t != null)
                t.gameObject.SetActive(!toActive);
        }
    }
}

[DisallowMultipleComponent]
public class TrList : MonoBehaviour {
    public string ActiverTag;
    public ActiverGroup activer;

    [ReadOnly] [SerializeField] private TrListRegistry _collector;

    public TrListRegistry Collector {
        get {
            if (this._collector == null) {
                this._collector = this.GetComponentInParent<TrListRegistry>();
            }

            return this._collector;
        }
    }

    protected void OnEnable() {
        if (this.Collector != null) {
            this.Collector.Register(this);
        }
    }

    protected void OnDisbale() {
        if (this.Collector != null) {
            this.Collector.Unregister(this);
        }
    }

    public void ShowHideBySetActive(bool toActive) {
        this.activer.SetActive(toActive);
    }
}
