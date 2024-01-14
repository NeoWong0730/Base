using System;
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class CP_LongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 按压多长时间时间之后认为是长按
    public float triggerDuration = 0.1f;

    public Action onStartPress;
    public Action onReleasePress;
    public Action<float> onPressing;

    private bool isPressing= false;
    private bool isStartPressProcessed = false;
    private float startPressTime;
    
    private void Update()
    {
        if (isPressing)
        {
            float diff = Time.time - startPressTime;
            if (diff > triggerDuration)
            {
                if (!isStartPressProcessed)
                {
                    onStartPress();
                    isStartPressProcessed = true;
                }

                OnPressing(diff);
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        startPressTime = Time.time;
        isStartPressProcessed = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;

        OnReleasePress();
    }
    
    private void OnStartPress()
    {
        onStartPress?.Invoke();
    }
    private void OnReleasePress()
    {
        onReleasePress?.Invoke();
    }
    private void OnPressing(float pressTime)
    {
        onPressing?.Invoke(pressTime);
    }
}
