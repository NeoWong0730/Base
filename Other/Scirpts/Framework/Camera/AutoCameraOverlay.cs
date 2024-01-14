using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    [RequireComponent(typeof(Camera))]
    public class AutoCameraOverlay : MonoBehaviour
    {
        private Camera _baseMain;

        private void Awake()
        {
            AutoCameraStack.RefreshNewCamera(this);
        }

        internal void SetBaseCamera(Camera baseCamera)
        {
            if (_baseMain == baseCamera)
                return;

            _baseMain = baseCamera;

            Camera camera = GetComponent<Camera>();
            UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = _baseMain == null ? CameraRenderType.Base : CameraRenderType.Overlay;
        }
    }
}