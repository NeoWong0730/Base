using System;
using UnityEngine;
using UnityEngine.UI;

public class CP_SliderLerp : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private float from = 0;
    [SerializeField] private float to = 1;
    [SerializeField] private float duration = 0.8f;
    private event Action onCompleted;

    private float lastTime = 0f;
    private bool isReached = true;

    public CP_SliderLerp SetCompleteAction(Action onCompleted)
    {
        this.onCompleted = onCompleted;
        return this;
    }
    public CP_SliderLerp Set(float to)
    {
        slider.value = to;
        return this;
    }
    public CP_SliderLerp Refresh(float to, float duration = 0.8f)
    {
        Refresh(slider.value, to, duration);
        return this;
    }
    public CP_SliderLerp Refresh(float from, float to, float duration = 0.8f)
    {
        this.from = from;
        this.to = to;
        this.duration = duration;

        this.isReached = false;
        this.lastTime = Time.time;
        return this;
    }

    private void Update()
    {
        if(!isReached)
        {
            float diff = Time.time - lastTime;
            if(diff <= duration)
            {
                slider.value = Mathf.Lerp(from, to, diff / duration);
            }
            else
            {
                isReached = true;
                slider.value = to;
                onCompleted?.Invoke();
            }
        }
    }
}
