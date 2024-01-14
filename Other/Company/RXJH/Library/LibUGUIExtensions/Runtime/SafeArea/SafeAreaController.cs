using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SafeAreaMethodType
{
    CanvasBased,
    CameraBased,
};

[Flags]
public enum AreaUpdateTiming
{
    OnReciveMessage = (1 << 0),
    Awake = (1 << 1),
    OnEnable = (1 << 2),
    Start = (1 << 3),
    Update = (1 << 4),
};

[RequireComponent(typeof(Canvas))]
public class SafeAreaController : MonoBehaviour
{
    public SafeAreaMethodType ControlType = SafeAreaMethodType.CanvasBased;

    public bool addSoftkeyArea = false;

    public AreaUpdateTiming UpdateTimming = AreaUpdateTiming.OnEnable;

    private Canvas _mainCanvas;
    // 表示左右上下左右相对于safeArea的偏移，+表示内缩， -表示外扩
    private Rect Offset { get { return new Rect(0, 0, 0, navigationBarHeight); } }
    private static int navigationBarHeight = 0;

    public System.Action<Rect> onSafeAreaChanged;
    public System.Action onOrientationChanged;

    // Update Canvas Function
    private void UpdateSubCanvasProperty()
    {
        var targetCanvasArray = GetComponentsInChildren<CanvasPropertyOverrider>();
        foreach (var targetCanvas in targetCanvasArray)
        {
            targetCanvas.UpdateCanvasProperty(_mainCanvas.sortingOrder, Offset);
        }
    }

    // Update Camera Function
    private void UpdateCameraProperty()
    {
        var targetCameraArray = FindObjectsOfType<CameraPropertyOverrider>();

        foreach (var targetCamera in targetCameraArray)
        {
            targetCamera.UpdateCameraProperty(Offset);
        }
    }

    // Update Function
    public void UpdateSafeArea()
    {
        switch (this.ControlType)
        {
            case SafeAreaMethodType.CanvasBased:
                UpdateSubCanvasProperty();
                lastSafeArea = Screen.safeArea;
                break;
            case SafeAreaMethodType.CameraBased:
                UpdateCameraProperty();
                break;
            default:
                break;
        }
    }

    // Life cycle function
    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (addSoftkeyArea)
            RunOnAndroidUiThread(GetNavigationBarHeight);
#endif
        _mainCanvas = GetComponent<Canvas>();

        if (HaveMask(AreaUpdateTiming.Awake))
            UpdateSafeArea();
    }

    private void OnEnable()
    {
        if (HaveMask(AreaUpdateTiming.OnEnable))
            UpdateSafeArea();
    }

    private void Start()
    {
        if (HaveMask(AreaUpdateTiming.Start))
            UpdateSafeArea();
    }

    private Rect lastSafeArea;
    private ScreenOrientation lastOrientation;
    private void Update()
    {
        if (HaveMask(AreaUpdateTiming.Update))
            UpdateSafeArea();

        if(lastSafeArea != Screen.safeArea)
        {
            lastSafeArea = Screen.safeArea;
            onSafeAreaChanged?.Invoke(lastSafeArea);
            UpdateSafeArea();
        }
        if (lastOrientation != Screen.orientation) {
            lastOrientation = Screen.orientation;
            onOrientationChanged?.Invoke();
            UpdateSafeArea();
        } 
    }

    // Utility
    private bool HaveMask(AreaUpdateTiming mask)
    {
        return ((int)UpdateTimming & (int)mask) != 0;
    }

// =================================================================
// 		Functions 4 Android
// =================================================================

    private static void RunOnAndroidUiThread(Action target)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {

            activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
        }}
#elif UNITY_EDITOR
        target();
#endif
    }

    private static void GetNavigationBarHeight()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
        using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
        using (var window = activity.Call<AndroidJavaObject>("getWindow")) {
        using (var resources = activity.Call<AndroidJavaObject>("getResources")) {
            var resourceId = resources.Call<int>("getIdentifier", "navigation_bar_height", "dimen", "android");
            if (resourceId > 0) {
                navigationBarHeight = resources.Call<int>("getDimensionPixelSize", resourceId);
            }
        }}}}
#elif UNITY_EDITOR
        navigationBarHeight = 0;
#endif
    }
}
