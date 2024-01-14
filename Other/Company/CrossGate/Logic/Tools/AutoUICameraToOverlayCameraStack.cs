using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 这里因为ILR的OnEnable不知道为什么不执行，所以这里会存在Awake和OnDisable不匹配的问题
/// </summary>
[RequireComponent(typeof(Camera))]
public class AutoUICameraToOverlayCameraStack : MonoBehaviour {
    public new Camera camera;

    private void Awake() {
        if (this.camera == null) {
            this.camera = this.GetComponent<Camera>();
        }
    }
    private void OnEnable() {
        this.SetCameraStack();
    }

    public void SetCameraStack() {
        if (this.camera == null) {
            this.camera = this.GetComponent<Camera>();
        }
        if (this.camera != null) {
            UniversalAdditionalCameraData cameraData = this.camera.GetUniversalAdditionalCameraData();
            if (cameraData) {
                Camera uicamera = Logic.Core.UIManager.mUICamera;
                if (uicamera != null && !cameraData.cameraStack.Contains(uicamera)) {
                    cameraData.cameraStack.Add(uicamera);
                }
            }
        }
    }

    private void OnDisable() {
        if (this.camera != null) {
            UniversalAdditionalCameraData cameraData = this.camera.GetUniversalAdditionalCameraData();
            if (cameraData) {
                cameraData.cameraStack.Clear();
            }
        }
    }
}
