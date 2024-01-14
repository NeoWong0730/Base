using Lib.Core;
using UnityEngine;
using UnityEngine.UI;

// 防止按钮的连续点击,给一个cd时间
[RequireComponent(typeof(Scrollbar))]
[DisallowMultipleComponent]
public class ScrollbarMoveTo : MonoBehaviour {
    public Scrollbar slider = null;
    [Range(0.02f, 999f)]
    public float duration = 0.5f;

    private Timer timer;
    private float beginValue;
    private float endValue;

    private void Awake() {
        if (this.slider == null) {
            this.slider = this.GetComponent<Scrollbar>();
        }
    }

    private void OnDestroy() {
        this.timer?.Cancel();
    }

    public void MoveTo(float value) {
        this.beginValue = this.slider.value;
        this.endValue = value;

        this.timer?.Cancel();

        if (duration <= 0) {
            this.slider.value = this.endValue;
        }
        else {
            this.timer = Timer.RegisterOrReuse(ref this.timer, duration, this.OnTimed, this.OnTiming);
        }
    }

    private void OnTiming(float dt) {
        float rate = dt / this.timer.duration;
        float t = Mathf.Lerp(this.beginValue, this.endValue, rate);
        this.slider.value = t;
    }

    private void OnTimed() {
        this.slider.value = this.endValue;
    }
}
