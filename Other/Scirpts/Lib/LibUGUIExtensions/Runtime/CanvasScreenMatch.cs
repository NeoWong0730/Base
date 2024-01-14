using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
[DisallowMultipleComponent]
public class CanvasScreenMatch : MonoBehaviour
{    
    void Start()
    {
        _Refresh();
    }

    private void _Refresh()
    {
#if UNITY_EDITOR
        referenceResolution = new Vector2Int(Screen.width, Screen.height);
#endif
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        float a = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
        float b = (float)Screen.width / (float)Screen.height;
//#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
//        Canvas canvas = GetComponent<Canvas>();
//        Camera camera = canvas.worldCamera;
//        if (AspectRotioController.IsExpandState && camera != null)
//        {
//            if(camera.gameObject.name!= "UICameraExpand")
//                b = (camera.rect.width * Screen.width) / Screen.height;
//            else
//                b = (float)Screen.width / (float)Screen.height;
//        }
//#endif

        //如果宽高比 比设计宽高比小 则根据宽度适配
        if (b < a)
        {
            canvasScaler.matchWidthOrHeight = 0;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 1;
        }
    }

//#if UNITY_EDITOR
    Vector2Int referenceResolution;
    private void Update()
    {
        if(referenceResolution.x != Screen.width || referenceResolution.y != Screen.height)
        {
            _Refresh();
        }
    }
//#endif
}
