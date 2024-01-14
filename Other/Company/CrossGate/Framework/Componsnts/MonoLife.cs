using System;
using UnityEngine;

public class MonoLife : MonoBehaviour {
    public Action onAwake;
    public Action onDestroy;

    public Action onEnable;
    public Action onDisable;

    public Action onUpdate;

    private void Awake() {
        this.onAwake?.Invoke();
    }
    private void OnDestroy() {
        this.onDestroy?.Invoke();
    }
    private void OnEnable() {
        this.onEnable?.Invoke();
    }
    private void OnDisable() {
        this.onDisable?.Invoke();
    }
    private void Update() {
        this.onUpdate?.Invoke();
    }
}
