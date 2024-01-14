using System;
using UnityEngine;
using UnityEngine.UI;

public class CP_SliderFTMLerp : MonoBehaviour {
    public Slider slider;
    [SerializeField] public float from = 0;
    [SerializeField] public float to = 1;
    [SerializeField] public float max = 1;
    [SerializeField] public float duration = 0.8f;

    public Action onCompleted;
    public Action<float, float, float, float> onChanged;

    private float lastTime = 0f;
    private bool isReached = true;

    public CP_SliderFTMLerp Refresh(float from, float to, float max, float duration = 0.8f) {
        this.from = from;
        this.to = to;
        this.max = max;
        this.duration = duration;

        this.isReached = false;
        this.lastTime = Time.time;
        return this;
    }

    private void Update() {
        if (!this.isReached) {
            float diff = Time.time - this.lastTime;
            if (diff <= this.duration) {
                float current = Mathf.Lerp(this.from, this.to, diff / this.duration);
                float rate = current / this.max;
                this.slider.value = rate;
                this.onChanged?.Invoke(current, current - from, max, rate);
            }
            else {
                this.isReached = true;
                float rate = this.to / this.max;
                this.slider.value = rate;

                this.onChanged?.Invoke(this.to, to - from, max, rate);
                this.onCompleted?.Invoke();
            }
        }
    }
}
