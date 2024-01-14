using Lib.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class AutoCameraStack : MonoBehaviour
{
    static private Dictionary<int, List<AutoCameraStack>> activeCamera = new Dictionary<int, List<AutoCameraStack>>(2);
    static public bool TryGetActiveCamera(int mutexID, out Camera camera) 
    {
        camera = null;
        if (activeCamera.TryGetValue(mutexID, out var ls) && ls.Count > 0) 
        {
            camera = ls[0].GetComponent<Camera>();
            return true;
        }
        return false;
    }

    [SerializeField]
    private int _priority = 0;
    [SerializeField]
    private int _mutexID = 0;
    [SerializeField]
    private List<string> _stackCameraTag = null;

    static private void SetStackCamera(UniversalAdditionalCameraData cameraData, Camera camera, bool active)
    {
        camera.enabled = active;
        List<Camera> cameras = cameraData.cameraStack;
        for (int i = 0; i < cameras.Count; ++i)
        {
            if (cameras[i])
            {
                if (cameras[i].TryGetComponent<AutoCameraOverlay>(out AutoCameraOverlay autoCameraOverlay))
                {
                    autoCameraOverlay.SetBaseCamera(active ? camera : null);
                }
            }
        }
    }

    static public void RefreshNewCamera(AutoCameraOverlay newCamera)
    {
        foreach(var cameras in activeCamera.Values)
        {
            for(int i = cameras.Count - 1; i >= 0; --i)
            {
                Camera camera = cameras[i].GetComponent<Camera>();
                List<string> stackCameraTag = cameras[i]._stackCameraTag;

                for (int tagIndex = 0; tagIndex < stackCameraTag.Count; ++tagIndex)
                {
                    if (newCamera.CompareTag(stackCameraTag[tagIndex]))
                    {
                        camera.GetUniversalAdditionalCameraData().cameraStack.AddOnce(newCamera.GetComponent<Camera>());
                        newCamera.SetBaseCamera(camera);
                    }
                }
            }
        }
    }

    private void Awake()
    {
        Camera camera = GetComponent<Camera>();
        UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
        if (cameraData.renderType != CameraRenderType.Base)
        {
            DebugUtil.LogWarningFormat("{0} cameraData.renderType is not CameraRenderType.Base", camera.gameObject.name);
            return;
        }

        if (_stackCameraTag == null || _stackCameraTag.Count == 0)
        {
            return;
        }

        Camera[] cameras = Camera.allCameras;
        int cameraCount = Camera.allCamerasCount;
        for (int tagIndex = 0; tagIndex < _stackCameraTag.Count; ++tagIndex)
        {
            for (int i = 0; i < cameraCount; ++i)
            {
                if (cameras[i].CompareTag(_stackCameraTag[tagIndex]))
                {
                    cameraData.cameraStack.AddOnce(cameras[i]);
                }
            }
        }
    }

    private void OnEnable()
    {
        Camera camera = GetComponent<Camera>();
        UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
        if (cameraData.renderType != CameraRenderType.Base)
        {
            DebugUtil.LogWarningFormat("{0} cameraData.renderType is not CameraRenderType.Base", camera.gameObject.name);
            return;
        }

        //当新的相机激活的时候， 会关闭掉互斥的相机
        if (!activeCamera.TryGetValue(_mutexID, out List<AutoCameraStack> otherCameras) || otherCameras == null)
        {
            otherCameras = new List<AutoCameraStack>();
            activeCamera[_mutexID] = otherCameras;
        }

        if (otherCameras.Count <= 0 || otherCameras[0] != this)
        {
            otherCameras.Remove(this);

            int i = 0;
            while (i < otherCameras.Count)
            {
                if (_priority >= otherCameras[i]._priority)
                {                    
                    break;
                }
                ++i;
            }

            otherCameras.Insert(i, this);

            if (i == 0)
            {                             
                if (otherCameras.Count > 1)
                {
                    Camera otherCamera = otherCameras[1].GetComponent<Camera>();
                    UniversalAdditionalCameraData otherCameraData = otherCamera.GetUniversalAdditionalCameraData();
                    SetStackCamera(otherCameraData, otherCamera, false);
                }
                SetStackCamera(cameraData, camera, true);
            }
            else
            {
                camera.enabled = false;
                //SetStackCamera(cameraData, camera, false);                
            }
        }
        if (CameraManager.mCamera != null && camera.rect != CameraManager.mCamera.rect)
            camera.rect = CameraManager.mCamera.rect;
    }

    private void OnDisable()
    {
        Camera camera = GetComponent<Camera>();
        UniversalAdditionalCameraData cameraData = camera.GetUniversalAdditionalCameraData();
        if (cameraData.renderType != CameraRenderType.Base)
        {
            DebugUtil.LogWarningFormat("{0} cameraData.renderType is not CameraRenderType.Base", camera.gameObject.name);
            return;
        }

        if (!activeCamera.TryGetValue(_mutexID, out List<AutoCameraStack> otherCameras) || otherCameras == null)
        {
            return;
        }

        if(otherCameras.Count > 0 && otherCameras[0] == this)
        {
            SetStackCamera(cameraData, camera, false);
            otherCameras.RemoveAt(0);

            if (otherCameras.Count > 0)
            {
                Camera otherCamera = otherCameras[0].GetComponent<Camera>();
                UniversalAdditionalCameraData otherCameraData = otherCamera.GetUniversalAdditionalCameraData();
                SetStackCamera(otherCameraData, otherCamera, true);
            }
        }
        else
        {
            otherCameras.Remove(this);
        }
    }

    public void Set(int priority, int mutexID, List<string> stackCameraTag)
    {
        _priority = priority;
        _mutexID = mutexID;
        _stackCameraTag = stackCameraTag;

        if (isActiveAndEnabled)
        {
            OnEnable();
        }
    }
}
