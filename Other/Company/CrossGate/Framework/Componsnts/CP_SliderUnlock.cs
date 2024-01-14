using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 滑动解锁
// https://www.jianshu.com/p/3bca8792c4ca
public class CP_SliderUnlock : Slider {
    private bool tween = false;
    private float startTweenTime;
    private float pointUpValue;

    public bool IsSliding { get; private set; } = false;
    public Action onBeginSlide;
    public Action onEndSlide;

    public override void OnPointerDown(PointerEventData eventData) {
        base.OnPointerDown(eventData);

        onBeginSlide?.Invoke();
        IsSliding = true;
    }
    public override void OnPointerUp(PointerEventData eventData) {
        base.OnPointerUp(eventData);

        onEndSlide?.Invoke();
        IsSliding = false;

        if (m_Value < 1f) {
            BeginTween();
        }
    }

    private void BeginTween() {
        tween = true;
        startTweenTime = Time.time;
        pointUpValue = m_Value;
    }
    private void EndTween() {
        tween = false;
    }

    protected override void Update() {
        base.Update();

        if (tween) {
            float diff = Time.time - startTweenTime;
            float rate = diff / 0.3f;
            value = (1 - rate) * pointUpValue;
            
            if (rate >= 1f) {
                EndTween();
            }
        }
    }
}
