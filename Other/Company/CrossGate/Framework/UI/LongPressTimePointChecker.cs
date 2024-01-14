using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CP_LongPress))]
[DisallowMultipleComponent]
public class LongPressTimePointChecker : MonoBehaviour
{
    public float triggerDuration = 3f;
    public CP_LongPress longPress;

    public Action onTrigger;

    private float startPressTime;
    private bool gotResult = false;

    private void Awake() {
        if (longPress == null) {
            longPress = GetComponent<CP_LongPress>();
        }

        RegistEvents(true);
    }

    private void RegistEvents(bool toRegist) {
        if (toRegist) {
            longPress.onStartPress += OnStartPress;
            longPress.onReleasePress += OnReleasePress;
            longPress.onPressing += OnPressing;
        }
        else {
            longPress.onStartPress -= OnStartPress;
            longPress.onReleasePress -= OnReleasePress;
            longPress.onPressing -= OnPressing;
        }
    }

    private void OnStartPress() {
        gotResult = false;
        startPressTime = Time.time;
    }
    private void OnReleasePress() {
    }
    private void OnPressing(float time) {
        if (!gotResult && Time.time - startPressTime > triggerDuration) {
            gotResult = true;
            onTrigger?.Invoke();
            Debug.LogError("长安到时");
            startPressTime = Time.time;
        }
    }
}
