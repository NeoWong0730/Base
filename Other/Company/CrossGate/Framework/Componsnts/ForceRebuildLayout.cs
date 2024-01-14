using Lib.Core;
using UnityEngine;

// 布局重刷
[DisallowMultipleComponent]
public class ForceRebuildLayout : MonoBehaviour {
    public float time = 0.05f;
    public bool enableUse = false;

    private Timer refreshListTimer;

    private void OnEnable() {
        if (enableUse) {
            Set();
        }
    }

    public void Set() {
        this.refreshListTimer?.Cancel();
        this.refreshListTimer = Timer.RegisterOrReuse(ref this.refreshListTimer, this.time, this.OnTimerCompleted);
    }

    private void OnTimerCompleted() {
        FrameworkTool.ForceRebuildLayout(this.gameObject);
    }

    private void OnDisable() {
        this.refreshListTimer?.Cancel();
    }
}
