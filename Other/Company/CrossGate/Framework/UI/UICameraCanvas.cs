using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraCanvas : MonoBehaviour
{
    public Canvas canvas;

    private void Awake() {
        if (canvas == null) {
            canvas = GetComponent<Canvas>();
        }
    }

    private void Start() {
        if (canvas != null) {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = false;
            canvas.worldCamera = CameraManager.mUICamera;
        }
    }
}
