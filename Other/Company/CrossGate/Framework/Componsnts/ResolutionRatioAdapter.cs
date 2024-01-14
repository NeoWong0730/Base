using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ResolutionRatioAdapter : MonoBehaviour
{
    [Header("参考分辨率， 同CanvasScaler设置")]
    public static Vector2 refResolutionRatio = new Vector2(1280f, 720f);

    public CanvasScaler canvasScaler;
    
    public System.Action<Vector2Int> onScreenResolutionChanged;
    private int lastWidth = Screen.width;
    private int lastHeight = Screen.height;

    private void OnEnable()
    {
        if (canvasScaler == null)
        {
            canvasScaler = gameObject.GetComponent<CanvasScaler>();
        }

        lastWidth = Screen.width;
        lastHeight = Screen.height;

        JudgeScreenResolution();
    }
    private void Update()
    {
        JudgeScreenResolution();
    }
    
    private void JudgeScreenResolution()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            OnScreenResolutionChanged();

            lastWidth = Screen.width;
            lastHeight = Screen.height;
        }
    }
    private void OnScreenResolutionChanged()
    {
        onScreenResolutionChanged?.Invoke(new Vector2Int(Screen.width, Screen.height));

        float oldRatio = refResolutionRatio.x / refResolutionRatio.y;
        float newRatio = 1f * Screen.width / Screen.height;
        if (newRatio > oldRatio)
        {
            canvasScaler.matchWidthOrHeight = 1f;
        }
        else if(newRatio < oldRatio)
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
    }
}
